using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Audio
{
	internal class GsmReader : WaveReader
	{
		internal GsmReader(string fileName)
		{
			base.Create(fileName);
		}

		protected GsmReader()
		{
		}

		protected override int MinimumLength
		{
			get
			{
				return 52;
			}
		}

		internal static bool TryCreate(string fileName, out SoundReader soundReader)
		{
			soundReader = null;
			if (AudioFile.IsWav(fileName))
			{
				soundReader = new GsmReader();
				if (!((GsmReader)soundReader).Initialize(fileName))
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
			base.WaveFormat = new GSM610WAVEFORMAT(false);
			WaveFormat waveFormat = new GSM610WAVEFORMAT(true);
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
			if (base.WaveFormat.FormatTag != waveFormat.FormatTag || base.WaveFormat.Size != waveFormat.Size)
			{
				return false;
			}
			((GSM610WAVEFORMAT)base.WaveFormat).SamplesPerBlock = reader.ReadInt16();
			return true;
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

		internal const int GsmHeaderSize = 52;
	}
}
