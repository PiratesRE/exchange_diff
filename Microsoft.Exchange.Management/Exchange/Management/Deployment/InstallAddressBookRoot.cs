using System;
using System.Management.Automation;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Install", "AddressBookRoot")]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class InstallAddressBookRoot : InstallAddressListBase
	{
		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			if (!base.IsContainerExisted)
			{
				base.PostExchange(this.DataObject.Id);
			}
		}
	}
}
