using System;
using System.Collections.ObjectModel;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	[ComVisible(false)]
	public class DesignerNamespaceResolveEventArgs : EventArgs
	{
		public string NamespaceName
		{
			get
			{
				return this._NamespaceName;
			}
		}

		public Collection<string> ResolvedAssemblyFiles
		{
			get
			{
				return this._ResolvedAssemblyFiles;
			}
		}

		public DesignerNamespaceResolveEventArgs(string namespaceName)
		{
			this._NamespaceName = namespaceName;
			this._ResolvedAssemblyFiles = new Collection<string>();
		}

		private string _NamespaceName;

		private Collection<string> _ResolvedAssemblyFiles;
	}
}
