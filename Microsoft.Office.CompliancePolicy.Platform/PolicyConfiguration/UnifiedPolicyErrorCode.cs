using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicyConfiguration
{
	public enum UnifiedPolicyErrorCode
	{
		[EnumMember]
		Unknown = -1,
		[EnumMember]
		Success,
		[EnumMember]
		InternalError,
		[EnumMember]
		FailedToOpenContainer = 16777217,
		[EnumMember]
		SiteInReadonlyOrNotAccessible,
		[EnumMember]
		SiteOutOfQuota,
		[EnumMember]
		PolicyNotifyError = 16777223,
		[EnumMember]
		PolicySyncTimeout
	}
}
