using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Search
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("CC907054-C058-101A-B554-08002B33B0E6")]
	[ComImport]
	internal interface IPhraseSink
	{
		[PreserveSig]
		void PutSmallPhrase([MarshalAs(UnmanagedType.LPWStr)] string pwcNoun, [MarshalAs(UnmanagedType.U4)] int cwcNoun, [MarshalAs(UnmanagedType.LPWStr)] string pwcModifier, [MarshalAs(UnmanagedType.U4)] int cwcModifier, [MarshalAs(UnmanagedType.U4)] int ulAttachmentType);

		[PreserveSig]
		void PutPhrase([MarshalAs(UnmanagedType.LPWStr)] string pwcPhrase, [MarshalAs(UnmanagedType.U4)] int cwcPhrase);
	}
}
