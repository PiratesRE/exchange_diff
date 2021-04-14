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
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Migration;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.AnchorService
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AnchorRunspaceProxy : DisposeTrackableBase, IAnchorRunspaceProxy, IDisposable
	{
		private AnchorRunspaceProxy(AnchorContext context, AnchorRunspaceProxy.RunspaceFactoryWithDCAffinity runspaceFactory)
		{
			AnchorUtil.ThrowOnNullArgument(context, "context");
			AnchorUtil.ThrowOnNullArgument(runspaceFactory, "runspaceFactory");
			this.Context = context;
			this.runspaceProxy = new RunspaceProxy(new RunspaceMediator(runspaceFactory, new EmptyRunspaceCache()));
		}

		public RunspaceProxy RunspaceProxy
		{
			get
			{
				return this.runspaceProxy;
			}
		}

		internal AnchorContext Context { get; private set; }

		public static AnchorRunspaceProxy CreateRunspaceForDatacenterAdmin(AnchorContext context, ADObjectId ownerId)
		{
			AnchorUtil.ThrowOnNullArgument(ownerId, "ownerId");
			return AnchorRunspaceProxy.CreateRunspaceForDatacenterAdmin(context, ownerId.ToString());
		}

		public static AnchorRunspaceProxy CreateRunspaceForDatacenterAdmin(AnchorContext context, string ownerId)
		{
			AnchorUtil.ThrowOnNullArgument(ownerId, "ownerId");
			context.Logger.Log(MigrationEventType.Verbose, "Creating runspace proxy for datacenter admin", new object[0]);
			return new AnchorRunspaceProxy(context, AnchorRunspaceProxy.RunspaceFactoryWithDCAffinity.CreateUnrestrictedFactory(ownerId));
		}

		public static AnchorRunspaceProxy CreateRunspaceForTenantAdmin(AnchorContext context, ADObjectId ownerId, ADUser tenantAdmin)
		{
			AnchorUtil.ThrowOnNullArgument(tenantAdmin, "tenantAdmin");
			AnchorUtil.ThrowOnNullArgument(ownerId, "ownerId");
			context.Logger.Log(MigrationEventType.Verbose, "AnchorRunspaceProxy. Creating runspace proxy for user {0}", new object[]
			{
				tenantAdmin.Name
			});
			ExchangeRunspaceConfigurationSettings configSettings = new ExchangeRunspaceConfigurationSettings(ExchangeRunspaceConfigurationSettings.ExchangeApplication.SimpleDataMigration, null, ExchangeRunspaceConfigurationSettings.SerializationLevel.None);
			return new AnchorRunspaceProxy(context, AnchorRunspaceProxy.RunspaceFactoryWithDCAffinity.CreateRbacFactory(context, ownerId.ToString(), new GenericSidIdentity(tenantAdmin.Name, string.Empty, tenantAdmin.Sid), configSettings));
		}

		public static AnchorRunspaceProxy CreateRunspaceForDelegatedTenantAdmin(AnchorContext context, DelegatedPrincipal delegatedTenantAdmin)
		{
			AnchorUtil.ThrowOnNullArgument(delegatedTenantAdmin, "delegatedTenantAdmin");
			context.Logger.Log(MigrationEventType.Verbose, "AnchorRunspaceProxy. Creating delegated runspace proxy for user {0}", new object[]
			{
				delegatedTenantAdmin
			});
			ExchangeRunspaceConfigurationSettings configSettings = new ExchangeRunspaceConfigurationSettings(ExchangeRunspaceConfigurationSettings.ExchangeApplication.SimpleDataMigration, null, ExchangeRunspaceConfigurationSettings.SerializationLevel.None);
			return new AnchorRunspaceProxy(context, AnchorRunspaceProxy.RunspaceFactoryWithDCAffinity.CreateRbacFactory(context, delegatedTenantAdmin.ToString(), delegatedTenantAdmin.Identity, configSettings));
		}

		public static AnchorRunspaceProxy CreateRunspaceForPartner(AnchorContext context, ADObjectId ownerId, ADUser tenantAdmin, string tenantOrganization)
		{
			AnchorUtil.ThrowOnNullArgument(ownerId, "ownerId");
			AnchorUtil.ThrowOnNullArgument(tenantAdmin, "tenantAdmin");
			AnchorUtil.ThrowOnNullOrEmptyArgument(tenantOrganization, "tenantOrganization");
			context.Logger.Log(MigrationEventType.Verbose, "AnchorRunspaceProxy. Creating partner runspace proxy for user {0}", new object[]
			{
				tenantAdmin.Name
			});
			ExchangeRunspaceConfigurationSettings configSettings = new ExchangeRunspaceConfigurationSettings(ExchangeRunspaceConfigurationSettings.ExchangeApplication.SimpleDataMigration, tenantOrganization, ExchangeRunspaceConfigurationSettings.GetDefaultInstance().CurrentSerializationLevel);
			return new AnchorRunspaceProxy(context, AnchorRunspaceProxy.RunspaceFactoryWithDCAffinity.CreateRbacFactory(context, ownerId.ToString(), new GenericSidIdentity(tenantAdmin.Name, string.Empty, tenantAdmin.Sid), configSettings));
		}

		public static AnchorRunspaceProxy CreateRunspaceForDelegatedPartner(AnchorContext context, DelegatedPrincipal delegatedPartnerAdmin, string tenantOrganization)
		{
			AnchorUtil.ThrowOnNullArgument(delegatedPartnerAdmin, "delegatedTenantAdmin");
			AnchorUtil.ThrowOnNullOrEmptyArgument(tenantOrganization, "tenantOrganization");
			context.Logger.Log(MigrationEventType.Verbose, "AnchorRunspaceProxy. Creating delegated partner runspace proxy for user {0}", new object[]
			{
				delegatedPartnerAdmin
			});
			ExchangeRunspaceConfigurationSettings configSettings = new ExchangeRunspaceConfigurationSettings(ExchangeRunspaceConfigurationSettings.ExchangeApplication.SimpleDataMigration, tenantOrganization, ExchangeRunspaceConfigurationSettings.GetDefaultInstance().CurrentSerializationLevel);
			return new AnchorRunspaceProxy(context, AnchorRunspaceProxy.RunspaceFactoryWithDCAffinity.CreateRbacFactory(context, delegatedPartnerAdmin.ToString(), delegatedPartnerAdmin.Identity, configSettings));
		}

		public static string GetCommandString(PSCommand command)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Command command2 in command.Commands)
			{
				stringBuilder.Append(command2.CommandText);
				stringBuilder.Append(" ");
			}
			return stringBuilder.ToString();
		}

		public T RunPSCommandSingleOrDefault<T>(PSCommand command, out ErrorRecord error)
		{
			Collection<T> source = this.RunPSCommand<T>(command, out error);
			if (error != null)
			{
				return default(T);
			}
			return source.FirstOrDefault<T>();
		}

		public T RunPSCommandSingleOrDefault<T>(PSCommand command) where T : class
		{
			ErrorRecord errorRecord = null;
			string commandString = AnchorRunspaceProxy.GetCommandString(command);
			try
			{
				T result = this.RunPSCommandSingleOrDefault<T>(command, out errorRecord);
				if (errorRecord == null)
				{
					this.Context.Logger.Log(MigrationEventType.Verbose, "Running PS command {0}", new object[]
					{
						commandString
					});
					return result;
				}
			}
			catch (ParameterBindingException ex)
			{
				return this.HandleException<T>(commandString, ex);
			}
			catch (CmdletInvocationException ex2)
			{
				return this.HandleException<T>(commandString, ex2);
			}
			AnchorUtil.AssertOrThrow(errorRecord != null, "expect to have an error at this point", new object[0]);
			if (errorRecord.Exception != null)
			{
				return this.HandleException<T>(commandString, errorRecord.Exception);
			}
			throw new MigrationPermanentException(ServerStrings.MigrationRunspaceError(commandString, errorRecord.ToString()));
		}

		public Collection<T> RunPSCommand<T>(PSCommand command, out ErrorRecord error)
		{
			PowerShellProxy powerShellProxy = null;
			Collection<T> result = AnchorUtil.RunTimedOperation<Collection<T>>(this.Context, delegate()
			{
				powerShellProxy = new PowerShellProxy(this.runspaceProxy, command);
				return powerShellProxy.Invoke<T>();
			}, AnchorRunspaceProxy.GetCommandString(command));
			if (powerShellProxy.Failed)
			{
				error = powerShellProxy.Errors[0];
				return null;
			}
			error = null;
			return result;
		}

		public Collection<T> RunPSCommand<T>(PSCommand command) where T : class
		{
			ErrorRecord errorRecord = null;
			string commandString = AnchorRunspaceProxy.GetCommandString(command);
			try
			{
				Collection<T> result = this.RunPSCommand<T>(command, out errorRecord);
				if (errorRecord == null)
				{
					this.Context.Logger.Log(MigrationEventType.Verbose, "Running PS command {0}", new object[]
					{
						commandString
					});
					return result;
				}
			}
			catch (ParameterBindingException ex)
			{
				this.HandleException<T>(commandString, ex);
			}
			catch (CmdletInvocationException ex2)
			{
				this.HandleException<T>(commandString, ex2);
			}
			AnchorUtil.AssertOrThrow(errorRecord != null, "expect to have an error at this point", new object[0]);
			if (errorRecord.Exception != null)
			{
				this.HandleException<T>(commandString, errorRecord.Exception);
			}
			throw new MigrationPermanentException(ServerStrings.MigrationRunspaceError(commandString, errorRecord.ToString()));
		}

		protected virtual T HandleException<T>(string commandString, Exception ex)
		{
			throw new MigrationPermanentException(ServerStrings.MigrationRunspaceError(commandString, ex.Message), ex);
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
			return DisposeTracker.Get<AnchorRunspaceProxy>(this);
		}

		private RunspaceProxy runspaceProxy;

		private class RunspaceFactoryWithDCAffinity : RunspaceFactory
		{
			private RunspaceFactoryWithDCAffinity(string affinityToken, InitialSessionStateFactory issFactory, PSHostFactory hostFactory) : base(issFactory, hostFactory, true)
			{
				AnchorUtil.ThrowOnNullOrEmptyArgument(affinityToken, "affinityToken");
				this.affinityToken = affinityToken;
			}

			private RunspaceFactoryWithDCAffinity(string affinityToken, AnchorRunspaceProxy.FullExchangeRunspaceConfigurationFactory configurationFactory, PSHostFactory hostFactory) : base(configurationFactory, hostFactory)
			{
				AnchorUtil.ThrowOnNullOrEmptyArgument(affinityToken, "affinityToken");
				this.affinityToken = affinityToken;
			}

			public static AnchorRunspaceProxy.RunspaceFactoryWithDCAffinity CreateUnrestrictedFactory(string affinityToken)
			{
				return new AnchorRunspaceProxy.RunspaceFactoryWithDCAffinity(affinityToken, AnchorRunspaceProxy.FullExchangeRunspaceConfigurationFactory.GetInstance(), new BasicPSHostFactory(typeof(RunspaceHost), true));
			}

			public static AnchorRunspaceProxy.RunspaceFactoryWithDCAffinity CreateRbacFactory(AnchorContext context, string affinityToken, IIdentity tenantIdentity, ExchangeRunspaceConfigurationSettings configSettings)
			{
				InitialSessionState initialSessionState;
				try
				{
					initialSessionState = new ExchangeExpiringRunspaceConfiguration(tenantIdentity, configSettings).CreateInitialSessionState();
					initialSessionState.LanguageMode = PSLanguageMode.FullLanguage;
				}
				catch (CmdletAccessDeniedException ex)
				{
					context.Logger.Log(MigrationEventType.Warning, ex, "AnchorRunspaceProxy. error creating session for user {0}", new object[]
					{
						tenantIdentity
					});
					throw new UserDoesNotHaveRBACException(tenantIdentity.ToString(), ex);
				}
				catch (AuthzException ex2)
				{
					context.Logger.Log(MigrationEventType.Error, ex2, "AnchorRunspaceProxy. authorization error creating session for user {0}", new object[]
					{
						tenantIdentity
					});
					throw new UserDoesNotHaveRBACException(tenantIdentity.ToString(), ex2);
				}
				return new AnchorRunspaceProxy.RunspaceFactoryWithDCAffinity(affinityToken, new BasicInitialSessionStateFactory(initialSessionState), new BasicPSHostFactory(typeof(RunspaceHost), true));
			}

			protected override void InitializeRunspace(Runspace runspace)
			{
				base.InitializeRunspace(runspace);
				RunspaceServerSettings value = RunspaceServerSettings.CreateGcOnlyRunspaceServerSettings(this.affinityToken, false);
				runspace.SessionStateProxy.SetVariable(ExchangePropertyContainer.ADServerSettingsVarName, value);
			}

			private readonly string affinityToken;
		}

		private class FullExchangeRunspaceConfigurationFactory : RunspaceConfigurationFactory
		{
			public static AnchorRunspaceProxy.FullExchangeRunspaceConfigurationFactory GetInstance()
			{
				if (AnchorRunspaceProxy.FullExchangeRunspaceConfigurationFactory.instance == null)
				{
					AnchorRunspaceProxy.FullExchangeRunspaceConfigurationFactory.instance = new AnchorRunspaceProxy.FullExchangeRunspaceConfigurationFactory();
				}
				return AnchorRunspaceProxy.FullExchangeRunspaceConfigurationFactory.instance;
			}

			public override RunspaceConfiguration CreateRunspaceConfiguration()
			{
				RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();
				AnchorRunspaceProxy.FullExchangeRunspaceConfigurationFactory.AddPSSnapIn(runspaceConfiguration, "Microsoft.Exchange.Management.PowerShell.E2010");
				return runspaceConfiguration;
			}

			private static void AddPSSnapIn(RunspaceConfiguration runspaceConfiguration, string mshSnapInName)
			{
				PSSnapInException ex = null;
				runspaceConfiguration.AddPSSnapIn(mshSnapInName, out ex);
				if (ex != null)
				{
					throw new CouldNotAddExchangeSnapInTransientException(mshSnapInName, ex);
				}
			}

			private static AnchorRunspaceProxy.FullExchangeRunspaceConfigurationFactory instance;
		}
	}
}
