using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class DistributionGroupTaskHelper
	{
		internal static void CheckMembershipRestriction(ADGroup distributionGroup, Task.ErrorLoggerDelegate errorLogger)
		{
			if (GroupTypeFlags.SecurityEnabled == (distributionGroup.GroupType & GroupTypeFlags.SecurityEnabled))
			{
				if (distributionGroup.MemberJoinRestriction != MemberUpdateType.Closed && distributionGroup.MemberJoinRestriction != MemberUpdateType.ApprovalRequired)
				{
					errorLogger(new RecipientTaskException(Strings.ErrorJoinRestrictionInvalid), ExchangeErrorCategory.Client, null);
				}
				if (distributionGroup.MemberDepartRestriction != MemberUpdateType.Closed)
				{
					errorLogger(new RecipientTaskException(Strings.ErrorDepartRestrictionInvalid), ExchangeErrorCategory.Client, null);
				}
			}
		}

		internal static void CheckModerationInMixedEnvironment(ADRecipient recipient, Task.TaskWarningLoggingDelegate warningLogger, LocalizedString warningText)
		{
			if (!recipient.ModerationEnabled || !recipient.IsModified(ADRecipientSchema.ModerationEnabled))
			{
				return;
			}
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 103, "CheckModerationInMixedEnvironment", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\DistributionList\\DistributionGroupTaskHelper.cs");
			ADPagedReader<MiniServer> adpagedReader = topologyConfigurationSession.FindPagedMiniServer(null, QueryScope.SubTree, DistributionGroupTaskHelper.filter, null, 1, DistributionGroupTaskHelper.ExchangeVersionProperties);
			using (IEnumerator<MiniServer> enumerator = adpagedReader.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					MiniServer miniServer = enumerator.Current;
					warningLogger(warningText);
				}
			}
		}

		internal static string GetGroupNameWithNamingPolicy(Organization organization, ADUser user, ADGroup group, string groupName, PropertyDefinition property, Task.ErrorLoggerDelegate errorLogger)
		{
			DistributionGroupTaskHelper.ValidateGroupNameWithBlockedWordsList(organization, group, groupName, property, errorLogger);
			if (organization.DistributionGroupNamingPolicy == null)
			{
				errorLogger(new RecipientTaskException(Strings.ErrorDistributionGroupNamingPolicy), ExchangeErrorCategory.ServerOperation, organization.Identity);
			}
			string appliedName = organization.DistributionGroupNamingPolicy.GetAppliedName(groupName, user);
			return appliedName.Trim();
		}

		internal static void ValidateGroupNameWithBlockedWordsList(Organization organization, ADGroup group, string groupName, PropertyDefinition property, Task.ErrorLoggerDelegate errorLogger)
		{
			if (organization == null)
			{
				throw new ArgumentNullException("organization");
			}
			if (group == null)
			{
				throw new ArgumentNullException("group");
			}
			if (string.IsNullOrEmpty(groupName))
			{
				throw new ArgumentNullException("groupName");
			}
			string text = groupName.ToLower();
			foreach (string text2 in organization.DistributionGroupNameBlockedWordsList)
			{
				if (text.Contains(text2.ToLower()))
				{
					throw new DataValidationException(new PropertyValidationError(Strings.ErrorGroupNameContainBlockedWords(text2), property, null));
				}
			}
		}

		internal static readonly ComparisonFilter EarlierThanMinimumE14VersionFilter = new ComparisonFilter(ComparisonOperator.LessThan, ServerSchema.VersionNumber, Server.E14MinVersion);

		internal static readonly ComparisonFilter EarlierThanMinimumE2007VersionFilter = new ComparisonFilter(ComparisonOperator.LessThan, ServerSchema.VersionNumber, Server.E2007MinVersion);

		internal static readonly BitMaskAndFilter HubRoleFilter = new BitMaskAndFilter(ServerSchema.CurrentServerRole, 32UL);

		internal static readonly QueryFilter filter = new OrFilter(new QueryFilter[]
		{
			DistributionGroupTaskHelper.EarlierThanMinimumE2007VersionFilter,
			new AndFilter(new QueryFilter[]
			{
				DistributionGroupTaskHelper.EarlierThanMinimumE14VersionFilter,
				DistributionGroupTaskHelper.HubRoleFilter
			})
		});

		internal static readonly PropertyDefinition[] ExchangeVersionProperties = new PropertyDefinition[]
		{
			ADObjectSchema.ExchangeVersion
		};
	}
}
