using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "TenantRelocationRequest", SupportsShouldProcess = true, DefaultParameterSetName = "DefaultParameterSet")]
	public sealed class SetTenantRelocationRequest : SetSystemConfigurationObjectTask<TenantRelocationRequestIdParameter, TenantRelocationRequest>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetTenantRelocationRequest(this.Identity.ToString());
			}
		}

		protected override bool RehomeDataSession
		{
			get
			{
				return false;
			}
		}

		[Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		public override TenantRelocationRequestIdParameter Identity
		{
			get
			{
				return (TenantRelocationRequestIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "DefaultParameterSet")]
		public RelocationStateRequestedByCmdlet RelocationStateRequested
		{
			get
			{
				return (RelocationStateRequestedByCmdlet)base.Fields[TenantRelocationRequestSchema.RelocationStateRequested];
			}
			set
			{
				base.Fields[TenantRelocationRequestSchema.RelocationStateRequested] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "DefaultParameterSet")]
		public bool AutoCompletionEnabled
		{
			get
			{
				return (bool)(base.Fields[TenantRelocationRequestSchema.AutoCompletionEnabled] ?? false);
			}
			set
			{
				base.Fields[TenantRelocationRequestSchema.AutoCompletionEnabled] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "DefaultParameterSet")]
		public bool LargeTenantModeEnabled
		{
			get
			{
				return (bool)(base.Fields[TenantRelocationRequestSchema.LargeTenantModeEnabled] ?? false);
			}
			set
			{
				base.Fields[TenantRelocationRequestSchema.LargeTenantModeEnabled] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "DefaultParameterSet")]
		public Schedule SafeLockdownSchedule
		{
			get
			{
				return (Schedule)base.Fields[TenantRelocationRequestSchema.SafeLockdownSchedule];
			}
			set
			{
				base.Fields[TenantRelocationRequestSchema.SafeLockdownSchedule] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "SuspendParameterSet")]
		public SwitchParameter Suspend
		{
			get
			{
				return (SwitchParameter)(base.Fields["SuspendParameter"] ?? false);
			}
			set
			{
				base.Fields["SuspendParameter"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ResumeParameterSet")]
		public SwitchParameter Resume
		{
			get
			{
				return (SwitchParameter)(base.Fields["ResumeParameter"] ?? false);
			}
			set
			{
				base.Fields["ResumeParameter"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "ResetPermanentErrorParameterSet")]
		public SwitchParameter ResetPermanentError
		{
			get
			{
				return (SwitchParameter)(base.Fields["ResetPermanentErrorParameter"] ?? false);
			}
			set
			{
				base.Fields["ResetPermanentErrorParameter"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "ResetPermanentErrorParameterSet")]
		public SwitchParameter ResetStartSyncTime
		{
			get
			{
				return (SwitchParameter)(base.Fields["ResetStartSyncTimeParameter"] ?? false);
			}
			set
			{
				base.Fields["ResetStartSyncTimeParameter"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ResetTransitionCounterParameterSet")]
		public SwitchParameter ResetTransitionCounter
		{
			get
			{
				return (SwitchParameter)(base.Fields["ResetTransitionCounterParameter"] ?? false);
			}
			set
			{
				base.Fields["ResetTransitionCounterParameter"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "DefaultParameterSet")]
		public SwitchParameter RollbackGls
		{
			get
			{
				return (SwitchParameter)(base.Fields["RollbackGlsParameter"] ?? false);
			}
			set
			{
				base.Fields["RollbackGlsParameter"] = value;
			}
		}

		protected override IConfigurable ResolveDataObject()
		{
			ADObject adobject = (ADObject)base.ResolveDataObject();
			base.RebindDataSessionToDataObjectPartitionRidMasterIncludeRetiredTenants(((ADObjectId)adobject.Identity).GetPartitionId());
			return (ADObject)base.ResolveDataObject();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (this.RollbackGls)
			{
				if (!this.DataObject.InPostGLSSwitchState())
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorRollbackGlsExpectsPostGlsState(this.Identity.ToString())), ErrorCategory.InvalidOperation, this.DataObject.Identity);
				}
				if (!base.Fields.IsModified(TenantRelocationRequestSchema.RelocationStateRequested))
				{
					this.RelocationStateRequested = RelocationStateRequestedByCmdlet.SynchronizationFinishedFullSync;
				}
				else if (this.RelocationStateRequested != RelocationStateRequestedByCmdlet.SynchronizationFinishedFullSync)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorRollbackGlsExpectsSynchronizationFinishedFullSync(this.Identity.ToString())), ErrorCategory.InvalidOperation, this.DataObject.Identity);
				}
			}
			if (base.Fields.IsModified(TenantRelocationRequestSchema.RelocationStateRequested) && (this.AutoCompletionEnabled || (!base.Fields.IsModified(TenantRelocationRequestSchema.AutoCompletionEnabled) && this.DataObject.AutoCompletionEnabled)))
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorRelocationStateRequestedIsNotAllowed(this.Identity.ToString())), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			if (base.Fields.IsModified(TenantRelocationRequestSchema.AutoCompletionEnabled) && !this.AutoCompletionEnabled && !base.Fields.IsModified(TenantRelocationRequestSchema.RelocationStateRequested))
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorRelocationStateRequestedIsMandatory(this.Identity.ToString())), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			if (base.Fields.IsModified(TenantRelocationRequestSchema.RelocationStateRequested))
			{
				if (this.RollbackGls || (this.RelocationStateRequested == RelocationStateRequestedByCmdlet.SynchronizationFinishedFullSync && this.DataObject.InLockdownBeforeGLSSwitchState()) || (this.RelocationStateRequested == RelocationStateRequestedByCmdlet.SynchronizationFinishedFullSync && this.DataObject.RelocationStatusDetailsSource == RelocationStatusDetailsSource.SynchronizationFinishedDeltaSync))
				{
					this.applyTransitionFromCmdlet = true;
				}
				else if (this.RelocationStateRequested < (RelocationStateRequestedByCmdlet)this.DataObject.RelocationStatusDetailsSource)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorRelocationStateRequestedIsTooLow(this.Identity.ToString(), this.RelocationStateRequested.ToString(), this.DataObject.RelocationStatusDetailsSource.ToString())), ErrorCategory.InvalidOperation, this.DataObject.Identity);
				}
				if (this.DataObject.RelocationStateRequested == Microsoft.Exchange.Data.Directory.SystemConfiguration.RelocationStateRequested.Cleanup && this.RelocationStateRequested != (RelocationStateRequestedByCmdlet)this.DataObject.RelocationStateRequested)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorCleanupRequestedNoRollback(this.Identity.ToString(), this.RelocationStateRequested.ToString(), this.DataObject.RelocationStateRequested.ToString())), ErrorCategory.InvalidOperation, this.DataObject.Identity);
				}
				if (this.RelocationStateRequested == RelocationStateRequestedByCmdlet.Cleanup)
				{
					if (this.DataObject.RelocationStatusDetailsSource != RelocationStatusDetailsSource.RetiredUpdatedTargetForest)
					{
						base.WriteError(new InvalidOperationException(Strings.ErrorCleanupRequestedAtWrongStage(this.Identity.ToString(), this.RelocationStateRequested.ToString(), this.DataObject.RelocationStatusDetailsSource.ToString(), RelocationStatusDetailsSource.RetiredUpdatedTargetForest.ToString())), ErrorCategory.InvalidOperation, this.DataObject.Identity);
					}
					if (!this.DataObject.IsRetiredSourceHoldTimedOut())
					{
						base.WriteError(new InvalidOperationException(Strings.ErrorSourceHoldNotTimedOut(this.Identity.ToString(), TenantRelocationRequest.WaitTimeBeforeRemoveSourceReplicaDays.ToString(), this.DataObject.RetiredStartTime.Value.ToUniversalTime().ToString())), ErrorCategory.InvalidOperation, this.DataObject.Identity);
					}
				}
			}
			if ((base.Fields.IsModified("ResumeParameter") && !this.Resume) || (base.Fields.IsModified("SuspendParameter") && !this.Suspend))
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorSuspendAndResumeDontSupportFalse), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			if (this.Resume && !this.DataObject.Suspended)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorCannotResumeIfNotSuspended), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			string forestFQDN = this.DataObject.OrganizationId.PartitionId.ForestFQDN;
			if (this.applyTransitionFromCmdlet)
			{
				LocalizedString? localizedString = null;
				if (!this.DataObject.AutoCompletionEnabled && this.DataObject.RelocationStatusDetailsSource != (RelocationStatusDetailsSource)this.DataObject.RelocationStateRequested)
				{
					localizedString = new LocalizedString?(Strings.WarningApplyingTransitionWhileRelocationStatusNotReachedStateRequested(this.Identity.ToString(), this.DataObject.RelocationStateRequested.ToString(), this.DataObject.RelocationStatusDetailsSource.ToString()));
				}
				else if (this.DataObject.AutoCompletionEnabled && this.DataObject.RelocationStatusDetailsSource != RelocationStatusDetailsSource.RetiredUpdatedTargetForest)
				{
					localizedString = new LocalizedString?(Strings.WarningApplyingTransitionWhileRelocationStatusNotReachedStateRequested(this.Identity.ToString(), this.DataObject.RelocationStateRequested.ToString(), this.DataObject.RelocationStatusDetailsSource.ToString()));
				}
				if (localizedString != null && !base.ShouldContinue(localizedString.Value))
				{
					return;
				}
				if (this.RollbackGls && this.DataObject.RelocationStatusDetailsSource == RelocationStatusDetailsSource.RetiredUpdatedTargetForest)
				{
					localizedString = new LocalizedString?(Strings.WarningPossibleDataLossWithGlsRollback(this.Identity.ToString(), this.DataObject.TargetForest, forestFQDN));
					if (!base.ShouldContinue(localizedString.Value))
					{
						return;
					}
				}
			}
			if (this.Suspend && this.DataObject.RelocationStatusDetailsSource > RelocationStatusDetailsSource.SynchronizationStartedDeltaSync)
			{
				LocalizedString? localizedString2 = new LocalizedString?(Strings.WarningSuspendSupportedOnlyDuringSync(this.DataObject.RelocationStatusDetailsSource.ToString(), RelocationStatusDetailsSource.SynchronizationStartedFullSync.ToString(), RelocationStatusDetailsSource.SynchronizationStartedDeltaSync.ToString()));
				if (!base.ShouldContinue(localizedString2.Value))
				{
					return;
				}
			}
			this.DataObject.RelocationLastError = RelocationError.None;
			if (base.Fields.IsChanged(TenantRelocationRequestSchema.RelocationStateRequested))
			{
				this.DataObject.RelocationStateRequested = (RelocationStateRequested)this.RelocationStateRequested;
			}
			if (base.Fields.IsChanged(TenantRelocationRequestSchema.LargeTenantModeEnabled))
			{
				this.DataObject.LargeTenantModeEnabled = this.LargeTenantModeEnabled;
			}
			if (base.Fields.IsChanged(TenantRelocationRequestSchema.AutoCompletionEnabled))
			{
				this.DataObject.AutoCompletionEnabled = this.AutoCompletionEnabled;
				if (this.AutoCompletionEnabled)
				{
					this.DataObject.RelocationStateRequested = Microsoft.Exchange.Data.Directory.SystemConfiguration.RelocationStateRequested.None;
				}
			}
			if (base.Fields.IsChanged(TenantRelocationRequestSchema.SafeLockdownSchedule))
			{
				this.DataObject.SafeLockdownSchedule = this.SafeLockdownSchedule;
			}
			if (this.Suspend)
			{
				this.DataObject.Suspended = true;
			}
			else if (this.Resume)
			{
				this.DataObject.Suspended = false;
			}
			if (this.applyTransitionFromCmdlet)
			{
				Exception ex;
				TenantRelocationRequest.PopulatePresentationObject(this.DataObject, null, out ex);
				if (ex != null)
				{
					throw ex;
				}
				RelocationStatusDetails relocationStatusDetailsRaw = this.DataObject.RelocationStatusDetailsRaw;
				this.DataObject.RelocationStatusDetailsRaw = RelocationStatusDetails.SynchronizationFinishedFullSync;
				this.DataObject.AutoCompletionEnabled = false;
				if (this.RollbackGls)
				{
					this.RevertGlsAccountForestToSource(forestFQDN);
					this.RevertTargetTenantStateToArriving();
					this.DataObject.IncrementTransitionCounter(TenantRelocationTransition.RetiredToSync);
					this.DataObject.OrganizationStatus = OrganizationStatus.Active;
					TenantRelocationRequest.SetRelocationCompletedOnOU((ITenantConfigurationSession)base.DataSession, this.DataObject.OrganizationId);
				}
				else
				{
					TenantRelocationTransition transition;
					if (relocationStatusDetailsRaw == RelocationStatusDetails.SynchronizationFinishedDeltaSync)
					{
						transition = TenantRelocationTransition.DeltaSyncToSync;
					}
					else
					{
						transition = TenantRelocationTransition.LockdownToSync;
					}
					this.DataObject.IncrementTransitionCounter(transition);
				}
			}
			if (this.ResetTransitionCounter)
			{
				this.DataObject.TransitionCounter = new MultiValuedProperty<TransitionCount>();
			}
			if (this.ResetPermanentError)
			{
				this.DataObject.RelocationLastError = RelocationError.None;
			}
			if (this.ResetStartSyncTime)
			{
				this.DataObject.LastSuccessfulRelocationSyncStart = new DateTime?(DateTime.UtcNow);
			}
			((IDirectorySession)base.DataSession).SessionSettings.RetiredTenantModificationAllowed = true;
			base.InternalProcessRecord();
			string ridMasterName = ForestTenantRelocationsCache.GetRidMasterName(new PartitionId(forestFQDN));
			if (this.DataObject.OriginatingServer != ridMasterName)
			{
				this.WriteWarning(Strings.WarningShouldWriteToRidMaster(this.DataObject.OriginatingServer, ridMasterName));
			}
		}

		private void RevertGlsAccountForestToSource(string sourceForest)
		{
			if (!ADSessionSettings.IsGlsDisabled)
			{
				Guid externalDirectoryOrganizationId = new Guid(this.DataObject.ExternalDirectoryOrganizationId);
				GlsDirectorySession glsDirectorySession = new GlsDirectorySession();
				glsDirectorySession.SetAccountForest(externalDirectoryOrganizationId, sourceForest, this.DataObject.OrganizationId.OrganizationalUnit.Name);
			}
		}

		private void RevertTargetTenantStateToArriving()
		{
			ITenantConfigurationSession tenantConfigurationSession = SetTenantRelocationRequest.CreateWritableTenantSession(this.DataObject.TargetOrganizationId);
			TenantRelocationRequest tenantRelocationRequest = tenantConfigurationSession.Read<TenantRelocationRequest>(this.DataObject.TargetOrganizationId.ConfigurationUnit);
			tenantRelocationRequest.RelocationStatusDetailsRaw = RelocationStatusDetails.Arriving;
			tenantRelocationRequest.OrganizationStatus = OrganizationStatus.PendingArrival;
			tenantConfigurationSession.Save(tenantRelocationRequest);
		}

		private static ITenantConfigurationSession CreateWritableTenantSession(OrganizationId organizationId)
		{
			string ridMasterName = ForestTenantRelocationsCache.GetRidMasterName(organizationId.PartitionId);
			ADSessionSettings adsessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId, false);
			adsessionSettings.RetiredTenantModificationAllowed = true;
			adsessionSettings.TenantConsistencyMode = TenantConsistencyMode.IncludeRetiredTenants;
			return DirectorySessionFactory.Default.CreateTenantConfigurationSession(ridMasterName, false, ConsistencyMode.PartiallyConsistent, adsessionSettings, 541, "CreateWritableTenantSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Relocation\\SetTenantRelocationRequest.cs");
		}

		private const string DefaultParameterSet = "DefaultParameterSet";

		private const string SuspendParameterSet = "SuspendParameterSet";

		private const string ResumeParameterSet = "ResumeParameterSet";

		private const string ResetTransitionCounterParameterSet = "ResetTransitionCounterParameterSet";

		private const string ResetPermanentErrorParameterSet = "ResetPermanentErrorParameterSet";

		private const string ResetStartSyncTimeParameter = "ResetStartSyncTimeParameter";

		private const string SuspendParameter = "SuspendParameter";

		private const string ResumeParameter = "ResumeParameter";

		private const string RollbackGlsParameter = "RollbackGlsParameter";

		private const string ResetTransitionCounterParameter = "ResetTransitionCounterParameter";

		private const string ResetPermanentErrorParameter = "ResetPermanentErrorParameter";

		private bool applyTransitionFromCmdlet;
	}
}
