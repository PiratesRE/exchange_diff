using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal static class MonitoringServerManager
	{
		public static IObserverFactory ObserverFactory { get; internal set; } = new ObserverFactory();

		public static ISubjectFactory SubjectFactory { get; internal set; } = new SubjectFactory();

		public static bool TryAddDatabase(Guid mdbGuid)
		{
			string text = mdbGuid.ToString().ToLower();
			WTFDiagnostics.TraceFunction<string>(ExTraceGlobals.CommonComponentsTracer, MonitoringServerManager.traceContext, "Try add mailbox database {0} into monitoring list", text, null, "TryAddDatabase", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MonitoringServerManager.cs", 200);
			bool result;
			using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(MonitoringServerManager.MdbListRegistry))
			{
				registryKey.SetValue(text, MonitoringServerManager.ConstructStatusCode(DatabaseMonitoringStatus.Received));
				result = true;
			}
			return result;
		}

		public static void RemoveDatabase(Guid mdbGuid)
		{
			string text = mdbGuid.ToString().ToLower();
			WTFDiagnostics.TraceFunction<string>(ExTraceGlobals.CommonComponentsTracer, MonitoringServerManager.traceContext, "Removing mailbox database {0} from monitoring list", text, null, "RemoveDatabase", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MonitoringServerManager.cs", 224);
			using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(MonitoringServerManager.MdbListRegistry))
			{
				registryKey.DeleteValue(text, false);
			}
		}

		public static string[] GetAllDatabaseGuids()
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, MonitoringServerManager.traceContext, "Read all monitored mailbox database lists", null, "GetAllDatabaseGuids", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MonitoringServerManager.cs", 242);
			string[] valueNames;
			using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(MonitoringServerManager.MdbListRegistry))
			{
				valueNames = registryKey.GetValueNames();
			}
			return valueNames;
		}

		public static void SetDatabaseStatus(string name, DatabaseMonitoringStatus databaseMonitoringStatus)
		{
			WTFDiagnostics.TraceFunction<string, DatabaseMonitoringStatus>(ExTraceGlobals.CommonComponentsTracer, MonitoringServerManager.traceContext, "Update mailbox database {0} with monitoring status {1}", name, databaseMonitoringStatus, null, "SetDatabaseStatus", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MonitoringServerManager.cs", 260);
			using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(MonitoringServerManager.MdbListRegistry))
			{
				registryKey.SetValue(name, MonitoringServerManager.ConstructStatusCode(databaseMonitoringStatus));
			}
		}

		public static Dictionary<string, int> GetDatabaseStatusList()
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, MonitoringServerManager.traceContext, "Read all monitored mailbox databases with status", null, "GetDatabaseStatusList", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MonitoringServerManager.cs", 274);
			return MonitoringServerManager.GetDatabaseListHelper(MonitoringServerManager.MdbListRegistry);
		}

		private static Dictionary<string, int> GetDatabaseListHelper(string subkey)
		{
			Dictionary<string, int> result;
			using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(subkey))
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				foreach (string text in registryKey.GetValueNames())
				{
					object value = registryKey.GetValue(text);
					if (value is int)
					{
						dictionary[text] = (int)value;
					}
				}
				result = dictionary;
			}
			return result;
		}

		public static bool TryAddObserver(string server)
		{
			WTFDiagnostics.TraceFunction<string>(ExTraceGlobals.CommonComponentsTracer, MonitoringServerManager.traceContext, "Trying to add server {0} into observers list", server, null, "TryAddObserver", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MonitoringServerManager.cs", 311);
			return MonitoringServerManager.TryAddServer(server, MonitoringServerManager.ObserverList, MonitoringServerManager.MaxObservers);
		}

		public static void RemoveObserver(string server)
		{
			WTFDiagnostics.TraceFunction<string>(ExTraceGlobals.CommonComponentsTracer, MonitoringServerManager.traceContext, "Removing server {0} from observers list", server, null, "RemoveObserver", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MonitoringServerManager.cs", 322);
			MonitoringServerManager.RemoveServer(server, MonitoringServerManager.ObserverList);
		}

		public static string[] GetAllObservers()
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, MonitoringServerManager.traceContext, "Read observers list", null, "GetAllObservers", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MonitoringServerManager.cs", 333);
			return MonitoringServerManager.GetAllServers(MonitoringServerManager.ObserverList);
		}

		public static bool TryAddSubject(string server)
		{
			WTFDiagnostics.TraceFunction<string>(ExTraceGlobals.CommonComponentsTracer, MonitoringServerManager.traceContext, "Trying to add server {0} into subjects list", server, null, "TryAddSubject", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MonitoringServerManager.cs", 345);
			ISubject subject = MonitoringServerManager.SubjectFactory.CreateSubject(NativeHelpers.GetLocalComputerFqdn(true));
			if (!subject.IsInMaintenance)
			{
				return MonitoringServerManager.TryAddServer(server, MonitoringServerManager.SubjectList, MonitoringServerManager.MaxSubjects);
			}
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, MonitoringServerManager.traceContext, "Cannot add server {0} into list because this machine is in maintenance.", server, null, "TryAddSubject", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MonitoringServerManager.cs", 356);
			return false;
		}

		public static void RemoveSubject(string server)
		{
			WTFDiagnostics.TraceFunction<string>(ExTraceGlobals.CommonComponentsTracer, MonitoringServerManager.traceContext, "Removing server {0} from subjects list", server, null, "RemoveSubject", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MonitoringServerManager.cs", 368);
			MonitoringServerManager.RemoveServer(server, MonitoringServerManager.SubjectList);
		}

		public static string[] GetAllSubjects()
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, MonitoringServerManager.traceContext, "Read subjects list", null, "GetAllSubjects", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MonitoringServerManager.cs", 379);
			return MonitoringServerManager.GetAllServers(MonitoringServerManager.SubjectList);
		}

		public static ObserverHeartbeatResponse UpdateObserverHeartbeat(string observer)
		{
			WTFDiagnostics.TraceFunction<string>(ExTraceGlobals.CommonComponentsTracer, MonitoringServerManager.traceContext, "Update heartbeat from observer {0}", observer, null, "UpdateObserverHeartbeat", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MonitoringServerManager.cs", 391);
			bool flag = false;
			ObserverHeartbeatResponse result;
			try
			{
				try
				{
					flag = MonitoringServerManager.registryMutex.WaitOne();
				}
				catch (AbandonedMutexException)
				{
					flag = true;
				}
				using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(MonitoringServerManager.ObserverList))
				{
					if (registryKey.GetValue(observer) == null)
					{
						WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, MonitoringServerManager.traceContext, "Invalid request from unknown observer {0}", observer, null, "UpdateObserverHeartbeat", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MonitoringServerManager.cs", 411);
						result = ObserverHeartbeatResponse.UnknownObserver;
					}
					else
					{
						registryKey.SetValue(observer, DateTime.UtcNow.ToString());
						result = ObserverHeartbeatResponse.Success;
					}
				}
			}
			finally
			{
				if (flag)
				{
					MonitoringServerManager.registryMutex.ReleaseMutex();
				}
			}
			return result;
		}

		public static DateTime? GetObserverHeartbeat(string observer)
		{
			WTFDiagnostics.TraceFunction<string>(ExTraceGlobals.CommonComponentsTracer, MonitoringServerManager.traceContext, "Looking for heartbeat timestamp from observer {0}", observer, null, "GetObserverHeartbeat", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MonitoringServerManager.cs", 438);
			bool flag = false;
			DateTime? result;
			try
			{
				try
				{
					flag = MonitoringServerManager.registryMutex.WaitOne();
				}
				catch (AbandonedMutexException)
				{
					flag = true;
				}
				using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(MonitoringServerManager.ObserverList))
				{
					object value = registryKey.GetValue(observer);
					DateTime value2;
					if (value == null)
					{
						WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, MonitoringServerManager.traceContext, "No heartbeat timestamp found for observer {0}", observer, null, "GetObserverHeartbeat", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MonitoringServerManager.cs", 458);
					}
					else if (DateTime.TryParse(value as string, out value2))
					{
						return new DateTime?(value2);
					}
					result = null;
				}
			}
			finally
			{
				if (flag)
				{
					MonitoringServerManager.registryMutex.ReleaseMutex();
				}
			}
			return result;
		}

		public static DateTime? GetLastObserverSelectionTimestamp()
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, MonitoringServerManager.traceContext, "Looking for timestamp of last maintenance run", null, "GetLastObserverSelectionTimestamp", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MonitoringServerManager.cs", 488);
			bool flag = false;
			DateTime? result;
			try
			{
				try
				{
					flag = MonitoringServerManager.registryMutex.WaitOne();
				}
				catch (AbandonedMutexException)
				{
					flag = true;
				}
				using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(MonitoringServerManager.RegistryPathBase))
				{
					object value = registryKey.GetValue("ObserverSelection");
					DateTime value2;
					if (value == null)
					{
						WTFDiagnostics.TraceInformation(ExTraceGlobals.CommonComponentsTracer, MonitoringServerManager.traceContext, "Timestamp of last maintenance run not found", null, "GetLastObserverSelectionTimestamp", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MonitoringServerManager.cs", 508);
					}
					else if (DateTime.TryParse(value as string, out value2))
					{
						return new DateTime?(value2);
					}
					result = null;
				}
			}
			finally
			{
				if (flag)
				{
					MonitoringServerManager.registryMutex.ReleaseMutex();
				}
			}
			return result;
		}

		public static void UpdateLastObserverSelectionTimestamp()
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, MonitoringServerManager.traceContext, "Update last observer selection timestamp", null, "UpdateLastObserverSelectionTimestamp", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MonitoringServerManager.cs", 537);
			bool flag = false;
			try
			{
				try
				{
					flag = MonitoringServerManager.registryMutex.WaitOne();
				}
				catch (AbandonedMutexException)
				{
					flag = true;
				}
				using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(MonitoringServerManager.RegistryPathBase))
				{
					registryKey.SetValue("ObserverSelection", DateTime.UtcNow.ToString());
				}
			}
			finally
			{
				if (flag)
				{
					MonitoringServerManager.registryMutex.ReleaseMutex();
				}
			}
		}

		internal static DatabaseMonitoringStatus DecodeStatus(int statusCode, out DateTime time)
		{
			DatabaseMonitoringStatus result = (DatabaseMonitoringStatus)(statusCode & 15);
			int num = statusCode >> 4;
			time = MonitoringServerManager.StartTime.AddMinutes((double)num);
			return result;
		}

		internal static bool TryAddServer(string server, string serverList, int maxServers)
		{
			WTFDiagnostics.TraceFunction<string>(ExTraceGlobals.CommonComponentsTracer, MonitoringServerManager.traceContext, "Trying to add server {0} into list", server, null, "TryAddServer", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MonitoringServerManager.cs", 591);
			bool flag = false;
			bool result;
			try
			{
				try
				{
					flag = MonitoringServerManager.registryMutex.WaitOne();
				}
				catch (AbandonedMutexException)
				{
					flag = true;
				}
				using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(serverList))
				{
					if (registryKey.ValueCount >= maxServers)
					{
						WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, MonitoringServerManager.traceContext, "Cannot add server {0} into list because list is full", server, null, "TryAddServer", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MonitoringServerManager.cs", 610);
						result = false;
					}
					else if (Array.Exists<string>(MonitoringServerManager.GetAllServers(serverList), (string newServer) => server == newServer))
					{
						WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, MonitoringServerManager.traceContext, "Server {0} is already in the list; reporting success even though we no-op'ed", server, null, "TryAddServer", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MonitoringServerManager.cs", 620);
						result = true;
					}
					else
					{
						registryKey.SetValue(server, DateTime.UtcNow.ToString());
						result = true;
					}
				}
			}
			finally
			{
				if (flag)
				{
					MonitoringServerManager.registryMutex.ReleaseMutex();
				}
			}
			return result;
		}

		internal static void RemoveServer(string server, string serverList)
		{
			WTFDiagnostics.TraceFunction<string>(ExTraceGlobals.CommonComponentsTracer, MonitoringServerManager.traceContext, "Removing server {0} from list", server, null, "RemoveServer", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MonitoringServerManager.cs", 645);
			bool flag = false;
			try
			{
				try
				{
					flag = MonitoringServerManager.registryMutex.WaitOne();
				}
				catch (AbandonedMutexException)
				{
					flag = true;
				}
				using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(serverList))
				{
					registryKey.DeleteValue(server, false);
				}
			}
			finally
			{
				if (flag)
				{
					MonitoringServerManager.registryMutex.ReleaseMutex();
				}
			}
		}

		internal static string[] GetAllServers(string serverList)
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, MonitoringServerManager.traceContext, "Read servers list", null, "GetAllServers", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MonitoringServerManager.cs", 681);
			bool flag = false;
			string[] valueNames;
			try
			{
				try
				{
					flag = MonitoringServerManager.registryMutex.WaitOne();
				}
				catch (AbandonedMutexException)
				{
					flag = true;
				}
				using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(serverList))
				{
					valueNames = registryKey.GetValueNames();
				}
			}
			finally
			{
				if (flag)
				{
					MonitoringServerManager.registryMutex.ReleaseMutex();
				}
			}
			return valueNames;
		}

		internal static bool IsSameServer(string owner, string p)
		{
			if (owner == null && p == null)
			{
				return true;
			}
			if (owner != null ^ p != null)
			{
				return false;
			}
			string[] array = owner.Split(new char[]
			{
				'.'
			});
			string[] array2 = p.Split(new char[]
			{
				'.'
			});
			return string.Compare(array[0], array2[0], true) == 0;
		}

		private static int ConstructStatusCode(DatabaseMonitoringStatus status)
		{
			int num = (int)(DateTime.UtcNow - MonitoringServerManager.StartTime).TotalMinutes;
			return num << 4 | (int)status;
		}

		private const string ObserverSelectionName = "ObserverSelection";

		private const string HeartbeatName = "Heartbeat";

		private const string FirstHeartbeatName = "FirstHeartbeat";

		private const string OwnerName = "Owner";

		private const string MutexName = "MSExchangeHMMonitoringServerManager";

		internal static readonly DateTime StartTime = new DateTime(2012, 7, 12, 0, 0, 0, DateTimeKind.Utc);

		internal static readonly int MaxObservers = Settings.MaxObservers;

		internal static readonly int MaxSubjects = Settings.MaxSubjects;

		internal static readonly int MaxRequestObservers = Settings.MaxRequestObservers;

		private static readonly string RegistryPathBase = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\";

		private static readonly string MdbListRegistry = MonitoringServerManager.RegistryPathBase + "MdbList";

		private static readonly string ObserverList = MonitoringServerManager.RegistryPathBase + "Observers\\";

		private static readonly string SubjectList = MonitoringServerManager.RegistryPathBase + "Subjects\\";

		private static TracingContext traceContext = TracingContext.Default;

		private static Mutex registryMutex = new Mutex(false, "MSExchangeHMMonitoringServerManager");
	}
}
