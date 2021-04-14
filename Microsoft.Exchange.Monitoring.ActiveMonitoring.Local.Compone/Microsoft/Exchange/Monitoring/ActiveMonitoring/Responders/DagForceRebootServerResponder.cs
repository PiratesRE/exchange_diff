using System;
using System.Reflection;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders
{
	public class DagForceRebootServerResponder : ForceRebootServerResponder
	{
		internal static ResponderDefinition CreateDefinition(string responderName, string monitorName, ServiceHealthStatus responderTargetState)
		{
			return DagForceRebootServerResponder.CreateDefinition(responderName, "Exchange", monitorName, responderTargetState);
		}

		internal static ResponderDefinition CreateDefinition(string responderName, string serviceName, string monitorName, ServiceHealthStatus responderTargetState)
		{
			ResponderDefinition responderDefinition = ForceRebootServerResponder.CreateDefinition(responderName, monitorName, responderTargetState, null, -1, "", "", "Datacenter, Stamp", "RecoveryData", "ArbitrationOnly", serviceName, true, "Dag", false);
			responderDefinition.AssemblyPath = DagForceRebootServerResponder.AssemblyPath;
			responderDefinition.TypeName = DagForceRebootServerResponder.TypeName;
			return responderDefinition;
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(DagForceRebootServerResponder).FullName;
	}
}
