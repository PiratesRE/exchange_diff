using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[Guid("00020302-0000-0000-C000-000000000046")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IMAPIAdviseSink
	{
		[PreserveSig]
		unsafe int OnNotify(int cNotif, NOTIFICATION* lpNotifications);
	}
}
