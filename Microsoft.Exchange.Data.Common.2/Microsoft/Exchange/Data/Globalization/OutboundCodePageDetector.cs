﻿using System;
using System.IO;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.Globalization
{
	public class OutboundCodePageDetector
	{
		public OutboundCodePageDetector()
		{
			this.detector.Initialize();
		}

		internal static bool IsCodePageDetectable(int cpid, bool onlyValid)
		{
			return CodePageDetect.IsCodePageDetectable(cpid, onlyValid);
		}

		internal static int[] GetDefaultCodePagePriorityList()
		{
			return CodePageDetect.GetDefaultPriorityList();
		}

		internal static char[] GetCommonExceptionCharacters()
		{
			return CodePageDetect.GetCommonExceptionCharacters();
		}

		public void Reset()
		{
			this.detector.Reset();
		}

		public void AddText(char ch)
		{
			this.detector.AddData(ch);
		}

		public void AddText(char[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (index < 0 || index > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("index", GlobalizationStrings.IndexOutOfRange);
			}
			if (count < 0 || count > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("count", GlobalizationStrings.CountOutOfRange);
			}
			if (buffer.Length - index < count)
			{
				throw new ArgumentOutOfRangeException("count", GlobalizationStrings.CountTooLarge);
			}
			this.detector.AddData(buffer, index, count);
		}

		public void AddText(char[] buffer)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			this.detector.AddData(buffer, 0, buffer.Length);
		}

		public void AddText(string value, int index, int count)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (index < 0 || index > value.Length)
			{
				throw new ArgumentOutOfRangeException("index", GlobalizationStrings.IndexOutOfRange);
			}
			if (count < 0 || count > value.Length)
			{
				throw new ArgumentOutOfRangeException("count", GlobalizationStrings.CountOutOfRange);
			}
			if (value.Length - index < count)
			{
				throw new ArgumentOutOfRangeException("count", GlobalizationStrings.CountTooLarge);
			}
			this.detector.AddData(value, index, count);
		}

		public void AddText(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.detector.AddData(value, 0, value.Length);
		}

		public void AddText(TextReader reader, int maxCharacters)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (maxCharacters < 0)
			{
				throw new ArgumentOutOfRangeException("maxCharacters", GlobalizationStrings.MaxCharactersCannotBeNegative);
			}
			this.detector.AddData(reader, maxCharacters);
		}

		public void AddText(TextReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			this.detector.AddData(reader, int.MaxValue);
		}

		public int GetCodePage()
		{
			return this.detector.GetCodePage(Culture.Default.CodepageDetectionPriorityOrder, false, false, true);
		}

		public int GetCodePage(Culture culture, bool allowCommonFallbackExceptions)
		{
			if (culture == null)
			{
				culture = Culture.Default;
			}
			return this.detector.GetCodePage(culture.CodepageDetectionPriorityOrder, allowCommonFallbackExceptions, false, true);
		}

		public int GetCodePage(Charset preferredCharset, bool allowCommonFallbackExceptions)
		{
			int[] array = Culture.Default.CodepageDetectionPriorityOrder;
			if (preferredCharset != null)
			{
				array = CultureCharsetDatabase.GetAdjustedCodepageDetectionPriorityOrder(preferredCharset, array);
			}
			return this.detector.GetCodePage(array, allowCommonFallbackExceptions, false, true);
		}

		internal int GetCodePage(int[] codePagePriorityList, FallbackExceptions fallbackExceptions, bool onlyValidCodePages)
		{
			if (codePagePriorityList != null)
			{
				for (int i = 0; i < codePagePriorityList.Length; i++)
				{
					if (!CodePageDetect.IsCodePageDetectable(codePagePriorityList[i], false))
					{
						throw new ArgumentException(GlobalizationStrings.PriorityListIncludesNonDetectableCodePage, "codePagePriorityList");
					}
				}
			}
			return this.detector.GetCodePage(codePagePriorityList, fallbackExceptions > FallbackExceptions.None, fallbackExceptions > FallbackExceptions.Common, onlyValidCodePages);
		}

		public int[] GetCodePages()
		{
			return this.detector.GetCodePages(Culture.Default.CodepageDetectionPriorityOrder, false, false, true);
		}

		public int[] GetCodePages(Culture culture, bool allowCommonFallbackExceptions)
		{
			if (culture == null)
			{
				culture = Culture.Default;
			}
			return this.detector.GetCodePages(culture.CodepageDetectionPriorityOrder, allowCommonFallbackExceptions, false, true);
		}

		public int[] GetCodePages(Charset preferredCharset, bool allowCommonFallbackExceptions)
		{
			int[] array = Culture.Default.CodepageDetectionPriorityOrder;
			if (preferredCharset != null)
			{
				array = CultureCharsetDatabase.GetAdjustedCodepageDetectionPriorityOrder(preferredCharset, array);
			}
			return this.detector.GetCodePages(array, allowCommonFallbackExceptions, false, true);
		}

		internal int[] GetCodePages(int[] codePagePriorityList, FallbackExceptions fallbackExceptions, bool onlyValidCodePages)
		{
			if (codePagePriorityList != null)
			{
				for (int i = 0; i < codePagePriorityList.Length; i++)
				{
					if (!CodePageDetect.IsCodePageDetectable(codePagePriorityList[i], false))
					{
						throw new ArgumentException(GlobalizationStrings.PriorityListIncludesNonDetectableCodePage, "codePagePriorityList");
					}
				}
			}
			return this.detector.GetCodePages(codePagePriorityList, fallbackExceptions > FallbackExceptions.None, fallbackExceptions > FallbackExceptions.Common, onlyValidCodePages);
		}

		public int GetCodePageCoverage(int codePage)
		{
			Charset charset = Charset.GetCharset(codePage);
			if (charset.UnicodeCoverage == CodePageUnicodeCoverage.Complete)
			{
				return 100;
			}
			if (!charset.IsDetectable)
			{
				if (charset.DetectableCodePageWithEquivalentCoverage == 0)
				{
					throw new ArgumentException("codePage is not detectable");
				}
				codePage = charset.DetectableCodePageWithEquivalentCoverage;
			}
			return this.detector.GetCodePageCoverage(codePage);
		}

		public int GetBestWindowsCodePage()
		{
			return this.detector.GetBestWindowsCodePage(false, false);
		}

		internal int GetBestWindowsCodePage(int preferredCodePage)
		{
			return this.detector.GetBestWindowsCodePage(false, false, preferredCodePage);
		}

		private CodePageDetect detector;
	}
}
