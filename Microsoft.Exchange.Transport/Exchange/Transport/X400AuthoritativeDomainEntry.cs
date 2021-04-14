using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Transport
{
	internal class X400AuthoritativeDomainEntry
	{
		public X400AuthoritativeDomainEntry(X400AuthoritativeDomain authoritativeDomain)
		{
			this.externalRelay = authoritativeDomain.X400ExternalRelay;
			this.domain = authoritativeDomain.X400DomainName;
		}

		public X400Domain Domain
		{
			get
			{
				return this.domain;
			}
		}

		public bool ExternalRelay
		{
			get
			{
				return this.externalRelay;
			}
		}

		private X400Domain domain;

		private bool externalRelay;
	}
}
