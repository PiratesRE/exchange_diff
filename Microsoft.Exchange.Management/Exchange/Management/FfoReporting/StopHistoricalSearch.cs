using System;
using System.Management.Automation;
using Microsoft.Exchange.Management.ReportingTask;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[Cmdlet("Stop", "HistoricalSearch")]
	public sealed class StopHistoricalSearch : HistoricalSearchBaseTask
	{
		public StopHistoricalSearch() : base("StopHistoricalSearch", "Microsoft.Exchange.Hygiene.ManagementHelper.HistoricalSearch.StopHistoricalSearchHelper")
		{
		}

		[Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
		public Guid JobId { get; set; }

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

		private const string ComponentName = "StopHistoricalSearch";
	}
}
