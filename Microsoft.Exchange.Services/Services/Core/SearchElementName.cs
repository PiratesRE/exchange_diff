using System;

namespace Microsoft.Exchange.Services.Core
{
	internal static class SearchElementName
	{
		public const string Restriction = "Restriction";

		public const string Exists = "Exists";

		public const string IsEqualTo = "IsEqualTo";

		public const string IsNotEqualTo = "IsNotEqualTo";

		public const string IsGreaterThan = "IsGreaterThan";

		public const string IsGreaterThanOrEqualTo = "IsGreaterThanOrEqualTo";

		public const string IsLessThan = "IsLessThan";

		public const string IsLessThanOrEqualTo = "IsLessThanOrEqualTo";

		public const string Excludes = "Excludes";

		public const string Contains = "Contains";

		public const string ContainsValueAttribute = "Value";

		public const string ContainmentModeAttribute = "ContainmentMode";

		public const string ContainmentComparisonAttribute = "ContainmentComparison";

		public const string FullStringContainmentMode = "FullString";

		public const string PrefixedContainmentMode = "Prefixed";

		public const string SubstringContainmentMode = "Substring";

		public const string SuffixedContainmentMode = "Suffixed";

		public const string PrefixOnWordsContainmentMode = "PrefixOnWords";

		public const string ExactPhraseContainmentMode = "ExactPhrase";

		public const string ExactComparisonType = "Exact";

		public const string IgnoreCaseComparisonType = "IgnoreCase";

		public const string IgnoreNonSpacingCharactersComparisonType = "IgnoreNonSpacingCharacters";

		public const string LooseComparisonType = "Loose";

		public const string IgnoreCaseAndNonSpacingCharactersComparisonType = "IgnoreCaseAndNonSpacingCharacters";

		public const string LooseAndIgnoreCaseComparisonType = "LooseAndIgnoreCase";

		public const string LooseAndIgnoreNonSpaceComparisonType = "LooseAndIgnoreNonSpace";

		public const string LooseAndIgnoreCaseAndIgnoreNonSpaceComparisonType = "LooseAndIgnoreCaseAndIgnoreNonSpace";

		public const string Not = "Not";

		public const string And = "And";

		public const string Or = "Or";

		public const string ConstantElementName = "Constant";

		public const string ConstantAttributeName = "Value";

		public const string BitmaskElementName = "Bitmask";

		public const string BitmaskAttributeName = "Value";

		public const string FieldUriOrConstantElementName = "FieldURIOrConstant";

		public const string ToRecipientsUri = "message:ToRecipients";

		public const string CcRecipientsUri = "message:CcRecipients";

		public const string BccRecipientsUri = "message:BccRecipients";

		public const string RequiredAttendeesUri = "calendar:RequiredAttendees";

		public const string OptionalAttendeesUri = "calendar:OptionalAttendees";

		public const string ResourceAttendeesUri = "calendar:Resources";

		public const string AttachmentsUri = "item:Attachments";

		public const string InternetMessageHeadersFieldUri = "item:InternetMessageHeader";

		public const string GroupedItemsElementName = "GroupedItems";

		public const string GroupIndexElementName = "GroupIndex";

		public const string ItemsElementName = "Items";
	}
}
