using System;
using Microsoft.Exchange.Data.TextConverters.Internal.Html;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Enriched
{
	internal class EnrichedParser : IDisposable
	{
		public EnrichedParser(ConverterInput input, int maxRuns, bool testBoundaryConditions)
		{
			this.input = input;
			this.tokenBuilder = new HtmlTokenBuilder(null, maxRuns, 0, testBoundaryConditions);
			this.token = this.tokenBuilder.Token;
		}

		public HtmlToken Token
		{
			get
			{
				return this.token;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && this.input != null)
			{
				((IDisposable)this.input).Dispose();
			}
			this.input = null;
			this.parseBuffer = null;
			this.token = null;
			this.tokenBuilder = null;
		}

		public HtmlTokenId Parse()
		{
			HtmlTokenBuilder htmlTokenBuilder = this.tokenBuilder;
			if (htmlTokenBuilder.Valid)
			{
				if (htmlTokenBuilder.IncompleteTag)
				{
					int num = htmlTokenBuilder.RewindTag();
					this.input.ReportProcessed(num - this.parseStart);
					this.parseStart = num;
				}
				else
				{
					this.input.ReportProcessed(this.parseCurrent - this.parseStart);
					this.parseStart = this.parseCurrent;
					htmlTokenBuilder.Reset();
				}
			}
			char[] array = this.parseBuffer;
			int num2 = this.parseCurrent;
			int num3 = this.parseEnd;
			int num4 = this.parseThreshold;
			ConverterDecodingInput converterDecodingInput;
			int num5;
			bool flag2;
			int num6;
			for (;;)
			{
				bool flag = false;
				if (num2 + num4 > num3)
				{
					if (!this.endOfFile)
					{
						this.parseCurrent = num2;
						if (!this.input.ReadMore(ref this.parseBuffer, ref this.parseStart, ref this.parseCurrent, ref this.parseEnd))
						{
							break;
						}
						htmlTokenBuilder.BufferChanged(this.parseBuffer, this.parseStart);
						converterDecodingInput = (this.input as ConverterDecodingInput);
						if (converterDecodingInput != null && converterDecodingInput.EncodingChanged)
						{
							goto Block_7;
						}
						array = this.parseBuffer;
						num2 = this.parseCurrent;
						num3 = this.parseEnd;
						if (this.input.EndOfFile)
						{
							this.endOfFile = true;
						}
						if (!this.endOfFile && num3 - this.parseStart < this.input.MaxTokenSize)
						{
							continue;
						}
					}
					flag = true;
				}
				char c = array[num2];
				CharClass charClass = ParseSupport.GetCharClass(c);
				if (ParseSupport.InvalidUnicodeCharacter(charClass) || num4 > 1)
				{
					if (!this.SkipInvalidCharacters(ref c, ref charClass, ref num2))
					{
						num3 = this.parseEnd;
						if (!flag)
						{
							continue;
						}
						if (num2 == num3 && !htmlTokenBuilder.IsStarted && this.endOfFile)
						{
							goto IL_695;
						}
					}
					num3 = this.parseEnd;
					num4 = (this.parseThreshold = 1);
				}
				num5 = num2;
				switch (this.parseState)
				{
				case EnrichedParser.ParseState.Text:
					htmlTokenBuilder.StartText(num5);
					goto IL_1F9;
				case EnrichedParser.ParseState.Tag:
					if (this.parseEnd - this.parseCurrent < 17 && !flag)
					{
						num4 = (this.parseThreshold = 17);
						continue;
					}
					c = array[++num2];
					charClass = ParseSupport.GetCharClass(c);
					flag2 = false;
					num6 = 1;
					if (c == '/')
					{
						flag2 = true;
						num6++;
						c = array[++num2];
						charClass = ParseSupport.GetCharClass(c);
					}
					c = this.ScanTag(c, ref charClass, ref num2);
					this.nameLength = num2 - (num5 + num6);
					if (c == '>')
					{
						goto IL_366;
					}
					if (this.newLineState == EnrichedParser.NewLineState.OneNewLine)
					{
						goto Block_22;
					}
					htmlTokenBuilder.StartTag(HtmlNameIndex.Unknown, num5);
					if (flag2)
					{
						htmlTokenBuilder.SetEndTag();
					}
					htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, num5, num5 + num6);
					htmlTokenBuilder.StartTagName();
					if (this.nameLength != 0)
					{
						htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, num5 + num6, num2);
					}
					this.parseState = EnrichedParser.ParseState.LongTag;
					goto IL_502;
				case EnrichedParser.ParseState.LongTag:
					if (!htmlTokenBuilder.PrepareToAddMoreRuns(2, num5, HtmlRunKind.Name))
					{
						goto IL_545;
					}
					c = this.ScanTag(c, ref charClass, ref num2);
					if (num2 != num5)
					{
						this.nameLength += num2 - num5;
						htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, num5, num2);
					}
					if (c == '>')
					{
						goto Block_46;
					}
					goto IL_502;
				}
				goto Block_15;
				IL_1F9:
				this.ParseText(c, charClass, ref num2);
				num4 = this.parseThreshold;
				if (this.token.IsEmpty && !flag)
				{
					htmlTokenBuilder.Reset();
					continue;
				}
				goto IL_227;
				IL_502:
				if (flag && num2 + num4 >= num3)
				{
					if (!this.endOfFile)
					{
						goto IL_545;
					}
					if (!this.token.IsTagBegin)
					{
						goto Block_43;
					}
					num2 = this.parseStart;
					htmlTokenBuilder.Reset();
					num5 = num2;
					htmlTokenBuilder.StartText(num5);
					c = array[++num2];
					charClass = ParseSupport.GetCharClass(c);
					this.PrepareToAddTextRun(num5);
					htmlTokenBuilder.AddTextRun(RunTextType.NonSpace, num5, num2);
					this.parseState = EnrichedParser.ParseState.Text;
					goto IL_1F9;
				}
			}
			return HtmlTokenId.None;
			Block_7:
			converterDecodingInput.EncodingChanged = false;
			return htmlTokenBuilder.MakeEmptyToken(HtmlTokenId.EncodingChange, converterDecodingInput.Encoding);
			Block_15:
			this.parseCurrent = num2;
			throw new TextConvertersException("internal error: invalid parse state");
			IL_227:
			htmlTokenBuilder.EndText();
			this.parseCurrent = num2;
			return this.token.HtmlTokenId;
			Block_22:
			this.newLineState = EnrichedParser.NewLineState.None;
			htmlTokenBuilder.StartText(num5);
			htmlTokenBuilder.AddLiteralRun(RunTextType.Space, HtmlRunKind.Text, num5, num5, 32);
			htmlTokenBuilder.EndText();
			this.parseCurrent = num5;
			return this.token.HtmlTokenId;
			IL_366:
			num2++;
			HtmlNameIndex htmlNameIndex = HtmlTokenBuilder.LookupName(array, num5 + num6, this.nameLength);
			if (htmlNameIndex == HtmlNameIndex.FlushLeft || htmlNameIndex == HtmlNameIndex.FlushRight || htmlNameIndex == HtmlNameIndex.FlushBoth || htmlNameIndex == HtmlNameIndex.Center || htmlNameIndex == HtmlNameIndex.Nofill || htmlNameIndex == HtmlNameIndex.ParaIndent || htmlNameIndex == HtmlNameIndex.Excerpt)
			{
				this.newLineState = EnrichedParser.NewLineState.EatTwoNewLines;
				if (htmlNameIndex == HtmlNameIndex.Nofill)
				{
					if (!flag2)
					{
						this.nofill++;
						this.newLineState = EnrichedParser.NewLineState.None;
					}
					else if (this.nofill != 0)
					{
						this.nofill--;
					}
				}
			}
			else
			{
				if (this.newLineState == EnrichedParser.NewLineState.OneNewLine)
				{
					this.newLineState = EnrichedParser.NewLineState.None;
					htmlTokenBuilder.StartText(num5);
					htmlTokenBuilder.AddLiteralRun(RunTextType.Space, HtmlRunKind.Text, num5, num5, 32);
					htmlTokenBuilder.EndText();
					this.parseCurrent = num5;
					return this.token.HtmlTokenId;
				}
				this.newLineState = EnrichedParser.NewLineState.None;
			}
			htmlTokenBuilder.StartTag(HtmlNameIndex.Unknown, num5);
			if (flag2)
			{
				htmlTokenBuilder.SetEndTag();
			}
			htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, num5, num5 + num6);
			htmlTokenBuilder.StartTagName();
			if (this.nameLength != 0)
			{
				htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, num5 + num6, num2 - 1);
			}
			htmlTokenBuilder.EndTagName(htmlNameIndex);
			htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagSuffix, num2 - 1, num2);
			htmlTokenBuilder.EndTag(true);
			if (array[num2] != '<' || num2 + 1 == num3 || array[num2 + 1] == '<' || ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(array[num2 + 1])))
			{
				this.parseState = EnrichedParser.ParseState.Text;
			}
			this.parseCurrent = num2;
			return this.token.HtmlTokenId;
			Block_43:
			htmlTokenBuilder.EndTag(true);
			this.parseCurrent = num2;
			return this.token.HtmlTokenId;
			IL_545:
			htmlTokenBuilder.EndTag(false);
			this.parseCurrent = num2;
			return this.token.HtmlTokenId;
			Block_46:
			htmlTokenBuilder.EndTagName(this.nameLength);
			num2++;
			htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagSuffix, num2 - 1, num2);
			htmlTokenBuilder.EndTag(true);
			if (array[num2] == '<' && num2 + 1 < num3 && array[num2 + 1] != '<' && !ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(array[num2 + 1])))
			{
				this.parseState = EnrichedParser.ParseState.Tag;
			}
			else
			{
				this.parseState = EnrichedParser.ParseState.Text;
			}
			this.parseCurrent = num2;
			return this.token.HtmlTokenId;
			IL_695:
			this.parseCurrent = num2;
			return htmlTokenBuilder.MakeEmptyToken(HtmlTokenId.EndOfFile);
		}

		private bool SkipInvalidCharacters(ref char ch, ref CharClass charClass, ref int parseCurrent)
		{
			int num = parseCurrent;
			int num2 = this.parseEnd;
			while (ParseSupport.InvalidUnicodeCharacter(charClass) && num < num2)
			{
				ch = this.parseBuffer[++num];
				charClass = ParseSupport.GetCharClass(ch);
			}
			if (this.parseThreshold > 1 && num + 1 < num2)
			{
				int num3 = num + 1;
				int num4 = num3;
				while (num4 < num2 && num3 < num + this.parseThreshold)
				{
					char c = this.parseBuffer[num4];
					CharClass charClass2 = ParseSupport.GetCharClass(c);
					if (!ParseSupport.InvalidUnicodeCharacter(charClass2))
					{
						if (num4 != num3)
						{
							this.parseBuffer[num3] = c;
							this.parseBuffer[num4] = '\0';
						}
						num3++;
					}
					num4++;
				}
				if (num4 == num2)
				{
					num2 = (this.parseEnd = this.input.RemoveGap(num3, num2));
				}
			}
			parseCurrent = num;
			return num + this.parseThreshold <= num2;
		}

		private char ScanTag(char ch, ref CharClass charClass, ref int parseCurrent)
		{
			while ((parseCurrent < this.parseEnd || !ParseSupport.InvalidUnicodeCharacter(charClass)) && ch != '>')
			{
				ch = this.parseBuffer[++parseCurrent];
				charClass = ParseSupport.GetCharClass(ch);
			}
			return ch;
		}

		private char ScanText(char ch, ref CharClass charClass, ref int parseCurrent)
		{
			char[] array = this.parseBuffer;
			while (ParseSupport.HtmlTextCharacter(charClass))
			{
				ch = array[++parseCurrent];
				charClass = ParseSupport.GetCharClass(ch);
			}
			return ch;
		}

		private void ParseText(char ch, CharClass charClass, ref int parseCurrent)
		{
			int num = parseCurrent;
			int num2 = this.parseEnd;
			char[] array = this.parseBuffer;
			HtmlTokenBuilder htmlTokenBuilder = this.tokenBuilder;
			char c;
			CharClass charClass2;
			for (;;)
			{
				ch = this.ScanText(ch, ref charClass, ref parseCurrent);
				if (ParseSupport.WhitespaceCharacter(charClass))
				{
					if (parseCurrent != num)
					{
						this.PrepareToAddTextRun(num);
						htmlTokenBuilder.AddTextRun(RunTextType.NonSpace, num, parseCurrent);
					}
					num = parseCurrent;
					if (ch == ' ')
					{
						c = array[parseCurrent + 1];
						charClass2 = ParseSupport.GetCharClass(c);
						if (!ParseSupport.WhitespaceCharacter(charClass2))
						{
							ch = c;
							charClass = charClass2;
							parseCurrent++;
							this.PrepareToAddTextRun(num);
							htmlTokenBuilder.AddTextRun(RunTextType.Space, num, parseCurrent);
							num = parseCurrent;
							goto IL_23F;
						}
					}
					this.ParseWhitespace(ch, charClass, ref parseCurrent);
					if (this.parseThreshold > 1)
					{
						break;
					}
					num = parseCurrent;
					ch = array[parseCurrent];
					charClass = ParseSupport.GetCharClass(ch);
					goto IL_23C;
				}
				else if (ch == '<')
				{
					if (parseCurrent != num)
					{
						this.PrepareToAddTextRun(num);
						htmlTokenBuilder.AddTextRun(RunTextType.NonSpace, num, parseCurrent);
						num = parseCurrent;
					}
					if (array[parseCurrent + 1] != '<')
					{
						goto IL_12A;
					}
					this.PrepareToAddTextRun(num);
					htmlTokenBuilder.AddLiteralRun(RunTextType.NonSpace, HtmlRunKind.Text, num, parseCurrent, 60);
					parseCurrent += 2;
					num = parseCurrent;
					ch = array[parseCurrent];
					charClass = ParseSupport.GetCharClass(ch);
				}
				else if (ch == '&')
				{
					ch = array[++parseCurrent];
					charClass = ParseSupport.GetCharClass(ch);
				}
				else
				{
					if (ParseSupport.NbspCharacter(charClass))
					{
						if (parseCurrent != num)
						{
							this.PrepareToAddTextRun(num);
							htmlTokenBuilder.AddTextRun(RunTextType.NonSpace, num, parseCurrent);
						}
						num = parseCurrent;
						do
						{
							ch = array[++parseCurrent];
							charClass = ParseSupport.GetCharClass(ch);
						}
						while (ParseSupport.NbspCharacter(charClass));
						this.PrepareToAddTextRun(num);
						htmlTokenBuilder.AddTextRun(RunTextType.Nbsp, num, parseCurrent);
						goto IL_23C;
					}
					if (parseCurrent != num)
					{
						this.PrepareToAddTextRun(num);
						htmlTokenBuilder.AddTextRun(RunTextType.NonSpace, num, parseCurrent);
					}
					if (parseCurrent >= num2)
					{
						return;
					}
					for (;;)
					{
						ch = array[++parseCurrent];
						charClass = ParseSupport.GetCharClass(ch);
						if (!ParseSupport.InvalidUnicodeCharacter(charClass) || parseCurrent >= num2)
						{
							goto IL_23C;
						}
					}
				}
				IL_23F:
				if (!htmlTokenBuilder.PrepareToAddMoreRuns(3, num, HtmlRunKind.Text))
				{
					return;
				}
				continue;
				IL_23C:
				num = parseCurrent;
				goto IL_23F;
			}
			return;
			IL_12A:
			c = array[parseCurrent + 1];
			charClass2 = ParseSupport.GetCharClass(c);
			if (!ParseSupport.InvalidUnicodeCharacter(charClass2))
			{
				this.parseState = EnrichedParser.ParseState.Tag;
				return;
			}
			if (this.endOfFile && parseCurrent + 1 == num2)
			{
				parseCurrent++;
				htmlTokenBuilder.AddTextRun(RunTextType.NonSpace, num, parseCurrent);
				return;
			}
			this.parseThreshold = 2;
		}

		private void ParseWhitespace(char ch, CharClass charClass, ref int parseCurrent)
		{
			int num = parseCurrent;
			char[] array = this.parseBuffer;
			HtmlTokenBuilder htmlTokenBuilder = this.tokenBuilder;
			for (;;)
			{
				char c = ch;
				switch (c)
				{
				case '\t':
					do
					{
						ch = array[++parseCurrent];
					}
					while (ch == '\t');
					this.PrepareToAddTextRun(num);
					htmlTokenBuilder.AddTextRun(RunTextType.Tabulation, num, parseCurrent);
					goto IL_196;
				case '\n':
					goto IL_AD;
				case '\v':
				case '\f':
					break;
				case '\r':
				{
					if (array[parseCurrent + 1] == '\n')
					{
						parseCurrent++;
						goto IL_AD;
					}
					CharClass charClass2 = ParseSupport.GetCharClass(array[parseCurrent + 1]);
					if (ParseSupport.InvalidUnicodeCharacter(charClass2) && (!this.endOfFile || parseCurrent + 1 < this.parseEnd))
					{
						this.parseThreshold = 2;
						goto IL_196;
					}
					goto IL_AD;
				}
				default:
					if (c == ' ')
					{
						do
						{
							ch = array[++parseCurrent];
						}
						while (ch == ' ');
						this.PrepareToAddTextRun(num);
						htmlTokenBuilder.AddTextRun(RunTextType.Space, num, parseCurrent);
						goto IL_196;
					}
					break;
				}
				do
				{
					ch = array[++parseCurrent];
				}
				while (ch == '\v' || ch == '\f');
				this.PrepareToAddTextRun(num);
				htmlTokenBuilder.AddTextRun(RunTextType.UnusualWhitespace, num, parseCurrent);
				IL_196:
				charClass = ParseSupport.GetCharClass(ch);
				num = parseCurrent;
				if (!ParseSupport.WhitespaceCharacter(charClass) || !htmlTokenBuilder.PrepareToAddMoreRuns(2, parseCurrent, HtmlRunKind.Text) || this.parseThreshold != 1)
				{
					break;
				}
				continue;
				IL_AD:
				ch = array[++parseCurrent];
				if (this.newLineState == EnrichedParser.NewLineState.None && this.nofill == 0)
				{
					this.newLineState = EnrichedParser.NewLineState.OneNewLine;
					htmlTokenBuilder.AddInvalidRun(parseCurrent, HtmlRunKind.Text);
					goto IL_196;
				}
				if (this.newLineState == EnrichedParser.NewLineState.EatTwoNewLines)
				{
					htmlTokenBuilder.AddInvalidRun(parseCurrent, HtmlRunKind.Text);
					this.newLineState = EnrichedParser.NewLineState.EatOneNewLine;
					goto IL_196;
				}
				if (this.newLineState == EnrichedParser.NewLineState.EatOneNewLine)
				{
					htmlTokenBuilder.AddInvalidRun(parseCurrent, HtmlRunKind.Text);
					this.newLineState = EnrichedParser.NewLineState.ManyNewLines;
					goto IL_196;
				}
				htmlTokenBuilder.AddTextRun(RunTextType.NewLine, num, parseCurrent);
				this.newLineState = EnrichedParser.NewLineState.ManyNewLines;
				goto IL_196;
			}
		}

		private void PrepareToAddTextRun(int runStart)
		{
			if (this.newLineState != EnrichedParser.NewLineState.None)
			{
				this.FlushNewLineState(runStart);
			}
		}

		private void FlushNewLineState(int runStart)
		{
			if (this.newLineState == EnrichedParser.NewLineState.OneNewLine)
			{
				this.tokenBuilder.AddLiteralRun(RunTextType.Space, HtmlRunKind.Text, runStart, runStart, 32);
			}
			this.newLineState = EnrichedParser.NewLineState.None;
		}

		private const int MaxValidTagLength = 17;

		private ConverterInput input;

		private bool endOfFile;

		private EnrichedParser.ParseState parseState;

		private char[] parseBuffer;

		private int parseStart;

		private int parseCurrent;

		private int parseEnd;

		private int parseThreshold = 1;

		private int nameLength;

		private HtmlTokenBuilder tokenBuilder;

		private HtmlToken token;

		private EnrichedParser.NewLineState newLineState;

		private int nofill;

		private enum NewLineState
		{
			None,
			OneNewLine,
			ManyNewLines,
			EatOneNewLine,
			EatTwoNewLines
		}

		protected enum ParseState : byte
		{
			Text,
			Tag,
			LongTag
		}
	}
}
