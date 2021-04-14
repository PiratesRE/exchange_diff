using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ComplexParticipantExtractorFromPropertyBag : ComplexParticipantExtractorBase<string>
	{
		protected override bool ExtractReplyTo(PropertyBag.BasicPropertyStore propertyBag, ComplexParticipantExtractorBase<string>.ParticipantConverter converter, out IList<string> replyToRecipients)
		{
			return Participant.TryGetParticipantsFromDisplayNameProperty(propertyBag, ComplexParticipantExtractorFromPropertyBag.ReplyToNamesPropertyDefinition, out replyToRecipients);
		}

		protected override bool ExtractToAndCC(PropertyBag.BasicPropertyStore propertyBag, ComplexParticipantExtractorBase<string>.ParticipantConverter converter, IEqualityComparer<string> comparer, out IDictionary<RecipientItemType, HashSet<string>> recipientTable)
		{
			recipientTable = ComplexParticipantExtractorBase<string>.InstantiateResultType(comparer);
			IList<string> other;
			if (!Participant.TryGetParticipantsFromDisplayNameProperty(propertyBag, InternalSchema.DisplayToInternal, out other))
			{
				return false;
			}
			IList<string> other2;
			if (!Participant.TryGetParticipantsFromDisplayNameProperty(propertyBag, InternalSchema.DisplayCcInternal, out other2))
			{
				return false;
			}
			recipientTable[RecipientItemType.To].UnionWith(other);
			recipientTable[RecipientItemType.Cc].UnionWith(other2);
			return true;
		}

		private static PropertyTagPropertyDefinition ReplyToNamesPropertyDefinition = InternalSchema.MapiReplyToNames;
	}
}
