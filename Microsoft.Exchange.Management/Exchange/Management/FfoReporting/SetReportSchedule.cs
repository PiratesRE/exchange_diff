using System;
using System.Management.Automation;
using Microsoft.Exchange.Management.ReportingTask;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[OutputType(new Type[]
	{
		typeof(ReportSchedule)
	})]
	[Cmdlet("Set", "ReportSchedule")]
	public sealed class SetReportSchedule : ReportScheduleBaseTask
	{
		public SetReportSchedule() : base("SetReportSchedule", "Microsoft.Exchange.Hygiene.ManagementHelper.ReportSchedule.SetReportScheduleHelper")
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

		private const string ComponentName = "SetReportSchedule";
	}
}
