// 
// RenameItemDialog.cs
//  
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2009 Novell, Inc (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;

using Gtk;

using MonoDevelop.Core;
using System.Collections.Generic;
using MonoDevelop.Ide;
using MonoDevelop.Ide.ProgressMonitoring;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.TypeSystem;

namespace MonoDevelop.Refactoring.Rename
{
	public partial class RenameItemDialog : Gtk.Dialog
	{
		RenameRefactoring rename;
		RefactoringOptions options;
		
		public RenameItemDialog (RefactoringOptions options, RenameRefactoring rename)
		{
			this.options = options;
			this.rename = rename;
			if (options.SelectedItem is IMethod && ((IMethod)options.SelectedItem).IsConstructor) {
				options.SelectedItem = ((IMethod)options.SelectedItem).DeclaringType;
			}
			this.Build ();

			if (options.SelectedItem is ITypeDefinition) {
				var type = (ITypeDefinition)options.SelectedItem;
				if (type.DeclaringType == null) {
					// not supported for inner types
					this.renameFileFlag.Visible = true;
					this.renameFileFlag.Active = true;
					// if more than one type is in the file, only rename the file as defilt if the file name contains the type name
					// see Bug 603938 - Renaming a Class in a file with multiple classes renames the file
					if (options.Document != null && options.Document.ParsedDocument.TopLevelTypeDefinitions.Count > 1) 
						this.renameFileFlag.Active = options.Document.FileName.FileNameWithoutExtension.Contains (type.Name);
				} else {
					this.renameFileFlag.Active = false;
				}
				if (type.Kind == TypeKind.Interface)
					this.Title = GettextCatalog.GetString ("Rename Interface");
				else
					this.Title = GettextCatalog.GetString ("Rename Class");
//				this.fileName = type.GetDefinition ().Region.FileName;
			} else if (options.SelectedItem is IField) {
				this.Title = GettextCatalog.GetString ("Rename Field");
			} else if (options.SelectedItem is IProperty) {
				if (((IProperty)options.SelectedItem).IsIndexer) {
					this.Title = GettextCatalog.GetString ("Rename Indexer");
				} else {
					this.Title = GettextCatalog.GetString ("Rename Property");
				}
			} else if (options.SelectedItem is IEvent) {
				this.Title = GettextCatalog.GetString ("Rename Event");
			} else if (options.SelectedItem is IMethod) { 
				var m = (IMethod)options.SelectedItem;
				if (m.IsConstructor || m.IsDestructor) {
					this.Title = GettextCatalog.GetString ("Rename Class");
				} else {
					this.Title = GettextCatalog.GetString ("Rename Method");
				}
			} else if (options.SelectedItem is IParameter) {
				this.Title = GettextCatalog.GetString ("Rename Parameter");
			} else if (options.SelectedItem is IVariable) {
				this.Title = GettextCatalog.GetString ("Rename Variable");
			} else {
				this.Title = GettextCatalog.GetString ("Rename Item");
			}
			
			if (options.SelectedItem is IEntity) {
				var member = (IEntity)options.SelectedItem;
				if (member.EntityType == EntityType.Constructor || member.EntityType == EntityType.Destructor) {
					entry.Text = member.DeclaringType.Name;
				} else {
					entry.Text = member.Name;
				}
//				fileName = member.Region.FileName;
			} else if (options.SelectedItem is IVariable) {
				var lvar = (IVariable)options.SelectedItem;
				entry.Text = lvar.Name;
//				this.fileName = lvar.Region.FileName;
			}
			entry.SelectRegion (0, -1);
			
			buttonPreview.Sensitive = buttonOk.Sensitive = false;
			entry.Changed += OnEntryChanged;
			entry.Activated += OnEntryActivated;
			
			buttonOk.Clicked += OnOKClicked;
			buttonPreview.Clicked += OnPreviewClicked;
			entry.Changed += delegate { buttonPreview.Sensitive = buttonOk.Sensitive = ValidateName (); };
			ValidateName ();
		}

		bool ValidateName ()
		{
			return true; // TODO: Name validation.
//			var nameValidator = MonoDevelop.Projects.LanguageBindingService.GetRefactorerForFile (fileName ?? "default.cs");
//			if (nameValidator == null)
//				return true;
//			ValidationResult result = nameValidator.ValidateName (this.options.SelectedItem, entry.Text);
//			if (!result.IsValid) {
//				imageWarning.IconName = Gtk.Stock.DialogError;
//			} else if (result.HasWarning) {
//				imageWarning.IconName = Gtk.Stock.DialogWarning;
//			} else {
//				imageWarning.IconName = Gtk.Stock.Apply;
//			}
//			labelWarning.Text = result.Message;
//			return result.IsValid;
		}

		void OnEntryChanged (object sender, EventArgs e)
		{
			// Don't allow the user to click OK unless there is a new name
			buttonPreview.Sensitive = buttonOk.Sensitive = entry.Text.Length > 0;
		}

		void OnEntryActivated (object sender, EventArgs e)
		{
			if (buttonOk.Sensitive)
				buttonOk.Click ();
		}
		
		RenameRefactoring.RenameProperties Properties {
			get {
				return new RenameRefactoring.RenameProperties () {
					NewName = entry.Text,
					RenameFile = renameFileFlag.Visible && renameFileFlag.Active
				};
			}
		}
		
		void OnOKClicked (object sender, EventArgs e)
		{
			var properties = Properties;
			((Widget)this).Destroy ();
			List<Change> changes = rename.PerformChanges (options, properties);
			IProgressMonitor monitor = IdeApp.Workbench.ProgressMonitors.GetBackgroundProgressMonitor (this.Title, null);
			RefactoringService.AcceptChanges (monitor, changes);
		}
		
		void OnPreviewClicked (object sender, EventArgs e)
		{
			var properties = Properties;
			((Widget)this).Destroy ();
			List<Change> changes = rename.PerformChanges (options, properties);
			MessageService.ShowCustomDialog (new RefactoringPreviewDialog (changes));
		}
	}
		
}
