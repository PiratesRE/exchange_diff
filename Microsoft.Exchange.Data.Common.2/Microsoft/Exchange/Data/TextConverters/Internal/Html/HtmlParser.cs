using System;
using System.Text;
using Microsoft.Exchange.Data.Globalization;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Html
{
	internal class HtmlParser : IHtmlParser, IRestartable, IReusable, IDisposable
	{
		public HtmlParser(ConverterInput input, bool detectEncodingFromMetaTag, bool preformatedText, int maxRuns, int maxAttrs, bool testBoundaryConditions)
		{
			this.input = input;
			this.detectEncodingFromMetaTag = detectEncodingFromMetaTag;
			input.SetRestartConsumer(this);
			this.tokenBuilder = new HtmlTokenBuilder(null, maxRuns, maxAttrs, testBoundaryConditions);
			this.token = this.tokenBuilder.Token;
			this.plaintext = preformatedText;
			this.literalEntities = preformatedText;
			this.parseDocumentOffset = 0;
		}

		public HtmlToken Token
		{
			get
			{
				return this.token;
			}
		}

		public int CurrentOffset
		{
			get
			{
				return this.parseDocumentOffset;
			}
		}

		public void SetRestartConsumer(IRestartable restartConsumer)
		{
			this.restartConsumer = restartConsumer;
		}

		private void ReportProcessed(int processedSize)
		{
			this.input.ReportProcessed(processedSize);
			this.parseDocumentOffset += processedSize;
		}

		private static void ProcessNumericEntityValue(int entityValue, out int literal)
		{
			if (entityValue < 65536)
			{
				if (128 <= entityValue && entityValue <= 159)
				{
					literal = ParseSupport.Latin1MappingInUnicodeControlArea(entityValue);
					return;
				}
				if (ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass((char)entityValue)))
				{
					literal = 63;
					return;
				}
				literal = entityValue;
				return;
			}
			else
			{
				if (entityValue < 1114112)
				{
					literal = entityValue;
					return;
				}
				literal = 63;
				return;
			}
		}

		private static bool FindEntityByHashName(short hash, char[] buffer, int nameOffset, int nameLength, out int entityValue)
		{
			entityValue = 0;
			bool result = false;
			HtmlEntityIndex htmlEntityIndex = HtmlNameData.entityHashTable[(int)hash];
			if (htmlEntityIndex > (HtmlEntityIndex)0)
			{
				for (;;)
				{
					if (HtmlNameData.entities[(int)htmlEntityIndex].Name.Length == nameLength)
					{
						int num = 0;
						while (num < nameLength && HtmlNameData.entities[(int)htmlEntityIndex].Name[num] == buffer[nameOffset + num])
						{
							num++;
						}
						if (num == nameLength)
						{
							break;
						}
					}
					htmlEntityIndex += 1;
					if (HtmlNameData.entities[(int)htmlEntityIndex].Hash != hash)
					{
						return result;
					}
				}
				entityValue = (int)HtmlNameData.entities[(int)htmlEntityIndex].Value;
				result = true;
			}
			return result;
		}

		private void Reinitialize()
		{
			this.endOfFile = false;
			this.literalTags = false;
			this.literalTagNameId = HtmlNameIndex._NOTANAME;
			this.literalEntities = false;
			this.plaintext = false;
			this.parseState = HtmlParser.ParseState.Text;
			this.parseBuffer = null;
			this.parseStart = 0;
			this.parseCurrent = 0;
			this.parseEnd = 0;
			this.parseThreshold = 1;
			this.parseDocumentOffset = 0;
			this.slowParse = true;
			this.scanQuote = '\0';
			this.valueQuote = '\0';
			this.lastCharClass = CharClass.Invalid;
			this.nameLength = 0;
			this.tokenBuilder.Reset();
			this.tokenBuilder.MakeEmptyToken(HtmlTokenId.Restart);
		}

		public bool ParsingFragment
		{
			get
			{
				return this.savedState != null && this.savedState.StateSaved;
			}
		}

		public void PushFragment(ConverterInput fragmentInput, bool literalTextInput)
		{
			if (this.savedState == null)
			{
				this.savedState = new HtmlParser.SavedParserState();
			}
			this.savedState.PushState(this, fragmentInput, literalTextInput);
		}

		public void PopFragment()
		{
			this.savedState.PopState(this);
		}

		bool IRestartable.CanRestart()
		{
			return this.restartConsumer != null && this.restartConsumer.CanRestart();
		}

		void IRestartable.Restart()
		{
			if (this.restartConsumer != null)
			{
				this.restartConsumer.Restart();
			}
			this.Reinitialize();
		}

		void IRestartable.DisableRestart()
		{
			if (this.restartConsumer != null)
			{
				this.restartConsumer.DisableRestart();
				this.restartConsumer = null;
			}
		}

		void IReusable.Initialize(object newSourceOrDestination)
		{
			((IReusable)this.input).Initialize(newSourceOrDestination);
			this.Reinitialize();
			this.input.SetRestartConsumer(this);
		}

		public void Initialize(string fragment, bool preformatedText)
		{
			(this.input as ConverterBufferInput).Initialize(fragment);
			this.Reinitialize();
			this.plaintext = preformatedText;
			this.literalEntities = preformatedText;
		}

		void IDisposable.Dispose()
		{
			if (this.input != null)
			{
				((IDisposable)this.input).Dispose();
			}
			this.input = null;
			this.restartConsumer = null;
			this.parseBuffer = null;
			this.token = null;
			this.tokenBuilder = null;
			this.hashValuesTable = null;
			GC.SuppressFinalize(this);
		}

		public HtmlTokenId Parse()
		{
			if (this.slowParse)
			{
				return this.ParseSlow();
			}
			if (this.tokenBuilder.Valid)
			{
				this.ReportProcessed(this.parseCurrent - this.parseStart);
				this.parseStart = this.parseCurrent;
				this.tokenBuilder.Reset();
			}
			char[] array = this.parseBuffer;
			int num = this.parseCurrent;
			int num2 = num;
			bool flag = false;
			char c = array[++num];
			if (c == '/')
			{
				flag = true;
				c = array[++num];
			}
			CharClass charClass = ParseSupport.GetCharClass(c);
			if (ParseSupport.AlphaCharacter(charClass))
			{
				this.tokenBuilder.StartTag(HtmlNameIndex.Unknown, num2);
				if (flag)
				{
					this.tokenBuilder.SetEndTag();
				}
				this.tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, num2, num);
				this.tokenBuilder.StartTagName();
				int num3 = 0;
				num2 = num;
				this.parseState = HtmlParser.ParseState.TagNamePrefix;
				do
				{
					c = array[++num];
					charClass = ParseSupport.GetCharClass(c);
				}
				while (ParseSupport.HtmlSimpleTagNameCharacter(charClass));
				if (c == ':')
				{
					this.tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, num2, num);
					this.tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.NamePrefixDelimiter, num, num + 1);
					this.tokenBuilder.EndTagNamePrefix();
					num3 = num + 1 - num2;
					num2 = num + 1;
					do
					{
						c = array[++num];
						charClass = ParseSupport.GetCharClass(c);
					}
					while (ParseSupport.HtmlSimpleTagNameCharacter(charClass));
					this.parseState = HtmlParser.ParseState.TagName;
				}
				if (num != num2)
				{
					this.tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, num2, num);
					num3 += num - num2;
				}
				if (ParseSupport.HtmlEndTagNameCharacter(charClass))
				{
					this.tokenBuilder.EndTagName(num3);
					for (;;)
					{
						IL_195:
						if (ParseSupport.WhitespaceCharacter(charClass))
						{
							num2 = num;
							do
							{
								c = array[++num];
								charClass = ParseSupport.GetCharClass(c);
							}
							while (ParseSupport.WhitespaceCharacter(charClass));
							this.tokenBuilder.AddRun(RunTextType.Space, HtmlRunKind.TagWhitespace, num2, num);
						}
						while (c != '>' && (c != '/' || array[num + 1] != '>'))
						{
							this.parseState = HtmlParser.ParseState.TagWsp;
							if (!ParseSupport.HtmlSimpleAttrNameCharacter(charClass) || !this.tokenBuilder.CanAddAttribute() || !this.tokenBuilder.PrepareToAddMoreRuns(11))
							{
								goto IL_5FD;
							}
							this.tokenBuilder.StartAttribute();
							num3 = 0;
							num2 = num;
							this.parseState = HtmlParser.ParseState.AttrNamePrefix;
							do
							{
								c = array[++num];
								charClass = ParseSupport.GetCharClass(c);
							}
							while (ParseSupport.HtmlSimpleAttrNameCharacter(charClass));
							if (c == ':')
							{
								this.tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, num2, num);
								this.tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.NamePrefixDelimiter, num, num + 1);
								this.tokenBuilder.EndAttributeNamePrefix();
								num3 = num + 1 - num2;
								num2 = num + 1;
								do
								{
									c = array[++num];
									charClass = ParseSupport.GetCharClass(c);
								}
								while (ParseSupport.HtmlSimpleAttrNameCharacter(charClass));
								this.parseState = HtmlParser.ParseState.AttrName;
							}
							if (num != num2)
							{
								this.tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, num2, num);
								num3 += num - num2;
							}
							if (!ParseSupport.HtmlEndAttrNameCharacter(charClass))
							{
								goto IL_5FD;
							}
							this.tokenBuilder.EndAttributeName(num3);
							if (ParseSupport.WhitespaceCharacter(charClass))
							{
								num2 = num;
								do
								{
									c = array[++num];
									charClass = ParseSupport.GetCharClass(c);
								}
								while (ParseSupport.WhitespaceCharacter(charClass));
								this.tokenBuilder.AddRun(RunTextType.Space, HtmlRunKind.TagWhitespace, num2, num);
								this.parseState = HtmlParser.ParseState.AttrWsp;
								if (ParseSupport.InvalidUnicodeCharacter(charClass))
								{
									goto IL_5FD;
								}
							}
							if (c != '=')
							{
								this.tokenBuilder.EndAttribute();
							}
							else
							{
								this.tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrEqual, num, num + 1);
								c = array[++num];
								charClass = ParseSupport.GetCharClass(c);
								if (ParseSupport.WhitespaceCharacter(charClass))
								{
									num2 = num;
									do
									{
										c = array[++num];
										charClass = ParseSupport.GetCharClass(c);
									}
									while (ParseSupport.WhitespaceCharacter(charClass));
									this.tokenBuilder.AddRun(RunTextType.Space, HtmlRunKind.TagWhitespace, num2, num);
									this.parseState = HtmlParser.ParseState.AttrValueWsp;
									if (ParseSupport.InvalidUnicodeCharacter(charClass))
									{
										goto IL_5FD;
									}
								}
								if (ParseSupport.QuoteCharacter(charClass))
								{
									this.valueQuote = c;
									this.tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrQuote, num, num + 1);
									this.tokenBuilder.StartValue();
									this.tokenBuilder.SetValueQuote(this.valueQuote);
									if ((charClass & CharClass.GraveAccent) == CharClass.GraveAccent)
									{
										this.tokenBuilder.SetBackquote();
									}
									if ((charClass & CharClass.Backslash) == CharClass.Backslash)
									{
										this.tokenBuilder.SetBackslash();
									}
									c = array[++num];
									charClass = ParseSupport.GetCharClass(c);
									if (ParseSupport.HtmlSimpleAttrQuotedValueCharacter(charClass))
									{
										num2 = num;
										do
										{
											c = array[++num];
											charClass = ParseSupport.GetCharClass(c);
										}
										while (ParseSupport.HtmlSimpleAttrQuotedValueCharacter(charClass));
										this.tokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.AttrValue, num2, num);
									}
									if (c != this.valueQuote)
									{
										goto Block_35;
									}
									this.valueQuote = '\0';
									this.tokenBuilder.EndValue();
									this.tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrQuote, num, num + 1);
									c = array[++num];
									charClass = ParseSupport.GetCharClass(c);
									this.tokenBuilder.EndAttribute();
									goto IL_195;
								}
								else
								{
									if (!ParseSupport.HtmlSimpleAttrUnquotedValueCharacter(charClass))
									{
										goto IL_5F5;
									}
									this.tokenBuilder.StartValue();
									num2 = num;
									do
									{
										c = array[++num];
										charClass = ParseSupport.GetCharClass(c);
									}
									while (ParseSupport.HtmlSimpleAttrUnquotedValueCharacter(charClass));
									this.tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrValue, num2, num);
									this.parseState = HtmlParser.ParseState.AttrValue;
									if (ParseSupport.HtmlEndAttrUnquotedValueCharacter(charClass))
									{
										this.tokenBuilder.EndValue();
										this.tokenBuilder.EndAttribute();
										goto IL_195;
									}
									goto IL_5FD;
								}
							}
						}
						break;
					}
					num2 = num;
					if (c == '/')
					{
						num++;
						this.tokenBuilder.SetEmptyScope();
					}
					num++;
					this.tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagSuffix, num2, num);
					this.tokenBuilder.EndTag(true);
					if (array[num] == '<')
					{
						this.parseState = HtmlParser.ParseState.TagStart;
					}
					else
					{
						this.parseState = HtmlParser.ParseState.Text;
						this.slowParse = true;
					}
					this.parseCurrent = num;
					this.HandleSpecialTag();
					return this.token.HtmlTokenId;
					Block_35:
					this.scanQuote = this.valueQuote;
					this.parseState = HtmlParser.ParseState.AttrValue;
					goto IL_5FD;
					IL_5F5:
					this.parseState = HtmlParser.ParseState.AttrValueWsp;
				}
				IL_5FD:
				this.parseCurrent = num;
				this.lastCharClass = ParseSupport.GetCharClass(array[num - 1]);
				this.nameLength = num3;
			}
			this.slowParse = true;
			return this.ParseSlow();
		}

		public HtmlTokenId ParseSlow()
		{
			if (this.tokenBuilder.Valid)
			{
				if (this.tokenBuilder.IncompleteTag)
				{
					int num = this.tokenBuilder.RewindTag();
					this.ReportProcessed(num - this.parseStart);
					this.parseStart = num;
				}
				else
				{
					this.ReportProcessed(this.parseCurrent - this.parseStart);
					this.parseStart = this.parseCurrent;
					this.tokenBuilder.Reset();
				}
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
							goto Block_7;
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
				char ch = this.parseBuffer[this.parseCurrent];
				CharClass charClass = ParseSupport.GetCharClass(ch);
				if (ParseSupport.InvalidUnicodeCharacter(charClass) || this.parseThreshold > 1)
				{
					bool flag2 = this.SkipInvalidCharacters(ref ch, ref charClass, ref this.parseCurrent);
					if (this.token.IsEmpty)
					{
						this.ReportProcessed(this.parseCurrent - this.parseStart);
						this.parseStart = this.parseCurrent;
						if (this.tokenBuilder.IncompleteTag)
						{
							this.tokenBuilder.BufferChanged(this.parseBuffer, this.parseStart);
						}
					}
					if (!flag2)
					{
						if (!flag)
						{
							continue;
						}
						if (this.parseCurrent == this.parseEnd && !this.tokenBuilder.IsStarted && this.endOfFile)
						{
							goto IL_212;
						}
					}
					this.parseThreshold = 1;
				}
				if (this.ParseStateMachine(ch, charClass, flag))
				{
					goto Block_17;
				}
			}
			return HtmlTokenId.None;
			Block_7:
			converterDecodingInput.EncodingChanged = false;
			return this.tokenBuilder.MakeEmptyToken(HtmlTokenId.EncodingChange, converterDecodingInput.Encoding);
			Block_17:
			return this.token.HtmlTokenId;
			IL_212:
			return this.tokenBuilder.MakeEmptyToken(HtmlTokenId.EndOfFile);
		}

		public bool ParseStateMachine(char ch, CharClass charClass, bool forceFlushToken)
		{
			HtmlTokenBuilder htmlTokenBuilder = this.tokenBuilder;
			char[] array = this.parseBuffer;
			int num = this.parseCurrent;
			int num2 = this.parseEnd;
			int num3 = num;
			switch (this.parseState)
			{
			case HtmlParser.ParseState.Text:
				if (ch == '<' && !this.plaintext)
				{
					this.parseState = HtmlParser.ParseState.TagStart;
					goto IL_E4;
				}
				break;
			case HtmlParser.ParseState.TagStart:
				goto IL_E4;
			case HtmlParser.ParseState.TagNamePrefix:
				goto IL_28A;
			case HtmlParser.ParseState.TagName:
				goto IL_358;
			case HtmlParser.ParseState.TagWsp:
				goto IL_431;
			case HtmlParser.ParseState.AttrNameStart:
				goto IL_4A5;
			case HtmlParser.ParseState.AttrNamePrefix:
				goto IL_4D7;
			case HtmlParser.ParseState.AttrName:
				goto IL_588;
			case HtmlParser.ParseState.AttrWsp:
				IL_60A:
				if (!htmlTokenBuilder.PrepareToAddMoreRuns(2, num3, HtmlRunKind.TagWhitespace))
				{
					goto IL_A53;
				}
				ch = this.ScanWhitespace(ch, ref charClass, ref num);
				if (num != num3)
				{
					htmlTokenBuilder.AddRun(RunTextType.Space, HtmlRunKind.TagWhitespace, num3, num);
				}
				if (ParseSupport.InvalidUnicodeCharacter(charClass))
				{
					goto IL_9F3;
				}
				num3 = num;
				if (ch == '=')
				{
					goto IL_68E;
				}
				htmlTokenBuilder.EndAttribute();
				if (ch == '>')
				{
					this.parseState = HtmlParser.ParseState.TagEnd;
					goto IL_8EB;
				}
				if (ch == '/')
				{
					this.parseState = HtmlParser.ParseState.EmptyTagEnd;
					goto IL_85B;
				}
				this.parseState = HtmlParser.ParseState.AttrNameStart;
				goto IL_4A5;
			case HtmlParser.ParseState.AttrValueWsp:
				IL_6C8:
				if (!htmlTokenBuilder.PrepareToAddMoreRuns(3, num3, HtmlRunKind.TagWhitespace))
				{
					goto IL_A53;
				}
				ch = this.ScanWhitespace(ch, ref charClass, ref num);
				if (num != num3)
				{
					htmlTokenBuilder.AddRun(RunTextType.Space, HtmlRunKind.TagWhitespace, num3, num);
				}
				if (!ParseSupport.InvalidUnicodeCharacter(charClass))
				{
					num3 = num;
					if (ParseSupport.QuoteCharacter(charClass))
					{
						if (ch == this.scanQuote)
						{
							this.scanQuote = '\0';
						}
						else if (this.scanQuote == '\0')
						{
							this.scanQuote = ch;
						}
						this.valueQuote = ch;
						this.lastCharClass = charClass;
						ch = array[++num];
						charClass = ParseSupport.GetCharClass(ch);
						htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrQuote, num3, num);
						num3 = num;
					}
					htmlTokenBuilder.StartValue();
					if (this.valueQuote != '\0')
					{
						htmlTokenBuilder.SetValueQuote(this.valueQuote);
					}
					this.parseState = HtmlParser.ParseState.AttrValue;
					goto IL_795;
				}
				goto IL_9F3;
			case HtmlParser.ParseState.AttrValue:
				goto IL_795;
			case HtmlParser.ParseState.EmptyTagEnd:
				goto IL_85B;
			case HtmlParser.ParseState.TagEnd:
				goto IL_8EB;
			case HtmlParser.ParseState.TagSkip:
				goto IL_979;
			case HtmlParser.ParseState.CommentStart:
				goto IL_ABE;
			case HtmlParser.ParseState.Comment:
			case HtmlParser.ParseState.Conditional:
			case HtmlParser.ParseState.CommentConditional:
			case HtmlParser.ParseState.Bang:
			case HtmlParser.ParseState.Dtd:
			case HtmlParser.ParseState.Asp:
				goto IL_CC6;
			default:
				this.parseCurrent = num;
				throw new TextConvertersException("internal error: invalid parse state");
			}
			IL_A0:
			htmlTokenBuilder.StartText(num3);
			this.ParseText(ch, charClass, ref num);
			if (this.token.IsEmpty && !forceFlushToken)
			{
				htmlTokenBuilder.Reset();
				this.slowParse = true;
				goto IL_E80;
			}
			htmlTokenBuilder.EndText();
			this.parseCurrent = num;
			return true;
			IL_E4:
			char c = array[num + 1];
			CharClass charClass2 = ParseSupport.GetCharClass(c);
			bool flag = false;
			if (c == '/')
			{
				c = array[num + 2];
				charClass2 = ParseSupport.GetCharClass(c);
				if (ParseSupport.InvalidUnicodeCharacter(charClass2) && (!this.endOfFile || num + 2 < num2))
				{
					this.parseThreshold = 3;
					goto IL_E80;
				}
				num++;
				flag = true;
			}
			else if (!ParseSupport.AlphaCharacter(charClass2) || this.literalTags)
			{
				if (c == '!')
				{
					this.parseState = HtmlParser.ParseState.CommentStart;
					goto IL_ABE;
				}
				if (c == '?' && !this.literalTags)
				{
					num += 2;
					htmlTokenBuilder.StartTag(HtmlNameIndex._DTD, num3);
					htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, num3, num);
					htmlTokenBuilder.StartTagText();
					this.lastCharClass = charClass2;
					ch = array[num];
					charClass = ParseSupport.GetCharClass(ch);
					num3 = num;
					this.parseState = HtmlParser.ParseState.Dtd;
					goto IL_CC6;
				}
				if (c == '%')
				{
					num += 2;
					htmlTokenBuilder.StartTag(HtmlNameIndex._ASP, num3);
					htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, num3, num);
					htmlTokenBuilder.StartTagText();
					ch = array[num];
					charClass = ParseSupport.GetCharClass(ch);
					num3 = num;
					this.parseState = HtmlParser.ParseState.Asp;
					goto IL_CC6;
				}
				if (ParseSupport.InvalidUnicodeCharacter(charClass2) && (!this.endOfFile || num + 1 < num2))
				{
					this.parseThreshold = 2;
					goto IL_E80;
				}
				this.parseState = HtmlParser.ParseState.Text;
				goto IL_A0;
			}
			num++;
			this.lastCharClass = charClass;
			ch = c;
			charClass = charClass2;
			htmlTokenBuilder.StartTag(HtmlNameIndex.Unknown, num3);
			if (flag)
			{
				htmlTokenBuilder.SetEndTag();
			}
			htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, num3, num);
			this.nameLength = 0;
			htmlTokenBuilder.StartTagName();
			num3 = num;
			this.parseState = HtmlParser.ParseState.TagNamePrefix;
			IL_28A:
			if (!htmlTokenBuilder.PrepareToAddMoreRuns(2, num3, HtmlRunKind.Name))
			{
				goto IL_A53;
			}
			ch = this.ScanTagName(ch, ref charClass, ref num, CharClass.HtmlTagNamePrefix);
			if (num != num3)
			{
				this.nameLength += num - num3;
				if (this.literalTags && (this.nameLength > 14 || ch == '<'))
				{
					goto IL_A80;
				}
				htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, num3, num);
			}
			if (ParseSupport.InvalidUnicodeCharacter(charClass))
			{
				goto IL_9F3;
			}
			if (ch != ':')
			{
				goto IL_3D1;
			}
			htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.NamePrefixDelimiter, num, num + 1);
			this.nameLength++;
			this.tokenBuilder.EndTagNamePrefix();
			ch = array[++num];
			charClass = ParseSupport.GetCharClass(ch);
			num3 = num;
			this.parseState = HtmlParser.ParseState.TagName;
			IL_358:
			if (!htmlTokenBuilder.PrepareToAddMoreRuns(1, num3, HtmlRunKind.Name))
			{
				goto IL_A53;
			}
			ch = this.ScanTagName(ch, ref charClass, ref num, CharClass.HtmlTagName);
			if (num != num3)
			{
				this.nameLength += num - num3;
				if (this.literalTags && (this.nameLength > 14 || ch == '<'))
				{
					goto IL_A80;
				}
				htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, num3, num);
			}
			if (ParseSupport.InvalidUnicodeCharacter(charClass))
			{
				goto IL_9F3;
			}
			IL_3D1:
			htmlTokenBuilder.EndTagName(this.nameLength);
			if (this.literalTags && this.token.NameIndex != this.literalTagNameId)
			{
				goto IL_A80;
			}
			num3 = num;
			if (ch == '>')
			{
				this.parseState = HtmlParser.ParseState.TagEnd;
				goto IL_8EB;
			}
			if (ch == '/')
			{
				this.parseState = HtmlParser.ParseState.EmptyTagEnd;
				goto IL_85B;
			}
			this.lastCharClass = charClass;
			this.parseState = HtmlParser.ParseState.TagWsp;
			IL_431:
			if (!htmlTokenBuilder.PrepareToAddMoreRuns(2, num3, HtmlRunKind.TagWhitespace))
			{
				goto IL_A53;
			}
			ch = this.ScanWhitespace(ch, ref charClass, ref num);
			if (num != num3)
			{
				htmlTokenBuilder.AddRun(RunTextType.Space, HtmlRunKind.TagWhitespace, num3, num);
			}
			if (ParseSupport.InvalidUnicodeCharacter(charClass))
			{
				goto IL_9F3;
			}
			num3 = num;
			if (ch == '>')
			{
				this.parseState = HtmlParser.ParseState.TagEnd;
				goto IL_8EB;
			}
			if (ch == '/')
			{
				this.parseState = HtmlParser.ParseState.EmptyTagEnd;
				goto IL_85B;
			}
			this.parseState = HtmlParser.ParseState.AttrNameStart;
			IL_4A5:
			if (!htmlTokenBuilder.CanAddAttribute() || !htmlTokenBuilder.PrepareToAddMoreRuns(3, num3, HtmlRunKind.Name))
			{
				goto IL_A53;
			}
			this.nameLength = 0;
			htmlTokenBuilder.StartAttribute();
			this.parseState = HtmlParser.ParseState.AttrNamePrefix;
			IL_4D7:
			if (!htmlTokenBuilder.PrepareToAddMoreRuns(3, num3, HtmlRunKind.Name))
			{
				goto IL_A53;
			}
			ch = this.ScanAttrName(ch, ref charClass, ref num, CharClass.HtmlAttrNamePrefix);
			if (num != num3)
			{
				this.nameLength += num - num3;
				htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, num3, num);
			}
			if (ParseSupport.InvalidUnicodeCharacter(charClass))
			{
				goto IL_9F3;
			}
			if (ch != ':')
			{
				goto IL_5E4;
			}
			htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.NamePrefixDelimiter, num, num + 1);
			this.nameLength++;
			this.tokenBuilder.EndAttributeNamePrefix();
			ch = array[++num];
			charClass = ParseSupport.GetCharClass(ch);
			num3 = num;
			this.parseState = HtmlParser.ParseState.AttrName;
			IL_588:
			if (!htmlTokenBuilder.PrepareToAddMoreRuns(2, num3, HtmlRunKind.Name))
			{
				goto IL_A53;
			}
			ch = this.ScanAttrName(ch, ref charClass, ref num, CharClass.HtmlAttrName);
			if (num != num3)
			{
				this.nameLength += num - num3;
				htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, num3, num);
			}
			if (ParseSupport.InvalidUnicodeCharacter(charClass))
			{
				goto IL_9F3;
			}
			IL_5E4:
			htmlTokenBuilder.EndAttributeName(this.nameLength);
			num3 = num;
			if (ch != '=')
			{
				this.lastCharClass = charClass;
				this.parseState = HtmlParser.ParseState.AttrWsp;
				goto IL_60A;
			}
			IL_68E:
			this.lastCharClass = charClass;
			ch = array[++num];
			charClass = ParseSupport.GetCharClass(ch);
			htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrEqual, num3, num);
			num3 = num;
			this.parseState = HtmlParser.ParseState.AttrValueWsp;
			goto IL_6C8;
			IL_795:
			if (!htmlTokenBuilder.PrepareToAddMoreRuns(2, num3, HtmlRunKind.AttrValue) || !this.ParseAttributeText(ch, charClass, ref num))
			{
				goto IL_A53;
			}
			ch = array[num];
			charClass = ParseSupport.GetCharClass(ch);
			if (ParseSupport.InvalidUnicodeCharacter(charClass) || this.parseThreshold > 1)
			{
				goto IL_9F3;
			}
			htmlTokenBuilder.EndValue();
			num3 = num;
			if (ch == this.valueQuote)
			{
				this.lastCharClass = charClass;
				ch = array[++num];
				charClass = ParseSupport.GetCharClass(ch);
				htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrQuote, num3, num);
				this.valueQuote = '\0';
				num3 = num;
			}
			htmlTokenBuilder.EndAttribute();
			if (ch == '>')
			{
				this.parseState = HtmlParser.ParseState.TagEnd;
				goto IL_8EB;
			}
			if (ch != '/')
			{
				this.parseState = HtmlParser.ParseState.TagWsp;
				goto IL_431;
			}
			this.parseState = HtmlParser.ParseState.EmptyTagEnd;
			IL_85B:
			if (!htmlTokenBuilder.PrepareToAddMoreRuns(1, num3, HtmlRunKind.TagWhitespace))
			{
				goto IL_A53;
			}
			c = array[num + 1];
			charClass2 = ParseSupport.GetCharClass(c);
			if (c == '>')
			{
				htmlTokenBuilder.SetEmptyScope();
				num++;
				this.lastCharClass = charClass;
				ch = c;
				charClass = charClass2;
				this.parseState = HtmlParser.ParseState.TagEnd;
			}
			else
			{
				if (ParseSupport.InvalidUnicodeCharacter(charClass2) && (!this.endOfFile || num + 1 < num2))
				{
					this.parseThreshold = 2;
					goto IL_9F3;
				}
				this.lastCharClass = charClass;
				num++;
				ch = c;
				charClass = charClass2;
				num3 = num;
				this.parseState = HtmlParser.ParseState.TagWsp;
				goto IL_431;
			}
			IL_8EB:
			if (!htmlTokenBuilder.PrepareToAddMoreRuns(1, num3, HtmlRunKind.TagSuffix))
			{
				goto IL_A53;
			}
			this.lastCharClass = charClass;
			num++;
			htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagSuffix, num3, num);
			if (this.scanQuote == '\0')
			{
				htmlTokenBuilder.EndTag(true);
				if (array[num] == '<')
				{
					this.parseState = HtmlParser.ParseState.TagStart;
					this.slowParse = false;
				}
				else
				{
					this.parseState = HtmlParser.ParseState.Text;
				}
				this.parseCurrent = num;
				this.HandleSpecialTag();
				return true;
			}
			num3 = num;
			ch = array[num];
			charClass = ParseSupport.GetCharClass(ch);
			this.parseState = HtmlParser.ParseState.TagSkip;
			IL_979:
			if (!htmlTokenBuilder.PrepareToAddMoreRuns(1, num3, HtmlRunKind.TagText))
			{
				goto IL_A53;
			}
			ch = this.ScanSkipTag(ch, ref charClass, ref num);
			if (num != num3)
			{
				htmlTokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.TagText, num3, num);
			}
			if (!ParseSupport.InvalidUnicodeCharacter(charClass))
			{
				num++;
				htmlTokenBuilder.EndTag(true);
				if (array[num] == '<')
				{
					this.parseState = HtmlParser.ParseState.TagStart;
					this.slowParse = false;
				}
				else
				{
					this.parseState = HtmlParser.ParseState.Text;
				}
				this.parseCurrent = num;
				this.HandleSpecialTag();
				return true;
			}
			IL_9F3:
			if (!forceFlushToken || num + this.parseThreshold < num2)
			{
				goto IL_E80;
			}
			if (this.endOfFile)
			{
				if (num < num2)
				{
					if (this.ScanForInternalInvalidCharacters(num))
					{
						goto IL_E80;
					}
					num = num2;
				}
				if (!this.token.IsTagBegin)
				{
					htmlTokenBuilder.EndTag(true);
					this.parseCurrent = num;
					this.HandleSpecialTag();
					this.parseState = HtmlParser.ParseState.Text;
					return true;
				}
				goto IL_A80;
			}
			IL_A53:
			if (!this.literalTags || this.token.NameIndex != HtmlNameIndex.Unknown)
			{
				htmlTokenBuilder.EndTag(false);
				this.parseCurrent = num;
				this.HandleSpecialTag();
				return true;
			}
			IL_A80:
			num = this.parseStart;
			this.scanQuote = (this.valueQuote = '\0');
			htmlTokenBuilder.Reset();
			num3 = num;
			ch = array[num];
			charClass = ParseSupport.GetCharClass(ch);
			this.parseState = HtmlParser.ParseState.Text;
			goto IL_A0;
			IL_ABE:
			int num4 = 2;
			c = array[num + num4];
			if (c == '-')
			{
				num4++;
				c = array[num + num4];
				if (c == '-')
				{
					num4++;
					c = array[num + num4];
					if (c == '>')
					{
						num += 5;
						htmlTokenBuilder.StartTag(HtmlNameIndex._COMMENT, num3);
						htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, num3, num - 1);
						htmlTokenBuilder.StartTagText();
						htmlTokenBuilder.EndTagText();
						htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagSuffix, num - 1, num);
						htmlTokenBuilder.EndTag(true);
						this.parseState = HtmlParser.ParseState.Text;
						this.parseCurrent = num;
						return true;
					}
					if (c == '-')
					{
						num4++;
						c = array[num + num4];
						if (c == '>')
						{
							num += 6;
							htmlTokenBuilder.StartTag(HtmlNameIndex._COMMENT, num3);
							htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, num3, num - 2);
							htmlTokenBuilder.StartTagText();
							htmlTokenBuilder.EndTagText();
							htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagSuffix, num - 2, num);
							htmlTokenBuilder.EndTag(true);
							this.parseState = HtmlParser.ParseState.Text;
							this.parseCurrent = num;
							return true;
						}
					}
					charClass2 = ParseSupport.GetCharClass(c);
					if (!ParseSupport.InvalidUnicodeCharacter(charClass2))
					{
						num += 4;
						htmlTokenBuilder.StartTag(HtmlNameIndex._COMMENT, num3);
						htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, num3, num);
						htmlTokenBuilder.StartTagText();
						ch = array[num];
						charClass = ParseSupport.GetCharClass(ch);
						num3 = num;
						this.parseState = HtmlParser.ParseState.Comment;
						goto IL_CC6;
					}
				}
			}
			charClass2 = ParseSupport.GetCharClass(c);
			if (ParseSupport.InvalidUnicodeCharacter(charClass2))
			{
				if (!this.endOfFile || num + num4 < num2)
				{
					this.parseThreshold = num4 + 1;
					goto IL_E80;
				}
				this.parseState = HtmlParser.ParseState.Text;
				goto IL_A0;
			}
			else
			{
				if (this.literalTags)
				{
					this.parseState = HtmlParser.ParseState.Text;
					goto IL_A0;
				}
				num += 2;
				htmlTokenBuilder.StartTag(HtmlNameIndex._BANG, num3);
				htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, num3, num);
				htmlTokenBuilder.StartTagText();
				this.lastCharClass = ParseSupport.GetCharClass('!');
				ch = array[num];
				charClass = ParseSupport.GetCharClass(ch);
				num3 = num;
				this.parseState = HtmlParser.ParseState.Bang;
			}
			IL_CC6:
			if (!htmlTokenBuilder.PrepareToAddMoreRuns(2, num3, HtmlRunKind.TagText))
			{
				goto IL_A53;
			}
			while (!ParseSupport.InvalidUnicodeCharacter(charClass))
			{
				if (ParseSupport.QuoteCharacter(charClass))
				{
					if (ch == this.scanQuote)
					{
						this.scanQuote = '\0';
					}
					else if (this.scanQuote == '\0' && ParseSupport.HtmlScanQuoteSensitiveCharacter(this.lastCharClass))
					{
						this.scanQuote = ch;
					}
				}
				else if (ParseSupport.HtmlSuffixCharacter(charClass))
				{
					int num5;
					int num6;
					bool flag2;
					if (!this.CheckSuffix(num, ch, out num5, out num6, out flag2))
					{
						num += num5;
						this.parseThreshold = num6 + 1;
						break;
					}
					if (!flag2)
					{
						num += num5;
						this.lastCharClass = charClass;
						ch = array[num];
						charClass = ParseSupport.GetCharClass(ch);
						continue;
					}
					this.scanQuote = '\0';
					num += num5;
					if (num != num3)
					{
						htmlTokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.TagText, num3, num);
					}
					htmlTokenBuilder.EndTagText();
					if (num6 != 0)
					{
						num3 = num;
						num += num6;
						htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagSuffix, num3, num);
					}
					htmlTokenBuilder.EndTag(true);
					this.parseState = HtmlParser.ParseState.Text;
					this.parseCurrent = num;
					return true;
				}
				this.lastCharClass = charClass;
				ch = array[++num];
				charClass = ParseSupport.GetCharClass(ch);
			}
			if (num != num3)
			{
				htmlTokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.TagText, num3, num);
				if (!htmlTokenBuilder.PrepareToAddMoreRuns(2))
				{
					goto IL_A53;
				}
			}
			if (forceFlushToken && num + this.parseThreshold > num2)
			{
				if (this.endOfFile && num < num2)
				{
					htmlTokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.TagText, num, num2);
					num = num2;
				}
				htmlTokenBuilder.EndTag(this.endOfFile);
				this.parseCurrent = num;
				return true;
			}
			IL_E80:
			this.parseCurrent = num;
			return false;
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

		private char ScanTagName(char ch, ref CharClass charClass, ref int parseCurrent, CharClass acceptCharClassSet)
		{
			char[] array = this.parseBuffer;
			while (ParseSupport.IsCharClassOneOf(charClass, acceptCharClassSet))
			{
				if (ParseSupport.QuoteCharacter(charClass))
				{
					if (ch == this.scanQuote)
					{
						this.scanQuote = '\0';
					}
					else if (this.scanQuote == '\0' && ParseSupport.HtmlScanQuoteSensitiveCharacter(this.lastCharClass))
					{
						this.scanQuote = ch;
					}
				}
				else if (ch == '<' && this.literalTags)
				{
					break;
				}
				this.lastCharClass = charClass;
				ch = array[++parseCurrent];
				charClass = ParseSupport.GetCharClass(ch);
			}
			return ch;
		}

		private char ScanAttrName(char ch, ref CharClass charClass, ref int parseCurrent, CharClass acceptCharClassSet)
		{
			char[] array = this.parseBuffer;
			while (ParseSupport.IsCharClassOneOf(charClass, acceptCharClassSet))
			{
				if (ParseSupport.QuoteCharacter(charClass))
				{
					if (ch == this.scanQuote)
					{
						this.scanQuote = '\0';
					}
					else if (this.scanQuote == '\0' && ParseSupport.HtmlScanQuoteSensitiveCharacter(this.lastCharClass))
					{
						this.scanQuote = ch;
					}
					if (ch != '`')
					{
						array[parseCurrent] = '?';
					}
				}
				this.lastCharClass = charClass;
				ch = array[++parseCurrent];
				charClass = ParseSupport.GetCharClass(ch);
			}
			return ch;
		}

		private char ScanWhitespace(char ch, ref CharClass charClass, ref int parseCurrent)
		{
			char[] array = this.parseBuffer;
			while (ParseSupport.WhitespaceCharacter(charClass))
			{
				this.lastCharClass = charClass;
				ch = array[++parseCurrent];
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

		private char ScanAttrValue(char ch, ref CharClass charClass, ref int parseCurrent)
		{
			char[] array = this.parseBuffer;
			while (ParseSupport.HtmlAttrValueCharacter(charClass))
			{
				this.lastCharClass = charClass;
				ch = array[++parseCurrent];
				charClass = ParseSupport.GetCharClass(ch);
			}
			return ch;
		}

		private char ScanSkipTag(char ch, ref CharClass charClass, ref int parseCurrent)
		{
			char[] array = this.parseBuffer;
			while (!ParseSupport.InvalidUnicodeCharacter(charClass) && (ch != '>' || this.scanQuote != '\0'))
			{
				if (ParseSupport.QuoteCharacter(charClass))
				{
					if (ch == this.scanQuote)
					{
						this.scanQuote = '\0';
					}
					else if (this.scanQuote == '\0' && ParseSupport.HtmlScanQuoteSensitiveCharacter(this.lastCharClass))
					{
						this.scanQuote = ch;
					}
				}
				this.lastCharClass = charClass;
				ch = array[++parseCurrent];
				charClass = ParseSupport.GetCharClass(ch);
			}
			return ch;
		}

		private bool ScanForInternalInvalidCharacters(int parseCurrent)
		{
			char[] array = this.parseBuffer;
			char ch;
			do
			{
				ch = array[parseCurrent++];
			}
			while (!ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(ch)));
			parseCurrent--;
			return parseCurrent < this.parseEnd;
		}

		private void ParseText(char ch, CharClass charClass, ref int parseCurrent)
		{
			int num = this.parseEnd;
			char[] array = this.parseBuffer;
			HtmlTokenBuilder htmlTokenBuilder = this.tokenBuilder;
			int num2 = parseCurrent;
			int num3 = num2;
			for (;;)
			{
				ch = this.ScanText(ch, ref charClass, ref parseCurrent);
				if (ParseSupport.WhitespaceCharacter(charClass))
				{
					if (parseCurrent != num3)
					{
						htmlTokenBuilder.AddTextRun(RunTextType.NonSpace, num3, parseCurrent);
						num3 = parseCurrent;
					}
					if (ch == ' ')
					{
						char c = array[parseCurrent + 1];
						CharClass charClass2 = ParseSupport.GetCharClass(c);
						if (!ParseSupport.WhitespaceCharacter(charClass2))
						{
							ch = c;
							charClass = charClass2;
							parseCurrent++;
							htmlTokenBuilder.AddTextRun(RunTextType.Space, num3, parseCurrent);
							num3 = parseCurrent;
							goto IL_338;
						}
					}
					this.ParseWhitespace(ch, charClass, ref parseCurrent);
					if (this.parseThreshold > 1)
					{
						break;
					}
					ch = array[parseCurrent];
					charClass = ParseSupport.GetCharClass(ch);
					goto IL_334;
				}
				else if (ch == '<')
				{
					if (!this.plaintext && num2 != parseCurrent)
					{
						goto IL_E4;
					}
					ch = array[++parseCurrent];
					charClass = ParseSupport.GetCharClass(ch);
				}
				else if (ch == '&')
				{
					if (this.literalEntities)
					{
						ch = array[++parseCurrent];
						charClass = ParseSupport.GetCharClass(ch);
					}
					else
					{
						int num4;
						int num5;
						if (!this.DecodeEntity(parseCurrent, false, out num4, out num5))
						{
							goto IL_286;
						}
						if (num5 != 1)
						{
							if (parseCurrent != num3)
							{
								htmlTokenBuilder.AddTextRun(RunTextType.NonSpace, num3, parseCurrent);
							}
							if (num4 <= 65535 && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass((char)num4)))
							{
								char c2 = (char)num4;
								switch (c2)
								{
								case '\t':
									htmlTokenBuilder.AddLiteralTextRun(RunTextType.Tabulation, parseCurrent, parseCurrent + num5, num4);
									goto IL_26C;
								case '\n':
									htmlTokenBuilder.AddLiteralTextRun(RunTextType.NewLine, parseCurrent, parseCurrent + num5, num4);
									goto IL_26C;
								case '\v':
								case '\f':
									break;
								case '\r':
									htmlTokenBuilder.AddLiteralTextRun(RunTextType.NewLine, parseCurrent, parseCurrent + num5, num4);
									goto IL_26C;
								default:
									if (c2 == ' ')
									{
										htmlTokenBuilder.AddLiteralTextRun(RunTextType.Space, parseCurrent, parseCurrent + num5, num4);
										goto IL_26C;
									}
									break;
								}
								htmlTokenBuilder.AddLiteralTextRun(RunTextType.UnusualWhitespace, parseCurrent, parseCurrent + num5, num4);
							}
							else if (num4 == 160)
							{
								htmlTokenBuilder.AddLiteralTextRun(RunTextType.Nbsp, parseCurrent, parseCurrent + num5, num4);
							}
							else
							{
								htmlTokenBuilder.AddLiteralTextRun(RunTextType.NonSpace, parseCurrent, parseCurrent + num5, num4);
							}
							IL_26C:
							parseCurrent += num5;
							ch = array[parseCurrent];
							charClass = ParseSupport.GetCharClass(ch);
							goto IL_334;
						}
						ch = array[++parseCurrent];
						charClass = ParseSupport.GetCharClass(ch);
					}
				}
				else
				{
					if (ParseSupport.NbspCharacter(charClass))
					{
						if (parseCurrent != num3)
						{
							htmlTokenBuilder.AddTextRun(RunTextType.NonSpace, num3, parseCurrent);
						}
						num3 = parseCurrent;
						do
						{
							ch = array[++parseCurrent];
							charClass = ParseSupport.GetCharClass(ch);
						}
						while (ParseSupport.NbspCharacter(charClass));
						htmlTokenBuilder.AddTextRun(RunTextType.Nbsp, num3, parseCurrent);
						goto IL_334;
					}
					if (parseCurrent != num3)
					{
						htmlTokenBuilder.AddTextRun(RunTextType.NonSpace, num3, parseCurrent);
					}
					if (parseCurrent >= num)
					{
						return;
					}
					for (;;)
					{
						ch = array[++parseCurrent];
						charClass = ParseSupport.GetCharClass(ch);
						if (!ParseSupport.InvalidUnicodeCharacter(charClass) || parseCurrent >= num)
						{
							goto IL_334;
						}
					}
				}
				IL_338:
				if (!htmlTokenBuilder.PrepareToAddMoreRuns(3, num3, HtmlRunKind.Text))
				{
					return;
				}
				continue;
				IL_334:
				num3 = parseCurrent;
				goto IL_338;
			}
			return;
			IL_E4:
			if (parseCurrent != num3)
			{
				htmlTokenBuilder.AddTextRun(RunTextType.NonSpace, num3, parseCurrent);
			}
			this.parseState = HtmlParser.ParseState.TagStart;
			this.slowParse = this.literalTags;
			return;
			IL_286:
			if (parseCurrent != num3)
			{
				htmlTokenBuilder.AddTextRun(RunTextType.NonSpace, num3, parseCurrent);
			}
			this.parseThreshold = 10;
		}

		private bool ParseAttributeText(char ch, CharClass charClass, ref int parseCurrent)
		{
			int num = parseCurrent;
			char[] array = this.parseBuffer;
			HtmlTokenBuilder htmlTokenBuilder = this.tokenBuilder;
			for (;;)
			{
				ch = this.ScanAttrValue(ch, ref charClass, ref parseCurrent);
				if (ParseSupport.QuoteCharacter(charClass))
				{
					if (charClass == CharClass.GraveAccent)
					{
						this.tokenBuilder.SetBackquote();
					}
					if (charClass == CharClass.Backslash)
					{
						this.tokenBuilder.SetBackslash();
					}
					if (ch == this.scanQuote)
					{
						this.scanQuote = '\0';
					}
					else if (this.scanQuote == '\0' && ParseSupport.HtmlScanQuoteSensitiveCharacter(this.lastCharClass))
					{
						this.scanQuote = ch;
					}
					this.lastCharClass = charClass;
					if (ch == this.valueQuote)
					{
						goto IL_174;
					}
					parseCurrent++;
				}
				else if (ch == '&')
				{
					this.lastCharClass = charClass;
					int literal;
					int num2;
					if (!this.DecodeEntity(parseCurrent, true, out literal, out num2))
					{
						goto IL_10D;
					}
					if (num2 == 1)
					{
						ch = array[++parseCurrent];
						charClass = ParseSupport.GetCharClass(ch);
						continue;
					}
					if (parseCurrent != num)
					{
						htmlTokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.AttrValue, num, parseCurrent);
					}
					htmlTokenBuilder.AddLiteralRun(RunTextType.Unknown, HtmlRunKind.AttrValue, parseCurrent, parseCurrent + num2, literal);
					parseCurrent += num2;
					if (!htmlTokenBuilder.PrepareToAddMoreRuns(2))
					{
						break;
					}
					num = parseCurrent;
				}
				else if (ch == '>')
				{
					this.lastCharClass = charClass;
					if (this.valueQuote == '\0')
					{
						goto IL_174;
					}
					if (this.scanQuote == '\0')
					{
						goto Block_15;
					}
					parseCurrent++;
				}
				else
				{
					if (!ParseSupport.WhitespaceCharacter(charClass))
					{
						goto IL_174;
					}
					this.lastCharClass = charClass;
					if (this.valueQuote == '\0')
					{
						goto IL_174;
					}
					parseCurrent++;
				}
				ch = array[parseCurrent];
				charClass = ParseSupport.GetCharClass(ch);
			}
			return false;
			IL_10D:
			this.parseThreshold = 10;
			goto IL_174;
			Block_15:
			this.valueQuote = '\0';
			IL_174:
			if (parseCurrent != num)
			{
				htmlTokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.AttrValue, num, parseCurrent);
			}
			return true;
		}

		private void ParseWhitespace(char ch, CharClass charClass, ref int parseCurrent)
		{
			int start = parseCurrent;
			char[] array = this.parseBuffer;
			HtmlTokenBuilder htmlTokenBuilder = this.tokenBuilder;
			do
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
					htmlTokenBuilder.AddTextRun(RunTextType.Tabulation, start, parseCurrent);
					goto IL_12F;
				case '\n':
					ch = array[++parseCurrent];
					htmlTokenBuilder.AddTextRun(RunTextType.NewLine, start, parseCurrent);
					goto IL_12F;
				case '\v':
				case '\f':
					break;
				case '\r':
					if (array[parseCurrent + 1] != '\n')
					{
						CharClass charClass2 = ParseSupport.GetCharClass(array[parseCurrent + 1]);
						if (ParseSupport.InvalidUnicodeCharacter(charClass2) && (!this.endOfFile || parseCurrent + 1 < this.parseEnd))
						{
							this.parseThreshold = 2;
							goto IL_12F;
						}
					}
					else
					{
						parseCurrent++;
					}
					ch = array[++parseCurrent];
					htmlTokenBuilder.AddTextRun(RunTextType.NewLine, start, parseCurrent);
					goto IL_12F;
				default:
					if (c == ' ')
					{
						do
						{
							ch = array[++parseCurrent];
						}
						while (ch == ' ');
						htmlTokenBuilder.AddTextRun(RunTextType.Space, start, parseCurrent);
						goto IL_12F;
					}
					break;
				}
				do
				{
					ch = array[++parseCurrent];
				}
				while (ch == '\f' || ch == '\v');
				htmlTokenBuilder.AddTextRun(RunTextType.UnusualWhitespace, start, parseCurrent);
				IL_12F:
				charClass = ParseSupport.GetCharClass(ch);
				start = parseCurrent;
			}
			while (ParseSupport.WhitespaceCharacter(charClass) && htmlTokenBuilder.PrepareToAddMoreRuns(1) && this.parseThreshold == 1);
		}

		private bool CheckSuffix(int parseCurrent, char ch, out int addToTextCnt, out int tagSuffixCnt, out bool endScan)
		{
			addToTextCnt = 1;
			tagSuffixCnt = 0;
			endScan = false;
			char c;
			switch (this.parseState)
			{
			case HtmlParser.ParseState.Comment:
				break;
			case HtmlParser.ParseState.Conditional:
			case HtmlParser.ParseState.CommentConditional:
				if (ch == '>')
				{
					this.parseState = ((this.parseState == HtmlParser.ParseState.CommentConditional) ? HtmlParser.ParseState.Comment : HtmlParser.ParseState.Bang);
					this.tokenBuilder.AbortConditional(this.parseState == HtmlParser.ParseState.Comment);
					return this.CheckSuffix(parseCurrent, ch, out addToTextCnt, out tagSuffixCnt, out endScan);
				}
				if (ch != '-' || this.parseState != HtmlParser.ParseState.CommentConditional)
				{
					if (ch != ']')
					{
						return true;
					}
					c = this.parseBuffer[parseCurrent + 1];
					if (c == '>')
					{
						addToTextCnt = 0;
						tagSuffixCnt = 2;
						endScan = true;
						return true;
					}
					int num = 1;
					if (c == '-')
					{
						num++;
						c = this.parseBuffer[parseCurrent + 2];
						if (c == '-')
						{
							num++;
							c = this.parseBuffer[parseCurrent + 3];
							if (c == '>')
							{
								addToTextCnt = 0;
								tagSuffixCnt = 4;
								endScan = true;
								return true;
							}
						}
					}
					if (!ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(c)))
					{
						addToTextCnt = num;
						return true;
					}
					addToTextCnt = 0;
					tagSuffixCnt = num;
					return false;
				}
				break;
			case HtmlParser.ParseState.Bang:
			case HtmlParser.ParseState.Dtd:
				if (ch == '>' && this.scanQuote == '\0')
				{
					addToTextCnt = 0;
					tagSuffixCnt = 1;
					endScan = true;
				}
				return true;
			case HtmlParser.ParseState.Asp:
				if (ch != '%')
				{
					return true;
				}
				c = this.parseBuffer[parseCurrent + 1];
				if (c == '>')
				{
					addToTextCnt = 0;
					tagSuffixCnt = 2;
					endScan = true;
					return true;
				}
				if (!ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(c)))
				{
					return true;
				}
				addToTextCnt = 0;
				tagSuffixCnt = 1;
				return false;
			default:
				return true;
			}
			if (ch != '-')
			{
				return true;
			}
			int num2 = parseCurrent;
			do
			{
				c = this.parseBuffer[++num2];
			}
			while (c == '-');
			if (c == '>' && num2 - parseCurrent >= 2)
			{
				if (this.parseState == HtmlParser.ParseState.CommentConditional)
				{
					this.parseState = HtmlParser.ParseState.Comment;
					this.tokenBuilder.AbortConditional(true);
				}
				addToTextCnt = num2 - parseCurrent - 2;
				tagSuffixCnt = 3;
				endScan = true;
				return true;
			}
			if (!ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(c)))
			{
				addToTextCnt = num2 - parseCurrent;
				return true;
			}
			addToTextCnt = ((num2 - parseCurrent > 2) ? (num2 - parseCurrent - 2) : 0);
			tagSuffixCnt = num2 - parseCurrent - addToTextCnt;
			return false;
		}

		private bool DecodeEntity(int parseCurrent, bool inAttribute, out int literal, out int consume)
		{
			char[] array = this.parseBuffer;
			int num = parseCurrent + 1;
			int num2 = num;
			int num3 = 0;
			int num4 = 0;
			char c = array[num2];
			CharClass charClass = ParseSupport.GetCharClass(c);
			if (c == '#')
			{
				c = array[++num2];
				charClass = ParseSupport.GetCharClass(c);
				if (c == 'x' || c == 'X')
				{
					c = array[++num2];
					charClass = ParseSupport.GetCharClass(c);
					while (ParseSupport.HexCharacter(charClass))
					{
						num3++;
						num4 = (num4 << 4) + ParseSupport.CharToHex(c);
						c = array[++num2];
						charClass = ParseSupport.GetCharClass(c);
					}
					if (!ParseSupport.InvalidUnicodeCharacter(charClass) || (this.endOfFile && num2 >= this.parseEnd) || num3 > 6)
					{
						if ((inAttribute || c == ';') && num4 != 0 && num3 <= 6)
						{
							HtmlParser.ProcessNumericEntityValue(num4, out literal);
							consume = num2 - parseCurrent;
							if (c == ';')
							{
								consume++;
							}
							return true;
						}
						literal = 0;
						consume = 1;
						return true;
					}
				}
				else
				{
					while (ParseSupport.NumericCharacter(charClass))
					{
						num3++;
						num4 = num4 * 10 + ParseSupport.CharToDecimal(c);
						num2++;
						c = array[num2];
						charClass = ParseSupport.GetCharClass(c);
					}
					if (!ParseSupport.InvalidUnicodeCharacter(charClass) || (this.endOfFile && num2 >= this.parseEnd) || num3 > 7)
					{
						if (num4 != 0 && num3 <= 7)
						{
							HtmlParser.ProcessNumericEntityValue(num4, out literal);
							consume = num2 - parseCurrent;
							if (c == ';')
							{
								consume++;
							}
							return true;
						}
						literal = 0;
						consume = 1;
						return true;
					}
				}
			}
			else
			{
				short[] array2 = this.hashValuesTable;
				if (array2 == null)
				{
					array2 = (this.hashValuesTable = new short[8]);
				}
				HashCode hashCode = new HashCode(true);
				while (ParseSupport.HtmlEntityCharacter(charClass) && num3 < 8)
				{
					short num5 = (short)((ulong)(hashCode.AdvanceAndFinalizeHash(c) ^ 230) % 705UL);
					array2[num3++] = num5;
					num2++;
					c = array[num2];
					charClass = ParseSupport.GetCharClass(c);
				}
				if (!ParseSupport.InvalidUnicodeCharacter(charClass) || (this.endOfFile && num2 >= this.parseEnd))
				{
					if (num3 > 1)
					{
						int num6;
						if (HtmlParser.FindEntityByHashName(array2[num3 - 1], array, num, num3, out num6) && (c == ';' || num6 <= 255))
						{
							num4 = num6;
						}
						else if (!inAttribute)
						{
							for (int i = num3 - 2; i >= 0; i--)
							{
								if (HtmlParser.FindEntityByHashName(array2[i], array, num, i + 1, out num6) && num6 <= 255)
								{
									num4 = num6;
									num3 = i + 1;
									break;
								}
							}
						}
						if (num4 != 0)
						{
							literal = num4;
							consume = num3 + 1;
							if (array[num + num3] == ';')
							{
								consume++;
							}
							return true;
						}
					}
					literal = 0;
					consume = 1;
					return true;
				}
			}
			literal = 0;
			consume = 0;
			return false;
		}

		private void HandleSpecialTag()
		{
			if (HtmlNameData.Names[(int)this.token.NameIndex].LiteralTag)
			{
				this.literalTags = !this.token.IsEndTag;
				this.literalTagNameId = (this.literalTags ? this.token.NameIndex : HtmlNameIndex.Unknown);
				if (HtmlNameData.Names[(int)this.token.NameIndex].LiteralEnt)
				{
					this.literalEntities = (!this.token.IsEndTag && !this.token.IsEmptyScope);
				}
				this.slowParse = (this.slowParse || this.literalTags);
			}
			HtmlNameIndex nameIndex = this.token.NameIndex;
			if (nameIndex != HtmlNameIndex.Meta)
			{
				if (nameIndex != HtmlNameIndex.PlainText)
				{
					return;
				}
				if (!this.token.IsEndTag)
				{
					this.plaintext = true;
					this.literalEntities = true;
					if (this.token.IsTagEnd)
					{
						this.parseState = HtmlParser.ParseState.Text;
					}
				}
			}
			else if (this.input is ConverterDecodingInput && this.detectEncodingFromMetaTag && ((IRestartable)this).CanRestart())
			{
				if (this.token.IsTagBegin)
				{
					this.rightMeta = false;
					this.newEncoding = null;
				}
				this.token.Attributes.Rewind();
				int num = -1;
				bool lookForWordCharset = false;
				foreach (HtmlAttribute htmlAttribute in this.token.Attributes)
				{
					if (htmlAttribute.NameIndex == HtmlNameIndex.HttpEquiv)
					{
						if (!htmlAttribute.Value.CaseInsensitiveCompareEqual("content-type") && !htmlAttribute.Value.CaseInsensitiveCompareEqual("charset"))
						{
							break;
						}
						this.rightMeta = true;
						if (num != -1)
						{
							break;
						}
					}
					else if (htmlAttribute.NameIndex == HtmlNameIndex.Content)
					{
						num = htmlAttribute.Index;
						lookForWordCharset = true;
						if (this.rightMeta)
						{
							break;
						}
					}
					else if (htmlAttribute.NameIndex == HtmlNameIndex.Charset)
					{
						num = htmlAttribute.Index;
						lookForWordCharset = false;
						this.rightMeta = true;
						break;
					}
				}
				if (num != -1)
				{
					string @string = this.token.Attributes[num].Value.GetString(100);
					string text = HtmlReader.CharsetFromString(@string, lookForWordCharset);
					if (text != null)
					{
						Charset.TryGetEncoding(text, out this.newEncoding);
					}
				}
				if (this.rightMeta && this.newEncoding != null)
				{
					(this.input as ConverterDecodingInput).RestartWithNewEncoding(this.newEncoding);
				}
				this.token.Attributes.Rewind();
				return;
			}
		}

		private const int ParseThresholdMax = 16;

		private ConverterInput input;

		private bool endOfFile;

		private bool literalTags;

		private HtmlNameIndex literalTagNameId;

		private bool literalEntities;

		private bool plaintext;

		private HtmlParser.ParseState parseState;

		private char[] parseBuffer;

		private int parseStart;

		private int parseCurrent;

		private int parseEnd;

		private int parseThreshold = 1;

		private int parseDocumentOffset;

		private bool slowParse = true;

		private char scanQuote;

		private char valueQuote;

		private CharClass lastCharClass;

		private int nameLength;

		private HtmlTokenBuilder tokenBuilder;

		private HtmlToken token;

		private IRestartable restartConsumer;

		private bool detectEncodingFromMetaTag;

		private short[] hashValuesTable;

		private bool rightMeta;

		private Encoding newEncoding;

		private HtmlParser.SavedParserState savedState;

		protected enum ParseState : byte
		{
			Text,
			TagStart,
			TagNamePrefix,
			TagName,
			TagWsp,
			AttrNameStart,
			AttrNamePrefix,
			AttrName,
			AttrWsp,
			AttrValueWsp,
			AttrValue,
			EmptyTagEnd,
			TagEnd,
			TagSkip,
			CommentStart,
			Comment,
			Conditional,
			CommentConditional,
			Bang,
			Dtd,
			Asp
		}

		private class SavedParserState
		{
			public bool StateSaved
			{
				get
				{
					return this.input != null;
				}
			}

			public void PushState(HtmlParser parser, ConverterInput newInput, bool literalTextInput)
			{
				this.input = parser.input;
				this.endOfFile = parser.endOfFile;
				this.parseState = parser.parseState;
				this.slowParse = parser.slowParse;
				this.literalTags = parser.literalTags;
				this.literalTagNameId = parser.literalTagNameId;
				this.literalEntities = parser.literalEntities;
				this.plaintext = parser.plaintext;
				this.parseBuffer = parser.parseBuffer;
				this.parseStart = parser.parseStart;
				this.parseCurrent = parser.parseCurrent;
				this.parseEnd = parser.parseEnd;
				this.parseThreshold = parser.parseThreshold;
				this.parseDocumentOffset = parser.parseDocumentOffset;
				parser.input = newInput;
				parser.endOfFile = false;
				parser.parseState = HtmlParser.ParseState.Text;
				parser.slowParse = true;
				parser.literalTags = literalTextInput;
				parser.literalTagNameId = HtmlNameIndex.PlainText;
				parser.literalEntities = literalTextInput;
				parser.plaintext = literalTextInput;
				parser.parseBuffer = null;
				parser.parseStart = 0;
				parser.parseCurrent = 0;
				parser.parseEnd = 0;
				parser.parseThreshold = 1;
			}

			public void PopState(HtmlParser parser)
			{
				parser.input = this.input;
				parser.endOfFile = this.endOfFile;
				parser.parseState = this.parseState;
				parser.slowParse = this.slowParse;
				parser.literalTags = this.literalTags;
				parser.literalTagNameId = this.literalTagNameId;
				parser.literalEntities = this.literalEntities;
				parser.plaintext = this.plaintext;
				parser.parseBuffer = this.parseBuffer;
				parser.parseStart = this.parseStart;
				parser.parseCurrent = this.parseCurrent;
				parser.parseEnd = this.parseEnd;
				parser.parseThreshold = this.parseThreshold;
				parser.parseDocumentOffset = this.parseDocumentOffset;
				this.input = null;
				this.parseBuffer = null;
			}

			private ConverterInput input;

			private bool endOfFile;

			private HtmlParser.ParseState parseState;

			private bool slowParse;

			private bool literalTags;

			private HtmlNameIndex literalTagNameId;

			private bool literalEntities;

			private bool plaintext;

			private char[] parseBuffer;

			private int parseStart;

			private int parseCurrent;

			private int parseEnd;

			private int parseThreshold;

			private int parseDocumentOffset;
		}
	}
}
