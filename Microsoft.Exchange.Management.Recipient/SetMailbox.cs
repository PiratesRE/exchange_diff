using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics.LatencyDetection;
using Microsoft.Exchange.InfoWorker.Common.ELC;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Management.Tasks.UM;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.FaultInjection;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Set", "Mailbox", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetMailbox : SetMailboxBase<MailboxIdParameter, Mailbox>
	{
		[Parameter(Mandatory = true, ParameterSetName = "RemoveAggregatedMailbox", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		[Parameter(Mandatory = true, ParameterSetName = "AddAggregatedMailbox", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		[Parameter(Mandatory = true, ParameterSetName = "Identity", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		public override MailboxIdParameter Identity
		{
			get
			{
				return (MailboxIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public new MailboxPlanIdParameter MailboxPlan
		{
			get
			{
				return base.MailboxPlan;
			}
			set
			{
				base.MailboxPlan = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool EnableRoomMailboxAccount
		{
			get
			{
				return base.Fields["EnableRoomMailboxAccount"] != null && (bool)base.Fields["EnableRoomMailboxAccount"];
			}
			set
			{
				base.Fields["EnableRoomMailboxAccount"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SecureString RoomMailboxPassword
		{
			private get
			{
				return (SecureString)base.Fields["RoomMailboxPassword"];
			}
			set
			{
				base.Fields["RoomMailboxPassword"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter SkipMailboxProvisioningConstraintValidation
		{
			get
			{
				return (SwitchParameter)(base.Fields["SkipMailboxProvisioningConstraintValidation"] ?? false);
			}
			set
			{
				base.Fields["SkipMailboxProvisioningConstraintValidation"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MailboxProvisioningConstraint MailboxProvisioningConstraint
		{
			get
			{
				return (MailboxProvisioningConstraint)base.Fields[ADRecipientSchema.MailboxProvisioningConstraint];
			}
			set
			{
				base.Fields[ADRecipientSchema.MailboxProvisioningConstraint] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<MailboxProvisioningConstraint> MailboxProvisioningPreferences
		{
			get
			{
				return (MultiValuedProperty<MailboxProvisioningConstraint>)base.Fields[ADRecipientSchema.MailboxProvisioningPreferences];
			}
			set
			{
				base.Fields[ADRecipientSchema.MailboxProvisioningPreferences] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter EvictLiveId { get; set; }

		[Parameter(Mandatory = false)]
		public bool RequireSecretQA { get; set; }

		[Parameter(Mandatory = false)]
		public NetID OriginalNetID
		{
			get
			{
				return (NetID)base.Fields["OriginalNetID"];
			}
			set
			{
				base.Fields["OriginalNetID"] = value;
			}
		}

		[ValidateNotNull]
		[Parameter(Mandatory = false)]
		public DatabaseIdParameter Database
		{
			get
			{
				return (DatabaseIdParameter)base.Fields[ADMailboxRecipientSchema.Database];
			}
			set
			{
				base.Fields[ADMailboxRecipientSchema.Database] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<EnhancedTimeSpan> LitigationHoldDuration
		{
			get
			{
				return (Unlimited<EnhancedTimeSpan>)base.Fields[ADRecipientSchema.LitigationHoldDuration];
			}
			set
			{
				base.Fields[ADRecipientSchema.LitigationHoldDuration] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool UMDataStorage
		{
			get
			{
				return (bool)base.Fields["UMDataStorage"];
			}
			set
			{
				base.Fields["UMDataStorage"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool UMGrammar
		{
			get
			{
				return (bool)base.Fields["UMGrammar"];
			}
			set
			{
				base.Fields["UMGrammar"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OABGen
		{
			get
			{
				return (bool)base.Fields["OABGen"];
			}
			set
			{
				base.Fields["OABGen"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool GMGen
		{
			get
			{
				return (bool)base.Fields["GMGen"];
			}
			set
			{
				base.Fields["GMGen"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ClientExtensions
		{
			get
			{
				return (bool)base.Fields["ClientExtensions"];
			}
			set
			{
				base.Fields["ClientExtensions"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MailRouting
		{
			get
			{
				return (bool)base.Fields["MailRouting"];
			}
			set
			{
				base.Fields["MailRouting"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool Management
		{
			get
			{
				return (bool)base.Fields["Management"];
			}
			set
			{
				base.Fields["Management"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool TenantUpgrade
		{
			get
			{
				return (bool)base.Fields["TenantUpgrade"];
			}
			set
			{
				base.Fields["TenantUpgrade"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool Migration
		{
			get
			{
				return (bool)base.Fields["Migration"];
			}
			set
			{
				base.Fields["Migration"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MessageTracking
		{
			get
			{
				return (bool)base.Fields["MessageTracking"];
			}
			set
			{
				base.Fields["MessageTracking"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OMEncryption
		{
			get
			{
				return (bool)base.Fields["OMEncryption"];
			}
			set
			{
				base.Fields["OMEncryption"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PstProvider
		{
			get
			{
				return (bool)base.Fields["PstProvider"];
			}
			set
			{
				base.Fields["PstProvider"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool SuiteServiceStorage
		{
			get
			{
				return (bool)base.Fields["SuiteServiceStorage"];
			}
			set
			{
				base.Fields["SuiteServiceStorage"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SecureString OldPassword { get; set; }

		[Parameter(Mandatory = false)]
		public SecureString NewPassword { get; set; }

		[Parameter(Mandatory = false)]
		[ValidateNotNull]
		public DatabaseIdParameter ArchiveDatabase
		{
			get
			{
				return (DatabaseIdParameter)base.Fields[ADUserSchema.ArchiveDatabase];
			}
			set
			{
				base.Fields[ADUserSchema.ArchiveDatabase] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public OrganizationalUnitIdParameter QueryBaseDN
		{
			get
			{
				return (OrganizationalUnitIdParameter)base.Fields[ADUserSchema.QueryBaseDN];
			}
			set
			{
				base.Fields[ADUserSchema.QueryBaseDN] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter DefaultPublicFolderMailbox
		{
			get
			{
				return (RecipientIdParameter)base.Fields[MailboxSchema.DefaultPublicFolderMailboxValue];
			}
			set
			{
				base.Fields[MailboxSchema.DefaultPublicFolderMailboxValue] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? MailboxMessagesPerFolderCountWarningQuota
		{
			get
			{
				return (int?)base.Fields["MailboxMessagesPerFolderCountWarningQuota"];
			}
			set
			{
				base.Fields["MailboxMessagesPerFolderCountWarningQuota"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? MailboxMessagesPerFolderCountReceiveQuota
		{
			get
			{
				return (int?)base.Fields["MailboxMessagesPerFolderCountReceiveQuota"];
			}
			set
			{
				base.Fields["MailboxMessagesPerFolderCountReceiveQuota"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? DumpsterMessagesPerFolderCountWarningQuota
		{
			get
			{
				return (int?)base.Fields["DumpsterMessagesPerFolderCountWarningQuota"];
			}
			set
			{
				base.Fields["DumpsterMessagesPerFolderCountWarningQuota"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? DumpsterMessagesPerFolderCountReceiveQuota
		{
			get
			{
				return (int?)base.Fields["DumpsterMessagesPerFolderCountReceiveQuota"];
			}
			set
			{
				base.Fields["DumpsterMessagesPerFolderCountReceiveQuota"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? FolderHierarchyChildrenCountWarningQuota
		{
			get
			{
				return (int?)base.Fields["FolderHierarchyChildrenCountWarningQuota"];
			}
			set
			{
				base.Fields["FolderHierarchyChildrenCountWarningQuota"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? FolderHierarchyChildrenCountReceiveQuota
		{
			get
			{
				return (int?)base.Fields["FolderHierarchyChildrenCountReceiveQuota"];
			}
			set
			{
				base.Fields["FolderHierarchyChildrenCountReceiveQuota"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? FolderHierarchyDepthWarningQuota
		{
			get
			{
				return (int?)base.Fields["FolderHierarchyDepthWarningQuota"];
			}
			set
			{
				base.Fields["FolderHierarchyDepthWarningQuota"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? FolderHierarchyDepthReceiveQuota
		{
			get
			{
				return (int?)base.Fields["FolderHierarchyDepthReceiveQuota"];
			}
			set
			{
				base.Fields["FolderHierarchyDepthReceiveQuota"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? FoldersCountWarningQuota
		{
			get
			{
				return (int?)base.Fields["FoldersCountWarningQuota"];
			}
			set
			{
				base.Fields["FoldersCountWarningQuota"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? FoldersCountReceiveQuota
		{
			get
			{
				return (int?)base.Fields["FoldersCountReceiveQuota"];
			}
			set
			{
				base.Fields["FoldersCountReceiveQuota"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? ExtendedPropertiesCountQuota
		{
			get
			{
				return (int?)base.Fields["ExtendedPropertiesCountQuota"];
			}
			set
			{
				base.Fields["ExtendedPropertiesCountQuota"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Guid? MailboxContainerGuid
		{
			get
			{
				return (Guid?)base.Fields[ADUserSchema.MailboxContainerGuid];
			}
			set
			{
				base.Fields[ADUserSchema.MailboxContainerGuid] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "AddAggregatedMailbox")]
		public SwitchParameter AddAggregatedAccount
		{
			get
			{
				return (SwitchParameter)(base.Fields["AddAggregatedAccount"] ?? false);
			}
			set
			{
				base.Fields["AddAggregatedAccount"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "RemoveAggregatedMailbox")]
		public SwitchParameter RemoveAggregatedAccount
		{
			get
			{
				return (SwitchParameter)(base.Fields["RemoveAggregatedAccount"] ?? false);
			}
			set
			{
				base.Fields["RemoveAggregatedAccount"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "AddAggregatedMailbox")]
		[Parameter(Mandatory = true, ParameterSetName = "RemoveAggregatedMailbox")]
		public Guid AggregatedMailboxGuid
		{
			get
			{
				return (Guid)(base.Fields["AggregatedMailboxGuid"] ?? Guid.Empty);
			}
			set
			{
				base.Fields["AggregatedMailboxGuid"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public CrossTenantObjectId UnifiedMailbox
		{
			get
			{
				return (CrossTenantObjectId)base.Fields[ADUserSchema.UnifiedMailbox];
			}
			set
			{
				base.Fields[ADUserSchema.UnifiedMailbox] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter ForestWideDomainControllerAffinityByExecutingUser
		{
			get
			{
				return (SwitchParameter)(base.Fields["ForestWideDomainControllerAffinityByExecutingUser"] ?? false);
			}
			set
			{
				base.Fields["ForestWideDomainControllerAffinityByExecutingUser"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MessageCopyForSentAsEnabled
		{
			get
			{
				return (bool)(base.Fields["MessageCopyForSentAsEnabled"] ?? false);
			}
			set
			{
				base.Fields["MessageCopyForSentAsEnabled"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MessageCopyForSendOnBehalfEnabled
		{
			get
			{
				return (bool)(base.Fields["MessageCopyForSendOnBehalfEnabled"] ?? false);
			}
			set
			{
				base.Fields["MessageCopyForSendOnBehalfEnabled"] = value;
			}
		}

		protected override IConfigurable ResolveDataObject()
		{
			ADRecipient adrecipient = (ADRecipient)base.ResolveDataObject();
			if (MailboxTaskHelper.ExcludeArbitrationMailbox(adrecipient, base.Arbitration) || MailboxTaskHelper.ExcludePublicFolderMailbox(adrecipient, base.PublicFolder) || MailboxTaskHelper.ExcludeMailboxPlan(adrecipient, false) || MailboxTaskHelper.ExcludeAuditLogMailbox(adrecipient, base.AuditLog))
			{
				base.WriteError(new ManagementObjectNotFoundException(base.GetErrorMessageObjectNotFound(this.Identity.ToString(), typeof(ADUser).ToString(), (base.DataSession != null) ? base.DataSession.Source : null)), ExchangeErrorCategory.Client, this.Identity);
			}
			return adrecipient;
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			if (base.Fields.IsModified(MailboxSchema.RetentionPolicy) && base.RetentionPolicy != null && SharedConfiguration.IsDehydratedConfiguration(base.CurrentOrganizationId))
			{
				base.WriteError(new InvalidOperationInDehydratedContextException(Strings.ErrorLinkOpOnDehydratedTenant("RetentionPolicy")), ExchangeErrorCategory.Client, null);
			}
			base.InternalBeginProcessing();
			if (base.Fields.IsModified("AggregatedMailboxGuid"))
			{
				if (!this.AddAggregatedAccount.IsPresent && !this.RemoveAggregatedAccount.IsPresent)
				{
					base.ThrowTerminatingError(new TaskArgumentException(Strings.ErrorNoAggregatedAccountOperationSpecified, null), ExchangeErrorCategory.Client, null);
				}
				else if (this.AddAggregatedAccount.IsPresent && this.RemoveAggregatedAccount.IsPresent)
				{
					base.ThrowTerminatingError(new TaskArgumentException(Strings.ErrorMoreThanOneAggregatedAccountOperationSpecified, null), ExchangeErrorCategory.Client, null);
				}
			}
			else if (this.AddAggregatedAccount.IsPresent || this.RemoveAggregatedAccount.IsPresent)
			{
				base.ThrowTerminatingError(new TaskArgumentException(Strings.ErrorAggregatedMailboxGuidNotSpecified, null), ExchangeErrorCategory.Client, null);
			}
			Mailbox mailbox = (Mailbox)this.GetDynamicParameters();
			if (base.PublicFolder)
			{
				foreach (object obj in SetMailbox.InvalidPublicFolderParameters)
				{
					ProviderPropertyDefinition providerPropertyDefinition = obj as ProviderPropertyDefinition;
					if (base.Fields.IsModified(obj) || (providerPropertyDefinition != null && mailbox.IsModified(providerPropertyDefinition)))
					{
						base.WriteError(new TaskArgumentException(Strings.ErrorInvalidParameterForPublicFolderTasks((providerPropertyDefinition == null) ? obj.ToString() : providerPropertyDefinition.Name, "PublicFolder")), ExchangeErrorCategory.Client, this.Identity);
					}
				}
			}
			else
			{
				foreach (object obj2 in SetMailbox.PublicFolderOnlyParameters)
				{
					ProviderPropertyDefinition providerPropertyDefinition2 = obj2 as ProviderPropertyDefinition;
					if (base.Fields.IsModified(obj2) || (providerPropertyDefinition2 != null && mailbox.IsModified(providerPropertyDefinition2)))
					{
						base.WriteError(new TaskArgumentException(Strings.ErrorInvalidMandatoryParameterForPublicFolderTasks((providerPropertyDefinition2 == null) ? obj2.ToString() : providerPropertyDefinition2.Name, "PublicFolder")), ExchangeErrorCategory.Client, this.Identity);
					}
				}
			}
			if (base.Fields.IsModified(ADMailboxRecipientSchema.Database))
			{
				this.rehomeDatabase = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(this.Database, base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorMailboxDatabaseNotFound(this.Database.ToString())), new LocalizedString?(Strings.ErrorMailboxDatabaseNotUnique(this.Database.ToString())), ExchangeErrorCategory.Client);
				mailbox[ADMailboxRecipientSchema.Database] = (ADObjectId)this.rehomeDatabase.Identity;
				if (this.rehomeDatabase.Server == null)
				{
					base.WriteError(new NullServerObjectException(), ExchangeErrorCategory.ServerOperation, this.Identity);
				}
				Server server = (Server)base.GetDataObject<Server>(new ServerIdParameter(this.rehomeDatabase.Server), base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorServerNotFound(this.rehomeDatabase.Server.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(this.rehomeDatabase.Server.ToString())), ExchangeErrorCategory.Client);
				mailbox[ADMailboxRecipientSchema.ServerLegacyDN] = server.ExchangeLegacyDN;
				mailbox[ADRecipientSchema.HomeMTA] = null;
			}
			if (base.Fields.IsModified(ADUserSchema.ArchiveDatabase))
			{
				this.rehomeArchiveDatabase = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(this.ArchiveDatabase, base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorMailboxDatabaseNotFound(this.ArchiveDatabase.ToString())), new LocalizedString?(Strings.ErrorMailboxDatabaseNotUnique(this.ArchiveDatabase.ToString())), ExchangeErrorCategory.Client);
				mailbox[ADUserSchema.ArchiveDatabase] = (ADObjectId)this.rehomeArchiveDatabase.Identity;
				if (this.rehomeArchiveDatabase.Server == null)
				{
					base.WriteError(new NullServerObjectException(), ExchangeErrorCategory.ServerOperation, this.Identity);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void ResolveLocalSecondaryIdentities()
		{
			base.ResolveLocalSecondaryIdentities();
			if (base.Fields.IsChanged(ADUserSchema.QueryBaseDN) && this.QueryBaseDN != null)
			{
				this.querybaseDNId = RecipientTaskHelper.ResolveOrganizationalUnitInOrganization(this.QueryBaseDN, this.ConfigurationSession, base.CurrentOrganizationId, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ExchangeOrganizationalUnit>), ExchangeErrorCategory.ServerOperation, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.ThrowTerminatingError)).Id;
			}
			Mailbox mailbox = (Mailbox)this.GetDynamicParameters();
			if (base.Fields.IsModified(ADRecipientSchema.DefaultPublicFolderMailbox))
			{
				if (this.DefaultPublicFolderMailbox != null)
				{
					ADUser aduser = (ADUser)base.GetDataObject<ADUser>(this.DefaultPublicFolderMailbox, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorMailboxAddressNotFound(this.DefaultPublicFolderMailbox.ToString())), new LocalizedString?(Strings.ErrorMailboxAddressNotFound(this.DefaultPublicFolderMailbox.ToString())), ExchangeErrorCategory.Client);
					ADObjectId id = aduser.Id;
					if (aduser == null || aduser.RecipientTypeDetails != RecipientTypeDetails.PublicFolderMailbox)
					{
						IConfigurationSession tenantLocalConfigSession = RecipientTaskHelper.GetTenantLocalConfigSession(base.CurrentOrganizationId, base.ExecutingUserOrganizationId, base.RootOrgContainerId);
						Organization orgContainer = tenantLocalConfigSession.GetOrgContainer();
						if (orgContainer.RemotePublicFolderMailboxes == null || !orgContainer.RemotePublicFolderMailboxes.Contains(id))
						{
							base.WriteError(new ObjectNotFoundException(Strings.PublicFolderMailboxNotFound), ExchangeErrorCategory.Client, aduser);
						}
					}
					mailbox[ADRecipientSchema.DefaultPublicFolderMailbox] = id;
					return;
				}
				mailbox[ADRecipientSchema.DefaultPublicFolderMailbox] = null;
			}
		}

		protected override ITaskModuleFactory CreateTaskModuleFactory()
		{
			return new SetMailboxTaskModuleFactory();
		}

		protected override void InternalValidate()
		{
			this.latencyContext = ProvisioningPerformanceHelper.StartLatencyDetection(this);
			base.InternalValidate();
			this.ValidateLitigationHoldLicenseCheck();
			RecipientTypeDetails recipientTypeDetails = this.DataObject.RecipientTypeDetails;
			MailboxTaskHelper.ValidateRoomMailboxPasswordParameterCanOnlyBeUsedWithEnableRoomMailboxPassword(base.Fields.IsModified("RoomMailboxPassword"), base.Fields.IsModified("EnableRoomMailboxAccount"), new Task.ErrorLoggerDelegate(base.WriteError));
			if (base.Fields.IsModified("EnableRoomMailboxAccount"))
			{
				this.ValidateEnableRoomMailboxAccountParameter();
			}
			this.ValidateParametersForChangingPassword();
			bool flag = this.NewPassword != null && this.NewPassword.Length > 0;
			bool flag2 = base.CurrentTaskContext.ExchangeRunspaceConfig != null && base.CurrentTaskContext.ExchangeRunspaceConfig.IsAppPasswordUsed;
			if ((base.UserSpecifiedParameters.IsModified(UserSchema.ResetPasswordOnNextLogon) || base.IsChangingOnPassword || flag) && flag2)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorChangePasswordForAppPasswordAccount), ExchangeErrorCategory.Client, this.Identity);
			}
			if (base.IsChangingOnPassword || (this.EnableRoomMailboxAccount && this.RoomMailboxPassword != null))
			{
				this.ValidatePassword(recipientTypeDetails);
			}
			if (this.rehomeDatabase != null && this.originalDatabase != null)
			{
				this.ValidateCrossVersionRehoming(this.originalDatabase, this.rehomeDatabase);
			}
			if (this.rehomeArchiveDatabase != null && this.originalArchiveDatabase != null)
			{
				this.ValidateCrossVersionRehoming(this.originalArchiveDatabase, this.rehomeArchiveDatabase);
			}
			MailboxTaskHelper.EnsureUserSpecifiedDatabaseMatchesMailboxProvisioningConstraint(this.rehomeDatabase, this.rehomeArchiveDatabase, base.Fields, this.DataObject.MailboxProvisioningConstraint, new Task.ErrorLoggerDelegate(base.WriteError), ADMailboxRecipientSchema.Database);
			if (this.DataObject.IsSoftDeleted)
			{
				foreach (object obj in base.UserSpecifiedParameters.Keys)
				{
					string item = (string)obj;
					if (!SetMailbox.LitigationHoldEnabledParameters.Contains(item))
					{
						base.WriteError(new TaskArgumentException(Strings.ErrorSoftDeletedMailboxCanOnlyChangeLitigationHoldEnabled), ExchangeErrorCategory.Client, this.Identity);
					}
				}
			}
			if ((this.DataObject.IsModified(MailboxSchema.ProhibitSendReceiveQuota) || this.DataObject.IsModified(MailboxSchema.ProhibitSendQuota) || this.DataObject.IsModified(MailboxSchema.IssueWarningQuota)) && this.DataObject.UseDatabaseQuotaDefaults != null && this.DataObject.UseDatabaseQuotaDefaults.Value)
			{
				this.WriteWarning(Strings.WarnAboutSetDatabaseDefaults);
			}
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled && base.CurrentTaskContext.ExchangeRunspaceConfig != null && "LiveIdBasic".Equals(base.CurrentTaskContext.ExchangeRunspaceConfig.AuthenticationType, StringComparison.OrdinalIgnoreCase))
			{
				if (this.DataObject.IsModified(MailboxSchema.UseDatabaseQuotaDefaults) && this.DataObject.UseDatabaseQuotaDefaults.Value)
				{
					base.WriteError(new TaskArgumentException(Strings.ErrorUseDatabaseQuotaDefaultsCanOnlySetToFalse), ExchangeErrorCategory.Client, this.Identity);
				}
				if (this.DataObject.IsModified(MailboxSchema.UseDatabaseRetentionDefaults) && this.DataObject.UseDatabaseRetentionDefaults)
				{
					base.WriteError(new TaskArgumentException(Strings.ErrorUseDatabaseRetentionDefaultsCanOnlySetToFalse), ExchangeErrorCategory.Client, this.Identity);
				}
			}
			this.ValidateOrganizationCapabilities();
			this.ValidateMailboxShapeFeatureVersion();
			this.ValidateCopySentItemToSenderFlags();
		}

		private void ValidateCopySentItemToSenderFlags()
		{
			if (this.DataObject.RecipientTypeDetails != RecipientTypeDetails.SharedMailbox && this.DataObject.RecipientTypeDetails != RecipientTypeDetails.RemoteSharedMailbox)
			{
				if (base.Fields.IsModified("MessageCopyForSentAsEnabled"))
				{
					base.WriteError(new TaskArgumentException(Strings.ErrorMessageCopyForSentAsEnabledCanOnlySetOnSharedMailbox), ExchangeErrorCategory.Client, this.Identity);
				}
				if (base.Fields.IsModified("MessageCopyForSendOnBehalfEnabled"))
				{
					base.WriteError(new TaskArgumentException(Strings.ErrorMessageCopyForSendOnBehalfEnabledCanOnlySetOnSharedMailbox), ExchangeErrorCategory.Client, this.Identity);
				}
			}
		}

		private void ValidateCrossVersionRehoming(ADObjectId originalDatabase, MailboxDatabase rehomeDatabase)
		{
			MailboxDatabase mailboxDatabase = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(new DatabaseIdParameter(originalDatabase), base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorMailboxDatabaseNotFound(originalDatabase.ToString())), new LocalizedString?(Strings.ErrorMailboxDatabaseNotUnique(originalDatabase.ToString())), ExchangeErrorCategory.Client);
			ActiveManager activeManagerInstance = ActiveManager.GetActiveManagerInstance();
			DatabaseLocationInfo databaseLocationInfo = MailboxTaskHelper.GetDatabaseLocationInfo(mailboxDatabase, activeManagerInstance, new Task.ErrorLoggerDelegate(base.WriteError));
			if (databaseLocationInfo == null)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorReHomeOriginalDatabaseLocationNotFound(mailboxDatabase.ToString())), ExchangeErrorCategory.Client, null);
			}
			DatabaseLocationInfo databaseLocationInfo2 = MailboxTaskHelper.GetDatabaseLocationInfo(rehomeDatabase, activeManagerInstance, new Task.ErrorLoggerDelegate(base.WriteError));
			if (databaseLocationInfo2 == null)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorReHomeTargetDatabaseLocationNotFound(rehomeDatabase.ToString())), ExchangeErrorCategory.Client, null);
			}
			if (new ServerVersion(databaseLocationInfo.ServerVersion).Major != new ServerVersion(databaseLocationInfo2.ServerVersion).Major && !base.ShouldContinue(Strings.ConfirmationMessageSetMailboxCrossVersionRehoming(this.Identity.ToString(), mailboxDatabase.Identity.ToString(), rehomeDatabase.Identity.ToString())))
			{
				base.WriteError(new TaskArgumentException(Strings.ErrorSetMailboxCrossVersionRehoming, null), ExchangeErrorCategory.Client, this.Identity);
			}
		}

		private void ValidateLitigationHoldLicenseCheck()
		{
			if (base.UserSpecifiedParameters.Contains("LitigationHoldEnabled") && (this.originalLitigationHold ^ this.DataObject.LitigationHoldEnabled) && this.DataObject.LitigationHoldEnabled && base.ExchangeRunspaceConfig != null && !base.ExchangeRunspaceConfig.IsCmdletAllowedInScope("Set-Mailbox", new string[]
			{
				"LitigationHoldDate"
			}, this.DataObject, ScopeLocation.RecipientWrite))
			{
				base.WriteError(new RecipientTaskException(DirectoryStrings.LitigationHold_License_Violation(this.Identity.ToString(), "Set-Mailbox", "LitigationHoldEnabled", "")), ExchangeErrorCategory.Client, this.Identity);
			}
		}

		private void ValidateParametersForChangingPassword()
		{
			bool flag = this.OldPassword != null && this.OldPassword.Length > 0;
			bool flag2 = this.NewPassword != null && this.NewPassword.Length > 0;
			if (base.IsChangingOnPassword && flag2)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorConflictingRestrictionParameters("NewPassword", "Password")), ExchangeErrorCategory.Client, this.Identity);
			}
			if ((!flag && flag2) || (flag && !flag2))
			{
				base.WriteError(new TaskArgumentException(Strings.ErrorChangePasswordRequiresOldPasswordNewPassword, null), ExchangeErrorCategory.Client, this.Identity);
			}
		}

		private void ValidatePassword(RecipientTypeDetails recipientTypeDetails)
		{
			if (!base.Fields.IsModified("EnableRoomMailboxAccount") && (this.DataObject.IsResource || recipientTypeDetails == RecipientTypeDetails.SharedMailbox))
			{
				base.WriteError(new TaskArgumentException(Strings.ErrorSetPasswordForLogonDisabledAccount, null), ExchangeErrorCategory.Client, this.Identity);
			}
			if (!base.HasSetPasswordPermission && VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).CmdletInfra.SetPasswordWithoutOldPassword.Enabled)
			{
				base.WriteError(new TaskArgumentException(Strings.ErrorSetPasswordWithoutPermission, null), ExchangeErrorCategory.Client, this.Identity);
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			IConfigurable configurable = base.PrepareDataObject();
			ADUser aduser = (ADUser)configurable;
			if ((this.originalRetentionHold ^ aduser.RetentionHoldEnabled) || aduser.IsModified(ADUserSchema.RetentionComment) || aduser.IsModified(ADUserSchema.RetentionUrl))
			{
				CmdletProxy.ThrowExceptionIfProxyIsNeeded(base.CurrentTaskContext, aduser, false, this.ConfirmationMessage, new CmdletProxyInfo.ChangeCmdletProxyParametersDelegate(CmdletProxy.AppendForceToProxyCmdlet));
			}
			if (!this.SkipMailboxProvisioningConstraintValidation)
			{
				if (this.MailboxProvisioningConstraint != null)
				{
					MailboxTaskHelper.ValidateMailboxProvisioningConstraintEntries(new MailboxProvisioningConstraint[]
					{
						this.MailboxProvisioningConstraint
					}, base.DomainController, delegate(string message)
					{
						base.WriteVerbose(new LocalizedString(message));
					}, new Task.ErrorLoggerDelegate(base.WriteError));
				}
				if (this.MailboxProvisioningPreferences != null)
				{
					MailboxTaskHelper.ValidateMailboxProvisioningConstraintEntries(this.MailboxProvisioningPreferences, base.DomainController, delegate(string message)
					{
						base.WriteVerbose(new LocalizedString(message));
					}, new Task.ErrorLoggerDelegate(base.WriteError));
				}
			}
			if (base.Fields.IsModified(ADRecipientSchema.MailboxProvisioningConstraint))
			{
				aduser.MailboxProvisioningConstraint = this.MailboxProvisioningConstraint;
			}
			if (base.Fields.IsModified(ADRecipientSchema.MailboxProvisioningPreferences))
			{
				aduser.MailboxProvisioningPreferences = this.MailboxProvisioningPreferences;
			}
			if (base.Fields.IsModified("MessageCopyForSentAsEnabled"))
			{
				this.DataObject[MailboxSchema.MessageCopyForSentAsEnabled] = this.MessageCopyForSentAsEnabled;
			}
			if (base.Fields.IsModified("MessageCopyForSendOnBehalfEnabled"))
			{
				this.DataObject[MailboxSchema.MessageCopyForSendOnBehalfEnabled] = this.MessageCopyForSendOnBehalfEnabled;
			}
			return configurable;
		}

		protected override bool IsObjectStateChanged()
		{
			return base.RemoveManagedFolderAndPolicy || (this.OldPassword != null && this.OldPassword.Length > 0) || base.IsObjectStateChanged();
		}

		protected override void InternalProcessRecord()
		{
			try
			{
				if (base.Fields.IsModified(ADMailboxRecipientSchema.Database) && false == base.Force)
				{
					if (base.PublicFolder)
					{
						if (!base.ShouldContinue(Strings.SetPublicFolderMailboxRehomeDatabaseConfirmationMessage(this.Identity.ToString())))
						{
							return;
						}
					}
					else if (!base.ShouldContinue(Strings.ConfirmationMessageSetMailboxWithDatabase(this.Identity.ToString(), this.rehomeDatabase.Identity.ToString())))
					{
						return;
					}
				}
				if (base.Fields.IsModified(ADUserSchema.ArchiveDatabase))
				{
					Guid archiveGuid = this.DataObject.ArchiveGuid;
					if (this.DataObject.ArchiveGuid == Guid.Empty)
					{
						base.WriteError(new TaskArgumentException(Strings.ErrorArchiveNotEnabled(this.Identity.ToString()), null), ExchangeErrorCategory.Client, this.Identity);
					}
				}
				if (!base.Fields.IsModified(ADUserSchema.ArchiveDatabase) || !(false == base.Force) || base.ShouldContinue(Strings.ConfirmationMessageSetMailboxWithDatabase(this.Identity.ToString(), this.rehomeArchiveDatabase.Identity.ToString())))
				{
					bool flag = false;
					if (this.originalLitigationHold ^ this.DataObject.LitigationHoldEnabled)
					{
						flag = true;
						ADUser dataObject = this.DataObject;
						int num;
						TaskHelper.GetRemoteServerForADUser(dataObject, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), out num);
						this.WriteWarning(Strings.WarningSetMailboxLitigationHoldDelay(COWSettings.COWCacheLifeTime.TotalMinutes));
						if (this.DataObject.LitigationHoldEnabled && base.ExchangeRunspaceConfig != null)
						{
							string text = base.ExchangeRunspaceConfig.ExecutingUserPrincipalName;
							if (string.IsNullOrEmpty(text))
							{
								SmtpAddress executingUserPrimarySmtpAddress = base.ExchangeRunspaceConfig.ExecutingUserPrimarySmtpAddress;
								if (!string.IsNullOrEmpty(base.ExchangeRunspaceConfig.ExecutingUserPrimarySmtpAddress.ToString()))
								{
									text = base.ExchangeRunspaceConfig.ExecutingUserPrimarySmtpAddress.ToString();
									base.WriteVerbose(Strings.WarningSetMailboxLitigationOwnerSMTP(text));
								}
								else
								{
									text = base.ExchangeRunspaceConfig.IdentityName;
									base.WriteVerbose(Strings.WarningSetMailboxLitigationOwnerIdentity(text));
								}
							}
							this.DataObject.LitigationHoldOwner = text;
						}
						if (dataObject != null && num >= Server.E15MinVersion)
						{
							dataObject.SetLitigationHoldEnabledWellKnownInPlaceHoldGuid(this.DataObject.LitigationHoldEnabled);
						}
					}
					if (base.Fields.IsModified(ADRecipientSchema.LitigationHoldDuration))
					{
						if (flag)
						{
							if (this.originalLitigationHold)
							{
								base.WriteError(new TaskArgumentException(Strings.WarningSetMailboxLitigationHoldDuration), ExchangeErrorCategory.Client, this.Identity);
								return;
							}
						}
						else if (!this.originalLitigationHold)
						{
							base.WriteError(new TaskArgumentException(Strings.WarningSetMailboxLitigationHoldDuration), ExchangeErrorCategory.Client, this.Identity);
							return;
						}
						if (this.LitigationHoldDuration <= EnhancedTimeSpan.Zero)
						{
							base.WriteError(new LocalizedException(Strings.ErrorSetMailboxLitigationHoldDuration), ExchangeErrorCategory.Client, this.Identity);
						}
						ADUser dataObject2 = this.DataObject;
						dataObject2.LitigationHoldDuration = new Unlimited<EnhancedTimeSpan>?(this.LitigationHoldDuration);
					}
					if (this.originalSingleItemRecovery ^ this.DataObject.SingleItemRecoveryEnabled)
					{
						this.WriteWarning(Strings.WarningSetMailboxSingleItemRecoveryDelay(COWSettings.COWCacheLifeTime.TotalMinutes));
					}
					if ((this.originalRetentionHold ^ this.DataObject.RetentionHoldEnabled) || this.DataObject.IsChanged(ADUserSchema.RetentionComment) || this.DataObject.IsChanged(ADUserSchema.RetentionUrl))
					{
						this.UpdateRetentionDataInStoreConfiguration();
					}
					this.AdjustUMEnabledStatus();
					this.StampOrganizationCapabilities();
					if (base.Fields.IsModified("EnableRoomMailboxAccount"))
					{
						this.ProcessEnableRoomMailboxAccountParameter();
					}
					if (base.Fields.IsModified(ADUserSchema.MailboxContainerGuid))
					{
						this.DataObject.MailboxContainerGuid = this.MailboxContainerGuid;
					}
					if (base.Fields.IsModified("AggregatedMailboxGuid"))
					{
						if (this.AddAggregatedAccount.IsPresent)
						{
							if (!this.DataObject.AggregatedMailboxGuids.Contains(this.AggregatedMailboxGuid))
							{
								this.DataObject.AggregatedMailboxGuids.Add(this.AggregatedMailboxGuid);
							}
						}
						else if (this.RemoveAggregatedAccount.IsPresent && this.DataObject.AggregatedMailboxGuids.Contains(this.AggregatedMailboxGuid))
						{
							this.DataObject.AggregatedMailboxGuids.Remove(this.AggregatedMailboxGuid);
						}
					}
					if (base.Fields.IsModified(ADUserSchema.UnifiedMailbox))
					{
						this.DataObject.UnifiedMailbox = this.UnifiedMailbox;
					}
					if (this.NewPassword != null && this.NewPassword.Length > 0 && this.OldPassword != null && this.OldPassword.Length > 0)
					{
						this.ChangePassword(this.OldPassword, this.NewPassword);
					}
					if (this.DataObject.IsSoftDeleted && this.DataObject.IsModified(MailboxSchema.LitigationHoldEnabled))
					{
						if (!this.DataObject.IsInactiveMailbox && this.DataObject.LitigationHoldEnabled)
						{
							this.DataObject.UpdateSoftDeletedStatusForHold(true);
						}
						else if (this.DataObject.IsInactiveMailbox && !this.DataObject.IsInLitigationHoldOrInplaceHold)
						{
							if (!base.Force && !base.ShouldContinue(Strings.ConfirmationTurnOffLitigationHold(this.DataObject.WhenSoftDeleted.ToString())))
							{
								return;
							}
							this.DataObject.UpdateSoftDeletedStatusForHold(false);
						}
					}
					base.InternalProcessRecord();
					if (this.rehomeDatabase != null)
					{
						if (this.originalDatabase != null)
						{
							this.RefreshMailboxInDatabase(this.DataObject.ExchangeGuid, this.originalDatabase.ObjectGuid);
						}
						this.RefreshMailboxInDatabase(this.DataObject.ExchangeGuid, this.rehomeDatabase.Id.ObjectGuid);
					}
					if (this.rehomeArchiveDatabase != null)
					{
						if (this.originalArchiveDatabase != null)
						{
							this.RefreshMailboxInDatabase(this.DataObject.ArchiveGuid, this.originalArchiveDatabase.ObjectGuid);
						}
						this.RefreshMailboxInDatabase(this.DataObject.ArchiveGuid, this.rehomeArchiveDatabase.Id.ObjectGuid);
					}
					List<PropValue> mailboxShapeParametersToSet = new List<PropValue>();
					List<PropTag> mailboxShapeParametersToDelete = new List<PropTag>();
					if (base.Fields.IsModified("MailboxMessagesPerFolderCountWarningQuota"))
					{
						this.ProcessMailboxShapeParameter(this.MailboxMessagesPerFolderCountWarningQuota, PropTag.MailboxMessagesPerFolderCountWarningQuota, mailboxShapeParametersToSet, mailboxShapeParametersToDelete);
					}
					if (base.Fields.IsModified("MailboxMessagesPerFolderCountReceiveQuota"))
					{
						this.ProcessMailboxShapeParameter(this.MailboxMessagesPerFolderCountReceiveQuota, PropTag.MailboxMessagesPerFolderCountReceiveQuota, mailboxShapeParametersToSet, mailboxShapeParametersToDelete);
					}
					if (base.Fields.IsModified("DumpsterMessagesPerFolderCountWarningQuota"))
					{
						this.ProcessMailboxShapeParameter(this.DumpsterMessagesPerFolderCountWarningQuota, PropTag.DumpsterMessagesPerFolderCountWarningQuota, mailboxShapeParametersToSet, mailboxShapeParametersToDelete);
					}
					if (base.Fields.IsModified("DumpsterMessagesPerFolderCountReceiveQuota"))
					{
						this.ProcessMailboxShapeParameter(this.DumpsterMessagesPerFolderCountReceiveQuota, PropTag.DumpsterMessagesPerFolderCountReceiveQuota, mailboxShapeParametersToSet, mailboxShapeParametersToDelete);
					}
					if (base.Fields.IsModified("FolderHierarchyChildrenCountWarningQuota"))
					{
						this.ProcessMailboxShapeParameter(this.FolderHierarchyChildrenCountWarningQuota, PropTag.FolderHierarchyChildrenCountWarningQuota, mailboxShapeParametersToSet, mailboxShapeParametersToDelete);
					}
					if (base.Fields.IsModified("FolderHierarchyChildrenCountReceiveQuota"))
					{
						this.ProcessMailboxShapeParameter(this.FolderHierarchyChildrenCountReceiveQuota, PropTag.FolderHierarchyChildrenCountReceiveQuota, mailboxShapeParametersToSet, mailboxShapeParametersToDelete);
					}
					if (base.Fields.IsModified("FolderHierarchyDepthWarningQuota"))
					{
						this.ProcessMailboxShapeParameter(this.FolderHierarchyDepthWarningQuota, PropTag.FolderHierarchyDepthWarningQuota, mailboxShapeParametersToSet, mailboxShapeParametersToDelete);
					}
					if (base.Fields.IsModified("FolderHierarchyDepthReceiveQuota"))
					{
						this.ProcessMailboxShapeParameter(this.FolderHierarchyDepthReceiveQuota, PropTag.FolderHierarchyDepthReceiveQuota, mailboxShapeParametersToSet, mailboxShapeParametersToDelete);
					}
					if (base.Fields.IsModified("FoldersCountWarningQuota"))
					{
						this.ProcessMailboxShapeParameter(this.FoldersCountWarningQuota, PropTag.FoldersCountWarningQuota, mailboxShapeParametersToSet, mailboxShapeParametersToDelete);
					}
					if (base.Fields.IsModified("FoldersCountReceiveQuota"))
					{
						this.ProcessMailboxShapeParameter(this.FoldersCountReceiveQuota, PropTag.FoldersCountReceiveQuota, mailboxShapeParametersToSet, mailboxShapeParametersToDelete);
					}
					if (base.Fields.IsModified("ExtendedPropertiesCountQuota"))
					{
						this.ProcessMailboxShapeParameter(this.ExtendedPropertiesCountQuota, PropTag.NamedPropertiesCountQuota, mailboxShapeParametersToSet, mailboxShapeParametersToDelete);
					}
					if (base.Fields.IsModified("AggregatedMailboxGuid") && this.AddAggregatedAccount.IsPresent)
					{
						this.PrePopulateStoreCacheForAggregatedMailbox();
					}
					this.UpdateMailboxShapeParameters(mailboxShapeParametersToSet, mailboxShapeParametersToDelete);
				}
			}
			finally
			{
				ProvisioningPerformanceHelper.StopLatencyDetection(this.latencyContext);
			}
		}

		private void UpdateMailboxShapeParameters(List<PropValue> mailboxShapeParametersToSet, List<PropTag> mailboxShapeParametersToDelete)
		{
			if (mailboxShapeParametersToSet.Count > 0 || mailboxShapeParametersToDelete.Count > 0)
			{
				DatabaseLocationInfo databaseLocationInfo = null;
				try
				{
					databaseLocationInfo = ActiveManager.GetActiveManagerInstance().GetServerForDatabase(this.DataObject.Database.ObjectGuid);
				}
				catch (ObjectNotFoundException exception)
				{
					base.WriteError(exception, ExchangeErrorCategory.ServerOperation, null);
				}
				if (base.IsVerboseOn)
				{
					base.WriteVerbose(Strings.VerboseConnectionMapiRpcInterface(databaseLocationInfo.ServerFqdn));
				}
				using (MapiStore mapiStore = MapiStore.OpenMailbox(databaseLocationInfo.ServerFqdn, this.DataObject.LegacyExchangeDN, this.DataObject.ExchangeGuid, this.DataObject.Database.ObjectGuid, this.DataObject.Name, null, null, ConnectFlag.UseAdminPrivilege | ConnectFlag.UseSeparateConnection, OpenStoreFlag.UseAdminPrivilege | OpenStoreFlag.TakeOwnership | OpenStoreFlag.MailboxGuid, CultureInfo.InvariantCulture, null, "Client=Management;Action=SetMailbox", null))
				{
					PropProblem[] array = null;
					List<PropProblem> list = null;
					if (mailboxShapeParametersToSet.Count > 0)
					{
						array = mapiStore.SetProps(mailboxShapeParametersToSet.ToArray());
					}
					if (array != null)
					{
						list = new List<PropProblem>(array);
					}
					if (mailboxShapeParametersToDelete.Count > 0)
					{
						array = mapiStore.DeleteProps(mailboxShapeParametersToDelete);
					}
					if (array != null)
					{
						if (list != null)
						{
							list.AddRange(array);
						}
						else
						{
							list = new List<PropProblem>(array);
						}
					}
					if (list != null)
					{
						if (base.IsVerboseOn)
						{
							foreach (PropProblem propProblem in list)
							{
								base.WriteVerbose(Strings.VerbosePropertyProblem(propProblem.ToString()));
							}
						}
						base.WriteError(new FailedToUpdateStoreMailboxInformationException(this.Identity.ToString()), ExchangeErrorCategory.ServerTransient, this.Identity);
					}
				}
			}
		}

		private void ProcessMailboxShapeParameter(int? paramValue, PropTag propTag, List<PropValue> mailboxShapeParametersToSet, List<PropTag> mailboxShapeParametersToDelete)
		{
			if (paramValue != null)
			{
				PropValue item = new PropValue(propTag, paramValue.Value);
				mailboxShapeParametersToSet.Add(item);
				return;
			}
			mailboxShapeParametersToDelete.Add(propTag);
		}

		protected override IConfigDataProvider CreateSession()
		{
			IRecipientSession recipientSession = (IRecipientSession)base.CreateSession();
			if (Globals.IsMicrosoftHostedOnly && base.CurrentOrganizationId != null && base.CurrentOrganizationId.OrganizationalUnit != null && base.UserSpecifiedParameters.Contains("LitigationHoldEnabled"))
			{
				recipientSession = SoftDeletedTaskHelper.GetSessionForSoftDeletedObjects(recipientSession, recipientSession.SearchRoot);
			}
			return recipientSession;
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			ADUser aduser = (ADUser)dataObject;
			if (aduser.RecipientTypeDetails == RecipientTypeDetails.GroupMailbox)
			{
				base.WriteError(new LocalizedException(Strings.ErrorCmdletNotSupportedForGroupMailbox("Set-Mailbox")), ExchangeErrorCategory.Client, this.Identity);
			}
			if (this.rehomeDatabase != null)
			{
				this.originalDatabase = aduser.Database;
				if (aduser.ArchiveGuid != Guid.Empty && this.rehomeArchiveDatabase == null && aduser.ArchiveDatabase == null)
				{
					aduser.ArchiveDatabase = this.rehomeDatabase.Id;
				}
			}
			if (this.rehomeArchiveDatabase != null)
			{
				this.originalArchiveDatabase = aduser.ArchiveDatabase;
				aduser.ArchiveDatabase = this.rehomeArchiveDatabase.Id;
			}
			this.originalSingleItemRecovery = aduser.SingleItemRecoveryEnabled;
			this.originalLitigationHold = aduser.LitigationHoldEnabled;
			this.originalRetentionHold = aduser.RetentionHoldEnabled;
			if (base.Fields.IsModified(ADUserSchema.QueryBaseDN))
			{
				aduser.QueryBaseDN = this.querybaseDNId;
			}
			base.StampChangesOn(dataObject);
		}

		private void RefreshMailboxInDatabase(Guid mailboxGuid, Guid dbGuid)
		{
			try
			{
				DatabaseLocationInfo serverForDatabase = ActiveManager.GetActiveManagerInstance().GetServerForDatabase(dbGuid, GetServerForDatabaseFlags.ThrowServerForDatabaseNotFoundException);
				using (ExRpcAdmin exRpcAdmin = ExRpcAdmin.Create("Client=Management", serverForDatabase.ServerFqdn, null, null, null))
				{
					exRpcAdmin.SyncMailboxWithDS(dbGuid, mailboxGuid);
				}
			}
			catch (LocalizedException)
			{
			}
		}

		private void ValidateEnableRoomMailboxAccountParameter()
		{
			if (this.DataObject.RecipientTypeDetails == RecipientTypeDetails.LinkedRoomMailbox)
			{
				base.WriteError(new TaskArgumentException(Strings.ErrorEnableRoomMailboxAccountCannotBeUsedWithLinkedRoomMailbox), ExchangeErrorCategory.Client, null);
			}
			if (this.DataObject.ResourceType != ExchangeResourceType.Room)
			{
				base.WriteError(new TaskArgumentException(Strings.ErrorEnableRoomMailboxAccountCanBeUsedWithRoomsOnly), ExchangeErrorCategory.Client, null);
			}
			if (this.EnableRoomMailboxAccount && this.RoomMailboxPassword == null)
			{
				base.WriteError(new TaskArgumentException(Strings.ErrorRoomPasswordMustBeSetWhenEnablingRoomADAccount), ExchangeErrorCategory.Client, null);
			}
			if (!this.EnableRoomMailboxAccount && this.RoomMailboxPassword != null)
			{
				base.WriteError(new TaskArgumentException(Strings.ErrorRoomMailboxPasswordCannotBeSetIfEnableRoomMailboxAccountIsFalse), ExchangeErrorCategory.Client, null);
			}
		}

		private void ProcessEnableRoomMailboxAccountParameter()
		{
			if (this.EnableRoomMailboxAccount)
			{
				this.DataObject.ExchangeUserAccountControl &= ~UserAccountControlFlags.AccountDisabled;
				this.DataObject.ExchangeUserAccountControl |= UserAccountControlFlags.NormalAccount;
				base.Password = this.RoomMailboxPassword;
				return;
			}
			this.DataObject.ExchangeUserAccountControl |= (UserAccountControlFlags.AccountDisabled | UserAccountControlFlags.NormalAccount | UserAccountControlFlags.DoNotExpirePassword);
		}

		private void AdjustUMEnabledStatus()
		{
			if (this.DataObject.UMEnabled && !Utils.UnifiedMessagingAvailable(this.DataObject))
			{
				try
				{
					if (Utils.RunningInTestMode)
					{
						FaultInjectionUtils.FaultInjectionTracer.TraceTest(3341167933U);
					}
					Utils.ResetUMMailbox(this.DataObject, true);
					Utility.ResetUMADProperties(this.DataObject, true);
				}
				catch (LocalizedException ex)
				{
					base.WriteError(new RecipientTaskException(Strings.MailboxCouldNotBeUmDisabled(this.Identity.ToString(), ex.LocalizedString), ex), ExchangeErrorCategory.ServerOperation, this.DataObject);
				}
			}
		}

		private void PrePopulateStoreCacheForAggregatedMailbox()
		{
			DatabaseLocationInfo databaseLocationInfo = null;
			try
			{
				databaseLocationInfo = ActiveManager.GetActiveManagerInstance().GetServerForDatabase(this.DataObject.Database.ObjectGuid);
			}
			catch (ObjectNotFoundException exception)
			{
				base.WriteError(exception, ExchangeErrorCategory.ServerOperation, null);
			}
			if (base.IsVerboseOn)
			{
				base.WriteVerbose(Strings.VerboseConnectionMapiRpcInterface(databaseLocationInfo.ServerFqdn));
			}
			if (databaseLocationInfo != null)
			{
				MailboxTaskHelper.PrepopulateCacheForMailbox(this.DataObject.Database.ObjectGuid, databaseLocationInfo.DatabaseName, databaseLocationInfo.ServerFqdn, this.DataObject.OrganizationId, this.DataObject.LegacyExchangeDN, this.AggregatedMailboxGuid, this.DataObject.OriginatingServer, new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
			}
		}

		private void UpdateRetentionDataInStoreConfiguration()
		{
			try
			{
				using (StoreRetentionPolicyTagHelper storeRetentionPolicyTagHelper = StoreRetentionPolicyTagHelper.FromMailboxId(base.DomainController, this.Identity, this.DataObject.OrganizationId))
				{
					storeRetentionPolicyTagHelper.HoldData = new RetentionHoldData(this.DataObject.RetentionHoldEnabled, this.DataObject.RetentionComment, this.DataObject.RetentionUrl);
					storeRetentionPolicyTagHelper.Save();
				}
			}
			catch (ElcUserConfigurationException ex)
			{
				TaskLogger.LogError(ex);
				this.WriteWarning(Strings.WarningSetMailboxRetentionHoldDataFAI(ex.Message));
			}
		}

		private void ValidateOrganizationCapabilities()
		{
			if ((base.Fields.IsModified("UMDataStorage") || base.Fields.IsModified("OABGen") || base.Fields.IsModified("UMGrammar") || base.Fields.IsModified("ClientExtensions") || base.Fields.IsModified("GMGen") || base.Fields.IsModified("MailRouting") || base.Fields.IsModified("Management") || base.Fields.IsModified("TenantUpgrade") || base.Fields.IsModified("Migration") || base.Fields.IsModified("MessageTracking") || base.Fields.IsModified("OMEncryption") || base.Fields.IsModified("PstProvider") || base.Fields.IsModified("SuiteServiceStorage")) && !base.Arbitration)
			{
				base.WriteError(new InvalidOrgCapabilityParameterException(), ExchangeErrorCategory.Client, null);
			}
			Dictionary<OrganizationCapability, string> dictionary = new Dictionary<OrganizationCapability, string>
			{
				{
					OrganizationCapability.UMDataStorage,
					"UMDataStorage"
				},
				{
					OrganizationCapability.ClientExtensions,
					"ClientExtensions"
				},
				{
					OrganizationCapability.OfficeMessageEncryption,
					"OMEncryption"
				},
				{
					OrganizationCapability.SuiteServiceStorage,
					"SuiteServiceStorage"
				}
			};
			foreach (KeyValuePair<OrganizationCapability, string> keyValuePair in dictionary)
			{
				OrganizationCapability key = keyValuePair.Key;
				string value = keyValuePair.Value;
				if (base.Fields.IsModified(value) && (bool)base.Fields[value] && !this.DataObject.PersistedCapabilities.Contains((Capability)key))
				{
					List<ADUser> organizationMailboxesByCapability = OrganizationMailbox.GetOrganizationMailboxesByCapability((IRecipientSession)base.DataSession, key);
					if (organizationMailboxesByCapability.Count > 0)
					{
						base.WriteError(new MultipleOrgMbxesWithCapabilityException(key.ToString()), ExchangeErrorCategory.Client, null);
					}
				}
			}
		}

		private void ValidateMailboxShapeFeatureVersion()
		{
			long num = 0L;
			if (base.Fields.IsModified("MailboxMessagesPerFolderCountWarningQuota") || base.Fields.IsModified("MailboxMessagesPerFolderCountReceiveQuota") || base.Fields.IsModified("DumpsterMessagesPerFolderCountWarningQuota") || base.Fields.IsModified("DumpsterMessagesPerFolderCountReceiveQuota") || base.Fields.IsModified("FolderHierarchyChildrenCountWarningQuota") || base.Fields.IsModified("FolderHierarchyChildrenCountReceiveQuota"))
			{
				num = 1L;
			}
			if (base.Fields.IsModified("FolderHierarchyDepthWarningQuota") || base.Fields.IsModified("FolderHierarchyDepthReceiveQuota"))
			{
				num = 2L;
			}
			if (base.Fields.IsModified("FoldersCountWarningQuota") || base.Fields.IsModified("FoldersCountReceiveQuota") || base.Fields.IsModified("ExtendedPropertiesCountQuota"))
			{
				num = 4L;
			}
			if (num > 0L)
			{
				DatabaseLocationInfo databaseLocationInfo = null;
				try
				{
					databaseLocationInfo = ActiveManager.GetActiveManagerInstance().GetServerForDatabase(this.DataObject.Database.ObjectGuid);
				}
				catch (ObjectNotFoundException exception)
				{
					base.WriteError(exception, ExchangeErrorCategory.ServerOperation, null);
				}
				using (ExRpcAdmin exRpcAdmin = ExRpcAdmin.Create("Client=Management", databaseLocationInfo.ServerFqdn, null, null, null))
				{
					if ((ulong)exRpcAdmin.GetMailboxShapeServerVersion() < (ulong)num)
					{
						base.WriteError(new UnsupportedMailboxShapeFeatureVersionException(this.Identity.ToString()), ExchangeErrorCategory.Client, this.Identity);
					}
				}
			}
		}

		private void StampOrganizationCapabilities()
		{
			if (base.Fields.IsModified("UMDataStorage"))
			{
				this.AddRemoveOrganizationCapability(OrganizationCapability.UMDataStorage, (bool)base.Fields["UMDataStorage"]);
			}
			if (base.Fields.IsModified("UMGrammar"))
			{
				this.AddRemoveOrganizationCapability(OrganizationCapability.UMGrammar, (bool)base.Fields["UMGrammar"]);
			}
			if (base.Fields.IsModified("OABGen"))
			{
				if ((bool)base.Fields["OABGen"])
				{
					this.WriteWarning(Strings.WarningMustInvokeUpdateOABToStartScheduledGeneration);
				}
				this.AddRemoveOrganizationCapability(OrganizationCapability.OABGen, (bool)base.Fields["OABGen"]);
			}
			if (base.Fields.IsModified("GMGen"))
			{
				this.AddRemoveOrganizationCapability(OrganizationCapability.GMGen, (bool)base.Fields["GMGen"]);
			}
			if (base.Fields.IsModified("ClientExtensions"))
			{
				this.AddRemoveOrganizationCapability(OrganizationCapability.ClientExtensions, (bool)base.Fields["ClientExtensions"]);
			}
			if (base.Fields.IsModified("MailRouting"))
			{
				this.AddRemoveOrganizationCapability(OrganizationCapability.MailRouting, (bool)base.Fields["MailRouting"]);
			}
			if (base.Fields.IsModified("Management"))
			{
				this.AddRemoveOrganizationCapability(OrganizationCapability.Management, (bool)base.Fields["Management"]);
			}
			if (base.Fields.IsModified("TenantUpgrade"))
			{
				this.AddRemoveOrganizationCapability(OrganizationCapability.TenantUpgrade, (bool)base.Fields["TenantUpgrade"]);
			}
			if (base.Fields.IsModified("Migration"))
			{
				this.AddRemoveOrganizationCapability(OrganizationCapability.Migration, (bool)base.Fields["Migration"]);
			}
			if (base.Fields.IsModified("MessageTracking"))
			{
				this.AddRemoveOrganizationCapability(OrganizationCapability.MessageTracking, (bool)base.Fields["MessageTracking"]);
			}
			if (base.Fields.IsModified("OMEncryption"))
			{
				this.AddRemoveOrganizationCapability(OrganizationCapability.OfficeMessageEncryption, (bool)base.Fields["OMEncryption"]);
			}
			if (base.Fields.IsModified("PstProvider"))
			{
				this.AddRemoveOrganizationCapability(OrganizationCapability.PstProvider, (bool)base.Fields["PstProvider"]);
			}
			if (base.Fields.IsModified("SuiteServiceStorage"))
			{
				this.AddRemoveOrganizationCapability(OrganizationCapability.SuiteServiceStorage, (bool)base.Fields["SuiteServiceStorage"]);
			}
		}

		private void AddRemoveOrganizationCapability(OrganizationCapability orgCapability, bool add)
		{
			if (add)
			{
				if (!this.DataObject.PersistedCapabilities.Contains((Capability)orgCapability))
				{
					this.DataObject.PersistedCapabilities.Add((Capability)orgCapability);
					return;
				}
			}
			else if (this.DataObject.PersistedCapabilities.Contains((Capability)orgCapability))
			{
				this.DataObject.PersistedCapabilities.Remove((Capability)orgCapability);
			}
		}

		private void ChangePassword(SecureString oldPassword, SecureString newPassword)
		{
			string domainname = ((ADObjectId)this.DataObject.Identity).DomainId.ToString();
			string username = this.DataObject.SamAccountName;
			SecurityIdentifier masterAccountSid = this.DataObject.MasterAccountSid;
			if (masterAccountSid != null && masterAccountSid.IsAccountSid())
			{
				string friendlyUserName = SecurityPrincipalIdParameter.GetFriendlyUserName(masterAccountSid, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
				if (string.IsNullOrEmpty(friendlyUserName))
				{
					base.WriteError(new ManagementObjectNotFoundException(base.GetErrorMessageObjectNotFound(masterAccountSid.ToString(), null, null)), ExchangeErrorCategory.Client, this.Identity);
				}
				string[] array = friendlyUserName.Split(new char[]
				{
					'\\'
				});
				if (array.Length == 2)
				{
					domainname = array[0];
					username = array[1];
				}
				else
				{
					username = array[0];
				}
			}
			IntPtr intPtr = IntPtr.Zero;
			IntPtr intPtr2 = IntPtr.Zero;
			try
			{
				intPtr = Marshal.SecureStringToGlobalAllocUnicode(oldPassword);
				intPtr2 = Marshal.SecureStringToGlobalAllocUnicode(newPassword);
				uint num = NativeMethods.NetUserChangePassword(domainname, username, intPtr, intPtr2);
				uint num2 = num;
				if (num2 <= 5U)
				{
					if (num2 == 0U)
					{
						goto IL_18D;
					}
					if (num2 == 5U)
					{
						base.WriteError(new RecipientTaskException(Strings.ChangePasswordLockedOut), ExchangeErrorCategory.Client, this.DataObject.Identity);
						goto IL_18D;
					}
				}
				else
				{
					if (num2 == 86U)
					{
						base.WriteError(new RecipientTaskException(Strings.ChangePasswordInvalidCredentials), ExchangeErrorCategory.Client, this.DataObject.Identity);
						goto IL_18D;
					}
					if (num2 == 2245U)
					{
						base.WriteError(new RecipientTaskException(Strings.ChangePasswordInvalidNewPassword), ExchangeErrorCategory.Client, this.DataObject.Identity);
						goto IL_18D;
					}
				}
				base.WriteError(new RecipientTaskException(Strings.ChangePasswordFailed), ExchangeErrorCategory.Client, this.DataObject.Identity);
				IL_18D:;
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.ZeroFreeGlobalAllocUnicode(intPtr);
				}
				if (intPtr2 != IntPtr.Zero)
				{
					Marshal.ZeroFreeGlobalAllocUnicode(intPtr2);
				}
			}
		}

		private const string ClientExtensionsParameterName = "ClientExtensions";

		private const string GMGenParameterName = "GMGen";

		private const string MailRoutingParameterName = "MailRouting";

		private const string OABGenParameterName = "OABGen";

		private const string UMGrammarParameterName = "UMGrammar";

		private const string UMDataStorageParameterName = "UMDataStorage";

		private const string ManagementParameterName = "Management";

		private const string TenantUpgradeParameterName = "TenantUpgrade";

		private const string MessageTrackingParameterName = "MessageTracking";

		private const string PstProviderParameterName = "PstProvider";

		private const string SuiteServiceStorageParameterName = "SuiteServiceStorage";

		private const string ForestWideDomainControllerAffinityName = "ForestWideDomainControllerAffinityByExecutingUser";

		private const string OMEncryptionParameterName = "OMEncryption";

		private const string MigrationParameterName = "Migration";

		private const string MailboxMessagesPerFolderCountWarningQuotaName = "MailboxMessagesPerFolderCountWarningQuota";

		private const string MailboxMessagesPerFolderCountReceiveQuotaName = "MailboxMessagesPerFolderCountReceiveQuota";

		private const string DumpsterMessagesPerFolderCountWarningQuotaName = "DumpsterMessagesPerFolderCountWarningQuota";

		private const string DumpsterMessagesPerFolderCountReceiveQuotaName = "DumpsterMessagesPerFolderCountReceiveQuota";

		private const string FolderHierarchyChildrenCountWarningQuotaName = "FolderHierarchyChildrenCountWarningQuota";

		private const string FolderHierarchyChildrenCountReceiveQuotaName = "FolderHierarchyChildrenCountReceiveQuota";

		private const string FolderHierarchyDepthWarningQuotaName = "FolderHierarchyDepthWarningQuota";

		private const string FolderHierarchyDepthReceiveQuotaName = "FolderHierarchyDepthReceiveQuota";

		private const string FoldersCountWarningQuotaName = "FoldersCountWarningQuota";

		private const string FoldersCountReceiveQuotaName = "FoldersCountReceiveQuota";

		private const string ExtendedPropertiesCountQuotaName = "ExtendedPropertiesCountQuota";

		private const string AggregatedMailboxGuidName = "AggregatedMailboxGuid";

		private const string AddAggregatedAccountName = "AddAggregatedAccount";

		private const string RemoveAggregatedAccountName = "RemoveAggregatedAccount";

		private const string MessageCopyForSendOnBehalfEnabledParameterName = "MessageCopyForSendOnBehalfEnabled";

		private const string MessageCopyForSentAsEnabledParameterName = "MessageCopyForSentAsEnabled";

		private const string LitigationHoldEnabledName = "LitigationHoldEnabled";

		private static readonly object[] InvalidPublicFolderParameters = new object[]
		{
			"Arbitration",
			ADRecipientSchema.ArbitrationMailbox,
			ADUserSchema.ArchiveDatabase,
			ADRecipientSchema.AuditEnabled,
			MailboxSchema.ArchiveDomain,
			MailboxSchema.ArchiveName,
			MailboxSchema.ArchiveQuota,
			MailboxSchema.ArchiveStatus,
			MailboxSchema.ArchiveWarningQuota,
			MailboxSchema.RetentionPolicy
		};

		private static readonly HashSet<string> LitigationHoldEnabledParameters = new HashSet<string>(new string[]
		{
			"Identity",
			"LitigationHoldEnabled",
			"Confirm",
			"Debug",
			"ErrorAction",
			"ErrorVariable",
			"Force",
			"OutBuffer",
			"OutVariable",
			"Verbose",
			"WarningAction",
			"WarningVariable",
			"WhatIf"
		}, StringComparer.OrdinalIgnoreCase);

		private static readonly object[] PublicFolderOnlyParameters = new object[]
		{
			MailboxSchema.IsExcludedFromServingHierarchy,
			MailboxSchema.IsHierarchyReady
		};

		private LatencyDetectionContext latencyContext;

		private MailboxDatabase rehomeDatabase;

		private ADObjectId originalDatabase;

		private MailboxDatabase rehomeArchiveDatabase;

		private ADObjectId originalArchiveDatabase;

		private ADObjectId querybaseDNId;

		private bool originalSingleItemRecovery;

		private bool originalLitigationHold;

		private bool originalRetentionHold;
	}
}
