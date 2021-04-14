using System;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	[Guid("346dd6e7-976e-4bc3-815d-ece243bc0f33")]
	[ComImport]
	internal interface IBindableVectorView : IBindableIterable
	{
		object GetAt(uint index);

		uint Size { get; }

		bool IndexOf(object value, out uint index);
	}
}
