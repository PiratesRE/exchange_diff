using System;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Transport.Responders
{
	public class ComponentStateBasedEscalateResponder : EscalateResponder
	{
		protected override bool ShouldRaiseActiveMonitoringAlerts(EscalationEnvironment environment)
		{
			if (!base.ShouldRaiseActiveMonitoringAlerts(environment))
			{
				return false;
			}
			string text = base.Definition.Attributes["ServerComponentName"];
			string text2 = base.Definition.Attributes["ExpectedComponentState"];
			string arg;
			if (ComponentState.VerifyExpectedState(text, text2, out arg))
			{
				return true;
			}
			base.Result.StateAttribute1 = string.Format("Component:{0} actual state: {1}, expected state: {2}, suppressing Responder", text, arg, text2.ToString());
			return false;
		}

		internal const string ServerComponentName = "ServerComponentName";

		internal const string ExpectedComponentState = "ExpectedComponentState";
	}
}
