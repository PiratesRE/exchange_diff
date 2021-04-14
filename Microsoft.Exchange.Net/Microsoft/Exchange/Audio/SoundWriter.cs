using System;
using System.IO;
using System.Text;

namespace Microsoft.Exchange.Audio
{
	internal abstract class SoundWriter : IDisposable
	{
		public WaveFormat WaveFormat
		{
			get
			{
				return this.waveFormat;
			}
			protected set
			{
				this.waveFormat = value;
			}
		}

		protected BinaryWriter Writer
		{
			get
			{
				return this.writer;
			}
		}

		protected abstract int DataOffset { get; }

		protected int NumBytesWritten
		{
			get
			{
				return this.numBytesWritten;
			}
		}

		public void Dispose()
		{
			if (!this.closed)
			{
				this.waveStream.Seek(0L, SeekOrigin.Begin);
				this.WriteFileHeader();
				if (this.waveStream != null)
				{
					this.waveStream.Close();
					this.waveStream = null;
				}
				if (this.writer != null)
				{
					this.writer.Close();
					this.writer = null;
				}
				this.closed = true;
			}
		}

		internal void Write(byte[] buffer, int count)
		{
			this.writer.Write(buffer, 0, count);
			this.numBytesWritten += count;
		}

		protected void Create(string fileName, WaveFormat waveFormat)
		{
			this.waveStream = new FileStream(fileName, FileMode.Create);
			this.waveStream.Seek((long)this.DataOffset, SeekOrigin.Begin);
			this.writer = new BinaryWriter(this.waveStream, Encoding.ASCII);
			this.waveFormat = waveFormat;
		}

		protected virtual void WriteFileHeader()
		{
		}

		private Stream waveStream;

		private WaveFormat waveFormat;

		private bool closed;

		private int numBytesWritten;

		private BinaryWriter writer;
	}
}
