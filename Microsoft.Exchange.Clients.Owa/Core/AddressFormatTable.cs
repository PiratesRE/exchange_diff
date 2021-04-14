using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal static class AddressFormatTable
	{
		private static Dictionary<PhysicalAddressType, PropertyDefinition[]> LoadAddressPropertyTable()
		{
			Dictionary<PhysicalAddressType, PropertyDefinition[]> dictionary = new Dictionary<PhysicalAddressType, PropertyDefinition[]>();
			dictionary[PhysicalAddressType.Business] = AddressFormatTable.BusinessAddressParts;
			dictionary[PhysicalAddressType.Home] = AddressFormatTable.HomeAddressParts;
			dictionary[PhysicalAddressType.Other] = AddressFormatTable.OtherAddressParts;
			return dictionary;
		}

		private static Dictionary<int, AddressFormatTable.AddressPart[]> LoadCultureAddressMap()
		{
			Dictionary<int, AddressFormatTable.AddressPart[]> dictionary = new Dictionary<int, AddressFormatTable.AddressPart[]>();
			dictionary[1025] = AddressFormatTable.AmericanAddressFormat;
			dictionary[2049] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[3073] = AddressFormatTable.EgyptianAddressFormat;
			dictionary[4097] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[5121] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[6145] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[7169] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[8193] = AddressFormatTable.OmanAddressFormat;
			dictionary[9217] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[10241] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[11265] = AddressFormatTable.AmericanAddressFormat;
			dictionary[12289] = AddressFormatTable.AmericanAddressFormat;
			dictionary[13313] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[14337] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[15361] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[16385] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[1069] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[1027] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[5124] = AddressFormatTable.zhMOAddressFormat;
			dictionary[1028] = AddressFormatTable.zhTWAddressFormat;
			dictionary[2052] = AddressFormatTable.zhTWAddressFormat;
			dictionary[3076] = AddressFormatTable.RussianAddressFormat;
			dictionary[4100] = AddressFormatTable.AmericanAddressFormat;
			dictionary[1029] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[1030] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[1043] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[2067] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[9225] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[1033] = AddressFormatTable.AmericanAddressFormat;
			dictionary[2057] = AddressFormatTable.AmericanAddressFormat;
			dictionary[3081] = AddressFormatTable.AmericanAddressFormat;
			dictionary[4105] = AddressFormatTable.AmericanAddressFormat;
			dictionary[5129] = AddressFormatTable.AmericanAddressFormat;
			dictionary[6153] = AddressFormatTable.AmericanAddressFormat;
			dictionary[7177] = AddressFormatTable.AmericanAddressFormat;
			dictionary[12297] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[8201] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[10249] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[11273] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[13321] = AddressFormatTable.TurkishAddressFormat;
			dictionary[16393] = AddressFormatTable.IndonesianAddressFormat;
			dictionary[17417] = AddressFormatTable.IndonesianAddressFormat;
			dictionary[18441] = AddressFormatTable.IndonesianAddressFormat;
			dictionary[1035] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[1036] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[2060] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[3084] = AddressFormatTable.AmericanAddressFormat;
			dictionary[4108] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[5132] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[6156] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[1031] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[2055] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[3079] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[4103] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[5127] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[1032] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[1037] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[1038] = AddressFormatTable.HungarianAddressFormat;
			dictionary[1040] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[2064] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[1041] = AddressFormatTable.JapaneseAddressFormat;
			dictionary[1042] = AddressFormatTable.JapaneseAddressFormat;
			dictionary[1045] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[1046] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[2070] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[1048] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[1049] = AddressFormatTable.RussianAddressFormat;
			dictionary[1034] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[2058] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[3082] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[4106] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[5130] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[6154] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[7178] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[8202] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[9226] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[10250] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[11274] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[12298] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[13322] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[14346] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[15370] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[16394] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[17418] = AddressFormatTable.AmericanAddressFormat;
			dictionary[18442] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[19466] = AddressFormatTable.AmericanAddressFormat;
			dictionary[20490] = AddressFormatTable.AmericanAddressFormat;
			dictionary[21514] = AddressFormatTable.AmericanAddressFormat;
			dictionary[1053] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[2077] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[1055] = AddressFormatTable.TurkishAddressFormat;
			dictionary[1044] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[1081] = AddressFormatTable.AmericanAddressFormat;
			dictionary[1086] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[1054] = AddressFormatTable.AmericanAddressFormat;
			dictionary[1051] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[1050] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[1057] = AddressFormatTable.IndonesianAddressFormat;
			dictionary[1060] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[1026] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[1039] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[2074] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[3098] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[1058] = AddressFormatTable.RussianAddressFormat;
			dictionary[1087] = AddressFormatTable.zhMOAddressFormat;
			dictionary[1061] = AddressFormatTable.EuropeanAddressFormat;
			dictionary[1062] = AddressFormatTable.IndonesianAddressFormat;
			dictionary[1063] = AddressFormatTable.IndonesianAddressFormat;
			dictionary[1066] = AddressFormatTable.OmanAddressFormat;
			dictionary[1056] = AddressFormatTable.IndonesianAddressFormat;
			dictionary[1065] = AddressFormatTable.AmericanAddressFormat;
			dictionary[1124] = AddressFormatTable.IndonesianAddressFormat;
			dictionary[2110] = AddressFormatTable.IndonesianAddressFormat;
			dictionary[2068] = AddressFormatTable.EuropeanAddressFormat;
			return dictionary;
		}

		public static AddressFormatTable.AddressPart[] GetCultureAddressMap(int lcid)
		{
			AddressFormatTable.AddressPart[] result;
			if (AddressFormatTable.cultureAddressMap.TryGetValue(lcid, out result))
			{
				return result;
			}
			return AddressFormatTable.AmericanAddressFormat;
		}

		public static PropertyDefinition LookupAddressProperty(AddressFormatTable.AddressPart addressPart, PhysicalAddressType type)
		{
			PropertyDefinition[] array;
			if (AddressFormatTable.addressPropertyTable.TryGetValue(type, out array))
			{
				return array[(int)addressPart];
			}
			return null;
		}

		public static PropertyDefinition LookupAddressPropertyAd(AddressFormatTable.AddressPart addressPart)
		{
			if (AddressFormatTable.BusinessAdAddressParts[(int)addressPart] != null)
			{
				return AddressFormatTable.BusinessAdAddressParts[(int)addressPart];
			}
			return null;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static AddressFormatTable()
		{
			AddressFormatTable.AddressPart[] array = new AddressFormatTable.AddressPart[5];
			array[0] = AddressFormatTable.AddressPart.PostalCode;
			array[1] = AddressFormatTable.AddressPart.Country;
			array[2] = AddressFormatTable.AddressPart.State;
			array[3] = AddressFormatTable.AddressPart.City;
			AddressFormatTable.zhTWAddressFormat = array;
			AddressFormatTable.AddressPart[] array2 = new AddressFormatTable.AddressPart[5];
			array2[0] = AddressFormatTable.AddressPart.Country;
			array2[1] = AddressFormatTable.AddressPart.PostalCode;
			array2[2] = AddressFormatTable.AddressPart.State;
			array2[3] = AddressFormatTable.AddressPart.City;
			AddressFormatTable.RussianAddressFormat = array2;
			AddressFormatTable.AddressPart[] array3 = new AddressFormatTable.AddressPart[5];
			array3[0] = AddressFormatTable.AddressPart.Country;
			array3[1] = AddressFormatTable.AddressPart.State;
			array3[2] = AddressFormatTable.AddressPart.City;
			array3[3] = AddressFormatTable.AddressPart.PostalCode;
			AddressFormatTable.zhMOAddressFormat = array3;
			AddressFormatTable.AmericanAddressFormat = new AddressFormatTable.AddressPart[]
			{
				AddressFormatTable.AddressPart.Street,
				AddressFormatTable.AddressPart.City,
				AddressFormatTable.AddressPart.State,
				AddressFormatTable.AddressPart.PostalCode,
				AddressFormatTable.AddressPart.Country
			};
			AddressFormatTable.EuropeanAddressFormat = new AddressFormatTable.AddressPart[]
			{
				AddressFormatTable.AddressPart.Street,
				AddressFormatTable.AddressPart.PostalCode,
				AddressFormatTable.AddressPart.City,
				AddressFormatTable.AddressPart.State,
				AddressFormatTable.AddressPart.Country
			};
			AddressFormatTable.JapaneseAddressFormat = new AddressFormatTable.AddressPart[]
			{
				AddressFormatTable.AddressPart.PostalCode,
				AddressFormatTable.AddressPart.State,
				AddressFormatTable.AddressPart.City,
				AddressFormatTable.AddressPart.Street,
				AddressFormatTable.AddressPart.Country
			};
			AddressFormatTable.TurkishAddressFormat = new AddressFormatTable.AddressPart[]
			{
				AddressFormatTable.AddressPart.Street,
				AddressFormatTable.AddressPart.PostalCode,
				AddressFormatTable.AddressPart.State,
				AddressFormatTable.AddressPart.City,
				AddressFormatTable.AddressPart.Country
			};
			AddressFormatTable.OmanAddressFormat = new AddressFormatTable.AddressPart[]
			{
				AddressFormatTable.AddressPart.Street,
				AddressFormatTable.AddressPart.City,
				AddressFormatTable.AddressPart.State,
				AddressFormatTable.AddressPart.Country,
				AddressFormatTable.AddressPart.PostalCode
			};
			AddressFormatTable.EgyptianAddressFormat = new AddressFormatTable.AddressPart[]
			{
				AddressFormatTable.AddressPart.Street,
				AddressFormatTable.AddressPart.Country,
				AddressFormatTable.AddressPart.City,
				AddressFormatTable.AddressPart.State,
				AddressFormatTable.AddressPart.PostalCode
			};
			AddressFormatTable.HungarianAddressFormat = new AddressFormatTable.AddressPart[]
			{
				AddressFormatTable.AddressPart.City,
				AddressFormatTable.AddressPart.Street,
				AddressFormatTable.AddressPart.PostalCode,
				AddressFormatTable.AddressPart.State,
				AddressFormatTable.AddressPart.Country
			};
			AddressFormatTable.IndonesianAddressFormat = new AddressFormatTable.AddressPart[]
			{
				AddressFormatTable.AddressPart.Street,
				AddressFormatTable.AddressPart.City,
				AddressFormatTable.AddressPart.PostalCode,
				AddressFormatTable.AddressPart.State,
				AddressFormatTable.AddressPart.Country
			};
			AddressFormatTable.HomeAddressParts = new PropertyDefinition[]
			{
				ContactSchema.HomeStreet,
				ContactSchema.HomeCity,
				ContactSchema.HomeState,
				ContactSchema.HomePostalCode,
				ContactSchema.HomeCountry
			};
			AddressFormatTable.BusinessAddressParts = new PropertyDefinition[]
			{
				ContactSchema.WorkAddressStreet,
				ContactSchema.WorkAddressCity,
				ContactSchema.WorkAddressState,
				ContactSchema.WorkAddressPostalCode,
				ContactSchema.WorkAddressCountry
			};
			AddressFormatTable.OtherAddressParts = new PropertyDefinition[]
			{
				ContactSchema.OtherStreet,
				ContactSchema.OtherCity,
				ContactSchema.OtherState,
				ContactSchema.OtherPostalCode,
				ContactSchema.OtherCountry
			};
			AddressFormatTable.BusinessAdAddressParts = new PropertyDefinition[]
			{
				ADOrgPersonSchema.StreetAddress,
				ADOrgPersonSchema.City,
				ADOrgPersonSchema.StateOrProvince,
				ADOrgPersonSchema.PostalCode,
				ADOrgPersonSchema.Co
			};
			AddressFormatTable.addressPropertyTable = AddressFormatTable.LoadAddressPropertyTable();
			AddressFormatTable.cultureAddressMap = AddressFormatTable.LoadCultureAddressMap();
		}

		private static readonly AddressFormatTable.AddressPart[] zhTWAddressFormat;

		private static readonly AddressFormatTable.AddressPart[] RussianAddressFormat;

		private static readonly AddressFormatTable.AddressPart[] zhMOAddressFormat;

		private static readonly AddressFormatTable.AddressPart[] AmericanAddressFormat;

		private static readonly AddressFormatTable.AddressPart[] EuropeanAddressFormat;

		private static readonly AddressFormatTable.AddressPart[] JapaneseAddressFormat;

		private static readonly AddressFormatTable.AddressPart[] TurkishAddressFormat;

		private static readonly AddressFormatTable.AddressPart[] OmanAddressFormat;

		private static readonly AddressFormatTable.AddressPart[] EgyptianAddressFormat;

		private static readonly AddressFormatTable.AddressPart[] HungarianAddressFormat;

		private static readonly AddressFormatTable.AddressPart[] IndonesianAddressFormat;

		private static readonly PropertyDefinition[] HomeAddressParts;

		private static readonly PropertyDefinition[] BusinessAddressParts;

		private static readonly PropertyDefinition[] OtherAddressParts;

		private static readonly PropertyDefinition[] BusinessAdAddressParts;

		private static Dictionary<PhysicalAddressType, PropertyDefinition[]> addressPropertyTable;

		private static Dictionary<int, AddressFormatTable.AddressPart[]> cultureAddressMap;

		public enum AddressPart
		{
			Street,
			City,
			State,
			PostalCode,
			Country
		}
	}
}
