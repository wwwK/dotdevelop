Starting from the 8.0 release Visual Studio for Mac introduces a new text editor engine and UI, the same that is used by Visual Studio. The benefits of sharing the editor between Visual Studio and Visual Studio for Mac is that code targeting the Visual Studio editor can run on Visual Studio for Mac with almost no changes required.

See details about the New Editor in the announcement here:
https://aka.ms/vs/mac/editor/learn-more

This guide is aimed at Visual Studio for Mac extenders and addin authors, and will collect information and tips on how to take advantage of the New Editor and its API.

General VS for Mac extensibility guidance is available here:
https://docs.microsoft.com/en-us/visualstudio/mac/extending-visual-studio-mac

# Caveats

In 8.0, the new editor is disabled by default, and only supports C# files. It will become the default editor in future releases, and gradually completely replace any need for the legacy editor.

The legacy editor implements some of the Visual Studio Editor APIs described below.

# Editor Overview

https://docs.microsoft.com/en-us/visualstudio/extensibility/inside-the-editor?view=vs-2017

Video introduction to the new editor: 
https://www.youtube.com/watch?v=PkYVztKjO9A

The primary concepts that you need to be familiar with are an `ITextBuffer` and an `ITextView`. An `ITextBuffer` is an in-memory representation of text that can be changed over time. At any given point in time the `CurrentSnapshot` property on `ITextBuffer` returns an immutable representation of the current contents of the buffer, an instance of `ITextSnapshot`. When an edit is made on the buffer, the CurrentSnapshot property is updated to the latest version. Analyzers can inspect the text snapshot on any thread and its contents is guaranteed to never change.

An `ITextView` is the UI representation of how `ITextBuffer` is rendered on screen in the editor control. It has a reference to its text buffer, as well as `Caret`, `Selection` and other UI related concepts.

For a [Document](http://source.monodevelop.com/#MonoDevelop.Ide/MonoDevelop.Ide.Gui/Document.cs,4e960d4735f089b5) you can retrieve the associated underlying `ITextBuffer` and `ITextView` via `.GetContent<ITextBuffer> ()` and `.GetContent<ITextView> ()` respectively.

### Document.AnalysisDocument
Retrieving `Document.AnalysisDocument` can be done via this code snippet (that we're working on implementing so `AnalysisDocument` will work again):

```
var textBuffer = document.GetContent<ITextBuffer> ();
if (textBuffer != null && textBuffer.AsTextContainer () is SourceTextContainer container) {
	var document = container.GetOpenDocumentInCurrentContext ();
	if (document != null) {
		return document;
	}
}
```

See also:
 * http://source.roslyn.io/#Microsoft.CodeAnalysis.Workspaces/Workspace/TextExtensions.cs,7d8ce0f29a31ba83

