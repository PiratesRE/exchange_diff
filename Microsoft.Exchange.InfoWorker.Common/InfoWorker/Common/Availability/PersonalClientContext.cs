using System;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Sharing;
using Microsoft.Exchange.Net.WSSecurity;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class PersonalClientContext : ExternalClientContext
	{
		internal PersonalClientContext(SmtpAddress emailAddress, SmtpAddress externalId, WSSecurityHeader wsSecurityHeader, SharingSecurityHeader sharingSecurityHeader, IBudget budget, ExTimeZone timeZone, CultureInfo clientCulture, string messageId) : base(emailAddress, wsSecurityHeader, budget, timeZone, clientCulture, messageId)
		{
			this.ExternalId = externalId;
			this.SharingSecurityHeader = sharingSecurityHeader;
		}

		public override ProxyAuthenticator CreateInternalProxyAuthenticator()
		{
			return ProxyAuthenticator.Create(base.WSSecurityHeader, this.SharingSecurityHeader, base.MessageId);
		}

		public SmtpAddress ExternalId { get; private set; }

		public SharingSecurityHeader SharingSecurityHeader { get; private set; }

		public override string IdentityForFilteredTracing
		{
			get
			{
				return this.ExternalId.ToString();
			}
		}

		public override string ToString()
		{
			return "<personal>" + this.ExternalId;
		}
	}
}
