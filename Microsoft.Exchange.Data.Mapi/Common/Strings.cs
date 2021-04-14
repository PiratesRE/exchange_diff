using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Mapi.Common
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(1413397944U, "ExceptionIdentityNull");
			Strings.stringIDs.Add(4167957496U, "ExceptionWriteReadOnlyData");
			Strings.stringIDs.Add(2515788448U, "SessionLimitExceptionError");
			Strings.stringIDs.Add(2090231513U, "ConstantNull");
			Strings.stringIDs.Add(773445177U, "ExceptionModifyIpmSubtree");
			Strings.stringIDs.Add(2760297639U, "ExceptionFormatNotSupported");
			Strings.stringIDs.Add(2613900068U, "ExceptionSessionInvalid");
			Strings.stringIDs.Add(1292191704U, "MapiNetworkErrorExceptionErrorSimple");
			Strings.stringIDs.Add(2484551555U, "ErrorMailboxStatisticsMailboxGuidEmpty");
			Strings.stringIDs.Add(3459524064U, "ExceptionIdentityTypeInvalid");
			Strings.stringIDs.Add(806229667U, "MapiAccessDeniedExceptionErrorSimple");
			Strings.stringIDs.Add(1655815296U, "ExceptionIdentityInvalid");
			Strings.stringIDs.Add(477740681U, "ExceptionConnectionNotConfigurated");
			Strings.stringIDs.Add(1136983657U, "ExceptionRawMapiEntryNull");
			Strings.stringIDs.Add(440370356U, "DatabaseUnavailableExceptionErrorSimple");
			Strings.stringIDs.Add(2921286064U, "ExceptionUnexpected");
			Strings.stringIDs.Add(1733928998U, "ExceptionModifyNonIpmSubtree");
			Strings.stringIDs.Add(461525725U, "ConstantNa");
			Strings.stringIDs.Add(695822400U, "ExceptionSessionNull");
		}

		public static LocalizedString ExceptionIdentityNull
		{
			get
			{
				return new LocalizedString("ExceptionIdentityNull", "ExF9B53F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionWriteReadOnlyData
		{
			get
			{
				return new LocalizedString("ExceptionWriteReadOnlyData", "Ex4D7608", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMapiTableSetColumn(string id, string server)
		{
			return new LocalizedString("ErrorMapiTableSetColumn", "Ex6B87ED", false, true, Strings.ResourceManager, new object[]
			{
				id,
				server
			});
		}

		public static LocalizedString ExceptionFindObject(string typeTarget, string root)
		{
			return new LocalizedString("ExceptionFindObject", "ExB6ECCE", false, true, Strings.ResourceManager, new object[]
			{
				typeTarget,
				root
			});
		}

		public static LocalizedString PublicFolderNotFoundExceptionError(string folder)
		{
			return new LocalizedString("PublicFolderNotFoundExceptionError", "ExDBA130", false, true, Strings.ResourceManager, new object[]
			{
				folder
			});
		}

		public static LocalizedString ExceptionNewObject(string identity)
		{
			return new LocalizedString("ExceptionNewObject", "ExE0BBF6", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString SessionLimitExceptionError
		{
			get
			{
				return new LocalizedString("SessionLimitExceptionError", "ExF50FBF", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FolderAlreadyExistsExceptionError(string folder)
		{
			return new LocalizedString("FolderAlreadyExistsExceptionError", "ExBB1A32", false, true, Strings.ResourceManager, new object[]
			{
				folder
			});
		}

		public static LocalizedString ErrorMapiTableSeekRow(string id, string server)
		{
			return new LocalizedString("ErrorMapiTableSeekRow", "ExA1789E", false, true, Strings.ResourceManager, new object[]
			{
				id,
				server
			});
		}

		public static LocalizedString ErrorGetAddressBookEntryIdFromLegacyDN(string legacyDN)
		{
			return new LocalizedString("ErrorGetAddressBookEntryIdFromLegacyDN", "ExFFCC96", false, true, Strings.ResourceManager, new object[]
			{
				legacyDN
			});
		}

		public static LocalizedString ExceptionSchemaInvalidCast(string type)
		{
			return new LocalizedString("ExceptionSchemaInvalidCast", "Ex2BE881", false, true, Strings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString ConstantNull
		{
			get
			{
				return new LocalizedString("ConstantNull", "Ex5D99C9", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PublicStoreLogonFailedExceptionError(string server)
		{
			return new LocalizedString("PublicStoreLogonFailedExceptionError", "Ex61A326", false, true, Strings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString MapiCalculatedPropertyGettingExceptionError(string name, string details)
		{
			return new LocalizedString("MapiCalculatedPropertyGettingExceptionError", "Ex8806A2", false, true, Strings.ResourceManager, new object[]
			{
				name,
				details
			});
		}

		public static LocalizedString ExceptionStartHierarchyReplication(string databaseId)
		{
			return new LocalizedString("ExceptionStartHierarchyReplication", "Ex622F86", false, true, Strings.ResourceManager, new object[]
			{
				databaseId
			});
		}

		public static LocalizedString ExceptionModifyIpmSubtree
		{
			get
			{
				return new LocalizedString("ExceptionModifyIpmSubtree", "Ex848D2C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToRefreshMailboxExceptionError(string exception, string mailbox)
		{
			return new LocalizedString("FailedToRefreshMailboxExceptionError", "ExF068FA", false, true, Strings.ResourceManager, new object[]
			{
				exception,
				mailbox
			});
		}

		public static LocalizedString ExceptionFormatNotSupported
		{
			get
			{
				return new LocalizedString("ExceptionFormatNotSupported", "ExA296C3", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorFolderSeparatorInFolderName(string separator, string folderName)
		{
			return new LocalizedString("ErrorFolderSeparatorInFolderName", "Ex3210D1", false, true, Strings.ResourceManager, new object[]
			{
				separator,
				folderName
			});
		}

		public static LocalizedString ErrorMapiTableQueryRows(string id, string server)
		{
			return new LocalizedString("ErrorMapiTableQueryRows", "Ex99ECEA", false, true, Strings.ResourceManager, new object[]
			{
				id,
				server
			});
		}

		public static LocalizedString ExceptionReadObject(string type, string identity)
		{
			return new LocalizedString("ExceptionReadObject", "Ex0ED9FD", false, true, Strings.ResourceManager, new object[]
			{
				type,
				identity
			});
		}

		public static LocalizedString ErrorGetPublicFolderAclTableMapiModifyTable(string id, string server)
		{
			return new LocalizedString("ErrorGetPublicFolderAclTableMapiModifyTable", "ExBF8905", false, true, Strings.ResourceManager, new object[]
			{
				id,
				server
			});
		}

		public static LocalizedString ExceptionUnmatchedPropTag(string sourceTag, string targetTag)
		{
			return new LocalizedString("ExceptionUnmatchedPropTag", "Ex31BFA5", false, true, Strings.ResourceManager, new object[]
			{
				sourceTag,
				targetTag
			});
		}

		public static LocalizedString ExceptionSessionInvalid
		{
			get
			{
				return new LocalizedString("ExceptionSessionInvalid", "ExAD9089", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionSetLocalReplicaAgeLimit(string databaseId)
		{
			return new LocalizedString("ExceptionSetLocalReplicaAgeLimit", "ExD7D31C", false, true, Strings.ResourceManager, new object[]
			{
				databaseId
			});
		}

		public static LocalizedString ErrorRemovalPartialCompleted(string identity)
		{
			return new LocalizedString("ErrorRemovalPartialCompleted", "Ex70C83C", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString ErrorDeletePropsProblem(string identity, string problemCount)
		{
			return new LocalizedString("ErrorDeletePropsProblem", "Ex962F9E", false, true, Strings.ResourceManager, new object[]
			{
				identity,
				problemCount
			});
		}

		public static LocalizedString MapiNetworkErrorExceptionErrorSimple
		{
			get
			{
				return new LocalizedString("MapiNetworkErrorExceptionErrorSimple", "Ex50A5C2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSetPublicFolderAdminSecurityDescriptor(string folderId, string server)
		{
			return new LocalizedString("ErrorSetPublicFolderAdminSecurityDescriptor", "Ex220D05", false, true, Strings.ResourceManager, new object[]
			{
				folderId,
				server
			});
		}

		public static LocalizedString ErrorMailboxStatisticsMailboxGuidEmpty
		{
			get
			{
				return new LocalizedString("ErrorMailboxStatisticsMailboxGuidEmpty", "ExD4BBB4", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionIdentityTypeInvalid
		{
			get
			{
				return new LocalizedString("ExceptionIdentityTypeInvalid", "Ex6D9AAB", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseUnavailableByIdentityExceptionError(string database, string server)
		{
			return new LocalizedString("DatabaseUnavailableByIdentityExceptionError", "ExD3EB33", false, true, Strings.ResourceManager, new object[]
			{
				database,
				server
			});
		}

		public static LocalizedString ExceptionNotMultiValuedPropertyDefinition(string name)
		{
			return new LocalizedString("ExceptionNotMultiValuedPropertyDefinition", "Ex502CD6", false, true, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ErrorCannotUpdateIdentityFolderPath(string identity)
		{
			return new LocalizedString("ErrorCannotUpdateIdentityFolderPath", "Ex89B9D5", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString ExceptionNoIdeaConvertPropType(string propType)
		{
			return new LocalizedString("ExceptionNoIdeaConvertPropType", "Ex86622A", false, true, Strings.ResourceManager, new object[]
			{
				propType
			});
		}

		public static LocalizedString MailboxNotFoundExceptionError(string mailbox)
		{
			return new LocalizedString("MailboxNotFoundExceptionError", "ExE692E3", false, true, Strings.ResourceManager, new object[]
			{
				mailbox
			});
		}

		public static LocalizedString ErrorPropProblem(string propTag, string propType, string sCode)
		{
			return new LocalizedString("ErrorPropProblem", "Ex334E31", false, true, Strings.ResourceManager, new object[]
			{
				propTag,
				propType,
				sCode
			});
		}

		public static LocalizedString ExceptionObjectNotConsistent(string identity)
		{
			return new LocalizedString("ExceptionObjectNotConsistent", "Ex500682", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString ExceptionGetDatabaseStatus(string exception)
		{
			return new LocalizedString("ExceptionGetDatabaseStatus", "Ex2FCB42", false, true, Strings.ResourceManager, new object[]
			{
				exception
			});
		}

		public static LocalizedString MapiNetworkErrorExceptionError(string server)
		{
			return new LocalizedString("MapiNetworkErrorExceptionError", "ExB9132A", false, true, Strings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString DatabaseUnavailableExceptionError(string server)
		{
			return new LocalizedString("DatabaseUnavailableExceptionError", "Ex1C84F8", false, true, Strings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString ExceptionDeleteObject(string identiy)
		{
			return new LocalizedString("ExceptionDeleteObject", "ExF3BE50", false, true, Strings.ResourceManager, new object[]
			{
				identiy
			});
		}

		public static LocalizedString ErrorGetMapiTableWithIdentityAndServer(string id, string server)
		{
			return new LocalizedString("ErrorGetMapiTableWithIdentityAndServer", "Ex8CEA10", false, true, Strings.ResourceManager, new object[]
			{
				id,
				server
			});
		}

		public static LocalizedString ExceptionCriticalPropTagMissing(string propTag)
		{
			return new LocalizedString("ExceptionCriticalPropTagMissing", "ExD89884", false, true, Strings.ResourceManager, new object[]
			{
				propTag
			});
		}

		public static LocalizedString ExceptionNoIdeaGenerateMultiValuedProperty(string type)
		{
			return new LocalizedString("ExceptionNoIdeaGenerateMultiValuedProperty", "Ex416625", false, true, Strings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString ErrorCannotUpdateIdentityLegacyDistinguishedName(string identity)
		{
			return new LocalizedString("ErrorCannotUpdateIdentityLegacyDistinguishedName", "ExFEDF53", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString ExceptionSetMailboxSecurityDescriptor(string databaseGuid, string mailboxGuid)
		{
			return new LocalizedString("ExceptionSetMailboxSecurityDescriptor", "ExF63558", false, true, Strings.ResourceManager, new object[]
			{
				databaseGuid,
				mailboxGuid
			});
		}

		public static LocalizedString ErrorSetPublicFolderAdminSecurityDescriptorWithErrorCodes(string folderId, string server, string problems)
		{
			return new LocalizedString("ErrorSetPublicFolderAdminSecurityDescriptorWithErrorCodes", "Ex793F32", false, true, Strings.ResourceManager, new object[]
			{
				folderId,
				server,
				problems
			});
		}

		public static LocalizedString ErrorByteArrayLength(string expected, string actual)
		{
			return new LocalizedString("ErrorByteArrayLength", "Ex8BA501", false, true, Strings.ResourceManager, new object[]
			{
				expected,
				actual
			});
		}

		public static LocalizedString MapiPackingExceptionError(string name, string value, string type, string isMultiValued, string propTag, string propType, string details)
		{
			return new LocalizedString("MapiPackingExceptionError", "Ex2CC01D", false, true, Strings.ResourceManager, new object[]
			{
				name,
				value,
				type,
				isMultiValued,
				propTag,
				propType,
				details
			});
		}

		public static LocalizedString ErrorGetPublicFolderAdminSecurityDescriptor(string folderId, string server)
		{
			return new LocalizedString("ErrorGetPublicFolderAdminSecurityDescriptor", "Ex8AF0E2", false, true, Strings.ResourceManager, new object[]
			{
				folderId,
				server
			});
		}

		public static LocalizedString ExceptionObjectNotRemovable(string identity)
		{
			return new LocalizedString("ExceptionObjectNotRemovable", "ExC1198C", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString MapiAccessDeniedExceptionErrorSimple
		{
			get
			{
				return new LocalizedString("MapiAccessDeniedExceptionErrorSimple", "Ex28386E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxLogonFailedInDatabaseExceptionError(string mailbox, string database, string server)
		{
			return new LocalizedString("MailboxLogonFailedInDatabaseExceptionError", "ExC43ABC", false, true, Strings.ResourceManager, new object[]
			{
				mailbox,
				database,
				server
			});
		}

		public static LocalizedString ErrorModifyMapiTableWithIdentityAndServer(string id, string server)
		{
			return new LocalizedString("ErrorModifyMapiTableWithIdentityAndServer", "ExA1634E", false, true, Strings.ResourceManager, new object[]
			{
				id,
				server
			});
		}

		public static LocalizedString ErrorSetPropsProblem(string identity, string problemCount)
		{
			return new LocalizedString("ErrorSetPropsProblem", "Ex286F9C", false, true, Strings.ResourceManager, new object[]
			{
				identity,
				problemCount
			});
		}

		public static LocalizedString ExceptionGetMailboxSecurityDescriptor(string databaseGuid, string mailboxGuid)
		{
			return new LocalizedString("ExceptionGetMailboxSecurityDescriptor", "Ex325678", false, true, Strings.ResourceManager, new object[]
			{
				databaseGuid,
				mailboxGuid
			});
		}

		public static LocalizedString ExceptionModifyFolder(string folderId)
		{
			return new LocalizedString("ExceptionModifyFolder", "ExC74894", false, true, Strings.ResourceManager, new object[]
			{
				folderId
			});
		}

		public static LocalizedString ExceptionIdentityInvalid
		{
			get
			{
				return new LocalizedString("ExceptionIdentityInvalid", "Ex4F7C9E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionConnectionNotConfigurated
		{
			get
			{
				return new LocalizedString("ExceptionConnectionNotConfigurated", "Ex52E8B2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiAccessDeniedExceptionError(string id)
		{
			return new LocalizedString("MapiAccessDeniedExceptionError", "Ex4B80CC", false, true, Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString MapiExceptionNoReplicaHereError(string type, string root)
		{
			return new LocalizedString("MapiExceptionNoReplicaHereError", "Ex0CA822", false, true, Strings.ResourceManager, new object[]
			{
				type,
				root
			});
		}

		public static LocalizedString MapiExtractingExceptionError(string name, string propTag, string propType, string rawValue, string rawValueType, string type, string isMultiValued, string details)
		{
			return new LocalizedString("MapiExtractingExceptionError", "Ex87CB90", false, true, Strings.ResourceManager, new object[]
			{
				name,
				propTag,
				propType,
				rawValue,
				rawValueType,
				type,
				isMultiValued,
				details
			});
		}

		public static LocalizedString ErrorCannotUpdateIdentityEntryId(string identity)
		{
			return new LocalizedString("ErrorCannotUpdateIdentityEntryId", "Ex1F0A53", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString MailboxNotFoundInDatabaseExceptionError(string mailbox, string database)
		{
			return new LocalizedString("MailboxNotFoundInDatabaseExceptionError", "ExAA6208", false, true, Strings.ResourceManager, new object[]
			{
				mailbox,
				database
			});
		}

		public static LocalizedString ExceptionDeleteMailbox(string database, string mailbox)
		{
			return new LocalizedString("ExceptionDeleteMailbox", "Ex05632B", false, true, Strings.ResourceManager, new object[]
			{
				database,
				mailbox
			});
		}

		public static LocalizedString ExceptionRawMapiEntryNull
		{
			get
			{
				return new LocalizedString("ExceptionRawMapiEntryNull", "Ex2B7298", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseUnavailableExceptionErrorSimple
		{
			get
			{
				return new LocalizedString("DatabaseUnavailableExceptionErrorSimple", "ExF71825", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionUnexpected
		{
			get
			{
				return new LocalizedString("ExceptionUnexpected", "ExDB8AD0", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionStartContentReplication(string databaseId, string folderId)
		{
			return new LocalizedString("ExceptionStartContentReplication", "Ex774B89", false, true, Strings.ResourceManager, new object[]
			{
				databaseId,
				folderId
			});
		}

		public static LocalizedString ErrorMandatoryPropertyMissing(string property)
		{
			return new LocalizedString("ErrorMandatoryPropertyMissing", "ExB9901F", false, true, Strings.ResourceManager, new object[]
			{
				property
			});
		}

		public static LocalizedString ExceptionSaveObject(string identity)
		{
			return new LocalizedString("ExceptionSaveObject", "Ex99F3B7", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString LogonFailedExceptionError(string message, string server)
		{
			return new LocalizedString("LogonFailedExceptionError", "ExE6399D", false, true, Strings.ResourceManager, new object[]
			{
				message,
				server
			});
		}

		public static LocalizedString MapiCalculatedPropertySettingExceptionError(string name, string value, string details)
		{
			return new LocalizedString("MapiCalculatedPropertySettingExceptionError", "Ex87915A", false, true, Strings.ResourceManager, new object[]
			{
				name,
				value,
				details
			});
		}

		public static LocalizedString ExceptionModifyNonIpmSubtree
		{
			get
			{
				return new LocalizedString("ExceptionModifyNonIpmSubtree", "Ex580166", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConstantNa
		{
			get
			{
				return new LocalizedString("ConstantNa", "ExB2E702", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxLogonFailedExceptionError(string mailbox, string server)
		{
			return new LocalizedString("MailboxLogonFailedExceptionError", "ExC7D380", false, true, Strings.ResourceManager, new object[]
			{
				mailbox,
				server
			});
		}

		public static LocalizedString ExceptionSessionNull
		{
			get
			{
				return new LocalizedString("ExceptionSessionNull", "ExAC8234", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorGetGetLegacyDNFromAddressBookEntryId(string entryId)
		{
			return new LocalizedString("ErrorGetGetLegacyDNFromAddressBookEntryId", "ExD29F1F", false, true, Strings.ResourceManager, new object[]
			{
				entryId
			});
		}

		public static LocalizedString ExceptionObjectStateInvalid(string state)
		{
			return new LocalizedString("ExceptionObjectStateInvalid", "Ex290F60", false, true, Strings.ResourceManager, new object[]
			{
				state
			});
		}

		public static LocalizedString ExceptionFailedToLetStorePickupMailboxChange(string mailbox, string mdb)
		{
			return new LocalizedString("ExceptionFailedToLetStorePickupMailboxChange", "Ex46AFD1", false, true, Strings.ResourceManager, new object[]
			{
				mailbox,
				mdb
			});
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(19);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Data.Mapi.Common.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			ExceptionIdentityNull = 1413397944U,
			ExceptionWriteReadOnlyData = 4167957496U,
			SessionLimitExceptionError = 2515788448U,
			ConstantNull = 2090231513U,
			ExceptionModifyIpmSubtree = 773445177U,
			ExceptionFormatNotSupported = 2760297639U,
			ExceptionSessionInvalid = 2613900068U,
			MapiNetworkErrorExceptionErrorSimple = 1292191704U,
			ErrorMailboxStatisticsMailboxGuidEmpty = 2484551555U,
			ExceptionIdentityTypeInvalid = 3459524064U,
			MapiAccessDeniedExceptionErrorSimple = 806229667U,
			ExceptionIdentityInvalid = 1655815296U,
			ExceptionConnectionNotConfigurated = 477740681U,
			ExceptionRawMapiEntryNull = 1136983657U,
			DatabaseUnavailableExceptionErrorSimple = 440370356U,
			ExceptionUnexpected = 2921286064U,
			ExceptionModifyNonIpmSubtree = 1733928998U,
			ConstantNa = 461525725U,
			ExceptionSessionNull = 695822400U
		}

		private enum ParamIDs
		{
			ErrorMapiTableSetColumn,
			ExceptionFindObject,
			PublicFolderNotFoundExceptionError,
			ExceptionNewObject,
			FolderAlreadyExistsExceptionError,
			ErrorMapiTableSeekRow,
			ErrorGetAddressBookEntryIdFromLegacyDN,
			ExceptionSchemaInvalidCast,
			PublicStoreLogonFailedExceptionError,
			MapiCalculatedPropertyGettingExceptionError,
			ExceptionStartHierarchyReplication,
			FailedToRefreshMailboxExceptionError,
			ErrorFolderSeparatorInFolderName,
			ErrorMapiTableQueryRows,
			ExceptionReadObject,
			ErrorGetPublicFolderAclTableMapiModifyTable,
			ExceptionUnmatchedPropTag,
			ExceptionSetLocalReplicaAgeLimit,
			ErrorRemovalPartialCompleted,
			ErrorDeletePropsProblem,
			ErrorSetPublicFolderAdminSecurityDescriptor,
			DatabaseUnavailableByIdentityExceptionError,
			ExceptionNotMultiValuedPropertyDefinition,
			ErrorCannotUpdateIdentityFolderPath,
			ExceptionNoIdeaConvertPropType,
			MailboxNotFoundExceptionError,
			ErrorPropProblem,
			ExceptionObjectNotConsistent,
			ExceptionGetDatabaseStatus,
			MapiNetworkErrorExceptionError,
			DatabaseUnavailableExceptionError,
			ExceptionDeleteObject,
			ErrorGetMapiTableWithIdentityAndServer,
			ExceptionCriticalPropTagMissing,
			ExceptionNoIdeaGenerateMultiValuedProperty,
			ErrorCannotUpdateIdentityLegacyDistinguishedName,
			ExceptionSetMailboxSecurityDescriptor,
			ErrorSetPublicFolderAdminSecurityDescriptorWithErrorCodes,
			ErrorByteArrayLength,
			MapiPackingExceptionError,
			ErrorGetPublicFolderAdminSecurityDescriptor,
			ExceptionObjectNotRemovable,
			MailboxLogonFailedInDatabaseExceptionError,
			ErrorModifyMapiTableWithIdentityAndServer,
			ErrorSetPropsProblem,
			ExceptionGetMailboxSecurityDescriptor,
			ExceptionModifyFolder,
			MapiAccessDeniedExceptionError,
			MapiExceptionNoReplicaHereError,
			MapiExtractingExceptionError,
			ErrorCannotUpdateIdentityEntryId,
			MailboxNotFoundInDatabaseExceptionError,
			ExceptionDeleteMailbox,
			ExceptionStartContentReplication,
			ErrorMandatoryPropertyMissing,
			ExceptionSaveObject,
			LogonFailedExceptionError,
			MapiCalculatedPropertySettingExceptionError,
			MailboxLogonFailedExceptionError,
			ErrorGetGetLegacyDNFromAddressBookEntryId,
			ExceptionObjectStateInvalid,
			ExceptionFailedToLetStorePickupMailboxChange
		}
	}
}
