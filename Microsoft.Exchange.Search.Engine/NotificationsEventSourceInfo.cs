using System;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.Mdb;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Search.Engine
{
	internal class NotificationsEventSourceInfo
	{
		public NotificationsEventSourceInfo(IWatermarkStorage watermarkStorage, INotificationsEventSource eventSource, IDiagnosticsSession diagnosticsSession, MdbInfo mdbInfo)
		{
			this.FirstEvent = eventSource.ReadFirstEventCounter();
			this.NotificationsWatermark = watermarkStorage.GetNotificationsWatermark();
			long num = Math.Max(0L, this.NotificationsWatermark);
			this.LastEvent = eventSource.ReadLastEvent().EventCounter;
			this.WatermarkDelta = this.LastEvent - num;
			long num2;
			MapiEvent[] array = eventSource.ReadEvents(num, 1, ReadEventsFlags.IncludeMoveDestinationEvents, out num2);
			DateTime d = (array.Length > 0) ? array[0].CreateTime : DateTime.UtcNow;
			this.CurrentEventAge = ((this.WatermarkDelta > 0L) ? TimeSpan.FromSeconds((double)Math.Max(0, (int)(DateTime.UtcNow - d).TotalSeconds)) : TimeSpan.Zero);
			if (array.Length == 0)
			{
				diagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, "Watermark events not found for Mdb [{0}] - Setting current event time to now", new object[]
				{
					mdbInfo
				});
			}
		}

		public long FirstEvent { get; private set; }

		public long LastEvent { get; private set; }

		public long WatermarkDelta { get; private set; }

		public TimeSpan CurrentEventAge { get; private set; }

		public long NotificationsWatermark { get; private set; }
	}
}
