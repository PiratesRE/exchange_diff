using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicyConfiguration
{
	[DataContract]
	public enum ChangeType
	{
		[EnumMember]
		None,
		[EnumMember]
		Update,
		[EnumMember]
		Delete,
		[EnumMember]
		Add
	}
}
