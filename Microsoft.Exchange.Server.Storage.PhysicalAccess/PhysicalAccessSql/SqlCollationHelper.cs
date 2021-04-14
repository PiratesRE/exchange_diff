﻿using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public static class SqlCollationHelper
	{
		private static Dictionary<int, string> InitializeMapping()
		{
			return new Dictionary<int, string>(100)
			{
				{
					-1,
					"SQL_Latin1_General_CP850_BIN"
				},
				{
					0,
					"Latin1_General_CI_AS"
				},
				{
					1078,
					"Latin1_General_CI_AS"
				},
				{
					1052,
					"Albanian_CI_AS"
				},
				{
					1025,
					"Arabic_CI_AS"
				},
				{
					2049,
					"Arabic_CI_AS"
				},
				{
					3073,
					"Arabic_CI_AS"
				},
				{
					4097,
					"Arabic_CI_AS"
				},
				{
					5121,
					"Arabic_CI_AS"
				},
				{
					6145,
					"Arabic_CI_AS"
				},
				{
					7169,
					"Arabic_CI_AS"
				},
				{
					8193,
					"Arabic_CI_AS"
				},
				{
					9217,
					"Arabic_CI_AS"
				},
				{
					10241,
					"Arabic_CI_AS"
				},
				{
					11265,
					"Arabic_CI_AS"
				},
				{
					12289,
					"Arabic_CI_AS"
				},
				{
					13313,
					"Arabic_CI_AS"
				},
				{
					14337,
					"Arabic_CI_AS"
				},
				{
					15361,
					"Arabic_CI_AS"
				},
				{
					16385,
					"Arabic_CI_AS"
				},
				{
					1067,
					"Latin1_General_CI_AS"
				},
				{
					1068,
					"Azeri_Latin_100_CI_AS"
				},
				{
					2092,
					"Azeri_Cyrillic_100_CI_AS"
				},
				{
					1069,
					"Latin1_General_CI_AS"
				},
				{
					1059,
					"Latin1_General_CI_AS"
				},
				{
					1093,
					"Latin1_General_CI_AS"
				},
				{
					5146,
					"Bosnian_Latin_100_CI_AS"
				},
				{
					1026,
					"Latin1_General_CI_AS"
				},
				{
					1027,
					"Latin1_General_CI_AS"
				},
				{
					3076,
					"Chinese_Hong_Kong_Stroke_90_CI_AS"
				},
				{
					5124,
					"Chinese_PRC_CI_AS"
				},
				{
					4100,
					"Chinese_PRC_CI_AS"
				},
				{
					2052,
					"Chinese_PRC_CI_AS"
				},
				{
					1028,
					"Chinese_Taiwan_Bopomofo_CI_AS"
				},
				{
					1050,
					"Croatian_CI_AS"
				},
				{
					4122,
					"Croatian_CI_AS"
				},
				{
					1029,
					"Czech_CI_AS"
				},
				{
					1030,
					"Danish_Greenlandic_100_CI_AS"
				},
				{
					1043,
					"Latin1_General_CI_AS"
				},
				{
					2067,
					"Latin1_General_CI_AS"
				},
				{
					1125,
					"Divehi_90_CI_AS"
				},
				{
					1033,
					"Latin1_General_CI_AS"
				},
				{
					2057,
					"Latin1_General_CI_AS"
				},
				{
					3081,
					"Latin1_General_CI_AS"
				},
				{
					4105,
					"Latin1_General_CI_AS"
				},
				{
					5129,
					"Latin1_General_CI_AS"
				},
				{
					6153,
					"Latin1_General_CI_AS"
				},
				{
					7177,
					"Latin1_General_CI_AS"
				},
				{
					8201,
					"Latin1_General_CI_AS"
				},
				{
					9225,
					"Latin1_General_CI_AS"
				},
				{
					10249,
					"Latin1_General_CI_AS"
				},
				{
					11273,
					"Latin1_General_CI_AS"
				},
				{
					12297,
					"Latin1_General_CI_AS"
				},
				{
					13321,
					"Latin1_General_CI_AS"
				},
				{
					1061,
					"Estonian_CI_AS"
				},
				{
					1080,
					"Latin1_General_CI_AS"
				},
				{
					1065,
					"Latin1_General_CI_AS"
				},
				{
					1035,
					"Finnish_Swedish_CI_AS"
				},
				{
					1036,
					"French_CI_AS"
				},
				{
					2060,
					"French_CI_AS"
				},
				{
					3084,
					"French_CI_AS"
				},
				{
					4108,
					"French_CI_AS"
				},
				{
					5132,
					"French_CI_AS"
				},
				{
					6156,
					"French_CI_AS"
				},
				{
					1079,
					"Georgian_Modern_Sort_CI_AS"
				},
				{
					1110,
					"Latin1_General_CI_AS"
				},
				{
					1031,
					"German_PhoneBook_CI_AS"
				},
				{
					2055,
					"German_PhoneBook_CI_AS"
				},
				{
					3079,
					"German_PhoneBook_CI_AS"
				},
				{
					4103,
					"German_PhoneBook_CI_AS"
				},
				{
					5127,
					"German_PhoneBook_CI_AS"
				},
				{
					1032,
					"Greek_CI_AS"
				},
				{
					1095,
					"Latin1_General_CI_AS"
				},
				{
					1037,
					"Hebrew_CI_AS"
				},
				{
					1081,
					"Latin1_General_CI_AS"
				},
				{
					1038,
					"Hungarian_CI_AS"
				},
				{
					1039,
					"Icelandic_CI_AS"
				},
				{
					1057,
					"Latin1_General_CI_AS"
				},
				{
					1040,
					"Latin1_General_CI_AS"
				},
				{
					2064,
					"Latin1_General_CI_AS"
				},
				{
					1041,
					"Japanese_CI_AS"
				},
				{
					1099,
					"Latin1_General_CI_AS"
				},
				{
					1087,
					"Kazakh_90_CI_AS"
				},
				{
					1111,
					"Latin1_General_CI_AS"
				},
				{
					1042,
					"Korean_90_CI_AS"
				},
				{
					1088,
					"Latin1_General_CI_AS"
				},
				{
					1062,
					"Latvian_CI_AS"
				},
				{
					1063,
					"Lithuanian_CI_AS"
				},
				{
					1071,
					"Macedonian_FYROM_90_CI_AS"
				},
				{
					1086,
					"Latin1_General_CI_AS"
				},
				{
					2110,
					"Latin1_General_CI_AS"
				},
				{
					1100,
					"Latin1_General_CI_AS"
				},
				{
					1082,
					"Maltese_100_CI_AS"
				},
				{
					1153,
					"Maori_100_CI_AS"
				},
				{
					1102,
					"Latin1_General_CI_AS"
				},
				{
					1104,
					"Latin1_General_CI_AS"
				},
				{
					1044,
					"Norwegian_100_CI_AS"
				},
				{
					2068,
					"Norwegian_100_CI_AS"
				},
				{
					1045,
					"Polish_CI_AS"
				},
				{
					1046,
					"Traditional_Spanish_CI_AS"
				},
				{
					2070,
					"Traditional_Spanish_CI_AS"
				},
				{
					1094,
					"Latin1_General_CI_AS"
				},
				{
					1131,
					"Latin1_General_CI_AS"
				},
				{
					2155,
					"Latin1_General_CI_AS"
				},
				{
					3179,
					"Latin1_General_CI_AS"
				},
				{
					1048,
					"Romanian_CI_AS"
				},
				{
					1049,
					"Cyrillic_General_CI_AS"
				},
				{
					9275,
					"Sami_Norway_100_CI_AS"
				},
				{
					4155,
					"Sami_Norway_100_CI_AS"
				},
				{
					5179,
					"Sami_Norway_100_CI_AS"
				},
				{
					1083,
					"Sami_Norway_100_CI_AS"
				},
				{
					2107,
					"Sami_Norway_100_CI_AS"
				},
				{
					8251,
					"Sami_Norway_100_CI_AS"
				},
				{
					6203,
					"Sami_Norway_100_CI_AS"
				},
				{
					7227,
					"Sami_Norway_100_CI_AS"
				},
				{
					3131,
					"Sami_Sweden_Finland_100_CI_AS"
				},
				{
					1103,
					"Latin1_General_CI_AS"
				},
				{
					2074,
					"Serbian_Latin_100_CI_AS"
				},
				{
					6170,
					"Serbian_Latin_100_CI_AS"
				},
				{
					3098,
					"Serbian_Cyrillic_100_CI_AS"
				},
				{
					7194,
					"Serbian_Cyrillic_100_CI_AS"
				},
				{
					1034,
					"Traditional_Spanish_CI_AS"
				},
				{
					2058,
					"Modern_Spanish_CI_AS"
				},
				{
					3028,
					"Modern_Spanish_CI_AS"
				},
				{
					4106,
					"Modern_Spanish_CI_AS"
				},
				{
					5130,
					"Modern_Spanish_CI_AS"
				},
				{
					6154,
					"Modern_Spanish_CI_AS"
				},
				{
					7178,
					"Modern_Spanish_CI_AS"
				},
				{
					8202,
					"Modern_Spanish_CI_AS"
				},
				{
					9226,
					"Modern_Spanish_CI_AS"
				},
				{
					10250,
					"Modern_Spanish_CI_AS"
				},
				{
					11274,
					"Modern_Spanish_CI_AS"
				},
				{
					12298,
					"Modern_Spanish_CI_AS"
				},
				{
					13322,
					"Modern_Spanish_CI_AS"
				},
				{
					14346,
					"Modern_Spanish_CI_AS"
				},
				{
					15370,
					"Modern_Spanish_CI_AS"
				},
				{
					16394,
					"Modern_Spanish_CI_AS"
				},
				{
					17418,
					"Modern_Spanish_CI_AS"
				},
				{
					18442,
					"Modern_Spanish_CI_AS"
				},
				{
					19466,
					"Modern_Spanish_CI_AS"
				},
				{
					20490,
					"Modern_Spanish_CI_AS"
				},
				{
					1089,
					"Latin1_General_CI_AS"
				},
				{
					1053,
					"Latin1_General_CI_AS"
				},
				{
					2077,
					"Latin1_General_CI_AS"
				},
				{
					1114,
					"Syriac_90_CI_AS"
				},
				{
					1051,
					"Slovak_CI_AS"
				},
				{
					1060,
					"Slovenian_CI_AS"
				},
				{
					1097,
					"Latin1_General_CI_AS"
				},
				{
					1098,
					"Latin1_General_CI_AS"
				},
				{
					1074,
					"Latin1_General_CI_AS"
				},
				{
					1092,
					"Tatar_90_CI_AS"
				},
				{
					1054,
					"Thai_CI_AS"
				},
				{
					1055,
					"Turkish_CI_AS"
				},
				{
					1058,
					"Ukrainian_CI_AS"
				},
				{
					1056,
					"Urdu_100_CI_AS"
				},
				{
					1091,
					"Uzbek_Latin_90_CI_AS"
				},
				{
					2115,
					"Cyrillic_General_CI_AS"
				},
				{
					1066,
					"Vietnamese_100_CI_AS"
				},
				{
					1106,
					"Welsh_100_CI_AS"
				},
				{
					1076,
					"Latin1_General_CI_AS"
				},
				{
					1077,
					"Latin1_General_CI_AS"
				}
			};
		}

		public static string MapLCIDToCollation(int lcid)
		{
			string result;
			if (!SqlCollationHelper.lcidMapping.TryGetValue(lcid, out result))
			{
				result = SqlCollationHelper.lcidMapping[0];
			}
			return result;
		}

		public static string GetCollation(Type colType, CultureInfo culture)
		{
			if (colType == typeof(string))
			{
				int lcid = (culture == null) ? -1 : culture.LCID;
				return SqlCollationHelper.MapLCIDToCollation(lcid);
			}
			return null;
		}

		public static void AppendCollation(Column column, CultureInfo culture, SqlCommand sqlCommand)
		{
			string collation = SqlCollationHelper.GetCollation(column.Type, culture);
			if (collation != null)
			{
				sqlCommand.Append(" COLLATE ");
				sqlCommand.Append(collation);
			}
		}

		private static Dictionary<int, string> lcidMapping = SqlCollationHelper.InitializeMapping();
	}
}