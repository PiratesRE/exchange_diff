using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum DlpPolicyState
	{
		Disabled_Audit,
		Disabled_AuditAndNotify,
		Disabled_Enforce,
		Enabled_Audit,
		Enabled_AuditAndNotify,
		Enabled_Enforce
	}
}
