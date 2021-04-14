using System;
using System.Text;

namespace Microsoft.Exchange.Data.Globalization
{
	internal class CodePageMap : CodePageMapData
	{
		public bool ChoseCodePage(int codePage)
		{
			if (codePage == this.codePage)
			{
				return true;
			}
			this.codePage = codePage;
			this.ranges = null;
			if (codePage == 1200)
			{
				return true;
			}
			for (int i = CodePageMapData.codePages.Length - 1; i >= 0; i--)
			{
				if ((int)CodePageMapData.codePages[i].cpid == codePage)
				{
					this.ranges = CodePageMapData.codePages[i].ranges;
					this.lastRangeIndex = this.ranges.Length / 2;
					this.lastRange = this.ranges[this.lastRangeIndex];
					return true;
				}
			}
			return false;
		}

		public bool ChoseCodePage(Encoding encoding)
		{
			return this.ChoseCodePage(CodePageMap.GetCodePage(encoding));
		}

		public bool IsUnsafeExtendedCharacter(char ch)
		{
			if (this.ranges == null)
			{
				return false;
			}
			if (ch <= (char)this.lastRange.last)
			{
				if (ch >= (char)this.lastRange.first)
				{
					return this.lastRange.offset != ushort.MaxValue && (CodePageMapData.bitmap[(int)(this.lastRange.offset + (ushort)(ch - (char)this.lastRange.first))] & this.lastRange.mask) == 0;
				}
				int num = this.lastRangeIndex;
				while (--num >= 0)
				{
					if (ch >= (char)this.ranges[num].first)
					{
						if (ch > (char)this.ranges[num].last)
						{
							break;
						}
						if (ch == (char)this.ranges[num].first)
						{
							return false;
						}
						this.lastRangeIndex = num;
						this.lastRange = this.ranges[num];
						return this.lastRange.offset != ushort.MaxValue && (CodePageMapData.bitmap[(int)(this.lastRange.offset + (ushort)(ch - (char)this.lastRange.first))] & this.lastRange.mask) == 0;
					}
				}
			}
			else
			{
				int num2 = this.lastRangeIndex;
				while (++num2 < this.ranges.Length)
				{
					if (ch <= (char)this.ranges[num2].last)
					{
						if (ch < (char)this.ranges[num2].first)
						{
							break;
						}
						if (ch == (char)this.ranges[num2].first)
						{
							return false;
						}
						this.lastRangeIndex = num2;
						this.lastRange = this.ranges[num2];
						return this.lastRange.offset != ushort.MaxValue && (CodePageMapData.bitmap[(int)(this.lastRange.offset + (ushort)(ch - (char)this.lastRange.first))] & this.lastRange.mask) == 0;
					}
				}
			}
			return true;
		}

		public static Encoding GetEncoding(int codePage)
		{
			return Encoding.GetEncoding(codePage);
		}

		public static int GetCodePage(Encoding encoding)
		{
			return encoding.CodePage;
		}

		public static int GetWindowsCodePage(Encoding encoding)
		{
			return encoding.WindowsCodePage;
		}

		private int codePage;

		private CodePageMapData.CodePageRange[] ranges;

		private int lastRangeIndex;

		private CodePageMapData.CodePageRange lastRange;
	}
}
