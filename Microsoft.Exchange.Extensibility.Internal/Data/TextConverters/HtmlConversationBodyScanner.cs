using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.TextConverters.Internal.Format;
using Microsoft.Exchange.Data.TextConverters.Internal.Html;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal class HtmlConversationBodyScanner : ConversationBodyScanner
	{
		public Encoding InputEncoding
		{
			get
			{
				return this.inputEncoding;
			}
			set
			{
				base.AssertNotLocked();
				this.inputEncoding = value;
			}
		}

		public bool DetectEncodingFromByteOrderMark
		{
			get
			{
				return this.detectEncodingFromByteOrderMark;
			}
			set
			{
				base.AssertNotLocked();
				this.detectEncodingFromByteOrderMark = value;
			}
		}

		public bool DetectEncodingFromMetaTag
		{
			get
			{
				return this.detectEncodingFromMetaTag;
			}
			set
			{
				base.AssertNotLocked();
				this.detectEncodingFromMetaTag = value;
			}
		}

		internal HtmlConversationBodyScanner SetInputEncoding(Encoding value)
		{
			this.InputEncoding = value;
			return this;
		}

		internal HtmlConversationBodyScanner SetDetectEncodingFromByteOrderMark(bool value)
		{
			this.DetectEncodingFromByteOrderMark = value;
			return this;
		}

		internal HtmlConversationBodyScanner SetDetectEncodingFromMetaTag(bool value)
		{
			this.DetectEncodingFromMetaTag = value;
			return this;
		}

		internal HtmlConversationBodyScanner SetInputStreamBufferSize(int value)
		{
			base.InputStreamBufferSize = value;
			return this;
		}

		internal HtmlConversationBodyScanner SetFilterHtml(bool value)
		{
			base.FilterHtml = value;
			return this;
		}

		internal HtmlConversationBodyScanner SetHtmlTagCallback(HtmlTagCallback value)
		{
			base.HtmlTagCallback = value;
			return this;
		}

		internal HtmlConversationBodyScanner SetTestBoundaryConditions(bool value)
		{
			base.TestBoundaryConditions = value;
			if (value)
			{
				this.testMaxHtmlTagSize = 123;
				this.testMaxHtmlTagAttributes = 5;
				this.testMaxHtmlNormalizerNesting = 10;
			}
			return this;
		}

		internal HtmlConversationBodyScanner SetTestMaxTokenRuns(int value)
		{
			this.testMaxTokenRuns = value;
			return this;
		}

		internal HtmlConversationBodyScanner SetTestTraceStream(Stream value)
		{
			this.testTraceStream = value;
			return this;
		}

		internal HtmlConversationBodyScanner SetTestTraceShowTokenNum(bool value)
		{
			this.testTraceShowTokenNum = value;
			return this;
		}

		internal HtmlConversationBodyScanner SetTestTraceStopOnTokenNum(int value)
		{
			this.testTraceStopOnTokenNum = value;
			return this;
		}

		internal HtmlConversationBodyScanner SetTestNormalizerTraceStream(Stream value)
		{
			this.testNormalizerTraceStream = value;
			return this;
		}

		internal HtmlConversationBodyScanner SetTestNormalizerTraceShowTokenNum(bool value)
		{
			this.testNormalizerTraceShowTokenNum = value;
			return this;
		}

		internal HtmlConversationBodyScanner SetTestNormalizerTraceStopOnTokenNum(int value)
		{
			this.testNormalizerTraceStopOnTokenNum = value;
			return this;
		}

		internal HtmlConversationBodyScanner SetTestFormatTraceStream(Stream value)
		{
			this.TestFormatTraceStream = value;
			return this;
		}

		internal HtmlConversationBodyScanner SetTestFormatConverterTraceStream(Stream value)
		{
			this.TestFormatConverterTraceStream = value;
			return this;
		}

		internal HtmlConversationBodyScanner SetTestFormatOutputTraceStream(Stream value)
		{
			this.TestFormatOutputTraceStream = value;
			return this;
		}

		internal HtmlConversationBodyScanner SetTestMaxHtmlTagSize(int value)
		{
			this.testMaxHtmlTagSize = value;
			return this;
		}

		internal HtmlConversationBodyScanner SetTestMaxHtmlTagAttributes(int value)
		{
			this.testMaxHtmlTagAttributes = value;
			return this;
		}

		internal HtmlConversationBodyScanner SetTestMaxHtmlRestartOffset(int value)
		{
			this.testMaxHtmlRestartOffset = value;
			return this;
		}

		internal HtmlConversationBodyScanner SetTestMaxHtmlNormalizerNesting(int value)
		{
			this.testMaxHtmlNormalizerNesting = value;
			return this;
		}

		internal override FormatConverter CreatePullChain(Stream sourceStream, IProgressMonitor progressMonitor)
		{
			ConverterInput input = new ConverterDecodingInput(sourceStream, false, this.inputEncoding, this.detectEncodingFromByteOrderMark, this.testMaxHtmlTagSize, this.testMaxHtmlRestartOffset, base.InputStreamBufferSize, base.TestBoundaryConditions, this as IResultsFeedback, progressMonitor);
			return this.CreateChain(input, progressMonitor);
		}

		internal override FormatConverter CreatePullChain(TextReader sourceReader, IProgressMonitor progressMonitor)
		{
			this.inputEncoding = Encoding.Unicode;
			ConverterInput input = new ConverterUnicodeInput(sourceReader, false, this.testMaxHtmlTagSize, base.TestBoundaryConditions, progressMonitor);
			return this.CreateChain(input, progressMonitor);
		}

		private FormatConverter CreateChain(ConverterInput input, IProgressMonitor progressMonitor)
		{
			this.Locked = true;
			HtmlParser parser = new HtmlParser(input, this.detectEncodingFromMetaTag, false, this.testMaxTokenRuns, this.testMaxHtmlTagAttributes, base.TestBoundaryConditions);
			HtmlNormalizingParser parser2 = new HtmlNormalizingParser(parser, null, false, this.testMaxHtmlNormalizerNesting, base.TestBoundaryConditions, this.testNormalizerTraceStream, this.testNormalizerTraceShowTokenNum, this.testNormalizerTraceStopOnTokenNum);
			FormatStore formatStore = new FormatStore();
			return new HtmlFormatConverter(parser2, formatStore, false, false, this.testTraceStream, this.testTraceShowTokenNum, this.testTraceStopOnTokenNum, this.TestFormatConverterTraceStream, progressMonitor);
		}

		private Encoding inputEncoding;

		private bool detectEncodingFromByteOrderMark = true;

		private bool detectEncodingFromMetaTag = true;

		private int testMaxTokenRuns = 512;

		private Stream testTraceStream;

		private bool testTraceShowTokenNum = true;

		private int testTraceStopOnTokenNum;

		internal Stream TestFormatConverterTraceStream;

		private Stream testNormalizerTraceStream;

		private bool testNormalizerTraceShowTokenNum = true;

		private int testNormalizerTraceStopOnTokenNum;

		private int testMaxHtmlTagSize = 4096;

		private int testMaxHtmlTagAttributes = 64;

		private int testMaxHtmlRestartOffset = 4096;

		private int testMaxHtmlNormalizerNesting = 4096;
	}
}
