using System;
using System.Reflection;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders
{
	public class CafeOfflineResponder : OfflineResponder
	{
		internal static ResponderDefinition CreateDefinition(string responderName, string monitorName, ServerComponentEnum componentToTakeOffline, ServiceHealthStatus responderTargetState, string serviceName, double minimumFractionRequiredOnline = -1.0, string failureReason = "", string arbitrationScope = "Datacenter", string arbitrationSource = "F5AvailabilityData", string requestedAction = "MachineOut")
		{
			ResponderDefinition responderDefinition = OfflineResponder.CreateDefinition(responderName, monitorName, componentToTakeOffline, responderTargetState, serviceName, null, -1, failureReason, arbitrationScope, arbitrationSource, requestedAction);
			responderDefinition.AssemblyPath = CafeOfflineResponder.AssemblyPath;
			responderDefinition.TypeName = CafeOfflineResponder.TypeName;
			responderDefinition.Attributes["CafeMinimumServerFractionOnline"] = minimumFractionRequiredOnline.ToString();
			return responderDefinition;
		}

		protected override void InitializeAttributes(AttributeHelper attributeHelper = null)
		{
			CafeUtils.ConfigureResponderForCafeMinimumValues(this, delegate(AttributeHelper attribHelper)
			{
				this.<>n__FabricatedMethod3(attribHelper);
			}, delegate(int minRequired)
			{
				base.MinimumRequiredServers = minRequired;
			}, base.TraceContext);
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(CafeOfflineResponder).FullName;
	}
}
