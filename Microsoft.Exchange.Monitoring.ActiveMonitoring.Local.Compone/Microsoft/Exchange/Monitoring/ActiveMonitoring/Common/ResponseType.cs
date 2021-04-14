using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	public enum ResponseType
	{
		success,
		failure,
		sendMore,
		error,
		bye,
		unknown
	}
}
