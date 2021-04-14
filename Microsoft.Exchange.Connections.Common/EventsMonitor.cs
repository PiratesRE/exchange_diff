using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Connections.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class EventsMonitor : IMonitorEvents
	{
		internal EventsMonitor(EventHandler<DownloadCompleteEventArgs> downloadsCompletedEventHandler, EventHandler<EventArgs> messagesDownloadedEventHandler, EventHandler<EventArgs> messagesUploadedEventHandler, EventHandler<RoundtripCompleteEventArgs> roundtripCompleteEventHandler)
		{
			this.DownloadsCompletedEventHandler = downloadsCompletedEventHandler;
			this.MessagesDownloadedEventHandler = messagesDownloadedEventHandler;
			this.MessagesUploadedEventHandler = messagesUploadedEventHandler;
			this.RoundtripCompleteEventHandler = roundtripCompleteEventHandler;
		}

		public EventHandler<DownloadCompleteEventArgs> DownloadsCompletedEventHandler { get; private set; }

		public EventHandler<EventArgs> MessagesDownloadedEventHandler { get; private set; }

		public EventHandler<EventArgs> MessagesUploadedEventHandler { get; private set; }

		public EventHandler<RoundtripCompleteEventArgs> RoundtripCompleteEventHandler { get; private set; }
	}
}
