using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[Serializable]
	public delegate bool TypeFilter(Type m, object filterCriteria);
}
