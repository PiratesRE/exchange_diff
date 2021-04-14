using System;
using System.IO;

namespace Microsoft.Exchange.Audio
{
	internal class PcmReader : WaveReader
	{
		internal PcmReader(string fileName)
		{
			base.Create(fileName);
		}

		protected PcmReader()
		{
		}

		protected override int MinimumLength
		{
			get
			{
				return 38;
			}
		}

		internal static bool TryCreate(string fileName, out SoundReader soundReader)
		{
			soundReader = null;
			if (AudioFile.IsWav(fileName))
			{
				soundReader = new PcmReader();
				if (!((PcmReader)soundReader).Initialize(fileName) || !((PcmReader)soundReader).IsSupportedInput())
				{
					soundReader.Dispose();
					soundReader = null;
				}
			}
			return soundReader != null;
		}

		protected override bool ReadRiffHeader(BinaryReader reader)
		{
			return this.ReadWaveChunk(reader) && this.ReadFmtChunk(reader) && this.ReadDataChunk(reader);
		}

		protected override bool ReadFmtChunk(BinaryReader reader)
		{
			if (base.GetString(reader, 4) != "fmt ")
			{
				return false;
			}
			base.FormatLength = reader.ReadInt32();
			if (base.FormatLength < 16)
			{
				return false;
			}
			base.WaveFormat = new WaveFormat();
			base.WaveFormat.FormatTag = reader.ReadInt16();
			base.WaveFormat.Channels = reader.ReadInt16();
			base.WaveFormat.SamplesPerSec = reader.ReadInt32();
			base.WaveFormat.AvgBytesPerSec = reader.ReadInt32();
			base.WaveFormat.BlockAlign = reader.ReadInt16();
			base.WaveFormat.BitsPerSample = reader.ReadInt16();
			if (base.WaveFormat.FormatTag != 1)
			{
				return false;
			}
			if (base.FormatLength > 16)
			{
				base.WaveStream.Position += (long)(base.FormatLength - 16);
			}
			return true;
		}

		private bool IsSupportedInput()
		{
			foreach (WaveFormat waveFormat in PcmReader.SupportedInputFormats)
			{
				if (base.WaveFormat.Channels == waveFormat.Channels && base.WaveFormat.FormatTag == waveFormat.FormatTag && base.WaveFormat.SamplesPerSec == waveFormat.SamplesPerSec && base.WaveFormat.BitsPerSample == waveFormat.BitsPerSample)
				{
					return true;
				}
			}
			return false;
		}

		internal const int WaveHeaderSize = 38;

		private const int ExpectedWaveFormatSize = 16;

		private static readonly WaveFormat[] SupportedInputFormats = new WaveFormat[]
		{
			WaveFormat.Pcm8WaveFormat,
			WaveFormat.Pcm16WaveFormat
		};
	}
}
