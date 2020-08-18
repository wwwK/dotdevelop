This document lists several common leaky patterns in MonoDevelop code, together with how to avoid them.

There are 3 major toolkits being used throughout MonoDevelop code, each with their own quirks, meaning they have to be handled differently:

* Gtk
* Xamarin.Mac
* Xwt

# Common implementation details

Both Gtk and Xamarin.Mac are bindings on top of native libraries with reference counted semantics. To implement the support for them to work under a garbage collected runtime, a ToggleRef is used.

**What is a ToggleRef?**

A ToggleRef is a construct ties a native object to a managed object using the following logic:

* Add a strong reference to the native object with a callback that notifies when the reference count changes.
* Removes the native construction strong reference
* Hold a managed reference to the object

In Gtk#, for example, the implementation looks like:
```
class ToggleRef
{
    IntPtr handle;
    object reference;
    GCHandle gch;
}
```

The handle is the native object's pointer.

The GCHandle is a special runtime class which wraps an object and tells the garbage collector to not collect that object _until_ the GCHandle is freed.

When the refcount change callback is called (that means, even when the initial ref is removed after adding the ToggleRef), what happens is:

* if refcount == 1 -> `reference` is a `WeakReference` to the managed object - meaning that it is eligible for garbage collection if only the ToggleRef (and other weak references) are holding the object alive
* if refcount > 1 -> `reference` is a strong reference to managed object (aka, the object itself), so managed will keep it alive, native too.

When a native wrapped object is disposed or finalized, logic is done to free the GCHandle of the ToggleRef and remove the last ref, so native will finally cleanup after.

Usual leaks mean that the refcount > 1, usually because a child widget holds a managed reference to its parent, or managed objects have reference between eachother.

Usually, disposing of a native object will free the toggleref, removing 1 refcount. Therefore, it's up to native to clean up. If the refcount never gets to 0, the native object will always leak.

Below, we will tackle each toolkit's ref cycle common causes and how to fix them.

# Gtk#

Compared to other toolkits, Gtk# has a `Destroy` method which is called in the toplevel and iterates all the children calling the same method.

`Destroy` informs implementations that they should releases all the refs they have added.

Thus, we have 2 ways to approach these issues with Gtk:

* Using WeakReferences
* Using OnDestroyed and fixing up refs.

## Problem: Keeping references on child -> parent

``` csharp
// MyWidget has a ref to MyButton in native.
class MyWidget : Widget
{
    public MyWidget()
    {
        Add (new Button(this));
    }

    public void NotifyClicked() {}
}

class MyButton : Button
{
    // MyButton has a ref to MyWidget in managed
    MyWidget widget;
    public MyButton (MyWidget widget)
    {
        this.widget = widget;
    }

    protected override void OnClicked() => widget.DoClicked();
}
```

### Solution 1 - MyButton holds a weak reference to MyWidget

``` csharp
class MyButton : Button
{
    // MyButton has a ref to MyWidget in managed
    WeakReference<MyWidget> widgetRef;

    public MyButton (MyWidget widget)
    {
        this.widgetRef = new WeakReference(widget);
    }

    protected override void OnClicked()
    {
        if (widgetRef.TryGetTarget (out var widget))
            widget.DoClicked();
    }
}
```

### Solution 2 - MyButton holds a strong reference, but destroy cleans it up
``` csharp
class MyButton : Button
{
    // MyButton has a ref to MyWidget in managed
    MyWidget widget;

    public MyButton (MyWidget widget)
    {
        this.widget = widget;
    }

    protected override void OnClicked() => widget.NotifyClicked();

    protected override OnDestroyed()
    {
        // Clean the strong reference here.
        widget = null;
    }
}
```

## Problem: Event subscription causes ref cycle

It is idiomatic in C# to use event handlers, but these have a pitfall caused by how eventhandlers actually work, because they are delegates. Usually, they have a `Target` and a `Method`. The Method is runtime information on what is being invoked, which does not matter here. The `Target` on the other hand, is the object which holds the `Method` in hand.

If possible, static handlers should be used, as they are much cheaper than instance handlers (both in code maintainability and in performance), and those definitely do not leak.

``` csharp
class MyWidget : Gtk.Widget
{
    public MyWidget()
    {
         var button = new Button();

         // Lambda below captures this, so we have a refcycle:
         // Widget -> button in native
         // button -> Widget in managed
         button.Clicked += (sender, args) => DoClicked();
         Content = button;
    }
    void DoClicked() => {}
}
```

### Solution 1: Use static event handlers/non-capturing lambdas

Static event handlers are not problematic, as they don't have a `Target` object.

