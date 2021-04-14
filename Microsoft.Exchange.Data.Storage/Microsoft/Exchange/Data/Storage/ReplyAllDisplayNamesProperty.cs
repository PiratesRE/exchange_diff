using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class ReplyAllDisplayNamesProperty : ReplyAllParticipantsRepresentationProperty<string>
	{
		internal ReplyAllDisplayNamesProperty() : base("ReplyAllDisplayNames")
		{
		}

		public override IEqualityComparer<string> ParticipantRepresentationComparer
		{
			get
			{
				return StringComparer.OrdinalIgnoreCase;
			}
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			IDictionary<RecipientItemType, HashSet<string>> recipients = null;
			foreach (ComplexParticipantExtractorBase<string> complexParticipantExtractorBase in ReplyAllDisplayNamesProperty.Extractors)
			{
				if (complexParticipantExtractorBase.ShouldExtract(propertyBag))
				{
					IList<string> replyTo;
					object result;
					if (complexParticipantExtractorBase.Extract(propertyBag, (Participant participant) => participant.DisplayName, this.ParticipantRepresentationComparer, out recipients, out replyTo))
					{
						IParticipant simpleParticipant = base.GetSimpleParticipant(InternalSchema.Sender, propertyBag);
						IParticipant simpleParticipant2 = base.GetSimpleParticipant(InternalSchema.From, propertyBag);
						result = ReplyAllParticipantsRepresentationProperty<string>.BuildReplyAllRecipients<string>((simpleParticipant == null) ? null : simpleParticipant.DisplayName, (simpleParticipant2 == null) ? null : simpleParticipant2.DisplayName, replyTo, recipients, this.ParticipantRepresentationComparer);
					}
					else
					{
						result = new PropertyError(this, PropertyErrorCode.GetCalculatedPropertyError);
					}
					return result;
				}
			}
			return new PropertyError(this, PropertyErrorCode.GetCalculatedPropertyError);
		}

		private static ComplexParticipantExtractorBase<string>[] Extractors = new ComplexParticipantExtractorBase<string>[]
		{
			new ComplexParticipantExtractorFromICoreItem<string>(),
			new ComplexParticipantExtractorFromPropertyBag()
		};
	}
}
