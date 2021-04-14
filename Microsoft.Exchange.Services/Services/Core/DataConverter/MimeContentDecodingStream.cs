using System;
using System.IO;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class MimeContentDecodingStream : DecodingStream
	{
		public MimeContentDecodingStream(TextWriter writer) : base(writer)
		{
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (this.leftOverBytesCount > 0)
			{
				int num = this.leftOverBytesCount;
				while (num < 3 && count > 0)
				{
					this.leftOverBytes[num++] = buffer[offset++];
					count--;
				}
				if (count == 0 && num < 3)
				{
					this.leftOverBytesCount = num;
					return;
				}
				this.writer.Write(Convert.ToBase64String(this.leftOverBytes));
			}
			this.leftOverBytesCount = count % 3;
			if (this.leftOverBytesCount > 0)
			{
				count -= this.leftOverBytesCount;
				if (this.leftOverBytes == null)
				{
					this.leftOverBytes = new byte[3];
				}
				for (int i = 0; i < this.leftOverBytesCount; i++)
				{
					this.leftOverBytes[i] = buffer[offset + count + i];
				}
			}
			this.writer.Write(Convert.ToBase64String(buffer, offset, count));
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.leftOverBytesCount > 0)
			{
				this.writer.Write(Convert.ToBase64String(this.leftOverBytes, 0, this.leftOverBytesCount));
			}
			base.Dispose(disposing);
		}

		private int leftOverBytesCount;

		private byte[] leftOverBytes;
	}
}
