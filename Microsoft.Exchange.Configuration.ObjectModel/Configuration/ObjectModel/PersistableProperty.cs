using System;

namespace Microsoft.Exchange.Configuration.ObjectModel
{
	[Serializable]
	internal class PersistableProperty : SchemaMappingEntry
	{
		public PersistableProperty()
		{
		}

		public PersistableProperty(string sourceClassName, string sourcePropertyName)
		{
			this.sourceClassName = sourceClassName;
			this.sourcePropertyName = sourcePropertyName;
		}

		public string SourceClassName
		{
			get
			{
				return this.sourceClassName;
			}
			set
			{
				this.sourceClassName = value;
			}
		}

		public string SourcePropertyName
		{
			get
			{
				return this.sourcePropertyName;
			}
			set
			{
				this.sourcePropertyName = value;
			}
		}

		public string PropertyTypeName
		{
			get
			{
				return this.propertyTypeName;
			}
			set
			{
				this.propertyTypeName = value;
				this.propertyType = Type.GetType(this.propertyTypeName);
			}
		}

		public Type PropertyType
		{
			get
			{
				return this.propertyType;
			}
		}

		public string StoragePropertyTypeName
		{
			get
			{
				return this.storagePropertyTypeName;
			}
			set
			{
				this.storagePropertyTypeName = value;
				this.storagePropertyType = Type.GetType(this.storagePropertyTypeName);
			}
		}

		public Type StoragePropertyType
		{
			get
			{
				return this.storagePropertyType;
			}
		}

		private string sourceClassName;

		private string sourcePropertyName;

		private string propertyTypeName;

		private Type propertyType;

		private string storagePropertyTypeName;

		private Type storagePropertyType;
	}
}
