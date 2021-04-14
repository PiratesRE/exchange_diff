using System;
using System.Management.Automation;
using System.Threading;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Data.Directory.Sync.TenantRelocationSync;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.TopologyDiscovery;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "TenantRelocationRequest", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveTenantRelocationRequest : SetSystemConfigurationObjectTask<TenantRelocationRequestIdParameter, TenantRelocationRequest>
	{
		[Parameter]
		public SwitchParameter Complete
		{
			get
			{
				return (SwitchParameter)(base.Fields["Complete"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Complete"] = value;
			}
		}

		[Parameter]
		public SwitchParameter DeprovisionedTarget
		{
			get
			{
				return (SwitchParameter)(base.Fields["DeprovisionedTarget"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["DeprovisionedTarget"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveTenantRelocationRequest(this.Identity.ToString());
			}
		}

		protected override bool RehomeDataSession
		{
			get
			{
				return false;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			IConfigDataProvider configDataProvider = base.CreateSession();
			ADSessionSettings sessionSettings = ((IDirectorySession)configDataProvider).SessionSettings;
			sessionSettings.TenantConsistencyMode = TenantConsistencyMode.IncludeRetiredTenants;
			sessionSettings.RetiredTenantModificationAllowed = true;
			return configDataProvider;
		}

		protected override IConfigurable ResolveDataObject()
		{
			ADObject adobject = (ADObject)base.ResolveDataObject();
			base.RebindDataSessionToDataObjectPartitionRidMasterIncludeRetiredTenants(((ADObjectId)adobject.Identity).GetPartitionId());
			return (ADObject)base.ResolveDataObject();
		}

		private ITenantConfigurationSession CreateAllTenantsScopedConfigSession(OrganizationId orgId)
		{
			ADSessionSettings adsessionSettings = ADSessionSettings.FromAllTenantsPartitionId(orgId.PartitionId);
			adsessionSettings.TenantConsistencyMode = TenantConsistencyMode.IncludeRetiredTenants;
			adsessionSettings.RetiredTenantModificationAllowed = true;
			return DirectorySessionFactory.Default.CreateTenantConfigurationSession(null, false, ConsistencyMode.PartiallyConsistent, null, adsessionSettings, 108, "CreateAllTenantsScopedConfigSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Relocation\\RemoveTenantRelocationRequest.cs");
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (string.IsNullOrEmpty(this.DataObject.TargetForest))
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorTenantNotBeingRelocated(this.Identity.ToString())), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			if (this.DataObject.RelocationStatus == TenantRelocationStatus.Lockdown)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorTenantInLockdown(this.Identity.ToString())), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			this.isRelocationComplete = (this.DataObject.RelocationStatusDetailsSource == RelocationStatusDetailsSource.RetiredUpdatedTargetForest);
			if (this.isRelocationComplete && !this.Complete)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorCompleteFlagRequired(this.Identity.ToString())), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			if (!this.isRelocationComplete && this.Complete)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorCompleteFlagNotAllowed(this.Identity.ToString(), this.DataObject.RelocationStatusDetailsSource.ToString(), RelocationStatusDetailsSource.RetiredUpdatedTargetForest.ToString())), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			if (this.Complete)
			{
				if (!this.DataObject.IsRetiredSourceHoldTimedOut())
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorSourceHoldNotTimedOut(this.Identity.ToString(), TenantRelocationRequest.WaitTimeBeforeRemoveSourceReplicaDays.ToString(), this.DataObject.RetiredStartTime.Value.ToUniversalTime().ToString())), ErrorCategory.InvalidOperation, this.DataObject.Identity);
				}
				if (this.DataObject.RelocationStateRequested != RelocationStateRequested.Cleanup)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorCleanupRequestedAtWrongRequestedState(this.Identity.ToString(), this.DataObject.RelocationStateRequested.ToString(), RelocationStateRequested.Cleanup.ToString())), ErrorCategory.InvalidOperation, this.DataObject.Identity);
				}
			}
			if (this.DeprovisionedTarget && !this.Complete)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorDeprovisionedTargetPassedWithoutComplete(this.Identity.ToString())), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			if (this.DataObject.RelocationStatusDetailsSource >= RelocationStatusDetailsSource.InitializationFinished)
			{
				Exception ex;
				TenantRelocationRequest.PopulatePresentationObject(this.DataObject, null, out ex);
				if (ex != null)
				{
					if (!(ex is CannotFindTargetTenantException))
					{
						base.WriteError(ex, ErrorCategory.InvalidOperation, this.DataObject.Identity);
					}
					else if (!this.DeprovisionedTarget)
					{
						base.WriteError(new InvalidOperationException(Strings.ErrorDeprovisionedTargetNotPassed(this.Identity.ToString())), ErrorCategory.InvalidOperation, this.DataObject.Identity);
					}
				}
				GetTenantRelocationRequest.PopulateGlsProperty(this.DataObject, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
				if (!ADSessionSettings.IsGlsDisabled)
				{
					if (!string.IsNullOrEmpty(this.DataObject.GLSResolvedForest) && this.DataObject.GLSResolvedForest != GetTenantRelocationRequest.GlsLookupFailed && this.DataObject.GLSResolvedForest != GetTenantRelocationRequest.GlsDisabled && ((this.Complete && !string.IsNullOrEmpty(this.DataObject.TargetForest) && !this.DataObject.TargetForest.Equals(this.DataObject.GLSResolvedForest, StringComparison.OrdinalIgnoreCase)) || (!this.Complete && !string.IsNullOrEmpty(this.DataObject.SourceForest) && !this.DataObject.SourceForest.Equals(this.DataObject.GLSResolvedForest, StringComparison.OrdinalIgnoreCase))))
					{
						base.WriteError(new InvalidOperationException(Strings.ErrorInvalidTenantGlsRecord(this.Identity.ToString(), this.DataObject.GLSResolvedForest, this.DataObject.SourceForest, this.DataObject.TargetForest)), ErrorCategory.InvalidOperation, this.DataObject.Identity);
					}
					if (this.DataObject.GLSResolvedForest == GetTenantRelocationRequest.GlsLookupFailed && !this.DeprovisionedTarget)
					{
						base.WriteError(new InvalidOperationException(Strings.ErrorNoTenantGlsRecord(this.Identity.ToString())), ErrorCategory.InvalidOperation, this.DataObject.Identity);
					}
				}
			}
			if (!TenantRelocationStateCache.IgnoreRelocationTimeConstraints() && this.DataObject.IsRelocationInProgress())
			{
				base.WriteError(new RelocationInProgressException(this.DataObject.OrganizationId.ToString(), this.DataObject.HasPermanentError().ToString(), this.DataObject.Suspended.ToString(), this.DataObject.AutoCompletionEnabled.ToString(), this.DataObject.RelocationStatusDetailsSource.ToString(), this.DataObject.AutoCompletionEnabled ? RelocationStatusDetailsSource.RetiredUpdatedTargetForest.ToString() : this.DataObject.RelocationStateRequested.ToString()), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (this.isRelocationComplete)
			{
				ITenantConfigurationSession tenantConfigurationSession = this.CreateAllTenantsScopedConfigSession(this.DataObject.OrganizationId);
				ITenantRecipientSession recipientSession = this.CreateRecipientSession(this.DataObject.OrganizationId.OrganizationalUnit);
				Container container = tenantConfigurationSession.Read<Container>(this.DataObject.OrganizationId.ConfigurationUnit.Parent);
				ADOrganizationalUnit adorganizationalUnit = null;
				bool useConfigNC = tenantConfigurationSession.UseConfigNC;
				try
				{
					tenantConfigurationSession.UseConfigNC = false;
					adorganizationalUnit = tenantConfigurationSession.Read<ADOrganizationalUnit>(this.DataObject.OrganizationId.OrganizationalUnit);
				}
				finally
				{
					tenantConfigurationSession.UseConfigNC = useConfigNC;
				}
				if (adorganizationalUnit != null)
				{
					this.CleanupRecipients(this.DataObject.OrganizationId.OrganizationalUnit, recipientSession);
					tenantConfigurationSession.DeleteTree(adorganizationalUnit, null);
				}
				if (container != null)
				{
					this.RemoveExternalDirOrgIdFromCU(tenantConfigurationSession, this.DataObject.OrganizationId.ConfigurationUnit);
					tenantConfigurationSession.DeleteTree(container, null);
				}
				if (!this.DeprovisionedTarget)
				{
					ITenantConfigurationSession tenantConfigurationSession2 = this.CreateAllTenantsScopedConfigSession(this.DataObject.TargetOrganizationId);
					ExchangeConfigurationUnit exchangeConfigurationUnit = tenantConfigurationSession2.Read<ExchangeConfigurationUnit>(this.DataObject.TargetOrganizationId.ConfigurationUnit);
					exchangeConfigurationUnit.RelocationSourceForestRaw = null;
					exchangeConfigurationUnit.RelocationStatusDetailsRaw = RelocationStatusDetails.NotStarted;
					exchangeConfigurationUnit.TenantRelocationCompletionTargetVector = null;
					tenantConfigurationSession2.Save(exchangeConfigurationUnit);
				}
			}
			else
			{
				ITenantConfigurationSession tenantConfigurationSession3 = this.CreateAllTenantsScopedConfigSession(this.DataObject.TargetOrganizationId);
				if (this.DataObject.TargetOrganizationId != null)
				{
					if (!TenantRelocationStateCache.IgnoreRelocationTimeConstraints() && !this.WaitForReplicationConvergenceInTargetForest(this.DataObject.TargetOriginatingServer, new TimeSpan(0, 5, 0)))
					{
						base.WriteError(new ReplicationNotCompleteException(this.DataObject.TargetForest, this.DataObject.TargetOriginatingServer), ErrorCategory.InvalidOperation, this.DataObject.Identity);
					}
					ITenantRecipientSession recipientSession2 = this.CreateRecipientSession(this.DataObject.TargetOrganizationId.OrganizationalUnit);
					Container container2 = tenantConfigurationSession3.Read<Container>(this.DataObject.TargetOrganizationId.ConfigurationUnit.Parent);
					ADOrganizationalUnit adorganizationalUnit2 = null;
					if (this.DataObject.TargetOrganizationId.OrganizationalUnit != null)
					{
						bool useConfigNC2 = tenantConfigurationSession3.UseConfigNC;
						try
						{
							tenantConfigurationSession3.UseConfigNC = false;
							adorganizationalUnit2 = tenantConfigurationSession3.Read<ADOrganizationalUnit>(this.DataObject.TargetOrganizationId.OrganizationalUnit);
						}
						finally
						{
							tenantConfigurationSession3.UseConfigNC = useConfigNC2;
						}
					}
					if (adorganizationalUnit2 != null)
					{
						this.CleanupRecipients(this.DataObject.TargetOrganizationId.OrganizationalUnit, recipientSession2);
						tenantConfigurationSession3.DeleteTree(adorganizationalUnit2, null);
					}
					if (container2 != null)
					{
						this.RemoveExternalDirOrgIdFromCU(tenantConfigurationSession3, this.DataObject.TargetOrganizationId.ConfigurationUnit);
						tenantConfigurationSession3.DeleteTree(container2, null);
					}
				}
				this.DataObject.RelocationSyncStartTime = null;
				this.DataObject.LockdownStartTime = null;
				this.DataObject.RetiredStartTime = null;
				this.DataObject.TransitionCounter = null;
				this.DataObject.TargetForest = null;
				this.DataObject.SafeLockdownSchedule = null;
				this.DataObject[TenantRelocationRequestSchema.RelocationStatusDetailsRaw] = null;
				this.DataObject[TenantRelocationRequestSchema.RelocationSourceForestRaw] = null;
				this.DataObject[TenantRelocationRequestSchema.TenantRelocationFlags] = 0;
				base.InternalProcessRecord();
			}
			TaskLogger.LogExit();
		}

		private void RemoveExternalDirOrgIdFromCU(ITenantConfigurationSession session, ADObjectId cuObjId)
		{
			ExchangeConfigurationUnit exchangeConfigurationUnit = session.Read<ExchangeConfigurationUnit>(cuObjId);
			exchangeConfigurationUnit.ExternalDirectoryOrganizationId = null;
			session.Save(exchangeConfigurationUnit);
		}

		private ITenantRecipientSession CreateRecipientSession(ADObjectId ouObjId)
		{
			ADSessionSettings adsessionSettings = ADSessionSettings.FromAllTenantsObjectId(ouObjId);
			adsessionSettings.IncludeSoftDeletedObjects = true;
			adsessionSettings.TenantConsistencyMode = TenantConsistencyMode.IncludeRetiredTenants;
			adsessionSettings.RetiredTenantModificationAllowed = true;
			return DirectorySessionFactory.Default.CreateTenantRecipientSession(false, ConsistencyMode.PartiallyConsistent, adsessionSettings, 421, "CreateRecipientSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Relocation\\RemoveTenantRelocationRequest.cs");
		}

		private void CleanupRecipients(ADObjectId ouObjId, ITenantRecipientSession recipientSession)
		{
			ADPagedReader<ADRecipient> adpagedReader = recipientSession.FindPaged(ouObjId, QueryScope.SubTree, new ExistsFilter(ADRecipientSchema.ExternalDirectoryObjectId), null, 0);
			using (TenantRelocationThrottlingManager tenantRelocationThrottlingManager = new TenantRelocationThrottlingManager(recipientSession.SessionSettings.PartitionId.ForestFQDN))
			{
				foreach (ADRecipient adrecipient in adpagedReader)
				{
					adrecipient.ExternalDirectoryObjectId = null;
					recipientSession.Save(adrecipient, true);
					recipientSession.Delete(adrecipient);
					tenantRelocationThrottlingManager.Throttle();
				}
			}
		}

		private long ReadDcHighestUSN(PartitionId partitionId, string domainController, bool useConfigNC, out Guid invocationId, out WatermarkMap watermarks)
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(domainController, true, ConsistencyMode.PartiallyConsistent, null, ADSessionSettings.FromAccountPartitionRootOrgScopeSet(partitionId), 452, "ReadDcHighestUSN", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Relocation\\RemoveTenantRelocationRequest.cs");
			invocationId = topologyConfigurationSession.GetInvocationIdByFqdn(domainController);
			topologyConfigurationSession.UseConfigNC = useConfigNC;
			watermarks = SyncConfiguration.GetReplicationCursors(topologyConfigurationSession, useConfigNC, false);
			return watermarks[invocationId];
		}

		private bool CheckReplicationStatus(ITopologyConfigurationSession session, WatermarkMap finishWaterMark, bool useConfigNC)
		{
			WatermarkMap replicationCursors = SyncConfiguration.GetReplicationCursors(session, useConfigNC, false);
			return replicationCursors.ContainsAllChanges(finishWaterMark);
		}

		private bool WaitForReplicationConvergenceInTargetForest(string domainController, TimeSpan timeout)
		{
			base.WriteVerbose(Strings.VerboseWaitingForReplicationInTargetForest);
			WatermarkMap watermarkMap = new WatermarkMap();
			ReadOnlyCollection<ADServer> readOnlyCollection = ADForest.GetForest(this.DataObject.TargetForest, null).FindRootDomain().FindAllDomainControllers();
			DateTime utcNow = DateTime.UtcNow;
			foreach (ADServer adserver in readOnlyCollection)
			{
				string text;
				LocalizedString localizedString;
				if (SuitabilityVerifier.IsServerSuitableIgnoreExceptions(adserver.DnsHostName, false, null, out text, out localizedString))
				{
					Guid key;
					WatermarkMap watermarkMap2;
					long value = this.ReadDcHighestUSN(this.DataObject.TargetOrganizationId.PartitionId, adserver.DnsHostName, false, out key, out watermarkMap2);
					watermarkMap[key] = value;
				}
			}
			ITopologyConfigurationSession session = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(domainController, true, ConsistencyMode.PartiallyConsistent, null, ADSessionSettings.FromAccountPartitionRootOrgScopeSet(this.DataObject.TargetOrganizationId.PartitionId), 514, "WaitForReplicationConvergenceInTargetForest", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Relocation\\RemoveTenantRelocationRequest.cs");
			bool flag = false;
			while (!flag)
			{
				flag = this.CheckReplicationStatus(session, watermarkMap, false);
				if (flag || utcNow + timeout < DateTime.UtcNow)
				{
					break;
				}
				Thread.Sleep(5000);
			}
			if (flag)
			{
				base.WriteVerbose(Strings.VerboseTargetDcIsUpToDate(domainController));
			}
			return flag;
		}

		private bool isRelocationComplete;
	}
}
