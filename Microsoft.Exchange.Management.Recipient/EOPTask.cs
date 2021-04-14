using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.EventMessages;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public class EOPTask : Task
	{
		[Parameter(Mandatory = false)]
		public OrganizationIdParameter Organization { get; set; }

		protected void ThrowAndLogTaskError(Exception e)
		{
			ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_FfoReportingTaskFailure, new string[]
			{
				e.ToString()
			});
			this.ThrowTaskError(e);
		}

		protected void ThrowTaskError(Exception e)
		{
			base.ThrowTerminatingError(e, ErrorCategory.NotSpecified, null);
		}
	}
}
