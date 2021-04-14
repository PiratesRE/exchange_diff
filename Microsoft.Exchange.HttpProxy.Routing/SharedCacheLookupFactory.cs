using System;
using Microsoft.Exchange.HttpProxy.Routing.Providers;
using Microsoft.Exchange.HttpProxy.Routing.RoutingLookups;

namespace Microsoft.Exchange.HttpProxy.Routing
{
	internal class SharedCacheLookupFactory : IRoutingLookupFactory
	{
		public SharedCacheLookupFactory(ISharedCache mailboxCache, ISharedCache databaseCache)
		{
			this.databaseCache = databaseCache;
			this.mailboxCache = mailboxCache;
		}

		public IRoutingLookup GetLookupForType(RoutingItemType routingEntryType)
		{
			switch (routingEntryType)
			{
			case RoutingItemType.ArchiveSmtp:
				return new ArchiveSmtpSharedCacheLookup(this.mailboxCache);
			case RoutingItemType.DatabaseGuid:
				return new DatabaseGuidSharedCacheLookup(this.databaseCache);
			case RoutingItemType.MailboxGuid:
				return new MailboxGuidSharedCacheLookup(this.mailboxCache);
			case RoutingItemType.Server:
				return new ServerRoutingLookup();
			case RoutingItemType.Smtp:
				return new SmtpSharedCacheLookup(this.mailboxCache);
			case RoutingItemType.ExternalDirectoryObjectId:
				return new ExternalDirectoryObjectIdSharedCacheLookup(this.mailboxCache);
			case RoutingItemType.LiveIdMemberName:
				return new LiveIdMemberNameSharedCacheLookup(this.mailboxCache);
			}
			return null;
		}

		private readonly ISharedCache mailboxCache;

		private readonly ISharedCache databaseCache;
	}
}
