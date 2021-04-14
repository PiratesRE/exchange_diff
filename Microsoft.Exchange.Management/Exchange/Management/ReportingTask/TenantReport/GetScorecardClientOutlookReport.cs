using System;
using System.Management.Automation;
using Microsoft.Exchange.Management.ReportingTask.Common;
using Microsoft.Exchange.Management.ReportingTask.Query;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[OutputType(new Type[]
	{
		typeof(ScorecardClientOutlookReport)
	})]
	[Cmdlet("Get", "ScorecardClientOutlookReport")]
	public sealed class GetScorecardClientOutlookReport : TenantReportBase<ScorecardClientOutlookReport>
	{
		[Parameter(Mandatory = false)]
		public DataCategory Category { get; set; }

		protected override void ProcessNonPipelineParameter()
		{
			base.ProcessNonPipelineParameter();
			base.AddQueryDecorator(new WhereDecorator<ScorecardClientOutlookReport>(base.TaskContext)
			{
				Predicate = ((ScorecardClientOutlookReport report) => this.Category.ToString().Equals(report.Category))
			});
		}
	}
}
