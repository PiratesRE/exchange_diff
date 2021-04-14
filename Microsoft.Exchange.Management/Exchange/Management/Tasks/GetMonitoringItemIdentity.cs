using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring.Management.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Get", "MonitoringItemIdentity")]
	public sealed class GetMonitoringItemIdentity : Task
	{
		[Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[ValidateNotNullOrEmpty]
		public string Identity
		{
			get
			{
				return (string)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public ServerIdParameter Server
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

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				List<RpcGetMonitoringItemIdentity.RpcMonitorItemIdentity> list = null;
				LocalizedException ex = null;
				try
				{
					list = RpcGetMonitoringItemIdentity.Invoke(this.Server.Fqdn, this.Identity, 900000);
				}
				catch (ActiveMonitoringServerException ex2)
				{
					ex = ex2;
				}
				catch (ActiveMonitoringServerTransientException ex3)
				{
					ex = ex3;
				}
				if (ex != null)
				{
					this.WriteWarning(ex.LocalizedString);
				}
				if (list != null)
				{
					foreach (RpcGetMonitoringItemIdentity.RpcMonitorItemIdentity rpcIdentity in list)
					{
						MonitoringItemIdentity sendToPipeline = new MonitoringItemIdentity(this.Server.Fqdn, rpcIdentity);
						base.WriteObject(sendToPipeline);
					}
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		private ServerIdParameter serverId;
	}
}
