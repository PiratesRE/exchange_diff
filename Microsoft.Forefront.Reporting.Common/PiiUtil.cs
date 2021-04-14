using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.Forefront.Reporting.Common
{
	public static class PiiUtil
	{
		public static string GetHashPartFromPiiString(string piiString)
		{
			Match match = PiiUtil.PiiUnitRegex.Match(piiString);
			if (match.Success)
			{
				return match.Groups[1].Value;
			}
			return string.Empty;
		}

		public static string ExtractLSH(string subjectPiiString)
		{
			Match match = PiiUtil.LshExtractRegex.Match(subjectPiiString);
			if (match.Success)
			{
				return match.Groups[1].Value;
			}
			return string.Empty;
		}

		public static string ExtractHashValue(string piiString)
		{
			Match match = PiiUtil.PiiUnitRegex.Match(piiString);
			if (match.Success)
			{
				return match.Groups[2].Value;
			}
			return string.Empty;
		}

		public static bool IsValidPiiUnit(string piiString)
		{
			return PiiUtil.PiiCompleteRegex.IsMatch(piiString);
		}

		public static string PiiCleanse(string columnData)
		{
			return PiiUtil.PiiUnitRegex.Replace(columnData, "<PII$1>");
		}

		public static bool MatchLSH(string target, string query, int percentageThreshold)
		{
			return PiiUtil.MatchLSH(Convert.FromBase64String(target), Convert.FromBase64String(query), percentageThreshold);
		}

		public static List<ushort> GetShinglesFromLSH(string lshString)
		{
			List<ushort> list = new List<ushort>();
			byte[] array = Convert.FromBase64String(lshString);
			byte[] array2 = new byte[2];
			for (int i = 0; i < (array.Length - 1) / 2; i++)
			{
				array2[PiiUtil.UshortIdx0] = array[1 + 2 * i];
				array2[PiiUtil.UshortIdx1] = array[2 + 2 * i];
				list.Add(BitConverter.ToUInt16(array2, 0));
			}
			return list;
		}

		private static bool MatchLSH(byte[] target, byte[] query, int percentageThreshold)
		{
			if (target.Length < 3 || query.Length < 3 || target[0] != query[0])
			{
				return false;
			}
			int num = Math.Max(percentageThreshold * (query.Length - 1) / 2 / 100, 1);
			int num2 = 1;
			int num3 = 1;
			int num4 = 0;
			while (target.Length - num2 >= (num - num4) * 2 && query.Length - num3 >= (num - num4) * 2)
			{
				if (target[num2] > query[num3])
				{
					num3 += 2;
				}
				else if (target[num2] < query[num3])
				{
					num2 += 2;
				}
				else if (target[num2 + 1] > query[num3 + 1])
				{
					num3 += 2;
				}
				else if (target[num2 + 1] < query[num3 + 1])
				{
					num2 += 2;
				}
				else
				{
					num4++;
					if (num4 >= num)
					{
						return true;
					}
					num3 += 2;
					num2 += 2;
				}
			}
			return false;
		}

		public const string HashValueRegexPattern = ":H[0-9]{1,5}\\(((?:[^\\)]){1,1000})\\)";

		public const string AnyValueGroupRegexPatter = ":[A-Z][0-9]{1,5}\\((?:[^\\)]){1,1000}\\)";

		public const string EncryptedValueRegexPattern = ":E[0-9]{1,5}\\((?:[^\\)]){1,10000}\\)";

		public const string LSHValueRegexPattern = ":L[0-9]{1,5}\\(((?:[^\\)]){1,10000})\\)";

		public const string PiiUnitRegexPattern = "<PII(:H[0-9]{1,5}\\(((?:[^\\)]){1,1000})\\))(?::[A-Z][0-9]{1,5}\\((?:[^\\)]){1,1000}\\))*>";

		public static readonly Regex PiiUnitRegex = new Regex("<PII(:H[0-9]{1,5}\\(((?:[^\\)]){1,1000})\\))(?::[A-Z][0-9]{1,5}\\((?:[^\\)]){1,1000}\\))*>", RegexOptions.Compiled);

		public static readonly Regex PiiCompleteRegex = new Regex("^<PII(:H[0-9]{1,5}\\(((?:[^\\)]){1,1000})\\))(?::[A-Z][0-9]{1,5}\\((?:[^\\)]){1,1000}\\))*>$", RegexOptions.Compiled);

		public static readonly Regex LshExtractRegex = new Regex(":L[0-9]{1,5}\\(((?:[^\\)]){1,10000})\\)", RegexOptions.Compiled);

		public static readonly Regex EncryptedExtractRegex = new Regex(":E[0-9]{1,5}\\((?:[^\\)]){1,10000}\\)", RegexOptions.Compiled);

		private static readonly int UshortIdx0 = BitConverter.IsLittleEndian ? 1 : 0;

		private static readonly int UshortIdx1 = BitConverter.IsLittleEndian ? 0 : 1;
	}
}
