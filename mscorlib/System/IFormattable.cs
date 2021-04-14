using System;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	public interface IFormattable
	{
		[__DynamicallyInvokable]
		string ToString(string format, IFormatProvider formatProvider);
	}
}
