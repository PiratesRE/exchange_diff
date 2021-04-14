using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class SubscriptionCache
	{
		public SubscriptionCache()
		{
			this.cacheEntries = new List<SubscriptionCacheEntry>(128);
		}

		public int CacheLength
		{
			get
			{
				return this.cacheEntries.Count;
			}
		}

		internal List<SubscriptionCacheEntry> CacheEntries
		{
			get
			{
				return this.cacheEntries;
			}
		}

		public static SubscriptionCache GetCache(UserContext userContext)
		{
			return SubscriptionCache.GetCache(userContext, true);
		}

		public static SubscriptionCache GetCache(UserContext userContext, bool useInMemCache)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			lock (SubscriptionCache.syncRoot)
			{
				if (!userContext.CanActAsOwner)
				{
					userContext.SubscriptionCache = new SubscriptionCache();
				}
				else if (userContext.SubscriptionCache == null || !useInMemCache)
				{
					List<PimAggregationSubscription> allSendAsSubscriptions = SubscriptionManager.GetAllSendAsSubscriptions(userContext.MailboxSession);
					userContext.SubscriptionCache = new SubscriptionCache();
					short num = 1;
					foreach (PimAggregationSubscription pimAggregationSubscription in allSendAsSubscriptions)
					{
						string address = Utilities.DecodeIDNDomain(pimAggregationSubscription.UserEmailAddress);
						if (128 <= num)
						{
							break;
						}
						SubscriptionCacheEntry item = new SubscriptionCacheEntry(pimAggregationSubscription.SubscriptionGuid, address, pimAggregationSubscription.UserDisplayName, pimAggregationSubscription.InstanceKey, userContext.MailboxSession.PreferedCulture);
						userContext.SubscriptionCache.cacheEntries.Add(item);
						num += 1;
					}
					userContext.SubscriptionCache.cacheEntries.Sort();
				}
			}
			return userContext.SubscriptionCache;
		}

		public void RenderToJavascript(TextWriter writer)
		{
			lock (SubscriptionCache.syncRoot)
			{
				for (int i = 0; i < this.cacheEntries.Count; i++)
				{
					if (i > 0)
					{
						writer.Write(",");
					}
					this.cacheEntries[i].RenderToJavascript(writer);
				}
			}
		}

		public int Add(SubscriptionCacheEntry entry)
		{
			if (128 == this.cacheEntries.Count)
			{
				return -1;
			}
			lock (SubscriptionCache.syncRoot)
			{
				for (int i = 0; i < this.cacheEntries.Count; i++)
				{
					int num = this.cacheEntries[i].CompareTo(entry);
					if (num == 0)
					{
						this.cacheEntries[i] = entry;
						return i;
					}
					if (0 < num)
					{
						this.cacheEntries.Insert(i, entry);
						return i;
					}
				}
				this.cacheEntries.Add(entry);
			}
			return this.cacheEntries.Count - 1;
		}

		public int Modify(SubscriptionCacheEntry entry, SendAsState sendAsState, AggregationStatus status)
		{
			int index = -1;
			this.ForFirstEntryByIdThenAddress(entry, delegate(int i)
			{
				if (!SubscriptionManager.IsValidForSendAs(sendAsState, status))
				{
					this.cacheEntries.RemoveAt(i);
					index = i;
					return;
				}
				if (this.cacheEntries[i].Id.Equals(entry.Id))
				{
					int num = this.cacheEntries[i].CompareTo(entry);
					if (num == 0 && entry.DisplayNameMatch(this.cacheEntries[i]))
					{
						index = -2;
						return;
					}
					if (0 < num)
					{
						while (0 < i)
						{
							if (0 >= this.cacheEntries[i - 1].CompareTo(entry))
							{
								break;
							}
							this.cacheEntries[i] = this.cacheEntries[--i];
						}
					}
					else if (0 > num)
					{
						while (this.cacheEntries.Count - 1 > i && 0 > this.cacheEntries[i + 1].CompareTo(entry))
						{
							this.cacheEntries[i] = this.cacheEntries[++i];
						}
					}
				}
				this.cacheEntries[i] = entry;
				index = i;
			});
			if (-1 == index && SubscriptionManager.IsValidForSendAs(sendAsState, status))
			{
				index = this.Add(entry);
			}
			return index;
		}

		public int Delete(byte[] instanceKey)
		{
			lock (SubscriptionCache.syncRoot)
			{
				for (int i = 0; i < this.cacheEntries.Count; i++)
				{
					if (this.cacheEntries[i].MatchOnInstanceKey(instanceKey))
					{
						this.cacheEntries.RemoveAt(i);
						return i;
					}
				}
			}
			return -1;
		}

		public bool TryGetEntry(Participant fromParticipant, out SubscriptionCacheEntry subscriptionCacheEntry)
		{
			SubscriptionCacheEntry subscriptionCacheEntry2 = new SubscriptionCacheEntry(Guid.Empty, fromParticipant.EmailAddress, fromParticipant.DisplayName, null, CultureInfo.InvariantCulture);
			lock (SubscriptionCache.syncRoot)
			{
				foreach (SubscriptionCacheEntry subscriptionCacheEntry3 in this.cacheEntries)
				{
					if (subscriptionCacheEntry3.CompareTo(subscriptionCacheEntry2) == 0)
					{
						subscriptionCacheEntry = subscriptionCacheEntry3;
						return true;
					}
				}
			}
			subscriptionCacheEntry = subscriptionCacheEntry2;
			return false;
		}

		public bool TryGetEntry(Guid subscriptionId, out SubscriptionCacheEntry subscriptionCacheEntry)
		{
			lock (SubscriptionCache.syncRoot)
			{
				foreach (SubscriptionCacheEntry subscriptionCacheEntry2 in this.cacheEntries)
				{
					if (subscriptionId.Equals(subscriptionCacheEntry2.Id))
					{
						subscriptionCacheEntry = subscriptionCacheEntry2;
						return true;
					}
				}
			}
			subscriptionCacheEntry = new SubscriptionCacheEntry(Guid.Empty, string.Empty, string.Empty, null, CultureInfo.InvariantCulture);
			return false;
		}

		public bool TryGetSendAsDefaultEntry(SendAddressDefaultSetting sendAddressDefaultSetting, out SubscriptionCacheEntry subscriptionCacheEntry)
		{
			subscriptionCacheEntry = new SubscriptionCacheEntry(Guid.Empty, string.Empty, string.Empty, null, CultureInfo.InvariantCulture);
			Guid subscriptionId;
			return sendAddressDefaultSetting.TryGetSubscriptionSendAddressId(out subscriptionId) && this.TryGetEntry(subscriptionId, out subscriptionCacheEntry);
		}

		private int ForFirstEntryByIdThenAddress(SubscriptionCacheEntry entry, SubscriptionCache.CacheEntryProcessor processor)
		{
			lock (SubscriptionCache.syncRoot)
			{
				for (int i = 0; i < this.cacheEntries.Count; i++)
				{
					if (this.cacheEntries[i].Id.Equals(entry.Id))
					{
						processor(i);
						return i;
					}
				}
				for (int j = 0; j < this.cacheEntries.Count; j++)
				{
					int num = this.cacheEntries[j].CompareTo(entry);
					if (num == 0)
					{
						processor(j);
						return j;
					}
					if (0 < num)
					{
						break;
					}
				}
			}
			return -1;
		}

		private const short Size = 128;

		private static object syncRoot = new object();

		private List<SubscriptionCacheEntry> cacheEntries;

		private delegate void CacheEntryProcessor(int i);
	}
}
