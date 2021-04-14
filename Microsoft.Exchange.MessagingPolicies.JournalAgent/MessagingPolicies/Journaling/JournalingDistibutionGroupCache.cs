using System;
using System.Collections.Generic;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;

namespace Microsoft.Exchange.MessagingPolicies.Journaling
{
	internal sealed class JournalingDistibutionGroupCache : LazyLookupTimeoutCache<string, JournalingDistributionGroupCacheItem>
	{
		protected override JournalingDistributionGroupCacheItem CreateOnCacheMiss(string key, ref bool shouldAdd)
		{
			if (this.adRecipientCache == null)
			{
				throw new ArgumentNullException("adRecipientCache");
			}
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			IADDistributionList group = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				SmtpProxyAddress proxyAddress = new SmtpProxyAddress(key, true);
				group = (this.adRecipientCache.ADSession.FindByProxyAddress(proxyAddress) as IADDistributionList);
			});
			if (!adoperationResult.Succeeded)
			{
				ExTraceGlobals.JournalingTracer.TraceDebug<Exception>((long)this.GetHashCode(), "invalid group object {0}", adoperationResult.Exception);
				shouldAdd = false;
				return null;
			}
			List<string> list = new List<string>();
			foreach (TransportMiniRecipient transportMiniRecipient in this.adRecipientCache.ExpandGroup<TransportMiniRecipient>(group))
			{
				list.Add(transportMiniRecipient[ADRecipientSchema.PrimarySmtpAddress].ToString());
			}
			return new JournalingDistributionGroupCacheItem(list);
		}

		public JournalingDistibutionGroupCache() : base(1, 100, false, TimeSpan.FromMinutes(20.0))
		{
		}

		public JournalingDistributionGroupCacheItem GetGroupCacheItem(IADRecipientCache recipientCache, string emailAddress)
		{
			JournalingDistributionGroupCacheItem result;
			lock (this.lockObj)
			{
				this.adRecipientCache = (IADRecipientCache<TransportMiniRecipient>)recipientCache;
				JournalingDistributionGroupCacheItem journalingDistributionGroupCacheItem = base.Get(emailAddress);
				this.adRecipientCache = null;
				result = journalingDistributionGroupCacheItem;
			}
			return result;
		}

		private IADRecipientCache<TransportMiniRecipient> adRecipientCache;

		private object lockObj = new object();
	}
}
