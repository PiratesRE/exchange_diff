using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal class GlobalSyncStateInfo : CustomSyncStateInfo
	{
		public override string UniqueName
		{
			get
			{
				return "Policy";
			}
			set
			{
				throw new InvalidOperationException("GlobalSyncStateInfo.UniqueName is not settable.");
			}
		}

		public override int Version
		{
			get
			{
				return 38;
			}
		}

		public override void HandleSyncStateVersioning(SyncState syncState)
		{
			if (syncState == null)
			{
				throw new ArgumentNullException("syncState");
			}
			if (syncState.BackendVersion == null)
			{
				return;
			}
			bool flag = true;
			if (syncState.BackendVersion < 2 || syncState.BackendVersion > this.Version)
			{
				flag = false;
			}
			else if (syncState.BackendVersion.Value != this.Version)
			{
				string text = null;
				switch (syncState.BackendVersion.Value)
				{
				case 2:
					syncState["WipeConfirmationAddresses"] = null;
					break;
				case 3:
					break;
				case 4:
					goto IL_14F;
				case 5:
				case 6:
				case 7:
				case 8:
				case 9:
				case 10:
				case 11:
				case 12:
				case 13:
				case 14:
				case 15:
				case 16:
				case 17:
				case 18:
				case 19:
					goto IL_3B6;
				case 20:
					goto IL_16C;
				case 21:
					goto IL_184;
				case 22:
					goto IL_1A6;
				case 23:
					goto IL_1D2;
				case 24:
					goto IL_1EF;
				case 25:
					goto IL_1FA;
				case 26:
					goto IL_212;
				case 27:
					goto IL_259;
				case 28:
					goto IL_2D8;
				case 29:
					goto IL_2E4;
				case 30:
					goto IL_2EF;
				case 31:
					goto IL_2FB;
				case 32:
					goto IL_306;
				case 33:
					goto IL_312;
				case 34:
					goto IL_31E;
				case 35:
					goto IL_32A;
				case 36:
					goto IL_358;
				case 37:
					goto IL_39E;
				default:
					goto IL_3B6;
				}
				syncState[CustomStateDatumType.UserAgent] = null;
				IL_14F:
				syncState["LastAdUpdateTime"] = null;
				syncState["DeviceHealth"] = new Int32Data(0);
				IL_16C:
				text = syncState.GetData<StringData, string>("LastPolicyXML", null);
				syncState.Remove("LastPolicyXML");
				IL_184:
				syncState["ProvisionSupported"] = new BooleanData(syncState.GetData<UInt32Data, uint>("PolicyKeyOnDevice", 0U) != 0U);
				IL_1A6:
				syncState["LastPolicyXMLHash"] = ((text == null) ? null : new NullableData<Int32Data, int>(new int?(PolicyData.GetPolicyHashCode(text + true))));
				IL_1D2:
				syncState["DeviceEnableOutboundSMS"] = new BooleanData(false);
				syncState["DeviceMobileOperator"] = null;
				IL_1EF:
				syncState.Remove("LastAdUpdateTime");
				IL_1FA:
				syncState["ClientAlternateMailboxInformationVersion"] = null;
				syncState["DeviceUMRegisteredPhoneNumber"] = null;
				IL_212:
				syncState["HaveSentBoostrapMailForWM61"] = new BooleanData(false);
				if (syncState.BackendVersion.Value < 20)
				{
					syncState["SSUpgradeDateTime"] = new DateTimeData(ExDateTime.UtcNow);
				}
				else
				{
					syncState["SSUpgradeDateTime"] = null;
				}
				IL_259:
				syncState.Remove("DeviceHealth");
				syncState["DeviceAccessState"] = new Int32Data(0);
				syncState["DeviceAccessStateReason"] = new Int32Data(0);
				syncState["DevicePolicyApplied"] = null;
				syncState["DevicePolicyApplicationStatus"] = new Int32Data(0);
				syncState["LastDeviceWipeRequestor"] = null;
				syncState["DeviceActiveSyncVersion"] = null;
				syncState["ADDeviceInfoHash"] = null;
				syncState["DeviceInformationReceived"] = new BooleanData(false);
				IL_2D8:
				syncState["ADCreationTime"] = null;
				IL_2E4:
				syncState.Remove("DeviceUMRegisteredPhoneNumber");
				IL_2EF:
				syncState["NextTimeToClearMailboxLogs"] = null;
				IL_2FB:
				syncState.Remove("ClientAlternateMailboxInformationVersion");
				IL_306:
				syncState["DeviceADObjectId"] = null;
				IL_312:
				syncState["BootstrapMailForWM61TriggeredTime"] = null;
				IL_31E:
				syncState["UserADObjectId"] = null;
				IL_32A:
				syncState["ABQMailId"] = null;
				syncState["ABQMailState"] = new Int32Data(0);
				syncState["DeviceInformationPromoted"] = new BooleanData(false);
				IL_358:
				syncState["DevicePhoneNumberForSms"] = ((syncState["DevicePhoneNumber"] != null) ? new StringData(((StringData)syncState["DevicePhoneNumber"]).Data) : null);
				syncState["SmsSearchFolderCreated"] = new BooleanData(false);
				IL_39E:
				syncState["DeviceBehavior"] = new DeviceBehaviorData(new DeviceBehavior(true));
				goto IL_3B8;
				IL_3B6:
				flag = false;
			}
			IL_3B8:
			if (!flag)
			{
				syncState.HandleCorruptSyncState();
			}
		}

		internal const string UniqueNameString = "Policy";

		internal const int CurrentVersion = 38;

		internal const int E14BaseVersion = 20;

		internal struct PropertyNames
		{
			internal const string DeviceFriendlyName = "DeviceFriendlyName";

			internal const string DeviceImei = "DeviceIMEI";

			internal const string DeviceModel = "DeviceModel";

			internal const string DeviceOS = "DeviceOS";

			internal const string DeviceOSLanguage = "DeviceOSLanguage";

			internal const string DevicePhoneNumber = "DevicePhoneNumber";

			internal const string DeviceUserAgent = "DeviceUserAgent";

			internal const string LastPolicyTime = "LastPolicyTime";

			internal const string LastPolicyXML = "LastPolicyXML";

			internal const string LastPolicyXMLHash = "LastPolicyXMLHash";

			internal const string NextTimeToClearMailboxLogs = "NextTimeToClearMailboxLogs";

			internal const string PolicyKeyNeeded = "PolicyKeyNeeded";

			internal const string PolicyKeyOnDevice = "PolicyKeyOnDevice";

			internal const string PolicyKeyWaitingAck = "PolicyKeyWaitingAck";

			internal const string RecoveryPassword = "RecoveryPassword";

			internal const string WipeAckTime = "WipeAckTime";

			internal const string WipeConfirmationAddresses = "WipeConfirmationAddresses";

			internal const string WipeRequestTime = "WipeRequestTime";

			internal const string WipeSendTime = "WipeSentTime";

			internal const string DeviceHealth = "DeviceHealth";

			internal const string ProvisionSupported = "ProvisionSupported";

			internal const string DeviceEnableOutboundSMS = "DeviceEnableOutboundSMS";

			internal const string DeviceMobileOperator = "DeviceMobileOperator";

			internal const string SSUpgradeDateTime = "SSUpgradeDateTime";

			internal const string HaveSentBoostrapMailForWM61 = "HaveSentBoostrapMailForWM61";

			internal const string BootstrapMailForWM61TriggeredTime = "BootstrapMailForWM61TriggeredTime";

			internal const string DeviceAccessState = "DeviceAccessState";

			internal const string DeviceAccessStateReason = "DeviceAccessStateReason";

			internal const string DeviceAccessControlRule = "DeviceAccessControlRule";

			internal const string DevicePolicyApplied = "DevicePolicyApplied";

			internal const string DevicePolicyApplicationStatus = "DevicePolicyApplicationStatus";

			internal const string LastDeviceWipeRequestor = "LastDeviceWipeRequestor";

			internal const string DeviceActiveSyncVersion = "DeviceActiveSyncVersion";

			internal const string ADDeviceInfoHash = "ADDeviceInfoHash";

			internal const string DeviceInformationReceived = "DeviceInformationReceived";

			internal const string ADCreationTime = "ADCreationTime";

			internal const string DeviceADObjectId = "DeviceADObjectId";

			internal const string UserADObjectId = "UserADObjectId";

			internal const string ABQMailId = "ABQMailId";

			internal const string ABQMailState = "ABQMailState";

			internal const string DeviceInformationPromoted = "DeviceInformationPromoted";

			internal const string DevicePhoneNumberForSms = "DevicePhoneNumberForSms";

			internal const string SmsSearchFolderCreated = "SmsSearchFolderCreated";

			internal const string DeviceBehavior = "DeviceBehavior";
		}
	}
}
