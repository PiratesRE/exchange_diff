using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Logging.MessageTracking;
using Microsoft.Exchange.Transport.Sync.Common.Logging;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class MailSubmitter
	{
		private event EventHandler<EventArgs> AggregatedMailSubmitted;

		public abstract TransportMailItem CreateNewMail();

		public abstract Stream GetWriteStream(TransportMailItem mailItem);

		public abstract Exception SubmitMail(string componentId, SyncLogSession syncLogSession, TransportMailItem mailItem, ISyncEmail syncEmail, IList<string> recipients, string deliveryFolder, Guid subscriptionGuid, string cloudId, string cloudVersion, DateTime originalReceivedTime, MsgTrackReceiveInfo msgTrackInfo);

		public void EnsureRegisteredEventHandler(EventHandler<EventArgs> aggregatedMailSubmitted)
		{
			if (this.AggregatedMailSubmitted == null)
			{
				lock (this.syncObject)
				{
					if (this.AggregatedMailSubmitted == null)
					{
						this.AggregatedMailSubmitted += aggregatedMailSubmitted;
					}
				}
			}
		}

		protected void TriggerMailSubmittedEvent(object sender, EventArgs eventArgs)
		{
			EventHandler<EventArgs> aggregatedMailSubmitted = this.AggregatedMailSubmitted;
			if (aggregatedMailSubmitted != null)
			{
				aggregatedMailSubmitted(sender, eventArgs);
			}
		}

		private object syncObject = new object();
	}
}
