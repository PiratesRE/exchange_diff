using System;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal sealed class MessageRecipientStatusSchema
	{
		internal static readonly HygienePropertyDefinition RecipientStatusIdProperty = new HygienePropertyDefinition("RecipientStatusId", typeof(Guid));

		internal static readonly HygienePropertyDefinition RecipientIdProperty = CommonMessageTraceSchema.RecipientIdProperty;

		internal static readonly HygienePropertyDefinition EventIdProperty = CommonMessageTraceSchema.EventIdProperty;

		internal static readonly HygienePropertyDefinition StatusProperty = new HygienePropertyDefinition("Status", typeof(string));

		internal static readonly HygienePropertyDefinition ReferenceProperty = new HygienePropertyDefinition("Reference", typeof(string));
	}
}
