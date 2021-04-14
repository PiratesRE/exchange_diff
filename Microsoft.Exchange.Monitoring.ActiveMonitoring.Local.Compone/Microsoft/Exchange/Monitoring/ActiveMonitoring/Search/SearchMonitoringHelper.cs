using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Management.Search;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Search.Responders;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Exchange.Search.Core;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Search
{
	internal static class SearchMonitoringHelper
	{
		static SearchMonitoringHelper()
		{
			MonitoringLogConfiguration configuration = new MonitoringLogConfiguration(ExchangeComponent.Search.Name);
			SearchMonitoringHelper.monitoringLogger = new MonitoringLogger(configuration);
			configuration = new MonitoringLogConfiguration(ExchangeComponent.Search.Name, "CopyStatusChange");
			SearchMonitoringHelper.copyStatusLogger = new MonitoringLogger(configuration);
			configuration = new MonitoringLogConfiguration(ExchangeComponent.Search.Name, "RecoveryActions");
			SearchMonitoringHelper.recoveryActionsLogger = new MonitoringLogger(configuration);
			configuration = new MonitoringLogConfiguration(ExchangeComponent.Search.Name, "NodeStateChange");
			SearchMonitoringHelper.nodeStateLogger = new MonitoringLogger(configuration);
		}

		internal static DiagnosticInfo DiagnosticInfo
		{
			get
			{
				return SearchMonitoringHelper.diagnosticInfoCache;
			}
		}

		internal static ProbeResult GetLastProbeResult(ProbeWorkItem probe, IProbeWorkBroker broker, CancellationToken cancellationToken)
		{
			ProbeResult lastProbeResult = null;
			if (broker != null)
			{
				IOrderedEnumerable<ProbeResult> query = from r in broker.GetProbeResults(probe.Definition, probe.Result.ExecutionStartTime.AddSeconds((double)(-5 * probe.Definition.RecurrenceIntervalSeconds)))
				orderby r.ExecutionStartTime descending
				select r;
				Task<int> task = broker.AsDataAccessQuery<ProbeResult>(query).ExecuteAsync(delegate(ProbeResult r)
				{
					if (lastProbeResult == null)
					{
						lastProbeResult = r;
					}
				}, cancellationToken, SearchMonitoringHelper.traceContext);
				task.Wait(cancellationToken);
				return lastProbeResult;
			}
			if (ExEnvironment.IsTest)
			{
				return null;
			}
			throw new ArgumentNullException("broker");
		}

		internal static CopyStatusClientCachedEntry GetCachedLocalDatabaseCopyStatus(string databaseName)
		{
			if (string.IsNullOrWhiteSpace(databaseName))
			{
				throw new ArgumentException("databaseName");
			}
			Guid databaseGuidFromName = SearchMonitoringHelper.GetDatabaseGuidFromName(databaseName);
			CopyStatusClientCachedEntry result;
			try
			{
				CopyStatusClientCachedEntry dbCopyStatusOnLocalServer = CachedDbStatusReader.Instance.GetDbCopyStatusOnLocalServer(databaseGuidFromName);
				if (dbCopyStatusOnLocalServer == null)
				{
					SearchMonitoringHelper.LogInfo("GetDbCopyStatusOnLocalServer() for database '{0}' returns null.", new object[]
					{
						databaseGuidFromName
					});
				}
				result = dbCopyStatusOnLocalServer;
			}
			catch (Exception ex)
			{
				SearchMonitoringHelper.LogInfo("Exception caught calling GetDbCopyStatusOnLocalServer() for database '{0}': '{1}'.", new object[]
				{
					databaseGuidFromName,
					ex.Message
				});
				result = null;
			}
			return result;
		}

		internal static List<CopyStatusClientCachedEntry> GetCachedDatabaseCopyStatus(string databaseName)
		{
			if (string.IsNullOrWhiteSpace(databaseName))
			{
				throw new ArgumentException("databaseName");
			}
			Guid databaseGuidFromName = SearchMonitoringHelper.GetDatabaseGuidFromName(databaseName);
			List<CopyStatusClientCachedEntry> result;
			try
			{
				List<CopyStatusClientCachedEntry> allCopyStatusesForDatabase = CachedDbStatusReader.Instance.GetAllCopyStatusesForDatabase(databaseGuidFromName);
				result = allCopyStatusesForDatabase;
			}
			catch (Exception ex)
			{
				SearchMonitoringHelper.LogInfo("Exception caught calling GetAllCopyStatusesForDatabase() for database '{0}': '{1}'.", new object[]
				{
					databaseGuidFromName,
					ex.Message
				});
				result = null;
			}
			return result;
		}

		internal static IndexStatus GetCachedLocalDatabaseIndexStatus(Guid databaseGuid, bool throwOnIndexStatusException = true)
		{
			if (SearchMonitoringHelper.indexStatusCache == null)
			{
				lock (SearchMonitoringHelper.indexStatusCacheTimeoutTimes)
				{
					if (SearchMonitoringHelper.indexStatusCache == null)
					{
						SearchMonitoringHelper.indexStatusCache = new Dictionary<Guid, IndexStatus>();
					}
				}
			}
			lock (SearchMonitoringHelper.indexStatusCache)
			{
				if (SearchMonitoringHelper.indexStatusCache.ContainsKey(databaseGuid) && SearchMonitoringHelper.indexStatusCacheTimeoutTimes[databaseGuid] > DateTime.UtcNow)
				{
					return SearchMonitoringHelper.indexStatusCache[databaseGuid];
				}
			}
			IndexStatus indexStatus = null;
			try
			{
				indexStatus = IndexStatusStore.Instance.GetIndexStatus(databaseGuid);
			}
			catch (IndexStatusException)
			{
				if (throwOnIndexStatusException)
				{
					throw;
				}
			}
			if (indexStatus != null)
			{
				lock (SearchMonitoringHelper.indexStatusCache)
				{
					SearchMonitoringHelper.indexStatusCacheTimeoutTimes[databaseGuid] = DateTime.UtcNow + SearchMonitoringHelper.IndexStatusCacheTimeout;
					SearchMonitoringHelper.indexStatusCache[databaseGuid] = indexStatus;
				}
			}
			return indexStatus;
		}

		internal static IndexStatus GetCachedLocalDatabaseIndexStatus(string databaseName, bool throwOnIndexStatusException = true)
		{
			if (string.IsNullOrWhiteSpace(databaseName))
			{
				throw new ArgumentException("databaseName");
			}
			Guid mailboxDatabaseGuid = SearchMonitoringHelper.GetDatabaseInfo(databaseName).MailboxDatabaseGuid;
			return SearchMonitoringHelper.GetCachedLocalDatabaseIndexStatus(mailboxDatabaseGuid, throwOnIndexStatusException);
		}

		internal static MailboxDatabaseInfo GetDatabaseInfo(string databaseName)
		{
			if (string.IsNullOrWhiteSpace(databaseName))
			{
				throw new ArgumentException("databaseName");
			}
			lock (SearchMonitoringHelper.databaseInfoDict)
			{
				if (SearchMonitoringHelper.databaseInfoDict.ContainsKey(databaseName))
				{
					return SearchMonitoringHelper.databaseInfoDict[databaseName];
				}
			}
			ICollection<MailboxDatabaseInfo> mailboxDatabaseInfoCollectionForBackend = LocalEndpointManager.Instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend;
			MailboxDatabaseInfo result;
			lock (SearchMonitoringHelper.databaseInfoDict)
			{
				if (!SearchMonitoringHelper.databaseInfoDict.ContainsKey(databaseName))
				{
					SearchMonitoringHelper.databaseInfoDict.Clear();
					foreach (MailboxDatabaseInfo mailboxDatabaseInfo in mailboxDatabaseInfoCollectionForBackend)
					{
						SearchMonitoringHelper.databaseInfoDict.Add(mailboxDatabaseInfo.MailboxDatabaseName, mailboxDatabaseInfo);
					}
				}
				if (SearchMonitoringHelper.databaseInfoDict.ContainsKey(databaseName))
				{
					result = SearchMonitoringHelper.databaseInfoDict[databaseName];
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		internal static Guid GetDatabaseGuidFromName(string databaseName)
		{
			if (string.IsNullOrWhiteSpace(databaseName))
			{
				throw new ArgumentException("databaseName");
			}
			MailboxDatabaseInfo databaseInfo = SearchMonitoringHelper.GetDatabaseInfo(databaseName);
			if (databaseInfo == null)
			{
				throw new ArgumentException("databaseName");
			}
			return databaseInfo.MailboxDatabaseGuid;
		}

		internal static bool IsCatalogDisabled(string databaseName)
		{
			if (string.IsNullOrWhiteSpace(databaseName))
			{
				throw new ArgumentException("databaseName");
			}
			IndexStatus cachedLocalDatabaseIndexStatus = SearchMonitoringHelper.GetCachedLocalDatabaseIndexStatus(databaseName, true);
			return cachedLocalDatabaseIndexStatus.IndexingState == ContentIndexStatusType.Disabled;
		}

		internal static bool IsCatalogSeeding(string databaseName)
		{
			if (string.IsNullOrWhiteSpace(databaseName))
			{
				throw new ArgumentException("databaseName");
			}
			IndexStatus cachedLocalDatabaseIndexStatus = SearchMonitoringHelper.GetCachedLocalDatabaseIndexStatus(databaseName, true);
			return cachedLocalDatabaseIndexStatus.IndexingState == ContentIndexStatusType.Seeding;
		}

		internal static bool IsDatabaseActive(string databaseName)
		{
			if (string.IsNullOrWhiteSpace(databaseName))
			{
				throw new ArgumentException("databaseName");
			}
			Guid mailboxDatabaseGuid = SearchMonitoringHelper.GetDatabaseInfo(databaseName).MailboxDatabaseGuid;
			return SearchMonitoringHelper.IsDatabaseActive(mailboxDatabaseGuid);
		}

		internal static bool IsDatabaseActive(Guid databaseGuid)
		{
			return DirectoryAccessor.Instance.IsDatabaseCopyActiveOnLocalServer(databaseGuid);
		}

		internal static Dictionary<string, int> GetNodeProcessIds()
		{
			NoderunnerResourceHelper noderunnerResourceHelper = new NoderunnerResourceHelper();
			return noderunnerResourceHelper.ProcessDictionary;
		}

		internal static Process[] GetProcessesForNodeRunner(string nodeRunnerInstanceName)
		{
			Dictionary<string, int> nodeProcessIds = SearchMonitoringHelper.GetNodeProcessIds();
			foreach (string text in nodeProcessIds.Keys)
			{
				if (nodeRunnerInstanceName.EndsWith(text, StringComparison.OrdinalIgnoreCase))
				{
					return new Process[]
					{
						Process.GetProcessById(nodeProcessIds[text])
					};
				}
			}
			return new Process[0];
		}

		internal static void CleanUpOrphanedWerProcesses()
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			string queryString = string.Format("SELECT ProcessId, ParentProcessId, CommandLine from Win32_Process WHERE Name LIKE \"WerFault.exe\" OR Name LIKE \"WerMgr.exe\"", new object[0]);
			using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(queryString))
			{
				using (ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get())
				{
					foreach (ManagementBaseObject managementBaseObject in managementObjectCollection)
					{
						ManagementObject managementObject = (ManagementObject)managementBaseObject;
						try
						{
							if (managementObject["CommandLine"] != null && managementObject["ProcessId"] != null && managementObject["ParentProcessId"] != null)
							{
								string text = managementObject["CommandLine"].ToString();
								int num = int.Parse(managementObject["ProcessId"].ToString());
								if (text.IndexOf("WerFault.exe", StringComparison.OrdinalIgnoreCase) >= 0)
								{
									Match match = Regex.Match(text, "-p ([0-9]+)", RegexOptions.IgnoreCase);
									if (match.Success)
									{
										SearchMonitoringHelper.LogInfo("CleanUpOrphanedWerProcesses: WerFault.exe process found with ID {0}, command line '{1}'.", new object[]
										{
											num,
											text
										});
										int value = int.Parse(match.Groups[1].Value);
										dictionary.Add(num, value);
									}
								}
								else if (text.IndexOf("WerMgr.exe", StringComparison.OrdinalIgnoreCase) >= 0)
								{
									int num2 = int.Parse(managementObject["ParentProcessId"].ToString());
									SearchMonitoringHelper.LogInfo("CleanUpOrphanedWerProcesses: WerMgr.exe process found with ID {0}, parent processID {1}.", new object[]
									{
										num,
										num2
									});
									dictionary.Add(num, num2);
								}
							}
						}
						finally
						{
							managementObject.Dispose();
						}
					}
				}
			}
			foreach (int num3 in dictionary.Keys)
			{
				int processId = dictionary[num3];
				using (Process processByIdBestEffort = ProcessHelper.GetProcessByIdBestEffort(processId))
				{
					if (processByIdBestEffort == null)
					{
						using (Process processByIdBestEffort2 = ProcessHelper.GetProcessByIdBestEffort(num3))
						{
							if (processByIdBestEffort2 != null)
							{
								SearchMonitoringHelper.LogInfo("CleanUpOrphanedWerProcesses: Orphaned WER process found with ID {0}. Killing.", new object[]
								{
									num3
								});
								ProcessHelper.KillProcess(processByIdBestEffort2, false, "SearchMonitoringHelper.CleanUpOrphanedWerProcesses()");
							}
						}
					}
				}
			}
		}

		internal static DateTime? GetRecentGracefulDegradationExecutionTime()
		{
			SearchMonitoringHelper.RefreshDiagnosticInfo();
			DateTime? recentGracefulDegradationExecutionTime;
			lock (SearchMonitoringHelper.diagnosticInfoCacheLock)
			{
				if (SearchMonitoringHelper.diagnosticInfoCache != null)
				{
					recentGracefulDegradationExecutionTime = SearchMonitoringHelper.diagnosticInfoCache.RecentGracefulDegradationExecutionTime;
				}
				else
				{
					if (SearchMonitoringHelper.getDiagnosticInfoException != null)
					{
						throw SearchMonitoringHelper.getDiagnosticInfoException;
					}
					throw new InvalidOperationException();
				}
			}
			return recentGracefulDegradationExecutionTime;
		}

		internal static DiagnosticInfo.FeedingControllerDiagnosticInfo GetCachedFeedingControllerDiagnosticInfo(string databaseName)
		{
			if (string.IsNullOrWhiteSpace(databaseName))
			{
				throw new ArgumentException("databaseName");
			}
			SearchMonitoringHelper.RefreshDiagnosticInfo();
			DiagnosticInfo.FeedingControllerDiagnosticInfo feedingControllerDiagnosticInfo;
			lock (SearchMonitoringHelper.diagnosticInfoCacheLock)
			{
				if (SearchMonitoringHelper.diagnosticInfoCache != null)
				{
					feedingControllerDiagnosticInfo = SearchMonitoringHelper.diagnosticInfoCache.GetFeedingControllerDiagnosticInfo(SearchMonitoringHelper.GetDatabaseGuidFromName(databaseName));
				}
				else
				{
					if (SearchMonitoringHelper.getDiagnosticInfoException != null)
					{
						throw SearchMonitoringHelper.getDiagnosticInfoException;
					}
					throw new InvalidOperationException();
				}
			}
			return feedingControllerDiagnosticInfo;
		}

		internal static long GetPerformanceCounterValue(string categoryName, string counterName, string instanceName)
		{
			string text = string.Join("\\", new string[]
			{
				categoryName,
				counterName,
				instanceName
			});
			PerformanceCounter performanceCounter = null;
			lock (SearchMonitoringHelper.perfCounters)
			{
				SearchMonitoringHelper.perfCounters.TryGetValue(text, out performanceCounter);
			}
			if (performanceCounter != null)
			{
				try
				{
					return performanceCounter.RawValue;
				}
				catch (Exception ex)
				{
					SearchMonitoringHelper.LogInfo("Fail to reuse the PerformanceCounter instance '{0}'. Exception: '{1}'.", new object[]
					{
						text,
						ex
					});
					try
					{
						performanceCounter.Dispose();
					}
					catch
					{
					}
				}
			}
			performanceCounter = new PerformanceCounter(categoryName, counterName, instanceName, true);
			lock (SearchMonitoringHelper.perfCounters)
			{
				SearchMonitoringHelper.perfCounters[text] = performanceCounter;
			}
			return performanceCounter.RawValue;
		}

		internal static List<EventRecord> GetEvents(string logName, int eventId, string providerName, int timePeriodSeconds, int maxCount, Func<EventRecord, bool> condition = null)
		{
			TimeSpan timeout = TimeSpan.FromSeconds(30.0);
			string query = string.Format("*[System[(EventID={0})] and System[Provider[@Name=\"{1}\"] and TimeCreated[timediff(@SystemTime) <= {2}]]]", eventId, providerName, timePeriodSeconds * 1000);
			List<EventRecord> list = new List<EventRecord>();
			using (EventLogReader eventLogReader = new EventLogReader(new EventLogQuery(logName, PathType.LogName, query)
			{
				ReverseDirection = true
			}))
			{
				EventRecord eventRecord = eventLogReader.ReadEvent(timeout);
				while (eventRecord != null)
				{
					if (condition != null && !condition(eventRecord))
					{
						eventRecord.Dispose();
					}
					else
					{
						list.Add(eventRecord);
						if (list.Count >= maxCount)
						{
							return list;
						}
						eventRecord = eventLogReader.ReadEvent(timeout);
					}
				}
			}
			return list;
		}

		internal static void SetNotificationServiceClass(ProbeResult result, NotificationServiceClass notificationServiceClass)
		{
			result.StateAttribute22 = notificationServiceClass.ToString();
		}

		private static void RefreshDiagnosticInfo()
		{
			lock (SearchMonitoringHelper.diagnosticInfoCacheLock)
			{
				if (DateTime.UtcNow > SearchMonitoringHelper.diagnosticInfoCacheTimeoutTime)
				{
					DiagnosticInfo diagnosticInformation = null;
					Exception ex = null;
					Action delegateGetDiagnosticInfo = delegate()
					{
						try
						{
							diagnosticInformation = new DiagnosticInfo(Environment.MachineName);
						}
						catch (Exception ex)
						{
							ex = ex;
						}
					};
					IAsyncResult asyncResult = delegateGetDiagnosticInfo.BeginInvoke(delegate(IAsyncResult r)
					{
						delegateGetDiagnosticInfo.EndInvoke(r);
					}, null);
					if (!asyncResult.AsyncWaitHandle.WaitOne(SearchMonitoringHelper.GetDiagnosticInfoCallTimeout))
					{
						ex = new TimeoutException(Strings.SearchGetDiagnosticInfoTimeout((int)SearchMonitoringHelper.GetDiagnosticInfoCallTimeout.TotalSeconds));
					}
					SearchMonitoringHelper.diagnosticInfoCacheTimeoutTime = DateTime.UtcNow + SearchMonitoringHelper.DiagnosticInfoCacheTimeout;
					SearchMonitoringHelper.diagnosticInfoCache = diagnosticInformation;
					SearchMonitoringHelper.getDiagnosticInfoException = ex;
				}
			}
		}

		internal static ResponderDefinition CreateRestartSearchServiceResponderDefinition(MaintenanceWorkItem maintenanceWorkItem, MonitorDefinition monitorDefinition, ServiceHealthStatus healthState, bool enabled)
		{
			int serviceStopTimeoutInSeconds = int.Parse(maintenanceWorkItem.Definition.Attributes["RestartSearchServiceStopTimeoutInSeconds"]);
			string responderName = SearchStrings.SearchRestartServiceResponderName(monitorDefinition.Name);
			string monitorName = monitorDefinition.ConstructWorkItemResultName();
			ResponderDefinition responderDefinition = RestartServiceResponder.CreateDefinition(responderName, monitorName, SearchDiscovery.ServiceName, healthState, serviceStopTimeoutInSeconds, 120, 0, false, DumpMode.None, null, 15.0, 0, "Exchange", null, true, true, null, false);
			responderDefinition.ServiceName = ExchangeComponent.Search.Name;
			responderDefinition.TargetResource = monitorDefinition.TargetResource;
			responderDefinition.Enabled = enabled;
			responderDefinition.RecurrenceIntervalSeconds = 0;
			return responderDefinition;
		}

		internal static ResponderDefinition CreateRestartHostControllerServiceResponderDefinition(MaintenanceWorkItem maintenanceWorkItem, MonitorDefinition monitorDefinition, ServiceHealthStatus healthState, bool enabled)
		{
			int serviceStopTimeoutInSeconds = int.Parse(maintenanceWorkItem.Definition.Attributes["RestartHostControllerServiceStopTimeoutInSeconds"]);
			string responderName = SearchStrings.SearchRestartHostControllerServiceResponderName(monitorDefinition.Name);
			string alertMask = monitorDefinition.ConstructWorkItemResultName();
			ResponderDefinition responderDefinition = RestartHostControllerServiceResponder2.CreateDefinition(responderName, alertMask, healthState, serviceStopTimeoutInSeconds, 120, 0, DumpMode.None, null, 15.0, 0, "Dag");
			responderDefinition.TargetResource = monitorDefinition.TargetResource;
			responderDefinition.Enabled = enabled;
			return responderDefinition;
		}

		internal static ResponderDefinition CreateRestartNodeResponderDefinition(MaintenanceWorkItem maintenanceWorkItem, MonitorDefinition monitorDefinition, string nodeNames, ServiceHealthStatus healthState, bool enabled)
		{
			int nodeStopTimeoutInSeconds = int.Parse(maintenanceWorkItem.Definition.Attributes["RestartNodeStopTimeoutInSeconds"]);
			string responderName = SearchStrings.HostControllerServiceRestartNodeResponderName(monitorDefinition.Name);
			string alertMask = monitorDefinition.ConstructWorkItemResultName();
			ResponderDefinition responderDefinition = RestartNodeResponder.CreateDefinition(responderName, alertMask, healthState, nodeNames, nodeStopTimeoutInSeconds, "Dag");
			responderDefinition.TargetResource = monitorDefinition.TargetResource;
			responderDefinition.Enabled = enabled;
			return responderDefinition;
		}

		internal static ResponderDefinition CreateDatabaseFailoverResponderDefinition(MaintenanceWorkItem maintenanceWorkItem, MonitorDefinition monitorDefinition, ServiceHealthStatus healthState, bool enabled)
		{
			string targetResource = monitorDefinition.TargetResource;
			Guid mailboxDatabaseGuid = SearchMonitoringHelper.GetDatabaseInfo(targetResource).MailboxDatabaseGuid;
			string alertMask = monitorDefinition.ConstructWorkItemResultName();
			ResponderDefinition responderDefinition = DatabaseFailoverResponder.CreateDefinition(SearchStrings.SearchDatabaseFailoverResponderName(monitorDefinition.Name), ExchangeComponent.Search.Name, "*", alertMask, targetResource, ExchangeComponent.Search.Name, mailboxDatabaseGuid, healthState);
			responderDefinition.TypeName = typeof(SearchDatabaseFailoverResponder).FullName;
			responderDefinition.RecurrenceIntervalSeconds = 0;
			responderDefinition.Enabled = enabled;
			return responderDefinition;
		}

		internal static ResponderDefinition CreateEscalateResponderDefinition(MonitorDefinition monitorDefinition, string escalationMessage, bool enabled, ServiceHealthStatus serviceHealthStatus = ServiceHealthStatus.None, SearchEscalateResponder.EscalateModes escalateMode = SearchEscalateResponder.EscalateModes.Scheduled, bool urgentInTraining = true)
		{
			string name = SearchStrings.SearchEscalateResponderName(monitorDefinition.Name);
			string text = monitorDefinition.ConstructWorkItemResultName();
			string text2 = escalationMessage.Split(new char[]
			{
				'.'
			})[0];
			if (LocalEndpointManager.IsDataCenter)
			{
				text2 = string.Format("({0}) - {1}", text, text2);
			}
			ResponderDefinition responderDefinition = SearchEscalateResponder.CreateDefinition(name, ExchangeComponent.Search.Name, monitorDefinition.Name, text, monitorDefinition.TargetResource, serviceHealthStatus, SearchMonitoringHelper.escalationTeam, text2, escalationMessage, escalateMode, urgentInTraining);
			responderDefinition.Enabled = enabled;
			return responderDefinition;
		}

		internal static MonitorDefinition CreateOverallConsecutiveProbeFailuresMonitorDefinition(string monitorName, string sampleMask, string targetResource, int recurrenceIntervalSeconds, int monitoringThreshold, int monitoringInterval, bool enabled)
		{
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(monitorName, sampleMask, ExchangeComponent.Search.Name, ExchangeComponent.Search, monitoringThreshold, true, 300);
			monitorDefinition.TargetResource = targetResource;
			monitorDefinition.RecurrenceIntervalSeconds = 0;
			monitorDefinition.MonitoringIntervalSeconds = monitoringInterval;
			monitorDefinition.Enabled = enabled;
			return monitorDefinition;
		}

		internal static MonitorDefinition CreateMonitorDefinition(string monitorName, Type monitorType, string sampleMask, string targetResource, int recurrenceIntervalSeconds, int monitoringIntervalSeconds, int monitoringThreshold, bool enabled)
		{
			MonitorDefinition monitorDefinition = new MonitorDefinition();
			monitorDefinition.Name = monitorName;
			monitorDefinition.AssemblyPath = monitorType.Assembly.Location;
			monitorDefinition.TypeName = monitorType.FullName;
			monitorDefinition.SampleMask = sampleMask;
			monitorDefinition.ServiceName = ExchangeComponent.Search.Name;
			monitorDefinition.TargetResource = targetResource;
			monitorDefinition.Component = ExchangeComponent.Search;
			monitorDefinition.RecurrenceIntervalSeconds = 0;
			monitorDefinition.InsufficientSamplesIntervalSeconds = Math.Max(5 * monitorDefinition.RecurrenceIntervalSeconds, Convert.ToInt32(ConfigurationManager.AppSettings["InsufficientSamplesIntervalInSeconds"]));
			monitorDefinition.TimeoutSeconds = recurrenceIntervalSeconds;
			monitorDefinition.MonitoringIntervalSeconds = monitoringIntervalSeconds;
			monitorDefinition.MonitoringThreshold = (double)monitoringThreshold;
			monitorDefinition.Enabled = enabled;
			return monitorDefinition;
		}

		internal static ProbeDefinition CreateProbeDefinition(string probeName, Type probeType, string targetResource, int recurrenceIntervalSeconds, bool enabled)
		{
			ProbeDefinition probeDefinition = new ProbeDefinition();
			probeDefinition.AssemblyPath = probeType.Assembly.Location;
			probeDefinition.TypeName = probeType.FullName;
			probeDefinition.Name = probeName;
			probeDefinition.RecurrenceIntervalSeconds = recurrenceIntervalSeconds;
			probeDefinition.TimeoutSeconds = recurrenceIntervalSeconds;
			probeDefinition.MaxRetryAttempts = 3;
			probeDefinition.TargetResource = targetResource;
			probeDefinition.ServiceName = ExchangeComponent.Search.Name;
			probeDefinition.Enabled = enabled;
			WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.SearchTracer, SearchMonitoringHelper.traceContext, "SearchDiscovery.CreateProbeDefinition: Created ProbeDefinition '{0}' for '{1}'.", probeName, targetResource, null, "CreateProbeDefinition", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Search\\SearchMonitoringHelper.cs", 1151);
			return probeDefinition;
		}

		internal static void CreateResponderChainForMonitor(MaintenanceWorkItem maintenanceWorkItem, string settingPrefix, MonitorDefinition monitorDefinition, string escalationMessage, bool enabled, bool databaseFailoverNeeded = false, bool restartSearchServiceNeeded = false, bool restartNodesNeeded = false, bool restartHostControllerServiceNeeded = false, SearchEscalateResponder.EscalateModes escalateMode = SearchEscalateResponder.EscalateModes.Scheduled, bool urgentInTraining = true)
		{
			AttributeHelper attributeHelper = new AttributeHelper(maintenanceWorkItem.Definition);
			ServiceHealthStatus[] array = new ServiceHealthStatus[]
			{
				ServiceHealthStatus.Unhealthy,
				ServiceHealthStatus.Unhealthy1,
				ServiceHealthStatus.Unhealthy2
			};
			int num = 0;
			List<MonitorStateTransition> list = new List<MonitorStateTransition>();
			if (restartSearchServiceNeeded)
			{
				ServiceHealthStatus serviceHealthStatus = array[num];
				num++;
				int transitionTimeoutSeconds = 0;
				list.Add(new MonitorStateTransition(serviceHealthStatus, transitionTimeoutSeconds));
				bool @bool = attributeHelper.GetBool(settingPrefix + "RestartSearchServiceResponderEnabled", true, true);
				ResponderDefinition definition = SearchMonitoringHelper.CreateRestartSearchServiceResponderDefinition(maintenanceWorkItem, monitorDefinition, serviceHealthStatus, enabled && @bool);
				maintenanceWorkItem.Broker.AddWorkDefinition<ResponderDefinition>(definition, SearchMonitoringHelper.traceContext);
			}
			if (restartNodesNeeded)
			{
				ServiceHealthStatus serviceHealthStatus2 = array[num];
				num++;
				int transitionTimeoutSeconds2 = 0;
				if (serviceHealthStatus2 != ServiceHealthStatus.Unhealthy)
				{
					transitionTimeoutSeconds2 = attributeHelper.GetInt(string.Concat(new object[]
					{
						settingPrefix,
						"Monitor",
						serviceHealthStatus2,
						"StateSeconds"
					}), true, 0, null, null);
				}
				list.Add(new MonitorStateTransition(serviceHealthStatus2, transitionTimeoutSeconds2));
				bool bool2 = attributeHelper.GetBool(settingPrefix + "RestartNodesResponderEnabled", true, true);
				string @string = attributeHelper.GetString(settingPrefix + "RestartNodesNodeNames", false, string.Empty);
				ResponderDefinition definition2 = SearchMonitoringHelper.CreateRestartNodeResponderDefinition(maintenanceWorkItem, monitorDefinition, @string, serviceHealthStatus2, enabled && bool2);
				maintenanceWorkItem.Broker.AddWorkDefinition<ResponderDefinition>(definition2, SearchMonitoringHelper.traceContext);
			}
			if (restartHostControllerServiceNeeded)
			{
				ServiceHealthStatus serviceHealthStatus3 = array[num];
				num++;
				int transitionTimeoutSeconds3 = 0;
				if (serviceHealthStatus3 != ServiceHealthStatus.Unhealthy)
				{
					transitionTimeoutSeconds3 = attributeHelper.GetInt(string.Concat(new object[]
					{
						settingPrefix,
						"Monitor",
						serviceHealthStatus3,
						"StateSeconds"
					}), true, 0, null, null);
				}
				list.Add(new MonitorStateTransition(serviceHealthStatus3, transitionTimeoutSeconds3));
				bool bool3 = attributeHelper.GetBool(settingPrefix + "RestartHostControllerServiceResponderEnabled", true, true);
				ResponderDefinition definition3 = SearchMonitoringHelper.CreateRestartHostControllerServiceResponderDefinition(maintenanceWorkItem, monitorDefinition, serviceHealthStatus3, enabled && bool3);
				maintenanceWorkItem.Broker.AddWorkDefinition<ResponderDefinition>(definition3, SearchMonitoringHelper.traceContext);
			}
			if (databaseFailoverNeeded)
			{
				int transitionTimeoutSeconds4 = 0;
				if (num > 0)
				{
					transitionTimeoutSeconds4 = attributeHelper.GetInt(settingPrefix + "MonitorUnrecoverable1StateSeconds", true, 0, null, null);
				}
				list.Add(new MonitorStateTransition(ServiceHealthStatus.Unrecoverable1, transitionTimeoutSeconds4));
				bool bool4 = attributeHelper.GetBool(settingPrefix + "DatabaseFailoverResponderEnabled", true, true);
				ResponderDefinition definition4 = SearchMonitoringHelper.CreateDatabaseFailoverResponderDefinition(maintenanceWorkItem, monitorDefinition, ServiceHealthStatus.Unrecoverable1, enabled && bool4);
				maintenanceWorkItem.Broker.AddWorkDefinition<ResponderDefinition>(definition4, SearchMonitoringHelper.traceContext);
			}
			int @int = attributeHelper.GetInt(settingPrefix + "MonitorUnrecoverableStateSeconds", true, 0, null, null);
			list.Add(new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, @int));
			ResponderDefinition definition5 = SearchMonitoringHelper.CreateEscalateResponderDefinition(monitorDefinition, escalationMessage, enabled, ServiceHealthStatus.Unrecoverable, escalateMode, urgentInTraining);
			maintenanceWorkItem.Broker.AddWorkDefinition<ResponderDefinition>(definition5, SearchMonitoringHelper.traceContext);
			monitorDefinition.MonitorStateTransitions = list.ToArray();
		}

		internal static void SetDiagnosticDefaults()
		{
			DiagnosticsSessionFactory.SetDefaults(Guid.Parse("a07f37cc-a2b2-4d4e-8dc6-8e198e8fa976"), "MSExchangeFastSearch", "Search Diagnostics Logs", Path.Combine(ExchangeSetupContext.InstallPath, "Logging\\Search"), "SearchMonitoring_", "SearchMonitoringLogs");
		}

		internal static void LogInfo(string message, params object[] messageArgs)
		{
			SearchMonitoringHelper.monitoringLogger.LogEvent(DateTime.UtcNow, message, messageArgs);
		}

		internal static void LogInfo(WorkItem workItem, string message, params object[] messageArgs)
		{
			SearchMonitoringHelper.monitoringLogger.LogEvent(DateTime.UtcNow, string.Format("{0}/{1}: ", workItem.Definition.Name, workItem.Definition.TargetResource) + message, messageArgs);
		}

		internal static void LogRecoveryAction(WorkItem workItem, string message, params object[] messageArgs)
		{
			SearchMonitoringHelper.recoveryActionsLogger.LogEvent(DateTime.UtcNow, string.Format("{0}/{1}: ", workItem.Definition.Name, workItem.Definition.TargetResource) + message, messageArgs);
		}

		internal static void LogStatusChange(string message, params object[] messageArgs)
		{
			SearchMonitoringHelper.copyStatusLogger.LogEvent(DateTime.UtcNow, message, messageArgs);
		}

		internal static void LogNodeStateChange(string message, params object[] messageArgs)
		{
			SearchMonitoringHelper.nodeStateLogger.LogEvent(DateTime.UtcNow, message, messageArgs);
		}

		internal static TimeSpan GetSystemUpTime()
		{
			if (SearchMonitoringHelper.machineBootTime == null)
			{
				using (PerformanceCounter performanceCounter = new PerformanceCounter("System", "System Up Time"))
				{
					performanceCounter.NextValue();
					double num = (double)performanceCounter.NextValue();
					SearchMonitoringHelper.machineBootTime = new DateTime?(DateTime.UtcNow.AddSeconds(-num));
				}
			}
			return DateTime.UtcNow - SearchMonitoringHelper.machineBootTime.Value;
		}

		internal static string GetAllLocalDatabaseCopyStatusString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (MailboxDatabaseInfo mailboxDatabaseInfo in LocalEndpointManager.Instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend)
			{
				CopyStatusClientCachedEntry cachedLocalDatabaseCopyStatus = SearchMonitoringHelper.GetCachedLocalDatabaseCopyStatus(mailboxDatabaseInfo.MailboxDatabaseName);
				if (cachedLocalDatabaseCopyStatus != null && cachedLocalDatabaseCopyStatus.CopyStatus != null)
				{
					RpcDatabaseCopyStatus2 copyStatus = cachedLocalDatabaseCopyStatus.CopyStatus;
					if (copyStatus != null)
					{
						stringBuilder.AppendLine(Strings.SearchIndexCopyStatus(mailboxDatabaseInfo.MailboxDatabaseName, copyStatus.CopyStatus.ToString(), copyStatus.ContentIndexStatus.ToString(), copyStatus.ContentIndexErrorMessage));
					}
				}
			}
			if (stringBuilder.Length > 0)
			{
				return stringBuilder.ToString();
			}
			return Strings.SearchInformationNotAvailable.ToString();
		}

		internal static string GetAllCopyStatusForDatabaseString(string databaseName)
		{
			StringBuilder stringBuilder = new StringBuilder();
			List<CopyStatusClientCachedEntry> cachedDatabaseCopyStatus = SearchMonitoringHelper.GetCachedDatabaseCopyStatus(databaseName);
			if (cachedDatabaseCopyStatus != null)
			{
				foreach (CopyStatusClientCachedEntry copyStatusClientCachedEntry in cachedDatabaseCopyStatus)
				{
					if (copyStatusClientCachedEntry.Result != CopyStatusRpcResult.Success)
					{
						string copyName = string.Format("{0}\\{1}", databaseName, copyStatusClientCachedEntry.ServerContacted.NetbiosName);
						stringBuilder.AppendLine(Strings.SearchIndexCopyStatusError(copyName, copyStatusClientCachedEntry.Result.ToString(), (copyStatusClientCachedEntry.LastException != null) ? copyStatusClientCachedEntry.LastException.Message : string.Empty));
					}
					else
					{
						RpcDatabaseCopyStatus2 copyStatus = copyStatusClientCachedEntry.CopyStatus;
						string copyName = string.Format("{0}\\{1}", copyStatus.DBName, copyStatus.MailboxServer);
						stringBuilder.AppendLine(Strings.SearchIndexCopyStatus(copyName, copyStatus.CopyStatus.ToString(), copyStatus.ContentIndexStatus.ToString(), copyStatus.ContentIndexErrorMessage));
					}
				}
			}
			if (stringBuilder.Length > 0)
			{
				return stringBuilder.ToString();
			}
			return Strings.SearchInformationNotAvailable.ToString();
		}

		internal static bool IsInMaintenance()
		{
			Server server = LocalServer.GetServer();
			return DirectoryAccessor.Instance.IsRecoveryActionsEnabledOffline(server.Name);
		}

		internal const string NumberOfDocumentsIndexedCrawlerCounterName = "Crawler: Items Processed";

		internal const string StorePerDatabasePerformanceCountersCategoryName = "MSExchangeIS Store";

		internal const string TotalSearchesCounterName = "Total searches";

		internal const string TotalSuccessfulSearchesCounterName = "Total number of successful search queries";

		internal const string TotalSearchesQueriesGreaterThan60SecondsCounterName = "Total search queries completed in > 60 sec";

		internal const string IndexAgentCountersCategoryName = "MSExchangeSearch Transport CTS Flow";

		internal const string IndexAgentCountersInstanceName = "EdgeTransport";

		internal const string NumberOfFailedDocumentsCounterName = "Number Of Failed Documents";

		internal const string NumberOfProcessedDocumentsCounterName = "Number Of Processed Documents";

		internal const string SearchContentProcessingCategoryName = "Search Content Processing";

		internal const string SearchContentProcessingInstanceName = "ContentEngineNode1";

		internal const string CompletedCallbacksTotalCounterName = "# Completed Callbacks Total";

		internal const string SearchHostControllerCategoryName = "Search Host Controller";

		internal const string ComponentRestartsCounterName = "Component Restarts";

		internal const string SearchHostControllerCounterInstanceSuffix = " Fsis";

		internal const string FailedCallbacksTotalCounterName = "# Failed Callbacks Total";

		internal const int CmdletMaxRetry = 2;

		internal const int CmdletRetryIntervalSeconds = 1;

		internal const int MaxRetryAttempt = 3;

		internal static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		internal static readonly TimeSpan GetDiagnosticInfoCallTimeout = TimeSpan.FromMinutes(2.0);

		private static readonly TimeSpan CopyStatusCacheTimeout = TimeSpan.FromMinutes(2.0);

		private static readonly TimeSpan IndexStatusCacheTimeout = TimeSpan.FromMinutes(1.0);

		private static readonly TimeSpan DiagnosticInfoCacheTimeout = TimeSpan.FromMinutes(1.0);

		private static readonly TracingContext traceContext = TracingContext.Default;

		private static readonly object diagnosticInfoCacheLock = new object();

		private static string escalationTeam = "Search";

		private static Dictionary<Guid, IndexStatus> indexStatusCache;

		private static Dictionary<Guid, DateTime> indexStatusCacheTimeoutTimes = new Dictionary<Guid, DateTime>();

		private static DiagnosticInfo diagnosticInfoCache;

		private static DateTime diagnosticInfoCacheTimeoutTime = DateTime.MinValue;

		private static Exception getDiagnosticInfoException;

		private static Dictionary<string, MailboxDatabaseInfo> databaseInfoDict = new Dictionary<string, MailboxDatabaseInfo>(StringComparer.OrdinalIgnoreCase);

		private static Dictionary<string, PerformanceCounter> perfCounters = new Dictionary<string, PerformanceCounter>(StringComparer.OrdinalIgnoreCase);

		private static MonitoringLogger monitoringLogger;

		private static MonitoringLogger copyStatusLogger;

		private static MonitoringLogger recoveryActionsLogger;

		private static MonitoringLogger nodeStateLogger;

		private static DateTime? machineBootTime;

		internal static class FastNodeNames
		{
			internal static bool IsNodeNameValid(string nodeName)
			{
				return nodeName == "IndexNode1" || nodeName == "AdminNode1" || nodeName == "ContentEngineNode1" || nodeName == "InteractionEngineNode1";
			}

			internal const string AdminNode1 = "AdminNode1";

			internal const string ContentEngineNode1 = "ContentEngineNode1";

			internal const string InteractionEngineNode1 = "InteractionEngineNode1";

			internal const string IndexNode1 = "IndexNode1";
		}
	}
}
