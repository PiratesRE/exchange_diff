using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class MessageWithBuffer : DataMessageBase
	{
		public MessageWithBuffer(byte[] buffer)
		{
			this.buffer = buffer;
		}

		public byte[] Buffer
		{
			get
			{
				return this.buffer;
			}
		}

		protected override int GetSizeInternal()
		{
			if (this.buffer == null)
			{
				return 0;
			}
			return this.buffer.Length;
		}

		private byte[] buffer;
	}
}
