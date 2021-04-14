using System;
using System.Text;
using Microsoft.Exchange.Conversion;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	internal sealed class LegacyDN : IEquatable<LegacyDN>
	{
		public LegacyDN(LegacyDN parentLegacyDN, string rdnPrefix, string legacyCommonName)
		{
			if (parentLegacyDN == null)
			{
				throw new ArgumentNullException("parentLegacyDN");
			}
			if (string.IsNullOrEmpty(rdnPrefix))
			{
				throw new ArgumentNullException("rdnPrefix");
			}
			if (string.IsNullOrEmpty(legacyCommonName))
			{
				throw new ArgumentNullException("legacyCommonName");
			}
			if (!LegacyDN.IsValidRdnPrefix(rdnPrefix))
			{
				throw new FormatException(DirectoryStrings.ErrorInvalidLegacyRdnPrefix(rdnPrefix));
			}
			if (!LegacyDN.IsValidLegacyCommonName(legacyCommonName))
			{
				throw new FormatException(DirectoryStrings.ErrorInvalidLegacyCommonName(legacyCommonName));
			}
			this.legacyDNString = string.Concat(new object[]
			{
				parentLegacyDN,
				"/",
				rdnPrefix,
				"=",
				legacyCommonName
			});
		}

		private LegacyDN(string legacyDNString)
		{
			this.legacyDNString = legacyDNString;
		}

		public static bool TryParse(string legacyDN, out LegacyDN result)
		{
			bool flag = LegacyDN.TryParse(legacyDN, LegacyDN.NullParserCallback.Instance);
			result = (flag ? new LegacyDN(legacyDN) : null);
			return flag;
		}

		public static LegacyDN Parse(string legacyDN)
		{
			LegacyDN result;
			if (!LegacyDN.TryParse(legacyDN, out result))
			{
				throw new FormatException(DirectoryStrings.ErrorInvalidLegacyDN(legacyDN));
			}
			return result;
		}

		public static bool IsValidLegacyDN(string legacyDN)
		{
			return LegacyDN.TryParse(legacyDN, LegacyDN.NullParserCallback.Instance);
		}

		public static bool IsValidLegacyCommonName(string cn)
		{
			return LegacyDN.IsValidLegacyCommonName(cn, 0, cn.Length);
		}

		public static string FormatAddressListDN(Guid guid)
		{
			return "/guid=" + HexConverter.ByteArrayToHexString(guid.ToByteArray());
		}

		public static string FormatTemplateGuid(Guid guid)
		{
			return LegacyDN.FormatLegacyDnFromGuid(Guid.Empty, guid);
		}

		public static string FormatLegacyDnFromGuid(Guid namingContext, Guid guid)
		{
			return "/o=NT5/ou=" + HexConverter.ByteArrayToHexString(namingContext.ToByteArray()) + "/cn=" + HexConverter.ByteArrayToHexString(guid.ToByteArray());
		}

		public static bool TryParseNspiDN(string dn, out Guid guid)
		{
			if (!string.IsNullOrEmpty(dn))
			{
				try
				{
					if (dn.Length == 38 && dn.StartsWith("/guid=", StringComparison.OrdinalIgnoreCase))
					{
						guid = new Guid(HexConverter.HexStringToByteArray(dn, 6, 32));
						return true;
					}
					if (dn.Length == 78 && dn.StartsWith("/o=NT5/ou=", StringComparison.OrdinalIgnoreCase) && string.Compare(dn, 42, "/cn=", 0, 4, StringComparison.OrdinalIgnoreCase) == 0)
					{
						new Guid(HexConverter.HexStringToByteArray(dn, 10, 32));
						guid = new Guid(HexConverter.HexStringToByteArray(dn, 46, 32));
						return true;
					}
				}
				catch (FormatException)
				{
				}
			}
			guid = Guid.Empty;
			return false;
		}

		public static string NormalizeDN(string dn)
		{
			if (string.IsNullOrEmpty(dn))
			{
				return dn;
			}
			return dn.Replace("//", "/");
		}

		private static bool IsValidLegacyCommonName(string cn, int startIndex, int length)
		{
			for (int i = 0; i < length; i++)
			{
				char c = cn[i + startIndex];
				if (c < '\0' || c >= 'Ā')
				{
					return false;
				}
				if (LegacyDN.AnsiLegacyDNMap[(int)c] != c)
				{
					return false;
				}
			}
			return true;
		}

		private static bool IsValidRdnPrefix(string prefix)
		{
			for (int i = 0; i < LegacyDN.RdnPrefixTable.Length; i++)
			{
				if (LegacyDN.RdnPrefixTable[i].Equals(prefix, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		private static bool IsValidRdnPrefix(string field, int prefixStartIndex, int prefixLength)
		{
			for (int i = 0; i < LegacyDN.RdnPrefixTable.Length; i++)
			{
				if (prefixLength == LegacyDN.RdnPrefixTable[i].Length && string.Compare(LegacyDN.RdnPrefixTable[i], 0, field, prefixStartIndex, prefixLength, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return true;
				}
			}
			return false;
		}

		private static bool TryParse(string legacyDN, LegacyDN.IParserCallback callback)
		{
			if (string.IsNullOrEmpty(legacyDN) || legacyDN.Length < 4)
			{
				return false;
			}
			if (legacyDN[0] != '/')
			{
				return false;
			}
			int num = 1;
			int num2 = legacyDN.IndexOf('/', num);
			if (num2 == -1)
			{
				num2 = legacyDN.Length;
			}
			for (;;)
			{
				int num3 = num2 - num;
				if (num3 < 3)
				{
					break;
				}
				int num4 = legacyDN.IndexOf('=', num, num3);
				if (num4 <= num || num4 == num2 - 1)
				{
					return false;
				}
				if (!LegacyDN.IsValidRdnPrefix(legacyDN, num, num4 - num))
				{
					return false;
				}
				if (!LegacyDN.IsValidLegacyCommonName(legacyDN, num4 + 1, num2 - num4 - 1))
				{
					return false;
				}
				callback.NewSection(legacyDN, num, num3, num, num4 - num, num4 + 1, num2 - num4 - 1);
				if (num2 >= legacyDN.Length)
				{
					return true;
				}
				num = num2 + 1;
				num2 = legacyDN.IndexOf('/', num);
				if (num2 == -1)
				{
					num2 = legacyDN.Length;
				}
			}
			return false;
		}

		public static string LegitimizeCN(string cn)
		{
			if (!LegacyDN.IsValidLegacyCommonName(cn))
			{
				cn = Convert.ToBase64String(Encoding.UTF8.GetBytes(cn)).Replace('=', '!').Replace('/', '&');
				if (cn.Length > 64)
				{
					cn = cn.Substring(0, 64);
				}
			}
			return cn;
		}

		public static string GenerateLegacyDN(string parentLegacyDN, ADObject obj, bool checkInvalidChar, LegacyDN.LegacyDNIsUnique dnIsUnique)
		{
			return LegacyDN.GenerateLegacyDN(parentLegacyDN, 0, obj, checkInvalidChar, dnIsUnique, null);
		}

		public static string GenerateLegacyDN(string parentLegacyDN, int suggestedMaxLength, ADObject obj, bool checkInvalidChar, LegacyDN.LegacyDNIsUnique dnIsUnique)
		{
			return LegacyDN.GenerateLegacyDN(parentLegacyDN, suggestedMaxLength, obj, checkInvalidChar, dnIsUnique, null);
		}

		public static string GenerateLegacyDN(string parentLegacyDN, int suggestedMaxLength, ADObject obj, bool checkInvalidChar, LegacyDN.LegacyDNIsUnique dnIsUnique, string cnName)
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			StringBuilder stringBuilder2 = new StringBuilder(parentLegacyDN).Append("/cn=");
			int length = stringBuilder2.Length;
			string text = cnName ?? obj.Name;
			if (checkInvalidChar && !LegacyDN.IsValidLegacyCommonName(text))
			{
				stringBuilder.Append(obj.MostDerivedObjectClass).Append(LegacyDN.GenerateRandomString(8));
			}
			else
			{
				stringBuilder.Append(text);
			}
			if (suggestedMaxLength > 0 && length + stringBuilder.Length > suggestedMaxLength && length <= suggestedMaxLength - 8)
			{
				stringBuilder.Length = suggestedMaxLength - length;
			}
			stringBuilder2.Append(stringBuilder.ToString());
			if (dnIsUnique != null)
			{
				int num = 0;
				while (num < 1000 && !dnIsUnique(stringBuilder2.ToString().Trim()))
				{
					string text2 = LegacyDN.GenerateRandomString(8);
					int num2 = stringBuilder.Length + text2.Length;
					if (num2 > 64)
					{
						stringBuilder.Length -= num2 - 64;
						num2 = 64;
					}
					if (suggestedMaxLength > 0 && length + num2 > suggestedMaxLength && length <= suggestedMaxLength - 8)
					{
						stringBuilder.Length = suggestedMaxLength - length - 8;
					}
					stringBuilder.Append(text2);
					stringBuilder2.Length = length;
					stringBuilder2.Append(stringBuilder.ToString());
					num++;
				}
				if (num >= 1000)
				{
					throw new GenerateUniqueLegacyDnException(DirectoryStrings.ErrorCannotFindUnusedLegacyDN);
				}
			}
			return stringBuilder2.ToString().Trim();
		}

		public static string GenerateLegacyDN(string parentLegacyDN, ADObject obj)
		{
			return LegacyDN.GenerateLegacyDN(parentLegacyDN, obj, false, null);
		}

		private static string GenerateRandomString(int len)
		{
			if (len > 0 && len <= 32)
			{
				return Guid.NewGuid().ToString("N").Substring(0, len);
			}
			return Guid.NewGuid().ToString("N");
		}

		public override string ToString()
		{
			return this.legacyDNString;
		}

		public LegacyDN GetChildLegacyDN(string rdnPrefix, string legacyCommonName)
		{
			return new LegacyDN(this, rdnPrefix, legacyCommonName);
		}

		public LegacyDN GetParentLegacyDN()
		{
			string text;
			string text2;
			return this.GetParentLegacyDN(out text, out text2);
		}

		public LegacyDN GetParentLegacyDN(out string childRdnPrefix, out string childCommonName)
		{
			LegacyDN.GetParentLegacyDNParserCallback getParentLegacyDNParserCallback = new LegacyDN.GetParentLegacyDNParserCallback(this.legacyDNString);
			LegacyDN.TryParse(this.legacyDNString, getParentLegacyDNParserCallback);
			childRdnPrefix = getParentLegacyDNParserCallback.ChildRdnPrefix;
			childCommonName = getParentLegacyDNParserCallback.ChildCommonName;
			return new LegacyDN(getParentLegacyDNParserCallback.ParentLegacyDN);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as LegacyDN);
		}

		public override int GetHashCode()
		{
			return LegacyDN.StringComparer.GetHashCode(this.legacyDNString);
		}

		public bool Equals(LegacyDN other)
		{
			return other != null && LegacyDN.StringComparer.Equals(this.legacyDNString, other.legacyDNString);
		}

		public const int MaximumCommonNameLength = 64;

		private const char CharX400Spec = 'ÿ';

		public const string OrganizationRdnPrefix = "o";

		public const string OrganizationalUnitRdnPrefix = "ou";

		public const string CommonNameRdnPrefix = "cn";

		public const string AlternateMailboxRdnPrefix = "guid";

		public const string AddressListPrefix = "/guid=";

		public const int AddressListPrefixLength = 6;

		private const int StringGuidLength = 32;

		private const int AddressListDnLength = 38;

		private const string NspiDnPrefix = "/o=NT5/ou=";

		private const int NspiDnPrefixLength = 10;

		private const string NspiDnSeparator = "/cn=";

		private const int NspiDnSeparatorLength = 4;

		private const int NspiDnSeparatorPosition = 42;

		private const int NspiDnLength = 78;

		private const int SuffixLen = 8;

		private static readonly char[] AnsiLegacyDNMap = new char[]
		{
			'\0',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			' ',
			'!',
			'"',
			'?',
			'?',
			'%',
			'&',
			'\'',
			'(',
			')',
			'*',
			'+',
			',',
			'-',
			'.',
			'?',
			'0',
			'1',
			'2',
			'3',
			'4',
			'5',
			'6',
			'7',
			'8',
			'9',
			':',
			'?',
			'<',
			'?',
			'>',
			'?',
			'@',
			'A',
			'B',
			'C',
			'D',
			'E',
			'F',
			'G',
			'H',
			'I',
			'J',
			'K',
			'L',
			'M',
			'N',
			'O',
			'P',
			'Q',
			'R',
			'S',
			'T',
			'U',
			'V',
			'W',
			'X',
			'Y',
			'Z',
			'[',
			'?',
			']',
			'?',
			'_',
			'?',
			'a',
			'b',
			'c',
			'd',
			'e',
			'f',
			'g',
			'h',
			'i',
			'j',
			'k',
			'l',
			'm',
			'n',
			'o',
			'p',
			'q',
			'r',
			's',
			't',
			'u',
			'v',
			'w',
			'x',
			'y',
			'z',
			'{',
			'|',
			'}',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'S',
			'?',
			'ÿ',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			'?',
			's',
			'?',
			'ÿ',
			'?',
			'?',
			'Y',
			'?',
			'?',
			'C',
			'L',
			'P',
			'Y',
			'I',
			'S',
			'?',
			'C',
			'A',
			'?',
			'?',
			'?',
			'R',
			'?',
			'?',
			'?',
			'2',
			'3',
			'?',
			'M',
			'P',
			'?',
			'?',
			'1',
			'O',
			'?',
			'?',
			'?',
			'?',
			'?',
			'A',
			'A',
			'A',
			'A',
			'ÿ',
			'A',
			'ÿ',
			'C',
			'E',
			'E',
			'E',
			'E',
			'I',
			'I',
			'I',
			'I',
			'D',
			'N',
			'O',
			'O',
			'O',
			'O',
			'ÿ',
			'X',
			'0',
			'U',
			'U',
			'U',
			'ÿ',
			'Y',
			'T',
			'ÿ',
			'a',
			'a',
			'a',
			'a',
			'ÿ',
			'a',
			'ÿ',
			'c',
			'e',
			'e',
			'e',
			'e',
			'i',
			'i',
			'i',
			'i',
			'd',
			'n',
			'o',
			'o',
			'o',
			'o',
			'ÿ',
			'?',
			'o',
			'u',
			'u',
			'u',
			'ÿ',
			'y',
			'T',
			'y'
		};

		private static readonly string[] RdnPrefixTable = new string[]
		{
			"o",
			"ou",
			"cn",
			"guid"
		};

		public static readonly StringComparer StringComparer = StringComparer.OrdinalIgnoreCase;

		private readonly string legacyDNString;

		private interface IParserCallback
		{
			void NewSection(string buffer, int startIndex, int length, int rdnPrefixStartIndex, int rdnPrefixLength, int commonNameStartIndex, int commonNameLength);
		}

		private sealed class NullParserCallback : LegacyDN.IParserCallback
		{
			private NullParserCallback()
			{
			}

			void LegacyDN.IParserCallback.NewSection(string buffer, int startIndex, int length, int rdnPrefixStartIndex, int rdnPrefixLength, int commonNameStartIndex, int commonNameLength)
			{
			}

			public static readonly LegacyDN.NullParserCallback Instance = new LegacyDN.NullParserCallback();
		}

		private sealed class GetParentLegacyDNParserCallback : LegacyDN.IParserCallback
		{
			public GetParentLegacyDNParserCallback(string sourceString)
			{
				this.sourceString = sourceString;
			}

			public string ParentLegacyDN
			{
				get
				{
					return this.sourceString.Substring(0, this.parentLegacyDNLength);
				}
			}

			public string ChildCommonName
			{
				get
				{
					return this.sourceString.Substring(this.currentCommonNameStartIndex, this.currentCommonNameLength);
				}
			}

			public string ChildRdnPrefix
			{
				get
				{
					return this.sourceString.Substring(this.currentRdnPrefixStartIndex, this.currentRdnPrefixLength);
				}
			}

			void LegacyDN.IParserCallback.NewSection(string buffer, int startIndex, int length, int rdnPrefixStartIndex, int rdnPrefixLength, int commonNameStartIndex, int commonNameLength)
			{
				this.parentLegacyDNLength = this.currentLegacyDNLength;
				this.currentLegacyDNLength = startIndex + length;
				this.currentRdnPrefixStartIndex = rdnPrefixStartIndex;
				this.currentRdnPrefixLength = rdnPrefixLength;
				this.currentCommonNameStartIndex = commonNameStartIndex;
				this.currentCommonNameLength = commonNameLength;
			}

			private readonly string sourceString;

			private int parentLegacyDNLength;

			private int currentLegacyDNLength;

			private int currentRdnPrefixStartIndex;

			private int currentRdnPrefixLength;

			private int currentCommonNameStartIndex;

			private int currentCommonNameLength;
		}

		public delegate bool LegacyDNIsUnique(string legacyDN);
	}
}
