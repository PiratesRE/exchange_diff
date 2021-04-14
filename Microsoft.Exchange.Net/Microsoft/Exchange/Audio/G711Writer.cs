using System;
using System.Text;

namespace Microsoft.Exchange.Audio
{
	internal class G711Writer : WaveWriter
	{
		internal G711Writer(string fileName, G711Format format)
		{
			base.Create(fileName, new G711WAVEFORMAT(format));
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
			G711WAVEFORMAT g711WAVEFORMAT = (G711WAVEFORMAT)base.WaveFormat;
			base.WriteFmtChunk();
			base.Writer.Write(g711WAVEFORMAT.Size);
		}

		protected override void WriteAdditionalChunks()
		{
			base.Writer.Write(Encoding.ASCII.GetBytes("fact"));
			base.Writer.Write(4);
			base.Writer.Write(base.NumBytesWritten);
		}
	}
}
