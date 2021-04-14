using System;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	[Guid("393de7de-6fd0-4c0d-bb71-47244a113e93")]
	[ComImport]
	internal interface IBindableVector : IBindableIterable
	{
		object GetAt(uint index);

		uint Size { get; }

		IBindableVectorView GetView();

		bool IndexOf(object value, out uint index);

		void SetAt(uint index, object value);

		void InsertAt(uint index, object value);

		void RemoveAt(uint index);

		void Append(object value);

		void RemoveAtEnd();

		void Clear();
	}
}
