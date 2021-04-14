using System;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class SmtpInBdatProxyParser : SmtpInParser, IDisposable
	{
		public SmtpInBdatProxyParser(MimeDocument headersDocument)
		{
			if (headersDocument == null)
			{
				throw new ArgumentNullException("headersDocument");
			}
			this.headersDocument = headersDocument;
			this.headersDocument.DangerousSetFixBadMimeBoundary(false);
			if (this.headersDocument.RootPart == null)
			{
				this.headersDocument.RootPart = new MimePart();
			}
		}

		public SmtpInDataProxyParser.EndOfHeadersCallback EndOfHeaders
		{
			get
			{
				return this.endOfHeaders;
			}
			set
			{
				this.endOfHeaders = value;
			}
		}

		public HeaderList ParsedHeaders
		{
			get
			{
				return this.parsedHeaders;
			}
		}

		public ParserState State
		{
			get
			{
				return this.parserState;
			}
		}

		public long ChunkSize
		{
			get
			{
				return this.chunkSize;
			}
			set
			{
				if (value < 0L)
				{
					throw new InvalidOperationException("chunk size cannot be negative");
				}
				this.chunkSize = value;
			}
		}

		public bool IsLastChunk
		{
			get
			{
				return this.isLastChunk;
			}
			set
			{
				this.isLastChunk = value;
			}
		}

		public override bool IsEodSeen
		{
			get
			{
				return this.bytesRead >= this.ChunkSize;
			}
		}

		public void StartDiscardingMessage()
		{
			base.IsDiscardingData = true;
			if (this.endOfHeaders != null)
			{
				this.endOfHeaders = null;
			}
		}

		public void ResetForNewBdatCommand(long chunkSize, bool discardingData, bool isLastChunk, SmtpInDataProxyParser.EndOfHeadersCallback endOfHeadersCallback, ExceptionFilter exceptionFilter)
		{
			this.chunkSize = chunkSize;
			base.IsDiscardingData = discardingData;
			this.isLastChunk = isLastChunk;
			this.EndOfHeaders = endOfHeadersCallback;
			base.ExceptionFilter = exceptionFilter;
			this.bytesRead = 0L;
		}

		public override void Reset()
		{
			base.Reset();
			this.parserState = ParserState.LF1;
			this.isLastChunk = false;
			this.chunkSize = 0L;
			this.parserState = ParserState.LF1;
			if (this.preEohDataStream != null)
			{
				this.preEohDataStream.Dispose();
			}
			if (this.postEohDataStream != null)
			{
				this.postEohDataStream.Dispose();
			}
			this.preEohDataStream = new MemoryStream();
			this.postEohDataStream = new MemoryStream();
			this.accumulatePostEohData = true;
			this.endOfHeaders = null;
			this.parsedHeaders = null;
		}

		public override bool Write(byte[] data, int offset, int numBytes, out int numBytesConsumed)
		{
			if (numBytes < 0)
			{
				throw new LocalizedException(Strings.SmtpReceiveParserNegativeBytes);
			}
			long val = this.chunkSize - this.bytesRead;
			numBytesConsumed = (int)Math.Min(val, (long)numBytes);
			int num = offset;
			int num2 = offset + numBytesConsumed;
			int num3 = -1;
			bool flag = false;
			while (num < num2 && base.EohPos == -1L)
			{
				switch (this.parserState)
				{
				case ParserState.NONE:
				{
					int num4 = Array.IndexOf<byte>(data, 13, num, num2 - num);
					if (num4 >= 0)
					{
						this.parserState = ParserState.CR1;
						num = num4 + 1;
						continue;
					}
					num = num2;
					continue;
				}
				case ParserState.CR1:
					if (data[num] == 10)
					{
						this.parserState = ParserState.LF1;
					}
					else if (data[num] == 13)
					{
						this.parserState = ParserState.CR1;
					}
					else
					{
						this.parserState = ParserState.NONE;
					}
					num++;
					continue;
				case ParserState.LF1:
					if (data[num] == 13)
					{
						if (base.EohPos != -1L)
						{
							this.parserState = ParserState.CR1;
						}
						else
						{
							this.parserState = ParserState.EOHCR2;
						}
					}
					else
					{
						this.parserState = ParserState.NONE;
					}
					num++;
					continue;
				case ParserState.EOHCR2:
					if (data[num] == 10)
					{
						base.EohPos = base.TotalBytesRead + (long)num - (long)offset - 1L;
						num3 = num + 1;
						this.parserState = ParserState.LF1;
						flag = true;
					}
					else if (data[num] == 13)
					{
						this.parserState = ParserState.CR1;
					}
					else
					{
						this.parserState = ParserState.NONE;
					}
					num++;
					continue;
				}
				throw new InvalidOperationException("SmtpInBdatParser got into an unknown state");
			}
			this.bytesRead += (long)numBytesConsumed;
			if (base.EohPos == -1L && this.isLastChunk && this.bytesRead >= this.chunkSize)
			{
				base.EohPos = base.TotalBytesRead + (long)num - (long)offset;
				num3 = num;
			}
			base.TotalBytesRead += (long)numBytesConsumed;
			bool result = this.bytesRead >= this.chunkSize;
			if (!base.IsDiscardingData)
			{
				base.TotalBytesWritten += (long)numBytesConsumed;
				int num5;
				if (base.EohPos != -1L && this.parsedHeaders == null)
				{
					num5 = num3 - offset;
					if (num5 > 0)
					{
						this.preEohDataStream.Write(data, offset, num5);
					}
					offset = num3;
					try
					{
						this.parsedHeaders = SmtpInDataProxyParser.GetHeadersFromStream(this.preEohDataStream, this.headersDocument);
						if (flag)
						{
							this.postEohDataStream.Write(SmtpInParser.EodSequence, 3, 2);
						}
						if (this.parsedHeaders != null)
						{
							this.endOfHeaders(this.ParsedHeaders);
						}
					}
					catch (IOException e)
					{
						base.FilterException(e);
						base.IsDiscardingData = true;
						return result;
					}
					catch (ExchangeDataException e2)
					{
						base.FilterException(e2);
						base.IsDiscardingData = true;
						return result;
					}
				}
				num5 = num2 - offset;
				if (num5 > 0)
				{
					if (this.parsedHeaders == null)
					{
						this.preEohDataStream.Write(data, offset, num5);
					}
					else if (this.accumulatePostEohData)
					{
						this.postEohDataStream.Write(data, offset, num5);
					}
				}
			}
			return result;
		}

		public byte[] GetAccumulatedBytesForProxying()
		{
			if (base.EohPos == -1L)
			{
				throw new InvalidOperationException("Cannot get bytes to proxy before End Of Headers");
			}
			this.accumulatePostEohData = false;
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				this.parsedHeaders.WriteTo(memoryStream);
				byte[] array = new byte[memoryStream.Length + this.postEohDataStream.Length];
				SmtpInDataProxyParser.CopyMemoryStreamToByteArray(memoryStream, array, 0L, memoryStream.Length);
				SmtpInDataProxyParser.CopyMemoryStreamToByteArray(this.postEohDataStream, array, memoryStream.Length, this.postEohDataStream.Length);
				result = array;
			}
			return result;
		}

		public void Dispose()
		{
			if (this.preEohDataStream != null)
			{
				this.preEohDataStream.Dispose();
				this.preEohDataStream = null;
			}
			if (this.postEohDataStream != null)
			{
				this.postEohDataStream.Dispose();
				this.postEohDataStream = null;
			}
		}

		private ParserState parserState = ParserState.LF1;

		private MemoryStream preEohDataStream = new MemoryStream();

		private MemoryStream postEohDataStream = new MemoryStream();

		private HeaderList parsedHeaders;

		private MimeDocument headersDocument;

		private SmtpInDataProxyParser.EndOfHeadersCallback endOfHeaders;

		private bool accumulatePostEohData = true;

		private bool isLastChunk;

		private long chunkSize;

		private long bytesRead;
	}
}
