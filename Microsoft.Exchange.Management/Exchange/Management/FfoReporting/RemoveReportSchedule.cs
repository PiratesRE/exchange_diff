using System;
using System.Management.Automation;
using Microsoft.Exchange.Management.ReportingTask;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[Cmdlet("Remove", "ReportSchedule")]
	public sealed class RemoveReportSchedule : ReportScheduleBaseTask
	{
		public RemoveReportSchedule() : base("RemoveReportSchedule", "Microsoft.Exchange.Hygiene.ManagementHelper.ReportSchedule.RemoveReportScheduleHelper")
		{
		}

		[Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
		public Guid ScheduleID { get; set; }

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

		private const string ComponentName = "RemoveReportSchedule";
	}
}
