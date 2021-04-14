using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Enable", "InboxRule", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class EnableInboxRule : EnableDisableInboxRuleBase
	{
		public EnableInboxRule() : base(true)
		{
		}
	}
}
