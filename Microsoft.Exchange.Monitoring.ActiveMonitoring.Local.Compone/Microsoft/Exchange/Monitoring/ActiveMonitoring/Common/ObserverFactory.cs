using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal class ObserverFactory : IObserverFactory
	{
		public IObserver CreateObserver(string serverName)
		{
			return new Observer(serverName);
		}
	}
}
