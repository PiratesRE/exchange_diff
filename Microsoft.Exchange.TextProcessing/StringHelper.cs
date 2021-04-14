using System;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.TextProcessing
{
	internal static class StringHelper
	{
		public static bool IsWhitespaceCharacter(char ch)
		{
			return (StringHelper.FindMask(ch) & 16384) != 0;
		}

		public static bool IsLeftHandSideDelimiter(char ch, BoundaryType boundaryType)
		{
			switch (boundaryType)
			{
			case BoundaryType.None:
			case BoundaryType.NormalRightOnly:
				return true;
			case BoundaryType.Normal:
			case BoundaryType.NormalLeftOnly:
				return (StringHelper.FindMask(ch) & 32768) != 0;
			case BoundaryType.Url:
				return (StringHelper.FindMask(ch) & 8192) != 0;
			case BoundaryType.FullUrl:
				return (StringHelper.FindMask(ch) & 4096) != 0;
			default:
				throw new InvalidOperationException(Strings.InvalidBoundaryType(boundaryType.ToString()));
			}
		}

		public static bool IsRightHandSideDelimiter(char ch, BoundaryType boundaryType)
		{
			switch (boundaryType)
			{
			case BoundaryType.None:
			case BoundaryType.NormalLeftOnly:
				return true;
			case BoundaryType.Normal:
			case BoundaryType.NormalRightOnly:
				return (StringHelper.FindMask(ch) & 32768) != 0;
			case BoundaryType.Url:
			case BoundaryType.FullUrl:
				return ch != '.' && ch != '@' && ch != '-' && !char.IsLetterOrDigit(ch);
			default:
				throw new InvalidOperationException(Strings.InvalidBoundaryType(boundaryType.ToString()));
			}
		}

		public static string NormalizeKeyword(string keyword)
		{
			return StringHelper.NormalizeString(StringHelper.removeSpaces.Replace(keyword.Trim(), " "));
		}

		public static string NormalizeString(string value)
		{
			return value.ToUpperInvariant();
		}

		public static ushort FindMask(char ch)
		{
			if ((int)ch > StringHelper.MaximumLookupIndex)
			{
				return 0;
			}
			return StringHelper.CharacterLookupTable[(int)ch];
		}

		private const ushort NoFlag = 0;

		private const ushort CustomFlag1 = 1;

		private const ushort CustomFlag2 = 2;

		private const ushort ABFlag = 4;

		private const ushort CDFlag = 8;

		private const ushort EFFlag = 16;

		private const ushort GHFlag = 32;

		private const ushort IJFlag = 64;

		private const ushort KLMFlag = 128;

		private const ushort NOPFlag = 256;

		private const ushort QRSFlag = 512;

		private const ushort TUVFlag = 1024;

		private const ushort WXYZFlag = 2048;

		private const ushort FullUrlFlag = 4096;

		private const ushort UrlFlag = 8192;

		private const ushort WhitespaceFlag = 16384;

		private const ushort NormalBoundaryFlag = 32768;

		private static readonly Regex removeSpaces = new Regex("\\s+", RegexOptions.Compiled);

		private static readonly ushort[] CharacterLookupTable = new ushort[]
		{
			32768,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			61440,
			61440,
			0,
			61440,
			61440,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			61440,
			32768,
			45056,
			32768,
			32768,
			32768,
			32768,
			45056,
			32768,
			32768,
			32768,
			32768,
			32768,
			32768,
			40960,
			45056,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			32768,
			45056,
			32768,
			32768,
			32768,
			32768,
			45056,
			4,
			4,
			8,
			8,
			16,
			16,
			32,
			32,
			64,
			64,
			128,
			128,
			128,
			256,
			256,
			256,
			512,
			512,
			512,
			1024,
			1024,
			1024,
			2048,
			2048,
			2048,
			2048,
			32768,
			32768,
			32768,
			32768,
			32768,
			32768,
			4,
			4,
			8,
			8,
			16,
			16,
			32,
			32,
			64,
			64,
			128,
			128,
			128,
			256,
			256,
			256,
			512,
			512,
			512,
			1024,
			1024,
			1024,
			2048,
			2048,
			2048,
			2048,
			32768,
			32768,
			32768,
			32768,
			0
		};

		private static readonly int MaximumLookupIndex = StringHelper.CharacterLookupTable.Length - 1;
	}
}
