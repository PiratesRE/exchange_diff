using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.TextConverters.Internal.Html;

namespace Microsoft.Exchange.Data.TextConverters
{
	public class HtmlToHtml : TextConverter
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

		public bool NormalizeHtml
		{
			get
			{
				return this.normalizeInputHtml;
			}
			set
			{
				base.AssertNotLocked();
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

		public bool OutputHtmlFragment
		{
			get
			{
				return this.outputFragment;
			}
			set
			{
				base.AssertNotLocked();
				this.outputFragment = value;
			}
		}

		public bool FilterHtml
		{
			get
			{
				return this.filterHtml;
			}
			set
			{
				base.AssertNotLocked();
				this.filterHtml = value;
			}
		}

		public HtmlTagCallback HtmlTagCallback
		{
			get
			{
				return this.htmlCallback;
			}
			set
			{
				base.AssertNotLocked();
				this.htmlCallback = value;
			}
		}

		public int MaxCallbackTagLength
		{
			get
			{
				return this.maxHtmlTagSize;
			}
			set
			{
				base.AssertNotLocked();
				this.maxHtmlTagSize = value;
			}
		}

		internal HtmlToHtml SetInputEncoding(Encoding value)
		{
			this.InputEncoding = value;
			return this;
		}

		internal HtmlToHtml SetDetectEncodingFromByteOrderMark(bool value)
		{
			this.DetectEncodingFromByteOrderMark = value;
			return this;
		}

		internal HtmlToHtml SetDetectEncodingFromMetaTag(bool value)
		{
			this.DetectEncodingFromMetaTag = value;
			return this;
		}

		internal HtmlToHtml SetOutputEncoding(Encoding value)
		{
			this.OutputEncoding = value;
			return this;
		}

		internal HtmlToHtml SetNormalizeHtml(bool value)
		{
			this.NormalizeHtml = value;
			return this;
		}

		internal HtmlToHtml SetHeaderFooterFormat(HeaderFooterFormat value)
		{
			this.HeaderFooterFormat = value;
			return this;
		}

		internal HtmlToHtml SetHeader(string value)
		{
			this.Header = value;
			return this;
		}

		internal HtmlToHtml SetFooter(string value)
		{
			this.Footer = value;
			return this;
		}

		internal HtmlToHtml SetFilterHtml(bool value)
		{
			this.FilterHtml = value;
			return this;
		}

		internal HtmlToHtml SetHtmlTagCallback(HtmlTagCallback value)
		{
			this.HtmlTagCallback = value;
			return this;
		}

		internal HtmlToHtml SetTestTruncateForCallback(bool value)
		{
			this.testTruncateForCallback = value;
			return this;
		}

		internal HtmlToHtml SetMaxCallbackTagLength(int value)
		{
			this.maxHtmlTagSize = value;
			return this;
		}

		internal HtmlToHtml SetInputStreamBufferSize(int value)
		{
			base.InputStreamBufferSize = value;
			return this;
		}

		internal HtmlToHtml SetOutputHtmlFragment(bool value)
		{
			this.OutputHtmlFragment = value;
			return this;
		}

		internal HtmlToHtml SetTestConvertHtmlFragment(bool value)
		{
			this.testConvertFragment = value;
			return this;
		}

		internal HtmlToHtml SetTestBoundaryConditions(bool value)
		{
			this.testBoundaryConditions = value;
			if (value)
			{
				this.maxHtmlTagSize = 123;
				this.testMaxHtmlTagAttributes = 5;
				this.testMaxHtmlNormalizerNesting = 10;
			}
			return this;
		}

		internal HtmlToHtml SetTestMaxTokenRuns(int value)
		{
			this.testMaxTokenRuns = value;
			return this;
		}

		internal HtmlToHtml SetTestTraceStream(Stream value)
		{
			this.testTraceStream = value;
			return this;
		}

		internal HtmlToHtml SetTestTraceShowTokenNum(bool value)
		{
			this.testTraceShowTokenNum = value;
			return this;
		}

		internal HtmlToHtml SetTestTraceStopOnTokenNum(int value)
		{
			this.testTraceStopOnTokenNum = value;
			return this;
		}

		internal HtmlToHtml SetTestNormalizerTraceStream(Stream value)
		{
			this.testNormalizerTraceStream = value;
			return this;
		}

		internal HtmlToHtml SetTestNormalizerTraceShowTokenNum(bool value)
		{
			this.testNormalizerTraceShowTokenNum = value;
			return this;
		}

		internal HtmlToHtml SetTestNormalizerTraceStopOnTokenNum(int value)
		{
			this.testNormalizerTraceStopOnTokenNum = value;
			return this;
		}

		internal HtmlToHtml SetTestMaxHtmlTagAttributes(int value)
		{
			this.testMaxHtmlTagAttributes = value;
			return this;
		}

		internal HtmlToHtml SetTestMaxHtmlRestartOffset(int value)
		{
			this.testMaxHtmlRestartOffset = value;
			return this;
		}

		internal HtmlToHtml SetTestMaxHtmlNormalizerNesting(int value)
		{
			this.testMaxHtmlNormalizerNesting = value;
			return this;
		}

		internal HtmlToHtml SetTestNoNewLines(bool value)
		{
			this.testNoNewLines = value;
			return this;
		}

		internal HtmlToHtml SetSmallCssBlockThreshold(int value)
		{
			this.smallCssBlockThreshold = value;
			return this;
		}

		internal HtmlToHtml SetPreserveDisplayNoneStyle(bool value)
		{
			this.preserveDisplayNoneStyle = value;
			return this;
		}

		internal override IProducerConsumer CreatePushChain(ConverterStream converterStream, Stream output)
		{
			if (this.inputEncoding == null)
			{
				throw new InvalidOperationException(TextConvertersStrings.InputEncodingRequired);
			}
			ConverterInput input = new ConverterDecodingInput(converterStream, true, this.inputEncoding, this.detectEncodingFromByteOrderMark, this.maxHtmlTagSize, this.testMaxHtmlRestartOffset, base.InputStreamBufferSize, this.testBoundaryConditions, this, null);
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
			ConverterInput input = new ConverterDecodingInput(converterStream, true, this.inputEncoding, this.detectEncodingFromByteOrderMark, this.maxHtmlTagSize, this.testMaxHtmlRestartOffset, base.InputStreamBufferSize, this.testBoundaryConditions, this, null);
			ConverterOutput output2 = new ConverterUnicodeOutput(output, true, true);
			return this.CreateChain(input, output2, converterStream);
		}

		internal override IProducerConsumer CreatePushChain(ConverterWriter converterWriter, Stream output)
		{
			this.inputEncoding = Encoding.Unicode;
			ConverterInput input = new ConverterUnicodeInput(converterWriter, true, this.maxHtmlTagSize, this.testBoundaryConditions, null);
			ConverterOutput output2 = new ConverterEncodingOutput(output, true, false, this.outputEncodingSameAsInput ? Encoding.UTF8 : this.outputEncoding, this.outputEncodingSameAsInput, this.testBoundaryConditions, this);
			return this.CreateChain(input, output2, converterWriter);
		}

		internal override IProducerConsumer CreatePushChain(ConverterWriter converterWriter, TextWriter output)
		{
			this.inputEncoding = Encoding.Unicode;
			this.outputEncoding = Encoding.Unicode;
			ConverterInput input = new ConverterUnicodeInput(converterWriter, true, this.maxHtmlTagSize, this.testBoundaryConditions, null);
			ConverterOutput output2 = new ConverterUnicodeOutput(output, true, false);
			return this.CreateChain(input, output2, converterWriter);
		}

		internal override IProducerConsumer CreatePullChain(Stream input, ConverterStream converterStream)
		{
			if (this.inputEncoding == null)
			{
				throw new InvalidOperationException(TextConvertersStrings.InputEncodingRequired);
			}
			ConverterInput input2 = new ConverterDecodingInput(input, false, this.inputEncoding, this.detectEncodingFromByteOrderMark, this.maxHtmlTagSize, this.testMaxHtmlRestartOffset, base.InputStreamBufferSize, this.testBoundaryConditions, this, converterStream);
			ConverterOutput output = new ConverterEncodingOutput(converterStream, false, true, this.outputEncodingSameAsInput ? this.inputEncoding : this.outputEncoding, this.outputEncodingSameAsInput, this.testBoundaryConditions, this);
			return this.CreateChain(input2, output, converterStream);
		}

		internal override IProducerConsumer CreatePullChain(TextReader input, ConverterStream converterStream)
		{
			this.inputEncoding = Encoding.Unicode;
			ConverterInput input2 = new ConverterUnicodeInput(input, false, this.maxHtmlTagSize, this.testBoundaryConditions, converterStream);
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
			ConverterInput input2 = new ConverterDecodingInput(input, false, this.inputEncoding, this.detectEncodingFromByteOrderMark, this.maxHtmlTagSize, this.testMaxHtmlRestartOffset, base.InputStreamBufferSize, this.testBoundaryConditions, this, converterReader);
			ConverterOutput output = new ConverterUnicodeOutput(converterReader, false, true);
			return this.CreateChain(input2, output, converterReader);
		}

		internal override IProducerConsumer CreatePullChain(TextReader input, ConverterReader converterReader)
		{
			this.inputEncoding = Encoding.Unicode;
			this.outputEncoding = Encoding.Unicode;
			ConverterInput input2 = new ConverterUnicodeInput(input, false, this.maxHtmlTagSize, this.testBoundaryConditions, converterReader);
			ConverterOutput output = new ConverterUnicodeOutput(converterReader, false, false);
			return this.CreateChain(input2, output, converterReader);
		}

		private IProducerConsumer CreateChain(ConverterInput input, ConverterOutput output, IProgressMonitor progressMonitor)
		{
			this.locked = true;
			HtmlInjection htmlInjection = null;
			HtmlInjection htmlInjection2 = null;
			IProducerConsumer result;
			try
			{
				if (this.injectHead != null || this.injectTail != null)
				{
					htmlInjection2 = new HtmlInjection(this.injectHead, this.injectTail, this.injectionFormat, this.filterHtml, this.htmlCallback, this.testBoundaryConditions, null, progressMonitor);
					htmlInjection = htmlInjection2;
					this.normalizeInputHtml = true;
				}
				if (this.filterHtml || this.outputFragment || this.htmlCallback != null)
				{
					this.normalizeInputHtml = true;
				}
				IHtmlParser parser2;
				if (this.normalizeInputHtml)
				{
					HtmlParser parser = new HtmlParser(input, this.detectEncodingFromMetaTag, false, this.testMaxTokenRuns, this.testMaxHtmlTagAttributes, this.testBoundaryConditions);
					parser2 = new HtmlNormalizingParser(parser, htmlInjection, this.htmlCallback != null, this.testMaxHtmlNormalizerNesting, this.testBoundaryConditions, this.testNormalizerTraceStream, this.testNormalizerTraceShowTokenNum, this.testNormalizerTraceStopOnTokenNum);
					htmlInjection2 = null;
				}
				else
				{
					parser2 = new HtmlParser(input, this.detectEncodingFromMetaTag, false, this.testMaxTokenRuns, this.testMaxHtmlTagAttributes, this.testBoundaryConditions);
				}
				HtmlWriter writer = new HtmlWriter(output, this.filterHtml, this.normalizeInputHtml && !this.testNoNewLines);
				result = new HtmlToHtmlConverter(parser2, writer, this.testConvertFragment, this.outputFragment, this.filterHtml, this.htmlCallback, this.testTruncateForCallback, htmlInjection != null && htmlInjection.HaveTail, this.testTraceStream, this.testTraceShowTokenNum, this.testTraceStopOnTokenNum, this.smallCssBlockThreshold, this.preserveDisplayNoneStyle, progressMonitor);
			}
			finally
			{
				IDisposable disposable = htmlInjection2;
				if (disposable != null)
				{
					disposable.Dispose();
					htmlInjection2 = null;
				}
			}
			return result;
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

		private Encoding inputEncoding;

		private bool detectEncodingFromByteOrderMark = true;

		private bool detectEncodingFromMetaTag = true;

		private Encoding outputEncoding;

		private bool outputEncodingSameAsInput = true;

		private bool normalizeInputHtml;

		private HeaderFooterFormat injectionFormat;

		private string injectHead;

		private string injectTail;

		private bool filterHtml;

		private HtmlTagCallback htmlCallback;

		private bool testTruncateForCallback = true;

		private bool testConvertFragment;

		private bool outputFragment;

		private int testMaxTokenRuns = 512;

		private Stream testTraceStream;

		private bool testTraceShowTokenNum = true;

		private int testTraceStopOnTokenNum;

		private Stream testNormalizerTraceStream;

		private bool testNormalizerTraceShowTokenNum = true;

		private int testNormalizerTraceStopOnTokenNum;

		private int maxHtmlTagSize = 32768;

		private int testMaxHtmlTagAttributes = 64;

		private int testMaxHtmlRestartOffset = 4096;

		private int testMaxHtmlNormalizerNesting = 4096;

		private int smallCssBlockThreshold = -1;

		private bool preserveDisplayNoneStyle;

		private bool testNoNewLines;
	}
}
