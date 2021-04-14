using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class MobileDeviceStatistics : OptionsPropertyChangeTracker
	{
		[DataMember(EmitDefaultValue = false)]
		[DateTimeString]
		public string FirstSyncTime
		{
			get
			{
				return this.firstSyncTime;
			}
			set
			{
				this.firstSyncTime = value;
				base.TrackPropertyChanged("FirstSyncTime");
			}
		}

		[DataMember(EmitDefaultValue = false)]
		[DateTimeString]
		public string LastPolicyUpdateTime
		{
			get
			{
				return this.lastPolicyUpdateTime;
			}
			set
			{
				this.lastPolicyUpdateTime = value;
				base.TrackPropertyChanged("LastPolicyUpdateTime");
			}
		}

		[DataMember(EmitDefaultValue = false)]
		[DateTimeString]
		public string LastSyncAttemptTime
		{
			get
			{
				return this.lastSyncAttemptTime;
			}
			set
			{
				this.lastSyncAttemptTime = value;
				base.TrackPropertyChanged("LastSyncAttemptTime");
			}
		}

		[DateTimeString]
		[DataMember(EmitDefaultValue = false)]
		public string LastSuccessSync
		{
			get
			{
				return this.lastSuccessSync;
			}
			set
			{
				this.lastSuccessSync = value;
				base.TrackPropertyChanged("LastSuccessSync");
			}
		}

		[DataMember]
		public string DeviceType
		{
			get
			{
				return this.deviceType;
			}
			set
			{
				this.deviceType = value;
				base.TrackPropertyChanged("DeviceType");
			}
		}

		[DataMember]
		public string DeviceId
		{
			get
			{
				return this.deviceId;
			}
			set
			{
				this.deviceId = value;
				base.TrackPropertyChanged("DeviceId");
			}
		}

		[DataMember]
		public string DeviceUserAgent
		{
			get
			{
				return this.deviceUserAgent;
			}
			set
			{
				this.deviceUserAgent = value;
				base.TrackPropertyChanged("DeviceUserAgent");
			}
		}

		[DateTimeString]
		[DataMember(EmitDefaultValue = false)]
		public string DeviceWipeSentTime
		{
			get
			{
				return this.deviceWipeSentTime;
			}
			set
			{
				this.deviceWipeSentTime = value;
				base.TrackPropertyChanged("DeviceWipeSentTime");
			}
		}

		[DataMember]
		public string DeviceModel
		{
			get
			{
				return this.deviceModel;
			}
			set
			{
				this.deviceModel = value;
				base.TrackPropertyChanged("DeviceModel");
			}
		}

		[DataMember]
		public string DeviceImei
		{
			get
			{
				return this.deviceImei;
			}
			set
			{
				this.deviceImei = value;
				base.TrackPropertyChanged("DeviceImei");
			}
		}

		[DataMember]
		public string DeviceFriendlyName
		{
			get
			{
				return this.deviceFriendlyName;
			}
			set
			{
				this.deviceFriendlyName = value;
				base.TrackPropertyChanged("DeviceFriendlyName");
			}
		}

		[DataMember]
		public string DeviceOS
		{
			get
			{
				return this.deviceOS;
			}
			set
			{
				this.deviceOS = value;
				base.TrackPropertyChanged("DeviceOS");
			}
		}

		[DataMember]
		public string DeviceOSLanguage
		{
			get
			{
				return this.deviceOSLanguage;
			}
			set
			{
				this.deviceOSLanguage = value;
				base.TrackPropertyChanged("DeviceOSLanguage");
			}
		}

		[DataMember]
		public string DevicePhoneNumber
		{
			get
			{
				return this.devicePhoneNumber;
			}
			set
			{
				this.devicePhoneNumber = value;
				base.TrackPropertyChanged("DevicePhoneNumber");
			}
		}

		[DataMember]
		public string DeviceMobileOperator
		{
			get
			{
				return this.deviceMobileOperator;
			}
			set
			{
				this.deviceMobileOperator = value;
				base.TrackPropertyChanged("DeviceMobileOperator");
			}
		}

		[DataMember]
		public string ClientVersion
		{
			get
			{
				return this.clientVersion;
			}
			set
			{
				this.clientVersion = value;
				base.TrackPropertyChanged("ClientVersion");
			}
		}

		[IgnoreDataMember]
		public DeviceAccessState DeviceAccessState
		{
			get
			{
				return this.deviceAccessState;
			}
			set
			{
				this.deviceAccessState = value;
				base.TrackPropertyChanged("DeviceAccessState");
			}
		}

		[DataMember(Name = "DeviceAccessState", IsRequired = false, EmitDefaultValue = false)]
		public string DeviceAccessStateString
		{
			get
			{
				return EnumUtilities.ToString<DeviceAccessState>(this.DeviceAccessState);
			}
			set
			{
				this.DeviceAccessState = EnumUtilities.Parse<DeviceAccessState>(value);
			}
		}

		[IgnoreDataMember]
		public DeviceAccessStateReason DeviceAccessStateReason
		{
			get
			{
				return this.deviceAccessStateReason;
			}
			set
			{
				this.deviceAccessStateReason = value;
				base.TrackPropertyChanged("DeviceAccessStateReason");
			}
		}

		[DataMember(Name = "DeviceAccessStateReason", IsRequired = false, EmitDefaultValue = false)]
		public string DeviceAccessStateReasonString
		{
			get
			{
				return EnumUtilities.ToString<DeviceAccessStateReason>(this.DeviceAccessStateReason);
			}
			set
			{
				this.DeviceAccessStateReason = EnumUtilities.Parse<DeviceAccessStateReason>(value);
				base.TrackPropertyChanged("DeviceAccessStateReasonString");
			}
		}

		[DataMember]
		public Identity DeviceAccessControlRule
		{
			get
			{
				return this.deviceAccessControlRule;
			}
			set
			{
				this.deviceAccessControlRule = value;
				base.TrackPropertyChanged("DeviceAccessControlRule");
			}
		}

		[IgnoreDataMember]
		public MobileClientType ClientType
		{
			get
			{
				return this.clientType;
			}
			set
			{
				this.clientType = value;
				base.TrackPropertyChanged("ClientType");
			}
		}

		[DataMember(Name = "ClientType", IsRequired = false, EmitDefaultValue = false)]
		public string ClientTypeString
		{
			get
			{
				return EnumUtilities.ToString<MobileClientType>(this.ClientType);
			}
			set
			{
				this.ClientType = EnumUtilities.Parse<MobileClientType>(value);
				base.TrackPropertyChanged("ClientTypeString");
			}
		}

		[DateTimeString]
		[DataMember(EmitDefaultValue = false)]
		public string DeviceWipeRequestTime
		{
			get
			{
				return this.deviceWipeRequestTime;
			}
			set
			{
				this.deviceWipeRequestTime = value;
				base.TrackPropertyChanged("DeviceWipeRequestTime");
			}
		}

		[DateTimeString]
		[DataMember(EmitDefaultValue = false)]
		public string DeviceWipeAckTime
		{
			get
			{
				return this.deviceWipeAckTime;
			}
			set
			{
				this.deviceWipeAckTime = value;
				base.TrackPropertyChanged("DeviceWipeAckTime");
			}
		}

		[DataMember]
		public uint? LastPingHeartBeat
		{
			get
			{
				return this.lastPingHeartBeat;
			}
			set
			{
				this.lastPingHeartBeat = value;
				base.TrackPropertyChanged("LastPingHeartBeat");
			}
		}

		[DataMember]
		public string RecoveryPassword
		{
			get
			{
				return this.recoveryPassword;
			}
			set
			{
				this.recoveryPassword = value;
				base.TrackPropertyChanged("RecoveryPassword");
			}
		}

		[DataMember]
		public Identity Identity
		{
			get
			{
				return this.identity;
			}
			set
			{
				this.identity = value;
				base.TrackPropertyChanged("Identity");
			}
		}

		[DataMember]
		public bool IsRemoteWipeSupported
		{
			get
			{
				return this.isRemoteWipeSupported;
			}
			set
			{
				this.isRemoteWipeSupported = value;
				base.TrackPropertyChanged("IsRemoteWipeSupported");
			}
		}

		[IgnoreDataMember]
		public DeviceRemoteWipeStatus Status
		{
			get
			{
				return this.status;
			}
			set
			{
				this.status = value;
				base.TrackPropertyChanged("Status");
			}
		}

		[DataMember(Name = "Status", IsRequired = false, EmitDefaultValue = false)]
		public string DeviceRemoteWipeStatusString
		{
			get
			{
				return EnumUtilities.ToString<DeviceRemoteWipeStatus>(this.Status);
			}
			set
			{
				this.Status = EnumUtilities.Parse<DeviceRemoteWipeStatus>(value);
				base.TrackPropertyChanged("DeviceRemoteWipeStatusString");
			}
		}

		[DataMember]
		public string StatusNote
		{
			get
			{
				return this.statusNote;
			}
			set
			{
				this.statusNote = value;
				base.TrackPropertyChanged("StatusNote");
			}
		}

		[DataMember]
		public Identity DevicePolicyApplied
		{
			get
			{
				return this.devicePolicyApplied;
			}
			set
			{
				this.devicePolicyApplied = value;
				base.TrackPropertyChanged("DevicePolicyApplied");
			}
		}

		[DataMember]
		public int NumberOfFoldersSynced
		{
			get
			{
				return this.numberOfFoldersSynced;
			}
			set
			{
				this.numberOfFoldersSynced = value;
				base.TrackPropertyChanged("NumberOfFoldersSynced");
			}
		}

		[IgnoreDataMember]
		public DevicePolicyApplicationStatus DevicePolicyApplicationStatus
		{
			get
			{
				return this.devicePolicyApplicationStatus;
			}
			set
			{
				this.devicePolicyApplicationStatus = value;
				base.TrackPropertyChanged("DevicePolicyApplicationStatus");
			}
		}

		[DataMember(Name = "DevicePolicyApplicationStatus", IsRequired = false, EmitDefaultValue = false)]
		public string DevicePolicyApplicationStatusString
		{
			get
			{
				return EnumUtilities.ToString<DevicePolicyApplicationStatus>(this.DevicePolicyApplicationStatus);
			}
			set
			{
				this.DevicePolicyApplicationStatus = EnumUtilities.Parse<DevicePolicyApplicationStatus>(value);
				base.TrackPropertyChanged("DevicePolicyApplicationStatusString");
			}
		}

		private string firstSyncTime;

		private string lastPolicyUpdateTime;

		private string lastSyncAttemptTime;

		private string lastSuccessSync;

		private string deviceType;

		private string deviceId;

		private string deviceUserAgent;

		private string deviceWipeSentTime;

		private string deviceModel;

		private string deviceImei;

		private string deviceFriendlyName;

		private string deviceOS;

		private string deviceOSLanguage;

		private string devicePhoneNumber;

		private string deviceMobileOperator;

		private string clientVersion;

		private DeviceAccessState deviceAccessState;

		private DeviceAccessStateReason deviceAccessStateReason;

		private Identity deviceAccessControlRule;

		private MobileClientType clientType;

		private string deviceWipeRequestTime;

		private string deviceWipeAckTime;

		private uint? lastPingHeartBeat;

		private string recoveryPassword;

		private Identity identity;

		private bool isRemoteWipeSupported;

		private DeviceRemoteWipeStatus status;

		private string statusNote;

		private Identity devicePolicyApplied;

		private DevicePolicyApplicationStatus devicePolicyApplicationStatus;

		private int numberOfFoldersSynced;
	}
}
