using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("8F590A55-9A10-4cd9-916C-8748E24C311F")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	[ComImport]
	internal interface IExchangeManifestCallback
	{
		[PreserveSig]
		unsafe int Change(ExchangeManifestCallbackChangeFlags flags, int cpvalHeader, SPropValue* ppvalHeader, int cpvalProps, SPropValue* ppvalProps);

		[PreserveSig]
		unsafe int Delete(ExchangeManifestCallbackDeleteFlags flags, int cElements, _CallbackInfo* lpCallbackInfo);

		[PreserveSig]
		unsafe int Read(ExchangeManifestCallbackReadFlags flags, int cElements, _CallbackInfo* lpCallbackInfo);
	}
}
