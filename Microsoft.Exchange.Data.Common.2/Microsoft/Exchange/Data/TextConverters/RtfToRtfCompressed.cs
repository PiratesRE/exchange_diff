using System;
using System.IO;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.TextConverters.Internal.RtfCompressed;

namespace Microsoft.Exchange.Data.TextConverters
{
	public class RtfToRtfCompressed : TextConverter
	{
		public RtfCompressionMode CompressionMode
		{
			get
			{
				return this.compressionMode;
			}
			set
			{
				base.AssertNotLocked();
				this.compressionMode = value;
			}
		}

		internal RtfToRtfCompressed SetCompressionMode(RtfCompressionMode value)
		{
			this.CompressionMode = value;
			return this;
		}

		internal RtfToRtfCompressed SetInputStreamBufferSize(int value)
		{
			base.InputStreamBufferSize = value;
			return this;
		}

		internal RtfToRtfCompressed SetOutputStreamBufferSize(int value)
		{
			base.OutputStreamBufferSize = value;
			return this;
		}

		internal RtfToRtfCompressed SetTestBoundaryConditions(bool value)
		{
			this.testBoundaryConditions = value;
			return this;
		}

		internal override IProducerConsumer CreatePushChain(ConverterStream converterStream, Stream output)
		{
			return new RtfCompressConverter(converterStream, true, output, this.compressionMode, base.InputStreamBufferSize, base.OutputStreamBufferSize);
		}

		internal override IProducerConsumer CreatePushChain(ConverterStream converterStream, TextWriter output)
		{
			throw new NotSupportedException(TextConvertersStrings.TextWriterUnsupported);
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
			return new RtfCompressConverter(input, false, converterStream, this.compressionMode, base.InputStreamBufferSize, base.OutputStreamBufferSize);
		}

		internal override IProducerConsumer CreatePullChain(TextReader input, ConverterStream converterStream)
		{
			throw new NotSupportedException(TextConvertersStrings.TextReaderUnsupported);
		}

		internal override IProducerConsumer CreatePullChain(Stream input, ConverterReader converterReader)
		{
			throw new NotSupportedException(TextConvertersStrings.CannotUseConverterReader);
		}

		internal override IProducerConsumer CreatePullChain(TextReader input, ConverterReader converterReader)
		{
			throw new NotSupportedException(TextConvertersStrings.TextReaderUnsupported);
		}

		private RtfCompressionMode compressionMode;
	}
}
