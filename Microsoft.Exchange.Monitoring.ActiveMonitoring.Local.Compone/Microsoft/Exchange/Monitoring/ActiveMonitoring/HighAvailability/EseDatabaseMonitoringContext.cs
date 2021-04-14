using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability
{
	internal sealed class EseDatabaseMonitoringContext : MonitoringContextBase
	{
		public EseDatabaseMonitoringContext(IMaintenanceWorkBroker broker, LocalEndpointManager endpointManager, TracingContext traceContext) : base(broker, endpointManager, traceContext)
		{
		}

		public override void CreateContext()
		{
			bool isDataCenter = LocalEndpointManager.IsDataCenter;
			using (IEnumerator<MailboxDatabaseInfo> enumerator = base.EndpointManager.MailboxDatabaseEndpoint.UnverifiedMailboxDatabaseInfoCollectionForBackendLiveIdAuthenticationProbe.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MailboxDatabaseInfo dbInfo = enumerator.Current;
					base.InvokeCatchAndLog(delegate
					{
						this.CreateDbErrorMonitor(dbInfo);
					});
					base.InvokeCatchAndLog(delegate
					{
						this.CreateInconsistentDataMonitor(dbInfo);
					});
					base.InvokeCatchAndLog(delegate
					{
						this.CreateLostFlushMonitor(dbInfo);
					});
					if (isDataCenter)
					{
						base.InvokeCatchAndLog(delegate
						{
							this.CreateSinglePageLogicalCorruptionMonitor(dbInfo);
						});
						base.InvokeCatchAndLog(delegate
						{
							this.CreateDbDivergenceMonitor(dbInfo);
						});
					}
				}
			}
		}

		private static bool EseDbTimeTooNewEventMatcher(EventLogNotification.EventRecordInternal eventRecord)
		{
			if (eventRecord.Id == 516)
			{
				IList<EventProperty> properties = eventRecord.EventRecord.Properties;
				return properties != null && properties.Count >= 8 && properties[7].Value != null && properties[7].Value.ToString().Equals(-567.ToString());
			}
			return true;
		}

		private static bool EseDbTimeTooOldEventMatcher(EventLogNotification.EventRecordInternal eventRecord)
		{
			if (eventRecord.Id == 516)
			{
				IList<EventProperty> properties = eventRecord.EventRecord.Properties;
				return properties != null && properties.Count >= 8 && properties[7].Value != null && properties[7].Value.ToString().Equals(-566.ToString());
			}
			return true;
		}

		private static void EseEventNotificationProcessor(EventLogNotification.EventRecordInternal eventRecord, ref EventLogNotification.EventNotificationMetadata eventNotification)
		{
			IList<EventProperty> properties = eventRecord.EventRecord.Properties;
			if (properties != null && properties.Count >= 3)
			{
				string text = (properties[2].Value == null) ? "NULL" : properties[2].Value.ToString().Split(new char[]
				{
					':'
				})[0];
				eventNotification.StateAttribute3 = text;
				if (properties[2].Value != null)
				{
					eventNotification.TagName = text;
				}
			}
		}

		private void CreateLostFlushMonitor(MailboxDatabaseInfo dbInfo)
		{
			string name = "EseLostFlushEventProbe";
			string name2 = "EseLostFlushMonitor";
			string name3 = "EseLostFlushEscalate";
			EventLogSubscription eventLogSubscription = new EventLogSubscription(name, TimeSpan.FromSeconds(600.0), new EventMatchingRule("Application", "ESE", new int[]
			{
				530
			}, 2, false, false, null, new EventMatchingRule.CustomNotification(EseDatabaseMonitoringContext.EseEventNotificationProcessor)), null, null, null);
			EventLogNotification.Instance.AddSubscription(eventLogSubscription);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(name2, EventLogNotification.ConstructResultMask(eventLogSubscription.Name, dbInfo.MailboxDatabaseName), HighAvailabilityConstants.ServiceName, ExchangeComponent.DataProtection, 1, true, 600);
			monitorDefinition.TargetResource = dbInfo.MailboxDatabaseName;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate HA health is not impacted by ESE issues";
			base.AddChainedResponders(ref monitorDefinition, new MonitorStateResponderTuple[]
			{
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
					Responder = EscalateResponder.CreateDefinition(name3, HighAvailabilityConstants.ServiceName, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.TargetResource, ServiceHealthStatus.Unhealthy, "ESE", Strings.EseLostFlushDetectedEscalationSubject(HighAvailabilityConstants.ServiceName, Environment.MachineName, dbInfo.MailboxDatabaseName), Strings.EseLostFlushDetectedEscalationMessage(Environment.MachineName, dbInfo.MailboxDatabaseName), true, NotificationServiceClass.Scheduled, 18000, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false)
				}
			});
		}

		private void CreateDbErrorMonitor(MailboxDatabaseInfo dbInfo)
		{
			string name = "EseDbTimeTooNewEventProbe";
			string name2 = "EseDbTimeTooOldEventProbe";
			string text = "EseDbTimeTooNewMonitor";
			string name3 = "EseDbTimeTooOldMonitor";
			string name4 = "EseDbTimeTooNewEscalate";
			string name5 = "EseDbTimeTooOldEscalate";
			string responderName = "EseDbTimeTooNewCollectAndMerge";
			EventLogSubscription eventLogSubscription = new EventLogSubscription(name, TimeSpan.FromSeconds(600.0), new EventMatchingRule("Application", "ESE", new int[]
			{
				516,
				538
			}, 2, true, true, new EventMatchingRule.CustomMatching(EseDatabaseMonitoringContext.EseDbTimeTooNewEventMatcher), new EventMatchingRule.CustomNotification(EseDatabaseMonitoringContext.EseEventNotificationProcessor)), null, null, null);
			EventLogNotification.Instance.AddSubscription(eventLogSubscription);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(text, EventLogNotification.ConstructResultMask(eventLogSubscription.Name, dbInfo.MailboxDatabaseName), HighAvailabilityConstants.ServiceName, ExchangeComponent.DataProtection, 1, true, 600);
			monitorDefinition.TargetResource = dbInfo.MailboxDatabaseName;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate HA health is not impacted by ESE issues";
			List<MonitorStateResponderTuple> list = new List<MonitorStateResponderTuple>
			{
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
					Responder = EscalateResponder.CreateDefinition(name4, HighAvailabilityConstants.ServiceName, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.TargetResource, ServiceHealthStatus.Unhealthy, "ESE", Strings.EseDbTimeAdvanceEscalationSubject(HighAvailabilityConstants.ServiceName, Environment.MachineName, dbInfo.MailboxDatabaseName), Strings.EseDbTimeAdvanceEscalationMessage(Environment.MachineName, dbInfo.MailboxDatabaseName), true, NotificationServiceClass.Urgent, 18000, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false)
				}
			};
			if (LocalEndpointManager.IsDataCenter)
			{
				list.Add(new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy, list[list.Count - 1].MonitorState.TransitionTimeout.Add(TimeSpan.FromMinutes(1.0))),
					Responder = CollectAndMergeResponder.CreateDefinition(responderName, text, ServiceHealthStatus.Unrecoverable2, dbInfo.MailboxDatabaseName, "Exchange", true)
				});
			}
			base.AddChainedResponders(ref monitorDefinition, list.ToArray());
			EventLogSubscription eventLogSubscription2 = new EventLogSubscription(name2, TimeSpan.FromSeconds(600.0), new EventMatchingRule("Application", "ESE", new int[]
			{
				516,
				539
			}, 2, true, true, new EventMatchingRule.CustomMatching(EseDatabaseMonitoringContext.EseDbTimeTooOldEventMatcher), new EventMatchingRule.CustomNotification(EseDatabaseMonitoringContext.EseEventNotificationProcessor)), null, null, null);
			EventLogNotification.Instance.AddSubscription(eventLogSubscription2);
			MonitorDefinition monitorDefinition2 = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(name3, EventLogNotification.ConstructResultMask(eventLogSubscription2.Name, dbInfo.MailboxDatabaseName), HighAvailabilityConstants.ServiceName, ExchangeComponent.DataProtection, 1, true, 600);
			monitorDefinition2.TargetResource = dbInfo.MailboxDatabaseName;
			monitorDefinition2.ServicePriority = 0;
			monitorDefinition2.ScenarioDescription = "Validate HA health is not impacted by ESE issues";
			base.AddChainedResponders(ref monitorDefinition2, new MonitorStateResponderTuple[]
			{
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
					Responder = EscalateResponder.CreateDefinition(name5, HighAvailabilityConstants.ServiceName, monitorDefinition2.Name, monitorDefinition2.ConstructWorkItemResultName(), monitorDefinition2.TargetResource, ServiceHealthStatus.Unhealthy, "ESE", Strings.EseDbTimeSmallerEscalationSubject(HighAvailabilityConstants.ServiceName, Environment.MachineName, dbInfo.MailboxDatabaseName), Strings.EseDbTimeSmallerEscalationMessage(Environment.MachineName, dbInfo.MailboxDatabaseName), true, NotificationServiceClass.Scheduled, 18000, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false)
				}
			});
		}

		private void CreateInconsistentDataMonitor(MailboxDatabaseInfo dbInfo)
		{
			string name = "EseInconsistentDataEventProbe";
			string name2 = "EseInconsistentDataMonitor";
			string name3 = "EseInconsistentDataEscalate";
			EventLogSubscription eventLogSubscription = new EventLogSubscription(name, TimeSpan.FromSeconds(600.0), new EventMatchingRule("Application", "ESE", new int[]
			{
				447,
				448
			}, 2, false, false, null, new EventMatchingRule.CustomNotification(EseDatabaseMonitoringContext.EseEventNotificationProcessor)), null, null, null);
			EventLogNotification.Instance.AddSubscription(eventLogSubscription);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(name2, EventLogNotification.ConstructResultMask(eventLogSubscription.Name, dbInfo.MailboxDatabaseName), HighAvailabilityConstants.ServiceName, ExchangeComponent.DataProtection, 1, true, 600);
			monitorDefinition.TargetResource = dbInfo.MailboxDatabaseName;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate HA health is not impacted by ESE issues";
			base.AddChainedResponders(ref monitorDefinition, new MonitorStateResponderTuple[]
			{
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
					Responder = EscalateResponder.CreateDefinition(name3, HighAvailabilityConstants.ServiceName, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.TargetResource, ServiceHealthStatus.Unhealthy, "ESE", Strings.EseInconsistentDataDetectedEscalationSubject(HighAvailabilityConstants.ServiceName, Environment.MachineName, dbInfo.MailboxDatabaseName), Strings.EseInconsistentDataDetectedEscalationMessage(Environment.MachineName, dbInfo.MailboxDatabaseName), true, NotificationServiceClass.Scheduled, 18000, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false)
				}
			});
		}

		private void CreateSinglePageLogicalCorruptionMonitor(MailboxDatabaseInfo dbInfo)
		{
			string name = "EseSinglePageLogicalCorruptionEventProbe";
			string name2 = "EseSinglePageLogicalCorruptionMonitor";
			string name3 = "EseSinglePageLogicalCorruptionEscalate";
			EventLogSubscription eventLogSubscription = new EventLogSubscription(name, TimeSpan.FromSeconds(600.0), new EventMatchingRule("Application", "ESE", new int[]
			{
				475,
				476,
				497,
				517,
				537,
				542
			}, 2, false, false, null, new EventMatchingRule.CustomNotification(EseDatabaseMonitoringContext.EseEventNotificationProcessor)), null, null, null);
			EventLogNotification.Instance.AddSubscription(eventLogSubscription);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(name2, EventLogNotification.ConstructResultMask(eventLogSubscription.Name, dbInfo.MailboxDatabaseName), HighAvailabilityConstants.ServiceName, ExchangeComponent.DataProtection, 1, true, 600);
			monitorDefinition.TargetResource = dbInfo.MailboxDatabaseName;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate HA health is not impacted by ESE issues";
			base.AddChainedResponders(ref monitorDefinition, new MonitorStateResponderTuple[]
			{
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
					Responder = EscalateResponder.CreateDefinition(name3, HighAvailabilityConstants.ServiceName, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.TargetResource, ServiceHealthStatus.Unhealthy, "ESE", Strings.EseSinglePageLogicalCorruptionDetectedSubject(HighAvailabilityConstants.ServiceName, Environment.MachineName, dbInfo.MailboxDatabaseName), Strings.EseSinglePageLogicalCorruptionDetectedEscalationMessage(Environment.MachineName, dbInfo.MailboxDatabaseName), true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false)
				}
			});
		}

		private void CreateDbDivergenceMonitor(MailboxDatabaseInfo dbInfo)
		{
			string name = "EseDbDivergenceEventProbe";
			string name2 = "EseDbDivergenceMonitor";
			string name3 = "EseDbDivergenceEscalate";
			EventLogSubscription eventLogSubscription = new EventLogSubscription(name, TimeSpan.FromSeconds(600.0), new EventMatchingRule("Application", "ESE", new int[]
			{
				540,
				541
			}, 2, false, false, null, new EventMatchingRule.CustomNotification(EseDatabaseMonitoringContext.EseEventNotificationProcessor)), null, null, null);
			EventLogNotification.Instance.AddSubscription(eventLogSubscription);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(name2, EventLogNotification.ConstructResultMask(eventLogSubscription.Name, dbInfo.MailboxDatabaseName), HighAvailabilityConstants.ServiceName, ExchangeComponent.DataProtection, 1, true, 600);
			monitorDefinition.TargetResource = dbInfo.MailboxDatabaseName;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate HA health is not impacted by ESE issues";
			base.AddChainedResponders(ref monitorDefinition, new MonitorStateResponderTuple[]
			{
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
					Responder = EscalateResponder.CreateDefinition(name3, HighAvailabilityConstants.ServiceName, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.TargetResource, ServiceHealthStatus.Unhealthy, "ESE", Strings.EseDbDivergenceDetectedSubject(HighAvailabilityConstants.ServiceName, Environment.MachineName, dbInfo.MailboxDatabaseName), Strings.EseDbDivergenceDetectedEscalationMessage(Environment.MachineName, dbInfo.MailboxDatabaseName), true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false)
				}
			});
		}

		private const int DbTimeMismatchEventId = 516;

		private const int DbtimeCheckActiveBehindEventId = 538;

		private const int DbtimeCheckPassiveBehindEventId = 539;

		private const string EseEventLogName = "Application";

		private const string EseEventProviderName = "ESE";

		public enum DbErrorEventType
		{
			DbTimeAdvance = -567,
			DbTimeSmaller
		}
	}
}
