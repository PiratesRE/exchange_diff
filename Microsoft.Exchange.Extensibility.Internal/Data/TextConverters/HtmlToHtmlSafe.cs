using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.TextConverters.Internal.Format;
using Microsoft.Exchange.Data.TextConverters.Internal.Html;

namespace Microsoft.Exchange.Data.TextConverters
{
	public class HtmlToHtmlSafe : TextConverter
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

		public bool OutputHtmlFragment
		{
			get
			{
				return this.OutputFragment;
			}
			set
			{
				base.AssertNotLocked();
				this.OutputFragment = value;
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

		internal HtmlToHtmlSafe SetInputEncoding(Encoding value)
		{
			this.InputEncoding = value;
			base.AssertNotLocked();
			return this;
		}

		internal HtmlToHtmlSafe SetDetectEncodingFromByteOrderMark(bool value)
		{
			this.DetectEncodingFromByteOrderMark = value;
			base.AssertNotLocked();
			return this;
		}

		internal HtmlToHtmlSafe SetDetectEncodingFromMetaTag(bool value)
		{
			this.DetectEncodingFromMetaTag = value;
			base.AssertNotLocked();
			return this;
		}

		internal HtmlToHtmlSafe SetHeaderFooterFormat(HeaderFooterFormat value)
		{
			this.HeaderFooterFormat = value;
			base.AssertNotLocked();
			return this;
		}

		internal HtmlToHtmlSafe SetHeader(string value)
		{
			base.AssertNotLocked();
			this.Header = value;
			return this;
		}

		internal HtmlToHtmlSafe SetFooter(string value)
		{
			base.AssertNotLocked();
			this.Footer = value;
			return this;
		}

		internal HtmlToHtmlSafe SetFilterHtml(bool value)
		{
			base.AssertNotLocked();
			this.FilterHtml = value;
			return this;
		}

		internal HtmlToHtmlSafe SetHtmlTagCallback(HtmlTagCallback value)
		{
			base.AssertNotLocked();
			this.HtmlTagCallback = value;
			return this;
		}

		internal HtmlToHtmlSafe SetInputStreamBufferSize(int value)
		{
			base.AssertNotLocked();
			base.InputStreamBufferSize = value;
			return this;
		}

		internal HtmlToHtmlSafe SetTestBoundaryConditions(bool value)
		{
			base.AssertNotLocked();
			this.testBoundaryConditions = value;
			this.maxTokenSize = (value ? TextConvertersDefaults.MaxTokenSize(value) : 32768);
			return this;
		}

		internal HtmlToHtmlSafe SetTestTraceStream(Stream value)
		{
			base.AssertNotLocked();
			this.testTraceStream = value;
			return this;
		}

		internal HtmlToHtmlSafe SetTestTraceShowTokenNum(bool value)
		{
			base.AssertNotLocked();
			this.testTraceShowTokenNum = value;
			return this;
		}

		internal HtmlToHtmlSafe SetTestTraceStopOnTokenNum(int value)
		{
			base.AssertNotLocked();
			this.testTraceStopOnTokenNum = value;
			return this;
		}

		internal HtmlToHtmlSafe SetTestNormalizerTraceStream(Stream value)
		{
			base.AssertNotLocked();
			this.testNormalizerTraceStream = value;
			return this;
		}

		internal HtmlToHtmlSafe SetTestNormalizerTraceShowTokenNum(bool value)
		{
			base.AssertNotLocked();
			this.testNormalizerTraceShowTokenNum = value;
			return this;
		}

		internal HtmlToHtmlSafe SetTestNormalizerTraceStopOnTokenNum(int value)
		{
			base.AssertNotLocked();
			this.testNormalizerTraceStopOnTokenNum = value;
			return this;
		}

		internal HtmlToHtmlSafe SetTestFormatTraceStream(Stream value)
		{
			base.AssertNotLocked();
			this.TestFormatTraceStream = value;
			return this;
		}

		internal HtmlToHtmlSafe SetTestFormatOutputTraceStream(Stream value)
		{
			base.AssertNotLocked();
			this.TestFormatOutputTraceStream = value;
			return this;
		}

		internal HtmlToHtmlSafe SetTestFormatConverterTraceStream(Stream value)
		{
			base.AssertNotLocked();
			this.TestFormatConverterTraceStream = value;
			return this;
		}

		internal override IProducerConsumer CreatePushChain(ConverterStream converterStream, Stream output)
		{
			if (this.inputEncoding == null)
			{
				throw new InvalidOperationException(TextConvertersStrings.InputEncodingRequired);
			}
			ConverterInput input = new ConverterDecodingInput(converterStream, true, this.inputEncoding, this.detectEncodingFromByteOrderMark, this.maxTokenSize, TextConvertersDefaults.MaxHtmlMetaRestartOffset(this.testBoundaryConditions), base.InputStreamBufferSize, this.testBoundaryConditions, this, null);
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
			ConverterInput input = new ConverterDecodingInput(converterStream, true, this.inputEncoding, this.detectEncodingFromByteOrderMark, this.maxTokenSize, TextConvertersDefaults.MaxHtmlMetaRestartOffset(this.testBoundaryConditions), base.InputStreamBufferSize, this.testBoundaryConditions, this, null);
			ConverterOutput output2 = new ConverterUnicodeOutput(output, true, true);
			return this.CreateChain(input, output2, converterStream);
		}

		internal override IProducerConsumer CreatePushChain(ConverterWriter converterWriter, Stream output)
		{
			this.inputEncoding = Encoding.Unicode;
			ConverterInput input = new ConverterUnicodeInput(converterWriter, true, this.maxTokenSize, this.testBoundaryConditions, null);
			ConverterOutput output2 = new ConverterEncodingOutput(output, true, false, this.outputEncodingSameAsInput ? Encoding.UTF8 : this.outputEncoding, this.outputEncodingSameAsInput, this.testBoundaryConditions, this);
			return this.CreateChain(input, output2, converterWriter);
		}

		internal override IProducerConsumer CreatePushChain(ConverterWriter converterWriter, TextWriter output)
		{
			this.inputEncoding = Encoding.Unicode;
			this.outputEncoding = Encoding.Unicode;
			ConverterInput input = new ConverterUnicodeInput(converterWriter, true, this.maxTokenSize, this.testBoundaryConditions, null);
			ConverterOutput output2 = new ConverterUnicodeOutput(output, true, false);
			return this.CreateChain(input, output2, converterWriter);
		}

		internal override IProducerConsumer CreatePullChain(Stream input, ConverterStream converterStream)
		{
			if (this.inputEncoding == null)
			{
				throw new InvalidOperationException(TextConvertersStrings.InputEncodingRequired);
			}
			ConverterInput input2 = new ConverterDecodingInput(input, false, this.inputEncoding, this.detectEncodingFromByteOrderMark, this.maxTokenSize, TextConvertersDefaults.MaxHtmlMetaRestartOffset(this.testBoundaryConditions), base.InputStreamBufferSize, this.testBoundaryConditions, this, converterStream);
			ConverterOutput output = new ConverterEncodingOutput(converterStream, false, true, this.outputEncodingSameAsInput ? this.inputEncoding : this.outputEncoding, this.outputEncodingSameAsInput, this.testBoundaryConditions, this);
			return this.CreateChain(input2, output, converterStream);
		}

		internal override IProducerConsumer CreatePullChain(TextReader input, ConverterStream converterStream)
		{
			this.inputEncoding = Encoding.Unicode;
			ConverterInput input2 = new ConverterUnicodeInput(input, false, this.maxTokenSize, this.testBoundaryConditions, converterStream);
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
			ConverterInput input2 = new ConverterDecodingInput(input, false, this.inputEncoding, this.detectEncodingFromByteOrderMark, this.maxTokenSize, TextConvertersDefaults.MaxHtmlMetaRestartOffset(this.testBoundaryConditions), base.InputStreamBufferSize, this.testBoundaryConditions, this, converterReader);
			ConverterOutput output = new ConverterUnicodeOutput(converterReader, false, true);
			return this.CreateChain(input2, output, converterReader);
		}

		internal override IProducerConsumer CreatePullChain(TextReader input, ConverterReader converterReader)
		{
			this.inputEncoding = Encoding.Unicode;
			this.outputEncoding = Encoding.Unicode;
			ConverterInput input2 = new ConverterUnicodeInput(input, false, this.maxTokenSize, this.testBoundaryConditions, converterReader);
			ConverterOutput output = new ConverterUnicodeOutput(converterReader, false, true);
			return this.CreateChain(input2, output, converterReader);
		}

		private IProducerConsumer CreateChain(ConverterInput input, ConverterOutput output, IProgressMonitor progressMonitor)
		{
			this.locked = true;
			HtmlInjection injection = null;
			if (this.injectHead != null || this.injectTail != null)
			{
				injection = new HtmlInjection(this.injectHead, this.injectTail, this.injectionFormat, this.filterHtml, this.htmlCallback, this.testBoundaryConditions, null, progressMonitor);
			}
			HtmlParser parser = new HtmlParser(input, this.detectEncodingFromMetaTag, false, TextConvertersDefaults.MaxTokenRuns(this.testBoundaryConditions), TextConvertersDefaults.MaxHtmlAttributes(this.testBoundaryConditions), this.testBoundaryConditions);
			HtmlNormalizingParser parser2 = new HtmlNormalizingParser(parser, injection, false, TextConvertersDefaults.MaxHtmlNormalizerNesting(this.testBoundaryConditions), this.testBoundaryConditions, this.testNormalizerTraceStream, this.testNormalizerTraceShowTokenNum, this.testNormalizerTraceStopOnTokenNum);
			HtmlWriter writer = new HtmlWriter(output, this.filterHtml, true);
			FormatOutput output2 = new HtmlFormatOutput(writer, null, this.OutputFragment, this.TestFormatTraceStream, this.TestFormatOutputTraceStream, this.filterHtml, this.htmlCallback, false);
			return new HtmlFormatConverter(parser2, output2, false, this.testTraceStream, this.testTraceShowTokenNum, this.testTraceStopOnTokenNum, this.TestFormatConverterTraceStream, progressMonitor);
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

		private Encoding outputEncoding;

		private bool outputEncodingSameAsInput = true;

		internal bool OutputFragment;

		private HeaderFooterFormat injectionFormat;

		private string injectHead;

		private string injectTail;

		private Stream testTraceStream;

		private bool testTraceShowTokenNum = true;

		private int testTraceStopOnTokenNum;

		internal Stream TestFormatTraceStream;

		internal Stream TestFormatOutputTraceStream;

		internal Stream TestFormatConverterTraceStream;

		private bool filterHtml;

		private HtmlTagCallback htmlCallback;

		private Stream testNormalizerTraceStream;

		private bool testNormalizerTraceShowTokenNum = true;

		private int testNormalizerTraceStopOnTokenNum;

		private int maxTokenSize = 32768;
	}
}
