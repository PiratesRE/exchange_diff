using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicyConfiguration
{
	[DataContract]
	public enum PolicyResourceScope
	{
		[EnumMember]
		ExchangeOnline,
		[EnumMember]
		SharepointOnline,
		[EnumMember]
		ExchangeAndSharepoint
	}
}
