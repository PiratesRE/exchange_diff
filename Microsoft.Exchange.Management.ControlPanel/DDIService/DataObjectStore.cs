using System;
using System.Collections.Generic;
using System.Data;
using System.Management.Automation;
using System.Reflection;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.SystemManager;
using Microsoft.Exchange.Management.SystemManager.WinForms;

namespace Microsoft.Exchange.Management.DDIService
{
	public class DataObjectStore
	{
		public DataObjectStore()
		{
		}

		public DataObjectStore(IList<DataObject> list) : this(list, null)
		{
		}

		public DataObjectStore(IList<DataObject> list, Type[] servicePredefinedTypes)
		{
			if (list == null)
			{
				throw new ArgumentException("list cannot be null");
			}
			this.servicePredefinedTypes = servicePredefinedTypes;
			foreach (DataObject dataObject in list)
			{
				this.store[dataObject.Name] = dataObject;
			}
		}

		public List<string> ModifiedColumns
		{
			get
			{
				return this.modifiedProperties;
			}
		}

		public ListAsyncType AsyncType { get; set; }

		public bool IsGetListWorkflow { get; set; }

		internal Type[] ServicePredefinedTypes
		{
			get
			{
				return this.servicePredefinedTypes;
			}
		}

		public void RetrievePropertyInfo(string dataObjectName, string propertyName, out Type type, out PropertyDefinition propertyDefinition)
		{
			type = null;
			propertyDefinition = null;
			if (!string.IsNullOrEmpty(dataObjectName))
			{
				this.store[dataObjectName].Retrieve(propertyName, out type, out propertyDefinition);
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
			this.UpdateDataObject(name, value, false);
		}

		public void UpdateDataObject(string name, object value, bool isDummy)
		{
			this.store[name].Value = value;
			if (isDummy)
			{
				this.AddDummyDataObject(name);
			}
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
			return this.store[name].Value;
		}

		public DataObject GetDataObjectDeclaration(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return null;
			}
			return this.store[name];
		}

		public bool IsDataObjectDummy(string name)
		{
			return this.dummyDataObjects.Contains(name);
		}

		private void AddDummyDataObject(string name)
		{
			if (!this.IsDataObjectDummy(name))
			{
				this.dummyDataObjects.Add(name);
			}
		}

		public object GetValue(string name, string propertyName)
		{
			PropertyInfo propertyEx = this.store[name].Type.GetPropertyEx(propertyName);
			if (!(propertyEx != null) || this.store[name].Value == null)
			{
				return null;
			}
			return propertyEx.GetValue(this.store[name].Value, null);
		}

		public void SetValue(string name, string propertyName, object value, IPropertySetter setter)
		{
			if (DBNull.Value.Equals(value))
			{
				value = null;
			}
			if (setter != null)
			{
				setter.Set(this.store[name].Value, value);
				return;
			}
			if (this.store[name].Value != null)
			{
				PropertyInfo propertyEx = this.store[name].Type.GetPropertyEx(propertyName);
				object value2 = LanguagePrimitives.ConvertTo(value, propertyEx.PropertyType);
				propertyEx.SetValue(this.store[name].Value, value2, null);
			}
		}

		public void SetModifiedColumns(List<string> columns)
		{
			foreach (string item in columns)
			{
				if (!this.ModifiedColumns.Contains(item))
				{
					this.ModifiedColumns.Add(item);
				}
			}
		}

		public void ClearModifiedColumns()
		{
			this.ModifiedColumns.Clear();
		}

		public List<string> GetModifiedPropertiesBasedOnDataObject(DataRow row, string dataObjectName)
		{
			List<string> list = new List<string>();
			foreach (string name in this.ModifiedColumns)
			{
				if (row.Table.Columns.Contains(name))
				{
					Variable variable = row.Table.Columns[name].ExtendedProperties["Variable"] as Variable;
					if (variable != null && !string.IsNullOrEmpty(variable.DataObjectName) && string.Equals(variable.DataObjectName, dataObjectName) && !variable.IgnoreChangeTracking)
					{
						list.Add(variable.MappingProperty);
					}
				}
			}
			return list;
		}

		private Dictionary<string, DataObject> store = new Dictionary<string, DataObject>();

		private List<string> dummyDataObjects = new List<string>();

		private List<string> modifiedProperties = new List<string>();

		private Type[] servicePredefinedTypes;
	}
}
