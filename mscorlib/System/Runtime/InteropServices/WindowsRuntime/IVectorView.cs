using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	[Guid("bbe1fa4c-b0e3-4583-baef-1f1b2e483e56")]
	[ComImport]
	internal interface IVectorView<T> : IIterable<T>, IEnumerable<!0>, IEnumerable
	{
		T GetAt(uint index);

		uint Size { get; }

		bool IndexOf(T value, out uint index);

		uint GetMany(uint startIndex, [Out] T[] items);
	}
}
