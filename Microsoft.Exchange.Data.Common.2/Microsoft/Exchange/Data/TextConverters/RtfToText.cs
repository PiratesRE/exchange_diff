using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.TextConverters.Internal.Rtf;
using Microsoft.Exchange.Data.TextConverters.Internal.Text;

namespace Microsoft.Exchange.Data.TextConverters
{
	public class RtfToText : TextConverter
	{
		public RtfToText()
		{
			this.textExtractionMode = TextExtractionMode.NormalConversion;
		}

		public RtfToText(TextExtractionMode textExtractionMode)
		{
			this.textExtractionMode = textExtractionMode;
		}

		public TextExtractionMode TextExtractionMode
		{
			get
			{
				return this.textExtractionMode;
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

		internal RtfToText SetOutputEncoding(Encoding value)
		{
			this.OutputEncoding = value;
			return this;
		}

		internal RtfToText SetWrap(bool value)
		{
			this.Wrap = value;
			return this;
		}

		internal RtfToText SetWrapDeleteSpace(bool value)
		{
			this.WrapDeleteSpace = value;
			return this;
		}

		internal RtfToText SetUseFallbacks(bool value)
		{
			this.fallbacks = value;
			return this;
		}

		internal RtfToText SetHtmlEscapeOutput(bool value)
		{
			this.HtmlEscapeOutput = value;
			return this;
		}

		internal RtfToText SetHeaderFooterFormat(HeaderFooterFormat value)
		{
			this.HeaderFooterFormat = value;
			return this;
		}

		internal RtfToText SetHeader(string value)
		{
			this.Header = value;
			return this;
		}

		internal RtfToText SetFooter(string value)
		{
			this.Footer = value;
			return this;
		}

		internal RtfToText SetInputStreamBufferSize(int value)
		{
			base.InputStreamBufferSize = value;
			return this;
		}

		internal RtfToText SetTestBoundaryConditions(bool value)
		{
			this.testBoundaryConditions = value;
			return this;
		}

		internal RtfToText SetTestPreserveTrailingSpaces(bool value)
		{
			this.testPreserveTrailingSpaces = value;
			return this;
		}

		internal RtfToText SetTestTreatNbspAsBreakable(bool value)
		{
			this.testTreatNbspAsBreakable = value;
			return this;
		}

		internal RtfToText SetTestTraceStream(Stream value)
		{
			this.testTraceStream = value;
			return this;
		}

		internal RtfToText SetTestTraceShowTokenNum(bool value)
		{
			this.testTraceShowTokenNum = value;
			return this;
		}

		internal RtfToText SetTestTraceStopOnTokenNum(int value)
		{
			this.testTraceStopOnTokenNum = value;
			return this;
		}

		internal RtfToText SetTestFormatTraceStream(Stream value)
		{
			this.testFormatTraceStream = value;
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
			RtfParser parser = new RtfParser(input, push, base.InputStreamBufferSize, this.testBoundaryConditions, push ? null : progressMonitor, null);
			if (this.textExtractionMode == TextExtractionMode.ExtractText)
			{
				return new RtfTextExtractionConverter(parser, output, this.testTraceStream, this.testTraceShowTokenNum, this.testTraceStopOnTokenNum);
			}
			Injection injection = null;
			if (this.injectHead != null || this.injectTail != null)
			{
				injection = ((this.injectionFormat == HeaderFooterFormat.Html) ? new HtmlInjection(this.injectHead, this.injectTail, this.injectionFormat, false, null, this.testBoundaryConditions, null, progressMonitor) : new TextInjection(this.injectHead, this.injectTail, this.testBoundaryConditions, null, progressMonitor));
			}
			TextOutput output2 = new TextOutput(output, this.wrapFlowed, this.wrapFlowed, 72, 78, null, this.fallbacks, this.htmlEscape, this.testPreserveTrailingSpaces, this.testFormatTraceStream);
			return new RtfToTextConverter(parser, output2, injection, this.testTreatNbspAsBreakable, this.testTraceStream, this.testTraceShowTokenNum, this.testTraceStopOnTokenNum);
		}

		internal override void SetResult(ConfigParameter parameterId, object val)
		{
			if (parameterId == ConfigParameter.OutputEncoding)
			{
				this.outputEncoding = (Encoding)val;
			}
			base.SetResult(parameterId, val);
		}

		private void AssertNotLockedAndNotTextExtraction()
		{
			base.AssertNotLocked();
			if (this.textExtractionMode == TextExtractionMode.ExtractText)
			{
				throw new InvalidOperationException(TextConvertersStrings.PropertyNotValidForTextExtractionMode);
			}
		}

		private TextExtractionMode textExtractionMode;

		private Encoding outputEncoding;

		private bool outputEncodingSameAsInput = true;

		private bool wrapFlowed;

		private bool wrapDelSp;

		private bool fallbacks = true;

		private bool htmlEscape;

		private HeaderFooterFormat injectionFormat;

		private string injectHead;

		private string injectTail;

		private bool testPreserveTrailingSpaces;

		private bool testTreatNbspAsBreakable;

		private Stream testTraceStream;

		private bool testTraceShowTokenNum = true;

		private int testTraceStopOnTokenNum;

		private Stream testFormatTraceStream;
	}
}
