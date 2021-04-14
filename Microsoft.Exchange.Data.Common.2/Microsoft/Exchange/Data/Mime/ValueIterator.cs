using System;

namespace Microsoft.Exchange.Data.Mime
{
	internal struct ValueIterator
	{
		public ValueIterator(MimeStringList lines, uint linesMask)
		{
			this.lines = lines;
			this.linesMask = linesMask;
			this.lineStart = (this.lineEnd = (this.currentLine = (this.currentOffset = 0)));
			this.lineBytes = null;
			this.endLine = this.lines.Count;
			this.endOffset = 0;
			while (this.currentLine != this.endLine)
			{
				MimeString mimeString = this.lines[this.currentLine];
				if ((mimeString.Mask & this.linesMask) != 0U)
				{
					int num;
					this.lineBytes = mimeString.GetData(out this.lineStart, out num);
					this.lineEnd = this.lineStart + num;
					this.currentOffset = this.lineStart;
					return;
				}
				this.currentLine++;
			}
		}

		public ValueIterator(MimeStringList lines, uint linesMask, ValuePosition startPosition, ValuePosition endPosition)
		{
			this.lines = lines;
			this.linesMask = linesMask;
			this.currentLine = startPosition.Line;
			this.currentOffset = startPosition.Offset;
			this.endLine = endPosition.Line;
			this.endOffset = endPosition.Offset;
			if (startPosition != endPosition)
			{
				int num;
				this.lineBytes = this.lines[this.currentLine].GetData(out this.lineStart, out num);
				this.lineEnd = ((this.currentLine == this.endLine) ? this.endOffset : (this.lineStart + num));
				return;
			}
			this.lineStart = (this.lineEnd = this.currentOffset);
			this.lineBytes = null;
		}

		public ValuePosition CurrentPosition
		{
			get
			{
				return new ValuePosition(this.currentLine, this.currentOffset);
			}
		}

		public byte[] Bytes
		{
			get
			{
				return this.lineBytes;
			}
		}

		public int Offset
		{
			get
			{
				return this.currentOffset;
			}
		}

		public int Length
		{
			get
			{
				return this.lineEnd - this.currentOffset;
			}
		}

		public int TotalLength
		{
			get
			{
				return this.lines.Length;
			}
		}

		public MimeStringList Lines
		{
			get
			{
				return this.lines;
			}
		}

		public uint LinesMask
		{
			get
			{
				return this.linesMask;
			}
		}

		public bool Eof
		{
			get
			{
				return this.currentLine == this.endLine && this.currentOffset == this.lineEnd;
			}
		}

		public void Get(int length)
		{
			this.currentOffset += length;
			if (this.currentOffset == this.lineEnd)
			{
				this.NextLine();
			}
		}

		public int Get()
		{
			if (this.Eof)
			{
				return -1;
			}
			byte result = this.lineBytes[this.currentOffset++];
			if (this.currentOffset == this.lineEnd)
			{
				this.NextLine();
			}
			return (int)result;
		}

		public int Pick()
		{
			if (this.Eof)
			{
				return -1;
			}
			return (int)this.lineBytes[this.currentOffset];
		}

		public void Unget()
		{
			if (this.currentOffset == this.lineStart)
			{
				MimeString mimeString;
				do
				{
					mimeString = this.lines[--this.currentLine];
				}
				while ((mimeString.Mask & this.linesMask) == 0U);
				int num;
				this.lineBytes = mimeString.GetData(out this.lineStart, out num);
				this.currentOffset = (this.lineEnd = this.lineStart + num);
			}
			this.currentOffset--;
		}

		public void SkipToEof()
		{
			while (!this.Eof)
			{
				this.Get(this.Length);
			}
		}

		private void NextLine()
		{
			if (!this.Eof)
			{
				MimeString mimeString;
				for (;;)
				{
					this.currentLine++;
					if (this.currentLine == this.lines.Count)
					{
						break;
					}
					mimeString = this.lines[this.currentLine];
					if ((mimeString.Mask & this.linesMask) != 0U)
					{
						goto Block_2;
					}
				}
				this.lineEnd = (this.lineStart = (this.currentOffset = 0));
				return;
				Block_2:
				int num;
				this.lineBytes = mimeString.GetData(out this.lineStart, out num);
				this.currentOffset = this.lineStart;
				this.lineEnd = ((this.currentLine == this.endLine) ? this.endOffset : (this.lineStart + num));
			}
		}

		private MimeStringList lines;

		private uint linesMask;

		private int currentLine;

		private int currentOffset;

		private int lineStart;

		private int lineEnd;

		private byte[] lineBytes;

		private int endLine;

		private int endOffset;
	}
}
