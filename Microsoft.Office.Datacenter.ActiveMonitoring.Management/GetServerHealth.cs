using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring.Management.Common;

namespace Microsoft.Office.Datacenter.ActiveMonitoring.Management
{
	[OutputType(new Type[]
	{
		typeof(MonitorHealthEntry)
	})]
	[Cmdlet("Get", "ServerHealth")]
	public sealed class GetServerHealth : GetHealthBase
	{
		protected override void ProcessRecord()
		{
			try
			{
				this.monitorHealthCommon = new MonitorHealthCommon(base.Identity, base.HealthSet, base.HaImpactingOnly);
				base.ProcessRecord();
				LocalizedException ex = null;
				List<MonitorHealthEntry> monitorHealthEntries = this.monitorHealthCommon.GetMonitorHealthEntries(out ex);
				if (ex != null)
				{
					base.WriteWarning(ex.LocalizedString);
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
				base.WriteError(new ErrorRecord(exception, string.Empty, ErrorCategory.CloseError, null));
			}
		}

		private MonitorHealthCommon monitorHealthCommon;
	}
}
