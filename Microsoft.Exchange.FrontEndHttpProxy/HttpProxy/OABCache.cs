using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Autodiscover;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.HttpProxy
{
	internal sealed class OABCache
	{
		private OABCache()
		{
		}

		public static OABCache Instance
		{
			get
			{
				if (OABCache.instance == null)
				{
					lock (OABCache.staticLock)
					{
						if (OABCache.instance == null)
						{
							OABCache.instance = new OABCache();
						}
					}
				}
				return OABCache.instance;
			}
		}

		public OABCache.OABCacheEntry GetOABFromCacheOrAD(Guid exchangeObjectId, string userAcceptedDomain)
		{
			OABCache.OABCacheEntry oabcacheEntry = null;
			if (this.oabTimeoutCache.TryGetValue(exchangeObjectId, out oabcacheEntry))
			{
				return oabcacheEntry;
			}
			IConfigurationSession configurationSessionFromDomain = DirectoryHelper.GetConfigurationSessionFromDomain(userAcceptedDomain);
			OfflineAddressBook offlineAddressBook = configurationSessionFromDomain.FindByExchangeObjectId<OfflineAddressBook>(exchangeObjectId);
			if (offlineAddressBook == null)
			{
				throw new ADNoSuchObjectException(new LocalizedString(exchangeObjectId.ToString()));
			}
			oabcacheEntry = new OABCache.OABCacheEntry(offlineAddressBook);
			this.oabTimeoutCache.TryInsertAbsolute(exchangeObjectId, oabcacheEntry, OABCache.cacheTimeToLive.Value);
			return oabcacheEntry;
		}

		private static TimeSpanAppSettingsEntry cacheTimeToLive = new TimeSpanAppSettingsEntry("OabCacheTimeToLive", TimeSpanUnit.Seconds, TimeSpan.FromMinutes(10.0), ExTraceGlobals.FrameworkTracer);

		private static IntAppSettingsEntry cacheBucketSize = new IntAppSettingsEntry("OabCacheMaximumBucketSize", 1000, ExTraceGlobals.FrameworkTracer);

		private static OABCache instance;

		private static object staticLock = new object();

		private ExactTimeoutCache<Guid, OABCache.OABCacheEntry> oabTimeoutCache = new ExactTimeoutCache<Guid, OABCache.OABCacheEntry>(null, null, null, OABCache.cacheBucketSize.Value, false);

		internal sealed class OABCacheEntry
		{
			internal OABCacheEntry(OfflineAddressBook oab)
			{
				this.exchangeVersion = oab.ExchangeVersion;
				this.virtualDirectories = oab.VirtualDirectories;
				this.globalWebDistributionEnabled = oab.GlobalWebDistributionEnabled;
				this.generatingMailbox = oab.GeneratingMailbox;
				this.shadowMailboxDistributionEnabled = oab.ShadowMailboxDistributionEnabled;
			}

			internal ExchangeObjectVersion ExchangeVersion
			{
				get
				{
					return this.exchangeVersion;
				}
			}

			internal MultiValuedProperty<ADObjectId> VirtualDirectories
			{
				get
				{
					return this.virtualDirectories;
				}
			}

			internal bool GlobalWebDistributionEnabled
			{
				get
				{
					return this.globalWebDistributionEnabled;
				}
			}

			internal bool ShadowMailboxDistributionEnabled
			{
				get
				{
					return this.shadowMailboxDistributionEnabled;
				}
			}

			internal ADObjectId GeneratingMailbox
			{
				get
				{
					return this.generatingMailbox;
				}
			}

			private readonly ExchangeObjectVersion exchangeVersion;

			private readonly MultiValuedProperty<ADObjectId> virtualDirectories;

			private readonly bool globalWebDistributionEnabled;

			private readonly bool shadowMailboxDistributionEnabled;

			private readonly ADObjectId generatingMailbox;
		}
	}
}
