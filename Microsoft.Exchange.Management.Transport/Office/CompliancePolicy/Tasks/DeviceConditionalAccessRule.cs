using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;
using Microsoft.Office.CompliancePolicy.PolicyEvaluation;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Serializable]
	public sealed class DeviceConditionalAccessRule : DeviceRuleBase
	{
		public DeviceConditionalAccessRule()
		{
		}

		public DeviceConditionalAccessRule(RuleStorage ruleStorage) : base(ruleStorage)
		{
		}

		public bool? AllowJailbroken { get; set; }

		public bool? PasswordRequired { get; set; }

		public bool? PhoneMemoryEncrypted { get; set; }

		public TimeSpan? PasswordTimeout { get; set; }

		public int? PasswordMinimumLength { get; set; }

		public int? PasswordHistoryCount { get; set; }

		public int? PasswordExpirationDays { get; set; }

		public int? PasswordMinComplexChars { get; set; }

		public bool? AllowSimplePassword { get; set; }

		public int? PasswordQuality { get; set; }

		public int? MaxPasswordAttemptsBeforeWipe { get; set; }

		public bool? EnableRemovableStorage { get; set; }

		public bool? CameraEnabled { get; set; }

		public bool? BluetoothEnabled { get; set; }

		public bool? ForceEncryptedBackup { get; set; }

		public bool? AllowiCloudDocSync { get; set; }

		public bool? AllowiCloudPhotoSync { get; set; }

		public bool? AllowiCloudBackup { get; set; }

		public CARatingRegionEntry? RegionRatings { get; set; }

		public CARatingMovieEntry? MoviesRating { get; set; }

		public CARatingTvShowEntry? TVShowsRating { get; set; }

		public CARatingAppsEntry? AppsRating { get; set; }

		public bool? AllowVoiceDialing { get; set; }

		public bool? AllowVoiceAssistant { get; set; }

		public bool? AllowAssistantWhileLocked { get; set; }

		public bool? AllowScreenshot { get; set; }

		public bool? AllowVideoConferencing { get; set; }

		public bool? AllowPassbookWhileLocked { get; set; }

		public bool? AllowDiagnosticSubmission { get; set; }

		public bool? AllowConvenienceLogon { get; set; }

		public TimeSpan? MaxPasswordGracePeriod { get; set; }

		public bool? AllowAppStore { get; set; }

		public bool? ForceAppStorePassword { get; set; }

		public bool? SystemSecurityTLS { get; set; }

		public CAUserAccountControlStatusEntry? UserAccountControlStatus { get; set; }

		public CAFirewallStatusEntry? FirewallStatus { get; set; }

		public CAAutoUpdateStatusEntry? AutoUpdateStatus { get; set; }

		public string AntiVirusStatus { get; set; }

		public bool? AntiVirusSignatureStatus { get; set; }

		public bool? SmartScreenEnabled { get; set; }

		public string WorkFoldersSyncUrl { get; set; }

		public string PasswordComplexity { get; set; }

		public bool? WLANEnabled { get; set; }

		public string AccountName { get; set; }

		public string AccountUserName { get; set; }

		public string ExchangeActiveSyncHost { get; set; }

		public string EmailAddress { get; set; }

		public bool? UseSSL { get; set; }

		public bool? AllowMove { get; set; }

		public bool? AllowRecentAddressSyncing { get; set; }

		public long? DaysToSync { get; set; }

		public long? ContentType { get; set; }

		public bool? UseSMIME { get; set; }

		public long? SyncSchedule { get; set; }

		public bool? UseOnlyInEmail { get; set; }

		protected override IEnumerable<Condition> GetTaskConditions()
		{
			List<Condition> list = new List<Condition>();
			if (base.TargetGroups != null)
			{
				List<string> list2 = new List<string>();
				foreach (Guid guid in base.TargetGroups)
				{
					list2.Add(guid.ToString());
				}
				list.Add(new IsPredicate(Property.CreateProperty("isMemberOf", typeof(Guid).ToString()), list2));
			}
			if (this.AllowJailbroken != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_Security_Jailbroken, typeof(string).ToString()), new List<string>
				{
					this.AllowJailbroken.ToString()
				}));
			}
			if (this.PasswordRequired != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_Password_Required, typeof(string).ToString()), new List<string>
				{
					this.PasswordRequired.ToString()
				}));
			}
			if (this.PhoneMemoryEncrypted != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_Encryption_PhoneMemoryEncrypted, typeof(string).ToString()), new List<string>
				{
					this.PhoneMemoryEncrypted.ToString()
				}));
			}
			if (this.PasswordTimeout != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_Password_Timeout, typeof(string).ToString()), new List<string>
				{
					this.PasswordTimeout.GetValueOrDefault().Subtract(TimeSpan.FromSeconds((double)this.PasswordTimeout.GetValueOrDefault().Seconds)).TotalMinutes.ToString()
				}));
			}
			if (this.PasswordMinimumLength != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_Password_MinimumLength, typeof(string).ToString()), new List<string>
				{
					this.PasswordMinimumLength.ToString()
				}));
			}
			if (this.PasswordHistoryCount != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_Password_History, typeof(string).ToString()), new List<string>
				{
					this.PasswordHistoryCount.ToString()
				}));
			}
			if (this.PasswordExpirationDays != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_Password_Expiration, typeof(string).ToString()), new List<string>
				{
					this.PasswordExpirationDays.ToString()
				}));
			}
			if (this.PasswordMinComplexChars != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_Password_MinComplexChars, typeof(string).ToString()), new List<string>
				{
					this.PasswordMinComplexChars.ToString()
				}));
			}
			if (this.AllowSimplePassword != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_Password_AllowSimplePassword, typeof(string).ToString()), new List<string>
				{
					this.AllowSimplePassword.ToString()
				}));
			}
			if (this.PasswordQuality != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_Password_PasswordQuality, typeof(string).ToString()), new List<string>
				{
					this.PasswordQuality.ToString()
				}));
			}
			if (this.MaxPasswordAttemptsBeforeWipe != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_Password_MaxAttemptsBeforeWipe, typeof(string).ToString()), new List<string>
				{
					this.MaxPasswordAttemptsBeforeWipe.ToString()
				}));
			}
			if (this.EnableRemovableStorage != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_Security_EnableRemovableStorage, typeof(string).ToString()), new List<string>
				{
					this.EnableRemovableStorage.ToString()
				}));
			}
			if (this.CameraEnabled != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_Security_CameraEnabled, typeof(string).ToString()), new List<string>
				{
					this.CameraEnabled.ToString()
				}));
			}
			if (this.BluetoothEnabled != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_Security_BluetoothEnabled, typeof(string).ToString()), new List<string>
				{
					this.BluetoothEnabled.ToString()
				}));
			}
			if (this.ForceEncryptedBackup != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_Cloud_ForceEncryptedBackup, typeof(string).ToString()), new List<string>
				{
					this.ForceEncryptedBackup.ToString()
				}));
			}
			if (this.AllowiCloudDocSync != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_Cloud_AllowiCloudDocSync, typeof(string).ToString()), new List<string>
				{
					this.AllowiCloudDocSync.ToString()
				}));
			}
			if (this.AllowiCloudPhotoSync != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_Cloud_AllowiCloudPhotoSync, typeof(string).ToString()), new List<string>
				{
					this.AllowiCloudPhotoSync.ToString()
				}));
			}
			if (this.AllowiCloudBackup != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_Cloud_AllowiCloudBackup, typeof(string).ToString()), new List<string>
				{
					this.AllowiCloudBackup.ToString()
				}));
			}
			if (this.RegionRatings != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_Restrictions_RatingsRegion, typeof(string).ToString()), new List<string>
				{
					this.RegionRatings.ToString()
				}));
			}
			if (this.MoviesRating != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_Restrictions_RatingMovies, typeof(string).ToString()), new List<string>
				{
					((int)this.MoviesRating.Value).ToString()
				}));
			}
			if (this.TVShowsRating != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_Restrictions_RatingTVShows, typeof(string).ToString()), new List<string>
				{
					((int)this.TVShowsRating.Value).ToString()
				}));
			}
			if (this.AppsRating != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_Restrictions_RatingApps, typeof(string).ToString()), new List<string>
				{
					((int)this.AppsRating.Value).ToString()
				}));
			}
			if (this.AllowVoiceDialing != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_Restrictions_AllowVoiceDialing, typeof(string).ToString()), new List<string>
				{
					this.AllowVoiceDialing.ToString()
				}));
			}
			if (this.AllowVoiceAssistant != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_Restrictions_AllowVoiceAssistant, typeof(string).ToString()), new List<string>
				{
					this.AllowVoiceAssistant.ToString()
				}));
			}
			if (this.AllowAssistantWhileLocked != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_Restrictions_AllowAssistantWhileLocked, typeof(string).ToString()), new List<string>
				{
					this.AllowAssistantWhileLocked.ToString()
				}));
			}
			if (this.AllowScreenshot != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_Restrictions_AllowScreenshot, typeof(string).ToString()), new List<string>
				{
					this.AllowScreenshot.ToString()
				}));
			}
			if (this.AllowVideoConferencing != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_Restrictions_AllowVideoConferencing, typeof(string).ToString()), new List<string>
				{
					this.AllowVideoConferencing.ToString()
				}));
			}
			if (this.AllowPassbookWhileLocked != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_Restrictions_AllowPassbookWhileLocked, typeof(string).ToString()), new List<string>
				{
					this.AllowPassbookWhileLocked.ToString()
				}));
			}
			if (this.AllowDiagnosticSubmission != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_Restrictions_AllowDiagnosticSubmission, typeof(string).ToString()), new List<string>
				{
					this.AllowDiagnosticSubmission.ToString()
				}));
			}
			if (this.AllowConvenienceLogon != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_Password_AllowConvenienceLogon, typeof(string).ToString()), new List<string>
				{
					this.AllowConvenienceLogon.ToString()
				}));
			}
			if (this.MaxPasswordGracePeriod != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_Password_MaxGracePeriod, typeof(string).ToString()), new List<string>
				{
					this.MaxPasswordGracePeriod.GetValueOrDefault().Subtract(TimeSpan.FromSeconds((double)this.MaxPasswordGracePeriod.GetValueOrDefault().Seconds)).TotalMinutes.ToString()
				}));
			}
			if (this.AllowAppStore != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_Restrictions_AllowAppStore, typeof(string).ToString()), new List<string>
				{
					this.AllowAppStore.ToString()
				}));
			}
			if (this.ForceAppStorePassword != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_Restrictions_ForceAppStorePassword, typeof(string).ToString()), new List<string>
				{
					this.ForceAppStorePassword.ToString()
				}));
			}
			if (this.SystemSecurityTLS != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_SystemSecurity_TLS, typeof(string).ToString()), new List<string>
				{
					this.SystemSecurityTLS.ToString()
				}));
			}
			if (this.UserAccountControlStatus != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_SystemSecurity_UserAccountControlStatus, typeof(string).ToString()), new List<string>
				{
					((int)this.UserAccountControlStatus.Value).ToString()
				}));
			}
			if (this.FirewallStatus != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_SystemSecurity_FirewallStatus, typeof(string).ToString()), new List<string>
				{
					((int)this.FirewallStatus.Value).ToString()
				}));
			}
			if (this.AutoUpdateStatus != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_SystemSecurity_AutoUpdateStatus, typeof(string).ToString()), new List<string>
				{
					((int)this.AutoUpdateStatus.Value).ToString()
				}));
			}
			if (this.AntiVirusStatus != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_SystemSecurity_AntiVirusStatus, typeof(string).ToString()), new List<string>
				{
					this.AntiVirusStatus.ToString()
				}));
			}
			if (this.AntiVirusSignatureStatus != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_SystemSecurity_AntiVirusSignatureStatus, typeof(string).ToString()), new List<string>
				{
					this.AntiVirusSignatureStatus.ToString()
				}));
			}
			if (this.SmartScreenEnabled != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_InternetExplorer_SmartScreenEnabled, typeof(string).ToString()), new List<string>
				{
					this.SmartScreenEnabled.ToString()
				}));
			}
			if (this.WorkFoldersSyncUrl != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_WorkFolders_SyncUrl, typeof(string).ToString()), new List<string>
				{
					this.WorkFoldersSyncUrl.ToString()
				}));
			}
			if (this.PasswordComplexity != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_Password_Type, typeof(string).ToString()), new List<string>
				{
					this.PasswordComplexity.ToString()
				}));
			}
			if (this.WLANEnabled != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Device_Wireless_WLANEnabled, typeof(string).ToString()), new List<string>
				{
					this.WLANEnabled.ToString()
				}));
			}
			if (this.AccountName != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Eas_AccountName, typeof(string).ToString()), new List<string>
				{
					this.AccountName.ToString()
				}));
			}
			if (this.AccountUserName != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Eas_UserName, typeof(string).ToString()), new List<string>
				{
					this.AccountUserName.ToString()
				}));
			}
			if (this.ExchangeActiveSyncHost != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Eas_Host, typeof(string).ToString()), new List<string>
				{
					this.ExchangeActiveSyncHost.ToString()
				}));
			}
			if (this.EmailAddress != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Eas_EmailAddress, typeof(string).ToString()), new List<string>
				{
					this.EmailAddress.ToString()
				}));
			}
			if (this.UseSSL != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Eas_UseSSL, typeof(string).ToString()), new List<string>
				{
					this.UseSSL.ToString()
				}));
			}
			if (this.AllowMove != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Eas_PreventAppSheet, typeof(string).ToString()), new List<string>
				{
					this.AllowMove.ToString()
				}));
			}
			if (this.AllowRecentAddressSyncing != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Eas_DisableMailRecentsSyncing, typeof(string).ToString()), new List<string>
				{
					this.AllowRecentAddressSyncing.ToString()
				}));
			}
			if (this.DaysToSync != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Eas_MailNumberOfPastDaysToSync, typeof(string).ToString()), new List<string>
				{
					this.DaysToSync.ToString()
				}));
			}
			if (this.ContentType != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Eas_ContentTypeToSync, typeof(string).ToString()), new List<string>
				{
					this.ContentType.ToString()
				}));
			}
			if (this.UseSMIME != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Eas_UseSMIME, typeof(string).ToString()), new List<string>
				{
					this.UseSMIME.ToString()
				}));
			}
			if (this.SyncSchedule != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Eas_SyncSchedule, typeof(string).ToString()), new List<string>
				{
					this.SyncSchedule.ToString()
				}));
			}
			if (this.UseOnlyInEmail != null)
			{
				list.Add(new NameValuesPairConfigurationPredicate(Property.CreateProperty(DeviceConditionalAccessRule.Eas_PreventMove, typeof(string).ToString()), new List<string>
				{
					this.UseOnlyInEmail.ToString()
				}));
			}
			return list;
		}

		protected override void SetTaskConditions(IEnumerable<Condition> conditions)
		{
			foreach (Condition condition in conditions)
			{
				if (condition.GetType() == typeof(NameValuesPairConfigurationPredicate) || condition.GetType() == typeof(IsPredicate))
				{
					IsPredicate isPredicate = condition as IsPredicate;
					if (isPredicate != null)
					{
						MultiValuedProperty<Guid> multiValuedProperty = new MultiValuedProperty<Guid>();
						if (isPredicate.Property.Name.Equals("isMemberOf"))
						{
							if (isPredicate.Value.ParsedValue is Guid)
							{
								multiValuedProperty.Add(isPredicate.Value.ParsedValue);
							}
							if (isPredicate.Value.ParsedValue is List<Guid>)
							{
								foreach (string item in ((List<string>)isPredicate.Value.ParsedValue))
								{
									multiValuedProperty.Add(item);
								}
							}
							base.TargetGroups = multiValuedProperty;
						}
					}
					else
					{
						NameValuesPairConfigurationPredicate nameValuesPairConfigurationPredicate = condition as NameValuesPairConfigurationPredicate;
						if (nameValuesPairConfigurationPredicate != null)
						{
							bool value;
							if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_Security_Jailbroken))
							{
								if (bool.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value))
								{
									this.AllowJailbroken = new bool?(value);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_Password_Required))
							{
								if (bool.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value))
								{
									this.PasswordRequired = new bool?(value);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_Encryption_PhoneMemoryEncrypted))
							{
								if (bool.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value))
								{
									this.PhoneMemoryEncrypted = new bool?(value);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_Password_Timeout))
							{
								int num;
								if (int.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out num))
								{
									this.PasswordTimeout = new TimeSpan?(TimeSpan.FromMinutes((double)num));
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_Password_MinimumLength))
							{
								int num;
								if (int.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out num))
								{
									this.PasswordMinimumLength = new int?(num);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_Password_History))
							{
								int num;
								if (int.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out num))
								{
									this.PasswordHistoryCount = new int?(num);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_Password_Expiration))
							{
								int num;
								if (int.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out num))
								{
									this.PasswordExpirationDays = new int?(num);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_Password_MinComplexChars))
							{
								int num;
								if (int.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out num))
								{
									this.PasswordMinComplexChars = new int?(num);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_Password_AllowSimplePassword))
							{
								if (bool.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value))
								{
									this.AllowSimplePassword = new bool?(value);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_Password_PasswordQuality))
							{
								int num;
								if (int.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out num))
								{
									this.PasswordQuality = new int?(num);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_Password_MaxAttemptsBeforeWipe))
							{
								int num;
								if (int.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out num))
								{
									this.MaxPasswordAttemptsBeforeWipe = new int?(num);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_Security_EnableRemovableStorage))
							{
								if (bool.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value))
								{
									this.EnableRemovableStorage = new bool?(value);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_Security_CameraEnabled))
							{
								if (bool.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value))
								{
									this.CameraEnabled = new bool?(value);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_Security_BluetoothEnabled))
							{
								if (bool.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value))
								{
									this.BluetoothEnabled = new bool?(value);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_Cloud_ForceEncryptedBackup))
							{
								if (bool.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value))
								{
									this.ForceEncryptedBackup = new bool?(value);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_Cloud_AllowiCloudDocSync))
							{
								if (bool.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value))
								{
									this.AllowiCloudDocSync = new bool?(value);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_Cloud_AllowiCloudPhotoSync))
							{
								if (bool.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value))
								{
									this.AllowiCloudPhotoSync = new bool?(value);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_Cloud_AllowiCloudBackup))
							{
								if (bool.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value))
								{
									this.AllowiCloudBackup = new bool?(value);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_Restrictions_RatingsRegion))
							{
								CARatingRegionEntry value2;
								if (Enum.TryParse<CARatingRegionEntry>(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value2))
								{
									this.RegionRatings = new CARatingRegionEntry?(value2);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_Restrictions_RatingMovies))
							{
								CARatingMovieEntry value3;
								if (Enum.TryParse<CARatingMovieEntry>(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value3))
								{
									this.MoviesRating = new CARatingMovieEntry?(value3);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_Restrictions_RatingTVShows))
							{
								CARatingTvShowEntry value4;
								if (Enum.TryParse<CARatingTvShowEntry>(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value4))
								{
									this.TVShowsRating = new CARatingTvShowEntry?(value4);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_Restrictions_RatingApps))
							{
								CARatingAppsEntry value5;
								if (Enum.TryParse<CARatingAppsEntry>(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value5))
								{
									this.AppsRating = new CARatingAppsEntry?(value5);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_Restrictions_AllowVoiceDialing))
							{
								if (bool.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value))
								{
									this.AllowVoiceDialing = new bool?(value);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_Restrictions_AllowVoiceAssistant))
							{
								if (bool.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value))
								{
									this.AllowVoiceAssistant = new bool?(value);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_Restrictions_AllowAssistantWhileLocked))
							{
								if (bool.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value))
								{
									this.AllowAssistantWhileLocked = new bool?(value);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_Restrictions_AllowScreenshot))
							{
								if (bool.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value))
								{
									this.AllowScreenshot = new bool?(value);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_Restrictions_AllowVideoConferencing))
							{
								if (bool.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value))
								{
									this.AllowVideoConferencing = new bool?(value);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_Restrictions_AllowPassbookWhileLocked))
							{
								if (bool.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value))
								{
									this.AllowPassbookWhileLocked = new bool?(value);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_Restrictions_AllowDiagnosticSubmission))
							{
								if (bool.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value))
								{
									this.AllowDiagnosticSubmission = new bool?(value);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_Password_AllowConvenienceLogon))
							{
								if (bool.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value))
								{
									this.AllowConvenienceLogon = new bool?(value);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_Password_MaxGracePeriod))
							{
								int num;
								if (int.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out num))
								{
									this.MaxPasswordGracePeriod = new TimeSpan?(TimeSpan.FromMinutes((double)num));
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_Restrictions_AllowAppStore))
							{
								if (bool.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value))
								{
									this.AllowAppStore = new bool?(value);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_Restrictions_ForceAppStorePassword))
							{
								if (bool.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value))
								{
									this.ForceAppStorePassword = new bool?(value);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_SystemSecurity_TLS))
							{
								if (bool.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value))
								{
									this.SystemSecurityTLS = new bool?(value);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_SystemSecurity_UserAccountControlStatus))
							{
								CAUserAccountControlStatusEntry value6;
								if (Enum.TryParse<CAUserAccountControlStatusEntry>(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value6))
								{
									this.UserAccountControlStatus = new CAUserAccountControlStatusEntry?(value6);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_SystemSecurity_FirewallStatus))
							{
								CAFirewallStatusEntry value7;
								if (Enum.TryParse<CAFirewallStatusEntry>(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value7))
								{
									this.FirewallStatus = new CAFirewallStatusEntry?(value7);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_SystemSecurity_AutoUpdateStatus))
							{
								CAAutoUpdateStatusEntry value8;
								if (Enum.TryParse<CAAutoUpdateStatusEntry>(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value8))
								{
									this.AutoUpdateStatus = new CAAutoUpdateStatusEntry?(value8);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_SystemSecurity_AntiVirusStatus))
							{
								this.AntiVirusStatus = nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>();
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_SystemSecurity_AntiVirusSignatureStatus))
							{
								if (bool.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value))
								{
									this.AntiVirusSignatureStatus = new bool?(value);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_InternetExplorer_SmartScreenEnabled))
							{
								if (bool.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value))
								{
									this.SmartScreenEnabled = new bool?(value);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_WorkFolders_SyncUrl))
							{
								this.WorkFoldersSyncUrl = nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>();
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_Password_Type))
							{
								this.PasswordComplexity = nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>();
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Device_Wireless_WLANEnabled))
							{
								if (bool.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value))
								{
									this.WLANEnabled = new bool?(value);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Eas_AccountName))
							{
								this.AccountName = nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>();
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Eas_UserName))
							{
								this.AccountUserName = nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>();
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Eas_Host))
							{
								this.ExchangeActiveSyncHost = nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>();
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Eas_EmailAddress))
							{
								this.EmailAddress = nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>();
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Eas_UseSSL))
							{
								if (bool.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value))
								{
									this.UseSSL = new bool?(value);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Eas_PreventAppSheet))
							{
								if (bool.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value))
								{
									this.AllowMove = new bool?(value);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Eas_DisableMailRecentsSyncing))
							{
								if (bool.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value))
								{
									this.AllowRecentAddressSyncing = new bool?(value);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Eas_MailNumberOfPastDaysToSync))
							{
								long value9;
								if (long.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value9))
								{
									this.DaysToSync = new long?(value9);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Eas_ContentTypeToSync))
							{
								long value9;
								if (long.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value9))
								{
									this.ContentType = new long?(value9);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Eas_UseSMIME))
							{
								if (bool.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value))
								{
									this.UseSMIME = new bool?(value);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Eas_SyncSchedule))
							{
								long value9;
								if (long.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value9))
								{
									this.SyncSchedule = new long?(value9);
								}
							}
							else if (nameValuesPairConfigurationPredicate.Property.Name.Equals(DeviceConditionalAccessRule.Eas_PreventMove) && bool.TryParse(nameValuesPairConfigurationPredicate.Value.RawValues.FirstOrDefault<string>(), out value))
							{
								this.UseOnlyInEmail = new bool?(value);
							}
						}
					}
				}
			}
		}

		public static string Device_Security_Jailbroken = "Device_Security_Jailbroken";

		public static string Device_Password_Required = "Device_Password_Required";

		public static string Device_Encryption_PhoneMemoryEncrypted = "Device_Encryption_PhoneMemoryEncrypted";

		public static string Device_Password_Timeout = "Device_Password_Timeout";

		public static string Device_Password_MinimumLength = "Device_Password_MinimumLength";

		public static string Device_Password_History = "Device_Password_History";

		public static string Device_Password_Expiration = "Device_Password_Expiration";

		public static string Device_Password_MinComplexChars = "Device_Password_MinComplexChars";

		public static string Device_Password_AllowSimplePassword = "Device_Password_AllowSimplePassword";

		public static string Device_Password_PasswordQuality = "Device_Password_PasswordQuality";

		public static string Device_Password_MaxAttemptsBeforeWipe = "Device_Password_MaxAttemptsBeforeWipe";

		public static string Device_Security_EnableRemovableStorage = "Device_Security_EnableRemovableStorage";

		public static string Device_Security_CameraEnabled = "Device_Security_CameraEnabled";

		public static string Device_Security_BluetoothEnabled = "Device_Security_BluetoothEnabled";

		public static string Device_Cloud_ForceEncryptedBackup = "Device_Cloud_ForceEncryptedBackup";

		public static string Device_Cloud_AllowiCloudDocSync = "Device_Cloud_AllowiCloudDocSync";

		public static string Device_Cloud_AllowiCloudPhotoSync = "Device_Cloud_AllowiCloudPhotoSync";

		public static string Device_Cloud_AllowiCloudBackup = "Device_Cloud_AllowiCloudBackup";

		public static string Device_Restrictions_RatingsRegion = "Device_Restrictions_RatingsRegion";

		public static string Device_Restrictions_RatingMovies = "Device_Restrictions_RatingMovies";

		public static string Device_Restrictions_RatingTVShows = "Device_Restrictions_RatingTVShows";

		public static string Device_Restrictions_RatingApps = "Device_Restrictions_RatingApps";

		public static string Device_Restrictions_AllowVoiceDialing = "Device_Restrictions_AllowVoiceDialing";

		public static string Device_Restrictions_AllowVoiceAssistant = "Device_Restrictions_AllowVoiceAssistant";

		public static string Device_Restrictions_AllowAssistantWhileLocked = "Device_Restrictions_AllowAssistantWhileLocked";

		public static string Device_Restrictions_AllowScreenshot = "Device_Restrictions_AllowScreenshot";

		public static string Device_Restrictions_AllowVideoConferencing = "Device_Restrictions_AllowVideoConferencing";

		public static string Device_Restrictions_AllowPassbookWhileLocked = "Device_Restrictions_AllowPassbookWhileLocked";

		public static string Device_Restrictions_AllowDiagnosticSubmission = "Device_Restrictions_AllowDiagnosticSubmission";

		public static string Device_Password_AllowConvenienceLogon = "Device_Password_AllowConvenienceLogon";

		public static string Device_Password_MaxGracePeriod = "Device_Password_MaxGracePeriod";

		public static string Device_Restrictions_AllowAppStore = "Device_Restrictions_AllowAppStore";

		public static string Device_Restrictions_ForceAppStorePassword = "Device_Restrictions_ForceAppStorePassword";

		public static string Device_SystemSecurity_TLS = "Device_SystemSecurity_TLS";

		public static string Device_SystemSecurity_UserAccountControlStatus = "Device_SystemSecurity_UserAccountControlStatus";

		public static string Device_SystemSecurity_FirewallStatus = "Device_SystemSecurity_FirewallStatus";

		public static string Device_SystemSecurity_AutoUpdateStatus = "Device_SystemSecurity_AutoUpdateStatus";

		public static string Device_SystemSecurity_AntiVirusStatus = "Device_SystemSecurity_AntiVirusStatus";

		public static string Device_SystemSecurity_AntiVirusSignatureStatus = "Device_SystemSecurity_AntiVirusSignatureStatus";

		public static string Device_InternetExplorer_SmartScreenEnabled = "Device_InternetExplorer_SmartScreenEnabled";

		public static string Device_WorkFolders_SyncUrl = "Device_WorkFolders_SyncUrl";

		public static string Device_Password_Type = "Device_Password_Type";

		public static string Device_Wireless_WLANEnabled = "Device_Wireless_WLANEnabled";

		public static string Eas_AccountName = "Eas_AccountName";

		public static string Eas_UserName = "Eas_UserName";

		public static string Eas_Host = "Eas_Host";

		public static string Eas_EmailAddress = "Eas_EmailAddress";

		public static string Eas_UseSSL = "Eas_UseSSL";

		public static string Eas_PreventAppSheet = "Eas_PreventAppSheet";

		public static string Eas_DisableMailRecentsSyncing = "Eas_DisableMailRecentsSyncing";

		public static string Eas_MailNumberOfPastDaysToSync = "Eas_MailNumberOfPastDaysToSync";

		public static string Eas_ContentTypeToSync = "Eas_ContentTypeToSync";

		public static string Eas_UseSMIME = "Eas_UseSMIME";

		public static string Eas_SyncSchedule = "Eas_SyncSchedule";

		public static string Eas_PreventMove = "Eas_PreventMove";
	}
}
