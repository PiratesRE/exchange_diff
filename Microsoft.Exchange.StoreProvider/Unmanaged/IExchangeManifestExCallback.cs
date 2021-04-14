using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("149C53E5-8519-45dc-9F8B-9D248DB1A78C")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	[ComImport]
	internal interface IExchangeManifestExCallback
	{
		[PreserveSig]
		unsafe int Change(ExchangeManifestCallbackChangeFlags flags, int cpvalHeader, SPropValue* ppvalHeader, int cpvalProps, SPropValue* ppvalProps);

		[PreserveSig]
		int Delete(ExchangeManifestCallbackDeleteFlags flags, int cbIdsetDeleted, IntPtr pbIdsetDeleted);

		[PreserveSig]
		int Read(ExchangeManifestCallbackReadFlags flags, int cbIdsetReadUnread, IntPtr pbIdsetReadUnread);
	}
}
