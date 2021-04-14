using System;
using System.IO;

namespace Microsoft.Exchange.Audio
{
	internal class Mp3Reader : SoundReader
	{
		protected override int MinimumLength
		{
			get
			{
				return 4;
			}
		}

		internal static bool TryCreate(string fileName, out SoundReader soundReader)
		{
			soundReader = null;
			if (AudioFile.IsMp3(fileName))
			{
				Mp3Reader mp3Reader = new Mp3Reader();
				if (!mp3Reader.Initialize(fileName))
				{
					mp3Reader.Dispose();
					mp3Reader = null;
				}
				soundReader = mp3Reader;
			}
			return soundReader != null;
		}

		protected override bool ReadHeader(BinaryReader reader)
		{
			while (base.WaveStream.Position + 4L <= base.WaveStream.Length)
			{
				byte b = reader.ReadByte();
				if (b == 255)
				{
					b = reader.ReadByte();
					if ((b & 224) == 224)
					{
						int num = (b & 24) >> 3;
						Mp3Reader.ThrowIfFalse(num == 2);
						int num2 = (b & 6) >> 1;
						Mp3Reader.ThrowIfFalse(num2 == 1);
						b = reader.ReadByte();
						int num3 = (b & 240) >> 4;
						Mp3Reader.ThrowIfFalse(num3 == 2 || num3 == 4);
						int num4 = (b & 12) >> 2;
						Mp3Reader.ThrowIfFalse(num4 == 2);
						int num5 = (b & 2) >> 1;
						Mp3Reader.ThrowIfFalse(num5 == 0);
						b = reader.ReadByte();
						int num6 = (b & 192) >> 6;
						Mp3Reader.ThrowIfFalse(num6 == 3);
						base.WaveFormat = ((num3 == 4) ? MPEGLAYER3WAVEFORMAT.WideBandFormat : MPEGLAYER3WAVEFORMAT.NarrowBandFormat);
						base.WaveDataLength = (int)base.WaveStream.Length;
						base.WaveDataPosition = 0L;
						break;
					}
				}
			}
			base.WaveStream.Seek(0L, SeekOrigin.Begin);
			return base.WaveFormat != null;
		}

		private static void ThrowIfFalse(bool condition)
		{
			if (!condition)
			{
				throw new ArgumentException();
			}
		}

		private const int Mpeg16Kbps = 2;

		private const int Mpeg32Kbps = 4;

		private const int MpegLayer3 = 1;

		private const int MpegVersion20 = 2;

		private const int MpegSingleChannel = 3;

		private const int MpegBitrate16Kbps = 1;

		private const int MpegV2SamplingRate16Khz = 2;
	}
}
