using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.TransportProcessingQuota
{
	[Cmdlet("Get", "TransportProcessingQuotaConfig")]
	public sealed class GetTransportProcessingQuotaConfig : TransportProcessingQuotaBaseTask
	{
		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			base.WriteObject(base.Session.GetTransportThrottlingConfig());
		}
	}
}
