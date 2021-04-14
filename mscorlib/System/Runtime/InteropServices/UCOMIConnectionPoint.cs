using System;

namespace System.Runtime.InteropServices
{
	[Obsolete("Use System.Runtime.InteropServices.ComTypes.IConnectionPoint instead. http://go.microsoft.com/fwlink/?linkid=14202", false)]
	[Guid("B196B286-BAB4-101A-B69C-00AA00341D07")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	public interface UCOMIConnectionPoint
	{
		void GetConnectionInterface(out Guid pIID);

		void GetConnectionPointContainer(out UCOMIConnectionPointContainer ppCPC);

		void Advise([MarshalAs(UnmanagedType.Interface)] object pUnkSink, out int pdwCookie);

		void Unadvise(int dwCookie);

		void EnumConnections(out UCOMIEnumConnections ppEnum);
	}
}
