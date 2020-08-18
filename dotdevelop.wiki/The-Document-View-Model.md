MonoDevelop 8.1 implements a new architecture for displaying views in the shell. The main changes of this architecture compared to the old one are:

* Introduction of a Model/View/Controller pattern. By having the data in a Model it will be possible to easily support displaying and editing a file in more than one view, even in different documents.
* Extensibility at document and controller level, so that add-ins can plug into file operations and provide custom commands.
* Proper support of split and tabbed views. Extensions can provide visual designers and previews, and the shell decides the best way to present them.

## Concepts

The following diagram shows the most relevant classes and their relations:

[[/images/doc-view-model.png]]

### Document
A document represents a tab in the shell, which can show a file in a text editor, a designer or a tool such as the assembly browser.

### DocumentController
A document controller is a class that implements the logic for loading, displaying and interacting with the contents of a document. It is roughly equivalent to the old BaseViewContent and ViewContent classes.

### FileDocumentController
A type of controller specialized in showing the content of a file.

### DocumentControllerExtension
An extension object that can be attached to a DocumentController. It can participate in controller operations and provide custom UI.

### DocumentView, DocumentViewContent, DocumentViewContainer
DocumentView and its derived classes can be used to create complex composite views inside a document. For example, it allows showing two views in tabs, or with splits.

### DocumentModel
A model is a class that contains the data shown in a view. This concept has no equivalent on the old design, in which data and view were mixed in a single class. By having the data isolated in its own class it will be possible to support simultaneous editing of models in more than one view or even document.

### FileModel
A type of model that represents a file.

### ModelRegistry
Keeps track of the loaded models.

## Document models

A model is the data being shown in a view. The isolation of models from controllers and views has two main goals:

 * Make it easier to share status between views. In the case, for example, of an XAML file being shown
   in a text editor and on a preview view there can be a single XAML file model that is shared between
   the views.
 * Make it possible to share status between documents. At some point we may want to allow editing a
   file in several documents at once. For example, a document showing a source view and another
   document showing a visual design view.

Models have their own lifecycle, independent from the document lifecycle. There is API to load, modify and save models, with independence of the API for handling documents. Documents and controllers can make use of models, and other services of the IDE can use them as well.

The use of a model by a document is optional. For example, the Assembly Browser doesn’t need to provide a model, since it doesn’t need to share state.

Models are implemented by subclasses of the `DocumentModel` class

## Document controllers

Controllers are in charge of loading model data into a view (or into other controllers), saving the data and implementing commands. The role of a controller is very similar to that of the old ViewContent class. 

An important difference is the introduction of the DocumentModel class. Some functionality previously implemented by ViewContent can now be implemented in the model. However, the use of models is optional, so all logic could be in the controller.

DocumentController supports extensions using the DocumentControllerExtension class. Extensions can hook into controller operations and provide custom UI.

## Document views

Document controllers can provide views using the DocumentView class and derived classes. Unlike the old ViewContent class, which created a Control, the DocumentController class creates a DocumentView. A DocumentView is a component of the shell that can show the content provided by a document controller. The DocumentViewContent subclass can show a Control, and the DocumentViewContainer can show a set of views using tabs or splits.

## Shell documents

Document handling logic has been moved from Workbench to a new DocumentManager class.

The Document class is not a subclass of DocumentContext anymore. It has a DocumentContext property instead. All logic in charge of parsing documents has been moved to the new RoslynDocumentExtension class, a document controller extension that is created only when the document is parseable.
