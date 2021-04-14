using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Services.OData.Web;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class UserQueryAdapter : QueryAdapter
	{
		public UserQueryAdapter(UserSchema entitySchema, ODataQueryOptions odataQueryOptions) : base(entitySchema, odataQueryOptions)
		{
			this.filterConverter = new ADDriverFilterConverter(base.EntitySchema);
		}

		public QueryFilter GetQueryFilter()
		{
			if (base.ODataQueryOptions.Filter != null)
			{
				return this.filterConverter.ConvertFilterClause(base.ODataQueryOptions.Filter);
			}
			return null;
		}

		public SortBy GetSortBy()
		{
			if (base.ODataQueryOptions.OrderBy != null)
			{
				return this.filterConverter.ConvertOrderByClause(base.ODataQueryOptions.OrderBy);
			}
			return new SortBy(ADObjectSchema.Name, SortOrder.Ascending);
		}

		public List<ADPropertyDefinition> GetRequestedADProperties()
		{
			List<ADPropertyDefinition> list = new List<ADPropertyDefinition>(base.RequestedProperties.Count);
			foreach (PropertyDefinition propertyDefinition in base.RequestedProperties)
			{
				DirectoryPropertyProvider directoryPropertyProvider = propertyDefinition.ADDriverPropertyProvider as DirectoryPropertyProvider;
				list.Add(directoryPropertyProvider.ADPropertyDefinition);
			}
			return list;
		}

		public int GetSkipCount()
		{
			int result = 0;
			if (base.ODataQueryOptions.Skip != null)
			{
				result = base.ODataQueryOptions.Skip.Value;
			}
			return result;
		}

		private ADDriverFilterConverter filterConverter;
	}
}
