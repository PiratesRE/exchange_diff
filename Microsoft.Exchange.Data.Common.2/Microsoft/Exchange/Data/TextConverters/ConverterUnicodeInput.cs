using System;
using System.IO;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal class ConverterUnicodeInput : ConverterInput, IReusable
	{
		public ConverterUnicodeInput(object source, bool push, int maxParseToken, bool testBoundaryConditions, IProgressMonitor progressMonitor) : base(progressMonitor)
		{
			if (push)
			{
				this.pushSource = (source as ConverterWriter);
			}
			else
			{
				this.pullSource = (source as TextReader);
			}
			this.maxTokenSize = maxParseToken;
			this.parseBuffer = new char[testBoundaryConditions ? 123 : 4096];
			if (this.pushSource != null)
			{
				this.pushSource.SetSink(this);
			}
		}

		private void Reinitialize()
		{
			this.parseStart = (this.parseEnd = 0);
			this.pushChunkStart = 0;
			this.pushChunkCount = 0;
			this.pushChunkUsed = 0;
			this.pushChunkBuffer = null;
			this.endOfFile = false;
		}

		void IReusable.Initialize(object newSourceOrDestination)
		{
			if (this.pullSource != null && newSourceOrDestination != null)
			{
				TextReader textReader = newSourceOrDestination as TextReader;
				if (textReader == null)
				{
					throw new InvalidOperationException("cannot reinitialize this converter - new input should be a TextReader object");
				}
				this.pullSource = textReader;
			}
			this.Reinitialize();
		}

		public override bool ReadMore(ref char[] buffer, ref int start, ref int current, ref int end)
		{
			int num = this.parseEnd - end;
			if (this.parseBuffer.Length - this.parseEnd <= 1 && !this.EnsureFreeSpace() && num == 0)
			{
				return true;
			}
			while (!this.endOfFile && this.parseBuffer.Length - this.parseEnd > 1)
			{
				if (this.pushSource != null)
				{
					if (this.pushChunkCount == 0 && !this.pushSource.GetInputChunk(out this.pushChunkBuffer, out this.pushChunkStart, out this.pushChunkCount, out this.endOfFile))
					{
						break;
					}
					if (this.pushChunkCount - this.pushChunkUsed != 0)
					{
						int num2 = Math.Min(this.pushChunkCount - this.pushChunkUsed, this.parseBuffer.Length - this.parseEnd - 1);
						Buffer.BlockCopy(this.pushChunkBuffer, (this.pushChunkStart + this.pushChunkUsed) * 2, this.parseBuffer, this.parseEnd * 2, num2 * 2);
						this.pushChunkUsed += num2;
						this.parseEnd += num2;
						this.parseBuffer[this.parseEnd] = '\0';
						num += num2;
						if (this.pushChunkCount - this.pushChunkUsed == 0)
						{
							this.pushSource.ReportRead(this.pushChunkCount);
							this.pushChunkStart = 0;
							this.pushChunkCount = 0;
							this.pushChunkUsed = 0;
							this.pushChunkBuffer = null;
						}
					}
				}
				else
				{
					int num3 = this.pullSource.Read(this.parseBuffer, this.parseEnd, this.parseBuffer.Length - this.parseEnd - 1);
					if (num3 == 0)
					{
						this.endOfFile = true;
					}
					else
					{
						this.parseEnd += num3;
						this.parseBuffer[this.parseEnd] = '\0';
						num += num3;
					}
					if (this.progressMonitor != null)
					{
						this.progressMonitor.ReportProgress();
					}
				}
			}
			buffer = this.parseBuffer;
			if (start != this.parseStart)
			{
				current = this.parseStart + (current - start);
				start = this.parseStart;
			}
			end = this.parseEnd;
			return num != 0 || this.endOfFile;
		}

		public override void ReportProcessed(int processedSize)
		{
			this.parseStart += processedSize;
		}

		public override int RemoveGap(int gapBegin, int gapEnd)
		{
			if (gapEnd == this.parseEnd)
			{
				this.parseEnd = gapBegin;
				this.parseBuffer[gapBegin] = '\0';
				return gapBegin;
			}
			Buffer.BlockCopy(this.parseBuffer, gapEnd, this.parseBuffer, gapBegin, this.parseEnd - gapEnd);
			this.parseEnd = gapBegin + (this.parseEnd - gapEnd);
			this.parseBuffer[this.parseEnd] = '\0';
			return this.parseEnd;
		}

		public void GetInputBuffer(out char[] inputBuffer, out int inputOffset, out int inputCount, out int parseCount)
		{
			inputBuffer = this.parseBuffer;
			inputOffset = this.parseEnd;
			inputCount = this.parseBuffer.Length - this.parseEnd - 1;
			parseCount = this.parseEnd - this.parseStart;
		}

		public void Commit(int inputCount)
		{
			this.parseEnd += inputCount;
			this.parseBuffer[this.parseEnd] = '\0';
		}

		protected override void Dispose(bool disposing)
		{
			this.pullSource = null;
			this.pushSource = null;
			this.parseBuffer = null;
			this.pushChunkBuffer = null;
			base.Dispose(disposing);
		}

		private bool EnsureFreeSpace()
		{
			if (this.parseBuffer.Length - (this.parseEnd - this.parseStart) <= 1 || (this.parseStart < 1 && (long)this.parseBuffer.Length < (long)this.maxTokenSize + 1L))
			{
				if ((long)this.parseBuffer.Length >= (long)this.maxTokenSize + 1L)
				{
					return false;
				}
				long num = (long)(this.parseBuffer.Length * 2);
				if (num > (long)this.maxTokenSize + 1L)
				{
					num = (long)this.maxTokenSize + 1L;
				}
				if (num > 2147483647L)
				{
					num = 2147483647L;
				}
				char[] dst = new char[(int)num];
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

		private TextReader pullSource;

		private ConverterWriter pushSource;

		private char[] parseBuffer;

		private int parseStart;

		private int parseEnd;

		private char[] pushChunkBuffer;

		private int pushChunkStart;

		private int pushChunkCount;

		private int pushChunkUsed;
	}
}
