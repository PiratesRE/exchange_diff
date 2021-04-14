using System;
using System.Management.Automation;
using Microsoft.Exchange.Management.ReportingTask;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[OutputType(new Type[]
	{
		typeof(HistoricalSearch)
	})]
	[Cmdlet("Get", "HistoricalSearch")]
	public sealed class GetHistoricalSearch : HistoricalSearchBaseTask
	{
		public GetHistoricalSearch() : base("GetHistoricalSeach", "Microsoft.Exchange.Hygiene.ManagementHelper.HistoricalSearch.GetHistoricalSearchHelper")
		{
		}

		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
		public Guid? JobId { get; set; }

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

		private const string ComponentName = "GetHistoricalSeach";
	}
}
