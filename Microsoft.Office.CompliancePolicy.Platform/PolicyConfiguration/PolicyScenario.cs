using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicyConfiguration
{
	[DataContract]
	public enum PolicyScenario
	{
		[EnumMember]
		Retention,
		[EnumMember]
		Hold,
		[EnumMember]
		Dlp,
		[EnumMember]
		DeviceSettings,
		[EnumMember]
		AuditSettings,
		[EnumMember]
		DeviceConditionalAccess,
		[EnumMember]
		DeviceTenantConditionalAccess
	}
}
