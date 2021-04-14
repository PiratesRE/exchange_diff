using System;
using System.IO;

namespace Microsoft.Exchange.Data.Globalization
{
	internal struct CodePageDetect
	{
		public static bool IsCodePageDetectable(int cpid, bool validOnly)
		{
			byte b = CodePageDetectData.codePageIndex[cpid % CodePageDetectData.codePageIndex.Length];
			return b != byte.MaxValue && ((int)CodePageDetectData.codePages[(int)b].cpid == cpid || (CodePageDetectData.codePages[(int)b].cpid == 38598 && cpid == 28598) || (CodePageDetectData.codePages[(int)b].cpid == 936 && cpid == 54936)) && (!validOnly || 0U != (CodePageDetect.ValidCodePagesMask & CodePageDetectData.codePages[(int)b].mask));
		}

		public static char[] GetCommonExceptionCharacters()
		{
			return CodePageDetectData.commonExceptions.Clone() as char[];
		}

		public static int[] GetDefaultPriorityList()
		{
			int[] array = new int[CodePageDetectData.codePages.Length];
			for (int i = 0; i < CodePageDetectData.codePages.Length; i++)
			{
				array[i] = (int)CodePageDetectData.codePages[i].cpid;
			}
			return array;
		}

		public void Initialize()
		{
			this.maskMap = new int[CodePageDetectData.codePageMask.Length];
		}

		public void Reset()
		{
			for (int i = 0; i < this.maskMap.Length; i++)
			{
				this.maskMap[i] = 0;
			}
		}

		public void AddData(char ch)
		{
			this.maskMap[(int)CodePageDetectData.index[(int)ch]]++;
		}

		public void AddData(char[] buffer, int offset, int count)
		{
			count++;
			while (--count != 0)
			{
				this.maskMap[(int)CodePageDetectData.index[(int)buffer[offset]]]++;
				offset++;
			}
		}

		public void AddData(string buffer, int offset, int count)
		{
			count++;
			while (--count != 0)
			{
				this.maskMap[(int)CodePageDetectData.index[(int)buffer[offset]]]++;
				offset++;
			}
		}

		public void AddData(TextReader reader, int maxCharacters)
		{
			char[] array = new char[1024];
			while (maxCharacters != 0)
			{
				int count = Math.Min(array.Length, maxCharacters);
				int num = reader.Read(array, 0, count);
				if (num == 0)
				{
					return;
				}
				int num2 = 0;
				maxCharacters -= num;
				num++;
				while (--num != 0)
				{
					this.maskMap[(int)CodePageDetectData.index[(int)array[num2]]]++;
					num2++;
				}
			}
		}

		public int GetCodePage(int[] codePagePriorityList, bool allowCommonFallbackExceptions, bool allowAnyFallbackExceptions, bool onlyValidCodePages)
		{
			uint num = uint.MaxValue;
			for (int i = 0; i < this.maskMap.Length; i++)
			{
				if (this.maskMap[i] != 0)
				{
					uint num2 = CodePageDetectData.codePageMask[i];
					if (allowAnyFallbackExceptions || (allowCommonFallbackExceptions && (CodePageDetectData.fallbackMask[i] & 1U) != 0U))
					{
						num2 |= (CodePageDetectData.fallbackMask[i] & 4294967294U);
					}
					num &= num2;
					if (num == 0U)
					{
						break;
					}
				}
			}
			if (onlyValidCodePages)
			{
				num &= CodePageDetect.ValidCodePagesMask;
			}
			return CodePageDetect.GetCodePage(ref num, codePagePriorityList);
		}

		public int[] GetCodePages(int[] codePagePriorityList, bool allowCommonFallbackExceptions, bool allowAnyFallbackExceptions, bool onlyValidCodePages)
		{
			uint num = uint.MaxValue;
			for (int i = 0; i < this.maskMap.Length; i++)
			{
				if (this.maskMap[i] != 0)
				{
					uint num2 = CodePageDetectData.codePageMask[i];
					if (allowAnyFallbackExceptions || (allowCommonFallbackExceptions && (CodePageDetectData.fallbackMask[i] & 1U) != 0U))
					{
						num2 |= (CodePageDetectData.fallbackMask[i] & 4294967294U);
					}
					num &= num2;
					if (num == 0U)
					{
						break;
					}
				}
			}
			if (onlyValidCodePages)
			{
				num &= CodePageDetect.ValidCodePagesMask;
			}
			int codePageCount = CodePageDetect.GetCodePageCount(num);
			int[] array = new int[codePageCount];
			int num3 = 0;
			while (num != 0U)
			{
				array[num3++] = CodePageDetect.GetCodePage(ref num, codePagePriorityList);
			}
			array[num3] = 65001;
			return array;
		}

