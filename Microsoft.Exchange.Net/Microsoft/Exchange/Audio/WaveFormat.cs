using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Audio
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal class WaveFormat
	{
		internal WaveFormat()
		{
		}

		internal WaveFormat(int samplesPerSec, int bitsPerSample, int channels)
		{
			this.formatTag = 1;
			this.channels = (short)channels;
			this.samplesPerSec = samplesPerSec;
			this.bitsPerSample = (short)bitsPerSample;
			this.size = 0;
			this.blockAlign = (short)(channels * (bitsPerSample / 8));
			this.avgBytesPerSec = samplesPerSec * (int)this.blockAlign;
		}

		internal short FormatTag
		{
			get
			{
				return this.formatTag;
			}
			set
			{
				this.formatTag = value;
			}
		}

		internal short Channels
		{
			get
			{
				return this.channels;
			}
			set
			{
				this.channels = value;
			}
		}

		internal int SamplesPerSec
		{
			get
			{
				return this.samplesPerSec;
			}
			set
			{
				this.samplesPerSec = value;
			}
		}

		internal int AvgBytesPerSec
		{
			get
			{
				return this.avgBytesPerSec;
			}
			set
			{
				this.avgBytesPerSec = value;
			}
		}

		internal short BlockAlign
		{
			get
			{
				return this.blockAlign;
			}
			set
			{
				this.blockAlign = value;
			}
		}

		internal short BitsPerSample
		{
			get
			{
				return this.bitsPerSample;
			}
			set
			{
				this.bitsPerSample = value;
			}
		}

		internal short Size
		{
			get
			{
				return this.size;
			}
			set
			{
				this.size = value;
			}
		}

		internal static readonly WaveFormat Pcm8WaveFormat = new WaveFormat(8000, 16, 1);

		internal static readonly WaveFormat Pcm16WaveFormat = new WaveFormat(16000, 16, 1);

		private short formatTag;

		private short channels;

		private int samplesPerSec;

		private int avgBytesPerSec;

		private short blockAlign;

		private short bitsPerSample;

		private short size;
	}
}
