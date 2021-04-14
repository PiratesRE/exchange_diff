using System;
using System.Management.Automation;
using Microsoft.Exchange.Management.ReportingTask.Query;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[Cmdlet("Get", "O365ClientBrowserReport")]
	[OutputType(new Type[]
	{
		typeof(ClientSoftwareBrowserSummaryReport)
	})]
	public sealed class GetClientBrowserReport : TenantReportBase<ClientSoftwareBrowserSummaryReport>
	{
		[Parameter(Mandatory = false)]
		public string Browser { get; set; }

		protected override void ProcessNonPipelineParameter()
		{
			base.ProcessNonPipelineParameter();
			WhereDecorator<ClientSoftwareBrowserSummaryReport> whereDecorator = new WhereDecorator<ClientSoftwareBrowserSummaryReport>(base.TaskContext);
			if (this.Browser != null)
			{
				whereDecorator.Predicate = ((ClientSoftwareBrowserSummaryReport report) => this.Browser.Equals(report.Category));
			}
			else
			{
				whereDecorator.Predicate = ((ClientSoftwareBrowserSummaryReport report) => "SUMMARY".Equals(report.Category));
			}
			base.AddQueryDecorator(whereDecorator);
		}
	}
}
