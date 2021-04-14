using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory
{
	internal class LcidMapper
	{
		internal static string OidFromLcid(int lcid)
		{
			string text = null;
			ExTraceGlobals.LcidMapperTracer.TraceDebug<int>(0L, "LcidMapper.OidFromLcid - Mapping Lcid {0}", lcid);
			if (!LcidMapper.lcidMapping.TryGetValue(lcid, out text))
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				ExTraceGlobals.LcidMapperTracer.TraceDebug<int, int>(0L, "LcidMapper.OidFromLcid - Failed to map Lcid {0}, trying {1}", lcid, currentCulture.LCID);
				if (!LcidMapper.lcidMapping.TryGetValue(currentCulture.LCID, out text))
				{
					text = "1.2.840.113556.1.4.1499";
					ExTraceGlobals.LcidMapperTracer.TraceDebug<int>(0L, "LcidMapper.OidFromLcid - Failed to map Lcid {0}, defaulting to English", currentCulture.LCID);
				}
			}
			ExTraceGlobals.LcidMapperTracer.TraceDebug<int, string>(0L, "LcidMapper.OidFromLcid - Lcid {0} mapped to Oid {1}", lcid, text);
			return text;
		}

		static LcidMapper()
		{
			LcidMapper.lcidMapping = new Dictionary<int, string>();
			LcidMapper.lcidMapping[1078] = "1.2.840.113556.1.4.1461";
			LcidMapper.lcidMapping[1052] = "1.2.840.113556.1.4.1462";
			LcidMapper.lcidMapping[1025] = "1.2.840.113556.1.4.1463";
			LcidMapper.lcidMapping[2049] = "1.2.840.113556.1.4.1464";
			LcidMapper.lcidMapping[3073] = "1.2.840.113556.1.4.1465";
			LcidMapper.lcidMapping[4097] = "1.2.840.113556.1.4.1466";
			LcidMapper.lcidMapping[5121] = "1.2.840.113556.1.4.1467";
			LcidMapper.lcidMapping[6145] = "1.2.840.113556.1.4.1468";
			LcidMapper.lcidMapping[7169] = "1.2.840.113556.1.4.1469";
			LcidMapper.lcidMapping[8193] = "1.2.840.113556.1.4.1470";
			LcidMapper.lcidMapping[9217] = "1.2.840.113556.1.4.1471";
			LcidMapper.lcidMapping[10241] = "1.2.840.113556.1.4.1472";
			LcidMapper.lcidMapping[11265] = "1.2.840.113556.1.4.1473";
			LcidMapper.lcidMapping[12289] = "1.2.840.113556.1.4.1474";
			LcidMapper.lcidMapping[13313] = "1.2.840.113556.1.4.1475";
			LcidMapper.lcidMapping[14337] = "1.2.840.113556.1.4.1476";
			LcidMapper.lcidMapping[15361] = "1.2.840.113556.1.4.1477";
			LcidMapper.lcidMapping[16385] = "1.2.840.113556.1.4.1478";
			LcidMapper.lcidMapping[1067] = "1.2.840.113556.1.4.1479";
			LcidMapper.lcidMapping[1101] = "1.2.840.113556.1.4.1480";
			LcidMapper.lcidMapping[1068] = "1.2.840.113556.1.4.1481";
			LcidMapper.lcidMapping[2092] = "1.2.840.113556.1.4.1482";
			LcidMapper.lcidMapping[1069] = "1.2.840.113556.1.4.1483";
			LcidMapper.lcidMapping[1059] = "1.2.840.113556.1.4.1484";
			LcidMapper.lcidMapping[1093] = "1.2.840.113556.1.4.1485";
			LcidMapper.lcidMapping[1026] = "1.2.840.113556.1.4.1486";
			LcidMapper.lcidMapping[1109] = "1.2.840.113556.1.4.1487";
			LcidMapper.lcidMapping[1027] = "1.2.840.113556.1.4.1488";
			LcidMapper.lcidMapping[1028] = "1.2.840.113556.1.4.1489";
			LcidMapper.lcidMapping[2052] = "1.2.840.113556.1.4.1490";
			LcidMapper.lcidMapping[3076] = "1.2.840.113556.1.4.1491";
			LcidMapper.lcidMapping[4100] = "1.2.840.113556.1.4.1492";
			LcidMapper.lcidMapping[5124] = "1.2.840.113556.1.4.1493";
			LcidMapper.lcidMapping[1050] = "1.2.840.113556.1.4.1494";
			LcidMapper.lcidMapping[1029] = "1.2.840.113556.1.4.1495";
			LcidMapper.lcidMapping[1030] = "1.2.840.113556.1.4.1496";
			LcidMapper.lcidMapping[1043] = "1.2.840.113556.1.4.1497";
			LcidMapper.lcidMapping[2067] = "1.2.840.113556.1.4.1498";
			LcidMapper.lcidMapping[1033] = "1.2.840.113556.1.4.1499";
			LcidMapper.lcidMapping[2057] = "1.2.840.113556.1.4.1500";
			LcidMapper.lcidMapping[3081] = "1.2.840.113556.1.4.1665";
			LcidMapper.lcidMapping[4105] = "1.2.840.113556.1.4.1666";
			LcidMapper.lcidMapping[5129] = "1.2.840.113556.1.4.1667";
			LcidMapper.lcidMapping[6153] = "1.2.840.113556.1.4.1668";
			LcidMapper.lcidMapping[7177] = "1.2.840.113556.1.4.1505";
			LcidMapper.lcidMapping[8201] = "1.2.840.113556.1.4.1506";
			LcidMapper.lcidMapping[9225] = "1.2.840.113556.1.4.1507";
			LcidMapper.lcidMapping[10249] = "1.2.840.113556.1.4.1508";
			LcidMapper.lcidMapping[11273] = "1.2.840.113556.1.4.1509";
			LcidMapper.lcidMapping[12297] = "1.2.840.113556.1.4.1510";
			LcidMapper.lcidMapping[13321] = "1.2.840.113556.1.4.1511";
			LcidMapper.lcidMapping[1061] = "1.2.840.113556.1.4.1512";
			LcidMapper.lcidMapping[1080] = "1.2.840.113556.1.4.1513";
			LcidMapper.lcidMapping[1065] = "1.2.840.113556.1.4.1514";
			LcidMapper.lcidMapping[1035] = "1.2.840.113556.1.4.1515";
			LcidMapper.lcidMapping[1036] = "1.2.840.113556.1.4.1516";
			LcidMapper.lcidMapping[2060] = "1.2.840.113556.1.4.1517";
			LcidMapper.lcidMapping[3084] = "1.2.840.113556.1.4.1518";
			LcidMapper.lcidMapping[4108] = "1.2.840.113556.1.4.1519";
			LcidMapper.lcidMapping[5132] = "1.2.840.113556.1.4.1520";
			LcidMapper.lcidMapping[6156] = "1.2.840.113556.1.4.1521";
			LcidMapper.lcidMapping[1079] = "1.2.840.113556.1.4.1522";
			LcidMapper.lcidMapping[1031] = "1.2.840.113556.1.4.1523";
			LcidMapper.lcidMapping[2055] = "1.2.840.113556.1.4.1524";
			LcidMapper.lcidMapping[3079] = "1.2.840.113556.1.4.1525";
			LcidMapper.lcidMapping[4103] = "1.2.840.113556.1.4.1526";
			LcidMapper.lcidMapping[5127] = "1.2.840.113556.1.4.1527";
			LcidMapper.lcidMapping[1032] = "1.2.840.113556.1.4.1528";
			LcidMapper.lcidMapping[1095] = "1.2.840.113556.1.4.1529";
			LcidMapper.lcidMapping[1037] = "1.2.840.113556.1.4.1530";
			LcidMapper.lcidMapping[1081] = "1.2.840.113556.1.4.1531";
			LcidMapper.lcidMapping[1038] = "1.2.840.113556.1.4.1532";
			LcidMapper.lcidMapping[1039] = "1.2.840.113556.1.4.1533";
			LcidMapper.lcidMapping[1057] = "1.2.840.113556.1.4.1534";
			LcidMapper.lcidMapping[1118] = "1.2.840.113556.1.4.1535";
			LcidMapper.lcidMapping[1040] = "1.2.840.113556.1.4.1536";
			LcidMapper.lcidMapping[2064] = "1.2.840.113556.1.4.1537";
			LcidMapper.lcidMapping[1041] = "1.2.840.113556.1.4.1538";
			LcidMapper.lcidMapping[1099] = "1.2.840.113556.1.4.1539";
			LcidMapper.lcidMapping[1120] = "1.2.840.113556.1.4.1540";
			LcidMapper.lcidMapping[2144] = "1.2.840.113556.1.4.1541";
			LcidMapper.lcidMapping[1087] = "1.2.840.113556.1.4.1542";
			LcidMapper.lcidMapping[1107] = "1.2.840.113556.1.4.1543";
			LcidMapper.lcidMapping[1088] = "1.2.840.113556.1.4.1544";
			LcidMapper.lcidMapping[1111] = "1.2.840.113556.1.4.1545";
			LcidMapper.lcidMapping[1042] = "1.2.840.113556.1.4.1546";
			LcidMapper.lcidMapping[2066] = "1.2.840.113556.1.4.1547";
			LcidMapper.lcidMapping[1062] = "1.2.840.113556.1.4.1548";
			LcidMapper.lcidMapping[1063] = "1.2.840.113556.1.4.1549";
			LcidMapper.lcidMapping[1071] = "1.2.840.113556.1.4.1550";
			LcidMapper.lcidMapping[1086] = "1.2.840.113556.1.4.1551";
			LcidMapper.lcidMapping[2110] = "1.2.840.113556.1.4.1552";
			LcidMapper.lcidMapping[1100] = "1.2.840.113556.1.4.1553";
			LcidMapper.lcidMapping[1082] = "1.2.840.113556.1.4.1554";
			LcidMapper.lcidMapping[1112] = "1.2.840.113556.1.4.1555";
			LcidMapper.lcidMapping[1102] = "1.2.840.113556.1.4.1556";
			LcidMapper.lcidMapping[1121] = "1.2.840.113556.1.4.1557";
			LcidMapper.lcidMapping[1044] = "1.2.840.113556.1.4.1558";
			LcidMapper.lcidMapping[2068] = "1.2.840.113556.1.4.1559";
			LcidMapper.lcidMapping[1096] = "1.2.840.113556.1.4.1560";
			LcidMapper.lcidMapping[1045] = "1.2.840.113556.1.4.1561";
			LcidMapper.lcidMapping[1046] = "1.2.840.113556.1.4.1562";
			LcidMapper.lcidMapping[2070] = "1.2.840.113556.1.4.1563";
			LcidMapper.lcidMapping[1094] = "1.2.840.113556.1.4.1564";
			LcidMapper.lcidMapping[1048] = "1.2.840.113556.1.4.1565";
			LcidMapper.lcidMapping[1049] = "1.2.840.113556.1.4.1566";
			LcidMapper.lcidMapping[1103] = "1.2.840.113556.1.4.1567";
			LcidMapper.lcidMapping[3098] = "1.2.840.113556.1.4.1568";
			LcidMapper.lcidMapping[2074] = "1.2.840.113556.1.4.1569";
			LcidMapper.lcidMapping[1113] = "1.2.840.113556.1.4.1570";
			LcidMapper.lcidMapping[1051] = "1.2.840.113556.1.4.1571";
			LcidMapper.lcidMapping[1060] = "1.2.840.113556.1.4.1572";
			LcidMapper.lcidMapping[1034] = "1.2.840.113556.1.4.1573";
			LcidMapper.lcidMapping[2058] = "1.2.840.113556.1.4.1574";
			LcidMapper.lcidMapping[3082] = "1.2.840.113556.1.4.1575";
			LcidMapper.lcidMapping[4106] = "1.2.840.113556.1.4.1576";
			LcidMapper.lcidMapping[5130] = "1.2.840.113556.1.4.1577";
			LcidMapper.lcidMapping[6154] = "1.2.840.113556.1.4.1578";
			LcidMapper.lcidMapping[7178] = "1.2.840.113556.1.4.1579";
			LcidMapper.lcidMapping[8202] = "1.2.840.113556.1.4.1580";
			LcidMapper.lcidMapping[9226] = "1.2.840.113556.1.4.1581";
			LcidMapper.lcidMapping[10250] = "1.2.840.113556.1.4.1582";
			LcidMapper.lcidMapping[11274] = "1.2.840.113556.1.4.1583";
			LcidMapper.lcidMapping[12298] = "1.2.840.113556.1.4.1584";
			LcidMapper.lcidMapping[13322] = "1.2.840.113556.1.4.1585";
			LcidMapper.lcidMapping[14346] = "1.2.840.113556.1.4.1586";
			LcidMapper.lcidMapping[15370] = "1.2.840.113556.1.4.1587";
			LcidMapper.lcidMapping[16394] = "1.2.840.113556.1.4.1588";
			LcidMapper.lcidMapping[17418] = "1.2.840.113556.1.4.1589";
			LcidMapper.lcidMapping[18442] = "1.2.840.113556.1.4.1590";
			LcidMapper.lcidMapping[19466] = "1.2.840.113556.1.4.1591";
			LcidMapper.lcidMapping[20490] = "1.2.840.113556.1.4.1592";
			LcidMapper.lcidMapping[1089] = "1.2.840.113556.1.4.1593";
			LcidMapper.lcidMapping[1053] = "1.2.840.113556.1.4.1594";
			LcidMapper.lcidMapping[2077] = "1.2.840.113556.1.4.1595";
			LcidMapper.lcidMapping[1097] = "1.2.840.113556.1.4.1596";
			LcidMapper.lcidMapping[1092] = "1.2.840.113556.1.4.1597";
			LcidMapper.lcidMapping[1098] = "1.2.840.113556.1.4.1598";
			LcidMapper.lcidMapping[1054] = "1.2.840.113556.1.4.1599";
			LcidMapper.lcidMapping[1055] = "1.2.840.113556.1.4.1600";
			LcidMapper.lcidMapping[1058] = "1.2.840.113556.1.4.1601";
			LcidMapper.lcidMapping[1056] = "1.2.840.113556.1.4.1602";
			LcidMapper.lcidMapping[2080] = "1.2.840.113556.1.4.1603";
			LcidMapper.lcidMapping[1091] = "1.2.840.113556.1.4.1604";
			LcidMapper.lcidMapping[2115] = "1.2.840.113556.1.4.1605";
			LcidMapper.lcidMapping[1066] = "1.2.840.113556.1.4.1606";
		}

		private static Dictionary<int, string> lcidMapping;

		internal static readonly int DefaultLcid = 1033;
	}
}
