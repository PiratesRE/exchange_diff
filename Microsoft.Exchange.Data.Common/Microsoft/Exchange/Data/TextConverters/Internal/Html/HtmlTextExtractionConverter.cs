using System;
using System.IO;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Html
{
	internal class HtmlTextExtractionConverter : IProducerConsumer, IRestartable, IDisposable
	{
		public HtmlTextExtractionConverter(IHtmlParser parser, ConverterOutput output, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum)
		{
			this.output = output;
			this.parser = parser;
			this.parser.SetRestartConsumer(this);
		}

		public bool CanRestart()
		{
			return ((IRestartable)this.output).CanRestart();
		}

		public void Restart()
		{
			((IRestartable)this.output).Restart();
			this.endOfFile = false;
			this.insideComment = false;
		}

		public void DisableRestart()
		{
			((IRestartable)this.output).DisableRestart();
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
			this.parser = null;
			this.output = null;
			GC.SuppressFinalize(this);
		}

		private void Process(HtmlTokenId tokenId)
		{
			HtmlToken token = this.parser.Token;
			switch (tokenId)
			{
			case HtmlTokenId.EndOfFile:
				this.output.Write("\r\n");
				this.output.Flush();
				this.endOfFile = true;
				break;
			case HtmlTokenId.Text:
				if (!this.insideComment)
				{
					token.Text.WriteToAndCollapseWhitespace(this.output, ref this.collapseWhitespaceState);
					return;
				}
				break;
			case HtmlTokenId.EncodingChange:
			{
				ConverterEncodingOutput converterEncodingOutput = this.output as ConverterEncodingOutput;
				if (converterEncodingOutput != null && converterEncodingOutput.CodePageSameAsInput)
				{
					converterEncodingOutput.Encoding = token.TokenEncoding;
					return;
				}
				break;
			}
			case HtmlTokenId.Tag:
			{
				if (token.IsTagBegin)
				{
					switch (token.TagIndex)
					{
					case HtmlTagIndex.A:
						if (!token.IsEndTag)
						{
						}
						break;
					case HtmlTagIndex.Address:
					case HtmlTagIndex.BlockQuote:
					case HtmlTagIndex.BR:
					case HtmlTagIndex.Caption:
					case HtmlTagIndex.Center:
					case HtmlTagIndex.Col:
					case HtmlTagIndex.ColGroup:
					case HtmlTagIndex.DD:
					case HtmlTagIndex.Dir:
					case HtmlTagIndex.Div:
					case HtmlTagIndex.DL:
					case HtmlTagIndex.DT:
					case HtmlTagIndex.FieldSet:
					case HtmlTagIndex.Form:
					case HtmlTagIndex.H1:
					case HtmlTagIndex.H2:
					case HtmlTagIndex.H3:
					case HtmlTagIndex.H4:
					case HtmlTagIndex.H5:
					case HtmlTagIndex.H6:
					case HtmlTagIndex.HR:
					case HtmlTagIndex.LI:
					case HtmlTagIndex.Listing:
					case HtmlTagIndex.Map:
					case HtmlTagIndex.Marquee:
					case HtmlTagIndex.Menu:
					case HtmlTagIndex.OL:
					case HtmlTagIndex.OptGroup:
					case HtmlTagIndex.Option:
					case HtmlTagIndex.P:
					case HtmlTagIndex.PlainText:
					case HtmlTagIndex.Pre:
					case HtmlTagIndex.Select:
					case HtmlTagIndex.Table:
					case HtmlTagIndex.Tbody:
					case HtmlTagIndex.TC:
					case HtmlTagIndex.Tfoot:
					case HtmlTagIndex.Thead:
					case HtmlTagIndex.TR:
					case HtmlTagIndex.UL:
						this.collapseWhitespaceState = CollapseWhitespaceState.NewLine;
						break;
					case HtmlTagIndex.Comment:
					case HtmlTagIndex.Script:
					case HtmlTagIndex.Style:
						this.insideComment = !token.IsEndTag;
						break;
					case HtmlTagIndex.NoEmbed:
					case HtmlTagIndex.NoFrames:
						this.insideComment = !token.IsEndTag;
						break;
					case HtmlTagIndex.TD:
					case HtmlTagIndex.TH:
						if (!token.IsEndTag)
						{
							this.output.Write("\t");
						}
						break;
					}
				}
				HtmlTagIndex tagIndex = token.TagIndex;
				if (tagIndex != HtmlTagIndex.A)
				{
					if (tagIndex != HtmlTagIndex.Area)
					{
						switch (tagIndex)
						{
						case HtmlTagIndex.Image:
						case HtmlTagIndex.Img:
							if (!token.IsEndTag)
							{
								return;
							}
							break;
						default:
							return;
						}
					}
					else if (!token.IsEndTag)
					{
						return;
					}
				}
				else if (!token.IsEndTag)
				{
					return;
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

		private const bool OutputAnchorLinks = true;

		private IHtmlParser parser;

		private bool endOfFile;

		private ConverterOutput output;

		private bool insideComment;

		private CollapseWhitespaceState collapseWhitespaceState;
	}
}
