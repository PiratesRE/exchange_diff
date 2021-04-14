using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Connections.Common
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IMonitorEvents
	{
		EventHandler<DownloadCompleteEventArgs> DownloadsCompletedEventHandler { get; }

		EventHandler<EventArgs> MessagesDownloadedEventHandler { get; }

		EventHandler<EventArgs> MessagesUploadedEventHandler { get; }

		EventHandler<RoundtripCompleteEventArgs> RoundtripCompleteEventHandler { get; }
	}
}
