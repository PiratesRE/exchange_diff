using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FNonExtensible | TypeLibTypeFlags.FDispatchable)]
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Guid("095DE050-9F1A-44E8-85B6-6C195D288BC5")]
	[ComImport]
	internal interface IOleConverter
	{
		[DispId(1)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		void ConfigureConverter(out int problem, uint maximumUncompressedImageSize, uint maximumMarshalledImageSize, [MarshalAs(UnmanagedType.BStr)] string tempFileDirectory, out uint processId);

		[DispId(2)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		void OleConvertToBmp(out int problem, [MarshalAs(UnmanagedType.Struct)] object oleObjectData, [MarshalAs(UnmanagedType.Struct)] out object pBitmapData);
	}
}
