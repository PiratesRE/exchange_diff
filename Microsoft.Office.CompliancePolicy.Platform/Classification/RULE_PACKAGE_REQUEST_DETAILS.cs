using System;
using System.Runtime.InteropServices;

namespace Microsoft.Office.CompliancePolicy.Classification
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct RULE_PACKAGE_REQUEST_DETAILS
	{
		[MarshalAs(UnmanagedType.BStr)]
		public string RulePackageSetID;

		[MarshalAs(UnmanagedType.BStr)]
		public string RulePackageID;

		[MarshalAs(UnmanagedType.BStr)]
		public string RulePackage;

		[MarshalAs(UnmanagedType.BStr)]
		public string LastUpdatedTime;
	}
}
