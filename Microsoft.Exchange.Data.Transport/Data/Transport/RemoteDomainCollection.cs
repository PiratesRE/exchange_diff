using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Transport
{
	public abstract class RemoteDomainCollection : IEnumerable<RemoteDomain>, IEnumerable
	{
		public RemoteDomain Find(RoutingAddress smtpAddress)
		{
			return this.Find(smtpAddress.DomainPart);
		}

		public abstract RemoteDomain Find(string domainName);

		public abstract IEnumerator<RemoteDomain> GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
