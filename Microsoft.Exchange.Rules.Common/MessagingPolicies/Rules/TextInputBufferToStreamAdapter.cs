using System;
using System.IO;
using Microsoft.Exchange.TextMatching;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class TextInputBufferToStreamAdapter : Stream
	{
		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int num = 0;
			do
			{
				int nextChar;
				while ((nextChar = this.textInputBuffer.NextChar) == 1)
				{
				}
				if (nextChar <= 1)
				{
					break;
				}
				buffer[offset + num] = (byte)nextChar;
			}
			while (++num != count);
			return num;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		public override void Flush()
		{
			throw new NotSupportedException();
		}

		public TextInputBufferToStreamAdapter(ITextInputBuffer textInputBuffer)
		{
			if (textInputBuffer == null)
			{
				throw new ArgumentNullException();
			}
			this.textInputBuffer = textInputBuffer;
		}

		private ITextInputBuffer textInputBuffer;
	}
}
