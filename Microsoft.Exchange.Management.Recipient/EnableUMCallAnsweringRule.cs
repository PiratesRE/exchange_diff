using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Enable", "UMCallAnsweringRule", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class EnableUMCallAnsweringRule : EnableDisableUMCallAnsweringRuleBase
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageEnableCallAnsweringRule(this.Identity.ToString());
			}
		}

		public EnableUMCallAnsweringRule() : base(true)
		{
		}
	}
}
