using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Storage.Messaging
{
	internal interface IMessagingDatabaseComponent : IStartableTransportComponent, ITransportComponent, IDiagnosable
	{
		IMessagingDatabase Database { get; }

		IEnumerable<RoutedQueueBase> Queues { get; }

		void SetLoadTimeDependencies(IMessagingDatabaseConfig config);

		IBootLoader CreateBootScanner();

		RoutedQueueBase GetQueue(NextHopSolutionKey queueNextHopSolutionKey);

		bool TryGetQueue(NextHopSolutionKey queueNextHopSolutionKey, out RoutedQueueBase queue);

		RoutedQueueBase GetOrAddQueue(NextHopSolutionKey queueNextHopSolutionKey);
	}
}
