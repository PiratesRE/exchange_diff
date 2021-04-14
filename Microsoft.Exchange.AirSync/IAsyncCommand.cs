using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal interface IAsyncCommand
	{
		bool ProcessingEventsEnabled { get; set; }

		bool PerUserTracingEnabled { get; }

		INotificationManagerContext Context { get; }

		void Consume(Event evt);

		void HandleAccountTerminated(NotificationManager.AsyncEvent evt);

		void HandleException(Exception ex);

		void HeartbeatCallback();

		void ReleaseNotificationManager(bool wasStolen);

		void SetContextDataInTls();

		uint GetHeartbeatInterval();
	}
}
