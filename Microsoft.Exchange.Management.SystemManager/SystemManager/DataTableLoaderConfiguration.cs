using System;
using System.Data;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class DataTableLoaderConfiguration
	{
		public DataTableLoaderConfiguration(string name, IDataTableLoaderCreator dataTableLoaderCreator)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("name can not be null or empty.", "name");
			}
			if (dataTableLoaderCreator == null)
			{
				throw new ArgumentNullException("dataTableLoaderCreator");
			}
			this.Name = name;
			this.DataTableLoaderCreator = dataTableLoaderCreator;
		}

		public string Name { get; private set; }

		public IDataTableLoaderCreator DataTableLoaderCreator { get; private set; }

		public DataTableLoader DataTableLoader { get; private set; }

		public DataTable DataTable
		{
			get
			{
				if (this.DataTableLoader != null)
				{
					return this.DataTableLoader.Table;
				}
				return null;
			}
		}

		public void CreateDataTableLoader()
		{
			if (this.DataTableLoader != null)
			{
				throw new InvalidOperationException("The DataTableLoader has been created.");
			}
			this.DataTableLoader = this.DataTableLoaderCreator.CreateDataTableLoader(this.Name);
		}
	}
}
