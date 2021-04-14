using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.BITS
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("01B7BD23-FB88-4A77-8490-5891D3E4653A")]
	[ComImport]
	internal interface IBackgroundCopyFile
	{
		void GetRemoteName([MarshalAs(UnmanagedType.LPWStr)] out string pVal);

		void GetLocalName([MarshalAs(UnmanagedType.LPWStr)] out string pVal);

		void GetProgress(out _BG_FILE_PROGRESS pVal);
	}
}
