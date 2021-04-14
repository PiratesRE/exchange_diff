using System;
using System.Collections.Generic;
using Microsoft.Exchange.HttpProxy.Routing;
using Microsoft.Exchange.HttpProxy.Routing.RoutingDestinations;
using Microsoft.Exchange.HttpProxy.Routing.RoutingEntries;
using Microsoft.Exchange.HttpProxy.Routing.RoutingKeys;

namespace Microsoft.Exchange.HttpProxy.RouteSelector
{
	internal class ServerLocator : IServerLocator
	{
		public ServerLocator(ISharedCacheClient anchorMailboxCacheClient, ISharedCacheClient mailboxServerCacheClient, IRoutingLookupFactory locatorServiceLookupFactory)
		{
			if (anchorMailboxCacheClient == null)
			{
				throw new ArgumentNullException("anchorMailboxCacheClient");
			}
			if (mailboxServerCacheClient == null)
			{
				throw new ArgumentNullException("mailboxServerCacheClient");
			}
			if (locatorServiceLookupFactory == null)
			{
				throw new ArgumentNullException("locatorServiceLookupFactory");
			}
			this.anchorMailboxCacheClient = anchorMailboxCacheClient;
			this.mailboxServerCacheClient = mailboxServerCacheClient;
			this.locatorServiceLookupFactory = locatorServiceLookupFactory;
			this.sharedCacheLookupFactory = new SharedCacheLookupFactory(this.anchorMailboxCacheClient, this.mailboxServerCacheClient);
		}

		public virtual ServerLocatorReturn LocateServer(IRoutingKey[] keys, IRouteSelectorDiagnostics diagnostics)
		{
			IRoutingKey successKey = null;
			IRoutingEntry routingEntry = null;
			IList<IRoutingEntry> routingEntries = null;
			string value;
			if (keys != null)
			{
				foreach (IRoutingKey routingKey in keys)
				{
					if (routingKey != null)
					{
						if (this.ResolveRoute(routingKey, diagnostics, out routingEntry, out routingEntries))
						{
							successKey = routingKey;
							break;
						}
					}
					else
					{
						value = "[ServerLocator::LocateServer]: null key value in collection.";
						diagnostics.AddErrorInfo(value);
					}
				}
			}
			else
			{
				value = "[ServerLocator::LocateServer]: null keys collection.";
				diagnostics.AddErrorInfo(value);
			}
			SuccessfulDatabaseGuidRoutingEntry successfulDatabaseGuidRoutingEntry = routingEntry as SuccessfulDatabaseGuidRoutingEntry;
			if (successfulDatabaseGuidRoutingEntry != null)
			{
				ServerRoutingDestination serverRoutingDestination = successfulDatabaseGuidRoutingEntry.Destination as ServerRoutingDestination;
				return new ServerLocatorReturn(serverRoutingDestination.Fqdn, serverRoutingDestination.Version, successKey, routingEntries);
			}
			SuccessfulServerRoutingEntry successfulServerRoutingEntry = routingEntry as SuccessfulServerRoutingEntry;
			if (successfulServerRoutingEntry != null)
			{
				ServerRoutingDestination serverRoutingDestination2 = successfulServerRoutingEntry.Destination as ServerRoutingDestination;
				return new ServerLocatorReturn(serverRoutingDestination2.Fqdn, serverRoutingDestination2.Version, successKey, routingEntries);
			}
			value = string.Format("[ServerLocator::LocateServer]: RoutingEntry returned was of an unexpected type: {0}", (routingEntry != null) ? routingEntry.GetType() : null);
			diagnostics.AddErrorInfo(value);
			return null;
		}

		internal static bool IsAnchorMailboxCacheKey(IRoutingKey key)
		{
			switch (key.RoutingItemType)
			{
			case RoutingItemType.ArchiveSmtp:
			case RoutingItemType.MailboxGuid:
			case RoutingItemType.Smtp:
			case RoutingItemType.ExternalDirectoryObjectId:
			case RoutingItemType.LiveIdMemberName:
				return true;
			}
			return false;
		}

		internal static bool IsMailboxServerCacheKey(IRoutingKey key)
		{
			return key.RoutingItemType == RoutingItemType.DatabaseGuid;
		}

