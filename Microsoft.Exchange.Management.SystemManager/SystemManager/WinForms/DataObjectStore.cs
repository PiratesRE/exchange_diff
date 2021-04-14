using System;
using System.Collections.Generic;
using System.Data;
using System.Management.Automation;
using System.Reflection;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.SnapIn;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class DataObjectStore
	{
		public DataObjectStore()
		{
		}

		public DataObjectStore(IList<DataObjectProfile> list)
		{
			if (list == null)
			{
				throw new ArgumentException("list cannot be null");
			}
			foreach (DataObjectProfile dataObjectProfile in list)
			{
				this.store[dataObjectProfile.Name] = dataObjectProfile;
			}
		}

		internal void RetrievePropertyInfo(string dataObjectName, string propertyName, out Type type)
		{
			type = null;
			if (!string.IsNullOrEmpty(dataObjectName))
			{
				this.store[dataObjectName].Retrieve(propertyName, out type);
			}
		}

		public Type GetDataObjectType(string name)
		{
			return this.store[name].Type;
		}

		public IDataObjectCreator GetDataObjectCreator(string name)
		{
			return this.store[name].DataObjectCreator;
		}

		public void UpdateDataObject(string name, object value)
		{
			this.store[name].DataObject = value;
			this.store[name].HasReportCorrupted = false;
		}

		public IList<string> GetKeys()
		{
			IList<string> list = new List<string>();
			foreach (string item in this.store.Keys)
			{
				list.Add(item);
			}
			return list;
		}

		public object GetDataObject(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return null;
			}
			return this.store[name].DataObject;
		}

		public object GetValue(string name, string propertyName)
		{
			PropertyInfo property = this.store[name].Type.GetProperty(propertyName);
			return property.GetValue(this.store[name].DataObject, null);
		}

		public void SetValue(string name, string propertyName, object value, IPropertySetter setter)
		{
			if (DBNull.Value.Equals(value))
			{
				value = null;
			}
			if (setter != null)
			{
				setter.Set(this.store[name].DataObject, value);
				return;
			}
			PropertyInfo property = this.store[name].Type.GetProperty(propertyName);
			object value2 = LanguagePrimitives.ConvertTo(value, property.PropertyType);
			property.SetValue(this.store[name].DataObject, value2, null);
		}

		public DataObjectStore Clone()
		{
			DataObjectStore dataObjectStore = new DataObjectStore();
			dataObjectStore.ModifiedColumns.AddRange(this.ModifiedColumns);
			dataObjectStore.ModifiedColumnsAfterCreation.AddRange(this.ModifiedColumnsAfterCreation);
			foreach (string key in this.store.Keys)
			{
				dataObjectStore.store[key] = (DataObjectProfile)this.store[key].Clone();
			}
			return dataObjectStore;
		}

		public ValidationError[] Validate(DataTable table)
		{
			List<ValidationError> list = new List<ValidationError>();
			foreach (string key in this.store.Keys)
			{
				ValidationError[] array = this.store[key].Validate();
				foreach (ValidationError error in array)
				{
					list.Add(this.ConvertMappingProperty(error, this.store[key].DataObject, table));
				}
			}
			return list.ToArray();
		}

		private ValidationError ConvertMappingProperty(ValidationError error, object dataObject, DataTable table)
		{
			ValidationError result = error;
			PropertyValidationError propertyValidationError = error as PropertyValidationError;
			if (propertyValidationError != null)
			{
				string columnNameThruMappingProperty = this.GetColumnNameThruMappingProperty(propertyValidationError.PropertyDefinition.Name, table, dataObject);
				if (!string.IsNullOrEmpty(columnNameThruMappingProperty))
				{
					PropertyDefinition propertyDefinition = new AdminPropertyDefinition(columnNameThruMappingProperty, ExchangeObjectVersion.Exchange2003, typeof(bool), true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
					result = new PropertyValidationError(propertyValidationError.Description, propertyDefinition, propertyValidationError.InvalidData);
				}
			}
			return result;
		}

		private string GetColumnNameThruMappingProperty(string mappingProperty, DataTable table, object dataObject)
		{
			string result = mappingProperty;
			foreach (object obj in table.Columns)
			{
				DataColumn dataColumn = (DataColumn)obj;
				ColumnProfile columnProfile = dataColumn.ExtendedProperties["ColumnProfile"] as ColumnProfile;
				if (mappingProperty.Equals(columnProfile.MappingProperty) && dataObject == this.GetDataObject(columnProfile.DataObjectName))
				{
					result = dataColumn.ColumnName;
				}
			}
			return result;
		}

		public bool IsCorrupted
		{
			get
			{
				bool flag = false;
				if (PSConnectionInfoSingleton.GetInstance().Type != OrganizationType.Cloud)
				{
					foreach (string key in this.store.Keys)
					{
						IConfigurable configurable = this.store[key].DataObject as IConfigurable;
						if (configurable != null && !this.store[key].HasReportCorrupted)
						{
							flag |= !configurable.IsValid;
							this.store[key].HasReportCorrupted = true;
						}
					}
				}
				return flag;
			}
		}

		public bool OverrideCorruptedValuesWithDefault()
		{
			bool flag = false;
			foreach (string key in this.store.Keys)
			{
				ADObject adobject = this.store[key].DataObject as ADObject;
				if (adobject != null)
				{
					flag |= WinformsHelper.OverrideCorruptedValuesWithDefault(adobject);
				}
			}
			return flag;
		}

		public List<string> ModifiedColumns
		{
			get
			{
				return this.modifiedProperties;
			}
		}

		public List<string> ModifiedColumnsAfterCreation
		{
			get
			{
				return this.modifiedPropertiesAfterCreation;
			}
		}

		public void SetModifiedColumns(IList<string> columns)
		{
			foreach (string item in columns)
			{
				if (!this.ModifiedColumns.Contains(item))
				{
					this.ModifiedColumns.Add(item);
				}
				if (!this.ModifiedColumnsAfterCreation.Contains(item))
				{
					this.ModifiedColumnsAfterCreation.Add(item);
				}
			}
		}

		public void ClearModifiedColumns()
		{
			this.ModifiedColumns.Clear();
		}

		public void ClearModifiedColumns(DataRow row, string dataObjectName)
		{
			List<string> properties = this.GetModifiedPropertiesBasedOnDataObject(row, dataObjectName);
			this.ModifiedColumns.RemoveAll((string c) => properties.Contains(c));
		}

		public List<string> GetModifiedPropertiesBasedOnDataObject(DataRow row, string dataObjectName)
		{
			List<string> list = new List<string>();
			foreach (string name in this.ModifiedColumns)
			{
				if (row.Table.Columns.Contains(name))
				{
					ColumnProfile columnProfile = row.Table.Columns[name].ExtendedProperties["ColumnProfile"] as ColumnProfile;
					if (columnProfile != null && string.Equals(columnProfile.DataObjectName, dataObjectName) && !columnProfile.IgnoreChangeTracking)
					{
						list.Add(columnProfile.MappingProperty);
					}
				}
			}
			return list;
		}

		private Dictionary<string, DataObjectProfile> store = new Dictionary<string, DataObjectProfile>();

		private List<string> modifiedProperties = new List<string>();

		private List<string> modifiedPropertiesAfterCreation = new List<string>();
	}
}
