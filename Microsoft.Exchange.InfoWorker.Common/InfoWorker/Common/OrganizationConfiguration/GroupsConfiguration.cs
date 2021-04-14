using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.IsMemberOfProvider;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.InfoWorker.Common.OrganizationConfiguration
{
	internal class GroupsConfiguration
	{
		public GroupsConfiguration(OrganizationId organizationId)
		{
			this.organizationId = organizationId;
		}

		public void Initialize()
		{
		}

		public GroupConfiguration GetGroupInformation(string group)
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.organizationId), 58, "GetGroupInformation", "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\OrganizationConfiguration\\GroupsConfiguration.cs");
			return this.GetGroupInformation(tenantOrRootOrgRecipientSession, group);
		}

		public GroupConfiguration GetGroupInformation(IRecipientSession session, string group)
		{
			return GroupsConfiguration.groupsResolver.Value.GetGroupInfo(session, (RoutingAddress)group);
		}

		public GroupConfiguration GetGroupInformation(IRecipientSession session, Guid group)
		{
			return GroupsConfiguration.groupsResolver.Value.GetGroupInfo(session, group);
		}

		public bool IsMemberOf(ADObjectId user, string group)
		{
			return this.IsMemberOf(user.ObjectGuid, (RoutingAddress)group);
		}

		public bool IsMemberOf(IRecipientSession session, ADObjectId user, string group)
		{
			return GroupsConfiguration.groupsResolver.Value.IsMemberOf(session, user, (RoutingAddress)group);
		}

		public bool IsMemberOf(Guid adUserObjectGuid, RoutingAddress group)
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.organizationId), 120, "IsMemberOf", "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\OrganizationConfiguration\\GroupsConfiguration.cs");
			return this.IsMemberOf(tenantOrRootOrgRecipientSession, adUserObjectGuid, group);
		}

		public bool IsMemberOf(IRecipientSession session, Guid adUserObjectGuid, RoutingAddress group)
		{
			return GroupsConfiguration.groupsResolver.Value.IsMemberOf(session, adUserObjectGuid, group);
		}

		private OrganizationId organizationId;

		private static Lazy<GroupsConfiguration.GroupsResolver> groupsResolver = new Lazy<GroupsConfiguration.GroupsResolver>(LazyThreadSafetyMode.ExecutionAndPublication);

		private class IsMemberOfResolverADAdapterVersionedResolver : IsMemberOfResolverADAdapter<RoutingAddress>.RoutingAddressResolver
		{
			public IsMemberOfResolverADAdapterVersionedResolver(bool disableDynamicGroups) : base(disableDynamicGroups)
			{
			}

			protected override ExpandedGroup CreateExpandedGroup(ADObject group, List<Guid> memberGroups, List<Guid> memberRecipients)
			{
				return new GroupsConfiguration.ExpandedGroupWithVersion(memberGroups, memberRecipients, group.Id.ObjectGuid, group.WhenChangedUTC.Value);
			}
		}

		internal class ExpandedGroupWithVersion : ExpandedGroup
		{
			public ExpandedGroupWithVersion(List<Guid> memberGroups, List<Guid> memberRecipients, Guid groupGuid, DateTime groupVersion) : base(memberGroups, memberRecipients)
			{
				this.GroupGuid = groupGuid;
				this.GroupVersion = groupVersion;
			}

			public Guid GroupGuid { get; private set; }

			public DateTime GroupVersion { get; private set; }

			public override long ItemSize
			{
				get
				{
					return base.ItemSize + 16L + 8L;
				}
			}

			private const int GuidLength = 16;

			private const int DateTimeLength = 8;
		}

		private class GroupsResolver : IsMemberOfResolver<RoutingAddress>
		{
			public GroupsResolver() : base(new GroupsConfiguration.GroupsResolver.GroupsResolverConfig(), new IsMemberOfResolverPerformanceCounters("Infoworker"), new GroupsConfiguration.IsMemberOfResolverADAdapterVersionedResolver(true))
			{
			}

			public GroupConfiguration GetGroupInfo(IRecipientSession session, RoutingAddress groupKey)
			{
				base.ThrowIfDisposed();
				if (this.enabled)
				{
					ResolvedGroup value = this.resolvedGroups.GetValue(null, new Tuple<PartitionId, OrganizationId, RoutingAddress>(session.SessionSettings.PartitionId, session.SessionSettings.CurrentOrganizationId, groupKey));
					if (value.GroupGuid != Guid.Empty)
					{
						return this.GetGroupInfo(session, value.GroupGuid);
					}
				}
				return null;
			}

			public GroupConfiguration GetGroupInfo(IRecipientSession session, Guid groupGuid)
			{
				base.ThrowIfDisposed();
				if (this.enabled)
				{
					GroupsConfiguration.ExpandedGroupWithVersion expandedGroupWithVersion = (GroupsConfiguration.ExpandedGroupWithVersion)this.expandedGroups.GetValue(null, new Tuple<PartitionId, OrganizationId, Guid>(session.SessionSettings.PartitionId, session.SessionSettings.CurrentOrganizationId, groupGuid));
					return new GroupConfiguration(expandedGroupWithVersion.GroupGuid, expandedGroupWithVersion.GroupVersion, expandedGroupWithVersion.MemberGroups);
				}
				return null;
			}

			private class GroupsResolverConfig : IsMemberOfResolverConfig
			{
				public bool Enabled
				{
					get
					{
						return true;
					}
				}

				public long ResolvedGroupsMaxSize
				{
					get
					{
						return 33554432L;
					}
				}

				public TimeSpan ResolvedGroupsExpirationInterval
				{
					get
					{
						return TimeSpan.FromHours(3.0);
					}
				}

				public TimeSpan ResolvedGroupsCleanupInterval
				{
					get
					{
						return TimeSpan.FromHours(1.0);
					}
				}

				public TimeSpan ResolvedGroupsPurgeInterval
				{
					get
					{
						return TimeSpan.FromMinutes(5.0);
					}
				}

				public TimeSpan ResolvedGroupsRefreshInterval
				{
					get
					{
						return TimeSpan.FromMinutes(10.0);
					}
				}

				public long ExpandedGroupsMaxSize
				{
					get
					{
						return 134217728L;
					}
				}

				public TimeSpan ExpandedGroupsExpirationInterval
				{
					get
					{
						return TimeSpan.FromHours(3.0);
					}
				}

				public TimeSpan ExpandedGroupsCleanupInterval
				{
					get
					{
						return TimeSpan.FromHours(1.0);
					}
				}

				public TimeSpan ExpandedGroupsPurgeInterval
				{
					get
					{
						return TimeSpan.FromMinutes(5.0);
					}
				}

				public TimeSpan ExpandedGroupsRefreshInterval
				{
					get
					{
						return TimeSpan.FromMinutes(10.0);
					}
				}
			}
		}
	}
}
