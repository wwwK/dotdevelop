
this version is based on:
	*) commit bd0ab28ba941b19b39322247db020dcd0fb305d0 
	*) tagged monodevelop-8.1.5.9 
	*) dated 2019-07-03 

some package versions:
	Microsoft.CodeAnalysis.*	(left as it was)
	Microsoft.VisualStudio.*	removed (from vs-editor-api)

modifications:

	 added/reverted:
	^^^^^^^^^^^^^^^^^
main/external/vs-editor-api		<new submodule>
main/msbuild/ReferencesVSEditor.Gtk.props
main/msbuild/RoslynVersion.props
main/src/addins/MonoDevelop.SourceEditor2/VSEditor/FakeWpf/Geometry.cs
main/src/addins/MonoDevelop.SourceEditor2/VSEditor/TextEditorFactoryService.cs
main/src/core/MonoDevelop.Ide/MonoDevelop.Ide.Editor/ITextEditorFactoryService.cs

	 modified:
	^^^^^^^^^^^
Makefile
NuGet.config
main/Directory.Build.props
main/Main.sln
main/Makefile.am
main/msbuild/MDBuildTasks/MDBuildTasks.csproj
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/ExceptionCaught/ExceptionCaughtAdornmentManager.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/ExceptionCaught/ExceptionCaughtProvider.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Glyphs/BreakpointGlyphFactoryProvider.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Glyphs/BreakpointGlyphMouseProcessor.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Glyphs/BreakpointGlyphMouseProcessorProvider.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Glyphs/BreakpointGlyphTag.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Glyphs/BreakpointGlyphTagger.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Glyphs/BreakpointGlyphTaggerProvider.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Glyphs/CurrentStatementGlyphFactoryProvider.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Glyphs/CurrentStatementGlyphTag.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Glyphs/CurrentStatementGlyphTagger.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Glyphs/CurrentStatementGlyphTaggerProvider.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Glyphs/GlyphCommandType.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Glyphs/IActiveGlyphDropHandler.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Glyphs/IInteractiveGlyph.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Glyphs/ImageSourceGlyphFactory.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Glyphs/ReturnStatementGlyphFactoryProvider.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Glyphs/ReturnStatementGlyphTag.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Glyphs/ReturnStatementGlyphTagger.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Glyphs/ReturnStatementGlyphTaggerProvider.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Tags/AbstractBreakpointTagger.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Tags/AbstractCurrentStatementTagger.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Tags/BreakpointForegroundTagger.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Tags/BreakpointForegroundTaggerProvider.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Tags/BreakpointMarkerDefinition.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Tags/BreakpointTag.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Tags/BreakpointTagger.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Tags/BreakpointTaggerProvider.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Tags/ClassificationTypes.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Tags/CurrentStatementForegroundTagger.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Tags/CurrentStatementForegroundTaggerProvider.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Tags/CurrentStatementMarkerDefinition.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Tags/CurrentStatementTag.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Tags/CurrentStatementTagger.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Tags/CurrentStatementTaggerProvider.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Tags/ReturnStatementForegroundTaggerProvider.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Tags/ReturnStatementMarkerDefinition.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Tags/ReturnStatementTag.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Tags/ReturnStatementTagger.cs
main/src/addins/MonoDevelop.Debugger/MonoDevelop.Debugger.VSTextView/Tags/ReturnStatementTaggerProvider.cs
main/src/addins/MonoDeveloperExtensions/AddinInfo.cs
main/src/addins/MonoDevelop.SourceEditor2/MonoDevelop.SourceEditor.Braces/BraceCompletionManager.cs
main/src/addins/MonoDevelop.SourceEditor2/MonoDevelop.SourceEditor.csproj
main/src/addins/MonoDevelop.SourceEditor2/Mono.TextEditor/Gui/MdTextViewLineCollection.MdTextViewLine.cs
main/src/addins/MonoDevelop.SourceEditor2/Mono.TextEditor/Gui/MonoTextEditor.ITextView.cs
main/src/addins/MonoDevelop.SourceEditor2/VSEditor/IMdTextView.cs
main/src/addins/MonoDevelop.SourceEditor2/VSEditor/Language/Impl/Intellisense/CurrentLineSpaceReservationAgent.cs
main/src/addins/MonoDevelop.SourceEditor2/VSEditor/SmartIndentationService.cs
main/src/addins/MonoDevelop.SourceEditor2/VSEditor/TagBasedSyntaxHighlighting.cs
main/src/addins/MonoDevelop.SourceEditor2/VSEditor/Text/Impl/WpfView/PopupAgent.cs
main/src/addins/MonoDevelop.TextEditor/MonoDevelop.TextEditor/EditorCommandHandlers.cs
main/src/addins/MonoDevelop.TextEditor/MonoDevelop.TextEditor/TextViewContent.Toolbox.cs
main/src/addins/MonoDevelop.TextEditor/MonoDevelop.TextEditor.Wpf/Undo/UndoHistoryRegistry.cs
main/src/addins/MonoDevelop.UnitTesting/MonoDevelop.UnitTesting.csproj
main/src/addins/MonoDevelop.UnitTesting/MonoDevelop.UnitTesting.VsTest/VsTestAdapter.cs
main/src/addins/MonoDevelop.UnitTesting/MonoDevelop.UnitTesting.VsTest/VsTestDiscoveryAdapter.cs
main/src/addins/MonoDevelop.UnitTesting/MonoDevelop.UnitTesting.VsTest/VsTestRunAdapter.cs
main/src/core/MonoDevelop.Ide/MonoDevelop.Ide.Composition/CompositionManager.cs
main/src/core/MonoDevelop.Ide/MonoDevelop.Ide.Composition/PlatformCatalog.cs
main/src/core/MonoDevelop.Ide/MonoDevelop.Ide.csproj
main/src/core/MonoDevelop.Ide/MonoDevelop.Ide.Editor/TextEditor.cs
main/tests/UserInterfaceTests/VersionControlTests/Git/GitBase.cs
main/tests/UserInterfaceTests/VersionControlTests/Git/GitRepositoryConfigurationTests.cs
main/tests/UserInterfaceTests/VersionControlTests/Git/GitStashManagerTests.cs
main/tests/UserInterfaceTests/VersionControlTests/Git/GitTests.cs
main/tests/UserInterfaceTests/VersionControlTests/SvnTests.cs
main/tests/UserInterfaceTests/VersionControlTests/VCSBase.cs

	 moved/renamed:
	^^^^^^^^^^^^^^^^

	 deleted:
	^^^^^^^^^^
