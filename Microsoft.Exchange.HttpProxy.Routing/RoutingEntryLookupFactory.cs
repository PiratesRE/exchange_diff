using System;
using Microsoft.Exchange.HttpProxy.Routing.Providers;
using Microsoft.Exchange.HttpProxy.Routing.RoutingLookups;

namespace Microsoft.Exchange.HttpProxy.Routing
{
	internal class RoutingEntryLookupFactory : IRoutingLookupFactory
	{
		public RoutingEntryLookupFactory(IDatabaseLocationProvider databaseLocationProvider, IUserProvider userProvider)
		{
			if (databaseLocationProvider == null)
			{
				throw new ArgumentNullException("databaseLocationProvider");
			}
			if (userProvider == null)
			{
				throw new ArgumentNullException("userProvider");
			}
			this.databaseLocationProvider = databaseLocationProvider;
			this.userProvider = userProvider;
		}

		public IRoutingLookup GetLookupForType(RoutingItemType routingEntryType)
		{
			switch (routingEntryType)
			{
			case RoutingItemType.ArchiveSmtp:
				return new ArchiveSmtpRoutingLookup(this.userProvider);
			case RoutingItemType.DatabaseGuid:
				return new DatabaseGuidRoutingLookup(this.databaseLocationProvider);
			case RoutingItemType.MailboxGuid:
				return new MailboxGuidRoutingLookup(this.userProvider);
			case RoutingItemType.Server:
				return new ServerRoutingLookup();
			case RoutingItemType.Smtp:
				return new SmtpRoutingLookup(this.userProvider);
			case RoutingItemType.ExternalDirectoryObjectId:
				return new ExternalDirectoryObjectIdRoutingLookup(this.userProvider);
			case RoutingItemType.LiveIdMemberName:
				return new LiveIdMemberNameRoutingLookup(this.userProvider);
			}
			return null;
		}

		private readonly IUserProvider userProvider;

		private readonly IDatabaseLocationProvider databaseLocationProvider;
	}
}
