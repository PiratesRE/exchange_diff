using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Audio
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
	internal class GSM610WAVEFORMAT : WaveFormat
	{
		internal GSM610WAVEFORMAT(bool useDefaults)
		{
			if (useDefaults)
			{
				base.FormatTag = 49;
				base.Channels = 1;
				base.SamplesPerSec = 8000;
				base.AvgBytesPerSec = 1625;
				base.BlockAlign = 65;
				base.BitsPerSample = 0;
				base.Size = 2;
				this.samplesPerBlock = 320;
			}
		}

		internal short SamplesPerBlock
		{
			get
			{
				return this.samplesPerBlock;
			}
			set
			{
				this.samplesPerBlock = value;
			}
		}

		private short samplesPerBlock;
	}
}