main/external/libgit-binary		<submodule>
main/external/libgit2			<submodule>
main/external/libgit2sharp		<submodule>
main/src/addins/VersionControl/MonoDevelop.VersionControl.Git	<directory>

##############################################################################################

	 how to build:
	^^^^^^^^^^^^^^^

$ git submodule init 
$ git submodule update 

$ ./configure 

	The build profile 'default' does not exist. A new profile will be created.
	Select the packages to include in the build for the profile 'default':

	1. [X] main
	2. [ ] extras/MonoDevelop.Database

	Enter the number of an add-in to enable/disable,
	(q) quit, (c) clear all, (s) select all, or ENTER to continue:  <ENTER>

$ make clean 
$ make 

	IF NEEDED, make copies of the following files with fixed filenames:
$ cp ./main/external/fsharpbinding/packages/FSharp.Core/fsharp.core.4.1.0.2.nupkg ./main/external/fsharpbinding/packages/FSharp.Core/FSharp.Core.4.1.0.2.nupkg
$ cp ./main/external/fsharpbinding/packages/System.ValueTuple/system.valuetuple.4.4.0.nupkg ./main/external/fsharpbinding/packages/System.ValueTuple/System.ValueTuple.4.4.0.nupkg
	THEN re-try building ( make clean && make ).

	 how to run:
	^^^^^^^^^^^^^

$ make run 

	or

$ mono ./main/build/bin/MonoDevelop.exe --no-redirect 

	or (with no logging output to console)

$ mono ./main/build/bin/MonoDevelop.exe 

	 known problems:
	^^^^^^^^^^^^^^^^^
*) GIT support has been removed.
*) the QuickFix menu is not working in editor.
*) win10 wsl : the process seems to hang and keep running after closing the app.

