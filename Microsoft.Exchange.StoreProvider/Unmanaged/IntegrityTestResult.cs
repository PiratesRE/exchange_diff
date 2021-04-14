using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct IntegrityTestResult
	{
		private const int TestNameLength = 40;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
		public string Name;

		public uint Errors;

		public uint Warnings;

		public uint Fixes;

		public uint Time;

		public uint Rows;

		public int ErrorCode;
	}
}
