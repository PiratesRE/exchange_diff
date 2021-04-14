using System;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public delegate void AssemblyLoadEventHandler(object sender, AssemblyLoadEventArgs args);
}
