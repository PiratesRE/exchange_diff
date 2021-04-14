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
	[Cmdlet("Get", "MxRecordReport")]
	[OutputType(new Type[]
	{
		typeof(MxRecordReport)
	})]
	public sealed class GetMxRecordReport : FfoReportingTask<MxRecordReport>
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

		protected override IReadOnlyList<MxRecordReport> AggregateOutput()
		{
			return DataProcessorDriver.Process<MxRecordReport>(ServiceLocator.Current.GetService<ISmtpCheckerProvider>().GetMxRecords(this.Domain, base.ConfigSession), ConversionProcessor.Create<MxRecordReport>(this));
		}
	}
}
