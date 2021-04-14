using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum HybridConfigurationStatusFlags
	{
		Unknown,
		NotEnabled,
		HCW14,
		HCW15,
		HCW15Upgrade,
		HCWManual,
		HCW15OAuth,
		HCWDecommissioned,
		HCWSecureMail,
		HCWCentralizedMail,
		HCWEdge
	}
}
