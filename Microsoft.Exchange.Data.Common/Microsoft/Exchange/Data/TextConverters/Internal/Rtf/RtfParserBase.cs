using System;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Rtf
{
	internal class RtfParserBase
	{
		public RtfParserBase(int inputBufferSize, bool testBoundaryConditions, IReportBytes reportBytes)
		{
			this.parseBuffer = new byte[1 + (testBoundaryConditions ? 133 : Math.Min(32767, inputBufferSize))];
			this.reportBytes = reportBytes;
			this.InitializeBase();
		}

		public RtfParserBase(int inputBufferSize, bool testBoundaryConditions, RtfParserBase previewParser, IReportBytes reportBytes)
		{
			int num = 1 + (testBoundaryConditions ? 133 : Math.Min(32767, inputBufferSize));
			if (previewParser.ParseBuffer.Length < num)
			{
				this.parseBuffer = new byte[1 + (testBoundaryConditions ? 133 : Math.Min(32767, inputBufferSize))];
				Buffer.BlockCopy(previewParser.ParseBuffer, 0, this.parseBuffer, 0, previewParser.ParseEnd);
			}
			else
			{
				this.parseBuffer = previewParser.ParseBuffer;
			}
			this.parseEnd = previewParser.ParseEnd;
			this.endOfFileVisible = previewParser.EndOfFileVisible;
			this.reportBytes = reportBytes;
			this.InitializeBase();
		}

		public byte[] ParseBuffer
		{
			get
			{
				return this.parseBuffer;
			}
		}

		public int ParseStart
		{
			get
			{
				return this.parseStart;
			}
		}

		public int ParseOffset
		{
			get
			{
				return this.parseOffset;
			}
		}

		public int ParseEnd
		{
			get
			{
				return this.parseEnd;
			}
		}

		public bool ParseBufferFull
		{
			get
			{
				return this.parseEnd + 128 >= this.parseBuffer.Length;
			}
		}

		public bool EndOfFile
		{
			get
			{
				return this.endOfFileVisible && this.parseOffset == this.parseEnd;
			}
		}

		public bool EndOfFileVisible
		{
			get
			{
				return this.endOfFileVisible;
			}
		}

		public bool ParseBufferNeedsRefill
		{
			get
			{
				return this.parseEnd - this.parseOffset < 128;
			}
		}

		public RtfRunKind RunKind
		{
			get
			{
				return this.run.Kind;
			}
		}

		public short KeywordId
		{
			get
			{
				return this.run.KeywordId;
			}
		}

		public int KeywordValue
		{
			get
			{
				return this.run.Value;
			}
		}

		public int GetBufferSpace(bool compact, out int offset)
		{
			if (compact && this.parseStart != 0 && (this.parseEnd - this.parseStart < 128 || this.parseEnd + 128 > this.parseBuffer.Length - 1))
			{
				if (this.parseEnd != this.parseStart)
				{
					Buffer.BlockCopy(this.parseBuffer, this.parseStart, this.parseBuffer, 0, this.parseEnd - this.parseStart);
				}
				this.parseOffset -= this.parseStart;
				this.parseEnd -= this.parseStart;
				this.parseStart = 0;
			}
			offset = this.parseEnd;
			return this.parseBuffer.Length - 1 - this.parseEnd;
		}

		public void ReportMoreDataAvailable(int length, bool endOfFileVisible)
		{
			this.parseEnd += length;
			this.parseBuffer[this.parseEnd] = 0;
			this.endOfFileVisible = endOfFileVisible;
			if (this.reportBytes != null)
			{
				this.reportBytes.ReportBytesRead(length);
			}
		}

		public bool ParseRun()
		{
			if (this.parseEnd == this.parseOffset)
			{
				if (this.endOfFileVisible)
				{
					this.run.Initialize(RtfRunKind.EndOfFile, 0, 0);
					return true;
				}
				return false;
			}
			else
			{
				if (this.binLength != 0)
				{
					int num = Math.Min(this.parseEnd - this.parseOffset, this.binLength);
					this.run.Initialize(RtfRunKind.Binary, num, 0, 0 != this.bytesToSkip, false);
					this.binLength -= num;
					this.parseOffset += num;
					if (this.binLength == 0 && this.bytesToSkip != 0)
					{
						this.bytesToSkip--;
					}
					return true;
				}
				int num2 = this.parseOffset;
				byte b = this.parseBuffer[num2];
				byte b2 = b;
				switch (b2)
				{
				case 9:
					if (!this.lastLeadByte)
					{
						this.run.InitializeKeyword(126, 0, 1, this.SkipIfNecessary(1), this.firstKeyword);
						this.parseOffset++;
						this.firstKeyword = false;
						return true;
					}
					break;
				case 10:
				case 13:
					do
					{
						b = this.parseBuffer[++num2];
					}
					while (b == 13 || b == 10);
					this.run.Initialize(RtfRunKind.Ignore, num2 - this.parseOffset, 0, false, false);
					this.parseOffset = num2;
					return true;
				case 11:
				case 12:
					break;
				default:
					if (b2 != 92)
					{
						switch (b2)
						{
						case 123:
							if (!this.lastLeadByte)
							{
								this.run.Initialize(RtfRunKind.Begin, 1, 0);
								this.parseOffset++;
								this.firstKeyword = true;
								this.bytesToSkip = 0;
								return true;
							}
							break;
						case 125:
							if (!this.lastLeadByte)
							{
								this.run.Initialize(RtfRunKind.End, 1, 0);
								this.parseOffset++;
								this.firstKeyword = false;
								this.bytesToSkip = 0;
								return true;
							}
							break;
						}
					}
					else
					{
						if (this.ParseKeywordRun())
						{
							this.firstKeyword = false;
							return true;
						}
						return false;
					}
					break;
				}
				this.EnsureCodePage();
				this.firstKeyword = false;
				return this.ParseTextRun();
			}
		}

		protected void ReportConsumed(int length)
		{
			this.parseStart += length;
		}

		protected void InitializeBase()
		{
			this.parseStart = 0;
			this.parseOffset = 0;
			this.bytesSkipForUnicodeEscape = 1;
			this.firstKeyword = false;
			this.bytesToSkip = 0;
			this.binLength = 0;
			this.defaultCodePage = 0;
			this.currentCodePage = 0;
			this.currentTextMapping = TextMapping.Unicode;
			this.leadMask = (DbcsLeadBits)0;
			this.lastLeadByte = false;
			this.currentCharRep = RtfSupport.CharRep.DEFAULT_INDEX;
			this.run.Reset();
		}

		protected void SetCodePage(ushort codePage, TextMapping textMapping)
		{
			if (codePage != this.currentCodePage)
			{
				this.currentCodePage = codePage;
				this.leadMask = ParseSupport.GetCodePageLeadMask((int)codePage);
			}
			if (textMapping != this.currentTextMapping)
			{
				this.currentTextMapping = textMapping;
			}
		}

		private void EnsureCodePage()
		{
			if (this.currentCodePage == 0)
			{
				this.SetCodePage((this.defaultCodePage == 0) ? 1252 : this.defaultCodePage, TextMapping.Unicode);
			}
		}

		private bool ParseKeywordRun()
		{
			if (1 != this.parseEnd - this.parseOffset)
			{
				byte b = this.parseBuffer[this.parseOffset + 1];
				char c = (char)b;
				int num;
				if (c > '-')
				{
					if (c <= '\\')
					{
						if (c == ':')
						{
							this.lastLeadByte = false;
							this.run.InitializeKeyword(3, 0, 2, this.SkipIfNecessary(1), this.firstKeyword);
							this.parseOffset += 2;
							return true;
						}
						if (c != '\\')
						{
							goto IL_CE;
						}
					}
					else
					{
						if (c != '_')
						{
							switch (c)
							{
							case '{':
							case '}':
								goto IL_EA;
							case '|':
								this.lastLeadByte = false;
								this.run.InitializeKeyword(2, 0, 2, this.SkipIfNecessary(1), this.firstKeyword);
								this.parseOffset += 2;
								return true;
							case '~':
								num = 160;
								goto IL_155;
							}
							goto IL_CE;
						}
						num = 8209;
						goto IL_155;
					}
					IL_EA:
					num = (int)b;
					goto IL_EC;
				}
				if (c <= '\'')
				{
					switch (c)
					{
					case '\t':
						this.lastLeadByte = false;
						this.run.InitializeKeyword(126, 0, 2, this.SkipIfNecessary(1), this.firstKeyword);
						this.parseOffset += 2;
						return true;
					case '\n':
					case '\r':
						this.lastLeadByte = false;
						this.run.InitializeKeyword(68, 0, 2, this.SkipIfNecessary(1), this.firstKeyword);
						this.parseOffset += 2;
						return true;
					case '\v':
					case '\f':
						break;
					default:
						if (c == '\'')
						{
							this.EnsureCodePage();
							if (this.parseEnd - this.parseOffset >= 4)
							{
								num = RtfSupport.Unescape(this.parseBuffer[this.parseOffset + 2], this.parseBuffer[this.parseOffset + 3]);
								if (num > 255)
								{
									if (this.lastLeadByte)
									{
										this.lastLeadByte = false;
										this.run.Initialize(RtfRunKind.Text, 1, 0, this.SkipIfNecessary(1), false);
										this.parseOffset++;
										return true;
									}
									num = 63;
								}
								else
								{
									if ((num == 13 || num == 10) && !this.lastLeadByte)
									{
										this.run.InitializeKeyword(68, 0, 4, this.SkipIfNecessary(1), this.firstKeyword);
										this.parseOffset += 4;
										return true;
									}
									if (num == 0)
									{
										num = 32;
									}
									this.lastLeadByte = (!this.lastLeadByte && ParseSupport.IsLeadByte((byte)num, this.leadMask));
								}
								this.run.Initialize(RtfRunKind.Escape, 4, num, this.SkipIfNecessary(1), this.lastLeadByte);
								this.parseOffset += 4;
								return true;
							}
							if (this.endOfFileVisible)
							{
								this.run.Initialize(RtfRunKind.Text, 1, 0, this.SkipIfNecessary(1), false);
								this.parseOffset++;
								this.lastLeadByte = false;
								return true;
							}
							return false;
						}
						break;
					}
				}
				else
				{
					if (c == '*')
					{
						this.lastLeadByte = false;
						this.run.InitializeKeyword(1, 0, 2, this.SkipIfNecessary(1), this.firstKeyword);
						this.parseOffset += 2;
						return true;
					}
					if (c == '-')
					{
						num = 173;
						goto IL_155;
					}
				}
				IL_CE:
				CharClass charClass = ParseSupport.GetCharClass(b);
				if (ParseSupport.AlphaCharacter(charClass))
				{
					this.lastLeadByte = false;
					return this.ParseKeyword(b);
				}
				num = (int)b;
				if (num == 0)
				{
					num = 32;
				}
				IL_EC:
				this.EnsureCodePage();
				this.lastLeadByte = (!this.lastLeadByte && ParseSupport.IsLeadByte((byte)num, this.leadMask));
				this.run.Initialize(RtfRunKind.Escape, 2, num, this.SkipIfNecessary(1), this.lastLeadByte);
				this.parseOffset += 2;
				return true;
				IL_155:
				this.EnsureCodePage();
				this.lastLeadByte = false;
				this.run.Initialize(RtfRunKind.Unicode, 2, num, this.SkipIfNecessary(1), false);
				this.parseOffset += 2;
				return true;
			}
			if (this.endOfFileVisible)
			{
				this.run.Initialize(RtfRunKind.Text, 1, 0, this.SkipIfNecessary(1), false);
				this.parseOffset++;
				return true;
			}
			return false;
		}

		private bool ParseKeyword(byte ch)
		{
			int num = this.parseOffset + 1;
			short hash = 0;
			do
			{
				hash = RTFData.AddHash(hash, ch);
				ch = this.parseBuffer[++num];
			}
			while ((ch | 32) - 97 <= 25);
			int num2 = num - (this.parseOffset + 1);
			bool flag = false;
			bool flag2 = false;
			int num3 = 0;
			if (ch == 45)
			{
				flag = true;
				flag2 = true;
				num++;
				ch = this.parseBuffer[num];
			}
			if (ch - 48 <= 9)
			{
				flag = true;
				do
				{
					num3 = num3 * 10 + (int)(ch - 48);
					ch = this.parseBuffer[++num];
				}
				while (ch - 48 <= 9);
				if (flag2)
				{
					num3 = -num3;
				}
			}
			if (num > this.parseOffset + 128 - 1)
			{
				num = this.parseOffset + 128 - 1;
				ch = this.parseBuffer[num];
				num3 = 0;
				num2 = Math.Min(num2, num - (this.parseOffset + 1));
			}
			if (ch == 32)
			{
				num++;
			}
			else if (ch == 0 && num == this.parseEnd && !this.endOfFileVisible)
			{
				return false;
			}
			int num4 = 0;
			if (num2 != 1 || (this.parseBuffer[this.parseOffset + 1] | 32) != 117)
			{
				short num5 = this.LookupKeyword(hash, this.parseOffset + 1, num2);
				if (RTFData.keywords[(int)num5].character == '\0')
				{
					if (!flag)
					{
						num3 = (int)RTFData.keywords[(int)num5].defaultValue;
					}
					if (num5 == 47)
					{
						this.binLength = ((num3 > 0) ? num3 : 0);
					}
					bool skip = (num5 != 47 || this.binLength == 0) && this.SkipIfNecessary(1);
					this.run.InitializeKeyword(num5, num3, num - this.parseOffset, skip, this.firstKeyword);
					this.parseOffset = num;
					return true;
				}
				num3 = (int)RTFData.keywords[(int)num5].character;
			}
			else
			{
				num4 = (int)this.bytesSkipForUnicodeEscape;
				if (num3 < 0)
				{
					num3 &= 65535;
				}
				else if (num3 > 1114111)
				{
					num3 = 63;
				}
				if (this.currentCharRep == RtfSupport.CharRep.SYMBOL_INDEX && 61440 <= num3 && num3 <= 61695)
				{
					num3 -= 61440;
				}
				if (num3 == 0)
				{
					num3 = 32;
				}
			}
			this.run.Initialize(RtfRunKind.Unicode, num - this.parseOffset, num3, this.SkipIfNecessary(1), ParseSupport.IsHighSurrogate((char)num3));
			this.parseOffset = num;
			this.bytesToSkip += num4;
			return true;
		}

		private bool ParseTextRun()
		{
			int num = this.parseOffset;
			int num2 = this.parseEnd;
			byte b = this.parseBuffer[num];
			if (this.bytesToSkip != 0)
			{
				num2 = Math.Min(num2, num + this.bytesToSkip);
			}
			bool skip;
			RtfRunKind kind;
			if (b == 0)
			{
				this.lastLeadByte = false;
				do
				{
					b = this.parseBuffer[++num];
				}
				while (b == 0 && num != num2);
				CharClass charClass = ParseSupport.GetCharClass(b);
				skip = this.SkipIfNecessary(num - this.parseOffset);
				kind = RtfRunKind.Zero;
			}
			else if (this.leadMask == (DbcsLeadBits)0)
			{
				this.lastLeadByte = false;
				if (this.bytesToSkip == 0)
				{
					CharClass charClass;
					do
					{
						b = this.parseBuffer[++num];
						charClass = ParseSupport.GetCharClass(b);
					}
					while (!ParseSupport.RtfInterestingCharacter(charClass));
					skip = false;
				}
				else
				{
					CharClass charClass;
					do
					{
						b = this.parseBuffer[++num];
						charClass = ParseSupport.GetCharClass(b);
					}
					while (num != num2 && !ParseSupport.RtfInterestingCharacter(charClass));
					skip = this.SkipIfNecessary(num - this.parseOffset);
				}
				kind = RtfRunKind.Text;
			}
			else
			{
				for (;;)
				{
					this.lastLeadByte = (!this.lastLeadByte && ParseSupport.IsLeadByte(b, this.leadMask));
					b = this.parseBuffer[++num];
					CharClass charClass = ParseSupport.GetCharClass(b);
					if (num == num2 || ParseSupport.RtfInterestingCharacter(charClass))
					{
						if (!this.lastLeadByte)
						{
							goto IL_181;
						}
						if (num == num2 || (b != 123 && b != 125))
						{
							break;
						}
					}
				}
				if (num - this.parseOffset > 1)
				{
					num--;
					this.lastLeadByte = false;
					b = this.parseBuffer[num];
					CharClass charClass = ParseSupport.GetCharClass(b);
				}
				else if (num == num2 && !this.endOfFileVisible && num2 == this.parseEnd)
				{
					this.lastLeadByte = false;
					return false;
				}
				IL_181:
				skip = this.SkipIfNecessary(num - this.parseOffset);
				kind = RtfRunKind.Text;
			}
			this.run.Initialize(kind, num - this.parseOffset, 0, skip, this.lastLeadByte);
			this.parseOffset = num;
			return true;
		}

		private bool SkipIfNecessary(int length)
		{
			if (this.bytesToSkip != 0)
			{
				this.bytesToSkip -= length;
				return true;
			}
			return false;
		}

		private short LookupKeyword(short hash, int nameOffset, int nameLength)
		{
			short num = RTFData.keywordHashTable[(int)hash];
			if (num != 0)
			{
				bool flag = false;
				for (;;)
				{
					if (RTFData.keywords[(int)num].name.Length == nameLength && RTFData.keywords[(int)num].name[0] == (char)this.parseBuffer[nameOffset])
					{
						int num2 = 1;
						while (num2 < nameLength && RTFData.keywords[(int)num].name[num2] == (char)this.parseBuffer[nameOffset + num2])
						{
							num2++;
						}
						if (num2 == nameLength)
						{
							break;
						}
					}
					if ((int)(num += 1) >= RTFData.keywords.Length || RTFData.keywords[(int)num].hash != hash)
					{
						goto IL_A3;
					}
				}
				flag = true;
				IL_A3:
				if (!flag)
				{
					num = 0;
				}
			}
			return num;
		}

		public const int ReadThreshold = 128;

		protected static readonly short TwelvePointsInTwips = 240;

		protected RtfRunEntry run;

		protected ushort defaultCodePage;

		protected ushort currentCodePage;

		protected TextMapping currentTextMapping;

		protected RtfSupport.CharRep currentCharRep;

		protected bool lastLeadByte;

		protected byte bytesSkipForUnicodeEscape;

		private byte[] parseBuffer;

		private int parseStart;

		private int parseOffset;

		private int parseEnd;

		private bool endOfFileVisible;

		private bool firstKeyword;

		private int bytesToSkip;

		private int binLength;

		private DbcsLeadBits leadMask;

		private IReportBytes reportBytes;
	}
}
