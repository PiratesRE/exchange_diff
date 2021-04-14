using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Net;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Provisioning;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Exchange.Data.Mapi.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Provisioning;

namespace Microsoft.Exchange.Management.MapiTasks
{
	[Cmdlet("connect", "Mailbox", DefaultParameterSetName = "User", SupportsShouldProcess = true)]
	public sealed class ConnectMailbox : MapiObjectActionTask<StoreMailboxIdParameter, MailboxStatistics>
	{
		private ITopologyConfigurationSession ResourceForestSession
		{
			get
			{
				if (this.resourceForestSession == null)
				{
					this.resourceForestSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 90, "ResourceForestSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MapiTasks\\Mailbox\\ConnectMailbox.cs");
				}
				return this.resourceForestSession;
			}
		}

		private ActiveManager ActiveManager
		{
			get
			{
				return RecipientTaskHelper.GetActiveManagerInstance();
			}
		}

		private new ServerIdParameter Server
		{
			get
			{
				return base.Server;
			}
			set
			{
				base.Server = value;
			}
		}

		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public new StoreMailboxIdParameter Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 1)]
		public DatabaseIdParameter Database
		{
			get
			{
				return (DatabaseIdParameter)base.Fields["MailboxDatabase"];
			}
			set
			{
				base.Fields["MailboxDatabase"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ValidateOnly")]
		public SwitchParameter ValidateOnly
		{
			get
			{
				return (SwitchParameter)base.Fields["ValidateOnly"];
			}
			set
			{
				base.Fields["ValidateOnly"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Shared")]
		[Parameter(Mandatory = false, ParameterSetName = "Equipment")]
		[Parameter(Mandatory = false, ParameterSetName = "Linked")]
		[Parameter(Mandatory = false, ParameterSetName = "Room")]
		[Parameter(Mandatory = false, ParameterSetName = "User")]
		public UserIdParameter User
		{
			get
			{
				return (UserIdParameter)base.Fields["User"];
			}
			set
			{
				base.Fields["User"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "User")]
		public SwitchParameter AllowLegacyDNMismatch
		{
			get
			{
				return (SwitchParameter)base.Fields["AllowLegacyDNMismatch"];
			}
			set
			{
				base.Fields["AllowLegacyDNMismatch"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "User")]
		[Parameter(Mandatory = false, ParameterSetName = "Shared")]
		[Parameter(Mandatory = false, ParameterSetName = "Equipment")]
		[Parameter(Mandatory = false, ParameterSetName = "Linked")]
		[Parameter(Mandatory = false, ParameterSetName = "Room")]
		public string Alias
		{
			get
			{
				return (string)base.Fields["Alias"];
			}
			set
			{
				base.Fields["Alias"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Room")]
		public SwitchParameter Room
		{
			get
			{
				return (SwitchParameter)base.Fields["Room"];
			}
			set
			{
				base.Fields["Room"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Equipment")]
		public SwitchParameter Equipment
		{
			get
			{
				return (SwitchParameter)base.Fields["Equipment"];
			}
			set
			{
				base.Fields["Equipment"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Shared")]
		public SwitchParameter Shared
		{
			get
			{
				return (SwitchParameter)base.Fields["Shared"];
			}
			set
			{
				base.Fields["Shared"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Linked")]
		public UserIdParameter LinkedMasterAccount
		{
			get
			{
				return (UserIdParameter)base.Fields["LinkedMasterAccount"];
			}
			set
			{
				base.Fields["LinkedMasterAccount"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? false);
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Linked")]
		public Fqdn LinkedDomainController
		{
			get
			{
				return (Fqdn)base.Fields["LinkedDomainController"];
			}
			set
			{
				base.Fields["LinkedDomainController"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Linked")]
		public PSCredential LinkedCredential
		{
			get
			{
				return (PSCredential)base.Fields["LinkedCredential"];
			}
			set
			{
				base.Fields["LinkedCredential"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "User")]
		[Parameter(Mandatory = false, ParameterSetName = "Shared")]
		[Parameter(Mandatory = false, ParameterSetName = "Linked")]
		[Parameter(Mandatory = false, ParameterSetName = "Room")]
		[Parameter(Mandatory = false, ParameterSetName = "Equipment")]
		public MailboxPolicyIdParameter ManagedFolderMailboxPolicy
		{
			get
			{
				return (MailboxPolicyIdParameter)base.Fields["ManagedFolderMailboxPolicy"];
			}
			set
			{
				base.Fields["ManagedFolderMailboxPolicy"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "User")]
		[Parameter(Mandatory = false, ParameterSetName = "Room")]
		[Parameter(Mandatory = false, ParameterSetName = "Linked")]
		[Parameter(Mandatory = false, ParameterSetName = "Equipment")]
		[Parameter(Mandatory = false, ParameterSetName = "Shared")]
		public MailboxPolicyIdParameter RetentionPolicy
		{
			get
			{
				return (MailboxPolicyIdParameter)base.Fields["RetentionPolicy"];
			}
			set
			{
				base.Fields["RetentionPolicy"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Linked")]
		[Parameter(Mandatory = false, ParameterSetName = "Room")]
		[Parameter(Mandatory = false, ParameterSetName = "User")]
		[Parameter(Mandatory = false, ParameterSetName = "Shared")]
		[Parameter(Mandatory = false, ParameterSetName = "Equipment")]
		public SwitchParameter ManagedFolderMailboxPolicyAllowed
		{
			get
			{
				return (SwitchParameter)(base.Fields["ManagedFolderMailboxPolicyAllowed"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ManagedFolderMailboxPolicyAllowed"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Linked")]
		[Parameter(Mandatory = false, ParameterSetName = "Equipment")]
		[Parameter(Mandatory = false, ParameterSetName = "Room")]
		[Parameter(Mandatory = false, ParameterSetName = "User")]
		[Parameter(Mandatory = false, ParameterSetName = "Shared")]
		public MailboxPolicyIdParameter ActiveSyncMailboxPolicy
		{
			get
			{
				return (MailboxPolicyIdParameter)base.Fields["ActiveSyncMailboxPolicy"];
			}
			set
			{
				base.Fields["ActiveSyncMailboxPolicy"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "User")]
		public AddressBookMailboxPolicyIdParameter AddressBookPolicy
		{
			get
			{
				return (AddressBookMailboxPolicyIdParameter)base.Fields[ADRecipientSchema.AddressBookPolicy];
			}
			set
			{
				base.Fields[ADRecipientSchema.AddressBookPolicy] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "User")]
		public SwitchParameter Archive
		{
			get
			{
				return (SwitchParameter)(base.Fields["Archive"] ?? false);
			}
			set
			{
				base.Fields["Archive"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if ("Room" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageConnectMailboxResource(this.Identity.ToString(), ExchangeResourceType.Room.ToString(), this.Database.ToString());
				}
				if ("Equipment" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageConnectMailboxResource(this.Identity.ToString(), ExchangeResourceType.Equipment.ToString(), this.Database.ToString());
				}
				if ("Linked" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageConnectMailboxLinked(this.Identity.ToString(), this.LinkedMasterAccount.ToString(), this.LinkedDomainController, this.Database.ToString());
				}
				if ("Shared" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageConnectMailboxShared(this.Identity.ToString(), this.Shared.ToString(), this.Database.ToString());
				}
				if ("ValidateOnly" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageConnectMailboxValidateOnly(this.Identity.ToString(), this.ValidateOnly.ToString(), this.Database.ToString());
				}
				return Strings.ConfirmationMessageConnectMailboxUser(this.Identity.ToString(), this.Database.ToString());
			}
		}

		private IConfigurationSession TenantConfigurationSession
		{
			get
			{
				return this.tenantConfigurationSession;
			}
		}

		private IRecipientSession RecipientSession
		{
			get
			{
				return this.recipientSession;
			}
		}

		private IRecipientSession GlobalCatalogSession
		{
			get
			{
				return this.globalCatalogSession;
			}
		}

		private MailboxDatabase OwnerMailboxDatabase
		{
			get
			{
				return this.ownerMailboxDatabase;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				if (this.OwnerMailboxDatabase == null)
				{
					return null;
				}
				return MapiTaskHelper.ConvertDatabaseADObjectToDatabaseId(this.OwnerMailboxDatabase);
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			this.ownerMailboxDatabase = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(this.Database, this.ResourceForestSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(this.Database.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(this.Database.ToString())));
			if (this.OwnerMailboxDatabase.Recovery)
			{
				base.WriteError(new MdbAdminTaskException(Strings.ErrorMailboxResidesInRDB(this.Identity.ToString())), ErrorCategory.InvalidArgument, this.Identity);
			}
			this.databaseLocationInfo = this.ActiveManager.GetServerForDatabase(this.OwnerMailboxDatabase.Guid);
			Server server = this.ownerMailboxDatabase.GetServer();
			if (!server.IsE15OrLater)
			{
				base.WriteError(new MdbAdminTaskException(Strings.ErrorMailboxDatabaseNotOnE15Server(this.Database.ToString())), ErrorCategory.InvalidArgument, this.Identity);
			}
			if (this.mapiAdministrationSession == null)
			{
				this.mapiAdministrationSession = new MapiAdministrationSession(server.ExchangeLegacyDN, Fqdn.Parse(server.Fqdn));
			}
			return this.mapiAdministrationSession;
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			MailboxStatistics deletedStoreMailbox = MailboxTaskHelper.GetDeletedStoreMailbox(base.DataSession, this.Identity, this.RootId, this.Database, new Task.ErrorLoggerDelegate(base.WriteError));
			ADSessionSettings sessionSettings;
			if (deletedStoreMailbox.ExternalDirectoryOrganizationId == Guid.Empty)
			{
				sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
			}
			else
			{
				sessionSettings = ADSessionSettings.FromExternalDirectoryOrganizationId(deletedStoreMailbox.ExternalDirectoryOrganizationId);
			}
			this.tenantConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, sessionSettings, 470, "PrepareDataObject", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MapiTasks\\Mailbox\\ConnectMailbox.cs");
			this.recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, sessionSettings, 476, "PrepareDataObject", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MapiTasks\\Mailbox\\ConnectMailbox.cs");
			this.recipientSession.UseGlobalCatalog = (base.ServerSettings.ViewEntireForest && null == base.DomainController);
			this.globalCatalogSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, sessionSettings, 486, "PrepareDataObject", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MapiTasks\\Mailbox\\ConnectMailbox.cs");
			if (!this.globalCatalogSession.IsReadConnectionAvailable())
			{
				this.globalCatalogSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.PartiallyConsistent, sessionSettings, 494, "PrepareDataObject", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MapiTasks\\Mailbox\\ConnectMailbox.cs");
			}
			TaskLogger.LogExit();
			return deletedStoreMailbox;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if ("Linked" == base.ParameterSetName)
			{
				try
				{
					NetworkCredential userForestCredential = (this.LinkedCredential == null) ? null : this.LinkedCredential.GetNetworkCredential();
					this.linkedUserSid = MailboxTaskHelper.GetAccountSidFromAnotherForest(this.LinkedMasterAccount, this.LinkedDomainController, userForestCredential, this.ResourceForestSession, new MailboxTaskHelper.GetUniqueObject(base.GetDataObject<ADUser>), new Task.ErrorLoggerDelegate(base.ThrowTerminatingError));
				}
				catch (PSArgumentException exception)
				{
					base.ThrowTerminatingError(exception, ErrorCategory.InvalidArgument, this.LinkedCredential);
				}
			}
			if (this.ManagedFolderMailboxPolicy != null)
			{
				ManagedFolderMailboxPolicy managedFolderMailboxPolicy = (ManagedFolderMailboxPolicy)base.GetDataObject<ManagedFolderMailboxPolicy>(this.ManagedFolderMailboxPolicy, this.TenantConfigurationSession, null, new LocalizedString?(Strings.ErrorManagedFolderMailboxPolicyNotFound(this.ManagedFolderMailboxPolicy.ToString())), new LocalizedString?(Strings.ErrorManagedFolderMailboxPolicyNotUnique(this.ManagedFolderMailboxPolicy.ToString())));
				this.elcPolicyId = (ADObjectId)managedFolderMailboxPolicy.Identity;
			}
			if (this.RetentionPolicy != null)
			{
				if (SharedConfiguration.IsDehydratedConfiguration(base.CurrentOrganizationId))
				{
					base.WriteError(new LocalizedException(Strings.ErrorLinkOpOnDehydratedTenant("RetentionPolicy")), ExchangeErrorCategory.Client, null);
				}
				RetentionPolicy retentionPolicy = (RetentionPolicy)base.GetDataObject<RetentionPolicy>(this.RetentionPolicy, this.TenantConfigurationSession, null, new LocalizedString?(Strings.ErrorRetentionPolicyNotFound(this.RetentionPolicy.ToString())), new LocalizedString?(Strings.ErrorRetentionPolicyNotUnique(this.RetentionPolicy.ToString())));
				this.retentionPolicyId = retentionPolicy.Id;
			}
			if (this.ActiveSyncMailboxPolicy != null)
			{
				MobileMailboxPolicy mobileMailboxPolicy = (MobileMailboxPolicy)base.GetDataObject<MobileMailboxPolicy>(this.ActiveSyncMailboxPolicy, this.TenantConfigurationSession, null, new LocalizedString?(Strings.ErrorMobileMailboxPolicyNotFound(this.ActiveSyncMailboxPolicy.ToString())), new LocalizedString?(Strings.ErrorMobileMailboxPolicyNotUnique(this.ActiveSyncMailboxPolicy.ToString())));
				this.mobilePolicyId = (ADObjectId)mobileMailboxPolicy.Identity;
			}
			if (this.AddressBookPolicy != null)
			{
				AddressBookMailboxPolicy addressBookMailboxPolicy = (AddressBookMailboxPolicy)base.GetDataObject<AddressBookMailboxPolicy>(this.AddressBookPolicy, this.TenantConfigurationSession, null, new LocalizedString?(Strings.ErrorAddressBookMailboxPolicyNotFound(this.AddressBookPolicy.ToString())), new LocalizedString?(Strings.ErrorAddressBookMailboxPolicyNotUnique(this.AddressBookPolicy.ToString())), ExchangeErrorCategory.Client);
				this.addressBookPolicyId = (ADObjectId)addressBookMailboxPolicy.Identity;
			}
			MailboxTaskHelper.ValidateMailboxIsDisconnected(this.GlobalCatalogSession, this.DataObject.MailboxGuid, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError));
			if (!this.Archive)
			{
				ConnectMailbox.CheckLegacyDNNotInUse(this.DataObject.Identity, this.DataObject.LegacyDN, this.GlobalCatalogSession, new Task.ErrorLoggerDelegate(base.WriteError));
			}
			if (this.User != null)
			{
				this.userToConnect = (ADUser)base.GetDataObject<ADUser>(this.User, this.RecipientSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(this.User.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(this.User.ToString())));
				if (this.Archive)
				{
					ConnectMailbox.CheckUserForArchive(this.DataObject, this.GlobalCatalogSession, new Task.ErrorLoggerDelegate(base.WriteError), this.userToConnect, this.OwnerMailboxDatabase, this.AllowLegacyDNMismatch);
				}
				else if (RecipientType.User != this.userToConnect.RecipientType)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorNoMatchedUserTypeFound(RecipientType.User.ToString(), this.User.ToString(), this.userToConnect.RecipientType.ToString())), ErrorCategory.InvalidArgument, this.User);
				}
			}
			else if (!this.Archive)
			{
				if ("ValidateOnly" == base.ParameterSetName)
				{
					this.matchedUsers = this.FindMatchedUser(this.DataObject, null);
				}
				else
				{
					this.matchedUsers = this.FindMatchedUser(this.DataObject, new bool?("User" == base.ParameterSetName));
				}
				if ("ValidateOnly" != base.ParameterSetName)
				{
					if (this.matchedUsers.Length == 0)
					{
						base.WriteError(new MdbAdminTaskException(Strings.ErrorNoMatchedUserFound), ErrorCategory.InvalidArgument, this.Identity);
					}
					else if (this.matchedUsers.Length > 1)
					{
						this.WriteWarning(Strings.ErrorMultipleMatchedUser(this.Identity.ToString()));
						this.needListMatchingUser = true;
					}
					else
					{
						this.userToConnect = (ADUser)this.matchedUsers[0];
						this.userToConnect = (ADUser)this.RecipientSession.Read(this.userToConnect.Id);
						if (this.userToConnect == null)
						{
							base.WriteError(new MdbAdminTaskException(Strings.ErrorNoMatchedUserFound), ErrorCategory.InvalidArgument, this.Identity);
						}
						if (this.Archive)
						{
							ConnectMailbox.CheckUserForArchive(this.DataObject, this.GlobalCatalogSession, new Task.ErrorLoggerDelegate(base.WriteError), this.userToConnect, this.OwnerMailboxDatabase, this.AllowLegacyDNMismatch);
						}
					}
				}
			}
			else
			{
				this.userToConnect = this.FindArchiveUser(this.DataObject, this.RecipientSession, new Task.TaskErrorLoggingDelegate(base.WriteError));
				ConnectMailbox.CheckUserForArchive(this.DataObject, this.GlobalCatalogSession, new Task.ErrorLoggerDelegate(base.WriteError), this.userToConnect, this.OwnerMailboxDatabase, this.AllowLegacyDNMismatch);
			}
			if (this.userToConnect != null && !this.Archive)
			{
				if ("User" == base.ParameterSetName)
				{
					if ((this.userToConnect.UserAccountControl & UserAccountControlFlags.AccountDisabled) != UserAccountControlFlags.None && this.DataObject.MailboxType == StoreMailboxType.Private)
					{
						base.WriteError(new RecipientTaskException(Strings.ErrorAccountDisabledForUserMailbox), ErrorCategory.InvalidArgument, this.userToConnect);
					}
				}
				else if ((this.userToConnect.UserAccountControl & UserAccountControlFlags.AccountDisabled) == UserAccountControlFlags.None)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorAccountEnabledForNonUserMailbox), ErrorCategory.InvalidArgument, this.userToConnect);
				}
				if (!string.IsNullOrEmpty(this.Alias))
				{
					this.alias = this.Alias;
				}
				else
				{
					this.alias = RecipientTaskHelper.GenerateUniqueAlias(this.globalCatalogSession, this.userToConnect.OrganizationId, this.userToConnect.Name, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
				}
			}
			if (this.Archive && this.userToConnect.ManagedFolderMailboxPolicy != null)
			{
				base.WriteError(new MdbAdminTaskException(Strings.ErrorNoArchiveWithManagedFolder(this.userToConnect.Name)), ErrorCategory.InvalidData, this.Identity);
			}
			if (this.DataObject.IsArchiveMailbox != null && this.Archive != this.DataObject.IsArchiveMailbox.Value)
			{
				if (this.Archive)
				{
					base.WriteError(new MdbAdminTaskException(Strings.ErrorDisconnectedMailboxNotArchive(this.Identity.ToString(), this.userToConnect.Name)), ErrorCategory.InvalidArgument, this.Identity);
				}
				else
				{
					base.WriteError(new MdbAdminTaskException(Strings.ErrorDisconnectedMailboxNotPrimary(this.Identity.ToString(), this.userToConnect.Name)), ErrorCategory.InvalidArgument, this.Identity);
				}
			}
			ADSessionSettings sessionSettings = ADSessionSettings.FromCustomScopeSet(base.ScopeSet, ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
			MapiTaskHelper.VerifyDatabaseIsWithinScope(sessionSettings, this.OwnerMailboxDatabase, new Task.ErrorLoggerDelegate(base.WriteError));
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject
			});
			if (base.ParameterSetName == "ValidateOnly" || this.needListMatchingUser)
			{
				base.WriteObject(this.matchedUsers, true);
			}
			else if (this.Archive)
			{
				if (!this.Force && this.User == null && !base.ShouldContinue(Strings.ComfirmConnectToMatchingUser(this.userToConnect.Identity.ToString(), this.userToConnect.Alias)))
				{
					TaskLogger.LogExit();
					return;
				}
				ConnectMailbox.ConnectArchiveCore(this.userToConnect, this.DataObject.MailboxGuid, base.ParameterSetName, this.RecipientSession, this.TenantConfigurationSession, (MapiAdministrationSession)base.DataSession, this.alias, this.linkedUserSid, this.databaseLocationInfo, this.OwnerMailboxDatabase, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning));
			}
			else
			{
				Organization orgContainer = this.TenantConfigurationSession.GetOrgContainer();
				if (this.DataObject.MailboxType != StoreMailboxType.Private)
				{
					if (orgContainer.DefaultPublicFolderMailbox.HierarchyMailboxGuid == Guid.Empty)
					{
						if (this.DataObject.MailboxType == StoreMailboxType.PublicFolderSecondary)
						{
							this.WriteWarning(Strings.WarningPromotingSecondaryToPrimary);
						}
					}
					else if (this.DataObject.MailboxType == StoreMailboxType.PublicFolderPrimary)
					{
						this.WriteWarning(Strings.WarningConnectingPrimaryAsSecondary);
					}
				}
				if (!this.Force && this.User == null && !base.ShouldContinue(Strings.ComfirmConnectToMatchingUser(this.userToConnect.Identity.ToString(), this.alias)))
				{
					TaskLogger.LogExit();
					return;
				}
				if (this.elcPolicyId != null && !this.Force && !this.ManagedFolderMailboxPolicyAllowed.IsPresent && !base.ShouldContinue(Strings.ConfirmManagedFolderMailboxPolicyAllowed(this.userToConnect.Identity.ToString())))
				{
					TaskLogger.LogExit();
					return;
				}
				if (!base.IsProvisioningLayerAvailable)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorNoProvisioningHandlerAvailable), ErrorCategory.InvalidOperation, null);
				}
				ADObjectId roleAssignmentPolicyId = null;
				RoleAssignmentPolicy roleAssignmentPolicy = RecipientTaskHelper.FindDefaultRoleAssignmentPolicy(this.TenantConfigurationSession, new Task.ErrorLoggerDelegate(base.WriteError), Strings.ErrorDefaultRoleAssignmentPolicyNotUnique, Strings.ErrorDefaultRoleAssignmentPolicyNotFound);
				if (roleAssignmentPolicy != null)
				{
					roleAssignmentPolicyId = (ADObjectId)roleAssignmentPolicy.Identity;
				}
				ConnectMailbox.ConnectMailboxCore(this.userToConnect, this.DataObject.MailboxGuid, this.DataObject.MailboxType, this.DataObject.LegacyDN, base.ParameterSetName, true, this.RecipientSession, (MapiAdministrationSession)base.DataSession, this.alias, this.linkedUserSid, this.databaseLocationInfo, this.OwnerMailboxDatabase, this.elcPolicyId, this.retentionPolicyId, this.mobilePolicyId, this.addressBookPolicyId, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning), roleAssignmentPolicyId, this);
				if (this.DataObject.MailboxType != StoreMailboxType.Private && orgContainer.DefaultPublicFolderMailbox.HierarchyMailboxGuid == Guid.Empty)
				{
					orgContainer.DefaultPublicFolderMailbox = orgContainer.DefaultPublicFolderMailbox.Clone();
					orgContainer.DefaultPublicFolderMailbox.SetHierarchyMailbox(this.DataObject.MailboxGuid, PublicFolderInformation.HierarchyType.MailboxGuid);
					this.TenantConfigurationSession.Save(orgContainer);
					MailboxTaskHelper.PrepopulateCacheForMailbox(this.OwnerMailboxDatabase, this.databaseLocationInfo.ServerFqdn, this.userToConnect.OrganizationId, this.DataObject.LegacyDN, this.DataObject.MailboxGuid, this.TenantConfigurationSession.LastUsedDc, new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
				}
			}
			TaskLogger.LogExit();
		}

		protected override void ProvisioningUpdateConfigurationObject()
		{
		}

		private ADRecipient[] FindMatchedUser(MailboxStatistics storeMailbox, bool? accountEnabled)
		{
			int num = storeMailbox.LegacyDN.ToUpperInvariant().LastIndexOf("/CN=");
			string propertyValue = storeMailbox.LegacyDN.Substring(num + "/CN=".Length);
			QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ADMailboxRecipientSchema.SamAccountName, propertyValue);
			QueryFilter queryFilter2 = new ComparisonFilter(ComparisonOperator.Equal, ADMailboxRecipientSchema.SamAccountName, storeMailbox.DisplayName);
			QueryFilter queryFilter3 = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.DisplayName, propertyValue);
			QueryFilter queryFilter4 = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.DisplayName, storeMailbox.DisplayName);
			QueryFilter queryFilter5 = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.User);
			AndFilter andFilter = new AndFilter(new QueryFilter[]
			{
				queryFilter5,
				new OrFilter(new QueryFilter[]
				{
					queryFilter,
					queryFilter2,
					queryFilter3,
					queryFilter4
				})
			});
			if (accountEnabled != null && storeMailbox.MailboxType == StoreMailboxType.Private)
			{
				QueryFilter queryFilter6 = new BitMaskAndFilter(ADUserSchema.UserAccountControl, 2UL);
				if (accountEnabled.Value)
				{
					queryFilter6 = new NotFilter(queryFilter6);
				}
				andFilter = new AndFilter(new QueryFilter[]
				{
					andFilter,
					queryFilter6
				});
			}
			return this.GlobalCatalogSession.Find(null, QueryScope.SubTree, andFilter, null, 0);
		}

		internal static ADRecipient FindMailboxByLegacyDN(string legacyDN, IRecipientSession globalCatalogSession)
		{
			QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.LegacyExchangeDN, legacyDN);
			QueryFilter queryFilter2 = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.UserMailbox);
			AndFilter filter = new AndFilter(new QueryFilter[]
			{
				queryFilter2,
				queryFilter
			});
			ADRecipient[] array = globalCatalogSession.Find(null, QueryScope.SubTree, filter, null, 1);
			if (array != null && array.Length > 0)
			{
				return array[0];
			}
			return null;
		}

		internal static void CheckLegacyDNNotInUse(MailboxId disconnectedMailboxIdentity, string disconnectedMailboxLegacyDN, IRecipientSession globalCatalogSession, Task.ErrorLoggerDelegate errorLogger)
		{
			ADRecipient adrecipient = ConnectMailbox.FindMailboxByLegacyDN(disconnectedMailboxLegacyDN, globalCatalogSession);
			if (adrecipient != null)
			{
				errorLogger(new MdbAdminTaskException(Strings.ErrorMailboxLegacyDNInUse(disconnectedMailboxLegacyDN, disconnectedMailboxIdentity.ToString(), adrecipient.DisplayName)), ExchangeErrorCategory.ServerOperation, disconnectedMailboxIdentity);
			}
		}

		private ADUser FindArchiveUser(MailboxStatistics storeMailbox, IRecipientSession globalCatalogSession, Task.TaskErrorLoggingDelegate errorLogger)
		{
			ADRecipient adrecipient = ConnectMailbox.FindMailboxByLegacyDN(storeMailbox.LegacyDN, globalCatalogSession);
			if (adrecipient == null)
			{
				errorLogger(new MdbAdminTaskException(Strings.ErrorRecipientNotFound(storeMailbox.LegacyDN)), ErrorCategory.InvalidArgument, storeMailbox);
			}
			return (ADUser)adrecipient;
		}

		private static void CheckUserForArchive(MailboxStatistics disconnectedMailbox, IRecipientSession globalCatalogSession, Task.ErrorLoggerDelegate errorLogger, ADUser user, MailboxDatabase database, bool allowLegacyDNmismatch)
		{
			if (!string.Equals(user.LegacyExchangeDN, disconnectedMailbox.LegacyDN, StringComparison.OrdinalIgnoreCase) && !allowLegacyDNmismatch)
			{
				errorLogger(new MdbAdminTaskException(Strings.ErrorArchiveLegacyDNDoesNotMatchUser(disconnectedMailbox.LegacyDN, user.LegacyExchangeDN)), ExchangeErrorCategory.Client, disconnectedMailbox);
			}
			if (user.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2010))
			{
				errorLogger(new MdbAdminTaskException(Strings.ErrorArchiveUserVersionTooOld(user.ExchangeVersion.ToString())), ExchangeErrorCategory.Client, disconnectedMailbox);
			}
		}

		private static RawSecurityDescriptor UpdateMailboxSecurityDescriptor(SecurityIdentifier userSid, ADUser userToConnect, MapiAdministrationSession mapiAdministrationSession, MailboxDatabase database, Guid deletedMailboxGuid, string parameterSetName, Task.TaskVerboseLoggingDelegate verboseLogger)
		{
			RawSecurityDescriptor rawSecurityDescriptor = null;
			try
			{
				rawSecurityDescriptor = mapiAdministrationSession.GetMailboxSecurityDescriptor(new MailboxId(MapiTaskHelper.ConvertDatabaseADObjectToDatabaseId(database), deletedMailboxGuid));
			}
			catch (Microsoft.Exchange.Data.Mapi.Common.MailboxNotFoundException)
			{
				rawSecurityDescriptor = new RawSecurityDescriptor(ControlFlags.DiscretionaryAclDefaulted | ControlFlags.SystemAclDefaulted | ControlFlags.SelfRelative, WindowsIdentity.GetCurrent().User, WindowsIdentity.GetCurrent().User, null, null);
				DiscretionaryAcl discretionaryAcl = new DiscretionaryAcl(true, true, 0);
				byte[] binaryForm = new byte[discretionaryAcl.BinaryLength];
				discretionaryAcl.GetBinaryForm(binaryForm, 0);
				rawSecurityDescriptor.DiscretionaryAcl = new RawAcl(binaryForm, 0);
			}
			bool flag = false;
			foreach (GenericAce genericAce in rawSecurityDescriptor.DiscretionaryAcl)
			{
				KnownAce knownAce = (KnownAce)genericAce;
				if (knownAce.SecurityIdentifier.IsWellKnown(WellKnownSidType.SelfSid))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				CommonAce ace = new CommonAce(AceFlags.ContainerInherit, AceQualifier.AccessAllowed, 131073, new SecurityIdentifier(WellKnownSidType.SelfSid, null), false, null);
				rawSecurityDescriptor.DiscretionaryAcl.InsertAce(0, ace);
			}
			rawSecurityDescriptor.SetFlags(rawSecurityDescriptor.ControlFlags | ControlFlags.SelfRelative);
			if ("Linked" == parameterSetName || "Shared" == parameterSetName || "Room" == parameterSetName || "Equipment" == parameterSetName)
			{
				RawSecurityDescriptor sd = userToConnect.ReadSecurityDescriptor();
				MailboxTaskHelper.GrantPermissionToLinkedUserAccount(userToConnect.MasterAccountSid, ref rawSecurityDescriptor, ref sd);
				verboseLogger(Strings.VerboseSaveADSecurityDescriptor(userToConnect.Id.ToString()));
				userToConnect.SaveSecurityDescriptor(sd);
			}
			mapiAdministrationSession.Administration.PurgeCachedMailboxObject(deletedMailboxGuid);
			return rawSecurityDescriptor;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.mapiAdministrationSession != null)
			{
				this.mapiAdministrationSession.Dispose();
				this.mapiAdministrationSession = null;
			}
			base.Dispose(disposing);
		}

		internal static void ConnectMailboxCore(ADUser userToConnect, Guid deletedMailboxGuid, StoreMailboxType mailboxType, string deletedMailboxLegacyDN, string parameterSetName, bool clearPropertiesBeforeConnecting, IRecipientSession recipientSession, MapiAdministrationSession mapiAdministrationSession, string alias, SecurityIdentifier linkedUserSid, DatabaseLocationInfo databaseLocationInfo, MailboxDatabase database, ADObjectId elcPolicyId, ADObjectId retentionPolicyId, ADObjectId mobilePolicyId, ADObjectId addressBookPolicyId, Task.TaskVerboseLoggingDelegate verboseLogger, Task.TaskWarningLoggingDelegate warningLogger, ADObjectId roleAssignmentPolicyId, Task task)
		{
			if (userToConnect.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2012))
			{
				verboseLogger(Strings.VerboseUpdatingVersion(userToConnect.Identity.ToString(), userToConnect.ExchangeVersion.ToString(), ExchangeObjectVersion.Exchange2012.ToString()));
				userToConnect.SetExchangeVersion(ExchangeObjectVersion.Exchange2012);
				recipientSession.Save(userToConnect);
				verboseLogger(Strings.VerboseADOperationSucceeded(userToConnect.Identity.ToString()));
				bool useGlobalCatalog = recipientSession.UseGlobalCatalog;
				try
				{
					recipientSession.UseGlobalCatalog = false;
					userToConnect = (ADUser)recipientSession.Read(userToConnect.Id);
				}
				finally
				{
					recipientSession.UseGlobalCatalog = useGlobalCatalog;
				}
			}
			if (clearPropertiesBeforeConnecting)
			{
				List<PropertyDefinition> list = new List<PropertyDefinition>(RecipientConstants.DisableMailbox_PropertiesToReset);
				MailboxTaskHelper.RemovePersistentProperties(list);
				list.Remove(ADObjectSchema.ExchangeVersion);
				MailboxTaskHelper.ClearExchangeProperties(userToConnect, list);
			}
			userToConnect.Alias = alias;
			if ("Linked" == parameterSetName)
			{
				userToConnect.MasterAccountSid = linkedUserSid;
			}
			else if ("Shared" == parameterSetName)
			{
				userToConnect.MasterAccountSid = new SecurityIdentifier(WellKnownSidType.SelfSid, null);
			}
			else if ("Room" == parameterSetName)
			{
				userToConnect.ResourceType = new ExchangeResourceType?(ExchangeResourceType.Room);
				userToConnect.MasterAccountSid = new SecurityIdentifier(WellKnownSidType.SelfSid, null);
			}
			else if ("Equipment" == parameterSetName)
			{
				userToConnect.ResourceType = new ExchangeResourceType?(ExchangeResourceType.Equipment);
				userToConnect.MasterAccountSid = new SecurityIdentifier(WellKnownSidType.SelfSid, null);
			}
			userToConnect.ServerLegacyDN = databaseLocationInfo.ServerLegacyDN;
			userToConnect.ExchangeGuid = deletedMailboxGuid;
			userToConnect.Database = (ADObjectId)database.Identity;
			userToConnect.LegacyExchangeDN = deletedMailboxLegacyDN.ToLowerInvariant();
			userToConnect.ManagedFolderMailboxPolicy = elcPolicyId;
			userToConnect.RetentionPolicy = retentionPolicyId;
			userToConnect.ActiveSyncMailboxPolicy = mobilePolicyId;
			userToConnect.UseDatabaseQuotaDefaults = new bool?(true);
			userToConnect.AddressBookPolicy = addressBookPolicyId;
			if (roleAssignmentPolicyId != null)
			{
				userToConnect.RoleAssignmentPolicy = roleAssignmentPolicyId;
			}
			if (mailboxType == StoreMailboxType.PublicFolderPrimary || mailboxType == StoreMailboxType.PublicFolderSecondary)
			{
				userToConnect.MasterAccountSid = new SecurityIdentifier(WellKnownSidType.SelfSid, null);
				userToConnect.UserAccountControl = (UserAccountControlFlags.AccountDisabled | UserAccountControlFlags.NormalAccount);
				userToConnect.ExchangeUserAccountControl |= UserAccountControlFlags.AccountDisabled;
				MailboxTaskHelper.StampMailboxRecipientTypes(userToConnect, "PublicFolder");
			}
			else
			{
				MailboxTaskHelper.StampMailboxRecipientTypes(userToConnect, parameterSetName);
			}
			if (MailboxTaskHelper.SupportsMailboxReleaseVersioning(userToConnect))
			{
				userToConnect.MailboxRelease = databaseLocationInfo.MailboxRelease;
			}
			userToConnect.EmailAddressPolicyEnabled = true;
			ProvisioningLayer.UpdateAffectedIConfigurable(task, RecipientTaskHelper.ConvertRecipientToPresentationObject(userToConnect), false);
			recipientSession.Save(userToConnect);
			verboseLogger(Strings.VerboseADOperationSucceeded(userToConnect.Identity.ToString()));
			ConnectMailbox.UpdateSDAndRefreshMailbox(mapiAdministrationSession, userToConnect, database, deletedMailboxGuid, parameterSetName, verboseLogger, warningLogger);
		}

		private static void ConnectArchiveCore(ADUser userToConnect, Guid deletedMailboxGuid, string parameterSetName, IRecipientSession recipientSession, IConfigurationSession configSession, MapiAdministrationSession mapiAdministrationSession, string alias, SecurityIdentifier linkedUserSid, DatabaseLocationInfo databaseLocationInfo, MailboxDatabase database, Task.TaskVerboseLoggingDelegate verboseLogger, Task.TaskWarningLoggingDelegate warningLogger)
		{
			userToConnect.ArchiveDatabase = (ADObjectId)database.Identity;
			userToConnect.ArchiveGuid = deletedMailboxGuid;
			userToConnect.ArchiveName = new MultiValuedProperty<string>(Strings.ArchiveNamePrefix + userToConnect.DisplayName);
			if (MailboxTaskHelper.SupportsMailboxReleaseVersioning(userToConnect))
			{
				userToConnect.ArchiveRelease = databaseLocationInfo.MailboxRelease;
			}
			userToConnect.ArchiveQuota = ProvisioningHelper.DefaultArchiveQuota;
			userToConnect.ArchiveWarningQuota = ProvisioningHelper.DefaultArchiveWarningQuota;
			MailboxTaskHelper.ApplyDefaultArchivePolicy(userToConnect, configSession);
			recipientSession.Save(userToConnect);
			verboseLogger(Strings.VerboseADOperationSucceeded(userToConnect.Identity.ToString()));
			ConnectMailbox.UpdateSDAndRefreshMailbox(mapiAdministrationSession, userToConnect, database, deletedMailboxGuid, parameterSetName, verboseLogger, warningLogger);
		}

		internal static void UpdateSDAndRefreshMailbox(MapiAdministrationSession mapiAdministrationSession, ADUser userToConnect, MailboxDatabase database, Guid mailboxGuid, string parameterSetName, Task.TaskVerboseLoggingDelegate verboseLogger, Task.TaskWarningLoggingDelegate warningLogger)
		{
			ConnectMailbox.UpdateMailboxSecurityDescriptor(userToConnect.Sid, userToConnect, mapiAdministrationSession, database, mailboxGuid, parameterSetName, verboseLogger);
			try
			{
				mapiAdministrationSession.ForceStoreToRefreshMailbox(new MailboxId(MapiTaskHelper.ConvertDatabaseADObjectToDatabaseId(database), mailboxGuid));
			}
			catch (FailedToRefreshMailboxException ex)
			{
				TaskLogger.Trace("An exception is caught and ignored when refreshing the mailbox '{0}'. Exception: {1}", new object[]
				{
					mailboxGuid,
					ex.Message
				});
				warningLogger(Strings.WarningReplicationLatency);
			}
			try
			{
				mapiAdministrationSession.SyncMailboxWithDS(new MailboxId(MapiTaskHelper.ConvertDatabaseADObjectToDatabaseId(database), mailboxGuid));
			}
			catch (DataSourceTransientException ex2)
			{
				TaskLogger.Trace("Swallowing exception {0} from mapi.net", new object[]
				{
					ex2
				});
				warningLogger(ex2.LocalizedString);
			}
			catch (DataSourceOperationException ex3)
			{
				TaskLogger.Trace("Swallowing exception {0} from mapi.net", new object[]
				{
					ex3
				});
				warningLogger(ex3.LocalizedString);
			}
			catch (ArgumentNullException ex4)
			{
				TaskLogger.Trace("Swallowing exception {0} from mapi.net", new object[]
				{
					ex4
				});
				warningLogger(Strings.ErrorNoDatabaseInfor);
			}
		}

		private const string NamePrefixInLegacyDN = "/CN=";

		internal const string ParameterSetValidateOnly = "ValidateOnly";

		private ADUser userToConnect;

		private ADRecipient[] matchedUsers;

		private bool needListMatchingUser;

		private SecurityIdentifier linkedUserSid;

		private ADObjectId elcPolicyId;

		private ADObjectId retentionPolicyId;

		private ADObjectId mobilePolicyId;

		private ADObjectId addressBookPolicyId;

		private string alias;

		private IConfigurationSession tenantConfigurationSession;

		private ITopologyConfigurationSession resourceForestSession;

		private IRecipientSession recipientSession;

		private MailboxDatabase ownerMailboxDatabase;

		private IRecipientSession globalCatalogSession;

		private MapiAdministrationSession mapiAdministrationSession;

		private DatabaseLocationInfo databaseLocationInfo;
	}
}
