using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.FfoReporting.Common;
using Microsoft.Exchange.Management.FfoReporting.Data;
using Microsoft.Exchange.Management.FfoReporting.Providers;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[Cmdlet("Get", "OutboundConnectorReport")]
	[OutputType(new Type[]
	{
		typeof(OutboundConnectorReport)
	})]
	public sealed class GetOutboundConnectorReport : FfoReportingTask<OutboundConnectorReport>
	{
		[CmdletValidator("ValidateRequiredField", new object[]
		{

		})]
		[Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public Fqdn Domain { get; set; }

		public override string ComponentName
		{
			get
			{
				return ExchangeComponent.FfoRws.Name;
			}
		}

		public override string MonitorEventName
		{
			get
			{
				return "FfoReporting.SmtpChecker";
			}
		}

		protected override IReadOnlyList<OutboundConnectorReport> AggregateOutput()
		{
			return DataProcessorDriver.Process<OutboundConnectorReport>(ServiceLocator.Current.GetService<ISmtpCheckerProvider>().GetOutboundConnectors(this.Domain, base.ConfigSession), ConversionProcessor.Create<OutboundConnectorReport>(this));
		}
	}
}
