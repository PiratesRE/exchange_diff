using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.BOX.UI.Shell
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "NavBarData", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.UI.Shell")]
	public class NavBarData : IExtensibleDataObject
	{
		public ExtensionDataObject ExtensionData
		{
			get
			{
				return this.extensionDataField;
			}
			set
			{
				this.extensionDataField = value;
			}
		}

		[DataMember]
		public NavBarLinkData AboutMeLink
		{
			get
			{
				return this.AboutMeLinkField;
			}
			set
			{
				this.AboutMeLinkField = value;
			}
		}

		[DataMember]
		public NavBarLinkData AdminLink
		{
			get
			{
				return this.AdminLinkField;
			}
			set
			{
				this.AdminLinkField = value;
			}
		}

		[DataMember]
		public NavBarImageData AppsImage
		{
			get
			{
				return this.AppsImageField;
			}
			set
			{
				this.AppsImageField = value;
			}
		}

		[DataMember]
		public NavBarLinkData[] AppsLinks
		{
			get
			{
				return this.AppsLinksField;
			}
			set
			{
				this.AppsLinksField = value;
			}
		}

		[DataMember]
		public string ClientData
		{
			get
			{
				return this.ClientDataField;
			}
			set
			{
				this.ClientDataField = value;
			}
		}

		[DataMember]
		public NavBarLinkData CommunityLink
		{
			get
			{
				return this.CommunityLinkField;
			}
			set
			{
				this.CommunityLinkField = value;
			}
		}

		[DataMember]
		public string CompanyDisplayName
		{
			get
			{
				return this.CompanyDisplayNameField;
			}
			set
			{
				this.CompanyDisplayNameField = value;
			}
		}

		[DataMember]
		public string CorrelationID
		{
			get
			{
				return this.CorrelationIDField;
			}
			set
			{
				this.CorrelationIDField = value;
			}
		}

		[DataMember]
		public string CultureName
		{
			get
			{
				return this.CultureNameField;
			}
			set
			{
				this.CultureNameField = value;
			}
		}

		[DataMember]
		public string CurrentMainLinkElementID
		{
			get
			{
				return this.CurrentMainLinkElementIDField;
			}
			set
			{
				this.CurrentMainLinkElementIDField = value;
			}
		}

		[DataMember]
		public NavBarLinkData[] CurrentWorkloadHelpSubLinks
		{
			get
			{
				return this.CurrentWorkloadHelpSubLinksField;
			}
			set
			{
				this.CurrentWorkloadHelpSubLinksField = value;
			}
		}

		[DataMember]
		public NavBarLinkData CurrentWorkloadSettingsLink
		{
			get
			{
				return this.CurrentWorkloadSettingsLinkField;
			}
			set
			{
				this.CurrentWorkloadSettingsLinkField = value;
			}
		}

		[DataMember]
		public NavBarLinkData[] CurrentWorkloadSettingsSubLinks
		{
			get
			{
				return this.CurrentWorkloadSettingsSubLinksField;
			}
			set
			{
				this.CurrentWorkloadSettingsSubLinksField = value;
			}
		}

		[DataMember]
		public NavBarLinkData[] CurrentWorkloadUserSubLinks
		{
			get
			{
				return this.CurrentWorkloadUserSubLinksField;
			}
			set
			{
				this.CurrentWorkloadUserSubLinksField = value;
			}
		}

		[DataMember]
		public ShellDimensions Dimensions
		{
			get
			{
				return this.DimensionsField;
			}
			set
			{
				this.DimensionsField = value;
			}
		}

		[DataMember]
		public NavBarImageData DownArrowImage
		{
			get
			{
				return this.DownArrowImageField;
			}
			set
			{
				this.DownArrowImageField = value;
			}
		}

		[DataMember]
		public NavBarImageData DownWhiteArrowImage
		{
			get
			{
				return this.DownWhiteArrowImageField;
			}
			set
			{
				this.DownWhiteArrowImageField = value;
			}
		}

		[DataMember]
		public NavBarLinkData FeedbackLink
		{
			get
			{
				return this.FeedbackLinkField;
			}
			set
			{
				this.FeedbackLinkField = value;
			}
		}

		[DataMember]
		public string FlightName
		{
			get
			{
				return this.FlightNameField;
			}
			set
			{
				this.FlightNameField = value;
			}
		}

		[DataMember]
		public bool FlipHelpIcon
		{
			get
			{
				return this.FlipHelpIconField;
			}
			set
			{
				this.FlipHelpIconField = value;
			}
		}

		[DataMember]
		public string FooterCopyrightLogoTitle
		{
			get
			{
				return this.FooterCopyrightLogoTitleField;
			}
			set
			{
				this.FooterCopyrightLogoTitleField = value;
			}
		}

		[DataMember]
		public string FooterCopyrightText
		{
			get
			{
				return this.FooterCopyrightTextField;
			}
			set
			{
				this.FooterCopyrightTextField = value;
			}
		}

		[DataMember]
		public NavBarLinkData FooterICPLink
		{
			get
			{
				return this.FooterICPLinkField;
			}
			set
			{
				this.FooterICPLinkField = value;
			}
		}

		[DataMember]
		public NavBarImageData FooterLogoImage
		{
			get
			{
				return this.FooterLogoImageField;
			}
			set
			{
				this.FooterLogoImageField = value;
			}
		}

		[DataMember]
		public bool HasTenantBranding
		{
			get
			{
				return this.HasTenantBrandingField;
			}
			set
			{
				this.HasTenantBrandingField = value;
			}
		}

		[DataMember]
		public NavBarImageData HelpImage
		{
			get
			{
				return this.HelpImageField;
			}
			set
			{
				this.HelpImageField = value;
			}
		}

		[DataMember]
		public NavBarLinkData HelpLink
		{
			get
			{
				return this.HelpLinkField;
			}
			set
			{
				this.HelpLinkField = value;
			}
		}

		[DataMember]
		public string IPv6Text
		{
			get
			{
				return this.IPv6TextField;
			}
			set
			{
				this.IPv6TextField = value;
			}
		}

		[DataMember]
		public string ImageClusterUrl
		{
			get
			{
				return this.ImageClusterUrlField;
			}
			set
			{
				this.ImageClusterUrlField = value;
			}
		}

		[DataMember]
		public bool IsAuthenticated
		{
			get
			{
				return this.IsAuthenticatedField;
			}
			set
			{
				this.IsAuthenticatedField = value;
			}
		}

		[DataMember]
		public NavBarLinkData LegalLink
		{
			get
			{
				return this.LegalLinkField;
			}
			set
			{
				this.LegalLinkField = value;
			}
		}

		[DataMember]
		public string LogoIconID
		{
			get
			{
				return this.LogoIconIDField;
			}
			set
			{
				this.LogoIconIDField = value;
			}
		}

		[DataMember]
		public NavBarImageData LogoImage
		{
			get
			{
				return this.LogoImageField;
			}
			set
			{
				this.LogoImageField = value;
			}
		}

		[DataMember]
		public string LogoNavigationUrl
		{
			get
			{
				return this.LogoNavigationUrlField;
			}
			set
			{
				this.LogoNavigationUrlField = value;
			}
		}

		[DataMember]
		public NavBarImageData LogoThemeableImage
		{
			get
			{
				return this.LogoThemeableImageField;
			}
			set
			{
				this.LogoThemeableImageField = value;
			}
		}

		[DataMember]
		public string MenuTitleText
		{
			get
			{
				return this.MenuTitleTextField;
			}
			set
			{
				this.MenuTitleTextField = value;
			}
		}

		[DataMember]
		public string MyProfileUrl
		{
			get
			{
				return this.MyProfileUrlField;
			}
			set
			{
				this.MyProfileUrlField = value;
			}
		}

		[DataMember]
		public string NavBarAriaLabel
		{
			get
			{
				return this.NavBarAriaLabelField;
			}
			set
			{
				this.NavBarAriaLabelField = value;
			}
		}

		[DataMember]
		public NavBarImageData NotificationsBellIconImage
		{
			get
			{
				return this.NotificationsBellIconImageField;
			}
			set
			{
				this.NotificationsBellIconImageField = value;
			}
		}

		[DataMember]
		public string NotificationsCountLabelText
		{
			get
			{
				return this.NotificationsCountLabelTextField;
			}
			set
			{
				this.NotificationsCountLabelTextField = value;
			}
		}

		[DataMember]
		public NavBarImageData NotificationsHighIconImage
		{
			get
			{
				return this.NotificationsHighIconImageField;
			}
			set
			{
				this.NotificationsHighIconImageField = value;
			}
		}

		[DataMember]
		public NavBarImageData NotificationsLowIconImage
		{
			get
			{
				return this.NotificationsLowIconImageField;
			}
			set
			{
				this.NotificationsLowIconImageField = value;
			}
		}

		[DataMember]
		public NavBarImageData NotificationsMediumIconImage
		{
			get
			{
				return this.NotificationsMediumIconImageField;
			}
			set
			{
				this.NotificationsMediumIconImageField = value;
			}
		}

		[DataMember]
		public string NotificationsPopupHeaderText
		{
			get
			{
				return this.NotificationsPopupHeaderTextField;
			}
			set
			{
				this.NotificationsPopupHeaderTextField = value;
			}
		}

		[DataMember]
		public NavBarUnclusteredImageData NotificationsProgressIconImage
		{
			get
			{
				return this.NotificationsProgressIconImageField;
			}
			set
			{
				this.NotificationsProgressIconImageField = value;
			}
		}

		[DataMember]
		public NavBarLinkData O365SettingsLink
		{
			get
			{
				return this.O365SettingsLinkField;
			}
			set
			{
				this.O365SettingsLinkField = value;
			}
		}

		[DataMember]
		public NavBarLinkData PartnerLink
		{
			get
			{
				return this.PartnerLinkField;
			}
			set
			{
				this.PartnerLinkField = value;
			}
		}

		[DataMember]
		public string PoweredByText
		{
			get
			{
				return this.PoweredByTextField;
			}
			set
			{
				this.PoweredByTextField = value;
			}
		}

		[DataMember]
		public NavBarLinkData PrivacyLink
		{
			get
			{
				return this.PrivacyLinkField;
			}
			set
			{
				this.PrivacyLinkField = value;
			}
		}

		[DataMember]
		public string SessionID
		{
			get
			{
				return this.SessionIDField;
			}
			set
			{
				this.SessionIDField = value;
			}
		}

		[DataMember]
		public NavBarImageData SettingsImage
		{
			get
			{
				return this.SettingsImageField;
			}
			set
			{
				this.SettingsImageField = value;
			}
		}

		[DataMember]
		public NavBarLinkData SignOutLink
		{
			get
			{
				return this.SignOutLinkField;
			}
			set
			{
				this.SignOutLinkField = value;
			}
		}

		[DataMember]
		public NavBarLinkData TenantBackgroundImageUrl
		{
			get
			{
				return this.TenantBackgroundImageUrlField;
			}
			set
			{
				this.TenantBackgroundImageUrlField = value;
			}
		}

		[DataMember]
		public NavBarLinkData TenantLogoNavigationUrl
		{
			get
			{
				return this.TenantLogoNavigationUrlField;
			}
			set
			{
				this.TenantLogoNavigationUrlField = value;
			}
		}

		[DataMember]
		public NavBarLinkData TenantLogoUrl
		{
			get
			{
				return this.TenantLogoUrlField;
			}
			set
			{
				this.TenantLogoUrlField = value;
			}
		}

		[DataMember]
		public string[] ThemeColors
		{
			get
			{
				return this.ThemeColorsField;
			}
			set
			{
				this.ThemeColorsField = value;
			}
		}

		[DataMember]
		public string TransparentImageUrl
		{
			get
			{
				return this.TransparentImageUrlField;
			}
			set
			{
				this.TransparentImageUrlField = value;
			}
		}

		[DataMember]
		public string TruncatedUserDisplayName
		{
			get
			{
				return this.TruncatedUserDisplayNameField;
			}
			set
			{
				this.TruncatedUserDisplayNameField = value;
			}
		}

		[DataMember]
		public NavBarImageData UpArrowImage
		{
			get
			{
				return this.UpArrowImageField;
			}
			set
			{
				this.UpArrowImageField = value;
			}
		}

		[DataMember]
		public bool UseSPOBehaviors
		{
			get
			{
				return this.UseSPOBehaviorsField;
			}
			set
			{
				this.UseSPOBehaviorsField = value;
			}
		}

		[DataMember]
		public string UserDisplayName
		{
			get
			{
				return this.UserDisplayNameField;
			}
			set
			{
				this.UserDisplayNameField = value;
			}
		}

		[DataMember]
		public NavBarLinkData[] WorkloadLinks
		{
			get
			{
				return this.WorkloadLinksField;
			}
			set
			{
				this.WorkloadLinksField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private NavBarLinkData AboutMeLinkField;

		private NavBarLinkData AdminLinkField;

		private NavBarImageData AppsImageField;

		private NavBarLinkData[] AppsLinksField;

		private string ClientDataField;

		private NavBarLinkData CommunityLinkField;

		private string CompanyDisplayNameField;

		private string CorrelationIDField;

		private string CultureNameField;

		private string CurrentMainLinkElementIDField;

		private NavBarLinkData[] CurrentWorkloadHelpSubLinksField;

		private NavBarLinkData CurrentWorkloadSettingsLinkField;

		private NavBarLinkData[] CurrentWorkloadSettingsSubLinksField;

		private NavBarLinkData[] CurrentWorkloadUserSubLinksField;

		private ShellDimensions DimensionsField;

		private NavBarImageData DownArrowImageField;

		private NavBarImageData DownWhiteArrowImageField;

		private NavBarLinkData FeedbackLinkField;

		private string FlightNameField;

		private bool FlipHelpIconField;

		private string FooterCopyrightLogoTitleField;

		private string FooterCopyrightTextField;

		private NavBarLinkData FooterICPLinkField;

		private NavBarImageData FooterLogoImageField;

		private bool HasTenantBrandingField;

		private NavBarImageData HelpImageField;

		private NavBarLinkData HelpLinkField;

		private string IPv6TextField;

		private string ImageClusterUrlField;

		private bool IsAuthenticatedField;

		private NavBarLinkData LegalLinkField;

		private string LogoIconIDField;

		private NavBarImageData LogoImageField;

		private string LogoNavigationUrlField;

		private NavBarImageData LogoThemeableImageField;

		private string MenuTitleTextField;

		private string MyProfileUrlField;

		private string NavBarAriaLabelField;

		private NavBarImageData NotificationsBellIconImageField;

		private string NotificationsCountLabelTextField;

		private NavBarImageData NotificationsHighIconImageField;

		private NavBarImageData NotificationsLowIconImageField;

		private NavBarImageData NotificationsMediumIconImageField;

		private string NotificationsPopupHeaderTextField;

		private NavBarUnclusteredImageData NotificationsProgressIconImageField;

		private NavBarLinkData O365SettingsLinkField;

		private NavBarLinkData PartnerLinkField;

		private string PoweredByTextField;

		private NavBarLinkData PrivacyLinkField;

		private string SessionIDField;

		private NavBarImageData SettingsImageField;

		private NavBarLinkData SignOutLinkField;

		private NavBarLinkData TenantBackgroundImageUrlField;

		private NavBarLinkData TenantLogoNavigationUrlField;

		private NavBarLinkData TenantLogoUrlField;

		private string[] ThemeColorsField;

		private string TransparentImageUrlField;

		private string TruncatedUserDisplayNameField;

		private NavBarImageData UpArrowImageField;

		private bool UseSPOBehaviorsField;

		private string UserDisplayNameField;

		private NavBarLinkData[] WorkloadLinksField;
	}
}
