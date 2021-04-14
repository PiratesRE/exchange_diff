using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.CommonHelpProvider
{
	internal sealed class HelpProviderCache : LazyLookupTimeoutCache<OrganizationId, HelpProviderCache.Item>
	{
		private HelpProviderCache() : base(HelpProviderCache.HelpProviderCacheBuckets.Value, HelpProviderCache.HelpProviderCacheBucketSize.Value, false, HelpProviderCache.HelpProviderCacheTimeToExpire.Value, HelpProviderCache.HelpProviderCacheTimeToLive.Value)
		{
		}

		internal static HelpProviderCache Instance
		{
			get
			{
				return HelpProviderCache.instance;
			}
		}

		protected override HelpProviderCache.Item CreateOnCacheMiss(OrganizationId key, ref bool shouldAdd)
		{
			shouldAdd = true;
			ExchangeAssistance exchangeAssistanceObjectFromAD = HelpProvider.GetExchangeAssistanceObjectFromAD(key);
			Uri privacyStatementUrl = null;
			Uri communityUrl = null;
			bool? privacyLinkDisplayEnabled = null;
			if (exchangeAssistanceObjectFromAD != null)
			{
				if (exchangeAssistanceObjectFromAD.CommunityLinkDisplayEnabled)
				{
					communityUrl = exchangeAssistanceObjectFromAD.CommunityURL;
				}
				privacyLinkDisplayEnabled = new bool?(exchangeAssistanceObjectFromAD.PrivacyLinkDisplayEnabled);
				if (exchangeAssistanceObjectFromAD.PrivacyLinkDisplayEnabled)
				{
					privacyStatementUrl = exchangeAssistanceObjectFromAD.PrivacyStatementURL;
				}
			}
			return new HelpProviderCache.Item(privacyStatementUrl, communityUrl, privacyLinkDisplayEnabled);
		}

		private static readonly IntAppSettingsEntry HelpProviderCacheBucketSize = new IntAppSettingsEntry("HelpProviderCacheBucketSize", 1024, ExTraceGlobals.CoreTracer);

		private static readonly IntAppSettingsEntry HelpProviderCacheBuckets = new IntAppSettingsEntry("HelpProviderCacheBuckets", 5, ExTraceGlobals.CoreTracer);

		private static readonly TimeSpanAppSettingsEntry HelpProviderCacheTimeToExpire = new TimeSpanAppSettingsEntry("HelpProviderCacheTimeToExpire", TimeSpanUnit.Minutes, TimeSpan.FromMinutes(15.0), ExTraceGlobals.CoreTracer);

		private static readonly TimeSpanAppSettingsEntry HelpProviderCacheTimeToLive = new TimeSpanAppSettingsEntry("HelpProviderCacheTimeToLive", TimeSpanUnit.Minutes, TimeSpan.FromMinutes(60.0), ExTraceGlobals.CoreTracer);

		private static HelpProviderCache instance = new HelpProviderCache();

		internal sealed class Item
		{
			public Item(Uri privacyStatementUrl, Uri communityUrl, bool? privacyLinkDisplayEnabled)
			{
				this.PrivacyStatementUrl = privacyStatementUrl;
				this.CommunityUrl = communityUrl;
				this.PrivacyLinkDisplayEnabled = privacyLinkDisplayEnabled;
			}

			public Uri CommunityUrl { get; private set; }

			public Uri PrivacyStatementUrl { get; private set; }

			public bool? PrivacyLinkDisplayEnabled { get; private set; }
		}
	}
}
