using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal struct UnicodeToGsmMap
	{
		static UnicodeToGsmMap()
		{
			UnicodeToGsmMap.unofficialMaps.Add(231, new UnicodeToGsmMap(231, 9, false));
			UnicodeToGsmMap.unofficialMaps.Add(913, new UnicodeToGsmMap(913, 65, false));
			UnicodeToGsmMap.unofficialMaps.Add(914, new UnicodeToGsmMap(914, 66, false));
			UnicodeToGsmMap.unofficialMaps.Add(917, new UnicodeToGsmMap(917, 69, false));
			UnicodeToGsmMap.unofficialMaps.Add(918, new UnicodeToGsmMap(918, 90, false));
			UnicodeToGsmMap.unofficialMaps.Add(919, new UnicodeToGsmMap(919, 72, false));
			UnicodeToGsmMap.unofficialMaps.Add(921, new UnicodeToGsmMap(921, 73, false));
			UnicodeToGsmMap.unofficialMaps.Add(922, new UnicodeToGsmMap(922, 75, false));
			UnicodeToGsmMap.unofficialMaps.Add(924, new UnicodeToGsmMap(924, 77, false));
			UnicodeToGsmMap.unofficialMaps.Add(925, new UnicodeToGsmMap(925, 78, false));
			UnicodeToGsmMap.unofficialMaps.Add(927, new UnicodeToGsmMap(927, 79, false));
			UnicodeToGsmMap.unofficialMaps.Add(929, new UnicodeToGsmMap(929, 80, false));
			UnicodeToGsmMap.unofficialMaps.Add(932, new UnicodeToGsmMap(932, 84, false));
			UnicodeToGsmMap.unofficialMaps.Add(933, new UnicodeToGsmMap(933, 85, false));
			UnicodeToGsmMap.unofficialMaps.Add(935, new UnicodeToGsmMap(935, 88, false));
			UnicodeToGsmMap.officialMaps.Add(10, new UnicodeToGsmMap(10, 10, true));
			UnicodeToGsmMap.officialMaps.Add(12, new UnicodeToGsmMap(12, 6922, true));
			UnicodeToGsmMap.officialMaps.Add(13, new UnicodeToGsmMap(13, 13, true));
			UnicodeToGsmMap.officialMaps.Add(32, new UnicodeToGsmMap(32, 32, true));
			UnicodeToGsmMap.officialMaps.Add(33, new UnicodeToGsmMap(33, 33, true));
			UnicodeToGsmMap.officialMaps.Add(34, new UnicodeToGsmMap(34, 34, true));
			UnicodeToGsmMap.officialMaps.Add(35, new UnicodeToGsmMap(35, 35, true));
			UnicodeToGsmMap.officialMaps.Add(36, new UnicodeToGsmMap(36, 2, true));
			UnicodeToGsmMap.officialMaps.Add(37, new UnicodeToGsmMap(37, 37, true));
			UnicodeToGsmMap.officialMaps.Add(38, new UnicodeToGsmMap(38, 38, true));
			UnicodeToGsmMap.officialMaps.Add(39, new UnicodeToGsmMap(39, 39, true));
			UnicodeToGsmMap.officialMaps.Add(40, new UnicodeToGsmMap(40, 40, true));
			UnicodeToGsmMap.officialMaps.Add(41, new UnicodeToGsmMap(41, 41, true));
			UnicodeToGsmMap.officialMaps.Add(42, new UnicodeToGsmMap(42, 42, true));
			UnicodeToGsmMap.officialMaps.Add(43, new UnicodeToGsmMap(43, 43, true));
			UnicodeToGsmMap.officialMaps.Add(44, new UnicodeToGsmMap(44, 44, true));
			UnicodeToGsmMap.officialMaps.Add(45, new UnicodeToGsmMap(45, 45, true));
			UnicodeToGsmMap.officialMaps.Add(46, new UnicodeToGsmMap(46, 46, true));
			UnicodeToGsmMap.officialMaps.Add(47, new UnicodeToGsmMap(47, 47, true));
			UnicodeToGsmMap.officialMaps.Add(48, new UnicodeToGsmMap(48, 48, true));
			UnicodeToGsmMap.officialMaps.Add(49, new UnicodeToGsmMap(49, 49, true));
			UnicodeToGsmMap.officialMaps.Add(50, new UnicodeToGsmMap(50, 50, true));
			UnicodeToGsmMap.officialMaps.Add(51, new UnicodeToGsmMap(51, 51, true));
			UnicodeToGsmMap.officialMaps.Add(52, new UnicodeToGsmMap(52, 52, true));
			UnicodeToGsmMap.officialMaps.Add(53, new UnicodeToGsmMap(53, 53, true));
			UnicodeToGsmMap.officialMaps.Add(54, new UnicodeToGsmMap(54, 54, true));
			UnicodeToGsmMap.officialMaps.Add(55, new UnicodeToGsmMap(55, 55, true));
			UnicodeToGsmMap.officialMaps.Add(56, new UnicodeToGsmMap(56, 56, true));
			UnicodeToGsmMap.officialMaps.Add(57, new UnicodeToGsmMap(57, 57, true));
			UnicodeToGsmMap.officialMaps.Add(58, new UnicodeToGsmMap(58, 58, true));
			UnicodeToGsmMap.officialMaps.Add(59, new UnicodeToGsmMap(59, 59, true));
			UnicodeToGsmMap.officialMaps.Add(60, new UnicodeToGsmMap(60, 60, true));
			UnicodeToGsmMap.officialMaps.Add(61, new UnicodeToGsmMap(61, 61, true));
			UnicodeToGsmMap.officialMaps.Add(62, new UnicodeToGsmMap(62, 62, true));
			UnicodeToGsmMap.officialMaps.Add(63, new UnicodeToGsmMap(63, 63, true));
			UnicodeToGsmMap.officialMaps.Add(64, new UnicodeToGsmMap(64, 0, true));
			UnicodeToGsmMap.officialMaps.Add(65, new UnicodeToGsmMap(65, 65, true));
			UnicodeToGsmMap.officialMaps.Add(66, new UnicodeToGsmMap(66, 66, true));
			UnicodeToGsmMap.officialMaps.Add(67, new UnicodeToGsmMap(67, 67, true));
			UnicodeToGsmMap.officialMaps.Add(68, new UnicodeToGsmMap(68, 68, true));
			UnicodeToGsmMap.officialMaps.Add(69, new UnicodeToGsmMap(69, 69, true));
			UnicodeToGsmMap.officialMaps.Add(70, new UnicodeToGsmMap(70, 70, true));
			UnicodeToGsmMap.officialMaps.Add(71, new UnicodeToGsmMap(71, 71, true));
			UnicodeToGsmMap.officialMaps.Add(72, new UnicodeToGsmMap(72, 72, true));
			UnicodeToGsmMap.officialMaps.Add(73, new UnicodeToGsmMap(73, 73, true));
			UnicodeToGsmMap.officialMaps.Add(74, new UnicodeToGsmMap(74, 74, true));
			UnicodeToGsmMap.officialMaps.Add(75, new UnicodeToGsmMap(75, 75, true));
			UnicodeToGsmMap.officialMaps.Add(76, new UnicodeToGsmMap(76, 76, true));
			UnicodeToGsmMap.officialMaps.Add(77, new UnicodeToGsmMap(77, 77, true));
			UnicodeToGsmMap.officialMaps.Add(78, new UnicodeToGsmMap(78, 78, true));
			UnicodeToGsmMap.officialMaps.Add(79, new UnicodeToGsmMap(79, 79, true));
			UnicodeToGsmMap.officialMaps.Add(80, new UnicodeToGsmMap(80, 80, true));
			UnicodeToGsmMap.officialMaps.Add(81, new UnicodeToGsmMap(81, 81, true));
			UnicodeToGsmMap.officialMaps.Add(82, new UnicodeToGsmMap(82, 82, true));
			UnicodeToGsmMap.officialMaps.Add(83, new UnicodeToGsmMap(83, 83, true));
			UnicodeToGsmMap.officialMaps.Add(84, new UnicodeToGsmMap(84, 84, true));
			UnicodeToGsmMap.officialMaps.Add(85, new UnicodeToGsmMap(85, 85, true));
			UnicodeToGsmMap.officialMaps.Add(86, new UnicodeToGsmMap(86, 86, true));
			UnicodeToGsmMap.officialMaps.Add(87, new UnicodeToGsmMap(87, 87, true));
			UnicodeToGsmMap.officialMaps.Add(88, new UnicodeToGsmMap(88, 88, true));
			UnicodeToGsmMap.officialMaps.Add(89, new UnicodeToGsmMap(89, 89, true));
			UnicodeToGsmMap.officialMaps.Add(90, new UnicodeToGsmMap(90, 90, true));
			UnicodeToGsmMap.officialMaps.Add(91, new UnicodeToGsmMap(91, 6972, true));
			UnicodeToGsmMap.officialMaps.Add(92, new UnicodeToGsmMap(92, 6959, true));
			UnicodeToGsmMap.officialMaps.Add(93, new UnicodeToGsmMap(93, 6974, true));
			UnicodeToGsmMap.officialMaps.Add(94, new UnicodeToGsmMap(94, 6932, true));
			UnicodeToGsmMap.officialMaps.Add(95, new UnicodeToGsmMap(95, 17, true));
			UnicodeToGsmMap.officialMaps.Add(97, new UnicodeToGsmMap(97, 97, true));
			UnicodeToGsmMap.officialMaps.Add(98, new UnicodeToGsmMap(98, 98, true));
			UnicodeToGsmMap.officialMaps.Add(99, new UnicodeToGsmMap(99, 99, true));
			UnicodeToGsmMap.officialMaps.Add(100, new UnicodeToGsmMap(100, 100, true));
			UnicodeToGsmMap.officialMaps.Add(101, new UnicodeToGsmMap(101, 101, true));
			UnicodeToGsmMap.officialMaps.Add(102, new UnicodeToGsmMap(102, 102, true));
			UnicodeToGsmMap.officialMaps.Add(103, new UnicodeToGsmMap(103, 103, true));
			UnicodeToGsmMap.officialMaps.Add(104, new UnicodeToGsmMap(104, 104, true));
			UnicodeToGsmMap.officialMaps.Add(105, new UnicodeToGsmMap(105, 105, true));
			UnicodeToGsmMap.officialMaps.Add(106, new UnicodeToGsmMap(106, 106, true));
			UnicodeToGsmMap.officialMaps.Add(107, new UnicodeToGsmMap(107, 107, true));
			UnicodeToGsmMap.officialMaps.Add(108, new UnicodeToGsmMap(108, 108, true));
			UnicodeToGsmMap.officialMaps.Add(109, new UnicodeToGsmMap(109, 109, true));
			UnicodeToGsmMap.officialMaps.Add(110, new UnicodeToGsmMap(110, 110, true));
			UnicodeToGsmMap.officialMaps.Add(111, new UnicodeToGsmMap(111, 111, true));
			UnicodeToGsmMap.officialMaps.Add(112, new UnicodeToGsmMap(112, 112, true));
			UnicodeToGsmMap.officialMaps.Add(113, new UnicodeToGsmMap(113, 113, true));
			UnicodeToGsmMap.officialMaps.Add(114, new UnicodeToGsmMap(114, 114, true));
			UnicodeToGsmMap.officialMaps.Add(115, new UnicodeToGsmMap(115, 115, true));
			UnicodeToGsmMap.officialMaps.Add(116, new UnicodeToGsmMap(116, 116, true));
			UnicodeToGsmMap.officialMaps.Add(117, new UnicodeToGsmMap(117, 117, true));
			UnicodeToGsmMap.officialMaps.Add(118, new UnicodeToGsmMap(118, 118, true));
			UnicodeToGsmMap.officialMaps.Add(119, new UnicodeToGsmMap(119, 119, true));
			UnicodeToGsmMap.officialMaps.Add(120, new UnicodeToGsmMap(120, 120, true));
			UnicodeToGsmMap.officialMaps.Add(121, new UnicodeToGsmMap(121, 121, true));
			UnicodeToGsmMap.officialMaps.Add(122, new UnicodeToGsmMap(122, 122, true));
			UnicodeToGsmMap.officialMaps.Add(123, new UnicodeToGsmMap(123, 6952, true));
			UnicodeToGsmMap.officialMaps.Add(124, new UnicodeToGsmMap(124, 6976, true));
			UnicodeToGsmMap.officialMaps.Add(125, new UnicodeToGsmMap(125, 6953, true));
			UnicodeToGsmMap.officialMaps.Add(126, new UnicodeToGsmMap(126, 6973, true));
			UnicodeToGsmMap.officialMaps.Add(160, new UnicodeToGsmMap(160, 27, true));
			UnicodeToGsmMap.officialMaps.Add(161, new UnicodeToGsmMap(161, 64, true));
			UnicodeToGsmMap.officialMaps.Add(163, new UnicodeToGsmMap(163, 1, true));
			UnicodeToGsmMap.officialMaps.Add(164, new UnicodeToGsmMap(164, 36, true));
			UnicodeToGsmMap.officialMaps.Add(165, new UnicodeToGsmMap(165, 3, true));
			UnicodeToGsmMap.officialMaps.Add(167, new UnicodeToGsmMap(167, 95, true));
			UnicodeToGsmMap.officialMaps.Add(191, new UnicodeToGsmMap(191, 96, true));
			UnicodeToGsmMap.officialMaps.Add(196, new UnicodeToGsmMap(196, 91, true));
			UnicodeToGsmMap.officialMaps.Add(197, new UnicodeToGsmMap(197, 14, true));
			UnicodeToGsmMap.officialMaps.Add(198, new UnicodeToGsmMap(198, 28, true));
			UnicodeToGsmMap.officialMaps.Add(199, new UnicodeToGsmMap(199, 9, true));
			UnicodeToGsmMap.officialMaps.Add(201, new UnicodeToGsmMap(201, 31, true));
			UnicodeToGsmMap.officialMaps.Add(209, new UnicodeToGsmMap(209, 93, true));
			UnicodeToGsmMap.officialMaps.Add(214, new UnicodeToGsmMap(214, 92, true));
			UnicodeToGsmMap.officialMaps.Add(216, new UnicodeToGsmMap(216, 11, true));
			UnicodeToGsmMap.officialMaps.Add(220, new UnicodeToGsmMap(220, 94, true));
			UnicodeToGsmMap.officialMaps.Add(223, new UnicodeToGsmMap(223, 30, true));
			UnicodeToGsmMap.officialMaps.Add(224, new UnicodeToGsmMap(224, 127, true));
			UnicodeToGsmMap.officialMaps.Add(228, new UnicodeToGsmMap(228, 123, true));
			UnicodeToGsmMap.officialMaps.Add(229, new UnicodeToGsmMap(229, 15, true));
			UnicodeToGsmMap.officialMaps.Add(230, new UnicodeToGsmMap(230, 29, true));
			UnicodeToGsmMap.officialMaps.Add(232, new UnicodeToGsmMap(232, 4, true));
			UnicodeToGsmMap.officialMaps.Add(233, new UnicodeToGsmMap(233, 5, true));
			UnicodeToGsmMap.officialMaps.Add(236, new UnicodeToGsmMap(236, 7, true));
			UnicodeToGsmMap.officialMaps.Add(241, new UnicodeToGsmMap(241, 125, true));
			UnicodeToGsmMap.officialMaps.Add(242, new UnicodeToGsmMap(242, 8, true));
			UnicodeToGsmMap.officialMaps.Add(246, new UnicodeToGsmMap(246, 124, true));
			UnicodeToGsmMap.officialMaps.Add(248, new UnicodeToGsmMap(248, 12, true));
			UnicodeToGsmMap.officialMaps.Add(249, new UnicodeToGsmMap(249, 6, true));
			UnicodeToGsmMap.officialMaps.Add(252, new UnicodeToGsmMap(252, 126, true));
			UnicodeToGsmMap.officialMaps.Add(915, new UnicodeToGsmMap(915, 19, true));
			UnicodeToGsmMap.officialMaps.Add(916, new UnicodeToGsmMap(916, 16, true));
			UnicodeToGsmMap.officialMaps.Add(920, new UnicodeToGsmMap(920, 25, true));
			UnicodeToGsmMap.officialMaps.Add(923, new UnicodeToGsmMap(923, 20, true));
			UnicodeToGsmMap.officialMaps.Add(926, new UnicodeToGsmMap(926, 26, true));
			UnicodeToGsmMap.officialMaps.Add(928, new UnicodeToGsmMap(928, 22, true));
			UnicodeToGsmMap.officialMaps.Add(931, new UnicodeToGsmMap(931, 24, true));
			UnicodeToGsmMap.officialMaps.Add(934, new UnicodeToGsmMap(934, 18, true));
			UnicodeToGsmMap.officialMaps.Add(936, new UnicodeToGsmMap(936, 23, true));
			UnicodeToGsmMap.officialMaps.Add(937, new UnicodeToGsmMap(937, 21, true));
			UnicodeToGsmMap.officialMaps.Add(8364, new UnicodeToGsmMap(8364, 7013, true));
		}

		private UnicodeToGsmMap(int unicode, int gsmcode, bool official)
		{
			if (0 > gsmcode || (247 < gsmcode && 27 != gsmcode >> 8))
			{
				throw new ArgumentOutOfRangeException("gsmcode");
			}
			this.unicode = unicode;
			this.gsmcode = gsmcode;
			this.official = official;
		}

		public static IEnumerable<char> UnofficialUnicodes
		{
			get
			{
				foreach (int unicode in UnicodeToGsmMap.unofficialMaps.Keys)
				{
					yield return Convert.ToChar(unicode);
				}
				yield break;
			}
		}

		private int Unicode
		{
			get
			{
				return this.unicode;
			}
		}

		private int Gsmcode
		{
			get
			{
				return this.gsmcode;
			}
		}

		private int GsmcodeRadixCount
		{
			get
			{
				if (27 != this.gsmcode >> 8)
				{
					return 1;
				}
				return 2;
			}
		}

		private bool Official
		{
			get
			{
				return this.official;
			}
		}

		public static int GetUnicodeToGsmRadixCount(char ch, bool includeUnofficial)
		{
			int key = Convert.ToInt32(ch);
			if (UnicodeToGsmMap.officialMaps.ContainsKey(key))
			{
				return UnicodeToGsmMap.officialMaps[key].GsmcodeRadixCount;
			}
			if (includeUnofficial && UnicodeToGsmMap.unofficialMaps.ContainsKey(key))
			{
				return UnicodeToGsmMap.unofficialMaps[key].GsmcodeRadixCount;
			}
			return 0;
		}

		private const int Gsm7Escape = 27;

		private static readonly Dictionary<int, UnicodeToGsmMap> unofficialMaps = new Dictionary<int, UnicodeToGsmMap>();

		private static readonly Dictionary<int, UnicodeToGsmMap> officialMaps = new Dictionary<int, UnicodeToGsmMap>();

		private int unicode;

		private int gsmcode;

		private bool official;
	}
}
