using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[Serializable]
	public delegate Module ModuleResolveEventHandler(object sender, ResolveEventArgs e);
}
