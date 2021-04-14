using System;

namespace Microsoft.Exchange.HttpProxy.RouteRefresher
{
	internal interface IRouteRefresherDiagnostics
	{
		void AddErrorInfo(object value);

		void AddGenericInfo(object value);

		void IncrementSuccessfulMailboxServerCacheUpdates();

		void IncrementTotalMailboxServerCacheUpdateAttempts();

		void IncrementSuccessfulAnchorMailboxCacheUpdates();

		void IncrementTotalAnchorMailboxCacheUpdateAttempts();

		void LogRouteRefresherLatency(Action operationToTrack);
	}
}
