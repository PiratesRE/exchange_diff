using System;

namespace Microsoft.Exchange.Data.Globalization
{
	internal struct SimpleCodepageDetector
	{
		public void AddData(char ch)
		{
			this.mask |= ~CodePageDetectData.codePageMask[(int)CodePageDetectData.index[(int)ch]];
		}

		public void AddData(char[] buffer, int offset, int count)
		{
			count++;
			while (--count != 0)
			{
				this.mask |= ~CodePageDetectData.codePageMask[(int)CodePageDetectData.index[(int)buffer[offset]]];
				offset++;
			}
		}

		public void AddData(string buffer, int offset, int count)
		{
			count++;
			while (--count != 0)
			{
				this.mask |= ~CodePageDetectData.codePageMask[(int)CodePageDetectData.index[(int)buffer[offset]]];
				offset++;
			}
		}

		public int GetCodePage(int[] codePagePriorityList, bool onlyValidCodePages)
		{
			uint num = ~this.mask;
			if (onlyValidCodePages)
			{
				num &= CodePageDetect.ValidCodePagesMask;
			}
			return CodePageDetect.GetCodePage(ref num, codePagePriorityList);
		}

		public int GetWindowsCodePage(int[] codePagePriorityList, bool onlyValidCodePages)
		{
			uint num = ~this.mask;
			num &= 260038656U;
			if (onlyValidCodePages)
			{
				num &= CodePageDetect.ValidCodePagesMask;
			}
			if (num == 0U)
			{
				return 1252;
			}
			return CodePageDetect.GetCodePage(ref num, codePagePriorityList);
		}

		private uint mask;
	}
}
