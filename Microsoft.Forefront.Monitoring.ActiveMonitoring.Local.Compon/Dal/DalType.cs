using System;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Dal
{
	public enum DalType
	{
		Spam = 2,
		Domain,
		ServiceHealth,
		Reporting,
		Mtrt,
		MtrtAggregated,
		IdGen,
		MtrtEtl,
		Kes,
		BackgroundJobBackend,
		Migration,
		Global = 1001,
		Tenant,
		Recipient
	}
}
