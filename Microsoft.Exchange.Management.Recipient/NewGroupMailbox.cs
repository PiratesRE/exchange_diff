using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Performance;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Provisioning.LoadBalancing;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("New", "GroupMailbox", DefaultParameterSetName = "GroupMailbox", SupportsShouldProcess = true)]
	public sealed class NewGroupMailbox : NewMailboxOrSyncMailbox
	{
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "GroupMailbox", Position = 0)]
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

		[Parameter(Mandatory = false, ParameterSetName = "GroupMailbox")]
		public ModernGroupTypeInfo ModernGroupType
		{
			get
			{
				object obj = base.Fields[ADRecipientSchema.ModernGroupType];
				if (obj == null)
				{
					return ModernGroupTypeInfo.Public;
				}
				return (ModernGroupTypeInfo)obj;
			}
			set
			{
				base.Fields[ADRecipientSchema.ModernGroupType] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "GroupMailbox")]
		public RecipientIdParameter[] Owners
		{
			get
			{
				return (RecipientIdParameter[])base.Fields[ADUserSchema.Owners];
			}
			set
			{
				base.Fields[ADUserSchema.Owners] = (value ?? new RecipientIdParameter[0]);
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "GroupMailbox")]
		private RecipientIdParameter[] PublicToGroups
		{
			get
			{
				return (RecipientIdParameter[])base.Fields[ADMailboxRecipientSchema.DelegateListLink];
			}
			set
			{
				base.Fields[ADMailboxRecipientSchema.DelegateListLink] = (value ?? new RecipientIdParameter[0]);
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "GroupMailbox")]
		[ValidateNotNullOrEmpty]
		public string Description
		{
			get
			{
				return (string)base.Fields[ADRecipientSchema.Description];
			}
			set
			{
				base.Fields[ADRecipientSchema.Description] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "GroupMailbox")]
		public RecipientIdParameter ExecutingUser
		{
			get
			{
				return (RecipientIdParameter)base.Fields["ExecutingUser"];
			}
			set
			{
				base.Fields["ExecutingUser"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "GroupMailbox")]
		public RecipientIdParameter[] Members
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["Members"];
			}
			set
			{
				base.Fields["Members"] = (value ?? new RecipientIdParameter[0]);
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "GroupMailbox")]
		public bool RequireSenderAuthenticationEnabled
		{
			get
			{
				return (bool)base.Fields["RequireSenderAuthenticationEnabled"];
			}
			set
			{
				base.Fields["RequireSenderAuthenticationEnabled"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "GroupMailbox")]
		public SwitchParameter AutoSubscribeNewGroupMembers
		{
			get
			{
				return (SwitchParameter)base.Fields["AutoSubscribeNewGroupMembers"];
			}
			set
			{
				base.Fields["AutoSubscribeNewGroupMembers"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "GroupMailbox")]
		public CultureInfo Language
		{
			get
			{
				return (CultureInfo)base.Fields["Language"];
			}
			set
			{
				base.Fields["Language"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "GroupMailbox")]
		public Uri SharePointUrl
		{
			get
			{
				return (Uri)base.Fields[ADMailboxRecipientSchema.SharePointUrl];
			}
			set
			{
				base.Fields[ADMailboxRecipientSchema.SharePointUrl] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "GroupMailbox")]
		public MultiValuedProperty<string> SharePointResources
		{
			get
			{
				return (MultiValuedProperty<string>)base.Fields[ADMailboxRecipientSchema.SharePointResources];
			}
			set
			{
				base.Fields[ADMailboxRecipientSchema.SharePointResources] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "GroupMailbox")]
		public string ValidationOrganization
		{
			get
			{
				return (string)base.Fields["ValidationOrganization"];
			}
			set
			{
				base.Fields["ValidationOrganization"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "GroupMailbox")]
		public RecipientIdType RecipientIdType
		{
			get
			{
				return (RecipientIdType)base.Fields["RecipientIdType"];
			}
			set
			{
				base.Fields["RecipientIdType"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "GroupMailbox")]
		public SwitchParameter FromSyncClient
		{
			get
			{
				return (SwitchParameter)(base.Fields["FromSyncClient"] ?? false);
			}
			set
			{
				base.Fields["FromSyncClient"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ProxyAddressCollection EmailAddresses
		{
			get
			{
				return (ProxyAddressCollection)base.Fields[GroupMailboxSchema.EmailAddresses];
			}
			set
			{
				base.Fields[GroupMailboxSchema.EmailAddresses] = value;
			}
		}

		private new SwitchParameter AccountDisabled { get; set; }

		private new MailboxPolicyIdParameter ActiveSyncMailboxPolicy { get; set; }

		private new AddressBookMailboxPolicyIdParameter AddressBookPolicy { get; set; }

		private new SwitchParameter Arbitration { get; set; }

		private new MailboxIdParameter ArbitrationMailbox { get; set; }

		private new SwitchParameter Archive { get; set; }

		private new DatabaseIdParameter ArchiveDatabase { get; set; }

		private new SmtpDomain ArchiveDomain { get; set; }

		private new SwitchParameter BypassLiveId { get; set; }

		private new SwitchParameter Discovery { get; set; }

		private new SwitchParameter Equipment { get; set; }

		private new SwitchParameter EvictLiveId { get; set; }

		private new string FederatedIdentity { get; set; }

		private new SwitchParameter ForestWideDomainControllerAffinityByExecutingUser { get; set; }

		private new string FirstName { get; set; }

		private new SwitchParameter Force { get; set; }

		private new string ImmutableId { get; set; }

		private new SwitchParameter ImportLiveId { get; set; }

		private new string Initials { get; set; }

		private new string LastName { get; set; }

		private new PSCredential LinkedCredential { get; set; }

		private new string LinkedDomainController { get; set; }

		private new UserIdParameter LinkedMasterAccount { get; set; }

		private new MailboxPlanIdParameter MailboxPlan { get; set; }

		private new Guid MailboxContainerGuid { get; set; }

		private new WindowsLiveId MicrosoftOnlineServicesID { get; set; }

		private new MultiValuedProperty<ModeratorIDParameter> ModeratedBy { get; set; }

		private new bool ModerationEnabled { get; set; }

		private new NetID NetID { get; set; }

		private new SecureString Password { get; set; }

		private new SwitchParameter PublicFolder { get; set; }

		private new SwitchParameter HoldForMigration { get; set; }

		private new bool QueryBaseDNRestrictionEnabled { get; set; }

		private new RemoteAccountPolicyIdParameter RemoteAccountPolicy { get; set; }

		private new SwitchParameter RemoteArchive { get; set; }

		private new bool RemotePowerShellEnabled { get; set; }

		private new RemovedMailboxIdParameter RemovedMailbox { get; set; }

		private new bool ResetPasswordOnNextLogon { get; set; }

		private new MailboxPolicyIdParameter RetentionPolicy { get; set; }

		private new MailboxPolicyIdParameter RoleAssignmentPolicy { get; set; }

		private new SwitchParameter Room { get; set; }

		private new string SamAccountName { get; set; }

		private new TransportModerationNotificationFlags SendModerationNotifications { get; set; }

		private new SwitchParameter Shared { get; set; }

		private new SharingPolicyIdParameter SharingPolicy { get; set; }

		private new bool SKUAssigned { get; set; }

		private new MultiValuedProperty<Capability> AddOnSKUCapability { get; set; }

		private new Capability SKUCapability { get; set; }

		private new SwitchParameter TargetAllMDBs { get; set; }

		private new ThrottlingPolicyIdParameter ThrottlingPolicy { get; set; }

		private new CountryInfo UsageLocation { get; set; }

		private new SwitchParameter UseExistingLiveId { get; set; }

		private new string UserPrincipalName { get; set; }

		private new WindowsLiveId WindowsLiveID { get; set; }

		private new SecureString RoomMailboxPassword { get; set; }

		private new bool EnableRoomMailboxAccount { get; set; }

		private new bool IsExcludedFromServingHierarchy { get; set; }

		protected override bool ShouldGenerateWindowsLiveID
		{
			get
			{
				return false;
			}
		}

		protected override void InternalBeginProcessing()
		{
			if (this.ValidationOrganization != null && !string.Equals(this.ValidationOrganization, base.CurrentOrganizationId.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				base.ThrowTerminatingError(new ValidationOrgCurrentOrgNotMatchException(this.ValidationOrganization, base.CurrentOrganizationId.ToString()), ExchangeErrorCategory.Client, null);
			}
			if (base.Fields.IsChanged(ADRecipientSchema.ModernGroupType))
			{
				this.groupType = (ModernGroupObjectType)base.Fields[ADRecipientSchema.ModernGroupType];
			}
			if (base.Fields.IsChanged(ADMailboxRecipientSchema.DelegateListLink) && this.groupType != ModernGroupObjectType.Public)
			{
				base.WriteError(new GroupMailboxInvalidOperationException(Strings.ErrorInvalidGroupTypeForPublicToGroups), ExchangeErrorCategory.Client, null);
			}
			if (this.IsWarmupCall())
			{
				this.InitializeDependencies();
			}
			if (this.FromSyncClient && base.Fields.IsChanged("Members"))
			{
				base.WriteError(new GroupMailboxInvalidOperationException(Strings.ErrorFromSyncClientAndMembersUsedTogether), ExchangeErrorCategory.Client, null);
			}
			base.InternalBeginProcessing();
		}

		protected override void InternalStateReset()
		{
			if (base.Database == null)
			{
				if (base.CurrentTaskContext.InvocationInfo.IsCmdletInvokedWithoutPSFramework || ExEnvironment.IsSdfDomain)
				{
					this.selectedLocalDatabase = this.FindDatabaseForProvisioning();
					if (this.selectedLocalDatabase == null)
					{
						this.WriteError(new RecipientTaskException(Strings.ErrorParameterRequiredButNotProvisioned("Database")), ExchangeErrorCategory.ServerOperation, null, true);
					}
					base.UserSpecifiedParameters["Database"] = new DatabaseIdParameter(this.selectedLocalDatabase.MailboxDatabase);
				}
				else
				{
					base.WriteVerbose(new LocalizedString(string.Format("Using MailboxProvisioningHandler to select a database. IsCmdletInvokedWithoutPSFramework={0}, IsSDFDomain={1}", base.CurrentTaskContext.InvocationInfo.IsCmdletInvokedWithoutPSFramework, ExEnvironment.IsSdfDomain)));
				}
			}
			using (new StopwatchPerformanceTracker("NewGroupMailbox.GenerateUniqueName", GenericCmdletInfoDataLogger.Instance))
			{
				this.GenerateUniqueNameIfRequired();
			}
			base.InternalStateReset();
		}

		protected override void ValidateProvisionedProperties(IConfigurable dataObject)
		{
			if (this.selectedLocalDatabase != null)
			{
				ADUser aduser = (ADUser)dataObject;
				aduser.DatabaseAndLocation = this.selectedLocalDatabase;
				aduser.Database = this.selectedLocalDatabase.MailboxDatabase.Id;
			}
			base.ValidateProvisionedProperties(dataObject);
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewGroupMailbox(this.Name.ToString(), base.UserPrincipalName.ToString(), base.RecipientContainerId.ToString());
			}
		}

		protected override void PrepareUserObject(ADUser user)
		{
			TaskLogger.LogEnter();
			base.PrepareUserObject(user);
			user.SetExchangeVersion(ADUser.GetMaximumSupportedExchangeObjectVersion(RecipientTypeDetails.GroupMailbox, false));
			user.ModernGroupType = this.groupType;
			if (base.Fields.IsChanged(ADRecipientSchema.Description))
			{
				user.Description.Add((string)base.Fields[ADRecipientSchema.Description]);
			}
			if (base.Fields.IsChanged("RequireSenderAuthenticationEnabled"))
			{
				user.RequireAllSendersAreAuthenticated = this.RequireSenderAuthenticationEnabled;
			}
			else
			{
				user.RequireAllSendersAreAuthenticated = true;
			}
			if (base.Fields.IsChanged("AutoSubscribeNewGroupMembers"))
			{
				user.AutoSubscribeNewGroupMembers = this.AutoSubscribeNewGroupMembers;
			}
			if (string.IsNullOrEmpty(user.ExternalDirectoryObjectId) && VariantConfiguration.InvariantNoFlightingSnapshot.CmdletInfra.GenerateNewExternalDirectoryObjectId.Enabled)
			{
				user.ExternalDirectoryObjectId = Guid.NewGuid().ToString("D");
			}
			if (base.Fields.IsChanged(ADMailboxRecipientSchema.SharePointUrl) && this.SharePointUrl != null)
			{
				if (user.SharePointResources == null)
				{
					user.SharePointResources = new MultiValuedProperty<string>();
				}
				user.SharePointResources.Add("SiteUrl=" + this.SharePointUrl);
			}
			if (base.Fields.IsChanged(ADMailboxRecipientSchema.SharePointResources))
			{
				user.SharePointResources = this.SharePointResources;
				user.SharePointUrl = null;
			}
			if (base.Fields.IsChanged(GroupMailboxSchema.EmailAddresses) && !this.FromSyncClient)
			{
				user.EmailAddresses = this.EmailAddresses;
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			IRecipientSession recipientSession = (IRecipientSession)base.DataSession;
			using (new StopwatchPerformanceTracker("NewGroupMailbox.CreateContext", GenericCmdletInfoDataLogger.Instance))
			{
				this.groupMailboxContext = new GroupMailboxContext(this.DataObject, base.CurrentOrganizationId, recipientSession, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADGroup>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.ErrorLoggerDelegate(base.WriteError));
			}
			if (base.Fields.IsChanged(ADMailboxRecipientSchema.DelegateListLink))
			{
				this.groupMailboxContext.AddPublicToGroups(this.PublicToGroups);
			}
			if (base.Fields.IsChanged("ExecutingUser") && this.ExecutingUser != null)
			{
				this.groupMailboxContext.SetExecutingUser(this.ExecutingUser);
			}
			if (base.Fields.IsChanged("Language") && this.Language != null)
			{
				this.DataObject.Languages.Add(this.Language);
			}
			else
			{
				this.DataObject.Languages.Add(CultureInfo.CreateSpecificCulture("en-US"));
			}
			if (base.Fields.IsChanged(ADUserSchema.Owners) && this.Owners != null)
			{
				this.groupMailboxContext.SetOwners(this.Owners);
			}
			using (new CmdletMonitoredScope(base.CurrentTaskContext.UniqueId, "NewGroupMailbox", "BaseInternalProcessRecord", LoggerHelper.CmdletPerfMonitors))
			{
				base.InternalProcessRecord();
				this.groupMailboxContext.SetGroupAdUser(this.DataObject);
			}
			if (!this.FromSyncClient)
			{
				if (base.Fields.IsChanged("Members") && this.Members != null)
				{
					this.groupMailboxContext.SetMembers(this.Members);
				}
				Exception ex;
				ExchangeErrorCategory? exchangeErrorCategory;
				this.groupMailboxContext.NewGroupMailbox(this.databaseLocationInfo, out ex, out exchangeErrorCategory);
				if (ex != null)
				{
					base.WriteError(new GroupMailboxFailedToLogonException(Strings.ErrorUnableToLogonGroupMailbox(this.DataObject.ExchangeGuid, this.databaseLocationInfo.ServerFqdn, recipientSession.LastUsedDc, ex.Message), ex), exchangeErrorCategory.GetValueOrDefault(ExchangeErrorCategory.ServerTransient), null);
				}
				base.WriteVerbose(Strings.DatabaseLogonSuccessful(this.DataObject.ExchangeGuid, this.databaseLocationInfo.ServerFqdn, recipientSession.LastUsedDc));
				this.groupMailboxContext.SetExternalResources(this.FromSyncClient);
				this.groupMailboxContext.EnsureGroupIsInDirectoryCache("NewGroupMailbox.InternalProcessRecord");
			}
		}

		protected override void WriteResult(ADObject result)
		{
			ADUser dataObject = (ADUser)result;
			base.WriteResult(GroupMailbox.FromDataObject(dataObject));
		}

		protected override ITaskModuleFactory CreateTaskModuleFactory()
		{
			return new NewGroupMailboxTaskModuleFactory();
		}

		private void InitializeDependencies()
		{
			using (new StopwatchPerformanceTracker("NewGroupMailbox.InitializeDependencies.InitializeXmlSerializer", GenericCmdletInfoDataLogger.Instance))
			{
				UpdateGroupMailboxEwsBinding.InitializeXmlSerializer();
			}
		}

		private void WriteDebugInfo(string message, params object[] args)
		{
			base.WriteDebug(new LocalizedString(string.Format(message, args)));
		}

		private MailboxDatabaseWithLocationInfo FindDatabaseForProvisioning()
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 874, "FindDatabaseForProvisioning", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\GroupMailbox\\NewGroupMailbox.cs");
			this.stopWatch.Restart();
			Server server = topologyConfigurationSession.FindLocalServer();
			this.WriteDebugInfo("Found localServer. Time took:{0} sec", new object[]
			{
				this.stopWatch.Elapsed.TotalSeconds.ToString("n2")
			});
			if (server == null)
			{
				this.WriteError(new ObjectNotFoundException(new LocalizedString(Environment.MachineName), null), ExchangeErrorCategory.ServerTransient, null, true);
			}
			DatabaseAvailabilityGroup databaseAvailabilityGroup = null;
			this.stopWatch.Restart();
			if (ExEnvironment.IsSdfDomain)
			{
				databaseAvailabilityGroup = DagTaskHelper.ReadDagByName("NAMSR01DG050", topologyConfigurationSession);
				this.WriteDebugInfo("Found Dag {0} . Time took:{1} sec", new object[]
				{
					"NAMSR01DG050",
					this.stopWatch.Elapsed.TotalSeconds.ToString("n2")
				});
				if (databaseAvailabilityGroup == null)
				{
					this.WriteError(new GroupMailboxFailedToFindDagException(Strings.ErrorIncorrectInputDag("NAMSR01DG050")), ExchangeErrorCategory.ServerTransient, null, true);
				}
			}
			else if (server.DatabaseAvailabilityGroup != null)
			{
				databaseAvailabilityGroup = DagTaskHelper.ReadDag(server.DatabaseAvailabilityGroup, topologyConfigurationSession);
				this.WriteDebugInfo("Dag found:{0} for id={1}. Time took:{2} sec", new object[]
				{
					databaseAvailabilityGroup != null,
					server.DatabaseAvailabilityGroup,
					this.stopWatch.Elapsed.TotalSeconds.ToString("n2")
				});
			}
			else
			{
				this.WriteDebugInfo("The current server does not belong to a dag.", new object[0]);
			}
			List<MailboxDatabase> list;
			List<MailboxDatabase> list2;
			this.FetchDatabases(topologyConfigurationSession, databaseAvailabilityGroup, server, out list, out list2);
			Random random = new Random();
			while (list.Count > 0)
			{
				int index = random.Next(list.Count);
				MailboxDatabase mailboxDatabase = list[index];
				list.RemoveAt(index);
				DatabaseLocationInfo databaseLocationInfo = this.GetDatabaseLocationInfo(mailboxDatabase);
				if (databaseLocationInfo != null)
				{
					if (string.Equals(databaseLocationInfo.ServerFqdn, server.Fqdn, StringComparison.OrdinalIgnoreCase))
					{
						return new MailboxDatabaseWithLocationInfo(mailboxDatabase, databaseLocationInfo);
					}
					list2.Add(mailboxDatabase);
				}
			}
			this.WriteDebugInfo("Picking a remote database", new object[0]);
			while (list2.Count > 0)
			{
				int index2 = random.Next(list2.Count);
				MailboxDatabase mailboxDatabase2 = list2[index2];
				list2.RemoveAt(index2);
				DatabaseLocationInfo databaseLocationInfo2 = this.GetDatabaseLocationInfo(mailboxDatabase2);
				if (databaseLocationInfo2 != null)
				{
					return new MailboxDatabaseWithLocationInfo(mailboxDatabase2, databaseLocationInfo2);
				}
			}
			return null;
		}

		private void FetchDatabases(ITopologyConfigurationSession topologyConfigSession, DatabaseAvailabilityGroup dag, Server localServer, out List<MailboxDatabase> localDatabases, out List<MailboxDatabase> remoteDatabases)
		{
			QueryFilter queryFilter;
			if (this.IsWarmupCall())
			{
				dag = null;
				queryFilter = new ComparisonFilter(ComparisonOperator.Equal, DatabaseSchema.Server, localServer.Id);
			}
			else
			{
				queryFilter = new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, MailboxDatabaseSchema.IsExcludedFromProvisioning, false),
					new ComparisonFilter(ComparisonOperator.Equal, MailboxDatabaseSchema.IsExcludedFromInitialProvisioning, false),
					new ComparisonFilter(ComparisonOperator.Equal, MailboxDatabaseSchema.IsExcludedFromProvisioningBySpaceMonitoring, false)
				});
				if (dag != null)
				{
					queryFilter = new AndFilter(new QueryFilter[]
					{
						queryFilter,
						new ComparisonFilter(ComparisonOperator.Equal, DatabaseSchema.MasterServerOrAvailabilityGroup, dag.Id)
					});
				}
				else
				{
					queryFilter = new AndFilter(new QueryFilter[]
					{
						queryFilter,
						new ComparisonFilter(ComparisonOperator.Equal, DatabaseSchema.Server, localServer.Id)
					});
				}
			}
			this.stopWatch.Restart();
			ADPagedReader<MailboxDatabase> adpagedReader = topologyConfigSession.FindPaged<MailboxDatabase>(null, QueryScope.SubTree, queryFilter, null, 0);
			List<MailboxDatabase> list = new List<MailboxDatabase>(adpagedReader.ReadAllPages());
			this.WriteDebugInfo("Found {0} databases on {1}. Time took:{2} sec", new object[]
			{
				list.Count,
				(dag != null) ? dag.Name : localServer.Name,
				this.stopWatch.Elapsed.TotalSeconds.ToString("n2")
			});
			if (list.Count == 0)
			{
				dag = null;
				MailboxDatabase executingUserDatabase = this.GetExecutingUserDatabase(topologyConfigSession);
				if (executingUserDatabase != null)
				{
					list.Add(executingUserDatabase);
				}
				else
				{
					this.WriteError(new GroupMailboxFailedToFindDatabaseException(Strings.ErrorDatabaseUnavailableForProvisioning), ExchangeErrorCategory.ServerTransient, null, true);
				}
			}
			if (dag != null)
			{
				localDatabases = new List<MailboxDatabase>(10);
				remoteDatabases = new List<MailboxDatabase>(list.Count);
				foreach (MailboxDatabase mailboxDatabase in list)
				{
					if (localServer.Name.Equals(mailboxDatabase.ServerName, StringComparison.OrdinalIgnoreCase))
					{
						localDatabases.Add(mailboxDatabase);
					}
					else
					{
						remoteDatabases.Add(mailboxDatabase);
					}
				}
				if (localDatabases.Count == 0)
				{
					this.WriteDebugInfo("No local database found", new object[0]);
					return;
				}
			}
			else
			{
				localDatabases = list;
				remoteDatabases = new List<MailboxDatabase>(0);
			}
		}

		private MailboxDatabase GetExecutingUserDatabase(ITopologyConfigurationSession topologyConfigSession)
		{
			this.WriteDebugInfo("Executing user:{0}", new object[]
			{
				this.ExecutingUser
			});
			ADObjectId databaseId = null;
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.CurrentOrganizationId.OrganizationalUnit, base.CurrentOrganizationId, base.CurrentOrganizationId, true);
			IRecipientSession session = DirectorySessionFactory.Default.CreateTenantRecipientSession(true, ConsistencyMode.PartiallyConsistent, sessionSettings, 1098, "GetExecutingUserDatabase", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\GroupMailbox\\NewGroupMailbox.cs");
			Exception ex = GroupMailboxContext.ExecuteADOperationAndHandleException(delegate
			{
				ADUser aduser = (ADUser)this.GetDataObject<ADUser>(this.ExecutingUser, session, this.CurrentOrganizationId.OrganizationalUnit, null, new LocalizedString?(Strings.ErrorRecipientNotFound(this.ExecutingUser.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(this.ExecutingUser.ToString())), ExchangeErrorCategory.Client);
				databaseId = aduser.Database;
				this.WriteDebugInfo("Located local database for the executing user:{0}", new object[]
				{
					databaseId
				});
			});
			if (ex != null)
			{
				this.WriteDebugInfo("Unable to find database belonging to the executing user because:{0}", new object[]
				{
					ex
				});
			}
			if (databaseId != null)
			{
				return topologyConfigSession.Read<MailboxDatabase>(databaseId);
			}
			return null;
		}

		private DatabaseLocationInfo GetDatabaseLocationInfo(MailboxDatabase database)
		{
			this.WriteDebugInfo("Checking {0} database with ActiveManager.", new object[]
			{
				database.Name
			});
			try
			{
				return RecipientTaskHelper.GetActiveManagerInstance().GetServerForDatabase(database.Guid);
			}
			catch (ObjectNotFoundException ex)
			{
				this.WriteDebugInfo(ex.Message, new object[0]);
			}
			catch (ServerForDatabaseNotFoundException ex2)
			{
				this.WriteDebugInfo(ex2.Message, new object[0]);
			}
			return null;
		}

		private void GenerateUniqueNameIfRequired()
		{
			if (this.IsNameUnique(this.Name))
			{
				this.WriteDebugInfo("Name is unique: {0}", new object[]
				{
					this.Name
				});
				return;
			}
			int num = 1000;
			string text;
			for (;;)
			{
				string str = Guid.NewGuid().ToString().Substring(0, 6);
				text = this.Name + str;
				if (this.IsNameUnique(text))
				{
					break;
				}
				if (num-- <= 0)
				{
					goto Block_3;
				}
			}
			this.WriteDebugInfo("Generated unique name: old: {0}, new: {1}", new object[]
			{
				this.Name,
				text
			});
			this.Name = text;
			return;
			Block_3:
			this.WriteDebugInfo("Could not generate a unique name", new object[0]);
		}

		private bool IsNameUnique(string name)
		{
			ADScope scope = null;
			if (base.CurrentOrganizationId.OrganizationalUnit != null)
			{
				scope = new ADScope(base.CurrentOrganizationId.OrganizationalUnit, new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.OrganizationalUnitRoot, base.CurrentOrganizationId.OrganizationalUnit));
			}
			return RecipientTaskHelper.IsPropertyValueUnique(base.TenantGlobalCatalogSession, scope, null, new ADPropertyDefinition[]
			{
				ADObjectSchema.Name
			}, ADObjectSchema.Name, name, false, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, ExchangeErrorCategory.Client, false);
		}

		private bool IsWarmupCall()
		{
			if (base.CurrentTaskContext.InvocationInfo.IsCmdletInvokedWithoutPSFramework)
			{
				SwitchParameter? switchParameter = base.CurrentTaskContext.InvocationInfo.UserSpecifiedParameters["WhatIf"] as SwitchParameter?;
				return switchParameter != null && switchParameter == SwitchParameter.Present;
			}
			return false;
		}

		private const string DefaultLanguage = "en-US";

		private const string ParameterExecutingUser = "ExecutingUser";

		private const string ParameterMembers = "Members";

		private const string ParameterRequireSenderAuthenticationEnabled = "RequireSenderAuthenticationEnabled";

		private const string ParameterAutoSubscribeNewGroupMembers = "AutoSubscribeNewGroupMembers";

		private const string ParameterLanguage = "Language";

		private const string ParameterValidationOrganization = "ValidationOrganization";

		private const string ParameterRecipientIdType = "RecipientIdType";

		private const string ParameterFromSyncClient = "FromSyncClient";

		private GroupMailboxContext groupMailboxContext;

		private ModernGroupObjectType groupType = ModernGroupObjectType.Public;

		private MailboxDatabaseWithLocationInfo selectedLocalDatabase;

		private Stopwatch stopWatch = new Stopwatch();
	}
}
