using System;
using System.ComponentModel;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class FilteredDataTableLoaderView : DataTableLoaderView
	{
		public FilteredDataTableLoaderView()
		{
		}

		public FilteredDataTableLoaderView(DataTableLoader dataTableLoader, IObjectComparer defaultObjectComparer, ObjectToFilterableConverter defaultObjectToFilterableConverter) : base(dataTableLoader, defaultObjectComparer, defaultObjectToFilterableConverter)
		{
		}

		[DefaultValue(null)]
		public QueryFilter PermanentFilter
		{
			get
			{
				return this.permanentFilter;
			}
			set
			{
				if (this.PermanentFilter != value)
				{
					this.permanentFilter = value;
					this.ApplyFilter(this.additionalFilter);
				}
			}
		}

		[DefaultValue(null)]
		public QueryFilter MasterFilter
		{
			get
			{
				return this.masterFilter;
			}
			set
			{
				if (this.MasterFilter != value)
				{
					this.masterFilter = value;
					this.ApplyFilter(this.additionalFilter);
				}
			}
		}

		public override void ApplyFilter(QueryFilter filter)
		{
			this.additionalFilter = filter;
			if (this.PermanentFilter != null)
			{
				if (filter == null)
				{
					filter = this.PermanentFilter;
				}
				else
				{
					filter = new AndFilter(new QueryFilter[]
					{
						filter,
						this.PermanentFilter
					});
				}
			}
			if (this.MasterFilter != null)
			{
				if (filter == null)
				{
					filter = this.MasterFilter;
				}
				else
				{
					filter = new AndFilter(new QueryFilter[]
					{
						filter,
						this.MasterFilter
					});
				}
			}
			base.ApplyFilter(filter);
		}

		public new static FilteredDataTableLoaderView Create(DataTableLoader dataTableLoader)
		{
			return new FilteredDataTableLoaderView(dataTableLoader, ObjectComparer.DefaultObjectComparer, ObjectToFilterableConverter.DefaultObjectToFilterableConverter);
		}

		private QueryFilter permanentFilter;

		private QueryFilter masterFilter;

		private QueryFilter additionalFilter;
	}
}
