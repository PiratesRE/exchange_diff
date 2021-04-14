using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	[ComVisible(false)]
	public class NamespaceResolveEventArgs : EventArgs
	{
		public string NamespaceName
		{
			get
			{
				return this._NamespaceName;
			}
		}

		public Assembly RequestingAssembly
		{
			get
			{
				return this._RequestingAssembly;
			}
		}

		public Collection<Assembly> ResolvedAssemblies
		{
			get
			{
				return this._ResolvedAssemblies;
			}
		}

		public NamespaceResolveEventArgs(string namespaceName, Assembly requestingAssembly)
		{
			this._NamespaceName = namespaceName;
			this._RequestingAssembly = requestingAssembly;
			this._ResolvedAssemblies = new Collection<Assembly>();
		}

		private string _NamespaceName;

		private Assembly _RequestingAssembly;

		private Collection<Assembly> _ResolvedAssemblies;
	}
}
