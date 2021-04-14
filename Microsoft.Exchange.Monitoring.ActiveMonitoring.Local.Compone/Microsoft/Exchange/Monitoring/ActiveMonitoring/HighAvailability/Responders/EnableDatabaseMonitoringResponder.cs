using System;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability.Responders
{
	public class EnableDatabaseMonitoringResponder : ResponderWorkItem
	{
		public static ResponderDefinition CreateDefinition(string name, string serviceName, MailboxDatabaseInfo targetDatabase, string alertTypeId, string alertMask, string targetResource, ServiceHealthStatus targetHealthState, string throttleGroupName = null)
		{
			ResponderDefinition responderDefinition = new ResponderDefinition();
			responderDefinition.AssemblyPath = Assembly.GetExecutingAssembly().Location;
			responderDefinition.TypeName = typeof(EnableDatabaseMonitoringResponder).FullName;
			responderDefinition.Name = name;
			responderDefinition.ServiceName = serviceName;
			responderDefinition.AlertTypeId = alertTypeId;
			responderDefinition.AlertMask = alertMask;
			responderDefinition.TargetResource = targetResource;
			responderDefinition.TargetHealthState = targetHealthState;
			responderDefinition.RecurrenceIntervalSeconds = 300;
			responderDefinition.WaitIntervalSeconds = 30;
			responderDefinition.TimeoutSeconds = 300;
			responderDefinition.MaxRetryAttempts = 3;
			responderDefinition.Enabled = true;
			responderDefinition.Attributes["dbGuid"] = targetDatabase.MailboxDatabaseGuid.ToString();
			return responderDefinition;
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			Guid dbGuid = new Guid(base.Definition.Attributes["dbGuid"]);
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 81, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\Responders\\EnableDatabaseMonitoringResponder.cs");
			MailboxDatabase mailboxDatabase = topologyConfigurationSession.FindDatabaseByGuid<MailboxDatabase>(dbGuid);
			if (mailboxDatabase == null)
			{
				throw new DatabaseNotFoundInADException(dbGuid.ToString());
			}
			mailboxDatabase.AutoDagExcludeFromMonitoring = false;
			base.Result.StateAttribute2 = string.Format("AutoDagExcludeFromMonitoring set to {0} for database '{1}.", mailboxDatabase.AutoDagExcludeFromMonitoring, mailboxDatabase.Name);
			topologyConfigurationSession.Save(mailboxDatabase);
		}
	}
}
