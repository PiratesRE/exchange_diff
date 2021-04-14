using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules.OutlookProtection
{
	internal static class OutlookProtectionRuleConstants
	{
		public const string ADCollectionName = "OutlookProtectionRules";

		public const string UserOverridableElement = "userOverridable";

		public const string RecipientIsPredicate = "recipientIs";

		public const string AllInternalPredicate = "allInternal";

		public const string RightsProtectMessage = "RightsProtectMessage";

		public const string MessageToCcBccProperty = "Message.ToCcBcc";

		public const string MessageSenderDepartmentProperty = "Message.Sender.Department";
	}
}
