using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal abstract class PopImapAdConfigurationSchema : ADEmailTransportSchema
	{
		public static readonly ADPropertyDefinition PopImapProtocolFlags = new ADPropertyDefinition("PopImapProtocolFlags", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchPopImapProtocolFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ProtocolLogEnabled = ADObject.BitfieldProperty("ProtocolLogEnabled", 0, PopImapAdConfigurationSchema.PopImapProtocolFlags);

		public static readonly ADPropertyDefinition LogFileLocation = new ADPropertyDefinition("LogFileLocation", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchPopImapLogFilePath", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LogFileRollOverSettings = new ADPropertyDefinition("LogFileRollOverSettings", ExchangeObjectVersion.Exchange2010, typeof(LogFileRollOver), "msExchPopImapLogFileRolloverFrequency", ADPropertyDefinitionFlags.None, LogFileRollOver.Daily, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LogPerFileSizeQuota = new ADPropertyDefinition("LogPerFileSizeQuota", ExchangeObjectVersion.Exchange2010, typeof(Unlimited<ByteQuantifiedSize>), "msExchPopImapPerLogFileSizeQuota", ADPropertyDefinitionFlags.None, Unlimited<ByteQuantifiedSize>.UnlimitedValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AllowCrossSiteSessions = ADObject.BitfieldProperty("AllowCrossSiteSessions", 1, PopImapAdConfigurationSchema.PopImapProtocolFlags);

		public static readonly ADPropertyDefinition EnforceCertificateErrors = ADObject.BitfieldProperty("EnforceCertificateErrors", 2, PopImapAdConfigurationSchema.PopImapProtocolFlags);

		public static readonly ADPropertyDefinition UnencryptedOrTLSBindings = new ADPropertyDefinition("UnencryptedOrTLSBindings", ExchangeObjectVersion.Exchange2003, typeof(IPBinding), "msExchServerBindings", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SSLBindings = new ADPropertyDefinition("SSLBindings", ExchangeObjectVersion.Exchange2003, typeof(IPBinding), "msExchSecureBindings", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition InternalConnectionSettings = new ADPropertyDefinition("InternalConnectionSettings", ExchangeObjectVersion.Exchange2010, typeof(ProtocolConnectionSettings), "msExchPOPIMAPInternalConnectionSettings", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExternalConnectionSettings = new ADPropertyDefinition("ExternalConnectionSettings", ExchangeObjectVersion.Exchange2010, typeof(ProtocolConnectionSettings), "msExchPOPIMAPExternalConnectionSettings", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition X509CertificateName = new ADPropertyDefinition("X509CertificateName", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchPopImapX509CertificateName", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 1024)
		}, null, null);

		public static readonly ADPropertyDefinition Banner = new ADPropertyDefinition("Banner", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchPopImapBanner", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 1024)
		}, null, null);

		public static readonly ADPropertyDefinition LoginType = new ADPropertyDefinition("LoginType", ExchangeObjectVersion.Exchange2003, typeof(LoginOptions), "msExchAuthenticationFlags", ADPropertyDefinitionFlags.None, LoginOptions.SecureLogin, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AuthenticatedConnectionTimeout = new ADPropertyDefinition("AuthenticatedConnectionTimeout", ExchangeObjectVersion.Exchange2003, typeof(EnhancedTimeSpan), "msExchIncomingConnectionTimeout", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromSeconds(1800.0), PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.FromSeconds(30.0), EnhancedTimeSpan.FromSeconds(86400.0)),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, null, null);

		public static readonly ADPropertyDefinition PreAuthenticatedConnectionTimeout = new ADPropertyDefinition("PreAuthenticatedConnectionTimeout", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchPopImapIncomingPreauthConnectionTimeout", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromSeconds(60.0), PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.FromSeconds(30.0), EnhancedTimeSpan.FromSeconds(3600.0)),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, null, null);

		public static readonly ADPropertyDefinition MaxConnections = new ADPropertyDefinition("MaxConnections", ExchangeObjectVersion.Exchange2003, typeof(int), "msExchMaxIncomingConnections", ADPropertyDefinitionFlags.PersistDefaultValue, int.MaxValue, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, int.MaxValue)
		}, null, null);

		public static readonly ADPropertyDefinition MaxConnectionFromSingleIP = new ADPropertyDefinition("MaxConnectionFromSingleIP", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchPopImapMaxIncomingConnectionFromSingleSource", ADPropertyDefinitionFlags.PersistDefaultValue, int.MaxValue, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, int.MaxValue)
		}, null, null);

		public static readonly ADPropertyDefinition MaxConnectionsPerUser = new ADPropertyDefinition("MaxConnectionsPerUser", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchPopImapMaxIncomingConnectionPerUser", ADPropertyDefinitionFlags.PersistDefaultValue, 16, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, int.MaxValue)
		}, null, null);

		public static readonly ADPropertyDefinition MessageRetrievalMimeFormat = new ADPropertyDefinition("MessageRetrievalMimeFormat", ExchangeObjectVersion.Exchange2007, typeof(MimeTextFormat), "contentType", ADPropertyDefinitionFlags.None, MimeTextFormat.BestBodyFormat, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ProxyTargetPort = new ADPropertyDefinition("ProxyTargetPort", ExchangeObjectVersion.Exchange2007, typeof(int), "portNumber", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 65535)
		}, null, null);

		public static readonly ADPropertyDefinition CalendarItemRetrievalOption = new ADPropertyDefinition("CalendarItemRetrievalOption", ExchangeObjectVersion.Exchange2007, typeof(CalendarItemRetrievalOptions), "msExchPopImapCalendarItemRetrievalOption", ADPropertyDefinitionFlags.None, CalendarItemRetrievalOptions.iCalendar, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OwaServerUrl = new ADPropertyDefinition("OwaServerUrl", ExchangeObjectVersion.Exchange2007, typeof(Uri), "oWAServer", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, null, null);

		public static readonly ADPropertyDefinition PopImapFlags = new ADPropertyDefinition("PopImapFlags", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchPopImapFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition EnableExactRFC822Size = ADObject.BitfieldProperty("EnableExactRFC822Size", 1, PopImapAdConfigurationSchema.PopImapFlags);

		public static readonly ADPropertyDefinition LiveIdBasicAuthReplacement = ADObject.BitfieldProperty("LiveIdBasicAuthReplacement", 2, PopImapAdConfigurationSchema.PopImapFlags);

		public static readonly ADPropertyDefinition SuppressReadReceipt = ADObject.BitfieldProperty("SuppressReadReceipt", 4, PopImapAdConfigurationSchema.PopImapFlags);

		public static readonly ADPropertyDefinition ExtendedProtectionPolicy = new ADPropertyDefinition("ExtendedProtectionPolicy", ExchangeObjectVersion.Exchange2007, typeof(ExtendedProtectionTokenCheckingMode), "msExchPopImapExtendedProtectionPolicy", ADPropertyDefinitionFlags.None, ExtendedProtectionTokenCheckingMode.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition EnableGSSAPIAndNTLMAuth = ADObject.BitfieldProperty("EnableGSSAPIAndNTLMAuth", 8, PopImapAdConfigurationSchema.PopImapFlags);
	}
}
