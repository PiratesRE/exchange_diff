using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.TextConverters.Internal.Format;
using Microsoft.Exchange.Data.TextConverters.Internal.Text;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal class TextConversationBodyScanner : ConversationBodyScanner
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

		public bool Unwrap
		{
			get
			{
				return this.unwrapFlowed;
			}
			set
			{
				base.AssertNotLocked();
				this.unwrapFlowed = value;
			}
		}

		internal bool UnwrapDeleteSpace
		{
			get
			{
				return this.unwrapDelSp;
			}
			set
			{
				base.AssertNotLocked();
				this.unwrapDelSp = value;
			}
		}

		internal TextConversationBodyScanner SetInputEncoding(Encoding value)
		{
			this.InputEncoding = value;
			return this;
		}

		internal TextConversationBodyScanner SetDetectEncodingFromByteOrderMark(bool value)
		{
			this.DetectEncodingFromByteOrderMark = value;
			return this;
		}

		internal TextConversationBodyScanner SetUnwrap(bool value)
		{
			this.Unwrap = value;
			return this;
		}

		internal TextConversationBodyScanner SetUnwrapDeleteSpace(bool value)
		{
			this.UnwrapDeleteSpace = value;
			return this;
		}

		internal TextConversationBodyScanner SetInputStreamBufferSize(int value)
		{
			base.InputStreamBufferSize = value;
			return this;
		}

		internal TextConversationBodyScanner SetFilterHtml(bool value)
		{
			base.FilterHtml = value;
			return this;
		}

		internal TextConversationBodyScanner SetHtmlTagCallback(HtmlTagCallback value)
		{
			base.HtmlTagCallback = value;
			return this;
		}

		internal TextConversationBodyScanner SetTestBoundaryConditions(bool value)
		{
			base.TestBoundaryConditions = value;
			return this;
		}

		internal TextConversationBodyScanner SetTestMaxTokenRuns(int value)
		{
			this.testMaxTokenRuns = value;
			return this;
		}

		internal TextConversationBodyScanner SetTestTraceStream(Stream value)
		{
			this.testTraceStream = value;
			return this;
		}

		internal TextConversationBodyScanner SetTestTraceShowTokenNum(bool value)
		{
			this.testTraceShowTokenNum = value;
			return this;
		}

		internal TextConversationBodyScanner SetTestTraceStopOnTokenNum(int value)
		{
			this.testTraceStopOnTokenNum = value;
			return this;
		}

		internal TextConversationBodyScanner SetTestFormatTraceStream(Stream value)
		{
			this.TestFormatTraceStream = value;
			return this;
		}

		internal TextConversationBodyScanner SetTestFormatConverterTraceStream(Stream value)
		{
			this.testFormatConverterTraceStream = value;
			return this;
		}

		internal TextConversationBodyScanner SetTestFormatOutputTraceStream(Stream value)
		{
			this.TestFormatOutputTraceStream = value;
			return this;
		}

		internal override FormatConverter CreatePullChain(Stream sourceStream, IProgressMonitor progressMonitor)
		{
			if (this.inputEncoding == null)
			{
				throw new InvalidOperationException(TextConvertersStrings.InputEncodingRequired);
			}
			ConverterInput input = new ConverterDecodingInput(sourceStream, false, this.inputEncoding, this.detectEncodingFromByteOrderMark, 4096, 0, base.InputStreamBufferSize, base.TestBoundaryConditions, this as IResultsFeedback, progressMonitor);
			return this.CreateChain(input, progressMonitor);
		}

		internal override FormatConverter CreatePullChain(TextReader sourceReader, IProgressMonitor progressMonitor)
		{
			this.inputEncoding = Encoding.Unicode;
			ConverterInput input = new ConverterUnicodeInput(sourceReader, false, 4096, base.TestBoundaryConditions, progressMonitor);
			return this.CreateChain(input, progressMonitor);
		}

		private FormatConverter CreateChain(ConverterInput input, IProgressMonitor progressMonitor)
		{
			this.RecognizeHyperlinks = true;
			this.Locked = true;
			TextParser parser = new TextParser(input, this.unwrapFlowed, this.unwrapDelSp, this.testMaxTokenRuns, base.TestBoundaryConditions);
			FormatStore store = new FormatStore();
			return new TextFormatConverter(parser, store, null, this.testTraceStream, this.testTraceShowTokenNum, this.testTraceStopOnTokenNum, this.testFormatConverterTraceStream);
		}

		private Encoding inputEncoding;

		private bool detectEncodingFromByteOrderMark = true;

		private bool unwrapFlowed;

		private bool unwrapDelSp;

		private int testMaxTokenRuns = 512;

		private Stream testTraceStream;

		private bool testTraceShowTokenNum = true;

		private int testTraceStopOnTokenNum;

		private Stream testFormatConverterTraceStream;
	}
}
