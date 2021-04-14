using System;
using Microsoft.Exchange.HttpProxy.Routing.Providers;
using Microsoft.Exchange.HttpProxy.Routing.RoutingKeys;

namespace Microsoft.Exchange.HttpProxy.Routing.RoutingLookups
{
	internal class SmtpRoutingLookup : MailboxRoutingLookupBase<SmtpRoutingKey>
	{
		public SmtpRoutingLookup(IUserProvider userProvider) : base(userProvider)
		{
		}

		protected override User FindUser(SmtpRoutingKey smtpRoutingKey, IRoutingDiagnostics diagnostics)
		{
			return base.UserProvider.FindBySmtpAddress(smtpRoutingKey.SmtpAddress, diagnostics);
		}

		protected override string GetDomainName(SmtpRoutingKey routingKey)
		{
			return routingKey.SmtpAddress.Domain;
		}
	}
}
