using System;
using System.Management.Automation;
using Microsoft.Exchange.Management.ReportingTask;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[OutputType(new Type[]
	{
		typeof(ReportSchedule)
	})]
	[Cmdlet("Get", "ReportScheduleHistory")]
	public sealed class GetReportScheduleHistory : ReportScheduleBaseTask
	{
		public GetReportScheduleHistory() : base("GetReportScheduleHistory", "Microsoft.Exchange.Hygiene.ManagementHelper.ReportSchedule.GetReportScheduleHistoryHelper")
		{
		}

		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
		public Guid? ScheduleID { get; set; }

		protected override void InternalValidate()
		{
			try
			{
				base.InternalValidate();
			}
			catch (InvalidExpressionException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidArgument, null);
			}
			catch (Exception exception2)
			{
				base.WriteError(exception2, ErrorCategory.InvalidOperation, null);
			}
		}

		private const string ComponentName = "GetReportScheduleHistory";
	}
}
