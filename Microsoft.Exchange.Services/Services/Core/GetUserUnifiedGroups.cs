using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.FederatedDirectory;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetUserUnifiedGroups : SingleStepServiceCommand<GetUserUnifiedGroupsRequest, GetUserUnifiedGroupsResponseMessage>
	{
		public GetUserUnifiedGroups(CallContext callContext, GetUserUnifiedGroupsRequest request) : base(callContext, request)
		{
			this.requestedGroupsSets = request.ValidateParams();
			WarmupGroupManagementDependency.WarmUpAsyncIfRequired(base.CallContext.AccessingPrincipal);
			OwsLogRegistry.Register(GetUserUnifiedGroups.GetGetUserUnifiedGroupsActionName, typeof(GetUserUnifiedGroupsMetadata), new Type[0]);
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new GetUserUnifiedGroupsResponseMessage(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<GetUserUnifiedGroupsResponseMessage> Execute()
		{
			GetUserUnifiedGroups.Tracer.TraceDebug<SmtpAddress>((long)this.GetHashCode(), "GetUserUnifiedGroups.Execute: Retrieving unified groups for user {0}.", base.CallContext.AccessingPrincipal.MailboxInfo.PrimarySmtpAddress);
			List<GroupMailbox> allGroups;
			if (GetUserUnifiedGroups.IsModernGroupsNewArchitecture(base.CallContext.AccessingADUser))
			{
				allGroups = this.GetJoinedGroupsSupplementedByAAD();
			}
			else
			{
				allGroups = this.GetJoinedGroupsFromMailboxOnly();
			}
			GetUserUnifiedGroupsResponseMessage getUserUnifiedGroupsResponseMessage = new GetUserUnifiedGroupsResponseMessage();
			List<UnifiedGroupsSet> list = new List<UnifiedGroupsSet>();
			foreach (RequestedUnifiedGroupsSet requestedUnifiedGroupsSet in this.requestedGroupsSets)
			{
				int totalGroups = 0;
				List<GroupMailbox> resultGroupsList = GetUserUnifiedGroups.GetResultGroupsList(allGroups, requestedUnifiedGroupsSet, out totalGroups);
				UnifiedGroupsSet item = new UnifiedGroupsSet
				{
					FilterType = requestedUnifiedGroupsSet.FilterType,
					TotalGroups = totalGroups,
					Groups = GetUserUnifiedGroups.ConvertToUnifiedGroupArray(resultGroupsList)
				};
				list.Add(item);
			}
			getUserUnifiedGroupsResponseMessage.GroupsSets = list.ToArray();
			return new ServiceResult<GetUserUnifiedGroupsResponseMessage>(getUserUnifiedGroupsResponseMessage);
		}

		private static UnifiedGroup[] ConvertToUnifiedGroupArray(List<GroupMailbox> groupMailboxes)
		{
			UnifiedGroup[] array = new UnifiedGroup[groupMailboxes.Count];
			for (int i = 0; i < groupMailboxes.Count; i++)
			{
				GroupMailbox groupMailbox = groupMailboxes[i];
				array[i] = new UnifiedGroup
				{
					DisplayName = groupMailbox.DisplayName,
					SmtpAddress = (string)groupMailbox.SmtpAddress,
					LegacyDN = groupMailbox.Locator.LegacyDn,
					IsFavorite = groupMailbox.IsPinned,
					AccessType = (UnifiedGroupAccessType)groupMailbox.Type
				};
			}
			return array;
		}

		private static List<GroupMailbox> GetResultGroupsList(List<GroupMailbox> allGroups, RequestedUnifiedGroupsSet requestedSet, out int totalGroupsNumber)
		{
			totalGroupsNumber = 0;
			if (allGroups == null || !allGroups.Any<GroupMailbox>())
			{
				return allGroups;
			}
			List<GroupMailbox> list = allGroups;
			switch (requestedSet.FilterType)
			{
			case UnifiedGroupsFilterType.Favorites:
				list = (from mailbox in allGroups
				where mailbox.IsPinned
				select mailbox).ToList<GroupMailbox>();
				break;
			case UnifiedGroupsFilterType.ExcludeFavorites:
				list = (from mailbox in allGroups
				where !mailbox.IsPinned
				select mailbox).ToList<GroupMailbox>();
				break;
			}
			switch (requestedSet.SortType)
			{
			case UnifiedGroupsSortType.DisplayName:
				list = GetUserUnifiedGroups.GetSortedGroupList<string>(list, (GroupMailbox mailbox) => mailbox.DisplayName, requestedSet.SortDirection);
				break;
			case UnifiedGroupsSortType.JoinDate:
				list = GetUserUnifiedGroups.GetSortedGroupList<ExDateTime>(list, (GroupMailbox mailbox) => mailbox.JoinDate, requestedSet.SortDirection);
				break;
			case UnifiedGroupsSortType.FavoriteDate:
				list = GetUserUnifiedGroups.GetSortedGroupList<ExDateTime>(list, (GroupMailbox mailbox) => mailbox.PinDate, requestedSet.SortDirection);
				break;
			}
			totalGroupsNumber = list.Count;
			if (requestedSet.GroupsLimit > 0 && requestedSet.GroupsLimit < totalGroupsNumber)
			{
				list = list.Take(requestedSet.GroupsLimit).ToList<GroupMailbox>();
			}
			return list;
		}

		private static List<GroupMailbox> GetSortedGroupList<T>(List<GroupMailbox> groups, Func<GroupMailbox, T> sortFunction, SortDirection sortDirection)
		{
			List<GroupMailbox> result;
			if (sortDirection == SortDirection.Descending)
			{
				result = groups.OrderByDescending(sortFunction).ToList<GroupMailbox>();
			}
			else
			{
				result = groups.OrderBy(sortFunction).ToList<GroupMailbox>();
			}
			return result;
		}

		private List<GroupMailbox> GetJoinedGroupsSupplementedByAAD()
		{
			IRecipientSession adrecipientSession = base.CallContext.ADRecipientSessionContext.GetADRecipientSession();
			ADUser accessingADUser = base.CallContext.AccessingADUser;
			GroupsLogger groupsLogger = new GroupsLogger(GroupTaskName.GetGroupMembership, base.CallContext.ProtocolLog.ActivityId);
			UserMailboxLocator userMailboxLocator = UserMailboxLocator.Instantiate(adrecipientSession, accessingADUser);
			QueuedGroupJoinInvoker queuedGroupJoinInvoker = new QueuedGroupJoinInvoker();
			GroupMembershipAADReader groupMembershipAADReader = new GroupMembershipAADReader(accessingADUser, groupsLogger);
			groupsLogger.LogTrace("Retrieving Joined Groups. User={0}. TenantDomain={1}", new object[]
			{
				userMailboxLocator.ExternalId,
				base.CallContext.AccessingADUser.PrimarySmtpAddress.Domain
			});
			List<GroupMailbox> result;
			try
			{
				GroupMembershipMailboxReader mailboxReader = new GroupMembershipMailboxReader(userMailboxLocator, adrecipientSession, base.MailboxIdentityMailboxSession);
				GroupMailboxCollectionBuilder groupMailboxCollectionBuilder = new GroupMailboxCollectionBuilder(adrecipientSession, groupsLogger);
				GroupMembershipCompositeReader groupMembershipCompositeReader = new GroupMembershipCompositeReader(groupMembershipAADReader, mailboxReader, queuedGroupJoinInvoker, groupsLogger, groupMailboxCollectionBuilder);
				result = groupMembershipCompositeReader.GetJoinedGroups().ToList<GroupMailbox>();
			}
			finally
			{
				int queueSize = queuedGroupJoinInvoker.QueueSize;
				base.CallContext.ProtocolLog.Set(GetUserUnifiedGroupsMetadata.AADOnlyGroupCount, queueSize);
				base.CallContext.ProtocolLog.Set(GetUserUnifiedGroupsMetadata.AADLatency, (int)groupMembershipAADReader.AADLatency.TotalMilliseconds);
				if (!queuedGroupJoinInvoker.ProcessQueue(userMailboxLocator, base.CallContext.ProtocolLog.ActivityId))
				{
					groupsLogger.LogTrace("Queue was not processed.", new object[0]);
				}
			}
			return result;
		}

		private List<GroupMailbox> GetJoinedGroupsFromMailboxOnly()
		{
			IRecipientSession adrecipientSession = base.CallContext.ADRecipientSessionContext.GetADRecipientSession();
			UserMailboxLocator mailbox = UserMailboxLocator.Instantiate(adrecipientSession, base.CallContext.AccessingADUser);
			GroupMembershipMailboxReader groupMembershipMailboxReader = new GroupMembershipMailboxReader(mailbox, adrecipientSession, base.MailboxIdentityMailboxSession);
			return groupMembershipMailboxReader.GetJoinedGroups().ToList<GroupMailbox>();
		}

		private static bool IsModernGroupsNewArchitecture(ADUser user)
		{
			ExchangeConfigurationUnit configurationUnit = null;
			if (user.OrganizationId != null && user.OrganizationId.ConfigurationUnit != null)
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(user.OrganizationId);
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 294, "IsModernGroupsNewArchitecture", "f:\\15.00.1497\\sources\\dev\\services\\src\\Core\\servicecommands\\GetUserUnifiedGroups.cs");
				configurationUnit = tenantOrTopologyConfigurationSession.Read<ExchangeConfigurationUnit>(user.OrganizationId.ConfigurationUnit);
			}
			return VariantConfiguration.GetSnapshot(user.GetContext(configurationUnit), null, null).OwaClientServer.ModernGroupsNewArchitecture.Enabled;
		}

		private static readonly string GetGetUserUnifiedGroupsActionName = typeof(GetUserUnifiedGroups).Name;

		private static readonly Trace Tracer = ExTraceGlobals.GetUserUnifiedGroupsTracer;

		private readonly RequestedUnifiedGroupsSet[] requestedGroupsSets;
	}
}
