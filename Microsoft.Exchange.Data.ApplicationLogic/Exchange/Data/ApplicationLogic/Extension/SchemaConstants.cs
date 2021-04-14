using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	internal static class SchemaConstants
	{
		public const string Xsd15FileName = "ExtensionManifestSchema.15.xsd";

		public const string Xsd15_1FileName = "ExtensionManifestSchema.15.1.xsd";

		public const string OweNamespacePrefix1_0 = "owe1_0";

		public const string OweNamespacePrefix1_1 = "owe1_1";

		public const string OweNamespaceUri1_0 = "http://schemas.microsoft.com/office/appforoffice/1.0";

		public const string OweNamespaceUri1_1 = "http://schemas.microsoft.com/office/appforoffice/1.1";

		public const string OweNamespaceUriNonVersionSpecificPart = "http://schemas.microsoft.com/office/appforoffice/";

		public const string XsiNamespacePrefix = "xsi";

		public const string XsiNamespaceUri = "http://www.w3.org/2001/XMLSchema-instance";

		public const string ItemHasRegularExpressionMatchRuleType = "ItemHasRegularExpressionMatch";

		public const string ItemIsRuleType = "ItemIs";

		public const string RuleCollectionRuleType = "RuleCollection";

		public const string ItemHasKnownEntityRuleType = "ItemHasKnownEntity";

		public const string ItemHasAttachmentRuleType = "ItemHasAttachment";

		public const string EntityTypeAttributeName = "EntityType";

		public const string RegExFilterAttributeName = "RegExFilter";

		public const string RegExValueAttributeName = "RegExValue";

		public const string FilterNameAttributeName = "FilterName";

		public const string IgnoreCaseAttributeName = "IgnoreCase";

		public const string DefaultValueAttributeName = "DefaultValue";

		public const string ValueAttributeName = "Value";

		public const string LocaleAttributeName = "Locale";

		public const string ItemClassAttributeName = "ItemClass";

		public const string IncludeSubClassesAttributeName = "IncludeSubClasses";

		public const string ItemIsRuleFormTypeAttributeName = "FormType";

		public const string OfficeAppElementName = "OfficeApp";

		public const string IconUrlElementName = "IconUrl";

		public const string HighResolutionIconUrlElementName = "HighResolutionIconUrl";

		public const string SourceLocationElementName = "SourceLocation";

		public const string RequirementsElementName = "Requirements";

		public const string SetsElementName = "Sets";

		public const string SetElementName = "Set";

		public const string SetElementNameAttributeName = "Name";

		public const string SetElementMinVersionAttributeName = "MinVersion";

		public const string RequiredSetNameForOutlookApp = "Mailbox";

		public const string HostsElementName = "Hosts";

		public const string HostElementName = "Host";

		public const string HostElementNameAttributeName = "Name";

		public const string HostExpectedValueForOutlookApp = "Mailbox";

		public const string DefaultMinVersionAttributeName = "DefaultMinVersion";

		public const string DefaultMinVersionAttributeValue = "1.1";

		public const string IdElementName = "Id";

		public const string PermissionsElementName = "Permissions";

		public const string RuleElementName = "Rule";

		public const string OverrideChildElementName = "Override";

		public const string XsiTypeAttributeName = "type";

		public const string RequestedHeightElementName = "RequestedHeight";

		public const string PhoneSettingsElementName = "PhoneSettings";

		public const string TabletSettingsElementName = "TabletSettings";

		public const string DesktopSettingsElementName = "DesktopSettings";

		public const string DisableEntityHighlightingElementName = "DisableEntityHighlighting";

		public static readonly Dictionary<string, string> SchemaNamespaceUriToFile = new Dictionary<string, string>(StringComparer.Ordinal)
		{
			{
				"http://schemas.microsoft.com/office/appforoffice/1.0",
				"ExtensionManifestSchema.15.xsd"
			},
			{
				"http://schemas.microsoft.com/office/appforoffice/1.1",
				"ExtensionManifestSchema.15.1.xsd"
			}
		};

		public static readonly Version SchemaVersion1_0 = new Version(1, 0);

		public static readonly Version SchemaVersion1_1 = new Version(1, 1);

		public static readonly Version LowestApiVersionSupportedBySchemaVersion1_1 = new Version(1, 1);

		public static readonly Version HighestSupportedApiVersion = new Version(1, 1);

		public static readonly Version Exchange2013RtmApiVersion = new Version(1, 0);
	}
}
