using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal abstract class ReplyAllParticipantsRepresentationProperty<T> : SmartPropertyDefinition
	{
		internal ReplyAllParticipantsRepresentationProperty(string displayName) : base(displayName, typeof(IDictionary<RecipientItemType, IEnumerable<T>>), PropertyFlags.ReadOnly, PropertyDefinitionConstraint.None, ReplyAllParticipantsRepresentationProperty<T>.PropertyDependencies)
		{
		}

		public static IDictionary<RecipientItemType, HashSet<RepType>> BuildReplyAllRecipients<RepType>(RepType sender, RepType from, IList<RepType> replyTo, IDictionary<RecipientItemType, HashSet<RepType>> recipients, IEqualityComparer<RepType> participantRepresentationComparer)
		{
			IDictionary<RecipientItemType, HashSet<RepType>> dictionary = ReplyAllParticipantsRepresentationProperty<T>.BuildToAndCCRecipients<RepType>(recipients, participantRepresentationComparer);
			if (replyTo == null || replyTo.Count == 0)
			{
				if (from != null)
				{
					dictionary[RecipientItemType.To].Add(from);
				}
				else if (sender != null)
				{
					dictionary[RecipientItemType.To].Add(sender);
				}
			}
			else
			{
				foreach (RepType item in replyTo)
				{
					dictionary[RecipientItemType.To].Add(item);
				}
			}
			dictionary[RecipientItemType.Cc].ExceptWith(dictionary[RecipientItemType.To]);
			return dictionary;
		}

		public static IDictionary<RecipientItemType, HashSet<RepType>> BuildToAndCCRecipients<RepType>(IDictionary<RecipientItemType, HashSet<RepType>> recipients, IEqualityComparer<RepType> participantRepresentationComparer)
		{
			IDictionary<RecipientItemType, HashSet<RepType>> dictionary = ReplyAllParticipantsRepresentationProperty<RepType>.InstantiateResultType(participantRepresentationComparer);
			foreach (KeyValuePair<RecipientItemType, HashSet<RepType>> keyValuePair in recipients)
			{
				if (keyValuePair.Key == RecipientItemType.To || keyValuePair.Key == RecipientItemType.Cc)
				{
					foreach (RepType item in keyValuePair.Value)
					{
						dictionary[keyValuePair.Key].Add(item);
					}
				}
			}
			dictionary[RecipientItemType.Cc].ExceptWith(dictionary[RecipientItemType.To]);
			return dictionary;
		}

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

		private static PropertyDependency[] PropertyDependencies
		{
			get
			{
				if (ReplyAllParticipantsRepresentationProperty<T>.propertyDependencies == null)
				{
					List<PropertyDependency> list = new List<PropertyDependency>();
					list.AddRange(InternalSchema.From.Dependencies);
					list.AddRange(InternalSchema.Sender.Dependencies);
					list.Add(new PropertyDependency(InternalSchema.MapiReplyToNames, PropertyDependencyType.NeedForRead));
					list.Add(new PropertyDependency(InternalSchema.DisplayToInternal, PropertyDependencyType.NeedForRead));
					list.Add(new PropertyDependency(InternalSchema.DisplayCcInternal, PropertyDependencyType.NeedForRead));
					list.Add(new PropertyDependency(InternalSchema.ItemClass, PropertyDependencyType.NeedForRead));
					ReplyAllParticipantsRepresentationProperty<T>.propertyDependencies = list.ToArray();
				}
				return ReplyAllParticipantsRepresentationProperty<T>.propertyDependencies;
			}
		}

		public abstract IEqualityComparer<T> ParticipantRepresentationComparer { get; }

		protected IParticipant GetSimpleParticipant(SmartPropertyDefinition definition, PropertyBag.BasicPropertyStore propertyBag)
		{
			return propertyBag.GetValue(definition) as IParticipant;
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			throw new NotSupportedException(ServerStrings.PropertyIsReadOnly(base.Name));
		}

		private static PropertyDependency[] propertyDependencies;
	}
}
