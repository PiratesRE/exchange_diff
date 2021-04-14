using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring.Management.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[OutputType(new Type[]
	{
		typeof(MonitorHealthEntry)
	})]
	[Cmdlet("Get", "ServerHealth")]
	public sealed class GetServerHealth : GetServerHealthBase
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				this.monitorHealthCommon = new MonitorHealthCommon((!string.IsNullOrWhiteSpace(base.Identity.Fqdn)) ? base.Identity.Fqdn : base.Identity.ToString(), base.HealthSet, base.HaImpactingOnly);
				LocalizedException ex = null;
				List<MonitorHealthEntry> monitorHealthEntries = this.monitorHealthCommon.GetMonitorHealthEntries(out ex);
				if (ex != null)
				{
					this.WriteWarning(ex.LocalizedString);
				}
				if (monitorHealthEntries != null)
				{
					foreach (MonitorHealthEntry sendToPipeline in monitorHealthEntries)
					{
						base.WriteObject(sendToPipeline);
					}
				}
			}
			catch (Exception exception)
			{
				base.WriteError(exception, (ErrorCategory)1000, null);
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		private MonitorHealthCommon monitorHealthCommon;
	}
}
