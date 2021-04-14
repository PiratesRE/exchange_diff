using System;

namespace Microsoft.Exchange.Audio
{
	internal class PcmWriter : WaveWriter
	{
		internal PcmWriter(string fileName, WaveFormat waveFormat)
		{
			base.Create(fileName, waveFormat);
		}

		protected override int WaveHeaderSize
		{
			get
			{
				return 38;
			}
		}

		protected override void WriteAdditionalChunks()
		{
		}
	}
}
