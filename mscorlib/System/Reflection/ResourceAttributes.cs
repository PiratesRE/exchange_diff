using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Flags]
	[ComVisible(true)]
	[Serializable]
	public enum ResourceAttributes
	{
		Public = 1,
		Private = 2
	}
}
