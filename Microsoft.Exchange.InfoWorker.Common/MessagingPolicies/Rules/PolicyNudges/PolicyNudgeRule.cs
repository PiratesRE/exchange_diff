using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.MessagingPolicies.Rules.PolicyNudges
{
	internal sealed class PolicyNudgeRule : IVersionedItem
	{
		public PolicyNudgeRule(string etrXml, string id, DateTime version, RuleState state, DateTime? activationDate, DateTime? expiryDate)
		{
			this.etrXml = etrXml;
			this.ID = id;
			this.Version = version;
			this.state = state;
			this.activationDate = activationDate;
			this.expiryDate = expiryDate;
			this.IsPnrXmlValid = true;
		}

		public string GetRuleXml(string locale, ETRToPNRTranslator.IMessageStrings messageStrings, ETRToPNRTranslator.IDistributionListResolver distributionListResolver, ETRToPNRTranslator.IDataClassificationResolver dataClassificationResolver, bool needFullPnrXml, out PolicyNudgeRule.References references)
		{
			if (this.localeToPnrMap == null)
			{
				lock (this.etrXml)
				{
					if (this.localeToPnrMap == null)
					{
						this.localeToPnrMap = new Dictionary<string, PolicyNudgeRule.CacheEntry>();
					}
				}
			}
			PolicyNudgeRule.CacheEntry cacheEntry;
			if (this.localeToPnrMap.TryGetValue(locale, out cacheEntry))
			{
				references = new PolicyNudgeRule.References(cacheEntry.usedMessages, cacheEntry.usedDistributionLists.SelectMany((string dl) => distributionListResolver.Get(dl)));
				if (!needFullPnrXml)
				{
					return cacheEntry.PnrXml;
				}
				return cacheEntry.FullPnrXml;
			}
			else
			{
				List<PolicyTipMessage> usedMessagesList = new List<PolicyTipMessage>();
				List<string> usedDistributionListsList = new List<string>();
				ETRToPNRTranslator etrtoPNRTranslator = new ETRToPNRTranslator(this.etrXml, new ETRToPNRTranslator.MessageStringCallbackImpl(messageStrings.OutlookCultureTag, (ETRToPNRTranslator.OutlookActionTypes action) => PolicyNudgeRule.Track<PolicyTipMessage>(messageStrings.Get(action), usedMessagesList), () => PolicyNudgeRule.Track<PolicyTipMessage>(messageStrings.Url, usedMessagesList)), new ETRToPNRTranslator.DistributionListResolverCallbackImpl(delegate(string distributionList)
				{
					PolicyNudgeRule.Track<string>(distributionList, usedDistributionListsList);
					return null;
				}, (string distributionList) => distributionListResolver.IsMemberOf(distributionList)), dataClassificationResolver);
				string pnrXml = etrtoPNRTranslator.PnrXml;
				this.IsPnrXmlValid = !string.IsNullOrEmpty(pnrXml);
				string fullPnrXml = etrtoPNRTranslator.FullPnrXml;
				lock (this.localeToPnrMap)
				{
					if (!this.localeToPnrMap.ContainsKey(locale))
					{
						this.localeToPnrMap.Add(locale, new PolicyNudgeRule.CacheEntry
						{
							PnrXml = pnrXml,
							FullPnrXml = fullPnrXml,
							usedMessages = usedMessagesList.ToArray(),
							usedDistributionLists = usedDistributionListsList.ToArray()
						});
					}
				}
				references = new PolicyNudgeRule.References(usedMessagesList, usedDistributionListsList.SelectMany((string dl) => distributionListResolver.Get(dl)));
				if (!needFullPnrXml)
				{
					return pnrXml;
				}
				return fullPnrXml;
			}
		}

		private static T Track<T>(T obj, IList<T> usedObjs)
		{
			usedObjs.Add(obj);
			return obj;
		}

		public string ID { get; private set; }

		public DateTime Version { get; private set; }

		public bool IsPnrXmlValid { get; private set; }

		public bool IsEnabled
		{
			get
			{
				return this.state == RuleState.Enabled && (this.expiryDate == null || DateTime.UtcNow <= this.expiryDate.Value) && (this.activationDate == null || DateTime.UtcNow >= this.activationDate.Value);
			}
		}

		private readonly string etrXml;

		private RuleState state;

		private DateTime? activationDate;

		private DateTime? expiryDate;

		private Dictionary<string, PolicyNudgeRule.CacheEntry> localeToPnrMap;

		private class CacheEntry
		{
			internal string PnrXml;

			internal string FullPnrXml;

			internal PolicyTipMessage[] usedMessages;

			internal string[] usedDistributionLists;
		}

		internal sealed class References
		{
			internal References(IEnumerable<PolicyTipMessage> messages, IEnumerable<IVersionedItem> distributionLists)
			{
				this.Messages = messages;
				this.DistributionLists = distributionLists;
			}

			internal readonly IEnumerable<PolicyTipMessage> Messages;

			internal readonly IEnumerable<IVersionedItem> DistributionLists;
		}
	}
}
