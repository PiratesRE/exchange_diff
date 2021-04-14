using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal abstract class AggregateRecipientProperty : SmartPropertyDefinition
	{
		protected AggregateRecipientProperty(string displayName, NativeStorePropertyDefinition storeComputedProperty, StorePropertyDefinition recipientStringProperty) : base(displayName, typeof(string), PropertyFlags.ReadOnly, PropertyDefinitionConstraint.None, (storeComputedProperty != null) ? new PropertyDependency[]
		{
			new PropertyDependency(storeComputedProperty, PropertyDependencyType.NeedForRead)
		} : Array<PropertyDependency>.Empty)
		{
			this.storeComputedProperty = storeComputedProperty;
			this.recipientStringProperty = recipientStringProperty;
			this.RegisterFilterTranslation();
		}

		public override StorePropertyCapabilities Capabilities
		{
			get
			{
				StorePropertyCapabilities storePropertyCapabilities = base.Capabilities;
				if (this.storeComputedProperty != null)
				{
					storePropertyCapabilities |= (StorePropertyCapabilities.CanQuery | StorePropertyCapabilities.CanSortBy);
				}
				return storePropertyCapabilities;
			}
		}

		protected sealed override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			base.InternalSetValue(propertyBag, value);
		}

		protected sealed override void InternalDeleteValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			base.InternalDeleteValue(propertyBag);
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			MessageItem messageItem = propertyBag.Context.StoreObject as MessageItem;
			CalendarItemBase calendarItemBase = propertyBag.Context.StoreObject as CalendarItemBase;
			if (messageItem != null)
			{
				return this.BuildAggregatedValue<Recipient>(messageItem.Recipients);
			}
			if (calendarItemBase != null)
			{
				return this.BuildAggregatedValue<Attendee>(calendarItemBase.AttendeeCollection);
			}
			if (this.storeComputedProperty != null)
			{
				object value = propertyBag.GetValue(this.storeComputedProperty);
				if (!PropertyError.IsPropertyError(value))
				{
					return value;
				}
			}
			return new PropertyError(this, PropertyErrorCode.GetCalculatedPropertyError);
		}

		internal override SortBy[] GetNativeSortBy(SortOrder sortOrder)
		{
			if (this.storeComputedProperty == null)
			{
				return base.GetNativeSortBy(sortOrder);
			}
			return new SortBy[]
			{
				new SortBy(this.storeComputedProperty, sortOrder)
			};
		}

		internal override QueryFilter NativeFilterToSmartFilter(QueryFilter filter)
		{
			return base.SinglePropertyNativeFilterToSmartFilter(filter, this.storeComputedProperty);
		}

		internal override QueryFilter SmartFilterToNativeFilter(SinglePropertyFilter filter)
		{
			return base.SinglePropertySmartFilterToNativeFilter(filter, this.storeComputedProperty);
		}

		internal override void RegisterFilterTranslation()
		{
			FilterRestrictionConverter.RegisterFilterTranslation(this, typeof(ComparisonFilter));
			FilterRestrictionConverter.RegisterFilterTranslation(this, typeof(ExistsFilter));
			FilterRestrictionConverter.RegisterFilterTranslation(this, typeof(TextFilter));
		}

		protected abstract bool IsRecipientIncluded(RecipientBase recipientBase);

		private void BuildStringForOneRecipient(StringBuilder sb, RecipientBase recipientBase)
		{
			if (!this.IsRecipientIncluded(recipientBase))
			{
				return;
			}
			if (sb.Length > 0)
			{
				sb.Append("; ");
			}
			sb.Append(recipientBase.TryGetProperty(this.recipientStringProperty) as string);
		}

		private string BuildAggregatedValue<T>(IRecipientBaseCollection<T> recipientBaseCollection) where T : RecipientBase
		{
			StringBuilder stringBuilder = new StringBuilder(recipientBaseCollection.Count * (15 + "; ".Length));
			foreach (T t in recipientBaseCollection)
			{
				RecipientBase recipientBase = t;
				this.BuildStringForOneRecipient(stringBuilder, recipientBase);
			}
			return stringBuilder.ToString();
		}

		private const string Separator = "; ";

		private readonly NativeStorePropertyDefinition storeComputedProperty;

		private readonly StorePropertyDefinition recipientStringProperty;
	}
}
