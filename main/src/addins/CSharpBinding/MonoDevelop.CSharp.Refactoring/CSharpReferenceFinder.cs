// 
// CSharpReferenceFinder.cs
//  
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2011 Xamarin Inc. (http://xamarin.com)
// Copyright (c) 2011 Novell, Inc (http://www.novell.com)
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
using System.Collections.Generic;

using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.CSharp.Resolver;
using MonoDevelop.Ide.FindInFiles;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using System.IO;
using MonoDevelop.TypeSystem;
using ICSharpCode.NRefactory.Semantics;
using Mono.TextEditor;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using System.Threading;

namespace MonoDevelop.CSharp.Refactoring
{
	using MonoDevelop.Projects;
	public class CSharpReferenceFinder : ReferenceFinder
	{
		ICSharpCode.NRefactory.CSharp.Resolver.FindReferences refFinder = new ICSharpCode.NRefactory.CSharp.Resolver.FindReferences ();
		List<object> searchedMembers;
		List<FilePath> files = new List<FilePath> ();
		List<Tuple<FilePath, MonoDevelop.Ide.Gui.Document>> openDocuments = new List<Tuple<FilePath, MonoDevelop.Ide.Gui.Document>> ();
		
		string memberName;
		
		public CSharpReferenceFinder ()
		{
			IncludeDocumentation = true;
		}
		
		public void SetSearchedMembers (IEnumerable<object> searchedMembers)
		{
			this.searchedMembers = new List<object> (searchedMembers);
			var firstMember = searchedMembers.FirstOrDefault ();
			if (firstMember is INamedElement)
				memberName = ((INamedElement)firstMember).Name;
			if (firstMember is string)
				memberName = firstMember.ToString ();
			if (firstMember is IVariable)
				memberName = ((IVariable)firstMember).Name;
		}
		
		void SetPossibleFiles (IEnumerable<FilePath> files)
		{
			foreach (var file in files) {
				var openDocument = IdeApp.Workbench.GetDocument (file);
				if (openDocument == null) {
					this.files.Add (file);
				} else {
					this.openDocuments.Add (Tuple.Create (file, openDocument));
				}
			}
		}
		
		MemberReference GetReference (ResolveResult result, AstNode node, string fileName, Mono.TextEditor.TextEditorData editor)
		{
			if (result == null) {
				return null;
			}
			object valid = null;
			if (result is MethodGroupResolveResult) {
				valid = ((MethodGroupResolveResult)result).Methods.FirstOrDefault (m => searchedMembers.Any (member => member is IMethod && ((IMethod)member).Region == m.Region));
			} else if (result is MemberResolveResult) {
				var foundMember = ((MemberResolveResult)result).Member;
				valid = searchedMembers.FirstOrDefault (member => member is IMember && ((IMember)member).Region == foundMember.Region);
			} else if (result is NamespaceResolveResult) {
				var ns = ((NamespaceResolveResult)result).NamespaceName;
				valid = searchedMembers.FirstOrDefault (n => n is string && n.ToString () == ns);
			} else if (result is LocalResolveResult) {
				var ns = ((LocalResolveResult)result).Variable;
				valid = searchedMembers.FirstOrDefault (n => n is IVariable && ((IVariable)n).Region == ns.Region);
			} else if (result is TypeResolveResult) {
				valid = searchedMembers.FirstOrDefault (n => n is IType && result.Type.Equals ((IType)n));
			}
			if (node is InvocationExpression)
				node = ((InvocationExpression)node).Target;
			
			if (node is MemberReferenceExpression)
				node = ((MemberReferenceExpression)node).MemberNameToken;
			if (node is MemberDeclaration && (searchedMembers.First () is IMember)) 
				node = ((MemberDeclaration)node).NameToken;
			
			if (node is TypeDeclaration && (searchedMembers.First () is IType)) 
				node = ((TypeDeclaration)node).NameToken;
			
			if (node is ParameterDeclaration && (searchedMembers.First () is IParameter)) 
				node = ((ParameterDeclaration)node).NameToken;
			
			var region = new DomRegion (fileName, node.StartLocation, node.EndLocation);
			
			return new MemberReference (valid as IEntity, region, editor.LocationToOffset (region.Begin), memberName.Length);
		}

		bool IsNodeValid (object searchedMember, AstNode node)
		{
			if (searchedMember is IField && node is FieldDeclaration)
				return false;
			return true;
		}
		
		public IEnumerable<MemberReference> FindInDocument (MonoDevelop.Ide.Gui.Document doc)
		{
			if (string.IsNullOrEmpty (memberName))
				return Enumerable.Empty<MemberReference> ();
			var editor = doc.Editor;
			var unit = doc.ParsedDocument.Annotation<CompilationUnit> ();
			var file = doc.ParsedDocument.Annotation<CSharpParsedFile> ();
			var ctx = doc.TypeResolveContext;
			var compilation = doc.Compilation;
			var result = new List<MemberReference> ();
			
			foreach (var obj in searchedMembers) {
				if (obj is IEntity) {
					refFinder.FindReferencesInFile (refFinder.GetSearchScopes ((IEntity)obj), file, unit, compilation, (astNode, r) => {
						if (IsNodeValid (obj, astNode))
							result.Add (GetReference (r, astNode, editor.FileName, editor)); 
					}, CancellationToken.None);
				} else if (obj is IVariable) {
					refFinder.FindLocalReferences ((IVariable)obj, file, unit, compilation, (astNode, r) => { 
						if (IsNodeValid (obj, astNode)) 
							result.Add (GetReference (r, astNode, editor.FileName, editor));
					}, CancellationToken.None);
				}
			}
			return result;
		}
		
		public override IEnumerable<MemberReference> FindReferences (Project project, IProjectContent content, IEnumerable<FilePath> possibleFiles, IEnumerable<object> searchedMembers)
		{
			if (project == null)
				throw new ArgumentNullException ("Project", "Project not set.");
			if (content == null)
				throw new ArgumentNullException ("content", "Project content not set.");
			
			SetPossibleFiles (possibleFiles);
			SetSearchedMembers (searchedMembers);
			
			var entity = searchedMembers.First () as IEntity;
			var scopes = searchedMembers.Select (e => refFinder.GetSearchScopes (e as IEntity));
			var compilation = entity != null ? entity.Compilation : content.CreateCompilation ();
			List<MemberReference> refs = new List<MemberReference> ();
			foreach (var opendoc in openDocuments) {
				foreach (var newRef in FindInDocument (opendoc.Item2)) {
					if (refs.Any (r => r.FileName == newRef.FileName && r.Region == newRef.Region))
						continue;
					refs.Add (newRef);
				}
			}
			
			foreach (var file in files) {
				string text = File.ReadAllText (file);
				if (memberName != null && text.IndexOf (memberName, StringComparison.Ordinal) < 0)
					continue;
				using (var editor = new TextEditorData ()) {
					editor.Document.FileName = file;
					editor.Text = text;
					var unit = new CSharpParser ().Parse (editor);
					if (unit == null)
						continue;
					
					var storedFile = content != null ? content.GetFile (file) : null;
					var parsedFile = storedFile as CSharpParsedFile;
					
					if (parsedFile == null && storedFile is ParsedDocumentDecorator) {
						parsedFile = ((ParsedDocumentDecorator)storedFile).ParsedFile as CSharpParsedFile;
					}
					
					if (parsedFile == null) {
						// for fallback purposes - should never happen.
						parsedFile = unit.ToTypeSystem ();
						content = content.UpdateProjectContent (content.GetFile (file), parsedFile);
						compilation = content.CreateCompilation ();
					}
					foreach (var scope in scopes) {
						refFinder.FindReferencesInFile (
							scope,
							parsedFile,
							unit,
							compilation,
							(astNode, result) => {
								var newRef = GetReference (result, astNode, file, editor);
								if (refs.Any (r => r.FileName == newRef.FileName && r.Region == newRef.Region))
									return;
								refs.Add (newRef);
							},
							CancellationToken.None
						);
					}
				}
			}
			return refs;
		}
	}
}

