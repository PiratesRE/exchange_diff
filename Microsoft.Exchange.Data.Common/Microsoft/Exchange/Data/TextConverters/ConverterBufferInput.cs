﻿using System;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal class ConverterBufferInput : ConverterInput, ITextSink
	{
		public ConverterBufferInput(IProgressMonitor progressMonitor) : this(32768, progressMonitor)
		{
		}

		public ConverterBufferInput(int maxLength, IProgressMonitor progressMonitor) : base(progressMonitor)
		{
			this.maxLength = maxLength;
		}

		public ConverterBufferInput(string fragment, IProgressMonitor progressMonitor) : this(32768, fragment, progressMonitor)
		{
		}

		public ConverterBufferInput(int maxLength, string fragment, IProgressMonitor progressMonitor) : base(progressMonitor)
		{
			this.maxLength = maxLength;
			this.originalFragment = fragment;
			this.parseBuffer = new char[fragment.Length + 1];
			fragment.CopyTo(0, this.parseBuffer, 0, fragment.Length);
			this.parseBuffer[fragment.Length] = '\0';
			this.maxTokenSize = fragment.Length;
		}

		public bool IsEnough
		{
			get
			{
				return this.maxTokenSize >= this.maxLength;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.maxTokenSize == 0;
			}
		}

		public void Write(string str)
		{
			int num = this.PrepareToBuffer(str.Length);
			if (num > 0)
			{
				str.CopyTo(0, this.parseBuffer, this.maxTokenSize, num);
				this.maxTokenSize += num;
				this.parseBuffer[this.maxTokenSize] = '\0';
			}
		}

		public void Write(char[] buffer, int offset, int count)
		{
			count = this.PrepareToBuffer(count);
			if (count > 0)
			{
				Buffer.BlockCopy(buffer, offset * 2, this.parseBuffer, this.maxTokenSize * 2, count * 2);
				this.maxTokenSize += count;
				this.parseBuffer[this.maxTokenSize] = '\0';
			}
		}

		public void Write(int ucs32Char)
		{
			if (ucs32Char > 65535)
			{
				int num = this.PrepareToBuffer(2);
				if (num > 0)
				{
					this.parseBuffer[this.maxTokenSize] = ParseSupport.HighSurrogateCharFromUcs4(ucs32Char);
					this.parseBuffer[this.maxTokenSize + 1] = ParseSupport.LowSurrogateCharFromUcs4(ucs32Char);
					this.maxTokenSize += num;
					this.parseBuffer[this.maxTokenSize] = '\0';
					return;
				}
			}
			else
			{
				int num = this.PrepareToBuffer(1);
				if (num > 0)
				{
					this.parseBuffer[this.maxTokenSize++] = (char)ucs32Char;
					this.parseBuffer[this.maxTokenSize] = '\0';
				}
			}
		}

		public void Reset()
		{
			this.maxTokenSize = 0;
			this.endOfFile = false;
		}

		public void Initialize(string fragment)
		{
			if (this.originalFragment != fragment)
			{
				this.originalFragment = fragment;
				this.parseBuffer = new char[fragment.Length + 1];
				fragment.CopyTo(0, this.parseBuffer, 0, fragment.Length);
				this.parseBuffer[fragment.Length] = '\0';
				this.maxTokenSize = fragment.Length;
			}
			this.endOfFile = false;
		}

		public override bool ReadMore(ref char[] buffer, ref int start, ref int current, ref int end)
		{
			if (buffer == null)
			{
				buffer = this.parseBuffer;
				start = 0;
				end = this.maxTokenSize;
				current = 0;
				if (end != 0)
				{
					return true;
				}
			}
			this.endOfFile = true;
			return true;
		}

		public override void ReportProcessed(int processedSize)
		{
			this.progressMonitor.ReportProgress();
		}

		public override int RemoveGap(int gapBegin, int gapEnd)
		{
			this.parseBuffer[gapBegin] = '\0';
			return gapBegin;
		}

		protected override void Dispose(bool disposing)
		{
			this.parseBuffer = null;
			base.Dispose(disposing);
		}

		private int PrepareToBuffer(int count)
		{
			if (this.maxTokenSize + count > this.maxLength)
			{
				count = this.maxLength - this.maxTokenSize;
			}
			if (count > 0)
			{
				if (this.parseBuffer == null)
				{
					this.parseBuffer = new char[count + 1];
				}
				else if (this.parseBuffer.Length <= this.maxTokenSize + count)
				{
					char[] src = this.parseBuffer;
					int num = (this.maxTokenSize + count) * 2;
					if (num > this.maxLength)
					{
						num = this.maxLength;
					}
					this.parseBuffer = new char[num + 1];
					if (this.maxTokenSize > 0)
					{
						Buffer.BlockCopy(src, 0, this.parseBuffer, 0, this.maxTokenSize * 2);
					}
				}
			}
			return count;
		}

		private const int DefaultMaxLength = 32768;

		private int maxLength;

		private string originalFragment;

		private char[] parseBuffer;
	}
}
