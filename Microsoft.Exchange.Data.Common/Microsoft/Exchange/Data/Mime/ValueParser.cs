using System;

namespace Microsoft.Exchange.Data.Mime
{
	internal struct ValueParser
	{
		public ValueParser(MimeStringList lines, bool allowUTF8)
		{
			this.lines = lines;
			this.allowUTF8 = allowUTF8;
			this.nextLine = 0;
			this.bytes = null;
			this.start = 0;
			this.end = 0;
			this.position = 0;
			this.ParseNextLine();
		}

		public ValueParser(MimeStringList lines, ValueParser valueParser)
		{
			this.lines = lines;
			this.allowUTF8 = valueParser.allowUTF8;
			this.nextLine = valueParser.nextLine;
			if (this.nextLine > 0 && this.nextLine <= this.lines.Count)
			{
				int num;
				this.bytes = this.lines[this.nextLine - 1].GetData(out this.start, out num);
				this.start = valueParser.start;
				this.position = valueParser.position;
				this.end = valueParser.end;
				return;
			}
			this.bytes = null;
			this.start = 0;
			this.end = 0;
			this.position = 0;
		}

		private bool Eof
		{
			get
			{
				return this.nextLine >= this.lines.Count;
			}
		}

		public static int ParseToken(string value, int currentOffset, bool allowUTF8)
		{
			return MimeScan.FindEndOf(MimeScan.Token.Token, value, currentOffset, allowUTF8);
		}

		public static int ParseToken(MimeString str, out int characterCount, bool allowUTF8)
		{
			return MimeScan.FindEndOf(MimeScan.Token.Token, str.Data, str.Offset, str.Length, out characterCount, allowUTF8);
		}

		public bool ParseToDelimiter(bool ignoreNextByte, bool separateWithWhitespace, ref MimeStringList phrase)
		{
			bool result = false;
			int num = ignoreNextByte ? 1 : 0;
			for (;;)
			{
				int num2 = 0;
				num += MimeScan.FindEndOf(MimeScan.Token.Atom, this.bytes, this.position + num, this.end - this.position - num, out num2, this.allowUTF8);
				if (num != 0)
				{
					result = true;
					if (phrase.Length != 0 && separateWithWhitespace)
					{
						if (this.position == this.start || this.bytes[this.position - 1] != 32)
						{
							phrase.AppendFragment(ValueParser.SpaceLine);
						}
						else
						{
							this.position--;
							num++;
						}
					}
					separateWithWhitespace = false;
					phrase.AppendFragment(new MimeString(this.bytes, this.position, num));
					this.position += num;
				}
				if (this.position != this.end || !this.ParseNextLine())
				{
					break;
				}
				num = 0;
			}
			return result;
		}

		public byte ParseGet()
		{
			if (this.position == this.end && !this.ParseNextLine())
			{
				return 0;
			}
			return this.bytes[this.position++];
		}

		public void ParseUnget()
		{
			if (this.position == this.start)
			{
				this.ParseUngetLine();
			}
			this.position--;
		}

		public void ParseQString(bool save, ref MimeStringList phrase, bool handleISO2022)
		{
			bool flag = false;
			if (save)
			{
				phrase.AppendFragment(new MimeString(this.bytes, this.position, 1, 268435456U));
			}
			this.position++;
			bool flag2 = true;
			for (;;)
			{
				int num = MimeScan.ScanQuotedString(this.bytes, this.position, this.end - this.position, handleISO2022, ref flag);
				if (num != 0)
				{
					if (save)
					{
						phrase.AppendFragment(new MimeString(this.bytes, this.position, num));
					}
					this.position += num;
				}
				if (this.position != this.end)
				{
					if (this.bytes[this.position] == 14 || this.bytes[this.position] == 27)
					{
						this.ParseEscapedString(save, ref phrase, out flag2);
					}
					else
					{
						if (save)
						{
							phrase.AppendFragment(new MimeString(this.bytes, this.position, 1, 268435456U));
						}
						this.position++;
						if (this.bytes[this.position - 1] == 34)
						{
							break;
						}
						flag = true;
					}
				}
				else if (!this.ParseNextLine())
				{
					goto Block_8;
				}
			}
			return;
			Block_8:
			if (save && flag2)
			{
				phrase.AppendFragment(new MimeString(MimeString.DoubleQuote, 0, MimeString.DoubleQuote.Length, 268435456U));
			}
		}

