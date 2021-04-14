using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Exchange.Audio
{
	internal abstract class WaveWriter : SoundWriter
	{
		protected abstract int WaveHeaderSize { get; }

		protected override int DataOffset
		{
			get
			{
				return this.WaveHeaderSize + 8;
			}
		}

		protected override void WriteFileHeader()
		{
			this.WriteRiffChunk();
			this.WriteFmtChunk();
			this.WriteAdditionalChunks();
			this.WriteDataChunk();
		}

		protected virtual void WriteRiffChunk()
		{
			base.Writer.Write(Encoding.ASCII.GetBytes("RIFF"));
			int value = this.WaveHeaderSize + base.NumBytesWritten;
			base.Writer.Write(value);
			base.Writer.Write(Encoding.ASCII.GetBytes("WAVE"));
		}

		protected virtual void WriteFmtChunk()
		{
			base.Writer.Write(Encoding.ASCII.GetBytes("fmt "));
			int num = (1 == base.WaveFormat.FormatTag) ? 2 : 0;
			base.Writer.Write(Marshal.SizeOf(base.WaveFormat) - num);
			base.Writer.Write(base.WaveFormat.FormatTag);
			base.Writer.Write(base.WaveFormat.Channels);
			base.Writer.Write((uint)base.WaveFormat.SamplesPerSec);
			base.Writer.Write((uint)base.WaveFormat.AvgBytesPerSec);
			base.Writer.Write(base.WaveFormat.BlockAlign);
			base.Writer.Write(base.WaveFormat.BitsPerSample);
		}

		protected abstract void WriteAdditionalChunks();

		protected virtual void WriteDataChunk()
		{
			base.Writer.Write(Encoding.ASCII.GetBytes("data"));
			base.Writer.Write(base.NumBytesWritten);
		}

		protected const int DataChunkSize = 8;

		protected const string RIFF = "RIFF";

		protected const string WAVE = "WAVE";

		protected const string FMT = "fmt ";

		protected const string DATA = "data";
	}
}
