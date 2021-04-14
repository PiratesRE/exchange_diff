using System;
using System.Runtime.InteropServices;

namespace System.Collections
{
	[Obsolete("Please use IEqualityComparer instead.")]
	[ComVisible(true)]
	public interface IHashCodeProvider
	{
		int GetHashCode(object obj);
	}
}
