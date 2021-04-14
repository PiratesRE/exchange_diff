using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.InfoWorker.Common.OrganizationConfiguration;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.MessagingPolicies.Rules.PolicyNudges;

namespace Microsoft.Exchange.Services.Core.PolicyNudges
{
	internal class HasChangedVisitor15 : Visitor15
	{
		private HasChangedVisitor15()
		{
		}

		internal static bool HasChanged(PolicyNudges15 policyNudges, CachedOrganizationConfiguration serverConfig, ADObjectId senderADObjectId)
		{
			if (serverConfig == null || senderADObjectId == null)
			{
				throw new ArgumentNullException();
			}
			if (policyNudges == null)
			{
				return true;
			}
			HasChangedVisitor15 hasChangedVisitor = new HasChangedVisitor15();
			hasChangedVisitor.serverConfig = serverConfig;
			hasChangedVisitor.senderADObjectId = senderADObjectId;
			policyNudges.Accept(hasChangedVisitor);
			return hasChangedVisitor.hasChanged;
		}

		internal override void Visit(PolicyNudges15 policyNudges)
		{
			if (!this.EnsureValid(policyNudges.PolicyNudgeRules) || !this.EnsureValid(policyNudges.ClassificationItems))
			{
				return;
			}
			this.outlookCultureTag = policyNudges.OutlookLocale;
			this.exchangeLocale = PolicyNudgeConfigurationUtils.GetExchangeLocaleFromOutlookCultureTag(this.outlookCultureTag);
			if (policyNudges.ClassificationItems != null && !string.IsNullOrEmpty(policyNudges.ClassificationItems.EngineVersion))
			{
				this.canOutlookSupportFullPnrXml = PolicyNudgeConfigurationUtils.CanOutlookSupportFullPnrXml(policyNudges.ClassificationItems.EngineVersion);
			}
			if (!this.EnsureValid(this.exchangeLocale))
			{
				return;
			}
			policyNudges.PolicyNudgeRules.Accept(this);
			policyNudges.ClassificationItems.Accept(this);
		}

		internal override void Visit(PolicyNudgeRules15 policyNudgeRules)
		{
			if (this.hasChanged || !this.EnsureValid(policyNudgeRules.Rules))
			{
				return;
			}
			this.IdVersionItemsHasChanged<PolicyNudgeRule15>(policyNudgeRules.Rules, from r in this.serverConfig.PolicyNudgeRules.Rules
			where r.IsEnabled && (this.canOutlookSupportFullPnrXml || r.IsPnrXmlValid)
			select r);
			if (this.hasChanged)
			{
				return;
			}
			IList<IVersionedItem> clientMessages = new List<IVersionedItem>();
			IList<IVersionedItem> clientDistributionLists = new List<IVersionedItem>();
			if (!(from r in policyNudgeRules.Rules
			select OtherAttribtuesUtils.TryGetVersionedItemFromOtherAttributes(r, "messageString", clientMessages)).Any((bool v) => !v))
			{
				if (!(from r in policyNudgeRules.Rules
				select OtherAttribtuesUtils.TryGetVersionedItemFromOtherAttributes(r, "distributionList", clientDistributionLists)).Any((bool v) => !v))
				{
					List<PolicyNudgeRule.References> list = new List<PolicyNudgeRule.References>();
					foreach (PolicyNudgeRule policyNudgeRule in from r in this.serverConfig.PolicyNudgeRules.Rules
					where r.IsEnabled && (this.canOutlookSupportFullPnrXml || r.IsPnrXmlValid)
					select r)
					{
						PolicyNudgeRule.References item;
						policyNudgeRule.GetRuleXml(this.exchangeLocale, new PolicyNudgeConfigurationUtils.AdMessageStrings(this.serverConfig, this.outlookCultureTag), new PolicyNudgeConfigurationUtils.AdDistributionListResolver(this.serverConfig, this.senderADObjectId), new PolicyNudgeConfigurationUtils.DataClassificationResolver(this.serverConfig), this.canOutlookSupportFullPnrXml, out item);
						list.Add(item);
					}
					this.IdVersionItemsHasChanged<IVersionedItem>(clientMessages, list.SelectMany((PolicyNudgeRule.References r) => r.Messages));
					this.IdVersionItemsHasChanged<IVersionedItem>(clientDistributionLists, list.SelectMany((PolicyNudgeRule.References r) => r.DistributionLists));
					return;
				}
			}
			this.hasChanged = true;
		}

		internal override void Visit(PolicyNudgeRule15 policyNudgeRule)
		{
			this.EnsureValid(policyNudgeRule.ID);
		}

		internal override void Visit(ClassificationItems15 classificationItems)
		{
			if (!this.EnsureValid(classificationItems.ClassificationDefinitions))
			{
				return;
			}
			classificationItems.ClassificationDefinitions.Accept(this);
		}

		internal override void Visit(ClassificationDefinitions15 classificationDefinitions)
		{
			if (this.hasChanged || !this.EnsureValid(classificationDefinitions.ClassificationDefinitions))
			{
				return;
			}
			this.IdVersionItemsHasChanged<ClassificationDefinition15>(classificationDefinitions.ClassificationDefinitions, this.serverConfig.ClassificationDefinitions);
		}

		internal override void Visit(ClassificationDefinition15 classificationDefinition)
		{
			this.EnsureValid(classificationDefinition.ID);
		}

		private void IdVersionItemsHasChanged<T>(IEnumerable<T> clientItems, IEnumerable<IVersionedItem> serverItems) where T : IVersionedItem
		{
			if (this.hasChanged)
			{
				return;
			}
			foreach (T t in clientItems)
			{
				if (t is IVisitee15)
				{
					((IVisitee15)((object)t)).Accept(this);
				}
			}
			IEnumerable<Tuple<T, IVersionedItem>> source = PolicyNudgeConfigurationUtils.OuterJoin<T, IVersionedItem, string>(clientItems, (T r) => r.ID, serverItems, (IVersionedItem r) => r.ID);
			this.hasChanged = source.Any(delegate(Tuple<T, IVersionedItem> i)
			{
				if (i.Item1 != null && i.Item2 != null)
				{
					T item = i.Item1;
					return item.Version != i.Item2.Version;
				}
				return true;
			});
		}

		private bool Ensure(bool b)
		{
			if (!b)
			{
				this.hasChanged = true;
			}
			return b;
		}

		private bool EnsureValid(object value)
		{
			return this.Ensure(value != null);
		}

		private CachedOrganizationConfiguration serverConfig;

		private ADObjectId senderADObjectId;

		private bool hasChanged;

		private string outlookCultureTag;

		private string exchangeLocale;

		private bool canOutlookSupportFullPnrXml;
	}
}
