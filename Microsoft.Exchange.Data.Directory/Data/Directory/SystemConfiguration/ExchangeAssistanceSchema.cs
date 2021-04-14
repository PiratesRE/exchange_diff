using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ExchangeAssistanceSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition ExchangeHelpAppOnline = new ADPropertyDefinition("ExchangeHelpAppOnline", ExchangeObjectVersion.Exchange2010, typeof(bool), "msExchExchangeHelpAppOnline", ADPropertyDefinitionFlags.None, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ControlPanelHelpURL = new ADPropertyDefinition("ControlPanelHelpURL", ExchangeObjectVersion.Exchange2010, typeof(Uri), "msExchControlPanelHelpURL", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, null, null);

		public static readonly ADPropertyDefinition ControlPanelFeedbackURL = new ADPropertyDefinition("ControlPanelFeedbackURL", ExchangeObjectVersion.Exchange2010, typeof(Uri), "msExchControlPanelFeedbackURL", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, null, null);

		public static readonly ADPropertyDefinition ControlPanelFeedbackEnabled = new ADPropertyDefinition("ControlPanelFeedbackEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), "msExchControlPanelFeedbackEnabled", ADPropertyDefinitionFlags.None, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ManagementConsoleHelpURL = new ADPropertyDefinition("ManagementConsoleHelpURL", ExchangeObjectVersion.Exchange2010, typeof(Uri), "msExchManagementConsoleHelpURL", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, null, null);

		public static readonly ADPropertyDefinition ManagementConsoleFeedbackURL = new ADPropertyDefinition("ManagementConsoleFeedbackURL", ExchangeObjectVersion.Exchange2010, typeof(Uri), "msExchManagementConsoleFeedbackURL", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, null, null);

		public static readonly ADPropertyDefinition ManagementConsoleFeedbackEnabled = new ADPropertyDefinition("ManagementConsoleFeedbackEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), "msExchManagementConsoleFeedbackEnabled", ADPropertyDefinitionFlags.None, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OWAHelpURL = new ADPropertyDefinition("OWAHelpURL", ExchangeObjectVersion.Exchange2010, typeof(Uri), "msExchOWAHelpURL", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, null, null);

		public static readonly ADPropertyDefinition OWAFeedbackURL = new ADPropertyDefinition("OWAFeedbackURL", ExchangeObjectVersion.Exchange2010, typeof(Uri), "msExchOWAFeedbackURL", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, null, null);

		public static readonly ADPropertyDefinition OWAFeedbackEnabled = new ADPropertyDefinition("OWAFeedbackEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), "msExchOWAFeedbackEnabled", ADPropertyDefinitionFlags.None, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OWALightHelpURL = new ADPropertyDefinition("OWALightHelpURL", ExchangeObjectVersion.Exchange2010, typeof(Uri), "msExchOWALightHelpURL", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, null, null);

		public static readonly ADPropertyDefinition OWALightFeedbackURL = new ADPropertyDefinition("OWALightFeedbackURL", ExchangeObjectVersion.Exchange2010, typeof(Uri), "msExchOWALightFeedbackURL", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, null, null);

		public static readonly ADPropertyDefinition OWALightFeedbackEnabled = new ADPropertyDefinition("OWALightFeedbackEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), "msExchOWALightFeedbackEnabled", ADPropertyDefinitionFlags.None, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition WindowsLiveAccountPageURL = new ADPropertyDefinition("WindowsLiveAccountPageURL", ExchangeObjectVersion.Exchange2010, typeof(Uri), "msExchWindowsLiveAccountURL", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, null, null);

		public static readonly ADPropertyDefinition WindowsLiveAccountURLEnabled = new ADPropertyDefinition("WindowsLiveAccountURLEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), "msExchWindowsLiveAccountURLEnabled", ADPropertyDefinitionFlags.None, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PrivacyStatementURL = new ADPropertyDefinition("PrivacyStatementURL", ExchangeObjectVersion.Exchange2010, typeof(Uri), "msExchPrivacyStatementURL", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, null, null);

		public static readonly ADPropertyDefinition PrivacyLinkDisplayEnabled = new ADPropertyDefinition("PrivacyLinkDisplayEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), "msExchPrivacyStatementURLEnabled", ADPropertyDefinitionFlags.None, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CommunityURL = new ADPropertyDefinition("CommunityURL", ExchangeObjectVersion.Exchange2010, typeof(Uri), "msExchCommunityURL", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, null, null);

		public static readonly ADPropertyDefinition CommunityLinkDisplayEnabled = new ADPropertyDefinition("CommunityLinkDisplayEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), "msExchCommunityURLEnabled", ADPropertyDefinitionFlags.None, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
