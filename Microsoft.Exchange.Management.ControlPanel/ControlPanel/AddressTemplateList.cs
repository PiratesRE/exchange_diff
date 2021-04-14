using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[CollectionDataContract]
	public class AddressTemplateList : List<EmailAddressItem>
	{
		public AddressTemplateList()
		{
		}

		public AddressTemplateList(ProxyAddressTemplateCollection templateCollection) : base(templateCollection.Count)
		{
			foreach (ProxyAddressBase address in templateCollection)
			{
				base.Add(new EmailAddressItem(address));
			}
		}
	}
}
