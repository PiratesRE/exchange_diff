using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.TextConverters
{
	public class ConverterWriter : TextWriter, IProgressMonitor
	{
		public ConverterWriter(Stream destinationStream, TextConverter converter)
		{
			if (destinationStream == null)
			{
				throw new ArgumentNullException("destinationStream");
			}
			if (converter == null)
			{
				throw new ArgumentNullException("converter");
			}
			if (!destinationStream.CanWrite)
			{
				throw new ArgumentException(TextConvertersStrings.CannotWriteToDestination, "destinationStream");
			}
			this.consumer = converter.CreatePushChain(this, destinationStream);
			this.destination = destinationStream;
			this.boundaryTesting = converter.TestBoundaryConditions;
			this.maxLoopsWithoutProgress = 100000 + converter.InputStreamBufferSize + converter.OutputStreamBufferSize;
		}

		public ConverterWriter(TextWriter destinationWriter, TextConverter converter)
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
			this.destination = destinationWriter;
			this.boundaryTesting = converter.TestBoundaryConditions;
			this.maxLoopsWithoutProgress = 100000 + converter.InputStreamBufferSize + converter.OutputStreamBufferSize;
		}

		public override Encoding Encoding
		{
			get
			{
				return null;
			}
		}

		public override void Flush()
		{
			if (this.destination == null)
			{
				throw new ObjectDisposedException("ConverterWriter");
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
			if (this.destination is Stream)
			{
				((Stream)this.destination).Flush();
				return;
			}
			((TextWriter)this.destination).Flush();
		}

		public override void Write(char value)
		{
			if (this.destination == null)
			{
				throw new ObjectDisposedException("ConverterWriter");
			}
			if (this.inconsistentState)
			{
				throw new InvalidOperationException(TextConvertersStrings.ConverterWriterInInconsistentStare);
			}
			int parseCount = 10000;
			if (!this.boundaryTesting)
			{
				char[] array;
				int num;
				int num2;
				this.sinkInputObject.GetInputBuffer(out array, out num, out num2, out parseCount);
				if (num2 >= 1)
				{
					array[num] = value;
					this.sinkInputObject.Commit(1);
					return;
				}
			}
			char[] buffer = new char[]
			{
				value
			};
			this.WriteBig(buffer, 0, 1, parseCount);
		}

		public override void Write(char[] buffer)
		{
			if (this.destination == null)
			{
				throw new ObjectDisposedException("ConverterWriter");
			}
			if (this.inconsistentState)
			{
				throw new InvalidOperationException(TextConvertersStrings.ConverterWriterInInconsistentStare);
			}
			if (buffer == null)
			{
				return;
			}
			int parseCount = 10000;
			if (!this.boundaryTesting)
			{
				char[] dst;
				int num;
				int num2;
				this.sinkInputObject.GetInputBuffer(out dst, out num, out num2, out parseCount);
				if (num2 >= buffer.Length)
				{
					Buffer.BlockCopy(buffer, 0, dst, num * 2, buffer.Length * 2);
					this.sinkInputObject.Commit(buffer.Length);
					return;
				}
			}
			this.WriteBig(buffer, 0, buffer.Length, parseCount);
		}

		public override void Write(char[] buffer, int index, int count)
		{
			if (this.destination == null)
			{
				throw new ObjectDisposedException("ConverterWriter");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (index < 0 || index > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("index", TextConvertersStrings.IndexOutOfRange);
			}
			if (count < 0 || count > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("count", TextConvertersStrings.CountOutOfRange);
			}
			if (buffer.Length - index < count)
			{
				throw new ArgumentOutOfRangeException("count", TextConvertersStrings.CountTooLarge);
			}
			if (this.inconsistentState)
			{
				throw new InvalidOperationException(TextConvertersStrings.ConverterWriterInInconsistentStare);
			}
			int parseCount = 10000;
			if (!this.boundaryTesting)
			{
				char[] dst;
				int num;
				int num2;
				this.sinkInputObject.GetInputBuffer(out dst, out num, out num2, out parseCount);
				if (num2 >= count)
				{
					Buffer.BlockCopy(buffer, index * 2, dst, num * 2, count * 2);
					this.sinkInputObject.Commit(count);
					return;
				}
			}
			this.WriteBig(buffer, index, count, parseCount);
		}

		public override void Write(string value)
		{
			if (this.destination == null)
			{
				throw new ObjectDisposedException("ConverterWriter");
			}
			if (this.inconsistentState)
			{
				throw new InvalidOperationException(TextConvertersStrings.ConverterWriterInInconsistentStare);
			}
			if (value == null)
			{
				return;
			}
			int parseCount = 10000;
			if (!this.boundaryTesting)
			{
				char[] array;
				int destinationIndex;
				int num;
				this.sinkInputObject.GetInputBuffer(out array, out destinationIndex, out num, out parseCount);
				if (num >= value.Length)
				{
					value.CopyTo(0, array, destinationIndex, value.Length);
					this.sinkInputObject.Commit(value.Length);
					return;
				}
			}
			char[] buffer = value.ToCharArray();
			this.WriteBig(buffer, 0, value.Length, parseCount);
		}

		public override void WriteLine(string value)
		{
			this.Write(value);
			this.WriteLine();
		}

		internal void SetSink(ConverterUnicodeInput sinkInputObject)
		{
			this.sinkInputObject = sinkInputObject;
		}

		internal bool GetInputChunk(out char[] chunkBuffer, out int chunkIndex, out int chunkCount, out bool eof)
		{
			chunkBuffer = this.chunkToReadBuffer;
			chunkIndex = this.chunkToReadIndex;
			chunkCount = this.chunkToReadCount;
			eof = (this.endOfFile && 0 == this.chunkToReadCount);
			return this.chunkToReadCount != 0 || this.endOfFile;
		}

		internal void ReportRead(int readCount)
		{
			if (readCount != 0)
			{
				this.chunkToReadCount -= readCount;
				this.chunkToReadIndex += readCount;
				if (this.chunkToReadCount == 0)
				{
					this.chunkToReadBuffer = null;
					this.chunkToReadIndex = 0;
				}
				this.madeProgress = true;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.destination != null)
				{
					if (!this.inconsistentState)
					{
						this.Flush();
					}
					if (this.destination is Stream)
					{
						((Stream)this.destination).Dispose();
					}
					else
					{
						((TextWriter)this.destination).Dispose();
					}
				}
				if (this.consumer != null && this.consumer is IDisposable)
				{
					((IDisposable)this.consumer).Dispose();
				}
			}
			this.destination = null;
			this.consumer = null;
			this.sinkInputObject = null;
			this.chunkToReadBuffer = null;
			base.Dispose(disposing);
		}

		private void WriteBig(char[] buffer, int index, int count, int parseCount)
		{
			this.chunkToReadBuffer = buffer;
			this.chunkToReadIndex = index;
			this.chunkToReadCount = count;
			long num = 0L;
			this.inconsistentState = true;
			while (this.chunkToReadCount != 0)
			{
				this.consumer.Run();
				if (this.madeProgress)
				{
					this.madeProgress = false;
					num = 0L;
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
		}

		void IProgressMonitor.ReportProgress()
		{
			this.madeProgress = true;
		}

		internal void Reuse(object newSink)
		{
			if (!(this.consumer is IReusable))
			{
				throw new NotSupportedException("this converter is not reusable");
			}
			((IReusable)this.consumer).Initialize(newSink);
			this.destination = newSink;
			this.chunkToReadBuffer = null;
			this.chunkToReadIndex = 0;
			this.chunkToReadCount = 0;
			this.endOfFile = false;
			this.inconsistentState = false;
		}

		private ConverterUnicodeInput sinkInputObject;

		private IProducerConsumer consumer;

		private bool madeProgress;

		private int maxLoopsWithoutProgress;

		private char[] chunkToReadBuffer;

		private int chunkToReadIndex;

		private int chunkToReadCount;

		private object destination;

		private bool endOfFile;

		private bool inconsistentState;

		private bool boundaryTesting;
	}
}
