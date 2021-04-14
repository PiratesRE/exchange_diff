using System;
using System.Management.Automation;
using Microsoft.Exchange.Management.ReportingTask;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[Cmdlet("Get", "ReportScheduleList")]
	[OutputType(new Type[]
	{
		typeof(ReportSchedule)
	})]
	public sealed class GetReportScheduleList : ReportScheduleBaseTask
	{
		public GetReportScheduleList() : base("GetReportScheduleList", "Microsoft.Exchange.Hygiene.ManagementHelper.ReportSchedule.GetReportScheduleListHelper")
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

		private const string ComponentName = "GetReportScheduleList";
	}
}
