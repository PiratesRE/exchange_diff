using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring.Management.Common;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Get", "MonitoringItemHelp")]
	public sealed class GetMonitoringItemHelp : Task
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

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				if (!MonitoringItemIdentity.MonitorIdentityId.IsValidFormat(this.Identity))
				{
					base.WriteError(new ArgumentException(Strings.InvalidMonitorIdentity(this.Identity)), (ErrorCategory)1000, null);
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				List<PropertyInformation> list = null;
				LocalizedException ex = null;
				try
				{
					list = RpcGetMonitoringItemHelp.Invoke(this.Server.Fqdn, this.Identity, 900000);
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
				foreach (PropertyInformation propertyInfo in list)
				{
					MonitorPropertyInformation sendToPipeline = new MonitorPropertyInformation(this.Server.Fqdn, propertyInfo);
					base.WriteObject(sendToPipeline);
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
