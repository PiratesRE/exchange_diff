using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.TextConverters.Internal.Html;
using Microsoft.Exchange.Data.TextConverters.Internal.Text;

namespace Microsoft.Exchange.Data.TextConverters
{
	public class HtmlToText : TextConverter
	{
		public HtmlToText()
		{
			this.mode = TextExtractionMode.NormalConversion;
		}

		public HtmlToText(TextExtractionMode mode)
		{
			this.mode = mode;
		}

		public TextExtractionMode TextExtractionMode
		{
			get
			{
				return this.mode;
			}
		}

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

		public bool NormalizeHtml
		{
			get
			{
				return this.normalizeInputHtml;
			}
			set
			{
				this.AssertNotLockedAndNotTextExtraction();
				this.normalizeInputHtml = value;
			}
		}

		public Encoding OutputEncoding
		{
			get
			{
				return this.outputEncoding;
			}
			set
			{
				base.AssertNotLocked();
				this.outputEncoding = value;
				this.outputEncodingSameAsInput = (value == null);
			}
		}

		public bool Wrap
		{
			get
			{
				return this.wrapFlowed;
			}
			set
			{
				this.AssertNotLockedAndNotTextExtraction();
				this.wrapFlowed = value;
			}
		}

		internal bool WrapDeleteSpace
		{
			get
			{
				return this.wrapDelSp;
			}
			set
			{
				this.AssertNotLockedAndNotTextExtraction();
				this.wrapDelSp = value;
			}
		}

		public bool HtmlEscapeOutput
		{
			get
			{
				return this.htmlEscape;
			}
			set
			{
				this.AssertNotLockedAndNotTextExtraction();
				this.htmlEscape = value;
				if (value)
				{
					this.fallbacks = true;
				}
			}
		}

		public HeaderFooterFormat HeaderFooterFormat
		{
			get
			{
				return this.injectionFormat;
			}
			set
			{
				this.AssertNotLockedAndNotTextExtraction();
				this.injectionFormat = value;
			}
		}

		public string Header
		{
			get
			{
				return this.injectHead;
			}
			set
			{
				this.AssertNotLockedAndNotTextExtraction();
				this.injectHead = value;
			}
		}

		public string Footer
		{
			get
			{
				return this.injectTail;
			}
			set
			{
				this.AssertNotLockedAndNotTextExtraction();
				this.injectTail = value;
			}
		}

		public bool ShouldUseNarrowGapForPTagHtmlToTextConversion { get; set; }

		public bool OutputAnchorLinks
		{
			get
			{
				return this.outputAnchorLinks;
			}
			set
			{
				this.outputAnchorLinks = value;
			}
		}

		public bool OutputImageLinks
		{
			get
			{
				return this.outputImageLinks;
			}
			set
			{
				this.outputImageLinks = value;
			}
		}

		internal HtmlToText SetInputEncoding(Encoding value)
		{
			this.InputEncoding = value;
			return this;
		}

		internal HtmlToText SetDetectEncodingFromByteOrderMark(bool value)
		{
			this.DetectEncodingFromByteOrderMark = value;
			return this;
		}

		internal HtmlToText SetDetectEncodingFromMetaTag(bool value)
		{
			this.DetectEncodingFromMetaTag = value;
			return this;
		}

		internal HtmlToText SetOutputEncoding(Encoding value)
		{
			this.OutputEncoding = value;
			return this;
		}

		internal HtmlToText SetNormalizeHtml(bool value)
		{
			this.NormalizeHtml = value;
			return this;
		}

		internal HtmlToText SetWrap(bool value)
		{
			this.Wrap = value;
			return this;
		}

		internal HtmlToText SetWrapDeleteSpace(bool value)
		{
			this.WrapDeleteSpace = value;
			return this;
		}

		internal HtmlToText SetUseFallbacks(bool value)
		{
			this.fallbacks = value;
			return this;
		}

		internal HtmlToText SetHtmlEscapeOutput(bool value)
		{
			this.HtmlEscapeOutput = value;
			return this;
		}

		internal HtmlToText SetHeaderFooterFormat(HeaderFooterFormat value)
		{
			this.HeaderFooterFormat = value;
			return this;
		}

		internal HtmlToText SetHeader(string value)
		{
			this.Header = value;
			return this;
		}

