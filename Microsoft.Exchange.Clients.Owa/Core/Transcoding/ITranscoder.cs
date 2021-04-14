using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Clients.Owa.Core.Transcoding
{
	[Guid("A556B022-130F-443F-AFF5-AE9AAC269D3D")]
	[TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FNonExtensible | TypeLibTypeFlags.FDispatchable)]
	[ComImport]
	internal interface ITranscoder
	{
		[DispId(1)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		TranscodeErrorCode Initialize([In] TranscodingInitOption initOption);

		[DispId(2)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		TranscodeErrorCode Convert([MarshalAs(UnmanagedType.BStr)] [In] string sourceDocPath, [MarshalAs(UnmanagedType.BStr)] [In] string outputFilePath, [MarshalAs(UnmanagedType.BStr)] [In] string sourceDocType, [In] int currentPageNumber, out int totalPageNumber, out int outputDataSize);
	}
}
