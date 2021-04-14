using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Disable", "UMCallAnsweringRule", SupportsShouldProcess = true, DefaultParameterSetName = "Identity", ConfirmImpact = ConfirmImpact.High)]
	public sealed class DisableUMCallAnsweringRule : EnableDisableUMCallAnsweringRuleBase
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageDisableCallAnsweringRule(this.Identity.ToString());
			}
		}

		public DisableUMCallAnsweringRule() : base(false)
		{
		}
	}
}