		internal HtmlToText SetFooter(string value)
		{
			this.Footer = value;
			return this;
		}

		internal HtmlToText SetInputStreamBufferSize(int value)
		{
			base.InputStreamBufferSize = value;
			return this;
		}

		internal HtmlToText SetImageRenderingCallback(ImageRenderingCallbackInternal value)
		{
			this.imageRenderingCallback = value;
			return this;
		}

		internal HtmlToText SetTestBoundaryConditions(bool value)
		{
			this.testBoundaryConditions = value;
			if (value)
			{
				this.testMaxHtmlTagSize = 123;
				this.testMaxHtmlTagAttributes = 5;
				this.testMaxHtmlNormalizerNesting = 10;
			}
			return this;
		}

		internal HtmlToText SetTestPreserveTrailingSpaces(bool value)
		{
			this.testPreserveTrailingSpaces = value;
			return this;
		}

		internal HtmlToText SetTestMaxTokenRuns(int value)
		{
			this.testMaxTokenRuns = value;
			return this;
		}

		internal HtmlToText SetTestTreatNbspAsBreakable(bool value)
		{
			this.testTreatNbspAsBreakable = value;
			return this;
		}

		internal HtmlToText SetTestTraceStream(Stream value)
		{
			this.testTraceStream = value;
			return this;
		}

		internal HtmlToText SetTestTraceShowTokenNum(bool value)
		{
			this.testTraceShowTokenNum = value;
			return this;
		}

		internal HtmlToText SetTestTraceStopOnTokenNum(int value)
		{
			this.testTraceStopOnTokenNum = value;
			return this;
		}

		internal HtmlToText SetTestNormalizerTraceStream(Stream value)
		{
			this.testNormalizerTraceStream = value;
			return this;
		}

		internal HtmlToText SetTestNormalizerTraceShowTokenNum(bool value)
		{
			this.testNormalizerTraceShowTokenNum = value;
			return this;
		}

		internal HtmlToText SetTestNormalizerTraceStopOnTokenNum(int value)
		{
			this.testNormalizerTraceStopOnTokenNum = value;
			return this;
		}

		internal HtmlToText SetTestMaxHtmlTagSize(int value)
		{
			this.testMaxHtmlTagSize = value;
			return this;
		}

		internal HtmlToText SetTestMaxHtmlTagAttributes(int value)
		{
			this.testMaxHtmlTagAttributes = value;
			return this;
		}

		internal HtmlToText SetTestMaxHtmlRestartOffset(int value)
		{
			this.testMaxHtmlRestartOffset = value;
			return this;
		}

		internal HtmlToText SetTestMaxHtmlNormalizerNesting(int value)
		{
			this.testMaxHtmlNormalizerNesting = value;
			return this;
		}

		internal HtmlToText SetTestFormatTraceStream(Stream value)
		{
			this.testFormatTraceStream = value;
			return this;
		}

		internal override IProducerConsumer CreatePushChain(ConverterStream converterStream, Stream output)
		{
			if (this.inputEncoding == null)
			{
				throw new InvalidOperationException(TextConvertersStrings.InputEncodingRequired);
			}
			ConverterInput input = new ConverterDecodingInput(converterStream, true, this.inputEncoding, this.detectEncodingFromByteOrderMark, this.testMaxHtmlTagSize, this.testMaxHtmlRestartOffset, base.InputStreamBufferSize, this.testBoundaryConditions, this, null);
			ConverterOutput output2 = new ConverterEncodingOutput(output, true, true, this.outputEncodingSameAsInput ? this.inputEncoding : this.outputEncoding, this.outputEncodingSameAsInput, this.testBoundaryConditions, this);
			return this.CreateChain(input, output2, converterStream);
		}

		internal override IProducerConsumer CreatePushChain(ConverterStream converterStream, TextWriter output)
		{
			if (this.inputEncoding == null)
			{
				throw new InvalidOperationException(TextConvertersStrings.InputEncodingRequired);
			}
			this.outputEncoding = Encoding.Unicode;
			ConverterInput input = new ConverterDecodingInput(converterStream, true, this.inputEncoding, this.detectEncodingFromByteOrderMark, this.testMaxHtmlTagSize, this.testMaxHtmlRestartOffset, base.InputStreamBufferSize, this.testBoundaryConditions, this, null);
			ConverterOutput output2 = new ConverterUnicodeOutput(output, true, true);
			return this.CreateChain(input, output2, converterStream);
		}

