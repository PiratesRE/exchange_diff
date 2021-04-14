using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[CollectionDataContract]
	public class EmailAddressList : List<EmailAddressItem>
	{
		public static EmailAddressList FromProxyAddressCollection(ProxyAddressCollection emailAddresses)
		{
			return new EmailAddressList(emailAddresses);
		}

		public EmailAddressList()
		{
		}

		public EmailAddressList(ICollection<ProxyAddress> emailAddresses) : base((emailAddresses == null) ? 0 : emailAddresses.Count)
		{
			if (emailAddresses != null)
			{
				foreach (ProxyAddress address in emailAddresses)
				{
					base.Add(new EmailAddressItem(address));
				}
			}
		}
	}
}
