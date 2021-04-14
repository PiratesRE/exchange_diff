using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "AuthRedirect", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class SetAuthRedirect : SetSystemConfigurationObjectTask<AuthRedirectIdParameter, AuthRedirect>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetAuthRedirect(this.Identity.RawIdentity);
			}
		}
	}
}
