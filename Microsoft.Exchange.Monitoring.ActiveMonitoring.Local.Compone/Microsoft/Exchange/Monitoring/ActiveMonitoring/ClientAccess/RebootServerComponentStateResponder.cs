using System;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ClientAccess
{
	internal class RebootServerComponentStateResponder : ForceRebootServerResponder
	{
		internal static ResponderDefinition CreateDefinition(string responderName, string serviceName, string monitorName, ServiceHealthStatus responderTargetState, ServerComponentEnum serverComponentToVerifyState, CafeUtils.TriggerConfig triggerConfig)
		{
			ResponderDefinition responderDefinition = ForceRebootServerResponder.CreateDefinition(responderName, monitorName, responderTargetState, null, -1, "", "", "Datacenter, Stamp", "RecoveryData", "ArbitrationOnly", serviceName, true, null, false);
			responderDefinition.AssemblyPath = RebootServerComponentStateResponder.AssemblyPath;
			responderDefinition.TypeName = RebootServerComponentStateResponder.TypeName;
			responderDefinition.Attributes["ComponentStateServerComponentName"] = serverComponentToVerifyState.ToString();
			responderDefinition.Attributes["ComponentStateTriggerConfig"] = triggerConfig.ToString();
			return responderDefinition;
		}

		protected override void InitializeAttributes(AttributeHelper attributeHelper = null)
		{
			CafeUtils.ConfigureResponderForCafeMinimumValues(this, delegate(AttributeHelper attribHelper)
			{
				this.<>n__FabricatedMethod5(attribHelper);
			}, delegate(int minRequired)
			{
				base.MinimumRequiredServers = minRequired;
			}, base.TraceContext);
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			CafeUtils.InvokeResponderGivenComponentState(this, delegate(CancellationToken cancelToken)
			{
				this.<>n__FabricatedMethod7(cancelToken);
			}, base.TraceContext, cancellationToken);
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(RebootServerComponentStateResponder).FullName;
	}
}