		public void ParseComment(bool save, bool saveInnerOnly, ref MimeStringList comment, bool handleISO2022)
		{
			int num = 1;
			bool flag = false;
			int num2 = 0;
			if (save && !saveInnerOnly)
			{
				comment.AppendFragment(new MimeString(this.bytes, this.position, 1));
			}
			this.position++;
			for (;;)
			{
				int num3 = MimeScan.ScanComment(this.bytes, this.position, this.end - this.position, handleISO2022, ref num, ref flag);
				if (num3 != 0)
				{
					if (save)
					{
						if (num == 0 && saveInnerOnly)
						{
							num2 = 1;
						}
						comment.AppendFragment(new MimeString(this.bytes, this.position, num3 - num2));
					}
					this.position += num3;
					if (num == 0)
					{
						break;
					}
				}
				if (this.position != this.end && (this.bytes[this.position] == 14 || this.bytes[this.position] == 27))
				{
					bool flag2;
					this.ParseEscapedString(save, ref comment, out flag2);
				}
				else if (!this.ParseNextLine())
				{
					return;
				}
			}
		}

		public bool ParseNextLine()
		{
			if (this.nextLine >= this.lines.Count)
			{
				return false;
			}
			int num;
			this.bytes = this.lines[this.nextLine].GetData(out this.start, out num);
			this.position = this.start;
			this.end = this.start + num;
			this.nextLine++;
			return true;
		}

		public void ParseUngetLine()
		{
			int num;
			this.bytes = this.lines[this.nextLine - 2].GetData(out this.start, out num);
			this.position = (this.end = this.start + num);
			this.nextLine--;
		}

		public void ParseWhitespace(bool save, ref MimeStringList phrase)
		{
			for (;;)
			{
				int num = MimeScan.SkipLwsp(this.bytes, this.position, this.end - this.position);
				if (save && num != 0)
				{
					phrase.AppendFragment(new MimeString(this.bytes, this.position, num));
				}
				this.position += num;
				if (this.position != this.end)
				{
					break;
				}
				if (!this.ParseNextLine())
				{
					return;
				}
			}
		}

		public void ParseCFWS(bool save, ref MimeStringList phrase, bool handleISO2022)
		{
			for (;;)
			{
				int num = MimeScan.SkipLwsp(this.bytes, this.position, this.end - this.position);
				if (save && num != 0)
				{
					phrase.AppendFragment(new MimeString(this.bytes, this.position, num));
				}
				this.position += num;
				if (this.position != this.end)
				{
					if (this.bytes[this.position] != 40)
					{
						break;
					}
					this.ParseComment(save, false, ref phrase, handleISO2022);
				}
				else if (!this.ParseNextLine())
				{
					break;
				}
			}
		}

		public void ParseSkipToNextDelimiterByte(byte delimiter)
		{
			MimeStringList mimeStringList = default(MimeStringList);
			for (;;)
			{
				if (this.position != this.end)
				{
					byte b = this.bytes[this.position];
					if (b == delimiter)
					{
						break;
					}
					if (b == 34)
					{
						this.ParseQString(false, ref mimeStringList, true);
					}
					else if (b == 40)
					{
						this.ParseComment(false, false, ref mimeStringList, true);
					}
					else
					{
						this.position++;
						this.ParseCFWS(false, ref mimeStringList, true);
						int num = 0;
						this.position += MimeScan.FindEndOf(MimeScan.Token.Atom, this.bytes, this.position, this.end - this.position, out num, this.allowUTF8);
					}
				}
				else if (!this.ParseNextLine())
				{
					return;
				}
			}
		}

		public MimeString ParseToken()
		{
			return this.ParseToken(MimeScan.Token.Token);
		}

		public MimeString ParseToken(MimeScan.Token token)
		{
			MimeStringList mimeStringList = default(MimeStringList);
			while (this.position != this.end || this.ParseNextLine())
			{
				int num = 0;
				int num2 = MimeScan.FindEndOf(token, this.bytes, this.position, this.end - this.position, out num, this.allowUTF8);
				if (num2 == 0)
				{
					break;
				}
				mimeStringList.AppendFragment(new MimeString(this.bytes, this.position, num2));
				this.position += num2;
			}
			if (mimeStringList.Count == 0)
			{
				return default(MimeString);
			}
			if (mimeStringList.Count == 1)
			{
				return mimeStringList[0];
			}
			byte[] sz = mimeStringList.GetSz();
			return new MimeString(sz, 0, sz.Length);
		}

		public void ParseParameterValue(ref MimeStringList value, ref bool goodValue, bool handleISO2022)
		{
			MimeStringList mimeStringList = default(MimeStringList);
			goodValue = true;
			while (this.position != this.end || this.ParseNextLine())
			{
				byte b = this.bytes[this.position];
				if (b == 34)
				{
					value.Reset();
					mimeStringList.Reset();
					this.ParseQString(true, ref value, handleISO2022);
					return;
				}
				if (b == 40 || MimeScan.IsLWSP(b))
				{
					this.ParseCFWS(true, ref mimeStringList, handleISO2022);
				}
				else
				{
					if (b == 59)
					{
						return;
					}
					int num = this.position;
					do
					{
						int num2 = 1;
						if (!MimeScan.IsToken(b))
						{
							if (this.allowUTF8 && b >= 128)
							{
								if (!MimeScan.IsUTF8NonASCII(this.bytes, this.position, this.end, out num2))
								{
									num2 = 1;
									goodValue = false;
								}
							}
							else
							{
								goodValue = false;
							}
						}
						this.position += num2;
						if (this.position == this.end)
						{
							break;
						}
						b = this.bytes[this.position];
					}
					while (b != 59 && b != 40 && !MimeScan.IsLWSP(b));
					value.TakeOverAppend(ref mimeStringList);
					value.AppendFragment(new MimeString(this.bytes, num, this.position - num));
				}
			}
		}

