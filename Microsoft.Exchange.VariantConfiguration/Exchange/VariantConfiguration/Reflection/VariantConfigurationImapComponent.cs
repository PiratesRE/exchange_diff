using System;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationImapComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationImapComponent() : base("Imap")
		{
			base.Add(new VariantConfigurationSection("Imap.settings.ini", "RfcIDImap", typeof(IFeature), true));
			base.Add(new VariantConfigurationSection("Imap.settings.ini", "IgnoreNonProvisionedServers", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Imap.settings.ini", "UseSamAccountNameAsUsername", typeof(IFeature), true));
			base.Add(new VariantConfigurationSection("Imap.settings.ini", "SkipAuthOnCafe", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Imap.settings.ini", "AllowPlainTextConversionWithoutUsingSkeleton", typeof(IFeature), true));
			base.Add(new VariantConfigurationSection("Imap.settings.ini", "RfcIDImapCafe", typeof(IFeature), true));
			base.Add(new VariantConfigurationSection("Imap.settings.ini", "GlobalCriminalCompliance", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Imap.settings.ini", "CheckOnlyAuthenticationStatus", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Imap.settings.ini", "RfcMoveImap", typeof(IFeature), true));
			base.Add(new VariantConfigurationSection("Imap.settings.ini", "RefreshSearchFolderItems", typeof(IFeature), true));
			base.Add(new VariantConfigurationSection("Imap.settings.ini", "ReloadMailboxBeforeGettingSubscriptionList", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Imap.settings.ini", "EnforceLogsRetentionPolicy", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Imap.settings.ini", "AppendServerNameInBanner", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Imap.settings.ini", "UsePrimarySmtpAddress", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Imap.settings.ini", "ImapClientAccessRulesEnabled", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Imap.settings.ini", "DontReturnLastMessageForUInt32MaxValue", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Imap.settings.ini", "LrsLogging", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Imap.settings.ini", "AllowKerberosAuth", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Imap.settings.ini", "RfcMoveImapCafe", typeof(IFeature), true));
		}

		public VariantConfigurationSection RfcIDImap
		{
			get
			{
				return base["RfcIDImap"];
			}
		}

		public VariantConfigurationSection IgnoreNonProvisionedServers
		{
			get
			{
				return base["IgnoreNonProvisionedServers"];
			}
		}

		public VariantConfigurationSection UseSamAccountNameAsUsername
		{
			get
			{
				return base["UseSamAccountNameAsUsername"];
			}
		}

		public VariantConfigurationSection SkipAuthOnCafe
		{
			get
			{
				return base["SkipAuthOnCafe"];
			}
		}

		public VariantConfigurationSection AllowPlainTextConversionWithoutUsingSkeleton
		{
			get
			{
				return base["AllowPlainTextConversionWithoutUsingSkeleton"];
			}
		}

		public VariantConfigurationSection RfcIDImapCafe
		{
			get
			{
				return base["RfcIDImapCafe"];
			}
		}

		public VariantConfigurationSection GlobalCriminalCompliance
		{
			get
			{
				return base["GlobalCriminalCompliance"];
			}
		}

		public VariantConfigurationSection CheckOnlyAuthenticationStatus
		{
			get
			{
				return base["CheckOnlyAuthenticationStatus"];
			}
		}

		public VariantConfigurationSection RfcMoveImap
		{
			get
			{
				return base["RfcMoveImap"];
			}
		}

		public VariantConfigurationSection RefreshSearchFolderItems
		{
			get
			{
				return base["RefreshSearchFolderItems"];
			}
		}

		public VariantConfigurationSection ReloadMailboxBeforeGettingSubscriptionList
		{
			get
			{
				return base["ReloadMailboxBeforeGettingSubscriptionList"];
			}
		}

		public VariantConfigurationSection EnforceLogsRetentionPolicy
		{
			get
			{
				return base["EnforceLogsRetentionPolicy"];
			}
		}

		public VariantConfigurationSection AppendServerNameInBanner
		{
			get
			{
				return base["AppendServerNameInBanner"];
			}
		}

		public VariantConfigurationSection UsePrimarySmtpAddress
		{
			get
			{
				return base["UsePrimarySmtpAddress"];
			}
		}

		public VariantConfigurationSection ImapClientAccessRulesEnabled
		{
			get
			{
				return base["ImapClientAccessRulesEnabled"];
			}
		}

		public VariantConfigurationSection DontReturnLastMessageForUInt32MaxValue
		{
			get
			{
				return base["DontReturnLastMessageForUInt32MaxValue"];
			}
		}

		public VariantConfigurationSection LrsLogging
		{
			get
			{
				return base["LrsLogging"];
			}
		}

		public VariantConfigurationSection AllowKerberosAuth
		{
			get
			{
				return base["AllowKerberosAuth"];
			}
		}

		public VariantConfigurationSection RfcMoveImapCafe
		{
			get
			{
				return base["RfcMoveImapCafe"];
			}
		}
	}
}
