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
	[Cmdlet("Get", "ServiceDeliveryReport")]
	[OutputType(new Type[]
	{
		typeof(ServiceDeliveryReport)
	})]
	public sealed class GetServiceDeliveryReport : FfoReportingTask<ServiceDeliveryReport>
	{
		[CmdletValidator("ValidateRequiredField", new object[]
		{

		})]
		[Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public SmtpAddress Recipient { get; set; }

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

		protected override IReadOnlyList<ServiceDeliveryReport> AggregateOutput()
		{
			return DataProcessorDriver.Process<ServiceDeliveryReport>(ServiceLocator.Current.GetService<ISmtpCheckerProvider>().GetServiceDeliveries(this.Recipient, base.ConfigSession), ConversionProcessor.Create<ServiceDeliveryReport>(this));
		}
	}
}
