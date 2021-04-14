using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class ReplyDisplayNamesProperty : ReplyAllParticipantsRepresentationProperty<string>
	{
		internal ReplyDisplayNamesProperty() : base("ReplyDisplayNames")
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
			foreach (ComplexParticipantExtractorBase<string> complexParticipantExtractorBase in ReplyDisplayNamesProperty.Extractors)
			{
				if (complexParticipantExtractorBase.ShouldExtract(propertyBag))
				{
					object result;
					if (complexParticipantExtractorBase.Extract(propertyBag, (Participant participant) => participant.DisplayName, this.ParticipantRepresentationComparer, out recipients))
					{
						result = ReplyAllParticipantsRepresentationProperty<string>.BuildToAndCCRecipients<string>(recipients, this.ParticipantRepresentationComparer);
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
