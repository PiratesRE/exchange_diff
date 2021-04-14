using System;
using Microsoft.Exchange.HttpProxy.Routing.Providers;
using Microsoft.Exchange.HttpProxy.Routing.RoutingKeys;

namespace Microsoft.Exchange.HttpProxy.Routing.RoutingLookups
{
	internal class ArchiveSmtpRoutingLookup : MailboxRoutingLookupBase<ArchiveSmtpRoutingKey>
	{
		public ArchiveSmtpRoutingLookup(IUserProvider userProvider) : base(userProvider)
		{
		}

		protected override User FindUser(ArchiveSmtpRoutingKey archiveSmtpRoutingKey, IRoutingDiagnostics diagnostics)
		{
			return base.UserProvider.FindBySmtpAddress(archiveSmtpRoutingKey.SmtpAddress, diagnostics);
		}

		protected override void SelectDatabaseGuidResourceForest(ArchiveSmtpRoutingKey mailboxGuidRoutingKey, User user, out Guid? databaseGuid, out string resourceForest)
		{
			databaseGuid = user.ArchiveDatabaseGuid;
			resourceForest = user.ArchiveDatabaseResourceForest;
		}

		protected override string GetDomainName(ArchiveSmtpRoutingKey routingKey)
		{
			return routingKey.SmtpAddress.Domain;
		}
	}
}
