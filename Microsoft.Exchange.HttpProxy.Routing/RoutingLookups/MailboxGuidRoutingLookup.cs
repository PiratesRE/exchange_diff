using System;
using Microsoft.Exchange.HttpProxy.Routing.Providers;
using Microsoft.Exchange.HttpProxy.Routing.RoutingKeys;

namespace Microsoft.Exchange.HttpProxy.Routing.RoutingLookups
{
	internal class MailboxGuidRoutingLookup : MailboxRoutingLookupBase<MailboxGuidRoutingKey>
	{
		public MailboxGuidRoutingLookup(IUserProvider userProvider) : base(userProvider)
		{
		}

		protected override User FindUser(MailboxGuidRoutingKey mailboxGuidRoutingKey, IRoutingDiagnostics diagnostics)
		{
			return base.UserProvider.FindByExchangeGuidIncludingAlternate(mailboxGuidRoutingKey.MailboxGuid, mailboxGuidRoutingKey.TenantDomain, diagnostics);
		}

		protected override void SelectDatabaseGuidResourceForest(MailboxGuidRoutingKey mailboxGuidRoutingKey, User user, out Guid? databaseGuid, out string resourceForest)
		{
			if (mailboxGuidRoutingKey.MailboxGuid == user.ArchiveGuid)
			{
				databaseGuid = user.ArchiveDatabaseGuid;
				resourceForest = user.ArchiveDatabaseResourceForest;
				return;
			}
			databaseGuid = user.DatabaseGuid;
			resourceForest = user.DatabaseResourceForest;
		}

		protected override string GetDomainName(MailboxGuidRoutingKey routingKey)
		{
			return routingKey.TenantDomain;
		}
	}
}
