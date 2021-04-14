using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal interface IObserver
	{
		string ServerName { get; }

		bool IsInMaintenance { get; }

		IEnumerable<ISubject> GetAllSubjects();

		bool TryAddSubject(ISubject subject);

		void RemoveSubject(ISubject subject);

		ObserverHeartbeatResponse SendHeartbeat(ISubject subject);
	}
}
