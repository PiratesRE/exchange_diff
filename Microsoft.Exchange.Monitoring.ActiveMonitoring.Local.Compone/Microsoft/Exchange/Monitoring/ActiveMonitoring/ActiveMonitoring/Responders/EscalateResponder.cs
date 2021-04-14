using System;
using System.Reflection;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders
{
	public class EscalateResponder : EscalateResponderBase
	{
		public static ResponderDefinition CreateDefinition(string name, string serviceName, string alertTypeId, string alertMask, string targetResource, ServiceHealthStatus targetHealthState, string escalationTeam, string escalationSubjectUnhealthy, string escalationMessageUnhealthy, bool enabled = true, NotificationServiceClass notificationServiceClass = NotificationServiceClass.Urgent, int minimumSecondsBetweenEscalates = 14400, string dailySchedulePattern = "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", bool loadEscalationMessageUnhealthyFromResource = false)
		{
			return EscalateResponder.CreateDefinition(name, serviceName, alertTypeId, alertMask, targetResource, targetHealthState, null, escalationTeam, escalationSubjectUnhealthy, escalationMessageUnhealthy, enabled, notificationServiceClass, minimumSecondsBetweenEscalates, dailySchedulePattern, loadEscalationMessageUnhealthyFromResource);
		}

		public static ResponderDefinition CreateDefinition(string name, string serviceName, string alertTypeId, string alertMask, string targetResource, ServiceHealthStatus targetHealthState, string escalationService, string escalationTeam, string escalationSubjectUnhealthy, string escalationMessageUnhealthy, bool enabled = true, NotificationServiceClass notificationServiceClass = NotificationServiceClass.Urgent, int minimumSecondsBetweenEscalates = 14400, string dailySchedulePattern = "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", bool loadEscalationMessageUnhealthyFromResource = false)
		{
			if (string.IsNullOrWhiteSpace(escalationSubjectUnhealthy))
			{
				throw new ArgumentException("escalationSubjectUnhealthy");
			}
			if (string.IsNullOrWhiteSpace(escalationMessageUnhealthy))
			{
				throw new ArgumentException("escalationMessageUnhealthy");
			}
			ResponderDefinition responderDefinition = new ResponderDefinition();
			responderDefinition.AssemblyPath = EscalateResponder.AssemblyPath;
			responderDefinition.TypeName = EscalateResponder.TypeName;
			responderDefinition.Name = name;
			responderDefinition.ServiceName = serviceName;
			responderDefinition.AlertTypeId = alertTypeId;
			responderDefinition.AlertMask = alertMask;
			responderDefinition.TargetResource = targetResource;
			responderDefinition.TargetHealthState = targetHealthState;
			responderDefinition.EscalationService = escalationService;
			responderDefinition.EscalationTeam = escalationTeam;
			responderDefinition.EscalationSubject = escalationSubjectUnhealthy;
			responderDefinition.EscalationMessage = escalationMessageUnhealthy;
			responderDefinition.NotificationServiceClass = notificationServiceClass;
			responderDefinition.MinimumSecondsBetweenEscalates = minimumSecondsBetweenEscalates;
			responderDefinition.DailySchedulePattern = dailySchedulePattern;
			responderDefinition.RecurrenceIntervalSeconds = 300;
			responderDefinition.WaitIntervalSeconds = minimumSecondsBetweenEscalates;
			responderDefinition.TimeoutSeconds = 300;
			responderDefinition.MaxRetryAttempts = 3;
			responderDefinition.Enabled = enabled;
			responderDefinition.Attributes[EscalateResponderBase.LoadFromResourceAttributeValue] = loadEscalationMessageUnhealthyFromResource.ToString();
			EscalateResponderBase.SetActiveMonitoringCertificateSettings(responderDefinition);
			return responderDefinition;
		}

		internal override void LogCustomUnhealthyEvent(EscalateResponderBase.UnhealthyMonitoringEvent unhealthyEvent)
		{
			ManagedAvailabilityCrimsonEvents.UnhealthyHealthSet.Log<string, string, string, string>(unhealthyEvent.HealthSet, unhealthyEvent.Subject, unhealthyEvent.Message, unhealthyEvent.Monitor);
		}

		internal override string GetFFOForestName()
		{
			return ComputerInformation.DnsPhysicalDomainName;
		}

		internal override EscalationEnvironment GetEscalationEnvironment()
		{
			if (this.escalationEnvironment == null)
			{
				this.escalationEnvironment = new EscalationEnvironment?(EscalationEnvironment.OnPrem);
				if (base.Broker.IsLocal() && !VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).ActiveMonitoring.EscalateResponder.Enabled)
				{
					this.escalationEnvironment = new EscalationEnvironment?(EscalationEnvironment.OnPrem);
				}
				else if (base.Broker.IsLocal() && VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).ActiveMonitoring.EscalateResponder.Enabled)
				{
					this.escalationEnvironment = new EscalationEnvironment?(EscalationEnvironment.Datacenter);
				}
				else if (!base.Broker.IsLocal())
				{
					this.escalationEnvironment = new EscalationEnvironment?(EscalationEnvironment.OutsideIn);
				}
				if (EscalateResponderBase.IsOBDGallatinMachine)
				{
					this.escalationEnvironment = new EscalationEnvironment?(EscalationEnvironment.Datacenter);
				}
			}
			return this.escalationEnvironment.Value;
		}

		internal override ScopeMappingEndpoint GetScopeMappingEndpoint()
		{
			return ScopeMappingEndpointManager.Instance.GetEndpoint();
		}

		static EscalateResponder()
		{
			EscalateResponderBase.DefaultEscalationSubject = Strings.DefaultEscalationSubject;
			EscalateResponderBase.DefaultEscalationMessage = Strings.DefaultEscalationMessage;
			EscalateResponderBase.HealthSetEscalationSubjectPrefix = Strings.HealthSetEscalationSubjectPrefix;
			EscalateResponderBase.HealthSetMaintenanceEscalationSubjectPrefix = Strings.HealthSetMaintenanceEscalationSubjectPrefix;
			EscalateResponderBase.EscalationHelper = new HealthSetEscalationLocalHelper();
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(EscalateResponder).FullName;

		private EscalationEnvironment? escalationEnvironment;
	}
}
