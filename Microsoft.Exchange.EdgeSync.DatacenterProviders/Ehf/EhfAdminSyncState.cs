using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.EdgeSync.Ehf
{
	internal sealed class EhfAdminSyncState
	{
		private EhfAdminSyncState(EhfCompanyIdentity companyIdentity, EhfTargetConnection targetConnection)
		{
			this.ehfTargetConnection = targetConnection;
			this.ehfCompanyIdentity = companyIdentity;
		}

		public static EhfAdminSyncState Create(EhfCompanyIdentity companyIdentity, ExSearchResultEntry entry, EhfTargetConnection targetConnection)
		{
			return new EhfAdminSyncState(companyIdentity, targetConnection)
			{
				orgAdminMembers = EhfAdminSyncState.GetAdminStateFromAttribute(entry, "msExchTargetServerAdmins"),
				viewOnlyOrgAdminMembers = EhfAdminSyncState.GetAdminStateFromAttribute(entry, "msExchTargetServerViewOnlyAdmins"),
				adminAgentMembers = EhfAdminSyncState.GetAdminStateFromAttribute(entry, "msExchTargetServerPartnerAdmins"),
				helpDeskAgentMembers = EhfAdminSyncState.GetAdminStateFromAttribute(entry, "msExchTargetServerPartnerViewOnlyAdmins")
			};
		}

		public static EhfAdminSyncState Create(EhfCompanyAdmins admins, bool addOrgAdminState, bool addViewOnlyOrgAdminState, bool addAdminAgentState, bool addHelpDeskAgentState, EhfTargetConnection targetConnection)
		{
			EhfAdminSyncState ehfAdminSyncState = admins.EhfAdminSyncState;
			EhfAdminSyncState ehfAdminSyncState2 = new EhfAdminSyncState(admins.EhfCompanyIdentity, targetConnection);
			ehfAdminSyncState2.orgAdminMembers = ehfAdminSyncState2.GetNewState(admins.OrganizationMangement, ehfAdminSyncState.orgAdminMembers, addOrgAdminState);
			ehfAdminSyncState2.viewOnlyOrgAdminMembers = ehfAdminSyncState2.GetNewState(admins.ViewonlyOrganizationManagement, ehfAdminSyncState.viewOnlyOrgAdminMembers, addViewOnlyOrgAdminState);
			ehfAdminSyncState2.adminAgentMembers = ehfAdminSyncState2.GetNewState(admins.AdminAgent, ehfAdminSyncState.adminAgentMembers, addAdminAgentState);
			ehfAdminSyncState2.helpDeskAgentMembers = ehfAdminSyncState2.GetNewState(admins.HelpdeskAgent, ehfAdminSyncState.helpDeskAgentMembers, addHelpDeskAgentState);
			return ehfAdminSyncState2;
		}

		public bool IsEmpty
		{
			get
			{
				return this.orgAdminMembers == null && this.viewOnlyOrgAdminMembers == null && this.adminAgentMembers == null && this.helpDeskAgentMembers == null;
			}
		}

		public EhfCompanyIdentity EhfCompanyIdentity
		{
			get
			{
				return this.ehfCompanyIdentity;
			}
		}

		public HashSet<Guid> OrganizationManagmentMembers
		{
			get
			{
				return this.orgAdminMembers;
			}
		}

		public HashSet<Guid> ViewOnlyOrganizationManagmentMembers
		{
			get
			{
				return this.viewOnlyOrgAdminMembers;
			}
		}

		public HashSet<Guid> AdminAgentMembers
		{
			get
			{
				return this.adminAgentMembers;
			}
		}

		public HashSet<Guid> HelpdeskAgentMembers
		{
			get
			{
				return this.helpDeskAgentMembers;
			}
		}

		public IEnumerable<KeyValuePair<string, List<byte[]>>> GetStatesToUpdate()
		{
			if (this.IsEmpty)
			{
				throw new InvalidOperationException("Should not update the state in AD if there is no change to update.");
			}
			if (this.orgAdminMembers != null)
			{
				yield return EhfAdminSyncState.GetStateToUpdateForAttribute(this.orgAdminMembers, "msExchTargetServerAdmins");
			}
			if (this.viewOnlyOrgAdminMembers != null)
			{
				yield return EhfAdminSyncState.GetStateToUpdateForAttribute(this.viewOnlyOrgAdminMembers, "msExchTargetServerViewOnlyAdmins");
			}
			if (this.adminAgentMembers != null)
			{
				yield return EhfAdminSyncState.GetStateToUpdateForAttribute(this.adminAgentMembers, "msExchTargetServerPartnerAdmins");
			}
			if (this.helpDeskAgentMembers != null)
			{
				yield return EhfAdminSyncState.GetStateToUpdateForAttribute(this.helpDeskAgentMembers, "msExchTargetServerPartnerViewOnlyAdmins");
			}
			yield break;
		}

		private static KeyValuePair<string, List<byte[]>> GetStateToUpdateForAttribute(HashSet<Guid> hashSet, string attributeName)
		{
			List<byte[]> list = new List<byte[]>(hashSet.Count);
			foreach (Guid guid in hashSet)
			{
				list.Add(guid.ToByteArray());
			}
			return new KeyValuePair<string, List<byte[]>>(attributeName, list);
		}

		private static HashSet<Guid> GetAdminStateFromAttribute(ExSearchResultEntry perimeterConfigEntry, string attributeName)
		{
			DirectoryAttribute attribute = perimeterConfigEntry.GetAttribute(attributeName);
			if (attribute == null)
			{
				return null;
			}
			HashSet<Guid> hashSet = new HashSet<Guid>();
			foreach (object obj in attribute.GetValues(typeof(byte[])))
			{
				byte[] array = obj as byte[];
				if (array != null && array.Length == 16)
				{
					hashSet.Add(new Guid(array));
				}
			}
			return hashSet;
		}

		private void AddToAdminSet<TAdminSyncUser>(Dictionary<Guid, TAdminSyncUser> members, HashSet<Guid> adminSet) where TAdminSyncUser : AdminSyncUser
		{
			if (members == null)
			{
				return;
			}
			foreach (Guid item in members.Keys)
			{
				if (adminSet.Count == this.ehfTargetConnection.Config.EhfSyncAppConfig.EhfAdminSyncMaxTargetAdminStateSize)
				{
					break;
				}
				adminSet.Add(item);
			}
		}

		private HashSet<Guid> GetNewState(EhfWellKnownGroup wellKnownGroup, HashSet<Guid> existingAdmins, bool updateState)
		{
			if (wellKnownGroup == null || !updateState)
			{
				return null;
			}
			HashSet<Guid> hashSet = new HashSet<Guid>();
			this.AddToAdminSet<MailboxAdminSyncUser>(wellKnownGroup.GroupMembers, hashSet);
			this.AddToAdminSet<PartnerGroupAdminSyncUser>(wellKnownGroup.LinkedRoleGroups, hashSet);
			this.AddToAdminSet<AdminSyncUser>(wellKnownGroup.SubGroups, hashSet);
			if (existingAdmins != null && existingAdmins.Count == hashSet.Count && existingAdmins.IsSubsetOf(hashSet))
			{
				this.ehfTargetConnection.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "The admin-state for group <{0}> is up to date in PerimeterConfig. No need to overwrite the state.", new object[]
				{
					wellKnownGroup.WellKnownGroupName
				});
				return null;
			}
			EhfSyncAppConfig ehfSyncAppConfig = this.ehfTargetConnection.Config.EhfSyncAppConfig;
			if (hashSet.Count >= ehfSyncAppConfig.EhfAdminSyncMaxTargetAdminStateSize)
			{
				this.ehfTargetConnection.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "Number of members (<{0}>) of group <{1}> is greater than or equal to the maximum number <{2}> we store in AD. An Empty Guid will be stored in the state indicating that the max limit has reached.", new object[]
				{
					hashSet.Count,
					wellKnownGroup.WellKnownGroupName,
					ehfSyncAppConfig.EhfAdminSyncMaxTargetAdminStateSize
				});
				hashSet.Clear();
				hashSet.Add(EhfCompanyAdmins.SyncStateFullGuid);
			}
			return hashSet;
		}

		private EhfCompanyIdentity ehfCompanyIdentity;

		private EhfTargetConnection ehfTargetConnection;

		private HashSet<Guid> orgAdminMembers;

		private HashSet<Guid> viewOnlyOrgAdminMembers;

		private HashSet<Guid> adminAgentMembers;

		private HashSet<Guid> helpDeskAgentMembers;
	}
}
