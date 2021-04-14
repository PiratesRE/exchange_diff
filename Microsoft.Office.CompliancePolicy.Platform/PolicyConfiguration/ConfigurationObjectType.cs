using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicyConfiguration
{
	[DataContract]
	public enum ConfigurationObjectType
	{
		[EnumMember]
		Policy,
		[EnumMember]
		Rule,
		[EnumMember]
		Association,
		[EnumMember]
		Binding,
		[EnumMember]
		Scope
	}
}
