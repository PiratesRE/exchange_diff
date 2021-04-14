using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Add", "IPBlockListEntry", SupportsShouldProcess = true, DefaultParameterSetName = "IPRange")]
	public sealed class AddIPBlockListEntry : AddIPListEntry<IPBlockListEntry>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if ("IPAddress" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageAddIPBlockListEntryIPAddress(base.IPAddress.ToString());
				}
				return Strings.ConfirmationMessageAddIPBlockListEntryIPRange(base.IPRange.ToString());
			}
		}
	}
}
