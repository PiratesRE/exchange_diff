using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class ADOwaVirtualDirectorySchema : ExchangeWebAppVirtualDirectorySchema
	{
		internal static SetterDelegate FlagSetterDelegateWithValidationOnOwaVersionIsExchange12OrLater(int mask, PropertyDefinition propertyDefinition, string description)
		{
			return ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersion(mask, propertyDefinition, description, OwaVersions.Exchange2007, false);
		}

		internal static SetterDelegate FlagSetterDelegateWithValidationOnOwaVersionIsExchange14OrLater(int mask, PropertyDefinition propertyDefinition, string description)
		{
			return ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersion(mask, propertyDefinition, description, OwaVersions.Exchange2010, false);
		}

		internal static SetterDelegate FlagSetterDelegateWithValidationOnOwaVersionIsExchange14OrLater(int mask, PropertyDefinition propertyDefinition, string description, bool invert)
		{
			return ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersion(mask, propertyDefinition, description, OwaVersions.Exchange2010, invert);
		}

		private static SetterDelegate FlagSetterDelegateWithValidationOnOwaVersion(int mask, PropertyDefinition propertyDefinition, string description, OwaVersions owaVersion, bool invert)
		{
			return delegate(object value, IPropertyBag bag)
			{
				if ((OwaVersions)bag[ADOwaVirtualDirectorySchema.OwaVersion] < owaVersion)
				{
					throw new DataValidationException(new PropertyValidationError(DirectoryStrings.ExceptionOwaCannotSetPropertyOnLegacyVirtualDirectory(description), propertyDefinition, value), null);
				}
				if (value != null)
				{
					bool flag = invert ? (!(bool)value) : ((bool)value);
					int num = (int)bag[propertyDefinition];
					bag[propertyDefinition] = (flag ? (num | mask) : (num & ~mask));
					return;
				}
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.ExceptionOwaCannotSetPropertyOnE12VirtualDirectoryToNull(description), propertyDefinition, value), null);
			};
		}

		private const int MaxNotificationInterval = 86400;

		private const int MaxUserContextTimeout = 604800;

		private const string FileExtensionOrSplatRegularExpression = "(^\\.([^.]+)|\\*)$";

		private const string FileExtensionRegularExpression = "^\\.([^.]+)$";

		internal static readonly string[] Exchange2007RTMWebReadyDocumentViewingSupportedFileTypes = new string[]
		{
			".doc",
			".dot",
			".rtf",
			".xls",
			".ppt",
			".pps",
			".pdf"
		};

		internal static readonly string[] Exchange2007RTMWebReadyDocumentViewingSupportedMimeTypes = new string[]
		{
			"application/msword",
			"application/vnd.ms-excel",
			"application/x-msexcel",
			"application/vnd.ms-powerpoint",
			"application/x-mspowerpoint",
			"application/pdf"
		};

		public static ADPropertyDefinition OwaVersion = new ADPropertyDefinition("OwaVersion", ExchangeObjectVersion.Exchange2007, typeof(OwaVersions), "msExchOwaVersion", ADPropertyDefinitionFlags.PersistDefaultValue, OwaVersions.Exchange2013, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition FileAccessControlOnPublicComputers = new ADPropertyDefinition("FileAccessControlOnPublicComputers", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchOWAFileAccessControlOnPublicComputers", ADPropertyDefinitionFlags.PersistDefaultValue, (int)OwaMailboxPolicySchema.FileAccessControlOnPublicComputers.DefaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition FileAccessControlOnPrivateComputers = new ADPropertyDefinition("FileAccessControlOnPrivateComputers", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchOWAFileAccessControlOnPrivateComputers", ADPropertyDefinitionFlags.PersistDefaultValue, (int)OwaMailboxPolicySchema.FileAccessControlOnPrivateComputers.DefaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RemoteDocumentsActionForUnknownServers = new ADPropertyDefinition("RemoteDocumentsActionForUnknownServers", ExchangeObjectVersion.Exchange2007, typeof(RemoteDocumentsActions), "msExchOWARemoteDocumentsActionForUnknownServers", ADPropertyDefinitionFlags.None, RemoteDocumentsActions.Block, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ActionForUnknownFileAndMIMETypes = new ADPropertyDefinition("ActionForUnknownFileAndMIMETypes", ExchangeObjectVersion.Exchange2007, typeof(AttachmentBlockingActions), "msExchOWAActionForUnknownFileAndMIMETypes", ADPropertyDefinitionFlags.None, (AttachmentBlockingActions)OwaMailboxPolicySchema.ActionForUnknownFileAndMIMETypes.DefaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition ADWebReadyFileTypes = SharedPropertyDefinitions.ADWebReadyFileTypes;

		public static readonly ADPropertyDefinition WebReadyFileTypes = new ADPropertyDefinition("WebReadyFileTypes", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADWebReadyFileTypes,
			ADObjectSchema.Id
		}, null, new GetterDelegate(ADOwaVirtualDirectory.WebReadyFileTypesGetter), new SetterDelegate(ADOwaVirtualDirectory.WebReadyFileTypesSetter), null, null);

		internal static readonly ADPropertyDefinition ADWebReadyMimeTypes = SharedPropertyDefinitions.ADWebReadyMimeTypes;

		public static readonly ADPropertyDefinition WebReadyMimeTypes = new ADPropertyDefinition("WebReadyMimeTypes", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADWebReadyMimeTypes,
			ADObjectSchema.Id
		}, null, new GetterDelegate(ADOwaVirtualDirectory.WebReadyMimeTypesGetter), new SetterDelegate(ADOwaVirtualDirectory.WebReadyMimeTypesSetter), null, null);

		internal static readonly ADPropertyDefinition ADWebReadyDocumentViewingFlags = new ADPropertyDefinition("ADWebReadyDocumentViewingFlags", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchOWATranscodingFlags", ADPropertyDefinitionFlags.PersistDefaultValue, (int)OwaMailboxPolicySchema.ADWebReadyDocumentViewingFlags.DefaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition WebReadyDocumentViewingForAllSupportedTypes = new ADPropertyDefinition("WebReadyDocumentViewingForAllSupportedTypes", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADWebReadyDocumentViewingFlags
		}, null, ADObject.FlagGetterDelegate(1, ADOwaVirtualDirectorySchema.ADWebReadyDocumentViewingFlags), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange12OrLater(1, ADOwaVirtualDirectorySchema.ADWebReadyDocumentViewingFlags, "WebReadyDocumentViewingForAllSupportedTypes"), null, null);

		internal static readonly ADPropertyDefinition ADAllowedFileTypes = SharedPropertyDefinitions.ADAllowedFileTypes;

		public static readonly ADPropertyDefinition AllowedFileTypes = new ADPropertyDefinition("AllowedFileTypes", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RegexConstraint("^\\.([^.]+)$", DataStrings.FileExtensionPatternDescription)
		}, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADAllowedFileTypes,
			ADObjectSchema.Id
		}, null, new GetterDelegate(ADOwaVirtualDirectory.AllowedFileTypesGetter), new SetterDelegate(ADOwaVirtualDirectory.AllowedFileTypesSetter), null, null);

		internal static readonly ADPropertyDefinition ADAllowedMimeTypes = SharedPropertyDefinitions.ADAllowedMimeTypes;

		public static readonly ADPropertyDefinition AllowedMimeTypes = new ADPropertyDefinition("AllowedMimeTypes", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADAllowedMimeTypes,
			ADObjectSchema.Id
		}, null, new GetterDelegate(ADOwaVirtualDirectory.AllowedMimeTypesGetter), new SetterDelegate(ADOwaVirtualDirectory.AllowedMimeTypesSetter), null, null);

		internal static readonly ADPropertyDefinition ADForceSaveFileTypes = SharedPropertyDefinitions.ADForceSaveFileTypes;

		public static readonly ADPropertyDefinition ForceSaveFileTypes = new ADPropertyDefinition("ForceSaveFileTypes", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RegexConstraint("^\\.([^.]+)$", DataStrings.FileExtensionPatternDescription)
		}, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADForceSaveFileTypes,
			ADObjectSchema.Id
		}, null, new GetterDelegate(ADOwaVirtualDirectory.ForceSaveFileTypesGetter), new SetterDelegate(ADOwaVirtualDirectory.ForceSaveFileTypesSetter), null, null);

		internal static readonly ADPropertyDefinition ADForceSaveMimeTypes = SharedPropertyDefinitions.ADForceSaveMimeTypes;

		public static readonly ADPropertyDefinition ForceSaveMimeTypes = new ADPropertyDefinition("ForceSaveMimeTypes", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADForceSaveMimeTypes,
			ADObjectSchema.Id
		}, null, new GetterDelegate(ADOwaVirtualDirectory.ForceSaveMimeTypesGetter), new SetterDelegate(ADOwaVirtualDirectory.ForceSaveMimeTypesSetter), null, null);

		internal static readonly ADPropertyDefinition ADBlockedFileTypes = SharedPropertyDefinitions.ADBlockedFileTypes;

		public static readonly ADPropertyDefinition BlockedFileTypes = new ADPropertyDefinition("BlockedFileTypes", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RegexConstraint("^\\.([^.]+)$", DataStrings.FileExtensionPatternDescription)
		}, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADBlockedFileTypes,
			ADObjectSchema.Id
		}, null, new GetterDelegate(ADOwaVirtualDirectory.BlockedFileTypesGetter), new SetterDelegate(ADOwaVirtualDirectory.BlockedFileTypesSetter), null, null);

		internal static readonly ADPropertyDefinition ADBlockedMimeTypes = SharedPropertyDefinitions.ADBlockedMimeTypes;

		public static readonly ADPropertyDefinition BlockedMimeTypes = new ADPropertyDefinition("BlockedMimeTypes", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADBlockedMimeTypes,
			ADObjectSchema.Id
		}, null, new GetterDelegate(ADOwaVirtualDirectory.BlockedMimeTypesGetter), new SetterDelegate(ADOwaVirtualDirectory.BlockedMimeTypesSetter), null, null);

		internal static readonly ADPropertyDefinition ADRemoteDocumentsAllowedServers = new ADPropertyDefinition("ADRemoteDocumentsAllowedServers", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchOWARemoteDocumentsAllowedServers", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RemoteDocumentsAllowedServers = new ADPropertyDefinition("RemoteDocumentsAllowedServers", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 256)
		}, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADRemoteDocumentsAllowedServers,
			ADObjectSchema.Id
		}, null, new GetterDelegate(ADOwaVirtualDirectory.RemoteDocumentsAllowedServersGetter), new SetterDelegate(ADOwaVirtualDirectory.RemoteDocumentsAllowedServersSetter), null, null);

		internal static readonly ADPropertyDefinition ADRemoteDocumentsBlockedServers = new ADPropertyDefinition("ADRemoteDocumentsBlockedServers", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchOWARemoteDocumentsBlockedServers", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RemoteDocumentsBlockedServers = new ADPropertyDefinition("RemoteDocumentsBlockedServers", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 256)
		}, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADRemoteDocumentsBlockedServers,
			ADObjectSchema.Id
		}, null, new GetterDelegate(ADOwaVirtualDirectory.RemoteDocumentsBlockedServersGetter), new SetterDelegate(ADOwaVirtualDirectory.RemoteDocumentsBlockedServersSetter), null, null);

		internal static readonly ADPropertyDefinition ADRemoteDocumentsInternalDomainSuffixList = new ADPropertyDefinition("ADRemoteDocumentsInternalDomainSuffixList", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchOWARemoteDocumentsInternalDomainSuffixList", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RemoteDocumentsInternalDomainSuffixList = new ADPropertyDefinition("RemoteDocumentsInternalDomainSuffixList", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new ValidDomainConstraint()
		}, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADRemoteDocumentsInternalDomainSuffixList,
			ADObjectSchema.Id
		}, null, new GetterDelegate(ADOwaVirtualDirectory.RemoteDocumentsInternalDomainSuffixListGetter), new SetterDelegate(ADOwaVirtualDirectory.RemoteDocumentsInternalDomainSuffixListSetter), null, null);

		public static readonly ADPropertyDefinition LogonFormat = new ADPropertyDefinition("LogonFormat", ExchangeObjectVersion.Exchange2007, typeof(LogonFormats), "msExchOWALogonFormat", ADPropertyDefinitionFlags.None, LogonFormats.FullDomain, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ClientAuthCleanupLevel = new ADPropertyDefinition("ClientAuthCleanupLevel", ExchangeObjectVersion.Exchange2007, typeof(ClientAuthCleanupLevels), "msExchOWAClientAuthCleanupLevel", ADPropertyDefinitionFlags.None, ClientAuthCleanupLevels.High, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ConfigurationXMLRaw = XMLSerializableBase.ConfigurationXmlRawProperty();

		public static readonly ADPropertyDefinition ConfigurationXML = XMLSerializableBase.ConfigurationXmlProperty<ADOwaVirtualDirectoryConfigXML>(ADOwaVirtualDirectorySchema.ConfigurationXMLRaw);

		public static readonly ADPropertyDefinition IsPublic = XMLSerializableBase.ConfigXmlProperty<ADOwaVirtualDirectoryConfigXML, bool>("IsPublic", ExchangeObjectVersion.Exchange2010, ADOwaVirtualDirectorySchema.ConfigurationXML, false, (ADOwaVirtualDirectoryConfigXML configXml) => configXml.IsPublic, delegate(ADOwaVirtualDirectoryConfigXML configXml, bool value)
		{
			configXml.IsPublic = value;
		}, null, null);

		public static readonly ADPropertyDefinition FilterWebBeaconsAndHtmlForms = new ADPropertyDefinition("FilterWebBeaconsAndHtmlForms", ExchangeObjectVersion.Exchange2007, typeof(WebBeaconFilterLevels), "msExchOWAFilterWebBeacons", ADPropertyDefinitionFlags.None, WebBeaconFilterLevels.UserFilterChoice, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition NotificationInterval = new ADPropertyDefinition("NotificationInterval", ExchangeObjectVersion.Exchange2007, typeof(int?), "msExchOWANotificationInterval", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(30, 86400)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DefaultTheme = new ADPropertyDefinition("DefaultTheme", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchOWADefaultTheme", ADPropertyDefinitionFlags.None, (string)OwaMailboxPolicySchema.DefaultTheme.DefaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UserContextTimeout = new ADPropertyDefinition("UserContextTimeout", ExchangeObjectVersion.Exchange2007, typeof(int?), "msExchOWAUserContextTimeout", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, 604800)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExchwebProxyDestination = new ADPropertyDefinition("ExchwebProxyDestination", ExchangeObjectVersion.Exchange2007, typeof(ExchwebProxyDestinations), "msExchOwaExchwebProxyDestination", ADPropertyDefinitionFlags.None, ExchwebProxyDestinations.NotSpecified, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition VirtualDirectoryType = new ADPropertyDefinition("VirtualDirectoryType", ExchangeObjectVersion.Exchange2007, typeof(VirtualDirectoryTypes), "msExchOwaVirtualDirectoryType", ADPropertyDefinitionFlags.None, VirtualDirectoryTypes.NotSpecified, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition FolderPathname = new ADPropertyDefinition("FolderPathname", ExchangeObjectVersion.Exchange2007, typeof(string), "folderPathname", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition Url = new ADPropertyDefinition("Url", ExchangeObjectVersion.Exchange2007, typeof(string), "url", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition InstantMessagingType = new ADPropertyDefinition("InstantMessagingType", ExchangeObjectVersion.Exchange2010, typeof(InstantMessagingTypeOptions), "msExchOWAIMProviderType", ADPropertyDefinitionFlags.None, (InstantMessagingTypeOptions)OwaMailboxPolicySchema.InstantMessagingType.DefaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition InstantMessagingCertificateThumbprint = new ADPropertyDefinition("InstantMessagingCertificateThumbprint", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchOWAIMCertificateThumbprint", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition InstantMessagingServerName = new ADPropertyDefinition("InstantMessagingServerName", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchOWAIMServerName", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition SetPhotoURL = new ADPropertyDefinition("SetPhotoURL", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchAggregationSubscriptionCredential", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RedirectToOptimalOWAServer = new ADPropertyDefinition("RedirectToOptimalOWAServer", ExchangeObjectVersion.Exchange2007, typeof(RedirectToOptimalOWAServerOptions), "msExchOWARedirectToOptimalOWAServer", ADPropertyDefinitionFlags.None, RedirectToOptimalOWAServerOptions.Enabled, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DefaultClientLanguage = new ADPropertyDefinition("DefaultClientLanguage", ExchangeObjectVersion.Exchange2007, typeof(int?), "msExchOWADefaultClientLanguage", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new OWASupportedLanguageConstraint()
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LogonAndErrorLanguage = new ADPropertyDefinition("LogonAndErrorLanguage", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchOWALogonAndErrorLanguage", ADPropertyDefinitionFlags.PersistDefaultValue, (int)OwaMailboxPolicySchema.LogonAndErrorLanguage.DefaultValue, new PropertyDefinitionConstraint[]
		{
			new OWASupportedLanguageConstraint()
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UseGB18030 = new ADPropertyDefinition("UseGB18030", ExchangeObjectVersion.Exchange2007, typeof(int?), "msExchOWAUseGB18030", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UseISO885915 = new ADPropertyDefinition("UseISO885915", ExchangeObjectVersion.Exchange2007, typeof(int?), "msExchOWAUseISO885915", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OutboundCharset = new ADPropertyDefinition("OutboundCharset", ExchangeObjectVersion.Exchange2007, typeof(OutboundCharsetOptions), "msExchOWAOutboundCharset", ADPropertyDefinitionFlags.None, (OutboundCharsetOptions)OwaMailboxPolicySchema.OutboundCharset.DefaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ADMailboxFolderSet = new ADPropertyDefinition("ADMailboxFolderSet", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchMailboxFolderSet", ADPropertyDefinitionFlags.PersistDefaultValue, (int)OwaMailboxPolicySchema.ADMailboxFolderSet.DefaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ADMailboxFolderSet2 = new ADPropertyDefinition("ADMailboxFolderSet2", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchMailboxFolderSet2", ADPropertyDefinitionFlags.PersistDefaultValue, (int)OwaMailboxPolicySchema.ADMailboxFolderSet2.DefaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition GlobalAddressListEnabled = new ADPropertyDefinition("GlobalAddressListEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(1, ADOwaVirtualDirectorySchema.ADMailboxFolderSet), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14OrLater(1, ADOwaVirtualDirectorySchema.ADMailboxFolderSet, "GlobalAddressListEnabled"), null, null);

		public static readonly ADPropertyDefinition OrganizationEnabled = new ADPropertyDefinition("OrganizationEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(128, ADOwaVirtualDirectorySchema.ADMailboxFolderSet), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14OrLater(128, ADOwaVirtualDirectorySchema.ADMailboxFolderSet, "OrganizationEnabled"), null, null);

		public static readonly ADPropertyDefinition ExplicitLogonEnabled = new ADPropertyDefinition("ExplicitLogonEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(8388608, ADOwaVirtualDirectorySchema.ADMailboxFolderSet), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14OrLater(8388608, ADOwaVirtualDirectorySchema.ADMailboxFolderSet, "ExplicitLogonEnabled"), null, null);

		public static readonly ADPropertyDefinition OWALightEnabled = new ADPropertyDefinition("OWALightEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(536870912, ADOwaVirtualDirectorySchema.ADMailboxFolderSet), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14OrLater(536870912, ADOwaVirtualDirectorySchema.ADMailboxFolderSet, "OWALightEnabled"), null, null);

		public static readonly ADPropertyDefinition DelegateAccessEnabled = new ADPropertyDefinition("DelegateAccessEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(1073741824, ADOwaVirtualDirectorySchema.ADMailboxFolderSet), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14OrLater(1073741824, ADOwaVirtualDirectorySchema.ADMailboxFolderSet, "DelegateAccessEnabled"), null, null);

		public static readonly ADPropertyDefinition IRMEnabled = new ADPropertyDefinition("IRMEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(int.MinValue, ADOwaVirtualDirectorySchema.ADMailboxFolderSet), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14OrLater(int.MinValue, ADOwaVirtualDirectorySchema.ADMailboxFolderSet, "IRMEnabled"), null, null);

		public static readonly ADPropertyDefinition CalendarEnabled = new ADPropertyDefinition("CalendarEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(2, ADOwaVirtualDirectorySchema.ADMailboxFolderSet), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange12OrLater(2, ADOwaVirtualDirectorySchema.ADMailboxFolderSet, "CalendarEnabled"), null, null);

		public static readonly ADPropertyDefinition ContactsEnabled = new ADPropertyDefinition("ContactsEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(4, ADOwaVirtualDirectorySchema.ADMailboxFolderSet), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange12OrLater(4, ADOwaVirtualDirectorySchema.ADMailboxFolderSet, "ContactsEnabled"), null, null);

		public static readonly ADPropertyDefinition TasksEnabled = new ADPropertyDefinition("TasksEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(8, ADOwaVirtualDirectorySchema.ADMailboxFolderSet), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange12OrLater(8, ADOwaVirtualDirectorySchema.ADMailboxFolderSet, "TasksEnabled"), null, null);

		public static readonly ADPropertyDefinition JournalEnabled = new ADPropertyDefinition("JournalEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(16, ADOwaVirtualDirectorySchema.ADMailboxFolderSet), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange12OrLater(16, ADOwaVirtualDirectorySchema.ADMailboxFolderSet, "JournalEnabled"), null, null);

		public static readonly ADPropertyDefinition NotesEnabled = new ADPropertyDefinition("NotesEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(32, ADOwaVirtualDirectorySchema.ADMailboxFolderSet), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange12OrLater(32, ADOwaVirtualDirectorySchema.ADMailboxFolderSet, "NotesEnabled"), null, null);

		public static readonly ADPropertyDefinition RemindersAndNotificationsEnabled = new ADPropertyDefinition("RemindersAndNotificationsEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(256, ADOwaVirtualDirectorySchema.ADMailboxFolderSet), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange12OrLater(256, ADOwaVirtualDirectorySchema.ADMailboxFolderSet, "RemindersAndNotificationsEnabled"), null, null);

		public static readonly ADPropertyDefinition PremiumClientEnabled = new ADPropertyDefinition("PremiumClientEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(512, ADOwaVirtualDirectorySchema.ADMailboxFolderSet), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange12OrLater(512, ADOwaVirtualDirectorySchema.ADMailboxFolderSet, "PremiumClientEnabled"), null, null);

		public static readonly ADPropertyDefinition SpellCheckerEnabled = new ADPropertyDefinition("SpellCheckerEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(1024, ADOwaVirtualDirectorySchema.ADMailboxFolderSet), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange12OrLater(1024, ADOwaVirtualDirectorySchema.ADMailboxFolderSet, "SpellCheckerEnabled"), null, null);

		public static readonly ADPropertyDefinition SearchFoldersEnabled = new ADPropertyDefinition("SearchFoldersEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(4096, ADOwaVirtualDirectorySchema.ADMailboxFolderSet), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange12OrLater(4096, ADOwaVirtualDirectorySchema.ADMailboxFolderSet, "SearchFoldersEnabled"), null, null);

		public static readonly ADPropertyDefinition SignaturesEnabled = new ADPropertyDefinition("SignaturesEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(8192, ADOwaVirtualDirectorySchema.ADMailboxFolderSet), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange12OrLater(8192, ADOwaVirtualDirectorySchema.ADMailboxFolderSet, "SignaturesEnabled"), null, null);

		public static readonly ADPropertyDefinition ThemeSelectionEnabled = new ADPropertyDefinition("ThemeSelectionEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(32768, ADOwaVirtualDirectorySchema.ADMailboxFolderSet), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange12OrLater(32768, ADOwaVirtualDirectorySchema.ADMailboxFolderSet, "ThemeSelectionEnabled"), null, null);

		public static readonly ADPropertyDefinition JunkEmailEnabled = new ADPropertyDefinition("JunkEmailEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(65536, ADOwaVirtualDirectorySchema.ADMailboxFolderSet), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange12OrLater(65536, ADOwaVirtualDirectorySchema.ADMailboxFolderSet, "JunkEmailEnabled"), null, null);

		public static readonly ADPropertyDefinition UMIntegrationEnabled = new ADPropertyDefinition("UMIntegrationEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(131072, ADOwaVirtualDirectorySchema.ADMailboxFolderSet), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange12OrLater(131072, ADOwaVirtualDirectorySchema.ADMailboxFolderSet, "UMIntegrationEnabled"), null, null);

		public static readonly ADPropertyDefinition WSSAccessOnPublicComputersEnabled = new ADPropertyDefinition("WSSAccessOnPublicComputersEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(262144, ADOwaVirtualDirectorySchema.ADMailboxFolderSet), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange12OrLater(262144, ADOwaVirtualDirectorySchema.ADMailboxFolderSet, "WSSAccessOnPublicComputersEnabled"), null, null);

		public static readonly ADPropertyDefinition WSSAccessOnPrivateComputersEnabled = new ADPropertyDefinition("WSSAccessOnPrivateComputersEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(524288, ADOwaVirtualDirectorySchema.ADMailboxFolderSet), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange12OrLater(524288, ADOwaVirtualDirectorySchema.ADMailboxFolderSet, "WSSAccessOnPrivateComputersEnabled"), null, null);

		public static readonly ADPropertyDefinition ChangePasswordEnabled = new ADPropertyDefinition("ChangePasswordEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(67108864, ADOwaVirtualDirectorySchema.ADMailboxFolderSet), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange12OrLater(67108864, ADOwaVirtualDirectorySchema.ADMailboxFolderSet, "ChangePasswordEnabled"), null, null);

		public static readonly ADPropertyDefinition UNCAccessOnPublicComputersEnabled = new ADPropertyDefinition("UNCAccessOnPublicComputersEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(1048576, ADOwaVirtualDirectorySchema.ADMailboxFolderSet), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange12OrLater(1048576, ADOwaVirtualDirectorySchema.ADMailboxFolderSet, "UNCAccessOnPublicComputersEnabled"), null, null);

		public static readonly ADPropertyDefinition UNCAccessOnPrivateComputersEnabled = new ADPropertyDefinition("UNCAccessOnPrivateComputersEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(2097152, ADOwaVirtualDirectorySchema.ADMailboxFolderSet), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange12OrLater(2097152, ADOwaVirtualDirectorySchema.ADMailboxFolderSet, "UNCAccessOnPrivateComputersEnabled"), null, null);

		public static readonly ADPropertyDefinition ActiveSyncIntegrationEnabled = new ADPropertyDefinition("ActiveSyncIntegrationEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(4194304, ADOwaVirtualDirectorySchema.ADMailboxFolderSet), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange12OrLater(4194304, ADOwaVirtualDirectorySchema.ADMailboxFolderSet, "ActiveSyncIntegrationEnabled"), null, null);

		public static readonly ADPropertyDefinition AllAddressListsEnabled = new ADPropertyDefinition("AllAddressListsEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(16777216, ADOwaVirtualDirectorySchema.ADMailboxFolderSet), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange12OrLater(16777216, ADOwaVirtualDirectorySchema.ADMailboxFolderSet, "AllAddressListsEnabled"), null, null);

		public static readonly ADPropertyDefinition RulesEnabled = new ADPropertyDefinition("RulesEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(16384, ADOwaVirtualDirectorySchema.ADMailboxFolderSet), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange12OrLater(16384, ADOwaVirtualDirectorySchema.ADMailboxFolderSet, "RulesEnabled"), null, null);

		public static readonly ADPropertyDefinition PublicFoldersEnabled = new ADPropertyDefinition("PublicFoldersEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(64, ADOwaVirtualDirectorySchema.ADMailboxFolderSet), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange12OrLater(64, ADOwaVirtualDirectorySchema.ADMailboxFolderSet, "PublicFoldersEnabled"), null, null);

		public static readonly ADPropertyDefinition SMimeEnabled = new ADPropertyDefinition("SMimeEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(2048, ADOwaVirtualDirectorySchema.ADMailboxFolderSet), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange12OrLater(2048, ADOwaVirtualDirectorySchema.ADMailboxFolderSet, "SMimeEnabled"), null, null);

		public static readonly ADPropertyDefinition RecoverDeletedItemsEnabled = new ADPropertyDefinition("RecoverDeletedItemsEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(33554432, ADOwaVirtualDirectorySchema.ADMailboxFolderSet), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange12OrLater(33554432, ADOwaVirtualDirectorySchema.ADMailboxFolderSet, "RecoverDeletedItemsEnabled"), null, null);

		public static readonly ADPropertyDefinition InstantMessagingEnabled = new ADPropertyDefinition("InstantMessagingEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(134217728, ADOwaVirtualDirectorySchema.ADMailboxFolderSet), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14OrLater(134217728, ADOwaVirtualDirectorySchema.ADMailboxFolderSet, "InstantMessagingEnabled"), null, null);

		public static readonly ADPropertyDefinition TextMessagingEnabled = new ADPropertyDefinition("TextMessagingEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(268435456, ADOwaVirtualDirectorySchema.ADMailboxFolderSet), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14OrLater(268435456, ADOwaVirtualDirectorySchema.ADMailboxFolderSet, "TextMessagingEnabled"), null, null);

		public static readonly ADPropertyDefinition ForceSaveAttachmentFilteringEnabled = new ADPropertyDefinition("ForceSaveAttachmentFilteringEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, (bool)OwaMailboxPolicySchema.ForceSaveAttachmentFilteringEnabled.DefaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet2,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(1, ADOwaVirtualDirectorySchema.ADMailboxFolderSet2), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14OrLater(1, ADOwaVirtualDirectorySchema.ADMailboxFolderSet2, "ForceSaveAttachmentFilteringEnabled"), null, null);

		public static readonly ADPropertyDefinition SilverlightEnabled = new ADPropertyDefinition("SilverlightEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, (bool)OwaMailboxPolicySchema.SilverlightEnabled.DefaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet2,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(2, ADOwaVirtualDirectorySchema.ADMailboxFolderSet2), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14OrLater(2, ADOwaVirtualDirectorySchema.ADMailboxFolderSet2, "SilverlightEnabled"), null, null);

		public static readonly ADPropertyDefinition PlacesEnabled = new ADPropertyDefinition("PlacesEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.PersistDefaultValue, (bool)OwaMailboxPolicySchema.PlacesEnabled.DefaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet2,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(64, ADOwaVirtualDirectorySchema.ADMailboxFolderSet2), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14OrLater(64, ADOwaVirtualDirectorySchema.ADMailboxFolderSet2, "PlacesEnabled"), null, null);

		public static readonly ADPropertyDefinition WeatherEnabled = new ADPropertyDefinition("WeatherEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.PersistDefaultValue, (bool)OwaMailboxPolicySchema.WeatherEnabled.DefaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet2,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(67108864, ADOwaVirtualDirectorySchema.ADMailboxFolderSet2), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14OrLater(67108864, ADOwaVirtualDirectorySchema.ADMailboxFolderSet2, "WeatherEnabled"), null, null);

		public static readonly ADPropertyDefinition AllowCopyContactsToDeviceAddressBook = new ADPropertyDefinition("AllowCopyContactsToDeviceAddressBook", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet2,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(4194304, ADOwaVirtualDirectorySchema.ADMailboxFolderSet2), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14OrLater(4194304, ADOwaVirtualDirectorySchema.ADMailboxFolderSet2, "AllowCopyContactsToDeviceAddressBook"), null, null);

		public static readonly ADPropertyDefinition LogonPagePublicPrivateSelectionEnabled = new ADPropertyDefinition("LogonPagePublicPrivateSelectionEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet2,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(4096, ADOwaVirtualDirectorySchema.ADMailboxFolderSet2), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14OrLater(4096, ADOwaVirtualDirectorySchema.ADMailboxFolderSet2, "LogonPagePublicPrivateSelectionEnabled"), null, null);

		public static readonly ADPropertyDefinition LogonPageLightSelectionEnabled = new ADPropertyDefinition("LogonPageLightSelectionEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet2,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(2048, ADOwaVirtualDirectorySchema.ADMailboxFolderSet2), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14OrLater(2048, ADOwaVirtualDirectorySchema.ADMailboxFolderSet2, "LogonPageLightSelectionEnabled"), null, null);

		public static readonly ADPropertyDefinition AllowOfflineOn = OwaMailboxPolicySchema.AllowOfflineOnProperty("AllowOfflineOn", AllowOfflineOnEnum.AllComputers, ADOwaVirtualDirectorySchema.ADMailboxFolderSet2);

		public static readonly ADPropertyDefinition AnonymousFeaturesEnabled = new ADPropertyDefinition("AnonymousFeaturesEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet2,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(4, ADOwaVirtualDirectorySchema.ADMailboxFolderSet2), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14OrLater(4, ADOwaVirtualDirectorySchema.ADMailboxFolderSet2, "AnonymousFeaturesEnabled"), null, null);

		public static readonly ADPropertyDefinition IntegratedFeaturesEnabled = new ADPropertyDefinition("IntegratedFeaturesEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet2,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(16384, ADOwaVirtualDirectorySchema.ADMailboxFolderSet2), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14OrLater(16384, ADOwaVirtualDirectorySchema.ADMailboxFolderSet2, "IntegratedFeaturesEnabled"), null, null);

		public static readonly ADPropertyDefinition DisplayPhotosEnabled = new ADPropertyDefinition("DisplayPhotosEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, OwaMailboxPolicySchema.DefaultSenderPhotoEnabled, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet2,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(512, ADOwaVirtualDirectorySchema.ADMailboxFolderSet2), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14OrLater(512, ADOwaVirtualDirectorySchema.ADMailboxFolderSet2, "DisplayPhotosEnabled"), null, null);

		public static readonly ADPropertyDefinition SetPhotoEnabled = new ADPropertyDefinition("SetPhotoEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, OwaMailboxPolicySchema.DefaultSenderPhotoEnabled, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.ADMailboxFolderSet2,
			ADOwaVirtualDirectorySchema.OwaVersion
		}, null, ADObject.FlagGetterDelegate(1024, ADOwaVirtualDirectorySchema.ADMailboxFolderSet2), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14OrLater(1024, ADOwaVirtualDirectorySchema.ADMailboxFolderSet2, "SetPhotoEnabled"), null, null);

		public static readonly ADPropertyDefinition PredictedActionsEnabled = new ADPropertyDefinition("PredictedActionsEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.PersistDefaultValue, (bool)OwaMailboxPolicySchema.PredictedActionsEnabled.DefaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet2,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(8192, ADOwaVirtualDirectorySchema.ADMailboxFolderSet2), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14OrLater(8192, ADOwaVirtualDirectorySchema.ADMailboxFolderSet2, "PredictedActionsEnabled"), null, null);

		public static readonly ADPropertyDefinition UserDiagnosticEnabled = new ADPropertyDefinition("UserDiagnosticEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet2,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(32768, ADOwaVirtualDirectorySchema.ADMailboxFolderSet2), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14OrLater(32768, ADOwaVirtualDirectorySchema.ADMailboxFolderSet2, "UserDiagnosticEnabled"), null, null);

		public static readonly ADPropertyDefinition ReportJunkEmailEnabled = new ADPropertyDefinition("ReportJunkEmailEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet2,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(8388608, ADOwaVirtualDirectorySchema.ADMailboxFolderSet2), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14OrLater(8388608, ADOwaVirtualDirectorySchema.ADMailboxFolderSet2, "ReportJunkEmailEnabled"), null, null);

		public static readonly ADPropertyDefinition WebPartsFrameOptionsType = ADObject.BitfieldProperty("WebPartsFrameOptionsType", 20, 2, OwaMailboxPolicySchema.ADMailboxFolderSet2);

		public static readonly ADPropertyDefinition DirectFileAccessOnPublicComputersEnabled = new ADPropertyDefinition("DirectFileAccessOnPublicComputersEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.FileAccessControlOnPublicComputers
		}, null, ADObject.FlagGetterDelegate(1, ADOwaVirtualDirectorySchema.FileAccessControlOnPublicComputers), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange12OrLater(1, ADOwaVirtualDirectorySchema.FileAccessControlOnPublicComputers, "DirectFileAccessOnPublicComputersEnabled"), null, null);

		public static readonly ADPropertyDefinition DirectFileAccessOnPrivateComputersEnabled = new ADPropertyDefinition("DirectFileAccessOnPrivateComputersEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.FileAccessControlOnPrivateComputers
		}, null, ADObject.FlagGetterDelegate(1, ADOwaVirtualDirectorySchema.FileAccessControlOnPrivateComputers), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange12OrLater(1, ADOwaVirtualDirectorySchema.FileAccessControlOnPrivateComputers, "DirectFileAccessOnPrivateComputersEnabled"), null, null);

		public static readonly ADPropertyDefinition WebReadyDocumentViewingOnPublicComputersEnabled = new ADPropertyDefinition("WebReadyDocumentViewingOnPublicComputersEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.FileAccessControlOnPublicComputers
		}, null, ADObject.FlagGetterDelegate(2, ADOwaVirtualDirectorySchema.FileAccessControlOnPublicComputers), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange12OrLater(2, ADOwaVirtualDirectorySchema.FileAccessControlOnPublicComputers, "WebReadyDocumentViewingOnPublicComputersEnabled"), null, null);

		public static readonly ADPropertyDefinition WebReadyDocumentViewingOnPrivateComputersEnabled = new ADPropertyDefinition("WebReadyDocumentViewingOnPrivateComputersEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.FileAccessControlOnPrivateComputers
		}, null, ADObject.FlagGetterDelegate(2, ADOwaVirtualDirectorySchema.FileAccessControlOnPrivateComputers), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange12OrLater(2, ADOwaVirtualDirectorySchema.FileAccessControlOnPrivateComputers, "WebReadyDocumentViewingOnPrivateComputersEnabled"), null, null);

		public static readonly ADPropertyDefinition ForceWebReadyDocumentViewingFirstOnPublicComputers = new ADPropertyDefinition("ForceWebReadyDocumentViewingFirstOnPublicComputers", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.FileAccessControlOnPublicComputers
		}, null, ADObject.FlagGetterDelegate(4, ADOwaVirtualDirectorySchema.FileAccessControlOnPublicComputers), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange12OrLater(4, ADOwaVirtualDirectorySchema.FileAccessControlOnPublicComputers, "ForceWebReadyDocumentViewingFirstOnPublicComputers"), null, null);

		public static readonly ADPropertyDefinition ForceWebReadyDocumentViewingFirstOnPrivateComputers = new ADPropertyDefinition("ForceWebReadyDocumentViewingFirstOnPrivateComputers", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.FileAccessControlOnPrivateComputers
		}, null, ADObject.FlagGetterDelegate(4, ADOwaVirtualDirectorySchema.FileAccessControlOnPrivateComputers), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange12OrLater(4, ADOwaVirtualDirectorySchema.FileAccessControlOnPrivateComputers, "ForceWebReadyDocumentViewingFirstOnPrivateComputers"), null, null);

		public static readonly ADPropertyDefinition WacViewingOnPublicComputersEnabled = new ADPropertyDefinition("WacViewingOnPublicComputersEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.FileAccessControlOnPublicComputers
		}, null, ADObject.InvertFlagGetterDelegate(ADOwaVirtualDirectorySchema.FileAccessControlOnPublicComputers, 8), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14OrLater(8, ADOwaVirtualDirectorySchema.FileAccessControlOnPublicComputers, "WacViewingOnPublicComputersEnabled", true), null, null);

		public static readonly ADPropertyDefinition WacViewingOnPrivateComputersEnabled = new ADPropertyDefinition("WacViewingOnPrivateComputersEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.FileAccessControlOnPrivateComputers
		}, null, ADObject.InvertFlagGetterDelegate(ADOwaVirtualDirectorySchema.FileAccessControlOnPrivateComputers, 8), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14OrLater(8, ADOwaVirtualDirectorySchema.FileAccessControlOnPrivateComputers, "WacViewingOnPrivateComputersEnabled", true), null, null);

		public static readonly ADPropertyDefinition ForceWacViewingFirstOnPublicComputers = new ADPropertyDefinition("ForceWacViewingFirstOnPublicComputers", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.FileAccessControlOnPublicComputers
		}, null, ADObject.FlagGetterDelegate(16, ADOwaVirtualDirectorySchema.FileAccessControlOnPublicComputers), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14OrLater(16, ADOwaVirtualDirectorySchema.FileAccessControlOnPublicComputers, "ForceWacViewingFirstOnPublicComputers"), null, null);

		public static readonly ADPropertyDefinition ForceWacViewingFirstOnPrivateComputers = new ADPropertyDefinition("ForceWacViewingFirstOnPrivateComputers", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOwaVirtualDirectorySchema.FileAccessControlOnPrivateComputers
		}, null, ADObject.FlagGetterDelegate(16, ADOwaVirtualDirectorySchema.FileAccessControlOnPrivateComputers), ADOwaVirtualDirectorySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14OrLater(16, ADOwaVirtualDirectorySchema.FileAccessControlOnPrivateComputers, "ForceWacViewingFirstOnPrivateComputers"), null, null);

		public static readonly ADPropertyDefinition Exchange2003Url = new ADPropertyDefinition("Exchange2003Url", ExchangeObjectVersion.Exchange2010, typeof(Uri), "msExch2003Url", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, null, null);

		public static readonly ADPropertyDefinition FailbackUrl = new ADPropertyDefinition("FailbackUrl", ExchangeObjectVersion.Exchange2010, typeof(Uri), "msExchOWAFailbackURL", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, null, null);

		private struct Defaults
		{
			internal const RemoteDocumentsActions RemoteDocumentsActionForUnknownServers = RemoteDocumentsActions.Block;

			internal const LogonFormats LogonFormat = LogonFormats.FullDomain;

			internal const ClientAuthCleanupLevels ClientAuthCleanupLevel = ClientAuthCleanupLevels.High;

			internal const WebBeaconFilterLevels FilterWebBeaconsAndHtmlForms = WebBeaconFilterLevels.UserFilterChoice;

			internal const ExchwebProxyDestinations ExchwebProxyDestination = ExchwebProxyDestinations.NotSpecified;

			internal const VirtualDirectoryTypes VirtualDirectoryType = VirtualDirectoryTypes.NotSpecified;

			internal const OwaVersions OwaVersion = OwaVersions.Exchange2013;

			internal const RedirectToOptimalOWAServerOptions RedirectToOptimalOWAServer = RedirectToOptimalOWAServerOptions.Enabled;
		}
	}
}
