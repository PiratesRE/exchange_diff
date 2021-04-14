using System;
using Microsoft.Exchange.AirSync;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Tasks
{
	[Serializable]
	public class MobileDeviceConfiguration : IConfigurable
	{
		public MobileDeviceConfiguration()
		{
		}

		public MobileDeviceConfiguration(DeviceInfo deviceInfo)
		{
			ArgumentValidator.ThrowIfNull("deviceInfo", deviceInfo);
			ArgumentValidator.ThrowIfNull("deviceInfo.DeviceIdentity", deviceInfo.DeviceIdentity);
			MobileClientType clientType;
			if (!deviceInfo.DeviceIdentity.TryGetMobileClientType(out clientType))
			{
				throw new ArgumentException("deviceInfo.Protocol", "Protocol can only be EAS or MOWA.");
			}
			this.ClientType = clientType;
			this.recoveryPassword = deviceInfo.RecoveryPassword;
			this.FirstSyncTime = ((deviceInfo.FirstSyncTime != null) ? ((DateTime?)deviceInfo.FirstSyncTime) : null);
			this.LastPolicyUpdateTime = ((deviceInfo.LastPolicyUpdateTime != null) ? ((DateTime?)deviceInfo.LastPolicyUpdateTime) : null);
			this.LastSyncAttemptTime = ((deviceInfo.LastSyncAttemptTime != null) ? ((DateTime?)deviceInfo.LastSyncAttemptTime) : null);
			this.LastSuccessSync = ((deviceInfo.LastSyncSuccessTime != null) ? ((DateTime?)deviceInfo.LastSyncSuccessTime) : null);
			this.DeviceID = deviceInfo.DeviceIdentity.DeviceId;
			this.DeviceUserAgent = (deviceInfo.UserAgent ?? string.Empty);
			this.DeviceWipeAckTime = ((deviceInfo.WipeAckTime != null) ? ((DateTime?)deviceInfo.WipeAckTime) : null);
			this.DeviceWipeRequestTime = ((deviceInfo.WipeRequestTime != null) ? ((DateTime?)deviceInfo.WipeRequestTime) : null);
			this.DeviceWipeSentTime = ((deviceInfo.WipeSentTime != null) ? ((DateTime?)deviceInfo.WipeSentTime) : null);
			this.LastPingHeartbeat = deviceInfo.LastPingHeartbeat;
			this.DeviceModel = deviceInfo.DeviceModel;
			this.DeviceImei = deviceInfo.DeviceImei;
			this.DeviceFriendlyName = deviceInfo.DeviceFriendlyName;
			this.DeviceOS = deviceInfo.DeviceOS;
			if (this.ClientType == MobileClientType.MOWA)
			{
				string format = Strings.MOWADeviceTypePrefix;
				string arg;
				if ((arg = deviceInfo.DeviceFriendlyName) == null && (arg = deviceInfo.DeviceModel) == null)
				{
					arg = (deviceInfo.DeviceIdentity.DeviceType ?? deviceInfo.DeviceOS);
				}
				this.DeviceType = string.Format(format, arg);
				this.IsRemoteWipeSupported = true;
			}
			else
			{
				this.DeviceType = deviceInfo.DeviceIdentity.DeviceType;
				this.IsRemoteWipeSupported = deviceInfo.IsRemoteWipeSupported;
			}
			this.DeviceOSLanguage = deviceInfo.DeviceOSLanguage;
			this.DeviceEnableOutboundSMS = deviceInfo.DeviceEnableOutboundSMS;
			this.DeviceMobileOperator = deviceInfo.DeviceMobileOperator;
			this.DeviceAccessState = deviceInfo.DeviceAccessState;
			this.DeviceAccessStateReason = deviceInfo.DeviceAccessStateReason;
			this.DeviceAccessControlRule = deviceInfo.DeviceAccessControlRule;
			this.DevicePolicyApplied = deviceInfo.DevicePolicyApplied;
			this.DevicePolicyApplicationStatus = deviceInfo.DevicePolicyApplicationStatus;
			this.LastDeviceWipeRequestor = deviceInfo.LastDeviceWipeRequestor;
			this.ClientVersion = deviceInfo.ClientVersion;
			this.NumberOfFoldersSynced = deviceInfo.NumberOfFoldersSynced;
			this.SyncStateUpgradeTime = ((deviceInfo.SSUpgradeDateTime != null) ? ((DateTime?)deviceInfo.SSUpgradeDateTime) : null);
			this.obfuscatedPhoneNumber = DeviceInfo.ObfuscatePhoneNumber(deviceInfo.DevicePhoneNumber);
			this.identity = deviceInfo.DeviceADObjectId;
			this.Guid = deviceInfo.DeviceADObjectId.ObjectGuid;
		}

		public DateTime? FirstSyncTime { get; set; }

		public DateTime? LastPolicyUpdateTime { get; set; }

		public DateTime? LastSyncAttemptTime { get; set; }

		public DateTime? LastSuccessSync { get; set; }

		public string DeviceType { get; set; }

		public string DeviceID { get; set; }

		public string DeviceUserAgent { get; set; }

		public DateTime? DeviceWipeSentTime { get; set; }

		public DateTime? DeviceWipeRequestTime { get; set; }

		public DateTime? DeviceWipeAckTime { get; set; }

		public uint? LastPingHeartbeat { get; set; }

		public string RecoveryPassword
		{
			get
			{
				return this.recoveryPassword;
			}
			set
			{
				this.recoveryPassword = value;
			}
		}

		public string DeviceModel { get; set; }

		public string DeviceImei { get; set; }

		public string DeviceFriendlyName { get; set; }

		public string DeviceOS { get; set; }

		public string DeviceOSLanguage { get; set; }

		public string DevicePhoneNumber
		{
			get
			{
				return this.obfuscatedPhoneNumber;
			}
			set
			{
				this.obfuscatedPhoneNumber = value;
			}
		}

		public string MailboxLogReport
		{
			get
			{
				return this.mailboxLogReport;
			}
			set
			{
				this.mailboxLogReport = value;
			}
		}

		public bool DeviceEnableOutboundSMS { get; set; }

		public string DeviceMobileOperator { get; set; }

		public ObjectId Identity
		{
			get
			{
				return this.identity;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.identity = value;
			}
		}

		public Guid Guid { get; set; }

		public bool IsRemoteWipeSupported { get; set; }

		public DeviceRemoteWipeStatus Status
		{
			get
			{
				if (this.DeviceWipeAckTime != null)
				{
					return DeviceRemoteWipeStatus.DeviceWipeSucceeded;
				}
				if (this.DeviceWipeRequestTime == null && this.DeviceWipeSentTime == null)
				{
					return DeviceRemoteWipeStatus.DeviceOk;
				}
				if (!this.IsRemoteWipeSupported)
				{
					return DeviceRemoteWipeStatus.DeviceBlocked;
				}
				return DeviceRemoteWipeStatus.DeviceWipePending;
			}
		}

		public string StatusNote
		{
			get
			{
				string result = string.Empty;
				switch (this.Status)
				{
				case DeviceRemoteWipeStatus.DeviceWipePending:
					result = Strings.WipePendingNote;
					break;
				case DeviceRemoteWipeStatus.DeviceWipeSucceeded:
					result = Strings.WipeSucceededNote;
					break;
				}
				return result;
			}
		}

		public DeviceAccessState DeviceAccessState { get; set; }

		public DeviceAccessStateReason DeviceAccessStateReason { get; set; }

		public ADObjectId DeviceAccessControlRule { get; set; }

		public ADObjectId DevicePolicyApplied { get; set; }

		public DevicePolicyApplicationStatus DevicePolicyApplicationStatus { get; set; }

		public string LastDeviceWipeRequestor { get; set; }

		public string ClientVersion { get; protected set; }

		public int NumberOfFoldersSynced { get; set; }

		public DateTime? SyncStateUpgradeTime { get; set; }

		public MobileClientType ClientType { get; set; }

		bool IConfigurable.IsValid
		{
			get
			{
				return true;
			}
		}

		ObjectState IConfigurable.ObjectState
		{
			get
			{
				return ObjectState.Unchanged;
			}
		}

		ValidationError[] IConfigurable.Validate()
		{
			throw new NotImplementedException();
		}

		void IConfigurable.CopyChangesFrom(IConfigurable source)
		{
			throw new NotImplementedException();
		}

		void IConfigurable.ResetChangeTracking()
		{
			throw new NotImplementedException();
		}

		private string recoveryPassword;

		private string obfuscatedPhoneNumber;

		private ObjectId identity;

		private string mailboxLogReport;
	}
}
