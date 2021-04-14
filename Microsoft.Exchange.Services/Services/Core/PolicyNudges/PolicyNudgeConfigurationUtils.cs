using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.InfoWorker.Common.OrganizationConfiguration;
using Microsoft.Exchange.Management.ClassificationDefinitions;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.MessagingPolicies.Rules.Classification;
using Microsoft.Exchange.MessagingPolicies.Rules.PolicyNudges;

namespace Microsoft.Exchange.Services.Core.PolicyNudges
{
	internal static class PolicyNudgeConfigurationUtils
	{
		internal static IEnumerable<Tuple<T1, T2>> OuterJoin<T1, T2, TID>(IEnumerable<T1> lefts, Func<T1, TID> leftId, IEnumerable<T2> rights, Func<T2, TID> rightId)
		{
			IEnumerable<Tuple<T1, T2>> first = from left in lefts
			join rightItem in rights on leftId(left) equals rightId(rightItem) into tempRight
			from rightItem in tempRight.DefaultIfEmpty<T2>()
			select new Tuple<T1, T2>(left, rightItem);
			IEnumerable<Tuple<T1, T2>> second = from right2 in rights
			join left2 in lefts on rightId(right2) equals leftId(left2) into tempLeft
			from left2 in tempLeft.DefaultIfEmpty<T1>()
			select new Tuple<T1, T2>(left2, right2);
			return first.Union(second);
		}

		internal static void MarkElementAsApply(XmlElement element, bool apply)
		{
			if (!apply)
			{
				while (element.HasChildNodes)
				{
					element.RemoveChild(element.FirstChild);
				}
			}
			else if (!element.HasChildNodes)
			{
				element.InnerText = " ";
			}
			element.SetAttribute("apply", apply.ToString());
		}

		internal static string GetExchangeLocaleFromOutlookCultureTag(string outlookCultureTag)
		{
			if (outlookCultureTag == null)
			{
				return null;
			}
			CultureInfo cultureInfo = null;
			try
			{
				cultureInfo = new CultureInfo(outlookCultureTag);
				goto IL_35;
			}
			catch (CultureNotFoundException)
			{
				return null;
			}
			IL_15:
			if (LanguagePackInfo.expectedCultureLcids.Contains(cultureInfo.LCID))
			{
				return cultureInfo.Name;
			}
			cultureInfo = cultureInfo.Parent;
			IL_35:
			if (cultureInfo == CultureInfo.InvariantCulture)
			{
				return null;
			}
			goto IL_15;
		}

		internal static bool CanOutlookSupportFullPnrXml(string engineVersion)
		{
			try
			{
				if (ExchangeBuild.Parse(engineVersion) >= ClassificationDefinitionConstants.CompatibleEngineVersion)
				{
					return true;
				}
			}
			catch (ArgumentException)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericError(RequestDetailsLogger.Current, "Invalid Outlook MCE Version", engineVersion);
			}
			return false;
		}

		internal class AdMessageStrings : ETRToPNRTranslator.IMessageStrings
		{
			public AdMessageStrings(CachedOrganizationConfiguration serverConfig, string outlookCultureTag)
			{
				this.serverConfig = serverConfig;
				this.OutlookCultureTag = outlookCultureTag;
			}

			public PolicyTipMessage Get(ETRToPNRTranslator.OutlookActionTypes type)
			{
				return this.Get(Tuple.Create<string, PolicyTipMessageConfigAction>(PolicyNudgeConfigurationUtils.GetExchangeLocaleFromOutlookCultureTag(this.OutlookCultureTag), this.GetPolicyTipAction(type)));
			}

			public string OutlookCultureTag { get; private set; }

			private PolicyTipMessageConfigAction GetPolicyTipAction(ETRToPNRTranslator.OutlookActionTypes outlookActionType)
			{
				switch (outlookActionType)
				{
				case ETRToPNRTranslator.OutlookActionTypes.NotifyOnly:
					return PolicyTipMessageConfigAction.NotifyOnly;
				case ETRToPNRTranslator.OutlookActionTypes.RejectMessage:
				case ETRToPNRTranslator.OutlookActionTypes.RejectUnlessFalsePositiveOverride:
					return PolicyTipMessageConfigAction.Reject;
				case ETRToPNRTranslator.OutlookActionTypes.RejectUnlessSilentOverride:
				case ETRToPNRTranslator.OutlookActionTypes.RejectUnlessExplicitOverride:
					return PolicyTipMessageConfigAction.RejectOverride;
				default:
					throw new IndexOutOfRangeException();
				}
			}

