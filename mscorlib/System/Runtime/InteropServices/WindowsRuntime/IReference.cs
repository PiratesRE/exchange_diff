using System;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	[Guid("61c17706-2d65-11e0-9ae8-d48564015472")]
	[ComImport]
	internal interface IReference<T> : IPropertyValue
	{
		T Value { get; }
	}
}
