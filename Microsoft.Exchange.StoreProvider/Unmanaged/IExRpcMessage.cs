using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Mapi.Unmanaged
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("83BB0082-568A-4227-A830-C1A3844B9331")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	[ComImport]
	internal interface IExRpcMessage
	{
		[PreserveSig]
		int Deliver(int ulFlags);

		[PreserveSig]
		int DoneWithMessage();

		[PreserveSig]
		int TransportSendMessage(out int lpcValues, [PointerType("SPropValue*")] out SafeExLinkedMemoryHandle lppPropArray);

		[PreserveSig]
		int SubmitMessageEx(int ulFlags);
	}
}
