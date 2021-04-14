using System;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal class ExportSettings
	{
		public DateTime ExportTime { get; set; }

		public bool IncludeDuplicates { get; set; }

		public bool IncludeSearchableItems { get; set; }

		public bool IncludeUnsearchableItems { get; set; }

		public bool RemoveRms { get; set; }

		public static ExportSettings FromBinary(byte[] data, int startIndex)
		{
			ExportSettings exportSettings = new ExportSettings();
			exportSettings.ExportTime = DateTime.FromBinary(BitConverter.ToInt64(data, startIndex));
			int num = startIndex + 8;
			exportSettings.IncludeDuplicates = BitConverter.ToBoolean(data, num);
			num++;
			exportSettings.IncludeSearchableItems = BitConverter.ToBoolean(data, num);
			num++;
			exportSettings.IncludeUnsearchableItems = BitConverter.ToBoolean(data, num);
			num++;
			exportSettings.RemoveRms = BitConverter.ToBoolean(data, num);
			num++;
			return exportSettings;
		}

		public byte[] ToBinary()
		{
			byte[] array = new byte[256];
			int num = 0;
			ExportSettings.CopyData(BitConverter.GetBytes(this.ExportTime.ToBinary()), array, ref num);
			ExportSettings.CopyData(BitConverter.GetBytes(this.IncludeDuplicates), array, ref num);
			ExportSettings.CopyData(BitConverter.GetBytes(this.IncludeSearchableItems), array, ref num);
			ExportSettings.CopyData(BitConverter.GetBytes(this.IncludeUnsearchableItems), array, ref num);
			ExportSettings.CopyData(BitConverter.GetBytes(this.RemoveRms), array, ref num);
			return array;
		}

		private static void CopyData(byte[] source, byte[] dest, ref int destIndex)
		{
			Buffer.BlockCopy(source, 0, dest, destIndex, source.Length);
			destIndex += source.Length;
		}

		public const int BinaryLength = 256;
	}
}
