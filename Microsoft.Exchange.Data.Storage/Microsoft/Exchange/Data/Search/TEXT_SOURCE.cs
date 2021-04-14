using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Search
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct TEXT_SOURCE
	{
		[MarshalAs(UnmanagedType.FunctionPtr)]
		public FillTextBuffer FillTextBuffer;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string Buffer;

		[MarshalAs(UnmanagedType.U4)]
		public int End;

		[MarshalAs(UnmanagedType.U4)]
		public int Current;
	}
}
