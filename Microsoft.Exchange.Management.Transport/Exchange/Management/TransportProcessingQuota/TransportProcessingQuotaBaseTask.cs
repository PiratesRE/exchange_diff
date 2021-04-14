using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Reporting;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.TransportProcessingQuota
{
	public class TransportProcessingQuotaBaseTask : Task
	{
		internal ITenantThrottlingSession Session
		{
			get
			{
				return this.session;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			this.session = HygieneUtils.InstantiateType<ITenantThrottlingSession>("Microsoft.Exchange.Hygiene.Data.Reporting.ReportingSession");
		}

		private ITenantThrottlingSession session;
	}
}
