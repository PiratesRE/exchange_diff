using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.AirSync
{
	internal static class EASServerStrings
	{
		static EASServerStrings()
		{
			EASServerStrings.stringIDs.Add(1807771821U, "AdminMailDevicePhoneNumber");
			EASServerStrings.stringIDs.Add(842327697U, "AdminMailBodyDeviceAccessState");
			EASServerStrings.stringIDs.Add(2127272853U, "AdminMailBodyDeviceModel");
			EASServerStrings.stringIDs.Add(3481243890U, "AdminMailBody4");
			EASServerStrings.stringIDs.Add(4167251934U, "MissingDiscoveryInfoError");
			EASServerStrings.stringIDs.Add(2341808075U, "AdminMailBodyDeviceID");
			EASServerStrings.stringIDs.Add(4280143591U, "AdminMailDeviceAccessControlRule");
			EASServerStrings.stringIDs.Add(3127591392U, "ExBegin");
			EASServerStrings.stringIDs.Add(1743625299U, "Null");
			EASServerStrings.stringIDs.Add(2057529303U, "AdminMailBodyEASVersion");
			EASServerStrings.stringIDs.Add(4013839307U, "SchemaDirectoryNotAccessible");
			EASServerStrings.stringIDs.Add(1704351524U, "AdminMailBodyDeviceType");
			EASServerStrings.stringIDs.Add(2227082144U, "ExEnd");
			EASServerStrings.stringIDs.Add(25353299U, "AnonymousAccessError");
			EASServerStrings.stringIDs.Add(508148408U, "MismatchSyncStateError");
			EASServerStrings.stringIDs.Add(1179008650U, "AdminMailBodyDeviceOS");
			EASServerStrings.stringIDs.Add(2628658688U, "UnableToLoadAddressBookProvider");
			EASServerStrings.stringIDs.Add(2920971799U, "AdminMailUser");
			EASServerStrings.stringIDs.Add(3077959363U, "AdminMailBody1");
			EASServerStrings.stringIDs.Add(862352803U, "ExInner");
			EASServerStrings.stringIDs.Add(2474028085U, "AdminMailBodyDeviceAccessStateReason");
			EASServerStrings.stringIDs.Add(134200922U, "UnhandledException");
			EASServerStrings.stringIDs.Add(2674674836U, "AdminMailBody2");
			EASServerStrings.stringIDs.Add(3929860227U, "NoXmlResponse");
			EASServerStrings.stringIDs.Add(3181624402U, "AdminMailDeviceInformation");
			EASServerStrings.stringIDs.Add(2585038184U, "AdminMailDevicePolicyStatus");
			EASServerStrings.stringIDs.Add(1372331172U, "AdminMailBodyDeviceUserAgent");
			EASServerStrings.stringIDs.Add(3870834299U, "AdminMailDevicePolicyApplied");
			EASServerStrings.stringIDs.Add(655864836U, "AdminMailBodyDeviceIMEI");
		}

		public static LocalizedString AdminMailDevicePhoneNumber
		{
			get
			{
				return new LocalizedString("AdminMailDevicePhoneNumber", "ExD3244A", false, true, EASServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AdminMailBodyDeviceAccessState
		{
			get
			{
				return new LocalizedString("AdminMailBodyDeviceAccessState", "Ex65D6B4", false, true, EASServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AdminMailBodyDeviceModel
		{
			get
			{
				return new LocalizedString("AdminMailBodyDeviceModel", "Ex63810E", false, true, EASServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AdminMailBody4
		{
			get
			{
				return new LocalizedString("AdminMailBody4", "Ex27F5E1", false, true, EASServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SchemaUnknownCompilationError(string dirPath)
		{
			return new LocalizedString("SchemaUnknownCompilationError", "Ex5B1997", false, true, EASServerStrings.ResourceManager, new object[]
			{
				dirPath
			});
		}

		public static LocalizedString CannotFindSchemaClassException(string objclass, string schemaDN)
		{
			return new LocalizedString("CannotFindSchemaClassException", "Ex9C5355", false, true, EASServerStrings.ResourceManager, new object[]
			{
				objclass,
				schemaDN
			});
		}

		public static LocalizedString XmlResponse(string response)
		{
			return new LocalizedString("XmlResponse", "Ex650E29", false, true, EASServerStrings.ResourceManager, new object[]
			{
				response
			});
		}

		public static LocalizedString MissingDiscoveryInfoError
		{
			get
			{
				return new LocalizedString("MissingDiscoveryInfoError", "Ex8E227F", false, true, EASServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToCreateNewActiveDevice(string deviceId, string deviceType, string user)
		{
			return new LocalizedString("FailedToCreateNewActiveDevice", "Ex8040ED", false, true, EASServerStrings.ResourceManager, new object[]
			{
				deviceId,
				deviceType,
				user
			});
		}

		public static LocalizedString AdminMailBodyDeviceID
		{
			get
			{
				return new LocalizedString("AdminMailBodyDeviceID", "Ex84E4C2", false, true, EASServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MultipleVirtualDirectoriesDetected(string root, string metabaseUrl)
		{
			return new LocalizedString("MultipleVirtualDirectoriesDetected", "Ex668707", false, true, EASServerStrings.ResourceManager, new object[]
			{
				root,
				metabaseUrl
			});
		}

		public static LocalizedString MissingADVirtualDirectory(string root, string metabaseUrl)
		{
			return new LocalizedString("MissingADVirtualDirectory", "ExA47A39", false, true, EASServerStrings.ResourceManager, new object[]
			{
				root,
				metabaseUrl
			});
		}

		public static LocalizedString AdminMailDeviceAccessControlRule
		{
			get
			{
				return new LocalizedString("AdminMailDeviceAccessControlRule", "Ex740E1A", false, true, EASServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExBegin
		{
			get
			{
				return new LocalizedString("ExBegin", "ExA5BD26", false, true, EASServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Null
		{
			get
			{
				return new LocalizedString("Null", "Ex4BE0F9", false, true, EASServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SyncStatusCode(int statusCode)
		{
			return new LocalizedString("SyncStatusCode", "Ex9FC7DD", false, true, EASServerStrings.ResourceManager, new object[]
			{
				statusCode
			});
		}

		public static LocalizedString ExMessage(string msg)
		{
			return new LocalizedString("ExMessage", "ExB94A69", false, true, EASServerStrings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString AdminMailBodyEASVersion
		{
			get
			{
				return new LocalizedString("AdminMailBodyEASVersion", "ExC3AE87", false, true, EASServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SchemaDirectoryNotAccessible
		{
			get
			{
				return new LocalizedString("SchemaDirectoryNotAccessible", "Ex49AF6B", false, true, EASServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AdminMailBodyDeviceType
		{
			get
			{
				return new LocalizedString("AdminMailBodyDeviceType", "Ex1F6A6D", false, true, EASServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExEnd
		{
			get
			{
				return new LocalizedString("ExEnd", "Ex2ABF18", false, true, EASServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AnonymousAccessError
		{
			get
			{
				return new LocalizedString("AnonymousAccessError", "Ex3C5591", false, true, EASServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MismatchSyncStateError
		{
			get
			{
				return new LocalizedString("MismatchSyncStateError", "Ex73418B", false, true, EASServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SchemaFileCorrupted(string fileName)
		{
			return new LocalizedString("SchemaFileCorrupted", "", false, false, EASServerStrings.ResourceManager, new object[]
			{
				fileName
			});
		}

		public static LocalizedString FailedToApplySecurityToContainer(string container)
		{
			return new LocalizedString("FailedToApplySecurityToContainer", "Ex032E9A", false, true, EASServerStrings.ResourceManager, new object[]
			{
				container
			});
		}

		public static LocalizedString InvalidDeviceFilterOperatorError(string filterOperator)
		{
			return new LocalizedString("InvalidDeviceFilterOperatorError", "", false, false, EASServerStrings.ResourceManager, new object[]
			{
				filterOperator
			});
		}

		public static LocalizedString AdminMailBodyDeviceOS
		{
			get
			{
				return new LocalizedString("AdminMailBodyDeviceOS", "Ex23054B", false, true, EASServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HttpStatusCode(int statusCode)
		{
			return new LocalizedString("HttpStatusCode", "Ex00CE77", false, true, EASServerStrings.ResourceManager, new object[]
			{
				statusCode
			});
		}

		public static LocalizedString UnableToLoadAddressBookProvider
		{
			get
			{
				return new LocalizedString("UnableToLoadAddressBookProvider", "Ex31DE6B", false, true, EASServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExType(string type)
		{
			return new LocalizedString("ExType", "Ex06B4F9", false, true, EASServerStrings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString InvalidDeviceFilterSettingsInAD(string organizationId, string errorDescription)
		{
			return new LocalizedString("InvalidDeviceFilterSettingsInAD", "", false, false, EASServerStrings.ResourceManager, new object[]
			{
				organizationId,
				errorDescription
			});
		}

		public static LocalizedString ExStackTrace(string trace)
		{
			return new LocalizedString("ExStackTrace", "Ex23A291", false, true, EASServerStrings.ResourceManager, new object[]
			{
				trace
			});
		}

		public static LocalizedString AdminMailUser
		{
			get
			{
				return new LocalizedString("AdminMailUser", "ExC95404", false, true, EASServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AdminMailBody1
		{
			get
			{
				return new LocalizedString("AdminMailBody1", "ExCBE0FE", false, true, EASServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInner
		{
			get
			{
				return new LocalizedString("ExInner", "Ex8687A5", false, true, EASServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AdminMailBodyDeviceAccessStateReason
		{
			get
			{
				return new LocalizedString("AdminMailBodyDeviceAccessStateReason", "ExE25DF3", false, true, EASServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AdminMailBodySentAt(string dateTime, string recipientsSMTP)
		{
			return new LocalizedString("AdminMailBodySentAt", "Ex0512BC", false, true, EASServerStrings.ResourceManager, new object[]
			{
				dateTime,
				recipientsSMTP
			});
		}

		public static LocalizedString MandatoryVirtualDirectoryDeleted(string dn)
		{
			return new LocalizedString("MandatoryVirtualDirectoryDeleted", "ExB7423B", false, true, EASServerStrings.ResourceManager, new object[]
			{
				dn
			});
		}

		public static LocalizedString AdminMailSubject(string displayName, string alias)
		{
			return new LocalizedString("AdminMailSubject", "ExA6B592", false, true, EASServerStrings.ResourceManager, new object[]
			{
				displayName,
				alias
			});
		}

		public static LocalizedString UnhandledException
		{
			get
			{
				return new LocalizedString("UnhandledException", "ExA2F74B", false, true, EASServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AdminMailBody2
		{
			get
			{
				return new LocalizedString("AdminMailBody2", "ExFCFC8F", false, true, EASServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToResolveWellKnownGuid(string guid, string name)
		{
			return new LocalizedString("FailedToResolveWellKnownGuid", "ExCF1342", false, true, EASServerStrings.ResourceManager, new object[]
			{
				guid,
				name
			});
		}

		public static LocalizedString NoXmlResponse
		{
			get
			{
				return new LocalizedString("NoXmlResponse", "Ex4C1BFF", false, true, EASServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AdminMailDeviceInformation
		{
			get
			{
				return new LocalizedString("AdminMailDeviceInformation", "Ex660EE0", false, true, EASServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AdminMailDevicePolicyStatus
		{
			get
			{
				return new LocalizedString("AdminMailDevicePolicyStatus", "Ex18EE20", false, true, EASServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AdminMailBodyDeviceUserAgent
		{
			get
			{
				return new LocalizedString("AdminMailBodyDeviceUserAgent", "Ex591F4C", false, true, EASServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AdminMailDevicePolicyApplied
		{
			get
			{
				return new LocalizedString("AdminMailDevicePolicyApplied", "Ex9A8459", false, true, EASServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NullNTSD(string name)
		{
			return new LocalizedString("NullNTSD", "ExAF3C19", false, true, EASServerStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString SchemaDirectoryVersionNotAccessible(string dirPath)
		{
			return new LocalizedString("SchemaDirectoryVersionNotAccessible", "", false, false, EASServerStrings.ResourceManager, new object[]
			{
				dirPath
			});
		}

		public static LocalizedString ExLevel(string level)
		{
			return new LocalizedString("ExLevel", "Ex4B2ED2", false, true, EASServerStrings.ResourceManager, new object[]
			{
				level
			});
		}

		public static LocalizedString MissingADServer(string serverId)
		{
			return new LocalizedString("MissingADServer", "Ex114E29", false, true, EASServerStrings.ResourceManager, new object[]
			{
				serverId
			});
		}

		public static LocalizedString AdminMailBodyDeviceIMEI
		{
			get
			{
				return new LocalizedString("AdminMailBodyDeviceIMEI", "Ex4D98C5", false, true, EASServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(EASServerStrings.IDs key)
		{
			return new LocalizedString(EASServerStrings.stringIDs[(uint)key], EASServerStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(29);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.AirSync.EASServerStrings", typeof(EASServerStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			AdminMailDevicePhoneNumber = 1807771821U,
			AdminMailBodyDeviceAccessState = 842327697U,
			AdminMailBodyDeviceModel = 2127272853U,
			AdminMailBody4 = 3481243890U,
			MissingDiscoveryInfoError = 4167251934U,
			AdminMailBodyDeviceID = 2341808075U,
			AdminMailDeviceAccessControlRule = 4280143591U,
			ExBegin = 3127591392U,
			Null = 1743625299U,
			AdminMailBodyEASVersion = 2057529303U,
			SchemaDirectoryNotAccessible = 4013839307U,
			AdminMailBodyDeviceType = 1704351524U,
			ExEnd = 2227082144U,
			AnonymousAccessError = 25353299U,
			MismatchSyncStateError = 508148408U,
			AdminMailBodyDeviceOS = 1179008650U,
			UnableToLoadAddressBookProvider = 2628658688U,
			AdminMailUser = 2920971799U,
			AdminMailBody1 = 3077959363U,
			ExInner = 862352803U,
			AdminMailBodyDeviceAccessStateReason = 2474028085U,
			UnhandledException = 134200922U,
			AdminMailBody2 = 2674674836U,
			NoXmlResponse = 3929860227U,
			AdminMailDeviceInformation = 3181624402U,
			AdminMailDevicePolicyStatus = 2585038184U,
			AdminMailBodyDeviceUserAgent = 1372331172U,
			AdminMailDevicePolicyApplied = 3870834299U,
			AdminMailBodyDeviceIMEI = 655864836U
		}

		private enum ParamIDs
		{
			SchemaUnknownCompilationError,
			CannotFindSchemaClassException,
			XmlResponse,
			FailedToCreateNewActiveDevice,
			MultipleVirtualDirectoriesDetected,
			MissingADVirtualDirectory,
			SyncStatusCode,
			ExMessage,
			SchemaFileCorrupted,
			FailedToApplySecurityToContainer,
			InvalidDeviceFilterOperatorError,
			HttpStatusCode,
			ExType,
			InvalidDeviceFilterSettingsInAD,
			ExStackTrace,
			AdminMailBodySentAt,
			MandatoryVirtualDirectoryDeleted,
			AdminMailSubject,
			FailedToResolveWellKnownGuid,
			NullNTSD,
			SchemaDirectoryVersionNotAccessible,
			ExLevel,
			MissingADServer
		}
	}
}
