using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	[Guid("913337e9-11a1-4345-a3a2-4e7f956e222d")]
	[ComImport]
	internal interface IVector<T> : IIterable<T>, IEnumerable<!0>, IEnumerable
	{
		T GetAt(uint index);

		uint Size { get; }

		IReadOnlyList<T> GetView();

		bool IndexOf(T value, out uint index);

		void SetAt(uint index, T value);

		void InsertAt(uint index, T value);

		void RemoveAt(uint index);

		void Append(T value);

		void RemoveAtEnd();

		void Clear();

		uint GetMany(uint startIndex, [Out] T[] items);

		void ReplaceAll(T[] items);
	}
}
