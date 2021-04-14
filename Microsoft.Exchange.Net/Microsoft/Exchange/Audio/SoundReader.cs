using System;
using System.IO;
using System.Text;

namespace Microsoft.Exchange.Audio
{
	internal abstract class SoundReader : IDisposable
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

		internal string FilePath
		{
			get
			{
				return this.filePath;
			}
		}

		protected internal long WaveDataPosition
		{
			get
			{
				return this.waveDataPosition;
			}
			protected set
			{
				this.waveDataPosition = value;
			}
		}

		protected internal int WaveDataLength
		{
			get
			{
				return this.waveDataLength;
			}
			protected set
			{
				this.waveDataLength = value;
			}
		}

		protected FileStream WaveStream
		{
			get
			{
				return this.waveStream;
			}
		}

		protected abstract int MinimumLength { get; }

		protected int FormatLength
		{
			get
			{
				return this.formatLength;
			}
			set
			{
				this.formatLength = value;
			}
		}

		public void Dispose()
		{
			if (this.waveStream != null)
			{
				this.waveStream.Close();
				this.waveStream = null;
			}
			if (this.reader != null)
			{
				this.reader.Close();
				this.reader = null;
			}
		}

		internal int Read(byte[] buffer, int count)
		{
			return this.Read(buffer, 0, count);
		}

		internal int Read(byte[] buffer, int offset, int count)
		{
			int num = this.WaveDataLength - ((int)this.WaveStream.Position - (int)this.WaveDataPosition);
			if (num < 0)
			{
				throw new InvalidWaveFormatException(this.FilePath);
			}
			int count2 = Math.Min(count, num);
			return this.WaveStream.Read(buffer, offset, count2);
		}

		protected void Create(string fileName)
		{
			if (!this.Initialize(fileName))
			{
				this.Dispose();
				throw new InvalidWaveFormatException(fileName);
			}
		}

		protected bool Initialize(string fileName)
		{
			this.filePath = fileName;
			this.waveStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
			if (this.WaveStream.Length < (long)this.MinimumLength)
			{
				return false;
			}
			bool result;
			try
			{
				this.reader = new BinaryReader(this.waveStream, Encoding.ASCII);
				result = this.ReadHeader(this.reader);
			}
			catch (EndOfStreamException)
			{
				result = false;
			}
			catch (ArgumentException)
			{
				result = false;
			}
			return result;
		}

		protected abstract bool ReadHeader(BinaryReader reader);

		private FileStream waveStream;

		private BinaryReader reader;

		private long waveDataPosition;

		private int waveDataLength;

		private WaveFormat waveFormat;

		private string filePath;

		private int formatLength;
	}
}
