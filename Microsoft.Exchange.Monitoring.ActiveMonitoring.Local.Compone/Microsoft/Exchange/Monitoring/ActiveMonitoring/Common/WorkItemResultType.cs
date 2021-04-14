using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	[Serializable]
	internal enum WorkItemResultType
	{
		Probe,
		Monitor,
		Responder
	}
}
