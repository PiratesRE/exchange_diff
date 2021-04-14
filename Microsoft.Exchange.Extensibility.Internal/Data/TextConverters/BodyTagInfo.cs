using System;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal class BodyTagInfo
	{
		internal BodyTagInfo(int wordCount, int wordCrc, int formatCrc)
		{
			this.wordCount = wordCount;
			this.wordCrc = wordCrc;
			this.formatCrc = formatCrc;
		}

		public int WordCount
		{
			get
			{
				return this.wordCount;
			}
		}

		public int WordCrc
		{
			get
			{
				return this.wordCrc;
			}
		}

		public int FormatCrc
		{
			get
			{
				return this.formatCrc;
			}
		}

		public static BodyTagInfo FromByteArray(byte[] byteArray)
		{
			int num = BodyTagInfo.ReadInt(byteArray, 0);
			int num2 = BodyTagInfo.ReadInt(byteArray, 4);
			int num3 = BodyTagInfo.ReadInt(byteArray, 8);
			return new BodyTagInfo(num, num2, num3);
		}

		public static bool operator !=(BodyTagInfo left, BodyTagInfo right)
		{
			return !(left == right);
		}

		public static bool operator ==(BodyTagInfo left, BodyTagInfo right)
		{
			if (!object.ReferenceEquals(left, null))
			{
				return left.Equals(right);
			}
			return object.ReferenceEquals(right, null);
		}

		public byte[] ToByteArray()
		{
			byte[] array = new byte[12];
			BodyTagInfo.WriteInt(array, 0, this.wordCount);
			BodyTagInfo.WriteInt(array, 4, this.wordCrc);
			BodyTagInfo.WriteInt(array, 8, this.formatCrc);
			return array;
		}

		public override bool Equals(object obj)
		{
			BodyTagInfo bodyTagInfo = obj as BodyTagInfo;
			return bodyTagInfo != null && bodyTagInfo.WordCount == this.WordCount && bodyTagInfo.WordCrc == this.WordCrc && bodyTagInfo.FormatCrc == this.FormatCrc;
		}

		public override int GetHashCode()
		{
			return this.WordCrc ^ this.WordCount ^ this.FormatCrc;
		}

		private static void WriteInt(byte[] byteArray, int beginIndex, int value)
		{
			int num = 0;
			while (num < 4 && beginIndex < byteArray.Length)
			{
				byteArray[beginIndex] = (byte)(value >> num * 8);
				num++;
				beginIndex++;
			}
		}

		private static int ReadInt(byte[] byteArray, int beginIndex)
		{
			int num = 0;
			int num2 = 0;
			while (num2 < 4 && beginIndex < byteArray.Length)
			{
				num += (int)byteArray[beginIndex] << num2 * 8;
				num2++;
				beginIndex++;
			}
			return num;
		}

		private readonly int wordCount;

		private readonly int wordCrc;

		private readonly int formatCrc;
	}
}
