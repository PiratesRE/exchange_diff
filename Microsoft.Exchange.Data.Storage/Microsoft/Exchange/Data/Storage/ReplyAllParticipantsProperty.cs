using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class ReplyAllParticipantsProperty : ReplyAllParticipantsRepresentationProperty<IParticipant>
	{
		internal ReplyAllParticipantsProperty() : base("ReplyAllParticipantsProperty")
		{
		}

		public override IEqualityComparer<IParticipant> ParticipantRepresentationComparer
		{
			get
			{
				return ParticipantComparer.EmailAddress;
			}
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			IDictionary<RecipientItemType, HashSet<IParticipant>> recipients = null;
			if (ReplyAllParticipantsProperty.Extractor.ShouldExtract(propertyBag))
			{
				IList<IParticipant> replyTo;
				if (ReplyAllParticipantsProperty.Extractor.Extract(propertyBag, (Participant participant) => participant, this.ParticipantRepresentationComparer, out recipients, out replyTo))
				{
					IParticipant simpleParticipant = base.GetSimpleParticipant(InternalSchema.Sender, propertyBag);
					IParticipant simpleParticipant2 = base.GetSimpleParticipant(InternalSchema.From, propertyBag);
					return ReplyAllParticipantsRepresentationProperty<IParticipant>.BuildReplyAllRecipients<IParticipant>(simpleParticipant, simpleParticipant2, replyTo, recipients, this.ParticipantRepresentationComparer);
				}
			}
			return ReplyAllParticipantsRepresentationProperty<IParticipant>.InstantiateResultType(this.ParticipantRepresentationComparer);
		}

		private static ComplexParticipantExtractorBase<IParticipant> Extractor = new ComplexParticipantExtractorFromICoreItem<IParticipant>();
	}
}
