//
// DotNetCoreRuntimeSystemInformation.cs
//
// Author:
//       Rodrigo Moya <rodrigo.moya@xamarin.com>
//
// Copyright (c) 2019 Microsoft (http://microsoft.com)
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
using System.Linq;
using System.Text;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Updater;

namespace MonoDevelop.DotNetCore
{
	sealed class DotNetCoreRuntimeSystemInformation : ProductInformationProvider
	{
		public override string Title => GettextCatalog.GetString (".NET Core Runtime");

		public override string Description => GetDescription ();

		public override string Version => DotNetCoreRuntime.Versions.FirstOrDefault ()?.ToString ();

		public override string ApplicationId => "392e9bd0-7214-4d2f-8b38-420b38e3b20f";

		public override UpdateInfo GetUpdateInfo ()
		{
			var latestRuntimeVersion = DotNetCoreRuntime.Versions.FirstOrDefault ();
			if (latestRuntimeVersion != null) {
				return new UpdateInfo (ApplicationId, latestRuntimeVersion.Version);
			}

			// .NET Core not installed, so don't offer as new install
			return null;
		}

		string GetDescription ()
		{
			var description = new StringBuilder ();

			description.AppendLine (GettextCatalog.GetString ("Runtime: {0}", GetDotNetRuntimeLocation ()));
			AppendDotNetCoreRuntimeVersions (description);

			return description.ToString ();
		}

		static string GetDotNetRuntimeLocation ()
		{
			if (DotNetCoreRuntime.IsInstalled)
				return DotNetCoreRuntime.FileName;

			return DotNetCoreSystemInformation.GetNotInstalledString ();
		}

		static void AppendDotNetCoreRuntimeVersions (StringBuilder description)
		{
			DotNetCoreSystemInformation.AppendVersions (
				description,
				DotNetCoreRuntime.Versions,
				version => GettextCatalog.GetString ("Runtime Version: {0}", version),
				() => GettextCatalog.GetString ("Runtime Versions:"));
		}
	}
}
