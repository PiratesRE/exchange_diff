using System;

namespace Microsoft.Exchange.Audio
{
	internal class Mp3Writer : SoundWriter
	{
		internal Mp3Writer(string fileName, MPEGLAYER3WAVEFORMAT format)
		{
			base.Create(fileName, format);
		}

		internal static Mp3Writer Create(string outputFile, int inputSamplingRate)
		{
			if (inputSamplingRate == 8000)
			{
				return new Mp3Writer(outputFile, MPEGLAYER3WAVEFORMAT.NarrowBandFormat);
			}
			if (inputSamplingRate == 16000)
			{
				return new Mp3Writer(outputFile, MPEGLAYER3WAVEFORMAT.WideBandFormat);
			}
			throw new UnsupportedAudioFormat(outputFile);
		}

		protected override int DataOffset
		{
			get
			{
				return 0;
			}
		}
	}
}
