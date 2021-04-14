using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Search
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("CC907054-C058-101A-B554-08002B33B0E6")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	[ComImport]
	internal interface IWordSink
	{
		[PreserveSig]
		void PutWord([MarshalAs(UnmanagedType.U4)] int cwc, [MarshalAs(UnmanagedType.LPWStr)] string pwcInBuf, [MarshalAs(UnmanagedType.U4)] int cwcSrcLen, [MarshalAs(UnmanagedType.U4)] int cwcSrcPos);

		[PreserveSig]
		void PutAltWord([MarshalAs(UnmanagedType.U4)] int cwc, [MarshalAs(UnmanagedType.LPWStr)] string pwcInBuf, [MarshalAs(UnmanagedType.U4)] int cwcSrcLen, [MarshalAs(UnmanagedType.U4)] int cwcSrcPos);

		[PreserveSig]
		void StartAltPhrase();

		[PreserveSig]
		void EndAltPhrase();

		[PreserveSig]
		void PutBreak([In] WordBreakType breakType);
	}
}
