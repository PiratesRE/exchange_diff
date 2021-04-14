using System;
using System.IO;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Monitoring;
using Microsoft.Exchange.Provisioning;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "ServerComponentState", SupportsShouldProcess = true)]
	public sealed class SetServerComponentState : Task
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetComponentState(this.Component, this.State.ToString());
			}
		}

		[Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public ServerIdParameter Identity
		{
			get
			{
				return this.serverId;
			}
			set
			{
				this.serverId = value;
			}
		}

		[Parameter(Mandatory = true)]
		[ValidateNotNullOrEmpty]
		public string Component
		{
			get
			{
				return (string)base.Fields["Component"];
			}
			set
			{
				base.Fields["Component"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public ServiceState State
		{
			get
			{
				return (ServiceState)base.Fields["State"];
			}
			set
			{
				base.Fields["State"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string Requester
		{
			get
			{
				return (string)base.Fields["Requester"];
			}
			set
			{
				base.Fields["Requester"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Fqdn DomainController
		{
			get
			{
				return this.domainController;
			}
			set
			{
				this.domainController = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter RemoteOnly
		{
			get
			{
				return (SwitchParameter)(base.Fields["RemoteOnly"] ?? false);
			}
			set
			{
				base.Fields["RemoteOnly"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter LocalOnly
		{
			get
			{
				return (SwitchParameter)(base.Fields["LocalOnly"] ?? false);
			}
			set
			{
				base.Fields["LocalOnly"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int TimeoutInSeconds
		{
			get
			{
				return (int)(base.Fields["TimeoutInSeconds"] ?? 120);
			}
			set
			{
				base.Fields["TimeoutInSeconds"] = value;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.ProvisioningHandlers != null)
			{
				foreach (ProvisioningHandler provisioningHandler in base.ProvisioningHandlers)
				{
					provisioningHandler.Validate(this.serverObject);
				}
			}
			if (!ServerComponentStateManager.IsValidComponent(this.Component))
			{
				base.WriteError(new ArgumentException(Strings.ServerComponentStateInvalidComponentName(this.Component)), ErrorCategory.InvalidArgument, null);
			}
			if (this.LocalOnly)
			{
				ServerComponentEnum serverComponentEnum;
				Enum.TryParse<ServerComponentEnum>(this.Component, true, out serverComponentEnum);
				if (serverComponentEnum == ServerComponentEnum.Monitoring || serverComponentEnum == ServerComponentEnum.RecoveryActionsEnabled)
				{
					base.WriteError(new ArgumentException(Strings.ServerComponentStateNoLocalOnly(this.Component)), ErrorCategory.InvalidArgument, null);
				}
			}
			ServerComponentRequest serverComponentRequest;
			if (!Enum.TryParse<ServerComponentRequest>(this.Requester, true, out serverComponentRequest))
			{
				string allowedRequesters = string.Join(",", Enum.GetNames(typeof(ServerComponentRequest)));
				base.WriteError(new ArgumentException(Strings.ServerComponentStateInvalidRequester(this.Requester, allowedRequesters)), ErrorCategory.InvalidArgument, null);
			}
			if (this.LocalOnly && this.RemoteOnly)
			{
				base.WriteError(new ArgumentException(Strings.SetServerComponentStateInvalidLocalRemoteSwitch), ErrorCategory.InvalidArgument, null);
			}
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			ADComputer adcomputer = null;
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(this.DomainController, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 213, "InternalProcessRecord", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\ComponentStates\\SetServerComponentState.cs");
			string fqdn = this.serverId.Fqdn;
			string text = fqdn;
			int num = fqdn.IndexOf('.');
			if (num > 0)
			{
				text = fqdn.Substring(0, num);
			}
			Server server = topologyConfigurationSession.FindServerByName(text);
			if (server == null)
			{
				topologyConfigurationSession.UseConfigNC = false;
				topologyConfigurationSession.UseGlobalCatalog = true;
				adcomputer = topologyConfigurationSession.FindComputerByHostName(text);
				if (adcomputer == null)
				{
					base.WriteError(new ADServerNotFoundException(fqdn), ErrorCategory.InvalidArgument, null);
				}
			}
			if (!this.LocalOnly)
			{
				if (server != null)
				{
					server.ComponentStates = ServerComponentStates.UpdateRemoteState(server.ComponentStates, this.Requester, this.Component, this.State);
					topologyConfigurationSession.Save(server);
				}
				else
				{
					adcomputer.ComponentStates = ServerComponentStates.UpdateRemoteState(adcomputer.ComponentStates, this.Requester, this.Component, this.State);
					topologyConfigurationSession.Save(adcomputer);
				}
			}
			if (!this.RemoteOnly)
			{
				string serverFqdn = (server != null) ? server.Fqdn : adcomputer.DnsHostName;
				TimeSpan invokeTimeout = TimeSpan.FromSeconds((double)this.TimeoutInSeconds);
				Exception ex = null;
				try
				{
					InvokeWithTimeout.Invoke(delegate()
					{
						ServerComponentStates.UpdateLocalState(serverFqdn, this.Requester, this.Component, this.State);
					}, null, invokeTimeout, true, null);
				}
				catch (IOException ex2)
				{
					ex = ex2;
				}
				catch (UnauthorizedAccessException ex3)
				{
					ex = ex3;
				}
				catch (SecurityException ex4)
				{
					ex = ex4;
				}
				if (ex != null && this.LocalOnly)
				{
					base.WriteError(new ArgumentException(Strings.SetServerComponentStateServerUnreachable(serverFqdn, ex.Message)), ErrorCategory.ResourceUnavailable, null);
				}
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || exception is IOException;
		}

		private ServerIdParameter serverId;

		private Fqdn domainController;

		private ServerComponentStatePresentationObject serverObject = new ServerComponentStatePresentationObject();
	}
}
