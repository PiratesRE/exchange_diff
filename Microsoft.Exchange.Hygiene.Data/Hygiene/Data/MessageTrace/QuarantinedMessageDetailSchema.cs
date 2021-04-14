using System;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class QuarantinedMessageDetailSchema
	{
		internal static readonly HygienePropertyDefinition OrganizationalUnitRootProperty = QuarantinedMessageCommonSchema.OrganizationalUnitRootProperty;

		internal static readonly HygienePropertyDefinition ExMessageIdProperty = QuarantinedMessageCommonSchema.ExMessageIdProperty;

		internal static readonly HygienePropertyDefinition EventIdProperty = QuarantinedMessageCommonSchema.EventIdProperty;

		internal static readonly HygienePropertyDefinition ClientMessageIdProperty = QuarantinedMessageCommonSchema.ClientMessageIdProperty;

		internal static readonly HygienePropertyDefinition ReceivedProperty = QuarantinedMessageCommonSchema.ReceivedProperty;

		internal static readonly HygienePropertyDefinition MailDirectionProperty = QuarantinedMessageCommonSchema.MailDirectionProperty;

		internal static readonly HygienePropertyDefinition MessageSizeProperty = QuarantinedMessageCommonSchema.MessageSizeProperty;

		internal static readonly HygienePropertyDefinition MessageSubjectProperty = QuarantinedMessageCommonSchema.MessageSubjectProperty;

		internal static readonly HygienePropertyDefinition QuarantineTypeProperty = QuarantinedMessageCommonSchema.QuarantineTypeProperty;

		internal static readonly HygienePropertyDefinition ExpiresProperty = QuarantinedMessageCommonSchema.ExpiresProperty;

		internal static readonly HygienePropertyDefinition PartNameProperty = QuarantinedMessageCommonSchema.PartNameProperty;

		internal static readonly HygienePropertyDefinition MimeNameProperty = QuarantinedMessageCommonSchema.MimeNameProperty;

		internal static readonly HygienePropertyDefinition SenderAddressProperty = QuarantinedMessageCommonSchema.SenderAddressProperty;

		internal static readonly HygienePropertyDefinition RecipientAddressProperty = QuarantinedMessageCommonSchema.RecipientAddressProperty;

		internal static readonly HygienePropertyDefinition QuarantinedProperty = QuarantinedMessageCommonSchema.QuarantinedProperty;

		internal static readonly HygienePropertyDefinition NotifiedProperty = QuarantinedMessageCommonSchema.NotifiedProperty;

		internal static readonly HygienePropertyDefinition ReportedProperty = QuarantinedMessageCommonSchema.ReportedProperty;

		internal static readonly HygienePropertyDefinition ReleasedProperty = QuarantinedMessageCommonSchema.ReleasedProperty;
	}
}
