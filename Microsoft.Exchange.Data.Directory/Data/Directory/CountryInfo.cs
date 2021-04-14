using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Core;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	public class CountryInfo : IComparable<CountryInfo>, IEquatable<CountryInfo>
	{
		private static int GeoIdToIsoCode(int geoId)
		{
			for (int i = 0; i < CountryInfo.mapGeoIDToISOCode.GetLength(0); i++)
			{
				if (CountryInfo.mapGeoIDToISOCode[i, 0] == geoId)
				{
					return CountryInfo.mapGeoIDToISOCode[i, 1];
				}
			}
			return -1;
		}

		private static bool GeoEnumProc(int geoId)
		{
			StringBuilder stringBuilder = new StringBuilder(4);
			string value = null;
			StringBuilder stringBuilder2 = new StringBuilder(64);
			string value2 = null;
			if (NativeMethods.GetGeoInfo(geoId, NativeMethods.SysGeoType.Iso2, stringBuilder, stringBuilder.Capacity, 0) != 0)
			{
				value = stringBuilder.ToString().ToUpperInvariant();
			}
			if (NativeMethods.GetGeoInfo(geoId, NativeMethods.SysGeoType.FriendlyName, stringBuilder2, stringBuilder2.Capacity, 0) != 0)
			{
				value2 = stringBuilder2.ToString();
			}
			if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(value2))
			{
				int num = CountryInfo.GeoIdToIsoCode(geoId);
				if (-1 != num)
				{
					CountryInfo item = new CountryInfo(num, value, value2, geoId);
					CountryInfo.allCountryInfos.Add(item);
				}
			}
			return true;
		}

		static CountryInfo()
		{
			NativeMethods.EnumSystemGeoID(16, 0, new NativeMethods.GeoEnumProc(CountryInfo.GeoEnumProc));
			CountryInfo[] array = CountryInfo.supplementaryEntries;
			for (int i = 0; i < array.Length; i++)
			{
				CountryInfo info = array[i];
				if (CountryInfo.allCountryInfos.FindIndex((CountryInfo x) => x.UniqueId == info.UniqueId) == -1)
				{
					CountryInfo.allCountryInfos.Add(info);
				}
			}
			CountryInfo.allCountryInfos.Sort();
			foreach (CountryInfo countryInfo in CountryInfo.allCountryInfos)
			{
				if (!CountryInfo.countryCodeToCountryInfo.ContainsKey(countryInfo.countryCode))
				{
					CountryInfo.countryCodeToCountryInfo.Add(countryInfo.countryCode, countryInfo);
				}
				if (!CountryInfo.nameToCountryInfo.ContainsKey(countryInfo.name))
				{
					CountryInfo.nameToCountryInfo.Add(countryInfo.name, countryInfo);
				}
				if (!CountryInfo.displayNameToCountryInfo.ContainsKey(countryInfo.LocalizedDisplayName))
				{
					CountryInfo.displayNameToCountryInfo.Add(countryInfo.LocalizedDisplayName, countryInfo);
				}
			}
		}

		internal static ReadOnlyCollection<CountryInfo> AllCountryInfos
		{
			get
			{
				return new ReadOnlyCollection<CountryInfo>(CountryInfo.allCountryInfos);
			}
		}

		private CountryInfo(int countryCode, string name, string displayName, int geoId)
		{
			this.countryCode = countryCode;
			this.name = name;
			this.displayName = displayName;
			this.geoId = geoId;
		}

		public static CountryInfo GetCountryInfo(int countryCode)
		{
			if (CountryInfo.countryCodeToCountryInfo.ContainsKey(countryCode))
			{
				return CountryInfo.countryCodeToCountryInfo[countryCode];
			}
			throw new InvalidCountryOrRegionException(DirectoryStrings.ErrorInvalidISOCountryCode(countryCode));
		}

		public static CountryInfo Parse(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			if (CountryInfo.nameToCountryInfo.ContainsKey(name))
			{
				return CountryInfo.nameToCountryInfo[name];
			}
			if (CountryInfo.displayNameToCountryInfo.ContainsKey(name))
			{
				return CountryInfo.displayNameToCountryInfo[name];
			}
			throw new InvalidCountryOrRegionException(DirectoryStrings.ErrorInvalidISOTwoLetterOrCountryCode(name));
		}

		internal static CountryInfo Parse(string name, int countryCode, string displayName)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			CountryInfo countryInfo = null;
			CountryInfo countryInfo2 = null;
			CountryInfo countryInfo3 = null;
			foreach (CountryInfo countryInfo4 in CountryInfo.AllCountryInfos)
			{
				bool flag = string.Equals(name, countryInfo4.name, StringComparison.OrdinalIgnoreCase);
				bool flag2 = countryCode == countryInfo4.countryCode;
				if (flag || flag2)
				{
					bool flag3 = string.Equals(displayName, countryInfo4.displayName);
					if (flag && flag2 && flag3)
					{
						return countryInfo4;
					}
					if (flag && flag2)
					{
						if (!(countryInfo == null))
						{
							continue;
						}
						countryInfo = countryInfo4;
					}
					if (flag)
					{
						if (!(countryInfo2 == null))
						{
							continue;
						}
						countryInfo2 = countryInfo4;
					}
					if (countryInfo3 == null)
					{
						countryInfo3 = countryInfo4;
					}
				}
			}
			if (countryInfo != null)
			{
				return countryInfo;
			}
			if (countryInfo2 != null)
			{
				return countryInfo2;
			}
			if (countryInfo3 != null)
			{
				return countryInfo3;
			}
			throw new InvalidCountryOrRegionException(DirectoryStrings.ErrorParseCountryInfo(name, countryCode, displayName));
		}

		public int CountryCode
		{
			get
			{
				return this.countryCode;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		public LocalizedString LocalizedDisplayName
		{
			get
			{
				LocalizedString result;
				try
				{
					Countries.IDs key = (Countries.IDs)Enum.Parse(typeof(Countries.IDs), this.UniqueId, true);
					result = Countries.GetLocalizedString(key);
				}
				catch (ArgumentException)
				{
					result = new LocalizedString(this.DisplayName);
				}
				return result;
			}
		}

		internal string UniqueId
		{
			get
			{
				return string.Format("{0}_{1}_{2}", this.name, this.countryCode, this.geoId);
			}
		}

		public override string ToString()
		{
			return this.LocalizedDisplayName.ToString();
		}

		public CountryInfo Self
		{
			get
			{
				return this;
			}
		}

		int IComparable<CountryInfo>.CompareTo(CountryInfo other)
		{
			if (other != null)
			{
				return string.Compare(this.LocalizedDisplayName, other.LocalizedDisplayName, StringComparison.CurrentCultureIgnoreCase);
			}
			return 1;
		}

		public bool Equals(CountryInfo other)
		{
			return other != null && string.Equals(this.UniqueId, other.UniqueId, StringComparison.OrdinalIgnoreCase);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as CountryInfo);
		}

		public override int GetHashCode()
		{
			return this.countryCode.GetHashCode();
		}

		public static bool operator ==(CountryInfo left, CountryInfo right)
		{
			return object.Equals(left, right);
		}

		public static bool operator !=(CountryInfo left, CountryInfo right)
		{
			return !(left == right);
		}

		public static explicit operator string(CountryInfo info)
		{
			if (!(info != null))
			{
				return string.Empty;
			}
			return info.ToString();
		}

		private const int MaxNameLegth = 4;

		private const int MaxDisplayNameLegth = 64;

		private static CountryInfo[] supplementaryEntries = new CountryInfo[]
		{
			new CountryInfo(248, "AX", "Åland Islands", 0),
			new CountryInfo(499, "ME", "Montenegro", 0),
			new CountryInfo(652, "BL", "Saint Barthélemy", 0),
			new CountryInfo(663, "MF", "Saint Martin", 0),
			new CountryInfo(688, "RS", "Serbia", 0),
			new CountryInfo(581, "UM", "U.S. Minor Outlying Islands", 0),
			new CountryInfo(581, "X1", "Navassa Island", 0),
			new CountryInfo(531, "CW", "Curaçao", 273),
			new CountryInfo(534, "SX", "Sint Maarten", 30967),
			new CountryInfo(535, "BQ", "Bonaire, Saint Eustatius and Saba", 161832258),
			new CountryInfo(732, "EH", "Western Sahara", 0),
			new CountryInfo(728, "SS", "South Sudan", 0)
		};

		private static int[,] mapGeoIDToISOCode = new int[,]
		{
			{
				2,
				28
			},
			{
				3,
				4
			},
			{
				4,
				12
			},
			{
				5,
				31
			},
			{
				6,
				8
			},
			{
				7,
				51
			},
			{
				8,
				20
			},
			{
				9,
				24
			},
			{
				10,
				16
			},
			{
				11,
				32
			},
			{
				12,
				36
			},
			{
				14,
				40
			},
			{
				17,
				48
			},
			{
				18,
				52
			},
			{
				19,
				72
			},
			{
				20,
				60
			},
			{
				21,
				56
			},
			{
				22,
				44
			},
			{
				23,
				50
			},
			{
				24,
				84
			},
			{
				25,
				70
			},
			{
				26,
				68
			},
			{
				27,
				104
			},
			{
				28,
				204
			},
			{
				29,
				112
			},
			{
				30,
				90
			},
			{
				32,
				76
			},
			{
				34,
				64
			},
			{
				35,
				100
			},
			{
				37,
				96
			},
			{
				38,
				108
			},
			{
				39,
				124
			},
			{
				40,
				116
			},
			{
				41,
				148
			},
			{
				42,
				144
			},
			{
				43,
				178
			},
			{
				44,
				180
			},
			{
				45,
				156
			},
			{
				46,
				152
			},
			{
				49,
				120
			},
			{
				50,
				174
			},
			{
				51,
				170
			},
			{
				54,
				188
			},
			{
				55,
				140
			},
			{
				56,
				192
			},
			{
				57,
				132
			},
			{
				59,
				196
			},
			{
				61,
				208
			},
			{
				62,
				262
			},
			{
				63,
				212
			},
			{
				65,
				214
			},
			{
				66,
				218
			},
			{
				67,
				818
			},
			{
				68,
				372
			},
			{
				69,
				226
			},
			{
				70,
				233
			},
			{
				71,
				232
			},
			{
				72,
				222
			},
			{
				73,
				231
			},
			{
				75,
				203
			},
			{
				77,
				246
			},
			{
				78,
				242
			},
			{
				80,
				583
			},
			{
				81,
				234
			},
			{
				84,
				250
			},
			{
				86,
				270
			},
			{
				87,
				266
			},
			{
				88,
				268
			},
			{
				89,
				288
			},
			{
				90,
				292
			},
			{
				91,
				308
			},
			{
				93,
				304
			},
			{
				94,
				276
			},
			{
				98,
				300
			},
			{
				99,
				320
			},
			{
				100,
				324
			},
			{
				101,
				328
			},
			{
				103,
				332
			},
			{
				104,
				344
			},
			{
				106,
				340
			},
			{
				108,
				191
			},
			{
				109,
				348
			},
			{
				110,
				352
			},
			{
				111,
				360
			},
			{
				113,
				356
			},
			{
				114,
				86
			},
			{
				116,
				364
			},
			{
				117,
				376
			},
			{
				118,
				380
			},
			{
				119,
				384
			},
			{
				121,
				368
			},
			{
				122,
				392
			},
			{
				124,
				388
			},
			{
				125,
				744
			},
			{
				126,
				400
			},
			{
				127,
				581
			},
			{
				129,
				404
			},
			{
				130,
				417
			},
			{
				131,
				408
			},
			{
				133,
				296
			},
			{
				134,
				410
			},
			{
				136,
				414
			},
			{
				137,
				398
			},
			{
				138,
				418
			},
			{
				139,
				422
			},
			{
				140,
				428
			},
			{
				141,
				440
			},
			{
				142,
				430
			},
			{
				143,
				703
			},
			{
				145,
				438
			},
			{
				146,
				426
			},
			{
				147,
				442
			},
			{
				148,
				434
			},
			{
				149,
				450
			},
			{
				151,
				446
			},
			{
				152,
				498
			},
			{
				154,
				496
			},
			{
				156,
				454
			},
			{
				157,
				466
			},
			{
				158,
				492
			},
			{
				159,
				504
			},
			{
				160,
				480
			},
			{
				162,
				478
			},
			{
				163,
				470
			},
			{
				164,
				512
			},
			{
				165,
				462
			},
			{
				166,
				484
			},
			{
				167,
				458
			},
			{
				168,
				508
			},
			{
				173,
				562
			},
			{
				174,
				548
			},
			{
				175,
				566
			},
			{
				176,
				528
			},
			{
				177,
				578
			},
			{
				178,
				524
			},
			{
				180,
				520
			},
			{
				181,
				740
			},
			{
				182,
				558
			},
			{
				183,
				554
			},
			{
				184,
				275
			},
			{
				185,
				600
			},
			{
				187,
				604
			},
			{
				190,
				586
			},
			{
				191,
				616
			},
			{
				192,
				591
			},
			{
				193,
				620
			},
			{
				194,
				598
			},
			{
				195,
				585
			},
			{
				196,
				624
			},
			{
				197,
				634
			},
			{
				198,
				638
			},
			{
				199,
				584
			},
			{
				200,
				642
			},
			{
				201,
				608
			},
			{
				202,
				630
			},
			{
				203,
				643
			},
			{
				204,
				646
			},
			{
				205,
				682
			},
			{
				206,
				666
			},
			{
				207,
				659
			},
			{
				208,
				690
			},
			{
				209,
				710
			},
			{
				210,
				686
			},
			{
				212,
				705
			},
			{
				213,
				694
			},
			{
				214,
				674
			},
			{
				215,
				702
			},
			{
				216,
				706
			},
			{
				217,
				724
			},
			{
				218,
				662
			},
			{
				219,
				736
			},
			{
				220,
				744
			},
			{
				221,
				752
			},
			{
				222,
				760
			},
			{
				223,
				756
			},
			{
				224,
				784
			},
			{
				225,
				780
			},
			{
				227,
				764
			},
			{
				228,
				762
			},
			{
				231,
				776
			},
			{
				232,
				768
			},
			{
				233,
				678
			},
			{
				234,
				788
			},
			{
				235,
				792
			},
			{
				236,
				798
			},
			{
				237,
				158
			},
			{
				238,
				795
			},
			{
				239,
				834
			},
			{
				240,
				800
			},
			{
				241,
				804
			},
			{
				242,
				826
			},
			{
				244,
				840
			},
			{
				245,
				854
			},
			{
				246,
				858
			},
			{
				247,
				860
			},
			{
				248,
				670
			},
			{
				249,
				862
			},
			{
				251,
				704
			},
			{
				252,
				850
			},
			{
				253,
				336
			},
			{
				254,
				516
			},
			{
				258,
				581
			},
			{
				259,
				882
			},
			{
				260,
				748
			},
			{
				261,
				887
			},
			{
				263,
				894
			},
			{
				264,
				716
			},
			{
				269,
				891
			},
			{
				273,
				531
			},
			{
				300,
				660
			},
			{
				301,
				10
			},
			{
				302,
				533
			},
			{
				305,
				581
			},
			{
				306,
				74
			},
			{
				307,
				136
			},
			{
				309,
				162
			},
			{
				311,
				166
			},
			{
				312,
				184
			},
			{
				315,
				238
			},
			{
				317,
				254
			},
			{
				318,
				258
			},
			{
				319,
				260
			},
			{
				321,
				312
			},
			{
				322,
				316
			},
			{
				324,
				831
			},
			{
				325,
				334
			},
			{
				326,
				581
			},
			{
				327,
				581
			},
			{
				328,
				832
			},
			{
				329,
				581
			},
			{
				330,
				474
			},
			{
				331,
				175
			},
			{
				332,
				500
			},
			{
				333,
				530
			},
			{
				334,
				540
			},
			{
				335,
				570
			},
			{
				336,
				574
			},
			{
				337,
				580
			},
			{
				338,
				581
			},
			{
				339,
				612
			},
			{
				342,
				239
			},
			{
				343,
				654
			},
			{
				347,
				772
			},
			{
				349,
				796
			},
			{
				351,
				92
			},
			{
				352,
				876
			},
			{
				15126,
				833
			},
			{
				19618,
				807
			},
			{
				21242,
				581
			},
			{
				30967,
				534
			},
			{
				7299303,
				626
			},
			{
				161832258,
				535
			}
		};

		private static List<CountryInfo> allCountryInfos = new List<CountryInfo>();

		private static Dictionary<int, CountryInfo> countryCodeToCountryInfo = new Dictionary<int, CountryInfo>();

		private static Dictionary<string, CountryInfo> nameToCountryInfo = new Dictionary<string, CountryInfo>(StringComparer.OrdinalIgnoreCase);

		private static Dictionary<string, CountryInfo> displayNameToCountryInfo = new Dictionary<string, CountryInfo>(StringComparer.OrdinalIgnoreCase);

		private int countryCode;

		private string name;

		private string displayName;

		private int geoId;
	}
}
