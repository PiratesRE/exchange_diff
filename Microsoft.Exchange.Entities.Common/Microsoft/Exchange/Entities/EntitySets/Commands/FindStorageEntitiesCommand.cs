using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.EntitySets.Linq.ExpressionVisitors;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Entities.EntitySets.Commands
{
	internal abstract class FindStorageEntitiesCommand<TContext, TEntity> : FindEntitiesCommand<TContext, TEntity>, IPropertyDefinitionMap where TEntity : IEntity
	{
		protected FindStorageEntitiesCommand()
		{
			base.RegisterOnBeforeExecute(new Action(this.AddRequestedPropertyDependencies));
		}

		public QueryFilter QueryFilter { get; private set; }

		public SortBy[] SortColumns { get; private set; }

		public HashSet<PropertyDefinition> Properties { get; private set; }

		public abstract IDictionary<string, PropertyDefinition> PropertyMap { get; }

		bool IPropertyDefinitionMap.TryGetPropertyDefinition(PropertyInfo propertyInfo, out PropertyDefinition propertyDefinition)
		{
			return this.PropertyMap.TryGetValue(propertyInfo.Name, out propertyDefinition);
		}

		protected override void OnQueryOptionsChanged()
		{
			QueryFilter queryFilter = null;
			SortBy[] sortColumns = null;
			if (base.QueryOptions != null)
			{
				if (base.QueryOptions.Filter != null)
				{
					queryFilter = base.QueryOptions.Filter.ToQueryFilter(this);
				}
				if (base.QueryOptions.OrderBy != null)
				{
					sortColumns = base.QueryOptions.OrderBy.ToSortBy(this).ToArray<SortBy>();
				}
			}
			this.QueryFilter = queryFilter;
			this.SortColumns = sortColumns;
			this.Properties = new HashSet<PropertyDefinition>();
			if (queryFilter != null)
			{
				this.Properties.AddRange(queryFilter.FilterProperties());
			}
			base.OnQueryOptionsChanged();
		}

		protected abstract IEnumerable<PropertyDefinition> GetRequestedPropertyDependencies();

		private void AddRequestedPropertyDependencies()
		{
			IEnumerable<PropertyDefinition> requestedPropertyDependencies = this.GetRequestedPropertyDependencies();
			this.Properties.AddRange(requestedPropertyDependencies);
		}
	}
}
