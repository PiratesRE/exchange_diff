using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Set", "DeviceConfigurationRule", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetDeviceConfigurationRule : SetDeviceRuleBase
	{
		public SetDeviceConfigurationRule() : base(PolicyScenario.DeviceSettings)
		{
		}

		protected override DeviceRuleBase CreateDeviceRule(RuleStorage dataObject)
		{
			return new DeviceConfigurationRule(dataObject);
		}

		protected override void ValidateUnacceptableParameter()
		{
			if (this.Identity != null && !DevicePolicyUtility.IsDeviceConfigurationRule(this.Identity.ToString()))
			{
				base.WriteError(new ArgumentException(Strings.CanOnlyManipulateDeviceConfigurationRule), ErrorCategory.InvalidArgument, null);
			}
			if (DevicePolicyUtility.IsPropertySpecified(base.DynamicParametersInstance, ADObjectSchema.Name))
			{
				base.WriteError(new ArgumentException(Strings.CannotChangeDeviceConfigurationRuleName), ErrorCategory.InvalidArgument, null);
			}
		}

		[Parameter(Mandatory = false)]
		public bool? PasswordRequired
		{
			get
			{
				return (bool?)base.Fields[DeviceConfigurationRule.Device_Password_Required];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_Password_Required] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? PhoneMemoryEncrypted
		{
			get
			{
				return (bool?)base.Fields[DeviceConfigurationRule.Device_Encryption_PhoneMemoryEncrypted];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_Encryption_PhoneMemoryEncrypted] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public TimeSpan? PasswordTimeout
		{
			get
			{
				return (TimeSpan?)base.Fields[DeviceConfigurationRule.Device_Password_Timeout];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_Password_Timeout] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? PasswordMinimumLength
		{
			get
			{
				return (int?)base.Fields[DeviceConfigurationRule.Device_Password_MinimumLength];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_Password_MinimumLength] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? PasswordHistoryCount
		{
			get
			{
				return (int?)base.Fields[DeviceConfigurationRule.Device_Password_History];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_Password_History] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? PasswordExpirationDays
		{
			get
			{
				return (int?)base.Fields[DeviceConfigurationRule.Device_Password_Expiration];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_Password_Expiration] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? MaxPasswordAttemptsBeforeWipe
		{
			get
			{
				return (int?)base.Fields[DeviceConfigurationRule.Device_Password_MaxAttemptsBeforeWipe];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_Password_MaxAttemptsBeforeWipe] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? PasswordMinComplexChars
		{
			get
			{
				return (int?)base.Fields[DeviceConfigurationRule.Device_Password_MinComplexChars];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_Password_MinComplexChars] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? AllowSimplePassword
		{
			get
			{
				return (bool?)base.Fields[DeviceConfigurationRule.Device_Password_AllowSimplePassword];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_Password_AllowSimplePassword] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? EnableRemovableStorage
		{
			get
			{
				return (bool?)base.Fields[DeviceConfigurationRule.Device_Security_EnableRemovableStorage];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_Security_EnableRemovableStorage] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? CameraEnabled
		{
			get
			{
				return (bool?)base.Fields[DeviceConfigurationRule.Device_Security_CameraEnabled];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_Security_CameraEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? BluetoothEnabled
		{
			get
			{
				return (bool?)base.Fields[DeviceConfigurationRule.Device_Security_BluetoothEnabled];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_Security_BluetoothEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? ForceEncryptedBackup
		{
			get
			{
				return (bool?)base.Fields[DeviceConfigurationRule.Device_Cloud_ForceEncryptedBackup];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_Cloud_ForceEncryptedBackup] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? AllowiCloudDocSync
		{
			get
			{
				return (bool?)base.Fields[DeviceConfigurationRule.Device_Cloud_AllowiCloudDocSync];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_Cloud_AllowiCloudDocSync] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? AllowiCloudPhotoSync
		{
			get
			{
				return (bool?)base.Fields[DeviceConfigurationRule.Device_Cloud_AllowiCloudPhotoSync];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_Cloud_AllowiCloudPhotoSync] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? AllowiCloudBackup
		{
			get
			{
				return (bool?)base.Fields[DeviceConfigurationRule.Device_Cloud_AllowiCloudBackup];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_Cloud_AllowiCloudBackup] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RatingRegionEntry? RegionRatings
		{
			get
			{
				return (RatingRegionEntry?)base.Fields[DeviceConfigurationRule.Device_Restrictions_RatingsRegion];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_Restrictions_RatingsRegion] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RatingMovieEntry? MoviesRating
		{
			get
			{
				return (RatingMovieEntry?)base.Fields[DeviceConfigurationRule.Device_Restrictions_RatingMovies];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_Restrictions_RatingMovies] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RatingTvShowEntry? TVShowsRating
		{
			get
			{
				return (RatingTvShowEntry?)base.Fields[DeviceConfigurationRule.Device_Restrictions_RatingTVShows];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_Restrictions_RatingTVShows] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RatingAppsEntry? AppsRating
		{
			get
			{
				return (RatingAppsEntry?)base.Fields[DeviceConfigurationRule.Device_Restrictions_RatingApps];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_Restrictions_RatingApps] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? AllowVoiceDialing
		{
			get
			{
				return (bool?)base.Fields[DeviceConfigurationRule.Device_Restrictions_AllowVoiceDialing];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_Restrictions_AllowVoiceDialing] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? AllowVoiceAssistant
		{
			get
			{
				return (bool?)base.Fields[DeviceConfigurationRule.Device_Restrictions_AllowVoiceAssistant];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_Restrictions_AllowVoiceAssistant] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? AllowAssistantWhileLocked
		{
			get
			{
				return (bool?)base.Fields[DeviceConfigurationRule.Device_Restrictions_AllowAssistantWhileLocked];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_Restrictions_AllowAssistantWhileLocked] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? AllowScreenshot
		{
			get
			{
				return (bool?)base.Fields[DeviceConfigurationRule.Device_Restrictions_AllowScreenshot];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_Restrictions_AllowScreenshot] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? AllowVideoConferencing
		{
			get
			{
				return (bool?)base.Fields[DeviceConfigurationRule.Device_Restrictions_AllowVideoConferencing];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_Restrictions_AllowVideoConferencing] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? AllowPassbookWhileLocked
		{
			get
			{
				return (bool?)base.Fields[DeviceConfigurationRule.Device_Restrictions_AllowPassbookWhileLocked];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_Restrictions_AllowPassbookWhileLocked] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? AllowDiagnosticSubmission
		{
			get
			{
				return (bool?)base.Fields[DeviceConfigurationRule.Device_Restrictions_AllowDiagnosticSubmission];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_Restrictions_AllowDiagnosticSubmission] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? AllowConvenienceLogon
		{
			get
			{
				return (bool?)base.Fields[DeviceConfigurationRule.Device_Password_AllowConvenienceLogon];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_Password_AllowConvenienceLogon] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public TimeSpan? MaxPasswordGracePeriod
		{
			get
			{
				return (TimeSpan?)base.Fields[DeviceConfigurationRule.Device_Password_MaxGracePeriod];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_Password_MaxGracePeriod] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? PasswordQuality
		{
			get
			{
				return (int?)base.Fields[DeviceConfigurationRule.Device_Password_PasswordQuality];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_Password_PasswordQuality] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? AllowAppStore
		{
			get
			{
				return (bool?)base.Fields[DeviceConfigurationRule.Device_Restrictions_AllowAppStore];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_Restrictions_AllowAppStore] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? ForceAppStorePassword
		{
			get
			{
				return (bool?)base.Fields[DeviceConfigurationRule.Device_Restrictions_ForceAppStorePassword];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_Restrictions_ForceAppStorePassword] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? SystemSecurityTLS
		{
			get
			{
				return (bool?)base.Fields[DeviceConfigurationRule.Device_SystemSecurity_TLS];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_SystemSecurity_TLS] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public UserAccountControlStatusEntry? UserAccountControlStatus
		{
			get
			{
				return (UserAccountControlStatusEntry?)base.Fields[DeviceConfigurationRule.Device_SystemSecurity_UserAccountControlStatus];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_SystemSecurity_UserAccountControlStatus] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public FirewallStatusEntry? FirewallStatus
		{
			get
			{
				return (FirewallStatusEntry?)base.Fields[DeviceConfigurationRule.Device_SystemSecurity_FirewallStatus];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_SystemSecurity_FirewallStatus] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public AutoUpdateStatusEntry? AutoUpdateStatus
		{
			get
			{
				return (AutoUpdateStatusEntry?)base.Fields[DeviceConfigurationRule.Device_SystemSecurity_AutoUpdateStatus];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_SystemSecurity_AutoUpdateStatus] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string AntiVirusStatus
		{
			get
			{
				return (string)base.Fields[DeviceConfigurationRule.Device_SystemSecurity_AntiVirusStatus];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_SystemSecurity_AntiVirusStatus] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? AntiVirusSignatureStatus
		{
			get
			{
				return (bool?)base.Fields[DeviceConfigurationRule.Device_SystemSecurity_AntiVirusSignatureStatus];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_SystemSecurity_AntiVirusSignatureStatus] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? SmartScreenEnabled
		{
			get
			{
				return (bool?)base.Fields[DeviceConfigurationRule.Device_InternetExplorer_SmartScreenEnabled];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_InternetExplorer_SmartScreenEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string WorkFoldersSyncUrl
		{
			get
			{
				return (string)base.Fields[DeviceConfigurationRule.Device_WorkFolders_SyncUrl];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_WorkFolders_SyncUrl] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string PasswordComplexity
		{
			get
			{
				return (string)base.Fields[DeviceConfigurationRule.Device_Password_Type];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_Password_Type] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? WLANEnabled
		{
			get
			{
				return (bool?)base.Fields[DeviceConfigurationRule.Device_Wireless_WLANEnabled];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Device_Wireless_WLANEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string AccountName
		{
			get
			{
				return (string)base.Fields[DeviceConfigurationRule.Eas_AccountName];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Eas_AccountName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string AccountUserName
		{
			get
			{
				return (string)base.Fields[DeviceConfigurationRule.Eas_UserName];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Eas_UserName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string ExchangeActiveSyncHost
		{
			get
			{
				return (string)base.Fields[DeviceConfigurationRule.Eas_Host];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Eas_Host] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string EmailAddress
		{
			get
			{
				return (string)base.Fields[DeviceConfigurationRule.Eas_EmailAddress];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Eas_EmailAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? UseSSL
		{
			get
			{
				return (bool?)base.Fields[DeviceConfigurationRule.Eas_UseSSL];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Eas_UseSSL] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? AllowMove
		{
			get
			{
				return (bool?)base.Fields[DeviceConfigurationRule.Eas_PreventAppSheet];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Eas_PreventAppSheet] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? AllowRecentAddressSyncing
		{
			get
			{
				return (bool?)base.Fields[DeviceConfigurationRule.Eas_DisableMailRecentsSyncing];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Eas_DisableMailRecentsSyncing] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public long? DaysToSync
		{
			get
			{
				return (long?)base.Fields[DeviceConfigurationRule.Eas_MailNumberOfPastDaysToSync];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Eas_MailNumberOfPastDaysToSync] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public long? ContentType
		{
			get
			{
				return (long?)base.Fields[DeviceConfigurationRule.Eas_ContentTypeToSync];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Eas_ContentTypeToSync] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? UseSMIME
		{
			get
			{
				return (bool?)base.Fields[DeviceConfigurationRule.Eas_UseSMIME];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Eas_UseSMIME] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public long? SyncSchedule
		{
			get
			{
				return (long?)base.Fields[DeviceConfigurationRule.Eas_SyncSchedule];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Eas_SyncSchedule] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? UseOnlyInEmail
		{
			get
			{
				return (bool?)base.Fields[DeviceConfigurationRule.Eas_PreventMove];
			}
			set
			{
				base.Fields[DeviceConfigurationRule.Eas_PreventMove] = value;
			}
		}

		protected override void SetPropsOnDeviceRule(DeviceRuleBase pdeviceRule)
		{
			DeviceConfigurationRule deviceConfigurationRule = (DeviceConfigurationRule)pdeviceRule;
			deviceConfigurationRule.PasswordRequired = this.PasswordRequired;
			deviceConfigurationRule.PhoneMemoryEncrypted = this.PhoneMemoryEncrypted;
			deviceConfigurationRule.PasswordTimeout = this.PasswordTimeout;
			deviceConfigurationRule.PasswordMinimumLength = this.PasswordMinimumLength;
			deviceConfigurationRule.PasswordHistoryCount = this.PasswordHistoryCount;
			deviceConfigurationRule.PasswordExpirationDays = this.PasswordExpirationDays;
			deviceConfigurationRule.MaxPasswordAttemptsBeforeWipe = this.MaxPasswordAttemptsBeforeWipe;
			deviceConfigurationRule.PasswordMinComplexChars = this.PasswordMinComplexChars;
			deviceConfigurationRule.AllowSimplePassword = this.AllowSimplePassword;
			deviceConfigurationRule.EnableRemovableStorage = this.EnableRemovableStorage;
			deviceConfigurationRule.CameraEnabled = this.CameraEnabled;
			deviceConfigurationRule.BluetoothEnabled = this.BluetoothEnabled;
			deviceConfigurationRule.ForceEncryptedBackup = this.ForceEncryptedBackup;
			deviceConfigurationRule.AllowiCloudDocSync = this.AllowiCloudDocSync;
			deviceConfigurationRule.AllowiCloudPhotoSync = this.AllowiCloudPhotoSync;
			deviceConfigurationRule.AllowiCloudBackup = this.AllowiCloudBackup;
			deviceConfigurationRule.RegionRatings = this.RegionRatings;
			deviceConfigurationRule.MoviesRating = this.MoviesRating;
			deviceConfigurationRule.TVShowsRating = this.TVShowsRating;
			deviceConfigurationRule.AppsRating = this.AppsRating;
			deviceConfigurationRule.AllowVoiceDialing = this.AllowVoiceDialing;
			deviceConfigurationRule.AllowVoiceAssistant = this.AllowVoiceAssistant;
			deviceConfigurationRule.AllowAssistantWhileLocked = this.AllowAssistantWhileLocked;
			deviceConfigurationRule.AllowScreenshot = this.AllowScreenshot;
			deviceConfigurationRule.AllowVideoConferencing = this.AllowVideoConferencing;
			deviceConfigurationRule.AllowPassbookWhileLocked = this.AllowPassbookWhileLocked;
			deviceConfigurationRule.AllowDiagnosticSubmission = this.AllowDiagnosticSubmission;
			deviceConfigurationRule.AllowConvenienceLogon = this.AllowConvenienceLogon;
			deviceConfigurationRule.MaxPasswordGracePeriod = this.MaxPasswordGracePeriod;
			deviceConfigurationRule.PasswordQuality = this.PasswordQuality;
			deviceConfigurationRule.AllowAppStore = this.AllowAppStore;
			deviceConfigurationRule.ForceAppStorePassword = this.ForceAppStorePassword;
			deviceConfigurationRule.SystemSecurityTLS = this.SystemSecurityTLS;
			deviceConfigurationRule.UserAccountControlStatus = this.UserAccountControlStatus;
			deviceConfigurationRule.FirewallStatus = this.FirewallStatus;
			deviceConfigurationRule.AutoUpdateStatus = this.AutoUpdateStatus;
			deviceConfigurationRule.AntiVirusStatus = this.AntiVirusStatus;
			deviceConfigurationRule.AntiVirusSignatureStatus = this.AntiVirusSignatureStatus;
			deviceConfigurationRule.SmartScreenEnabled = this.SmartScreenEnabled;
			deviceConfigurationRule.WorkFoldersSyncUrl = this.WorkFoldersSyncUrl;
			deviceConfigurationRule.PasswordComplexity = this.PasswordComplexity;
			deviceConfigurationRule.WLANEnabled = this.WLANEnabled;
			deviceConfigurationRule.AccountName = this.AccountName;
			deviceConfigurationRule.AccountUserName = this.AccountUserName;
			deviceConfigurationRule.ExchangeActiveSyncHost = this.ExchangeActiveSyncHost;
			deviceConfigurationRule.EmailAddress = this.EmailAddress;
			deviceConfigurationRule.UseSSL = this.UseSSL;
			deviceConfigurationRule.AllowMove = this.AllowMove;
			deviceConfigurationRule.AllowRecentAddressSyncing = this.AllowRecentAddressSyncing;
			deviceConfigurationRule.DaysToSync = this.DaysToSync;
			deviceConfigurationRule.ContentType = this.ContentType;
			deviceConfigurationRule.UseSMIME = this.UseSMIME;
			deviceConfigurationRule.SyncSchedule = this.SyncSchedule;
			deviceConfigurationRule.UseOnlyInEmail = this.UseOnlyInEmail;
		}
	}
}
