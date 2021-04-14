using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Transport;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes
{
	public class MailboxDeliveryAvailabilityProbe : ProbeWorkItem
	{
		internal List<ProbeResult> AllInstanceProbeResults
		{
			get
			{
				return this.allActiveInstanceProbeResults;
			}
			set
			{
				this.allActiveInstanceProbeResults = value;
			}
		}

		internal Dictionary<string, List<ProbeResult>> DatabaseInstanceProbeResults
		{
			get
			{
				return this.activeDatabaseInstanceProbeResults;
			}
			set
			{
				this.activeDatabaseInstanceProbeResults = value;
			}
		}

		internal int MailboxesAtDiscovery { get; set; }

		public static void SetCustomErrorResponderMessage(ProbeResult result, string message)
		{
			result.StateAttribute1 = message;
		}

		public static string GetCustomErrorResponderMessage(ProbeResult result)
		{
			return result.StateAttribute1;
		}

		public static ProbeDefinition CreateMailboxDeliveryAvailabilityProbe(int mailboxCount)
		{
			return new ProbeDefinition
			{
				AssemblyPath = typeof(MailboxDeliveryAvailabilityProbe).Assembly.Location,
				TypeName = typeof(MailboxDeliveryAvailabilityProbe).FullName,
				Name = "MailboxDeliveryAvailabilityAggregationProbe",
				ServiceName = ExchangeComponent.MailboxTransport.Name,
				RecurrenceIntervalSeconds = 120,
				TimeoutSeconds = 90,
				MaxRetryAttempts = 3,
				TargetGroup = mailboxCount.ToString()
			};
		}

		public static MonitorDefinition CreateMailboxDeliveryAvailabilityMonitor(ProbeDefinition probe)
		{
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("MailboxDeliveryAvailabilityAggregationMonitor", probe.ConstructWorkItemResultName(), ExchangeComponent.MailboxTransport.Name, ExchangeComponent.MailboxTransport, 1, true, 120);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, TimeSpan.FromSeconds(0.0)),
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, TimeSpan.FromSeconds(600.0))
			};
			return monitorDefinition;
		}

		public static ResponderDefinition CreateMailboxDeliveryAvailabilityRestartResponder(ProbeDefinition probe, MonitorDefinition monitor)
		{
			string responderName = "MailboxDeliveryAvailabilityAggregationRestartResponder";
			string monitorName = monitor.ConstructWorkItemResultName();
			string windowsServiceName = "MSExchangeDelivery";
			ServiceHealthStatus responderTargetState = ServiceHealthStatus.Degraded;
			int serviceStopTimeoutInSeconds = 300;
			int serviceStartTimeoutInSeconds = 300;
			int serviceStartDelayInSeconds = 2;
			bool enabled = true;
			return RestartServiceResponder.CreateDefinition(responderName, monitorName, windowsServiceName, responderTargetState, serviceStopTimeoutInSeconds, serviceStartTimeoutInSeconds, serviceStartDelayInSeconds, false, DumpMode.FullDump, null, 15.0, 0, ExchangeComponent.MailboxTransport.Name, null, true, enabled, null, false);
		}

		public static ResponderDefinition CreateMailboxDeliveryAvailabilityEscalateResponder(ProbeDefinition probe, MonitorDefinition monitor)
		{
			return EscalateResponder.CreateDefinition("MailboxDeliveryAvailabilityAggregationEscalateResponder", ExchangeComponent.MailboxTransport.Name, monitor.Name, monitor.ConstructWorkItemResultName(), Environment.MachineName, ServiceHealthStatus.Unhealthy, ExchangeComponent.MailboxTransport.EscalationTeam, "MSExchangeDelivery service is not working.", "The MSExchangeDelivery service is failing due to this exception: \r\n{Probe.Exception}\r\n\r\nAdditional details:\r\n{Probe.StateAttribute1}", true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
		}

		internal static int GetMailboxesAtDiscovery(ProbeDefinition definition)
		{
			int result;
			if (int.TryParse(definition.TargetGroup, out result))
			{
				return result;
			}
			return -1;
		}

		internal static bool ExactExceptionFinder(ProbeResult result, string exception)
		{
			return result.ResultType == ResultType.Failed && result.Exception.IndexOf(exception, StringComparison.OrdinalIgnoreCase) > -1;
		}

		internal static bool AnyExceptionFinder(ProbeResult result, string exception)
		{
			return result.ResultType == ResultType.Failed && !string.IsNullOrEmpty(result.Exception);
		}

		internal static bool RegexExceptionFinder(ProbeResult result, string exception)
		{
			Regex regex = new Regex(exception, RegexOptions.IgnoreCase);
			return result.ResultType == ResultType.Failed && regex.IsMatch(result.Exception);
		}

		internal static void DetectHangOrStoppedProcess(ProbeWorkItem workitem)
		{
			using (ServiceController serviceController = new ServiceController("MSExchangeDelivery"))
			{
				if (serviceController.Status == ServiceControllerStatus.Running)
				{
					MailboxDeliveryAvailabilityProbe.SetCustomErrorResponderMessage(workitem.Result, "The Delivery service appears to be hung. The service is listed as running in the service controller. As part of the restart responder a dump was taken.");
				}
				else if (serviceController.Status == ServiceControllerStatus.Stopped)
				{
					MailboxDeliveryAvailabilityProbe.SetCustomErrorResponderMessage(workitem.Result, "The Delivery service is stopped. The restart responder has failed or the service stopped since the restart responder performed its recovery.");
				}
				else
				{
					MailboxDeliveryAvailabilityProbe.SetCustomErrorResponderMessage(workitem.Result, string.Format("The Delivery service is hung during {0}. As part of the restart responder a dump was taken.", serviceController.Status.ToString()));
				}
			}
		}

		internal void CheckKnownExceptions()
		{
			foreach (MailboxDeliveryAvailabilityProbe.KnownException ex in MailboxDeliveryAvailabilityProbe.KnownExceptionList)
			{
				string text;
				if (this.FindCommonException(ex.Exception, ex.MonitoringThreshold, ex.FailurePercent, new MailboxDeliveryAvailabilityProbe.ExceptionFinder(MailboxDeliveryAvailabilityProbe.ExactExceptionFinder), out text))
				{
					if (ex.Followups != null && ex.Followups.Count > 0)
					{
						foreach (MailboxDeliveryAvailabilityProbe.Followup followup in ex.Followups)
						{
							followup(this);
						}
					}
					throw new MailboxDeliveryAvailabilityProbe.MailDeliveryAvailabilityException(string.Format("'{2}' caused {0}% failure over {1} samples.", ex.FailurePercent, ex.MonitoringThreshold, ex.Exception));
				}
			}
			foreach (MailboxDeliveryAvailabilityProbe.KnownException ex2 in this.ParseExtensionAttributes())
			{
				string text2;
				if (this.FindCommonException(ex2.Exception, ex2.MonitoringThreshold, ex2.FailurePercent, new MailboxDeliveryAvailabilityProbe.ExceptionFinder(MailboxDeliveryAvailabilityProbe.ExactExceptionFinder), out text2))
				{
					throw new MailboxDeliveryAvailabilityProbe.MailDeliveryAvailabilityException(string.Format("'{2}' caused {0}% failure over {1} samples.", ex2.FailurePercent, ex2.MonitoringThreshold, ex2.Exception));
				}
			}
		}

		internal void CheckAllInstancesForSameFailures()
		{
			List<ProbeResult> source;
			if (!this.FindAnyException(5, 100, out source))
			{
				return;
			}
			List<MailboxDeliveryAvailabilityProbe.ExceptionHitBucket> list = new List<MailboxDeliveryAvailabilityProbe.ExceptionHitBucket>((from g in source
			group g by g.Exception into s
			select new MailboxDeliveryAvailabilityProbe.ExceptionHitBucket
			{
				Exception = s.Key,
				Count = s.Count<ProbeResult>()
			}).ToArray<MailboxDeliveryAvailabilityProbe.ExceptionHitBucket>());
			if (list.Count != 1)
			{
				return;
			}
			MailboxDeliveryAvailabilityProbe.SetCustomErrorResponderMessage(base.Result, string.Format("{0} - {1}", list[0].Count, list[0].Exception));
			throw new MailboxDeliveryAvailabilityProbe.MailDeliveryAvailabilityProbeException("Same Unknown Exception");
		}

		internal void CheckAllInstancesForDifferentFailures()
		{
			List<ProbeResult> source;
			if (this.FindAnyException(10, 100, out source))
			{
				List<MailboxDeliveryAvailabilityProbe.ExceptionHitBucket> list = new List<MailboxDeliveryAvailabilityProbe.ExceptionHitBucket>((from g in source
				group g by string.Format("{1}: {0}", g.Exception, g.StateAttribute21) into s
				select new MailboxDeliveryAvailabilityProbe.ExceptionHitBucket
				{
					Exception = s.Key,
					Count = s.Count<ProbeResult>()
				}).ToArray<MailboxDeliveryAvailabilityProbe.ExceptionHitBucket>());
				list.Sort((MailboxDeliveryAvailabilityProbe.ExceptionHitBucket x, MailboxDeliveryAvailabilityProbe.ExceptionHitBucket y) => x.Count.CompareTo(y.Count));
				MailboxDeliveryAvailabilityProbe.SetCustomErrorResponderMessage(base.Result, string.Join(",", from x in list
				select string.Format("{0} - {1}, ", x.Count, x.Exception)));
				throw new MailboxDeliveryAvailabilityProbe.MailDeliveryAvailabilityProbeException("Multiple different exceptions");
			}
		}

		internal IEnumerable<MailboxDeliveryAvailabilityProbe.KnownException> ParseExtensionAttributes()
		{
			string knownExceptionsValue;
			if (base.Definition.Attributes.TryGetValue("KnownExceptionTypes", out knownExceptionsValue) && !string.IsNullOrWhiteSpace(knownExceptionsValue))
			{
				string[] array = knownExceptionsValue.Split(new char[]
				{
					';'
				});
				int i = 0;
				while (i < array.Length)
				{
					string token = array[i];
					MailboxDeliveryAvailabilityProbe.KnownException exception;
					try
					{
						string[] array2 = token.Split(new char[]
						{
							'|'
						});
						if (string.IsNullOrWhiteSpace(array2[0]) || string.IsNullOrWhiteSpace(array2[1]) || string.IsNullOrWhiteSpace(array2[2]))
						{
							ProbeResult result = base.Result;
							result.FailureContext += string.Format("Found known exception with missing values: {0}. ", token);
							goto IL_189;
						}
						exception = new MailboxDeliveryAvailabilityProbe.KnownException
						{
							Exception = array2[0],
							FailurePercent = int.Parse(array2[1]),
							MonitoringThreshold = int.Parse(array2[2])
						};
					}
					catch
					{
						ProbeResult result2 = base.Result;
						result2.FailureContext += string.Format("Failed to parse this token: {0}. ", token);
						goto IL_189;
					}
					goto IL_16B;
					IL_189:
					i++;
					continue;
					IL_16B:
					yield return exception;
					goto IL_189;
				}
			}
			yield break;
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			this.MailboxesAtDiscovery = MailboxDeliveryAvailabilityProbe.GetMailboxesAtDiscovery(base.Definition);
			ProbeDefinition[] array = (from w in LocalDataAccess.GetAllDefinitions<ProbeDefinition>()
			where w.Name.Equals("MailboxDeliveryInstanceAvailabilityProbe", StringComparison.OrdinalIgnoreCase)
			select w).ToArray<ProbeDefinition>();
			if (this.MailboxesAtDiscovery > 0 && array.Length != this.MailboxesAtDiscovery)
			{
				ProbeResult result = base.Result;
				result.ExecutionContext += string.Format("Found {0} instance probe definitions. Expected count is {1}. ", array.Length, this.MailboxesAtDiscovery);
			}
			LocalDataAccess localDataAccess = new LocalDataAccess();
			DateTime minExecutionEndTime = DateTime.UtcNow.ToLocalTime().Subtract(TimeSpan.FromMinutes(60.0));
			this.allActiveInstanceProbeResults = new List<ProbeResult>();
			this.activeDatabaseInstanceProbeResults = new Dictionary<string, List<ProbeResult>>();
			List<ProbeResult> list = new List<ProbeResult>();
			foreach (ProbeDefinition probeDefinition in array)
			{
				List<ProbeResult> list2 = new List<ProbeResult>(from o in localDataAccess.GetTable<ProbeResult, string>(WorkItemResultIndex<ProbeResult>.ResultNameAndExecutionEndTime(probeDefinition.ConstructWorkItemResultName(), minExecutionEndTime))
				orderby o.ExecutionEndTime descending
				select o);
				if (list2.Any<ProbeResult>())
				{
					list.AddRange(list2);
					if (MailboxDeliveryInstanceAvailabilityProbe.GetActiveMDBStatus(list2.First<ProbeResult>()))
					{
						List<ProbeResult> list3 = new List<ProbeResult>(from w in list2
						where MailboxDeliveryInstanceAvailabilityProbe.GetActiveMDBStatus(w)
						select w);
						if (list3.Any<ProbeResult>())
						{
							this.allActiveInstanceProbeResults.AddRange(list3);
							this.activeDatabaseInstanceProbeResults.Add(probeDefinition.TargetResource, list3);
						}
					}
				}
			}
			if (!this.allActiveInstanceProbeResults.Any<ProbeResult>() && list.Any<ProbeResult>())
			{
				ProbeResult result2 = base.Result;
				result2.ExecutionContext += "No active instance probe results found, only passive databases. Proceeding as success. ";
				return;
			}
			if (!list.Any<ProbeResult>() && this.MailboxesAtDiscovery > 0)
			{
				ProbeResult result3 = base.Result;
				result3.ExecutionContext += "Discovery indicates mailboxes, but no probe results for any mailboxes. Proceeding as success. ";
				return;
			}
			this.CheckKnownExceptions();
			this.CheckAllInstancesForSameFailures();
			this.CheckAllInstancesForDifferentFailures();
		}

		private bool FindAnyException(int monitoringThreshold, int failurePercentage, out List<ProbeResult> allResults)
		{
			return this.FindException(monitoringThreshold, failurePercentage, string.Empty, new MailboxDeliveryAvailabilityProbe.ExceptionFinder(MailboxDeliveryAvailabilityProbe.AnyExceptionFinder), out allResults);
		}

		private bool FindCommonException(string exception, int monitoringThreshold, int failurePercentage, MailboxDeliveryAvailabilityProbe.ExceptionFinder searchQuery, out string exceptionInfo)
		{
			List<ProbeResult> list;
			bool flag = this.FindException(monitoringThreshold, failurePercentage, exception, searchQuery, out list);
			if (flag && list.Count > 0)
			{
				exceptionInfo = list.First<ProbeResult>().Exception;
			}
			else
			{
				exceptionInfo = string.Empty;
			}
			return flag;
		}

		private bool FindException(int monitoringThreshold, int failurePercentage, string exception, MailboxDeliveryAvailabilityProbe.ExceptionFinder searchQuery, out List<ProbeResult> allResults)
		{
			Dictionary<string, bool> dictionary = this.BuildTrackingDictionary();
			double num = (double)(failurePercentage / 100);
			int num2 = (int)num * monitoringThreshold;
			string empty = string.Empty;
			List<ProbeResult> list = new List<ProbeResult>();
			foreach (KeyValuePair<string, List<ProbeResult>> keyValuePair in this.activeDatabaseInstanceProbeResults)
			{
				List<ProbeResult> list2 = this.SearchForMatches(keyValuePair.Value, exception, monitoringThreshold, searchQuery);
				if (list2.Count >= num2)
				{
					dictionary[keyValuePair.Key] = true;
					list.AddRange(list2);
				}
			}
			bool flag = (from w in dictionary
			where !w.Value
			select w).Count<KeyValuePair<string, bool>>() == 0;
			allResults = new List<ProbeResult>();
			if (flag)
			{
				allResults = list;
			}
			return flag;
		}

		private List<ProbeResult> SearchForMatches(List<ProbeResult> instanceProbeResults, string exception, int monitoringThreshold, MailboxDeliveryAvailabilityProbe.ExceptionFinder findException)
		{
			List<ProbeResult> range = instanceProbeResults.GetRange(0, (instanceProbeResults.Count > monitoringThreshold) ? monitoringThreshold : instanceProbeResults.Count);
			return new List<ProbeResult>((from w in range
			where findException(w, exception)
			select w).ToArray<ProbeResult>());
		}

		private Dictionary<string, bool> BuildTrackingDictionary()
		{
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
			foreach (KeyValuePair<string, List<ProbeResult>> keyValuePair in this.activeDatabaseInstanceProbeResults)
			{
				dictionary.Add(keyValuePair.Key, false);
			}
			return dictionary;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static MailboxDeliveryAvailabilityProbe()
		{
			List<MailboxDeliveryAvailabilityProbe.KnownException> list = new List<MailboxDeliveryAvailabilityProbe.KnownException>();
			List<MailboxDeliveryAvailabilityProbe.KnownException> list2 = list;
			MailboxDeliveryAvailabilityProbe.KnownException item2 = default(MailboxDeliveryAvailabilityProbe.KnownException);
			item2.Exception = "System.Net.Sockets.SocketException";
			item2.MonitoringThreshold = 2;
			item2.FailurePercent = 100;
			List<MailboxDeliveryAvailabilityProbe.Followup> list3 = new List<MailboxDeliveryAvailabilityProbe.Followup>();
			list3.Add(delegate(ProbeWorkItem item)
			{
				MailboxDeliveryAvailabilityProbe.DetectHangOrStoppedProcess(item);
			});
			item2.Followups = list3;
			list2.Add(item2);
			MailboxDeliveryAvailabilityProbe.KnownExceptionList = list;
		}

		internal const string MailboxDeliveryAvailabilityProbeName = "MailboxDeliveryAvailabilityAggregationProbe";

		internal const string MailboxDeliveryAvailabilityMonitorName = "MailboxDeliveryAvailabilityAggregationMonitor";

		internal const string MailboxDeliveryAvailabilityRestartResponderName = "MailboxDeliveryAvailabilityAggregationRestartResponder";

		internal const string MailboxDeliveryAvailabilityEscalateResponderName = "MailboxDeliveryAvailabilityAggregationEscalateResponder";

		internal const string RestartResponderServiceToRestart = "MSExchangeDelivery";

		internal const string EscalateResponderEmailSubject = "MSExchangeDelivery service is not working.";

		internal const string EscalateResponderEmailMessage = "The MSExchangeDelivery service is failing due to this exception: \r\n{Probe.Exception}\r\n\r\nAdditional details:\r\n{Probe.StateAttribute1}";

		internal const int RandomExceptionMonitoringThreshold = 10;

		internal const int RandomExceptionFailurePercentThreshold = 100;

		internal const int UnknownSameExceptionMonitoringThreshold = 5;

		internal const int UnknownSameExceptionFailurePercentThreshold = 100;

		internal const string ExtensionAttributesKnownExceptionTypes = "KnownExceptionTypes";

		internal const int ProbeRecurrenceIntervalSeconds = 120;

		internal const int ProbeTimeoutSeconds = 90;

		internal const int ProbeMaxRetryAttempts = 3;

		internal const int MonitorFailureCount = 1;

		internal const int MonitorMonitoringInterval = 120;

		internal const bool MonitorEnabled = true;

		internal const int MonitorTransitionToRestartInSeconds = 0;

		internal const int MonitorTransitionToEscalateInSeconds = 600;

		internal const int RestartResponderServiceStopTimeoutSeconds = 300;

		internal const int RestartResponderServiceStartTimeoutSeconds = 300;

		internal const int RestartResponderServiceStartDelayInSeconds = 2;

		internal const bool RestartResponderEnabled = true;

		internal const bool EscalateResponderEnabled = true;

		internal const NotificationServiceClass EscalateLevel = NotificationServiceClass.Urgent;

		internal static readonly List<string> TransientSmtpResponseList = new List<string>
		{
			AckReason.MessageDelayedDeleteByAdmin.ToString(),
			AckReason.MessageDeletedByAdmin.ToString(),
			AckReason.MessageDeletedByTransportAgent.ToString(),
			AckReason.PoisonMessageDeletedByAdmin.ToString(),
			AckReason.MessageDelayedDeleteByAdmin.ToString(),
			AckReason.MessageDeletedByAdmin.ToString(),
			AckReason.MessageDeletedByTransportAgent.ToString(),
			AckReason.PoisonMessageDeletedByAdmin.ToString(),
			AckReason.MessageDelayedDeleteByAdmin.ToString(),
			AckReason.MessageDeletedByAdmin.ToString(),
			AckReason.MessageDeletedByTransportAgent.ToString(),
			AckReason.PoisonMessageDeletedByAdmin.ToString(),
			AckReason.MailboxServerOffline.ToString(),
			AckReason.MDBOffline.ToString(),
			AckReason.MapiNoAccessFailure.ToString(),
			AckReason.MailboxServerTooBusy.ToString(),
			AckReason.MailboxMapiSessionLimit.ToString(),
			AckReason.MailboxServerMaxThreadsPerMdbExceeded.ToString(),
			AckReason.MapiExceptionMaxThreadsPerSCTExceeded.ToString(),
			AckReason.MailboxDatabaseThreadLimitExceeded.ToString(),
			AckReason.RecipientThreadLimitExceeded.ToString(),
			AckReason.DeliverySourceThreadLimitExceeded.ToString(),
			AckReason.DynamicMailboxDatabaseThrottlingLimitExceeded.ToString(),
			AckReason.MailboxIOError.ToString(),
			AckReason.MailboxServerNotEnoughMemory.ToString(),
			AckReason.MissingMdbProperties.ToString(),
			AckReason.RecipientMailboxQuarantined.ToString()
		};

		internal static readonly List<string> WildCardTransientResponseList = new List<string>
		{
			"MailboxOfflineException",
			"MailboxUnavailableException.MapiExceptionUnknownUser",
			"StorageTransientException.MapiExceptionTimeout",
			"501 5.1.3 Invalid address"
		};

		internal static readonly List<MailboxDeliveryAvailabilityProbe.KnownException> KnownExceptionList;

		private List<ProbeResult> allActiveInstanceProbeResults;

		private Dictionary<string, List<ProbeResult>> activeDatabaseInstanceProbeResults;

		internal delegate void Followup(ProbeWorkItem workitem);

		private delegate bool ExceptionFinder(ProbeResult result, string exception);

		internal struct KnownException
		{
			public string Exception;

			public int MonitoringThreshold;

			public int FailurePercent;

			public List<MailboxDeliveryAvailabilityProbe.Followup> Followups;
		}

		internal struct ExceptionHitBucket
		{
			public string Exception;

			public int Count;
		}

		internal class MailDeliveryAvailabilityProbeException : Exception
		{
			public MailDeliveryAvailabilityProbeException(string message) : base(message)
			{
			}
		}

		internal class MailDeliveryAvailabilityException : Exception
		{
			public MailDeliveryAvailabilityException(string message) : base(message)
			{
			}
		}
	}
}
