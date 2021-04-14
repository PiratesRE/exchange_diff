using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Migration.Logging;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationRunspaceProxy : DisposeTrackableBase, IMigrationRunspaceProxy, IDisposable
	{
		private MigrationRunspaceProxy(MigrationRunspaceProxy.RunspaceFactoryWithDCAffinity runspaceFactory)
		{
			MigrationUtil.ThrowOnNullArgument(runspaceFactory, "runspaceFactory");
			this.runspaceProxy = new RunspaceProxy(new RunspaceMediator(runspaceFactory, new EmptyRunspaceCache()));
		}

		public RunspaceProxy RunspaceProxy
		{
			get
			{
				return this.runspaceProxy;
			}
		}

		public static MigrationRunspaceProxy CreateRunspaceForDatacenterAdmin(OrganizationId organizationId)
		{
			MigrationUtil.ThrowOnNullArgument(organizationId, "organizationId");
			MigrationLogger.Log(MigrationEventType.Verbose, "MigrationRunspaceProxy. Creating runspace proxy for datacenter admin", new object[0]);
			return new MigrationRunspaceProxy(MigrationRunspaceProxy.RunspaceFactoryWithDCAffinity.CreateUnrestrictedFactory(organizationId));
		}

		public static MigrationRunspaceProxy CreateRunspaceForTenantAdmin(ADObjectId ownerId, ADUser tenantAdmin)
		{
			MigrationUtil.ThrowOnNullArgument(tenantAdmin, "tenantAdmin");
			MigrationUtil.ThrowOnNullArgument(ownerId, "ownerId");
			MigrationLogger.Log(MigrationEventType.Verbose, "MigrationRunspaceProxy. Creating runspace proxy for user {0}", new object[]
			{
				tenantAdmin.Name
			});
			ExchangeRunspaceConfigurationSettings configSettings = new ExchangeRunspaceConfigurationSettings(ExchangeRunspaceConfigurationSettings.ExchangeApplication.SimpleDataMigration, null, ExchangeRunspaceConfigurationSettings.SerializationLevel.None);
			return new MigrationRunspaceProxy(MigrationRunspaceProxy.RunspaceFactoryWithDCAffinity.CreateRbacFactory(tenantAdmin.OrganizationId, new GenericSidIdentity(tenantAdmin.Name, string.Empty, tenantAdmin.Sid), configSettings));
		}

		public static MigrationRunspaceProxy CreateRunspaceForDelegatedTenantAdmin(DelegatedPrincipal delegatedTenantAdmin)
		{
			MigrationUtil.ThrowOnNullArgument(delegatedTenantAdmin, "delegatedTenantAdmin");
			MigrationLogger.Log(MigrationEventType.Verbose, "MigrationRunspaceProxy. Creating delegated runspace proxy for user {0}", new object[]
			{
				delegatedTenantAdmin
			});
			ExchangeRunspaceConfigurationSettings configSettings = new ExchangeRunspaceConfigurationSettings(ExchangeRunspaceConfigurationSettings.ExchangeApplication.SimpleDataMigration, null, ExchangeRunspaceConfigurationSettings.SerializationLevel.None);
			return new MigrationRunspaceProxy(MigrationRunspaceProxy.RunspaceFactoryWithDCAffinity.CreateRbacFactory(MigrationADProvider.GetOrganization(delegatedTenantAdmin.DelegatedOrganization), delegatedTenantAdmin.Identity, configSettings));
		}

		public static MigrationRunspaceProxy CreateRunspaceForPartner(ADObjectId ownerId, ADUser tenantAdmin, string tenantOrganization)
		{
			MigrationUtil.ThrowOnNullArgument(ownerId, "ownerId");
			MigrationUtil.ThrowOnNullArgument(tenantAdmin, "tenantAdmin");
			MigrationUtil.ThrowOnNullOrEmptyArgument(tenantOrganization, "tenantOrganization");
			MigrationLogger.Log(MigrationEventType.Verbose, "MigrationRunspaceProxy. Creating partner runspace proxy for user {0}", new object[]
			{
				tenantAdmin.Name
			});
			ExchangeRunspaceConfigurationSettings configSettings = new ExchangeRunspaceConfigurationSettings(ExchangeRunspaceConfigurationSettings.ExchangeApplication.SimpleDataMigration, tenantOrganization, ExchangeRunspaceConfigurationSettings.GetDefaultInstance().CurrentSerializationLevel);
			return new MigrationRunspaceProxy(MigrationRunspaceProxy.RunspaceFactoryWithDCAffinity.CreateRbacFactory(tenantAdmin.OrganizationId, new GenericSidIdentity(tenantAdmin.Name, string.Empty, tenantAdmin.Sid), configSettings));
		}

		public static MigrationRunspaceProxy CreateRunspaceForDelegatedPartner(DelegatedPrincipal delegatedPartnerAdmin, string tenantOrganization)
		{
			MigrationUtil.ThrowOnNullArgument(delegatedPartnerAdmin, "delegatedTenantAdmin");
			MigrationUtil.ThrowOnNullOrEmptyArgument(tenantOrganization, "tenantOrganization");
			MigrationLogger.Log(MigrationEventType.Verbose, "MigrationRunspaceProxy. Creating delegated partner runspace proxy for user {0}", new object[]
			{
				delegatedPartnerAdmin
			});
			ExchangeRunspaceConfigurationSettings configSettings = new ExchangeRunspaceConfigurationSettings(ExchangeRunspaceConfigurationSettings.ExchangeApplication.SimpleDataMigration, tenantOrganization, ExchangeRunspaceConfigurationSettings.GetDefaultInstance().CurrentSerializationLevel);
			return new MigrationRunspaceProxy(MigrationRunspaceProxy.RunspaceFactoryWithDCAffinity.CreateRbacFactory(MigrationADProvider.GetOrganization(tenantOrganization), delegatedPartnerAdmin.Identity, configSettings));
		}

		public static string GetCommandString(PSCommand command)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Command command2 in command.Commands)
			{
				stringBuilder.Append(command2.CommandText);
				foreach (CommandParameter commandParameter in command2.Parameters)
				{
					stringBuilder.AppendFormat(" -{0}", commandParameter.Name);
				}
				stringBuilder.Append(" ");
			}
			return stringBuilder.ToString();
		}

		public T RunPSCommand<T>(PSCommand command, out ErrorRecord error)
		{
			PowerShellProxy powerShellProxy = null;
			Collection<T> source = MigrationUtil.RunTimedOperation<Collection<T>>(delegate()
			{
				powerShellProxy = new PowerShellProxy(this.runspaceProxy, command);
				return powerShellProxy.Invoke<T>();
			}, MigrationRunspaceProxy.GetCommandString(command));
			if (powerShellProxy.Failed)
			{
				error = powerShellProxy.Errors[0];
				return default(T);
			}
			error = null;
			return source.FirstOrDefault<T>();
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.runspaceProxy != null)
				{
					this.runspaceProxy.Dispose();
				}
				this.runspaceProxy = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MigrationRunspaceProxy>(this);
		}

		private RunspaceProxy runspaceProxy;

		private class RunspaceFactoryWithDCAffinity : RunspaceFactory
		{
			private RunspaceFactoryWithDCAffinity(OrganizationId organizationId, InitialSessionStateFactory issFactory, PSHostFactory hostFactory) : base(issFactory, hostFactory, true)
			{
				MigrationUtil.ThrowOnNullArgument(organizationId, "organizationId");
				this.organizationId = organizationId;
			}

			private RunspaceFactoryWithDCAffinity(OrganizationId organizationId, MigrationRunspaceProxy.FullExchangeRunspaceConfigurationFactory configurationFactory, PSHostFactory hostFactory) : base(configurationFactory, hostFactory)
			{
				MigrationUtil.ThrowOnNullArgument(organizationId, "organizationId");
				this.organizationId = organizationId;
			}

			public static MigrationRunspaceProxy.RunspaceFactoryWithDCAffinity CreateUnrestrictedFactory(OrganizationId organizationId)
			{
				return new MigrationRunspaceProxy.RunspaceFactoryWithDCAffinity(organizationId, MigrationRunspaceProxy.FullExchangeRunspaceConfigurationFactory.GetInstance(), new BasicPSHostFactory(typeof(RunspaceHost), true));
			}

			public static MigrationRunspaceProxy.RunspaceFactoryWithDCAffinity CreateRbacFactory(OrganizationId organizationId, IIdentity tenantIdentity, ExchangeRunspaceConfigurationSettings configSettings)
			{
				InitialSessionState initialSessionState;
				try
				{
					initialSessionState = new ExchangeExpiringRunspaceConfiguration(tenantIdentity, configSettings).CreateInitialSessionState();
					initialSessionState.LanguageMode = PSLanguageMode.FullLanguage;
				}
				catch (CmdletAccessDeniedException ex)
				{
					MigrationLogger.Log(MigrationEventType.Warning, ex, "MigrationRunspaceProxy. error creating session for user {0}", new object[]
					{
						tenantIdentity
					});
					throw new UserDoesNotHaveRBACException(tenantIdentity.ToString(), ex);
				}
				catch (AuthzException ex2)
				{
					MigrationLogger.Log(MigrationEventType.Error, ex2, "MigrationRunspaceProxy. authorization error creating session for user {0}", new object[]
					{
						tenantIdentity
					});
					throw new UserDoesNotHaveRBACException(tenantIdentity.ToString(), ex2);
				}
				return new MigrationRunspaceProxy.RunspaceFactoryWithDCAffinity(organizationId, new BasicInitialSessionStateFactory(initialSessionState), new BasicPSHostFactory(typeof(RunspaceHost), true));
			}

			protected override void InitializeRunspace(Runspace runspace)
			{
				base.InitializeRunspace(runspace);
				MigrationLogger.Log(MigrationEventType.Verbose, "Initializing runspace for organization {0}", new object[]
				{
					this.organizationId
				});
				string token = (this.organizationId == OrganizationId.ForestWideOrgId) ? "RootOrg" : RunspaceServerSettings.GetTokenForOrganization(this.organizationId);
				RunspaceServerSettings runspaceServerSettings;
				if (this.organizationId != null && !this.organizationId.PartitionId.IsLocalForestPartition())
				{
					runspaceServerSettings = RunspaceServerSettings.CreateGcOnlyRunspaceServerSettings(token, this.organizationId.PartitionId.ForestFQDN, false);
					runspaceServerSettings.RecipientViewRoot = ADSystemConfigurationSession.GetRootOrgContainerId(null, null).DomainId;
				}
				else
				{
					runspaceServerSettings = RunspaceServerSettings.CreateGcOnlyRunspaceServerSettings(token, false);
				}
				runspace.SessionStateProxy.SetVariable(ExchangePropertyContainer.ADServerSettingsVarName, runspaceServerSettings);
			}

			private readonly OrganizationId organizationId;
		}

		private class FullExchangeRunspaceConfigurationFactory : RunspaceConfigurationFactory
		{
			public static MigrationRunspaceProxy.FullExchangeRunspaceConfigurationFactory GetInstance()
			{
				if (MigrationRunspaceProxy.FullExchangeRunspaceConfigurationFactory.instance == null)
				{
					MigrationRunspaceProxy.FullExchangeRunspaceConfigurationFactory.instance = new MigrationRunspaceProxy.FullExchangeRunspaceConfigurationFactory();
				}
				return MigrationRunspaceProxy.FullExchangeRunspaceConfigurationFactory.instance;
			}

			public override RunspaceConfiguration CreateRunspaceConfiguration()
			{
				RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();
				MigrationRunspaceProxy.FullExchangeRunspaceConfigurationFactory.AddPSSnapIn(runspaceConfiguration, "Microsoft.Exchange.Management.PowerShell.E2010");
				return runspaceConfiguration;
			}

			private static void AddPSSnapIn(RunspaceConfiguration runspaceConfiguration, string mshSnapInName)
			{
				PSSnapInException ex;
				runspaceConfiguration.AddPSSnapIn(mshSnapInName, out ex);
				if (ex != null)
				{
					MigrationLogger.Log(MigrationEventType.Warning, ex, "MigrationRunspaceProxy.AddPSSnapIn: error creating loading exchange snappin {0}", new object[]
					{
						mshSnapInName
					});
					throw new CouldNotAddExchangeSnapInTransientException(mshSnapInName, ex);
				}
			}

			private static MigrationRunspaceProxy.FullExchangeRunspaceConfigurationFactory instance;
		}
	}
}
