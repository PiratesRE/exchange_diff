using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.EhfHybridMailflow
{
	[Cmdlet("Get", "HybridMailflow")]
	public sealed class GetHybridMailflow : HybridMailflowTaskBase
	{
		protected override void InternalProcessRecord()
		{
			if (base.OriginalInboundConnector == null)
			{
				base.WriteVerbose(base.NullInboundConnectorMessage);
				return;
			}
			if (base.OriginalOutboundConnector == null)
			{
				base.WriteVerbose(base.NullOutboundConnectorMessage);
				return;
			}
			base.WriteObject(HybridMailflowTaskBase.ConvertToHybridMailflowConfiguration(base.OriginalInboundConnector, base.OriginalOutboundConnector));
		}

		private HybridMailflowConfiguration GetHybridMailflowSettingsFromMock()
		{
			HybridMailflowConfiguration result;
			if (base.Organization != null)
			{
				result = new HybridMailflowConfiguration(new List<SmtpDomainWithSubdomains>
				{
					new SmtpDomainWithSubdomains("contoso.com"),
					new SmtpDomainWithSubdomains("test.contoso.com")
				}, new List<IPRange>(), new Fqdn("mail.contoso.com"), "*.contoso.com", new bool?(false), new bool?(false));
			}
			else
			{
				result = new HybridMailflowConfiguration(new List<SmtpDomainWithSubdomains>(), new List<IPRange>(), new Fqdn("mail.fabrikam.com"), "*.fabrikam.com", new bool?(true), new bool?(false));
			}
			return result;
		}

		private const string OperationName = "Get-HybridMailflow";
	}
}
