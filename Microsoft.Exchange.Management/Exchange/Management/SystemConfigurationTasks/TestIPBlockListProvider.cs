using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Test", "IPBlockListProvider", SupportsShouldProcess = true)]
	public class TestIPBlockListProvider : TestIPListProvider<IPBlockListProviderIdParameter, IPBlockListProvider>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageTestIPBlockListProvider(this.Identity.ToString(), base.IPAddress.ToString());
			}
		}
	}
}
