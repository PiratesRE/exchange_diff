using System;

namespace Microsoft.Exchange.Configuration.ObjectModel
{
	[Serializable]
	internal class MasterSchemaMappingEntry
	{
		public MasterSchemaMappingEntry()
		{
		}

		public MasterSchemaMappingEntry(string className, string dataSourceInfoName, string schemaFileName, string dataSourceManagerAssemblyName, string dataSourceInfoPath)
		{
			this.className = className;
			this.dataSourceInfoName = dataSourceInfoName;
			this.schemaFileName = schemaFileName;
			this.dataSourceManagerAssemblyName = dataSourceManagerAssemblyName;
			this.dataSourceInfoPath = dataSourceInfoPath;
		}

		public string ClassName
		{
			get
			{
				return this.className;
			}
			set
			{
				this.className = value;
			}
		}

		public string DataSourceInfoName
		{
			get
			{
				return this.dataSourceInfoName;
			}
			set
			{
				this.dataSourceInfoName = value;
			}
		}

		public string SchemaFileName
		{
			get
			{
				return this.schemaFileName;
			}
			set
			{
				this.schemaFileName = value;
			}
		}

		public string DataSourceManagerAssemblyName
		{
			get
			{
				return this.dataSourceManagerAssemblyName;
			}
			set
			{
				this.dataSourceManagerAssemblyName = value;
			}
		}

		public string DataSourceInfoPath
		{
			get
			{
				return this.dataSourceInfoPath;
			}
			set
			{
				this.dataSourceInfoPath = value;
			}
		}

		private string className;

		private string dataSourceInfoName;

		private string schemaFileName;

		private string dataSourceManagerAssemblyName;

		private string dataSourceInfoPath;
	}
}
