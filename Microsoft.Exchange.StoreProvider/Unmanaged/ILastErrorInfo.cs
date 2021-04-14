using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[Guid("42A2AEE7-E53B-49e3-9011-8DF591F16085")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface ILastErrorInfo
	{
		[PreserveSig]
		int GetLastError(int hResult, out SafeExLinkedMemoryHandle lpMapiError);

		[PreserveSig]
		int GetExtendedErrorInfo(out SafeExMemoryHandle pExtendedErrorInfo);
	}
}
