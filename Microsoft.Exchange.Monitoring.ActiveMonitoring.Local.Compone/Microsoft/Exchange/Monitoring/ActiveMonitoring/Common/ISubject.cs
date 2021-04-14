using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal interface ISubject
	{
		string ServerName { get; }

		bool IsInMaintenance { get; }

		IEnumerable<IObserver> GetAllObservers();

		bool TryAddObserver(IObserver observer);

		void RemoveObserver(IObserver observer);

		bool SendRequest(IObserver observer);

		void SendCancel(IObserver observer);

		DateTime? GetLastResultTimestamp();

		IPStatus Ping(TimeSpan timeout);

		DateTime? GetLastObserverSelectionTimestamp();

		DateTime? GetObserverHeartbeat(IObserver observer);
	}
}
