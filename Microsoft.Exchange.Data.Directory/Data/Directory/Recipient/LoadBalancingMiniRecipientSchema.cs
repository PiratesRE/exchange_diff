using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal class LoadBalancingMiniRecipientSchema : MiniRecipientSchema
	{
		internal static readonly ADPropertyDefinition MailboxMoveStatus = ADUserSchema.MailboxMoveStatus;

		internal static readonly ADPropertyDefinition MailboxMoveFlags = ADUserSchema.MailboxMoveFlags;

		internal static readonly ADPropertyDefinition MailboxMoveBatchName = ADUserSchema.MailboxMoveBatchName;

		internal static readonly ADPropertyDefinition[] LoadBalancingProperties = new ADPropertyDefinition[]
		{
			MiniRecipientSchema.ConfigurationXML,
			LoadBalancingMiniRecipientSchema.MailboxMoveStatus,
			LoadBalancingMiniRecipientSchema.MailboxMoveFlags,
			LoadBalancingMiniRecipientSchema.MailboxMoveBatchName
		};
	}
}
