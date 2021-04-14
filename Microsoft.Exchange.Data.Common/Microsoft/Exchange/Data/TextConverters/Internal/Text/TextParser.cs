using System;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Text
{
	internal class TextParser : IDisposable
	{
		public TextParser(ConverterInput input, bool unwrapFlowed, bool unwrapDelSp, int maxRuns, bool testBoundaryConditions)
		{
			this.input = input;
			this.tokenBuilder = new TextTokenBuilder(null, maxRuns, testBoundaryConditions);
			this.token = this.tokenBuilder.Token;
			this.unwrapFlowed = unwrapFlowed;
			this.unwrapDelSpace = unwrapDelSp;
		}

		public TextToken Token
		{
			get
			{
				return this.token;
			}
		}

		public void Initialize(string fragment)
		{
			(this.input as ConverterBufferInput).Initialize(fragment);
			this.endOfFile = false;
			this.parseBuffer = null;
			this.parseStart = 0;
			this.parseCurrent = 0;
			this.parseEnd = 0;
			this.parseThreshold = 1;
			this.tokenBuilder.Reset();
			this.lastSpace = false;
			this.lineCount = 0;
			this.quotingExpected = true;
			this.quotingLevel = 0;
			this.lastLineQuotingLevel = 0;
			this.lastLineFlowed = false;
			this.signaturePossible = true;
		}

		public TextTokenId Parse()
		{
			if (this.tokenBuilder.Valid)
			{
				this.input.ReportProcessed(this.parseCurrent - this.parseStart);
				this.parseStart = this.parseCurrent;
				this.tokenBuilder.Reset();
			}
			ConverterDecodingInput converterDecodingInput;
			for (;;)
			{
				bool flag = false;
				if (this.parseCurrent + this.parseThreshold > this.parseEnd)
				{
					if (!this.endOfFile)
					{
						if (!this.input.ReadMore(ref this.parseBuffer, ref this.parseStart, ref this.parseCurrent, ref this.parseEnd))
						{
							break;
						}
						this.tokenBuilder.BufferChanged(this.parseBuffer, this.parseStart);
						converterDecodingInput = (this.input as ConverterDecodingInput);
						if (converterDecodingInput != null && converterDecodingInput.EncodingChanged)
						{
							goto Block_6;
						}
						if (this.input.EndOfFile)
						{
							this.endOfFile = true;
						}
						if (!this.endOfFile && this.parseEnd - this.parseStart < this.input.MaxTokenSize)
						{
							continue;
						}
					}
					flag = true;
				}
				char c = this.parseBuffer[this.parseCurrent];
				CharClass charClass = ParseSupport.GetCharClass(c);
				if (!ParseSupport.InvalidUnicodeCharacter(charClass))
				{
					if (this.parseThreshold <= 1)
					{
						goto IL_2D2;
					}
				}
				while (ParseSupport.InvalidUnicodeCharacter(charClass) && this.parseCurrent < this.parseEnd)
				{
					c = this.parseBuffer[++this.parseCurrent];
					charClass = ParseSupport.GetCharClass(c);
				}
				if (this.parseThreshold > 1 && this.parseCurrent + 1 < this.parseEnd)
				{
					int num = this.parseCurrent + 1;
					int num2 = this.parseCurrent + 1;
					while (num < this.parseEnd && num2 < this.parseCurrent + this.parseThreshold)
					{
						char c2 = this.parseBuffer[num];
						CharClass charClass2 = ParseSupport.GetCharClass(c2);
						if (!ParseSupport.InvalidUnicodeCharacter(charClass2))
						{
							if (num != num2)
							{
								this.parseBuffer[num2] = c2;
								this.parseBuffer[num] = '\0';
							}
							num2++;
						}
						num++;
					}
					if (num == this.parseEnd && this.parseCurrent + this.parseThreshold > num2)
					{
						Array.Copy(this.parseBuffer, this.parseCurrent, this.parseBuffer, this.parseEnd - (num2 - this.parseCurrent), num2 - this.parseCurrent);
						this.parseCurrent = this.parseEnd - (num2 - this.parseCurrent);
						this.input.ReportProcessed(this.parseCurrent - this.parseStart);
						this.parseStart = this.parseCurrent;
					}
				}
				if (this.parseCurrent + this.parseThreshold > this.parseEnd)
				{
					if (!flag)
					{
						continue;
					}
					if (this.parseCurrent == this.parseEnd && !this.tokenBuilder.IsStarted && this.endOfFile)
					{
						goto IL_589;
					}
				}
				this.parseThreshold = 1;
				IL_2D2:
				int num3 = this.parseCurrent;
				this.tokenBuilder.StartText(num3);
				while (this.tokenBuilder.PrepareToAddMoreRuns(9, num3, RunKind.Text))
				{
					while (ParseSupport.TextUriCharacter(charClass))
					{
						c = this.parseBuffer[++this.parseCurrent];
						charClass = ParseSupport.GetCharClass(c);
					}
					if (ParseSupport.TextNonUriCharacter(charClass))
					{
						if (this.parseCurrent != num3)
						{
							this.AddTextRun(RunTextType.NonSpace, num3, this.parseCurrent);
						}
						num3 = this.parseCurrent;
						do
						{
							c = this.parseBuffer[++this.parseCurrent];
							charClass = ParseSupport.GetCharClass(c);
						}
						while (ParseSupport.NbspCharacter(charClass));
						this.AddTextRun(RunTextType.NonSpace, num3, this.parseCurrent);
					}
					else if (ParseSupport.WhitespaceCharacter(charClass))
					{
						if (this.parseCurrent != num3)
						{
							this.AddTextRun(RunTextType.NonSpace, num3, this.parseCurrent);
						}
						num3 = this.parseCurrent;
						if (c == ' ')
						{
							char c2 = this.parseBuffer[this.parseCurrent + 1];
							CharClass charClass2 = ParseSupport.GetCharClass(c2);
							if (!ParseSupport.WhitespaceCharacter(charClass2))
							{
								c = c2;
								charClass = charClass2;
								this.parseCurrent++;
								this.AddTextRun(RunTextType.Space, num3, this.parseCurrent);
								num3 = this.parseCurrent;
								continue;
							}
						}
						this.ParseWhitespace(c, charClass);
						if (this.parseThreshold > 1)
						{
							break;
						}
						num3 = this.parseCurrent;
						c = this.parseBuffer[this.parseCurrent];
						charClass = ParseSupport.GetCharClass(c);
					}
					else if (ParseSupport.NbspCharacter(charClass))
					{
						if (this.parseCurrent != num3)
						{
							this.AddTextRun(RunTextType.NonSpace, num3, this.parseCurrent);
						}
						num3 = this.parseCurrent;
						do
						{
							c = this.parseBuffer[++this.parseCurrent];
							charClass = ParseSupport.GetCharClass(c);
						}
						while (ParseSupport.NbspCharacter(charClass));
						this.AddTextRun(RunTextType.Nbsp, num3, this.parseCurrent);
					}
					else
					{
						if (this.parseCurrent != num3)
						{
							this.AddTextRun(RunTextType.NonSpace, num3, this.parseCurrent);
						}
						if (this.parseCurrent >= this.parseEnd)
						{
							break;
						}
						do
						{
							c = this.parseBuffer[++this.parseCurrent];
							charClass = ParseSupport.GetCharClass(c);
						}
						while (ParseSupport.InvalidUnicodeCharacter(charClass) && this.parseCurrent < this.parseEnd);
					}
					num3 = this.parseCurrent;
				}
				if (!this.token.IsEmpty)
				{
					goto IL_572;
				}
				this.tokenBuilder.Reset();
				this.input.ReportProcessed(this.parseCurrent - this.parseStart);
				this.parseStart = this.parseCurrent;
			}
			return TextTokenId.None;
			Block_6:
			converterDecodingInput.EncodingChanged = false;
			return this.tokenBuilder.MakeEmptyToken(TextTokenId.EncodingChange, converterDecodingInput.Encoding);
			IL_572:
			this.tokenBuilder.EndText();
			return (TextTokenId)this.token.TokenId;
			IL_589:
			return this.tokenBuilder.MakeEmptyToken(TextTokenId.EndOfFile);
		}

		void IDisposable.Dispose()
		{
			if (this.input != null)
			{
				((IDisposable)this.input).Dispose();
			}
			this.input = null;
			this.parseBuffer = null;
			this.token = null;
			this.tokenBuilder = null;
			GC.SuppressFinalize(this);
		}

		private void ParseWhitespace(char ch, CharClass charClass)
		{
			int num = this.parseCurrent;
			do
			{
				char c = ch;
				switch (c)
				{
				case '\t':
					do
					{
						ch = this.parseBuffer[++this.parseCurrent];
					}
					while (ch == '\t');
					this.AddTextRun(RunTextType.Tabulation, num, this.parseCurrent);
					goto IL_196;
				case '\n':
					ch = this.parseBuffer[++this.parseCurrent];
					this.AddTextRun(RunTextType.NewLine, num, this.parseCurrent);
					goto IL_196;
				case '\v':
				case '\f':
					break;
				case '\r':
					if (this.parseBuffer[this.parseCurrent + 1] != '\n')
					{
						CharClass charClass2 = ParseSupport.GetCharClass(this.parseBuffer[this.parseCurrent + 1]);
						if (ParseSupport.InvalidUnicodeCharacter(charClass2) && (!this.endOfFile || this.parseCurrent + 1 < this.parseEnd))
						{
							this.parseThreshold = 2;
							goto IL_196;
						}
					}
					else
					{
						this.parseCurrent++;
					}
					ch = this.parseBuffer[++this.parseCurrent];
					this.AddTextRun(RunTextType.NewLine, num, this.parseCurrent);
					goto IL_196;
				default:
					if (c == ' ')
					{
						do
						{
							ch = this.parseBuffer[++this.parseCurrent];
						}
						while (ch == ' ');
						this.AddTextRun(RunTextType.Space, num, this.parseCurrent);
						goto IL_196;
					}
					break;
				}
				do
				{
					ch = this.parseBuffer[++this.parseCurrent];
				}
				while (ch == '\v' || ch == '\f');
				this.AddTextRun(RunTextType.UnusualWhitespace, num, this.parseCurrent);
				IL_196:
				charClass = ParseSupport.GetCharClass(ch);
				num = this.parseCurrent;
			}
			while (ParseSupport.WhitespaceCharacter(charClass) && this.tokenBuilder.PrepareToAddMoreRuns(4, num, RunKind.Text) && this.parseThreshold == 1);
		}

		private void AddTextRun(RunTextType textType, int runStart, int runEnd)
		{
			if (!this.unwrapFlowed)
			{
				this.tokenBuilder.AddTextRun(textType, runStart, runEnd);
				return;
			}
			this.AddTextRunUnwrap(textType, runStart, runEnd);
		}

		private void AddTextRunUnwrap(RunTextType textType, int runStart, int runEnd)
		{
			if (textType <= RunTextType.Tabulation)
			{
				if (textType != RunTextType.Space)
				{
					if (textType == RunTextType.NewLine)
					{
						if (!this.lastSpace || (this.signaturePossible && this.lineCount == 3))
						{
							this.lastLineFlowed = false;
							this.tokenBuilder.AddTextRun(textType, runStart, runEnd);
						}
						else
						{
							this.lastLineFlowed = true;
						}
						this.lineCount = 0;
						this.lastSpace = false;
						this.signaturePossible = true;
						this.quotingExpected = true;
						this.lastLineQuotingLevel = this.quotingLevel;
						this.quotingLevel = 0;
						return;
					}
					if (textType != RunTextType.Tabulation)
					{
						return;
					}
					if (this.quotingExpected)
					{
						if (this.lastLineQuotingLevel != this.quotingLevel)
						{
							if (this.lastLineFlowed)
							{
								this.tokenBuilder.AddLiteralTextRun(RunTextType.NewLine, runStart, runStart, 10);
							}
							this.tokenBuilder.AddSpecialRun(TextRunKind.QuotingLevel, runStart, this.quotingLevel);
							this.lastLineQuotingLevel = this.quotingLevel;
						}
						this.quotingExpected = false;
					}
					this.tokenBuilder.AddTextRun(textType, runStart, runEnd);
					this.lineCount += runEnd - runStart;
					this.lastSpace = false;
					this.signaturePossible = false;
					return;
				}
			}
			else if (textType != RunTextType.UnusualWhitespace)
			{
				if (textType != RunTextType.Nbsp && textType != RunTextType.NonSpace)
				{
					return;
				}
				if (this.quotingExpected)
				{
					while (runStart != runEnd && this.parseBuffer[runStart] == '>')
					{
						this.quotingLevel++;
						runStart++;
					}
					this.tokenBuilder.SkipRunIfNecessary(runStart, RunKind.Text);
					if (runStart != runEnd)
					{
						if (this.lastLineQuotingLevel != this.quotingLevel)
						{
							if (this.lastLineFlowed)
							{
								this.tokenBuilder.AddLiteralTextRun(RunTextType.NewLine, runStart, runStart, 10);
							}
							this.tokenBuilder.AddSpecialRun(TextRunKind.QuotingLevel, runStart, this.quotingLevel);
							this.lastLineQuotingLevel = this.quotingLevel;
						}
						this.quotingExpected = false;
					}
				}
				if (runStart == runEnd)
				{
					return;
				}
				this.tokenBuilder.AddTextRun(textType, runStart, runEnd);
				this.lineCount += runEnd - runStart;
				this.lastSpace = false;
				if (this.lineCount > 2 || this.parseBuffer[runStart] != '-' || (runEnd - runStart == 2 && this.parseBuffer[runStart + 1] != '-'))
				{
					this.signaturePossible = false;
					return;
				}
				return;
			}
			if (this.quotingExpected)
			{
				runStart++;
				this.tokenBuilder.SkipRunIfNecessary(runStart, RunKind.Text);
				if (this.lastLineQuotingLevel != this.quotingLevel)
				{
					if (this.lastLineFlowed)
					{
						this.tokenBuilder.AddLiteralTextRun(RunTextType.NewLine, runStart, runStart, 10);
					}
					this.tokenBuilder.AddSpecialRun(TextRunKind.QuotingLevel, runStart, this.quotingLevel);
					this.lastLineQuotingLevel = this.quotingLevel;
				}
				this.quotingExpected = false;
			}
			if (runStart != runEnd)
			{
				this.lineCount += runEnd - runStart;
				this.lastSpace = true;
				this.tokenBuilder.AddTextRun(textType, runStart, runEnd);
				if (this.lineCount != 3 || runEnd - runStart != 1)
				{
					this.signaturePossible = false;
					return;
				}
			}
		}

		protected ConverterInput input;

		protected bool endOfFile;

		protected char[] parseBuffer;

		protected int parseStart;

		protected int parseCurrent;

		protected int parseEnd;

		protected int parseThreshold = 1;

		protected TextTokenBuilder tokenBuilder;

		protected TextToken token;

		protected bool unwrapFlowed;

		protected bool unwrapDelSpace;

		protected bool lastSpace;

		protected int lineCount;

		protected bool quotingExpected = true;

		protected int quotingLevel;

		protected int lastLineQuotingLevel;

		protected bool lastLineFlowed;

		protected bool signaturePossible = true;
	}
}
