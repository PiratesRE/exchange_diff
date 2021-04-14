using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Mobility;

namespace Microsoft.Exchange.Management.Extension
{
	[Cmdlet("Disable", "App", SupportsShouldProcess = true, DefaultParameterSetName = "Identity", ConfirmImpact = ConfirmImpact.High)]
	public sealed class DisableApp : EnableDisableOWAExtensionBase
	{
		public DisableApp() : base(false)
		{
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageDisableOwaExtension(this.Identity.ToString(), this.mailboxOwner);
			}
		}
	}
}
