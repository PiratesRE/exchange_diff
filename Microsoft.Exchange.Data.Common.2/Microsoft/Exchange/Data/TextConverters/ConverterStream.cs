using System;
using System.IO;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.TextConverters
{
	public class ConverterStream : Stream, IProgressMonitor
	{
		public ConverterStream(Stream stream, TextConverter converter, ConverterStreamAccess access)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (converter == null)
			{
				throw new ArgumentNullException("converter");
			}
			if (access < ConverterStreamAccess.Read || ConverterStreamAccess.Write < access)
			{
				throw new ArgumentException(TextConvertersStrings.AccessShouldBeReadOrWrite, "access");
			}
			if (access == ConverterStreamAccess.Read)
			{
				if (!stream.CanRead)
				{
					throw new ArgumentException(TextConvertersStrings.CannotReadFromSource, "stream");
				}
				this.producer = converter.CreatePullChain(stream, this);
			}
			else
			{
				if (!stream.CanWrite)
				{
					throw new ArgumentException(TextConvertersStrings.CannotWriteToDestination, "stream");
				}
				this.consumer = converter.CreatePushChain(this, stream);
			}
			this.sourceOrDestination = stream;
			this.maxLoopsWithoutProgress = 100000 + converter.InputStreamBufferSize + converter.OutputStreamBufferSize;
		}

		public ConverterStream(TextReader sourceReader, TextConverter converter)
		{
			if (sourceReader == null)
			{
				throw new ArgumentNullException("sourceReader");
			}
			if (converter == null)
			{
				throw new ArgumentNullException("converter");
			}
			this.producer = converter.CreatePullChain(sourceReader, this);
			this.sourceOrDestination = sourceReader;
			this.maxLoopsWithoutProgress = 100000 + converter.InputStreamBufferSize + converter.OutputStreamBufferSize;
		}

		public ConverterStream(TextWriter destinationWriter, TextConverter converter)
		{
			if (destinationWriter == null)
			{
				throw new ArgumentNullException("destinationWriter");
			}
			if (converter == null)
			{
				throw new ArgumentNullException("converter");
			}
			this.consumer = converter.CreatePushChain(this, destinationWriter);
			this.sourceOrDestination = destinationWriter;
			this.maxLoopsWithoutProgress = 100000 + converter.InputStreamBufferSize + converter.OutputStreamBufferSize;
		}

		public override bool CanRead
		{
			get
			{
				return this.producer != null;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this.consumer != null;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException(TextConvertersStrings.SeekUnsupported);
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException(TextConvertersStrings.SeekUnsupported);
			}
			set
			{
				throw new NotSupportedException(TextConvertersStrings.SeekUnsupported);
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException(TextConvertersStrings.SeekUnsupported);
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException(TextConvertersStrings.SeekUnsupported);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (this.sourceOrDestination == null)
			{
				throw new ObjectDisposedException("ConverterStream");
			}
			if (this.consumer == null)
			{
				throw new InvalidOperationException(TextConvertersStrings.WriteUnsupported);
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset > buffer.Length || offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", TextConvertersStrings.OffsetOutOfRange);
			}
			if (count > buffer.Length || count < 0)
			{
				throw new ArgumentOutOfRangeException("count", TextConvertersStrings.CountOutOfRange);
			}
			if (count + offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("count", TextConvertersStrings.CountTooLarge);
			}
			if (this.endOfFile)
			{
				throw new InvalidOperationException(TextConvertersStrings.WriteAfterFlush);
			}
			if (this.inconsistentState)
			{
				throw new InvalidOperationException(TextConvertersStrings.ConverterStreamInInconsistentStare);
			}
			this.chunkToReadBuffer = buffer;
			this.chunkToReadOffset = offset;
			this.chunkToReadCount = count;
			long num = 0L;
			this.inconsistentState = true;
			while (this.chunkToReadCount != 0)
			{
				this.consumer.Run();
				if (this.madeProgress)
				{
					num = 0L;
					this.madeProgress = false;
				}
				else
				{
					long num2 = (long)this.maxLoopsWithoutProgress;
					long num3 = num;
					num = num3 + 1L;
					if (num2 == num3)
					{
						throw new TextConvertersException(TextConvertersStrings.TooManyIterationsToProcessInput);
					}
				}
			}
			this.inconsistentState = false;
			this.chunkToReadBuffer = null;
		}

		public override void Flush()
		{
			if (this.sourceOrDestination == null)
			{
				throw new ObjectDisposedException("ConverterStream");
			}
			if (this.consumer == null)
			{
				throw new InvalidOperationException(TextConvertersStrings.WriteUnsupported);
			}
			this.endOfFile = true;
			if (!this.inconsistentState)
			{
				long num = 0L;
				this.inconsistentState = true;
				while (!this.consumer.Flush())
				{
					if (this.madeProgress)
					{
						num = 0L;
						this.madeProgress = false;
					}
					else
					{
						long num2 = (long)this.maxLoopsWithoutProgress;
						long num3 = num;
						num = num3 + 1L;
						if (num2 == num3)
						{
							throw new TextConvertersException(TextConvertersStrings.TooManyIterationsToFlushConverter);
						}
					}
				}
				this.inconsistentState = false;
			}
			if (this.sourceOrDestination is Stream)
			{
				((Stream)this.sourceOrDestination).Flush();
				return;
			}
			if (this.sourceOrDestination is TextWriter)
			{
				((TextWriter)this.sourceOrDestination).Flush();
			}
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					if (this.sourceOrDestination != null && this.consumer != null && !this.inconsistentState)
					{
						this.Flush();
					}
					if (this.producer != null && this.producer is IDisposable)
					{
						((IDisposable)this.producer).Dispose();
					}
					if (this.consumer != null && this.consumer is IDisposable)
					{
						((IDisposable)this.consumer).Dispose();
					}
				}
			}
			finally
			{
				if (disposing && this.sourceOrDestination != null)
				{
					if (this.sourceOrDestination is Stream)
					{
						((Stream)this.sourceOrDestination).Dispose();
					}
					else if (this.sourceOrDestination is TextReader)
					{
						((TextReader)this.sourceOrDestination).Dispose();
					}
					else
					{
						((TextWriter)this.sourceOrDestination).Dispose();
					}
				}
				this.sourceOrDestination = null;
				this.consumer = null;
				this.producer = null;
				this.chunkToReadBuffer = null;
				this.writeBuffer = null;
				this.byteSource = null;
			}
			base.Dispose(disposing);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (this.sourceOrDestination == null)
			{
				throw new ObjectDisposedException("ConverterStream");
			}
			if (this.producer == null)
			{
				throw new InvalidOperationException(TextConvertersStrings.ReadUnsupported);
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset > buffer.Length || offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", TextConvertersStrings.OffsetOutOfRange);
			}
			if (count > buffer.Length || count < 0)
			{
				throw new ArgumentOutOfRangeException("count", TextConvertersStrings.CountOutOfRange);
			}
			if (count + offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("count", TextConvertersStrings.CountTooLarge);
			}
			if (this.inconsistentState)
			{
				throw new InvalidOperationException(TextConvertersStrings.ConverterStreamInInconsistentStare);
			}
			int num = count;
			if (this.byteSource != null)
			{
				byte[] src;
				int srcOffset;
				int val;
				while (count != 0 && this.byteSource.GetOutputChunk(out src, out srcOffset, out val))
				{
					int num2 = Math.Min(val, count);
					Buffer.BlockCopy(src, srcOffset, buffer, offset, num2);
					offset += num2;
					count -= num2;
					this.byteSource.ReportOutput(num2);
				}
			}
			if (count != 0)
			{
				long num3 = 0L;
				this.writeBuffer = buffer;
				this.writeOffset = offset;
				this.writeCount = count;
				this.inconsistentState = true;
				while (this.writeCount != 0 && !this.endOfFile)
				{
					this.producer.Run();
					if (this.madeProgress)
					{
						num3 = 0L;
						this.madeProgress = false;
					}
					else
					{
						long num4 = (long)this.maxLoopsWithoutProgress;
						long num5 = num3;
						num3 = num5 + 1L;
						if (num4 == num5)
						{
							throw new TextConvertersException(TextConvertersStrings.TooManyIterationsToProduceOutput);
						}
					}
				}
				count = this.writeCount;
				this.writeBuffer = null;
				this.writeOffset = 0;
				this.writeCount = 0;
				this.inconsistentState = false;
			}
			return num - count;
		}

		internal void SetSource(IByteSource byteSource)
		{
			this.byteSource = byteSource;
		}

		internal void GetOutputBuffer(out byte[] outputBuffer, out int outputOffset, out int outputCount)
		{
			outputBuffer = this.writeBuffer;
			outputOffset = this.writeOffset;
			outputCount = this.writeCount;
		}

		internal void ReportOutput(int outputCount)
		{
			if (outputCount != 0)
			{
				this.madeProgress = true;
				this.writeCount -= outputCount;
				this.writeOffset += outputCount;
			}
		}

		internal void ReportEndOfFile()
		{
			this.endOfFile = true;
		}

		internal bool GetInputChunk(out byte[] chunkBuffer, out int chunkOffset, out int chunkCount, out bool eof)
		{
			chunkBuffer = this.chunkToReadBuffer;
			chunkOffset = this.chunkToReadOffset;
			chunkCount = this.chunkToReadCount;
			eof = (this.endOfFile && 0 == this.chunkToReadCount);
			return this.chunkToReadCount != 0 || this.endOfFile;
		}

		internal void ReportRead(int readCount)
		{
			if (readCount != 0)
			{
				this.madeProgress = true;
				this.chunkToReadCount -= readCount;
				this.chunkToReadOffset += readCount;
				if (this.chunkToReadCount == 0)
				{
					this.chunkToReadBuffer = null;
					this.chunkToReadOffset = 0;
				}
			}
		}

		void IProgressMonitor.ReportProgress()
		{
			this.madeProgress = true;
		}

		internal void Reuse(object newSourceOrSink)
		{
			if (this.producer != null)
			{
				if (!(this.producer is IReusable))
				{
					throw new NotSupportedException("this converter is not reusable");
				}
				((IReusable)this.producer).Initialize(newSourceOrSink);
			}
			else
			{
				if (!(this.consumer is IReusable))
				{
					throw new NotSupportedException("this converter is not reusable");
				}
				((IReusable)this.consumer).Initialize(newSourceOrSink);
			}
			this.sourceOrDestination = newSourceOrSink;
			this.chunkToReadBuffer = null;
			this.chunkToReadOffset = 0;
			this.chunkToReadCount = 0;
			this.writeBuffer = null;
			this.writeOffset = 0;
			this.writeCount = 0;
			this.endOfFile = false;
			this.inconsistentState = false;
		}

		private IProducerConsumer consumer;

		private int maxLoopsWithoutProgress;

		private bool madeProgress;

		private byte[] chunkToReadBuffer;

		private int chunkToReadOffset;

		private int chunkToReadCount;

		private IByteSource byteSource;

		private IProducerConsumer producer;

		private byte[] writeBuffer;

		private int writeOffset;

		private int writeCount;

		private object sourceOrDestination;

		private bool endOfFile;

		private bool inconsistentState;
	}
}
