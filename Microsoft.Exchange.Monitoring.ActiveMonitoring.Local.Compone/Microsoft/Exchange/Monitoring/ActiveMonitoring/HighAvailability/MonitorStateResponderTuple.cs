using System;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability
{
	internal struct MonitorStateResponderTuple
	{
		public MonitorStateTransition MonitorState;

		public ResponderDefinition Responder;
	}
}
