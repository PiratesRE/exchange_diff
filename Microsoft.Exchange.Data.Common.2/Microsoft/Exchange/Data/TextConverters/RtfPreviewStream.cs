using System;
using System.IO;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.TextConverters.Internal.Rtf;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal class RtfPreviewStream : Stream
	{
		public RtfPreviewStream(Stream inputRtfStream, int inputBufferSize)
		{
			this.InputRtfStream = inputRtfStream;
			this.Parser = new RtfParserBase(inputBufferSize, false, null);
			int offset;
			int num = this.Parser.GetBufferSpace(false, out offset);
			num = this.InputRtfStream.Read(this.Parser.ParseBuffer, offset, num);
			this.Parser.ReportMoreDataAvailable(num, num == 0);
			int num2 = 0;
			while (this.Parser.ParseRun())
			{
				RtfRunKind runKind = this.Parser.RunKind;
				if (runKind != RtfRunKind.Ignore)
				{
					if (runKind != RtfRunKind.Begin)
					{
						if (runKind != RtfRunKind.Keyword)
						{
							return;
						}
						if (num2++ > 10)
						{
							return;
						}
						if (this.Parser.KeywordId == 292)
						{
							if (this.Parser.KeywordValue >= 1)
							{
								this.rtfEncapsulation = RtfEncapsulation.Html;
								return;
							}
							break;
						}
						else if (this.Parser.KeywordId == 329)
						{
							this.rtfEncapsulation = RtfEncapsulation.Text;
							return;
						}
					}
					else if (num2++ != 0)
					{
						return;
					}
				}
			}
		}

		public RtfEncapsulation Encapsulation
		{
			get
			{
				return this.rtfEncapsulation;
			}
		}

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException(TextConvertersStrings.SeekUnsupported);
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException(TextConvertersStrings.SeekUnsupported);
			}
			set
			{
				throw new NotSupportedException(TextConvertersStrings.SeekUnsupported);
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException(TextConvertersStrings.SeekUnsupported);
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException(TextConvertersStrings.SeekUnsupported);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException(TextConvertersStrings.WriteUnsupported);
		}

		public override void Flush()
		{
			throw new NotSupportedException(TextConvertersStrings.WriteUnsupported);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (this.InputRtfStream == null)
			{
				throw new ObjectDisposedException("RtfPreviewStream");
			}
			int num = 0;
			if (this.InternalPosition != 2147483647)
			{
				if (this.Parser != null && this.InternalPosition < this.Parser.ParseEnd)
				{
					int num2 = Math.Min(this.Parser.ParseEnd - this.InternalPosition, count);
					Buffer.BlockCopy(this.Parser.ParseBuffer, 0, buffer, offset, num2);
					this.InternalPosition += num2;
					count -= num2;
					offset += num2;
					num += num2;
					if (this.InternalPosition == this.Parser.ParseEnd)
					{
						this.Parser = null;
					}
				}
				if (count != 0)
				{
					int num2 = this.InputRtfStream.Read(buffer, offset, count);
					num += num2;
				}
			}
			return num;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.InputRtfStream != null)
			{
				this.InputRtfStream.Dispose();
			}
			this.InputRtfStream = null;
			this.Parser = null;
			base.Dispose(disposing);
		}

		internal Stream InputRtfStream;

		internal RtfParserBase Parser;

		internal int InternalPosition;

		private RtfEncapsulation rtfEncapsulation;
	}
}
