using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[CollectionDataContract]
	public class SmtpAddressList : EmailAddressList
	{
		public SmtpAddressList()
		{
		}

		public SmtpAddressList(ICollection<ProxyAddress> emailAddresses)
		{
			ICollection<ProxyAddress> emailAddresses2;
			if (emailAddresses != null)
			{
				emailAddresses2 = (from emailAddress in emailAddresses
				where emailAddress is SmtpProxyAddress
				select emailAddress).ToList<ProxyAddress>();
			}
			else
			{
				emailAddresses2 = null;
			}
			base..ctor(emailAddresses2);
		}
	}
}
