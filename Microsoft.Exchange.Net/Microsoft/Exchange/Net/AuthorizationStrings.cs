using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Net
{
	internal static class AuthorizationStrings
	{
		static AuthorizationStrings()
		{
			AuthorizationStrings.stringIDs.Add(1929848388U, "UnknownExtensionDataKey");
			AuthorizationStrings.stringIDs.Add(508499146U, "InvalidExtensionDataLength");
			AuthorizationStrings.stringIDs.Add(3093504254U, "SidNodeExpected");
			AuthorizationStrings.stringIDs.Add(1823279302U, "InvalidCommonAccessTokenString");
			AuthorizationStrings.stringIDs.Add(2930595838U, "LogonNameIsMissing");
			AuthorizationStrings.stringIDs.Add(524350434U, "InvalidGroupAttributesValue");
			AuthorizationStrings.stringIDs.Add(2738351686U, "MissingVersion");
			AuthorizationStrings.stringIDs.Add(3881300465U, "InvalidGroupAttributes");
			AuthorizationStrings.stringIDs.Add(3419671342U, "AuthenticationTypeIsMissing");
			AuthorizationStrings.stringIDs.Add(3730243391U, "InvalidRestrictedGroupLength");
			AuthorizationStrings.stringIDs.Add(2708243460U, "UserSidMustNotHaveAttributes");
			AuthorizationStrings.stringIDs.Add(4181701447U, "InvalidExtensionDataValue");
			AuthorizationStrings.stringIDs.Add(945651915U, "MissingUserSid");
			AuthorizationStrings.stringIDs.Add(2952593655U, "InvalidGroupSidValue");
			AuthorizationStrings.stringIDs.Add(3725269515U, "InvalidFieldType");
			AuthorizationStrings.stringIDs.Add(3321965778U, "InvalidXml");
			AuthorizationStrings.stringIDs.Add(2765023819U, "InvalidRestrictedGroupAttributesValue");
			AuthorizationStrings.stringIDs.Add(3428857605U, "InvalidWindowsAccessToken");
			AuthorizationStrings.stringIDs.Add(1782138211U, "MultipleUserSid");
			AuthorizationStrings.stringIDs.Add(832686951U, "ExpectingEndOfSid");
			AuthorizationStrings.stringIDs.Add(3955902527U, "InvalidRoot");
			AuthorizationStrings.stringIDs.Add(4129109959U, "InvalidSidType");
			AuthorizationStrings.stringIDs.Add(1629171792U, "ExpectingSidValue");
			AuthorizationStrings.stringIDs.Add(787285511U, "MissingTokenType");
			AuthorizationStrings.stringIDs.Add(102074919U, "InvalidExtensionDataKey");
			AuthorizationStrings.stringIDs.Add(986794111U, "ExpectingWindowsAccessToken");
			AuthorizationStrings.stringIDs.Add(3967521285U, "MissingIsCompressed");
			AuthorizationStrings.stringIDs.Add(4168034456U, "InvalidGroupLength");
			AuthorizationStrings.stringIDs.Add(31770294U, "InvalidRestrictedGroupSidValue");
			AuthorizationStrings.stringIDs.Add(369811396U, "InvalidAttributeValue");
			AuthorizationStrings.stringIDs.Add(1681390866U, "InvalidRestrictedGroupAttributes");
		}

		public static LocalizedString InvalidSidAttribute(string attribute)
		{
			return new LocalizedString("InvalidSidAttribute", "Ex24A1BE", false, true, AuthorizationStrings.ResourceManager, new object[]
			{
				attribute
			});
		}

		public static LocalizedString UnknownExtensionDataKey
		{
			get
			{
				return new LocalizedString("UnknownExtensionDataKey", "", false, false, AuthorizationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidExtensionDataLength
		{
			get
			{
				return new LocalizedString("InvalidExtensionDataLength", "", false, false, AuthorizationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SidNodeExpected
		{
			get
			{
				return new LocalizedString("SidNodeExpected", "Ex5C061D", false, true, AuthorizationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidCommonAccessTokenString
		{
			get
			{
				return new LocalizedString("InvalidCommonAccessTokenString", "", false, false, AuthorizationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogonNameIsMissing
		{
			get
			{
				return new LocalizedString("LogonNameIsMissing", "ExAC36C7", false, true, AuthorizationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidRootAttribute(string attribute)
		{
			return new LocalizedString("InvalidRootAttribute", "Ex14F4DF", false, true, AuthorizationStrings.ResourceManager, new object[]
			{
				attribute
			});
		}

		public static LocalizedString InvalidGroupAttributesValue
		{
			get
			{
				return new LocalizedString("InvalidGroupAttributesValue", "", false, false, AuthorizationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MissingVersion
		{
			get
			{
				return new LocalizedString("MissingVersion", "", false, false, AuthorizationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidGroupAttributes
		{
			get
			{
				return new LocalizedString("InvalidGroupAttributes", "", false, false, AuthorizationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AuthenticationTypeIsMissing
		{
			get
			{
				return new LocalizedString("AuthenticationTypeIsMissing", "Ex58BE70", false, true, AuthorizationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidRestrictedGroupLength
		{
			get
			{
				return new LocalizedString("InvalidRestrictedGroupLength", "", false, false, AuthorizationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserSidMustNotHaveAttributes
		{
			get
			{
				return new LocalizedString("UserSidMustNotHaveAttributes", "Ex7DC171", false, true, AuthorizationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidExtensionDataValue
		{
			get
			{
				return new LocalizedString("InvalidExtensionDataValue", "", false, false, AuthorizationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MissingUserSid
		{
			get
			{
				return new LocalizedString("MissingUserSid", "Ex256AEB", false, true, AuthorizationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidGroupSidValue
		{
			get
			{
				return new LocalizedString("InvalidGroupSidValue", "", false, false, AuthorizationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SerializedAccessTokenParserException(int lineNumber, int position, LocalizedString reason)
		{
			return new LocalizedString("SerializedAccessTokenParserException", "Ex0D7072", false, true, AuthorizationStrings.ResourceManager, new object[]
			{
				lineNumber,
				position,
				reason
			});
		}

		public static LocalizedString InvalidFieldType
		{
			get
			{
				return new LocalizedString("InvalidFieldType", "", false, false, AuthorizationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidXml
		{
			get
			{
				return new LocalizedString("InvalidXml", "Ex844887", false, true, AuthorizationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidRestrictedGroupAttributesValue
		{
			get
			{
				return new LocalizedString("InvalidRestrictedGroupAttributesValue", "", false, false, AuthorizationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidWindowsAccessToken
		{
			get
			{
				return new LocalizedString("InvalidWindowsAccessToken", "", false, false, AuthorizationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MultipleUserSid
		{
			get
			{
				return new LocalizedString("MultipleUserSid", "Ex29A505", false, true, AuthorizationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExpectingEndOfSid
		{
			get
			{
				return new LocalizedString("ExpectingEndOfSid", "Ex66DF42", false, true, AuthorizationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidRoot
		{
			get
			{
				return new LocalizedString("InvalidRoot", "Ex5F98BF", false, true, AuthorizationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidSidType
		{
			get
			{
				return new LocalizedString("InvalidSidType", "ExF4750F", false, true, AuthorizationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CommonAccessTokenException(int version, LocalizedString reason)
		{
			return new LocalizedString("CommonAccessTokenException", "", false, false, AuthorizationStrings.ResourceManager, new object[]
			{
				version,
				reason
			});
		}

		public static LocalizedString ExpectingSidValue
		{
			get
			{
				return new LocalizedString("ExpectingSidValue", "Ex06465A", false, true, AuthorizationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MissingTokenType
		{
			get
			{
				return new LocalizedString("MissingTokenType", "", false, false, AuthorizationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidExtensionDataKey
		{
			get
			{
				return new LocalizedString("InvalidExtensionDataKey", "", false, false, AuthorizationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExpectingWindowsAccessToken
		{
			get
			{
				return new LocalizedString("ExpectingWindowsAccessToken", "", false, false, AuthorizationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MissingIsCompressed
		{
			get
			{
				return new LocalizedString("MissingIsCompressed", "", false, false, AuthorizationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TooManySidNodes(string userName, int maximumSidCount)
		{
			return new LocalizedString("TooManySidNodes", "Ex823B5C", false, true, AuthorizationStrings.ResourceManager, new object[]
			{
				userName,
				maximumSidCount
			});
		}

		public static LocalizedString InvalidGroupLength
		{
			get
			{
				return new LocalizedString("InvalidGroupLength", "", false, false, AuthorizationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidRestrictedGroupSidValue
		{
			get
			{
				return new LocalizedString("InvalidRestrictedGroupSidValue", "", false, false, AuthorizationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidAttributeValue
		{
			get
			{
				return new LocalizedString("InvalidAttributeValue", "Ex44BCDA", false, true, AuthorizationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidRestrictedGroupAttributes
		{
			get
			{
				return new LocalizedString("InvalidRestrictedGroupAttributes", "", false, false, AuthorizationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(AuthorizationStrings.IDs key)
		{
			return new LocalizedString(AuthorizationStrings.stringIDs[(uint)key], AuthorizationStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(31);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Net.AuthorizationStrings", typeof(AuthorizationStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			UnknownExtensionDataKey = 1929848388U,
			InvalidExtensionDataLength = 508499146U,
			SidNodeExpected = 3093504254U,
			InvalidCommonAccessTokenString = 1823279302U,
			LogonNameIsMissing = 2930595838U,
			InvalidGroupAttributesValue = 524350434U,
			MissingVersion = 2738351686U,
			InvalidGroupAttributes = 3881300465U,
			AuthenticationTypeIsMissing = 3419671342U,
			InvalidRestrictedGroupLength = 3730243391U,
			UserSidMustNotHaveAttributes = 2708243460U,
			InvalidExtensionDataValue = 4181701447U,
			MissingUserSid = 945651915U,
			InvalidGroupSidValue = 2952593655U,
			InvalidFieldType = 3725269515U,
			InvalidXml = 3321965778U,
			InvalidRestrictedGroupAttributesValue = 2765023819U,
			InvalidWindowsAccessToken = 3428857605U,
			MultipleUserSid = 1782138211U,
			ExpectingEndOfSid = 832686951U,
			InvalidRoot = 3955902527U,
			InvalidSidType = 4129109959U,
			ExpectingSidValue = 1629171792U,
			MissingTokenType = 787285511U,
			InvalidExtensionDataKey = 102074919U,
			ExpectingWindowsAccessToken = 986794111U,
			MissingIsCompressed = 3967521285U,
			InvalidGroupLength = 4168034456U,
			InvalidRestrictedGroupSidValue = 31770294U,
			InvalidAttributeValue = 369811396U,
			InvalidRestrictedGroupAttributes = 1681390866U
		}

		private enum ParamIDs
		{
			InvalidSidAttribute,
			InvalidRootAttribute,
			SerializedAccessTokenParserException,
			CommonAccessTokenException,
			TooManySidNodes
		}
	}
}
