using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Transport
{
	public abstract class AcceptedDomainCollection : IEnumerable<AcceptedDomain>, IEnumerable
	{
		public AcceptedDomain Find(RoutingAddress smtpAddress)
		{
			return this.Find(smtpAddress.DomainPart);
		}

		public abstract AcceptedDomain Find(string domainName);

		public abstract IEnumerator<AcceptedDomain> GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
