using System;
using System.IO;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.TextConverters
{
	public class ConverterReader : TextReader, IProgressMonitor
	{
		public ConverterReader(Stream sourceStream, TextConverter converter)
		{
			if (sourceStream == null)
			{
				throw new ArgumentNullException("sourceStream");
			}
			if (converter == null)
			{
				throw new ArgumentNullException("converter");
			}
			if (!sourceStream.CanRead)
			{
				throw new ArgumentException(TextConvertersStrings.CannotReadFromSource, "sourceStream");
			}
			this.producer = converter.CreatePullChain(sourceStream, this);
			this.source = sourceStream;
			this.maxLoopsWithoutProgress = 100000 + converter.InputStreamBufferSize + converter.OutputStreamBufferSize;
		}

		public ConverterReader(TextReader sourceReader, TextConverter converter)
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
			this.source = sourceReader;
			this.maxLoopsWithoutProgress = 100000 + converter.InputStreamBufferSize + converter.OutputStreamBufferSize;
		}

		public override int Peek()
		{
			if (this.source == null)
			{
				throw new ObjectDisposedException("ConverterReader");
			}
			if (this.inconsistentState)
			{
				throw new InvalidOperationException(TextConvertersStrings.ConverterReaderInInconsistentStare);
			}
			long num = 0L;
			this.inconsistentState = true;
			while (!this.endOfFile)
			{
				char[] array;
				int num2;
				int num3;
				if (this.sourceOutputObject.GetOutputChunk(out array, out num2, out num3))
				{
					this.inconsistentState = false;
					return (int)array[num2];
				}
				this.producer.Run();
				if (this.madeProgress)
				{
					this.madeProgress = false;
					num = 0L;
				}
				else
				{
					long num4 = (long)this.maxLoopsWithoutProgress;
					long num5 = num;
					num = num5 + 1L;
					if (num4 == num5)
					{
						throw new TextConvertersException(TextConvertersStrings.TooManyIterationsToProduceOutput);
					}
				}
			}
			this.inconsistentState = false;
			return -1;
		}

		public override int Read()
		{
			if (this.source == null)
			{
				throw new ObjectDisposedException("ConverterReader");
			}
			if (this.inconsistentState)
			{
				throw new InvalidOperationException(TextConvertersStrings.ConverterReaderInInconsistentStare);
			}
			long num = 0L;
			this.inconsistentState = true;
			while (!this.endOfFile)
			{
				char[] array;
				int num2;
				int num3;
				if (this.sourceOutputObject.GetOutputChunk(out array, out num2, out num3))
				{
					this.sourceOutputObject.ReportOutput(1);
					this.inconsistentState = false;
					return (int)array[num2];
				}
				this.producer.Run();
				if (this.madeProgress)
				{
					this.madeProgress = false;
					num = 0L;
				}
				else
				{
					long num4 = (long)this.maxLoopsWithoutProgress;
					long num5 = num;
					num = num5 + 1L;
					if (num4 == num5)
					{
						throw new TextConvertersException(TextConvertersStrings.TooManyIterationsToProduceOutput);
					}
				}
			}
			this.inconsistentState = false;
			return -1;
		}

		public override int Read(char[] buffer, int index, int count)
		{
			if (this.source == null)
			{
				throw new ObjectDisposedException("ConverterReader");
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
				throw new InvalidOperationException(TextConvertersStrings.ConverterReaderInInconsistentStare);
			}
			int num = count;
			char[] src;
			int num2;
			int val;
			while (count != 0 && this.sourceOutputObject.GetOutputChunk(out src, out num2, out val))
			{
				int num3 = Math.Min(val, count);
				Buffer.BlockCopy(src, num2 * 2, buffer, index * 2, num3 * 2);
				index += num3;
				count -= num3;
				this.sourceOutputObject.ReportOutput(num3);
			}
			if (count != 0)
			{
				long num4 = 0L;
				this.writeBuffer = buffer;
				this.writeIndex = index;
				this.writeCount = count;
				this.inconsistentState = true;
				while (this.writeCount != 0 && !this.endOfFile)
				{
					this.producer.Run();
					if (this.madeProgress)
					{
						this.madeProgress = false;
						num4 = 0L;
					}
					else
					{
						long num5 = (long)this.maxLoopsWithoutProgress;
						long num6 = num4;
						num4 = num6 + 1L;
						if (num5 == num6)
						{
							throw new TextConvertersException(TextConvertersStrings.TooManyIterationsToProduceOutput);
						}
					}
				}
				count = this.writeCount;
				this.writeBuffer = null;
				this.writeIndex = 0;
				this.writeCount = 0;
				this.inconsistentState = false;
			}
			return num - count;
		}

		internal void SetSource(ConverterUnicodeOutput sourceOutputObject)
		{
			this.sourceOutputObject = sourceOutputObject;
		}

		internal void GetOutputBuffer(out char[] outputBuffer, out int outputIndex, out int outputCount)
		{
			outputBuffer = this.writeBuffer;
			outputIndex = this.writeIndex;
			outputCount = this.writeCount;
		}

		internal void ReportOutput(int outputCount)
		{
			if (outputCount != 0)
			{
				this.writeCount -= outputCount;
				this.writeIndex += outputCount;
				this.madeProgress = true;
			}
		}

		internal void ReportEndOfFile()
		{
			this.endOfFile = true;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.source != null)
				{
					if (this.source is Stream)
					{
						((Stream)this.source).Dispose();
					}
					else
					{
						((TextReader)this.source).Dispose();
					}
				}
				if (this.producer != null && this.producer is IDisposable)
				{
					((IDisposable)this.producer).Dispose();
				}
			}
			this.source = null;
			this.producer = null;
			this.sourceOutputObject = null;
			this.writeBuffer = null;
			base.Dispose(disposing);
		}

		void IProgressMonitor.ReportProgress()
		{
			this.madeProgress = true;
		}

		internal void Reuse(object newSource)
		{
			if (!(this.producer is IReusable))
			{
				throw new NotSupportedException("this converter is not reusable");
			}
			((IReusable)this.producer).Initialize(newSource);
			this.source = newSource;
			this.writeBuffer = null;
			this.writeIndex = 0;
			this.writeCount = 0;
			this.endOfFile = false;
			this.inconsistentState = false;
		}

		private ConverterUnicodeOutput sourceOutputObject;

		private IProducerConsumer producer;

		private bool madeProgress;

		private int maxLoopsWithoutProgress;

		private char[] writeBuffer;

		private int writeIndex;

		private int writeCount;

		private object source;

		private bool endOfFile;

		private bool inconsistentState;
	}
}
