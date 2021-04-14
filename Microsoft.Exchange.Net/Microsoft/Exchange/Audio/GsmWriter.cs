using System;
using System.Text;

namespace Microsoft.Exchange.Audio
{
	internal class GsmWriter : WaveWriter
	{
		internal GsmWriter(string fileName)
		{
			base.Create(fileName, new GSM610WAVEFORMAT(true));
		}

		protected override int WaveHeaderSize
		{
			get
			{
				return 52;
			}
		}

		protected override void WriteFmtChunk()
		{
			GSM610WAVEFORMAT gsm610WAVEFORMAT = (GSM610WAVEFORMAT)base.WaveFormat;
			base.WriteFmtChunk();
			base.Writer.Write(gsm610WAVEFORMAT.Size);
			base.Writer.Write(gsm610WAVEFORMAT.SamplesPerBlock);
		}

		protected override void WriteAdditionalChunks()
		{
			base.Writer.Write(Encoding.ASCII.GetBytes("fact"));
			base.Writer.Write(4);
			base.Writer.Write(base.NumBytesWritten / (int)base.WaveFormat.BlockAlign * (int)((GSM610WAVEFORMAT)base.WaveFormat).SamplesPerBlock);
		}
	}
}
