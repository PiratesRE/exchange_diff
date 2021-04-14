using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.TextConverters.Internal.Format;
using Microsoft.Exchange.Data.TextConverters.Internal.Html;
using Microsoft.Exchange.Data.TextConverters.Internal.Rtf;

namespace Microsoft.Exchange.Data.TextConverters
{
	public class HtmlToRtf : TextConverter
	{
		public HtmlToRtf()
		{
			this.encapsulateMarkup = RegistryConfigManager.HtmlEncapsulationOverride;
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

		public HeaderFooterFormat HeaderFooterFormat
		{
			get
			{
				return this.injectionFormat;
			}
			set
			{
				base.AssertNotLocked();
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
				base.AssertNotLocked();
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
				base.AssertNotLocked();
				this.injectTail = value;
			}
		}

		public bool EncapsulateHtmlMarkup
		{
			get
			{
				return this.encapsulateMarkup;
			}
			set
			{
				base.AssertNotLocked();
				this.encapsulateMarkup = value;
			}
		}

		internal HtmlToRtf SetInputEncoding(Encoding value)
		{
			this.InputEncoding = value;
			return this;
		}

		internal HtmlToRtf SetDetectEncodingFromByteOrderMark(bool value)
		{
			this.DetectEncodingFromByteOrderMark = value;
			return this;
		}

		internal HtmlToRtf SetDetectEncodingFromMetaTag(bool value)
		{
			this.DetectEncodingFromMetaTag = value;
			return this;
		}

		internal HtmlToRtf SetHeaderFooterFormat(HeaderFooterFormat value)
		{
			this.HeaderFooterFormat = value;
			return this;
		}

		internal HtmlToRtf SetHeader(string value)
		{
			this.Header = value;
			return this;
		}

		internal HtmlToRtf SetFooter(string value)
		{
			this.Footer = value;
			return this;
		}

		internal HtmlToRtf SetImageRenderingCallback(ImageRenderingCallbackInternal value)
		{
			this.imageRenderingCallback = value;
			return this;
		}

		internal HtmlToRtf SetInputStreamBufferSize(int value)
		{
			base.InputStreamBufferSize = value;
			return this;
		}

		internal HtmlToRtf SetTestBoundaryConditions(bool value)
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

		internal HtmlToRtf SetTestMaxTokenRuns(int value)
		{
			this.testMaxTokenRuns = value;
			return this;
		}

		internal HtmlToRtf SetTestTraceStream(Stream value)
		{
			this.testTraceStream = value;
			return this;
		}

		internal HtmlToRtf SetTestTraceShowTokenNum(bool value)
		{
			this.testTraceShowTokenNum = value;
			return this;
		}

		internal HtmlToRtf SetTestTraceStopOnTokenNum(int value)
		{
			this.testTraceStopOnTokenNum = value;
			return this;
		}

		internal HtmlToRtf SetTestNormalizerTraceStream(Stream value)
		{
			this.testNormalizerTraceStream = value;
			return this;
		}

		internal HtmlToRtf SetTestNormalizerTraceShowTokenNum(bool value)
		{
			this.testNormalizerTraceShowTokenNum = value;
			return this;
		}

		internal HtmlToRtf SetTestNormalizerTraceStopOnTokenNum(int value)
		{
			this.testNormalizerTraceStopOnTokenNum = value;
			return this;
		}

		internal HtmlToRtf SetTestFormatTraceStream(Stream value)
		{
			this.testFormatTraceStream = value;
			return this;
		}

		internal HtmlToRtf SetTestFormatOutputTraceStream(Stream value)
		{
			this.testFormatOutputTraceStream = value;
			return this;
		}

		internal HtmlToRtf SetTestFormatConverterTraceStream(Stream value)
		{
			this.testFormatConverterTraceStream = value;
			return this;
		}

		internal HtmlToRtf SetTestMaxHtmlTagSize(int value)
		{
			this.testMaxHtmlTagSize = value;
			return this;
		}

		internal HtmlToRtf SetTestMaxHtmlTagAttributes(int value)
		{
			this.testMaxHtmlTagAttributes = value;
			return this;
		}

		internal HtmlToRtf SetTestMaxHtmlRestartOffset(int value)
		{
			this.testMaxHtmlRestartOffset = value;
			return this;
		}

		internal HtmlToRtf SetTestMaxHtmlNormalizerNesting(int value)
		{
			this.testMaxHtmlNormalizerNesting = value;
			return this;
		}

		internal override IProducerConsumer CreatePushChain(ConverterStream converterStream, Stream output)
		{
			ConverterInput input = new ConverterDecodingInput(converterStream, true, this.inputEncoding, this.detectEncodingFromByteOrderMark, this.testMaxHtmlTagSize, this.testMaxHtmlRestartOffset, base.InputStreamBufferSize, this.testBoundaryConditions, this, null);
			FormatOutput output2 = new RtfFormatOutput(output, true, true, this.testBoundaryConditions, this, this.imageRenderingCallback, this.testFormatTraceStream, this.testFormatOutputTraceStream, this.inputEncoding);
			return this.CreateChain(input, output2, converterStream);
		}

		internal override IProducerConsumer CreatePushChain(ConverterStream converterStream, TextWriter output)
		{
			throw new NotSupportedException(TextConvertersStrings.TextWriterUnsupported);
		}

		internal override IProducerConsumer CreatePushChain(ConverterWriter converterWriter, Stream output)
		{
			this.inputEncoding = Encoding.Unicode;
			ConverterInput input = new ConverterUnicodeInput(converterWriter, true, this.testMaxHtmlTagSize, this.testBoundaryConditions, null);
			FormatOutput output2 = new RtfFormatOutput(output, true, false, this.testBoundaryConditions, this, this.imageRenderingCallback, this.testFormatTraceStream, this.testFormatOutputTraceStream, null);
			return this.CreateChain(input, output2, converterWriter);
		}

		internal override IProducerConsumer CreatePushChain(ConverterWriter converterWriter, TextWriter output)
		{
			throw new NotSupportedException(TextConvertersStrings.TextWriterUnsupported);
		}

		internal override IProducerConsumer CreatePullChain(Stream input, ConverterStream converterStream)
		{
			ConverterInput input2 = new ConverterDecodingInput(input, false, this.inputEncoding, this.detectEncodingFromByteOrderMark, this.testMaxHtmlTagSize, this.testMaxHtmlRestartOffset, base.InputStreamBufferSize, this.testBoundaryConditions, this, converterStream);
			FormatOutput output = new RtfFormatOutput(converterStream, false, true, this.testBoundaryConditions, this, this.imageRenderingCallback, this.testFormatTraceStream, this.testFormatOutputTraceStream, this.inputEncoding);
			return this.CreateChain(input2, output, converterStream);
		}

		internal override IProducerConsumer CreatePullChain(TextReader input, ConverterStream converterStream)
		{
			this.inputEncoding = Encoding.Unicode;
			ConverterInput input2 = new ConverterUnicodeInput(input, false, this.testMaxHtmlTagSize, this.testBoundaryConditions, converterStream);
			FormatOutput output = new RtfFormatOutput(converterStream, false, false, this.testBoundaryConditions, this, this.imageRenderingCallback, this.testFormatTraceStream, this.testFormatOutputTraceStream, null);
			return this.CreateChain(input2, output, converterStream);
		}

		internal override IProducerConsumer CreatePullChain(Stream input, ConverterReader converterReader)
		{
			throw new NotSupportedException(TextConvertersStrings.CannotUseConverterReader);
		}

		internal override IProducerConsumer CreatePullChain(TextReader input, ConverterReader converterReader)
		{
			throw new NotSupportedException(TextConvertersStrings.CannotUseConverterReader);
		}

		private IProducerConsumer CreateChain(ConverterInput input, FormatOutput output, IProgressMonitor progressMonitor)
		{
			this.locked = true;
			HtmlInjection injection = null;
			if (this.injectHead != null || this.injectTail != null)
			{
				injection = new HtmlInjection(this.injectHead, this.injectTail, this.injectionFormat, false, null, this.testBoundaryConditions, null, progressMonitor);
			}
			HtmlParser parser = new HtmlParser(input, this.detectEncodingFromMetaTag, false, this.testMaxTokenRuns, this.testMaxHtmlTagAttributes, this.testBoundaryConditions);
			HtmlNormalizingParser parser2 = new HtmlNormalizingParser(parser, injection, false, this.testMaxHtmlNormalizerNesting, this.testBoundaryConditions, this.testNormalizerTraceStream, this.testNormalizerTraceShowTokenNum, this.testNormalizerTraceStopOnTokenNum);
			HtmlFormatConverter result;
			if (this.encapsulateMarkup)
			{
				result = new HtmlFormatConverterWithEncapsulation(parser2, output, this.encapsulateMarkup, false, this.testTraceStream, this.testTraceShowTokenNum, this.testTraceStopOnTokenNum, this.testFormatConverterTraceStream, progressMonitor);
			}
			else
			{
				result = new HtmlFormatConverter(parser2, output, false, this.testTraceStream, this.testTraceShowTokenNum, this.testTraceStopOnTokenNum, this.testFormatConverterTraceStream, progressMonitor);
			}
			return result;
		}

		internal override void SetResult(ConfigParameter parameterId, object val)
		{
			if (parameterId == ConfigParameter.InputEncoding)
			{
				this.inputEncoding = (Encoding)val;
			}
			base.SetResult(parameterId, val);
		}

		private Encoding inputEncoding;

		private bool detectEncodingFromByteOrderMark = true;

		private bool detectEncodingFromMetaTag = true;

		private HeaderFooterFormat injectionFormat;

		private string injectHead;

		private string injectTail;

		private bool encapsulateMarkup;

		private ImageRenderingCallbackInternal imageRenderingCallback;

		private int testMaxTokenRuns = 512;

		private Stream testTraceStream;

		private bool testTraceShowTokenNum = true;

		private int testTraceStopOnTokenNum;

		private Stream testFormatTraceStream;

		private Stream testFormatOutputTraceStream;

		private Stream testFormatConverterTraceStream;

		private Stream testNormalizerTraceStream;

		private bool testNormalizerTraceShowTokenNum = true;

		private int testNormalizerTraceStopOnTokenNum;

		private int testMaxHtmlTagSize = 4096;

		private int testMaxHtmlTagAttributes = 64;

		private int testMaxHtmlRestartOffset = 4096;

		private int testMaxHtmlNormalizerNesting = 4096;
	}
}
