using System;
using System.Management.Automation;
using Microsoft.Exchange.Cluster.Replay.Monitoring;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "MailboxServerRedundancy", DefaultParameterSetName = "Identity")]
	[OutputType(new Type[]
	{
		typeof(ServerRedundancy)
	})]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class GetMailboxServerRedundancy : GetRedundancyTaskBase<ServerIdParameter, Server>
	{
		[Alias(new string[]
		{
			"Server"
		})]
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "Identity", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public override ServerIdParameter Identity
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

		protected override ADObjectId LookupIdentityObjectAndGetDagId()
		{
			Server server = base.LookupServer(this.Identity);
			ADObjectId databaseAvailabilityGroup = server.DatabaseAvailabilityGroup;
			if (databaseAvailabilityGroup == null)
			{
				base.WriteError(new ServerMustBeInDagException(server.Fqdn), ErrorCategory.InvalidData, this.Identity);
				return null;
			}
			return databaseAvailabilityGroup;
		}

		protected override void WriteResultsFromHealthInfo(HealthInfoPersisted hip, string serverContactedFqdn)
		{
			bool flag = false;
			foreach (ServerHealthInfoPersisted serverHealthInfoPersisted in hip.Servers)
			{
				if (this.Identity == null || serverHealthInfoPersisted.ServerFqdn.IndexOf(this.Identity.RawIdentity, StringComparison.InvariantCultureIgnoreCase) >= 0)
				{
					flag = true;
					ServerRedundancy dataObject = new ServerRedundancy(hip, serverHealthInfoPersisted, serverContactedFqdn);
					this.WriteResult(dataObject);
				}
			}
			if (!flag)
			{
				this.WriteWarning(Strings.GetDagHealthInfoNoResultsWarning);
			}
		}
	}
}
