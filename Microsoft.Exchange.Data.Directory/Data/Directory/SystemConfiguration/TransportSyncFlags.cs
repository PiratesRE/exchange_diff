using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	internal enum TransportSyncFlags
	{
		None = 0,
		TransportSyncEnabled = 1,
		TransportSyncHubHealthLogEnabled = 2,
		TransportSyncPopEnabled = 4,
		WindowsLiveHotmailTransportSyncEnabled = 8,
		TransportSyncExchangeEnabled = 32,
		TransportSyncImapEnabled = 64,
		HttpProtocolLogEnabled = 128,
		TransportSyncLogEnabled = 256,
		TransportSyncAccountsPoisonDetectionEnabled = 1024,
		TransportSyncDispatchEnabled = 512,
		TransportSyncMailboxLogEnabled = 2048,
		TransportSyncMailboxHealthLogEnabled = 4096,
		TransportSyncFacebookEnabled = 8192,
		TransportSyncLinkedInEnabled = 16384,
		All = 32751
	}
}
