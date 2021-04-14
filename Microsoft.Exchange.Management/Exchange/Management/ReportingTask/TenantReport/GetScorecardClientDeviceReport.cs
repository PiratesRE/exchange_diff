using System;
using System.Management.Automation;
using Microsoft.Exchange.Management.ReportingTask.Common;
using Microsoft.Exchange.Management.ReportingTask.Query;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[OutputType(new Type[]
	{
		typeof(ScorecardClientDeviceReport)
	})]
	[Cmdlet("Get", "ScorecardClientDeviceReport")]
	public sealed class GetScorecardClientDeviceReport : TenantReportBase<ScorecardClientDeviceReport>
	{
		[Parameter(Mandatory = false)]
		public DataCategory Category { get; set; }

		protected override void ProcessNonPipelineParameter()
		{
			base.ProcessNonPipelineParameter();
			base.AddQueryDecorator(new WhereDecorator<ScorecardClientDeviceReport>(base.TaskContext)
			{
				Predicate = ((ScorecardClientDeviceReport report) => this.Category.ToString().Equals(report.Category))
			});
		}
	}
}