		internal override IProducerConsumer CreatePushChain(ConverterWriter converterWriter, Stream output)
		{
			this.inputEncoding = Encoding.Unicode;
			ConverterInput input = new ConverterUnicodeInput(converterWriter, true, this.testMaxHtmlTagSize, this.testBoundaryConditions, null);
			ConverterOutput output2 = new ConverterEncodingOutput(output, true, false, this.outputEncodingSameAsInput ? Encoding.UTF8 : this.outputEncoding, this.outputEncodingSameAsInput, this.testBoundaryConditions, this);
			return this.CreateChain(input, output2, converterWriter);
		}

		internal override IProducerConsumer CreatePushChain(ConverterWriter converterWriter, TextWriter output)
		{
			this.inputEncoding = Encoding.Unicode;
			this.outputEncoding = Encoding.Unicode;
			ConverterInput input = new ConverterUnicodeInput(converterWriter, true, this.testMaxHtmlTagSize, this.testBoundaryConditions, null);
			ConverterOutput output2 = new ConverterUnicodeOutput(output, true, false);
			return this.CreateChain(input, output2, converterWriter);
		}

		internal override IProducerConsumer CreatePullChain(Stream input, ConverterStream converterStream)
		{
			if (this.inputEncoding == null)
			{
				throw new InvalidOperationException(TextConvertersStrings.InputEncodingRequired);
			}
			ConverterInput input2 = new ConverterDecodingInput(input, false, this.inputEncoding, this.detectEncodingFromByteOrderMark, this.testMaxHtmlTagSize, this.testMaxHtmlRestartOffset, base.InputStreamBufferSize, this.testBoundaryConditions, this, converterStream);
			ConverterOutput output = new ConverterEncodingOutput(converterStream, false, true, this.outputEncodingSameAsInput ? this.inputEncoding : this.outputEncoding, this.outputEncodingSameAsInput, this.testBoundaryConditions, this);
			return this.CreateChain(input2, output, converterStream);
		}

		internal override IProducerConsumer CreatePullChain(TextReader input, ConverterStream converterStream)
		{
			this.inputEncoding = Encoding.Unicode;
			ConverterInput input2 = new ConverterUnicodeInput(input, false, this.testMaxHtmlTagSize, this.testBoundaryConditions, converterStream);
			ConverterOutput output = new ConverterEncodingOutput(converterStream, false, false, this.outputEncodingSameAsInput ? Encoding.UTF8 : this.outputEncoding, this.outputEncodingSameAsInput, this.testBoundaryConditions, this);
			return this.CreateChain(input2, output, converterStream);
		}

		internal override IProducerConsumer CreatePullChain(Stream input, ConverterReader converterReader)
		{
			if (this.inputEncoding == null)
			{
				throw new InvalidOperationException(TextConvertersStrings.InputEncodingRequired);
			}
			this.outputEncoding = Encoding.Unicode;
			ConverterInput input2 = new ConverterDecodingInput(input, false, this.inputEncoding, this.detectEncodingFromByteOrderMark, this.testMaxHtmlTagSize, this.testMaxHtmlRestartOffset, base.InputStreamBufferSize, this.testBoundaryConditions, this, converterReader);
			ConverterOutput output = new ConverterUnicodeOutput(converterReader, false, true);
			return this.CreateChain(input2, output, converterReader);
		}

		internal override IProducerConsumer CreatePullChain(TextReader input, ConverterReader converterReader)
		{
			this.inputEncoding = Encoding.Unicode;
			this.outputEncoding = Encoding.Unicode;
			ConverterInput input2 = new ConverterUnicodeInput(input, false, this.testMaxHtmlTagSize, this.testBoundaryConditions, converterReader);
			ConverterOutput output = new ConverterUnicodeOutput(converterReader, false, false);
			return this.CreateChain(input2, output, converterReader);
		}

