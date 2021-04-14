using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Search
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("D53552C8-77E3-101A-B552-08002B33B0E6")]
	[ComImport]
	internal interface IWordBreaker
	{
		[PreserveSig]
		int Init([In] bool fQuery, [MarshalAs(UnmanagedType.U4)] int maxTokenSize, out bool pfLicense);

		[PreserveSig]
		int BreakText([MarshalAs(UnmanagedType.Struct)] [In] [Out] ref TEXT_SOURCE pTextSource, [In] IWordSink pWordSink, [In] IPhraseSink pPhraseSink);

		[PreserveSig]
		int GetLicenseToUse([MarshalAs(UnmanagedType.LPWStr)] out string ppwcsLicense);
	}
}
