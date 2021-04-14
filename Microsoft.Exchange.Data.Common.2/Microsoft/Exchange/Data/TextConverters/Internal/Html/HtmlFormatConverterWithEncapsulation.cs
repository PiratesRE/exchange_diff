using System;
using System.IO;
using Microsoft.Exchange.Data.TextConverters.Internal.Format;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Html
{
	internal class HtmlFormatConverterWithEncapsulation : HtmlFormatConverter
	{
		public HtmlFormatConverterWithEncapsulation(HtmlNormalizingParser parser, FormatOutput output, bool testTreatNbspAsBreakable, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum, Stream formatConverterTraceStream, IProgressMonitor progressMonitor) : this(parser, output, false, testTreatNbspAsBreakable, traceStream, traceShowTokenNum, traceStopOnTokenNum, formatConverterTraceStream, progressMonitor)
		{
		}

		public HtmlFormatConverterWithEncapsulation(HtmlNormalizingParser parser, FormatOutput output, bool encapsulateMarkup, bool testTreatNbspAsBreakable, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum, Stream formatConverterTraceStream, IProgressMonitor progressMonitor) : base(parser, output, testTreatNbspAsBreakable, traceStream, traceShowTokenNum, traceStopOnTokenNum, formatConverterTraceStream, progressMonitor)
		{
			this.encapsulateMarkup = encapsulateMarkup;
			if (this.output != null && this.encapsulateMarkup)
			{
				this.output.Initialize(this.Store, SourceFormat.HtmlEncapsulateMarkup, "converted from html");
			}
		}

		protected override void Process(HtmlTokenId tokenId)
		{
			this.token = this.parser.Token;
			switch (tokenId)
			{
			case HtmlTokenId.EndOfFile:
				base.CloseAllContainersAndSetEOF();
				break;
			case HtmlTokenId.Text:
				if (this.insideStyle)
				{
					this.OutputEncapsulatedMarkup();
					this.token.Text.WriteTo(this.cssParserInput);
					return;
				}
				if (this.insideComment)
				{
					this.OutputEncapsulatedMarkup();
					return;
				}
				if (this.insidePre)
				{
					base.ProcessPreformatedText();
					return;
				}
				base.ProcessText();
				return;
			case HtmlTokenId.EncodingChange:
				if (this.output != null && this.output.OutputCodePageSameAsInput)
				{
					this.output.OutputEncoding = this.token.TokenEncoding;
					return;
				}
				break;
			case HtmlTokenId.Tag:
				this.OutputEncapsulatedMarkup();
				if (this.token.TagIndex <= HtmlTagIndex.Unknown)
				{
					if (this.insideStyle && this.token.TagIndex == HtmlTagIndex._COMMENT)
					{
						this.token.Text.WriteTo(this.cssParserInput);
						return;
					}
				}
				else
				{
					HtmlDtd.TagDefinition tagDefinition = HtmlFormatConverter.GetTagDefinition(this.token.TagIndex);
					if (!this.token.IsEndTag)
					{
						if (this.token.IsTagBegin)
						{
							base.PushElement(tagDefinition, this.token.IsEmptyScope);
						}
						base.ProcessStartTagAttributes(tagDefinition);
						return;
					}
					if (this.token.IsTagEnd)
					{
						base.PopElement(this.BuildStackTop - 1 - this.temporarilyClosedLevels, this.token.Argument != 1);
						return;
					}
				}
				break;
			case HtmlTokenId.Restart:
				break;
			case HtmlTokenId.OverlappedClose:
				this.temporarilyClosedLevels = this.token.Argument;
				return;
			case HtmlTokenId.OverlappedReopen:
				this.temporarilyClosedLevels = 0;
				return;
			default:
				return;
			}
		}

		private void OutputEncapsulatedMarkup()
		{
			if (this.encapsulateMarkup)
			{
				if (this.markupSink == null)
				{
					this.markupSink = new HtmlFormatConverterWithEncapsulation.MarkupSink(this);
				}
				if (this.token.IsEndTag && this.token.TagIndex > HtmlTagIndex.Unknown)
				{
					char[] endTagText = this.GetEndTagText(this.token);
					this.markupSink.Write(endTagText, 0, endTagText.Length);
					return;
				}
				this.token.Text.WriteTo(this.markupSink);
			}
		}

		private char[] GetEndTagText(HtmlToken htmlToken)
		{
			char[] array = this.token.NameIndex.ToString().ToCharArray();
			int num = array.Length;
			char[] array2 = new char[num + 3];
			array2[0] = '<';
			array2[1] = '/';
			array.CopyTo(array2, 2);
			array2[array2.Length - 1] = '>';
			return array2;
		}

		private readonly bool encapsulateMarkup;

		private HtmlFormatConverterWithEncapsulation.MarkupSink markupSink;

		private class MarkupSink : ITextSink
		{
			public MarkupSink(HtmlFormatConverter builder)
			{
				this.builder = builder;
				this.literalBuffer = new char[2];
			}

			public bool IsEnough
			{
				get
				{
					return false;
				}
			}

			public void Write(char[] buffer, int offset, int count)
			{
				this.builder.AddMarkupText(buffer, offset, count);
			}

			public void Write(int ucs32Char)
			{
				int num = Token.LiteralLength(ucs32Char);
				this.literalBuffer[0] = Token.LiteralFirstChar(ucs32Char);
				if (num > 1)
				{
					this.literalBuffer[1] = Token.LiteralLastChar(ucs32Char);
				}
				this.builder.AddMarkupText(this.literalBuffer, 0, num);
			}

			private HtmlFormatConverter builder;

			private char[] literalBuffer;
		}
	}
}
