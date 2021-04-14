using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Mobility;

namespace Microsoft.Exchange.Management.Extension
{
	[Cmdlet("Enable", "App", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class EnableApp : EnableDisableOWAExtensionBase
	{
		public EnableApp() : base(true)
		{
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageEnableOwaExtension(this.Identity.ToString(), this.mailboxOwner);
			}
		}
	}
}
