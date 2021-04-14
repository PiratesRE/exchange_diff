using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class EseLogPos
	{
		public long Generation { get; set; }

		public int Sector { get; set; }

		public int ByteOffset { get; set; }

		public static EseLogPos Parse(string input)
		{
			Regex regex = new Regex(string.Format("\\s*(?<{0}>[0-9a-fA-F]+):(?<{1}>[0-9a-fA-F]+):(?<{2}>[0-9a-fA-F]+)", "Generation", "Sector", "ByteOffset"));
			Match match = regex.Match(input);
			return new EseLogPos
			{
				Generation = long.Parse(match.Groups["Generation"].ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture),
				Sector = int.Parse(match.Groups["Sector"].ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture),
				ByteOffset = int.Parse(match.Groups["ByteOffset"].ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture)
			};
		}

		public static int CheckSectorSize(int logSectorSize)
		{
			if (logSectorSize <= 0)
			{
				logSectorSize = 512;
			}
			return logSectorSize;
		}

		public static EseLogPos BuildNextPos(EseLogPos inPos, int logRecordLen, int logSectorSize)
		{
			logSectorSize = EseLogPos.CheckSectorSize(logSectorSize);
			EseLogPos eseLogPos = new EseLogPos();
			eseLogPos.Generation = inPos.Generation;
			int num = inPos.Sector * logSectorSize + inPos.ByteOffset + logRecordLen;
			eseLogPos.Sector = num / logSectorSize;
			eseLogPos.ByteOffset = num % logSectorSize;
			return eseLogPos;
		}

		public int ToBytePos(int logSectorSize)
		{
			logSectorSize = EseLogPos.CheckSectorSize(logSectorSize);
			return this.Sector * logSectorSize + this.ByteOffset;
		}

		public override string ToString()
		{
			return string.Format("{0:X8}:{1:X4}:{2:X4}", this.Generation, this.Sector, this.ByteOffset);
		}

		private const string GenerationGroup = "Generation";

		private const string SectorGroup = "Sector";

		private const string ByteOffsetGroup = "ByteOffset";
	}
}
