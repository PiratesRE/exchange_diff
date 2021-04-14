using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security;
using System.Security.Principal;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Autodiscover;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ExchangeTopology;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Search;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal class DirectoryAccessor
	{
		private DirectoryAccessor()
		{
			try
			{
				this.sessionSelector = new DirectoryAccessor.SessionSelector();
				this.server = this.sessionSelector.TopologyConfigurationSession.FindServerByName(Environment.MachineName);
				if (this.server == null)
				{
					this.computer = this.sessionSelector.GcScopedConfigurationSession.FindLocalComputer();
				}
				else
				{
					this.sessionSelector.InitializeMonitoringTenants();
				}
			}
			catch (InvalidOperationException)
			{
				if (!FfoLocalEndpointManager.IsCentralAdminRoleInstalled)
				{
					throw;
				}
			}
			this.getRpcHttpVirtualDirectory = new DirectoryAccessor.GetRpcHttpVirtualDirectoryStrategy(this.GetLocalRpcHttpVirtualDirectories);
		}

		public static DirectoryAccessor Instance
		{
			get
			{
				if (DirectoryAccessor.instance == null)
				{
					lock (DirectoryAccessor.locker)
					{
						if (DirectoryAccessor.instance == null)
						{
							DirectoryAccessor.instance = new DirectoryAccessor();
						}
					}
				}
				return DirectoryAccessor.instance;
			}
		}

		public Server Server
		{
			get
			{
				return this.server;
			}
		}

		public ADComputer Computer
		{
			get
			{
				return this.computer;
			}
		}

		public AcceptedDomain DefaultMonitoringDomain
		{
			get
			{
				return this.sessionSelector.AcceptedDomain;
			}
		}

		public string MonitoringTenantPartitionId
		{
			get
			{
				if (this.sessionSelector.MonitoringTenantInfo != null)
				{
					return this.sessionSelector.MonitoringTenantInfo.MonitoringTenantPartitionId;
				}
				return null;
			}
		}

		public OrganizationId MonitoringTenantOrganizationId
		{
			get
			{
				if (this.sessionSelector.MonitoringTenantInfo != null)
				{
					return this.sessionSelector.MonitoringTenantInfo.MonitoringTenantOrganizationId;
				}
				return null;
			}
		}

		public bool CanMonitoringMailboxBeProvisioned
		{
			get
			{
				return !DirectoryAccessor.RunningInDatacenter || (this.sessionSelector.MonitoringTenantInfo != null && this.sessionSelector.MonitoringTenantInfo.MonitoringTenantReady);
			}
		}

		public string MonitoringTenantForestFqdn
		{
			get
			{
				if (this.sessionSelector.MonitoringTenantInfo != null)
				{
					return this.sessionSelector.MonitoringTenantInfo.MonitoringTenantForestFqdn;
				}
				return null;
			}
		}

		internal bool TracingCredentials
		{
			get
			{
				return Settings.TracingCredentials;
			}
		}

		public static SecurityIdentifier GetManagedAvailabilityServersUsgSid()
		{
			IRootOrganizationRecipientSession rootOrganizationRecipientSession = DirectorySessionFactory.Default.CreateRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 361, "GetManagedAvailabilityServersUsgSid", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs");
			return rootOrganizationRecipientSession.GetWellKnownExchangeGroupSid(WellKnownGuid.MaSWkGuid);
		}

		public bool IsServerCompatible(string serverName)
		{
			Server exchangeServerByName = this.GetExchangeServerByName(serverName);
			return exchangeServerByName != null && this.IsServerCompatible(exchangeServerByName);
		}

		public bool IsServerCompatible(Server target)
		{
			this.RefreshServerOrComputerObject();
			WTFDiagnostics.TraceDebug<string, string, string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Server {0} serial number is {1}, monitoring group is {2}.", target.Name, target.SerialNumber, target.MonitoringGroup, null, "IsServerCompatible", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 395);
			return string.Compare(target.MonitoringGroup, (this.server == null) ? this.computer.MonitoringGroup : this.server.MonitoringGroup, true) == 0;
		}

		public bool IsServerCompatible(ADComputer target)
		{
			this.RefreshServerOrComputerObject();
			WTFDiagnostics.TraceDebug<string, string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Server {0} monitoring group is {1}", target.Name, target.MonitoringGroup, null, "IsServerCompatible", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 410);
			return string.Compare(target.MonitoringGroup, (this.server == null) ? this.computer.MonitoringGroup : this.server.MonitoringGroup, true) == 0;
		}

		public Server GetExchangeServerByName(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				return null;
			}
			return this.sessionSelector.TopologyConfigurationSession.FindServerByName(this.FqdnToName(name));
		}

		public ADComputer GetNonExchangeServerByName(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				return null;
			}
			return this.sessionSelector.GcScopedConfigurationSession.FindComputerByHostName(this.FqdnToName(name));
		}

		public void LoadGlobalOverrides()
		{
			Container globalOverridesContainer = this.GetGlobalOverridesContainer();
			if (globalOverridesContainer == null)
			{
				return;
			}
			if (globalOverridesContainer.EncryptionKey0 != null && globalOverridesContainer.EncryptionKey0.Length == 16)
			{
				this.globalOverrideWaterMark = new Guid?(new Guid(globalOverridesContainer.EncryptionKey0));
			}
			ProbeDefinition.GlobalOverrides = this.LoadGlobalOverridesForType<ProbeDefinition>(globalOverridesContainer);
			MonitorDefinition.GlobalOverrides = this.LoadGlobalOverridesForType<MonitorDefinition>(globalOverridesContainer);
			ResponderDefinition.GlobalOverrides = this.LoadGlobalOverridesForType<ResponderDefinition>(globalOverridesContainer);
			MaintenanceDefinition.GlobalOverrides = this.LoadGlobalOverridesForType<MaintenanceDefinition>(globalOverridesContainer);
		}

		public bool IsGlobalOverridesChanged()
		{
			Container globalOverridesContainer = this.GetGlobalOverridesContainer();
			if (globalOverridesContainer != null && globalOverridesContainer.EncryptionKey0 != null && globalOverridesContainer.EncryptionKey0.Length == 16)
			{
				Guid guid = new Guid(globalOverridesContainer.EncryptionKey0);
				if (guid != this.globalOverrideWaterMark)
				{
					this.globalOverrideWaterMark = new Guid?(guid);
					return this.IsGlobalOverridesChangedForType<ProbeDefinition>(ProbeDefinition.GlobalOverrides, globalOverridesContainer) || this.IsGlobalOverridesChangedForType<MonitorDefinition>(MonitorDefinition.GlobalOverrides, globalOverridesContainer) || this.IsGlobalOverridesChangedForType<ResponderDefinition>(ResponderDefinition.GlobalOverrides, globalOverridesContainer) || this.IsGlobalOverridesChangedForType<MaintenanceDefinition>(MaintenanceDefinition.GlobalOverrides, globalOverridesContainer);
				}
			}
			return false;
		}

		public DatabaseCopy[] GetMailboxDatabaseCopies()
		{
			return this.GetMailboxDatabaseCopies(Environment.MachineName);
		}

		public DatabaseCopy[] GetMailboxDatabaseCopies(string serverName)
		{
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, serverName);
			return this.sessionSelector.TopologyConfigurationSession.Find<DatabaseCopy>(null, QueryScope.SubTree, filter, null, 0);
		}

		public MailboxDatabase GetMailboxDatabaseFromGuid(Guid mdbGuid)
		{
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Guid, mdbGuid);
			MailboxDatabase[] array = this.sessionSelector.TopologyConfigurationSession.Find<MailboxDatabase>(null, QueryScope.SubTree, filter, null, 0);
			if (array == null || array.Length == 0)
			{
				return null;
			}
			return array[0];
		}

		public MailboxDatabase GetMailboxDatabaseFromName(string databaseName)
		{
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, DatabaseSchema.Name, databaseName);
			MailboxDatabase[] array = this.sessionSelector.TopologyConfigurationSession.Find<MailboxDatabase>(null, QueryScope.SubTree, filter, null, 0);
			if (array == null || array.Length == 0)
			{
				return null;
			}
			return array[0];
		}

		public MailboxDatabase GetMailboxDatabaseFromCopy(DatabaseCopy copy)
		{
			return this.sessionSelector.TopologyConfigurationSession.Read<MailboxDatabase>(((ADObjectId)copy.Identity).Parent);
		}

		internal ADUser SearchMonitoringMailbox(string displayName, string userPrincipalName, ref MailboxDatabase database)
		{
			return this.SearchMonitoringMailboxInternal(displayName, userPrincipalName, ref database, this.sessionSelector.RecipientSession);
		}

		public IEnumerable<ADUser> SearchMonitoringMailboxesByDisplayName(string displayName)
		{
			if (string.IsNullOrWhiteSpace(displayName))
			{
				WTFDiagnostics.TraceError(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Invalid DisplayName", null, "SearchMonitoringMailboxesByDisplayName", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 616);
				throw new ArgumentException("Invalid DisplayName");
			}
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Searching for monitoring mailbox with DisplayName {0}", displayName, null, "SearchMonitoringMailboxesByDisplayName", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 621);
			QueryFilter queryFilter = this.CreateWildcardOrEqualFilter(ADRecipientSchema.DisplayName, displayName);
			QueryFilter queryFilter2 = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetailsValue, RecipientTypeDetails.MonitoringMailbox);
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				queryFilter,
				queryFilter2
			});
			return this.sessionSelector.RecipientSession.FindADUser(null, QueryScope.SubTree, filter, null, 1000);
		}

		internal QueryFilter CreateWildcardOrEqualFilter(ADPropertyDefinition schemaProperty, string identityString)
		{
			if (string.IsNullOrEmpty(identityString))
			{
				throw new ArgumentException("identityString");
			}
			string text;
			MatchOptions matchOptions;
			if (identityString.StartsWith("*") && identityString.EndsWith("*"))
			{
				if (identityString.Length <= 2)
				{
					return null;
				}
				text = identityString.Substring(1, identityString.Length - 2);
				matchOptions = MatchOptions.SubString;
			}
			else if (identityString.EndsWith("*"))
			{
				text = identityString.Substring(0, identityString.Length - 1);
				matchOptions = MatchOptions.Prefix;
			}
			else
			{
				if (!identityString.StartsWith("*"))
				{
					return new ComparisonFilter(ComparisonOperator.Equal, schemaProperty, identityString);
				}
				text = identityString.Substring(1);
				matchOptions = MatchOptions.Suffix;
			}
			return new TextFilter(schemaProperty, text, matchOptions, MatchFlags.IgnoreCase);
		}

		internal ADUser StampProvisioningConstraint(string userPrincipalName)
		{
			MailboxDatabase mailboxDatabase = null;
			ADUser aduser = this.SearchMonitoringMailboxInternal(null, userPrincipalName, ref mailboxDatabase, this.sessionSelector.WritableRecipientSession);
			if (aduser == null)
			{
				return null;
			}
			if (VariantConfiguration.InvariantNoFlightingSnapshot.ActiveMonitoring.PinMonitoringMailboxesToDatabases.Enabled && (aduser.MailboxProvisioningConstraint == null || string.IsNullOrWhiteSpace(aduser.MailboxProvisioningConstraint.Value)))
			{
				string text = string.Format("{{DatabaseName -eq '{0}'}}", mailboxDatabase.Name);
				WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Stamping mailbox '{0}' with provisioning constraint '{1}'", aduser.Name, text, null, "StampProvisioningConstraint", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 714);
				aduser.MailboxProvisioningConstraint = new MailboxProvisioningConstraint(text);
				this.sessionSelector.WritableRecipientSession.Save(aduser);
				aduser = (this.SearchMailbox(aduser.Name, this.sessionSelector.WritableRecipientSession, aduser.Id.Parent) as ADUser);
			}
			return aduser;
		}

		internal void DeleteMonitoringMailbox(ADUser monitoringMailbox)
		{
			if (monitoringMailbox == null)
			{
				throw new ArgumentNullException("monitoringMailbox");
			}
			if (monitoringMailbox.RecipientTypeDetails != RecipientTypeDetails.MonitoringMailbox)
			{
				throw new InvalidOperationException("Mailbox needs to be monitoring mailbox");
			}
			ADSessionSettings sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
			IConfigurationSession configurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 749, "DeleteMonitoringMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs");
			ADObjectId orgContainerId = configurationSession.GetOrgContainerId();
			ADOrganizationConfig adorganizationConfig = configurationSession.Read<ADOrganizationConfig>(orgContainerId);
			OrganizationId organizationId = adorganizationConfig.OrganizationId;
			AcceptedDomain defaultAcceptedDomain = configurationSession.GetDefaultAcceptedDomain();
			if (defaultAcceptedDomain == null || defaultAcceptedDomain.DomainName == null || defaultAcceptedDomain.DomainName.Domain == null)
			{
				WTFDiagnostics.TraceWarning(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Can't delete monitoring mailbox because can't find the default accepted domain for monitoring mailboxes", null, "DeleteMonitoringMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 761);
				return;
			}
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Create the cmdlet to delete mailbox {0}", monitoringMailbox.UserPrincipalName, null, "DeleteMonitoringMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 765);
			using (PSLocalTask<RemoveMailbox, Mailbox> pslocalTask = CmdletTaskFactory.Instance.CreateRemoveMailboxTask(organizationId, new SmtpAddress("admin@" + defaultAcceptedDomain)))
			{
				pslocalTask.CaptureAdditionalIO = true;
				pslocalTask.Task.Permanent = true;
				pslocalTask.Task.Force = true;
				pslocalTask.Task.Identity = new GeneralMailboxIdParameter(monitoringMailbox);
				pslocalTask.Task.Execute();
				if (pslocalTask.Error != null)
				{
					throw new Exception(pslocalTask.ErrorMessage);
				}
			}
		}

		internal ADUser CreateMonitoringMailbox(string displayName, MailboxDatabase database, out string password)
		{
			password = null;
			if (!this.CanMonitoringMailboxBeProvisioned)
			{
				WTFDiagnostics.TraceWarning(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Can't create monitoring mailbox because monitoring tenant does not exist.", null, "CreateMonitoringMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 804);
				return null;
			}
			WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Creating monitoring mailbox {0} on database {1}", displayName, database.Name, null, "CreateMonitoringMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 808);
			ADSessionSettings sessionSettings;
			IConfigurationSession configurationSession;
			IRecipientSession recipientSession;
			if (DirectoryAccessor.RunningInMultiTenantEnvironment)
			{
				string text;
				if (this.sessionSelector.MonitoringTenantInfo == null)
				{
					text = null;
				}
				else
				{
					text = this.sessionSelector.MonitoringTenantInfo.MonitoringTenantName;
				}
				if (string.IsNullOrWhiteSpace(text))
				{
					text = MailboxTaskHelper.GetMonitoringTenantName("E15");
				}
				try
				{
					sessionSettings = ADSessionSettings.FromTenantCUName(text);
					configurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 842, "CreateMonitoringMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs");
					recipientSession = DirectorySessionFactory.Default.CreateTenantRecipientSession(false, ConsistencyMode.IgnoreInvalid, sessionSettings, 843, "CreateMonitoringMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs");
				}
				catch (CannotResolveTenantNameException)
				{
					WTFDiagnostics.TraceWarning<string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Can't create monitoring mailbox because monitoring tenant '{0}' does not exist.", text, null, "CreateMonitoringMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 848);
					return null;
				}
				ExchangeConfigurationUnit[] array = configurationSession.Find<ExchangeConfigurationUnit>(null, QueryScope.SubTree, null, null, 0);
				if (array == null || array.Length == 0)
				{
					WTFDiagnostics.TraceWarning<string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Can't create monitoring mailbox because monitoring tenant '{0}' does not exist.", text, null, "CreateMonitoringMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 857);
					return null;
				}
				ExchangeConfigurationUnit exchangeConfigurationUnit = array[0];
				if (exchangeConfigurationUnit.OrganizationStatus != OrganizationStatus.Active)
				{
					WTFDiagnostics.TraceWarning<string, string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Can't create monitoring mailbox because monitoring tenant '{0}' is not active. Its status is '{1}'.", text, exchangeConfigurationUnit.OrganizationStatus.ToString(), null, "CreateMonitoringMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 865);
					return null;
				}
				goto IL_1D5;
			}
			sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
			configurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 873, "CreateMonitoringMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs");
			recipientSession = DirectorySessionFactory.Default.CreateRootOrgRecipientSession(false, ConsistencyMode.IgnoreInvalid, sessionSettings, 874, "CreateMonitoringMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs");
			IL_1D5:
			AcceptedDomain defaultAcceptedDomain = configurationSession.GetDefaultAcceptedDomain();
			if (defaultAcceptedDomain == null || defaultAcceptedDomain.DomainName == null || defaultAcceptedDomain.DomainName.Domain == null)
			{
				throw new Exception("Can't find the default accepted domain for monitoring mailboxes");
			}
			ADObjectId orgContainerId = configurationSession.GetOrgContainerId();
			ADOrganizationConfig adorganizationConfig = configurationSession.Read<ADOrganizationConfig>(orgContainerId);
			OrganizationId organizationId = adorganizationConfig.OrganizationId;
			ADObjectId adobjectId = null;
			if (organizationId != OrganizationId.ForestWideOrgId)
			{
				adobjectId = organizationId.OrganizationalUnit;
			}
			else
			{
				bool useConfigNC = configurationSession.UseConfigNC;
				bool useGlobalCatalog = configurationSession.UseGlobalCatalog;
				ADComputer adcomputer;
				try
				{
					configurationSession.UseConfigNC = false;
					configurationSession.UseGlobalCatalog = true;
					Server server = database.GetServer();
					adcomputer = ((ITopologyConfigurationSession)configurationSession).FindComputerByHostName(server.Name);
				}
				finally
				{
					configurationSession.UseConfigNC = useConfigNC;
					configurationSession.UseGlobalCatalog = useGlobalCatalog;
				}
				if (adcomputer == null)
				{
					throw new Exception(string.Format("The Exchange server for the database '{0}' wasn't found in Active Directory Domain Services. The object may be corrupted.", database.Name));
				}
				ADObjectId adobjectId2 = adcomputer.Id.DomainId;
				adobjectId2 = adobjectId2.GetChildId("Microsoft Exchange System Objects");
				adobjectId = adobjectId2.GetChildId("Monitoring Mailboxes");
			}
			string text2 = string.Format("HealthMailbox{0}", Guid.NewGuid().ToString("N"));
			string str = defaultAcceptedDomain.DomainName.Domain.ToString();
			string text3 = text2 + "@" + str;
			password = LocalMonitoringMailboxManagement.GetStaticPassword(this.traceContext);
			if (string.IsNullOrEmpty(password))
			{
				if (DirectoryAccessor.RunningInDatacenter)
				{
					password = PasswordHelper.GetRandomPassword(text2, text3, 16);
				}
				else
				{
					password = PasswordHelper.GetRandomPassword(displayName, text2, 128);
				}
			}
			using (PSLocalTask<NewMailbox, Mailbox> pslocalTask = CmdletTaskFactory.Instance.CreateNewMonitoringMailboxTask(organizationId, new SmtpAddress("admin@" + defaultAcceptedDomain)))
			{
				pslocalTask.CaptureAdditionalIO = true;
				pslocalTask.Task.Name = text2;
				pslocalTask.Task.Alias = text2;
				pslocalTask.Task.DisplayName = displayName;
				pslocalTask.Task.Database = new DatabaseIdParameter(database);
				pslocalTask.Task.ResetPasswordOnNextLogon = false;
				pslocalTask.Task.SendModerationNotifications = TransportModerationNotificationFlags.Never;
				pslocalTask.Task.Archive = true;
				pslocalTask.Task.OrganizationalUnit = new OrganizationalUnitIdParameter(adobjectId);
				if (DirectoryAccessor.ShouldStampProvisioningConstraint)
				{
					pslocalTask.Task.MailboxProvisioningConstraint = new MailboxProvisioningConstraint(string.Format("{{DatabaseName -eq '{0}'}}", database.Name));
				}
				if (DirectoryAccessor.RunningInDatacenter)
				{
					pslocalTask.Task.WindowsLiveID = new WindowsLiveId(text3);
				}
				else
				{
					pslocalTask.Task.UserPrincipalName = text3;
				}
				if (DirectoryAccessor.RunningInMultiTenantEnvironment)
				{
					pslocalTask.Task.Organization = new OrganizationIdParameter(organizationId);
				}
				using (SecureString secureString = password.ConvertToSecureString())
				{
					pslocalTask.Task.Password = secureString;
					pslocalTask.Task.Execute();
				}
				if (pslocalTask.Error != null)
				{
					throw new Exception(pslocalTask.ErrorMessage);
				}
				Mailbox result = pslocalTask.Result;
				string originatingServer = result.OriginatingServer;
				WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Mailbox '{0}' created using Domain Controller '{1}'", text3, originatingServer, null, "CreateMonitoringMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1014);
				recipientSession.LinkResolutionServer = originatingServer;
				recipientSession.DomainController = originatingServer;
				recipientSession.UseGlobalCatalog = false;
			}
			Thread.Sleep(TimeSpan.FromSeconds(10.0));
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, text2);
			ADUser[] array2 = recipientSession.FindADUser(adobjectId, QueryScope.SubTree, filter, null, 1);
			if (array2 != null && array2.Length > 0)
			{
				ADUser aduser = array2[0];
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Mailbox '{0}' found in AD", text3, null, "CreateMonitoringMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1032);
				if (DirectoryAccessor.RunningInDatacenter)
				{
					bool useBecAPIsforLiveId = ProvisioningTasksConfigImpl.UseBecAPIsforLiveId;
					if (useBecAPIsforLiveId)
					{
						if (aduser.ExternalDirectoryObjectId == null || string.IsNullOrWhiteSpace(aduser.ExternalDirectoryObjectId.ToString()))
						{
							WTFDiagnostics.TraceError<string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Mailbox '{0}' failed to return valid ExternalDirectoryObjectId - abandoning it.", text3, null, "CreateMonitoringMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1042);
							return null;
						}
						WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Mailbox '{0}' has a valid BEC ExternalDirectoryObjectId", text3, null, "CreateMonitoringMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1047);
					}
					else
					{
						WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "BEC is not enabled; skipping ExternalDirectoryObjectId validation for Mailbox '{0}'", text3, null, "CreateMonitoringMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1052);
					}
				}
				else
				{
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Enterprise SKU; skipping ExternalDirectoryObjectId validation for Mailbox '{0}'", text3, null, "CreateMonitoringMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1057);
				}
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Attempting to create Search probe message in Mailbox '{0}'", text3, null, "CreateMonitoringMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1061);
				try
				{
					string smtpAddress = text3;
					using (MailboxSession mailboxSession = SearchStoreHelper.GetMailboxSession(smtpAddress, true, "Monitoring"))
					{
						using (Folder inboxFolder = SearchStoreHelper.GetInboxFolder(mailboxSession))
						{
							SearchStoreHelper.CreateMessage(mailboxSession, inboxFolder, "SearchQueryStxProbe");
						}
					}
				}
				catch (Exception arg)
				{
					WTFDiagnostics.TraceWarning<Exception>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Exception is caught trying to create Search message in monitoring mailbox: {0}", arg, null, "CreateMonitoringMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1075);
				}
				return aduser;
			}
			WTFDiagnostics.TraceError<string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Mailbox '{0}' could not be found in AD even though New-Mailbox returned successfully.", text3, null, "CreateMonitoringMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1082);
			return null;
		}

		public ADUser SearchOrCreateMonitoringMailbox(bool canUpdate, ref MailboxDatabase database, Guid guid, string userPrincipalName = null)
		{
			string monitoringMailboxName = ADUser.GetMonitoringMailboxName(guid);
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Searching monitoring mailbox {0}", monitoringMailboxName, null, "SearchOrCreateMonitoringMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1107);
			ADUser aduser = this.SearchMonitoringMailboxInternal(monitoringMailboxName, userPrincipalName, ref database, this.sessionSelector.RecipientSession);
			if (canUpdate && aduser == null)
			{
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Monitoring mailbox {0} not found, try creating one", monitoringMailboxName, null, "SearchOrCreateMonitoringMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1117);
				if (database == null)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Mailbox database not specified, try pick one", null, "SearchOrCreateMonitoringMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1123);
					MailboxDatabase[] candidateMailboxDatabases = this.GetCandidateMailboxDatabases(1);
					if (candidateMailboxDatabases != null && candidateMailboxDatabases.Length > 0)
					{
						database = candidateMailboxDatabases[0];
					}
				}
				if (database != null)
				{
					WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Creating monitoring mailbox on database {0}", database.Id.ToString(), null, "SearchOrCreateMonitoringMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1133);
					string text = null;
					aduser = this.CreateMonitoringMailbox(ADUser.GetMonitoringMailboxName(guid), database, out text);
				}
				else
				{
					WTFDiagnostics.TraceDebug(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Monitoring mailbox cannot be created because no mailbox database available", null, "SearchOrCreateMonitoringMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1140);
				}
			}
			return aduser;
		}

		internal ADRecipient SearchMailbox(string account, IRecipientSession session, ADObjectId root = null)
		{
			ADRecipient adrecipient = DirectoryAccessor.SearchForRecipient(new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, account), session, root);
			if (adrecipient != null)
			{
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Mailbox {0} found", account, null, "SearchMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1160);
			}
			return adrecipient;
		}

		public Guid GetSystemMailboxGuid(Guid databaseGuid)
		{
			string text = string.Format(CultureInfo.InvariantCulture, "SystemMailbox{{{0}}}", new object[]
			{
				databaseGuid.ToString()
			});
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Searching for System mailbox {0}", text, null, "GetSystemMailboxGuid", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1178);
			ADRecipient adrecipient = this.SearchMailbox(text, this.sessionSelector.RootorgRecipientSession, null);
			if (adrecipient == null || !(adrecipient is ADSystemMailbox))
			{
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "System mailbox {0} not found", text, null, "GetSystemMailboxGuid", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1187);
				return Guid.Empty;
			}
			return ((ADSystemMailbox)adrecipient).ExchangeGuid;
		}

		public string GeneratePasswordForMailbox(ADUser mailbox)
		{
			string text = LocalMonitoringMailboxManagement.GetStaticPassword(this.traceContext);
			if (string.IsNullOrWhiteSpace(text))
			{
				if (!VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).ActiveMonitoring.DirectoryAccessor.Enabled)
				{
					text = PasswordHelper.GetRandomPassword(mailbox.DisplayName, mailbox.SamAccountName, 128);
				}
				else
				{
					text = PasswordHelper.GetRandomPassword(mailbox.Name, mailbox.UserPrincipalName, 16);
				}
			}
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "GeneratePasswordForMailbox {0} password", text, null, "GeneratePasswordForMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1215);
			return text;
		}

		private void ResetPasswordForMailbox(IRecipientSession recipientSession, IConfigurationSession configurationSession, ADUser mailbox, string password)
		{
			MailboxIdParameter identity = new MailboxIdParameter(mailbox.ObjectId);
			ADObjectId orgContainerId = configurationSession.GetOrgContainerId();
			ADOrganizationConfig adorganizationConfig = configurationSession.Read<ADOrganizationConfig>(orgContainerId);
			OrganizationId organizationId = adorganizationConfig.OrganizationId;
			using (PSLocalTask<SetMailbox, object> pslocalTask = CmdletTaskFactory.Instance.CreateSetMailboxTask(organizationId))
			{
				pslocalTask.CaptureAdditionalIO = true;
				pslocalTask.Task.Identity = identity;
				using (SecureString secureString = password.ConvertToSecureString())
				{
					pslocalTask.Task.Password = secureString;
					pslocalTask.Task.Execute();
				}
				if (pslocalTask.Error != null)
				{
					string errorMessage = pslocalTask.ErrorMessage;
					WTFDiagnostics.TraceError<string, string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Failed to change password for mailbox '{0}'. Error message: {1}", mailbox.UserPrincipalName, errorMessage, null, "ResetPasswordForMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1260);
					throw new Exception(errorMessage);
				}
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Successfully changed password for mailbox '{0}'", mailbox.UserPrincipalName, null, "ResetPasswordForMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1264);
			}
		}

		public string GetMonitoringMailboxCredential(ADUser monitoringMailbox)
		{
			WTFDiagnostics.TraceFunction<ADUser>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Retrieving monitoring mailbox {0} password", monitoringMailbox, null, "GetMonitoringMailboxCredential", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1280);
			string text = this.GeneratePasswordForMailbox(monitoringMailbox);
			if (!DirectoryAccessor.RunningInDatacenter)
			{
				this.ResetPasswordForMailbox(this.sessionSelector.WritableRecipientSession, this.sessionSelector.TopologyConfigurationSession, monitoringMailbox, text);
			}
			else
			{
				this.ResetPasswordForMailbox(this.sessionSelector.WritableRecipientSession, this.sessionSelector.MonitoringTenantInfo.TenantConfigurationSession, monitoringMailbox, text);
			}
			if (this.TracingCredentials)
			{
				WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Password for monitoring mailbox {0} is {1}", monitoringMailbox.UserPrincipalName, text, null, "GetMonitoringMailboxCredential", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1299);
			}
			return text;
		}

		public string GetServerFqdnForDatabase(Guid dbGuid)
		{
			DatabaseLocationInfo serverForDatabase = this.activeManager.Value.GetServerForDatabase(dbGuid);
			if (serverForDatabase != null)
			{
				return serverForDatabase.ServerFqdn;
			}
			return null;
		}

		public bool IsDatabaseCopyActiveOnLocalServer(Guid databaseGuid)
		{
			return this.IsDatabaseCopyActiveOnLocalServer(databaseGuid, null);
		}

		public bool IsDatabaseCopyActiveOnLocalServer(MailboxDatabase database)
		{
			return this.IsDatabaseCopyActiveOnLocalServer(database.Guid, database);
		}

		public string GetDatabaseActiveHost(MailboxDatabase database)
		{
			if (database == null || database.Server == null)
			{
				return null;
			}
			return database.Server.Name;
		}

		public string GetDatabaseActiveHost(Guid mdbGuid)
		{
			MailboxDatabase mailboxDatabaseFromGuid = this.GetMailboxDatabaseFromGuid(mdbGuid);
			return this.GetDatabaseActiveHost(mailboxDatabaseFromGuid);
		}

		public Server[] GetCandidateCafeServers(int max)
		{
			WTFDiagnostics.TraceFunction<int>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "DirectoryAccessor.GetCandidateCafeServers: max count is {0}", max, null, "GetCandidateCafeServers", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1375);
			QueryFilter compatibilityFilter;
			if (string.IsNullOrEmpty(this.server.MonitoringGroup))
			{
				compatibilityFilter = new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.IsCafeServer, true),
					new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ServerSchema.SerialNumber, DirectoryAccessor.versionCutoff.ToString(true)),
					new NotFilter(new ExistsFilter(ServerSchema.MonitoringGroup))
				});
			}
			else
			{
				compatibilityFilter = new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.IsCafeServer, true),
					new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ServerSchema.SerialNumber, DirectoryAccessor.versionCutoff.ToString(true)),
					new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.MonitoringGroup, this.server.MonitoringGroup)
				});
			}
			return this.GetCandidates<Server>(compatibilityFilter, max);
		}

		public List<string> GetCandidateObservers()
		{
			string text = (this.server != null) ? this.server.MonitoringGroup : this.computer.MonitoringGroup;
			QueryFilter queryFilter;
			if (string.IsNullOrEmpty(text))
			{
				queryFilter = new NotFilter(new ExistsFilter(ADComputerSchema.MonitoringGroup));
			}
			else
			{
				queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ADComputerSchema.MonitoringGroup, text);
			}
			queryFilter = new AndFilter(new QueryFilter[]
			{
				queryFilter,
				new ComparisonFilter(ComparisonOperator.Equal, ADComputerSchema.MonitoringInstalled, (this.server != null) ? 1 : 2)
			});
			ADPagedReader<ADComputer> adpagedReader = this.sessionSelector.GcScopedConfigurationSession.FindPaged<ADComputer>(null, QueryScope.SubTree, queryFilter, null, 0);
			ADComputer[] array = adpagedReader.ReadAllPages();
			if (array == null || array.Length == 0)
			{
				return new List<string>();
			}
			return (from svr in array
			select svr.DnsHostName).ToList<string>();
		}

		public bool IsMonitoringOffline(string serverName)
		{
			Server exchangeServerByName = this.GetExchangeServerByName(serverName);
			if (exchangeServerByName != null)
			{
				return this.IsMonitoringOffline(exchangeServerByName);
			}
			ADComputer nonExchangeServerByName = this.GetNonExchangeServerByName(serverName);
			if (nonExchangeServerByName != null)
			{
				return this.IsMonitoringOffline(nonExchangeServerByName);
			}
			throw new ServerNotFoundException("Invalid server name; No Exchange server AD object or computer object found", serverName);
		}

		public bool IsMonitoringOffline(Server server)
		{
			return !ServerComponentStates.IsRemoteComponentOnlineAccordingToAD(server, ServerComponentEnum.ServerWideOffline) || !ServerComponentStates.IsRemoteComponentOnlineAccordingToAD(server, ServerComponentEnum.Monitoring);
		}

		public bool IsMonitoringOffline(ADComputer server)
		{
			return !ServerComponentStates.IsRemoteComponentOnlineAccordingToAD(server, ServerComponentEnum.ServerWideOffline) || !ServerComponentStates.IsRemoteComponentOnlineAccordingToAD(server, ServerComponentEnum.Monitoring);
		}

		public bool IsRecoveryActionsEnabledOffline(string serverName)
		{
			return !this.IsServerComponentOnline(serverName, ServerComponentEnum.RecoveryActionsEnabled);
		}

		internal bool IsServerComponentOnline(string serverName, ServerComponentEnum component)
		{
			Server exchangeServerByName = this.GetExchangeServerByName(serverName);
			if (exchangeServerByName != null)
			{
				return ServerComponentStates.IsRemoteComponentOnlineAccordingToAD(exchangeServerByName, component);
			}
			ADComputer nonExchangeServerByName = this.GetNonExchangeServerByName(serverName);
			if (nonExchangeServerByName != null)
			{
				return ServerComponentStates.IsRemoteComponentOnlineAccordingToAD(nonExchangeServerByName, component);
			}
			throw new ArgumentException("Invalid server name; No Exchange server AD object or computer object found");
		}

		public IEnumerable<ADOwaVirtualDirectory> GetLocalOwaVirtualDirectories()
		{
			if (this.Server == null)
			{
				return null;
			}
			return this.sessionSelector.TopologyConfigurationSession.Find<ADOwaVirtualDirectory>((ADObjectId)this.Server.Identity, QueryScope.SubTree, null, null, 0);
		}

		public Guid[] GetAllOfflineAddressBookGuids()
		{
			IConfigurationSession configurationSession = (this.sessionSelector.MonitoringTenantInfo != null) ? this.sessionSelector.MonitoringTenantInfo.TenantConfigurationSession : this.sessionSelector.TopologyConfigurationSession;
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ADObjectSchema.ExchangeVersion, ExchangeObjectVersion.Exchange2012);
			OfflineAddressBook[] array = configurationSession.Find<OfflineAddressBook>(null, QueryScope.SubTree, filter, null, 0);
			if (array == null)
			{
				return new Guid[0];
			}
			return (from n in array
			select n.Guid into n
			orderby n
			select n).ToArray<Guid>();
		}

		public List<ADUser> GetAllOrganizationMailboxes()
		{
			return OrganizationMailbox.GetOrganizationMailboxesByCapability(this.sessionSelector.RecipientSession, OrganizationCapability.OABGen);
		}

		public Guid[] GetAllDatabaseGuidsForOrganizationMailboxes()
		{
			List<ADUser> allOrganizationMailboxes = this.GetAllOrganizationMailboxes();
			return (from x in allOrganizationMailboxes.ConvertAll<Guid>((ADUser n) => n.Database.ObjectGuid).Distinct<Guid>()
			orderby x
			select x).ToArray<Guid>();
		}

		public Container GetGlobalOverridesContainer()
		{
			if (this.sessionSelector.TopologyConfigurationSession == null)
			{
				return null;
			}
			ADObjectId rootOrgId = this.sessionSelector.TopologyConfigurationSession.SessionSettings.RootOrgId;
			ADObjectId descendantId = rootOrgId.GetDescendantId(MonitoringOverride.RdnContainer);
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, MonitoringOverride.ContainerName);
			Container[] array = this.sessionSelector.TopologyConfigurationSession.Find<Container>(descendantId, QueryScope.SubTree, filter, null, 0);
			if (array == null || array.Length == 0)
			{
				return null;
			}
			return array[0];
		}

		internal MailboxDatabase[] GetCandidateMailboxDatabases(int max)
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "DirectoryAccessor.GetCandidateMailboxDatabases called", null, "GetCandidateMailboxDatabases", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1637);
			DatabaseAvailabilityGroup[] candidates = this.GetCandidates<DatabaseAvailabilityGroup>(null, max);
			int num = 0;
			List<MailboxDatabase> list = new List<MailboxDatabase>();
			for (int i = 0; i < max; i++)
			{
				DatabaseAvailabilityGroup databaseAvailabilityGroup = null;
				if (candidates != null && candidates.Length > 0)
				{
					databaseAvailabilityGroup = candidates[num];
					num = (num + 1) % candidates.Length;
				}
				List<QueryFilter> list2 = new List<QueryFilter>();
				list2.Add(new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.IsMailboxServer, true));
				list2.Add(new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ServerSchema.SerialNumber, DirectoryAccessor.versionCutoff.ToString(true)));
				if (databaseAvailabilityGroup != null)
				{
					list2.Add(new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.DatabaseAvailabilityGroup, (ADObjectId)databaseAvailabilityGroup.Identity));
				}
				if (string.IsNullOrEmpty(this.server.MonitoringGroup))
				{
					list2.Add(new NotFilter(new ExistsFilter(ServerSchema.MonitoringGroup)));
				}
				else
				{
					list2.Add(new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.MonitoringGroup, this.server.MonitoringGroup));
				}
				QueryFilter compatibilityFilter = new AndFilter(list2.ToArray());
				Server[] candidates2 = this.GetCandidates<Server>(compatibilityFilter, 10);
				if (candidates2 != null && candidates2.Length != 0)
				{
					MailboxDatabase mailboxDatabase = null;
					int num2 = this.random.Next(candidates2.Length);
					int num3 = 0;
					do
					{
						int num4 = (num2 + num3) % candidates2.Length;
						Server server = candidates2[num4];
						WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "DirectoryAccessor.GetCandidateMailboxDatabase: try find from server {0}", server.Name, null, "GetCandidateMailboxDatabases", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1705);
						if (this.IsMonitoringOffline(server))
						{
							WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "DirectoryAccessor.GetCandidateMailboxDatabase: ignore server {0} because monitoring state is not active", server.Name, null, "GetCandidateMailboxDatabases", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1710);
						}
						else
						{
							DatabaseCopy[] mailboxDatabaseCopies = this.GetMailboxDatabaseCopies(server.Name);
							if (mailboxDatabaseCopies != null && mailboxDatabaseCopies.Length != 0)
							{
								int num5 = this.random.Next(mailboxDatabaseCopies.Length);
								int num6 = 0;
								do
								{
									int num7 = (num5 + num6) % mailboxDatabaseCopies.Length;
									DatabaseCopy copy = mailboxDatabaseCopies[num7];
									MailboxDatabase mdb = this.GetMailboxDatabaseFromCopy(copy);
									if (!list.Exists((MailboxDatabase element) => element.Guid == mdb.Guid) && mdb != null && !DatabaseTasksHelper.IsMailboxDatabaseExcludedFromMonitoring(mdb) && mdb.Server.ObjectGuid == server.Guid)
									{
										goto Block_12;
									}
								}
								while (++num6 < mailboxDatabaseCopies.Length);
								IL_2A6:
								if (mailboxDatabase == null)
								{
									goto IL_2AA;
								}
								break;
								Block_12:
								mailboxDatabase = CS$<>8__locals1.mdb;
								goto IL_2A6;
							}
							WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "DirectoryAccessor.GetCandidateMailboxDatabase: ignore server {0} because no database copy is found there", server.Name, null, "GetCandidateMailboxDatabases", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1718);
						}
						IL_2AA:;
					}
					while (++num3 < candidates2.Length);
					if (mailboxDatabase != null)
					{
						list.Add(mailboxDatabase);
					}
				}
			}
			return list.ToArray();
		}

		private ADUser SearchMonitoringMailboxInternal(string displayName, string userPrincipalName, ref MailboxDatabase database, IRecipientSession session)
		{
			if (session == null)
			{
				WTFDiagnostics.TraceError(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "SearchMonitoringMailboxInternal: invalid IRecipientSession", null, "SearchMonitoringMailboxInternal", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1794);
				throw new ArgumentException("SearchMonitoringMailboxInternal: invalid IRecipientSession");
			}
			if (userPrincipalName == null && displayName == null)
			{
				WTFDiagnostics.TraceError(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "SearchMonitoringMailboxInternal: invalid DisplayName and UPN", null, "SearchMonitoringMailboxInternal", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1800);
				throw new ArgumentException("SearchMonitoringMailboxInternal: invalid DisplayName and UPN");
			}
			if (!this.CanMonitoringMailboxBeProvisioned)
			{
				string domainName = IPGlobalProperties.GetIPGlobalProperties().DomainName;
				string message;
				if (domainName.Contains("extest.microsoft.com"))
				{
					message = "Can't find the Monitoring tenant for this test forest. Please examine your TDS deployment logs (especially the 'Datacenter PostReqs' step) to find out what went wrong.";
				}
				else
				{
					message = "Can't find the Monitoring tenant for this forest. Please page the Monitoring On Call Engineer, as this forest should never have gone live with this issue.";
				}
				WTFDiagnostics.TraceError(ExTraceGlobals.CommonComponentsTracer, this.traceContext, message, null, "SearchMonitoringMailboxInternal", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1818);
				throw new Exception(message);
			}
			QueryFilter queryFilter;
			if (string.IsNullOrWhiteSpace(userPrincipalName))
			{
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Searching for monitoring mailbox with DisplayName {0}", displayName, null, "SearchMonitoringMailboxInternal", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1826);
				queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.DisplayName, displayName);
			}
			else
			{
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Searching for monitoring mailbox with UPN {0}", userPrincipalName, null, "SearchMonitoringMailboxInternal", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1831);
				queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ADUserSchema.UserPrincipalName, userPrincipalName);
			}
			QueryFilter queryFilter2 = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetailsValue, RecipientTypeDetails.MonitoringMailbox);
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				queryFilter,
				queryFilter2
			});
			ADUser[] array = session.FindADUser(null, QueryScope.SubTree, filter, null, 1000);
			if (array != null && array.Length > 0)
			{
				WTFDiagnostics.TraceInformation<int, string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Found {0} monitorng mailboxes with display name {1}", array.Length, array[0].DisplayName, null, "SearchMonitoringMailboxInternal", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1853);
				foreach (ADUser aduser in array)
				{
					if (this.CanMonitoringMailboxBeUsed(aduser, ref database))
					{
						return aduser;
					}
				}
			}
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Unable to find any usable monitorng mailboxes with name {0}", (!string.IsNullOrWhiteSpace(userPrincipalName)) ? userPrincipalName : displayName, null, "SearchMonitoringMailboxInternal", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1864);
			return null;
		}

		private bool CanMonitoringMailboxBeUsed(ADUser monitoringMailbox, ref MailboxDatabase presumedDatabase)
		{
			if (DirectoryAccessor.RunningInDatacenter)
			{
				bool useBecAPIsforLiveId = ProvisioningTasksConfigImpl.UseBecAPIsforLiveId;
				if (useBecAPIsforLiveId && (monitoringMailbox.ExternalDirectoryObjectId == null || string.IsNullOrWhiteSpace(monitoringMailbox.ExternalDirectoryObjectId.ToString())))
				{
					WTFDiagnostics.TraceError<string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Mailbox '{0}' does not have a valid ExternalDirectoryObjectId - abandoning it.", monitoringMailbox.UserPrincipalName, null, "CanMonitoringMailboxBeUsed", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1884);
					return false;
				}
			}
			MailboxDatabase mailboxDatabase = this.sessionSelector.TopologyConfigurationSession.Read<MailboxDatabase>(monitoringMailbox.Database);
			if (mailboxDatabase == null)
			{
				WTFDiagnostics.TraceError<string, string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Failed to look up database for Monitoring mailbox {0} ({1})", monitoringMailbox.UserPrincipalName, monitoringMailbox.DisplayName, null, "CanMonitoringMailboxBeUsed", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1894);
				return false;
			}
			if (presumedDatabase != null && presumedDatabase.Guid != mailboxDatabase.Guid)
			{
				WTFDiagnostics.TraceError<string, string, string, string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Mailbox {0} ({1}) is supposed to live on database {2} but actually lives on database {3}", monitoringMailbox.UserPrincipalName, monitoringMailbox.DisplayName, presumedDatabase.Name, mailboxDatabase.Name, null, "CanMonitoringMailboxBeUsed", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1901);
				return false;
			}
			string databaseActiveHost = DirectoryAccessor.Instance.GetDatabaseActiveHost(mailboxDatabase);
			if (string.IsNullOrWhiteSpace(databaseActiveHost))
			{
				WTFDiagnostics.TraceError<string, string, string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Failed to look up active host for database {0} when verifying mailbox {1} ({2})", mailboxDatabase.Name, monitoringMailbox.UserPrincipalName, monitoringMailbox.DisplayName, null, "CanMonitoringMailboxBeUsed", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1909);
				return false;
			}
			if (!string.Equals(databaseActiveHost, Dns.GetHostName(), StringComparison.OrdinalIgnoreCase))
			{
				Server exchangeServerByName = DirectoryAccessor.Instance.GetExchangeServerByName(databaseActiveHost);
				if (this.IsMonitoringOffline(exchangeServerByName))
				{
					WTFDiagnostics.TraceInformation<string, string, string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Monitoring mailbox {0} ({1}) cannot be used because the monitoring component state of server {2} is offline", monitoringMailbox.UserPrincipalName, monitoringMailbox.DisplayName, databaseActiveHost, null, "CanMonitoringMailboxBeUsed", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1922);
					return false;
				}
				if (!this.IsServerCompatible(exchangeServerByName))
				{
					WTFDiagnostics.TraceInformation<string, string, string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Monitoring mailbox {0} ({1}) cannot be used because server {2} belongs to a different monitoring group", monitoringMailbox.UserPrincipalName, monitoringMailbox.DisplayName, databaseActiveHost, null, "CanMonitoringMailboxBeUsed", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1929);
					return false;
				}
				if (string.Compare(exchangeServerByName.SerialNumber, DirectoryAccessor.versionCutoff.ToString(true), true) < 0)
				{
					WTFDiagnostics.TraceInformation<string, string, string, string, string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Monitoring mailbox {0} ({1}) cannot be used because server {2} is running build {3} which is older than {4}", monitoringMailbox.UserPrincipalName, monitoringMailbox.DisplayName, databaseActiveHost, exchangeServerByName.SerialNumber, DirectoryAccessor.versionCutoff.ToString(true), null, "CanMonitoringMailboxBeUsed", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1936);
					return false;
				}
			}
			if (presumedDatabase == null)
			{
				presumedDatabase = mailboxDatabase;
			}
			return true;
		}

		private T[] GetCandidates<T>(QueryFilter compatibilityFilter, int max) where T : ADConfigurationObject, new()
		{
			T[] array = this.sessionSelector.TopologyConfigurationSession.Find<T>(null, QueryScope.SubTree, compatibilityFilter, new SortBy(ADObjectSchema.Guid, SortOrder.Ascending), 1);
			if (array == null || array.Length == 0)
			{
				return new T[0];
			}
			WTFDiagnostics.TraceInformation<string, Guid>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "DirectoryAccessor.GetCandidates: lower bound of object {0} with Guid {1}", array[0].Name, array[0].Guid, null, "GetCandidates", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1994);
			T[] array2 = this.sessionSelector.TopologyConfigurationSession.Find<T>(null, QueryScope.SubTree, compatibilityFilter, new SortBy(ADObjectSchema.Guid, SortOrder.Descending), 1);
			WTFDiagnostics.TraceInformation<string, Guid>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "DirectoryAccessor.GetCandidates: upper bound of object {0} with Guid {1}", array2[0].Name, array2[0].Guid, null, "GetCandidates", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 1996);
			string text = array[0].Guid.ToString("N");
			string text2 = array2[0].Guid.ToString("N");
			uint num = uint.Parse(text.Substring(0, 8), NumberStyles.HexNumber);
			uint num2 = uint.Parse(text2.Substring(0, 8), NumberStyles.HexNumber);
			num = (uint)IPAddress.HostToNetworkOrder((int)num);
			num2 = (uint)IPAddress.HostToNetworkOrder((int)num2);
			uint num3 = num;
			if (num < num2)
			{
				uint hashCode;
				if (this.server.DatabaseAvailabilityGroup != null)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "DirectoryAccessor.GetCandidates: server is on a DAG, using DAG guid to compute hash", null, "GetCandidates", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 2020);
					hashCode = (uint)this.server.DatabaseAvailabilityGroup.ObjectGuid.GetHashCode();
				}
				else
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "DirectoryAccessor.GetCandidates: server is not on a DAG, using server guid to compute hash", null, "GetCandidates", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 2025);
					hashCode = (uint)this.server.Guid.GetHashCode();
				}
				num3 += hashCode % (num2 - num);
			}
			string input = ((uint)IPAddress.NetworkToHostOrder((int)num3)).ToString("x8") + "000000000000000000000000";
			Guid guid = Guid.Parse(input);
			WTFDiagnostics.TraceInformation<Guid>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "DirectoryAccessor.GetCandidates: Guid for filter {0}", guid, null, "GetCandidates", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 2037);
			QueryFilter filter;
			if (compatibilityFilter != null)
			{
				filter = new AndFilter(new QueryFilter[]
				{
					compatibilityFilter,
					new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ADObjectSchema.Guid, guid)
				});
			}
			else
			{
				filter = new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ADObjectSchema.Guid, guid);
			}
			T[] array3 = this.sessionSelector.TopologyConfigurationSession.Find<T>(null, QueryScope.SubTree, filter, new SortBy(ADObjectSchema.Guid, SortOrder.Ascending), max);
			if (array3.Length < max)
			{
				WTFDiagnostics.TraceInformation<int, int>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "DirectoryAccessor.GetCandidates: Number of objects is {0} less than required max {1}. Looking up from beginning for the rest", array3.Length, max, null, "GetCandidates", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 2055);
				QueryFilter filter2;
				if (compatibilityFilter != null)
				{
					filter2 = new AndFilter(new QueryFilter[]
					{
						compatibilityFilter,
						new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ADObjectSchema.Guid, array[0].Guid),
						new ComparisonFilter(ComparisonOperator.LessThan, ADObjectSchema.Guid, guid)
					});
				}
				else
				{
					filter2 = new AndFilter(new QueryFilter[]
					{
						new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ADObjectSchema.Guid, array[0].Guid),
						new ComparisonFilter(ComparisonOperator.LessThan, ADObjectSchema.Guid, guid)
					});
				}
				T[] second = this.sessionSelector.TopologyConfigurationSession.Find<T>(null, QueryScope.SubTree, filter2, new SortBy(ADObjectSchema.Guid, SortOrder.Ascending), max - array3.Length);
				array3 = array3.Union(second).ToArray<T>();
			}
			return array3;
		}

		internal List<AutodiscoverRpcHttpSettings> GetRpcHttpServiceSettings()
		{
			MiniVirtualDirectory[] virtualDirectories = null;
			List<AutodiscoverRpcHttpSettings> list = new List<AutodiscoverRpcHttpSettings>();
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				virtualDirectories = this.getRpcHttpVirtualDirectory();
			});
			if (!adoperationResult.Succeeded || virtualDirectories.Length == 0)
			{
				return list;
			}
			MiniVirtualDirectory virtualDirectory = virtualDirectories[0];
			this.AddRpcHttpSettingsIfAvailable(list, virtualDirectory, ClientAccessType.Internal);
			this.AddRpcHttpSettingsIfAvailable(list, virtualDirectory, ClientAccessType.External);
			return list;
		}

		internal MiniVirtualDirectory[] GetLocalRpcHttpVirtualDirectories()
		{
			return this.sessionSelector.TopologyConfigurationSession.Find<MiniVirtualDirectory>((ADObjectId)this.Server.Identity, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, ADRpcHttpVirtualDirectory.MostDerivedClass), null, 1);
		}

		private void AddRpcHttpSettingsIfAvailable(List<AutodiscoverRpcHttpSettings> settings, MiniVirtualDirectory virtualDirectory, ClientAccessType clientAccessType)
		{
			Uri uri = (clientAccessType == ClientAccessType.Internal) ? virtualDirectory.InternalUrl : virtualDirectory.ExternalUrl;
			if (uri == null)
			{
				return;
			}
			TopologySite topologySite = new TopologySite(this.sessionSelector.TopologyConfigurationSession.GetLocalSite());
			Site site = new Site(topologySite);
			TopologyServer topologyServer = new TopologyServer(this.Server);
			TopologyServerInfo serverInfo = new TopologyServerInfo(site, topologyServer);
			Service service = null;
			if (!RpcHttpService.TryCreateRpcHttpService(virtualDirectory, serverInfo, uri, clientAccessType, AuthenticationMethod.None, out service))
			{
				return;
			}
			RpcHttpService service2 = (RpcHttpService)service;
			AutodiscoverRpcHttpSettings rpcHttpAuthSettingsFromService = AutodiscoverRpcHttpSettings.GetRpcHttpAuthSettingsFromService(service2, clientAccessType, new AutodiscoverRpcHttpSettings.AuthMethodGetter(AutodiscoverRpcHttpSettings.UseProvidedAuthenticationMethod));
			settings.Add(rpcHttpAuthSettingsFromService);
		}

		public string CreatePersonalizedServerName(Guid mailboxGuid, string smtpDomain)
		{
			return ExchangeRpcClientAccess.CreatePersonalizedServer(mailboxGuid, smtpDomain);
		}

		public string CreateAlternateMailboxLegDN(string parentLegacyDNString, Guid mailboxGuid)
		{
			return ADRecipient.CreateAlternateMailboxLegDN(parentLegacyDNString, mailboxGuid);
		}

		internal void RefreshServerOrComputerObject()
		{
			this.server = this.GetExchangeServerByName(Environment.MachineName);
			if (this.server == null)
			{
				this.computer = this.GetNonExchangeServerByName(Environment.MachineName);
			}
		}

		private static ADRecipient SearchForRecipient(QueryFilter queryFilter, IRecipientSession session, ADObjectId root = null)
		{
			ADRecipient[] array = session.Find(root, QueryScope.SubTree, queryFilter, null, 2);
			if (array == null || array.Length <= 0)
			{
				return null;
			}
			if (array.Length == 1)
			{
				return array[0];
			}
			throw new MultipleRecipientsFoundException(queryFilter.ToString());
		}

		private bool IsDatabaseCopyActiveOnLocalServer(Guid databaseGuid, MailboxDatabase database)
		{
			bool result = false;
			DatabaseLocationInfo serverForDatabase = this.activeManager.Value.GetServerForDatabase(databaseGuid);
			if (serverForDatabase != null && serverForDatabase.ServerGuid == this.server.Guid)
			{
				result = true;
			}
			else if (serverForDatabase == null)
			{
				if (database == null)
				{
					database = this.GetMailboxDatabaseFromGuid(databaseGuid);
				}
				if (database != null && database.Server.ObjectGuid == this.server.Guid)
				{
					result = true;
				}
			}
			return result;
		}

		private bool IsGlobalOverridesChangedForType<TWorkDefinition>(IEnumerable<WorkDefinitionOverride> overrides, Container container) where TWorkDefinition : WorkDefinition
		{
			Dictionary<string, WorkDefinitionOverride> dictionary = new Dictionary<string, WorkDefinitionOverride>();
			if (overrides != null)
			{
				foreach (WorkDefinitionOverride workDefinitionOverride in overrides)
				{
					dictionary[workDefinitionOverride.GetIdentityString()] = workDefinitionOverride;
				}
			}
			List<WorkDefinitionOverride> list = this.LoadGlobalOverridesForType<TWorkDefinition>(container);
			foreach (WorkDefinitionOverride workDefinitionOverride2 in list)
			{
				string identityString = workDefinitionOverride2.GetIdentityString();
				if (!dictionary.ContainsKey(identityString))
				{
					return true;
				}
				WorkDefinitionOverride workDefinitionOverride3 = dictionary[identityString];
				if (workDefinitionOverride3.NewPropertyValue != workDefinitionOverride2.NewPropertyValue || workDefinitionOverride3.ExpirationDate != workDefinitionOverride2.ExpirationDate)
				{
					return true;
				}
				dictionary.Remove(identityString);
			}
			return dictionary.Count != 0;
		}

		private List<WorkDefinitionOverride> LoadGlobalOverridesForType<TWorkDefinition>(Container container) where TWorkDefinition : WorkDefinition
		{
			string text = typeof(TWorkDefinition).Name;
			text = text.Substring(0, text.IndexOf("Definition"));
			Container childContainer = container.GetChildContainer(text);
			WTFDiagnostics.TraceInformation<string, Container>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "DirectoryAccessor.LoadGlobalOverridesForType: Override container for {0} is {1}", typeof(TWorkDefinition).Name, childContainer, null, "LoadGlobalOverridesForType", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 2338);
			MonitoringOverride[] array = this.sessionSelector.TopologyConfigurationSession.Find<MonitoringOverride>(childContainer.Id, QueryScope.SubTree, null, null, 1000);
			List<WorkDefinitionOverride> list = new List<WorkDefinitionOverride>();
			foreach (MonitoringOverride monitoringOverride in array)
			{
				if ((monitoringOverride.ExpirationTime == null || !(monitoringOverride.ExpirationTime.Value < DateTime.UtcNow)) && (!(monitoringOverride.ApplyVersion != null) || (this.Server != null && !(monitoringOverride.ApplyVersion != this.Server.AdminDisplayVersion))))
				{
					WorkDefinitionOverride item = new WorkDefinitionOverride
					{
						WorkDefinitionName = monitoringOverride.MonitoringItemName,
						ExpirationDate = (monitoringOverride.ExpirationTime ?? DateTime.MaxValue),
						ServiceName = monitoringOverride.HealthSet,
						PropertyName = monitoringOverride.PropertyName,
						NewPropertyValue = monitoringOverride.PropertyValue
					};
					list.Add(item);
				}
			}
			return list;
		}

		private string FqdnToName(string fqdn)
		{
			int num = fqdn.IndexOf(".");
			if (num != -1)
			{
				return fqdn.Substring(0, num);
			}
			return fqdn;
		}

		private bool IsTenantInLocalForest(string tenantName)
		{
			bool flag = false;
			try
			{
				PartitionId partitionIdByAcceptedDomainName = ADAccountPartitionLocator.GetPartitionIdByAcceptedDomainName(tenantName);
				if (partitionIdByAcceptedDomainName != null)
				{
					flag = ADAccountPartitionLocator.IsKnownPartition(partitionIdByAcceptedDomainName);
					if (!flag)
					{
						WTFDiagnostics.TraceWarning<PartitionId, string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Partition {0} from Tenant name : {1} ", partitionIdByAcceptedDomainName, tenantName, null, "IsTenantInLocalForest", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 2413);
						WTFDiagnostics.TraceWarning<string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Tenant {0} is not in the local forest. Update is needed to relocate the tenant.", tenantName, null, "IsTenantInLocalForest", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 2414);
					}
				}
			}
			catch (CannotResolveTenantNameException)
			{
				WTFDiagnostics.TraceWarning<string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "Error when getting the partition ID with tenant name : {0}", tenantName, null, "IsTenantInLocalForest", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs", 2420);
				flag = false;
			}
			return flag;
		}

		private const int MaxNumberOfGlobalOverrides = 1000;

		private const int MaxADPasswordLength = 128;

		private const int MaxLiveIdPasswordLength = 16;

		private const int NumOfDigitsForHashing = 8;

		private const string PaddingForHashing = "000000000000000000000000";

		private const string DatacenterComponentAssemblyName = "Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.Datacenter.Components.dll";

		private const string LiveIdHelperTypeName = "Microsoft.Exchange.Monitoring.ActiveMonitoring.Common.Datacenter.LiveIdHelper";

		private readonly Random random = new Random();

		private static DirectoryAccessor instance = null;

		private static object locker = new object();

		private static ServerVersion versionCutoff = new ServerVersion(15, 0, 500, 0);

		private Server server;

		private ADComputer computer;

		private Guid? globalOverrideWaterMark = null;

		private Lazy<ActiveManager> activeManager = new Lazy<ActiveManager>(() => ActiveManager.GetCachingActiveManagerInstance());

		private TracingContext traceContext = TracingContext.Default;

		private DirectoryAccessor.SessionSelector sessionSelector;

		private static readonly bool RunningInMultiTenantEnvironment = VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled;

		private static readonly bool RunningInDatacenter = VariantConfiguration.InvariantNoFlightingSnapshot.ActiveMonitoring.DirectoryAccessor.Enabled;

		private static readonly bool ShouldStampProvisioningConstraint = VariantConfiguration.InvariantNoFlightingSnapshot.ActiveMonitoring.PinMonitoringMailboxesToDatabases.Enabled;

		internal DirectoryAccessor.GetRpcHttpVirtualDirectoryStrategy getRpcHttpVirtualDirectory;

		internal delegate MiniVirtualDirectory[] GetRpcHttpVirtualDirectoryStrategy();

		private class SessionSelector
		{
			public SessionSelector()
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
				this.TopologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 2458, ".ctor", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs");
				this.GcScopedConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 2460, ".ctor", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs");
				this.GcScopedConfigurationSession.UseConfigNC = false;
				this.GcScopedConfigurationSession.UseGlobalCatalog = true;
				this.RootorgRecipientSession = DirectorySessionFactory.Default.CreateRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 2464, ".ctor", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs");
				if (!VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).ActiveMonitoring.DirectoryAccessor.Enabled)
				{
					this.writableRootorgRecipientSession = DirectorySessionFactory.Default.CreateRootOrgRecipientSession(false, ConsistencyMode.IgnoreInvalid, sessionSettings, 2468, ".ctor", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs");
					this.rootOrgAcceptedDomain = this.TopologyConfigurationSession.GetDefaultAcceptedDomain();
				}
			}

			public ITopologyConfigurationSession TopologyConfigurationSession { get; private set; }

			public ITopologyConfigurationSession GcScopedConfigurationSession { get; private set; }

			public IRecipientSession RootorgRecipientSession { get; private set; }

			public IRecipientSession RecipientSession
			{
				get
				{
					if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).ActiveMonitoring.DirectoryAccessor.Enabled)
					{
						return this.MonitoringTenantInfo.RecipientSession;
					}
					return this.RootorgRecipientSession;
				}
			}

			public IRecipientSession WritableRecipientSession
			{
				get
				{
					if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).ActiveMonitoring.DirectoryAccessor.Enabled)
					{
						return this.MonitoringTenantInfo.WritableRecipientSession;
					}
					return this.writableRootorgRecipientSession;
				}
			}

			public AcceptedDomain AcceptedDomain
			{
				get
				{
					if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).ActiveMonitoring.DirectoryAccessor.Enabled)
					{
						return this.MonitoringTenantInfo.AcceptedDomain;
					}
					return this.rootOrgAcceptedDomain;
				}
			}

			public DirectoryAccessor.MonitoringTenantInfo MonitoringTenantInfo
			{
				get
				{
					if (Settings.UseE14MonitoringTenant)
					{
						return this.e14Tenant;
					}
					return this.e15Tenant;
				}
			}

			public void InitializeMonitoringTenants()
			{
				if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).ActiveMonitoring.DirectoryAccessor.Enabled)
				{
					this.e14Tenant = new DirectoryAccessor.MonitoringTenantInfo(string.Empty);
					this.e15Tenant = new DirectoryAccessor.MonitoringTenantInfo("E15");
				}
			}

			private AcceptedDomain rootOrgAcceptedDomain;

			private IRecipientSession writableRootorgRecipientSession;

			private DirectoryAccessor.MonitoringTenantInfo e14Tenant;

			private DirectoryAccessor.MonitoringTenantInfo e15Tenant;
		}

		private class MonitoringTenantInfo
		{
			public MonitoringTenantInfo(string suffix)
			{
				this.MonitoringTenantName = MailboxTaskHelper.GetMonitoringTenantName(suffix);
				try
				{
					ADSessionSettings adsessionSettings = ADSessionSettings.FromTenantCUName(this.MonitoringTenantName);
					this.TenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.IgnoreInvalid, adsessionSettings, 2599, ".ctor", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs");
					this.RecipientSession = DirectorySessionFactory.Default.CreateTenantRecipientSession(true, ConsistencyMode.IgnoreInvalid, adsessionSettings, 2600, ".ctor", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs");
					this.WritableRecipientSession = DirectorySessionFactory.Default.CreateTenantRecipientSession(false, ConsistencyMode.IgnoreInvalid, adsessionSettings, 2601, ".ctor", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DirectoryAccessor.cs");
					this.AcceptedDomain = this.TenantConfigurationSession.GetDefaultAcceptedDomain();
					this.MonitoringTenantPartitionId = adsessionSettings.PartitionId.ToString();
					this.MonitoringTenantForestFqdn = adsessionSettings.PartitionId.ForestFQDN;
					this.MonitoringTenantOrganizationId = adsessionSettings.CurrentOrganizationId;
					ExchangeConfigurationUnit[] array = this.TenantConfigurationSession.Find<ExchangeConfigurationUnit>(null, QueryScope.SubTree, null, null, 0);
					if (array != null && array.Length == 1 && array[0].OrganizationStatus == OrganizationStatus.Active)
					{
						this.MonitoringTenantReady = true;
					}
				}
				catch (CannotResolveTenantNameException)
				{
				}
			}

			public string MonitoringTenantName { get; private set; }

			public IConfigurationSession TenantConfigurationSession { get; private set; }

			public IRecipientSession RecipientSession { get; private set; }

			public IRecipientSession WritableRecipientSession { get; private set; }

			public string MonitoringTenantPartitionId { get; private set; }

			public OrganizationId MonitoringTenantOrganizationId { get; private set; }

			public bool MonitoringTenantReady { get; private set; }

			public string MonitoringTenantForestFqdn { get; private set; }

			public AcceptedDomain AcceptedDomain { get; private set; }
		}
	}
}
