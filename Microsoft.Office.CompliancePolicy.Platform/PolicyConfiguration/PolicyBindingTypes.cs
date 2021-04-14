using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicyConfiguration
{
	[DataContract]
	public enum PolicyBindingTypes
	{
		[EnumMember]
		IndividualResource,
		[EnumMember]
		Tenant,
		[EnumMember]
		SiteTemplate
	}
}
