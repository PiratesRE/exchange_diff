using System;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ConversationMembersQueryResult : QueryResult
	{
		internal ConversationMembersQueryResult(MapiTable mapiTable, ICollection<PropertyDefinition> propertyDefinitions, IList<PropTag> alteredProperties, StoreSession session, bool isTableOwned, SortOrder sortOrder, AggregationExtension aggregationExtension) : base(mapiTable, PropertyDefinitionCollection.Merge<PropertyDefinition>(propertyDefinitions, ConversationMembersQueryResult.RequiredProperties), alteredProperties, session, isTableOwned, sortOrder)
		{
			this.originalProperties = propertyDefinitions;
			this.aggregationExtension = aggregationExtension;
		}

		public override object[][] ExpandRow(int rowCount, long categoryId, out int rowsInExpandedCategory)
		{
			throw new InvalidOperationException();
		}

		public override int CollapseRow(long categoryId)
		{
			throw new InvalidOperationException();
		}

		public override byte[] GetCollapseState(byte[] instanceKey)
		{
			throw new InvalidOperationException();
		}

		public override uint SetCollapseState(byte[] collapseState)
		{
			throw new InvalidOperationException();
		}

		public override object[][] GetRows(int rowCount, QueryRowsFlags flags, out bool mightBeMoreRows)
		{
			base.CheckDisposed("GetRows");
			object[][] items = this.GetItems<object[]>(rowCount, flags, (PropertyBag item) => item.GetProperties<PropertyDefinition>(this.originalProperties));
			mightBeMoreRows = (items.Length > 0);
			return items;
		}

		public override IStorePropertyBag[] GetPropertyBags(int rowCount)
		{
			base.CheckDisposed("GetPropertyBags");
			return this.GetItems<IStorePropertyBag>(rowCount, QueryRowsFlags.None, (PropertyBag item) => item.AsIStorePropertyBag());
		}

		private T PrepareItem<T>(List<IStorePropertyBag> sources, Func<PropertyBag, T> converResultItem)
		{
			this.aggregationExtension.BeforeAggregation(sources);
			PropertyAggregationContext propertyAggregationContext = this.aggregationExtension.GetPropertyAggregationContext(sources);
			PropertyBag arg = ApplicationAggregatedProperty.AggregateAsPropertyBag(propertyAggregationContext, this.originalProperties);
			return converResultItem(arg);
		}

		private T[] GetItems<T>(int rowCount, QueryRowsFlags flags, Func<PropertyBag, T> converResultItem)
		{
			if (rowCount < 0)
			{
				throw new ArgumentOutOfRangeException("rowCount", ServerStrings.ExInvalidRowCount);
			}
			List<T> list = new List<T>(rowCount);
			rowCount = Math.Min(200, rowCount);
			PropValue[][] array = base.Fetch(rowCount, flags);
			if (array != null)
			{
				byte[] array2 = null;
				List<IStorePropertyBag> list2 = null;
				foreach (PropValue[] queryResultRow in array)
				{
					QueryResultPropertyBag queryResultPropertyBag = new QueryResultPropertyBag(base.HeaderPropBag);
					queryResultPropertyBag.SetQueryResultRow(queryResultRow);
					IStorePropertyBag storePropertyBag = queryResultPropertyBag.AsIStorePropertyBag();
					byte[] valueOrDefault = storePropertyBag.GetValueOrDefault<byte[]>(InternalSchema.MapiConversationId, null);
					if (array2 != null && !ArrayComparer<byte>.Comparer.Equals(valueOrDefault, array2))
					{
						list.Add(this.PrepareItem<T>(list2, converResultItem));
						list2 = null;
						array2 = null;
					}
					if (array2 == null)
					{
						array2 = valueOrDefault;
					}
					if (list2 == null)
					{
						list2 = new List<IStorePropertyBag>(1);
					}
					list2.Add(storePropertyBag);
				}
				if (list2 != null)
				{
					list.Add(this.PrepareItem<T>(list2, converResultItem));
				}
			}
			return list.ToArray();
		}

		private const int MaximumNumberOfRowsForExpandedConversationView = 200;

		private static readonly PropertyDefinition[] RequiredProperties = new PropertyTagPropertyDefinition[]
		{
			InternalSchema.MapiConversationId
		};

		private readonly ICollection<PropertyDefinition> originalProperties;

		private readonly AggregationExtension aggregationExtension;
	}
}
