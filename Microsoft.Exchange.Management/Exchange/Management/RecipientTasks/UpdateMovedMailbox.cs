using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Management.Tasks.UM;
using Microsoft.Exchange.Provisioning;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Update", "MovedMailbox", DefaultParameterSetName = "UpdateMailbox", ConfirmImpact = ConfirmImpact.None, SupportsShouldProcess = false)]
	public sealed class UpdateMovedMailbox : RecipientObjectActionTask<MailboxOrMailUserIdParameter, ADUser>
	{
		public UpdateMovedMailbox()
		{
			TestIntegration.Instance.ForceRefresh();
		}

		[Parameter(Mandatory = true)]
		public override MailboxOrMailUserIdParameter Identity
		{
			get
			{
				return (MailboxOrMailUserIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte[] PartitionHint
		{
			get
			{
				if (this.partitionHint != null)
				{
					return this.partitionHint.GetPersistablePartitionHint();
				}
				return null;
			}
			set
			{
				if (value != null)
				{
					this.partitionHint = TenantPartitionHint.FromPersistablePartitionHint(value);
					return;
				}
				this.partitionHint = null;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "UpdateMailbox")]
		[Parameter(Mandatory = true, ParameterSetName = "MorphToMailbox")]
		public Guid NewHomeMDB
		{
			get
			{
				return (Guid)(base.Fields["NewHomeMDB"] ?? Guid.Empty);
			}
			set
			{
				base.Fields["NewHomeMDB"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "UpdateMailbox")]
		public Guid? NewContainerGuid
		{
			get
			{
				return (Guid?)base.Fields["NewContainerGuid"];
			}
			set
			{
				base.Fields["NewContainerGuid"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "UpdateMailbox")]
		public CrossTenantObjectId NewUnifiedMailboxId
		{
			get
			{
				return (CrossTenantObjectId)base.Fields["NewUnifiedMailboxId"];
			}
			set
			{
				base.Fields["NewUnifiedMailboxId"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Guid NewArchiveMDB
		{
			get
			{
				return (Guid)(base.Fields["NewArchiveMDB"] ?? Guid.Empty);
			}
			set
			{
				base.Fields["NewArchiveMDB"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string ArchiveDomain
		{
			get
			{
				return (string)base.Fields["ArchiveDomain"];
			}
			set
			{
				base.Fields["ArchiveDomain"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ArchiveStatusFlags ArchiveStatus
		{
			get
			{
				return (ArchiveStatusFlags)(base.Fields["ArchiveStatus"] ?? ArchiveStatusFlags.None);
			}
			set
			{
				base.Fields["ArchiveStatus"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "MorphToMailUser")]
		public SwitchParameter MorphToMailUser
		{
			get
			{
				return (SwitchParameter)(base.Fields["MorphToMailUser"] ?? false);
			}
			set
			{
				base.Fields["MorphToMailUser"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "MorphToMailbox")]
		public SwitchParameter MorphToMailbox
		{
			get
			{
				return (SwitchParameter)(base.Fields["MorphToMailbox"] ?? false);
			}
			set
			{
				base.Fields["MorphToMailbox"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "UpdateArchive")]
		public SwitchParameter UpdateArchiveOnly
		{
			get
			{
				return (SwitchParameter)(base.Fields["UpdateArchiveOnly"] ?? false);
			}
			set
			{
				base.Fields["UpdateArchiveOnly"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "UpdateMailbox")]
		public SwitchParameter UpdateMailbox
		{
			get
			{
				return (SwitchParameter)(base.Fields["UpdateMailbox"] ?? false);
			}
			set
			{
				base.Fields["UpdateMailbox"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "UpdateArchive")]
		[Parameter(Mandatory = true, ParameterSetName = "MorphToMailUser")]
		[Parameter(Mandatory = true, ParameterSetName = "MorphToMailbox")]
		public ADUser RemoteRecipientData
		{
			get
			{
				return (ADUser)base.Fields["RemoteRecipientData"];
			}
			set
			{
				base.Fields["RemoteRecipientData"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MorphToMailbox")]
		[Parameter(Mandatory = false, ParameterSetName = "MorphToMailUser")]
		public PSCredential Credential
		{
			get
			{
				return (PSCredential)base.Fields["Credential"];
			}
			set
			{
				base.Fields["Credential"] = value;
				base.NetCredential = ((value != null) ? value.GetNetworkCredential() : null);
			}
		}

		[Parameter(Mandatory = false)]
		public Fqdn ConfigDomainController
		{
			get
			{
				return (Fqdn)base.Fields["ConfigDomainController"];
			}
			set
			{
				base.Fields["ConfigDomainController"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "UpdateArchive")]
		[Parameter(Mandatory = false, ParameterSetName = "UpdateMailbox")]
		public SwitchParameter SkipMailboxReleaseCheck
		{
			get
			{
				return (SwitchParameter)(base.Fields["SkipMailboxReleaseCheck"] ?? false);
			}
			set
			{
				base.Fields["SkipMailboxReleaseCheck"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "UpdateMailbox")]
		[Parameter(Mandatory = false, ParameterSetName = "MorphToMailbox")]
		public SwitchParameter SkipProvisioningCheck
		{
			get
			{
				return (SwitchParameter)(base.Fields["SkipProvisioningCheck"] ?? false);
			}
			set
			{
				base.Fields["SkipProvisioningCheck"] = value;
			}
		}

		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter();
			this.databaseIds = new List<ADObjectId>();
			this.originalMailboxDatabaseId = null;
			this.originalArchiveDatabaseId = null;
			this.newMailboxDatabase = null;
			this.newArchiveDatabase = null;
			base.InternalStateReset();
			this.reportEntries = (base.SessionState.Variables["UMM_ReportEntries"] as List<ReportEntry>);
			if (this.reportEntries == null)
			{
				this.reportEntries = new List<ReportEntry>();
			}
			TaskLogger.LogExit();
		}

		protected override IConfigDataProvider CreateSession()
		{
			TaskLogger.LogEnter();
			IConfigDataProvider result;
			try
			{
				this.globalConfigSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(this.ConfigDomainController, true, ConsistencyMode.PartiallyConsistent, base.NetCredential, ADSessionSettings.FromRootOrgScopeSet(), 555, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxReplication\\UpdateMovedMailbox.cs");
				this.ResolveCurrentOrganization();
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(base.CurrentOrganizationId);
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, base.NetCredential, sessionSettings, 565, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxReplication\\UpdateMovedMailbox.cs");
				tenantOrRootOrgRecipientSession.EnforceDefaultScope = false;
				if (MapiTaskHelper.IsDatacenter || MapiTaskHelper.IsDatacenterDedicated)
				{
					tenantOrRootOrgRecipientSession.SessionSettings.IncludeSoftDeletedObjects = true;
				}
				result = tenantOrRootOrgRecipientSession;
			}
			finally
			{
				TaskLogger.LogExit();
			}
			return result;
		}

		protected override void ProvisioningUpdateConfigurationObject()
		{
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			IConfigurable result;
			try
			{
				ADUser aduser = (ADUser)base.PrepareDataObject();
				MrsTracer.ActivityID = aduser.ExchangeGuid.GetHashCode();
				this.originalMailboxDatabaseId = aduser.Database;
				this.originalArchiveDatabaseId = aduser.ArchiveDatabase;
				DatabaseInformation? databaseInformation = null;
				if (this.IsFieldSet("NewHomeMDB"))
				{
					DatabaseIdParameter databaseIdParameter = new DatabaseIdParameter(new ADObjectId(this.NewHomeMDB));
					databaseIdParameter.AllowLegacy = true;
					MailboxDatabase mailboxDatabase = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(databaseIdParameter, this.globalConfigSession, null, new LocalizedString?(Microsoft.Exchange.Management.Tasks.Strings.ErrorDatabaseNotFound(databaseIdParameter.ToString())), new LocalizedString?(Microsoft.Exchange.Management.Tasks.Strings.ErrorDatabaseNotUnique(databaseIdParameter.ToString())));
					this.newMailboxDatabase = mailboxDatabase;
					databaseInformation = new DatabaseInformation?(this.FindMDBInfo(this.newMailboxDatabase.Id));
					if (CommonUtils.ShouldHonorProvisioningSettings() && !this.newMailboxDatabase.Id.Equals(this.originalMailboxDatabaseId) && !this.SkipProvisioningCheck && this.newMailboxDatabase.IsExcludedFromProvisioning)
					{
						base.WriteError(new DatabaseExcludedFromProvisioningException(this.newMailboxDatabase.Name), ErrorCategory.InvalidArgument, this.Identity);
					}
					if (this.originalMailboxDatabaseId != null && !this.newMailboxDatabase.Id.Equals(this.originalMailboxDatabaseId))
					{
						this.databaseIds.Add(this.originalMailboxDatabaseId);
					}
					this.databaseIds.Add(this.newMailboxDatabase.Id);
				}
				DatabaseInformation? databaseInformation2 = null;
				if (this.IsFieldSet("NewArchiveMDB"))
				{
					if (this.NewArchiveMDB == Guid.Empty)
					{
						this.newArchiveDatabase = null;
					}
					else
					{
						DatabaseIdParameter databaseIdParameter2 = new DatabaseIdParameter(new ADObjectId(this.NewArchiveMDB));
						databaseIdParameter2.AllowLegacy = true;
						MailboxDatabase mailboxDatabase2 = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(databaseIdParameter2, this.globalConfigSession, null, new LocalizedString?(Microsoft.Exchange.Management.Tasks.Strings.ErrorDatabaseNotFound(databaseIdParameter2.ToString())), new LocalizedString?(Microsoft.Exchange.Management.Tasks.Strings.ErrorDatabaseNotUnique(databaseIdParameter2.ToString())));
						this.newArchiveDatabase = mailboxDatabase2;
						databaseInformation2 = new DatabaseInformation?(this.FindMDBInfo(this.newArchiveDatabase.Id));
						if (CommonUtils.ShouldHonorProvisioningSettings() && !ADObjectId.Equals(this.originalArchiveDatabaseId, this.newArchiveDatabase.Id) && !this.SkipProvisioningCheck && this.newArchiveDatabase.IsExcludedFromProvisioning)
						{
							base.WriteError(new DatabaseExcludedFromProvisioningException(this.newArchiveDatabase.Name), ErrorCategory.InvalidArgument, this.Identity);
						}
					}
				}
				if (!this.SkipMailboxReleaseCheck && MailboxTaskHelper.SupportsMailboxReleaseVersioning(aduser) && (databaseInformation != null || databaseInformation2 != null))
				{
					MailboxRelease requiredMailboxRelease = MailboxTaskHelper.ComputeRequiredMailboxRelease(this.ConfigurationSession, aduser, null, new Task.ErrorLoggerDelegate(base.WriteError));
					if (databaseInformation != null)
					{
						MailboxTaskHelper.ValidateMailboxRelease(databaseInformation.Value.MailboxRelease, requiredMailboxRelease, aduser.Id.ToString(), databaseInformation.Value.DatabaseName, new Task.ErrorLoggerDelegate(base.WriteError));
						aduser.MailboxRelease = databaseInformation.Value.MailboxRelease;
					}
					if (databaseInformation2 != null)
					{
						MailboxTaskHelper.ValidateMailboxRelease(databaseInformation2.Value.MailboxRelease, requiredMailboxRelease, aduser.Id.ToString(), databaseInformation2.Value.DatabaseName, new Task.ErrorLoggerDelegate(base.WriteError));
						aduser.ArchiveRelease = databaseInformation2.Value.MailboxRelease;
					}
				}
				bool isArchiveMoving = (this.newArchiveDatabase == null) ? (this.originalArchiveDatabaseId != null) : (!ADObjectId.Equals(this.originalArchiveDatabaseId, this.newArchiveDatabase.Id));
				if (this.MorphToMailUser)
				{
					if (this.originalMailboxDatabaseId != null && !this.databaseIds.Contains(this.originalMailboxDatabaseId))
					{
						this.databaseIds.Add(this.originalMailboxDatabaseId);
					}
					this.ConvertToMailUser(aduser, isArchiveMoving);
				}
				else if (this.MorphToMailbox || this.UpdateMailbox)
				{
					if (this.MorphToMailbox && VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
					{
						IConfigurationSession tenantLocalConfigSession = RecipientTaskHelper.GetTenantLocalConfigSession(aduser.OrganizationId, base.ExecutingUserOrganizationId, base.RootOrgContainerId, true, base.DomainController, base.NetCredential);
						RecipientTaskHelper.ValidateSmtpAddress(tenantLocalConfigSession, aduser.EmailAddresses, aduser, new Task.ErrorLoggerDelegate(base.WriteError), base.ProvisioningCache, true);
					}
					this.UpdatePrimaryDatabase(aduser);
					this.ConvertToOrUpdateMailbox(aduser, this.MorphToMailbox, isArchiveMoving);
					this.UpdateContainerProperties(aduser);
				}
				else
				{
					MailboxMoveTransition transition;
					ADUser aduser2;
					ADUser aduser3;
					if (this.originalArchiveDatabaseId != null && this.newArchiveDatabase == null)
					{
						if (aduser.IsFromDatacenter)
						{
							aduser.ArchiveRelease = MailboxRelease.None;
						}
						transition = MailboxMoveTransition.UpdateSourceUser;
						aduser2 = aduser;
						aduser3 = this.RemoteRecipientData;
					}
					else if (this.originalArchiveDatabaseId == null && this.newArchiveDatabase != null)
					{
						transition = MailboxMoveTransition.UpdateTargetUser;
						aduser2 = this.RemoteRecipientData;
						aduser3 = aduser;
					}
					else
					{
						if (this.originalArchiveDatabaseId == null || this.newArchiveDatabase == null)
						{
							this.reportEntries.Add(new ReportEntry(MrsStrings.ReportArchiveAlreadyUpdated, ReportEntryType.Warning));
							return aduser;
						}
						transition = MailboxMoveTransition.IntraOrg;
						aduser2 = null;
						aduser3 = aduser;
					}
					MailboxMoveHelper.UpdateRecipientTypeProperties(aduser2, (aduser2 != null) ? (aduser2.IsFromDatacenter ? UserHoster.Datacenter : UserHoster.OnPremise) : UserHoster.None, Server.E14SP1MinVersion, aduser3, aduser3.IsFromDatacenter ? UserHoster.Datacenter : UserHoster.OnPremise, Server.E14SP1MinVersion, MailboxMoveType.IsArchiveMoving, transition);
				}
				this.UpdateArchiveAttributes(aduser);
				result = aduser;
			}
			finally
			{
				TaskLogger.LogExit();
			}
			return result;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				if (this.MorphToMailbox || this.UpdateMailbox)
				{
					this.ApplyMailboxPlan(this.DataObject);
				}
				if (this.MorphToMailbox)
				{
					this.SetUMPropertiesWhenConvertingToMailbox(this.DataObject);
				}
				bool writeShadowProperties = ((IDirectorySession)base.DataSession).ServerSettings.WriteShadowProperties;
				try
				{
					if (base.NetCredential != null)
					{
						((IDirectorySession)base.DataSession).ServerSettings.WriteShadowProperties = false;
					}
					MrsTracer.UpdateMovedMailbox.Debug(string.Format("Saving the ADUser to Domain Controller {0}", base.DataSession.Source), new object[0]);
					base.InternalProcessRecord();
					base.SessionState.Variables["UMM_UpdateSucceeded"] = true;
					base.SessionState.Variables["UMM_DCName"] = base.DataSession.Source;
				}
				finally
				{
					((IDirectorySession)base.DataSession).ServerSettings.WriteShadowProperties = writeShadowProperties;
				}
				if (this.UpdateMailbox)
				{
					this.reportEntries.Add(new ReportEntry(MrsStrings.ReportMovedMailboxUpdated(base.DataSession.Source)));
				}
				else if (this.MorphToMailbox)
				{
					this.reportEntries.Add(new ReportEntry(MrsStrings.ReportMovedMailUserMorphedToMailbox(base.DataSession.Source)));
				}
				else if (this.MorphToMailUser)
				{
					this.reportEntries.Add(new ReportEntry(MrsStrings.ReportMovedMailboxMorphedToMailUser(base.DataSession.Source)));
				}
				else
				{
					this.reportEntries.Add(new ReportEntry(MrsStrings.ReportArchiveUpdated(base.DataSession.Source)));
				}
				try
				{
					CommonUtils.CatchKnownExceptions(delegate
					{
						this.UpdateAllADSites();
					}, delegate(Exception ex)
					{
						LocalizedString error2 = CommonUtils.FullExceptionMessage(ex);
						this.reportEntries.Add(new ReportEntry(MrsStrings.ReportUpdateMovedMailboxFailureAfterADSwitchover(error2), ReportEntryType.WarningCondition, ex, ReportEntryFlags.Cleanup));
					});
				}
				catch (Exception ex)
				{
					Exception ex2;
					LocalizedString error = CommonUtils.FullExceptionMessage(ex2);
					this.reportEntries.Add(new ReportEntry(MrsStrings.ReportUpdateMovedMailboxFailureAfterADSwitchover(error), ReportEntryType.WarningCondition, ex2, ReportEntryFlags.Cleanup));
					ExWatson.SendReport(ex2);
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override void InternalEndProcessing()
		{
			TaskLogger.LogEnter();
			try
			{
				base.InternalEndProcessing();
				if (TestIntegration.Instance.InjectUmmEndProcessingFailure)
				{
					throw new MailboxReplicationPermanentException(new LocalizedString("Injected UMM EndProcessing failure"));
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return RequestTaskHelper.IsKnownExceptionHandler(exception, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose)) || base.IsKnownException(exception);
		}

		private bool IsFieldSet(string fieldName)
		{
			return base.Fields.IsChanged(fieldName) || base.Fields.IsModified(fieldName);
		}

		private void ClearAttributes(ADUser mailbox, ICollection<PropertyDefinition> properties)
		{
			TaskLogger.LogEnter();
			foreach (PropertyDefinition propertyDefinition in properties)
			{
				ADPropertyDefinition adpropertyDefinition = (ADPropertyDefinition)propertyDefinition;
				if (!mailbox.ExchangeVersion.IsOlderThan(adpropertyDefinition.VersionAdded))
				{
					if (adpropertyDefinition.IsMultivalued)
					{
						((MultiValuedPropertyBase)mailbox[adpropertyDefinition]).Clear();
					}
					else
					{
						mailbox[adpropertyDefinition] = null;
					}
				}
			}
			TaskLogger.LogExit();
		}

		private void CopyAttributes(ADUser sourceMailbox, ADUser targetMailbox, PropertyDefinition[] propertiesToCopy)
		{
			TaskLogger.LogEnter();
			foreach (ADPropertyDefinition propertyToCopy in propertiesToCopy)
			{
				this.CopyAttribute(sourceMailbox, targetMailbox, propertyToCopy);
			}
			TaskLogger.LogExit();
		}

		private void CopyAttribute(ADUser sourceMailbox, ADUser targetMailbox, ADPropertyDefinition propertyToCopy)
		{
			TaskLogger.LogEnter();
			if (propertyToCopy.Type != typeof(ADObjectId))
			{
				this.SetADProperty(targetMailbox, propertyToCopy, sourceMailbox[propertyToCopy]);
			}
			TaskLogger.LogExit();
		}

		private void SetADProperty(ADUser user, ADPropertyDefinition property, object value)
		{
			if (!user.ExchangeVersion.IsOlderThan(property.VersionAdded))
			{
				user[property] = value;
			}
		}

		private void ConvertToOrUpdateMailbox(ADUser user, bool isCrossOrg, bool isArchiveMoving)
		{
			TaskLogger.LogEnter();
			MrsTracer.UpdateMovedMailbox.Function("UpdateMovedMailbox.ConvertToOrUpdateMailbox", new object[0]);
			try
			{
				IRecipientSession recipientSession = (IRecipientSession)base.DataSession;
				int num;
				string text;
				this.FindMDBServerInfo(this.newMailboxDatabase.Id, out num, out text);
				if (this.newMailboxDatabase.Id.Equals(this.originalMailboxDatabaseId) && num >= Server.E2007MinVersion)
				{
					this.reportEntries.Add(new ReportEntry(Microsoft.Exchange.Management.Tasks.Strings.ErrorUserAlreadyInTargetDatabase(this.Identity.ToString(), this.newMailboxDatabase.Id.ToString()), ReportEntryType.WarningCondition));
				}
				if (!isCrossOrg && this.originalMailboxDatabaseId != null)
				{
					int num2;
					this.FindMDBServerInfo(this.originalMailboxDatabaseId, out num2, out text);
					ExchangeObjectVersion exchangeObjectVersion;
					if (num >= Server.E15MinVersion)
					{
						exchangeObjectVersion = ExchangeObjectVersion.Exchange2012;
					}
					else if (num >= Server.E14MinVersion)
					{
						exchangeObjectVersion = ExchangeObjectVersion.Exchange2010;
					}
					else if (num >= Server.E2007MinVersion)
					{
						exchangeObjectVersion = ExchangeObjectVersion.Exchange2007;
					}
					else
					{
						exchangeObjectVersion = ExchangeObjectVersion.Exchange2003;
					}
					ExchangeObjectVersion other;
					if (num2 >= Server.E15MinVersion)
					{
						other = ExchangeObjectVersion.Exchange2012;
					}
					else if (num2 >= Server.E14MinVersion)
					{
						other = ExchangeObjectVersion.Exchange2010;
					}
					else if (num2 >= Server.E2007MinVersion)
					{
						other = ExchangeObjectVersion.Exchange2007;
					}
					else
					{
						other = ExchangeObjectVersion.Exchange2003;
					}
					if (exchangeObjectVersion.IsOlderThan(other))
					{
						List<PropertyDefinition> list = new List<PropertyDefinition>();
						ADUserSchema instance = ObjectSchema.GetInstance<ADUserSchema>();
						foreach (PropertyDefinition propertyDefinition in instance.AllProperties)
						{
							ADPropertyDefinition adpropertyDefinition = (ADPropertyDefinition)propertyDefinition;
							if (exchangeObjectVersion.IsOlderThan(adpropertyDefinition.VersionAdded) && !adpropertyDefinition.IsCalculated && !adpropertyDefinition.IsReadOnly && !adpropertyDefinition.IsBackLink)
							{
								list.Add(adpropertyDefinition);
							}
						}
						this.ClearAttributes(user, list);
					}
					if (num >= Server.E15MinVersion)
					{
						user.SetExchangeVersion(ExchangeObjectVersion.Exchange2012);
						if (user.LitigationHoldEnabled)
						{
							user.SetLitigationHoldEnabledWellKnownInPlaceHoldGuid(true);
						}
					}
					else if (num >= Server.E14MinVersion)
					{
						user.SetExchangeVersion(ExchangeObjectVersion.Exchange2010);
					}
					else if (num >= Server.E2007MinVersion)
					{
						user.SetExchangeVersion(ExchangeObjectVersion.Exchange2007);
					}
					else
					{
						user.SetExchangeVersion(null);
					}
				}
				else
				{
					if (num >= Server.E15MinVersion)
					{
						user.SetExchangeVersion(ExchangeObjectVersion.Exchange2012);
					}
					else if (num >= Server.E14MinVersion)
					{
						user.SetExchangeVersion(ExchangeObjectVersion.Exchange2010);
					}
					else if (num >= Server.E2007MinVersion)
					{
						user.SetExchangeVersion(ExchangeObjectVersion.Exchange2007);
					}
					else
					{
						user.SetExchangeVersion(null);
					}
					if (user.UseDatabaseQuotaDefaults == null)
					{
						user.UseDatabaseQuotaDefaults = new bool?(VariantConfiguration.InvariantNoFlightingSnapshot.CmdletInfra.UseDatabaseQuotaDefaults.Enabled);
					}
					if (num >= Server.E2007MinVersion)
					{
						this.CopyAttributes(this.RemoteRecipientData, user, UpdateMovedMailbox.ElcProperties);
						this.SetElcFlagsForCrossForestMove(user, num);
						this.CopyAttributes(this.RemoteRecipientData, user, UpdateMovedMailbox.ResourceProperties);
						this.CopyAttributes(this.RemoteRecipientData, user, UpdateMovedMailbox.UMProperties);
						this.CopyAttributes(this.RemoteRecipientData, user, UpdateMovedMailbox.OtherProperties);
						if (this.RemoteRecipientData.MailboxPlan == null && !Datacenter.IsMicrosoftHostedOnly(true))
						{
							this.CopyAttributes(this.RemoteRecipientData, user, UpdateMovedMailbox.TransportProperties);
							if (this.RemoteRecipientData.IsOWAEnabledStatusConsistent())
							{
								user.OWAEnabled = this.RemoteRecipientData.OWAEnabled;
							}
						}
					}
				}
				if (isCrossOrg && user.RawExternalEmailAddress != null)
				{
					user.RawExternalEmailAddress = null;
				}
				this.ApplyCustomUpdates(user);
				MailboxMoveHelper.UpdateRecipientTypeProperties(isCrossOrg ? this.RemoteRecipientData : null, isCrossOrg ? (this.RemoteRecipientData.IsFromDatacenter ? UserHoster.Datacenter : UserHoster.OnPremise) : UserHoster.None, 0, user, user.IsFromDatacenter ? UserHoster.Datacenter : UserHoster.OnPremise, num, (!isArchiveMoving) ? MailboxMoveType.IsPrimaryMoving : (MailboxMoveType.IsPrimaryMoving | MailboxMoveType.IsArchiveMoving), isCrossOrg ? MailboxMoveTransition.UpdateTargetUser : MailboxMoveTransition.IntraOrg);
				this.ClearAttributes(user, new PropertyDefinition[]
				{
					ADRecipientSchema.HomeMTA
				});
				this.CheckAndClearLegalHoldPropertiesForDownlevelMove(num, user);
				if (num >= Server.E2007MinVersion)
				{
					this.UpdateRecipient(user);
				}
				if (user.WindowsEmailAddress != user.PrimarySmtpAddress && ADRecipientSchema.WindowsEmailAddress.ValidateValue(user.PrimarySmtpAddress, false) == null)
				{
					user.WindowsEmailAddress = user.PrimarySmtpAddress;
				}
				user.OriginalPrimarySmtpAddress = user.PrimarySmtpAddress;
				user.OriginalWindowsEmailAddress = user.WindowsEmailAddress;
				if (!isCrossOrg && num >= Server.E14MinVersion)
				{
					NetID netID = user.NetID;
					if (netID != null)
					{
						user.NetID = netID;
					}
				}
				if (num >= Server.E14MinVersion)
				{
					this.ApplyRoleAssignmentPolicy(user);
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		private void CheckAndClearLegalHoldPropertiesForDownlevelMove(int targetServerVersion, ADUser targetUser)
		{
			if (targetServerVersion < Server.E15MinVersion)
			{
				this.ClearAttributes(targetUser, new PropertyDefinition[]
				{
					ADRecipientSchema.InPlaceHoldsRaw
				});
			}
		}

		private void ConvertToMailUser(ADUser user, bool isArchiveMoving)
		{
			TaskLogger.LogEnter();
			MrsTracer.UpdateMovedMailbox.Function("UpdateMovedMailbox.ConvertToMailUser", new object[0]);
			try
			{
				if (user.Database == null)
				{
					this.reportEntries.Add(new ReportEntry(MrsStrings.ReportMovedMailboxAlreadyMorphedToMailUser, ReportEntryType.Warning));
				}
				else
				{
					int num;
					string text;
					this.FindMDBServerInfo(user.Database, out num, out text);
					this.SetUMPropertiesWhenConvertingToMailUser(user);
					SmtpAddress primarySmtpAddress = user.PrimarySmtpAddress;
					List<PropertyDefinition> list = new List<PropertyDefinition>();
					foreach (PropertyDefinition propertyDefinition in RecipientConstants.DisableMailbox_PropertiesToReset)
					{
						if (propertyDefinition != ADMailboxRecipientSchema.ExchangeGuid && propertyDefinition != ADRecipientSchema.Alias && propertyDefinition != ADRecipientSchema.EmailAddresses && propertyDefinition != ADRecipientSchema.PoliciesIncluded && propertyDefinition != ADRecipientSchema.PoliciesExcluded && propertyDefinition != ADRecipientSchema.LegacyExchangeDN && propertyDefinition != ADRecipientSchema.MasterAccountSid && propertyDefinition != ADUserSchema.ArchiveGuid && propertyDefinition != ADUserSchema.ArchiveName && propertyDefinition != ADUserSchema.MailboxMoveFlags && propertyDefinition != ADUserSchema.MailboxMoveRemoteHostName && propertyDefinition != ADUserSchema.MailboxMoveStatus && propertyDefinition != ADUserSchema.MailboxMoveSourceMDB && propertyDefinition != ADUserSchema.MailboxMoveTargetMDB && propertyDefinition != ADUserSchema.MailboxMoveBatchName && propertyDefinition != ADUserSchema.UMEnabledFlags && !RecipientConstants.DisableMailUserBase_PropertiesToReset.Contains(propertyDefinition))
						{
							list.Add(propertyDefinition);
						}
					}
					this.ClearAttributes(user, list);
					user.RawExternalEmailAddress = null;
					if (this.RemoteRecipientData.ExternalEmailAddress != null)
					{
						user.ExternalEmailAddress = this.RemoteRecipientData.ExternalEmailAddress;
					}
					this.ApplyCustomUpdates(user);
					if (TestIntegration.Instance.RemoteExchangeGuidOverride == Guid.Empty)
					{
						CustomProxyAddress item = new CustomProxyAddress((CustomProxyAddressPrefix)ProxyAddressPrefix.X500, this.RemoteRecipientData.LegacyExchangeDN, false);
						if (!user.EmailAddresses.Contains(item))
						{
							user.EmailAddresses.Add(item);
						}
					}
					int upgradeSourceUserWhileOnboarding = TestIntegration.Instance.UpgradeSourceUserWhileOnboarding;
					int num2 = (upgradeSourceUserWhileOnboarding == 1) ? Server.E15MinVersion : Server.E14SP1MinVersion;
					bool flag = !user.IsFromDatacenter && this.RemoteRecipientData.IsFromDatacenter;
					if (upgradeSourceUserWhileOnboarding != -1 && num < num2 && flag)
					{
						ExchangeObjectVersion exchangeObjectVersion = (upgradeSourceUserWhileOnboarding == 1) ? ExchangeObjectVersion.Exchange2012 : ExchangeObjectVersion.Exchange2010;
						if (exchangeObjectVersion.IsNewerThan(user.ExchangeVersion ?? ExchangeObjectVersion.Exchange2003))
						{
							user.SetExchangeVersion(exchangeObjectVersion);
						}
					}
					else if (num < Server.E2007MinVersion)
					{
						user.SetExchangeVersion(null);
					}
					MailboxMoveHelper.UpdateRecipientTypeProperties(user, user.IsFromDatacenter ? UserHoster.Datacenter : UserHoster.OnPremise, num, this.RemoteRecipientData, this.RemoteRecipientData.IsFromDatacenter ? UserHoster.Datacenter : UserHoster.OnPremise, 0, (!isArchiveMoving) ? MailboxMoveType.IsPrimaryMoving : (MailboxMoveType.IsPrimaryMoving | MailboxMoveType.IsArchiveMoving), MailboxMoveTransition.UpdateSourceUser);
					this.UpdateRecipient(user);
					if (user.PrimarySmtpAddress == SmtpAddress.Empty && primarySmtpAddress != SmtpAddress.Empty)
					{
						user.PrimarySmtpAddress = primarySmtpAddress;
					}
					user.OriginalPrimarySmtpAddress = user.PrimarySmtpAddress;
					user.OriginalWindowsEmailAddress = user.WindowsEmailAddress;
					user.MailboxRelease = MailboxRelease.None;
					if (isArchiveMoving)
					{
						user.ArchiveRelease = MailboxRelease.None;
					}
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		private void UpdatePrimaryDatabase(ADUser user)
		{
			TaskLogger.LogEnter();
			MrsTracer.UpdateMovedMailbox.Function("UpdateMovedMailbox.UpdatePrimaryDatabase", new object[0]);
			try
			{
				MrsTracer.UpdateMovedMailbox.Debug("Setting Database to '{0}'", new object[]
				{
					this.newMailboxDatabase.Id
				});
				user.Database = this.newMailboxDatabase.Id;
				int num;
				string text;
				this.FindMDBServerInfo(this.newMailboxDatabase.Id, out num, out text);
				MrsTracer.UpdateMovedMailbox.Debug("Setting ServerLegacyDN to '{0}'", new object[]
				{
					text
				});
				user.ServerLegacyDN = text;
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		private void UpdateContainerProperties(ADUser user)
		{
			TaskLogger.LogEnter();
			MrsTracer.UpdateMovedMailbox.Function("UpdateMovedMailbox.UpdateContainerProperties", new object[0]);
			try
			{
				if (this.IsFieldSet("NewContainerGuid") && user.MailboxContainerGuid != this.NewContainerGuid)
				{
					MrsTracer.UpdateMovedMailbox.Debug("Setting MailboxContainerGuid to '{0}'", new object[]
					{
						this.NewContainerGuid
					});
					user.MailboxContainerGuid = this.NewContainerGuid;
				}
				if (this.IsFieldSet("NewUnifiedMailboxId") && user.UnifiedMailbox != this.NewUnifiedMailboxId)
				{
					MrsTracer.UpdateMovedMailbox.Debug("Setting UnifiedMailbox to '{0}'", new object[]
					{
						this.NewUnifiedMailboxId
					});
					user.UnifiedMailbox = this.NewUnifiedMailboxId;
				}
				if ((this.IsFieldSet("NewContainerGuid") || this.IsFieldSet("NewUnifiedMailboxId")) && user.UnifiedMailbox != null)
				{
					ADRecipient adrecipient;
					ADRecipient.TryGetFromCrossTenantObjectId(user.UnifiedMailbox, out adrecipient);
					ADUser aduser = (ADUser)adrecipient;
					if (!aduser.MailboxContainerGuid.Equals(user.MailboxContainerGuid))
					{
						base.WriteError(new InvalidMailboxContainerGuidException((user.MailboxContainerGuid != null) ? user.MailboxContainerGuid.Value.ToString() : "null", (aduser.MailboxContainerGuid != null) ? aduser.MailboxContainerGuid.Value.ToString() : "null"), ErrorCategory.InvalidArgument, this.Identity);
					}
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		private void UpdateArchiveAttributes(ADUser user)
		{
			TaskLogger.LogEnter();
			MrsTracer.UpdateMovedMailbox.Function("UpdateMovedMailbox.UpdateArchiveAttributes", new object[0]);
			try
			{
				if ((this.newArchiveDatabase == null) ? (this.originalArchiveDatabaseId != null) : (!ADObjectId.Equals(this.originalArchiveDatabaseId, this.newArchiveDatabase.Id)))
				{
					MrsTracer.UpdateMovedMailbox.Debug("Setting ArchiveDatabase to '{0}'", new object[]
					{
						(this.newArchiveDatabase == null) ? "<null>" : this.newArchiveDatabase.Id.ToString()
					});
					user.ArchiveDatabase = ((this.newArchiveDatabase != null) ? this.newArchiveDatabase.Id : null);
					if (this.originalArchiveDatabaseId != null && !this.databaseIds.Contains(this.originalArchiveDatabaseId))
					{
						this.databaseIds.Add(this.originalArchiveDatabaseId);
					}
					if (this.newArchiveDatabase != null && this.newArchiveDatabase.Id != null)
					{
						if (!this.databaseIds.Contains(this.newArchiveDatabase.Id))
						{
							this.databaseIds.Add(this.newArchiveDatabase.Id);
						}
						if (Datacenter.IsMicrosoftHostedOnly(true) && this.UpdateArchiveOnly && this.originalArchiveDatabaseId == null)
						{
							user.ArchiveQuota = RecipientConstants.ArchiveAddOnQuota;
							user.ArchiveWarningQuota = RecipientConstants.ArchiveAddOnWarningQuota;
						}
					}
				}
				if (this.IsFieldSet("ArchiveDomain"))
				{
					MrsTracer.UpdateMovedMailbox.Debug("Setting ArchiveDomain to '{0}'", new object[]
					{
						this.ArchiveDomain
					});
					if (this.ArchiveDomain == null)
					{
						if (user.ArchiveDomain != null)
						{
							user.ArchiveDomain = null;
						}
					}
					else
					{
						user.ArchiveDomain = new SmtpDomain(this.ArchiveDomain);
					}
				}
				if (this.IsFieldSet("ArchiveStatus"))
				{
					MrsTracer.UpdateMovedMailbox.Debug("Setting ArchiveStatus to '{0}'", new object[]
					{
						this.ArchiveStatus
					});
					if (user.ArchiveStatus != this.ArchiveStatus)
					{
						user.ArchiveStatus = this.ArchiveStatus;
					}
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		private void ApplyMailboxPlan(ADUser user)
		{
			TaskLogger.LogEnter();
			MrsTracer.UpdateMovedMailbox.Function("UpdateMovedMailbox.ApplyMailboxPlan", new object[0]);
			if (!Datacenter.IsMicrosoftHostedOnly(true))
			{
				return;
			}
			try
			{
				if (this.MorphToMailbox)
				{
					ApplyMailboxPlanFlags flags = ConfigBase<MRSConfigSchema>.GetConfig<bool>("CheckMailUserPlanQuotasForOnboarding") ? ApplyMailboxPlanFlags.PreservePreviousExplicitlySetValues : ApplyMailboxPlanFlags.None;
					MrsTracer.UpdateMovedMailbox.Function("ApplyMailboxPlan(): MorphToMailbox is set to true, applying mbxplan", new object[0]);
					MailboxTaskHelper.ApplyMbxPlanSettingsInTargetForest(user, (ADObjectId mbxPlanId) => (ADUser)base.GetDataObject<ADUser>(new MailboxPlanIdParameter(mbxPlanId), base.TenantGlobalCatalogSession, null, new LocalizedString?(Microsoft.Exchange.Management.Tasks.Strings.ErrorMailboxPlanNotFound(mbxPlanId.ToString())), new LocalizedString?(Microsoft.Exchange.Management.Tasks.Strings.ErrorMailboxPlanNotUnique(mbxPlanId.ToString()))), flags);
				}
				else
				{
					MrsTracer.UpdateMovedMailbox.Function("ApplyMailboxPlan(): MorphToMailbox is set to false, no need to apply MailboxPlan, leaving", new object[0]);
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		private void ApplyRoleAssignmentPolicy(ADUser user)
		{
			TaskLogger.LogEnter();
			MrsTracer.UpdateMovedMailbox.Function("UpdateMovedMailbox.ApplyRoleAssignmentPolicy", new object[0]);
			try
			{
				if (user.RoleAssignmentPolicy == null && user.RecipientTypeDetails != RecipientTypeDetails.ArbitrationMailbox)
				{
					IConfigurationSession tenantLocalConfigSession = RecipientTaskHelper.GetTenantLocalConfigSession(user.OrganizationId, base.ExecutingUserOrganizationId, base.RootOrgContainerId, true, base.DomainController, base.NetCredential);
					RoleAssignmentPolicy roleAssignmentPolicy = RecipientTaskHelper.FindDefaultRoleAssignmentPolicy(tenantLocalConfigSession, new Task.ErrorLoggerDelegate(base.WriteError), Microsoft.Exchange.Management.Tasks.Strings.ErrorDefaultRoleAssignmentPolicyNotUnique, Microsoft.Exchange.Management.Tasks.Strings.ErrorDefaultRoleAssignmentPolicyNotFound);
					if (roleAssignmentPolicy != null)
					{
						user.RoleAssignmentPolicy = (ADObjectId)roleAssignmentPolicy.Identity;
					}
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		private void UpdateRecipient(ADUser user)
		{
			if (base.IsProvisioningLayerAvailable)
			{
				MrsTracer.UpdateMovedMailbox.Debug("Provisioning Layer is available, calling UpdateRecipient", new object[0]);
				ProvisioningLayer.UpdateAffectedIConfigurable(this, RecipientTaskHelper.ConvertRecipientToPresentationObject(user), false);
				return;
			}
			MrsTracer.UpdateMovedMailbox.Error("Provisioning Layer is not available!", new object[0]);
			Exception ex = new InvalidOperationException(Microsoft.Exchange.Configuration.Common.LocStrings.Strings.ErrorNoProvisioningHandlerAvailable);
			this.reportEntries.Add(new ReportEntry(MrsStrings.ReportUpdateMovedMailboxError(new LocalizedString("ErrorNoProvisioningHandlerAvailable")), ReportEntryType.Error, ex, ReportEntryFlags.None));
			base.WriteError(ex, ErrorCategory.InvalidOperation, null);
		}

		private void UpdateAllADSites()
		{
			TaskLogger.LogEnter();
			MrsTracer.UpdateMovedMailbox.Function("UpdateMovedMailbox.UpdateAllADSites", new object[0]);
			if (TestIntegration.Instance.GetIntValue("SquishyLobster-UpdateAllADSitesOnMoveCompletion", 1, 0, 1) == 0 || this.databaseIds == null)
			{
				return;
			}
			MrsTracer.UpdateMovedMailbox.Debug("Preparing to replicate object to all possible AD Sites for Database-Copy locations", new object[0]);
			try
			{
				List<ADObjectId> list = new List<ADObjectId>();
				foreach (ADObjectId adobjectId in this.databaseIds)
				{
					DatabaseIdParameter databaseIdParameter = new DatabaseIdParameter(adobjectId);
					ITopologyConfigurationSession configSessionForDatabase = RequestTaskHelper.GetConfigSessionForDatabase(this.globalConfigSession, adobjectId);
					MailboxDatabase mailboxDatabase = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(databaseIdParameter, configSessionForDatabase, null, new LocalizedString?(Microsoft.Exchange.Management.Tasks.Strings.ErrorDatabaseNotFound(databaseIdParameter.ToString())), new LocalizedString?(Microsoft.Exchange.Management.Tasks.Strings.ErrorDatabaseNotUnique(databaseIdParameter.ToString())));
					if (mailboxDatabase == null || mailboxDatabase.Servers == null)
					{
						MrsTracer.UpdateMovedMailbox.Debug("A database could not be found or had no copies.", new object[0]);
					}
					else
					{
						foreach (ADObjectId adObjectId in mailboxDatabase.Servers)
						{
							ServerIdParameter serverIdParameter = new ServerIdParameter(adObjectId);
							Server server = (Server)base.GetDataObject<Server>(serverIdParameter, this.globalConfigSession, null, new LocalizedString?(Microsoft.Exchange.Management.Tasks.Strings.ErrorServerNotFound(serverIdParameter.ToString())), new LocalizedString?(Microsoft.Exchange.Management.Tasks.Strings.ErrorServerNotUnique(serverIdParameter.ToString())));
							ADObjectId serverSite = server.ServerSite;
							if (!list.Contains(serverSite))
							{
								list.Add(serverSite);
								MrsTracer.UpdateMovedMailbox.Debug("Adding site '{0}' for replication.", new object[]
								{
									serverSite.ToString()
								});
							}
						}
					}
				}
				this.globalConfigSession.DomainController = this.globalConfigSession.Source;
				RootDse rootDse = this.globalConfigSession.GetRootDse();
				ADObjectId site = rootDse.Site;
				if (list.Contains(site))
				{
					MrsTracer.UpdateMovedMailbox.Debug("Removing local site '{0}' from list for replication.", new object[]
					{
						site.ToString()
					});
					list.Remove(site);
				}
				if (list.Count > 0)
				{
					MrsTracer.UpdateMovedMailbox.Debug("Replicating object to all possible AD Sites for Database-Copy locations", new object[0]);
					((IRecipientSession)base.DataSession).ReplicateSingleObject(this.DataObject, list.ToArray());
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		private void FindMDBServerInfo(ADObjectId mdbId, out int serverVersion, out string serverDN)
		{
			DatabaseInformation databaseInformation = this.FindMDBInfo(mdbId);
			serverVersion = databaseInformation.ServerVersion;
			serverDN = databaseInformation.ServerDN;
		}

		private DatabaseInformation FindMDBInfo(ADObjectId mdbId)
		{
			return MapiUtils.FindServerForMdb(mdbId, this.ConfigDomainController, base.NetCredential, FindServerFlags.None);
		}

		private void SetElcFlagsForCrossForestMove(ADUser user, int targetServerVersion)
		{
			TaskLogger.LogEnter();
			MrsTracer.UpdateMovedMailbox.Function("UpdateMovedMailbox.SetElcFlagsForCrossForestMove", new object[0]);
			try
			{
				CommonUtils.CatchKnownExceptions(delegate
				{
					ElcMailboxFlags elcMailboxFlags = (ElcMailboxFlags)Enum.Parse(typeof(ElcMailboxFlags), this.RemoteRecipientData[ADUserSchema.ElcMailboxFlags].ToString());
					ElcMailboxFlags elcMailboxFlags2 = ElcMailboxFlags.ExpirationSuspended | ElcMailboxFlags.ElcV2 | ElcMailboxFlags.DisableCalendarLogging | ElcMailboxFlags.LitigationHold | ElcMailboxFlags.SingleItemRecovery | ElcMailboxFlags.ShouldUseDefaultRetentionPolicy;
					user[ADUserSchema.ElcMailboxFlags] = (elcMailboxFlags & elcMailboxFlags2);
					if (user.LitigationHoldEnabled && targetServerVersion >= Server.E15MinVersion)
					{
						user.SetLitigationHoldEnabledWellKnownInPlaceHoldGuid(true);
					}
				}, delegate(Exception ex)
				{
					this.reportEntries.Add(new ReportEntry(MrsStrings.ReportUpdateMovedMailboxError(new LocalizedString(CommonUtils.GetFailureType(ex))), ReportEntryType.WarningCondition, ex, ReportEntryFlags.Cleanup));
				});
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		private void SetUMPropertiesWhenConvertingToMailUser(ADUser user)
		{
			TaskLogger.LogEnter();
			MrsTracer.UpdateMovedMailbox.Function("UpdateMovedMailbox.SetUMPropertiesWhenConvertingToMailUser", new object[0]);
			try
			{
				CommonUtils.CatchKnownExceptions(delegate
				{
					this.SetUMPropertiesWhenConvertingToMailUserWorker(user);
				}, delegate(Exception ex)
				{
					this.reportEntries.Add(new ReportEntry(MrsStrings.ReportUpdateMovedMailboxError(new LocalizedString(CommonUtils.GetFailureType(ex))), ReportEntryType.WarningCondition, ex, ReportEntryFlags.Cleanup));
				});
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		private void SetUMPropertiesWhenConvertingToMailUserWorker(ADUser user)
		{
			TaskLogger.LogEnter();
			MrsTracer.UpdateMovedMailbox.Function("UpdateMovedMailbox.SetUMPropertiesWhenConvertingToMailUserWorker", new object[0]);
			try
			{
				if (!user.UMEnabled)
				{
					MrsTracer.UpdateMovedMailbox.Debug("User {0} is not enabled for UM, so no changes required", new object[]
					{
						user
					});
				}
				else
				{
					Utility.ResetUMADProperties(user, false);
					this.CopyAttribute(this.RemoteRecipientData, user, ADOrgPersonSchema.VoiceMailSettings);
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		private void ApplyCustomUpdates(ADUser user)
		{
			if (this.RemoteRecipientData == null)
			{
				return;
			}
			PropertyUpdateXML[] updates = XMLSerializableBase.Deserialize<PropertyUpdateXML[]>(this.RemoteRecipientData.LinkedMasterAccount, false);
			PropertyUpdateXML.Apply(updates, user);
		}

		private void SetUMPropertiesWhenConvertingToMailbox(ADUser user)
		{
			TaskLogger.LogEnter();
			MrsTracer.UpdateMovedMailbox.Function("UpdateMovedMailbox.SetUMPropertiesWhenConvertingToMailbox", new object[0]);
			try
			{
				CommonUtils.CatchKnownExceptions(delegate
				{
					if (!this.RemoteRecipientData.UMEnabled)
					{
						MrsTracer.UpdateMovedMailbox.Debug("User {0} is not enabled for UM, so no changes required", new object[]
						{
							user
						});
						return;
					}
					this.EnableUserForUM(user);
					this.SetADProperty(user, ADRecipientSchema.UMProvisioningRequested, true);
				}, delegate(Exception ex1)
				{
					this.reportEntries.Add(new ReportEntry(Microsoft.Exchange.Management.Tasks.Strings.UserCanNotBeEnabledForUM(user.ExchangeGuid.ToString(), new LocalizedString(CommonUtils.GetFailureType(ex1))), ReportEntryType.WarningCondition, ex1, ReportEntryFlags.Cleanup));
					CommonUtils.CatchKnownExceptions(delegate
					{
						Utility.ResetUMADProperties(user, true);
						this.SetADProperty(user, ADRecipientSchema.UMProvisioningRequested, true);
					}, delegate(Exception ex2)
					{
						this.reportEntries.Add(new ReportEntry(Microsoft.Exchange.Management.Tasks.Strings.ReportUpdateMovedMailboxError(new LocalizedString(CommonUtils.GetFailureType(ex1))), ReportEntryType.WarningCondition, ex2, ReportEntryFlags.Cleanup));
					});
				});
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		private void EnableUserForUM(ADUser user)
		{
			TaskLogger.LogEnter();
			MrsTracer.UpdateMovedMailbox.Function("UpdateMovedMailbox.TryEnableUserForUM", new object[0]);
			try
			{
				IRecipientSession tenantLocalRecipientSession = RecipientTaskHelper.GetTenantLocalRecipientSession(user.OrganizationId, base.ExecutingUserOrganizationId, base.RootOrgContainerId, base.DomainController, base.NetCredential);
				IConfigurationSession tenantLocalConfigSession = RecipientTaskHelper.GetTenantLocalConfigSession(user.OrganizationId, base.ExecutingUserOrganizationId, base.RootOrgContainerId, true, base.DomainController, base.NetCredential);
				MigrationHelper.EnableTargetUserForUM(tenantLocalRecipientSession, tenantLocalConfigSession, Datacenter.IsMicrosoftHostedOnly(true), this.RemoteRecipientData, user);
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		private void ResolveCurrentOrganization()
		{
			if (this.partitionHint == null)
			{
				base.CurrentOrganizationId = OrganizationId.ForestWideOrgId;
				return;
			}
			ADSessionSettings adsessionSettings = ADSessionSettings.FromTenantPartitionHint(this.partitionHint);
			base.CurrentOrganizationId = adsessionSettings.CurrentOrganizationId;
		}

		public const string ParameterIdentity = "Identity";

		public const string ParameterPartitionHint = "PartitionHint";

		public const string ParameterNewHomeMDB = "NewHomeMDB";

		public const string ParameterNewContainerGuid = "NewContainerGuid";

		public const string ParameterNewUnifiedMailboxId = "NewUnifiedMailboxId";

		public const string ParameterNewArchiveMDB = "NewArchiveMDB";

		public const string ParameterArchiveDomain = "ArchiveDomain";

		public const string ParameterArchiveStatus = "ArchiveStatus";

		public const string ParameterMorphToMailUser = "MorphToMailUser";

		public const string ParameterMorphToMailbox = "MorphToMailbox";

		public const string ParameterUpdateArchiveOnly = "UpdateArchiveOnly";

		public const string ParameterUpdateMailbox = "UpdateMailbox";

		public const string ParameterRemoteRecipientData = "RemoteRecipientData";

		public const string ParameterCredential = "Credential";

		public const string ParameterConfigDomainController = "ConfigDomainController";

		public const string ParameterSkipMailboxReleaseCheck = "SkipMailboxReleaseCheck";

		public const string ParameterSkipProvisioningCheck = "SkipProvisioningCheck";

		private const string ParameterSetIntraOrg = "UpdateMailbox";

		private const string ParameterSetMorphToMailbox = "MorphToMailbox";

		private const string ParameterSetMorphToMailUser = "MorphToMailUser";

		private const string ParameterSetUpdateArchive = "UpdateArchive";

		internal static readonly PropertyDefinition[] UMProperties = new PropertyDefinition[]
		{
			ADRecipientSchema.UMDtmfMap,
			ADRecipientSchema.AllowUMCallsFromNonUsers,
			ADRecipientSchema.UMRecipientDialPlanId,
			ADRecipientSchema.UMSpokenName,
			ADUserSchema.UMCallingLineIds,
			ADUserSchema.UMEnabledFlags,
			ADUserSchema.UMEnabledFlags2,
			ADUserSchema.OperatorNumber,
			ADUserSchema.PhoneProviderId,
			ADUserSchema.UMPinChecksum,
			ADUserSchema.UMServerWritableFlags,
			ADUserSchema.CallAnsweringAudioCodecLegacy,
			ADUserSchema.CallAnsweringAudioCodec2
		};

		private static readonly PropertyDefinition[] ElcProperties = new PropertyDefinition[]
		{
			ADUserSchema.ElcExpirationSuspensionEndDate,
			ADUserSchema.ElcExpirationSuspensionStartDate,
			ADUserSchema.LitigationHoldDate,
			ADUserSchema.LitigationHoldOwner,
			ADUserSchema.RetentionComment,
			ADUserSchema.RetentionUrl
		};

		private static readonly PropertyDefinition[] TransportProperties = new PropertyDefinition[]
		{
			ADRecipientSchema.MessageHygieneFlags,
			ADRecipientSchema.SCLDeleteThresholdInt,
			ADRecipientSchema.SCLRejectThresholdInt,
			ADRecipientSchema.SCLQuarantineThresholdInt,
			ADRecipientSchema.SCLJunkThresholdInt
		};

		private static readonly PropertyDefinition[] ResourceProperties = new PropertyDefinition[]
		{
			ADRecipientSchema.ResourceCapacity,
			ADRecipientSchema.ResourceMetaData,
			ADRecipientSchema.ResourcePropertiesDisplay,
			ADRecipientSchema.ResourceSearchProperties
		};

		private static readonly PropertyDefinition[] OtherProperties = new PropertyDefinition[]
		{
			ADMailboxRecipientSchema.ExternalOofOptions,
			ADMailboxRecipientSchema.RulesQuota,
			IADMailStorageSchema.SharingPartnerIdentitiesRaw
		};

		private MailboxDatabase newMailboxDatabase;

		private MailboxDatabase newArchiveDatabase;

		private ADObjectId originalMailboxDatabaseId;

		private ADObjectId originalArchiveDatabaseId;

		private List<ADObjectId> databaseIds;

		private ITopologyConfigurationSession globalConfigSession;

		private TenantPartitionHint partitionHint;

		private List<ReportEntry> reportEntries;
	}
}
