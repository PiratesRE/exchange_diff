using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class SyncStorageProviderFactory
	{
		internal static void Register(ISyncStorageProvider provider, AggregationSubscriptionType subscriptionType)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			lock (SyncStorageProviderFactory.syncStorageProviders)
			{
				if (!SyncStorageProviderFactory.registrationAllowed)
				{
					throw new InvalidOperationException("Registration attempted when it was explicitly disabled by the consuming code");
				}
				if (!SyncStorageProviderFactory.syncStorageProviders.ContainsKey((int)subscriptionType))
				{
					if (!Enum.IsDefined(typeof(AggregationSubscriptionType), subscriptionType))
					{
						throw new ArgumentOutOfRangeException("subscriptionType");
					}
					SyncStorageProviderFactory.syncStorageProviders[(int)subscriptionType] = provider;
				}
				else if (SyncStorageProviderFactory.syncStorageProviders[(int)subscriptionType].GetType() != provider.GetType())
				{
					throw new InvalidOperationException("Already a different kind of sync storage provider is registered for this aggregation type.");
				}
			}
		}

		internal static void Unregister(AggregationSubscriptionType subscriptionType)
		{
			lock (SyncStorageProviderFactory.syncStorageProviders)
			{
				if (SyncStorageProviderFactory.syncStorageProviders.ContainsKey((int)subscriptionType))
				{
					SyncStorageProviderFactory.syncStorageProviders.Remove((int)subscriptionType);
				}
			}
		}

		internal static void DisableRegistration()
		{
			lock (SyncStorageProviderFactory.syncStorageProviders)
			{
				SyncStorageProviderFactory.registrationAllowed = false;
			}
		}

		internal static void EnableRegistration()
		{
			lock (SyncStorageProviderFactory.syncStorageProviders)
			{
				SyncStorageProviderFactory.registrationAllowed = true;
			}
		}

		internal static NativeSyncStorageProvider CreateNativeSyncStorageProvider(ISyncWorkerData subscription)
		{
			NativeSyncStorageProvider result;
			if (subscription.IsMigration || subscription.IsMirrored || SyncUtilities.IsContactSubscriptionType(subscription.SubscriptionType))
			{
				result = SyncStorageProviderFactory.XSOSyncStorageProvider;
			}
			else
			{
				result = SyncStorageProviderFactory.TransportSyncStorageProvider;
			}
			return result;
		}

		internal static ISyncStorageProvider CreateCloudSyncStorageProvider(ISyncWorkerData subscription)
		{
			ISyncStorageProvider result = null;
			lock (SyncStorageProviderFactory.syncStorageProviders)
			{
				if (SyncStorageProviderFactory.syncStorageProviders.ContainsKey((int)subscription.SubscriptionType))
				{
					result = SyncStorageProviderFactory.syncStorageProviders[(int)subscription.SubscriptionType];
				}
			}
			return result;
		}

		private static readonly TransportSyncStorageProvider TransportSyncStorageProvider = new TransportSyncStorageProvider();

		private static readonly XSOSyncStorageProvider XSOSyncStorageProvider = new XSOSyncStorageProvider();

		private static Dictionary<int, ISyncStorageProvider> syncStorageProviders = new Dictionary<int, ISyncStorageProvider>(Enum.GetNames(typeof(AggregationSubscriptionType)).Length);

		private static bool registrationAllowed = true;
	}
}
