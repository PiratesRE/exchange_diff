using System;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport.Internal.MExRuntime;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.Management.AgentTasks
{
	public abstract class AgentBaseTask : DataAccessTask<Server>
	{
		protected Server GetServerObject()
		{
			Server result = null;
			try
			{
				result = ((ITopologyConfigurationSession)base.DataSession).FindLocalServer();
			}
			catch (LocalServerNotFoundException)
			{
				this.ThrowRoleRestrictionError();
			}
			return result;
		}

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

		[Parameter(Mandatory = false)]
		public TransportService TransportService
		{
			get
			{
				return (TransportService)(base.Fields["TransportService"] ?? TransportService.Hub);
			}
			set
			{
				base.Fields["TransportService"] = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(this.DomainController, true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 101, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\Agents\\AgentBaseTask.cs");
		}

		protected override void InternalProcessRecord()
		{
			this.EnsureTransportServiceIsSupported();
			this.EnsureMExConfigLoaded();
		}

		private void EnsureTransportServiceIsSupported()
		{
			if (this.TransportService != TransportService.Hub && this.TransportService != TransportService.Edge && this.TransportService != TransportService.FrontEnd)
			{
				base.ThrowTerminatingError(new LocalizedException(AgentStrings.TransportServiceNotSupported(this.TransportService.ToString())), ErrorCategory.InvalidOperation, null);
			}
		}

		private void EnsureMExConfigLoaded()
		{
			if (this.mexConfig != null)
			{
				return;
			}
			Server serverObject = this.GetServerObject();
			if (serverObject == null || (!serverObject.IsEdgeServer && !serverObject.IsHubTransportServer && !serverObject.IsFrontendTransportServer))
			{
				this.ThrowRoleRestrictionError();
			}
			this.SetMExConfigPath();
			ProcessTransportRole transportProcessRole = ProcessTransportRole.Hub;
			if (serverObject.IsEdgeServer)
			{
				transportProcessRole = ProcessTransportRole.Edge;
			}
			else if (!serverObject.IsHubTransportServer && serverObject.IsFrontendTransportServer)
			{
				transportProcessRole = ProcessTransportRole.FrontEnd;
			}
			this.mexConfig = new MExConfiguration(transportProcessRole, ConfigurationContext.Setup.InstallPath);
			try
			{
				this.mexConfig.Load(this.mexConfigPath);
			}
			catch (ExchangeConfigurationException ex)
			{
				if (ex.InnerException is FileNotFoundException)
				{
					if (!(this is InstallTransportAgent))
					{
						base.ThrowTerminatingError(ex, ErrorCategory.ReadError, null);
					}
					this.missingConfigFile = true;
				}
				else
				{
					base.ThrowTerminatingError(ex, ErrorCategory.ReadError, null);
				}
			}
		}

		private void ThrowRoleRestrictionError()
		{
			base.ThrowTerminatingError(new LocalizedException(AgentStrings.TransportAgentTasksOnlyOnFewRoles), ErrorCategory.InvalidOperation, null);
		}

		internal string ValidateAndNormalizeAgentIdentity(string identity)
		{
			if (string.IsNullOrEmpty(identity))
			{
				base.WriteError(new ArgumentNullException("Identity", AgentStrings.NoIdentityArgument), ErrorCategory.InvalidArgument, null);
			}
			string text = identity.TrimStart(new char[]
			{
				'"',
				' '
			});
			text = text.TrimEnd(new char[]
			{
				'"',
				' '
			});
			if (string.IsNullOrEmpty(text))
			{
				base.WriteError(new ArgumentNullException("Identity", AgentStrings.NoIdentityArgument), ErrorCategory.InvalidArgument, null);
			}
			string text2 = "\",*";
			char[] anyOf = text2.ToCharArray();
			int num = text.IndexOfAny(anyOf, 0);
			if (num > -1)
			{
				base.WriteError(new ArgumentException(AgentStrings.InvalidIdentity, "Identity"), ErrorCategory.InvalidArgument, null);
			}
			return text;
		}

		internal bool AgentExists(string name)
		{
			if (this.mexConfig != null)
			{
				foreach (AgentInfo agentInfo in this.MExConfiguration.AgentList)
				{
					if (string.Compare(agentInfo.AgentName, name, StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		internal void Save()
		{
			if (this.mexConfig != null)
			{
				try
				{
					this.mexConfig.Save(this.mexConfigPath);
				}
				catch (ExchangeConfigurationException exception)
				{
					base.WriteError(exception, ErrorCategory.WriteError, null);
				}
			}
		}

		internal void SetAgentEnabled(string identity, bool enabled)
		{
			string strB = this.ValidateAndNormalizeAgentIdentity(identity);
			if (this.mexConfig != null)
			{
				foreach (AgentInfo agentInfo in this.mexConfig.GetPublicAgentList())
				{
					if (string.Compare(agentInfo.AgentName, strB, StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						agentInfo.Enabled = enabled;
						return;
					}
				}
			}
			base.WriteError(new ArgumentException(AgentStrings.AgentNotFound(identity), "Identity"), ErrorCategory.InvalidArgument, null);
		}

		private void SetMExConfigPath()
		{
			switch (this.TransportService)
			{
			case TransportService.Hub:
			case TransportService.Edge:
				this.mexConfigPath = Path.Combine(ConfigurationContext.Setup.InstallPath, "TransportRoles\\Shared\\agents.config");
				return;
			case TransportService.FrontEnd:
				this.mexConfigPath = Path.Combine(ConfigurationContext.Setup.InstallPath, "TransportRoles\\Shared\\fetagents.config");
				return;
			default:
				throw new InvalidOperationException(string.Format("TransportService is set to a value that is not supported: {0}", this.TransportService));
			}
		}

		internal string GetTransportServiceName()
		{
			switch (this.TransportService)
			{
			case TransportService.Hub:
			case TransportService.Edge:
				return "MSExchangeTransport";
			case TransportService.FrontEnd:
				return "MSExchangeFrontEndTransport";
			default:
				throw new InvalidOperationException(string.Format("TransportService is set to a value that is not supported: {0}", this.TransportService));
			}
		}

		internal MExConfiguration MExConfiguration
		{
			get
			{
				return this.mexConfig;
			}
		}

		internal string MExConfigPath
		{
			get
			{
				return this.mexConfigPath;
			}
		}

		internal bool MissingConfigFile
		{
			get
			{
				return this.missingConfigFile;
			}
		}

		private const string MSExchangeTransportServiceName = "MSExchangeTransport";

		private const string MSExchangeFrontEndTransportServiceName = "MSExchangeFrontEndTransport";

		private MExConfiguration mexConfig;

		private bool missingConfigFile;

		private string mexConfigPath;
	}
}
