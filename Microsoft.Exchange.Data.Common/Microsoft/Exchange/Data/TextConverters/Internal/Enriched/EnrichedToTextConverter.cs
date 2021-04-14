using System;
using System.IO;
using Microsoft.Exchange.Data.TextConverters.Internal.Html;
using Microsoft.Exchange.Data.TextConverters.Internal.Text;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Enriched
{
	internal class EnrichedToTextConverter : IProducerConsumer, IDisposable
	{
		public EnrichedToTextConverter(EnrichedParser parser, TextOutput output, Injection injection, bool testTreatNbspAsBreakable, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum)
		{
			this.treatNbspAsBreakable = testTreatNbspAsBreakable;
			this.output = output;
			this.parser = parser;
			this.injection = injection;
			this.output.OpenDocument();
			if (this.injection != null && this.injection.HaveHead)
			{
				this.injection.Inject(true, this.output);
			}
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
				if (this.injection != null && this.injection.HaveTail)
				{
					if (!this.output.LineEmpty)
					{
						this.output.OutputNewLine();
					}
					this.injection.Inject(false, this.output);
				}
				this.output.CloseDocument();
				this.output.Flush();
				this.endOfFile = true;
				break;
			case HtmlTokenId.Text:
				if (!this.insideParam)
				{
					this.OutputText(this.token);
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
				if (this.token.IsTagBegin)
				{
					HtmlNameIndex nameIndex = this.token.NameIndex;
					if (nameIndex <= HtmlNameIndex.ParaIndent)
					{
						if (nameIndex <= HtmlNameIndex.Param)
						{
							if (nameIndex != HtmlNameIndex.Nofill && nameIndex != HtmlNameIndex.FlushRight)
							{
								if (nameIndex != HtmlNameIndex.Param)
								{
									return;
								}
								this.insideParam = !this.token.IsEndTag;
								return;
							}
						}
						else if (nameIndex <= HtmlNameIndex.Italic)
						{
							if (nameIndex != HtmlNameIndex.Color && nameIndex != HtmlNameIndex.Italic)
							{
								return;
							}
							break;
						}
						else
						{
							switch (nameIndex)
							{
							case HtmlNameIndex.Center:
							case HtmlNameIndex.FlushBoth:
								break;
							case HtmlNameIndex.Height:
							case HtmlNameIndex.Underline:
								return;
							default:
								if (nameIndex != HtmlNameIndex.ParaIndent)
								{
									return;
								}
								break;
							}
						}
					}
					else if (nameIndex <= HtmlNameIndex.FlushLeft)
					{
						if (nameIndex == HtmlNameIndex.Fixed || nameIndex == HtmlNameIndex.Smaller)
						{
							break;
						}
						switch (nameIndex)
						{
						case HtmlNameIndex.Bold:
						case HtmlNameIndex.Strike:
							return;
						case HtmlNameIndex.FlushLeft:
							break;
						default:
							return;
						}
					}
					else if (nameIndex <= HtmlNameIndex.Lang)
					{
						if (nameIndex == HtmlNameIndex.Excerpt)
						{
							if (!this.output.LineEmpty)
							{
								this.output.OutputNewLine();
							}
							if (!this.token.IsEndTag)
							{
								this.quotingLevel++;
							}
							else
							{
								this.quotingLevel--;
							}
							this.output.SetQuotingLevel(this.quotingLevel);
							return;
						}
						if (nameIndex != HtmlNameIndex.Lang)
						{
							return;
						}
						break;
					}
					else
					{
						if (nameIndex != HtmlNameIndex.Bigger && nameIndex != HtmlNameIndex.FontFamily)
						{
							return;
						}
						break;
					}
					if (!this.output.LineEmpty)
					{
						this.output.OutputNewLine();
						return;
					}
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
								this.output.OutputNewLine();
								continue;
							}
							if (textType == RunTextType.Tabulation)
							{
								this.output.OutputTabulation(tokenRun.Length);
								continue;
							}
						}
						this.output.OutputSpace(tokenRun.Length);
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
						this.output.OutputNonspace(tokenRun.Literal, TextMapping.Unicode);
					}
					else
					{
						this.output.OutputNonspace(tokenRun.RawBuffer, tokenRun.RawOffset, tokenRun.RawLength, TextMapping.Unicode);
					}
				}
			}
		}

		void IDisposable.Dispose()
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
			this.parser = null;
			this.output = null;
			this.token = null;
			GC.SuppressFinalize(this);
		}

		private EnrichedParser parser;

		private bool endOfFile;

		private TextOutput output;

		private HtmlToken token;

		private bool treatNbspAsBreakable;

		private bool insideParam;

		private int quotingLevel;

		private Injection injection;
	}
}
