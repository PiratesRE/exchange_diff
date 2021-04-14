using System;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	[Guid("6a79e863-4300-459a-9966-cbb660963ee1")]
	[ComImport]
	internal interface IIterator<T>
	{
		T Current { get; }

		bool HasCurrent { get; }

		bool MoveNext();

		int GetMany([Out] T[] items);
	}
}
