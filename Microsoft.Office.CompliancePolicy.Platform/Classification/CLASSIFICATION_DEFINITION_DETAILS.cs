using System;
using System.Runtime.InteropServices;

namespace Microsoft.Office.CompliancePolicy.Classification
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct CLASSIFICATION_DEFINITION_DETAILS
	{
		[MarshalAs(UnmanagedType.BStr)]
		public string PublisherName;

		[MarshalAs(UnmanagedType.BStr)]
		public string RulePackageName;

		[MarshalAs(UnmanagedType.BStr)]
		public string RulePackageDesc;

		[MarshalAs(UnmanagedType.BStr)]
		public string DefinitionName;

		[MarshalAs(UnmanagedType.BStr)]
		public string Description;
	}
}
