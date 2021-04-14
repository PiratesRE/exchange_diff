using System;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class QuarantinedMessageSchema
	{
		internal static readonly HygienePropertyDefinition OrganizationalUnitRootProperty = QuarantinedMessageCommonSchema.OrganizationalUnitRootProperty;

		internal static readonly HygienePropertyDefinition ExMessageIdProperty = QuarantinedMessageCommonSchema.ExMessageIdProperty;

		internal static readonly HygienePropertyDefinition EventIdProperty = QuarantinedMessageCommonSchema.EventIdProperty;

		internal static readonly HygienePropertyDefinition ClientMessageIdProperty = QuarantinedMessageCommonSchema.ClientMessageIdProperty;

		internal static readonly HygienePropertyDefinition ReceivedProperty = QuarantinedMessageCommonSchema.ReceivedProperty;

		internal static readonly HygienePropertyDefinition SenderAddressProperty = QuarantinedMessageCommonSchema.SenderAddressProperty;

		internal static readonly HygienePropertyDefinition MessageSubjectProperty = QuarantinedMessageCommonSchema.MessageSubjectProperty;

		internal static readonly HygienePropertyDefinition MessageSizeProperty = QuarantinedMessageCommonSchema.MessageSizeProperty;

		internal static readonly HygienePropertyDefinition MailDirectionProperty = QuarantinedMessageCommonSchema.MailDirectionProperty;

		internal static readonly HygienePropertyDefinition QuarantineTypeProperty = QuarantinedMessageCommonSchema.QuarantineTypeProperty;

		internal static readonly HygienePropertyDefinition ExpiresProperty = QuarantinedMessageCommonSchema.ExpiresProperty;

		internal static readonly HygienePropertyDefinition NotifiedProperty = QuarantinedMessageCommonSchema.NotifiedProperty;

		internal static readonly HygienePropertyDefinition QuarantinedProperty = QuarantinedMessageCommonSchema.QuarantinedProperty;

		internal static readonly HygienePropertyDefinition ReleasedProperty = QuarantinedMessageCommonSchema.ReleasedProperty;

		internal static readonly HygienePropertyDefinition ReportedProperty = QuarantinedMessageCommonSchema.ReportedProperty;

		internal static readonly HygienePropertyDefinition StartDateQueryProperty = QuarantinedMessageCommonSchema.StartDateQueryProperty;

		internal static readonly HygienePropertyDefinition EndDateQueryProperty = QuarantinedMessageCommonSchema.EndDateQueryProperty;

		internal static readonly HygienePropertyDefinition DomainListQueryProperty = QuarantinedMessageCommonSchema.DomainListQueryProperty;

		internal static readonly HygienePropertyDefinition MailDirectionListQueryProperty = QuarantinedMessageCommonSchema.MailDirectionListQueryProperty;

		internal static readonly HygienePropertyDefinition ClientMessageListQueryProperty = QuarantinedMessageCommonSchema.ClientMessageListQueryProperty;

		internal static readonly HygienePropertyDefinition SenderAddressListQueryProperty = QuarantinedMessageCommonSchema.SenderAddressListQueryProperty;

		internal static readonly HygienePropertyDefinition RecipientAddressListQueryProperty = QuarantinedMessageCommonSchema.RecipientAddressListQueryProperty;

		internal static readonly HygienePropertyDefinition TransportRuleListQueryProperty = QuarantinedMessageCommonSchema.TransportRuleListQueryProperty;

		internal static readonly HygienePropertyDefinition QuarantinedUserAddressListQueryProperty = QuarantinedMessageCommonSchema.QuarantinedUserAddressListQueryProperty;

		internal static readonly HygienePropertyDefinition StartExpireDateQueryProperty = QuarantinedMessageCommonSchema.StartExpireDateQueryProperty;

		internal static readonly HygienePropertyDefinition EndExpireDateQueryProperty = QuarantinedMessageCommonSchema.EndExpireDateQueryProperty;

		internal static readonly HygienePropertyDefinition PageNumberQueryProperty = QuarantinedMessageCommonSchema.PageNumberQueryProperty;

		internal static readonly HygienePropertyDefinition PageSizeQueryProperty = QuarantinedMessageCommonSchema.PageSizeQueryProperty;
	}
}
