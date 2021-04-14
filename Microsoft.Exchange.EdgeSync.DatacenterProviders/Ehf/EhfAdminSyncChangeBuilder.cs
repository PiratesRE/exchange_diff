using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.EdgeSync.Ehf
{
	internal class EhfAdminSyncChangeBuilder
	{
		public EhfAdminSyncChangeBuilder(string tenantOU, string tenantConfigUnitDN, EhfTargetConnection targetConnection)
		{
			if (string.IsNullOrEmpty(tenantOU))
			{
				throw new ArgumentNullException("tenantOU");
			}
			if (string.IsNullOrEmpty(tenantConfigUnitDN))
			{
				throw new ArgumentNullException("tenantConfigUnitDN");
			}
			this.tenantOU = tenantOU;
			this.tenantConfigUnitDN = tenantConfigUnitDN;
			this.ehfTargetConnection = targetConnection;
		}

		public string TenantOU
		{
			get
			{
				return this.tenantOU;
			}
		}

		public string ConfigUnitDN
		{
			get
			{
				return this.tenantConfigUnitDN;
			}
		}

		public bool UpdateOrgManagementGroup
		{
			get
			{
				return this.updateOrgManagementGroup;
			}
		}

		public bool UpdateViewOnlyOrgManagementGroup
		{
			get
			{
				return this.updateViewOnlyOrgManagementGroup;
			}
		}

		public bool UpdateAdminAgentGroup
		{
			get
			{
				return this.updateAdminAgentGroup;
			}
		}

		public bool UpdateHelpdeskAgentGroup
		{
			get
			{
				return this.updateHelpdeskAgentGroup;
			}
		}

		public List<Guid> DeletedObjects
		{
			get
			{
				return this.deletedObjects;
			}
		}

		public List<ExSearchResultEntry> GroupChanges
		{
			get
			{
				return this.groupChanges;
			}
		}

		public List<ExSearchResultEntry> LiveIdChanges
		{
			get
			{
				return this.liveIdChanges;
			}
		}

		public bool ChangeExists
		{
			get
			{
				return this.updateOrgManagementGroup || this.updateViewOnlyOrgManagementGroup || this.updateHelpdeskAgentGroup || this.updateAdminAgentGroup || (this.groupChanges.Count != 0 || this.liveIdChanges.Count != 0 || this.deletedObjects.Count != 0);
			}
		}

		public EhfTargetConnection EhfTargetConnection
		{
			get
			{
				return this.ehfTargetConnection;
			}
		}

		public void AddGroupAdditionChange(ExSearchResultEntry change)
		{
			if (EhfWellKnownGroup.IsOrganizationManagementGroup(change) || EhfWellKnownGroup.IsViewOnlyOrganizationManagementGroup(change))
			{
				this.updateOrgManagementGroup = true;
				this.updateViewOnlyOrgManagementGroup = true;
			}
			else if (EhfWellKnownGroup.IsAdminAgentGroup(change.DistinguishedName))
			{
				this.updateAdminAgentGroup = true;
			}
			else if (EhfWellKnownGroup.IsHelpdeskAgentGroup(change.DistinguishedName))
			{
				this.updateHelpdeskAgentGroup = true;
			}
			if (this.IsFullTenantAdminSyncRequired())
			{
				this.ClearCachedChanges();
			}
			this.SetFullTenantAdminSyncIfTooManyCachedChanges();
		}

		public void AddGroupMembershipChange(ExSearchResultEntry change)
		{
			if (this.IsFullTenantAdminSyncRequired())
			{
				return;
			}
			if (EhfWellKnownGroup.IsOrganizationManagementGroup(change))
			{
				this.updateOrgManagementGroup = true;
			}
			else if (EhfWellKnownGroup.IsViewOnlyOrganizationManagementGroup(change))
			{
				this.updateViewOnlyOrgManagementGroup = true;
			}
			else if (EhfWellKnownGroup.IsAdminAgentGroup(change.DistinguishedName))
			{
				this.updateAdminAgentGroup = true;
			}
			else if (EhfWellKnownGroup.IsHelpdeskAgentGroup(change.DistinguishedName))
			{
				this.updateHelpdeskAgentGroup = true;
			}
			if (this.IsFullTenantAdminSyncRequired())
			{
				this.ClearCachedChanges();
			}
			else
			{
				this.groupChanges.Add(change);
			}
			this.SetFullTenantAdminSyncIfTooManyCachedChanges();
		}

		public void AddWlidChanges(ExSearchResultEntry change)
		{
			if (this.IsFullTenantAdminSyncRequired())
			{
				return;
			}
			this.liveIdChanges.Add(change);
			this.SetFullTenantAdminSyncIfTooManyCachedChanges();
		}

		public void HandleGroupDeletedEvent(ExSearchResultEntry entry)
		{
			if (this.IsFullTenantAdminSyncRequired())
			{
				return;
			}
			this.deletedObjects.Add(entry.GetObjectGuid());
			this.SetFullTenantAdminSyncIfTooManyCachedChanges();
		}

		public void HandleWlidDeletedEvent(ExSearchResultEntry entry)
		{
			if (this.IsFullTenantAdminSyncRequired())
			{
				return;
			}
			this.deletedObjects.Add(entry.GetObjectGuid());
			this.SetFullTenantAdminSyncIfTooManyCachedChanges();
		}

		private void SetFullTenantAdminSyncIfTooManyCachedChanges()
		{
			EhfSyncAppConfig ehfSyncAppConfig = this.ehfTargetConnection.Config.EhfSyncAppConfig;
			if (this.groupChanges.Count + this.liveIdChanges.Count > ehfSyncAppConfig.EhfAdminSyncMaxAccumulatedChangeSize || this.deletedObjects.Count > ehfSyncAppConfig.EhfAdminSyncMaxAccumulatedDeleteChangeSize)
			{
				this.ehfTargetConnection.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "Setting the tenant <{0}> for FullAdminSync since there are too many cached changes. GroupChanges:<{1}>; LiveidChanges:<{2}>; DeletedObject:<{3}>", new object[]
				{
					this.tenantOU,
					this.groupChanges.Count,
					this.liveIdChanges.Count,
					this.deletedObjects.Count
				});
				this.SetFullTenantAdminSyncRequired();
			}
		}

		public void HandleOrganizationChangedEvent(ExSearchResultEntry entry)
		{
			this.ehfTargetConnection.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.High, "Encountered a MODIFY organization event. The org '{0}' is set to do a complete adminsync", new object[]
			{
				this.tenantOU
			});
			this.SetFullTenantAdminSyncRequired();
		}

		public void HandleOrganizationAddedEvent(ExSearchResultEntry entry)
		{
			this.ehfTargetConnection.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.High, "Encountered a ADD organization event. The org '{0}' is set to do a complete adminsync", new object[]
			{
				this.tenantOU
			});
			this.SetFullTenantAdminSyncRequired();
		}

		public void ClearCachedChanges()
		{
			this.groupChanges.Clear();
			this.liveIdChanges.Clear();
			this.deletedObjects.Clear();
		}

		public EhfCompanyAdmins Flush(EhfADAdapter configADAdapter)
		{
			if (this.flushed)
			{
				throw new InvalidOperationException("Flush() should be called only once");
			}
			this.flushed = true;
			return EhfCompanyAdmins.CreateEhfCompanyAdmins(this, this.ehfTargetConnection, configADAdapter);
		}

		public bool IsFullTenantAdminSyncRequired()
		{
			return this.updateOrgManagementGroup && this.updateViewOnlyOrgManagementGroup && this.updateAdminAgentGroup && this.updateHelpdeskAgentGroup;
		}

		public bool HasDirectChangeForGroup(string distinguishedName)
		{
			foreach (ExSearchResultEntry exSearchResultEntry in this.groupChanges)
			{
				if (exSearchResultEntry.DistinguishedName.Equals(distinguishedName, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		private void SetFullTenantAdminSyncRequired()
		{
			this.updateOrgManagementGroup = true;
			this.updateViewOnlyOrgManagementGroup = true;
			this.updateAdminAgentGroup = true;
			this.updateHelpdeskAgentGroup = true;
			this.ClearCachedChanges();
		}

		private bool updateOrgManagementGroup;

		private bool updateViewOnlyOrgManagementGroup;

		private bool updateAdminAgentGroup;

		private bool updateHelpdeskAgentGroup;

		private bool flushed;

		private string tenantOU;

		private string tenantConfigUnitDN;

		private EhfTargetConnection ehfTargetConnection;

		private List<ExSearchResultEntry> groupChanges = new List<ExSearchResultEntry>();

		private List<ExSearchResultEntry> liveIdChanges = new List<ExSearchResultEntry>();

		private List<Guid> deletedObjects = new List<Guid>();
	}
}
