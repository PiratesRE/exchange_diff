using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal interface IObserverFactory
	{
		IObserver CreateObserver(string serverName);
	}
}
