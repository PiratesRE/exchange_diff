using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public delegate Assembly ResolveEventHandler(object sender, ResolveEventArgs args);
}
