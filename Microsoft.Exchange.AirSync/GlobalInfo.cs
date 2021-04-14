using System;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal class GlobalInfo : SyncStateDataInfo, IGlobalInfo
	{
		public GlobalInfo(CustomSyncState wrappedSyncState) : base(wrappedSyncState)
		{
		}

		public int? LastPolicyXMLHash
		{
			get
			{
				return base.Fetch<NullableData<Int32Data, int>, int?>(CustomStateDatumType.LastPolicyXMLHash, null);
			}
			set
			{
				base.Assign<NullableData<Int32Data, int>, int?>(CustomStateDatumType.LastPolicyXMLHash, value);
			}
		}

		public bool HasNewSyncData { get; private set; }

		public bool HasNewAutdData { get; private set; }

		public ExDateTime? NextTimeToClearMailboxLogs
		{
			get
			{
				return base.FetchDateTime(CustomStateDatumType.NextTimeToClearMailboxLogs);
			}
			set
			{
				base.Assign<NullableData<DateTimeData, ExDateTime>, ExDateTime?>(CustomStateDatumType.NextTimeToClearMailboxLogs, value);
			}
		}

		public uint PolicyKeyNeeded
		{
			get
			{
				return base.Fetch<UInt32Data, uint>(CustomStateDatumType.PolicyKeyNeeded, 0U);
			}
			set
			{
				base.Assign<UInt32Data, uint>(CustomStateDatumType.PolicyKeyNeeded, value);
			}
		}

		public uint PolicyKeyWaitingAck
		{
			get
			{
				return base.Fetch<UInt32Data, uint>(CustomStateDatumType.PolicyKeyWaitingAck, 0U);
			}
			set
			{
				base.Assign<UInt32Data, uint>(CustomStateDatumType.PolicyKeyWaitingAck, value);
			}
		}

		public uint PolicyKeyOnDevice
		{
			get
			{
				return base.Fetch<UInt32Data, uint>(CustomStateDatumType.PolicyKeyOnDevice, 0U);
			}
			set
			{
				base.Assign<UInt32Data, uint>(CustomStateDatumType.PolicyKeyOnDevice, value);
			}
		}

		public bool ProvisionSupported
		{
			get
			{
				return base.Fetch<BooleanData, bool>(CustomStateDatumType.ProvisionSupported, false);
			}
			set
			{
				base.Assign<BooleanData, bool>(CustomStateDatumType.ProvisionSupported, value);
			}
		}

		public ExDateTime? LastPolicyTime
		{
			get
			{
				return base.FetchDateTime(CustomStateDatumType.LastPolicyTime);
			}
			set
			{
				base.Assign<NullableData<DateTimeData, ExDateTime>, ExDateTime?>(CustomStateDatumType.LastPolicyTime, value);
			}
		}

		public int[] ClientCategoryHashList
		{
			get
			{
				int[] result;
				if (base.TryGetProperty<int[]>(AirSyncStateSchema.ClientCategoryList, out result))
				{
					return result;
				}
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "[GlobalInfo.ClientCategoryHashList] Failed to get data from GlobalInfo.  Returning null.");
				return null;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					base.DeleteProperty(AirSyncStateSchema.ClientCategoryList);
				}
				else
				{
					base.SetProperty(AirSyncStateSchema.ClientCategoryList, value);
				}
				base.IsDirty = true;
			}
		}

		public string[] LastClientIdsSeen
		{
			get
			{
				string[] result;
				if (base.TryGetProperty<string[]>(AirSyncStateSchema.LastSeenClientIds, out result))
				{
					return result;
				}
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "[GlobalInfo.LastClientIdsSeen] Failed to get data from GlobalInfo.  Returning null.");
				return null;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					base.DeleteProperty(AirSyncStateSchema.LastSeenClientIds);
				}
				else
				{
					base.SetProperty(AirSyncStateSchema.LastSeenClientIds, value);
				}
				base.IsDirty = true;
			}
		}

		public string LastSyncUserAgent
		{
			get
			{
				string result;
				if (base.TryGetProperty<string>(AirSyncStateSchema.LastSyncUserAgent, out result))
				{
					return result;
				}
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "[GlobalInfo.LastSyncUserAgent] Failed to get user agent from GlobalInfo.  Returning null.");
				return null;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DeleteProperty(AirSyncStateSchema.LastSyncUserAgent);
				}
				else
				{
					base.SetProperty(AirSyncStateSchema.LastSyncUserAgent, value);
				}
				base.IsDirty = true;
			}
		}

		public int? LastPingHeartbeat
		{
			get
			{
				int value;
				if (base.TryGetProperty<int>(AirSyncStateSchema.LastPingHeartbeatInterval, out value))
				{
					return new int?(value);
				}
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "[GlobalInfo.LastPingHeartbeatInterval] Failed to get user agent from GlobalInfo.  Returning null.");
				return null;
			}
			set
			{
				if (value == null)
				{
					base.DeleteProperty(AirSyncStateSchema.LastPingHeartbeatInterval);
				}
				else
				{
					base.SetProperty(AirSyncStateSchema.LastPingHeartbeatInterval, value.Value);
				}
				base.IsDirty = true;
			}
		}

		public ExDateTime? LastSyncAttemptTime
		{
			get
			{
				if (this.HasNewSyncData)
				{
					ExDateTime value;
					if (base.TryGetProperty<ExDateTime>(AirSyncStateSchema.LastSyncAttemptTime, out value))
					{
						return new ExDateTime?(value);
					}
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "[GlobalInfo.LastSyncAttemptTime] Tried using new sync data, but failed to get property from bag.  Will look in sync state (old data).");
				}
				return base.FetchDateTime(CustomStateDatumType.LastSyncAttemptTime);
			}
			set
			{
				if (this.WriteNewSyncData)
				{
					AirSyncDiagnostics.TraceDebug<bool>(ExTraceGlobals.RequestsTracer, this, "[GlobalInfo.LastSyncAttemptTime] Using new sync data - saving on first class property.  Has value? {0}", value != null);
					if (value != null)
					{
						base.SetProperty(AirSyncStateSchema.LastSyncAttemptTime, value.Value);
					}
					else
					{
						base.DeleteProperty(AirSyncStateSchema.LastSyncAttemptTime);
					}
					base.IsDirty = true;
					return;
				}
				base.DeleteProperty(AirSyncStateSchema.LastSyncAttemptTime);
				base.Assign<NullableData<DateTimeData, ExDateTime>, ExDateTime?>(CustomStateDatumType.LastSyncAttemptTime, value);
			}
		}

		public ExDateTime? LastSyncSuccessTime
		{
			get
			{
				if (this.HasNewSyncData)
				{
					ExDateTime value;
					if (base.TryGetProperty<ExDateTime>(AirSyncStateSchema.LastSyncSuccessTime, out value))
					{
						return new ExDateTime?(value);
					}
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "[GlobalInfo.LastSyncSuccessTime] Tried using new sync data, but failed to get property from bag.  Will look in sync state (old data).");
				}
				return base.FetchDateTime(CustomStateDatumType.LastSyncSuccessTime);
			}
			set
			{
				if (this.WriteNewSyncData)
				{
					AirSyncDiagnostics.TraceDebug<bool>(ExTraceGlobals.RequestsTracer, this, "[GlobalInfo.LastSyncSuccessTime] Using new sync data - saving on first class property.  Has value? {0}", value != null);
					if (value != null)
					{
						base.SetProperty(AirSyncStateSchema.LastSyncSuccessTime, value.Value);
					}
					else
					{
						base.DeleteProperty(AirSyncStateSchema.LastSyncSuccessTime);
					}
					base.IsDirty = true;
					return;
				}
				base.DeleteProperty(AirSyncStateSchema.LastSyncSuccessTime);
				base.Assign<NullableData<DateTimeData, ExDateTime>, ExDateTime?>(CustomStateDatumType.LastSyncSuccessTime, value);
			}
		}

		public ExDateTime? RemoteWipeRequestedTime
		{
			get
			{
				return base.FetchDateTime(CustomStateDatumType.WipeRequestTime);
			}
			set
			{
				base.Assign<NullableData<DateTimeData, ExDateTime>, ExDateTime?>(CustomStateDatumType.WipeRequestTime, value);
			}
		}

		public ExDateTime? RemoteWipeSentTime
		{
			get
			{
				return base.FetchDateTime(CustomStateDatumType.WipeSendTime);
			}
			set
			{
				base.Assign<NullableData<DateTimeData, ExDateTime>, ExDateTime?>(CustomStateDatumType.WipeSendTime, value);
			}
		}

		public ExDateTime? RemoteWipeAckTime
		{
			get
			{
				return base.FetchDateTime(CustomStateDatumType.WipeAckTime);
			}
			set
			{
				base.Assign<NullableData<DateTimeData, ExDateTime>, ExDateTime?>(CustomStateDatumType.WipeAckTime, value);
			}
		}

		public string DeviceModel
		{
			get
			{
				return base.Fetch<StringData, string>(CustomStateDatumType.DeviceModel, null);
			}
			set
			{
				base.Assign<StringData, string>(CustomStateDatumType.DeviceModel, value);
			}
		}

		public string DeviceImei
		{
			get
			{
				return base.Fetch<StringData, string>(CustomStateDatumType.DeviceImei, null);
			}
			set
			{
				base.Assign<StringData, string>(CustomStateDatumType.DeviceImei, value);
			}
		}

		public string DeviceFriendlyName
		{
			get
			{
				return base.Fetch<StringData, string>(CustomStateDatumType.DeviceFriendlyName, null);
			}
			set
			{
				base.Assign<StringData, string>(CustomStateDatumType.DeviceFriendlyName, value);
			}
		}

		public string DeviceOS
		{
			get
			{
				return base.Fetch<StringData, string>(CustomStateDatumType.DeviceOS, null);
			}
			set
			{
				base.Assign<StringData, string>(CustomStateDatumType.DeviceOS, value);
			}
		}

		public string DeviceOSLanguage
		{
			get
			{
				return base.Fetch<StringData, string>(CustomStateDatumType.DeviceOSLanguage, null);
			}
			set
			{
				base.Assign<StringData, string>(CustomStateDatumType.DeviceOSLanguage, value);
			}
		}

		public string DevicePhoneNumber
		{
			get
			{
				return base.Fetch<StringData, string>(CustomStateDatumType.DevicePhoneNumber, null);
			}
			set
			{
				base.Assign<StringData, string>(CustomStateDatumType.DevicePhoneNumber, value);
			}
		}

		public string UserAgent
		{
			get
			{
				return base.Fetch<StringData, string>(CustomStateDatumType.UserAgent, null);
			}
			set
			{
				base.Assign<StringData, string>(CustomStateDatumType.UserAgent, value);
			}
		}

		public bool DeviceEnableOutboundSMS
		{
			get
			{
				return base.Fetch<BooleanData, bool>(CustomStateDatumType.DeviceEnableOutboundSMS, false);
			}
			set
			{
				base.Assign<BooleanData, bool>(CustomStateDatumType.DeviceEnableOutboundSMS, value);
			}
		}

		public string DeviceMobileOperator
		{
			get
			{
				return base.Fetch<StringData, string>(CustomStateDatumType.DeviceMobileOperator, null);
			}
			set
			{
				base.Assign<StringData, string>(CustomStateDatumType.DeviceMobileOperator, value);
			}
		}

		public string RecoveryPassword
		{
			get
			{
				return base.Fetch<StringData, string>(CustomStateDatumType.RecoveryPassword, null);
			}
			set
			{
				base.Assign<StringData, string>(CustomStateDatumType.RecoveryPassword, value);
			}
		}

		public DeviceAccessState DeviceAccessState
		{
			get
			{
				return (DeviceAccessState)base.Fetch<Int32Data, int>(CustomStateDatumType.DeviceAccessState, 0);
			}
			set
			{
				base.Assign<Int32Data, int>(CustomStateDatumType.DeviceAccessState, (int)value);
			}
		}

		public DeviceAccessStateReason DeviceAccessStateReason
		{
			get
			{
				return (DeviceAccessStateReason)base.Fetch<Int32Data, int>(CustomStateDatumType.DeviceAccessStateReason, 0);
			}
			set
			{
				base.Assign<Int32Data, int>(CustomStateDatumType.DeviceAccessStateReason, (int)value);
			}
		}

		public DevicePolicyApplicationStatus DevicePolicyApplicationStatus
		{
			get
			{
				return (DevicePolicyApplicationStatus)base.Fetch<Int32Data, int>(CustomStateDatumType.DevicePolicyApplicationStatus, 1);
			}
			set
			{
				base.Assign<Int32Data, int>(CustomStateDatumType.DevicePolicyApplicationStatus, (int)value);
			}
		}

		public ADObjectId DevicePolicyApplied
		{
			get
			{
				return base.Fetch<ADObjectIdData, ADObjectId>(CustomStateDatumType.DevicePolicyApplied, null);
			}
			set
			{
				base.Assign<ADObjectIdData, ADObjectId>(CustomStateDatumType.DevicePolicyApplied, value);
			}
		}

		public ADObjectId DeviceAccessControlRule
		{
			get
			{
				return base.Fetch<ADObjectIdData, ADObjectId>(CustomStateDatumType.DeviceAccessControlRule, null);
			}
			set
			{
				base.Assign<ADObjectIdData, ADObjectId>(CustomStateDatumType.DeviceAccessControlRule, value);
			}
		}

		public string LastDeviceWipeRequestor
		{
			get
			{
				return base.Fetch<StringData, string>(CustomStateDatumType.LastDeviceWipeRequestor, null);
			}
			set
			{
				base.Assign<StringData, string>(CustomStateDatumType.LastDeviceWipeRequestor, value);
			}
		}

		public string DeviceActiveSyncVersion
		{
			get
			{
				return base.Fetch<StringData, string>(CustomStateDatumType.DeviceActiveSyncVersion, null);
			}
			set
			{
				base.Assign<StringData, string>(CustomStateDatumType.DeviceActiveSyncVersion, value);
			}
		}

		public string[] RemoteWipeConfirmationAddresses
		{
			get
			{
				return base.Fetch<ArrayData<StringData, string>, string[]>(CustomStateDatumType.WipeConfirmationAddresses, null);
			}
			set
			{
				base.Assign<ArrayData<StringData, string>, string[]>(CustomStateDatumType.WipeConfirmationAddresses, value);
			}
		}

		public int? ADDeviceInfoHash
		{
			get
			{
				return base.Fetch<NullableData<Int32Data, int>, int?>(CustomStateDatumType.ADDeviceInfoHash, null);
			}
			set
			{
				base.Assign<NullableData<Int32Data, int>, int?>(CustomStateDatumType.ADDeviceInfoHash, value);
			}
		}

		public bool HaveSentBoostrapMailForWM61
		{
			get
			{
				return base.Fetch<BooleanData, bool>(CustomStateDatumType.HaveSentBoostrapMailForWM61, false);
			}
			set
			{
				base.Assign<BooleanData, bool>(CustomStateDatumType.HaveSentBoostrapMailForWM61, value);
			}
		}

		public ExDateTime? BootstrapMailForWM61TriggeredTime
		{
			get
			{
				return base.FetchDateTime(CustomStateDatumType.BootstrapMailForWM61TriggeredTime);
			}
			set
			{
				base.Assign<NullableData<DateTimeData, ExDateTime>, ExDateTime?>(CustomStateDatumType.BootstrapMailForWM61TriggeredTime, value);
			}
		}

		public bool DeviceInformationReceived
		{
			get
			{
				return base.Fetch<BooleanData, bool>(CustomStateDatumType.DeviceInformationReceived, false);
			}
			set
			{
				base.Assign<BooleanData, bool>(CustomStateDatumType.DeviceInformationReceived, value);
			}
		}

		public ExDateTime? SyncStateUpgradeTime
		{
			get
			{
				return base.FetchDateTime(CustomStateDatumType.SSUpgradeDateTime);
			}
			set
			{
				base.Assign<NullableData<DateTimeData, ExDateTime>, ExDateTime?>(CustomStateDatumType.SSUpgradeDateTime, value);
			}
		}

		public ExDateTime? ADCreationTime
		{
			get
			{
				return base.FetchDateTime(CustomStateDatumType.ADCreationTime);
			}
			set
			{
				base.Assign<NullableData<DateTimeData, ExDateTime>, ExDateTime?>(CustomStateDatumType.ADCreationTime, value);
			}
		}

		public ADObjectId DeviceADObjectId
		{
			get
			{
				return base.Fetch<ADObjectIdData, ADObjectId>(CustomStateDatumType.DeviceADObjectId, null);
			}
			set
			{
				base.Assign<ADObjectIdData, ADObjectId>(CustomStateDatumType.DeviceADObjectId, value);
			}
		}

		public ADObjectId UserADObjectId
		{
			get
			{
				return base.Fetch<ADObjectIdData, ADObjectId>(CustomStateDatumType.UserADObjectId, null);
			}
			set
			{
				base.Assign<ADObjectIdData, ADObjectId>(CustomStateDatumType.UserADObjectId, value);
			}
		}

		public bool IsSyncStateJustUpgraded { get; private set; }

		public StoreObjectId ABQMailId
		{
			get
			{
				return base.Fetch<StoreObjectIdData, StoreObjectId>(CustomStateDatumType.ABQMailId, null);
			}
			set
			{
				base.Assign<StoreObjectIdData, StoreObjectId>(CustomStateDatumType.ABQMailId, value);
			}
		}

		public ABQMailState ABQMailState
		{
			get
			{
				return (ABQMailState)base.Fetch<Int32Data, int>(CustomStateDatumType.ABQMailState, 0);
			}
			set
			{
				base.Assign<Int32Data, int>(CustomStateDatumType.ABQMailState, (int)value);
			}
		}

		public bool DeviceInformationPromoted
		{
			get
			{
				return base.Fetch<BooleanData, bool>(CustomStateDatumType.DeviceInformationPromoted, false);
			}
			set
			{
				base.Assign<BooleanData, bool>(CustomStateDatumType.DeviceInformationPromoted, value);
			}
		}

		public string DevicePhoneNumberForSms
		{
			get
			{
				return base.Fetch<StringData, string>(CustomStateDatumType.DevicePhoneNumberForSms, null);
			}
			set
			{
				base.Assign<StringData, string>(CustomStateDatumType.DevicePhoneNumberForSms, value);
			}
		}

		public bool SmsSearchFolderCreated
		{
			get
			{
				return base.Fetch<BooleanData, bool>(CustomStateDatumType.SmsSearchFolderCreated, false);
			}
			set
			{
				base.Assign<BooleanData, bool>(CustomStateDatumType.SmsSearchFolderCreated, value);
			}
		}

		public DeviceBehavior DeviceBehavior
		{
			get
			{
				DeviceBehavior deviceBehavior = base.Fetch<DeviceBehaviorData, DeviceBehavior>(CustomStateDatumType.DeviceBehavior, null);
				if (deviceBehavior == null || !deviceBehavior.Validate())
				{
					deviceBehavior = new DeviceBehavior(true);
					this.DeviceBehavior = deviceBehavior;
				}
				if (deviceBehavior.Owner == null)
				{
					deviceBehavior.Owner = this;
				}
				return deviceBehavior;
			}
			set
			{
				base.Assign<DeviceBehaviorData, DeviceBehavior>(CustomStateDatumType.DeviceBehavior, value);
				value.Owner = this;
			}
		}

		internal bool WriteNewSyncData { get; set; }

		public static GlobalInfo LoadFromMailbox(MailboxSession mailboxSession, SyncStateStorage syncStateStorage, ProtocolLogger protocolLogger)
		{
			bool flag;
			return GlobalInfo.LoadFromMailbox(mailboxSession, syncStateStorage, protocolLogger, out flag);
		}

		public static GlobalInfo LoadFromMailbox(MailboxSession mailboxSession, SyncStateStorage syncStateStorage, ProtocolLogger protocolLogger, out bool updateUserHasPartnership)
		{
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			if (syncStateStorage == null)
			{
				throw new ArgumentNullException("syncStateStorage");
			}
			updateUserHasPartnership = false;
			CustomSyncState customSyncState = null;
			GlobalInfo globalInfo = null;
			bool isDirty = false;
			bool hasNewSyncData = false;
			bool hasNewAutdData = false;
			bool isSyncStateJustUpgraded = false;
			bool flag = false;
			GlobalInfo result;
			try
			{
				GlobalSyncStateInfo syncStateInfo = new GlobalSyncStateInfo();
				customSyncState = syncStateStorage.GetCustomSyncState(syncStateInfo, GlobalInfo.ExtraGlobalInfoPropertiesToFetch);
				if (customSyncState == null)
				{
					isDirty = true;
					using (CustomSyncState customSyncState2 = syncStateStorage.GetCustomSyncState(new FolderIdMappingSyncStateInfo(), new PropertyDefinition[0]))
					{
						if (customSyncState2 == null)
						{
							updateUserHasPartnership = true;
							syncStateStorage.DeleteAllSyncStates();
						}
					}
					customSyncState = syncStateStorage.CreateCustomSyncState(syncStateInfo);
					isDirty = true;
				}
				else
				{
					try
					{
						object obj = customSyncState.StoreObject.TryGetProperty(AirSyncStateSchema.LastSyncAttemptTime);
						hasNewSyncData = (obj != null && !(obj is PropertyError));
					}
					catch (Exception arg)
					{
						AirSyncDiagnostics.TraceError<Exception>(ExTraceGlobals.RequestsTracer, null, "[GlobalInfo.LoadFromMailbox] Encountered exception when checking for new syncStatus properties.  Exception: {0}", arg);
					}
					try
					{
						object obj2 = customSyncState.StoreObject.TryGetProperty(AirSyncStateSchema.LastPingHeartbeatInterval);
						hasNewAutdData = (obj2 != null && !(obj2 is PropertyError));
					}
					catch (Exception arg2)
					{
						AirSyncDiagnostics.TraceError<Exception>(ExTraceGlobals.RequestsTracer, null, "[GlobalInfo.LoadFromMailbox] Encountered exception when checking for new syncStatus properties.  Exception: {0}", arg2);
					}
					if (customSyncState.BackendVersion != null && customSyncState.BackendVersion.Value != customSyncState.Version)
					{
						isDirty = true;
						isSyncStateJustUpgraded = true;
						if (protocolLogger != null)
						{
							protocolLogger.SetValue(ProtocolLoggerData.Ssu, "2007");
						}
					}
				}
				globalInfo = new GlobalInfo(customSyncState);
				globalInfo.IsDirty = isDirty;
				globalInfo.HasNewSyncData = hasNewSyncData;
				globalInfo.HasNewAutdData = hasNewAutdData;
				globalInfo.IsSyncStateJustUpgraded = isSyncStateJustUpgraded;
				if (Command.CurrentCommand != null)
				{
					globalInfo.WriteNewSyncData = Command.CurrentCommand.User.Features.IsEnabled(EasFeature.SyncStatusOnGlobalInfo);
				}
				flag = true;
				result = globalInfo;
			}
			finally
			{
				if (!flag)
				{
					if (globalInfo != null)
					{
						globalInfo.Dispose();
					}
					else if (customSyncState != null)
					{
						customSyncState.Dispose();
					}
				}
			}
			return result;
		}

		internal static int ComputeADDeviceInfoHash(GlobalInfo globalInfo)
		{
			if (globalInfo == null)
			{
				return 0;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(globalInfo.DeviceFriendlyName);
			stringBuilder.Append(globalInfo.DeviceImei);
			stringBuilder.Append(globalInfo.DeviceOS);
			stringBuilder.Append(globalInfo.DeviceOSLanguage);
			stringBuilder.Append(DeviceInfo.ObfuscatePhoneNumber(globalInfo.DevicePhoneNumber));
			stringBuilder.Append(globalInfo.UserAgent);
			stringBuilder.Append(globalInfo.DeviceModel);
			stringBuilder.Append(globalInfo.DeviceMobileOperator);
			stringBuilder.Append(globalInfo.DeviceAccessState);
			stringBuilder.Append(globalInfo.DeviceAccessStateReason);
			stringBuilder.Append(globalInfo.DeviceActiveSyncVersion);
			stringBuilder.Append(globalInfo.DeviceAccessControlRule);
			return stringBuilder.ToString().GetHashCode();
		}

		internal static int ComputeADDeviceInfoHash(MobileDevice activeSyncDevice)
		{
			if (activeSyncDevice == null)
			{
				return 0;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(activeSyncDevice.FriendlyName);
			stringBuilder.Append(activeSyncDevice.DeviceImei);
			stringBuilder.Append(activeSyncDevice.DeviceOS);
			stringBuilder.Append(activeSyncDevice.DeviceOSLanguage);
			stringBuilder.Append(DeviceInfo.ObfuscatePhoneNumber(activeSyncDevice.DeviceTelephoneNumber));
			stringBuilder.Append(activeSyncDevice.DeviceUserAgent);
			stringBuilder.Append(activeSyncDevice.DeviceModel);
			stringBuilder.Append(activeSyncDevice.DeviceMobileOperator);
			stringBuilder.Append(activeSyncDevice.DeviceAccessState);
			stringBuilder.Append(activeSyncDevice.DeviceAccessStateReason);
			stringBuilder.Append(activeSyncDevice.ClientVersion);
			stringBuilder.Append(activeSyncDevice.DeviceAccessControlRule);
			return stringBuilder.ToString().GetHashCode();
		}

		internal static void CopyValuesFromGlobalInfo(MobileDevice mobileDevice, GlobalInfo globalInfo)
		{
			if (globalInfo != null)
			{
				mobileDevice.FriendlyName = globalInfo.DeviceFriendlyName;
				mobileDevice.DeviceImei = globalInfo.DeviceImei;
				mobileDevice.DeviceOS = globalInfo.DeviceOS;
				mobileDevice.DeviceOSLanguage = globalInfo.DeviceOSLanguage;
				mobileDevice.DeviceTelephoneNumber = DeviceInfo.ObfuscatePhoneNumber(globalInfo.DevicePhoneNumber);
				mobileDevice.DeviceUserAgent = globalInfo.UserAgent;
				mobileDevice.DeviceModel = globalInfo.DeviceModel;
				mobileDevice.DeviceMobileOperator = globalInfo.DeviceMobileOperator;
				mobileDevice.DeviceAccessState = globalInfo.DeviceAccessState;
				mobileDevice.DeviceAccessStateReason = globalInfo.DeviceAccessStateReason;
				mobileDevice.ClientVersion = globalInfo.DeviceActiveSyncVersion;
				mobileDevice.DeviceAccessControlRule = globalInfo.DeviceAccessControlRule;
			}
		}

		public static readonly PropertyDefinition[] ExtraGlobalInfoPropertiesToFetch = new PropertyDefinition[]
		{
			AirSyncStateSchema.ClientCategoryList,
			AirSyncStateSchema.LastSeenClientIds,
			AirSyncStateSchema.LastSyncAttemptTime,
			AirSyncStateSchema.LastSyncSuccessTime,
			AirSyncStateSchema.LastSyncUserAgent,
			AirSyncStateSchema.LastPingHeartbeatInterval
		};
	}
}
