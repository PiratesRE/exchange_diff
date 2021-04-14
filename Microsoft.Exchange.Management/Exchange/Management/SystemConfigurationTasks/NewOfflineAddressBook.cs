using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "OfflineAddressBook", SupportsShouldProcess = true)]
	public sealed class NewOfflineAddressBook : NewOfflineAddressBookInternal
	{
		[Parameter(Mandatory = true)]
		public override AddressBookBaseIdParameter[] AddressLists
		{
			get
			{
				return base.AddressLists;
			}
			set
			{
				base.AddressLists = value;
			}
		}
	}
}
