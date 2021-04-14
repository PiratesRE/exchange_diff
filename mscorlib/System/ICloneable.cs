using System;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	public interface ICloneable
	{
		object Clone();
	}
}
