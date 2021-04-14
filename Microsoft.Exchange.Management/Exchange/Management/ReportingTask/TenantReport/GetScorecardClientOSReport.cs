using System;
using System.Management.Automation;
using Microsoft.Exchange.Management.ReportingTask.Common;
using Microsoft.Exchange.Management.ReportingTask.Query;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[Cmdlet("Get", "ScorecardClientOSReport")]
	[OutputType(new Type[]
	{
		typeof(ScorecardClientOSReport)
	})]
	public sealed class GetScorecardClientOSReport : TenantReportBase<ScorecardClientOSReport>
	{
		[Parameter(Mandatory = false)]
		public DataCategory Category { get; set; }

		protected override void ProcessNonPipelineParameter()
		{
			base.ProcessNonPipelineParameter();
			base.AddQueryDecorator(new WhereDecorator<ScorecardClientOSReport>(base.TaskContext)
			{
				Predicate = ((ScorecardClientOSReport report) => this.Category.ToString().Equals(report.Category))
			});
		}
	}
}
