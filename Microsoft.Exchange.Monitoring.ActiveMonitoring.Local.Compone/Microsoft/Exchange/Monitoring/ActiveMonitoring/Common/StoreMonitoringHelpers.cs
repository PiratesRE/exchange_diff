using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal static class StoreMonitoringHelpers
	{
		internal static string GetStoreUsageStatisticsFilePath()
		{
			string path = string.Format("StoreUsageStatisticsData-{0}-{1}.csv", DateTime.UtcNow.ToString("yy-MM-dd-HHmmssfff"), Environment.MachineName.ToLower());
			string text = Path.Combine(ExchangeSetupContext.InstallPath, "Diagnostics", "StoreUsageStatistics");
			string result = Path.Combine(text, path);
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return result;
		}

		internal static CorrelatedMonitorInfo GetStoreCorrelation(string databaseName)
		{
			return StoreMonitoringHelpers.GetStoreCorrelation(databaseName, new string[]
			{
				typeof(MapiExceptionMdbOffline).FullName,
				typeof(MapiExceptionNetworkError).FullName,
				typeof(UnableToFindServerForDatabaseException).FullName
			});
		}

		internal static CorrelatedMonitorInfo GetStoreCorrelation(string databaseName, string[] exceptionTypes)
		{
			return new CorrelatedMonitorInfo(string.Format("{0}\\{1}\\{2}", ExchangeComponent.Store.Name, "ActiveDatabaseAvailabilityMonitor", (!string.IsNullOrWhiteSpace(databaseName)) ? databaseName : "*"), StoreMonitoringHelpers.GetRegExMatchFromException(exceptionTypes), CorrelatedMonitorInfo.MatchMode.RegEx);
		}

		internal static ProbeDefinition GetProbeDefinition(string serverName, string probeName, string targetResource, string serviceName)
		{
			List<ProbeDefinition> definitionsFromCrimson = StoreMonitoringHelpers.GetDefinitionsFromCrimson<ProbeDefinition>(serverName);
			foreach (ProbeDefinition probeDefinition in definitionsFromCrimson)
			{
				if (!string.IsNullOrWhiteSpace(probeDefinition.ServiceName) && probeDefinition.ServiceName.Equals(serviceName, StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrWhiteSpace(probeDefinition.Name) && probeDefinition.Name.Equals(probeName, StringComparison.InvariantCultureIgnoreCase) && probeDefinition.TargetResource.Equals(targetResource, StringComparison.InvariantCultureIgnoreCase))
				{
					return probeDefinition;
				}
			}
			return null;
		}

		internal static string PopulateEscalationMessage(string escalationMessage, ProbeResult probeResult)
		{
			ResponseMessageReader responseMessageReader = new ResponseMessageReader();
			responseMessageReader.AddObject<ProbeResult>("Probe", probeResult);
			return responseMessageReader.ReplaceValues(escalationMessage);
		}

		private static List<TDefinition> GetDefinitionsFromCrimson<TDefinition>(string serverName) where TDefinition : WorkDefinition, IPersistence, new()
		{
			List<TDefinition> list = new List<TDefinition>();
			using (CrimsonReader<TDefinition> crimsonReader = new CrimsonReader<TDefinition>())
			{
				crimsonReader.ConnectionInfo = new CrimsonConnectionInfo(serverName);
				foreach (TDefinition item in crimsonReader.ReadAll())
				{
					list.Add(item);
				}
			}
			return list;
		}

		internal static string GetRegExMatchFromException(string[] exceptionTypes)
		{
			if (exceptionTypes != null && exceptionTypes.Length > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < exceptionTypes.Length; i++)
				{
					string text = Regex.Escape(exceptionTypes[i]);
					if (i == 0)
					{
						stringBuilder.Append(text);
					}
					else
					{
						stringBuilder.AppendFormat("|{0}", text);
					}
				}
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		internal static Process[] GetStoreWorkerProcess(string databaseGuid)
		{
			if (!string.IsNullOrWhiteSpace(databaseGuid))
			{
				using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT CommandLine, ProcessId FROM Win32_Process WHERE Name = 'Microsoft.Exchange.Store.Worker.exe'"))
				{
					foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
					{
						ManagementObject managementObject = (ManagementObject)managementBaseObject;
						if (managementObject["CommandLine"].ToString().IndexOf(databaseGuid, 0, StringComparison.OrdinalIgnoreCase) != -1)
						{
							int num = Convert.ToInt32(managementObject["ProcessId"]);
							if (num != 0)
							{
								return new Process[]
								{
									Process.GetProcessById(num)
								};
							}
						}
					}
				}
			}
			return new Process[0];
		}

		internal const string StoreWorker = "Microsoft.Exchange.Store.Worker.exe";
	}
}
