using System;
using System.IO;
using Microsoft.Exchange.Data.TextConverters.Internal.Format;
using Microsoft.Exchange.Data.TextConverters.Internal.Html;
using Microsoft.Exchange.Data.TextConverters.Internal.Rtf;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal class RtfConversationBodyScanner : ConversationBodyScanner
	{
		public bool EnableHtmlDeencapsulation
		{
			get
			{
				return this.enableHtmlDeencapsulation;
			}
			set
			{
				base.AssertNotLocked();
				this.enableHtmlDeencapsulation = value;
			}
		}

		internal RtfConversationBodyScanner SetInputStreamBufferSize(int value)
		{
			base.InputStreamBufferSize = value;
			return this;
		}

		internal RtfConversationBodyScanner SetFilterHtml(bool value)
		{
			base.FilterHtml = value;
			return this;
		}

		internal RtfConversationBodyScanner SetHtmlTagCallback(HtmlTagCallback value)
		{
			base.HtmlTagCallback = value;
			return this;
		}

		internal RtfConversationBodyScanner SetTestBoundaryConditions(bool value)
		{
			base.TestBoundaryConditions = value;
			if (value)
			{
				this.TestMaxHtmlTagSize = 123;
				this.TestMaxHtmlTagAttributes = 5;
				this.TestMaxHtmlNormalizerNesting = 10;
			}
			return this;
		}

		internal RtfConversationBodyScanner SetTestTraceStream(Stream value)
		{
			this.TestTraceStream = value;
			return this;
		}

		internal RtfConversationBodyScanner SetTestTraceShowTokenNum(bool value)
		{
			this.TestTraceShowTokenNum = value;
			return this;
		}

		internal RtfConversationBodyScanner SetTestTraceStopOnTokenNum(int value)
		{
			this.TestTraceStopOnTokenNum = value;
			return this;
		}

		internal RtfConversationBodyScanner SetTestFormatTraceStream(Stream value)
		{
			this.TestFormatTraceStream = value;
			return this;
		}

		internal RtfConversationBodyScanner SetTestFormatConverterTraceStream(Stream value)
		{
			this.TestFormatConverterTraceStream = value;
			return this;
		}

		internal RtfConversationBodyScanner SetTestFormatOutputTraceStream(Stream value)
		{
			this.TestFormatOutputTraceStream = value;
			return this;
		}

		internal RtfConversationBodyScanner SetTestHtmlTraceStream(Stream value)
		{
			this.TestHtmlTraceStream = value;
			return this;
		}

		internal RtfConversationBodyScanner SetTestHtmlTraceShowTokenNum(bool value)
		{
			this.TestHtmlTraceShowTokenNum = value;
			return this;
		}

		internal RtfConversationBodyScanner SetTestHtmlTraceStopOnTokenNum(int value)
		{
			this.TestHtmlTraceStopOnTokenNum = value;
			return this;
		}

		internal RtfConversationBodyScanner SetTestNormalizerTraceStream(Stream value)
		{
			this.TestNormalizerTraceStream = value;
			return this;
		}

		internal RtfConversationBodyScanner SetTestNormalizerTraceShowTokenNum(bool value)
		{
			this.TestNormalizerTraceShowTokenNum = value;
			return this;
		}

		internal RtfConversationBodyScanner SetTestNormalizerTraceStopOnTokenNum(int value)
		{
			this.TestNormalizerTraceStopOnTokenNum = value;
			return this;
		}

		internal RtfConversationBodyScanner SetTestMaxTokenRuns(int value)
		{
			this.TestMaxTokenRuns = value;
			return this;
		}

		internal RtfConversationBodyScanner SetTestMaxHtmlTagAttributes(int value)
		{
			this.TestMaxHtmlTagAttributes = value;
			return this;
		}

		internal RtfConversationBodyScanner SetTestMaxHtmlNormalizerNesting(int value)
		{
			this.TestMaxHtmlNormalizerNesting = value;
			return this;
		}

		internal override FormatConverter CreatePullChain(Stream sourceStream, IProgressMonitor progressMonitor)
		{
			this.Locked = true;
			this.reportBytes = new ReportBytes();
			RtfParser parser;
			if (this.enableHtmlDeencapsulation)
			{
				RtfPreviewStream rtfPreviewStream = sourceStream as RtfPreviewStream;
				if (rtfPreviewStream == null || rtfPreviewStream.Parser == null || rtfPreviewStream.InternalPosition != 0 || rtfPreviewStream.InputRtfStream == null)
				{
					rtfPreviewStream = new RtfPreviewStream(sourceStream, base.InputStreamBufferSize);
				}
				parser = new RtfParser(rtfPreviewStream.InputRtfStream, base.InputStreamBufferSize, base.TestBoundaryConditions, progressMonitor, rtfPreviewStream.Parser, this.reportBytes);
				if (rtfPreviewStream.Encapsulation == RtfEncapsulation.Html)
				{
					return this.CreateDeencapsulationChain(parser, progressMonitor);
				}
			}
			else
			{
				parser = new RtfParser(sourceStream, false, base.InputStreamBufferSize, base.TestBoundaryConditions, progressMonitor, this.reportBytes);
			}
			FormatStore formatStore = new FormatStore();
			return new RtfFormatConverter(parser, formatStore, false, this.TestTraceStream, this.TestTraceShowTokenNum, this.TestTraceStopOnTokenNum, this.TestFormatConverterTraceStream);
		}

		internal override FormatConverter CreatePullChain(TextReader sourceReader, IProgressMonitor progressMonitor)
		{
			throw new NotSupportedException("RTF input must be a stream");
		}

		internal FormatConverter CreateDeencapsulationChain(RtfParser parser, IProgressMonitor progressMonitor)
		{
			HtmlInRtfExtractingInput input = new HtmlInRtfExtractingInput(parser, this.TestMaxHtmlTagSize, base.TestBoundaryConditions, this.TestTraceStream, this.TestTraceShowTokenNum, this.TestTraceStopOnTokenNum);
			HtmlParser parser2 = new HtmlParser(input, false, false, this.TestMaxTokenRuns, this.TestMaxHtmlTagAttributes, base.TestBoundaryConditions);
			HtmlNormalizingParser parser3 = new HtmlNormalizingParser(parser2, null, false, this.TestMaxHtmlNormalizerNesting, base.TestBoundaryConditions, this.TestNormalizerTraceStream, this.TestNormalizerTraceShowTokenNum, this.TestNormalizerTraceStopOnTokenNum);
			FormatStore formatStore = new FormatStore();
			return new HtmlFormatConverter(parser3, formatStore, false, false, this.TestHtmlTraceStream, this.TestHtmlTraceShowTokenNum, this.TestHtmlTraceStopOnTokenNum, this.TestFormatConverterTraceStream, progressMonitor);
		}

		private bool enableHtmlDeencapsulation = true;

		internal Stream TestTraceStream;

		internal bool TestTraceShowTokenNum = true;

		internal int TestTraceStopOnTokenNum;

		internal Stream TestFormatConverterTraceStream;

		internal Stream TestHtmlTraceStream;

		internal bool TestHtmlTraceShowTokenNum = true;

		internal int TestHtmlTraceStopOnTokenNum;

		internal Stream TestNormalizerTraceStream;

		internal bool TestNormalizerTraceShowTokenNum = true;

		internal int TestNormalizerTraceStopOnTokenNum;

		internal int TestMaxTokenRuns = 512;

		internal int TestMaxHtmlTagSize = 32768;

		internal int TestMaxHtmlTagAttributes = 64;

		internal int TestMaxHtmlNormalizerNesting = 4096;

		private ReportBytes reportBytes;
	}
}
