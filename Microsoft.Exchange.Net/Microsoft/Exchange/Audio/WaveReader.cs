using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.Audio
{
	internal abstract class WaveReader : SoundReader
	{
		protected override bool ReadHeader(BinaryReader reader)
		{
			return this.ReadRiffHeader(reader);
		}

		protected virtual bool ReadWaveChunk(BinaryReader reader)
		{
			if (this.GetString(reader, 4) != "RIFF")
			{
				return false;
			}
			reader.ReadInt32();
			return !(this.GetString(reader, 4) != "WAVE");
		}

		protected virtual bool ReadDataChunk(BinaryReader reader)
		{
			string text = "data";
			int num = 0;
			long num2 = Math.Min(256L, base.WaveStream.Length - base.WaveStream.Position);
			while (num2 >= 0L)
			{
				if (num >= text.Length)
				{
					base.WaveDataLength = reader.ReadInt32();
					base.WaveDataPosition = base.WaveStream.Position;
					return true;
				}
				if (reader.PeekChar() == (int)text[num])
				{
					num++;
					reader.ReadChar();
					num2 -= 1L;
				}
				else if (num == 0)
				{
					reader.ReadChar();
					num2 -= 1L;
				}
				else
				{
					num = 0;
				}
			}
			ExTraceGlobals.UtilTracer.TraceDebug((long)this.GetHashCode(), "ReadDataChunk bailing because data chunk not found");
			return false;
		}

		protected abstract bool ReadRiffHeader(BinaryReader reader);

		protected abstract bool ReadFmtChunk(BinaryReader reader);

		protected string GetString(BinaryReader reader, int numBytes)
		{
			byte[] array = new byte[numBytes];
			reader.Read(array, 0, array.Length);
			return Encoding.ASCII.GetString(array);
		}
	}
}
