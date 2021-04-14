using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.TextConverters.Internal.Text;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Rtf
{
	internal class RtfToken
	{
		public RtfToken(byte[] buffer, RtfRunEntry[] runQueue)
		{
			this.dataBuffer = buffer;
			this.runQueue = runQueue;
		}

		public RtfTokenId Id
		{
			get
			{
				return this.id;
			}
		}

		public byte[] Buffer
		{
			get
			{
				return this.dataBuffer;
			}
		}

		public int Offset
		{
			get
			{
				return this.offset;
			}
		}

		public int Length
		{
			get
			{
				return this.length;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.runQueueTail == 0;
			}
		}

		public RtfToken.RunEnumerator Runs
		{
			get
			{
				return new RtfToken.RunEnumerator(this);
			}
		}

		public RtfToken.KeywordEnumerator Keywords
		{
			get
			{
				return new RtfToken.KeywordEnumerator(this);
			}
		}

		public TextMapping TextMapping
		{
			get
			{
				return this.textMapping;
			}
		}

		public int TextCodePage
		{
			get
			{
				return this.textCodePage;
			}
		}

		public RtfToken.TextReader Text
		{
			get
			{
				return new RtfToken.TextReader(this);
			}
		}

		public RtfToken.TextEnumerator TextElements
		{
			get
			{
				return new RtfToken.TextEnumerator(this);
			}
		}

		public bool StripZeroBytes
		{
			get
			{
				return this.stripZeroBytes;
			}
			set
			{
				this.stripZeroBytes = value;
			}
		}

		internal RtfRunEntry[] RunQueue
		{
			get
			{
				return this.runQueue;
			}
		}

		internal int CurrentRun
		{
			get
			{
				return this.currentRun;
			}
		}

		internal int CurrentRunOffset
		{
			get
			{
				return this.currentRunOffset;
			}
		}

		internal char[] CharBuffer
		{
			get
			{
				return this.charBuffer;
			}
		}

		internal bool IsTextEof
		{
			get
			{
				return this.charBufferCount == 0 && this.byteBufferCount == 0 && this.currentRun == this.runQueueTail;
			}
		}

		internal RunTextType ElementTextType
		{
			get
			{
				return this.elementTextType;
			}
		}

		internal int ElementOffset
		{
			get
			{
				return this.elementOffset;
			}
		}

		internal int ElementLength
		{
			get
			{
				return this.elementLength;
			}
		}

		public static RtfTokenId TokenIdFromRunKind(RtfRunKind runKind)
		{
			if (runKind >= RtfRunKind.Text)
			{
				return RtfTokenId.Text;
			}
			return (RtfTokenId)(runKind >> 12);
		}

		public void Reset()
		{
			this.id = RtfTokenId.None;
			this.offset = 0;
			this.length = 0;
			this.runQueueTail = 0;
			this.textCodePage = 0;
			this.Rewind();
		}

		public void Initialize(RtfTokenId tokenId, int queueTail, int offset, int length)
		{
			this.id = tokenId;
			this.offset = offset;
			this.length = length;
			this.runQueueTail = queueTail;
			this.Rewind();
		}

		public void SetCodePage(int codePage, TextMapping textMapping)
		{
			this.textCodePage = codePage;
			this.textMapping = textMapping;
		}

		internal void Rewind()
		{
			this.charBufferOffet = (this.charBufferCount = 0);
			this.byteBufferOffet = (this.byteBufferCount = 0);
			this.currentRun = -1;
			this.currentRunOffset = this.offset;
			this.currentRunDelta = 0;
			this.elementOffset = (this.elementLength = 0);
		}

		private bool DecodeMore()
		{
			if (this.charBuffer == null)
			{
				this.charBuffer = new char[1025];
			}
			int i = this.charBuffer.Length - 1;
			int num = 0;
			this.charBufferOffet = 0;
			while (i >= 32)
			{
				if (this.byteBufferCount != 0 || this.byteBufferOffet != 0)
				{
					Decoder decoder = this.GetDecoder();
					bool flush = this.NeedToFlushDecoderBeforeRun(this.currentRun);
					int num2;
					int num3;
					bool flag;
					decoder.Convert(this.byteBuffer, this.byteBufferOffet, this.byteBufferCount, this.charBuffer, num, i, flush, out num2, out num3, out flag);
					num += num3;
					i -= num3;
					this.byteBufferOffet += num2;
					this.byteBufferCount -= num2;
					if (!flag)
					{
						break;
					}
					this.byteBufferOffet = 0;
					if (i < 32)
					{
						break;
					}
				}
				if (this.currentRun == this.runQueueTail)
				{
					break;
				}
				if (this.currentRun == -1 || this.CurrentRunIsSkiped())
				{
					do
					{
						this.MoveToNextRun();
					}
					while (this.currentRun != this.runQueueTail && this.CurrentRunIsSkiped());
					if (this.currentRun == this.runQueueTail)
					{
						break;
					}
				}
				if (this.CurrentRunIsSmall())
				{
					while (this.CurrentRunIsSkiped() || (this.CurrentRunIsSmall() && this.CopyCurrentRunToBuffer()))
					{
						this.MoveToNextRun();
						if (this.currentRun == this.runQueueTail)
						{
							break;
						}
					}
				}
				else if (!this.CurrentRunIsUnicode())
				{
					int num4 = this.currentRunOffset + this.currentRunDelta;
					int num5 = (int)this.runQueue[this.currentRun].Length - this.currentRunDelta;
					Decoder decoder = this.GetDecoder();
					bool flush = this.NeedToFlushDecoderBeforeRun((this.currentRun + 1) % this.runQueue.Length);
					int num2;
					int num3;
					bool flag;
					decoder.Convert(this.dataBuffer, num4, num5, this.charBuffer, num, i, flush, out num2, out num3, out flag);
					num += num3;
					i -= num3;
					num4 += num2;
					num5 -= num2;
					this.currentRunDelta += num2;
					if (!flag)
					{
						break;
					}
					this.MoveToNextRun();
				}
				else
				{
					do
					{
						if (!this.CurrentRunIsSkiped())
						{
							if (!this.CurrentRunIsUnicode())
							{
								break;
							}
							int value = this.runQueue[this.currentRun].Value;
							if (i < 2)
							{
								break;
							}
							if (value > 65535)
							{
								this.charBuffer[num++] = ParseSupport.HighSurrogateCharFromUcs4(value);
								this.charBuffer[num++] = ParseSupport.LowSurrogateCharFromUcs4(value);
								i -= 2;
							}
							else
							{
								this.charBuffer[num++] = (char)value;
								i--;
							}
						}
						this.MoveToNextRun();
					}
					while (this.currentRun != this.runQueueTail);
				}
			}
			this.charBufferCount = num;
			this.charBuffer[this.charBufferCount] = '\0';
			return num != 0;
		}

		private bool MoveToNextTextElement()
		{
			if (this.charBufferCount == 0 && !this.DecodeMore())
			{
				return false;
			}
			int num = this.charBufferOffet;
			this.elementOffset = num;
			char c = this.charBuffer[num];
			if (c > ' ' && c != '\u00a0')
			{
				this.elementTextType = RunTextType.NonSpace;
				do
				{
					c = this.charBuffer[++num];
					if (c <= ' ')
					{
						break;
					}
				}
				while (c != '\u00a0');
			}
			else if (c == ' ')
			{
				this.elementTextType = RunTextType.Space;
				while (this.charBuffer[++num] == ' ')
				{
				}
			}
			else
			{
				char c2 = c;
				switch (c2)
				{
				case '\t':
				case '\n':
				case '\v':
				case '\f':
				case '\r':
					this.elementTextType = RunTextType.UnusualWhitespace;
					while (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(c = this.charBuffer[++num])))
					{
						if (c == ' ')
						{
							break;
						}
					}
					break;
				default:
					if (c2 != '\u00a0')
					{
						this.elementTextType = RunTextType.NonSpace;
						while (ParseSupport.ControlCharacter(ParseSupport.GetCharClass(this.charBuffer[++num])))
						{
						}
					}
					else if (this.textMapping == TextMapping.Unicode)
					{
						this.elementTextType = RunTextType.Nbsp;
						while (this.charBuffer[++num] == '\u00a0')
						{
						}
					}
					else
					{
						this.elementTextType = RunTextType.NonSpace;
						do
						{
							c = this.charBuffer[++num];
							if (c <= ' ')
							{
								break;
							}
						}
						while (c != '\u00a0');
					}
					break;
				}
			}
			this.elementLength = num - this.elementOffset;
			this.charBufferOffet = num;
			this.charBufferCount -= this.elementLength;
			return true;
		}

		private int WriteTo(ITextSink sink)
		{
			int num = 0;
			while (this.charBufferCount != 0 || this.DecodeMore())
			{
				sink.Write(this.charBuffer, this.charBufferOffet, this.charBufferCount);
				this.charBufferOffet = 0;
				this.charBufferCount = 0;
				num += this.charBufferCount;
			}
			return num;
		}

		private void OutputTextElements(TextOutput output, bool treatNbspAsBreakable)
		{
			int num = 0;
			char c = '\0';
			for (;;)
			{
				if (this.charBufferCount == 0)
				{
					if (!this.DecodeMore())
					{
						break;
					}
					num = this.charBufferOffet;
					c = this.charBuffer[num];
				}
				int num2 = num;
				if (c > ' ' && c != '\u00a0')
				{
					do
					{
						c = this.charBuffer[++num];
					}
					while (c > ' ' && c != '\u00a0');
					output.OutputNonspace(this.charBuffer, num2, num - num2, this.textMapping);
				}
				else if (c == ' ')
				{
					do
					{
						c = this.charBuffer[++num];
					}
					while (c == ' ');
					output.OutputSpace(num - num2);
				}
				else
				{
					char c2 = c;
					if (c2 != '\0')
					{
						switch (c2)
						{
						case '\t':
						case '\n':
						case '\v':
						case '\f':
						case '\r':
							do
							{
								c = this.charBuffer[++num];
							}
							while (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(c)) && c != ' ');
							output.OutputSpace(1);
							break;
						default:
							if (c2 != '\u00a0')
							{
								do
								{
									c = this.charBuffer[++num];
								}
								while (ParseSupport.ControlCharacter(ParseSupport.GetCharClass(c)));
								output.OutputNonspace(this.charBuffer, num2, num - num2, this.textMapping);
							}
							else if (this.textMapping == TextMapping.Unicode)
							{
								do
								{
									c = this.charBuffer[++num];
								}
								while (c == '\u00a0');
								if (treatNbspAsBreakable)
								{
									output.OutputSpace(num - num2);
								}
								else
								{
									output.OutputNbsp(num - num2);
								}
							}
							else
							{
								do
								{
									c = this.charBuffer[++num];
								}
								while (c > ' ');
								output.OutputNonspace(this.charBuffer, num2, num - num2, this.textMapping);
							}
							break;
						}
					}
				}
				this.charBufferOffet = num;
				this.charBufferCount -= num - num2;
			}
		}

		private bool NeedToFlushDecoderBeforeRun(int run)
		{
			return run == this.runQueueTail || this.runQueue[run].Kind == RtfRunKind.Unicode || this.runQueue[run].Kind == RtfRunKind.Ignore;
		}

		private Decoder GetDecoder()
		{
			if (this.mruDecoders == null)
			{
				this.mruDecoders = new RtfToken.DecoderMruEntry[4];
				this.mruDecodersLastIndex = this.mruDecoders.Length - 1;
			}
			if (this.textCodePage == 0)
			{
				this.textCodePage = 1252;
			}
			if (this.mruDecoders[this.mruDecodersLastIndex].CodePage == this.textCodePage)
			{
				return this.mruDecoders[this.mruDecodersLastIndex].Decoder;
			}
			for (int i = 0; i < this.mruDecoders.Length; i++)
			{
				if (this.mruDecoders[i].CodePage == this.textCodePage)
				{
					this.mruDecodersLastIndex = i;
					return this.mruDecoders[i].Decoder;
				}
			}
			Decoder decoder = null;
			if (this.decoderCache != null && this.decoderCache.ContainsKey(this.textCodePage))
			{
				decoder = this.decoderCache[this.textCodePage];
			}
			if (decoder == null)
			{
				int num = this.textCodePage;
				if (num == 42)
				{
					num = 28591;
				}
				Encoding encoding = Charset.GetEncoding(num);
				decoder = encoding.GetDecoder();
			}
			this.mruDecodersLastIndex = (this.mruDecodersLastIndex + 1) % this.mruDecoders.Length;
			if (this.mruDecoders[this.mruDecodersLastIndex].Decoder != null)
			{
				if (this.decoderCache == null)
				{
					this.decoderCache = new Dictionary<int, Decoder>();
				}
				if (!this.decoderCache.ContainsKey(this.mruDecoders[this.mruDecodersLastIndex].CodePage))
				{
					this.decoderCache[this.mruDecoders[this.mruDecodersLastIndex].CodePage] = this.mruDecoders[this.mruDecodersLastIndex].Decoder;
				}
			}
			this.mruDecoders[this.mruDecodersLastIndex].Decoder = decoder;
			this.mruDecoders[this.mruDecodersLastIndex].CodePage = this.textCodePage;
			return decoder;
		}

		private void MoveToNextRun()
		{
			if (this.currentRun >= 0)
			{
				this.currentRunOffset += (int)this.runQueue[this.currentRun].Length;
			}
			this.currentRunDelta = 0;
			this.currentRun++;
		}

		private bool CurrentRunIsSkiped()
		{
			return this.runQueue[this.currentRun].IsSkiped;
		}

		private bool CurrentRunIsSmall()
		{
			return this.runQueue[this.currentRun].IsSmall;
		}

		private bool CurrentRunIsUnicode()
		{
			return this.runQueue[this.currentRun].IsUnicode;
		}

		private bool CopyCurrentRunToBuffer()
		{
			if (this.byteBuffer == null)
			{
				this.byteBuffer = new byte[256];
			}
			if (this.byteBuffer.Length == this.byteBufferCount)
			{
				return false;
			}
			RtfRunKind kind = this.runQueue[this.currentRun].Kind;
			if (kind != RtfRunKind.Text)
			{
				if (kind == RtfRunKind.Escape)
				{
					this.byteBuffer[this.byteBufferOffet + this.byteBufferCount] = (byte)this.runQueue[this.currentRun].Value;
					this.byteBufferCount++;
					return true;
				}
				if (kind == RtfRunKind.Zero)
				{
					int num = Math.Min((int)this.runQueue[this.currentRun].Length - this.currentRunDelta, this.byteBuffer.Length - this.byteBufferCount);
					if (!this.stripZeroBytes)
					{
						for (int i = 0; i < num; i++)
						{
							this.byteBuffer[this.byteBufferOffet + this.byteBufferCount] = 32;
							this.byteBufferCount++;
						}
					}
					this.currentRunDelta += num;
				}
			}
			else
			{
				int num = Math.Min((int)this.runQueue[this.currentRun].Length - this.currentRunDelta, this.byteBuffer.Length - this.byteBufferCount);
				System.Buffer.BlockCopy(this.dataBuffer, this.currentRunOffset + this.currentRunDelta, this.byteBuffer, this.byteBufferOffet + this.byteBufferCount, num);
				this.byteBufferCount += num;
				this.currentRunDelta += num;
			}
			return this.currentRunDelta == (int)this.runQueue[this.currentRun].Length;
		}

		private RtfTokenId id;

		private byte[] dataBuffer;

		private int offset;

		private int length;

		private RtfRunEntry[] runQueue;

		private int runQueueTail;

		private int textCodePage;

		private TextMapping textMapping;

		private int currentRun;

		private int currentRunOffset;

		private int currentRunDelta;

		private byte[] byteBuffer;

		private int byteBufferOffet;

		private int byteBufferCount;

		private char[] charBuffer;

		private int charBufferOffet;

		private int charBufferCount;

		private RunTextType elementTextType;

		private int elementOffset;

		private int elementLength;

		private RtfToken.DecoderMruEntry[] mruDecoders;

		private int mruDecodersLastIndex;

		private bool stripZeroBytes;

		private Dictionary<int, Decoder> decoderCache;

		internal struct RunEnumerator
		{
			internal RunEnumerator(RtfToken token)
			{
				this.token = token;
			}

			public int Count
			{
				get
				{
					return this.token.runQueueTail;
				}
			}

			public RtfRun Current
			{
				get
				{
					return new RtfRun(this.token);
				}
			}

			public bool MoveNext()
			{
				if (this.token.currentRun != this.token.runQueueTail)
				{
					this.token.MoveToNextRun();
					if (this.token.currentRun != this.token.runQueueTail)
					{
						return true;
					}
				}
				return false;
			}

			public void Rewind()
			{
				this.token.Rewind();
			}

			public RtfToken.RunEnumerator GetEnumerator()
			{
				return this;
			}

			[Conditional("DEBUG")]
			private void AssertCurrent()
			{
			}

			private RtfToken token;
		}

		internal struct KeywordEnumerator
		{
			internal KeywordEnumerator(RtfToken token)
			{
				this.token = token;
			}

			public int Count
			{
				get
				{
					return this.token.runQueueTail;
				}
			}

			public RtfKeyword Current
			{
				get
				{
					return new RtfKeyword(this.token);
				}
			}

			public bool MoveNext()
			{
				if (this.token.currentRun != this.token.runQueueTail)
				{
					this.token.MoveToNextRun();
					if (this.token.currentRun != this.token.runQueueTail)
					{
						return true;
					}
				}
				return false;
			}

			public void Rewind()
			{
				this.token.Rewind();
			}

			public RtfToken.KeywordEnumerator GetEnumerator()
			{
				return this;
			}

			[Conditional("DEBUG")]
			private void AssertCurrent()
			{
			}

			private RtfToken token;
		}

		internal struct TextReader
		{
			internal TextReader(RtfToken token)
			{
				this.token = token;
			}

			public int Read(char[] buffer, int offset, int count)
			{
				int num = offset;
				if (!this.token.IsTextEof)
				{
					do
					{
						if (this.token.charBufferCount != 0)
						{
							int num2 = Math.Min(count, this.token.charBufferCount);
							System.Buffer.BlockCopy(this.token.charBuffer, this.token.charBufferOffet * 2, buffer, offset * 2, num2 * 2);
							offset += num2;
							count -= num2;
							this.token.charBufferOffet += num2;
							this.token.charBufferCount -= num2;
							if (count == 0)
							{
								break;
							}
						}
					}
					while (this.token.DecodeMore());
				}
				return offset - num;
			}

			public void Rewind()
			{
				this.token.Rewind();
			}

			public int WriteTo(ITextSink sink)
			{
				return this.token.WriteTo(sink);
			}

			[Conditional("DEBUG")]
			private void AssertCurrent()
			{
			}

			private RtfToken token;
		}

		internal struct TextEnumerator
		{
			internal TextEnumerator(RtfToken token)
			{
				this.token = token;
			}

			public RtfTextElement Current
			{
				get
				{
					return new RtfTextElement(this.token);
				}
			}

			public bool MoveNext()
			{
				return this.token.MoveToNextTextElement();
			}

			public bool MoveNext(bool skipAllWhitespace)
			{
				return this.token.MoveToNextTextElement();
			}

			public void Rewind()
			{
				this.token.Rewind();
			}

			public RtfToken.TextEnumerator GetEnumerator()
			{
				return this;
			}

			public void OutputTextElements(TextOutput output, bool treatNbspAsBreakable)
			{
				this.token.OutputTextElements(output, treatNbspAsBreakable);
			}

			[Conditional("DEBUG")]
			private void AssertCurrent()
			{
			}

			private RtfToken token;
		}

		private struct DecoderMruEntry
		{
			public int CodePage;

			public Decoder Decoder;
		}
	}
}
