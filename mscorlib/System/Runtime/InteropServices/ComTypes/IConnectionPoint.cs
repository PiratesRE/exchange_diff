using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[Guid("B196B286-BAB4-101A-B69C-00AA00341D07")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[__DynamicallyInvokable]
	[ComImport]
	public interface IConnectionPoint
	{
		[__DynamicallyInvokable]
		void GetConnectionInterface(out Guid pIID);

		[__DynamicallyInvokable]
		void GetConnectionPointContainer(out IConnectionPointContainer ppCPC);

		[__DynamicallyInvokable]
		void Advise([MarshalAs(UnmanagedType.Interface)] object pUnkSink, out int pdwCookie);

		[__DynamicallyInvokable]
		void Unadvise(int dwCookie);

		[__DynamicallyInvokable]
		void EnumConnections(out IEnumConnections ppEnum);
	}
}
