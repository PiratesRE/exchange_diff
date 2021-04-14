using System;
using Microsoft.Exchange.Common;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class OwaMailboxPolicySchema : MailboxPolicySchema
	{
		internal static ADPropertyDefinition AllowOfflineOnProperty(string name, AllowOfflineOnEnum defaultValue, ADPropertyDefinition fieldProperty)
		{
			GetterDelegate getterDelegate = delegate(IPropertyBag bag)
			{
				object obj = null;
				if (!(bag as ADPropertyBag).TryGetField(fieldProperty, ref obj))
				{
					return defaultValue;
				}
				if (obj == null)
				{
					return defaultValue;
				}
				int num = (int)obj & 384;
				int num2 = num;
				AllowOfflineOnEnum allowOfflineOnEnum;
				if (num2 != 128)
				{
					if (num2 != 256)
					{
						if (num2 != 384)
						{
						}
						allowOfflineOnEnum = AllowOfflineOnEnum.AllComputers;
					}
					else
					{
						allowOfflineOnEnum = AllowOfflineOnEnum.NoComputers;
					}
				}
				else
				{
					allowOfflineOnEnum = AllowOfflineOnEnum.PrivateComputersOnly;
				}
				return allowOfflineOnEnum;
			};
			SetterDelegate setterDelegate = OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(fieldProperty, name, delegate(object value, IPropertyBag bag)
			{
				int num = (int)value;
				int num2 = (int)bag[fieldProperty];
				num2 &= -385;
				switch (num)
				{
				case 1:
					num2 |= 128;
					break;
				case 2:
					num2 |= 256;
					break;
				case 3:
					num2 |= 384;
					break;
				default:
					throw new DataValidationException(new PropertyValidationError(DataStrings.BadEnumValue(typeof(AllowOfflineOnEnum)), fieldProperty, value));
				}
				bag[fieldProperty] = num2;
			});
			return new ADPropertyDefinition(name, fieldProperty.VersionAdded, typeof(AllowOfflineOnEnum), null, ADPropertyDefinitionFlags.Calculated, defaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ADPropertyDefinition[]
			{
				fieldProperty
			}, null, getterDelegate, setterDelegate, null, null);
		}

		private static SetterDelegate FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(int mask, PropertyDefinition propertyDefinition, string description)
		{
			return OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(mask, propertyDefinition, description, false);
		}

		private static SetterDelegate FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(int mask, PropertyDefinition propertyDefinition, string description, bool invert)
		{
			return OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(propertyDefinition, description, delegate(object value, IPropertyBag bag)
			{
				bool flag = invert ? (!(bool)value) : ((bool)value);
				int num = (int)bag[propertyDefinition];
				bag[propertyDefinition] = (flag ? (num | mask) : (num & ~mask));
			});
		}

		private static SetterDelegate FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(PropertyDefinition propertyDefinition, string description, SetterDelegate callback)
		{
			return delegate(object value, IPropertyBag bag)
			{
				if (!((ExchangeObjectVersion)bag[ADObjectSchema.ExchangeVersion]).Equals(ExchangeObjectVersion.Exchange2010))
				{
					throw new DataValidationException(new PropertyValidationError(DirectoryStrings.ExceptionOwaCannotSetPropertyOnLegacyMailboxPolicy(description), propertyDefinition, value), null);
				}
				if (value != null)
				{
					callback(value, bag);
					return;
				}
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.ExceptionOwaCannotSetPropertyOnE14MailboxPolicyToNull(description), propertyDefinition, value), null);
			};
		}

		private const string FileExtensionRegularExpression = "^\\.([^.]+)$";

		internal static readonly string[] DefaultWebReadyFileTypes = new string[]
		{
			".doc",
			".dot",
			".rtf",
			".xls",
			".ppt",
			".pps",
			".pdf",
			".docx",
			".xlsx",
			".pptx"
		};

		internal static readonly string[] DefaultWebReadyMimeTypes = new string[]
		{
			"application/msword",
			"application/vnd.ms-excel",
			"application/x-msexcel",
			"application/vnd.ms-powerpoint",
			"application/x-mspowerpoint",
			"application/pdf",
			"application/vnd.openxmlformats-officedocument.wordprocessingml.document",
			"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
			"application/vnd.openxmlformats-officedocument.presentationml.presentation"
		};

		internal static readonly string[] DefaultAllowedFileTypes = new string[]
		{
			".jpg",
			".gif",
			".bmp",
			".png",
			".tif",
			".tiff",
			".pdf",
			".txt",
			".rtf",
			".wav",
			".mp3",
			".avi",
			".wmv",
			".wma",
			".doc",
			".xls",
			".ppt",
			".vsd",
			".vss",
			".vst",
			".vdx",
			".vsx",
			".vtx",
			".vsdx",
			".vssx",
			".vstx",
			".vsdm",
			".vssm",
			".vstm",
			".one",
			".pub",
			".rpmsg",
			".docx",
			".docm",
			".xlsx",
			".xlsm",
			".xlsb",
			".pptx",
			".pptm",
			".ppsx",
			".ppsm",
			".zip"
		};

		internal static readonly string[] DefaultAllowedMimeTypes = new string[]
		{
			"image/bmp",
			"image/gif",
			"image/jpeg",
			"image/png"
		};

		internal static readonly string[] DefaultForceSaveFileTypes = new string[]
		{
			".dir",
			".dcr",
			".spl",
			".swf",
			".htm",
			".html"
		};

		internal static readonly string[] DefaultForceSaveMimeTypes = new string[]
		{
			"Application/octet-stream",
			"Application/x-shockwave-flash",
			"Application/futuresplash",
			"Application/x-director",
			"text/html"
		};

		internal static readonly string[] DefaultBlockedFileTypes = new string[]
		{
			".ade",
			".adp",
			".app",
			".asp",
			".aspx",
			".asx",
			".bas",
			".bat",
			".cer",
			".chm",
			".cmd",
			".cnt",
			".com",
			".cpl",
			".crt",
			".csh",
			".der",
			".exe",
			".fxp",
			".gadget",
			".hlp",
			".hpj",
			".hta",
			".htc",
			".inf",
			".ins",
			".isp",
			".its",
			".js",
			".jse",
			".ksh",
			".lnk",
			".mad",
			".maf",
			".mag",
			".mam",
			".maq",
			".mar",
			".mas",
			".mat",
			".mau",
			".mav",
			".maw",
			".mda",
			".mdb",
			".mde",
			".mdt",
			".mdw",
			".mdz",
			".mht",
			".mhtml",
			".msc",
			".msh",
			".msh1",
			".msh1xml",
			".msh2",
			".msh2xml",
			".mshxml",
			".msi",
			".msp",
			".mst",
			".ops",
			".osd",
			".pcd",
			".pif",
			".plg",
			".prf",
			".prg",
			".ps1",
			".ps1xml",
			".ps2",
			".ps2xml",
			".psc1",
			".psc2",
			".pst",
			".reg",
			".scf",
			".scr",
			".sct",
			".shb",
			".shs",
			".tmp",
			".url",
			".vb",
			".vbe",
			".vbp",
			".vbs",
			".vsmacros",
			".vsw",
			".ws",
			".wsc",
			".wsf",
			".wsh",
			".xml"
		};

		internal static readonly string[] DefaultBlockedMimeTypes = new string[]
		{
			"application/hta",
			"x-internet-signup",
			"application/javascript",
			"application/x-javascript",
			"text/javascript",
			"application/msaccess",
			"application/prg",
			"application/xml",
			"text/scriplet",
			"text/xml"
		};

		internal static readonly bool DefaultForceSaveAttachmentFilteringEnabled = false;

		internal static readonly bool DefaultSilverlightEnabled = true;

		internal static readonly bool DefaultPlacesEnabled = Datacenter.IsMicrosoftHostedOnly(true);

		internal static readonly bool DefaultWeatherEnabled = Datacenter.IsMicrosoftHostedOnly(true);

		internal static readonly bool DefaultSenderPhotoEnabled = true;

		internal static readonly bool DefaultPredictedActionsEnabled = false;

		public static readonly ADPropertyDefinition FileAccessControlOnPublicComputers = new ADPropertyDefinition("FileAccessControlOnPublicComputers", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchOWAFileAccessControlOnPublicComputers", ADPropertyDefinitionFlags.PersistDefaultValue, 3, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition FileAccessControlOnPrivateComputers = new ADPropertyDefinition("FileAccessControlOnPrivateComputers", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchOWAFileAccessControlOnPrivateComputers", ADPropertyDefinitionFlags.PersistDefaultValue, 3, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ActionForUnknownFileAndMIMETypes = new ADPropertyDefinition("ActionForUnknownFileAndMIMETypes", ExchangeObjectVersion.Exchange2010, typeof(AttachmentBlockingActions), "msExchOWAActionForUnknownFileAndMIMETypes", ADPropertyDefinitionFlags.None, AttachmentBlockingActions.ForceSave, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition ADWebReadyFileTypes = SharedPropertyDefinitions.ADWebReadyFileTypes;

		public static readonly ADPropertyDefinition WebReadyFileTypes = new ADPropertyDefinition("WebReadyFileTypes", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, new PropertyDefinitionConstraint[]
		{
			new InStringArrayConstraint(OwaMailboxPolicySchema.DefaultWebReadyFileTypes, true)
		}, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADWebReadyFileTypes,
			ADObjectSchema.Id
		}, null, new GetterDelegate(OwaMailboxPolicy.WebReadyFileTypesGetter), new SetterDelegate(OwaMailboxPolicy.WebReadyFileTypesSetter), null, null);

		internal static readonly ADPropertyDefinition ADWebReadyMimeTypes = SharedPropertyDefinitions.ADWebReadyMimeTypes;

		public static readonly ADPropertyDefinition WebReadyMimeTypes = new ADPropertyDefinition("WebReadyMimeTypes", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, new PropertyDefinitionConstraint[]
		{
			new InStringArrayConstraint(OwaMailboxPolicySchema.DefaultWebReadyMimeTypes, true)
		}, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADWebReadyMimeTypes,
			ADObjectSchema.Id
		}, null, new GetterDelegate(OwaMailboxPolicy.WebReadyMimeTypesGetter), new SetterDelegate(OwaMailboxPolicy.WebReadyMimeTypesSetter), null, null);

		internal static readonly ADPropertyDefinition ADWebReadyDocumentViewingFlags = new ADPropertyDefinition("ADWebReadyDocumentViewingFlags", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchOWATranscodingFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 1, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition WebReadyDocumentViewingForAllSupportedTypes = new ADPropertyDefinition("WebReadyDocumentViewingForAllSupportedTypes", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADWebReadyDocumentViewingFlags
		}, null, ADObject.FlagGetterDelegate(1, OwaMailboxPolicySchema.ADWebReadyDocumentViewingFlags), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(1, OwaMailboxPolicySchema.ADWebReadyDocumentViewingFlags, "WebReadyDocumentViewingForAllSupportedTypes"), null, null);

		internal static readonly ADPropertyDefinition ADAllowedFileTypes = SharedPropertyDefinitions.ADAllowedFileTypes;

		public static readonly ADPropertyDefinition AllowedFileTypes = new ADPropertyDefinition("AllowedFileTypes", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RegexConstraint("^\\.([^.]+)$", DataStrings.FileExtensionPatternDescription)
		}, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADAllowedFileTypes,
			ADObjectSchema.Id
		}, null, new GetterDelegate(OwaMailboxPolicy.AllowedFileTypesGetter), new SetterDelegate(OwaMailboxPolicy.AllowedFileTypesSetter), null, null);

		internal static readonly ADPropertyDefinition ADAllowedMimeTypes = SharedPropertyDefinitions.ADAllowedMimeTypes;

		public static readonly ADPropertyDefinition AllowedMimeTypes = new ADPropertyDefinition("AllowedMimeTypes", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADAllowedMimeTypes,
			ADObjectSchema.Id
		}, null, new GetterDelegate(OwaMailboxPolicy.AllowedMimeTypesGetter), new SetterDelegate(OwaMailboxPolicy.AllowedMimeTypesSetter), null, null);

		internal static readonly ADPropertyDefinition ADForceSaveFileTypes = SharedPropertyDefinitions.ADForceSaveFileTypes;

		public static readonly ADPropertyDefinition ForceSaveFileTypes = new ADPropertyDefinition("ForceSaveFileTypes", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RegexConstraint("^\\.([^.]+)$", DataStrings.FileExtensionPatternDescription)
		}, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADForceSaveFileTypes,
			ADObjectSchema.Id
		}, null, new GetterDelegate(OwaMailboxPolicy.ForceSaveFileTypesGetter), new SetterDelegate(OwaMailboxPolicy.ForceSaveFileTypesSetter), null, null);

		internal static readonly ADPropertyDefinition ADForceSaveMimeTypes = SharedPropertyDefinitions.ADForceSaveMimeTypes;

		public static readonly ADPropertyDefinition ForceSaveMimeTypes = new ADPropertyDefinition("ForceSaveMimeTypes", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADForceSaveMimeTypes,
			ADObjectSchema.Id
		}, null, new GetterDelegate(OwaMailboxPolicy.ForceSaveMimeTypesGetter), new SetterDelegate(OwaMailboxPolicy.ForceSaveMimeTypesSetter), null, null);

		internal static readonly ADPropertyDefinition ADBlockedFileTypes = SharedPropertyDefinitions.ADBlockedFileTypes;

		public static readonly ADPropertyDefinition BlockedFileTypes = new ADPropertyDefinition("BlockedFileTypes", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RegexConstraint("^\\.([^.]+)$", DataStrings.FileExtensionPatternDescription)
		}, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADBlockedFileTypes,
			ADObjectSchema.Id
		}, null, new GetterDelegate(OwaMailboxPolicy.BlockedFileTypesGetter), new SetterDelegate(OwaMailboxPolicy.BlockedFileTypesSetter), null, null);

		internal static readonly ADPropertyDefinition ADBlockedMimeTypes = SharedPropertyDefinitions.ADBlockedMimeTypes;

		public static readonly ADPropertyDefinition BlockedMimeTypes = new ADPropertyDefinition("BlockedMimeTypes", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADBlockedMimeTypes,
			ADObjectSchema.Id
		}, null, new GetterDelegate(OwaMailboxPolicy.BlockedMimeTypesGetter), new SetterDelegate(OwaMailboxPolicy.BlockedMimeTypesSetter), null, null);

		public static readonly ADPropertyDefinition PhoneticSupportEnabled = new ADPropertyDefinition("PhoneticSupportEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), "msExchPhoneticSupport", ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DefaultTheme = new ADPropertyDefinition("DefaultTheme", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchOWADefaultTheme", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition SetPhotoURL = new ADPropertyDefinition("SetPhotoURL", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchAggregationSubscriptionCredential", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IsDefault = new ADPropertyDefinition("IsDefault", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MailboxPolicySchema.MailboxPolicyFlags
		}, new CustomFilterBuilderDelegate(OwaMailboxPolicy.IsDefaultFilterBuilder), ADObject.FlagGetterDelegate(MailboxPolicySchema.MailboxPolicyFlags, 1), ADObject.FlagSetterDelegate(MailboxPolicySchema.MailboxPolicyFlags, 1), null, null);

		public static readonly ADPropertyDefinition DefaultClientLanguage = new ADPropertyDefinition("DefaultClientLanguage", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchOWADefaultClientLanguage", ADPropertyDefinitionFlags.PersistDefaultValue, 0, new PropertyDefinitionConstraint[]
		{
			new OWASupportedLanguageConstraint()
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LogonAndErrorLanguage = new ADPropertyDefinition("LogonAndErrorLanguage", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchOWALogonAndErrorLanguage", ADPropertyDefinitionFlags.PersistDefaultValue, 0, new PropertyDefinitionConstraint[]
		{
			new OWASupportedLanguageConstraint()
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UseGB18030 = new ADPropertyDefinition("UseGB18030", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchOWAUseGB18030", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UseISO885915 = new ADPropertyDefinition("UseISO885915", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchOWAUseISO885915", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OutboundCharset = new ADPropertyDefinition("OutboundCharset", ExchangeObjectVersion.Exchange2010, typeof(OutboundCharsetOptions), "msExchOWAOutboundCharset", ADPropertyDefinitionFlags.None, OutboundCharsetOptions.AutoDetect, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition InstantMessagingType = new ADPropertyDefinition("InstantMessagingType", ExchangeObjectVersion.Exchange2010, typeof(InstantMessagingTypeOptions), "msExchOWAIMProviderType", ADPropertyDefinitionFlags.None, InstantMessagingTypeOptions.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ADMailboxFolderSet = new ADPropertyDefinition("ADMailboxFolderSet", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchMailboxFolderSet", ADPropertyDefinitionFlags.PersistDefaultValue, -1, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ADMailboxFolderSet2 = new ADPropertyDefinition("ADMailboxFolderSet2", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchMailboxFolderSet2", ADPropertyDefinitionFlags.PersistDefaultValue, -63578, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition GlobalAddressListEnabled = new ADPropertyDefinition("GlobalAddressListEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(1, OwaMailboxPolicySchema.ADMailboxFolderSet), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(1, OwaMailboxPolicySchema.ADMailboxFolderSet, "GlobalAddressListEnabled"), null, null);

		public static readonly ADPropertyDefinition OrganizationEnabled = new ADPropertyDefinition("OrganizationEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(128, OwaMailboxPolicySchema.ADMailboxFolderSet), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(128, OwaMailboxPolicySchema.ADMailboxFolderSet, "OrganizationEnabled"), null, null);

		public static readonly ADPropertyDefinition ExplicitLogonEnabled = new ADPropertyDefinition("ExplicitLogonEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(8388608, OwaMailboxPolicySchema.ADMailboxFolderSet), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(8388608, OwaMailboxPolicySchema.ADMailboxFolderSet, "ExplicitLogonEnabled"), null, null);

		public static readonly ADPropertyDefinition OWALightEnabled = new ADPropertyDefinition("OWALightEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(536870912, OwaMailboxPolicySchema.ADMailboxFolderSet), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(536870912, OwaMailboxPolicySchema.ADMailboxFolderSet, "OWALightEnabled"), null, null);

		public static readonly ADPropertyDefinition DelegateAccessEnabled = new ADPropertyDefinition("DelegateAccessEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(1073741824, OwaMailboxPolicySchema.ADMailboxFolderSet), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(1073741824, OwaMailboxPolicySchema.ADMailboxFolderSet, "DelegateAccessEnabled"), null, null);

		public static readonly ADPropertyDefinition IRMEnabled = new ADPropertyDefinition("IRMEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(int.MinValue, OwaMailboxPolicySchema.ADMailboxFolderSet), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(int.MinValue, OwaMailboxPolicySchema.ADMailboxFolderSet, "IRMEnabled"), null, null);

		public static readonly ADPropertyDefinition CalendarEnabled = new ADPropertyDefinition("CalendarEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(2, OwaMailboxPolicySchema.ADMailboxFolderSet), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(2, OwaMailboxPolicySchema.ADMailboxFolderSet, "CalendarEnabled"), null, null);

		public static readonly ADPropertyDefinition ContactsEnabled = new ADPropertyDefinition("ContactsEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(4, OwaMailboxPolicySchema.ADMailboxFolderSet), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(4, OwaMailboxPolicySchema.ADMailboxFolderSet, "ContactsEnabled"), null, null);

		public static readonly ADPropertyDefinition TasksEnabled = new ADPropertyDefinition("TasksEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(8, OwaMailboxPolicySchema.ADMailboxFolderSet), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(8, OwaMailboxPolicySchema.ADMailboxFolderSet, "TasksEnabled"), null, null);

		public static readonly ADPropertyDefinition JournalEnabled = new ADPropertyDefinition("JournalEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(16, OwaMailboxPolicySchema.ADMailboxFolderSet), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(16, OwaMailboxPolicySchema.ADMailboxFolderSet, "JournalEnabled"), null, null);

		public static readonly ADPropertyDefinition NotesEnabled = new ADPropertyDefinition("NotesEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(32, OwaMailboxPolicySchema.ADMailboxFolderSet), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(32, OwaMailboxPolicySchema.ADMailboxFolderSet, "NotesEnabled"), null, null);

		public static readonly ADPropertyDefinition RemindersAndNotificationsEnabled = new ADPropertyDefinition("RemindersAndNotificationsEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(256, OwaMailboxPolicySchema.ADMailboxFolderSet), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(256, OwaMailboxPolicySchema.ADMailboxFolderSet, "RemindersAndNotificationsEnabled"), null, null);

		public static readonly ADPropertyDefinition PremiumClientEnabled = new ADPropertyDefinition("PremiumClientEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(512, OwaMailboxPolicySchema.ADMailboxFolderSet), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(512, OwaMailboxPolicySchema.ADMailboxFolderSet, "PremiumClientEnabled"), null, null);

		public static readonly ADPropertyDefinition SpellCheckerEnabled = new ADPropertyDefinition("SpellCheckerEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(1024, OwaMailboxPolicySchema.ADMailboxFolderSet), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(1024, OwaMailboxPolicySchema.ADMailboxFolderSet, "SpellCheckerEnabled"), null, null);

		public static readonly ADPropertyDefinition SearchFoldersEnabled = new ADPropertyDefinition("SearchFoldersEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(4096, OwaMailboxPolicySchema.ADMailboxFolderSet), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(4096, OwaMailboxPolicySchema.ADMailboxFolderSet, "SearchFoldersEnabled"), null, null);

		public static readonly ADPropertyDefinition SignaturesEnabled = new ADPropertyDefinition("SignaturesEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(8192, OwaMailboxPolicySchema.ADMailboxFolderSet), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(8192, OwaMailboxPolicySchema.ADMailboxFolderSet, "SignaturesEnabled"), null, null);

		public static readonly ADPropertyDefinition ThemeSelectionEnabled = new ADPropertyDefinition("ThemeSelectionEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(32768, OwaMailboxPolicySchema.ADMailboxFolderSet), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(32768, OwaMailboxPolicySchema.ADMailboxFolderSet, "ThemeSelectionEnabled"), null, null);

		public static readonly ADPropertyDefinition JunkEmailEnabled = new ADPropertyDefinition("JunkEmailEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(65536, OwaMailboxPolicySchema.ADMailboxFolderSet), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(65536, OwaMailboxPolicySchema.ADMailboxFolderSet, "JunkEmailEnabled"), null, null);

		public static readonly ADPropertyDefinition UMIntegrationEnabled = new ADPropertyDefinition("UMIntegrationEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(131072, OwaMailboxPolicySchema.ADMailboxFolderSet), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(131072, OwaMailboxPolicySchema.ADMailboxFolderSet, "UMIntegrationEnabled"), null, null);

		public static readonly ADPropertyDefinition WSSAccessOnPublicComputersEnabled = new ADPropertyDefinition("WSSAccessOnPublicComputersEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(262144, OwaMailboxPolicySchema.ADMailboxFolderSet), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(262144, OwaMailboxPolicySchema.ADMailboxFolderSet, "WSSAccessOnPublicComputersEnabled"), null, null);

		public static readonly ADPropertyDefinition WSSAccessOnPrivateComputersEnabled = new ADPropertyDefinition("WSSAccessOnPrivateComputersEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(524288, OwaMailboxPolicySchema.ADMailboxFolderSet), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(524288, OwaMailboxPolicySchema.ADMailboxFolderSet, "WSSAccessOnPrivateComputersEnabled"), null, null);

		public static readonly ADPropertyDefinition ChangePasswordEnabled = new ADPropertyDefinition("ChangePasswordEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(67108864, OwaMailboxPolicySchema.ADMailboxFolderSet), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(67108864, OwaMailboxPolicySchema.ADMailboxFolderSet, "ChangePasswordEnabled"), null, null);

		public static readonly ADPropertyDefinition UNCAccessOnPublicComputersEnabled = new ADPropertyDefinition("UNCAccessOnPublicComputersEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(1048576, OwaMailboxPolicySchema.ADMailboxFolderSet), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(1048576, OwaMailboxPolicySchema.ADMailboxFolderSet, "UNCAccessOnPublicComputersEnabled"), null, null);

		public static readonly ADPropertyDefinition UNCAccessOnPrivateComputersEnabled = new ADPropertyDefinition("UNCAccessOnPrivateComputersEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(2097152, OwaMailboxPolicySchema.ADMailboxFolderSet), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(2097152, OwaMailboxPolicySchema.ADMailboxFolderSet, "UNCAccessOnPrivateComputersEnabled"), null, null);

		public static readonly ADPropertyDefinition ActiveSyncIntegrationEnabled = new ADPropertyDefinition("ActiveSyncIntegrationEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(4194304, OwaMailboxPolicySchema.ADMailboxFolderSet), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(4194304, OwaMailboxPolicySchema.ADMailboxFolderSet, "ActiveSyncIntegrationEnabled"), null, null);

		public static readonly ADPropertyDefinition AllAddressListsEnabled = new ADPropertyDefinition("AllAddressListsEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(16777216, OwaMailboxPolicySchema.ADMailboxFolderSet), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(16777216, OwaMailboxPolicySchema.ADMailboxFolderSet, "AllAddressListsEnabled"), null, null);

		public static readonly ADPropertyDefinition RulesEnabled = new ADPropertyDefinition("RulesEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(16384, OwaMailboxPolicySchema.ADMailboxFolderSet), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(16384, OwaMailboxPolicySchema.ADMailboxFolderSet, "RulesEnabled"), null, null);

		public static readonly ADPropertyDefinition PublicFoldersEnabled = new ADPropertyDefinition("PublicFoldersEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(64, OwaMailboxPolicySchema.ADMailboxFolderSet), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(64, OwaMailboxPolicySchema.ADMailboxFolderSet, "PublicFoldersEnabled"), null, null);

		public static readonly ADPropertyDefinition SMimeEnabled = new ADPropertyDefinition("SMimeEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(2048, OwaMailboxPolicySchema.ADMailboxFolderSet), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(2048, OwaMailboxPolicySchema.ADMailboxFolderSet, "SMimeEnabled"), null, null);

		public static readonly ADPropertyDefinition RecoverDeletedItemsEnabled = new ADPropertyDefinition("RecoverDeletedItemsEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(33554432, OwaMailboxPolicySchema.ADMailboxFolderSet), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(33554432, OwaMailboxPolicySchema.ADMailboxFolderSet, "RecoverDeletedItemsEnabled"), null, null);

		public static readonly ADPropertyDefinition InstantMessagingEnabled = new ADPropertyDefinition("InstantMessagingEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(134217728, OwaMailboxPolicySchema.ADMailboxFolderSet), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(134217728, OwaMailboxPolicySchema.ADMailboxFolderSet, "InstantMessagingEnabled"), null, null);

		public static readonly ADPropertyDefinition TextMessagingEnabled = new ADPropertyDefinition("TextMessagingEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(268435456, OwaMailboxPolicySchema.ADMailboxFolderSet), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(268435456, OwaMailboxPolicySchema.ADMailboxFolderSet, "TextMessagingEnabled"), null, null);

		public static readonly ADPropertyDefinition DisplayPhotosEnabled = new ADPropertyDefinition("DisplayPhotosEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, OwaMailboxPolicySchema.DefaultSenderPhotoEnabled, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet2,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(512, OwaMailboxPolicySchema.ADMailboxFolderSet2), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(512, OwaMailboxPolicySchema.ADMailboxFolderSet2, "DisplayPhotosEnabled"), null, null);

		public static readonly ADPropertyDefinition SetPhotoEnabled = new ADPropertyDefinition("SetPhotoEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, OwaMailboxPolicySchema.DefaultSenderPhotoEnabled, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet2,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(1024, OwaMailboxPolicySchema.ADMailboxFolderSet2), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(1024, OwaMailboxPolicySchema.ADMailboxFolderSet2, "SetPhotoEnabled"), null, null);

		public static readonly ADPropertyDefinition AllowOfflineOn = OwaMailboxPolicySchema.AllowOfflineOnProperty("AllowOfflineOn", AllowOfflineOnEnum.AllComputers, OwaMailboxPolicySchema.ADMailboxFolderSet2);

		public static readonly ADPropertyDefinition DirectFileAccessOnPublicComputersEnabled = new ADPropertyDefinition("DirectFileAccessOnPublicComputersEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.FileAccessControlOnPublicComputers
		}, null, ADObject.FlagGetterDelegate(1, OwaMailboxPolicySchema.FileAccessControlOnPublicComputers), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(1, OwaMailboxPolicySchema.FileAccessControlOnPublicComputers, "DirectFileAccessOnPublicComputersEnabled"), null, null);

		public static readonly ADPropertyDefinition DirectFileAccessOnPrivateComputersEnabled = new ADPropertyDefinition("DirectFileAccessOnPrivateComputersEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.FileAccessControlOnPrivateComputers
		}, null, ADObject.FlagGetterDelegate(1, OwaMailboxPolicySchema.FileAccessControlOnPrivateComputers), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(1, OwaMailboxPolicySchema.FileAccessControlOnPrivateComputers, "DirectFileAccessOnPrivateComputersEnabled"), null, null);

		public static readonly ADPropertyDefinition WebReadyDocumentViewingOnPublicComputersEnabled = new ADPropertyDefinition("WebReadyDocumentViewingOnPublicComputersEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.FileAccessControlOnPublicComputers
		}, null, ADObject.FlagGetterDelegate(2, OwaMailboxPolicySchema.FileAccessControlOnPublicComputers), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(2, OwaMailboxPolicySchema.FileAccessControlOnPublicComputers, "WebReadyDocumentViewingOnPublicComputersEnabled"), null, null);

		public static readonly ADPropertyDefinition WebReadyDocumentViewingOnPrivateComputersEnabled = new ADPropertyDefinition("WebReadyDocumentViewingOnPrivateComputersEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.FileAccessControlOnPrivateComputers
		}, null, ADObject.FlagGetterDelegate(2, OwaMailboxPolicySchema.FileAccessControlOnPrivateComputers), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(2, OwaMailboxPolicySchema.FileAccessControlOnPrivateComputers, "WebReadyDocumentViewingOnPrivateComputersEnabled"), null, null);

		public static readonly ADPropertyDefinition ForceWebReadyDocumentViewingFirstOnPublicComputers = new ADPropertyDefinition("ForceWebReadyDocumentViewingFirstOnPublicComputers", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.FileAccessControlOnPublicComputers
		}, null, ADObject.FlagGetterDelegate(4, OwaMailboxPolicySchema.FileAccessControlOnPublicComputers), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(4, OwaMailboxPolicySchema.FileAccessControlOnPublicComputers, "ForceWebReadyDocumentViewingFirstOnPublicComputers"), null, null);

		public static readonly ADPropertyDefinition ForceWebReadyDocumentViewingFirstOnPrivateComputers = new ADPropertyDefinition("ForceWebReadyDocumentViewingFirstOnPrivateComputers", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.FileAccessControlOnPrivateComputers
		}, null, ADObject.FlagGetterDelegate(4, OwaMailboxPolicySchema.FileAccessControlOnPrivateComputers), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(4, OwaMailboxPolicySchema.FileAccessControlOnPrivateComputers, "ForceWebReadyDocumentViewingFirstOnPrivateComputers"), null, null);

		public static readonly ADPropertyDefinition WacViewingOnPublicComputersEnabled = new ADPropertyDefinition("WacViewingOnPublicComputersEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.FileAccessControlOnPublicComputers
		}, null, ADObject.InvertFlagGetterDelegate(OwaMailboxPolicySchema.FileAccessControlOnPublicComputers, 8), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(8, OwaMailboxPolicySchema.FileAccessControlOnPublicComputers, "WacViewingOnPublicComputersEnabled", true), null, null);

		public static readonly ADPropertyDefinition WacViewingOnPrivateComputersEnabled = new ADPropertyDefinition("WacViewingOnPrivateComputersEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.FileAccessControlOnPrivateComputers
		}, null, ADObject.InvertFlagGetterDelegate(OwaMailboxPolicySchema.FileAccessControlOnPrivateComputers, 8), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(8, OwaMailboxPolicySchema.FileAccessControlOnPrivateComputers, "WacViewingOnPrivateComputersEnabled", true), null, null);

		public static readonly ADPropertyDefinition ForceWacViewingFirstOnPublicComputers = new ADPropertyDefinition("ForceWacViewingFirstOnPublicComputers", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.FileAccessControlOnPublicComputers
		}, null, ADObject.FlagGetterDelegate(16, OwaMailboxPolicySchema.FileAccessControlOnPublicComputers), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(16, OwaMailboxPolicySchema.FileAccessControlOnPublicComputers, "ForceWacViewingFirstOnPublicComputers"), null, null);

		public static readonly ADPropertyDefinition ForceWacViewingFirstOnPrivateComputers = new ADPropertyDefinition("ForceWacViewingFirstOnPrivateComputers", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.FileAccessControlOnPrivateComputers
		}, null, ADObject.FlagGetterDelegate(16, OwaMailboxPolicySchema.FileAccessControlOnPrivateComputers), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(16, OwaMailboxPolicySchema.FileAccessControlOnPrivateComputers, "ForceWacViewingFirstOnPrivateComputers"), null, null);

		public static readonly ADPropertyDefinition ForceSaveAttachmentFilteringEnabled = new ADPropertyDefinition("ForceSaveAttachmentFilteringEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, OwaMailboxPolicySchema.DefaultForceSaveAttachmentFilteringEnabled, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet2,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(1, OwaMailboxPolicySchema.ADMailboxFolderSet2), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(1, OwaMailboxPolicySchema.ADMailboxFolderSet2, "ForceSaveAttachmentFilteringEnabled"), null, null);

		public static readonly ADPropertyDefinition SilverlightEnabled = new ADPropertyDefinition("SilverlightEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, OwaMailboxPolicySchema.DefaultSilverlightEnabled, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet2,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(2, OwaMailboxPolicySchema.ADMailboxFolderSet2), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(2, OwaMailboxPolicySchema.ADMailboxFolderSet2, "SilverlightEnabled"), null, null);

		public static readonly ADPropertyDefinition PlacesEnabled = new ADPropertyDefinition("PlacesEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.PersistDefaultValue, OwaMailboxPolicySchema.DefaultPlacesEnabled, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet2,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(64, OwaMailboxPolicySchema.ADMailboxFolderSet2), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(64, OwaMailboxPolicySchema.ADMailboxFolderSet2, "PlacesEnabled"), null, null);

		public static readonly ADPropertyDefinition WeatherEnabled = new ADPropertyDefinition("WeatherEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.PersistDefaultValue, OwaMailboxPolicySchema.DefaultWeatherEnabled, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet2,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(67108864, OwaMailboxPolicySchema.ADMailboxFolderSet2), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(67108864, OwaMailboxPolicySchema.ADMailboxFolderSet2, "WeatherEnabled"), null, null);

		public static readonly ADPropertyDefinition AllowCopyContactsToDeviceAddressBook = new ADPropertyDefinition("AllowCopyContactsToDeviceAddressBook", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet2,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(4194304, OwaMailboxPolicySchema.ADMailboxFolderSet2), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(4194304, OwaMailboxPolicySchema.ADMailboxFolderSet2, "AllowCopyContactsToDeviceAddressBook"), null, null);

		public static readonly ADPropertyDefinition PredictedActionsEnabled = new ADPropertyDefinition("PredictedActionsEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.PersistDefaultValue, OwaMailboxPolicySchema.DefaultPredictedActionsEnabled, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet2,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(8192, OwaMailboxPolicySchema.ADMailboxFolderSet2), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(8192, OwaMailboxPolicySchema.ADMailboxFolderSet2, "PredictedActionsEnabled"), null, null);

		public static readonly ADPropertyDefinition UserDiagnosticEnabled = new ADPropertyDefinition("UserDiagnosticEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet2,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(32768, OwaMailboxPolicySchema.ADMailboxFolderSet2), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(32768, OwaMailboxPolicySchema.ADMailboxFolderSet2, "UserDiagnosticEnabled"), null, null);

		public static readonly ADPropertyDefinition FacebookEnabled = new ADPropertyDefinition("FacebookEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.PersistDefaultValue, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet2,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(OwaMailboxPolicySchema.ADMailboxFolderSet2, 65536), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(65536, OwaMailboxPolicySchema.ADMailboxFolderSet2, "FacebookEnabled"), null, null);

		public static readonly ADPropertyDefinition LinkedInEnabled = new ADPropertyDefinition("LinkedInEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.PersistDefaultValue, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet2,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(OwaMailboxPolicySchema.ADMailboxFolderSet2, 131072), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(131072, OwaMailboxPolicySchema.ADMailboxFolderSet2, "LinkedInEnabled"), null, null);

		public static readonly ADPropertyDefinition WacExternalServicesEnabled = new ADPropertyDefinition("WacExternalServicesEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.PersistDefaultValue, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet2,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(OwaMailboxPolicySchema.ADMailboxFolderSet2, 262144), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(262144, OwaMailboxPolicySchema.ADMailboxFolderSet2, "WacExternalServicesEnabled"), null, null);

		public static readonly ADPropertyDefinition WacOMEXEnabled = new ADPropertyDefinition("WacOMEXEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet2,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(OwaMailboxPolicySchema.ADMailboxFolderSet2, 524288), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(524288, OwaMailboxPolicySchema.ADMailboxFolderSet2, "WacOMEXEnabledMask"), null, null);

		public static readonly ADPropertyDefinition ReportJunkEmailEnabled = new ADPropertyDefinition("ReportJunkEmailEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet2,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(OwaMailboxPolicySchema.ADMailboxFolderSet2, 8388608), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(8388608, OwaMailboxPolicySchema.ADMailboxFolderSet2, "ReportJunkEmailEnabledMask"), null, null);

		public static readonly ADPropertyDefinition GroupCreationEnabled = new ADPropertyDefinition("GroupCreationEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.PersistDefaultValue, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet2,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(OwaMailboxPolicySchema.ADMailboxFolderSet2, 16777216), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(16777216, OwaMailboxPolicySchema.ADMailboxFolderSet2, "GroupCreationEnabledMask"), null, null);

		public static readonly ADPropertyDefinition SkipCreateUnifiedGroupCustomSharepointClassification = new ADPropertyDefinition("SkipCreateUnifiedGroupCustomSharepointClassification", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.PersistDefaultValue, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OwaMailboxPolicySchema.ADMailboxFolderSet2,
			ADObjectSchema.ExchangeVersion
		}, null, ADObject.FlagGetterDelegate(OwaMailboxPolicySchema.ADMailboxFolderSet2, 33554432), OwaMailboxPolicySchema.FlagSetterDelegateWithValidationOnOwaVersionIsExchange14(33554432, OwaMailboxPolicySchema.ADMailboxFolderSet2, "SkipCreateUnifiedGroupCustomSharepointClassificationMask"), null, null);

		public static readonly ADPropertyDefinition WebPartsFrameOptionsType = ADObject.BitfieldProperty("WebPartsFrameOptionsType", 20, 2, OwaMailboxPolicySchema.ADMailboxFolderSet2);
	}
}
