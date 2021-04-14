using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("New", "DeviceConditionalAccessRule", SupportsShouldProcess = true)]
	public sealed class NewDeviceConditionalAccessRule : NewDeviceRuleBase
	{
		public NewDeviceConditionalAccessRule() : base(PolicyScenario.DeviceConditionalAccess)
		{
		}

		protected override DeviceRuleBase CreateDeviceRule(RuleStorage ruleStorage)
		{
			return new DeviceConditionalAccessRule(ruleStorage);
		}

		protected override Exception GetDeviceRuleAlreadyExistsException(string name)
		{
			return new DeviceConditionalAccessRuleAlreadyExistsException(name);
		}

		protected override bool GetDeviceRuleGuidFromWorkload(Workload workload, out Guid ruleGuid)
		{
			ruleGuid = default(Guid);
			return DevicePolicyUtility.GetConditionalAccessRuleGuidFromWorkload(workload, out ruleGuid);
		}

		[Parameter(Mandatory = false)]
		public bool? AllowJailbroken
		{
			get
			{
				return (bool?)base.Fields[DeviceConditionalAccessRule.Device_Security_Jailbroken];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_Security_Jailbroken] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? PasswordRequired
		{
			get
			{
				return (bool?)base.Fields[DeviceConditionalAccessRule.Device_Password_Required];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_Password_Required] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? PhoneMemoryEncrypted
		{
			get
			{
				return (bool?)base.Fields[DeviceConditionalAccessRule.Device_Encryption_PhoneMemoryEncrypted];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_Encryption_PhoneMemoryEncrypted] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public TimeSpan? PasswordTimeout
		{
			get
			{
				return (TimeSpan?)base.Fields[DeviceConditionalAccessRule.Device_Password_Timeout];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_Password_Timeout] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? PasswordMinimumLength
		{
			get
			{
				return (int?)base.Fields[DeviceConditionalAccessRule.Device_Password_MinimumLength];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_Password_MinimumLength] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? PasswordHistoryCount
		{
			get
			{
				return (int?)base.Fields[DeviceConditionalAccessRule.Device_Password_History];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_Password_History] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? PasswordExpirationDays
		{
			get
			{
				return (int?)base.Fields[DeviceConditionalAccessRule.Device_Password_Expiration];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_Password_Expiration] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? PasswordMinComplexChars
		{
			get
			{
				return (int?)base.Fields[DeviceConditionalAccessRule.Device_Password_MinComplexChars];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_Password_MinComplexChars] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? AllowSimplePassword
		{
			get
			{
				return (bool?)base.Fields[DeviceConditionalAccessRule.Device_Password_AllowSimplePassword];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_Password_AllowSimplePassword] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? PasswordQuality
		{
			get
			{
				return (int?)base.Fields[DeviceConditionalAccessRule.Device_Password_PasswordQuality];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_Password_PasswordQuality] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? MaxPasswordAttemptsBeforeWipe
		{
			get
			{
				return (int?)base.Fields[DeviceConditionalAccessRule.Device_Password_MaxAttemptsBeforeWipe];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_Password_MaxAttemptsBeforeWipe] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? EnableRemovableStorage
		{
			get
			{
				return (bool?)base.Fields[DeviceConditionalAccessRule.Device_Security_EnableRemovableStorage];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_Security_EnableRemovableStorage] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? CameraEnabled
		{
			get
			{
				return (bool?)base.Fields[DeviceConditionalAccessRule.Device_Security_CameraEnabled];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_Security_CameraEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? BluetoothEnabled
		{
			get
			{
				return (bool?)base.Fields[DeviceConditionalAccessRule.Device_Security_BluetoothEnabled];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_Security_BluetoothEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? ForceEncryptedBackup
		{
			get
			{
				return (bool?)base.Fields[DeviceConditionalAccessRule.Device_Cloud_ForceEncryptedBackup];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_Cloud_ForceEncryptedBackup] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? AllowiCloudDocSync
		{
			get
			{
				return (bool?)base.Fields[DeviceConditionalAccessRule.Device_Cloud_AllowiCloudDocSync];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_Cloud_AllowiCloudDocSync] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? AllowiCloudPhotoSync
		{
			get
			{
				return (bool?)base.Fields[DeviceConditionalAccessRule.Device_Cloud_AllowiCloudPhotoSync];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_Cloud_AllowiCloudPhotoSync] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? AllowiCloudBackup
		{
			get
			{
				return (bool?)base.Fields[DeviceConditionalAccessRule.Device_Cloud_AllowiCloudBackup];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_Cloud_AllowiCloudBackup] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public CARatingRegionEntry? RegionRatings
		{
			get
			{
				return (CARatingRegionEntry?)base.Fields[DeviceConditionalAccessRule.Device_Restrictions_RatingsRegion];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_Restrictions_RatingsRegion] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public CARatingMovieEntry? MoviesRating
		{
			get
			{
				return (CARatingMovieEntry?)base.Fields[DeviceConditionalAccessRule.Device_Restrictions_RatingMovies];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_Restrictions_RatingMovies] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public CARatingTvShowEntry? TVShowsRating
		{
			get
			{
				return (CARatingTvShowEntry?)base.Fields[DeviceConditionalAccessRule.Device_Restrictions_RatingTVShows];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_Restrictions_RatingTVShows] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public CARatingAppsEntry? AppsRating
		{
			get
			{
				return (CARatingAppsEntry?)base.Fields[DeviceConditionalAccessRule.Device_Restrictions_RatingApps];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_Restrictions_RatingApps] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? AllowVoiceDialing
		{
			get
			{
				return (bool?)base.Fields[DeviceConditionalAccessRule.Device_Restrictions_AllowVoiceDialing];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_Restrictions_AllowVoiceDialing] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? AllowVoiceAssistant
		{
			get
			{
				return (bool?)base.Fields[DeviceConditionalAccessRule.Device_Restrictions_AllowVoiceAssistant];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_Restrictions_AllowVoiceAssistant] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? AllowAssistantWhileLocked
		{
			get
			{
				return (bool?)base.Fields[DeviceConditionalAccessRule.Device_Restrictions_AllowAssistantWhileLocked];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_Restrictions_AllowAssistantWhileLocked] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? AllowScreenshot
		{
			get
			{
				return (bool?)base.Fields[DeviceConditionalAccessRule.Device_Restrictions_AllowScreenshot];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_Restrictions_AllowScreenshot] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? AllowVideoConferencing
		{
			get
			{
				return (bool?)base.Fields[DeviceConditionalAccessRule.Device_Restrictions_AllowVideoConferencing];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_Restrictions_AllowVideoConferencing] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? AllowPassbookWhileLocked
		{
			get
			{
				return (bool?)base.Fields[DeviceConditionalAccessRule.Device_Restrictions_AllowPassbookWhileLocked];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_Restrictions_AllowPassbookWhileLocked] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? AllowDiagnosticSubmission
		{
			get
			{
				return (bool?)base.Fields[DeviceConditionalAccessRule.Device_Restrictions_AllowDiagnosticSubmission];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_Restrictions_AllowDiagnosticSubmission] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? AllowConvenienceLogon
		{
			get
			{
				return (bool?)base.Fields[DeviceConditionalAccessRule.Device_Password_AllowConvenienceLogon];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_Password_AllowConvenienceLogon] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public TimeSpan? MaxPasswordGracePeriod
		{
			get
			{
				return (TimeSpan?)base.Fields[DeviceConditionalAccessRule.Device_Password_MaxGracePeriod];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_Password_MaxGracePeriod] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? AllowAppStore
		{
			get
			{
				return (bool?)base.Fields[DeviceConditionalAccessRule.Device_Restrictions_AllowAppStore];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_Restrictions_AllowAppStore] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? ForceAppStorePassword
		{
			get
			{
				return (bool?)base.Fields[DeviceConditionalAccessRule.Device_Restrictions_ForceAppStorePassword];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_Restrictions_ForceAppStorePassword] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? SystemSecurityTLS
		{
			get
			{
				return (bool?)base.Fields[DeviceConditionalAccessRule.Device_SystemSecurity_TLS];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_SystemSecurity_TLS] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public CAUserAccountControlStatusEntry? UserAccountControlStatus
		{
			get
			{
				return (CAUserAccountControlStatusEntry?)base.Fields[DeviceConditionalAccessRule.Device_SystemSecurity_UserAccountControlStatus];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_SystemSecurity_UserAccountControlStatus] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public CAFirewallStatusEntry? FirewallStatus
		{
			get
			{
				return (CAFirewallStatusEntry?)base.Fields[DeviceConditionalAccessRule.Device_SystemSecurity_FirewallStatus];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_SystemSecurity_FirewallStatus] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public CAAutoUpdateStatusEntry? AutoUpdateStatus
		{
			get
			{
				return (CAAutoUpdateStatusEntry?)base.Fields[DeviceConditionalAccessRule.Device_SystemSecurity_AutoUpdateStatus];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_SystemSecurity_AutoUpdateStatus] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string AntiVirusStatus
		{
			get
			{
				return (string)base.Fields[DeviceConditionalAccessRule.Device_SystemSecurity_AntiVirusStatus];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_SystemSecurity_AntiVirusStatus] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? AntiVirusSignatureStatus
		{
			get
			{
				return (bool?)base.Fields[DeviceConditionalAccessRule.Device_SystemSecurity_AntiVirusSignatureStatus];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_SystemSecurity_AntiVirusSignatureStatus] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? SmartScreenEnabled
		{
			get
			{
				return (bool?)base.Fields[DeviceConditionalAccessRule.Device_InternetExplorer_SmartScreenEnabled];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_InternetExplorer_SmartScreenEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string WorkFoldersSyncUrl
		{
			get
			{
				return (string)base.Fields[DeviceConditionalAccessRule.Device_WorkFolders_SyncUrl];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_WorkFolders_SyncUrl] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string PasswordComplexity
		{
			get
			{
				return (string)base.Fields[DeviceConditionalAccessRule.Device_Password_Type];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_Password_Type] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? WLANEnabled
		{
			get
			{
				return (bool?)base.Fields[DeviceConditionalAccessRule.Device_Wireless_WLANEnabled];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Device_Wireless_WLANEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string AccountName
		{
			get
			{
				return (string)base.Fields[DeviceConditionalAccessRule.Eas_AccountName];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Eas_AccountName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string AccountUserName
		{
			get
			{
				return (string)base.Fields[DeviceConditionalAccessRule.Eas_UserName];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Eas_UserName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string ExchangeActiveSyncHost
		{
			get
			{
				return (string)base.Fields[DeviceConditionalAccessRule.Eas_Host];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Eas_Host] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string EmailAddress
		{
			get
			{
				return (string)base.Fields[DeviceConditionalAccessRule.Eas_EmailAddress];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Eas_EmailAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? UseSSL
		{
			get
			{
				return (bool?)base.Fields[DeviceConditionalAccessRule.Eas_UseSSL];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Eas_UseSSL] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? AllowMove
		{
			get
			{
				return (bool?)base.Fields[DeviceConditionalAccessRule.Eas_PreventAppSheet];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Eas_PreventAppSheet] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? AllowRecentAddressSyncing
		{
			get
			{
				return (bool?)base.Fields[DeviceConditionalAccessRule.Eas_DisableMailRecentsSyncing];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Eas_DisableMailRecentsSyncing] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public long? DaysToSync
		{
			get
			{
				return (long?)base.Fields[DeviceConditionalAccessRule.Eas_MailNumberOfPastDaysToSync];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Eas_MailNumberOfPastDaysToSync] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public long? ContentType
		{
			get
			{
				return (long?)base.Fields[DeviceConditionalAccessRule.Eas_ContentTypeToSync];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Eas_ContentTypeToSync] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? UseSMIME
		{
			get
			{
				return (bool?)base.Fields[DeviceConditionalAccessRule.Eas_UseSMIME];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Eas_UseSMIME] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public long? SyncSchedule
		{
			get
			{
				return (long?)base.Fields[DeviceConditionalAccessRule.Eas_SyncSchedule];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Eas_SyncSchedule] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? UseOnlyInEmail
		{
			get
			{
				return (bool?)base.Fields[DeviceConditionalAccessRule.Eas_PreventMove];
			}
			set
			{
				base.Fields[DeviceConditionalAccessRule.Eas_PreventMove] = value;
			}
		}

		protected override void SetPropsOnDeviceRule(DeviceRuleBase pdeviceRule)
		{
			DeviceConditionalAccessRule deviceConditionalAccessRule = (DeviceConditionalAccessRule)pdeviceRule;
			deviceConditionalAccessRule.AllowJailbroken = this.AllowJailbroken;
			deviceConditionalAccessRule.PasswordRequired = this.PasswordRequired;
			deviceConditionalAccessRule.PhoneMemoryEncrypted = this.PhoneMemoryEncrypted;
			deviceConditionalAccessRule.PasswordTimeout = this.PasswordTimeout;
			deviceConditionalAccessRule.PasswordMinimumLength = this.PasswordMinimumLength;
			deviceConditionalAccessRule.PasswordHistoryCount = this.PasswordHistoryCount;
			deviceConditionalAccessRule.PasswordExpirationDays = this.PasswordExpirationDays;
			deviceConditionalAccessRule.PasswordMinComplexChars = this.PasswordMinComplexChars;
			deviceConditionalAccessRule.AllowSimplePassword = this.AllowSimplePassword;
			deviceConditionalAccessRule.PasswordQuality = this.PasswordQuality;
			deviceConditionalAccessRule.MaxPasswordAttemptsBeforeWipe = this.MaxPasswordAttemptsBeforeWipe;
			deviceConditionalAccessRule.EnableRemovableStorage = this.EnableRemovableStorage;
			deviceConditionalAccessRule.CameraEnabled = this.CameraEnabled;
			deviceConditionalAccessRule.BluetoothEnabled = this.BluetoothEnabled;
			deviceConditionalAccessRule.ForceEncryptedBackup = this.ForceEncryptedBackup;
			deviceConditionalAccessRule.AllowiCloudDocSync = this.AllowiCloudDocSync;
			deviceConditionalAccessRule.AllowiCloudPhotoSync = this.AllowiCloudPhotoSync;
			deviceConditionalAccessRule.AllowiCloudBackup = this.AllowiCloudBackup;
			deviceConditionalAccessRule.RegionRatings = this.RegionRatings;
			deviceConditionalAccessRule.MoviesRating = this.MoviesRating;
			deviceConditionalAccessRule.TVShowsRating = this.TVShowsRating;
			deviceConditionalAccessRule.AppsRating = this.AppsRating;
			deviceConditionalAccessRule.AllowVoiceDialing = this.AllowVoiceDialing;
			deviceConditionalAccessRule.AllowVoiceAssistant = this.AllowVoiceAssistant;
			deviceConditionalAccessRule.AllowAssistantWhileLocked = this.AllowAssistantWhileLocked;
			deviceConditionalAccessRule.AllowScreenshot = this.AllowScreenshot;
			deviceConditionalAccessRule.AllowVideoConferencing = this.AllowVideoConferencing;
			deviceConditionalAccessRule.AllowPassbookWhileLocked = this.AllowPassbookWhileLocked;
			deviceConditionalAccessRule.AllowDiagnosticSubmission = this.AllowDiagnosticSubmission;
			deviceConditionalAccessRule.AllowConvenienceLogon = this.AllowConvenienceLogon;
			deviceConditionalAccessRule.MaxPasswordGracePeriod = this.MaxPasswordGracePeriod;
			deviceConditionalAccessRule.AllowAppStore = this.AllowAppStore;
			deviceConditionalAccessRule.ForceAppStorePassword = this.ForceAppStorePassword;
			deviceConditionalAccessRule.SystemSecurityTLS = this.SystemSecurityTLS;
			deviceConditionalAccessRule.UserAccountControlStatus = this.UserAccountControlStatus;
			deviceConditionalAccessRule.FirewallStatus = this.FirewallStatus;
			deviceConditionalAccessRule.AutoUpdateStatus = this.AutoUpdateStatus;
			deviceConditionalAccessRule.AntiVirusStatus = this.AntiVirusStatus;
			deviceConditionalAccessRule.AntiVirusSignatureStatus = this.AntiVirusSignatureStatus;
			deviceConditionalAccessRule.SmartScreenEnabled = this.SmartScreenEnabled;
			deviceConditionalAccessRule.WorkFoldersSyncUrl = this.WorkFoldersSyncUrl;
			deviceConditionalAccessRule.PasswordComplexity = this.PasswordComplexity;
			deviceConditionalAccessRule.WLANEnabled = this.WLANEnabled;
			deviceConditionalAccessRule.AccountName = this.AccountName;
			deviceConditionalAccessRule.AccountUserName = this.AccountUserName;
			deviceConditionalAccessRule.ExchangeActiveSyncHost = this.ExchangeActiveSyncHost;
			deviceConditionalAccessRule.EmailAddress = this.EmailAddress;
			deviceConditionalAccessRule.UseSSL = this.UseSSL;
			deviceConditionalAccessRule.AllowMove = this.AllowMove;
			deviceConditionalAccessRule.AllowRecentAddressSyncing = this.AllowRecentAddressSyncing;
			deviceConditionalAccessRule.DaysToSync = this.DaysToSync;
			deviceConditionalAccessRule.ContentType = this.ContentType;
			deviceConditionalAccessRule.UseSMIME = this.UseSMIME;
			deviceConditionalAccessRule.SyncSchedule = this.SyncSchedule;
			deviceConditionalAccessRule.UseOnlyInEmail = this.UseOnlyInEmail;
		}
	}
}