``` csharp
class MyWidget : Widget
{
    public MyWidget()
    {
         var button = new Button();
         button.Clicked += OnClicked;
         Content = button;
    }

    // A similar implementation could be done with a lambda.
    static void OnClicked(object sender, EventArgs args)
    {
         var button = (Button)sender;
         var widget = (MyWidget)button.Parent;
         widget.DoClicked();
     }

    void DoClicked() {}
}
```

### Solution 2: Use an instance event handler and unsubscribe on Destroy

``` csharp
class MyWidget : Widget
{
    // We now hold a ref from MyWidget to button in managed too.
    Button button;

    public MyWidget()
    {
         button = new Button();
         // Lambda no longer captures any state
         button.Clicked += OnClicked;
         Content = button;
    }

    void OnClicked(object sender, EventArgs args) => DoClicked();
    void DoClicked() {}

    protected override void OnDestroyed()
    {
         button.Clicked -= OnClicked;
         // nulling out is not absolutely needed, since it's not a cycle, but usually good to do.
         button = null;
    }
}
```

# Xamarin.Mac

Unlike Gtk#, Xamarin.Mac does not have a Destroy method. Therefore, we require to make refcounting bookkeeping in all the code.

But, Xamarin.Mac has the niceties of exposing the retaining semantics of every bound method/property.

There are multiple values for [ArgumentSemantic](https://developer.xamarin.com/api/type/MonoTouch.ObjCRuntime.ArgumentSemantic/), which means that every method has the chance of possibly changing the object's refcounting.

This can be useful if you want an object to be released as soon as possible, without waiting on the finalizer queue.

Given that Dispose is not called on the whole view hierarchy, it tends to get messy if the Dispose method is preferred, since manually disposing is needed everywhere.

Therefore, for Xamarin.Mac, prefer weak semantics.

## Problem: Keeping references on child -> parent

``` csharp
// MyView has a ref to MyButton in native.
class MyView : NSView
{
    public MyView()
    {
        AddSubView (new MyButton(this));
    }

    public void NotifyClicked() {}
}

class MyButton : NSButton
{
    // MyButton has a ref to MyView in managed
    MyView widget;

    public MyButton (MyView widget)
    {
        this.widget = widget;
        Action = new Selector("onClicked:");
        Target = this;
    }

    [Export("onClicked:")]
    public void OnClicked(NSObject target)
    {
        widget.NotifyClicked();
    }
}
```

## Solution 1: keep a weakreference
``` csharp
// MyView has a ref to MyButton in native.
class MyView : NSView
{
    public MyView()
    {
        AddSubView (new MyButton(this));
    }

    public void NotifyClicked() {}
}

class MyButton : NSButton
{
    // MyButton has a ref to MyView in managed
    WeakReference<MyView> widgetRef;

    public MyButton (MyView widget)
    {
        widgetRef = new WeakReference (widget);
        Action = new Selector("onClicked:");
        Target = this;
    }

    [Export("onClicked:")]
    public void OnClicked(NSObject target)
    {
        if (widgetRef.TryGetTarget (out var widget))
            widget.NotifyClicked();
    }
}
```

## Solution 2: Make use of the Target of an action

``` csharp
// MyView has a ref to MyButton in native.
class MyView : NSView
{
    public MyView()
    {
        AddSubView (new MyButton(this));
    }

    [Export("onClicked:")]
    public void OnClicked(NSObject target) => NotifyClicked();
    public void NotifyClicked() {}
}

class MyButton : NSButton
{
    public MyButton (MyView widget)
    {
        Action = new Selector("onClicked:");
        Target = widget; // this has ArgumentSemantic.Weak, so will not increase the refcount
    }
}
```

## Solution 3: We don't necessarily need a subclass

```csharp
// MyView has a ref to MyButton in native.
class MyView : NSView
{
    public MyView()
    {
        var button = new NSButton {
            Target = this,
            Action = new Selector("onClicked:"),
        }
        AddSubView (button);
    }

    [Export("onClicked:")]
    public void OnClicked(NSObject target) => NotifyClicked();
    public void NotifyClicked() {}
}
```


## Problem: Event handlers

Always prefer selectors and weakreferences as opposed to event handlers. Avoid event handlers, if possible. They are usually implemented on top of the native APIs, so you can inspect how they're implemented, so you can make your own version.

Event handlers can cause ref cycles and manually bookkeeping disposing of resources can get tedious. 

## Problem: strong delegates

Always prefer weak delegates, if available. These remove the need to manually bookkeep where to remove references from delegates.

# Xwt

TODO

# Static events

Similar to how GCHandles are GC roots (as in tree root, a node that has its children - references - retained in memory), static contexts are also GC roots.

Therefore if an object is referenced in another static object or an event handler is subscribed to a static event, we have the same problem. Manual bookkeeping is required there too.