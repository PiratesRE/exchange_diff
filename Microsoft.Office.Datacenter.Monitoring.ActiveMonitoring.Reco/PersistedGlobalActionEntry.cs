using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	[Serializable]
	public class PersistedGlobalActionEntry
	{
		public PersistedGlobalActionEntry()
		{
		}

		public PersistedGlobalActionEntry(RecoveryActionEntry entry)
		{
			this.Id = entry.Id;
			this.ResourceName = entry.ResourceName;
			this.RequestorName = entry.RequestorName;
			this.StartTime = entry.StartTime;
			this.ExpectedEndTime = entry.EndTime;
			this.Context = entry.Context;
			this.InstanceId = entry.InstanceId;
			this.LamProcessStartTime = entry.LamProcessStartTime;
			this.ReportedTime = ExDateTime.Now.LocalTime;
			this.ThrottleIdentity = entry.ThrottleIdentity;
			this.ThrottleParametersXml = entry.ThrottleParametersXml;
		}

		public RecoveryActionId Id { get; set; }

		public string ResourceName { get; set; }

		public string RequestorName { get; set; }

		public DateTime StartTime { get; set; }

		public DateTime ExpectedEndTime { get; set; }

		public string InstanceId { get; set; }

		public string Context { get; set; }

		public DateTime LamProcessStartTime { get; set; }

		public string FinishEntryContext { get; set; }

		public DateTime ReportedTime { get; set; }

		public string ThrottleIdentity { get; set; }

		public string ThrottleParametersXml { get; set; }

		public static PersistedGlobalActionEntry ReadFromFile(RecoveryActionId id)
		{
			string fileName = PersistedGlobalActionEntry.GetFileName(id);
			PersistedGlobalActionEntry result = null;
			if (File.Exists(fileName))
			{
				try
				{
					result = Utilities.DeserializeObjectFromFile<PersistedGlobalActionEntry>(fileName);
				}
				catch (Exception ex)
				{
					ManagedAvailabilityCrimsonEvents.ActiveMonitoringUnexpectedError.Log<string, string>(string.Format("PersistedGlobalActionEntry.DeserializeObjectFromFile failed for {0}", fileName), ex.Message);
				}
			}
			return result;
		}

		public bool WriteToFile(TimeSpan timeout)
		{
			Task task = Task.Factory.StartNew(new Action(this.WriteToFile));
			return task.Wait(timeout);
		}

		public void WriteToFile()
		{
			string fileName = PersistedGlobalActionEntry.GetFileName(this.Id);
			try
			{
				Utilities.SerializeObjectToFile(this, fileName);
			}
			catch (Exception ex)
			{
				ManagedAvailabilityCrimsonEvents.ActiveMonitoringUnexpectedError.Log<string, string>(string.Format("PersistedGlobalActionEntry.SerializeObjectToFile failed for {0}", fileName), ex.Message);
			}
		}

		public RecoveryActionEntry ConvertToRecoveryActionStartEntry()
		{
			return RecoveryActionHelper.ConstructStartActionEntry(this.Id, this.ResourceName, this.RequestorName, this.StartTime, this.ExpectedEndTime, this.ThrottleIdentity, this.ThrottleParametersXml, this.Context, this.InstanceId, this.LamProcessStartTime);
		}

		internal static string GetFileName(RecoveryActionId id)
		{
			return Path.Combine(ExchangeSetupContext.InstallPath, string.Format("Logging\\Monitoring\\PersistedGlobalRecoveryAction\\{0}.xml", id));
		}
	}
}