		internal static string OrganizationNameForLogging(IRoutingKey key)
		{
			RoutingItemType routingItemType = key.RoutingItemType;
			if (routingItemType != RoutingItemType.ArchiveSmtp)
			{
				switch (routingItemType)
				{
				case RoutingItemType.MailboxGuid:
				{
					MailboxGuidRoutingKey mailboxGuidRoutingKey = key as MailboxGuidRoutingKey;
					return mailboxGuidRoutingKey.TenantDomain;
				}
				case RoutingItemType.Smtp:
				{
					SmtpRoutingKey smtpRoutingKey = key as SmtpRoutingKey;
					return smtpRoutingKey.SmtpAddress.Domain;
				}
				case RoutingItemType.LiveIdMemberName:
				{
					LiveIdMemberNameRoutingKey liveIdMemberNameRoutingKey = key as LiveIdMemberNameRoutingKey;
					return liveIdMemberNameRoutingKey.OrganizationDomain;
				}
				}
				return string.Empty;
			}
			ArchiveSmtpRoutingKey archiveSmtpRoutingKey = key as ArchiveSmtpRoutingKey;
			return archiveSmtpRoutingKey.SmtpAddress.Domain;
		}

		internal bool ResolveRoute(IRoutingKey routingKey, IRouteSelectorDiagnostics diagnostics, out IRoutingEntry foundEntry, out IList<IRoutingEntry> routingEntries)
		{
			routingEntries = new List<IRoutingEntry>();
			for (int i = 0; i < 5; i++)
			{
				diagnostics.ProcessRoutingKey(routingKey);
				IRoutingEntry routingEntry = ServerLocator.GetRoutingEntry(routingKey, this.sharedCacheLookupFactory, diagnostics);
				if (routingEntry != null)
				{
					diagnostics.ProcessRoutingEntry(routingEntry);
				}
				if (routingEntry == null)
				{
					routingEntry = ServerLocator.GetRoutingEntry(routingKey, this.locatorServiceLookupFactory, diagnostics);
					if (routingEntry != null)
					{
						if (ServerLocator.IsMailboxServerCacheKey(routingEntry.Key) && !this.mailboxServerCacheClient.AddEntry(routingEntry))
						{
							string value = string.Format("[ServerLocator::ResolveRoute]: RoutingEntry returned from MBLS could not be added to MailboxServer cache: {0}", routingEntry);
							diagnostics.AddErrorInfo(value);
						}
						if (ServerLocator.IsAnchorMailboxCacheKey(routingEntry.Key) && !this.anchorMailboxCacheClient.AddEntry(routingEntry))
						{
							string value2 = string.Format("[ServerLocator::ResolveRoute]: RoutingEntry returned from MBLS could not be added to AnchorMailbox cache: {0}", routingEntry);
							diagnostics.AddErrorInfo(value2);
						}
					}
					else
					{
						string value3 = string.Format("[ServerLocator::ResolveRoute]: MBLS could not find entry for key {0}", routingKey);
						diagnostics.AddErrorInfo(value3);
					}
				}
				if (routingEntry == null)
				{
					break;
				}
				routingEntries.Add(routingEntry);
				string text = ServerLocator.OrganizationNameForLogging(routingEntry.Key);
				if (!string.IsNullOrEmpty(text))
				{
					diagnostics.SetOrganization(text);
				}
				IRoutingKey routingKey2 = routingEntry.Destination.CreateRoutingKey();
				if (routingKey2 == null)
				{
					foundEntry = routingEntry;
					return true;
				}
				routingKey = routingKey2;
			}
			foundEntry = null;
			return false;
		}

		private static IRoutingEntry GetRoutingEntry(IRoutingKey routingKey, IRoutingLookupFactory factory, IRoutingDiagnostics diagnostics)
		{
			IRoutingLookup lookupForType = factory.GetLookupForType(routingKey.RoutingItemType);
			IRoutingEntry routingEntry = null;
			if (lookupForType != null)
			{
				routingEntry = lookupForType.GetRoutingEntry(routingKey, diagnostics);
				if (routingEntry != null && (routingEntry.Destination.RoutingItemType == RoutingItemType.Error || routingEntry.Destination.RoutingItemType == RoutingItemType.Unknown))
				{
					routingEntry = null;
				}
			}
			return routingEntry;
		}

		private ISharedCacheClient anchorMailboxCacheClient;

		private ISharedCacheClient mailboxServerCacheClient;

		private SharedCacheLookupFactory sharedCacheLookupFactory;

		private IRoutingLookupFactory locatorServiceLookupFactory;
	}
}
