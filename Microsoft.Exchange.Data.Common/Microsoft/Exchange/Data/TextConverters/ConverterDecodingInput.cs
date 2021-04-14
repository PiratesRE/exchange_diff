using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal class ConverterDecodingInput : ConverterInput, IReusable
	{
		public ConverterDecodingInput(Stream source, bool push, Encoding encoding, bool detectEncodingFromByteOrderMark, int maxParseToken, int restartMax, int inputBufferSize, bool testBoundaryConditions, IResultsFeedback resultFeedback, IProgressMonitor progressMonitor) : base(progressMonitor)
		{
			this.resultFeedback = resultFeedback;
			this.restartMax = restartMax;
			if (push)
			{
				this.pushSource = (source as ConverterStream);
			}
			else
			{
				this.pullSource = source;
			}
			this.detectEncodingFromByteOrderMark = detectEncodingFromByteOrderMark;
			this.minDecodeBytes = (testBoundaryConditions ? 1 : 64);
			this.originalEncoding = encoding;
			this.SetNewEncoding(encoding);
			this.maxTokenSize = ((maxParseToken == int.MaxValue) ? maxParseToken : (testBoundaryConditions ? maxParseToken : ((maxParseToken + 1023) / 1024 * 1024)));
			this.parseBuffer = new char[testBoundaryConditions ? 55L : Math.Min(4096L, (long)this.maxTokenSize + (long)(this.minDecodeChars + 1))];
			if (this.pushSource != null)
			{
				this.readBuffer = new byte[Math.Max(this.minDecodeBytes * 2, 8)];
				return;
			}
			int num = Math.Max(this.CalculateMaxBytes(this.parseBuffer.Length), inputBufferSize);
			this.readBuffer = new byte[num];
		}

		private void Reinitialize()
		{
			this.parseStart = 0;
			this.parseEnd = 0;
			this.rawEndOfFile = false;
			this.SetNewEncoding(this.originalEncoding);
			this.encodingChanged = false;
			this.readFileOffset = 0;
			this.readCurrent = 0;
			this.readEnd = 0;
			this.pushChunkBuffer = null;
			this.pushChunkStart = 0;
			this.pushChunkCount = 0;
			this.pushChunkUsed = 0;
			if (this.restartCache != null)
			{
				this.restartCache.Reset();
			}
			this.restarting = false;
			this.endOfFile = false;
		}

		public Encoding Encoding
		{
			get
			{
				return this.encoding;
			}
		}

		public bool EncodingChanged
		{
			get
			{
				return this.encodingChanged;
			}
			set
			{
				this.encodingChanged = false;
			}
		}

		public override void SetRestartConsumer(IRestartable restartConsumer)
		{
			if (this.restartMax != 0 || restartConsumer == null)
			{
				this.restartConsumer = restartConsumer;
			}
		}

		public override bool ReadMore(ref char[] buffer, ref int start, ref int current, ref int end)
		{
			if (this.parseBuffer.Length - this.parseEnd <= this.minDecodeChars && !this.EnsureFreeSpace())
			{
				return true;
			}
			int num = 0;
			while ((!this.rawEndOfFile || this.readEnd - this.readCurrent != 0 || this.restarting) && this.parseBuffer.Length - this.parseEnd > this.minDecodeChars)
			{
				if (this.readEnd - this.readCurrent >= ((this.readFileOffset == 0) ? Math.Max(4, this.minDecodeBytes) : this.minDecodeBytes) || (this.rawEndOfFile && !this.restarting))
				{
					num += this.DecodeFromBuffer(this.readBuffer, ref this.readCurrent, this.readEnd, this.readFileOffset + this.readCurrent, this.rawEndOfFile);
				}
				else if (this.restarting)
				{
					byte[] buffer2;
					int num2;
					int end2;
					if (!this.GetRestartChunk(out buffer2, out num2, out end2))
					{
						this.restarting = false;
					}
					else
					{
						int num3 = num2;
						num += this.DecodeFromBuffer(buffer2, ref num2, end2, this.readFileOffset, false);
						this.readFileOffset += num2 - num3;
						this.ReportRestartChunkUsed(num2 - num3);
					}
				}
				else if (this.pushSource != null)
				{
					if (this.pushChunkCount == 0)
					{
						if (!this.pushSource.GetInputChunk(out this.pushChunkBuffer, out this.pushChunkStart, out this.pushChunkCount, out this.rawEndOfFile))
						{
							break;
						}
					}
					else if (this.pushChunkCount - this.pushChunkUsed == 0)
					{
						if (this.restartConsumer != null)
						{
							this.BackupForRestart(this.pushChunkBuffer, this.pushChunkStart, this.pushChunkCount, this.readFileOffset, false);
						}
						this.pushSource.ReportRead(this.pushChunkCount);
						this.readFileOffset += this.pushChunkCount;
						this.pushChunkCount = 0;
						this.pushChunkUsed = 0;
						break;
					}
					if (this.pushChunkCount - this.pushChunkUsed < ((this.readFileOffset == 0) ? Math.Max(4, this.minDecodeBytes) : this.minDecodeBytes))
					{
						if (this.pushChunkCount - this.pushChunkUsed != 0)
						{
							if (this.readBuffer.Length - this.readEnd < this.pushChunkCount - this.pushChunkUsed)
							{
								if (this.restartConsumer != null)
								{
									this.BackupForRestart(this.readBuffer, 0, this.readCurrent, this.readFileOffset, false);
								}
								Buffer.BlockCopy(this.readBuffer, this.readCurrent, this.readBuffer, 0, this.readEnd - this.readCurrent);
								this.readFileOffset += this.readCurrent;
								this.readEnd -= this.readCurrent;
								this.readCurrent = 0;
							}
							if (this.pushChunkUsed != 0)
							{
								if (this.restartConsumer != null)
								{
									this.BackupForRestart(this.pushChunkBuffer, this.pushChunkStart, this.pushChunkUsed, this.readFileOffset + this.readEnd, false);
								}
								this.readFileOffset += this.pushChunkUsed;
							}
							Buffer.BlockCopy(this.pushChunkBuffer, this.pushChunkStart + this.pushChunkUsed, this.readBuffer, this.readEnd, this.pushChunkCount - this.pushChunkUsed);
							this.readEnd += this.pushChunkCount - this.pushChunkUsed;
							this.pushSource.ReportRead(this.pushChunkCount);
							this.pushChunkCount = 0;
							this.pushChunkUsed = 0;
							if (this.readEnd - this.readCurrent < ((this.readFileOffset == 0) ? Math.Max(4, this.minDecodeBytes) : this.minDecodeBytes))
							{
								break;
							}
						}
						num += this.DecodeFromBuffer(this.readBuffer, ref this.readCurrent, this.readEnd, this.readFileOffset + this.readCurrent, this.rawEndOfFile);
					}
					else if (this.readEnd - this.readCurrent != 0)
					{
						if (this.readFileOffset == 0 && this.readCurrent == 0)
						{
							int num4 = Math.Max(4, this.minDecodeBytes) - (this.readEnd - this.readCurrent);
							Buffer.BlockCopy(this.pushChunkBuffer, this.pushChunkStart + this.pushChunkUsed, this.readBuffer, this.readEnd, num4);
							this.readEnd += num4;
							this.pushSource.ReportRead(num4);
							this.pushChunkCount -= num4;
							this.pushChunkStart += num4;
						}
						num += this.DecodeFromBuffer(this.readBuffer, ref this.readCurrent, this.readEnd, this.readFileOffset + this.readCurrent, false);
					}
					if (this.parseBuffer.Length - this.parseEnd > this.minDecodeChars && this.pushChunkCount - this.pushChunkUsed != 0 && this.readEnd - this.readCurrent == 0)
					{
						if (this.readEnd != 0)
						{
							if (this.restartConsumer != null)
							{
								this.BackupForRestart(this.readBuffer, 0, this.readCurrent, this.readFileOffset, false);
							}
							this.readFileOffset += this.readCurrent;
							this.readEnd = 0;
							this.readCurrent = 0;
						}
						int num5 = this.pushChunkStart + this.pushChunkUsed;
						num += this.DecodeFromBuffer(this.pushChunkBuffer, ref num5, this.pushChunkStart + this.pushChunkCount, this.readFileOffset + this.pushChunkUsed, false);
						this.pushChunkUsed = num5 - this.pushChunkStart;
					}
				}
				else
				{
					if (this.readBuffer.Length - this.readEnd < this.minDecodeBytes)
					{
						if (this.restartConsumer != null)
						{
							this.BackupForRestart(this.readBuffer, 0, this.readCurrent, this.readFileOffset, false);
						}
						Buffer.BlockCopy(this.readBuffer, this.readCurrent, this.readBuffer, 0, this.readEnd - this.readCurrent);
						this.readFileOffset += this.readCurrent;
						this.readEnd -= this.readCurrent;
						this.readCurrent = 0;
					}
					int num6 = this.pullSource.Read(this.readBuffer, this.readEnd, this.readBuffer.Length - this.readEnd);
					if (num6 == 0)
					{
						this.rawEndOfFile = true;
					}
					else
					{
						this.readEnd += num6;
						if (this.progressMonitor != null)
						{
							this.progressMonitor.ReportProgress();
						}
					}
					num += this.DecodeFromBuffer(this.readBuffer, ref this.readCurrent, this.readEnd, this.readFileOffset + this.readCurrent, this.rawEndOfFile);
				}
			}
			if (this.rawEndOfFile && this.readEnd - this.readCurrent == 0)
			{
				this.endOfFile = true;
			}
			if (buffer != this.parseBuffer)
			{
				buffer = this.parseBuffer;
			}
			if (start != this.parseStart)
			{
				current = this.parseStart + (current - start);
				start = this.parseStart;
			}
			end = this.parseEnd;
			return num != 0 || this.endOfFile || this.encodingChanged;
		}

		public override void ReportProcessed(int processedSize)
		{
			this.parseStart += processedSize;
		}

		public override int RemoveGap(int gapBegin, int gapEnd)
		{
			this.parseEnd = gapBegin;
			this.parseBuffer[gapBegin] = '\0';
			return gapBegin;
		}

		public bool RestartWithNewEncoding(Encoding newEncoding)
		{
			if (this.encoding == newEncoding)
			{
				if (this.restartConsumer != null)
				{
					this.restartConsumer.DisableRestart();
					this.restartConsumer = null;
					if (this.restartCache != null)
					{
						this.restartCache.Reset();
						this.restartCache = null;
					}
				}
				return false;
			}
			if (this.restartConsumer == null || !this.restartConsumer.CanRestart())
			{
				return false;
			}
			this.restartConsumer.Restart();
			this.SetNewEncoding(newEncoding);
			this.encodingChanged = true;
			if (this.readEnd != 0 && this.readFileOffset != 0)
			{
				this.BackupForRestart(this.readBuffer, 0, this.readEnd, this.readFileOffset, true);
				this.readEnd = 0;
				this.readFileOffset = 0;
			}
			this.readCurrent = 0;
			this.pushChunkUsed = 0;
			this.restartConsumer = null;
			this.parseStart = (this.parseEnd = 0);
			this.restarting = (this.restartCache != null && this.restartCache.Length != 0);
			return true;
		}

		private void SetNewEncoding(Encoding newEncoding)
		{
			this.encoding = newEncoding;
			this.decoder = this.encoding.GetDecoder();
			this.preamble = this.encoding.GetPreamble();
			this.minDecodeChars = this.GetMaxCharCount(this.minDecodeBytes);
			if (this.resultFeedback != null)
			{
				this.resultFeedback.Set(ConfigParameter.InputEncoding, newEncoding);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.restartCache != null && this.restartCache is IDisposable)
			{
				((IDisposable)this.restartCache).Dispose();
			}
			this.restartCache = null;
			this.pullSource = null;
			this.pushSource = null;
			this.parseBuffer = null;
			this.readBuffer = null;
			this.pushChunkBuffer = null;
			this.preamble = null;
			this.restartConsumer = null;
			base.Dispose(disposing);
		}

		private int DecodeFromBuffer(byte[] buffer, ref int start, int end, int fileOffset, bool flush)
		{
			int num = 0;
			if (fileOffset == 0)
			{
				if (this.detectEncodingFromByteOrderMark)
				{
					this.DetectEncoding(buffer, start, end);
				}
				if (this.preamble.Length != 0 && end - start >= this.preamble.Length)
				{
					int num2 = 0;
					while (num2 < this.preamble.Length && this.preamble[num2] == buffer[start + num2])
					{
						num2++;
					}
					if (num2 == this.preamble.Length)
					{
						start += this.preamble.Length;
						num = this.preamble.Length;
						if (this.restartConsumer != null)
						{
							this.restartConsumer.DisableRestart();
							this.restartConsumer = null;
						}
					}
				}
				this.encodingChanged = true;
				this.preamble = null;
			}
			int num3 = end - start;
			if (this.GetMaxCharCount(num3) >= this.parseBuffer.Length - this.parseEnd)
			{
				num3 = this.CalculateMaxBytes(this.parseBuffer.Length - this.parseEnd - 1);
			}
			int chars = this.decoder.GetChars(buffer, start, num3, this.parseBuffer, this.parseEnd);
			this.parseEnd += chars;
			this.parseBuffer[this.parseEnd] = '\0';
			start += num3;
			return num3 + num;
		}

		private bool EnsureFreeSpace()
		{
			if (this.parseBuffer.Length - (this.parseEnd - this.parseStart) < this.minDecodeChars + 1 || (this.parseStart < this.minDecodeChars && (long)this.parseBuffer.Length < (long)this.maxTokenSize + (long)(this.minDecodeChars + 1)))
			{
				if ((long)this.parseBuffer.Length >= (long)this.maxTokenSize + (long)(this.minDecodeChars + 1))
				{
					return false;
				}
				long num = (long)(this.parseBuffer.Length * 2);
				if (num > (long)this.maxTokenSize + (long)(this.minDecodeChars + 1))
				{
					num = (long)this.maxTokenSize + (long)(this.minDecodeChars + 1);
				}
				if (num > 2147483647L)
				{
					num = 2147483647L;
				}
				if (num - (long)(this.parseEnd - this.parseStart) < (long)(this.minDecodeChars + 1))
				{
					return false;
				}
				char[] dst;
				try
				{
					dst = new char[(int)num];
				}
				catch (OutOfMemoryException innerException)
				{
					throw new TextConvertersException(TextConvertersStrings.TagTooLong, innerException);
				}
				Buffer.BlockCopy(this.parseBuffer, this.parseStart * 2, dst, 0, (this.parseEnd - this.parseStart + 1) * 2);
				this.parseBuffer = dst;
				this.parseEnd -= this.parseStart;
				this.parseStart = 0;
			}
			else
			{
				Buffer.BlockCopy(this.parseBuffer, this.parseStart * 2, this.parseBuffer, 0, (this.parseEnd - this.parseStart + 1) * 2);
				this.parseEnd -= this.parseStart;
				this.parseStart = 0;
			}
			return true;
		}

		private int GetMaxCharCount(int byteCount)
		{
			if (string.Compare(this.encoding.WebName, "utf-8", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return byteCount + 1;
			}
			if (string.Compare(this.encoding.WebName, "GB18030", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return byteCount + 3;
			}
			return this.encoding.GetMaxCharCount(byteCount);
		}

		private int CalculateMaxBytes(int charCount)
		{
			if (charCount == this.GetMaxCharCount(charCount))
			{
				return charCount;
			}
			if (charCount == this.GetMaxCharCount(charCount - 1))
			{
				return charCount - 1;
			}
			if (charCount == this.GetMaxCharCount(charCount - 3))
			{
				return charCount - 3;
			}
			int num = charCount - 4;
			int maxCharCount = this.GetMaxCharCount(num);
			int num2 = (int)((float)num * (float)charCount / (float)maxCharCount);
			while (this.GetMaxCharCount(num2) < charCount)
			{
				num2++;
			}
			do
			{
				num2--;
			}
			while (this.GetMaxCharCount(num2) > charCount);
			return num2;
		}

		private void DetectEncoding(byte[] buffer, int start, int end)
		{
			if (end - start < 2)
			{
				return;
			}
			Encoding encoding = null;
			if (buffer[start] == 254 && buffer[start + 1] == 255)
			{
				encoding = Encoding.BigEndianUnicode;
			}
			else if (buffer[start] == 255 && buffer[start + 1] == 254)
			{
				if (end - start >= 4 && buffer[start + 2] == 0 && buffer[start + 3] == 0)
				{
					encoding = Encoding.GetEncoding("utf-32");
				}
				else
				{
					encoding = Encoding.Unicode;
				}
			}
			else if (end - start >= 3 && buffer[start] == 239 && buffer[start + 1] == 187 && buffer[start + 2] == 191)
			{
				encoding = Encoding.UTF8;
			}
			else if (end - start >= 4 && buffer[start] == 0 && buffer[start + 1] == 0 && buffer[start + 2] == 254 && buffer[start + 3] == 255)
			{
				encoding = Encoding.GetEncoding("utf-32BE");
			}
			if (encoding != null)
			{
				this.encoding = encoding;
				this.decoder = this.encoding.GetDecoder();
				this.preamble = this.encoding.GetPreamble();
				this.minDecodeChars = this.GetMaxCharCount(this.minDecodeBytes);
				if (this.restartConsumer != null)
				{
					this.restartConsumer.DisableRestart();
					this.restartConsumer = null;
				}
			}
		}

		private void BackupForRestart(byte[] buffer, int offset, int count, int fileOffset, bool force)
		{
			if (!force && fileOffset > this.restartMax)
			{
				this.restartConsumer.DisableRestart();
				this.restartConsumer = null;
				this.preamble = null;
				return;
			}
			if (this.restartCache == null)
			{
				this.restartCache = new ByteCache();
			}
			byte[] dst;
			int dstOffset;
			this.restartCache.GetBuffer(count, out dst, out dstOffset);
			Buffer.BlockCopy(buffer, offset, dst, dstOffset, count);
			this.restartCache.Commit(count);
		}

		private bool GetRestartChunk(out byte[] restartChunk, out int restartStart, out int restartEnd)
		{
			if (this.restartCache.Length == 0)
			{
				restartChunk = null;
				restartStart = 0;
				restartEnd = 0;
				return false;
			}
			int num;
			this.restartCache.GetData(out restartChunk, out restartStart, out num);
			restartEnd = restartStart + num;
			return true;
		}

		private void ReportRestartChunkUsed(int count)
		{
			this.restartCache.ReportRead(count);
		}

		void IReusable.Initialize(object newSourceOrDestination)
		{
			if (this.pullSource != null && newSourceOrDestination != null)
			{
				Stream stream = newSourceOrDestination as Stream;
				if (stream == null || !stream.CanRead)
				{
					throw new InvalidOperationException("cannot reinitialize this converter - new input should be a readable Stream object");
				}
				this.pullSource = stream;
			}
			this.Reinitialize();
		}

		private IResultsFeedback resultFeedback;

		private Stream pullSource;

		private ConverterStream pushSource;

		private bool rawEndOfFile;

		private Encoding originalEncoding;

		private Encoding encoding;

		private Decoder decoder;

		private bool encodingChanged;

		private int minDecodeBytes;

		private int minDecodeChars;

		private char[] parseBuffer;

		private int parseStart;

		private int parseEnd;

		private int readFileOffset;

		private byte[] readBuffer;

		private int readCurrent;

		private int readEnd;

		private byte[] pushChunkBuffer;

		private int pushChunkStart;

		private int pushChunkCount;

		private int pushChunkUsed;

		private bool detectEncodingFromByteOrderMark;

		private byte[] preamble;

		private IRestartable restartConsumer;

		private int restartMax;

		private ByteCache restartCache;

		private bool restarting;
	}
}