			public PolicyTipMessage Url
			{
				get
				{
					return this.Get(Tuple.Create<string, PolicyTipMessageConfigAction>(string.Empty, PolicyTipMessageConfigAction.Url));
				}
			}

			private PolicyTipMessage Get(Tuple<string, PolicyTipMessageConfigAction> key)
			{
				PerTenantPolicyNudgeRulesCollection.PolicyTipMessages messages = this.serverConfig.PolicyNudgeRules.Messages;
				PolicyTipMessage result;
				if (!messages.TryGetValue(key, out result))
				{
					throw new IndexOutOfRangeException();
				}
				return result;
			}

			private CachedOrganizationConfiguration serverConfig;
		}

		internal class AdDistributionListResolver : ETRToPNRTranslator.IDistributionListResolver
		{
			public AdDistributionListResolver(CachedOrganizationConfiguration serverConfig, ADObjectId senderADObjectId)
			{
				this.serverConfig = serverConfig;
				this.senderADObjectId = senderADObjectId;
			}

			public IEnumerable<IVersionedItem> Get(string distributionList)
			{
				GroupConfiguration groupInformation = this.serverConfig.GroupsConfiguration.GetGroupInformation(distributionList);
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.serverConfig.OrganizationId), 260, "Get", "f:\\15.00.1497\\sources\\dev\\services\\src\\Core\\servicecommands\\PolicyNudges\\PolicyNudgeConfigurationUtils.cs");
				HashSet<GroupConfiguration> hashSet = new HashSet<GroupConfiguration>(new PolicyNudgeConfigurationUtils.AdDistributionListResolver.GroupConfigurationEqualityComparer());
				this.GetAllGroups(groupInformation, tenantOrRootOrgRecipientSession, hashSet);
				return (from g in hashSet
				select new VersionedItem(g.Id.ToString(), g.Version)).DefaultIfEmpty(new VersionedItem(distributionList, DateTime.MinValue));
			}

			private void GetAllGroups(GroupConfiguration group, IRecipientSession session, HashSet<GroupConfiguration> groups)
			{
				if (group == null || groups.Add(group))
				{
					return;
				}
				foreach (GroupConfiguration group2 in from memberGroupGuid in @group.GroupGuids
				select this.serverConfig.GroupsConfiguration.GetGroupInformation(session, memberGroupGuid))
				{
					this.GetAllGroups(group2, session, groups);
				}
			}

			public bool IsMemberOf(string distributionList)
			{
				return this.serverConfig.GroupsConfiguration.IsMemberOf(this.senderADObjectId, distributionList);
			}

			private CachedOrganizationConfiguration serverConfig;

			private ADObjectId senderADObjectId;

			private class GroupConfigurationEqualityComparer : IEqualityComparer<GroupConfiguration>
			{
				public bool Equals(GroupConfiguration x, GroupConfiguration y)
				{
					return (x == null && y == null) || (x != null && y != null && x.Id == y.Id);
				}

				public int GetHashCode(GroupConfiguration obj)
				{
					if (obj == null)
					{
						return 0;
					}
					return obj.Id.GetHashCode();
				}
			}
		}

		internal class DataClassificationResolver : ETRToPNRTranslator.IDataClassificationResolver
		{
			public DataClassificationResolver(CachedOrganizationConfiguration organizationConfig)
			{
				this.organizationConfig = organizationConfig;
			}

			public bool IsVersionedDataClassification(string dataClassificationId, string rulePackageId)
			{
				bool result = false;
				if (this.organizationConfig != null && this.organizationConfig.ClassificationDefinitions != null)
				{
					ClassificationRulePackage classificationRulePackage = this.organizationConfig.ClassificationDefinitions.FirstOrDefault((ClassificationRulePackage rulePack) => string.Equals(rulePack.ID, rulePackageId, StringComparison.OrdinalIgnoreCase));
					if (classificationRulePackage != null && classificationRulePackage.VersionedDataClassificationIds != null)
					{
						result = classificationRulePackage.VersionedDataClassificationIds.Contains(dataClassificationId);
					}
				}
				return result;
			}

			private CachedOrganizationConfiguration organizationConfig;
		}
	}
}
