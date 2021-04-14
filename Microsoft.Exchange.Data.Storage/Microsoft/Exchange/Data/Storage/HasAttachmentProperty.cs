using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class HasAttachmentProperty : SmartPropertyDefinition
	{
		internal HasAttachmentProperty() : base("HasAttachment", typeof(bool), PropertyFlags.ReadOnly, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.MapiHasAttachment, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.AllAttachmentsHidden, PropertyDependencyType.NeedForRead)
		})
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			CalendarItem calendarItem = propertyBag.Context.StoreObject as CalendarItem;
			if (calendarItem != null && calendarItem.IsAttachmentCollectionLoaded)
			{
				return calendarItem.AttachmentCollection.Count > 0;
			}
			object value = propertyBag.GetValue(InternalSchema.MapiHasAttachment);
			if (!(value is bool))
			{
				return new PropertyError(this, PropertyErrorCode.NotFound);
			}
			object value2 = propertyBag.GetValue(InternalSchema.AllAttachmentsHidden);
			if (value2 is bool && (bool)value2)
			{
				return false;
			}
			if (!(bool)value)
			{
				return false;
			}
			Item item = propertyBag.Context.StoreObject as Item;
			if (item != null)
			{
				foreach (AttachmentHandle attachmentHandle in item.AttachmentCollection)
				{
					if (!attachmentHandle.IsInline)
					{
						return true;
					}
				}
				return false;
			}
			return (bool)value;
		}

		internal override QueryFilter SmartFilterToNativeFilter(SinglePropertyFilter filter)
		{
			ComparisonFilter comparisonFilter = filter as ComparisonFilter;
			if (comparisonFilter != null && comparisonFilter.PropertyValue is bool && (comparisonFilter.ComparisonOperator == ComparisonOperator.Equal || comparisonFilter.ComparisonOperator == ComparisonOperator.NotEqual))
			{
				bool flag = (comparisonFilter.ComparisonOperator == ComparisonOperator.Equal) ? ((bool)comparisonFilter.PropertyValue) : (!(bool)comparisonFilter.PropertyValue);
				QueryFilter result;
				if (flag)
				{
					result = HasAttachmentProperty.HasAttachmentFilter;
				}
				else
				{
					result = HasAttachmentProperty.NotHasAttachmentFilter;
				}
				return result;
			}
			return base.SmartFilterToNativeFilter(filter);
		}

		internal override QueryFilter NativeFilterToSmartFilter(QueryFilter filter)
		{
			if (filter.Equals(HasAttachmentProperty.HasAttachmentFilter) || filter.Equals(HasAttachmentProperty.NativeToSmartHasAttachmentFilter))
			{
				return new ComparisonFilter(ComparisonOperator.Equal, this, true);
			}
			if (filter.Equals(HasAttachmentProperty.NotHasAttachmentFilter))
			{
				return new ComparisonFilter(ComparisonOperator.Equal, this, false);
			}
			return null;
		}

		public override StorePropertyCapabilities Capabilities
		{
			get
			{
				return StorePropertyCapabilities.CanQuery | StorePropertyCapabilities.CanSortBy;
			}
		}

		internal override void RegisterFilterTranslation()
		{
			FilterRestrictionConverter.RegisterFilterTranslation(this, typeof(OrFilter));
			FilterRestrictionConverter.RegisterFilterTranslation(this, typeof(NotFilter));
		}

		internal override SortBy[] GetNativeSortBy(SortOrder sortOrder)
		{
			SortOrder sortOrder2 = (sortOrder == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending;
			return new SortBy[]
			{
				new SortBy(InternalSchema.MapiHasAttachment, sortOrder),
				new SortBy(InternalSchema.AllAttachmentsHidden, sortOrder2)
			};
		}

		private static QueryFilter NativeToSmartHasAttachmentFilter
		{
			get
			{
				if (HasAttachmentProperty.nativeToSmartHasAttachmentFilter == null)
				{
					HasAttachmentProperty.nativeToSmartHasAttachmentFilter = new NotFilter(new ComparisonFilter(ComparisonOperator.Equal, InternalSchema.HasAttachment, false));
				}
				return HasAttachmentProperty.nativeToSmartHasAttachmentFilter;
			}
		}

		private static readonly QueryFilter NotHasAttachmentFilter = new OrFilter(new QueryFilter[]
		{
			new AndFilter(new QueryFilter[]
			{
				new ExistsFilter(InternalSchema.AllAttachmentsHidden),
				new ComparisonFilter(ComparisonOperator.Equal, InternalSchema.AllAttachmentsHidden, true)
			}),
			new ComparisonFilter(ComparisonOperator.Equal, InternalSchema.MapiHasAttachment, false)
		});

		private static QueryFilter nativeToSmartHasAttachmentFilter;

		private static readonly QueryFilter HasAttachmentFilter = new NotFilter(HasAttachmentProperty.NotHasAttachmentFilter);
	}
}
