using System;
using System.Collections.Generic;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ProcessIsolation.Monitors
{
	internal class ProcessConfiguration
	{
		internal ProcessConfiguration(Component component, ProcessType processType, ProcessResponderConfiguration responders) : this(component, processType, responders, new Dictionary<string, object>())
		{
		}

		internal ProcessConfiguration(Component component, ProcessType processType, ProcessResponderConfiguration responders, Dictionary<string, object> parameters) : this(component, processType, responders, parameters, null)
		{
		}

		internal ProcessConfiguration(Component component, ProcessType processType, ProcessResponderConfiguration responders, Dictionary<string, object> parameters, Func<bool> shouldRunOnLocalServer)
		{
			this.Component = component;
			this.ProcessType = processType;
			this.Responders = responders;
			this.Parameters = parameters;
			this.ShouldRunOnLocalServer = shouldRunOnLocalServer;
		}

		internal Component Component { get; private set; }

		internal ProcessType ProcessType { get; private set; }

		internal ProcessResponderConfiguration Responders { get; private set; }

		internal Dictionary<string, object> Parameters { get; private set; }

		internal Func<bool> ShouldRunOnLocalServer { get; private set; }
	}
}
