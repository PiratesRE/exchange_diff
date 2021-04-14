using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.OAB
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class LzxInterop
	{
		[DllImport("mspatchLInterop.dll", EntryPoint = "InteropRawLzxCompressBuffer", SetLastError = true)]
		public static extern uint RawLzxCompressBuffer(IntPtr uncompressedBuffer, int uncompressedSize, IntPtr compressedBuffer, int compressedBufferSize, out int compressedSize);

		[DllImport("mspatchLInterop.dll", EntryPoint = "InteropCreateRawLzxPatchDataFromBuffers", SetLastError = true)]
		public static extern uint CreateRawLzxPatchDataFromBuffers(IntPtr oldDataBuffer, int oldDataSize, IntPtr newDataBuffer, int newDataSize, IntPtr patchDataBuffer, int patchDataSize, out int actualPatchDataSize);

		[DllImport("mspatchLInterop.dll", EntryPoint = "InteropApplyRawLzxPatchToBuffer", SetLastError = true)]
		public static extern uint ApplyRawLzxPatchToBuffer(IntPtr oldDataBuffer, int oldDataSize, IntPtr patchDataBuffer, int patchDataSize, IntPtr newDataBuffer, int newDataSize);

		public const uint Win32ErrorInsufficientBuffer = 122U;
	}
}
