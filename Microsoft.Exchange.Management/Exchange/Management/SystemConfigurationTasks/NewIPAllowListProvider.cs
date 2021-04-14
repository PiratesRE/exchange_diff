using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Add", "IPAllowListProvider", SupportsShouldProcess = true)]
	public sealed class NewIPAllowListProvider : NewIPListProvider<IPAllowListProvider>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageAddIPAllowListProvider(base.Name.ToString(), base.LookupDomain.ToString());
			}
		}
	}
}
