using System;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Css
{
	internal class CssParser : IDisposable
	{
		public CssParser(ConverterInput input, int maxRuns, bool testBoundaryConditions)
		{
			this.input = input;
			this.tokenBuilder = new CssTokenBuilder(null, 256, 256, maxRuns, testBoundaryConditions);
			this.token = this.tokenBuilder.Token;
		}

		public CssToken Token
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
		}

		public void Reset()
		{
			this.endOfFile = false;
			this.parseBuffer = null;
			this.parseStart = 0;
			this.parseCurrent = 0;
			this.parseEnd = 0;
			this.ruleDepth = 0;
		}

		public void SetParseMode(CssParseMode parseMode)
		{
			this.parseMode = parseMode;
		}

		public CssTokenId Parse()
		{
			if (this.endOfFile)
			{
				return CssTokenId.EndOfFile;
			}
			this.tokenBuilder.Reset();
			char[] array = this.parseBuffer;
			int num = this.parseCurrent;
			int num2 = this.parseEnd;
			if (num >= num2)
			{
				this.input.ReadMore(ref this.parseBuffer, ref this.parseStart, ref this.parseCurrent, ref this.parseEnd);
				if (this.parseEnd == 0)
				{
					return CssTokenId.EndOfFile;
				}
				this.tokenBuilder.BufferChanged(this.parseBuffer, this.parseStart);
				array = this.parseBuffer;
				num = this.parseCurrent;
				num2 = this.parseEnd;
			}
			char ch = array[num];
			CharClass charClass = ParseSupport.GetCharClass(ch);
			int num3 = num;
			if (this.parseMode == CssParseMode.StyleTag)
			{
				this.ScanStyleSheet(ch, ref charClass, ref num);
				if (num3 >= num)
				{
					this.tokenBuilder.Reset();
					return CssTokenId.EndOfFile;
				}
				if (this.tokenBuilder.Incomplete)
				{
					this.tokenBuilder.EndRuleSet();
				}
			}
			else
			{
				this.ScanDeclarations(ch, ref charClass, ref num);
				if (num < num2)
				{
					this.endOfFile = true;
					this.tokenBuilder.Reset();
					return CssTokenId.EndOfFile;
				}
				if (this.tokenBuilder.Incomplete)
				{
					this.tokenBuilder.EndDeclarations();
				}
			}
			this.endOfFile = (num == num2);
			this.parseCurrent = num;
			return this.token.CssTokenId;
		}

		private static bool IsNameCharacterNoEscape(char ch, CharClass charClass)
		{
			return CssParser.IsNameStartCharacterNoEscape(ch, charClass) || ParseSupport.NumericCharacter(charClass) || ch == '-';
		}

		private static bool IsNameStartCharacterNoEscape(char ch, CharClass charClass)
		{
			return ParseSupport.AlphaCharacter(charClass) || ch == '_' || ch > '\u007f';
		}

		private static bool IsUrlCharacterNoEscape(char ch, CharClass charClass)
		{
			return (ch >= '*' && ch != '\u007f') || (ch >= '#' && ch <= '&') || ch == '!';
		}

		private char ScanStyleSheet(char ch, ref CharClass charClass, ref int parseCurrent)
		{
			int num = this.parseEnd;
			char[] array = this.parseBuffer;
			for (;;)
			{
				int num2 = parseCurrent;
				ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
				if (parseCurrent == num)
				{
					break;
				}
				if (this.IsNameStartCharacter(ch, charClass, parseCurrent) || ch == '*' || ch == '.' || ch == ':' || ch == '#' || ch == '[')
				{
					ch = this.ScanRuleSet(ch, ref charClass, ref parseCurrent);
					if (parseCurrent == num)
					{
						return ch;
					}
					if (!this.isInvalid)
					{
						return ch;
					}
				}
				else if (ch == '@')
				{
					ch = this.ScanAtRule(ch, ref charClass, ref parseCurrent);
					if (parseCurrent == num)
					{
						return ch;
					}
					if (!this.isInvalid)
					{
						return ch;
					}
				}
				else if (ch == '/' && parseCurrent < num && array[parseCurrent + 1] == '*')
				{
					ch = this.ScanComment(ch, ref charClass, ref parseCurrent);
					if (parseCurrent == num)
					{
						return ch;
					}
				}
				else if (ch == '<')
				{
					ch = this.ScanCdo(ch, ref charClass, ref parseCurrent);
					if (parseCurrent == num)
					{
						return ch;
					}
				}
				else if (ch == '-')
				{
					ch = this.ScanCdc(ch, ref charClass, ref parseCurrent);
					if (parseCurrent == num)
					{
						return ch;
					}
				}
				else
				{
					this.isInvalid = true;
				}
				if (this.isInvalid)
				{
					this.isInvalid = false;
					this.tokenBuilder.Reset();
					ch = this.SkipToNextRule(ch, ref charClass, ref parseCurrent);
					if (parseCurrent == num)
					{
						return ch;
					}
				}
				if (num2 >= parseCurrent)
				{
					return ch;
				}
			}
			return ch;
		}

		private char ScanCdo(char ch, ref CharClass charClass, ref int parseCurrent)
		{
			parseCurrent++;
			if (parseCurrent + 3 >= this.parseEnd)
			{
				parseCurrent = this.parseEnd;
				return ch;
			}
			if (this.parseBuffer[parseCurrent++] != '!' || this.parseBuffer[parseCurrent++] != '-' || this.parseBuffer[parseCurrent++] != '-')
			{
				return this.SkipToNextRule(ch, ref charClass, ref parseCurrent);
			}
			ch = this.parseBuffer[parseCurrent];
			charClass = ParseSupport.GetCharClass(ch);
			return ch;
		}

		private char ScanCdc(char ch, ref CharClass charClass, ref int parseCurrent)
		{
			parseCurrent++;
			if (parseCurrent + 2 >= this.parseEnd)
			{
				parseCurrent = this.parseEnd;
				return ch;
			}
			if (this.parseBuffer[parseCurrent++] != '-' || this.parseBuffer[parseCurrent++] != '>')
			{
				return this.SkipToNextRule(ch, ref charClass, ref parseCurrent);
			}
			ch = this.parseBuffer[parseCurrent];
			charClass = ParseSupport.GetCharClass(ch);
			return ch;
		}

		private char ScanAtRule(char ch, ref CharClass charClass, ref int parseCurrent)
		{
			int num = this.parseEnd;
			char[] array = this.parseBuffer;
			int num2 = parseCurrent;
			ch = array[++parseCurrent];
			charClass = ParseSupport.GetCharClass(ch);
			if (!this.IsIdentStartCharacter(ch, charClass, parseCurrent))
			{
				this.isInvalid = true;
				return ch;
			}
			this.tokenBuilder.StartRuleSet(num2, CssTokenId.AtRule);
			if (!this.tokenBuilder.CanAddSelector())
			{
				parseCurrent = num;
				return ch;
			}
			this.tokenBuilder.StartSelectorName();
			this.PrepareAndAddRun(CssRunKind.AtRuleName, num2, ref parseCurrent);
			if (parseCurrent == num)
			{
				return ch;
			}
			int nameLength;
			ch = this.ScanIdent(CssRunKind.AtRuleName, ch, ref charClass, ref parseCurrent, out nameLength);
			this.tokenBuilder.EndSelectorName(nameLength);
			if (parseCurrent == num)
			{
				return ch;
			}
			if (this.IsNameEqual("page", num2 + 1, parseCurrent - num2 - 1))
			{
				ch = this.ScanPageSelector(ch, ref charClass, ref parseCurrent);
				if (parseCurrent == num)
				{
					return ch;
				}
			}
			else if (!this.IsNameEqual("font-face", num2 + 1, parseCurrent - num2 - 1))
			{
				this.isInvalid = true;
				return ch;
			}
			this.tokenBuilder.EndSimpleSelector();
			ch = this.ScanDeclarationBlock(ch, ref charClass, ref parseCurrent);
			if (parseCurrent == num)
			{
				return ch;
			}
			return ch;
		}

		private char ScanPageSelector(char ch, ref CharClass charClass, ref int parseCurrent)
		{
			ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, false);
			if (parseCurrent == this.parseEnd)
			{
				return ch;
			}
			if (this.IsIdentStartCharacter(ch, charClass, parseCurrent))
			{
				this.tokenBuilder.EndSimpleSelector();
				this.tokenBuilder.StartSelectorName();
				int nameLength;
				ch = this.ScanIdent(CssRunKind.PageIdent, ch, ref charClass, ref parseCurrent, out nameLength);
				this.tokenBuilder.EndSelectorName(nameLength);
				if (parseCurrent == this.parseEnd)
				{
					return ch;
				}
				this.tokenBuilder.SetSelectorCombinator(CssSelectorCombinator.Descendant, false);
			}
			if (ch == ':')
			{
				ch = this.parseBuffer[++parseCurrent];
				charClass = ParseSupport.GetCharClass(ch);
				this.PrepareAndAddRun(CssRunKind.PagePseudoStart, parseCurrent - 1, ref parseCurrent);
				if (parseCurrent == this.parseEnd)
				{
					return ch;
				}
				if (!this.IsIdentStartCharacter(ch, charClass, parseCurrent))
				{
					this.tokenBuilder.InvalidateLastValidRun(CssRunKind.SelectorPseudoStart);
					return ch;
				}
				this.tokenBuilder.StartSelectorClass(CssSelectorClassType.Pseudo);
				int num;
				ch = this.ScanIdent(CssRunKind.PagePseudo, ch, ref charClass, ref parseCurrent, out num);
				this.tokenBuilder.EndSelectorClass();
				if (parseCurrent == this.parseEnd)
				{
					return ch;
				}
			}
			ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, false);
			if (parseCurrent == this.parseEnd)
			{
				return ch;
			}
			return ch;
		}

		private char ScanRuleSet(char ch, ref CharClass charClass, ref int parseCurrent)
		{
			this.tokenBuilder.StartRuleSet(parseCurrent, CssTokenId.RuleSet);
			ch = this.ScanSelectors(ch, ref charClass, ref parseCurrent);
			if (parseCurrent == this.parseEnd || this.isInvalid)
			{
				return ch;
			}
			ch = this.ScanDeclarationBlock(ch, ref charClass, ref parseCurrent);
			if (parseCurrent == this.parseEnd)
			{
				return ch;
			}
			return ch;
		}

		private char ScanDeclarationBlock(char ch, ref CharClass charClass, ref int parseCurrent)
		{
			ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, false);
			if (parseCurrent == this.parseEnd)
			{
				return ch;
			}
			if (ch != '{')
			{
				this.isInvalid = true;
				return ch;
			}
			this.ruleDepth++;
			ch = this.parseBuffer[++parseCurrent];
			charClass = ParseSupport.GetCharClass(ch);
			this.PrepareAndAddRun(CssRunKind.Delimiter, parseCurrent - 1, ref parseCurrent);
			if (parseCurrent == this.parseEnd)
			{
				return ch;
			}
			ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
			if (parseCurrent == this.parseEnd)
			{
				return ch;
			}
			ch = this.ScanDeclarations(ch, ref charClass, ref parseCurrent);
			if (parseCurrent == this.parseEnd)
			{
				return ch;
			}
			if (ch != '}')
			{
				this.isInvalid = true;
				return ch;
			}
			this.ruleDepth--;
			ch = this.parseBuffer[++parseCurrent];
			charClass = ParseSupport.GetCharClass(ch);
			this.PrepareAndAddRun(CssRunKind.Delimiter, parseCurrent - 1, ref parseCurrent);
			if (parseCurrent == this.parseEnd)
			{
				return ch;
			}
			return ch;
		}

		private char ScanSelectors(char ch, ref CharClass charClass, ref int parseCurrent)
		{
			int num = this.parseEnd;
			char[] array = this.parseBuffer;
			int i = parseCurrent;
			ch = this.ScanSimpleSelector(ch, ref charClass, ref parseCurrent);
			if (parseCurrent == num || this.isInvalid)
			{
				return ch;
			}
			while (i < parseCurrent)
			{
				CssSelectorCombinator combinator = CssSelectorCombinator.None;
				bool flag = false;
				bool flag2 = false;
				i = parseCurrent;
				ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, false);
				if (parseCurrent == num)
				{
					return ch;
				}
				if (i < parseCurrent)
				{
					flag = true;
					combinator = CssSelectorCombinator.Descendant;
				}
				if (ch == '+' || ch == '>' || ch == ',')
				{
					combinator = ((ch == '+') ? CssSelectorCombinator.Adjacent : ((ch == '>') ? CssSelectorCombinator.Child : CssSelectorCombinator.None));
					ch = array[++parseCurrent];
					charClass = ParseSupport.GetCharClass(ch);
					if (parseCurrent == num)
					{
						return ch;
					}
					this.PrepareAndAddRun(CssRunKind.SelectorCombinatorOrComma, parseCurrent - 1, ref parseCurrent);
					if (parseCurrent == num)
					{
						return ch;
					}
					flag2 = true;
					ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
					if (parseCurrent == num)
					{
						return ch;
					}
				}
				else if (i == parseCurrent)
				{
					break;
				}
				i = parseCurrent;
				ch = this.ScanSimpleSelector(ch, ref charClass, ref parseCurrent);
				if (i == parseCurrent)
				{
					if (flag2)
					{
						this.tokenBuilder.InvalidateLastValidRun(CssRunKind.SelectorCombinatorOrComma);
					}
					if (flag)
					{
						this.tokenBuilder.InvalidateLastValidRun(CssRunKind.Space);
						break;
					}
					break;
				}
				else
				{
					if (this.isInvalid)
					{
						return ch;
					}
					this.tokenBuilder.SetSelectorCombinator(combinator, true);
					if (parseCurrent == num)
					{
						return ch;
					}
				}
			}
			return ch;
		}

		private char ScanSimpleSelector(char ch, ref CharClass charClass, ref int parseCurrent)
		{
			int num = this.parseEnd;
			char[] array = this.parseBuffer;
			if (ch == '.' || ch == ':' || ch == '#' || ch == '[')
			{
				if (!this.tokenBuilder.CanAddSelector())
				{
					parseCurrent = num;
					return ch;
				}
				this.tokenBuilder.BuildUniversalSelector();
			}
			else
			{
				if (!this.IsIdentStartCharacter(ch, charClass, parseCurrent) && ch != '*')
				{
					return ch;
				}
				if (!this.tokenBuilder.CanAddSelector())
				{
					parseCurrent = num;
					return ch;
				}
				this.tokenBuilder.StartSelectorName();
				int nameLength;
				if (ch == '*')
				{
					nameLength = 1;
					ch = array[++parseCurrent];
					charClass = ParseSupport.GetCharClass(ch);
					this.PrepareAndAddRun(CssRunKind.SelectorName, parseCurrent - 1, ref parseCurrent);
				}
				else
				{
					ch = this.ScanIdent(CssRunKind.SelectorName, ch, ref charClass, ref parseCurrent, out nameLength);
				}
				this.tokenBuilder.EndSelectorName(nameLength);
				if (parseCurrent == num)
				{
					return ch;
				}
			}
			ch = this.ScanSelectorSuffix(ch, ref charClass, ref parseCurrent);
			this.tokenBuilder.EndSimpleSelector();
			return ch;
		}

		private char ScanSelectorSuffix(char ch, ref CharClass charClass, ref int parseCurrent)
		{
			if (ch == '[')
			{
				this.tokenBuilder.StartSelectorClass(CssSelectorClassType.Attrib);
				ch = this.ScanSelectorAttrib(ch, ref charClass, ref parseCurrent);
				this.tokenBuilder.EndSelectorClass();
				return ch;
			}
			int num = this.parseEnd;
			char[] array = this.parseBuffer;
			if (ch != ':')
			{
				if (ch == '.' || ch == '#')
				{
					bool flag = ch == '.';
					ch = array[++parseCurrent];
					charClass = ParseSupport.GetCharClass(ch);
					if (this.IsNameCharacter(ch, charClass, parseCurrent) && (!flag || this.IsIdentStartCharacter(ch, charClass, parseCurrent)))
					{
						this.PrepareAndAddRun(flag ? CssRunKind.SelectorClassStart : CssRunKind.SelectorHashStart, parseCurrent - 1, ref parseCurrent);
						if (parseCurrent == num)
						{
							return ch;
						}
						this.tokenBuilder.StartSelectorClass(flag ? CssSelectorClassType.Regular : CssSelectorClassType.Hash);
						ch = this.ScanName(flag ? CssRunKind.SelectorClass : CssRunKind.SelectorHash, ch, ref charClass, ref parseCurrent);
						this.tokenBuilder.EndSelectorClass();
						if (parseCurrent == num)
						{
							return ch;
						}
					}
					else
					{
						this.PrepareAndAddInvalidRun(CssRunKind.FunctionStart, ref parseCurrent);
					}
				}
				return ch;
			}
			ch = array[++parseCurrent];
			charClass = ParseSupport.GetCharClass(ch);
			this.PrepareAndAddRun(CssRunKind.SelectorPseudoStart, parseCurrent - 1, ref parseCurrent);
			if (parseCurrent == num)
			{
				return ch;
			}
			this.tokenBuilder.StartSelectorClass(CssSelectorClassType.Pseudo);
			ch = this.ScanSelectorPseudo(ch, ref charClass, ref parseCurrent);
			this.tokenBuilder.EndSelectorClass();
			return ch;
		}

		private char ScanSelectorPseudo(char ch, ref CharClass charClass, ref int parseCurrent)
		{
			int num = this.parseEnd;
			char[] array = this.parseBuffer;
			if (!this.IsIdentStartCharacter(ch, charClass, parseCurrent))
			{
				this.tokenBuilder.InvalidateLastValidRun(CssRunKind.SelectorPseudoStart);
				return ch;
			}
			int start = parseCurrent;
			int num2;
			ch = this.ScanIdent(CssRunKind.SelectorPseudo, ch, ref charClass, ref parseCurrent, out num2);
			if (parseCurrent == num)
			{
				return ch;
			}
			if (ch == '(')
			{
				if (!this.IsSafeIdentifier(CssParser.SafePseudoFunctions, start, parseCurrent))
				{
					return ch;
				}
				ch = array[++parseCurrent];
				charClass = ParseSupport.GetCharClass(ch);
				if (parseCurrent == num)
				{
					return ch;
				}
				this.PrepareAndAddRun(CssRunKind.FunctionStart, parseCurrent - 1, ref parseCurrent);
				if (parseCurrent == num)
				{
					return ch;
				}
				ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
				if (parseCurrent == num)
				{
					return ch;
				}
				if (!this.IsIdentStartCharacter(ch, charClass, parseCurrent))
				{
					this.isInvalid = true;
					return ch;
				}
				ch = this.ScanIdent(CssRunKind.SelectorPseudoArg, ch, ref charClass, ref parseCurrent, out num2);
				if (parseCurrent == num)
				{
					return ch;
				}
				ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
				if (parseCurrent == num)
				{
					return ch;
				}
				if (ch != ')')
				{
					this.isInvalid = true;
					return ch;
				}
				ch = array[++parseCurrent];
				charClass = ParseSupport.GetCharClass(ch);
				this.PrepareAndAddRun(CssRunKind.FunctionEnd, parseCurrent - 1, ref parseCurrent);
				if (parseCurrent == num)
				{
					return ch;
				}
			}
			return ch;
		}

		private char ScanSelectorAttrib(char ch, ref CharClass charClass, ref int parseCurrent)
		{
			int num = this.parseEnd;
			char[] array = this.parseBuffer;
			ch = array[++parseCurrent];
			charClass = ParseSupport.GetCharClass(ch);
			this.PrepareAndAddRun(CssRunKind.SelectorAttribStart, parseCurrent - 1, ref parseCurrent);
			if (parseCurrent == num)
			{
				return ch;
			}
			ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
			if (parseCurrent == num)
			{
				return ch;
			}
			if (!this.IsIdentStartCharacter(ch, charClass, parseCurrent))
			{
				this.isInvalid = true;
				return ch;
			}
			int num2;
			ch = this.ScanIdent(CssRunKind.SelectorAttribName, ch, ref charClass, ref parseCurrent, out num2);
			if (parseCurrent == num)
			{
				return ch;
			}
			ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
			if (parseCurrent == num)
			{
				return ch;
			}
			int num3 = parseCurrent;
			if (ch == '=')
			{
				ch = array[++parseCurrent];
				charClass = ParseSupport.GetCharClass(ch);
				this.PrepareAndAddRun(CssRunKind.SelectorAttribEquals, parseCurrent - 1, ref parseCurrent);
				if (parseCurrent == num)
				{
					return ch;
				}
			}
			else if ((ch == '~' || ch == '|') && array[parseCurrent + 1] == '=')
			{
				parseCurrent += 2;
				this.PrepareAndAddRun((ch == '~') ? CssRunKind.SelectorAttribIncludes : CssRunKind.SelectorAttribDashmatch, parseCurrent - 2, ref parseCurrent);
				if (parseCurrent == num)
				{
					return ch;
				}
				ch = array[parseCurrent];
				charClass = ParseSupport.GetCharClass(ch);
			}
			if (num3 < parseCurrent)
			{
				ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
				if (parseCurrent == num)
				{
					return ch;
				}
				if (this.IsIdentStartCharacter(ch, charClass, parseCurrent))
				{
					num3 = parseCurrent;
					ch = this.ScanIdent(CssRunKind.SelectorAttribIdentifier, ch, ref charClass, ref parseCurrent, out num2);
					if (parseCurrent == num)
					{
						return ch;
					}
				}
				else if (ch == '"' || ch == '\'')
				{
					num3 = parseCurrent;
					ch = this.ScanString(ch, ref charClass, ref parseCurrent, false);
					if (this.isInvalid)
					{
						return ch;
					}
					this.PrepareAndAddRun(CssRunKind.SelectorAttribString, num3, ref parseCurrent);
					if (parseCurrent == num)
					{
						return ch;
					}
				}
				ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
				if (parseCurrent == num)
				{
					return ch;
				}
			}
			if (ch != ']')
			{
				this.isInvalid = true;
				return ch;
			}
			ch = array[++parseCurrent];
			charClass = ParseSupport.GetCharClass(ch);
			this.PrepareAndAddRun(CssRunKind.SelectorAttribEnd, parseCurrent - 1, ref parseCurrent);
			if (parseCurrent == num)
			{
				return ch;
			}
			return ch;
		}

		private char ScanDeclarations(char ch, ref CharClass charClass, ref int parseCurrent)
		{
			int num = this.parseEnd;
			char[] array = this.parseBuffer;
			this.tokenBuilder.StartDeclarations(parseCurrent);
			for (;;)
			{
				int num2 = parseCurrent;
				ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
				if (parseCurrent == num)
				{
					break;
				}
				if (this.IsIdentStartCharacter(ch, charClass, parseCurrent))
				{
					if (!this.tokenBuilder.CanAddProperty())
					{
						goto Block_3;
					}
					this.tokenBuilder.StartPropertyName();
					int nameLength;
					ch = this.ScanIdent(CssRunKind.PropertyName, ch, ref charClass, ref parseCurrent, out nameLength);
					this.tokenBuilder.EndPropertyName(nameLength);
					if (parseCurrent == num)
					{
						return ch;
					}
					ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
					if (parseCurrent == num)
					{
						return ch;
					}
					if (ch != ':')
					{
						goto Block_6;
					}
					ch = array[++parseCurrent];
					charClass = ParseSupport.GetCharClass(ch);
					this.PrepareAndAddRun(CssRunKind.PropertyColon, parseCurrent - 1, ref parseCurrent);
					if (parseCurrent == num)
					{
						return ch;
					}
					ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
					if (parseCurrent == num)
					{
						return ch;
					}
					this.tokenBuilder.StartPropertyValue();
					ch = this.ScanPropertyValue(ch, ref charClass, ref parseCurrent);
					if (parseCurrent == num)
					{
						return ch;
					}
					this.tokenBuilder.EndPropertyValue();
					this.tokenBuilder.EndProperty();
					ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
					if (parseCurrent == num)
					{
						return ch;
					}
				}
				if (ch != ';')
				{
					goto Block_11;
				}
				ch = array[++parseCurrent];
				charClass = ParseSupport.GetCharClass(ch);
				this.PrepareAndAddRun(CssRunKind.Delimiter, parseCurrent - 1, ref parseCurrent);
				if (parseCurrent == num)
				{
					return ch;
				}
				if (num2 >= parseCurrent)
				{
					return ch;
				}
			}
			return ch;
			Block_3:
			parseCurrent = num;
			return ch;
			Block_6:
			this.tokenBuilder.MarkPropertyAsDeleted();
			return ch;
			Block_11:
			this.tokenBuilder.EndDeclarations();
			return ch;
		}

		private char ScanPropertyValue(char ch, ref CharClass charClass, ref int parseCurrent)
		{
			int num = this.parseEnd;
			char[] array = this.parseBuffer;
			ch = this.ScanExpr(ch, ref charClass, ref parseCurrent, 0);
			if (parseCurrent == num)
			{
				return ch;
			}
			if (ch == '!')
			{
				ch = array[++parseCurrent];
				charClass = ParseSupport.GetCharClass(ch);
				if (parseCurrent == num)
				{
					this.tokenBuilder.MarkPropertyAsDeleted();
					return ch;
				}
				this.PrepareAndAddRun(CssRunKind.ImportantStart, parseCurrent - 1, ref parseCurrent);
				if (parseCurrent == num)
				{
					return ch;
				}
				ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
				if (parseCurrent == num)
				{
					this.tokenBuilder.MarkPropertyAsDeleted();
					return ch;
				}
				if (!this.IsNameStartCharacter(ch, charClass, parseCurrent))
				{
					this.tokenBuilder.MarkPropertyAsDeleted();
					return ch;
				}
				int num2 = parseCurrent;
				ch = this.ScanName(CssRunKind.Important, ch, ref charClass, ref parseCurrent);
				if (parseCurrent == num)
				{
					return ch;
				}
				if (!this.IsNameEqual("important", num2, parseCurrent - num2))
				{
					this.tokenBuilder.MarkPropertyAsDeleted();
					return ch;
				}
			}
			return ch;
		}

		private char ScanExpr(char ch, ref CharClass charClass, ref int parseCurrent, int level)
		{
			int num = this.parseEnd;
			char[] array = this.parseBuffer;
			int i = parseCurrent;
			ch = this.ScanTerm(ch, ref charClass, ref parseCurrent, level);
			if (parseCurrent == num)
			{
				return ch;
			}
			while (i < parseCurrent)
			{
				bool flag = false;
				bool flag2 = false;
				i = parseCurrent;
				ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, false);
				if (parseCurrent == num)
				{
					return ch;
				}
				if (i < parseCurrent)
				{
					flag = true;
				}
				if (ch == '/' || ch == ',')
				{
					ch = array[++parseCurrent];
					charClass = ParseSupport.GetCharClass(ch);
					if (parseCurrent == num)
					{
						return ch;
					}
					this.PrepareAndAddRun(CssRunKind.Operator, parseCurrent - 1, ref parseCurrent);
					if (parseCurrent == num)
					{
						return ch;
					}
					flag2 = true;
					ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
					if (parseCurrent == num)
					{
						return ch;
					}
				}
				else if (i == parseCurrent)
				{
					break;
				}
				i = parseCurrent;
				ch = this.ScanTerm(ch, ref charClass, ref parseCurrent, level);
				if (parseCurrent == num)
				{
					return ch;
				}
				if (i == parseCurrent)
				{
					if (flag2)
					{
						this.tokenBuilder.InvalidateLastValidRun(CssRunKind.Operator);
					}
					if (flag)
					{
						this.tokenBuilder.InvalidateLastValidRun(CssRunKind.Space);
						break;
					}
					break;
				}
			}
			return ch;
		}

		private char ScanTerm(char ch, ref CharClass charClass, ref int parseCurrent, int level)
		{
			int num = this.parseEnd;
			char[] array = this.parseBuffer;
			bool flag = false;
			if (ch == '-' || ch == '+')
			{
				ch = array[++parseCurrent];
				charClass = ParseSupport.GetCharClass(ch);
				if (parseCurrent == num)
				{
					this.tokenBuilder.MarkPropertyAsDeleted();
					return ch;
				}
				this.PrepareAndAddRun(CssRunKind.UnaryOperator, parseCurrent - 1, ref parseCurrent);
				if (parseCurrent == num)
				{
					return ch;
				}
				flag = true;
			}
			if (ParseSupport.NumericCharacter(charClass) || ch == '.')
			{
				ch = this.ScanNumeric(ch, ref charClass, ref parseCurrent);
				if (parseCurrent == num)
				{
					return ch;
				}
				if (ch == '.')
				{
					ch = array[++parseCurrent];
					charClass = ParseSupport.GetCharClass(ch);
					this.PrepareAndAddRun(CssRunKind.Dot, parseCurrent - 1, ref parseCurrent);
					if (parseCurrent == num)
					{
						return ch;
					}
					int num2 = parseCurrent;
					ch = this.ScanNumeric(ch, ref charClass, ref parseCurrent);
					if (parseCurrent == num)
					{
						return ch;
					}
					if (num2 == parseCurrent)
					{
						this.tokenBuilder.MarkPropertyAsDeleted();
					}
				}
				if (ch == '%')
				{
					ch = array[++parseCurrent];
					charClass = ParseSupport.GetCharClass(ch);
					this.PrepareAndAddRun(CssRunKind.Percent, parseCurrent - 1, ref parseCurrent);
					if (parseCurrent == num)
					{
						return ch;
					}
				}
				else if (this.IsIdentStartCharacter(ch, charClass, parseCurrent))
				{
					int num3;
					ch = this.ScanIdent(CssRunKind.Metrics, ch, ref charClass, ref parseCurrent, out num3);
					if (parseCurrent == num)
					{
						return ch;
					}
				}
			}
			else if (this.IsIdentStartCharacter(ch, charClass, parseCurrent))
			{
				int num2 = parseCurrent;
				int num4;
				ch = this.ScanIdent(CssRunKind.TermIdentifier, ch, ref charClass, ref parseCurrent, out num4);
				if (parseCurrent == num)
				{
					return ch;
				}
				int start = parseCurrent;
				if (ch == '+' && num2 + 1 == parseCurrent && (array[num2] == 'u' || array[num2] == 'U'))
				{
					ch = this.ScanUnicodeRange(ch, ref charClass, ref parseCurrent);
					this.PrepareAndAddRun(CssRunKind.UnicodeRange, start, ref parseCurrent);
					if (parseCurrent == num)
					{
						return ch;
					}
				}
				else if (ch == '(')
				{
					bool flag2 = false;
					if (!this.IsSafeIdentifier(CssParser.SafeTermFunctions, num2, parseCurrent))
					{
						this.tokenBuilder.MarkPropertyAsDeleted();
						if (this.IsNameEqual("url", num2, parseCurrent - num2))
						{
							flag2 = true;
						}
					}
					ch = array[++parseCurrent];
					charClass = ParseSupport.GetCharClass(ch);
					if (parseCurrent == num)
					{
						this.tokenBuilder.MarkPropertyAsDeleted();
						return ch;
					}
					this.PrepareAndAddRun(CssRunKind.FunctionStart, parseCurrent - 1, ref parseCurrent);
					if (parseCurrent == num)
					{
						this.tokenBuilder.MarkPropertyAsDeleted();
						return ch;
					}
					ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
					if (parseCurrent == num)
					{
						this.tokenBuilder.MarkPropertyAsDeleted();
						return ch;
					}
					if (flag2)
					{
						if (ch == '"' || ch == '\'')
						{
							num2 = parseCurrent;
							ch = this.ScanString(ch, ref charClass, ref parseCurrent, true);
							if (this.isInvalid)
							{
								return ch;
							}
							this.PrepareAndAddRun(CssRunKind.String, num2, ref parseCurrent);
							if (parseCurrent == num)
							{
								return ch;
							}
						}
						else
						{
							num2 = parseCurrent;
							ch = this.ScanUrl(ch, ref charClass, ref parseCurrent);
							if (parseCurrent == num)
							{
								return ch;
							}
						}
						ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
						if (parseCurrent == num)
						{
							return ch;
						}
					}
					else
					{
						if (++level > 16)
						{
							this.tokenBuilder.MarkPropertyAsDeleted();
							return ch;
						}
						ch = this.ScanExpr(ch, ref charClass, ref parseCurrent, level);
						if (parseCurrent == num)
						{
							this.tokenBuilder.MarkPropertyAsDeleted();
							return ch;
						}
					}
					if (ch != ')')
					{
						this.tokenBuilder.MarkPropertyAsDeleted();
					}
					ch = array[++parseCurrent];
					charClass = ParseSupport.GetCharClass(ch);
					this.PrepareAndAddRun(CssRunKind.FunctionEnd, parseCurrent - 1, ref parseCurrent);
					if (parseCurrent == num)
					{
						return ch;
					}
				}
				else if (flag)
				{
					this.tokenBuilder.MarkPropertyAsDeleted();
				}
			}
			else if (flag)
			{
				this.tokenBuilder.MarkPropertyAsDeleted();
			}
			else if (ch == '#')
			{
				ch = array[++parseCurrent];
				charClass = ParseSupport.GetCharClass(ch);
				this.PrepareAndAddRun(CssRunKind.HexColorStart, parseCurrent - 1, ref parseCurrent);
				if (parseCurrent == num)
				{
					return ch;
				}
				if (this.IsNameCharacter(ch, charClass, parseCurrent))
				{
					ch = this.ScanName(CssRunKind.HexColor, ch, ref charClass, ref parseCurrent);
					if (parseCurrent == num)
					{
						return ch;
					}
				}
				else
				{
					this.tokenBuilder.MarkPropertyAsDeleted();
				}
			}
			else if (ch == '"' || ch == '\'')
			{
				int num2 = parseCurrent;
				ch = this.ScanString(ch, ref charClass, ref parseCurrent, true);
				if (this.isInvalid)
				{
					return ch;
				}
				this.PrepareAndAddRun(CssRunKind.String, num2, ref parseCurrent);
				if (parseCurrent == num)
				{
					return ch;
				}
			}
			return ch;
		}

		private char ScanNumeric(char ch, ref CharClass charClass, ref int parseCurrent)
		{
			int start = parseCurrent;
			char[] array = this.parseBuffer;
			while (ParseSupport.NumericCharacter(charClass))
			{
				ch = array[++parseCurrent];
				charClass = ParseSupport.GetCharClass(ch);
			}
			this.PrepareAndAddRun(CssRunKind.Numeric, start, ref parseCurrent);
			return ch;
		}

		private char ScanString(char ch, ref CharClass charClass, ref int parseCurrent, bool inProperty)
		{
			int num = this.parseEnd;
			char[] array = this.parseBuffer;
			char c = ch;
			char c2 = '\0';
			char c3 = '\0';
			bool flag = false;
			for (;;)
			{
				ch = array[++parseCurrent];
				if (parseCurrent == num)
				{
					break;
				}
				if (CssToken.AttemptUnescape(array, num, ref ch, ref parseCurrent))
				{
					flag = true;
					if (parseCurrent == num)
					{
						goto Block_4;
					}
					if (ch == c)
					{
						goto IL_BA;
					}
					c2 = '\0';
					c3 = '\0';
				}
				else
				{
					if (ch == c || (ch == '\n' && c2 == '\r' && c3 != '\\') || (((ch == '\n' && c2 != '\r') || ch == '\r' || ch == '\f') && c2 != '\\'))
					{
						goto IL_BA;
					}
					c3 = c2;
					c2 = ch;
				}
			}
			if (inProperty)
			{
				this.tokenBuilder.MarkPropertyAsDeleted();
			}
			charClass = ParseSupport.GetCharClass(ch);
			return ch;
			Block_4:
			if (inProperty)
			{
				this.tokenBuilder.MarkPropertyAsDeleted();
			}
			charClass = ParseSupport.GetCharClass(array[parseCurrent]);
			return array[parseCurrent];
			IL_BA:
			ch = array[++parseCurrent];
			charClass = ParseSupport.GetCharClass(ch);
			if (flag)
			{
				if (inProperty)
				{
					this.tokenBuilder.MarkPropertyAsDeleted();
				}
				else
				{
					this.isInvalid = true;
				}
			}
			return ch;
		}

		private char ScanName(CssRunKind runKind, char ch, ref CharClass charClass, ref int parseCurrent)
		{
			for (;;)
			{
				int num = parseCurrent;
				while (CssParser.IsNameCharacterNoEscape(ch, ParseSupport.GetCharClass(ch)) && parseCurrent != this.parseEnd)
				{
					ch = this.parseBuffer[++parseCurrent];
				}
				if (parseCurrent != num)
				{
					this.PrepareAndAddRun(runKind, num, ref parseCurrent);
				}
				if (parseCurrent == this.parseEnd)
				{
					goto IL_C7;
				}
				num = parseCurrent;
				if (ch != '\\')
				{
					goto IL_C7;
				}
				if (!CssToken.AttemptUnescape(this.parseBuffer, this.parseEnd, ref ch, ref parseCurrent) || !CssParser.IsNameCharacterNoEscape(ch, ParseSupport.GetCharClass(ch)))
				{
					break;
				}
				parseCurrent++;
				this.PrepareAndAddLiteralRun(runKind, num, ref parseCurrent, (int)ch);
				if (parseCurrent == this.parseEnd)
				{
					goto IL_C7;
				}
				ch = this.parseBuffer[parseCurrent];
			}
			ch = this.parseBuffer[++parseCurrent];
			this.PrepareAndAddInvalidRun(runKind, ref parseCurrent);
			IL_C7:
			charClass = ParseSupport.GetCharClass(ch);
			return ch;
		}

		private char ScanIdent(CssRunKind runKind, char ch, ref CharClass charClass, ref int parseCurrent, out int nameLength)
		{
			bool flag = false;
			nameLength = 0;
			for (;;)
			{
				int num = parseCurrent;
				while (CssParser.IsNameCharacterNoEscape(ch, ParseSupport.GetCharClass(ch)))
				{
					if (nameLength == 0 && ch == '-')
					{
						flag = true;
					}
					if (nameLength == 1 && flag && char.IsDigit(ch))
					{
						goto Block_5;
					}
					nameLength++;
					if (parseCurrent == this.parseEnd)
					{
						break;
					}
					ch = this.parseBuffer[++parseCurrent];
				}
				if (parseCurrent != num)
				{
					this.PrepareAndAddRun(runKind, num, ref parseCurrent);
				}
				if (parseCurrent == this.parseEnd)
				{
					goto IL_133;
				}
				num = parseCurrent;
				if (ch != '\\')
				{
					goto IL_133;
				}
				if (!CssToken.AttemptUnescape(this.parseBuffer, this.parseEnd, ref ch, ref parseCurrent) || !CssParser.IsNameCharacterNoEscape(ch, ParseSupport.GetCharClass(ch)))
				{
					goto IL_B9;
				}
				parseCurrent++;
				if (nameLength == 0 && ch == '-')
				{
					flag = true;
				}
				if (nameLength == 1 && flag && char.IsDigit(ch))
				{
					goto Block_15;
				}
				nameLength++;
				this.PrepareAndAddLiteralRun(runKind, num, ref parseCurrent, (int)ch);
				if (parseCurrent == this.parseEnd)
				{
					goto IL_133;
				}
				ch = this.parseBuffer[parseCurrent];
			}
			Block_5:
			nameLength = 0;
			charClass = ParseSupport.GetCharClass(ch);
			return ch;
			IL_B9:
			ch = this.parseBuffer[++parseCurrent];
			this.PrepareAndAddInvalidRun(runKind, ref parseCurrent);
			nameLength = 0;
			goto IL_133;
			Block_15:
			nameLength = 0;
			IL_133:
			charClass = ParseSupport.GetCharClass(ch);
			return ch;
		}

		private char ScanUrl(char ch, ref CharClass charClass, ref int parseCurrent)
		{
			for (;;)
			{
				int num = parseCurrent;
				while (this.IsUrlCharacter(ch, ParseSupport.GetCharClass(ch), parseCurrent) && parseCurrent != this.parseEnd)
				{
					ch = this.parseBuffer[++parseCurrent];
				}
				if (parseCurrent != num)
				{
					this.PrepareAndAddRun(CssRunKind.Url, num, ref parseCurrent);
				}
				if (parseCurrent == this.parseEnd)
				{
					goto IL_BA;
				}
				num = parseCurrent;
				if (ch != '\\')
				{
					goto IL_BA;
				}
				if (!CssToken.AttemptUnescape(this.parseBuffer, this.parseEnd, ref ch, ref parseCurrent))
				{
					break;
				}
				parseCurrent++;
				this.PrepareAndAddLiteralRun(CssRunKind.Url, num, ref parseCurrent, (int)ch);
				if (parseCurrent == this.parseEnd)
				{
					goto IL_BA;
				}
				ch = this.parseBuffer[parseCurrent];
			}
			ch = this.parseBuffer[++parseCurrent];
			this.PrepareAndAddInvalidRun(CssRunKind.Url, ref parseCurrent);
			IL_BA:
			charClass = ParseSupport.GetCharClass(ch);
			return ch;
		}

		private char ScanUnicodeRange(char ch, ref CharClass charClass, ref int parseCurrent)
		{
			char[] array = this.parseBuffer;
			int num = parseCurrent + 1;
			int i = num;
			bool flag = true;
			char c;
			while (i < num + 6)
			{
				c = array[i];
				if ('?' == c)
				{
					flag = false;
					for (i++; i < num + 6; i++)
					{
						if ('?' != array[i])
						{
							break;
						}
					}
					break;
				}
				if (!ParseSupport.HexCharacter(ParseSupport.GetCharClass(c)))
				{
					if (i == num)
					{
						return ch;
					}
					break;
				}
				else
				{
					i++;
				}
			}
			c = array[i];
			if ('-' == c && flag)
			{
				i++;
				num = i;
				while (i < num + 6)
				{
					c = array[i];
					if (!ParseSupport.HexCharacter(ParseSupport.GetCharClass(c)))
					{
						if (i == num)
						{
							return ch;
						}
						break;
					}
					else
					{
						i++;
					}
				}
			}
			c = array[i];
			charClass = ParseSupport.GetCharClass(c);
			parseCurrent = i;
			return c;
		}

		private char ScanWhitespace(char ch, ref CharClass charClass, ref int parseCurrent, bool ignorable)
		{
			char[] array = this.parseBuffer;
			int num = this.parseEnd;
			IL_7F:
			while (ParseSupport.WhitespaceCharacter(charClass) || ch == '/')
			{
				if (ch != '/')
				{
					int start = parseCurrent;
					while (++parseCurrent != num)
					{
						ch = array[parseCurrent];
						charClass = ParseSupport.GetCharClass(ch);
						if (!ParseSupport.WhitespaceCharacter(charClass))
						{
							if (this.tokenBuilder.IsStarted)
							{
								this.PrepareAndAddRun(ignorable ? CssRunKind.Invalid : CssRunKind.Space, start, ref parseCurrent);
								goto IL_7F;
							}
							goto IL_7F;
						}
					}
					return ch;
				}
				if (parseCurrent >= num || array[parseCurrent + 1] != '*')
				{
					break;
				}
				ch = this.ScanComment(ch, ref charClass, ref parseCurrent);
				if (parseCurrent == num)
				{
					return ch;
				}
			}
			return ch;
		}

		private char ScanComment(char ch, ref CharClass charClass, ref int parseCurrent)
		{
			char[] array = this.parseBuffer;
			int num = this.parseEnd;
			int start = parseCurrent;
			ch = array[++parseCurrent];
			while (++parseCurrent != num)
			{
				if (array[parseCurrent] == '*' && parseCurrent + 1 != num && array[parseCurrent + 1] == '/')
				{
					parseCurrent++;
					if (++parseCurrent == num)
					{
						return ch;
					}
					if (this.tokenBuilder.IsStarted)
					{
						this.PrepareAndAddRun(CssRunKind.Space, start, ref parseCurrent);
					}
					ch = array[parseCurrent];
					charClass = ParseSupport.GetCharClass(ch);
					return ch;
				}
			}
			return ch;
		}

		private void PrepareAndAddRun(CssRunKind runKind, int start, ref int parseCurrent)
		{
			if (!this.tokenBuilder.PrepareAndAddRun(runKind, start, parseCurrent))
			{
				parseCurrent = this.parseEnd;
			}
		}

		private void PrepareAndAddInvalidRun(CssRunKind runKind, ref int parseCurrent)
		{
			if (!this.tokenBuilder.PrepareAndAddInvalidRun(runKind, parseCurrent))
			{
				parseCurrent = this.parseEnd;
			}
		}

		private void PrepareAndAddLiteralRun(CssRunKind runKind, int start, ref int parseCurrent, int value)
		{
			if (!this.tokenBuilder.PrepareAndAddLiteralRun(runKind, start, parseCurrent, value))
			{
				parseCurrent = this.parseEnd;
			}
		}

		private char SkipToNextRule(char ch, ref CharClass charClass, ref int parseCurrent)
		{
			int num = this.parseEnd;
			char[] array = this.parseBuffer;
			for (;;)
			{
				if (ch == '"' || ch == '\'')
				{
					ch = this.ScanString(ch, ref charClass, ref parseCurrent, false);
					if (parseCurrent == num)
					{
						break;
					}
				}
				else
				{
					if (ch == '{')
					{
						this.ruleDepth++;
					}
					else if (ch == '}')
					{
						if (this.ruleDepth > 0)
						{
							this.ruleDepth--;
						}
						if (this.ruleDepth == 0)
						{
							goto Block_6;
						}
					}
					else if (ch == ';' && this.ruleDepth == 0)
					{
						goto Block_8;
					}
					if (++parseCurrent == num)
					{
						return ch;
					}
					ch = array[parseCurrent];
				}
			}
			return ch;
			Block_6:
			ch = array[++parseCurrent];
			charClass = ParseSupport.GetCharClass(ch);
			return ch;
			Block_8:
			ch = array[++parseCurrent];
			charClass = ParseSupport.GetCharClass(ch);
			return ch;
		}

		private bool IsSafeIdentifier(string[] table, int start, int end)
		{
			int length = end - start;
			for (int i = 0; i < table.Length; i++)
			{
				if (this.IsNameEqual(table[i], start, length))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsNameEqual(string name, int start, int length)
		{
			return name.Equals(new string(this.parseBuffer, start, length), StringComparison.OrdinalIgnoreCase);
		}

		private bool IsNameCharacter(char ch, CharClass charClass, int parseCurrent)
		{
			return this.IsNameStartCharacter(ch, charClass, parseCurrent) || ParseSupport.NumericCharacter(charClass) || ch == '-';
		}

		private bool IsIdentStartCharacter(char ch, CharClass charClass, int parseCurrent)
		{
			if (CssParser.IsNameStartCharacterNoEscape(ch, charClass))
			{
				return true;
			}
			if (ch == '-')
			{
				return true;
			}
			if (CssToken.AttemptUnescape(this.parseBuffer, this.parseEnd, ref ch, ref parseCurrent))
			{
				charClass = ParseSupport.GetCharClass(ch);
				return CssParser.IsNameStartCharacterNoEscape(ch, charClass) || ch == '-';
			}
			return false;
		}

		private bool IsNameStartCharacter(char ch, CharClass charClass, int parseCurrent)
		{
			if (CssParser.IsNameStartCharacterNoEscape(ch, charClass))
			{
				return true;
			}
			if (CssToken.AttemptUnescape(this.parseBuffer, this.parseEnd, ref ch, ref parseCurrent))
			{
				charClass = ParseSupport.GetCharClass(ch);
				return CssParser.IsNameStartCharacterNoEscape(ch, charClass);
			}
			return false;
		}

		private bool IsUrlCharacter(char ch, CharClass charClass, int parseCurrent)
		{
			return CssParser.IsUrlCharacterNoEscape(ch, charClass) || this.IsEscape(ch, parseCurrent);
		}

		private bool IsEscape(char ch, int parseCurrent)
		{
			return CssToken.AttemptUnescape(this.parseBuffer, this.parseEnd, ref ch, ref parseCurrent);
		}

		internal const int MaxCssLength = 524288;

		protected CssTokenBuilder tokenBuilder;

		private static readonly string[] SafeTermFunctions = new string[]
		{
			"rgb",
			"counter"
		};

		private static readonly string[] SafePseudoFunctions = new string[]
		{
			"lang"
		};

		private ConverterInput input;

		private bool endOfFile;

		private CssParseMode parseMode;

		private bool isInvalid;

		private char[] parseBuffer;

		private int parseStart;

		private int parseCurrent;

		private int parseEnd;

		private int ruleDepth;

		private CssToken token;
	}
}
