using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct FxBlock
	{
		public static readonly int SizeOf = Marshal.SizeOf(typeof(FxBlock));

		public IntPtr buffer;

		public int bufferSize;

		public uint steps;

		public uint progress;

		public ushort state;
	}
}
