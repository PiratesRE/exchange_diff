using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.TextConverters.Internal.Format;
using Microsoft.Exchange.Data.TextConverters.Internal.Html;
using Microsoft.Exchange.Data.TextConverters.Internal.Rtf;

namespace Microsoft.Exchange.Data.TextConverters
{
	public class RtfToHtml : TextConverter
	{
		public bool EncapsulatedHtml
		{
			get
			{
				return this.rtfEncapsulation == RtfEncapsulation.Html;
			}
		}

		public bool ConvertedFromText
		{
			get
			{
				return this.rtfEncapsulation == RtfEncapsulation.Text;
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

		internal RtfToHtml SetOutputEncoding(Encoding value)
		{
			this.OutputEncoding = value;
			return this;
		}

		internal RtfToHtml SetNormalizeHtml(bool value)
		{
			this.NormalizeHtml = value;
			return this;
		}

		internal RtfToHtml SetFilterHtml(bool value)
		{
			this.FilterHtml = value;
			return this;
		}

		internal RtfToHtml SetHtmlTagCallback(HtmlTagCallback value)
		{
			this.HtmlTagCallback = value;
			return this;
		}

		internal RtfToHtml SetTestTruncateForCallback(bool value)
		{
			this.testTruncateForCallback = value;
			return this;
		}

		internal RtfToHtml SetMaxCallbackTagLength(int value)
		{
			this.maxHtmlTagSize = value;
			return this;
		}

		internal RtfToHtml SetHeaderFooterFormat(HeaderFooterFormat value)
		{
			this.HeaderFooterFormat = value;
			return this;
		}

		internal RtfToHtml SetHeader(string value)
		{
			this.Header = value;
			return this;
		}

		internal RtfToHtml SetFooter(string value)
		{
			this.Footer = value;
			return this;
		}

		internal RtfToHtml SetInputStreamBufferSize(int value)
		{
			base.InputStreamBufferSize = value;
			return this;
		}

		internal RtfToHtml SetTestBoundaryConditions(bool value)
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

		internal RtfToHtml SetTestTraceStream(Stream value)
		{
			this.testTraceStream = value;
			return this;
		}

		internal RtfToHtml SetTestTraceShowTokenNum(bool value)
		{
			this.testTraceShowTokenNum = value;
			return this;
		}

		internal RtfToHtml SetTestTraceStopOnTokenNum(int value)
		{
			this.testTraceStopOnTokenNum = value;
			return this;
		}

		internal RtfToHtml SetTestFormatTraceStream(Stream value)
		{
			this.testFormatTraceStream = value;
			return this;
		}

		internal RtfToHtml SetTestFormatOutputTraceStream(Stream value)
		{
			this.testFormatOutputTraceStream = value;
			return this;
		}

		internal RtfToHtml SetTestFormatConverterTraceStream(Stream value)
		{
			this.testFormatConverterTraceStream = value;
			return this;
		}

		internal RtfToHtml SetTestHtmlTraceStream(Stream value)
		{
			this.testHtmlTraceStream = value;
			return this;
		}

		internal RtfToHtml SetTestHtmlTraceShowTokenNum(bool value)
		{
			this.testHtmlTraceShowTokenNum = value;
			return this;
		}

		internal RtfToHtml SetTestHtmlTraceStopOnTokenNum(int value)
		{
			this.testHtmlTraceStopOnTokenNum = value;
			return this;
		}

		internal RtfToHtml SetTestNormalizerTraceStream(Stream value)
		{
			this.testNormalizerTraceStream = value;
			return this;
		}

		internal RtfToHtml SetTestNormalizerTraceShowTokenNum(bool value)
		{
			this.testNormalizerTraceShowTokenNum = value;
			return this;
		}

		internal RtfToHtml SetTestNormalizerTraceStopOnTokenNum(int value)
		{
			this.testNormalizerTraceStopOnTokenNum = value;
			return this;
		}

		internal RtfToHtml SetTestMaxTokenRuns(int value)
		{
			this.testMaxTokenRuns = value;
			return this;
		}

		internal RtfToHtml SetTestMaxHtmlTagAttributes(int value)
		{
			this.testMaxHtmlTagAttributes = value;
			return this;
		}

		internal RtfToHtml SetTestMaxHtmlNormalizerNesting(int value)
		{
			this.testMaxHtmlNormalizerNesting = value;
			return this;
		}

		internal RtfToHtml SetTestNoNewLines(bool value)
		{
			this.testNoNewLines = value;
			return this;
		}

		internal RtfToHtml SetSmallCssBlockThreshold(int value)
		{
			this.smallCssBlockThreshold = value;
			return this;
		}

		internal RtfToHtml SetPreserveDisplayNoneStyle(bool value)
		{
			this.preserveDisplayNoneStyle = value;
			return this;
		}

		internal RtfToHtml SetExpansionSizeLimit(int value)
		{
			this.expansionSizeLimit = value;
			return this;
		}

		internal RtfToHtml SetExpansionSizeMultiple(int value)
		{
			this.expansionSizeMultiple = value;
			return this;
		}

		internal override IProducerConsumer CreatePushChain(ConverterStream converterStream, Stream output)
		{
			ConverterOutput output2 = new ConverterEncodingOutput(output, true, false, this.outputEncodingSameAsInput ? Encoding.GetEncoding("Windows-1252") : this.outputEncoding, this.outputEncodingSameAsInput, this.testBoundaryConditions, this);
			return this.CreateChain(converterStream, true, output2, converterStream);
		}

		internal override IProducerConsumer CreatePushChain(ConverterStream converterStream, TextWriter output)
		{
			ConverterOutput output2 = new ConverterUnicodeOutput(output, true, false);
			return this.CreateChain(converterStream, true, output2, converterStream);
		}

		internal override IProducerConsumer CreatePushChain(ConverterWriter converterWriter, Stream output)
		{
			throw new NotSupportedException(TextConvertersStrings.CannotUseConverterWriter);
		}

		internal override IProducerConsumer CreatePushChain(ConverterWriter converterWriter, TextWriter output)
		{
			throw new NotSupportedException(TextConvertersStrings.CannotUseConverterWriter);
		}

		internal override IProducerConsumer CreatePullChain(Stream input, ConverterStream converterStream)
		{
			ConverterOutput output = new ConverterEncodingOutput(converterStream, false, false, this.outputEncodingSameAsInput ? Encoding.GetEncoding("Windows-1252") : this.outputEncoding, this.outputEncodingSameAsInput, this.testBoundaryConditions, this);
			return this.CreateChain(input, false, output, converterStream);
		}

		internal override IProducerConsumer CreatePullChain(TextReader input, ConverterStream converterStream)
		{
			throw new NotSupportedException(TextConvertersStrings.TextReaderUnsupported);
		}

		internal override IProducerConsumer CreatePullChain(Stream input, ConverterReader converterReader)
		{
			ConverterOutput output = new ConverterUnicodeOutput(converterReader, false, false);
			return this.CreateChain(input, false, output, converterReader);
		}

		internal override IProducerConsumer CreatePullChain(TextReader input, ConverterReader converterReader)
		{
			throw new NotSupportedException(TextConvertersStrings.TextReaderUnsupported);
		}

		private IProducerConsumer CreateChain(Stream input, bool push, ConverterOutput output, IProgressMonitor progressMonitor)
		{
			this.locked = true;
			this.reportBytes = new ReportBytes(this.expansionSizeLimit, this.expansionSizeMultiple);
			output.ReportBytes = this.reportBytes;
			RtfParser parser;
			if (!this.enableHtmlDeencapsulation)
			{
				parser = new RtfParser(input, push, base.InputStreamBufferSize, this.testBoundaryConditions, push ? null : progressMonitor, this.reportBytes);
				HtmlInjection injection = null;
				if (this.injectHead != null || this.injectTail != null)
				{
					injection = new HtmlInjection(this.injectHead, this.injectTail, this.injectionFormat, this.filterHtml, this.htmlCallback, this.testBoundaryConditions, null, progressMonitor);
				}
				HtmlWriter writer = new HtmlWriter(output, this.filterHtml, !this.testNoNewLines);
				FormatOutput output2 = new HtmlFormatOutput(writer, injection, this.outputFragment, this.testFormatTraceStream, this.testFormatOutputTraceStream, this.filterHtml, this.htmlCallback, true);
				return new RtfFormatConverter(parser, output2, null, false, this.testTraceStream, this.testTraceShowTokenNum, this.testTraceStopOnTokenNum, this.testFormatConverterTraceStream);
			}
			RtfPreviewStream rtfPreviewStream = input as RtfPreviewStream;
			if (!push && rtfPreviewStream != null && rtfPreviewStream.Parser != null && rtfPreviewStream.InternalPosition == 0 && rtfPreviewStream.InputRtfStream != null)
			{
				rtfPreviewStream.InternalPosition = int.MaxValue;
				parser = new RtfParser(rtfPreviewStream.InputRtfStream, base.InputStreamBufferSize, this.testBoundaryConditions, push ? null : progressMonitor, rtfPreviewStream.Parser, this.reportBytes);
				return this.CreateChain(rtfPreviewStream.Encapsulation, parser, output, progressMonitor);
			}
			parser = new RtfParser(input, push, base.InputStreamBufferSize, this.testBoundaryConditions, push ? null : progressMonitor, this.reportBytes);
			return new RtfToHtmlAdapter(parser, output, this, progressMonitor);
		}

		internal IProducerConsumer CreateChain(RtfEncapsulation encapsulation, RtfParser parser, ConverterOutput output, IProgressMonitor progressMonitor)
		{
			this.rtfEncapsulation = encapsulation;
			if (this.reportBytes == null)
			{
				throw new InvalidOperationException("I have an RtfParser but no ReportBytes.");
			}
			output.ReportBytes = this.reportBytes;
			IProducerConsumer result;
			if (encapsulation != RtfEncapsulation.Html)
			{
				HtmlInjection injection = null;
				if (this.injectHead != null || this.injectTail != null)
				{
					injection = new HtmlInjection(this.injectHead, this.injectTail, this.injectionFormat, this.filterHtml, this.htmlCallback, this.testBoundaryConditions, null, progressMonitor);
				}
				HtmlWriter writer = new HtmlWriter(output, this.filterHtml, !this.testNoNewLines);
				FormatOutput output2 = new HtmlFormatOutput(writer, injection, this.outputFragment, this.testFormatTraceStream, this.testFormatOutputTraceStream, this.filterHtml, this.htmlCallback, true);
				RtfFormatConverter rtfFormatConverter = new RtfFormatConverter(parser, output2, null, false, this.testTraceStream, this.testTraceShowTokenNum, this.testTraceStopOnTokenNum, this.testFormatConverterTraceStream);
				result = rtfFormatConverter;
			}
			else
			{
				HtmlInjection htmlInjection = null;
				HtmlInjection htmlInjection2 = null;
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
					HtmlInRtfExtractingInput input = new HtmlInRtfExtractingInput(parser, this.maxHtmlTagSize, this.testBoundaryConditions, this.testTraceStream, this.testTraceShowTokenNum, this.testTraceStopOnTokenNum);
					IHtmlParser parser3;
					if (this.normalizeInputHtml)
					{
						HtmlParser parser2 = new HtmlParser(input, false, false, this.testMaxTokenRuns, this.testMaxHtmlTagAttributes, this.testBoundaryConditions);
						parser3 = new HtmlNormalizingParser(parser2, htmlInjection, this.htmlCallback != null, this.testMaxHtmlNormalizerNesting, this.testBoundaryConditions, this.testNormalizerTraceStream, this.testNormalizerTraceShowTokenNum, this.testNormalizerTraceStopOnTokenNum);
						htmlInjection2 = null;
					}
					else
					{
						parser3 = new HtmlParser(input, false, false, this.testMaxTokenRuns, this.testMaxHtmlTagAttributes, this.testBoundaryConditions);
					}
					HtmlWriter writer2 = new HtmlWriter(output, this.filterHtml, this.normalizeInputHtml && !this.testNoNewLines);
					result = new HtmlToHtmlConverter(parser3, writer2, false, this.outputFragment, this.filterHtml, this.htmlCallback, this.testTruncateForCallback, htmlInjection != null && htmlInjection.HaveTail, this.testHtmlTraceStream, this.testHtmlTraceShowTokenNum, this.testHtmlTraceStopOnTokenNum, this.smallCssBlockThreshold, this.preserveDisplayNoneStyle, progressMonitor);
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
			}
			return result;
		}

		internal override void SetResult(ConfigParameter parameterId, object val)
		{
			switch (parameterId)
			{
			case ConfigParameter.OutputEncoding:
				this.outputEncoding = (Encoding)val;
				break;
			case ConfigParameter.RtfEncapsulation:
				this.rtfEncapsulation = (RtfEncapsulation)val;
				break;
			}
			base.SetResult(parameterId, val);
		}

		private Encoding outputEncoding;

		private bool outputEncodingSameAsInput = true;

		private bool enableHtmlDeencapsulation = true;

		private bool normalizeInputHtml;

		private bool filterHtml;

		private HtmlTagCallback htmlCallback;

		private bool testTruncateForCallback = true;

		private HeaderFooterFormat injectionFormat;

		private string injectHead;

		private string injectTail;

		private bool outputFragment;

		private RtfEncapsulation rtfEncapsulation;

		private Stream testTraceStream;

		private bool testTraceShowTokenNum = true;

		private int testTraceStopOnTokenNum;

		private Stream testFormatTraceStream;

		private Stream testFormatOutputTraceStream;

		private Stream testFormatConverterTraceStream;

		private Stream testHtmlTraceStream;

		private bool testHtmlTraceShowTokenNum = true;

		private int testHtmlTraceStopOnTokenNum;

		private Stream testNormalizerTraceStream;

		private bool testNormalizerTraceShowTokenNum = true;

		private int testNormalizerTraceStopOnTokenNum;

		private int testMaxTokenRuns = 512;

		private int maxHtmlTagSize = 32768;

		private int testMaxHtmlTagAttributes = 64;

		private int testMaxHtmlNormalizerNesting = 4096;

		private bool testNoNewLines;

		private int smallCssBlockThreshold = -1;

		private bool preserveDisplayNoneStyle;

		private ReportBytes reportBytes;

		private int expansionSizeLimit;

		private int expansionSizeMultiple;
	}
}
