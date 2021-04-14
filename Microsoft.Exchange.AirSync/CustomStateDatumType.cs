using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal struct CustomStateDatumType
	{
		internal static string SupportedTags
		{
			get
			{
				return CustomStateDatumType.GetStaticString("SupportedTags");
			}
		}

		internal static string IdMapping
		{
			get
			{
				return CustomStateDatumType.GetStaticString("IdMapping");
			}
		}

		internal static string SyncKey
		{
			get
			{
				return CustomStateDatumType.GetStaticString("SyncKey");
			}
		}

		internal static string RecoverySyncKey
		{
			get
			{
				return CustomStateDatumType.GetStaticString("RecoverySyncKey");
			}
		}

		internal static string FilterType
		{
			get
			{
				return CustomStateDatumType.GetStaticString("FilterType");
			}
		}

		internal static string MaxItems
		{
			get
			{
				return CustomStateDatumType.GetStaticString("MaxItems");
			}
		}

		internal static string ConversationMode
		{
			get
			{
				return CustomStateDatumType.GetStaticString("ConversationMode");
			}
		}

		internal static string CalendarSyncState
		{
			get
			{
				return CustomStateDatumType.GetStaticString("CalendarSyncState");
			}
		}

		internal static string RecoveryCalendarSyncState
		{
			get
			{
				return CustomStateDatumType.GetStaticString("RecoveryCalendarSyncState");
			}
		}

		internal static string CalendarMasterItems
		{
			get
			{
				return CustomStateDatumType.GetStaticString("CalendarMasterItems");
			}
		}

		internal static string CustomCalendarSyncFilter
		{
			get
			{
				return CustomStateDatumType.GetStaticString("CustomCalendarSyncFilter");
			}
		}

		internal static string AirSyncProtocolVersion
		{
			get
			{
				return CustomStateDatumType.GetStaticString("AirSyncProtocolVersion");
			}
		}

		internal static string AirSyncClassType
		{
			get
			{
				return CustomStateDatumType.GetStaticString("AirSyncClassType");
			}
		}

		internal static string CachedOptionsNode
		{
			get
			{
				return CustomStateDatumType.GetStaticString("CachedOptionsNode");
			}
		}

		internal static string WipeRequestTime
		{
			get
			{
				return CustomStateDatumType.GetStaticString("WipeRequestTime");
			}
		}

		internal static string WipeSendTime
		{
			get
			{
				return CustomStateDatumType.GetStaticString("WipeSentTime");
			}
		}

		internal static string WipeAckTime
		{
			get
			{
				return CustomStateDatumType.GetStaticString("WipeAckTime");
			}
		}

		internal static string WipeConfirmationAddresses
		{
			get
			{
				return CustomStateDatumType.GetStaticString("WipeConfirmationAddresses");
			}
		}

		internal static string LastSyncAttemptTime
		{
			get
			{
				return CustomStateDatumType.GetStaticString("LastSyncAttemptTime");
			}
		}

		internal static string LastSyncSuccessTime
		{
			get
			{
				return CustomStateDatumType.GetStaticString("LastSyncSuccessTime");
			}
		}

		internal static string UserAgent
		{
			get
			{
				return CustomStateDatumType.GetStaticString("UserAgent");
			}
		}

		internal static string LastPingHeartbeat
		{
			get
			{
				return CustomStateDatumType.GetStaticString("LastPingHeartbeat");
			}
		}

		internal static string DeviceModel
		{
			get
			{
				return CustomStateDatumType.GetStaticString("DeviceModel");
			}
		}

		internal static string DeviceImei
		{
			get
			{
				return CustomStateDatumType.GetStaticString("DeviceIMEI");
			}
		}

		internal static string DeviceFriendlyName
		{
			get
			{
				return CustomStateDatumType.GetStaticString("DeviceFriendlyName");
			}
		}

		internal static string DeviceOS
		{
			get
			{
				return CustomStateDatumType.GetStaticString("DeviceOS");
			}
		}

		internal static string DeviceOSLanguage
		{
			get
			{
				return CustomStateDatumType.GetStaticString("DeviceOSLanguage");
			}
		}

		internal static string DevicePhoneNumber
		{
			get
			{
				return CustomStateDatumType.GetStaticString("DevicePhoneNumber");
			}
		}

		internal static string DeviceEnableOutboundSMS
		{
			get
			{
				return CustomStateDatumType.GetStaticString("DeviceEnableOutboundSMS");
			}
		}

		internal static string DeviceMobileOperator
		{
			get
			{
				return CustomStateDatumType.GetStaticString("DeviceMobileOperator");
			}
		}

		internal static string DPFolderList
		{
			get
			{
				return CustomStateDatumType.GetStaticString("DPFolderList");
			}
		}

		internal static string LastPolicyXMLHash
		{
			get
			{
				return CustomStateDatumType.GetStaticString("LastPolicyXMLHash");
			}
		}

		internal static string LastPolicyTime
		{
			get
			{
				return CustomStateDatumType.GetStaticString("LastPolicyTime");
			}
		}

		internal static string NextTimeToClearMailboxLogs
		{
			get
			{
				return CustomStateDatumType.GetStaticString("NextTimeToClearMailboxLogs");
			}
		}

		internal static string PolicyKeyNeeded
		{
			get
			{
				return CustomStateDatumType.GetStaticString("PolicyKeyNeeded");
			}
		}

		internal static string PolicyKeyWaitingAck
		{
			get
			{
				return CustomStateDatumType.GetStaticString("PolicyKeyWaitingAck");
			}
		}

		internal static string PolicyKeyOnDevice
		{
			get
			{
				return CustomStateDatumType.GetStaticString("PolicyKeyOnDevice");
			}
		}

		internal static string RecoveryPassword
		{
			get
			{
				return CustomStateDatumType.GetStaticString("RecoveryPassword");
			}
		}

		internal static string LastCachableWbxmlDocument
		{
			get
			{
				return CustomStateDatumType.GetStaticString("LastCachableWbxmlDocument");
			}
		}

		internal static string ClientCanSendUpEmptyRequests
		{
			get
			{
				return CustomStateDatumType.GetStaticString("ClientCanSendUpEmptyRequests");
			}
		}

		internal static string LastSyncRequestRandomNumber
		{
			get
			{
				return CustomStateDatumType.GetStaticString("LastSyncRequestRandomString");
			}
		}

		internal static string LastClientIdsSent
		{
			get
			{
				return CustomStateDatumType.GetStaticString("LastClientIdsSent");
			}
		}

		internal static string ProvisionSupported
		{
			get
			{
				return CustomStateDatumType.GetStaticString("ProvisionSupported");
			}
		}

		internal static string Permissions
		{
			get
			{
				return CustomStateDatumType.GetStaticString("Permissions");
			}
		}

		internal static string FullFolderTree
		{
			get
			{
				return CustomStateDatumType.GetStaticString("FullFolderTree");
			}
		}

		internal static string RecoveryFullFolderTree
		{
			get
			{
				return CustomStateDatumType.GetStaticString("RecoveryFullFolderTree");
			}
		}

		internal static string ClientCategoryList
		{
			get
			{
				return CustomStateDatumType.GetStaticString("ClientCategoryList");
			}
		}

		internal static string SSUpgradeDateTime
		{
			get
			{
				return CustomStateDatumType.GetStaticString("SSUpgradeDateTime");
			}
		}

		internal static string HaveSentBoostrapMailForWM61
		{
			get
			{
				return CustomStateDatumType.GetStaticString("HaveSentBoostrapMailForWM61");
			}
		}

		internal static string BootstrapMailForWM61TriggeredTime
		{
			get
			{
				return CustomStateDatumType.GetStaticString("BootstrapMailForWM61TriggeredTime");
			}
		}

		internal static string DeviceAccessState
		{
			get
			{
				return CustomStateDatumType.GetStaticString("DeviceAccessState");
			}
		}

		internal static string DeviceAccessStateReason
		{
			get
			{
				return CustomStateDatumType.GetStaticString("DeviceAccessStateReason");
			}
		}

		internal static string DeviceAccessControlRule
		{
			get
			{
				return CustomStateDatumType.GetStaticString("DeviceAccessControlRule");
			}
		}

		internal static string DevicePolicyApplied
		{
			get
			{
				return CustomStateDatumType.GetStaticString("DevicePolicyApplied");
			}
		}

		internal static string DevicePolicyApplicationStatus
		{
			get
			{
				return CustomStateDatumType.GetStaticString("DevicePolicyApplicationStatus");
			}
		}

		internal static string LastDeviceWipeRequestor
		{
			get
			{
				return CustomStateDatumType.GetStaticString("LastDeviceWipeRequestor");
			}
		}

		internal static string DeviceActiveSyncVersion
		{
			get
			{
				return CustomStateDatumType.GetStaticString("DeviceActiveSyncVersion");
			}
		}

		internal static string ADDeviceInfoHash
		{
			get
			{
				return CustomStateDatumType.GetStaticString("ADDeviceInfoHash");
			}
		}

		internal static string DeviceInformationReceived
		{
			get
			{
				return CustomStateDatumType.GetStaticString("DeviceInformationReceived");
			}
		}

		internal static string ADCreationTime
		{
			get
			{
				return CustomStateDatumType.GetStaticString("ADCreationTime");
			}
		}

		internal static string DeviceADObjectId
		{
			get
			{
				return CustomStateDatumType.GetStaticString("DeviceADObjectId");
			}
		}

		internal static string UserADObjectId
		{
			get
			{
				return CustomStateDatumType.GetStaticString("UserADObjectId");
			}
		}

		internal static string ABQMailId
		{
			get
			{
				return CustomStateDatumType.GetStaticString("ABQMailId");
			}
		}

		internal static string ABQMailState
		{
			get
			{
				return CustomStateDatumType.GetStaticString("ABQMailState");
			}
		}

		internal static string DeviceInformationPromoted
		{
			get
			{
				return CustomStateDatumType.GetStaticString("DeviceInformationPromoted");
			}
		}

		internal static string DevicePhoneNumberForSms
		{
			get
			{
				return CustomStateDatumType.GetStaticString("DevicePhoneNumberForSms");
			}
		}

		internal static string SmsSearchFolderCreated
		{
			get
			{
				return CustomStateDatumType.GetStaticString("SmsSearchFolderCreated");
			}
		}

		internal static string LastMaxDevicesExceededMailSentTime
		{
			get
			{
				return CustomStateDatumType.GetStaticString("LastMaxDevicesExceededMailSentTime");
			}
		}

		internal static string DeviceBehavior
		{
			get
			{
				return CustomStateDatumType.GetStaticString("DeviceBehavior");
			}
		}

		internal static string MeetingOrganizerInfo
		{
			get
			{
				return CustomStateDatumType.GetStaticString("MeetingOrganizerInfo");
			}
		}

		private static void Initialize()
		{
			lock (CustomStateDatumType.syncRoot)
			{
				if (!CustomStateDatumType.initialized)
				{
					StaticStringPool.Instance.Intern("SupportedTags");
					StaticStringPool.Instance.Intern("IdMapping");
					StaticStringPool.Instance.Intern("SyncKey");
					StaticStringPool.Instance.Intern("RecoverySyncKey");
					StaticStringPool.Instance.Intern("FilterType");
					StaticStringPool.Instance.Intern("MaxItems");
					StaticStringPool.Instance.Intern("ConversationMode");
					StaticStringPool.Instance.Intern("CustomCalendarSyncFilter");
					StaticStringPool.Instance.Intern("AirSyncProtocolVersion");
					StaticStringPool.Instance.Intern("AirSyncClassType");
					StaticStringPool.Instance.Intern("CachedOptionsNode");
					StaticStringPool.Instance.Intern("Permissions");
					StaticStringPool.Instance.Intern("WipeRequestTime");
					StaticStringPool.Instance.Intern("WipeSentTime");
					StaticStringPool.Instance.Intern("WipeAckTime");
					StaticStringPool.Instance.Intern("WipeConfirmationAddresses");
					StaticStringPool.Instance.Intern("LastSyncAttemptTime");
					StaticStringPool.Instance.Intern("LastSyncSuccessTime");
					StaticStringPool.Instance.Intern("UserAgent");
					StaticStringPool.Instance.Intern("LastPingHeartbeat");
					StaticStringPool.Instance.Intern("DeviceModel");
					StaticStringPool.Instance.Intern("DeviceIMEI");
					StaticStringPool.Instance.Intern("DeviceFriendlyName");
					StaticStringPool.Instance.Intern("DeviceOS");
					StaticStringPool.Instance.Intern("DeviceOSLanguage");
					StaticStringPool.Instance.Intern("DevicePhoneNumber");
					StaticStringPool.Instance.Intern("DPFolderList");
					StaticStringPool.Instance.Intern("LastPolicyXMLHash");
					StaticStringPool.Instance.Intern("LastPolicyTime");
					StaticStringPool.Instance.Intern("NextTimeToClearMailboxLogs");
					StaticStringPool.Instance.Intern("PolicyKeyNeeded");
					StaticStringPool.Instance.Intern("PolicyKeyWaitingAck");
					StaticStringPool.Instance.Intern("PolicyKeyOnDevice");
					StaticStringPool.Instance.Intern("RecoveryPassword");
					StaticStringPool.Instance.Intern("LastCachableWbxmlDocument");
					StaticStringPool.Instance.Intern("ClientCanSendUpEmptyRequests");
					StaticStringPool.Instance.Intern("LastSyncRequestRandomString");
					StaticStringPool.Instance.Intern("LastClientIdsSent");
					StaticStringPool.Instance.Intern("ProvisionSupported");
					StaticStringPool.Instance.Intern("DeviceEnableOutboundSMS");
					StaticStringPool.Instance.Intern("DeviceMobileOperator");
					StaticStringPool.Instance.Intern("FullFolderTree");
					StaticStringPool.Instance.Intern("RecoveryFullFolderTree");
					StaticStringPool.Instance.Intern("ClientCategoryList");
					StaticStringPool.Instance.Intern("SSUpgradeDateTime");
					StaticStringPool.Instance.Intern("HaveSentBoostrapMailForWM61");
					StaticStringPool.Instance.Intern("BootstrapMailForWM61TriggeredTime");
					StaticStringPool.Instance.Intern("DeviceAccessState");
					StaticStringPool.Instance.Intern("DeviceAccessStateReason");
					StaticStringPool.Instance.Intern("DevicePolicyApplied");
					StaticStringPool.Instance.Intern("DevicePolicyApplicationStatus");
					StaticStringPool.Instance.Intern("DeviceAccessControlRule");
					StaticStringPool.Instance.Intern("LastDeviceWipeRequestor");
					StaticStringPool.Instance.Intern("DeviceActiveSyncVersion");
					StaticStringPool.Instance.Intern("ADDeviceInfoHash");
					StaticStringPool.Instance.Intern("DeviceInformationReceived");
					StaticStringPool.Instance.Intern("ADCreationTime");
					StaticStringPool.Instance.Intern("DeviceADObjectId");
					StaticStringPool.Instance.Intern("UserADObjectId");
					StaticStringPool.Instance.Intern("ABQMailId");
					StaticStringPool.Instance.Intern("ABQMailState");
					StaticStringPool.Instance.Intern("DeviceInformationPromoted");
					StaticStringPool.Instance.Intern("DevicePhoneNumberForSms");
					StaticStringPool.Instance.Intern("SmsSearchFolderCreated");
					StaticStringPool.Instance.Intern("LastMaxDevicesExceededMailSentTime");
					StaticStringPool.Instance.Intern("DeviceBehavior");
					StaticStringPool.Instance.Intern("MeetingOrganizerInfo");
					StaticStringPool.Instance.Intern("CalendarSyncState");
					StaticStringPool.Instance.Intern("RecoveryCalendarSyncState");
					CustomStateDatumType.initialized = true;
				}
			}
		}

		private static string GetStaticString(string key)
		{
			if (!CustomStateDatumType.initialized)
			{
				CustomStateDatumType.Initialize();
			}
			return key;
		}

		private static object syncRoot = new object();

		private static bool initialized;
	}
}
