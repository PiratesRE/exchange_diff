using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Globalization;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal class ConverterEncodingOutput : ConverterOutput, IByteSource, IRestartable, IReusable
	{
		public ConverterEncodingOutput(Stream destination, bool push, bool restartable, Encoding encoding, bool encodingSameAsInput, bool testBoundaryConditions, IResultsFeedback resultFeedback)
		{
			this.resultFeedback = resultFeedback;
			if (!push)
			{
				this.pullSink = (destination as ConverterStream);
				this.pullSink.SetSource(this);
			}
			else
			{
				this.pushSink = destination;
				if (restartable && destination.CanSeek && destination.Position == destination.Length)
				{
					this.restartablePushSink = true;
					this.restartPosition = destination.Position;
				}
			}
			this.canRestart = restartable;
			this.restartable = restartable;
			this.lineBuffer = new char[4096];
			this.minCharsEncode = (testBoundaryConditions ? 1 : 256);
			this.encodingSameAsInput = encodingSameAsInput;
			this.originalEncoding = encoding;
			this.ChangeEncoding(encoding);
			if (this.resultFeedback != null)
			{
				this.resultFeedback.Set(ConfigParameter.OutputEncoding, this.encoding);
			}
		}

		private void Reinitialize()
		{
			this.endOfFile = false;
			this.lineBufferCount = 0;
			this.isFirstChar = true;
			this.cache.Reset();
			this.encoding = null;
			this.ChangeEncoding(this.originalEncoding);
			this.canRestart = this.restartable;
		}

		public Encoding Encoding
		{
			get
			{
				return this.encoding;
			}
			set
			{
				if (this.encoding != value)
				{
					this.ChangeEncoding(value);
					if (this.resultFeedback != null)
					{
						this.resultFeedback.Set(ConfigParameter.OutputEncoding, this.encoding);
					}
				}
			}
		}

		public bool CodePageSameAsInput
		{
			get
			{
				return this.encodingSameAsInput;
			}
		}

		bool IRestartable.CanRestart()
		{
			return this.canRestart;
		}

		void IRestartable.Restart()
		{
			if (this.pullSink == null && this.restartablePushSink)
			{
				this.pushSink.Position = this.restartPosition;
				this.pushSink.SetLength(this.restartPosition);
			}
			this.Reinitialize();
			this.canRestart = false;
		}

		void IRestartable.DisableRestart()
		{
			this.canRestart = false;
			this.FlushCached();
		}

		void IReusable.Initialize(object newSourceOrDestination)
		{
			this.restartablePushSink = false;
			if (this.pushSink != null && newSourceOrDestination != null)
			{
				Stream stream = newSourceOrDestination as Stream;
				if (stream == null || !stream.CanWrite)
				{
					throw new InvalidOperationException("cannot reinitialize this converter - new output should be a writable Stream object");
				}
				this.pushSink = stream;
				if (this.restartable && stream.CanSeek && stream.Position == stream.Length)
				{
					this.restartablePushSink = true;
					this.restartPosition = stream.Position;
				}
			}
			this.Reinitialize();
		}

		public override bool CanAcceptMore
		{
			get
			{
				return this.canRestart || this.pullSink == null || this.cache.Length == 0;
			}
		}

		public override void Write(char[] buffer, int offset, int count, IFallback fallback)
		{
			if (fallback == null && !this.lineModeEncoding && this.lineBufferCount + count <= this.lineBuffer.Length - this.minCharsEncode)
			{
				if (count == 1)
				{
					this.lineBuffer[this.lineBufferCount++] = buffer[offset];
					return;
				}
				if (count < 16)
				{
					if ((count & 8) != 0)
					{
						this.lineBuffer[this.lineBufferCount] = buffer[offset];
						this.lineBuffer[this.lineBufferCount + 1] = buffer[offset + 1];
						this.lineBuffer[this.lineBufferCount + 2] = buffer[offset + 2];
						this.lineBuffer[this.lineBufferCount + 3] = buffer[offset + 3];
						this.lineBuffer[this.lineBufferCount + 4] = buffer[offset + 4];
						this.lineBuffer[this.lineBufferCount + 5] = buffer[offset + 5];
						this.lineBuffer[this.lineBufferCount + 6] = buffer[offset + 6];
						this.lineBuffer[this.lineBufferCount + 7] = buffer[offset + 7];
						this.lineBufferCount += 8;
						offset += 8;
					}
					if ((count & 4) != 0)
					{
						this.lineBuffer[this.lineBufferCount] = buffer[offset];
						this.lineBuffer[this.lineBufferCount + 1] = buffer[offset + 1];
						this.lineBuffer[this.lineBufferCount + 2] = buffer[offset + 2];
						this.lineBuffer[this.lineBufferCount + 3] = buffer[offset + 3];
						this.lineBufferCount += 4;
						offset += 4;
					}
					if ((count & 2) != 0)
					{
						this.lineBuffer[this.lineBufferCount] = buffer[offset];
						this.lineBuffer[this.lineBufferCount + 1] = buffer[offset + 1];
						this.lineBufferCount += 2;
						offset += 2;
					}
					if ((count & 1) != 0)
					{
						this.lineBuffer[this.lineBufferCount++] = buffer[offset];
					}
					return;
				}
			}
			this.WriteComplete(buffer, offset, count, fallback);
		}

		public void WriteComplete(char[] buffer, int offset, int count, IFallback fallback)
		{
			int num = 0;
			if (fallback != null || this.lineModeEncoding)
			{
				byte b = 0;
				byte[] array = null;
				uint num2 = 0U;
				bool flag = false;
				bool flag2 = false;
				if (fallback != null)
				{
					array = fallback.GetUnsafeAsciiMap(out b);
					if (array != null)
					{
						num2 = (uint)array.Length;
					}
					flag = fallback.HasUnsafeUnicode();
					flag2 = fallback.TreatNonAsciiAsUnsafe(this.encoding.WebName);
				}
				while (count != 0)
				{
					while (count != 0 && this.lineBufferCount != this.lineBuffer.Length)
					{
						char c = buffer[offset];
						if (fallback != null && (((uint)c < num2 && (array[(int)c] & b) != 0) || (!this.encodingCompleteUnicode && (c >= '\u007f' || c < ' ') && this.codePageMap.IsUnsafeExtendedCharacter(c)) || (flag && c >= '\u007f' && (flag2 || fallback.IsUnsafeUnicode(c, this.isFirstChar)))))
						{
							if (!fallback.FallBackChar(c, this.lineBuffer, ref this.lineBufferCount, this.lineBuffer.Length))
							{
								break;
							}
							this.isFirstChar = false;
						}
						else
						{
							this.lineBuffer[this.lineBufferCount++] = c;
							this.isFirstChar = false;
							if (this.lineModeEncoding)
							{
								if (c == '\n' || c == '\r')
								{
									num = this.lineBufferCount;
								}
								else if (num > this.lineBuffer.Length - 256)
								{
									count--;
									offset++;
									break;
								}
							}
						}
						count--;
						offset++;
					}
					if (this.lineModeEncoding && (num > this.lineBuffer.Length - 256 || (this.lineBufferCount > this.lineBuffer.Length - 32 && num != 0)))
					{
						this.EncodeBuffer(this.lineBuffer, 0, num, false);
						this.lineBufferCount -= num;
						if (this.lineBufferCount != 0)
						{
							Buffer.BlockCopy(this.lineBuffer, num * 2, this.lineBuffer, 0, this.lineBufferCount * 2);
						}
					}
					else if (this.lineBufferCount > this.lineBuffer.Length - Math.Max(this.minCharsEncode, 32))
					{
						this.EncodeBuffer(this.lineBuffer, 0, this.lineBufferCount, false);
						this.lineBufferCount = 0;
					}
					num = 0;
				}
				return;
			}
			if (count > this.minCharsEncode)
			{
				if (this.lineBufferCount != 0)
				{
					this.EncodeBuffer(this.lineBuffer, 0, this.lineBufferCount, false);
					this.lineBufferCount = 0;
				}
				this.EncodeBuffer(buffer, offset, count, false);
				return;
			}
			Buffer.BlockCopy(buffer, offset * 2, this.lineBuffer, this.lineBufferCount * 2, count * 2);
			this.lineBufferCount += count;
			if (this.lineBufferCount > this.lineBuffer.Length - this.minCharsEncode)
			{
				this.EncodeBuffer(this.lineBuffer, 0, this.lineBufferCount, false);
				this.lineBufferCount = 0;
			}
		}

		public override void Write(string text)
		{
			if (text.Length == 0)
			{
				return;
			}
			if (this.lineModeEncoding || this.lineBufferCount + text.Length > this.lineBuffer.Length - this.minCharsEncode)
			{
				base.Write(text, 0, text.Length);
				return;
			}
			if (text.Length <= 4)
			{
				int num = text.Length;
				this.lineBuffer[this.lineBufferCount++] = text[0];
				if (--num != 0)
				{
					this.lineBuffer[this.lineBufferCount++] = text[1];
					if (--num != 0)
					{
						this.lineBuffer[this.lineBufferCount++] = text[2];
						if (num - 1 != 0)
						{
							this.lineBuffer[this.lineBufferCount++] = text[3];
							return;
						}
					}
				}
			}
			else
			{
				text.CopyTo(0, this.lineBuffer, this.lineBufferCount, text.Length);
				this.lineBufferCount += text.Length;
			}
		}

		public override void Flush()
		{
			if (this.endOfFile)
			{
				return;
			}
			this.canRestart = false;
			this.FlushCached();
			this.EncodeBuffer(this.lineBuffer, 0, this.lineBufferCount, true);
			this.lineBufferCount = 0;
			if (this.pullSink == null)
			{
				this.pushSink.Flush();
			}
			else if (this.cache.Length == 0)
			{
				this.pullSink.ReportEndOfFile();
			}
			this.endOfFile = true;
		}

		bool IByteSource.GetOutputChunk(out byte[] chunkBuffer, out int chunkOffset, out int chunkLength)
		{
			if (this.cache.Length == 0 || this.canRestart)
			{
				chunkBuffer = null;
				chunkOffset = 0;
				chunkLength = 0;
				return false;
			}
			this.cache.GetData(out chunkBuffer, out chunkOffset, out chunkLength);
			return true;
		}

		void IByteSource.ReportOutput(int readCount)
		{
			this.cache.ReportRead(readCount);
			if (this.cache.Length == 0 && this.endOfFile)
			{
				this.pullSink.ReportEndOfFile();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.cache != null && this.cache is IDisposable)
			{
				((IDisposable)this.cache).Dispose();
			}
			this.cache = null;
			this.pushSink = null;
			this.pullSink = null;
			this.lineBuffer = null;
			this.encoding = null;
			this.encoder = null;
			this.codePageMap = null;
			base.Dispose(disposing);
		}

		private void EncodeBuffer(char[] buffer, int offset, int count, bool flush)
		{
			int maxByteCount = this.encoding.GetMaxByteCount(count);
			byte[] array = null;
			int num = 0;
			int num2 = 0;
			bool flag = true;
			byte[] array2;
			int num3;
			if (this.canRestart || this.pullSink == null || this.cache.Length != 0)
			{
				this.cache.GetBuffer(maxByteCount, out array2, out num3);
			}
			else
			{
				this.pullSink.GetOutputBuffer(out array, out num, out num2);
				if (num2 >= maxByteCount)
				{
					array2 = array;
					num3 = num;
					flag = false;
				}
				else
				{
					this.cache.GetBuffer(maxByteCount, out array2, out num3);
				}
			}
			int num4 = this.encoder.GetBytes(buffer, offset, count, array2, num3, flush);
			if (this.reportBytes != null)
			{
				this.reportBytes.ReportBytesWritten(num4);
			}
			if (flag)
			{
				this.cache.Commit(num4);
				if (this.pullSink == null)
				{
					if (this.canRestart)
					{
						if (!this.restartablePushSink)
						{
							return;
						}
					}
					while (this.cache.Length != 0)
					{
						int count2;
						this.cache.GetData(out array2, out num3, out count2);
						this.pushSink.Write(array2, num3, count2);
						this.cache.ReportRead(count2);
					}
					return;
				}
				if (!this.canRestart)
				{
					num4 = this.cache.Read(array, num, num2);
					this.pullSink.ReportOutput(num4);
					return;
				}
			}
			else
			{
				this.pullSink.ReportOutput(num4);
			}
		}

		internal void ChangeEncoding(Encoding newEncoding)
		{
			if (this.encoding != null)
			{
				this.EncodeBuffer(this.lineBuffer, 0, this.lineBufferCount, true);
				this.lineBufferCount = 0;
			}
			this.encoding = newEncoding;
			this.encoder = newEncoding.GetEncoder();
			int codePage = CodePageMap.GetCodePage(newEncoding);
			if (codePage == 1200 || codePage == 1201 || codePage == 12000 || codePage == 12001 || codePage == 65000 || codePage == 65001 || codePage == 65005 || codePage == 65006 || codePage == 54936)
			{
				this.lineModeEncoding = false;
				this.encodingCompleteUnicode = true;
				this.codePageMap.ChoseCodePage(1200);
				return;
			}
			this.encodingCompleteUnicode = false;
			this.codePageMap.ChoseCodePage(codePage);
			if (codePage == 50220 || codePage == 50221 || codePage == 50222 || codePage == 50225 || codePage == 50227 || codePage == 50229 || codePage == 52936)
			{
				this.lineModeEncoding = true;
			}
		}

		private bool FlushCached()
		{
			if (this.canRestart || this.cache.Length == 0)
			{
				return false;
			}
			if (this.pullSink == null)
			{
				while (this.cache.Length != 0)
				{
					byte[] buffer;
					int offset;
					int num;
					this.cache.GetData(out buffer, out offset, out num);
					this.pushSink.Write(buffer, offset, num);
					this.cache.ReportRead(num);
				}
			}
			else
			{
				byte[] buffer;
				int offset;
				int count;
				this.pullSink.GetOutputBuffer(out buffer, out offset, out count);
				int num = this.cache.Read(buffer, offset, count);
				this.pullSink.ReportOutput(num);
			}
			return true;
		}

		private const int LineSpaceThreshold = 256;

		private const int SpaceThreshold = 32;

		protected IResultsFeedback resultFeedback;

		private Stream pushSink;

		private ConverterStream pullSink;

		private bool endOfFile;

		private bool restartablePushSink;

		private long restartPosition;

		private bool encodingSameAsInput;

		private bool restartable;

		private bool canRestart;

		private bool lineModeEncoding;

		private int minCharsEncode;

		private char[] lineBuffer;

		private int lineBufferCount;

		private ByteCache cache = new ByteCache();

		private Encoding originalEncoding;

		private Encoding encoding;

		private Encoder encoder;

		private bool encodingCompleteUnicode;

		private CodePageMap codePageMap = new CodePageMap();

		private bool isFirstChar = true;
	}
}