		private IProducerConsumer CreateChain(ConverterInput input, ConverterOutput output, IProgressMonitor progressMonitor)
		{
			this.locked = true;
			HtmlParser htmlParser = new HtmlParser(input, this.detectEncodingFromMetaTag, false, this.testMaxTokenRuns, this.testMaxHtmlTagAttributes, this.testBoundaryConditions);
			if (this.mode == TextExtractionMode.ExtractText)
			{
				return new HtmlTextExtractionConverter(htmlParser, output, this.testTraceStream, this.testTraceShowTokenNum, this.testTraceStopOnTokenNum);
			}
			Injection injection = null;
			if (this.injectHead != null || this.injectTail != null)
			{
				injection = ((this.injectionFormat == HeaderFooterFormat.Html) ? new HtmlInjection(this.injectHead, this.injectTail, this.injectionFormat, false, null, this.testBoundaryConditions, null, progressMonitor) : new TextInjection(this.injectHead, this.injectTail, this.testBoundaryConditions, null, progressMonitor));
			}
			IHtmlParser parser = htmlParser;
			if (this.normalizeInputHtml)
			{
				if (this.injectionFormat == HeaderFooterFormat.Html)
				{
					parser = new HtmlNormalizingParser(htmlParser, (HtmlInjection)injection, false, this.testMaxHtmlNormalizerNesting, this.testBoundaryConditions, this.testNormalizerTraceStream, this.testNormalizerTraceShowTokenNum, this.testNormalizerTraceStopOnTokenNum);
					injection = null;
				}
				else
				{
					parser = new HtmlNormalizingParser(htmlParser, null, false, this.testMaxHtmlNormalizerNesting, this.testBoundaryConditions, this.testNormalizerTraceStream, this.testNormalizerTraceShowTokenNum, this.testNormalizerTraceStopOnTokenNum);
				}
			}
			TextOutput output2 = new TextOutput(output, this.wrapFlowed, this.wrapFlowed, 72, 78, this.imageRenderingCallback, this.fallbacks, this.htmlEscape, this.testPreserveTrailingSpaces, this.testFormatTraceStream);
			return new HtmlToTextConverter(parser, output2, injection, false, false, this.testTreatNbspAsBreakable, this.testTraceStream, this.testTraceShowTokenNum, this.testTraceStopOnTokenNum, this.ShouldUseNarrowGapForPTagHtmlToTextConversion, this.OutputAnchorLinks, this.OutputImageLinks);
		}

		internal override void SetResult(ConfigParameter parameterId, object val)
		{
			switch (parameterId)
			{
			case ConfigParameter.InputEncoding:
				this.inputEncoding = (Encoding)val;
				break;
			case ConfigParameter.OutputEncoding:
				this.outputEncoding = (Encoding)val;
				break;
			}
			base.SetResult(parameterId, val);
		}

		private void AssertNotLockedAndNotTextExtraction()
		{
			base.AssertNotLocked();
			if (this.mode == TextExtractionMode.ExtractText)
			{
				throw new InvalidOperationException(TextConvertersStrings.PropertyNotValidForTextExtractionMode);
			}
		}

		private TextExtractionMode mode;

		private Encoding inputEncoding;

		private bool detectEncodingFromByteOrderMark = true;

		private bool detectEncodingFromMetaTag = true;

		private bool normalizeInputHtml;

		private Encoding outputEncoding;

		private bool outputEncodingSameAsInput = true;

		private bool wrapFlowed;

		private bool wrapDelSp;

		private bool fallbacks = true;

		private bool htmlEscape;

		private HeaderFooterFormat injectionFormat;

		private string injectHead;

		private string injectTail;

		private ImageRenderingCallbackInternal imageRenderingCallback;

		private bool testPreserveTrailingSpaces;

		private int testMaxTokenRuns = 512;

		private bool testTreatNbspAsBreakable;

		private Stream testTraceStream;

		private bool testTraceShowTokenNum = true;

		private int testTraceStopOnTokenNum;

		private Stream testNormalizerTraceStream;

		private bool testNormalizerTraceShowTokenNum = true;

		private int testNormalizerTraceStopOnTokenNum;

		private int testMaxHtmlTagSize = 4096;

		private int testMaxHtmlTagAttributes = 64;

		private int testMaxHtmlRestartOffset = 4096;

		private int testMaxHtmlNormalizerNesting = 4096;

		private Stream testFormatTraceStream;

		private bool outputAnchorLinks = true;

		private bool outputImageLinks = true;
	}
}
