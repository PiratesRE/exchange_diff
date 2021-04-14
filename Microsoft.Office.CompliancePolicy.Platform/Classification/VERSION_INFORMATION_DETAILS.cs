using System;
using System.Runtime.InteropServices;

namespace Microsoft.Office.CompliancePolicy.Classification
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct VERSION_INFORMATION_DETAILS
	{
		public uint MajorVersion;

		public uint MinorVersion;

		public uint BuildNumber;

		public uint RevisionNumber;
	}
}
