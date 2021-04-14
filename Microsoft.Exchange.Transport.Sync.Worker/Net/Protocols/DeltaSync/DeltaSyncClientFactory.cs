using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxTransport.ContentAggregation;
using Microsoft.Exchange.Net.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Logging;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class DeltaSyncClientFactory
	{
		protected DeltaSyncClientFactory()
		{
		}

		public static DeltaSyncClientFactory Instance
		{
			get
			{
				return DeltaSyncClientFactory.instance;
			}
		}

		public virtual IDeltaSyncClient Create(DeltaSyncUserAccount userAccount, int remoteConnectionTimeout, IWebProxy proxy, long maxDownloadSizePerMessage, ProtocolLog httpProtocolLog, SyncLogSession syncLogSession, EventHandler<RoundtripCompleteEventArgs> roundtripCompleteEventHandler)
		{
			return new DeltaSyncClient(userAccount, remoteConnectionTimeout, proxy, maxDownloadSizePerMessage, httpProtocolLog, syncLogSession, roundtripCompleteEventHandler);
		}

		private static readonly DeltaSyncClientFactory instance = new DeltaSyncClientFactory();
	}
}
