using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(4054043086U, "ReservedString1");
			Strings.stringIDs.Add(2470358009U, "ErrorUnableToGetGroupOwners");
			Strings.stringIDs.Add(366868245U, "SenderNotSpecifiedAndNotPresentInMessage");
			Strings.stringIDs.Add(2628020095U, "ValidateRepairInvalidUser");
			Strings.stringIDs.Add(3952589442U, "MimeDoesNotComplyWithStandards");
			Strings.stringIDs.Add(1272170093U, "ErrorUnableToGetCreatorFromGroupMailbox");
			Strings.stringIDs.Add(3855259485U, "ErrorLocalMachineIsNotExchangeServer");
			Strings.stringIDs.Add(3353703238U, "TestMessageDefaultBody");
			Strings.stringIDs.Add(3038652633U, "MustSpecifyAtLeastOneSmtpRecipientAddress");
			Strings.stringIDs.Add(3248953978U, "ValidateRepairUpdateMissingStatus");
			Strings.stringIDs.Add(210240938U, "WarningUnableToUpdateUserMailboxes");
			Strings.stringIDs.Add(1157487U, "ErrorUnableToGetUnifiedGroup");
			Strings.stringIDs.Add(110985478U, "CalendarValidationTask");
			Strings.stringIDs.Add(84207768U, "ErrorUnableToSessionWithAAD");
			Strings.stringIDs.Add(2906052647U, "WarningUnableToUpdateExchangeResources");
			Strings.stringIDs.Add(46749802U, "ErrorUnableToUpdateUnifiedGroup");
			Strings.stringIDs.Add(891487637U, "ErrorUnableToCreateUnifiedGroup");
			Strings.stringIDs.Add(3322833458U, "TestMessageDefaultSubject");
			Strings.stringIDs.Add(775190192U, "UnableToDiscoverDefaultDomain");
			Strings.stringIDs.Add(2995683657U, "MessageFileOrSenderMustBeSpecified");
			Strings.stringIDs.Add(2481051232U, "NoHubsAvailable");
			Strings.stringIDs.Add(2635939130U, "ExchangeSupportPSSnapInDescription");
			Strings.stringIDs.Add(332607451U, "SpnRegistrationSucceeded");
			Strings.stringIDs.Add(784901781U, "ErrorMissingWebDnsInformation");
			Strings.stringIDs.Add(4054043087U, "ReservedString2");
			Strings.stringIDs.Add(3311173118U, "ValidateRepairUpdateStatus");
			Strings.stringIDs.Add(4054043081U, "ReservedString4");
			Strings.stringIDs.Add(2589164433U, "ConfirmationMessageTestMessage");
			Strings.stringIDs.Add(1531210305U, "ConfirmationMessageRepairMigration");
			Strings.stringIDs.Add(1622178204U, "ErrorLocalServerIsNotMailboxServer");
			Strings.stringIDs.Add(4054043088U, "ReservedString3");
			Strings.stringIDs.Add(4142544366U, "MessageFileDataSpecifiedAsNull");
			Strings.stringIDs.Add(4054043082U, "ReservedString5");
		}

		public static LocalizedString ReservedString1
		{
			get
			{
				return new LocalizedString("ReservedString1", "ExDC7495", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WarningUnableToRemoveMembers(string members)
		{
			return new LocalizedString("WarningUnableToRemoveMembers", "", false, false, Strings.ResourceManager, new object[]
			{
				members
			});
		}

		public static LocalizedString ErrorUnableToGetGroupOwners
		{
			get
			{
				return new LocalizedString("ErrorUnableToGetGroupOwners", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpnRegistrationFailed(int errorCode)
		{
			return new LocalizedString("SpnRegistrationFailed", "Ex5337E8", false, true, Strings.ResourceManager, new object[]
			{
				errorCode
			});
		}

		public static LocalizedString SenderNotSpecifiedAndNotPresentInMessage
		{
			get
			{
				return new LocalizedString("SenderNotSpecifiedAndNotPresentInMessage", "Ex1EE6BD", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ValidateRepairInvalidUser
		{
			get
			{
				return new LocalizedString("ValidateRepairInvalidUser", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ValidatingCalendar(string username)
		{
			return new LocalizedString("ValidatingCalendar", "Ex304041", false, true, Strings.ResourceManager, new object[]
			{
				username
			});
		}

		public static LocalizedString ValidateRepairInvalidRevert(string name, string status)
		{
			return new LocalizedString("ValidateRepairInvalidRevert", "", false, false, Strings.ResourceManager, new object[]
			{
				name,
				status
			});
		}

		public static LocalizedString MimeDoesNotComplyWithStandards
		{
			get
			{
				return new LocalizedString("MimeDoesNotComplyWithStandards", "ExC2EC33", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ValidateRepairMultipleUsers(string name, int count)
		{
			return new LocalizedString("ValidateRepairMultipleUsers", "", false, false, Strings.ResourceManager, new object[]
			{
				name,
				count
			});
		}

		public static LocalizedString ErrorUnableToGetCreatorFromGroupMailbox
		{
			get
			{
				return new LocalizedString("ErrorUnableToGetCreatorFromGroupMailbox", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMissingServerFqdn(string idStringValue)
		{
			return new LocalizedString("ErrorMissingServerFqdn", "Ex174BA5", false, true, Strings.ResourceManager, new object[]
			{
				idStringValue
			});
		}

		public static LocalizedString ConfirmRepairSubscription(string action, string type, string mailboxData)
		{
			return new LocalizedString("ConfirmRepairSubscription", "", false, false, Strings.ResourceManager, new object[]
			{
				action,
				type,
				mailboxData
			});
		}

		public static LocalizedString ErrorNoLocalOrganizationMailbox(string identity)
		{
			return new LocalizedString("ErrorNoLocalOrganizationMailbox", "", false, false, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString ErrorLocalMachineIsNotExchangeServer
		{
			get
			{
				return new LocalizedString("ErrorLocalMachineIsNotExchangeServer", "ExF7A5D4", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TestMessageDefaultBody
		{
			get
			{
				return new LocalizedString("TestMessageDefaultBody", "Ex5D93AE", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MustSpecifyAtLeastOneSmtpRecipientAddress
		{
			get
			{
				return new LocalizedString("MustSpecifyAtLeastOneSmtpRecipientAddress", "Ex814118", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ValidateRepairUpdateMissingStatus
		{
			get
			{
				return new LocalizedString("ValidateRepairUpdateMissingStatus", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WarningUnableToUpdateUserMailboxes
		{
			get
			{
				return new LocalizedString("WarningUnableToUpdateUserMailboxes", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorGetRestrictionTableForFolderFailed(string databaseId, string folderId)
		{
			return new LocalizedString("ErrorGetRestrictionTableForFolderFailed", "", false, false, Strings.ResourceManager, new object[]
			{
				databaseId,
				folderId
			});
		}

		public static LocalizedString ErrorUnableToGetUnifiedGroup
		{
			get
			{
				return new LocalizedString("ErrorUnableToGetUnifiedGroup", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WarningUnableToAddMembers(string members)
		{
			return new LocalizedString("WarningUnableToAddMembers", "", false, false, Strings.ResourceManager, new object[]
			{
				members
			});
		}

		public static LocalizedString WorkItemNotFoundException(string workitemId)
		{
			return new LocalizedString("WorkItemNotFoundException", "", false, false, Strings.ResourceManager, new object[]
			{
				workitemId
			});
		}

		public static LocalizedString CalendarValidationTask
		{
			get
			{
				return new LocalizedString("CalendarValidationTask", "Ex22AA7E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUnableToSessionWithAAD
		{
			get
			{
				return new LocalizedString("ErrorUnableToSessionWithAAD", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSGOwningServerNotFound(string idStringValue)
		{
			return new LocalizedString("ErrorSGOwningServerNotFound", "ExA888DC", false, true, Strings.ResourceManager, new object[]
			{
				idStringValue
			});
		}

		public static LocalizedString ErrorUnableToRemove(string id)
		{
			return new LocalizedString("ErrorUnableToRemove", "", false, false, Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString WarningUnableToUpdateExchangeResources
		{
			get
			{
				return new LocalizedString("WarningUnableToUpdateExchangeResources", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorStorageGroupNotUnique(string idStringValue)
		{
			return new LocalizedString("ErrorStorageGroupNotUnique", "ExA9CCAF", false, true, Strings.ResourceManager, new object[]
			{
				idStringValue
			});
		}

		public static LocalizedString ErrorUnableToUpdateUnifiedGroup
		{
			get
			{
				return new LocalizedString("ErrorUnableToUpdateUnifiedGroup", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUnableToCreateUnifiedGroup
		{
			get
			{
				return new LocalizedString("ErrorUnableToCreateUnifiedGroup", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDatabaseNotUnique(string idStringValue)
		{
			return new LocalizedString("ErrorDatabaseNotUnique", "Ex99FB63", false, true, Strings.ResourceManager, new object[]
			{
				idStringValue
			});
		}

		public static LocalizedString TestMessageDefaultSubject
		{
			get
			{
				return new LocalizedString("TestMessageDefaultSubject", "Ex89393F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnableToDiscoverDefaultDomain
		{
			get
			{
				return new LocalizedString("UnableToDiscoverDefaultDomain", "Ex1EBAE3", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmRepairRemoveProperty(string property, string value, string migrationObject)
		{
			return new LocalizedString("ConfirmRepairRemoveProperty", "", false, false, Strings.ResourceManager, new object[]
			{
				property,
				value,
				migrationObject
			});
		}

		public static LocalizedString MessageFileOrSenderMustBeSpecified
		{
			get
			{
				return new LocalizedString("MessageFileOrSenderMustBeSpecified", "ExDC8C6A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmRepairResumeIMAPSubscription(string user)
		{
			return new LocalizedString("ConfirmRepairResumeIMAPSubscription", "", false, false, Strings.ResourceManager, new object[]
			{
				user
			});
		}

		public static LocalizedString ErrorCannotUpdateExternalDirectoryObjectId(string id, string objectId)
		{
			return new LocalizedString("ErrorCannotUpdateExternalDirectoryObjectId", "", false, false, Strings.ResourceManager, new object[]
			{
				id,
				objectId
			});
		}

		public static LocalizedString WarningUnableToAddOwners(string owners)
		{
			return new LocalizedString("WarningUnableToAddOwners", "", false, false, Strings.ResourceManager, new object[]
			{
				owners
			});
		}

		public static LocalizedString NoHubsAvailable
		{
			get
			{
				return new LocalizedString("NoHubsAvailable", "ExC4054F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorServerNotFound(string idStringValue)
		{
			return new LocalizedString("ErrorServerNotFound", "ExAA7402", false, true, Strings.ResourceManager, new object[]
			{
				idStringValue
			});
		}

		public static LocalizedString SetUpgradeWorkItemConfirmationMessage(string workitemId, string modifiedProperties)
		{
			return new LocalizedString("SetUpgradeWorkItemConfirmationMessage", "", false, false, Strings.ResourceManager, new object[]
			{
				workitemId,
				modifiedProperties
			});
		}

		public static LocalizedString ErrorInvalidObjectMissingCriticalProperty(string type, string identity, string property)
		{
			return new LocalizedString("ErrorInvalidObjectMissingCriticalProperty", "ExFD016D", false, true, Strings.ResourceManager, new object[]
			{
				type,
				identity,
				property
			});
		}

		public static LocalizedString ExchangeSupportPSSnapInDescription
		{
			get
			{
				return new LocalizedString("ExchangeSupportPSSnapInDescription", "Ex3C4848", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpnRegistrationSucceeded
		{
			get
			{
				return new LocalizedString("SpnRegistrationSucceeded", "ExEE0658", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotReadDatabaseEvents(string databaseId)
		{
			return new LocalizedString("ErrorCannotReadDatabaseEvents", "Ex2BDE89", false, true, Strings.ResourceManager, new object[]
			{
				databaseId
			});
		}

		public static LocalizedString WarningUnableToRemoveOwners(string owners)
		{
			return new LocalizedString("WarningUnableToRemoveOwners", "", false, false, Strings.ResourceManager, new object[]
			{
				owners
			});
		}

		public static LocalizedString ErrorMissingWebDnsInformation
		{
			get
			{
				return new LocalizedString("ErrorMissingWebDnsInformation", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReservedString2
		{
			get
			{
				return new LocalizedString("ReservedString2", "Ex8B43F6", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ValidateRepairUpdateStatus
		{
			get
			{
				return new LocalizedString("ValidateRepairUpdateStatus", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorStorageGroupNotFound(string idStringValue)
		{
			return new LocalizedString("ErrorStorageGroupNotFound", "ExE6BB2D", false, true, Strings.ResourceManager, new object[]
			{
				idStringValue
			});
		}

		public static LocalizedString ErrorServerNotUnique(string idStringValue)
		{
			return new LocalizedString("ErrorServerNotUnique", "Ex9943D0", false, true, Strings.ResourceManager, new object[]
			{
				idStringValue
			});
		}

		public static LocalizedString TryingToSubmitTestmessage(string serverName)
		{
			return new LocalizedString("TryingToSubmitTestmessage", "ExF32415", false, true, Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString ValidateRepairMissingReport(string name)
		{
			return new LocalizedString("ValidateRepairMissingReport", "", false, false, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ValidateRepairMissingSubscription(string name, string error)
		{
			return new LocalizedString("ValidateRepairMissingSubscription", "", false, false, Strings.ResourceManager, new object[]
			{
				name,
				error
			});
		}

		public static LocalizedString ConfirmRepairUser(string action, string user)
		{
			return new LocalizedString("ConfirmRepairUser", "", false, false, Strings.ResourceManager, new object[]
			{
				action,
				user
			});
		}

		public static LocalizedString InvalidTenantGuidError(string id)
		{
			return new LocalizedString("InvalidTenantGuidError", "", false, false, Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString ErrorMaxThreadPoolThreads(int maxTPoolThreads)
		{
			return new LocalizedString("ErrorMaxThreadPoolThreads", "ExD494AC", false, true, Strings.ResourceManager, new object[]
			{
				maxTPoolThreads
			});
		}

		public static LocalizedString ConfirmRepairUpdateProperty(string property, string oldValue, string newValue, string migrationObject)
		{
			return new LocalizedString("ConfirmRepairUpdateProperty", "", false, false, Strings.ResourceManager, new object[]
			{
				property,
				oldValue,
				newValue,
				migrationObject
			});
		}

		public static LocalizedString ReservedString4
		{
			get
			{
				return new LocalizedString("ReservedString4", "Ex99A1B0", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmRepairUpdateCacheEntry(string org)
		{
			return new LocalizedString("ConfirmRepairUpdateCacheEntry", "", false, false, Strings.ResourceManager, new object[]
			{
				org
			});
		}

		public static LocalizedString SubmittedSuccessfully(string serverName)
		{
			return new LocalizedString("SubmittedSuccessfully", "ExA14C26", false, true, Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString ConfirmationMessageTestMessage
		{
			get
			{
				return new LocalizedString("ConfirmationMessageTestMessage", "Ex791CD8", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UsingDefaultDomainFromAD(string domain)
		{
			return new LocalizedString("UsingDefaultDomainFromAD", "Ex822608", false, true, Strings.ResourceManager, new object[]
			{
				domain
			});
		}

		public static LocalizedString ConfirmRepairRemoveReport(string report)
		{
			return new LocalizedString("ConfirmRepairRemoveReport", "", false, false, Strings.ResourceManager, new object[]
			{
				report
			});
		}

		public static LocalizedString ConfirmationMessageRepairMigration
		{
			get
			{
				return new LocalizedString("ConfirmationMessageRepairMigration", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnableToCreateFromMsg(string exceptionTest)
		{
			return new LocalizedString("UnableToCreateFromMsg", "Ex2778D7", false, true, Strings.ResourceManager, new object[]
			{
				exceptionTest
			});
		}

		public static LocalizedString ErrorDatabaseNotFound(string idStringValue)
		{
			return new LocalizedString("ErrorDatabaseNotFound", "Ex293997", false, true, Strings.ResourceManager, new object[]
			{
				idStringValue
			});
		}

		public static LocalizedString ConfirmRepairRemoveFolder(string folder)
		{
			return new LocalizedString("ConfirmRepairRemoveFolder", "", false, false, Strings.ResourceManager, new object[]
			{
				folder
			});
		}

		public static LocalizedString ErrorStartDateEqualGreaterThanEndDate(string startDate, string endDate)
		{
			return new LocalizedString("ErrorStartDateEqualGreaterThanEndDate", "Ex2130B8", false, true, Strings.ResourceManager, new object[]
			{
				startDate,
				endDate
			});
		}

		public static LocalizedString ErrorRepairConvertStatus(string name, string type)
		{
			return new LocalizedString("ErrorRepairConvertStatus", "", false, false, Strings.ResourceManager, new object[]
			{
				name,
				type
			});
		}

		public static LocalizedString ValidateRepairMissingSubscriptionHandler(string name)
		{
			return new LocalizedString("ValidateRepairMissingSubscriptionHandler", "", false, false, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ErrorDBOwningServerNotFound(string idStringValue)
		{
			return new LocalizedString("ErrorDBOwningServerNotFound", "Ex5A9B8A", false, true, Strings.ResourceManager, new object[]
			{
				idStringValue
			});
		}

		public static LocalizedString ErrorResultSizeOutOfRange(string min, string max)
		{
			return new LocalizedString("ErrorResultSizeOutOfRange", "Ex9AEA0D", false, true, Strings.ResourceManager, new object[]
			{
				min,
				max
			});
		}

		public static LocalizedString InvalidTestMessageFileData(string mimeError)
		{
			return new LocalizedString("InvalidTestMessageFileData", "ExC952D6", false, true, Strings.ResourceManager, new object[]
			{
				mimeError
			});
		}

		public static LocalizedString ErrorRepairReverting(string name)
		{
			return new LocalizedString("ErrorRepairReverting", "", false, false, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ErrorLocalServerIsNotMailboxServer
		{
			get
			{
				return new LocalizedString("ErrorLocalServerIsNotMailboxServer", "Ex3BF88C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmRepairRemoveUsers(string email)
		{
			return new LocalizedString("ConfirmRepairRemoveUsers", "", false, false, Strings.ResourceManager, new object[]
			{
				email
			});
		}

		public static LocalizedString ConfirmRepairBatch(string action, string job)
		{
			return new LocalizedString("ConfirmRepairBatch", "", false, false, Strings.ResourceManager, new object[]
			{
				action,
				job
			});
		}

		public static LocalizedString ReservedString3
		{
			get
			{
				return new LocalizedString("ReservedString3", "Ex26688F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ValidateRepairInvalidRevertJobType(string name)
		{
			return new LocalizedString("ValidateRepairInvalidRevertJobType", "", false, false, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString NoExternalDirectoryObjectIdForRecipientId(string recipId)
		{
			return new LocalizedString("NoExternalDirectoryObjectIdForRecipientId", "", false, false, Strings.ResourceManager, new object[]
			{
				recipId
			});
		}

		public static LocalizedString MessageFileDataSpecifiedAsNull
		{
			get
			{
				return new LocalizedString("MessageFileDataSpecifiedAsNull", "Ex65970C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReservedString5
		{
			get
			{
				return new LocalizedString("ReservedString5", "Ex090BFC", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmRepairRemoveStoreObject(string id)
		{
			return new LocalizedString("ConfirmRepairRemoveStoreObject", "", false, false, Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString UsingDefaultDomainFromRecipient(string domain)
		{
			return new LocalizedString("UsingDefaultDomainFromRecipient", "Ex60CB42", false, true, Strings.ResourceManager, new object[]
			{
				domain
			});
		}

		public static LocalizedString ErrorCannotReadDatabaseWatermarks(string databaseId)
		{
			return new LocalizedString("ErrorCannotReadDatabaseWatermarks", "ExF17064", false, true, Strings.ResourceManager, new object[]
			{
				databaseId
			});
		}

		public static LocalizedString WarnRepairRemovingUser(string name)
		{
			return new LocalizedString("WarnRepairRemovingUser", "", false, false, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString InvalidStatusDetailError(string uri)
		{
			return new LocalizedString("InvalidStatusDetailError", "", false, false, Strings.ResourceManager, new object[]
			{
				uri
			});
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(33);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Management.Powershell.Support.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			ReservedString1 = 4054043086U,
			ErrorUnableToGetGroupOwners = 2470358009U,
			SenderNotSpecifiedAndNotPresentInMessage = 366868245U,
			ValidateRepairInvalidUser = 2628020095U,
			MimeDoesNotComplyWithStandards = 3952589442U,
			ErrorUnableToGetCreatorFromGroupMailbox = 1272170093U,
			ErrorLocalMachineIsNotExchangeServer = 3855259485U,
			TestMessageDefaultBody = 3353703238U,
			MustSpecifyAtLeastOneSmtpRecipientAddress = 3038652633U,
			ValidateRepairUpdateMissingStatus = 3248953978U,
			WarningUnableToUpdateUserMailboxes = 210240938U,
			ErrorUnableToGetUnifiedGroup = 1157487U,
			CalendarValidationTask = 110985478U,
			ErrorUnableToSessionWithAAD = 84207768U,
			WarningUnableToUpdateExchangeResources = 2906052647U,
			ErrorUnableToUpdateUnifiedGroup = 46749802U,
			ErrorUnableToCreateUnifiedGroup = 891487637U,
			TestMessageDefaultSubject = 3322833458U,
			UnableToDiscoverDefaultDomain = 775190192U,
			MessageFileOrSenderMustBeSpecified = 2995683657U,
			NoHubsAvailable = 2481051232U,
			ExchangeSupportPSSnapInDescription = 2635939130U,
			SpnRegistrationSucceeded = 332607451U,
			ErrorMissingWebDnsInformation = 784901781U,
			ReservedString2 = 4054043087U,
			ValidateRepairUpdateStatus = 3311173118U,
			ReservedString4 = 4054043081U,
			ConfirmationMessageTestMessage = 2589164433U,
			ConfirmationMessageRepairMigration = 1531210305U,
			ErrorLocalServerIsNotMailboxServer = 1622178204U,
			ReservedString3 = 4054043088U,
			MessageFileDataSpecifiedAsNull = 4142544366U,
			ReservedString5 = 4054043082U
		}

		private enum ParamIDs
		{
			WarningUnableToRemoveMembers,
			SpnRegistrationFailed,
			ValidatingCalendar,
			ValidateRepairInvalidRevert,
			ValidateRepairMultipleUsers,
			ErrorMissingServerFqdn,
			ConfirmRepairSubscription,
			ErrorNoLocalOrganizationMailbox,
			ErrorGetRestrictionTableForFolderFailed,
			WarningUnableToAddMembers,
			WorkItemNotFoundException,
			ErrorSGOwningServerNotFound,
			ErrorUnableToRemove,
			ErrorStorageGroupNotUnique,
			ErrorDatabaseNotUnique,
			ConfirmRepairRemoveProperty,
			ConfirmRepairResumeIMAPSubscription,
			ErrorCannotUpdateExternalDirectoryObjectId,
			WarningUnableToAddOwners,
			ErrorServerNotFound,
			SetUpgradeWorkItemConfirmationMessage,
			ErrorInvalidObjectMissingCriticalProperty,
			ErrorCannotReadDatabaseEvents,
			WarningUnableToRemoveOwners,
			ErrorStorageGroupNotFound,
			ErrorServerNotUnique,
			TryingToSubmitTestmessage,
			ValidateRepairMissingReport,
			ValidateRepairMissingSubscription,
			ConfirmRepairUser,
			InvalidTenantGuidError,
			ErrorMaxThreadPoolThreads,
			ConfirmRepairUpdateProperty,
			ConfirmRepairUpdateCacheEntry,
			SubmittedSuccessfully,
			UsingDefaultDomainFromAD,
			ConfirmRepairRemoveReport,
			UnableToCreateFromMsg,
			ErrorDatabaseNotFound,
			ConfirmRepairRemoveFolder,
			ErrorStartDateEqualGreaterThanEndDate,
			ErrorRepairConvertStatus,
			ValidateRepairMissingSubscriptionHandler,
			ErrorDBOwningServerNotFound,
			ErrorResultSizeOutOfRange,
			InvalidTestMessageFileData,
			ErrorRepairReverting,
			ConfirmRepairRemoveUsers,
			ConfirmRepairBatch,
			ValidateRepairInvalidRevertJobType,
			NoExternalDirectoryObjectIdForRecipientId,
			ConfirmRepairRemoveStoreObject,
			UsingDefaultDomainFromRecipient,
			ErrorCannotReadDatabaseWatermarks,
			WarnRepairRemovingUser,
			InvalidStatusDetailError
		}
	}
}