		public int GetCodePageCoverage(int codePage)
		{
			uint num = 0U;
			int num2 = 0;
			int num3 = 0;
			byte b = CodePageDetectData.codePageIndex[codePage % CodePageDetectData.codePageIndex.Length];
			if (b != 255 && ((int)CodePageDetectData.codePages[(int)b].cpid == codePage || (CodePageDetectData.codePages[(int)b].cpid == 38598 && codePage == 28598) || (CodePageDetectData.codePages[(int)b].cpid == 936 && codePage == 54936)))
			{
				num = CodePageDetectData.codePages[(int)b].mask;
			}
			if (num != 0U)
			{
				for (int i = 0; i < this.maskMap.Length; i++)
				{
					if (num == (CodePageDetectData.codePageMask[i] & num))
					{
						num2 += this.maskMap[i];
					}
					num3 += this.maskMap[i];
				}
			}
			if (num3 != 0)
			{
				return (int)((long)num2 * 100L / (long)num3);
			}
			return 0;
		}

		public int GetBestWindowsCodePage(bool allowCommonFallbackExceptions, bool allowAnyFallbackExceptions)
		{
			return this.GetBestWindowsCodePage(allowCommonFallbackExceptions, allowAnyFallbackExceptions, 0);
		}

		public int GetBestWindowsCodePage(bool allowCommonFallbackExceptions, bool allowAnyFallbackExceptions, int preferredCodePage)
		{
			int num = 0;
			if (this.codePageList == null)
			{
				this.codePageList = new int[CodePageDetectData.codePages.Length + 1];
			}
			else
			{
				for (int i = 0; i < this.codePageList.Length; i++)
				{
					this.codePageList[i] = 0;
				}
			}
			uint num2;
			for (int j = 0; j < this.maskMap.Length; j++)
			{
				if (this.maskMap[j] != 0)
				{
					num2 = CodePageDetectData.codePageMask[j];
					if (allowAnyFallbackExceptions || (allowCommonFallbackExceptions && (CodePageDetectData.fallbackMask[j] & 1U) != 0U))
					{
						num2 |= (CodePageDetectData.fallbackMask[j] & 4294967294U);
					}
					num2 &= 260038656U;
					if (num2 != 0U)
					{
						int num3 = 0;
						while (num2 != 0U)
						{
							if ((num2 & 1U) != 0U)
							{
								this.codePageList[num3] += this.maskMap[j];
							}
							num3++;
							num2 >>= 1;
						}
					}
					num += this.maskMap[j];
				}
			}
			int num4 = 0;
			int num5 = 0;
			num2 = 260038656U;
			int num6 = 0;
			while (num2 != 0U)
			{
				if (CodePageDetectData.codePages[num6].windowsCodePage && this.codePageList[num6] > num4)
				{
					num4 = this.codePageList[num6];
					num5 = num6;
				}
				else if (this.codePageList[num6] == num4 && (CodePageDetectData.codePages[num6].cpid == 1252 || (int)CodePageDetectData.codePages[num6].cpid == preferredCodePage))
				{
					num5 = num6;
				}
				num2 &= ~CodePageDetectData.codePages[num6].mask;
				num6++;
			}
			return (int)CodePageDetectData.codePages[num5].cpid;
		}

		internal static int GetCodePage(ref uint cumulativeMask, int[] codePagePriorityList)
		{
			if (cumulativeMask != 0U)
			{
				if (codePagePriorityList != null)
				{
					for (int i = 0; i < codePagePriorityList.Length; i++)
					{
						byte b = CodePageDetectData.codePageIndex[codePagePriorityList[i] % CodePageDetectData.codePageIndex.Length];
						if (b != 255 && (cumulativeMask & CodePageDetectData.codePages[(int)b].mask) != 0U && ((int)CodePageDetectData.codePages[(int)b].cpid == codePagePriorityList[i] || (CodePageDetectData.codePages[(int)b].cpid == 38598 && codePagePriorityList[i] == 28598) || (CodePageDetectData.codePages[(int)b].cpid == 936 && codePagePriorityList[i] == 54936)))
						{
							cumulativeMask &= ~CodePageDetectData.codePages[(int)b].mask;
							return codePagePriorityList[i];
						}
					}
				}
				for (int j = 0; j < CodePageDetectData.codePages.Length; j++)
				{
					if ((cumulativeMask & CodePageDetectData.codePages[j].mask) != 0U)
					{
						cumulativeMask &= ~CodePageDetectData.codePages[j].mask;
						return (int)CodePageDetectData.codePages[j].cpid;
					}
				}
			}
			return 65001;
		}

		internal static int GetCodePageCount(uint cumulativeMask)
		{
			int num = 1;
			while (cumulativeMask != 0U)
			{
				if ((cumulativeMask & 1U) != 0U)
				{
					num++;
				}
				cumulativeMask >>= 1;
			}
			return num;
		}

		private static uint InitializeValidCodePagesMask()
		{
			uint num = 0U;
			for (int i = 0; i < CodePageDetectData.codePages.Length; i++)
			{
				Charset charset;
				if (Charset.TryGetCharset((int)CodePageDetectData.codePages[i].cpid, out charset) && charset.IsAvailable)
				{
					num |= CodePageDetectData.codePages[i].mask;
				}
			}
			return num;
		}

		internal static uint ValidCodePagesMask = CodePageDetect.InitializeValidCodePagesMask();

		private int[] maskMap;

		private int[] codePageList;
	}
}
