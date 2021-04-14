using System;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationPopComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationPopComponent() : base("Pop")
		{
			base.Add(new VariantConfigurationSection("Pop.settings.ini", "PopClientAccessRulesEnabled", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Pop.settings.ini", "IgnoreNonProvisionedServers", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Pop.settings.ini", "UseSamAccountNameAsUsername", typeof(IFeature), true));
			base.Add(new VariantConfigurationSection("Pop.settings.ini", "SkipAuthOnCafe", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Pop.settings.ini", "GlobalCriminalCompliance", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Pop.settings.ini", "CheckOnlyAuthenticationStatus", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Pop.settings.ini", "EnforceLogsRetentionPolicy", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Pop.settings.ini", "AppendServerNameInBanner", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Pop.settings.ini", "UsePrimarySmtpAddress", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Pop.settings.ini", "LrsLogging", typeof(IFeature), false));
		}

		public VariantConfigurationSection PopClientAccessRulesEnabled
		{
			get
			{
				return base["PopClientAccessRulesEnabled"];
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

		public VariantConfigurationSection LrsLogging
		{
			get
			{
				return base["LrsLogging"];
			}
		}
	}
}
