using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicyConfiguration
{
	[Flags]
	[DataContract]
	public enum Workload
	{
		None = 0,
		[EnumMember]
		Exchange = 1,
		[EnumMember]
		SharePoint = 2,
		[EnumMember]
		Intune = 4,
		[EnumMember]
		OneDriveForBusiness = 8
	}
}
