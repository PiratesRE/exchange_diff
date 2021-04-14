using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class DataTableLoaderSet
	{
		public DataTableLoaderSet() : this(new DataSet())
		{
		}

		public DataTableLoaderSet(DataSet dataSet)
		{
			if (dataSet == null)
			{
				throw new ArgumentNullException("dataSet");
			}
			this.DataSet = dataSet;
		}

		public DataSet DataSet { get; private set; }

		public ReadOnlyCollection<DataTableLoaderConfiguration> DataTableLoaderConfigurations
		{
			get
			{
				if (this.dataTableLoaderConfigurations == null)
				{
					this.dataTableLoaderConfigurations = new ReadOnlyCollection<DataTableLoaderConfiguration>(this.backendDataTableLoaderConfigurations);
				}
				return this.dataTableLoaderConfigurations;
			}
		}

		public DataTableLoaderConfiguration AddDataTableLoaderConfiguration(string name, IDataTableLoaderCreator dataTableLoaderCreator)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("name can not be null or empty.", "name");
			}
			if (dataTableLoaderCreator == null)
			{
				throw new ArgumentNullException("dataTableLoaderCreator");
			}
			DataTableLoaderConfiguration dataTableLoaderConfiguration = new DataTableLoaderConfiguration(name, dataTableLoaderCreator);
			this.backendDataTableLoaderConfigurations.Add(dataTableLoaderConfiguration);
			return dataTableLoaderConfiguration;
		}

		public DataTableLoaderConfiguration GetDataTableLoaderConfiguration(string name)
		{
			foreach (DataTableLoaderConfiguration dataTableLoaderConfiguration in this.DataTableLoaderConfigurations)
			{
				if (dataTableLoaderConfiguration.Name == name)
				{
					return dataTableLoaderConfiguration;
				}
			}
			return null;
		}

		public ReadOnlyCollection<DataTableRelation> DataTableRelations
		{
			get
			{
				if (this.dataTableRelations == null)
				{
					this.dataTableRelations = new ReadOnlyCollection<DataTableRelation>(this.backendDataTableRelations);
				}
				return this.dataTableRelations;
			}
		}

		public void AddDataTableRelation(DataTableRelation relation)
		{
			if (relation == null)
			{
				throw new ArgumentNullException("relation");
			}
			if (!this.backendDataTableRelations.Contains(relation))
			{
				if (relation.DataTableLoaderSet != null && relation.DataTableLoaderSet != this)
				{
					throw new InvalidOperationException(string.Format("the relation '{0}' has been added into anoter DataTableLoaderSet.", relation.RelationName));
				}
				this.backendDataTableRelations.Add(relation);
				relation.DataTableLoaderSet = this;
			}
		}

		public DataTableRelation GetDataTableRelation(string relationName)
		{
			foreach (DataTableRelation dataTableRelation in this.DataTableRelations)
			{
				if (dataTableRelation.RelationName == relationName)
				{
					return dataTableRelation;
				}
			}
			return null;
		}

		private void CreateDataTableLoader(DataTableLoaderConfiguration config)
		{
			if (config.DataTableLoader != null)
			{
				throw new InvalidOperationException(string.Format("DataTableLoader for '{0}' has been created.", config.Name));
			}
			config.CreateDataTableLoader();
			if (this.DataSet.Tables.IndexOf(config.DataTable) == -1)
			{
				this.DataSet.Tables.Add(config.DataTable);
			}
			foreach (DataTableRelation dataTableRelation in this.DataTableRelations)
			{
				if (dataTableRelation.Appliable)
				{
					dataTableRelation.Apply();
				}
			}
		}

		public DataTableLoader GetDataTableLoader(string name)
		{
			DataTableLoaderConfiguration dataTableLoaderConfiguration = this.GetDataTableLoaderConfiguration(name);
			if (dataTableLoaderConfiguration != null)
			{
				if (dataTableLoaderConfiguration.DataTableLoader == null)
				{
					this.CreateDataTableLoader(dataTableLoaderConfiguration);
				}
				return dataTableLoaderConfiguration.DataTableLoader;
			}
			return null;
		}

		public DataTableLoader GetRefreshableSource(string dataMember)
		{
			DataTableLoader dataTableLoader = this.GetDataTableLoader(dataMember);
			if (dataTableLoader == null)
			{
				DataTableRelation dataTableRelation = this.GetDataTableRelation(dataMember);
				if (dataTableRelation != null)
				{
					if (dataTableRelation.ParentDataTableLoaderConfiguration.DataTableLoader == null)
					{
						this.CreateDataTableLoader(dataTableRelation.ParentDataTableLoaderConfiguration);
					}
					if (dataTableRelation.ChildDataTableLoaderConfiguration.DataTableLoader == null)
					{
						this.CreateDataTableLoader(dataTableRelation.ChildDataTableLoaderConfiguration);
					}
					dataTableLoader = dataTableRelation.ChildDataTableLoaderConfiguration.DataTableLoader;
				}
			}
			return dataTableLoader;
		}

		private List<DataTableLoaderConfiguration> backendDataTableLoaderConfigurations = new List<DataTableLoaderConfiguration>();

		private ReadOnlyCollection<DataTableLoaderConfiguration> dataTableLoaderConfigurations;

		private List<DataTableRelation> backendDataTableRelations = new List<DataTableRelation>();

		private ReadOnlyCollection<DataTableRelation> dataTableRelations;
	}
}
