using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Services.OData.Web;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class DataEntityQueryAdpater : QueryAdapter
	{
		public DataEntityQueryAdpater(EntitySchema entitySchema, ODataQueryOptions odataQueryOptions) : base(entitySchema, odataQueryOptions)
		{
			this.filterConverter = new DataEntityFilterConverter(base.EntitySchema);
		}

		public IEntityQueryOptions GetEntityQueryOptions()
		{
			DataEntityQueryAdpater.EntityQueryOptions entityQueryOptions = new DataEntityQueryAdpater.EntityQueryOptions();
			entityQueryOptions.Skip = base.ODataQueryOptions.Skip;
			entityQueryOptions.Take = new int?(base.GetPageSize());
			if (base.ODataQueryOptions.Filter != null)
			{
				entityQueryOptions.Filter = this.filterConverter.ConvertFilterClause(base.ODataQueryOptions.Filter);
			}
			if (base.ODataQueryOptions.OrderBy != null)
			{
				entityQueryOptions.OrderBy = this.filterConverter.ConvertOrderByClause(base.ODataQueryOptions.OrderBy);
			}
			return entityQueryOptions;
		}

		protected DataEntityFilterConverter filterConverter;

		private class EntityQueryOptions : IEntityQueryOptions
		{
			public int? Skip { get; set; }

			public IReadOnlyList<OrderByClause> OrderBy { get; set; }

			public int? Take { get; set; }

			public Expression Filter { get; set; }
		}
	}
}
