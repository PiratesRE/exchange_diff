using System;
using System.Diagnostics;
using System.Text;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal class Token
	{
		public Token()
		{
			this.Reset();
		}

		public TokenId TokenId
		{
			get
			{
				return this.tokenId;
			}
			set
			{
				this.tokenId = value;
			}
		}

		public int Argument
		{
			get
			{
				return this.argument;
			}
			set
			{
				this.argument = value;
			}
		}

		public Encoding TokenEncoding
		{
			get
			{
				return this.tokenEncoding;
			}
			set
			{
				this.tokenEncoding = value;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.Whole.Tail == this.Whole.Head;
			}
		}

		public Token.RunEnumerator Runs
		{
			get
			{
				return new Token.RunEnumerator(this);
			}
		}

		public Token.TextReader Text
		{
			get
			{
				return new Token.TextReader(this);
			}
		}

		public bool IsWhitespaceOnly
		{
			get
			{
				return this.IsWhitespaceOnlyImp(ref this.Whole);
			}
		}

		internal static int LiteralLength(int literal)
		{
			if (literal <= 65535)
			{
				return 1;
			}
			return 2;
		}

		internal static char LiteralFirstChar(int literal)
		{
			if (literal <= 65535)
			{
				return (char)literal;
			}
			return ParseSupport.HighSurrogateCharFromUcs4(literal);
		}

		internal static char LiteralLastChar(int literal)
		{
			if (literal <= 65535)
			{
				return (char)literal;
			}
			return ParseSupport.LowSurrogateCharFromUcs4(literal);
		}

		protected internal bool IsWhitespaceOnlyImp(ref Token.Fragment fragment)
		{
			bool result = true;
			for (int num = fragment.Head; num != fragment.Tail; num++)
			{
				if (this.RunList[num].Type >= (RunType)2147483648U && this.RunList[num].TextType > RunTextType.UnusualWhitespace)
				{
					result = false;
					break;
				}
			}
			return result;
		}

		protected internal int Read(ref Token.Fragment fragment, ref Token.FragmentPosition position, char[] buffer, int offset, int count)
		{
			int num = offset;
			int num2 = position.Run;
			if (num2 == fragment.Head - 1)
			{
				num2 = (position.Run = fragment.Head);
			}
			if (num2 != fragment.Tail)
			{
				int num3 = position.RunOffset;
				int num4 = position.RunDeltaOffset;
				int num6;
				for (;;)
				{
					Token.RunEntry runEntry = this.RunList[num2];
					if (runEntry.Type == (RunType)3221225472U)
					{
						int num5 = Token.LiteralLength(runEntry.Value);
						if (num4 != num5)
						{
							if (num5 == 1)
							{
								buffer[offset++] = (char)runEntry.Value;
								count--;
							}
							else if (num4 != 0)
							{
								buffer[offset++] = Token.LiteralLastChar(runEntry.Value);
								count--;
							}
							else
							{
								buffer[offset++] = Token.LiteralFirstChar(runEntry.Value);
								count--;
								if (count == 0)
								{
									break;
								}
								buffer[offset++] = Token.LiteralLastChar(runEntry.Value);
								count--;
							}
						}
					}
					else if (runEntry.Type == (RunType)2147483648U)
					{
						num6 = Math.Min(count, runEntry.Length - num4);
						System.Buffer.BlockCopy(this.Buffer, (num3 + num4) * 2, buffer, offset * 2, num6 * 2);
						offset += num6;
						count -= num6;
						if (num4 + num6 != runEntry.Length)
						{
							goto Block_9;
						}
					}
					num3 += runEntry.Length;
					num4 = 0;
					if (++num2 == fragment.Tail || count == 0)
					{
						goto IL_17D;
					}
				}
				num4 = 1;
				goto IL_17D;
				Block_9:
				num4 += num6;
				IL_17D:
				position.Run = num2;
				position.RunOffset = num3;
				position.RunDeltaOffset = num4;
			}
			return offset - num;
		}

		protected internal int ReadOriginal(ref Token.Fragment fragment, ref Token.FragmentPosition position, char[] buffer, int offset, int count)
		{
			int num = offset;
			int num2 = position.Run;
			if (num2 == fragment.Head - 1)
			{
				num2 = (position.Run = fragment.Head);
			}
			if (num2 != fragment.Tail)
			{
				int num3 = position.RunOffset;
				int num4 = position.RunDeltaOffset;
				int num5;
				for (;;)
				{
					Token.RunEntry runEntry = this.RunList[num2];
					if (runEntry.Type == (RunType)3221225472U || runEntry.Type == (RunType)2147483648U)
					{
						num5 = Math.Min(count, runEntry.Length - num4);
						System.Buffer.BlockCopy(this.Buffer, (num3 + num4) * 2, buffer, offset * 2, num5 * 2);
						offset += num5;
						count -= num5;
						if (num4 + num5 != runEntry.Length)
						{
							break;
						}
					}
					num3 += runEntry.Length;
					num4 = 0;
					if (++num2 == fragment.Tail || count == 0)
					{
						goto IL_DD;
					}
				}
				num4 += num5;
				IL_DD:
				position.Run = num2;
				position.RunOffset = num3;
				position.RunDeltaOffset = num4;
			}
			return offset - num;
		}

		protected internal int Read(Token.LexicalUnit unit, ref Token.FragmentPosition position, char[] buffer, int offset, int count)
		{
			int num = offset;
			if (unit.Head != -1)
			{
				uint majorKind = this.RunList[unit.Head].MajorKind;
				int num2 = position.Run;
				if (num2 == unit.Head - 1)
				{
					num2 = (position.Run = unit.Head);
				}
				Token.RunEntry runEntry = this.RunList[num2];
				if (num2 == unit.Head || runEntry.MajorKindPlusStartFlag == majorKind)
				{
					int num3 = position.RunOffset;
					int num4 = position.RunDeltaOffset;
					int num6;
					for (;;)
					{
						if (runEntry.Type == (RunType)3221225472U)
						{
							int num5 = Token.LiteralLength(runEntry.Value);
							if (num4 != num5)
							{
								if (num5 == 1)
								{
									buffer[offset++] = (char)runEntry.Value;
									count--;
								}
								else if (num4 != 0)
								{
									buffer[offset++] = Token.LiteralLastChar(runEntry.Value);
									count--;
								}
								else
								{
									buffer[offset++] = Token.LiteralFirstChar(runEntry.Value);
									count--;
									if (count == 0)
									{
										break;
									}
									buffer[offset++] = Token.LiteralLastChar(runEntry.Value);
									count--;
								}
							}
						}
						else if (runEntry.Type == (RunType)2147483648U)
						{
							num6 = Math.Min(count, runEntry.Length - num4);
							System.Buffer.BlockCopy(this.Buffer, (num3 + num4) * 2, buffer, offset * 2, num6 * 2);
							offset += num6;
							count -= num6;
							if (num4 + num6 != runEntry.Length)
							{
								goto Block_10;
							}
						}
						num3 += runEntry.Length;
						num4 = 0;
						runEntry = this.RunList[++num2];
						if (runEntry.MajorKindPlusStartFlag != majorKind || count == 0)
						{
							goto IL_1CF;
						}
					}
					num4 = 1;
					goto IL_1CF;
					Block_10:
					num4 += num6;
					IL_1CF:
					position.Run = num2;
					position.RunOffset = num3;
					position.RunDeltaOffset = num4;
				}
			}
			return offset - num;
		}

		protected internal virtual void Rewind()
		{
			this.WholePosition.Rewind(this.Whole);
		}

		protected internal int GetLength(ref Token.Fragment fragment)
		{
			int num = fragment.Head;
			int num2 = 0;
			if (num != fragment.Tail)
			{
				do
				{
					Token.RunEntry runEntry = this.RunList[num];
					if (runEntry.Type == (RunType)2147483648U)
					{
						num2 += runEntry.Length;
					}
					else if (runEntry.Type == (RunType)3221225472U)
					{
						num2 += Token.LiteralLength(runEntry.Value);
					}
				}
				while (++num != fragment.Tail);
			}
			return num2;
		}

		protected internal int GetOriginalLength(ref Token.Fragment fragment)
		{
			int num = fragment.Head;
			int num2 = 0;
			if (num != fragment.Tail)
			{
				do
				{
					Token.RunEntry runEntry = this.RunList[num];
					if (runEntry.Type == (RunType)2147483648U || runEntry.Type == (RunType)3221225472U)
					{
						num2 += runEntry.Length;
					}
				}
				while (++num != fragment.Tail);
			}
			return num2;
		}

		protected internal int GetLength(Token.LexicalUnit unit)
		{
			int num = unit.Head;
			int num2 = 0;
			if (num != -1)
			{
				Token.RunEntry runEntry = this.RunList[num];
				uint majorKind = runEntry.MajorKind;
				do
				{
					if (runEntry.Type == (RunType)2147483648U)
					{
						num2 += runEntry.Length;
					}
					else if (runEntry.Type == (RunType)3221225472U)
					{
						num2 += Token.LiteralLength(runEntry.Value);
					}
					runEntry = this.RunList[++num];
				}
				while (runEntry.MajorKindPlusStartFlag == majorKind);
			}
			return num2;
		}

		protected internal bool IsFragmentEmpty(ref Token.Fragment fragment)
		{
			int num = fragment.Head;
			if (num != fragment.Tail)
			{
				for (;;)
				{
					Token.RunEntry runEntry = this.RunList[num];
					if (runEntry.Type == (RunType)2147483648U || runEntry.Type == (RunType)3221225472U)
					{
						break;
					}
					if (++num == fragment.Tail)
					{
						return true;
					}
				}
				return false;
			}
			return true;
		}

		protected internal bool IsFragmentEmpty(Token.LexicalUnit unit)
		{
			int num = unit.Head;
			if (num != -1)
			{
				Token.RunEntry runEntry = this.RunList[num];
				uint majorKind = runEntry.MajorKind;
				while (runEntry.Type != (RunType)2147483648U && runEntry.Type != (RunType)3221225472U)
				{
					runEntry = this.RunList[++num];
					if (runEntry.MajorKindPlusStartFlag != majorKind)
					{
						return true;
					}
				}
				return false;
			}
			return true;
		}

		protected internal bool IsContiguous(ref Token.Fragment fragment)
		{
			return fragment.Head + 1 == fragment.Tail && this.RunList[fragment.Head].Type == (RunType)2147483648U;
		}

		protected internal bool IsContiguous(Token.LexicalUnit unit)
		{
			return this.RunList[unit.Head].Type == (RunType)2147483648U && this.RunList[unit.Head].MajorKind != this.RunList[unit.Head + 1].MajorKindPlusStartFlag;
		}

		protected internal int CalculateHashLowerCase(Token.Fragment fragment)
		{
			int num = fragment.Head;
			if (num == fragment.Tail)
			{
				return HashCode.CalculateEmptyHash();
			}
			int num2 = fragment.HeadOffset;
			if (num + 1 == fragment.Tail && this.RunList[num].Type == (RunType)2147483648U)
			{
				return HashCode.CalculateLowerCase(this.Buffer, num2, this.RunList[num].Length);
			}
			HashCode hashCode = new HashCode(true);
			do
			{
				Token.RunEntry runEntry = this.RunList[num];
				if (runEntry.Type == (RunType)2147483648U)
				{
					hashCode.AdvanceLowerCase(this.Buffer, num2, runEntry.Length);
				}
				else if (runEntry.Type == (RunType)3221225472U)
				{
					hashCode.AdvanceLowerCase(runEntry.Value);
				}
				num2 += runEntry.Length;
			}
			while (++num != fragment.Tail);
			return hashCode.FinalizeHash();
		}

		protected internal int CalculateHashLowerCase(Token.LexicalUnit unit)
		{
			int num = unit.Head;
			if (num == -1)
			{
				return HashCode.CalculateEmptyHash();
			}
			int num2 = unit.HeadOffset;
			Token.RunEntry runEntry = this.RunList[num];
			uint majorKind = runEntry.MajorKind;
			if (runEntry.Type == (RunType)2147483648U && majorKind != this.RunList[num + 1].MajorKindPlusStartFlag)
			{
				return HashCode.CalculateLowerCase(this.Buffer, num2, runEntry.Length);
			}
			HashCode hashCode = new HashCode(true);
			do
			{
				if (runEntry.Type == (RunType)2147483648U)
				{
					hashCode.AdvanceLowerCase(this.Buffer, num2, runEntry.Length);
				}
				else if (runEntry.Type == (RunType)3221225472U)
				{
					hashCode.AdvanceLowerCase(runEntry.Value);
				}
				num2 += runEntry.Length;
				runEntry = this.RunList[++num];
			}
			while (runEntry.MajorKindPlusStartFlag == majorKind);
			return hashCode.FinalizeHash();
		}

		protected internal int CalculateHash(Token.Fragment fragment)
		{
			int num = fragment.Head;
			if (num == fragment.Tail)
			{
				return HashCode.CalculateEmptyHash();
			}
			int num2 = fragment.HeadOffset;
			if (num + 1 == fragment.Tail && this.RunList[num].Type == (RunType)2147483648U)
			{
				return HashCode.Calculate(this.Buffer, num2, this.RunList[num].Length);
			}
			HashCode hashCode = new HashCode(true);
			do
			{
				Token.RunEntry runEntry = this.RunList[num];
				if (runEntry.Type == (RunType)2147483648U)
				{
					hashCode.Advance(this.Buffer, num2, runEntry.Length);
				}
				else if (runEntry.Type == (RunType)3221225472U)
				{
					hashCode.Advance(runEntry.Value);
				}
				num2 += runEntry.Length;
			}
			while (++num != fragment.Tail);
			return hashCode.FinalizeHash();
		}

		protected internal int CalculateHash(Token.LexicalUnit unit)
		{
			int num = unit.Head;
			if (num == -1)
			{
				return HashCode.CalculateEmptyHash();
			}
			int num2 = unit.HeadOffset;
			Token.RunEntry runEntry = this.RunList[num];
			uint majorKind = runEntry.MajorKind;
			if (runEntry.Type == (RunType)2147483648U && majorKind != this.RunList[num + 1].MajorKindPlusStartFlag)
			{
				return HashCode.Calculate(this.Buffer, num2, runEntry.Length);
			}
			HashCode hashCode = new HashCode(true);
			do
			{
				if (runEntry.Type == (RunType)2147483648U)
				{
					hashCode.Advance(this.Buffer, num2, runEntry.Length);
				}
				else if (runEntry.Type == (RunType)3221225472U)
				{
					hashCode.Advance(runEntry.Value);
				}
				num2 += runEntry.Length;
				runEntry = this.RunList[++num];
			}
			while (runEntry.MajorKindPlusStartFlag == majorKind);
			return hashCode.FinalizeHash();
		}

		protected internal void WriteOriginalTo(ref Token.Fragment fragment, ITextSink sink)
		{
			int num = fragment.Head;
			if (num != fragment.Tail)
			{
				int num2 = fragment.HeadOffset;
				do
				{
					Token.RunEntry runEntry = this.RunList[num];
					if (runEntry.Type == (RunType)2147483648U || runEntry.Type == (RunType)3221225472U)
					{
						sink.Write(this.Buffer, num2, runEntry.Length);
					}
					num2 += runEntry.Length;
				}
				while (++num != fragment.Tail && !sink.IsEnough);
			}
		}

		protected internal void WriteTo(ref Token.Fragment fragment, ITextSink sink)
		{
			int num = fragment.Head;
			if (num != fragment.Tail)
			{
				int num2 = fragment.HeadOffset;
				do
				{
					Token.RunEntry runEntry = this.RunList[num];
					if (runEntry.Type == (RunType)2147483648U)
					{
						sink.Write(this.Buffer, num2, runEntry.Length);
					}
					else if (runEntry.Type == (RunType)3221225472U)
					{
						sink.Write(runEntry.Value);
					}
					num2 += runEntry.Length;
				}
				while (++num != fragment.Tail && !sink.IsEnough);
			}
		}

		protected internal void WriteTo(Token.LexicalUnit unit, ITextSink sink)
		{
			int num = unit.Head;
			if (num != -1)
			{
				int num2 = unit.HeadOffset;
				Token.RunEntry runEntry = this.RunList[num];
				uint majorKind = runEntry.MajorKind;
				do
				{
					if (runEntry.Type == (RunType)2147483648U)
					{
						sink.Write(this.Buffer, num2, runEntry.Length);
					}
					else if (runEntry.Type == (RunType)3221225472U)
					{
						sink.Write(runEntry.Value);
					}
					num2 += runEntry.Length;
					runEntry = this.RunList[++num];
				}
				while (runEntry.MajorKindPlusStartFlag == majorKind && !sink.IsEnough);
			}
		}

		protected internal void WriteToAndCollapseWhitespace(ref Token.Fragment fragment, ITextSink sink, ref CollapseWhitespaceState collapseWhitespaceState)
		{
			int num = fragment.Head;
			if (num != fragment.Tail)
			{
				int num2 = fragment.HeadOffset;
				if (this.RunList[num].Type < (RunType)2147483648U)
				{
					this.SkipNonTextRuns(ref num, ref num2, fragment.Tail);
				}
				while (num != fragment.Tail && !sink.IsEnough)
				{
					if (this.RunList[num].TextType <= RunTextType.Nbsp)
					{
						if (this.RunList[num].TextType == RunTextType.NewLine)
						{
							collapseWhitespaceState = CollapseWhitespaceState.NewLine;
						}
						else if (collapseWhitespaceState == CollapseWhitespaceState.NonSpace)
						{
							collapseWhitespaceState = CollapseWhitespaceState.Whitespace;
						}
					}
					else
					{
						if (collapseWhitespaceState != CollapseWhitespaceState.NonSpace)
						{
							if (collapseWhitespaceState == CollapseWhitespaceState.NewLine)
							{
								sink.Write(Token.staticCollapseWhitespace, 1, 2);
							}
							else
							{
								sink.Write(Token.staticCollapseWhitespace, 0, 1);
							}
							collapseWhitespaceState = CollapseWhitespaceState.NonSpace;
						}
						if (this.RunList[num].Type == (RunType)3221225472U)
						{
							sink.Write(this.RunList[num].Value);
						}
						else
						{
							sink.Write(this.Buffer, num2, this.RunList[num].Length);
						}
					}
					num2 += this.RunList[num].Length;
					num++;
					if (num != fragment.Tail && this.RunList[num].Type < (RunType)2147483648U)
					{
						this.SkipNonTextRuns(ref num, ref num2, fragment.Tail);
					}
				}
			}
		}

		protected internal string GetString(ref Token.Fragment fragment, int maxLength)
		{
			if (fragment.Head == fragment.Tail)
			{
				return string.Empty;
			}
			if (this.IsContiguous(ref fragment))
			{
				return new string(this.Buffer, fragment.HeadOffset, this.GetLength(ref fragment));
			}
			if (this.IsFragmentEmpty(ref fragment))
			{
				return string.Empty;
			}
			if (this.stringBuildSink == null)
			{
				this.stringBuildSink = new StringBuildSink();
			}
			this.stringBuildSink.Reset(maxLength);
			this.WriteTo(ref fragment, this.stringBuildSink);
			return this.stringBuildSink.ToString();
		}

		protected internal string GetString(Token.LexicalUnit unit, int maxLength)
		{
			if (this.IsFragmentEmpty(unit))
			{
				return string.Empty;
			}
			if (this.IsContiguous(unit))
			{
				return new string(this.Buffer, unit.HeadOffset, this.GetLength(unit));
			}
			if (this.stringBuildSink == null)
			{
				this.stringBuildSink = new StringBuildSink();
			}
			this.stringBuildSink.Reset(maxLength);
			this.WriteTo(unit, this.stringBuildSink);
			return this.stringBuildSink.ToString();
		}

		protected internal bool CaseInsensitiveCompareEqual(ref Token.Fragment fragment, string str)
		{
			if (this.compareSink == null)
			{
				this.compareSink = new Token.LowerCaseCompareSink();
			}
			this.compareSink.Reset(str);
			this.WriteTo(ref fragment, this.compareSink);
			return this.compareSink.IsEqual;
		}

		protected internal bool CaseInsensitiveCompareEqual(Token.LexicalUnit unit, string str)
		{
			if (this.compareSink == null)
			{
				this.compareSink = new Token.LowerCaseCompareSink();
			}
			this.compareSink.Reset(str);
			this.WriteTo(unit, this.compareSink);
			return this.compareSink.IsEqual;
		}

		protected internal virtual bool CaseInsensitiveCompareRunEqual(int runOffset, string str, int strOffset)
		{
			int i = strOffset;
			while (i < str.Length)
			{
				if (ParseSupport.ToLowerCase(this.Buffer[runOffset++]) != str[i++])
				{
					return false;
				}
			}
			return true;
		}

		protected internal bool CaseInsensitiveContainsSubstring(ref Token.Fragment fragment, string str)
		{
			if (this.searchSink == null)
			{
				this.searchSink = new Token.LowerCaseSubstringSearchSink();
			}
			this.searchSink.Reset(str);
			this.WriteTo(ref fragment, this.searchSink);
			return this.searchSink.IsFound;
		}

		protected internal bool CaseInsensitiveContainsSubstring(Token.LexicalUnit unit, string str)
		{
			if (this.searchSink == null)
			{
				this.searchSink = new Token.LowerCaseSubstringSearchSink();
			}
			this.searchSink.Reset(str);
			this.WriteTo(unit, this.searchSink);
			return this.searchSink.IsFound;
		}

		protected internal void StripLeadingWhitespace(ref Token.Fragment fragment)
		{
			int num = fragment.Head;
			if (num != fragment.Tail)
			{
				int num2 = fragment.HeadOffset;
				if (this.RunList[num].Type < (RunType)2147483648U)
				{
					this.SkipNonTextRuns(ref num, ref num2, fragment.Tail);
				}
				if (num == fragment.Tail)
				{
					return;
				}
				int i;
				do
				{
					if (this.RunList[num].Type == (RunType)3221225472U)
					{
						if (this.RunList[num].Value > 65535)
						{
							break;
						}
						CharClass charClass = ParseSupport.GetCharClass((char)this.RunList[num].Value);
						if (!ParseSupport.WhitespaceCharacter(charClass))
						{
							break;
						}
					}
					else
					{
						for (i = num2; i < num2 + this.RunList[num].Length; i++)
						{
							CharClass charClass = ParseSupport.GetCharClass(this.Buffer[i]);
							if (!ParseSupport.WhitespaceCharacter(charClass))
							{
								break;
							}
						}
						if (i < num2 + this.RunList[num].Length)
						{
							goto Block_8;
						}
					}
					num2 += this.RunList[num].Length;
					num++;
					if (num != fragment.Tail && this.RunList[num].Type < (RunType)2147483648U)
					{
						this.SkipNonTextRuns(ref num, ref num2, fragment.Tail);
					}
				}
				while (num != fragment.Tail);
				goto IL_162;
				Block_8:
				Token.RunEntry[] runList = this.RunList;
				int num3 = num;
				runList[num3].Length = runList[num3].Length - (i - num2);
				num2 = i;
				IL_162:
				fragment.Head = num;
				fragment.HeadOffset = num2;
			}
		}

		protected internal bool SkipLeadingWhitespace(Token.LexicalUnit unit, ref Token.FragmentPosition position)
		{
			int num = unit.Head;
			if (num != -1)
			{
				int num2 = unit.HeadOffset;
				Token.RunEntry runEntry = this.RunList[num];
				uint majorKind = runEntry.MajorKind;
				int runDeltaOffset = 0;
				int i;
				do
				{
					if (runEntry.Type == (RunType)3221225472U)
					{
						if (runEntry.Value > 65535)
						{
							break;
						}
						CharClass charClass = ParseSupport.GetCharClass((char)runEntry.Value);
						if (!ParseSupport.WhitespaceCharacter(charClass))
						{
							break;
						}
					}
					else if (runEntry.Type == (RunType)2147483648U)
					{
						for (i = num2; i < num2 + runEntry.Length; i++)
						{
							CharClass charClass = ParseSupport.GetCharClass(this.Buffer[i]);
							if (!ParseSupport.WhitespaceCharacter(charClass))
							{
								break;
							}
						}
						if (i < num2 + runEntry.Length)
						{
							goto Block_7;
						}
					}
					num2 += runEntry.Length;
					runEntry = this.RunList[++num];
				}
				while (runEntry.MajorKindPlusStartFlag == majorKind);
				goto IL_EF;
				Block_7:
				runDeltaOffset = i - num2;
				IL_EF:
				position.Run = num;
				position.RunOffset = num2;
				position.RunDeltaOffset = runDeltaOffset;
				if (num == unit.Head || runEntry.MajorKindPlusStartFlag == majorKind)
				{
					return true;
				}
			}
			return false;
		}

		protected internal bool MoveToNextRun(ref Token.Fragment fragment, ref Token.FragmentPosition position, bool skipInvalid)
		{
			int num = position.Run;
			if (num != fragment.Tail)
			{
				if (num >= fragment.Head)
				{
					position.RunOffset += this.RunList[num].Length;
					position.RunDeltaOffset = 0;
				}
				num++;
				if (skipInvalid)
				{
					while (num != fragment.Tail && this.RunList[num].Type == RunType.Invalid)
					{
						position.RunOffset += this.RunList[num].Length;
						num++;
					}
				}
				position.Run = num;
				return num != fragment.Tail;
			}
			return false;
		}

		protected internal bool IsCurrentEof(ref Token.FragmentPosition position)
		{
			int run = position.Run;
			if (this.RunList[run].Type == (RunType)3221225472U)
			{
				return position.RunDeltaOffset == Token.LiteralLength(this.RunList[run].Value);
			}
			return position.RunDeltaOffset == this.RunList[run].Length;
		}

		protected internal int ReadCurrent(ref Token.FragmentPosition position, char[] buffer, int offset, int count)
		{
			int run = position.Run;
			if (this.RunList[run].Type != (RunType)3221225472U)
			{
				int num = Math.Min(count, this.RunList[run].Length - position.RunDeltaOffset);
				if (num != 0)
				{
					System.Buffer.BlockCopy(this.Buffer, (position.RunOffset + position.RunDeltaOffset) * 2, buffer, offset * 2, num * 2);
					position.RunDeltaOffset += num;
				}
				return num;
			}
			int num2 = Token.LiteralLength(this.RunList[run].Value);
			if (position.RunDeltaOffset == num2)
			{
				return 0;
			}
			if (num2 == 1)
			{
				buffer[offset] = (char)this.RunList[run].Value;
				position.RunDeltaOffset++;
				return 1;
			}
			if (position.RunDeltaOffset != 0)
			{
				buffer[offset] = Token.LiteralLastChar(this.RunList[run].Value);
				position.RunDeltaOffset++;
				return 1;
			}
			buffer[offset++] = Token.LiteralFirstChar(this.RunList[run].Value);
			count--;
			position.RunDeltaOffset++;
			if (count == 0)
			{
				return 1;
			}
			buffer[offset] = Token.LiteralLastChar(this.RunList[run].Value);
			position.RunDeltaOffset++;
			return 2;
		}

		internal void SkipNonTextRuns(ref int run, ref int runOffset, int tail)
		{
			do
			{
				runOffset += this.RunList[run].Length;
				run++;
			}
			while (run != tail && this.RunList[run].Type < (RunType)2147483648U);
		}

		internal void Reset()
		{
			this.tokenId = TokenId.None;
			this.argument = 0;
			this.tokenEncoding = null;
			this.Whole.Reset();
			this.WholePosition.Reset();
		}

		protected internal char[] Buffer;

		protected internal Token.RunEntry[] RunList;

		protected internal Token.Fragment Whole;

		protected internal Token.FragmentPosition WholePosition;

		private static char[] staticCollapseWhitespace = new char[]
		{
			' ',
			'\r',
			'\n'
		};

		private TokenId tokenId;

		private Encoding tokenEncoding;

		private int argument;

		private Token.LowerCaseCompareSink compareSink;

		private Token.LowerCaseSubstringSearchSink searchSink;

		private StringBuildSink stringBuildSink;

		public struct RunEnumerator
		{
			internal RunEnumerator(Token token)
			{
				this.token = token;
			}

			public TokenRun Current
			{
				get
				{
					return new TokenRun(this.token);
				}
			}

			public bool IsValidPosition
			{
				get
				{
					return this.token.WholePosition.Run >= this.token.Whole.Head && this.token.WholePosition.Run < this.token.Whole.Tail;
				}
			}

			public int CurrentIndex
			{
				get
				{
					return this.token.WholePosition.Run;
				}
			}

			public int CurrentOffset
			{
				get
				{
					return this.token.WholePosition.RunOffset;
				}
			}

			public bool MoveNext()
			{
				return this.token.MoveToNextRun(ref this.token.Whole, ref this.token.WholePosition, false);
			}

			public bool MoveNext(bool skipInvalid)
			{
				return this.token.MoveToNextRun(ref this.token.Whole, ref this.token.WholePosition, skipInvalid);
			}

			public void Rewind()
			{
				this.token.WholePosition.Rewind(this.token.Whole);
			}

			public Token.RunEnumerator GetEnumerator()
			{
				return this;
			}

			[Conditional("DEBUG")]
			private void AssertCurrent()
			{
			}

			private Token token;
		}

		public struct TextReader
		{
			internal TextReader(Token token)
			{
				this.token = token;
			}

			public int Length
			{
				get
				{
					return this.token.GetLength(ref this.token.Whole);
				}
			}

			public int OriginalLength
			{
				get
				{
					return this.token.GetOriginalLength(ref this.token.Whole);
				}
			}

			public int Read(char[] buffer, int offset, int count)
			{
				return this.token.Read(ref this.token.Whole, ref this.token.WholePosition, buffer, offset, count);
			}

			public void Rewind()
			{
				this.token.WholePosition.Rewind(this.token.Whole);
			}

			public void WriteTo(ITextSink sink)
			{
				this.token.WriteTo(ref this.token.Whole, sink);
			}

			public void WriteToAndCollapseWhitespace(ITextSink sink, ref CollapseWhitespaceState collapseWhitespaceState)
			{
				this.token.WriteToAndCollapseWhitespace(ref this.token.Whole, sink, ref collapseWhitespaceState);
			}

			public void StripLeadingWhitespace()
			{
				this.token.StripLeadingWhitespace(ref this.token.Whole);
				this.Rewind();
			}

			public int ReadOriginal(char[] buffer, int offset, int count)
			{
				return this.token.ReadOriginal(ref this.token.Whole, ref this.token.WholePosition, buffer, offset, count);
			}

			public void WriteOriginalTo(ITextSink sink)
			{
				this.token.WriteOriginalTo(ref this.token.Whole, sink);
			}

			[Conditional("DEBUG")]
			private void AssertCurrent()
			{
			}

			private Token token;
		}

		internal struct RunEntry
		{
			public RunType Type
			{
				get
				{
					return (RunType)(this.lengthAndType & 3221225472U);
				}
			}

			public RunTextType TextType
			{
				get
				{
					return (RunTextType)(this.lengthAndType & 939524096U);
				}
			}

			public int Length
			{
				get
				{
					return (int)(this.lengthAndType & 16777215U);
				}
				set
				{
					this.lengthAndType = (uint)(value | (int)(this.lengthAndType & 4278190080U));
				}
			}

			public uint Kind
			{
				get
				{
					return this.valueAndKind & 4278190080U;
				}
			}

			public uint MajorKindPlusStartFlag
			{
				get
				{
					return this.valueAndKind & 4227858432U;
				}
			}

			public uint MajorKind
			{
				get
				{
					return this.valueAndKind & 2080374784U;
				}
			}

			public int Value
			{
				get
				{
					return (int)(this.valueAndKind & 16777215U);
				}
			}

			public void Initialize(RunType type, RunTextType textType, uint kind, int length, int value)
			{
				this.lengthAndType = (uint)(length | (int)type | (int)textType);
				this.valueAndKind = (uint)(value | (int)kind);
			}

			public void InitializeSentinel()
			{
				this.valueAndKind = 2147483648U;
			}

			public override string ToString()
			{
				return string.Concat(new object[]
				{
					this.Type.ToString(),
					" - ",
					this.TextType.ToString(),
					" - ",
					((this.Kind & 2147483647U) >> 26).ToString(),
					"/",
					(this.Kind >> 24 & 3U).ToString(),
					" (",
					this.Length,
					") = ",
					this.Value.ToString("X6")
				});
			}

			internal const int MaxRunLength = 134217727;

			internal const int MaxRunValue = 16777215;

			private uint lengthAndType;

			private uint valueAndKind;
		}

		internal struct LexicalUnit
		{
			public void Reset()
			{
				this.Head = -1;
				this.HeadOffset = 0;
			}

			public void Initialize(int run, int offset)
			{
				this.Head = run;
				this.HeadOffset = offset;
			}

			public override string ToString()
			{
				return this.Head.ToString("X") + " / " + this.HeadOffset.ToString("X");
			}

			public int Head;

			public int HeadOffset;
		}

		internal struct Fragment
		{
			public bool IsEmpty
			{
				get
				{
					return this.Head == this.Tail;
				}
			}

			public void Reset()
			{
				this.Head = (this.Tail = (this.HeadOffset = 0));
			}

			public void Initialize(int run, int offset)
			{
				this.Tail = run;
				this.Head = run;
				this.HeadOffset = offset;
			}

			public override string ToString()
			{
				return string.Concat(new string[]
				{
					this.Head.ToString("X"),
					" - ",
					this.Tail.ToString("X"),
					" / ",
					this.HeadOffset.ToString("X")
				});
			}

			public int Head;

			public int Tail;

			public int HeadOffset;
		}

		internal struct FragmentPosition
		{
			public void Reset()
			{
				this.Run = -2;
				this.RunOffset = 0;
				this.RunDeltaOffset = 0;
			}

			public void Rewind(Token.LexicalUnit unit)
			{
				this.Run = unit.Head - 1;
				this.RunOffset = unit.HeadOffset;
				this.RunDeltaOffset = 0;
			}

			public void Rewind(Token.Fragment fragment)
			{
				this.Run = fragment.Head - 1;
				this.RunOffset = fragment.HeadOffset;
				this.RunDeltaOffset = 0;
			}

			public bool SameAs(Token.FragmentPosition pos2)
			{
				return this.Run == pos2.Run && this.RunOffset == pos2.RunOffset && this.RunDeltaOffset == pos2.RunDeltaOffset;
			}

			public override string ToString()
			{
				return string.Concat(new string[]
				{
					this.Run.ToString("X"),
					" / ",
					this.RunOffset.ToString("X"),
					" + ",
					this.RunDeltaOffset.ToString("X")
				});
			}

			public int Run;

			public int RunOffset;

			public int RunDeltaOffset;
		}

		private class LowerCaseCompareSink : ITextSink
		{
			public bool IsEqual
			{
				get
				{
					return !this.definitelyNotEqual && this.strIndex == this.str.Length;
				}
			}

			public bool IsEnough
			{
				get
				{
					return this.definitelyNotEqual;
				}
			}

			public void Reset(string str)
			{
				this.str = str;
				this.strIndex = 0;
				this.definitelyNotEqual = false;
			}

			public void Write(char[] buffer, int offset, int count)
			{
				int num = offset + count;
				while (offset < num)
				{
					if (this.strIndex == 0)
					{
						if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(buffer[offset])))
						{
							offset++;
							continue;
						}
					}
					else if (this.strIndex == this.str.Length)
					{
						if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(buffer[offset])))
						{
							offset++;
							continue;
						}
						this.definitelyNotEqual = true;
						return;
					}
					if (ParseSupport.ToLowerCase(buffer[offset]) != this.str[this.strIndex])
					{
						this.definitelyNotEqual = true;
						return;
					}
					offset++;
					this.strIndex++;
				}
			}

			public void Write(int ucs32Char)
			{
				if (Token.LiteralLength(ucs32Char) != 1)
				{
					this.definitelyNotEqual = true;
					return;
				}
				if (this.strIndex == 0)
				{
					if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass((char)ucs32Char)))
					{
						return;
					}
				}
				else if (this.strIndex == this.str.Length)
				{
					if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass((char)ucs32Char)))
					{
						return;
					}
					this.definitelyNotEqual = true;
					return;
				}
				if (this.str[this.strIndex] != ParseSupport.ToLowerCase((char)ucs32Char))
				{
					this.definitelyNotEqual = true;
					return;
				}
				this.strIndex++;
			}

			private bool definitelyNotEqual;

			private int strIndex;

			private string str;
		}

		private class LowerCaseSubstringSearchSink : ITextSink
		{
			public bool IsFound
			{
				get
				{
					return this.found;
				}
			}

			public bool IsEnough
			{
				get
				{
					return this.found;
				}
			}

			public void Reset(string str)
			{
				this.str = str;
				this.strIndex = 0;
				this.found = false;
			}

			public void Write(char[] buffer, int offset, int count)
			{
				int num = offset + count;
				while (offset < num && this.strIndex < this.str.Length)
				{
					if (ParseSupport.ToLowerCase(buffer[offset]) == this.str[this.strIndex])
					{
						this.strIndex++;
					}
					else
					{
						this.strIndex = 0;
					}
					offset++;
				}
				if (this.strIndex == this.str.Length)
				{
					this.found = true;
				}
			}

			public void Write(int ucs32Char)
			{
				if (Token.LiteralLength(ucs32Char) != 1 || this.str[this.strIndex] != ParseSupport.ToLowerCase((char)ucs32Char))
				{
					this.strIndex = 0;
					return;
				}
				this.strIndex++;
				if (this.strIndex == this.str.Length)
				{
					this.found = true;
				}
			}

			private bool found;

			private int strIndex;

			private string str;
		}
	}
}
