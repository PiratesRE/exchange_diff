using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	public class AssemblyLoadEventArgs : EventArgs
	{
		public Assembly LoadedAssembly
		{
			get
			{
				return this._LoadedAssembly;
			}
		}

		public AssemblyLoadEventArgs(Assembly loadedAssembly)
		{
			this._LoadedAssembly = loadedAssembly;
		}

		private Assembly _LoadedAssembly;
	}
}
