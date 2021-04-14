using System;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	public interface IFormatProvider
	{
		[__DynamicallyInvokable]
		object GetFormat(Type formatType);
	}
}
