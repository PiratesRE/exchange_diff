using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.BITS
{
	[Guid("19C613A0-FCB8-4F28-81AE-897C3D078F81")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IBackgroundCopyError
	{
		void GetError(out BG_ERROR_CONTEXT pContext, [MarshalAs(UnmanagedType.Error)] out int pCode);

		void GetFile([MarshalAs(UnmanagedType.Interface)] out IBackgroundCopyFile pVal);

		void GetErrorDescription(uint LanguageId, [MarshalAs(UnmanagedType.LPWStr)] out string pErrorDescription);

		void GetErrorContextDescription(uint LanguageId, [MarshalAs(UnmanagedType.LPWStr)] out string pContextDescription);

		void GetProtocol([MarshalAs(UnmanagedType.LPWStr)] out string pProtocol);
	}
}
