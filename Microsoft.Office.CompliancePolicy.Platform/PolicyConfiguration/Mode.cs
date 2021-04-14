using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicyConfiguration
{
	[DataContract]
	public enum Mode
	{
		[EnumMember]
		Enforce,
		[EnumMember]
		Audit,
		[EnumMember]
		PendingDeletion,
		[EnumMember]
		Deleted
	}
}
