using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Tasks
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	public class ManageRoutingGroupMember : SetupTaskBase
	{
		[Parameter(Mandatory = false)]
		public string ServerName
		{
			get
			{
				return (string)base.Fields["ServerName"];
			}
			set
			{
				base.Fields["ServerName"] = value;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (string.IsNullOrEmpty(this.ServerName))
			{
				this.server = ((ITopologyConfigurationSession)this.configurationSession).FindLocalServer();
			}
			else
			{
				this.server = ((ITopologyConfigurationSession)this.configurationSession).FindServerByName(this.ServerName);
			}
			if (this.server == null)
			{
				base.ThrowTerminatingError(new CouldNotFindServerDirectoryEntryException(string.IsNullOrEmpty(this.ServerName) ? NativeHelpers.GetLocalComputerFqdn(false) : this.ServerName), ErrorCategory.ObjectNotFound, null);
			}
			base.LogReadObject(this.server);
			if (this.add)
			{
				this.routingGroup = ((ITopologyConfigurationSession)this.configurationSession).GetRoutingGroup();
			}
			else if (this.server.HomeRoutingGroup != null)
			{
				IConfigurable configurable = this.configurationSession.Read<RoutingGroup>(this.server.HomeRoutingGroup);
				this.routingGroup = (RoutingGroup)configurable;
			}
			TaskLogger.LogExit();
		}

		protected Server server;

		protected RoutingGroup routingGroup;

		protected bool add = true;
	}
}
