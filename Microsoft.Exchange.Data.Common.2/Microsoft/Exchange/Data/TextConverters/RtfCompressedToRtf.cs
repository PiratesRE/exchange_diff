using System;
using System.IO;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.TextConverters.Internal.RtfCompressed;

namespace Microsoft.Exchange.Data.TextConverters
{
	public class RtfCompressedToRtf : TextConverter
	{
		public RtfCompressionMode CompressionMode
		{
			get
			{
				return this.compressionMode;
			}
		}

		internal RtfCompressedToRtf SetInputStreamBufferSize(int value)
		{
			base.InputStreamBufferSize = value;
			return this;
		}

		internal RtfCompressedToRtf SetOutputStreamBufferSize(int value)
		{
			base.OutputStreamBufferSize = value;
			return this;
		}

		internal RtfCompressedToRtf SetTestDisableFastLoop(bool value)
		{
			this.testDisableFastLoop = value;
			return this;
		}

		internal RtfCompressedToRtf SetTestBoundaryConditions(bool value)
		{
			base.TestBoundaryConditions = value;
			return this;
		}

		internal override IProducerConsumer CreatePushChain(ConverterStream converterStream, Stream output)
		{
			return new RtfDecompressConverter(converterStream, true, output, this.testDisableFastLoop, this, base.InputStreamBufferSize, base.OutputStreamBufferSize);
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
			return new RtfDecompressConverter(input, false, converterStream, this.testDisableFastLoop, this, base.InputStreamBufferSize, base.OutputStreamBufferSize);
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

		internal override void SetResult(ConfigParameter parameterId, object val)
		{
			if (parameterId == ConfigParameter.RtfCompressionMode)
			{
				this.compressionMode = (RtfCompressionMode)val;
			}
			base.SetResult(parameterId, val);
		}

		private RtfCompressionMode compressionMode = RtfCompressionMode.Uncompressed;

		private bool testDisableFastLoop;
	}
}
