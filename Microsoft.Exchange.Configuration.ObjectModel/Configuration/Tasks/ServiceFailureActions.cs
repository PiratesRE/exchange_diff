using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal struct ServiceFailureActions
	{
		public uint resetPeriod;

		[MarshalAs(UnmanagedType.LPTStr)]
		public string rebootMessage;

		[MarshalAs(UnmanagedType.LPTStr)]
		public string command;

		public uint actionCount;

		public IntPtr actions;
	}
}
