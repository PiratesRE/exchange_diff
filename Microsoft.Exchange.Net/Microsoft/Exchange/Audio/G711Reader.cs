using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Audio
{
	internal class G711Reader : PcmReader
	{
		internal G711Reader(string fileName)
		{
			base.Create(fileName);
		}

		protected G711Reader()
		{
		}

		protected override int MinimumLength
		{
			get
			{
				return 52;
			}
		}

		internal new static bool TryCreate(string fileName, out SoundReader soundReader)
		{
			soundReader = null;
			if (AudioFile.IsWav(fileName))
			{
				soundReader = new G711Reader();
				if (!((G711Reader)soundReader).Initialize(fileName))
				{
					soundReader.Dispose();
					soundReader = null;
				}
			}
			return soundReader != null;
		}

		protected override bool ReadRiffHeader(BinaryReader reader)
		{
			return this.ReadWaveChunk(reader) && this.ReadFmtChunk(reader) && this.ReadFactChunk(reader) && this.ReadDataChunk(reader);
		}

		protected override bool ReadFmtChunk(BinaryReader reader)
		{
			base.WaveFormat = new G711WAVEFORMAT(G711Format.ALAW);
			if (base.GetString(reader, 4) != "fmt ")
			{
				return false;
			}
			base.FormatLength = reader.ReadInt32();
			if (base.FormatLength != Marshal.SizeOf(base.WaveFormat))
			{
				return false;
			}
			base.WaveFormat.FormatTag = reader.ReadInt16();
			base.WaveFormat.Channels = reader.ReadInt16();
			base.WaveFormat.SamplesPerSec = reader.ReadInt32();
			base.WaveFormat.AvgBytesPerSec = reader.ReadInt32();
			base.WaveFormat.BlockAlign = reader.ReadInt16();
			base.WaveFormat.BitsPerSample = reader.ReadInt16();
			base.WaveFormat.Size = reader.ReadInt16();
			return (base.WaveFormat.FormatTag == 6 || base.WaveFormat.FormatTag == 7) && 1 == base.WaveFormat.Channels && 8000 == base.WaveFormat.SamplesPerSec && 8 == base.WaveFormat.BitsPerSample;
		}

		private bool ReadFactChunk(BinaryReader reader)
		{
			if ("fact" != base.GetString(reader, 4))
			{
				return false;
			}
			if (4 != reader.ReadInt32())
			{
				return false;
			}
			reader.ReadInt32();
			return true;
		}

		internal const int G711HeaderSize = 52;
	}
}
