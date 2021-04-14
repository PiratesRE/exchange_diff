using System;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net.WSSecurity;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class OrganizationalClientContext : ExternalClientContext
	{
		internal OrganizationalClientContext(SmtpAddress emailAddress, string requestorDomain, WSSecurityHeader wsSecurityHeader, IBudget budget, ExTimeZone timeZone, CultureInfo clientCulture, string messageId) : base(emailAddress, wsSecurityHeader, budget, timeZone, clientCulture, messageId)
		{
			this.RequestorDomain = requestorDomain;
		}

		public override ProxyAuthenticator CreateInternalProxyAuthenticator()
		{
			return ProxyAuthenticator.Create(base.WSSecurityHeader, null, base.MessageId);
		}

		public string RequestorDomain { get; private set; }

		public override string ToString()
		{
			return "<organizational>" + this.RequestorDomain;
		}
	}
}
