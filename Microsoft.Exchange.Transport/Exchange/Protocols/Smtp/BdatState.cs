using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class BdatState
	{
		public BdatState(string messageId, Stream bdatStream, long chunkSize, long originalMessageSize, long messageSizeLimit, bool isEohSeen)
		{
			ArgumentValidator.ThrowIfNull("bdatStream", bdatStream);
			this.MessageId = messageId;
			this.BdatStream = bdatStream;
			this.AccumulatedChunkSize = chunkSize;
			this.OriginalMessageSize = originalMessageSize;
			this.MessageSizeLimit = messageSizeLimit;
			this.IsEohSeen = isEohSeen;
		}

		public string MessageId { get; private set; }

		public Stream BdatStream { get; private set; }

		public long AccumulatedChunkSize { get; private set; }

		public long OriginalMessageSize { get; private set; }

		public long MessageSizeLimit { get; private set; }

		public bool IsEohSeen { get; private set; }

		public void IncrementAccumulatedChunkSize(long chunkSize)
		{
			this.AccumulatedChunkSize += chunkSize;
		}

		public void UpdateState(string messageId, long originalMessageSize, long messageSizeLimit, bool isEohSeen)
		{
			this.MessageId = messageId;
			this.OriginalMessageSize = originalMessageSize;
			this.MessageSizeLimit = messageSizeLimit;
			this.IsEohSeen = isEohSeen;
		}
	}
}
