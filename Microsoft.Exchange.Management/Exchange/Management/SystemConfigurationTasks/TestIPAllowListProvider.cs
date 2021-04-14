using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Test", "IPAllowListProvider", SupportsShouldProcess = true)]
	public class TestIPAllowListProvider : TestIPListProvider<IPAllowListProviderIdParameter, IPAllowListProvider>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageTestIPAllowListProvider(this.Identity.ToString(), base.IPAddress.ToString());
			}
		}
	}
}
