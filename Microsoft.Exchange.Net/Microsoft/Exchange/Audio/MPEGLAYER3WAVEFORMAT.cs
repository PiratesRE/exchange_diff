using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Audio
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
	internal class MPEGLAYER3WAVEFORMAT : WaveFormat
	{
		internal MPEGLAYER3WAVEFORMAT(int bitRate, int samplingRate)
		{
			int num = 144 * bitRate / samplingRate;
			base.FormatTag = 85;
			base.Channels = 1;
			base.SamplesPerSec = samplingRate;
			base.AvgBytesPerSec = bitRate / 8;
			base.BlockAlign = 1;
			base.BitsPerSample = 0;
			base.Size = 12;
			this.ID = 1;
			this.Flags = 2;
			this.FramesPerBlock = 1;
			this.BlockSize = (short)num;
			this.CodecDelay = 1393;
		}

		internal short ID { get; set; }

		internal int Flags { get; set; }

		internal short BlockSize { get; set; }

		internal short FramesPerBlock { get; set; }

		internal short CodecDelay { get; set; }

		internal const short MP3FormatTag = 85;

		internal static readonly MPEGLAYER3WAVEFORMAT WideBandFormat = new MPEGLAYER3WAVEFORMAT(32000, 16000);

		internal static readonly MPEGLAYER3WAVEFORMAT NarrowBandFormat = new MPEGLAYER3WAVEFORMAT(16000, 16000);
	}
}
