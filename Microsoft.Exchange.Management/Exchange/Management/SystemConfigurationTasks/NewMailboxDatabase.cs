using System;
using System.Globalization;
using System.Management.Automation;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "MailboxDatabase", SupportsShouldProcess = true, DefaultParameterSetName = "NonRecovery")]
	public sealed class NewMailboxDatabase : NewDatabaseTask<MailboxDatabase>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if ("Recovery" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageNewMailboxDatabaseRecovery(this.Name.ToString());
				}
				return Strings.ConfirmationMessageNewMailboxDatabaseNonRecovery(this.Name.ToString());
			}
		}

		[Parameter(Mandatory = true, Position = 0, ParameterSetName = "NonRecovery")]
		[Parameter(Mandatory = false, Position = 0, ParameterSetName = "Recovery")]
		[ValidateNotNullOrEmpty]
		public new string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		[Parameter(ParameterSetName = "NonRecovery")]
		public DatabaseIdParameter PublicFolderDatabase
		{
			get
			{
				return (DatabaseIdParameter)base.Fields["PublicFolderDatabase"];
			}
			set
			{
				base.Fields["PublicFolderDatabase"] = value;
			}
		}

		[Parameter(ParameterSetName = "NonRecovery")]
		public OfflineAddressBookIdParameter OfflineAddressBook
		{
			get
			{
				return (OfflineAddressBookIdParameter)base.Fields["OfflineAddressBook"];
			}
			set
			{
				base.Fields["OfflineAddressBook"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Recovery")]
		public SwitchParameter Recovery
		{
			get
			{
				return (SwitchParameter)(base.Fields["Recovery"] ?? false);
			}
			set
			{
				base.Fields["Recovery"] = value;
			}
		}

		[Parameter(ParameterSetName = "NonRecovery")]
		public bool IsExcludedFromProvisioning
		{
			get
			{
				return (bool)(base.Fields["IsExcludedFromProvisioning"] ?? Datacenter.IsMicrosoftHostedOnly(true));
			}
			set
			{
				base.Fields["IsExcludedFromProvisioning"] = value;
			}
		}

		[Parameter(ParameterSetName = "NonRecovery")]
		public bool IsSuspendedFromProvisioning
		{
			get
			{
				return (bool)(base.Fields["IsSuspendedProvisioning"] ?? false);
			}
			set
			{
				base.Fields["IsSuspendedProvisioning"] = value;
			}
		}

		[Parameter(ParameterSetName = "NonRecovery")]
		public SwitchParameter IsExcludedFromInitialProvisioning
		{
			get
			{
				return (SwitchParameter)base.Fields["IsExcludedFromInitialProvisioning"];
			}
			set
			{
				base.Fields["IsExcludedFromInitialProvisioning"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNull]
		public MailboxProvisioningAttributes MailboxProvisioningAttributes
		{
			get
			{
				return (MailboxProvisioningAttributes)base.Fields[DatabaseSchema.MailboxProvisioningAttributes];
			}
			set
			{
				base.Fields[DatabaseSchema.MailboxProvisioningAttributes] = value;
			}
		}

		[Parameter(ParameterSetName = "NonRecovery")]
		public bool AutoDagExcludeFromMonitoring
		{
			get
			{
				return (bool)(base.Fields["AutoDagExcludeFromMonitoring"] ?? false);
			}
			set
			{
				base.Fields["AutoDagExcludeFromMonitoring"] = value;
			}
		}

		protected override NewDatabaseTask<MailboxDatabase>.ExchangeDatabaseType DatabaseType
		{
			get
			{
				return NewDatabaseTask<MailboxDatabase>.ExchangeDatabaseType.Private;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			bool flag = false;
			try
			{
				base.InternalProcessRecord();
				this.WriteWarning(Strings.WarnAdministratorToRestartService(base.OwnerServer.Name));
				flag = true;
			}
			finally
			{
				if (!flag && this.preExistingDatabase == null)
				{
					this.RollbackOperation(this.DataObject, this.dbCopy, this.systemMailbox);
				}
			}
			TaskLogger.LogExit();
		}

		private void RunConfigurationUpdaterRpc(Database db)
		{
			DatabaseTasksHelper.RunConfigurationUpdaterRpcAsync(base.OwnerServer.Fqdn, db, ReplayConfigChangeHints.DbCopyAdded, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning));
		}

		private void RollbackOperation(MailboxDatabase mdb, DatabaseCopy dbCopy, ADSystemMailbox systemMailbox)
		{
			if (mdb == null || dbCopy == null)
			{
				if (systemMailbox != null)
				{
					try
					{
						base.WriteVerbose(Strings.VerboseDeleteSystemMailbox(systemMailbox.Id.ToString()));
						this.RecipientSessionForSystemMailbox.Delete(systemMailbox);
					}
					catch (DataSourceTransientException ex)
					{
						this.WriteWarning(Strings.FailedToDeleteSystemMailbox(systemMailbox.Identity.ToString(), ex.Message));
						TaskLogger.Trace("Failed to delete System Mailbox {0} when rolling back created database object '{1}'. {2}", new object[]
						{
							systemMailbox.Identity,
							mdb.Identity,
							ex.ToString()
						});
					}
					catch (DataSourceOperationException ex2)
					{
						this.WriteWarning(Strings.FailedToDeleteSystemMailbox(systemMailbox.Identity.ToString(), ex2.Message));
						TaskLogger.Trace("Failed to delete System Mailbox {0} when rolling back created database object '{1}'. {2}", new object[]
						{
							systemMailbox.Identity,
							mdb.Identity,
							ex2.ToString()
						});
					}
				}
				if (dbCopy != null)
				{
					try
					{
						base.WriteVerbose(Strings.VerboseDeleteDBCopy(dbCopy.Id.ToString()));
						base.DataSession.Delete(dbCopy);
					}
					catch (DataSourceTransientException ex3)
					{
						this.WriteWarning(Strings.FailedToDeleteDatabaseCopy(dbCopy.Identity.ToString(), ex3.Message));
						TaskLogger.Trace("Failed to delete Database Copy {0} when rolling back created database object '{1}'. {2}", new object[]
						{
							dbCopy.Identity,
							mdb.Identity,
							ex3.ToString()
						});
					}
					catch (DataSourceOperationException ex4)
					{
						this.WriteWarning(Strings.FailedToDeleteDatabaseCopy(dbCopy.Identity.ToString(), ex4.Message));
						TaskLogger.Trace("Failed to delete Database Copy {0} when rolling back created database object '{1}'. {2}", new object[]
						{
							dbCopy.Identity,
							mdb.Identity,
							ex4.ToString()
						});
					}
				}
				if (mdb != null)
				{
					try
					{
						base.WriteVerbose(Strings.VerboseDeleteMDB(mdb.Id.ToString()));
						base.DataSession.Delete(mdb);
						DatabaseTasksHelper.RemoveDatabaseFromClusterDB((ITopologyConfigurationSession)base.DataSession, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskErrorLoggingDelegate(base.WriteError), mdb);
					}
					catch (DataSourceTransientException ex5)
					{
						this.WriteWarning(Strings.FailedToDeleteMailboxDatabase(mdb.Identity.ToString(), ex5.Message));
						TaskLogger.Trace("Failed to delete Mailbox Database {0} when rolling back. {1}", new object[]
						{
							mdb.Identity,
							ex5.ToString()
						});
					}
					catch (DataSourceOperationException ex6)
					{
						this.WriteWarning(Strings.FailedToDeleteMailboxDatabase(mdb.Identity.ToString(), ex6.Message));
						TaskLogger.Trace("Failed to delete Mailbox Database {0} when rolling back. {1}", new object[]
						{
							mdb.Identity,
							ex6.ToString()
						});
					}
					catch (ClusterException ex7)
					{
						this.WriteWarning(Strings.FailedToDeleteMailboxDatabase(mdb.Identity.ToString(), ex7.Message));
						TaskLogger.Trace("Failed to delete Mailbox Database {0} when rolling back. {1}", new object[]
						{
							mdb.Identity,
							ex7.ToString()
						});
					}
				}
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			MailboxDatabase mailboxDatabase = (MailboxDatabase)base.PrepareDataObject();
			if (this.preExistingDatabase != null)
			{
				TaskLogger.LogExit();
				return mailboxDatabase;
			}
			if (base.ParameterSetName == "Recovery")
			{
				mailboxDatabase.Recovery = true;
				mailboxDatabase.AllowFileRestore = true;
			}
			else
			{
				mailboxDatabase.Recovery = false;
			}
			this.DataObject.IsExcludedFromProvisioning = this.IsExcludedFromProvisioning;
			if (base.Fields.IsModified("IsSuspendedProvisioning"))
			{
				this.DataObject.IsSuspendedFromProvisioning = this.IsSuspendedFromProvisioning;
			}
			if (base.Fields.IsModified("IsExcludedFromInitialProvisioning"))
			{
				this.DataObject.IsExcludedFromInitialProvisioning = this.IsExcludedFromInitialProvisioning;
			}
			base.PrepareFilePaths(mailboxDatabase.Name, base.ParameterSetName == "Recovery", mailboxDatabase);
			if (base.Fields.IsModified(DatabaseSchema.MailboxProvisioningAttributes))
			{
				this.DataObject.MailboxProvisioningAttributes = this.MailboxProvisioningAttributes;
			}
			TaskLogger.LogExit();
			return mailboxDatabase;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (this.preExistingDatabase != null)
			{
				TaskLogger.LogExit();
				return;
			}
			if (this.PublicFolderDatabase == null)
			{
				if (base.OwnerServerPublicFolderDatabases != null && base.OwnerServerPublicFolderDatabases.Length > 0)
				{
					this.DataObject.PublicFolderDatabase = (ADObjectId)base.OwnerServerPublicFolderDatabases[0].Identity;
				}
				else
				{
					base.WriteVerbose(Strings.VerboseFindClosestPublicFolderDatabaseFromServer(base.OwnerServer.Id.ToString()));
					this.DataObject.PublicFolderDatabase = Microsoft.Exchange.Data.Directory.SystemConfiguration.PublicFolderDatabase.FindClosestPublicFolderDatabase(base.DataSession, base.OwnerServer.Id);
				}
			}
			else
			{
				this.PublicFolderDatabase.AllowLegacy = true;
				IConfigurable dataObject = base.GetDataObject<PublicFolderDatabase>(this.PublicFolderDatabase, base.DataSession, null, new LocalizedString?(Strings.ErrorPublicFolderDatabaseNotFound(this.PublicFolderDatabase.ToString())), new LocalizedString?(Strings.ErrorPublicFolderDatabaseNotUnique(this.PublicFolderDatabase.ToString())));
				this.DataObject.PublicFolderDatabase = (ADObjectId)dataObject.Identity;
			}
			if (this.OfflineAddressBook != null)
			{
				IConfigurable dataObject2 = base.GetDataObject<OfflineAddressBook>(this.OfflineAddressBook, base.DataSession, null, new LocalizedString?(Strings.ErrorOfflineAddressBookNotFound(this.OfflineAddressBook.ToString())), new LocalizedString?(Strings.ErrorOfflineAddressBookNotUnique(this.OfflineAddressBook.ToString())));
				this.DataObject.OfflineAddressBook = (ADObjectId)dataObject2.Identity;
			}
			base.ValidateFilePaths(base.ParameterSetName == "Recovery");
			if (base.Fields.IsModified("AutoDagExcludeFromMonitoring"))
			{
				this.DataObject.AutoDagExcludeFromMonitoring = this.AutoDagExcludeFromMonitoring;
			}
			else
			{
				DatabaseAvailabilityGroup databaseAvailabilityGroup = DagTaskHelper.ReadDag(this.DataObject.MasterServerOrAvailabilityGroup, this.ConfigurationSession);
				if (databaseAvailabilityGroup != null)
				{
					DatabaseAvailabilityGroupConfiguration databaseAvailabilityGroupConfiguration = DagConfigurationHelper.ReadDagConfig(databaseAvailabilityGroup.DatabaseAvailabilityGroupConfiguration, this.ConfigurationSession);
					if (databaseAvailabilityGroupConfiguration != null)
					{
						DagConfigurationHelper dagConfigurationHelper = DagConfigurationHelper.Deserialize(databaseAvailabilityGroupConfiguration.ConfigurationXML);
						if (dagConfigurationHelper.MinCopiesPerDatabaseForMonitoring > 1)
						{
							this.DataObject.AutoDagExcludeFromMonitoring = true;
						}
					}
				}
			}
			TaskLogger.LogExit();
		}

		protected override void WriteResult()
		{
			TaskLogger.LogEnter();
			this.dbCopy = base.SaveDBCopy();
			if (this.preExistingDatabase != null)
			{
				TaskLogger.LogExit();
				return;
			}
			MailboxDatabase mailboxDatabase = (MailboxDatabase)base.GetDataObject(new DatabaseIdParameter((ADObjectId)this.DataObject.Identity));
			try
			{
				int maximumSupportedDatabaseSchemaVersion = DatabaseTasksHelper.GetMaximumSupportedDatabaseSchemaVersion((ITopologyConfigurationSession)base.DataSession, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskErrorLoggingDelegate(base.WriteError), mailboxDatabase);
				DatabaseTasksHelper.SetRequestedDatabaseSchemaVersion((ITopologyConfigurationSession)base.DataSession, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning), null, mailboxDatabase, maximumSupportedDatabaseSchemaVersion);
			}
			catch (ClusterException)
			{
			}
			mailboxDatabase.CompleteAllCalculatedProperties();
			this.RunConfigurationUpdaterRpc(mailboxDatabase);
			this.systemMailbox = NewMailboxDatabase.SaveSystemMailbox(mailboxDatabase, base.OwnerServer, base.RootOrgContainerId, (ITopologyConfigurationSession)this.ConfigurationSession, this.RecipientSessionForSystemMailbox, this.forcedReplicationSites, new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
			base.WriteObject(mailboxDatabase);
			TaskLogger.LogExit();
		}

		internal static ADSystemMailbox SaveSystemMailbox(MailboxDatabase mdb, Server owningServer, ADObjectId rootOrgContainerId, ITopologyConfigurationSession configSession, IRecipientSession recipientSession, ADObjectId[] forcedReplicationSites, Task.TaskWarningLoggingDelegate writeWarning, Task.TaskVerboseLoggingDelegate writeVerbose)
		{
			TaskLogger.LogEnter();
			bool useConfigNC = configSession.UseConfigNC;
			bool useGlobalCatalog = configSession.UseGlobalCatalog;
			string text = "SystemMailbox" + mdb.Guid.ToString("B");
			SecurityIdentifier securityIdentifier = new SecurityIdentifier("SY");
			ADSystemMailbox adsystemMailbox = new ADSystemMailbox();
			adsystemMailbox.StampPersistableDefaultValues();
			adsystemMailbox.Name = text;
			adsystemMailbox.DisplayName = text;
			adsystemMailbox.Alias = text;
			adsystemMailbox.HiddenFromAddressListsEnabled = true;
			adsystemMailbox.Database = mdb.Id;
			if (owningServer == null)
			{
				throw new InvalidOperationException(Strings.ErrorDBOwningServerNotFound(mdb.Identity.ToString()));
			}
			adsystemMailbox.ServerLegacyDN = owningServer.ExchangeLegacyDN;
			adsystemMailbox.ExchangeGuid = Guid.NewGuid();
			AcceptedDomain defaultAcceptedDomain = configSession.GetDefaultAcceptedDomain();
			if (defaultAcceptedDomain == null || defaultAcceptedDomain.DomainName == null || defaultAcceptedDomain.DomainName.Domain == null)
			{
				throw new ManagementObjectNotFoundException(Strings.ErrorNoDefaultAcceptedDomainFound(mdb.Identity.ToString()));
			}
			adsystemMailbox.EmailAddresses.Add(ProxyAddress.Parse("SMTP:" + adsystemMailbox.Alias + "@" + defaultAcceptedDomain.DomainName.Domain.ToString()));
			adsystemMailbox.WindowsEmailAddress = adsystemMailbox.PrimarySmtpAddress;
			adsystemMailbox.SendModerationNotifications = TransportModerationNotificationFlags.Never;
			Organization organization = configSession.Read<Organization>(rootOrgContainerId);
			if (organization == null)
			{
				throw new ManagementObjectNotFoundException(Strings.ErrorOrganizationNotFound(rootOrgContainerId.Name));
			}
			string parentLegacyDN = string.Format(CultureInfo.InvariantCulture, "{0}/ou={1}/cn=Recipients", new object[]
			{
				organization.LegacyExchangeDN,
				configSession.GetAdministrativeGroupId().Name
			});
			adsystemMailbox.LegacyExchangeDN = LegacyDN.GenerateLegacyDN(parentLegacyDN, adsystemMailbox);
			ADComputer adcomputer;
			try
			{
				configSession.UseConfigNC = false;
				configSession.UseGlobalCatalog = true;
				adcomputer = configSession.FindComputerByHostName(owningServer.Name);
			}
			finally
			{
				configSession.UseConfigNC = useConfigNC;
				configSession.UseGlobalCatalog = useGlobalCatalog;
			}
			if (adcomputer == null)
			{
				throw new ManagementObjectNotFoundException(Strings.ErrorDBOwningServerNotFound(mdb.Identity.ToString()));
			}
			ADObjectId adobjectId = adcomputer.Id.DomainId;
			adobjectId = adobjectId.GetChildId("Microsoft Exchange System Objects");
			adsystemMailbox.SetId(adobjectId.GetChildId(text));
			GenericAce[] aces = new GenericAce[]
			{
				new CommonAce(AceFlags.None, AceQualifier.AccessAllowed, 131075, securityIdentifier, false, null)
			};
			DirectoryCommon.SetAclOnAlternateProperty(adsystemMailbox, aces, ADSystemAttendantMailboxSchema.ExchangeSecurityDescriptor, securityIdentifier, securityIdentifier);
			recipientSession.LinkResolutionServer = mdb.OriginatingServer;
			bool enforceDefaultScope = recipientSession.EnforceDefaultScope;
			try
			{
				writeVerbose(TaskVerboseStringHelper.GetSaveObjectVerboseString(adsystemMailbox, recipientSession, typeof(ADSystemMailbox)));
				recipientSession.EnforceDefaultScope = false;
				recipientSession.Save(adsystemMailbox);
			}
			catch (ADConstraintViolationException ex)
			{
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ex.Server, false, ConsistencyMode.PartiallyConsistent, configSession.SessionSettings, 705, "SaveSystemMailbox", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\database\\NewMailboxDatabase.cs");
				if (!tenantOrTopologyConfigurationSession.ReplicateSingleObjectToTargetDC(mdb, ex.Server))
				{
					throw;
				}
				writeVerbose(TaskVerboseStringHelper.GetSaveObjectVerboseString(adsystemMailbox, recipientSession, typeof(ADSystemMailbox)));
				recipientSession.Save(adsystemMailbox);
			}
			finally
			{
				writeVerbose(TaskVerboseStringHelper.GetSourceVerboseString(recipientSession));
				recipientSession.EnforceDefaultScope = enforceDefaultScope;
			}
			if (forcedReplicationSites != null)
			{
				DagTaskHelper.ForceReplication(recipientSession, adsystemMailbox, forcedReplicationSites, mdb.Name, writeWarning, writeVerbose);
			}
			TaskLogger.LogExit();
			return adsystemMailbox;
		}

		internal const string paramPublicFolderDatabase = "PublicFolderDatabase";

		internal const string paramOfflineAddressBook = "OfflineAddressBook";

		internal const string paramRecovery = "Recovery";

		internal const string paramIsExcludedFromProvisioning = "IsExcludedFromProvisioning";

		internal const string paramIsSuspendedFromProvisioning = "IsSuspendedProvisioning";

		internal const string paramIsExcludedFromInitialProvisioning = "IsExcludedFromInitialProvisioning";

		internal const string paramAutoDagExcludeFromMonitoring = "AutoDagExcludeFromMonitoring";

		private ADSystemMailbox systemMailbox;
	}
}
