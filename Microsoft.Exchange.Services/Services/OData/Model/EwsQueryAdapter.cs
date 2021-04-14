using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.OData.Web;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal abstract class EwsQueryAdapter : QueryAdapter
	{
		public EwsQueryAdapter(EntitySchema entitySchema, ODataQueryOptions odataQueryOptions) : base(entitySchema, odataQueryOptions)
		{
			this.filterConverter = new EwsFilterConverter(base.EntitySchema);
		}

		public BasePagingType GetPaging()
		{
			IndexedPageView indexedPageView = new IndexedPageView
			{
				MaxRows = base.GetPageSize(),
				Offset = 0,
				Origin = BasePagingType.PagingOrigin.Beginning
			};
			if (base.ODataQueryOptions.Skip != null)
			{
				indexedPageView.Offset = base.ODataQueryOptions.Skip.Value;
			}
			return indexedPageView;
		}

		public RestrictionType GetRestriction()
		{
			if (base.ODataQueryOptions.Filter != null)
			{
				SearchExpressionType item = this.filterConverter.ConvertFilterClause(base.ODataQueryOptions.Filter);
				return new RestrictionType
				{
					Item = item
				};
			}
			return null;
		}

		public SortResults[] GetSortOrder()
		{
			if (base.ODataQueryOptions.OrderBy != null)
			{
				return this.filterConverter.ConvertOrderByClause(base.ODataQueryOptions.OrderBy);
			}
			return null;
		}

		protected PropertyPath[] GetRequestedPropertyPaths()
		{
			List<PropertyInformation> list = new List<PropertyInformation>();
			foreach (PropertyDefinition propertyDefinition in base.RequestedProperties)
			{
				EwsPropertyProvider ewsPropertyProvider = propertyDefinition.EwsPropertyProvider.GetEwsPropertyProvider(base.EntitySchema);
				if (ewsPropertyProvider.IsMultiValueProperty)
				{
					list.AddRange(ewsPropertyProvider.PropertyInformationList);
				}
				else
				{
					list.Add(ewsPropertyProvider.PropertyInformation);
				}
			}
			IEnumerable<PropertyPath> source = from x in list
			select x.PropertyPath;
			return source.ToArray<PropertyPath>();
		}

		protected EwsFilterConverter filterConverter;
	}
}
