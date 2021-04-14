using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ComplexParticipantExtractorFromICoreItem<T> : ComplexParticipantExtractorBase<T>
	{
		public override bool ShouldExtract(PropertyBag.BasicPropertyStore propertyBag)
		{
			return base.ShouldExtract(propertyBag) && propertyBag.Context.CoreObject is ICoreItem;
		}

		protected override bool ExtractToAndCC(PropertyBag.BasicPropertyStore propertyBag, ComplexParticipantExtractorBase<T>.ParticipantConverter converter, IEqualityComparer<T> comparer, out IDictionary<RecipientItemType, HashSet<T>> recipientTable)
		{
			ICoreItem coreItem = propertyBag.Context.CoreObject as ICoreItem;
			recipientTable = null;
			if (coreItem != null)
			{
				CoreRecipientCollection recipientCollection = coreItem.GetRecipientCollection(true);
				if (recipientCollection != null)
				{
					recipientTable = ComplexParticipantExtractorBase<T>.InstantiateResultType(comparer);
					foreach (CoreRecipient coreRecipient in recipientCollection)
					{
						if (coreRecipient.RecipientItemType == RecipientItemType.To || coreRecipient.RecipientItemType == RecipientItemType.Cc)
						{
							recipientTable[coreRecipient.RecipientItemType].Add(converter(coreRecipient.Participant));
						}
					}
					return true;
				}
			}
			return false;
		}

		protected override bool ExtractReplyTo(PropertyBag.BasicPropertyStore propertyBag, ComplexParticipantExtractorBase<T>.ParticipantConverter converter, out IList<T> replyToRecipients)
		{
			ReplyTo first = new ReplyTo((PropertyBag)propertyBag);
			replyToRecipients = new List<T>(from participant in first
			select converter(participant));
			return true;
		}
	}
}
