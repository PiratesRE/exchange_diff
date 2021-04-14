using System;
using System.IO;
using Microsoft.Exchange.Data.TextConverters.Internal.Format;
using Microsoft.Exchange.Data.TextConverters.Internal.Text;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Html
{
	internal class HtmlToTextConverter : IProducerConsumer, IRestartable, IReusable, IDisposable
	{
		public HtmlToTextConverter(IHtmlParser parser, TextOutput output, Injection injection, bool convertFragment, bool preformattedText, bool testTreatNbspAsBreakable, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum, bool shouldUseNarrowGapForPTagHtmlToTextConversion, bool outputAnchorLinks, bool outputImageLinks)
		{
			this.normalizedInput = (parser is HtmlNormalizingParser);
			this.treatNbspAsBreakable = testTreatNbspAsBreakable;
			this.convertFragment = convertFragment;
			this.output = output;
			this.parser = parser;
			this.parser.SetRestartConsumer(this);
			this.injection = injection;
			if (!convertFragment)
			{
				this.output.OpenDocument();
				if (this.injection != null && this.injection.HaveHead)
				{
					this.injection.Inject(true, this.output);
				}
			}
			else
			{
				this.insidePre = preformattedText;
			}
			this.shouldUseNarrowGapForPTagHtmlToTextConversion = shouldUseNarrowGapForPTagHtmlToTextConversion;
			this.outputAnchorLinks = outputImageLinks;
			this.outputImageLinks = outputImageLinks;
		}

		private void Reinitialize()
		{
			this.endOfFile = false;
			this.normalizerContext.HasSpace = false;
			this.normalizerContext.EatSpace = false;
			this.normalizerContext.OneNL = false;
			this.normalizerContext.LastCh = '\0';
			this.lineStarted = false;
			this.wideGap = false;
			this.nextParagraphCloseWideGap = true;
			this.afterFirstParagraph = false;
			this.ignoreNextP = false;
			this.insideComment = false;
			this.insidePre = false;
			this.insideAnchor = false;
			if (this.urlCompareSink != null)
			{
				this.urlCompareSink.Reset();
			}
			this.listLevel = 0;
			this.listIndex = 0;
			this.listOrdered = false;
			if (!this.convertFragment)
			{
				this.output.OpenDocument();
				if (this.injection != null)
				{
					this.injection.Reset();
					if (this.injection.HaveHead)
					{
						this.injection.Inject(true, this.output);
					}
				}
			}
			this.textMapping = TextMapping.Unicode;
		}

		public void Run()
		{
			if (!this.endOfFile)
			{
				HtmlTokenId htmlTokenId = this.parser.Parse();
				if (htmlTokenId != HtmlTokenId.None)
				{
					this.Process(htmlTokenId);
				}
			}
		}

		public bool Flush()
		{
			if (!this.endOfFile)
			{
				this.Run();
			}
			return this.endOfFile;
		}

		private void Process(HtmlTokenId tokenId)
		{
			this.token = this.parser.Token;
			switch (tokenId)
			{
			case HtmlTokenId.EndOfFile:
				if (this.lineStarted)
				{
					this.output.OutputNewLine();
					this.lineStarted = false;
				}
				if (!this.convertFragment)
				{
					if (this.injection != null && this.injection.HaveHead)
					{
						if (this.wideGap)
						{
							this.output.OutputNewLine();
							this.wideGap = false;
						}
						this.injection.Inject(false, this.output);
					}
					this.output.CloseDocument();
					this.output.Flush();
				}
				this.endOfFile = true;
				break;
			case HtmlTokenId.Text:
				if (!this.insideComment)
				{
					if (this.insideAnchor && this.urlCompareSink.IsActive)
					{
						this.token.Text.WriteTo(this.urlCompareSink);
					}
					if (this.insidePre)
					{
						this.ProcessPreformatedText();
						return;
					}
					if (this.normalizedInput)
					{
						this.ProcessText();
						return;
					}
					this.NormalizeProcessText();
					return;
				}
				break;
			case HtmlTokenId.EncodingChange:
				if (this.output.OutputCodePageSameAsInput)
				{
					this.output.OutputEncoding = this.token.TokenEncoding;
					return;
				}
				break;
			case HtmlTokenId.Tag:
			{
				if (this.token.TagIndex <= HtmlTagIndex.Unknown)
				{
					return;
				}
				HtmlDtd.TagDefinition tagDefinition = HtmlToTextConverter.GetTagDefinition(this.token.TagIndex);
				if (this.normalizedInput)
				{
					if (!this.token.IsEndTag)
					{
						if (this.token.IsTagBegin)
						{
							this.PushElement(tagDefinition);
						}
						this.ProcessStartTagAttributes(tagDefinition);
						return;
					}
					if (this.token.IsTagBegin)
					{
						this.PopElement(tagDefinition);
						return;
					}
				}
				else
				{
					if (!this.token.IsEndTag)
					{
						if (this.token.IsTagBegin)
						{
							this.LFillTagB(tagDefinition);
							this.PushElement(tagDefinition);
							this.RFillTagB(tagDefinition);
						}
						this.ProcessStartTagAttributes(tagDefinition);
						return;
					}
					if (this.token.IsTagBegin)
					{
						this.LFillTagE(tagDefinition);
						this.PopElement(tagDefinition);
						this.RFillTagE(tagDefinition);
						return;
					}
				}
				break;
			}
			case HtmlTokenId.Restart:
			case HtmlTokenId.OverlappedClose:
			case HtmlTokenId.OverlappedReopen:
				break;
			default:
				return;
			}
		}

		private void PushElement(HtmlDtd.TagDefinition tagDef)
		{
			HtmlTagIndex tagIndex = tagDef.TagIndex;
			if (tagIndex <= HtmlTagIndex.Listing)
			{
				if (tagIndex <= HtmlTagIndex.DT)
				{
					if (tagIndex != HtmlTagIndex.A)
					{
						if (tagIndex == HtmlTagIndex.BR)
						{
							goto IL_193;
						}
						switch (tagIndex)
						{
						case HtmlTagIndex.Comment:
							break;
						case HtmlTagIndex.DD:
							if (this.lineStarted)
							{
								this.EndLine();
								goto IL_2FF;
							}
							goto IL_2FF;
						case HtmlTagIndex.Del:
						case HtmlTagIndex.Dfn:
						case HtmlTagIndex.Div:
							goto IL_2F0;
						case HtmlTagIndex.Dir:
							goto IL_1BC;
						case HtmlTagIndex.DL:
							this.EndParagraph(true);
							goto IL_2FF;
						case HtmlTagIndex.DT:
							if (this.lineStarted)
							{
								this.EndLine();
								goto IL_2FF;
							}
							goto IL_2FF;
						default:
							goto IL_2F0;
						}
					}
					else
					{
						if (this.insideAnchor)
						{
							this.EndAnchor();
							goto IL_2FF;
						}
						goto IL_2FF;
					}
				}
				else if (tagIndex <= HtmlTagIndex.HR)
				{
					if (tagIndex == HtmlTagIndex.Font)
					{
						goto IL_2FF;
					}
					if (tagIndex != HtmlTagIndex.HR)
					{
						goto IL_2F0;
					}
					this.EndParagraph(false);
					this.OutputText("________________________________");
					this.EndParagraph(false);
					goto IL_2FF;
				}
				else
				{
					switch (tagIndex)
					{
					case HtmlTagIndex.Image:
					case HtmlTagIndex.Img:
						goto IL_2FF;
					default:
						switch (tagIndex)
						{
						case HtmlTagIndex.LI:
						{
							this.EndParagraph(false);
							this.OutputText("  ");
							for (int i = 0; i < this.listLevel - 1; i++)
							{
								this.OutputText("   ");
							}
							if (this.listLevel > 1 || !this.listOrdered)
							{
								this.OutputText("*");
								this.output.OutputSpace(3);
								goto IL_2FF;
							}
							string text = this.listIndex.ToString();
							this.OutputText(text);
							this.OutputText(".");
							this.output.OutputSpace((text.Length == 1) ? 2 : 1);
							this.listIndex++;
							goto IL_2FF;
						}
						case HtmlTagIndex.Link:
							goto IL_2F0;
						case HtmlTagIndex.Listing:
							goto IL_2E0;
						default:
							goto IL_2F0;
						}
						break;
					}
				}
			}
			else if (tagIndex <= HtmlTagIndex.Style)
			{
				if (tagIndex <= HtmlTagIndex.Script)
				{
					switch (tagIndex)
					{
					case HtmlTagIndex.Menu:
					case HtmlTagIndex.OL:
						goto IL_1BC;
					case HtmlTagIndex.Meta:
					case HtmlTagIndex.NextId:
					case HtmlTagIndex.NoBR:
					case HtmlTagIndex.NoScript:
					case HtmlTagIndex.Object:
					case HtmlTagIndex.OptGroup:
					case HtmlTagIndex.Param:
						goto IL_2F0;
					case HtmlTagIndex.NoEmbed:
					case HtmlTagIndex.NoFrames:
						break;
					case HtmlTagIndex.Option:
						goto IL_193;
					case HtmlTagIndex.P:
						if (!this.ignoreNextP)
						{
							this.EndParagraph(true);
						}
						this.nextParagraphCloseWideGap = true;
						goto IL_2FF;
					case HtmlTagIndex.PlainText:
					case HtmlTagIndex.Pre:
						goto IL_2E0;
					default:
						if (tagIndex != HtmlTagIndex.Script)
						{
							goto IL_2F0;
						}
						break;
					}
				}
				else
				{
					if (tagIndex == HtmlTagIndex.Span)
					{
						goto IL_2FF;
					}
					if (tagIndex != HtmlTagIndex.Style)
					{
						goto IL_2F0;
					}
				}
			}
			else if (tagIndex <= HtmlTagIndex.Title)
			{
				if (tagIndex != HtmlTagIndex.TD)
				{
					switch (tagIndex)
					{
					case HtmlTagIndex.TH:
						break;
					case HtmlTagIndex.Thead:
						goto IL_2F0;
					case HtmlTagIndex.Title:
						goto IL_13A;
					default:
						goto IL_2F0;
					}
				}
				if (this.lineStarted)
				{
					this.output.OutputTabulation(1);
					goto IL_2FF;
				}
				goto IL_2FF;
			}
			else
			{
				if (tagIndex == HtmlTagIndex.UL)
				{
					goto IL_1BC;
				}
				if (tagIndex != HtmlTagIndex.Xmp)
				{
					goto IL_2F0;
				}
				goto IL_2E0;
			}
			IL_13A:
			this.insideComment = true;
			goto IL_2FF;
			IL_193:
			this.EndLine();
			goto IL_2FF;
			IL_1BC:
			this.EndParagraph(this.listLevel == 0);
			if (this.listLevel < 10)
			{
				this.listLevel++;
				if (this.listLevel == 1)
				{
					this.listIndex = 1;
					this.listOrdered = (this.token.TagIndex == HtmlTagIndex.OL);
				}
			}
			this.nextParagraphCloseWideGap = false;
			goto IL_2FF;
			IL_2E0:
			this.EndParagraph(true);
			this.insidePre = true;
			goto IL_2FF;
			IL_2F0:
			if (tagDef.BlockElement)
			{
				this.EndParagraph(false);
			}
			IL_2FF:
			this.ignoreNextP = false;
			if (tagDef.TagIndex == HtmlTagIndex.LI)
			{
				this.ignoreNextP = true;
			}
		}

		private void ProcessStartTagAttributes(HtmlDtd.TagDefinition tagDef)
		{
			HtmlTagIndex tagIndex = tagDef.TagIndex;
			if (tagIndex <= HtmlTagIndex.Font)
			{
				if (tagIndex != HtmlTagIndex.A)
				{
					if (tagIndex != HtmlTagIndex.Font)
					{
						return;
					}
					foreach (HtmlAttribute attr in this.token.Attributes)
					{
						if (attr.NameIndex == HtmlNameIndex.Face)
						{
							this.scratch.Reset();
							this.scratch.AppendHtmlAttributeValue(attr, 4096);
							RecognizeInterestingFontName recognizeInterestingFontName = default(RecognizeInterestingFontName);
							int num = 0;
							while (num < this.scratch.Length && !recognizeInterestingFontName.IsRejected)
							{
								recognizeInterestingFontName.AddCharacter(this.scratch.Buffer[num]);
								num++;
							}
							this.textMapping = recognizeInterestingFontName.TextMapping;
							return;
						}
					}
					return;
				}
				else if (this.outputAnchorLinks)
				{
					foreach (HtmlAttribute attr2 in this.token.Attributes)
					{
						if (attr2.NameIndex == HtmlNameIndex.Href)
						{
							if (attr2.IsAttrBegin)
							{
								this.urlScratch.Reset();
							}
							this.urlScratch.AppendHtmlAttributeValue(attr2, 4096);
							break;
						}
					}
					if (this.token.IsTagEnd)
					{
						BufferString bufferString = this.urlScratch.BufferString;
						bufferString.TrimWhitespace();
						if (bufferString.Length != 0 && bufferString[0] != '#' && bufferString[0] != '?' && bufferString[0] != ';')
						{
							if (!this.lineStarted)
							{
								this.StartParagraphOrLine();
							}
							string text = bufferString.ToString();
							if (text.IndexOf(' ') != -1)
							{
								text = text.Replace(" ", "%20");
							}
							this.output.OpenAnchor(text);
							this.insideAnchor = true;
							if (this.urlCompareSink == null)
							{
								this.urlCompareSink = new UrlCompareSink();
							}
							this.urlCompareSink.Initialize(text);
						}
						this.urlScratch.Reset();
						return;
					}
				}
			}
			else
			{
				switch (tagIndex)
				{
				case HtmlTagIndex.Image:
				case HtmlTagIndex.Img:
					if (this.outputImageLinks)
					{
						foreach (HtmlAttribute attr3 in this.token.Attributes)
						{
							if (attr3.NameIndex == HtmlNameIndex.Src)
							{
								if (attr3.IsAttrBegin)
								{
									this.urlScratch.Reset();
								}
								this.urlScratch.AppendHtmlAttributeValue(attr3, 4096);
							}
							else if (attr3.NameIndex == HtmlNameIndex.Alt)
							{
								if (attr3.IsAttrBegin)
								{
									this.imageAltText.Reset();
								}
								this.imageAltText.AppendHtmlAttributeValue(attr3, 4096);
							}
							else if (attr3.NameIndex == HtmlNameIndex.Height)
							{
								if (!attr3.Value.IsEmpty)
								{
									PropertyValue propertyValue;
									if (attr3.Value.IsContiguous)
									{
										propertyValue = HtmlSupport.ParseNumber(attr3.Value.ContiguousBufferString, HtmlSupport.NumberParseFlags.Length);
									}
									else
									{
										this.scratch.Reset();
										this.scratch.AppendHtmlAttributeValue(attr3, 4096);
										propertyValue = HtmlSupport.ParseNumber(this.scratch.BufferString, HtmlSupport.NumberParseFlags.Length);
									}
									if (propertyValue.IsAbsRelLength)
									{
										this.imageHeightPixels = propertyValue.PixelsInteger;
										if (this.imageHeightPixels == 0)
										{
											this.imageHeightPixels = 1;
										}
									}
								}
							}
							else if (attr3.NameIndex == HtmlNameIndex.Width && !attr3.Value.IsEmpty)
							{
								PropertyValue propertyValue2;
								if (attr3.Value.IsContiguous)
								{
									propertyValue2 = HtmlSupport.ParseNumber(attr3.Value.ContiguousBufferString, HtmlSupport.NumberParseFlags.Length);
								}
								else
								{
									this.scratch.Reset();
									this.scratch.AppendHtmlAttributeValue(attr3, 4096);
									propertyValue2 = HtmlSupport.ParseNumber(this.scratch.BufferString, HtmlSupport.NumberParseFlags.Length);
								}
								if (propertyValue2.IsAbsRelLength)
								{
									this.imageWidthPixels = propertyValue2.PixelsInteger;
									if (this.imageWidthPixels == 0)
									{
										this.imageWidthPixels = 1;
									}
								}
							}
						}
						if (this.token.IsTagEnd)
						{
							string imageUrl = null;
							string text2 = null;
							BufferString bufferString2 = this.imageAltText.BufferString;
							bufferString2.TrimWhitespace();
							if (bufferString2.Length != 0)
							{
								text2 = bufferString2.ToString();
							}
							if (text2 == null || this.output.ImageRenderingCallbackDefined)
							{
								BufferString bufferString3 = this.urlScratch.BufferString;
								bufferString3.TrimWhitespace();
								if (bufferString3.Length != 0)
								{
									imageUrl = bufferString3.ToString();
								}
							}
							if (!this.lineStarted)
							{
								this.StartParagraphOrLine();
							}
							this.output.OutputImage(imageUrl, text2, this.imageWidthPixels, this.imageHeightPixels);
							this.urlScratch.Reset();
							this.imageAltText.Reset();
							this.imageHeightPixels = 0;
							this.imageWidthPixels = 0;
							return;
						}
					}
					break;
				default:
					if (tagIndex == HtmlTagIndex.P)
					{
						if (!this.shouldUseNarrowGapForPTagHtmlToTextConversion)
						{
							if (!this.token.Attributes.Find(HtmlNameIndex.Class))
							{
								break;
							}
							HtmlAttribute htmlAttribute = this.token.Attributes.Current;
							if (!htmlAttribute.Value.CaseInsensitiveCompareEqual("msonormal"))
							{
								break;
							}
						}
						this.wideGap = false;
						this.nextParagraphCloseWideGap = false;
						return;
					}
					if (tagIndex != HtmlTagIndex.Span)
					{
						return;
					}
					foreach (HtmlAttribute attr4 in this.token.Attributes)
					{
						if (attr4.NameIndex == HtmlNameIndex.Style)
						{
							this.scratch.Reset();
							this.scratch.AppendHtmlAttributeValue(attr4, 4096);
							RecognizeInterestingFontNameInInlineStyle recognizeInterestingFontNameInInlineStyle = default(RecognizeInterestingFontNameInInlineStyle);
							int num2 = 0;
							while (num2 < this.scratch.Length && !recognizeInterestingFontNameInInlineStyle.IsFinished)
							{
								recognizeInterestingFontNameInInlineStyle.AddCharacter(this.scratch.Buffer[num2]);
								num2++;
							}
							this.textMapping = recognizeInterestingFontNameInInlineStyle.TextMapping;
							return;
						}
					}
					break;
				}
			}
		}

		private void PopElement(HtmlDtd.TagDefinition tagDef)
		{
			HtmlTagIndex tagIndex = tagDef.TagIndex;
			if (tagIndex <= HtmlTagIndex.Listing)
			{
				if (tagIndex <= HtmlTagIndex.DT)
				{
					if (tagIndex <= HtmlTagIndex.BR)
					{
						if (tagIndex != HtmlTagIndex.A)
						{
							if (tagIndex != HtmlTagIndex.BR)
							{
								goto IL_1D6;
							}
							goto IL_173;
						}
						else
						{
							if (this.insideAnchor)
							{
								this.EndAnchor();
								goto IL_1E5;
							}
							goto IL_1E5;
						}
					}
					else
					{
						switch (tagIndex)
						{
						case HtmlTagIndex.Comment:
							break;
						case HtmlTagIndex.DD:
							goto IL_1E5;
						case HtmlTagIndex.Del:
						case HtmlTagIndex.Dfn:
							goto IL_1D6;
						case HtmlTagIndex.Dir:
							goto IL_196;
						default:
							if (tagIndex != HtmlTagIndex.DT)
							{
								goto IL_1D6;
							}
							goto IL_1E5;
						}
					}
				}
				else if (tagIndex <= HtmlTagIndex.HR)
				{
					if (tagIndex == HtmlTagIndex.Font)
					{
						goto IL_1CD;
					}
					if (tagIndex != HtmlTagIndex.HR)
					{
						goto IL_1D6;
					}
					this.EndParagraph(false);
					this.OutputText("________________________________");
					this.EndParagraph(false);
					goto IL_1E5;
				}
				else
				{
					switch (tagIndex)
					{
					case HtmlTagIndex.Image:
					case HtmlTagIndex.Img:
						goto IL_1E5;
					default:
						if (tagIndex != HtmlTagIndex.Listing)
						{
							goto IL_1D6;
						}
						goto IL_1BD;
					}
				}
			}
			else if (tagIndex <= HtmlTagIndex.Style)
			{
				if (tagIndex <= HtmlTagIndex.Script)
				{
					switch (tagIndex)
					{
					case HtmlTagIndex.Menu:
					case HtmlTagIndex.OL:
						goto IL_196;
					case HtmlTagIndex.Meta:
					case HtmlTagIndex.NextId:
					case HtmlTagIndex.NoBR:
					case HtmlTagIndex.NoScript:
					case HtmlTagIndex.Object:
					case HtmlTagIndex.OptGroup:
					case HtmlTagIndex.Param:
						goto IL_1D6;
					case HtmlTagIndex.NoEmbed:
					case HtmlTagIndex.NoFrames:
						break;
					case HtmlTagIndex.Option:
						goto IL_173;
					case HtmlTagIndex.P:
						this.EndParagraph(this.nextParagraphCloseWideGap);
						this.nextParagraphCloseWideGap = true;
						goto IL_1E5;
					case HtmlTagIndex.PlainText:
					case HtmlTagIndex.Pre:
						goto IL_1BD;
					default:
						if (tagIndex != HtmlTagIndex.Script)
						{
							goto IL_1D6;
						}
						break;
					}
				}
				else
				{
					if (tagIndex == HtmlTagIndex.Span)
					{
						goto IL_1CD;
					}
					if (tagIndex != HtmlTagIndex.Style)
					{
						goto IL_1D6;
					}
				}
			}
			else
			{
				if (tagIndex <= HtmlTagIndex.Title)
				{
					if (tagIndex != HtmlTagIndex.TD)
					{
						switch (tagIndex)
						{
						case HtmlTagIndex.TH:
							break;
						case HtmlTagIndex.Thead:
							goto IL_1D6;
						case HtmlTagIndex.Title:
							goto IL_130;
						default:
							goto IL_1D6;
						}
					}
					this.lineStarted = true;
					goto IL_1E5;
				}
				if (tagIndex == HtmlTagIndex.UL)
				{
					goto IL_196;
				}
				if (tagIndex != HtmlTagIndex.Xmp)
				{
					goto IL_1D6;
				}
				goto IL_1BD;
			}
			IL_130:
			this.insideComment = false;
			goto IL_1E5;
			IL_173:
			this.EndLine();
			goto IL_1E5;
			IL_196:
			if (this.listLevel != 0)
			{
				this.listLevel--;
			}
			this.EndParagraph(this.listLevel == 0);
			goto IL_1E5;
			IL_1BD:
			this.EndParagraph(true);
			this.insidePre = false;
			goto IL_1E5;
			IL_1CD:
			this.textMapping = TextMapping.Unicode;
			goto IL_1E5;
			IL_1D6:
			if (tagDef.BlockElement)
			{
				this.EndParagraph(false);
			}
			IL_1E5:
			this.ignoreNextP = false;
		}

		private void ProcessText()
		{
			if (!this.lineStarted)
			{
				this.StartParagraphOrLine();
			}
			foreach (TokenRun tokenRun in this.token.Runs)
			{
				if (tokenRun.IsTextRun)
				{
					if (tokenRun.IsAnyWhitespace)
					{
						this.output.OutputSpace(1);
					}
					else if (tokenRun.TextType == RunTextType.Nbsp)
					{
						if (this.treatNbspAsBreakable)
						{
							this.output.OutputSpace(tokenRun.Length);
						}
						else
						{
							this.output.OutputNbsp(tokenRun.Length);
						}
					}
					else if (tokenRun.IsLiteral)
					{
						this.output.OutputNonspace(tokenRun.Literal, this.textMapping);
					}
					else
					{
						this.output.OutputNonspace(tokenRun.RawBuffer, tokenRun.RawOffset, tokenRun.RawLength, this.textMapping);
					}
				}
			}
		}

		private void ProcessPreformatedText()
		{
			if (!this.lineStarted)
			{
				this.StartParagraphOrLine();
			}
			foreach (TokenRun tokenRun in this.token.Runs)
			{
				if (tokenRun.IsTextRun)
				{
					if (tokenRun.IsAnyWhitespace)
					{
						RunTextType textType = tokenRun.TextType;
						if (textType != RunTextType.Space)
						{
							if (textType == RunTextType.NewLine)
							{
								this.output.OutputNewLine();
								continue;
							}
							if (textType == RunTextType.Tabulation)
							{
								this.output.OutputTabulation(tokenRun.Length);
								continue;
							}
						}
						if (this.treatNbspAsBreakable)
						{
							this.output.OutputSpace(tokenRun.Length);
						}
						else
						{
							this.output.OutputNbsp(tokenRun.Length);
						}
					}
					else if (tokenRun.TextType == RunTextType.Nbsp)
					{
						if (this.treatNbspAsBreakable)
						{
							this.output.OutputSpace(tokenRun.Length);
						}
						else
						{
							this.output.OutputNbsp(tokenRun.Length);
						}
					}
					else if (tokenRun.IsLiteral)
					{
						this.output.OutputNonspace(tokenRun.Literal, this.textMapping);
					}
					else
					{
						this.output.OutputNonspace(tokenRun.RawBuffer, tokenRun.RawOffset, tokenRun.RawLength, this.textMapping);
					}
				}
			}
		}

		private void NormalizeProcessText()
		{
			Token.RunEnumerator runs = this.token.Runs;
			runs.MoveNext(true);
			while (runs.IsValidPosition)
			{
				TokenRun run = runs.Current;
				if (run.IsAnyWhitespace)
				{
					int num = 0;
					TokenRun tokenRun2;
					do
					{
						int num2 = num;
						TokenRun tokenRun = runs.Current;
						num = num2 + ((tokenRun.TextType == RunTextType.NewLine) ? 1 : 2);
						if (!runs.MoveNext(true))
						{
							break;
						}
						tokenRun2 = runs.Current;
					}
					while (tokenRun2.TextType <= RunTextType.UnusualWhitespace);
					this.NormalizeAddSpace(num == 1);
				}
				else if (run.TextType == RunTextType.Nbsp)
				{
					this.NormalizeAddNbsp(run.Length);
					runs.MoveNext(true);
				}
				else
				{
					this.NormalizeAddNonspace(run);
					runs.MoveNext(true);
				}
			}
		}

		private void NormalizeAddNonspace(TokenRun run)
		{
			if (!this.lineStarted)
			{
				this.StartParagraphOrLine();
			}
			if (this.normalizerContext.HasSpace)
			{
				this.normalizerContext.HasSpace = false;
				if (this.normalizerContext.LastCh == '\0' || !this.normalizerContext.OneNL || !ParseSupport.TwoFarEastNonHanguelChars(this.normalizerContext.LastCh, run.FirstChar))
				{
					this.output.OutputSpace(1);
				}
			}
			if (run.IsLiteral)
			{
				this.output.OutputNonspace(run.Literal, this.textMapping);
			}
			else
			{
				this.output.OutputNonspace(run.RawBuffer, run.RawOffset, run.RawLength, this.textMapping);
			}
			this.normalizerContext.EatSpace = false;
			this.normalizerContext.LastCh = run.LastChar;
			this.normalizerContext.OneNL = false;
		}

		private void NormalizeAddNbsp(int count)
		{
			if (!this.lineStarted)
			{
				this.StartParagraphOrLine();
			}
			if (this.normalizerContext.HasSpace)
			{
				this.normalizerContext.HasSpace = false;
				this.output.OutputSpace(1);
			}
			if (this.treatNbspAsBreakable)
			{
				this.output.OutputSpace(count);
			}
			else
			{
				this.output.OutputNbsp(count);
			}
			this.normalizerContext.EatSpace = false;
			this.normalizerContext.LastCh = '\u00a0';
			this.normalizerContext.OneNL = false;
		}

		private void NormalizeAddSpace(bool oneNL)
		{
			if (!this.normalizerContext.EatSpace && this.afterFirstParagraph)
			{
				this.normalizerContext.HasSpace = true;
			}
			if (this.normalizerContext.LastCh != '\0')
			{
				if (oneNL && !this.normalizerContext.OneNL)
				{
					this.normalizerContext.OneNL = true;
					return;
				}
				this.normalizerContext.LastCh = '\0';
			}
		}

		private void LFillTagB(HtmlDtd.TagDefinition tagDef)
		{
			if (!this.insidePre)
			{
				this.LFill(tagDef.Fill.LB);
			}
		}

		private void RFillTagB(HtmlDtd.TagDefinition tagDef)
		{
			if (!this.insidePre)
			{
				this.RFill(tagDef.Fill.RB);
			}
		}

		private void LFillTagE(HtmlDtd.TagDefinition tagDef)
		{
			if (!this.insidePre)
			{
				this.LFill(tagDef.Fill.LE);
			}
		}

		private void RFillTagE(HtmlDtd.TagDefinition tagDef)
		{
			if (!this.insidePre)
			{
				this.RFill(tagDef.Fill.RE);
			}
		}

		private void LFill(HtmlDtd.FillCode codeLeft)
		{
			this.normalizerContext.LastCh = '\0';
			if (this.normalizerContext.HasSpace)
			{
				if (codeLeft == HtmlDtd.FillCode.PUT)
				{
					if (!this.lineStarted)
					{
						this.StartParagraphOrLine();
					}
					this.output.OutputSpace(1);
					this.normalizerContext.EatSpace = true;
				}
				this.normalizerContext.HasSpace = (codeLeft == HtmlDtd.FillCode.NUL);
			}
		}

		private void RFill(HtmlDtd.FillCode code)
		{
			if (code == HtmlDtd.FillCode.EAT)
			{
				this.normalizerContext.HasSpace = false;
				this.normalizerContext.EatSpace = true;
				return;
			}
			if (code == HtmlDtd.FillCode.PUT)
			{
				this.normalizerContext.EatSpace = false;
			}
		}

		private static HtmlDtd.TagDefinition GetTagDefinition(HtmlTagIndex tagIndex)
		{
			if (tagIndex == HtmlTagIndex._NULL)
			{
				return null;
			}
			return HtmlDtd.tags[(int)tagIndex];
		}

		private void EndAnchor()
		{
			if (!this.urlCompareSink.IsMatch)
			{
				if (!this.lineStarted)
				{
					this.StartParagraphOrLine();
				}
				this.output.CloseAnchor();
			}
			else
			{
				this.output.CancelAnchor();
			}
			this.insideAnchor = false;
			this.urlCompareSink.Reset();
		}

		private void OutputText(string text)
		{
			if (!this.lineStarted)
			{
				this.StartParagraphOrLine();
			}
			this.output.OutputNonspace(text, this.textMapping);
		}

		private void StartParagraphOrLine()
		{
			if (this.wideGap)
			{
				if (this.afterFirstParagraph)
				{
					this.output.OutputNewLine();
				}
				this.wideGap = false;
			}
			this.lineStarted = true;
			this.afterFirstParagraph = true;
		}

		private void EndLine()
		{
			this.output.OutputNewLine();
			this.lineStarted = false;
			this.wideGap = false;
		}

		private void EndParagraph(bool wideGap)
		{
			if (this.insideAnchor)
			{
				this.EndAnchor();
			}
			if (this.lineStarted)
			{
				this.output.OutputNewLine();
				this.lineStarted = false;
			}
			this.wideGap = (this.wideGap || wideGap);
		}

		void IDisposable.Dispose()
		{
			if (this.parser != null)
			{
				((IDisposable)this.parser).Dispose();
			}
			if (!this.convertFragment && this.output != null && this.output != null)
			{
				((IDisposable)this.output).Dispose();
			}
			if (this.token != null && this.token is IDisposable)
			{
				((IDisposable)this.token).Dispose();
			}
			if (this.injection != null)
			{
				((IDisposable)this.injection).Dispose();
			}
			this.parser = null;
			this.output = null;
			this.token = null;
			this.injection = null;
			GC.SuppressFinalize(this);
		}

		bool IRestartable.CanRestart()
		{
			return this.convertFragment || ((IRestartable)this.output).CanRestart();
		}

		void IRestartable.Restart()
		{
			if (!this.convertFragment)
			{
				((IRestartable)this.output).Restart();
			}
			this.Reinitialize();
		}

		void IRestartable.DisableRestart()
		{
			if (!this.convertFragment)
			{
				((IRestartable)this.output).DisableRestart();
			}
		}

		void IReusable.Initialize(object newSourceOrDestination)
		{
			((IReusable)this.parser).Initialize(newSourceOrDestination);
			((IReusable)this.output).Initialize(newSourceOrDestination);
			this.Reinitialize();
			this.parser.SetRestartConsumer(this);
		}

		public void Initialize(string fragment, bool preformatedText)
		{
			if (this.normalizedInput)
			{
				((HtmlNormalizingParser)this.parser).Initialize(fragment, preformatedText);
			}
			else
			{
				((HtmlParser)this.parser).Initialize(fragment, preformatedText);
			}
			if (!this.convertFragment)
			{
				((IReusable)this.output).Initialize(null);
			}
			this.Reinitialize();
		}

		private readonly bool outputImageLinks;

		private readonly bool outputAnchorLinks;

		private readonly bool shouldUseNarrowGapForPTagHtmlToTextConversion;

		private bool convertFragment;

		private IHtmlParser parser;

		private bool endOfFile;

		private TextOutput output;

		private HtmlToken token;

		private bool treatNbspAsBreakable;

		protected bool normalizedInput;

		private HtmlToTextConverter.NormalizerContext normalizerContext;

		private TextMapping textMapping;

		private bool lineStarted;

		private bool wideGap;

		private bool nextParagraphCloseWideGap = true;

		private bool afterFirstParagraph;

		private bool ignoreNextP;

		private int listLevel;

		private int listIndex;

		private bool listOrdered;

		private bool insideComment;

		private bool insidePre;

		private bool insideAnchor;

		private ScratchBuffer urlScratch;

		private int imageHeightPixels;

		private int imageWidthPixels;

		private ScratchBuffer imageAltText;

		private ScratchBuffer scratch;

		private Injection injection;

		private UrlCompareSink urlCompareSink;

		private struct NormalizerContext
		{
			public char LastCh;

			public bool OneNL;

			public bool HasSpace;

			public bool EatSpace;
		}
	}
}
