using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicyConfiguration
{
	[DataContract]
	public enum CARatingAppsEntry
	{
		[EnumMember]
		DontAllow,
		[EnumMember]
		Rating4plus = 100,
		[EnumMember]
		Rating9plus = 200,
		[EnumMember]
		Rating12plus = 300,
		[EnumMember]
		Rating17plus = 600,
		[EnumMember]
		AllowAll = 1000
	}
}
