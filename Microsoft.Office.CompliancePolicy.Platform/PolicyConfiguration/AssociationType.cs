using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicyConfiguration
{
	[DataContract]
	public enum AssociationType
	{
		[EnumMember]
		SPSiteCollection,
		[EnumMember]
		SPTemplate
	}
}
