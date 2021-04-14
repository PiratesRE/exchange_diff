using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class UpdateVirtualDirectoryVersion<T> : DataAccessTask<T> where T : ExchangeVirtualDirectory, new()
	{
		[Parameter]
		public new Fqdn DomainController
		{
			get
			{
				return base.DomainController;
			}
			set
			{
				base.DomainController = value;
			}
		}

		protected Server Server
		{
			get
			{
				return this.server;
			}
			set
			{
				this.server = value;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			ADSessionSettings sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
			this.configurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(this.DomainController, false, ConsistencyMode.PartiallyConsistent, null, sessionSettings, 70, "InternalBeginProcessing", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\VirtualDirectoryTasks\\UpdateVirtualDirectoryVersion.cs");
			TaskLogger.LogExit();
		}

		protected override IConfigDataProvider CreateSession()
		{
			return this.configurationSession;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			IConfigDataProvider dataSession = base.DataSession;
			IEnumerable<T> enumerable = dataSession.FindPaged<T>(null, this.Server.Identity, true, null, 0);
			foreach (T t in enumerable)
			{
				if (!this.ShouldSkipVDir(t) && t.ExchangeVersion.IsOlderThan(t.MaximumSupportedExchangeObjectVersion))
				{
					try
					{
						t.SetExchangeVersion(t.MaximumSupportedExchangeObjectVersion);
						base.DataSession.Save(t);
					}
					catch (DataSourceTransientException exception)
					{
						base.WriteError(exception, ErrorCategory.WriteError, null);
					}
				}
			}
			TaskLogger.LogExit();
		}

		protected virtual bool ShouldSkipVDir(T vDir)
		{
			return false;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			ServerIdParameter serverIdParameter = new ServerIdParameter();
			this.Server = (Server)base.GetDataObject<Server>(serverIdParameter, base.DataSession, null, new LocalizedString?(Strings.ErrorServerNotFound(serverIdParameter.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(serverIdParameter.ToString())));
			if (!this.ShouldUpdateVirtualDirectory())
			{
				base.WriteError(this.Server.GetServerRoleError(ServerRole.Mailbox | ServerRole.ClientAccess | ServerRole.UnifiedMessaging | ServerRole.HubTransport), ErrorCategory.InvalidOperation, this.Server);
			}
			base.InternalValidate();
			TaskLogger.LogExit();
		}

		protected virtual bool ShouldUpdateVirtualDirectory()
		{
			return this.Server.IsClientAccessServer || this.Server.IsFfoWebServiceRole || this.Server.IsCafeServer || this.Server.IsOSPRole;
		}

		private ITopologyConfigurationSession configurationSession;

		private Server server;
	}
}
