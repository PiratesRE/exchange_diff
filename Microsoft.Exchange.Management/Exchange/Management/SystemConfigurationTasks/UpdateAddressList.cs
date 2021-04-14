﻿using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("update", "AddressList", SupportsShouldProcess = true)]
	public sealed class UpdateAddressList : UpdateAddressBookBase<AddressListIdParameter>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageUpdateAddressList(this.Identity.ToString());
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return AddressList.FromDataObject((AddressBookBase)dataObject);
		}
	}
}
