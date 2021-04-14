using System;
using System.Runtime.InteropServices;

namespace Microsoft.Office.CompliancePolicy.Classification
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct RULE_PACKAGE_TIMESTAMP_DETAILS
	{
		[MarshalAs(UnmanagedType.BStr)]
		public string RulePackageSetID;

		[MarshalAs(UnmanagedType.BStr)]
		public string RulePackageID;

		[MarshalAs(UnmanagedType.BStr)]
		public string LastUpdatedTime;

		[MarshalAs(UnmanagedType.VariantBool)]
		public bool RulePackageChanged;
	}
}
