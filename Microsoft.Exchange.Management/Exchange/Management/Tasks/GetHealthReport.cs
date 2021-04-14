using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring.Management.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Get", "HealthReport")]
	[OutputType(new Type[]
	{
		typeof(ConsolidatedHealth)
	})]
	public sealed class GetHealthReport : GetServerHealthBase
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter RollupGroup
		{
			get
			{
				return (SwitchParameter)(base.Fields["RollupGroup"] ?? false);
			}
			set
			{
				base.Fields["RollupGroup"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int GroupSize
		{
			get
			{
				return this.monitorHealthCommon.GroupSize;
			}
			set
			{
				this.monitorHealthCommon.GroupSize = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MinimumOnlinePercent
		{
			get
			{
				return this.monitorHealthCommon.MinimumOnlinePercent;
			}
			set
			{
				this.monitorHealthCommon.MinimumOnlinePercent = value;
			}
		}

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
				this.monitorHealthCommon.SetServerHealthMap(monitorHealthEntries);
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override void InternalEndProcessing()
		{
			if (this.monitorHealthCommon.ServerHealthMap.Count > 0)
			{
				if (this.RollupGroup)
				{
					this.ProcessRollupByGroup();
				}
				else
				{
					this.ProcessRollupByServerHealthSet();
				}
			}
			base.InternalEndProcessing();
		}

		private void ProcessRollupByServerHealthSet()
		{
			List<ConsolidatedHealth> consolidateHealthEntries = this.monitorHealthCommon.GetConsolidateHealthEntries();
			if (consolidateHealthEntries != null)
			{
				foreach (ConsolidatedHealth sendToPipeline in consolidateHealthEntries)
				{
					base.WriteObject(sendToPipeline);
				}
			}
		}

		private void ProcessRollupByGroup()
		{
			Dictionary<string, Dictionary<string, ConsolidatedHealth>> dictionary = new Dictionary<string, Dictionary<string, ConsolidatedHealth>>();
			IEnumerable<string> keys = this.monitorHealthCommon.ServerHealthMap.Keys;
			List<ConsolidatedHealth> consolidateHealthEntries = this.monitorHealthCommon.GetConsolidateHealthEntries();
			if (consolidateHealthEntries != null)
			{
				foreach (ConsolidatedHealth consolidatedHealth in consolidateHealthEntries)
				{
					Dictionary<string, ConsolidatedHealth> dictionary2 = null;
					if (!dictionary.TryGetValue(consolidatedHealth.HealthSet, out dictionary2))
					{
						dictionary2 = new Dictionary<string, ConsolidatedHealth>();
						foreach (string key in keys)
						{
							dictionary2[key] = null;
						}
						dictionary[consolidatedHealth.HealthSet] = dictionary2;
					}
					dictionary2[consolidatedHealth.Server] = consolidatedHealth;
				}
			}
			foreach (KeyValuePair<string, Dictionary<string, ConsolidatedHealth>> keyValuePair in dictionary)
			{
				Dictionary<string, ConsolidatedHealth> value = keyValuePair.Value;
				ConsolidatedHealth sendToPipeline = this.monitorHealthCommon.ConsolidateAcrossServers(value);
				base.WriteObject(sendToPipeline);
			}
		}

		private MonitorHealthCommon monitorHealthCommon;
	}
}
