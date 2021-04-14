using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class ComplexParticipantExtractorBase<T>
	{
		protected static IDictionary<RecipientItemType, HashSet<T>> InstantiateResultType(IEqualityComparer<T> participantRepresentationComparer)
		{
			return new Dictionary<RecipientItemType, HashSet<T>>
			{
				{
					RecipientItemType.To,
					new HashSet<T>(participantRepresentationComparer)
				},
				{
					RecipientItemType.Cc,
					new HashSet<T>(participantRepresentationComparer)
				}
			};
		}

		public virtual bool ShouldExtract(PropertyBag.BasicPropertyStore propertyBag)
		{
			return ComplexParticipantExtractorBase<T>.CanExtractRecipients(propertyBag);
		}

		public static bool CanExtractRecipients(IStorePropertyBag propertyBag)
		{
			string itemClass = propertyBag.TryGetProperty(InternalSchema.ItemClass) as string;
			return ComplexParticipantExtractorBase<T>.CanExtractRecipients(itemClass);
		}

		public static bool CanExtractRecipients(ICorePropertyBag propertyBag)
		{
			string itemClass = propertyBag.TryGetProperty(InternalSchema.ItemClass) as string;
			return ComplexParticipantExtractorBase<T>.CanExtractRecipients(itemClass);
		}

		private static bool CanExtractRecipients(PropertyBag.BasicPropertyStore propertyBag)
		{
			string itemClass = propertyBag.GetValue(InternalSchema.ItemClass) as string;
			return ComplexParticipantExtractorBase<T>.CanExtractRecipients(itemClass);
		}

		private static bool CanExtractRecipients(string itemClass)
		{
			return !string.IsNullOrEmpty(itemClass) && (ObjectClass.IsMessage(itemClass, true) || ObjectClass.IsCalendarItem(itemClass));
		}

		public bool Extract(PropertyBag.BasicPropertyStore propertyBag, ComplexParticipantExtractorBase<T>.ParticipantConverter converter, IEqualityComparer<T> comparer, out IDictionary<RecipientItemType, HashSet<T>> recipientTable, out IList<T> replyToRecipients)
		{
			replyToRecipients = null;
			recipientTable = null;
			return this.ExtractToAndCC(propertyBag, converter, comparer, out recipientTable) && (!this.CanExtractReplyTo(propertyBag) || this.ExtractReplyTo(propertyBag, converter, out replyToRecipients));
		}

		public bool Extract(PropertyBag.BasicPropertyStore propertyBag, ComplexParticipantExtractorBase<T>.ParticipantConverter converter, IEqualityComparer<T> comparer, out IDictionary<RecipientItemType, HashSet<T>> recipientTable)
		{
			return this.ExtractToAndCC(propertyBag, converter, comparer, out recipientTable);
		}

		protected abstract bool ExtractToAndCC(PropertyBag.BasicPropertyStore propertyBag, ComplexParticipantExtractorBase<T>.ParticipantConverter converter, IEqualityComparer<T> comparer, out IDictionary<RecipientItemType, HashSet<T>> recipientTable);

		protected abstract bool ExtractReplyTo(PropertyBag.BasicPropertyStore propertyBag, ComplexParticipantExtractorBase<T>.ParticipantConverter converter, out IList<T> replyToRecipients);

		private bool CanExtractReplyTo(PropertyBag.BasicPropertyStore propertyBag)
		{
			string text = propertyBag.GetValue(InternalSchema.ItemClass) as string;
			return !string.IsNullOrEmpty(text) && ObjectClass.IsMessage(text, false);
		}

		internal delegate T ParticipantConverter(Participant p);
	}
}
