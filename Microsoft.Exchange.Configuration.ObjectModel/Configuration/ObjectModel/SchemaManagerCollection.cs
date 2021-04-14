using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Diagnostics.Components.ObjectModel;

namespace Microsoft.Exchange.Configuration.ObjectModel
{
	internal class SchemaManagerCollection
	{
		public SchemaManagerCollection(Type classType)
		{
			ExTraceGlobals.SchemaManagerTracer.Information<string>((long)this.GetHashCode(), "SchemaManagerCollection::SchemaManagerCollection - creating a schema manager collection for class type {0}", (null == classType) ? "null" : classType.Name);
			this.classType = classType;
			MasterSchemaMappingEntry[] masterSchemaInfos = this.GetMasterSchemaInfos();
			this.schemaManagerArray = new SchemaManager[masterSchemaInfos.Length];
			for (int i = 0; i < masterSchemaInfos.Length; i++)
			{
				this.schemaManagerArray[i] = new SchemaManager(classType, masterSchemaInfos[i]);
			}
		}

		public DataSourceManager[] GetDataSourceManagers(string propertyName)
		{
			DataSourceManager[] array = new DataSourceManager[this.schemaManagerArray.Length];
			int num = 0;
			foreach (SchemaManager schemaManager in this.schemaManagerArray)
			{
				if (schemaManager.GetPersistablePropertyMappings(this.classType.ToString(), propertyName) != null)
				{
					array[num++] = schemaManager.DataSourceManager;
					break;
				}
			}
			foreach (SchemaManager schemaManager2 in this.schemaManagerArray)
			{
				if (schemaManager2.DataSourceManager != array[0])
				{
					array[num++] = schemaManager2.DataSourceManager;
				}
			}
			return array;
		}

		private MasterSchemaMappingEntry[] GetMasterSchemaInfos()
		{
			ExTraceGlobals.SchemaManagerTracer.Information<string>((long)this.GetHashCode(), "SchemaManagerCollection::GetMasterSchemaInfo - retrieving the master schema mapping entries for class type {0}", (null == this.classType) ? "null" : this.classType.Name);
			string fullName = this.classType.FullName;
			List<MasterSchemaMappingEntry> list = new List<MasterSchemaMappingEntry>();
			foreach (MasterSchemaMappingEntry masterSchemaMappingEntry in SchemaManager.masterSchemaMappingEntryArray)
			{
				if (fullName == masterSchemaMappingEntry.ClassName)
				{
					list.Add(masterSchemaMappingEntry);
					break;
				}
			}
			if (list.Count == 0)
			{
				throw new SchemaMappingException(Strings.ExceptionMissingDetailSchemaFile(string.Empty, fullName));
			}
			return list.ToArray();
		}

		private SchemaManager[] schemaManagerArray;

		private Type classType;
	}
}
