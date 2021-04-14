using System;
using System.IO;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ConversationBodyTextReader : BodyTextReader
	{
		internal ConversationBodyTextReader(ICoreItem coreItem, BodyReadConfiguration configuration, Stream inputStream, long bytesLoadedForConversation, long maxBytesForConversation) : base(coreItem, configuration, inputStream)
		{
			this.bytesLoadedForConversation = bytesLoadedForConversation;
			this.maxBytes = maxBytesForConversation;
		}

		public long BytesRead
		{
			get
			{
				this.CheckDisposed();
				return this.bytesRead;
			}
		}

		public override int Read()
		{
			int num = base.Read();
			if (num != -1)
			{
				this.bytesRead += 2L;
				if (this.maxBytes > -1L && this.bytesRead + this.bytesLoadedForConversation > this.maxBytes)
				{
					throw new MessageLoadFailedInConversationException(new LocalizedString("Message body size exceeded the conversation threshold for loading"));
				}
			}
			return num;
		}

		public override int Read(char[] buffer, int index, int count)
		{
			int num = base.Read(buffer, index, count);
			if (num > 0)
			{
				this.bytesRead += (long)(num * 2);
				if (this.maxBytes > -1L && this.bytesRead + this.bytesLoadedForConversation > this.maxBytes)
				{
					throw new MessageLoadFailedInConversationException(new LocalizedString("Message body size exceeded the conversation threshold for loading"));
				}
			}
			return num;
		}

		private void CheckDisposed()
		{
			if (base.IsDisposed())
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		private readonly long bytesLoadedForConversation;

		private readonly long maxBytes = -1L;

		private long bytesRead;
	}
}
