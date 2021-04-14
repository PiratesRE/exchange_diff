using System;
using System.Runtime.InteropServices;

namespace System.Collections
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	public interface IComparer
	{
		[__DynamicallyInvokable]
		int Compare(object x, object y);
	}
}
