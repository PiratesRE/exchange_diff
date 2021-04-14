using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxSyncPropertyBag : IReadOnlyPropertyBag
	{
		public MailboxSyncPropertyBag(ICollection<PropertyDefinition> initialColumns)
		{
			this.columnDictionary = new Dictionary<PropertyDefinition, int>(initialColumns.Count);
			foreach (PropertyDefinition property in initialColumns)
			{
				this.AddColumn(property);
			}
			this.columnArray = initialColumns;
		}

		public ICollection<PropertyDefinition> Columns
		{
			get
			{
				if (this.columnArray == null)
				{
					PropertyDefinition[] array = new PropertyDefinition[this.columnDictionary.Count];
					foreach (KeyValuePair<PropertyDefinition, int> keyValuePair in this.columnDictionary)
					{
						array[keyValuePair.Value] = keyValuePair.Key;
					}
					this.columnArray = array;
				}
				return this.columnArray;
			}
		}

		public object this[PropertyDefinition propertyDefinition]
		{
			get
			{
				int num;
				if (this.columnDictionary.TryGetValue(propertyDefinition, out num))
				{
					return this.row[num];
				}
				return new PropertyError(propertyDefinition, PropertyErrorCode.NotFound);
			}
		}

		public object this[int idx]
		{
			get
			{
				return this.row[idx];
			}
		}

		public void Bind(object[] row)
		{
			this.row = row;
		}

		public int AddColumn(PropertyDefinition property)
		{
			int count;
			if (!this.columnDictionary.TryGetValue(property, out count))
			{
				count = this.columnDictionary.Count;
				this.columnDictionary[property] = count;
				this.columnArray = null;
			}
			return count;
		}

		public void AddColumnsFromFilter(QueryFilter filter)
		{
			SinglePropertyFilter singlePropertyFilter = filter as SinglePropertyFilter;
			if (singlePropertyFilter != null)
			{
				this.AddColumn(singlePropertyFilter.Property);
				return;
			}
			CompositeFilter compositeFilter = filter as CompositeFilter;
			if (compositeFilter != null)
			{
				foreach (QueryFilter filter2 in compositeFilter.Filters)
				{
					this.AddColumnsFromFilter(filter2);
				}
				return;
			}
			NotFilter notFilter = filter as NotFilter;
			if (notFilter != null)
			{
				this.AddColumnsFromFilter(notFilter.Filter);
				return;
			}
			PropertyComparisonFilter propertyComparisonFilter = filter as PropertyComparisonFilter;
			if (propertyComparisonFilter != null)
			{
				this.AddColumn(propertyComparisonFilter.Property1);
				this.AddColumn(propertyComparisonFilter.Property2);
				return;
			}
			CommentFilter commentFilter = filter as CommentFilter;
			if (commentFilter != null)
			{
				this.AddColumnsFromFilter(commentFilter.Filter);
				return;
			}
			if (filter is FalseFilter || filter is TrueFilter)
			{
				return;
			}
			throw new NotSupportedException();
		}

		public object[] GetProperties(ICollection<PropertyDefinition> propertyDefinitions)
		{
			throw new NotImplementedException();
		}

		private object[] row;

		private Dictionary<PropertyDefinition, int> columnDictionary;

		private ICollection<PropertyDefinition> columnArray;
	}
}
