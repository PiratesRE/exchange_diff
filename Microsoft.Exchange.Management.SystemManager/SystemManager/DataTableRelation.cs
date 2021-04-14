using System;
using System.Data;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class DataTableRelation
	{
		public DataTableRelation(string relationName, DataTableLoaderConfiguration parentDataTableLoaderConfiguration, string parentColumn, DataTableLoaderConfiguration childDataTableLoaderConfiguration, string childColumn)
		{
			if (string.IsNullOrEmpty(relationName))
			{
				throw new ArgumentException("relationName can not be null or empty.", "relationName");
			}
			if (parentDataTableLoaderConfiguration == null)
			{
				throw new ArgumentNullException("parentDataTableLoaderConfiguration");
			}
			if (string.IsNullOrEmpty(parentColumn))
			{
				throw new ArgumentException("parentColumn can not be null or empty.", "parentColumn");
			}
			if (childDataTableLoaderConfiguration == null)
			{
				throw new ArgumentNullException("childDataTableLoaderConfiguration");
			}
			if (string.IsNullOrEmpty(childColumn))
			{
				throw new ArgumentException("childColumn can not be null or empty.", "childColumn");
			}
			this.RelationName = relationName;
			this.ParentDataTableLoaderConfiguration = parentDataTableLoaderConfiguration;
			this.ParentColumn = parentColumn;
			this.ChildDataTableLoaderConfiguration = childDataTableLoaderConfiguration;
			this.ChildColumn = childColumn;
			this.Nested = true;
		}

		public string RelationName { get; private set; }

		public DataTableLoaderConfiguration ParentDataTableLoaderConfiguration { get; private set; }

		public string ParentColumn { get; private set; }

		public DataTableLoaderConfiguration ChildDataTableLoaderConfiguration { get; private set; }

		public string ChildColumn { get; private set; }

		public bool Nested { get; set; }

		public DataTableLoaderSet DataTableLoaderSet
		{
			get
			{
				return this.dataTableLoaderSet;
			}
			set
			{
				if (this.DataTableLoaderSet != value)
				{
					if (this.DataTableLoaderSet != null)
					{
						throw new InvalidOperationException("Can not change the DataTableLoaderSet once it is setted.");
					}
					this.dataTableLoaderSet = value;
					this.dataTableLoaderSet.AddDataTableRelation(this);
				}
			}
		}

		public bool Appliable
		{
			get
			{
				return this.DataTableLoaderSet != null && this.ParentDataTableLoaderConfiguration.DataTableLoader != null && this.ChildDataTableLoaderConfiguration.DataTableLoader != null;
			}
		}

		public void Apply()
		{
			if (!this.DataTableLoaderSet.DataSet.Relations.Contains(this.RelationName))
			{
				DataRelation dataRelation = new DataRelation(this.RelationName, this.ParentDataTableLoaderConfiguration.DataTable.Columns[this.ParentColumn], this.ChildDataTableLoaderConfiguration.DataTableLoader.Columns[this.ChildColumn], false);
				dataRelation.Nested = this.Nested;
				this.DataTableLoaderSet.DataSet.Relations.Add(dataRelation);
			}
		}

		private DataTableLoaderSet dataTableLoaderSet;
	}
}
