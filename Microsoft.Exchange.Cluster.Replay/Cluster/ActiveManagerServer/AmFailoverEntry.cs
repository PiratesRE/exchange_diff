using System;
using System.Threading;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmFailoverEntry
	{
		internal AmDbActionReason ReasonCode { get; set; }

		internal AmServerName ServerName { get; set; }

		internal ExDateTime TimeCreated { get; set; }

		internal TimeSpan Delay { get; set; }

		internal Timer Timer { get; set; }

		private AmFailoverEntry()
		{
		}

		private static string GetPersistentStateKeyName(AmServerName serverName)
		{
			return "SystemState\\" + serverName.NetbiosName + "\\DeferredFailover";
		}

		internal AmFailoverEntry(AmDbActionReason reasonCode, AmServerName serverName)
		{
			this.ReasonCode = reasonCode;
			this.ServerName = serverName;
		}

		internal static AmFailoverEntry ReadFromPersistentStoreBestEffort(AmServerName serverName)
		{
			AmFailoverEntry entry = null;
			Exception ex = AmHelper.HandleKnownExceptions(delegate(object param0, EventArgs param1)
			{
				entry = AmFailoverEntry.ReadFromPersistentStore(serverName);
			});
			if (ex != null)
			{
				ReplayCrimsonEvents.TransientFailoverSuppressionPersistentStoreFailure.Log<string, AmServerName, string>("Read", serverName, ex.Message);
			}
			return entry;
		}

		internal static AmFailoverEntry ReadFromPersistentStore(AmServerName serverName)
		{
			AmFailoverEntry amFailoverEntry = null;
			using (AmPersistentClusdbState amPersistentClusdbState = new AmPersistentClusdbState(AmSystemManager.Instance.Config.DagConfig.Cluster, AmFailoverEntry.GetPersistentStateKeyName(serverName)))
			{
				bool flag = false;
				string text = amPersistentClusdbState.ReadProperty<string>("TimeCreated", out flag);
				if (string.IsNullOrEmpty(text))
				{
					text = ExDateTime.MinValue.ToString("o");
				}
				if (flag)
				{
					amFailoverEntry = new AmFailoverEntry();
					bool flag2;
					string value = amPersistentClusdbState.ReadProperty<string>("ReasonCode", out flag2);
					if (string.IsNullOrEmpty(value))
					{
						value = AmDbActionReason.NodeDown.ToString();
					}
					AmDbActionReason reasonCode;
					EnumUtility.TryParse<AmDbActionReason>(value, out reasonCode, AmDbActionReason.NodeDown, true);
					amFailoverEntry.ServerName = serverName;
					amFailoverEntry.TimeCreated = ExDateTime.Parse(ExTimeZone.CurrentTimeZone, text);
					amFailoverEntry.ReasonCode = reasonCode;
					amFailoverEntry.Delay = TimeSpan.FromSeconds((double)RegistryParameters.TransientFailoverSuppressionDelayInSec);
				}
			}
			return amFailoverEntry;
		}

		internal void WriteToPersistentStoreBestEffort()
		{
			Exception ex = AmHelper.HandleKnownExceptions(delegate(object param0, EventArgs param1)
			{
				this.WriteToPersistentStore();
			});
			if (ex != null)
			{
				ReplayCrimsonEvents.TransientFailoverSuppressionPersistentStoreFailure.Log<string, AmServerName, string>("Write", this.ServerName, ex.Message);
			}
		}

		internal void WriteToPersistentStore()
		{
			AmConfig config = AmSystemManager.Instance.Config;
			if (config != null && config.DagConfig != null)
			{
				using (AmPersistentClusdbState amPersistentClusdbState = new AmPersistentClusdbState(config.DagConfig.Cluster, AmFailoverEntry.GetPersistentStateKeyName(this.ServerName)))
				{
					amPersistentClusdbState.WriteProperty<string>("ReasonCode", this.ReasonCode.ToString());
					amPersistentClusdbState.WriteProperty<string>("TimeCreated", this.TimeCreated.ToString("o"));
					return;
				}
			}
			throw new AmServiceShuttingDownException();
		}

		internal void DeleteFromPersistentStoreBestEffort()
		{
			Exception ex = AmHelper.HandleKnownExceptions(delegate(object param0, EventArgs param1)
			{
				this.DeleteFromPersistentStore();
			});
			if (ex != null)
			{
				ReplayCrimsonEvents.TransientFailoverSuppressionPersistentStoreFailure.Log<string, AmServerName, string>("Delete", this.ServerName, ex.Message);
			}
		}

		internal void DeleteFromPersistentStore()
		{
			using (AmPersistentClusdbState amPersistentClusdbState = new AmPersistentClusdbState(AmSystemManager.Instance.Config.DagConfig.Cluster, AmFailoverEntry.GetPersistentStateKeyName(this.ServerName)))
			{
				amPersistentClusdbState.DeleteProperty("TimeCreated");
				amPersistentClusdbState.DeleteProperty("ReasonCode");
			}
		}
	}
}
