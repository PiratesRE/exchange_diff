using System;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Data.Reporting;

namespace Microsoft.Exchange.Management.TransportProcessingQuota
{
	[Cmdlet("Get", "TransportProcessingQuotaDigest")]
	public sealed class GetTransportProcessingQuotaDigest : TransportProcessingQuotaBaseTask
	{
		[Parameter(Mandatory = false)]
		public int? ResultSize { get; set; }

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			foreach (TenantThrottleInfo tenantThrottleInfo in from i in base.Session.GetTenantThrottlingDigest(0, null, false, this.ResultSize ?? 100, false)
			orderby i.AverageMessageCostMs descending
			select i)
			{
				base.WriteObject(TransportProcessingQuotaDigest.Create(tenantThrottleInfo));
			}
		}

		private const int DefaultResultSize = 100;
	}
}