		public void ParseDomainLiteral(bool save, ref MimeStringList domain)
		{
			bool flag = false;
			int num = this.position;
			this.position++;
			for (;;)
			{
				if (this.position == this.end)
				{
					if (num != this.position && save)
					{
						domain.AppendFragment(new MimeString(this.bytes, num, this.position - num));
					}
					if (!this.ParseNextLine())
					{
						break;
					}
					num = this.position;
				}
				byte b = this.bytes[this.position++];
				if (flag)
				{
					flag = false;
				}
				else if (b == 92)
				{
					flag = true;
				}
				else if (b == 93)
				{
					goto IL_91;
				}
			}
			num = this.position;
			IL_91:
			if (num != this.position && save)
			{
				domain.AppendFragment(new MimeString(this.bytes, num, this.position - num));
			}
		}

		public void ParseToEnd(ref MimeStringList phrase)
		{
			if (this.position != this.end)
			{
				phrase.AppendFragment(new MimeString(this.bytes, this.position, this.end - this.position));
				this.position = this.end;
			}
			while (this.ParseNextLine())
			{
				phrase.AppendFragment(new MimeString(this.bytes, this.start, this.end - this.start));
				this.position = this.end;
			}
		}

		public void ParseAppendLastByte(ref MimeStringList phrase)
		{
			phrase.AppendFragment(new MimeString(this.bytes, this.position - 1, 1));
		}

		public void ParseAppendSpace(ref MimeStringList phrase)
		{
			if (this.position == this.start || this.bytes[this.position - 1] != 32)
			{
				phrase.AppendFragment(ValueParser.SpaceLine);
				return;
			}
			phrase.AppendFragment(new MimeString(this.bytes, this.position - 1, 1));
		}

		private void ParseEscapedString(bool save, ref MimeStringList outStr, out bool singleByte)
		{
			bool flag = this.bytes[this.position] == 27;
			if (save)
			{
				outStr.AppendFragment(new MimeString(this.bytes, this.position, 1, 536870912U));
			}
			this.position++;
			if (flag && !this.ParseEscapeSequence(save, ref outStr))
			{
				singleByte = true;
				return;
			}
			singleByte = false;
			do
			{
				int num = MimeScan.ScanJISString(this.bytes, this.position, this.end - this.position, ref singleByte);
				if (save && num != 0)
				{
					outStr.AppendFragment(new MimeString(this.bytes, this.position, num, 536870912U));
				}
				this.position += num;
			}
			while (!singleByte && this.ParseNextLine());
			if (!flag && this.position != this.end && this.bytes[this.position] == 15)
			{
				if (save)
				{
					outStr.AppendFragment(new MimeString(this.bytes, this.position, 1, 536870912U));
				}
				this.position++;
			}
		}

		private bool ParseEscapeSequence(bool save, ref MimeStringList outStr)
		{
			byte b = this.ParseGet();
			byte b2 = this.ParseGet();
			byte b3 = this.ParseGet();
			if (b3 != 0)
			{
				this.ParseUnget();
			}
			if (b2 != 0)
			{
				this.ParseUnget();
			}
			if (b != 0)
			{
				this.ParseUnget();
			}
			int num = 0;
			bool result = false;
			byte b4 = b;
			if (b4 != 36)
			{
				if (b4 != 40)
				{
					switch (b4)
					{
					case 78:
					case 79:
						if (b2 >= 33)
						{
							num = 2;
							if (b3 >= 33)
							{
								num = 3;
							}
						}
						break;
					}
				}
				else if (b2 == 73)
				{
					result = true;
					num = 2;
				}
				else if (b2 == 66 || b2 == 74 || b2 == 72)
				{
					num = 2;
				}
			}
			else if (b2 == 66 || b2 == 65 || b2 == 64)
			{
				num = 2;
				result = true;
			}
			else if (b2 == 40 && (b3 == 67 || b3 == 68))
			{
				num = 3;
				result = true;
			}
			while (num-- != 0)
			{
				this.ParseGet();
				if (save)
				{
					outStr.AppendFragment(new MimeString(this.bytes, this.position - 1, 1, 536870912U));
				}
			}
			return result;
		}

		private static readonly MimeString SpaceLine = new MimeString(" ");

		private MimeStringList lines;

		private int nextLine;

		private byte[] bytes;

		private int start;

		private int end;

		private int position;

		private readonly bool allowUTF8;
	}
}
