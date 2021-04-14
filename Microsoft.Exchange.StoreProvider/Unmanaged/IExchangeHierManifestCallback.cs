using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("36F25379-50FD-4662-9021-AC684B0D6AAA")]
	[ComImport]
	internal interface IExchangeHierManifestCallback
	{
		[PreserveSig]
		unsafe int Change(int cpval, SPropValue* ppval);

		[PreserveSig]
		int Delete(int cbIdsetDeleted, IntPtr pbIdsetDeleted);
	}
}
