using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using System.Text;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class DomainContentConfig : ADLegacyVersionableObject
	{
		public SmtpDomainWithSubdomains DomainName
		{
			get
			{
				return (SmtpDomainWithSubdomains)this[EdgeDomainContentConfigSchema.DomainName];
			}
			set
			{
				this[EdgeDomainContentConfigSchema.DomainName] = value;
			}
		}

		[Parameter]
		public bool IsInternal
		{
			get
			{
				return (bool)this[DomainContentConfigSchema.InternalDomain];
			}
			set
			{
				this[DomainContentConfigSchema.InternalDomain] = value;
			}
		}

		public bool TargetDeliveryDomain
		{
			get
			{
				return (bool)this[DomainContentConfigSchema.TargetDeliveryDomain];
			}
			set
			{
				this[DomainContentConfigSchema.TargetDeliveryDomain] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteEncoderTypeFor7BitCharsetsEnum ByteEncoderTypeFor7BitCharsets
		{
			get
			{
				return (ByteEncoderTypeFor7BitCharsetsEnum)this[DomainContentConfigSchema.ByteEncoderTypeFor7BitCharsets];
			}
			set
			{
				this[DomainContentConfigSchema.ByteEncoderTypeFor7BitCharsets] = value;
			}
		}

		[Parameter]
		public string CharacterSet
		{
			get
			{
				return (string)this[DomainContentConfigSchema.CharacterSet];
			}
			set
			{
				this[DomainContentConfigSchema.CharacterSet] = value;
			}
		}

		[Parameter]
		public string NonMimeCharacterSet
		{
			get
			{
				return (string)this[EdgeDomainContentConfigSchema.NonMimeCharacterSet];
			}
			set
			{
				this[EdgeDomainContentConfigSchema.NonMimeCharacterSet] = value;
			}
		}

		[Parameter]
		public AllowedOOFType AllowedOOFType
		{
			get
			{
				return (AllowedOOFType)this[DomainContentConfigSchema.AllowedOOFType];
			}
			set
			{
				this[DomainContentConfigSchema.AllowedOOFType] = value;
			}
		}

		[Parameter]
		public bool AutoReplyEnabled
		{
			get
			{
				return (bool)this[DomainContentConfigSchema.AutoReplyEnabled];
			}
			set
			{
				this[DomainContentConfigSchema.AutoReplyEnabled] = value;
			}
		}

		[Parameter]
		public bool AutoForwardEnabled
		{
			get
			{
				return (bool)this[DomainContentConfigSchema.AutoForwardEnabled];
			}
			set
			{
				this[DomainContentConfigSchema.AutoForwardEnabled] = value;
			}
		}

		[Parameter]
		public bool DeliveryReportEnabled
		{
			get
			{
				return (bool)this[DomainContentConfigSchema.DeliveryReportEnabled];
			}
			set
			{
				this[DomainContentConfigSchema.DeliveryReportEnabled] = value;
			}
		}

		[Parameter]
		public bool NDREnabled
		{
			get
			{
				return (bool)this[DomainContentConfigSchema.NDREnabled];
			}
			set
			{
				this[DomainContentConfigSchema.NDREnabled] = value;
			}
		}

		[Parameter]
		public bool MeetingForwardNotificationEnabled
		{
			get
			{
				return (bool)this[DomainContentConfigSchema.MFNEnabled];
			}
			set
			{
				this[DomainContentConfigSchema.MFNEnabled] = value;
			}
		}

		[Parameter]
		public ContentType ContentType
		{
			get
			{
				return (ContentType)this[DomainContentConfigSchema.ContentType];
			}
			set
			{
				this[DomainContentConfigSchema.ContentType] = value;
			}
		}

		[Parameter]
		public bool DisplaySenderName
		{
			get
			{
				return (bool)this[DomainContentConfigSchema.DisplaySenderName];
			}
			set
			{
				this[DomainContentConfigSchema.DisplaySenderName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public PreferredInternetCodePageForShiftJisEnum PreferredInternetCodePageForShiftJis
		{
			get
			{
				return (PreferredInternetCodePageForShiftJisEnum)this[DomainContentConfigSchema.PreferredInternetCodePageForShiftJis];
			}
			set
			{
				this[DomainContentConfigSchema.PreferredInternetCodePageForShiftJis] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? RequiredCharsetCoverage
		{
			get
			{
				return (int?)this[DomainContentConfigSchema.RequiredCharsetCoverage];
			}
			set
			{
				this[DomainContentConfigSchema.RequiredCharsetCoverage] = value;
			}
		}

		[Parameter]
		public bool? TNEFEnabled
		{
			get
			{
				return (bool?)this[DomainContentConfigSchema.TNEFEnabled];
			}
			set
			{
				this[DomainContentConfigSchema.TNEFEnabled] = value;
			}
		}

		[Parameter]
		public Unlimited<int> LineWrapSize
		{
			get
			{
				return (Unlimited<int>)this[DomainContentConfigSchema.LineWrapSize];
			}
			set
			{
				this[DomainContentConfigSchema.LineWrapSize] = value;
			}
		}

		[Parameter]
		public bool TrustedMailOutboundEnabled
		{
			get
			{
				return (bool)this[EdgeDomainContentConfigSchema.TrustedMailOutboundEnabled];
			}
			set
			{
				this[EdgeDomainContentConfigSchema.TrustedMailOutboundEnabled] = value;
			}
		}

		[Parameter]
		public bool TrustedMailInboundEnabled
		{
			get
			{
				return (bool)this[EdgeDomainContentConfigSchema.TrustedMailInboundEnabled];
			}
			set
			{
				this[EdgeDomainContentConfigSchema.TrustedMailInboundEnabled] = value;
			}
		}

		[Parameter]
		public bool UseSimpleDisplayName
		{
			get
			{
				return (bool)this[DomainContentConfigSchema.UseSimpleDisplayName];
			}
			set
			{
				this[DomainContentConfigSchema.UseSimpleDisplayName] = value;
			}
		}

		[Parameter]
		public bool NDRDiagnosticInfoEnabled
		{
			get
			{
				return !(bool)this[DomainContentConfigSchema.NDRDiagnosticInfoDisabled];
			}
			set
			{
				this[DomainContentConfigSchema.NDRDiagnosticInfoDisabled] = !value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MessageCountThreshold
		{
			get
			{
				return (int)this[DomainContentConfigSchema.MessageCountThreshold];
			}
			set
			{
				this[DomainContentConfigSchema.MessageCountThreshold] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				if (TopologyProvider.IsAdamTopology())
				{
					return DomainContentConfig.edgeSchema;
				}
				return DomainContentConfig.schema;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return DomainContentConfig.RootId;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchDomainContentConfig";
			}
		}

		internal static object AllowedOOFTypeGetter(IPropertyBag propertyBag)
		{
			AcceptMessageType acceptMessageType = (AcceptMessageType)((int)propertyBag[DomainContentConfigSchema.AcceptMessageTypes]);
			acceptMessageType &= (AcceptMessageType.LegacyOOF | AcceptMessageType.BlockOOF | AcceptMessageType.InternalDomain);
			AcceptMessageType acceptMessageType2 = acceptMessageType;
			switch (acceptMessageType2)
			{
			case AcceptMessageType.Default:
				return AllowedOOFType.External;
			case AcceptMessageType.LegacyOOF:
				return AllowedOOFType.ExternalLegacy;
			default:
				if (acceptMessageType2 == AcceptMessageType.BlockOOF)
				{
					return AllowedOOFType.None;
				}
				if (acceptMessageType2 != (AcceptMessageType.LegacyOOF | AcceptMessageType.InternalDomain))
				{
					return AllowedOOFType.External;
				}
				return AllowedOOFType.InternalLegacy;
			}
		}

		internal static void AllowedOOFTypeSetter(object value, IPropertyBag propertyBag)
		{
			AcceptMessageType acceptMessageType = (AcceptMessageType)((int)propertyBag[DomainContentConfigSchema.AcceptMessageTypes]);
			acceptMessageType &= ~(AcceptMessageType.LegacyOOF | AcceptMessageType.BlockOOF | AcceptMessageType.InternalDomain);
			AllowedOOFType allowedOOFType = (AllowedOOFType)value;
			acceptMessageType |= (AcceptMessageType)allowedOOFType;
			propertyBag[DomainContentConfigSchema.AcceptMessageTypes] = (int)acceptMessageType;
		}

		internal static object NonMimeCharacterSetGetter(IPropertyBag propertyBag)
		{
			string text = (string)propertyBag[EdgeDomainContentConfigSchema.ADNonMimeCharacterSet];
			if (!DomainContentConfig.IsValidCharset(text))
			{
				text = DomainContentConfig.MapCharset(text);
			}
			return text.ToLower(CultureInfo.InvariantCulture);
		}

		internal static void NonMimeCharacterSetSetter(object value, IPropertyBag propertyBag)
		{
			DomainContentConfig.ThrowOnInvalidCharset((string)value, EdgeDomainContentConfigSchema.NonMimeCharacterSet);
			propertyBag[EdgeDomainContentConfigSchema.ADNonMimeCharacterSet] = value;
		}

		internal static object CharacterSetGetter(IPropertyBag propertyBag)
		{
			string text = (string)propertyBag[DomainContentConfigSchema.ADCharacterSet];
			if (!DomainContentConfig.IsValidCharset(text))
			{
				text = DomainContentConfig.MapCharset(text);
			}
			return text.ToLower(CultureInfo.InvariantCulture);
		}

		internal static void CharacterSetSetter(object value, IPropertyBag propertyBag)
		{
			DomainContentConfig.ThrowOnInvalidCharset((string)value, DomainContentConfigSchema.CharacterSet);
			propertyBag[DomainContentConfigSchema.ADCharacterSet] = value;
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (this.TargetDeliveryDomain && this.DomainName != null && this.DomainName.IsStar)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.TargetDeliveryDomainCannotBeStar, DomainContentConfigSchema.TargetDeliveryDomain, this));
			}
		}

		private static bool IsValidCharset(string inputCharset)
		{
			foreach (DomainContentConfig.CharacterSetInfo characterSetInfo in DomainContentConfig.CharacterSetList)
			{
				if (string.Equals(inputCharset, characterSetInfo.CharsetName, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		private static void ThrowOnInvalidCharset(string inputCharset, PropertyDefinition propertyDefinition)
		{
			if (!DomainContentConfig.IsValidCharset(inputCharset))
			{
				StringBuilder stringBuilder = new StringBuilder(512);
				for (int i = 0; i < DomainContentConfig.CharacterSetList.Length; i++)
				{
					if (!string.IsNullOrEmpty(DomainContentConfig.CharacterSetList[i].CharsetName))
					{
						stringBuilder.Append(DomainContentConfig.CharacterSetList[i].CharsetName);
						stringBuilder.Append(", ");
					}
				}
				stringBuilder.Remove(stringBuilder.Length - 2, 2);
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.InvalidCharacterSet(inputCharset, stringBuilder.ToString()), propertyDefinition, inputCharset));
			}
		}

		private static string MapCharset(string inputCharset)
		{
			foreach (DomainContentConfig.CharsetMapping charsetMapping in DomainContentConfig.charsetMappingList)
			{
				if (string.Equals(inputCharset, charsetMapping.LegacyCharsetName, StringComparison.OrdinalIgnoreCase))
				{
					return charsetMapping.CharsetName;
				}
			}
			return string.Empty;
		}

		internal AcceptMessageType GetOOFSettings()
		{
			return (AcceptMessageType)((int)this[DomainContentConfigSchema.AcceptMessageTypes]);
		}

		public const string DefaultName = "Default";

		public const string Noun = "RemoteDomain";

		private const string MostDerivedClass = "msExchDomainContentConfig";

		private static readonly ADObjectId RootId = new ADObjectId("CN=Internet Message Formats,CN=Global Settings");

		private static readonly DomainContentConfigSchema schema = ObjectSchema.GetInstance<DomainContentConfigSchema>();

		private static readonly EdgeDomainContentConfigSchema edgeSchema = ObjectSchema.GetInstance<EdgeDomainContentConfigSchema>();

		private static DomainContentConfig.CharsetMapping[] charsetMappingList = new DomainContentConfig.CharsetMapping[]
		{
			new DomainContentConfig.CharsetMapping("ansi", string.Empty),
			new DomainContentConfig.CharsetMapping("iso-2022-jpdbcs", "iso-2022-jp"),
			new DomainContentConfig.CharsetMapping("iso-2022-jpesc", "iso-2022-jp"),
			new DomainContentConfig.CharsetMapping("iso-2022-jpsio", "iso-2022-jp"),
			new DomainContentConfig.CharsetMapping("us-ascii", string.Empty),
			new DomainContentConfig.CharsetMapping("x-euc", "euc-jp")
		};

		internal static DomainContentConfig.CharacterSetInfo[] CharacterSetList = new DomainContentConfig.CharacterSetInfo[]
		{
			new DomainContentConfig.CharacterSetInfo(string.Empty, "None", -1),
			new DomainContentConfig.CharacterSetInfo("big5", "Chinese Traditional (Big5)", 950),
			new DomainContentConfig.CharacterSetInfo("din_66003", "German (IA5)", 20106),
			new DomainContentConfig.CharacterSetInfo("euc-jp", "Japanese (EUC)", 51932),
			new DomainContentConfig.CharacterSetInfo("euc-kr", "Korean (EUC)", 51949),
			new DomainContentConfig.CharacterSetInfo("gb18030", "Chinese Simplified (GB18030)", 54936),
			new DomainContentConfig.CharacterSetInfo("gb2312", "Chinese Simplified (GB2312)", 936),
			new DomainContentConfig.CharacterSetInfo("hz-gb-2312", "Chinese Simplified (HZ)", 52936),
			new DomainContentConfig.CharacterSetInfo("iso-2022-jp", "Japanese (JIS)", 50220),
			new DomainContentConfig.CharacterSetInfo("iso-2022-kr", "Korean (ISO)", 50225),
			new DomainContentConfig.CharacterSetInfo("iso-8859-1", "Western European (ISO)", 28591),
			new DomainContentConfig.CharacterSetInfo("iso-8859-13", "Estonian (ISO)", 28603),
			new DomainContentConfig.CharacterSetInfo("iso-8859-15", "Latin 9 (ISO)", 28605),
			new DomainContentConfig.CharacterSetInfo("iso-8859-2", "Central European (ISO)", 28592),
			new DomainContentConfig.CharacterSetInfo("iso-8859-3", "Latin 3 (ISO)", 28593),
			new DomainContentConfig.CharacterSetInfo("iso-8859-4", "Baltic (ISO)", 28594),
			new DomainContentConfig.CharacterSetInfo("iso-8859-5", "Cyrillic (ISO)", 28595),
			new DomainContentConfig.CharacterSetInfo("iso-8859-6", "Arabic (ISO)", 28596),
			new DomainContentConfig.CharacterSetInfo("iso-8859-7", "Greek (ISO)", 28597),
			new DomainContentConfig.CharacterSetInfo("iso-8859-8", "Hebrew (ISO)", 28598),
			new DomainContentConfig.CharacterSetInfo("iso-8859-9", "Turkish (ISO)", 28599),
			new DomainContentConfig.CharacterSetInfo("koi8-r", "Cyrillic (KOI8-R)", 20866),
			new DomainContentConfig.CharacterSetInfo("koi8-u", "Cyrillic (KOI8-U)", 21866),
			new DomainContentConfig.CharacterSetInfo("ks_c_5601-1987", "Korean (Windows)", 949),
			new DomainContentConfig.CharacterSetInfo("ns_4551-1", "Norwegian (IA5)", 20108),
			new DomainContentConfig.CharacterSetInfo("sen_850200_b", "Swedish (IA5)", 20107),
			new DomainContentConfig.CharacterSetInfo("shift_jis", "Japanese (Shift-JIS)", 932),
			new DomainContentConfig.CharacterSetInfo("utf-7", "Unicode (UTF-7)", 65000),
			new DomainContentConfig.CharacterSetInfo("utf-8", "Unicode (UTF-8)", 65001),
			new DomainContentConfig.CharacterSetInfo("windows-1250", "Central European (Windows)", 1250),
			new DomainContentConfig.CharacterSetInfo("windows-1251", "Cyrillic (Windows)", 1251),
			new DomainContentConfig.CharacterSetInfo("windows-1252", "Western European (Windows)", 1252),
			new DomainContentConfig.CharacterSetInfo("windows-1253", "Greek (Windows)", 1253),
			new DomainContentConfig.CharacterSetInfo("windows-1254", "Turkish (Windows)", 1254),
			new DomainContentConfig.CharacterSetInfo("windows-1255", "Hebrew (Windows)", 1255),
			new DomainContentConfig.CharacterSetInfo("windows-1256", "Arabic (Windows)", 1256),
			new DomainContentConfig.CharacterSetInfo("windows-1257", "Baltic (Windows)", 1257),
			new DomainContentConfig.CharacterSetInfo("windows-1258", "Vietnamese (Windows)", 1258),
			new DomainContentConfig.CharacterSetInfo("windows-874", "Thai (Windows)", 874)
		};

		private struct CharsetMapping
		{
			public CharsetMapping(string legacyCharsetName, string charsetName)
			{
				this.LegacyCharsetName = legacyCharsetName;
				this.CharsetName = charsetName;
			}

			public string LegacyCharsetName;

			public string CharsetName;
		}

		internal struct CharacterSetInfo
		{
			public CharacterSetInfo(string charsetName, string charsetDesp, int codePage)
			{
				this.CharsetName = charsetName;
				this.CharsetDescription = charsetDesp;
				this.CodePage = codePage;
			}

			public string CharacterSetName
			{
				get
				{
					return this.CharsetName;
				}
			}

			public string CharacterSetDescription
			{
				get
				{
					return this.CharsetDescription;
				}
			}

			public string CharsetName;

			public string CharsetDescription;

			public int CodePage;
		}
	}
}
