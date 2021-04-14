using System;
using System.IO;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.TextConverters
{
	public abstract class TextConverter : IResultsFeedback
	{
		internal TextConverter()
		{
		}

		internal bool TestBoundaryConditions
		{
			get
			{
				return this.testBoundaryConditions;
			}
			set
			{
				this.AssertNotLocked();
				this.testBoundaryConditions = value;
			}
		}

		public int InputStreamBufferSize
		{
			get
			{
				return this.inputBufferSize;
			}
			set
			{
				this.AssertNotLocked();
				if (value < 1024 || value > 81920)
				{
					throw new ArgumentOutOfRangeException("value", TextConvertersStrings.BufferSizeValueRange);
				}
				this.inputBufferSize = value;
			}
		}

		public int OutputStreamBufferSize
		{
			get
			{
				return this.outputBufferSize;
			}
			set
			{
				this.AssertNotLocked();
				if (value < 1024 || value > 81920)
				{
					throw new ArgumentOutOfRangeException("value", TextConvertersStrings.BufferSizeValueRange);
				}
				this.outputBufferSize = value;
			}
		}

		public void Convert(Stream sourceStream, Stream destinationStream)
		{
			if (destinationStream == null)
			{
				throw new ArgumentNullException("destinationStream");
			}
			Stream stream = new ConverterStream(sourceStream, this, ConverterStreamAccess.Read);
			byte[] array = new byte[this.outputBufferSize];
			for (;;)
			{
				int num = stream.Read(array, 0, array.Length);
				if (num == 0)
				{
					break;
				}
				destinationStream.Write(array, 0, num);
			}
			destinationStream.Flush();
		}

		public void Convert(Stream sourceStream, TextWriter destinationWriter)
		{
			if (destinationWriter == null)
			{
				throw new ArgumentNullException("destinationWriter");
			}
			TextReader textReader = new ConverterReader(sourceStream, this);
			char[] array = new char[4096];
			for (;;)
			{
				int num = textReader.Read(array, 0, array.Length);
				if (num == 0)
				{
					break;
				}
				destinationWriter.Write(array, 0, num);
			}
			destinationWriter.Flush();
		}

		public void Convert(TextReader sourceReader, Stream destinationStream)
		{
			if (destinationStream == null)
			{
				throw new ArgumentNullException("destinationStream");
			}
			Stream stream = new ConverterStream(sourceReader, this);
			byte[] array = new byte[this.outputBufferSize];
			for (;;)
			{
				int num = stream.Read(array, 0, array.Length);
				if (num == 0)
				{
					break;
				}
				destinationStream.Write(array, 0, num);
			}
			destinationStream.Flush();
		}

		public void Convert(TextReader sourceReader, TextWriter destinationWriter)
		{
			if (destinationWriter == null)
			{
				throw new ArgumentNullException("destinationWriter");
			}
			TextReader textReader = new ConverterReader(sourceReader, this);
			char[] array = new char[4096];
			for (;;)
			{
				int num = textReader.Read(array, 0, array.Length);
				if (num == 0)
				{
					break;
				}
				destinationWriter.Write(array, 0, num);
			}
			destinationWriter.Flush();
		}

		internal abstract IProducerConsumer CreatePushChain(ConverterStream converterStream, Stream output);

		internal abstract IProducerConsumer CreatePushChain(ConverterStream converterStream, TextWriter output);

		internal abstract IProducerConsumer CreatePushChain(ConverterWriter converterWriter, Stream output);

		internal abstract IProducerConsumer CreatePushChain(ConverterWriter converterWriter, TextWriter output);

		internal abstract IProducerConsumer CreatePullChain(Stream input, ConverterStream converterStream);

		internal abstract IProducerConsumer CreatePullChain(TextReader input, ConverterStream converterStream);

		internal abstract IProducerConsumer CreatePullChain(Stream input, ConverterReader converterReader);

		internal abstract IProducerConsumer CreatePullChain(TextReader input, ConverterReader converterReader);

		internal virtual void SetResult(ConfigParameter parameterId, object val)
		{
		}

		void IResultsFeedback.Set(ConfigParameter parameterId, object val)
		{
			this.SetResult(parameterId, val);
		}

		internal void AssertNotLocked()
		{
			if (this.locked)
			{
				throw new InvalidOperationException(TextConvertersStrings.ParametersCannotBeChangedAfterConverterObjectIsUsed);
			}
		}

		protected bool testBoundaryConditions;

		private int inputBufferSize = 4096;

		private int outputBufferSize = 4096;

		protected bool locked;
	}
}
