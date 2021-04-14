using System;
using System.IO;
using Microsoft.Exchange.Data.TextConverters.Internal.Format;
using Microsoft.Exchange.Data.TextConverters.Internal.Html;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Enriched
{
	internal class EnrichedFormatConverter : FormatConverter, IProducerConsumer, IDisposable
	{
		public EnrichedFormatConverter(EnrichedParser parser, FormatOutput output, Injection injection, bool testTreatNbspAsBreakable, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum, Stream formatConverterTraceStream) : base(formatConverterTraceStream)
		{
			this.treatNbspAsBreakable = testTreatNbspAsBreakable;
			this.output = output;
			if (this.output != null)
			{
				this.output.Initialize(this.Store, SourceFormat.Rtf, "converted from text/enriched");
			}
			this.parser = parser;
			this.injection = injection;
			base.InitializeDocument();
			if (this.injection != null)
			{
				bool haveHead = this.injection.HaveHead;
			}
		}

		public override void Run()
		{
			if (!base.EndOfFile)
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
			if (!base.EndOfFile)
			{
				this.Run();
			}
			return base.EndOfFile;
		}

		private void Process(HtmlTokenId tokenId)
		{
			this.token = this.parser.Token;
			switch (tokenId)
			{
			case HtmlTokenId.EndOfFile:
				if (this.injection != null)
				{
					bool haveHead = this.injection.HaveHead;
				}
				base.CloseAllContainersAndSetEOF();
				if (this.output != null)
				{
					this.output.Flush();
				}
				break;
			case HtmlTokenId.Text:
				if (!this.insideParam)
				{
					this.tagPendingParameter = HtmlNameIndex._NOTANAME;
					this.OutputText(this.token);
					return;
				}
				if (this.tagPendingParameter != HtmlNameIndex._NOTANAME)
				{
					this.scratch.AppendTokenText(this.token, 256);
					return;
				}
				break;
			case HtmlTokenId.EncodingChange:
				if (this.output != null && this.output.OutputCodePageSameAsInput)
				{
					this.output.OutputEncoding = this.token.TokenEncoding;
					return;
				}
				break;
			case HtmlTokenId.Tag:
				if (this.token.IsTagBegin)
				{
					if (this.token.IsEndTag)
					{
						if (this.token.NameIndex == HtmlNameIndex.Param)
						{
							if (this.insideParam)
							{
								HtmlNameIndex htmlNameIndex = this.tagPendingParameter;
								if (htmlNameIndex <= HtmlNameIndex.ParaIndent)
								{
									if (htmlNameIndex != HtmlNameIndex.Color)
									{
										if (htmlNameIndex == HtmlNameIndex.ParaIndent)
										{
											BufferString bufferString = this.scratch.BufferString;
											bufferString.TrimWhitespace();
											int num = 0;
											int num2 = 0;
											int num3 = 0;
											int num4 = 0;
											int num6;
											for (int num5 = 0; num5 != bufferString.Length; num5 = num6)
											{
												int i = num5;
												num6 = num5;
												while (i < bufferString.Length)
												{
													if (bufferString[i] == ',')
													{
														break;
													}
													i++;
													num6++;
												}
												while (i > num5 && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(bufferString[i - 1])))
												{
													i--;
												}
												if (num6 < bufferString.Length)
												{
													do
													{
														num6++;
													}
													while (num6 < bufferString.Length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(bufferString[num6])));
												}
												BufferString bufferString2 = bufferString.SubString(num5, i - num5);
												if (bufferString2.EqualsToLowerCaseStringIgnoreCase("left"))
												{
													num++;
												}
												else if (bufferString2.EqualsToLowerCaseStringIgnoreCase("right"))
												{
													num2++;
												}
												else if (bufferString2.EqualsToLowerCaseStringIgnoreCase("in"))
												{
													num3++;
												}
												else if (bufferString2.EqualsToLowerCaseStringIgnoreCase("out"))
												{
													num4++;
												}
											}
											if (num + num4 != 0)
											{
												base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.LeftPadding, new PropertyValue(LengthUnits.Points, 30 * (num + num4) - ((this.indentLevel == 0) ? 12 : 0)));
											}
											if (num3 - num4 != 0)
											{
												if (num3 - num4 > 0)
												{
													base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.FirstLineIndent, new PropertyValue(LengthUnits.Points, 30 * (num3 - num4) - ((this.indentLevel == 0 && num + num4 == 0) ? 12 : 0)));
												}
												else
												{
													base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.FirstLineIndent, new PropertyValue(LengthUnits.Points, 30 * (num3 - num4) + ((this.indentLevel == 0 && num4 - num3 == num + num4) ? 12 : 0)));
												}
											}
											if (num2 != 0)
											{
												base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.RightPadding, new PropertyValue(LengthUnits.Points, 30 * num2));
											}
											if (num + num4 != 0 && this.indentLevel == 0)
											{
												this.indentLevel++;
											}
										}
									}
									else
									{
										PropertyValue value = HtmlSupport.ParseColor(this.scratch.BufferString, true, false);
										if (value.IsColor)
										{
											if (value.Color.Red > 250U && value.Color.Green > 250U && value.Color.Blue > 250U)
											{
												value = new PropertyValue(PropertyType.Color, 14737632U);
											}
											base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.FontColor, value);
										}
									}
								}
								else if (htmlNameIndex != HtmlNameIndex.Excerpt)
								{
									if (htmlNameIndex != HtmlNameIndex.Lang)
									{
										if (htmlNameIndex == HtmlNameIndex.FontFamily)
										{
											PropertyValue value = HtmlSupport.ParseFontFace(this.scratch.BufferString, this);
											if (!value.IsNull)
											{
												base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.FontFace, value);
											}
										}
									}
									else
									{
										PropertyValue value = HtmlSupport.ParseLanguage(this.scratch.BufferString, null);
										if (!value.IsNull)
										{
											base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.Language, value);
										}
									}
								}
								this.insideParam = false;
							}
						}
						else if (this.token.NameIndex != HtmlNameIndex.Unknown)
						{
							if (this.token.NameIndex == HtmlNameIndex.ParaIndent && this.indentLevel != 0)
							{
								this.indentLevel--;
							}
							base.CloseContainer(this.token.NameIndex);
						}
						this.tagPendingParameter = HtmlNameIndex._NOTANAME;
						return;
					}
					HtmlNameIndex nameIndex = this.token.NameIndex;
					if (nameIndex <= HtmlNameIndex.ParaIndent)
					{
						if (nameIndex <= HtmlNameIndex.Param)
						{
							if (nameIndex == HtmlNameIndex.Nofill)
							{
								base.OpenContainer(FormatContainerType.Block, false, 2, base.GetStyle(14), this.token.NameIndex);
								this.tagPendingParameter = HtmlNameIndex._NOTANAME;
								return;
							}
							if (nameIndex == HtmlNameIndex.FlushRight)
							{
								base.OpenContainer(FormatContainerType.Block, false, 2, FormatStyle.Null, this.token.NameIndex);
								base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.TextAlignment, new PropertyValue(TextAlign.Right));
								this.tagPendingParameter = HtmlNameIndex._NOTANAME;
								return;
							}
							if (nameIndex == HtmlNameIndex.Param)
							{
								this.insideParam = true;
								this.scratch.Reset();
								return;
							}
						}
						else if (nameIndex <= HtmlNameIndex.Italic)
						{
							if (nameIndex == HtmlNameIndex.Color)
							{
								base.OpenContainer(FormatContainerType.PropertyContainer, false, 2, FormatStyle.Null, this.token.NameIndex);
								this.tagPendingParameter = HtmlNameIndex.Color;
								return;
							}
							if (nameIndex == HtmlNameIndex.Italic)
							{
								base.OpenContainer(FormatContainerType.PropertyContainer, false, 2, base.GetStyle(4), this.token.NameIndex);
								this.tagPendingParameter = HtmlNameIndex._NOTANAME;
								return;
							}
						}
						else
						{
							switch (nameIndex)
							{
							case HtmlNameIndex.Center:
								base.OpenContainer(FormatContainerType.Block, false, 2, base.GetStyle(13), this.token.NameIndex);
								this.tagPendingParameter = HtmlNameIndex._NOTANAME;
								return;
							case HtmlNameIndex.Height:
								break;
							case HtmlNameIndex.Underline:
								base.OpenContainer(FormatContainerType.PropertyContainer, false, 2, base.GetStyle(5), this.token.NameIndex);
								this.tagPendingParameter = HtmlNameIndex._NOTANAME;
								return;
							case HtmlNameIndex.FlushBoth:
								base.OpenContainer(FormatContainerType.Block, false, 2, FormatStyle.Null, this.token.NameIndex);
								base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.TextAlignment, new PropertyValue(TextAlign.Justify));
								this.tagPendingParameter = HtmlNameIndex._NOTANAME;
								return;
							default:
								if (nameIndex == HtmlNameIndex.ParaIndent)
								{
									base.OpenContainer(FormatContainerType.Block, false, 2, FormatStyle.Null, this.token.NameIndex);
									this.tagPendingParameter = HtmlNameIndex.ParaIndent;
									if (this.indentLevel != 0)
									{
										this.indentLevel++;
										return;
									}
									return;
								}
								break;
							}
						}
					}
					else if (nameIndex <= HtmlNameIndex.FlushLeft)
					{
						if (nameIndex == HtmlNameIndex.Fixed)
						{
							base.OpenContainer(FormatContainerType.PropertyContainer, false, 2, base.GetStyle(10), this.token.NameIndex);
							this.tagPendingParameter = HtmlNameIndex._NOTANAME;
							return;
						}
						if (nameIndex == HtmlNameIndex.Smaller)
						{
							base.OpenContainer(FormatContainerType.PropertyContainer, false, 2, FormatStyle.Null, this.token.NameIndex);
							base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.FontSize, new PropertyValue(PropertyType.RelHtmlFontUnits, -1));
							this.tagPendingParameter = HtmlNameIndex._NOTANAME;
							return;
						}
						switch (nameIndex)
						{
						case HtmlNameIndex.Bold:
							base.OpenContainer(FormatContainerType.PropertyContainer, false, 2, base.GetStyle(1), this.token.NameIndex);
							this.tagPendingParameter = HtmlNameIndex._NOTANAME;
							return;
						case HtmlNameIndex.FlushLeft:
							base.OpenContainer(FormatContainerType.Block, false, 2, FormatStyle.Null, this.token.NameIndex);
							base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.TextAlignment, new PropertyValue(TextAlign.Left));
							this.tagPendingParameter = HtmlNameIndex._NOTANAME;
							return;
						}
					}
					else if (nameIndex <= HtmlNameIndex.Lang)
					{
						if (nameIndex == HtmlNameIndex.Excerpt)
						{
							base.OpenContainer(FormatContainerType.Block, false, 2, FormatStyle.Null, this.token.NameIndex);
							base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.QuotingLevelDelta, new PropertyValue(PropertyType.Integer, 1));
							this.tagPendingParameter = HtmlNameIndex.Excerpt;
							return;
						}
						if (nameIndex == HtmlNameIndex.Lang)
						{
							base.OpenContainer(FormatContainerType.PropertyContainer, false, 2, base.GetStyle(6), this.token.NameIndex);
							this.tagPendingParameter = HtmlNameIndex.Lang;
							return;
						}
					}
					else
					{
						if (nameIndex == HtmlNameIndex.Bigger)
						{
							base.OpenContainer(FormatContainerType.PropertyContainer, false, 2, FormatStyle.Null, this.token.NameIndex);
							base.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.FontSize, new PropertyValue(PropertyType.RelHtmlFontUnits, 1));
							this.tagPendingParameter = HtmlNameIndex._NOTANAME;
							return;
						}
						if (nameIndex == HtmlNameIndex.FontFamily)
						{
							base.OpenContainer(FormatContainerType.PropertyContainer, false, 2, FormatStyle.Null, this.token.NameIndex);
							this.tagPendingParameter = HtmlNameIndex.FontFamily;
							return;
						}
					}
					this.tagPendingParameter = HtmlNameIndex._NOTANAME;
					return;
				}
				break;
			case HtmlTokenId.Restart:
			case HtmlTokenId.OverlappedClose:
			case HtmlTokenId.OverlappedReopen:
				break;
			default:
				return;
			}
		}

		private void OutputText(HtmlToken token)
		{
			foreach (TokenRun tokenRun in token.Runs)
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
								base.AddLineBreak(1);
								continue;
							}
							if (textType == RunTextType.Tabulation)
							{
								base.AddTabulation(tokenRun.Length);
								continue;
							}
						}
						base.AddSpace(tokenRun.Length);
					}
					else if (tokenRun.TextType == RunTextType.Nbsp)
					{
						if (this.treatNbspAsBreakable)
						{
							base.AddSpace(tokenRun.Length);
						}
						else
						{
							base.AddNbsp(tokenRun.Length);
						}
					}
					else if (tokenRun.IsLiteral)
					{
						base.AddNonSpaceText(this.literalBuffer, 0, tokenRun.ReadLiteral(this.literalBuffer));
					}
					else
					{
						base.AddNonSpaceText(tokenRun.RawBuffer, tokenRun.RawOffset, tokenRun.RawLength);
					}
				}
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.parser != null)
				{
					((IDisposable)this.parser).Dispose();
				}
				if (this.output != null && this.output != null)
				{
					((IDisposable)this.output).Dispose();
				}
				if (this.token != null && this.token is IDisposable)
				{
					((IDisposable)this.token).Dispose();
				}
			}
			this.parser = null;
			this.output = null;
			this.token = null;
			this.literalBuffer = null;
		}

		public const int PointsPerIndent = 30;

		public const int LeftIndentIncrementPoints = 12;

		public const int MaxIndent = 50;

		private EnrichedParser parser;

		private FormatOutput output;

		private HtmlToken token;

		private bool treatNbspAsBreakable;

		private bool insideParam;

		private HtmlNameIndex tagPendingParameter;

		private int indentLevel;

		private ScratchBuffer scratch;

		private char[] literalBuffer = new char[2];

		private Injection injection;
	}
}
