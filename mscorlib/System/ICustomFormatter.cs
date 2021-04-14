using System;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	public interface ICustomFormatter
	{
		[__DynamicallyInvokable]
		string Format(string format, object arg, IFormatProvider formatProvider);
	}
}
