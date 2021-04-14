using System;
using System.Reflection;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.O365.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders
{
	public class OBDEscalateResponder : EscalateResponder
	{
		public new static ResponderDefinition CreateDefinition(string name, string serviceName, string alertTypeId, string alertMask, string targetResource, ServiceHealthStatus targetHealthState, string escalationTeam, string escalationSubjectUnhealthy, string escalationMessageUnhealthy, bool enabled = true, NotificationServiceClass notificationServiceClass = NotificationServiceClass.Urgent, int minimumSecondsBetweenEscalates = 14400, string dailySchedulePattern = "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", bool loadEscalationMessageUnhealthyFromResource = false)
		{
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition(name, serviceName, alertTypeId, alertMask, targetResource, targetHealthState, escalationTeam, escalationSubjectUnhealthy, escalationMessageUnhealthy, enabled, notificationServiceClass, minimumSecondsBetweenEscalates, dailySchedulePattern, loadEscalationMessageUnhealthyFromResource);
			responderDefinition.AssemblyPath = OBDEscalateResponder.AssemblyPath;
			responderDefinition.TypeName = OBDEscalateResponder.TypeName;
			return responderDefinition;
		}

		protected override void InvokeNewServiceAlert(Guid alertGuid, string alertTypeId, string alertName, string alertDescription, DateTime raisedTime, string escalationTeam, string service, string alertSource, bool isDatacenter, bool urgent, string environment, string location, string forest, string dag, string site, string region, string capacityUnit, string rack, string alertCategory)
		{
			alertName = alertName.Substring(0, (alertName.Length > 255) ? 255 : alertName.Length);
			if (!Datacenter.IsGallatinDatacenter())
			{
				string environment2 = Settings.IsProductionEnvironment ? "prod" : "ppe";
				SmartAlertsV2Client.NewAlert(alertGuid, alertTypeId, alertName, alertDescription, raisedTime, escalationTeam, service, "LocalActiveMonitoring", urgent, environment2, location, forest, dag, site, region, capacityUnit, rack, base.Definition.NotificationServiceClass, string.IsNullOrEmpty(base.Definition.Account) ? "CN=exouser.outlook.com" : base.Definition.Account);
				return;
			}
			base.InvokeNewServiceAlert(alertGuid, alertTypeId, alertName, alertDescription, raisedTime, escalationTeam, service, alertSource, isDatacenter, urgent, environment, location, forest, dag, site, region, capacityUnit, rack, alertCategory);
		}

		private DateTime ConvertToPST(DateTime utcTime)
		{
			utcTime = DateTime.SpecifyKind(utcTime, DateTimeKind.Utc);
			return TimeZoneInfo.ConvertTime(utcTime, OBDEscalateResponder.pstTimeZoneInfo);
		}

		private const int MaxAlertNameLength = 255;

		private const string OBDAlertSource = "LocalActiveMonitoring";

		private const string ManagementCertificateSubject = "CN=exouser.outlook.com";

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(OBDEscalateResponder).FullName;

		private static TimeZoneInfo pstTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
	}
}
