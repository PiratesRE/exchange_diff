using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.RightsManagementServices.Online
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "TenantStatus", Namespace = "http://microsoft.com/RightsManagementServiceOnline/2011/04")]
	public enum TenantStatus
	{
		[EnumMember]
		Disabled,
		[EnumMember]
		Enabled,
		[EnumMember]
		Deprovisioned
	}
}
