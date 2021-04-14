using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Aggregation;
using Microsoft.Exchange.Management.Mobility;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Rpc.Cache;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CacheDataProvider : IConfigDataProvider
	{
		public CacheDataProvider(SubscriptionCacheAction cacheAction, ExchangePrincipal userPrincipal)
		{
			SyncUtilities.ThrowIfArgumentNull("userPrincipal", userPrincipal);
			this.cacheAction = cacheAction;
			this.userPrincipal = userPrincipal;
		}

		public string Source
		{
			get
			{
				return "SubscriptionCacheDataProvider";
			}
		}

		public IConfigurable Read<T>(ObjectId identity) where T : IConfigurable, new()
		{
			return this.GetCacheForRead();
		}

		public IConfigurable[] Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy) where T : IConfigurable, new()
		{
			SubscriptionsCache cacheForRead = this.GetCacheForRead();
			return new IConfigurable[]
			{
				cacheForRead
			};
		}

		public IEnumerable<T> FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize) where T : IConfigurable, new()
		{
			IConfigurable cacheForRead = this.GetCacheForRead();
			return new T[]
			{
				(T)((object)cacheForRead)
			};
		}

		public void Save(IConfigurable instance)
		{
			SubscriptionsCache subscriptionsCache = (SubscriptionsCache)instance;
			this.TestUserCache(subscriptionsCache);
		}

		public void Delete(IConfigurable instance)
		{
			SubscriptionsCache subscriptionsCache = (SubscriptionsCache)instance;
			this.TestUserCache(subscriptionsCache);
		}

		private SubscriptionsCache GetCacheForRead()
		{
			SubscriptionsCache subscriptionsCache = new SubscriptionsCache();
			subscriptionsCache.SetIdentity(this.userPrincipal.ObjectId);
			if (this.cacheAction == SubscriptionCacheAction.None || this.cacheAction == SubscriptionCacheAction.Validate)
			{
				this.TestUserCache(subscriptionsCache);
			}
			return subscriptionsCache;
		}

		private void TestUserCache(SubscriptionsCache subscriptionsCache)
		{
			string serverFqdn = this.userPrincipal.MailboxInfo.Location.ServerFqdn;
			string primarySmtpAddress = this.userPrincipal.MailboxInfo.PrimarySmtpAddress.ToString();
			string failureReason;
			uint num;
			List<SubscriptionCacheObject> subscriptionCacheObjects;
			ObjectState objectState;
			if (!SubscriptionCacheClient.TryTestUserCache(serverFqdn, primarySmtpAddress, this.cacheAction, out failureReason, out num, out subscriptionCacheObjects, out objectState))
			{
				subscriptionsCache.FailureReason = failureReason;
				return;
			}
			uint num2 = num;
			if (num2 != 0U)
			{
				switch (num2)
				{
				case 268435456U:
					subscriptionsCache.FailureReason = Strings.CacheRpcServerFailed(serverFqdn, failureReason);
					break;
				case 268435457U:
					subscriptionsCache.FailureReason = Strings.CacheRpcServerStopped(serverFqdn);
					break;
				case 268435458U:
					subscriptionsCache.FailureReason = Strings.CacheRpcInvalidServerVersionIssue(serverFqdn);
					break;
				default:
					throw new InvalidObjectOperationException(Strings.InvalidCacheActionResult(num));
				}
			}
			subscriptionsCache.SubscriptionCacheObjects = subscriptionCacheObjects;
			subscriptionsCache.propertyBag.SetField(SimpleProviderObjectSchema.ObjectState, objectState);
			subscriptionsCache.propertyBag.ResetChangeTracking(SimpleProviderObjectSchema.ObjectState);
		}

		private SubscriptionCacheAction cacheAction;

		private ExchangePrincipal userPrincipal;
	}
}
