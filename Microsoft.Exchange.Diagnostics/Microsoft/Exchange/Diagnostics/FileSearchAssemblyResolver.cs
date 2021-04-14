using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Exchange.Diagnostics
{
	public sealed class FileSearchAssemblyResolver : AssemblyResolver
	{
		public bool Recursive { get; set; }

		public string[] SearchPaths { get; set; }

		protected override IEnumerable<string> GetCandidateAssemblyPaths(AssemblyName nameToResolve)
		{
			string fileName = AssemblyResolver.GetAssemblyFileNameFromFullName(nameToResolve);
			return base.FilterDirectoryPaths(this.SearchPaths).SelectMany((string path) => this.FindAssembly(path, fileName, this.Recursive));
		}
	}
}
