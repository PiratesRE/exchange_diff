using System;
using System.Management.Automation;
using Microsoft.Exchange.Management.ReportingTask.Query;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[Cmdlet("Get", "O365ClientOSReport")]
	[OutputType(new Type[]
	{
		typeof(ClientSoftwareOSSummaryReport)
	})]
	public sealed class GetClientOSReport : TenantReportBase<ClientSoftwareOSSummaryReport>
	{
		[Parameter(Mandatory = false)]
		public string OS { get; set; }

		protected override void ProcessNonPipelineParameter()
		{
			base.ProcessNonPipelineParameter();
			WhereDecorator<ClientSoftwareOSSummaryReport> whereDecorator = new WhereDecorator<ClientSoftwareOSSummaryReport>(base.TaskContext);
			if (this.OS != null)
			{
				whereDecorator.Predicate = ((ClientSoftwareOSSummaryReport report) => this.OS.Equals(report.Category));
			}
			else
			{
				whereDecorator.Predicate = ((ClientSoftwareOSSummaryReport report) => "SUMMARY".Equals(report.Category));
			}
			base.AddQueryDecorator(whereDecorator);
		}
	}
}
