using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Reflection;
using Microsoft.Exchange.Configuration.Common;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.ObjectModel;

namespace Microsoft.Exchange.Configuration.ObjectModel
{
	internal class DataSourceManager : IDisposable, IConfigDataProvider
	{
		public DataSourceManager(SchemaManager schemaManager)
		{
			ExTraceGlobals.DataSourceManagerTracer.Information<string>((long)this.GetHashCode(), "DataSourceManager::DataSourceManager - initializing data source manager for data source info type {0}.", "null");
			this.schemaManager = schemaManager;
		}

		public string Source
		{
			get
			{
				return string.Empty;
			}
		}

		public SchemaManager SchemaManager
		{
			get
			{
				return this.schemaManager;
			}
		}

		public virtual bool IsDisposed
		{
			get
			{
				return this.isDisposed;
			}
		}

		protected Type PersistableType
		{
			get
			{
				return this.schemaManager.PersistableType;
			}
		}

		public static DataSourceManager[] GetDataSourceManagers(Type classType, string propertyName)
		{
			ExTraceGlobals.DataSourceManagerTracer.Information<string, string>(0L, "DataSourceManager::GetDataSourceManagers - retrieving data source managers for class type {0} and data source info type {1}.", classType.Name, "null");
			SchemaManagerCollection schemaManagerCollection = (SchemaManagerCollection)DataSourceManager.schemaManagerHashtable[classType];
			if (schemaManagerCollection == null)
			{
				lock (typeof(DataSourceManager))
				{
					schemaManagerCollection = new SchemaManagerCollection(classType);
					DataSourceManager.schemaManagerHashtable[classType] = schemaManagerCollection;
				}
			}
			return schemaManagerCollection.GetDataSourceManagers(propertyName);
		}

		public virtual void ReadLinked(ConfigObject instanceToRead, Type objectType, string linkValue)
		{
			throw new NotImplementedException("Multiple DSMs are not supported yet.");
		}

		public virtual ConfigObject Read(Type configObjectType, string identity)
		{
			return null;
		}

		public virtual void Save(ConfigObject instanceToSave)
		{
		}

		public virtual void Delete(ConfigObject instanceToDelete)
		{
		}

		public virtual ConfigObject[] Find(Type classType, string searchExpr, bool searchMany, ConfigObject[] objectArgs)
		{
			return null;
		}

		IConfigurable IConfigDataProvider.Read<T>(ObjectId identity)
		{
			return this.Read(typeof(T), ((ConfigObjectId)identity).ToString());
		}

		IConfigurable[] IConfigDataProvider.Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy)
		{
			return (IConfigurable[])this.Find(typeof(T), "", true, null);
		}

		IEnumerable<T> IConfigDataProvider.FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize)
		{
			return (IEnumerable<T>)((IEnumerable<IConfigurable>)this.Find(typeof(T), "", true, null));
		}

		void IConfigDataProvider.Save(IConfigurable instance)
		{
			this.Save((ConfigObject)instance);
		}

		void IConfigDataProvider.Delete(IConfigurable instance)
		{
			this.Delete((ConfigObject)instance);
		}

		public virtual void Dispose()
		{
			ExTraceGlobals.DataSourceManagerTracer.Information((long)this.GetHashCode(), "DataSourceManager::Dispose - disposing of data source session.");
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public virtual object StartRange(string identity, string propertyName, int pageSize)
		{
			return null;
		}

		public virtual bool NextRange(object context, List<object> resultStore)
		{
			return false;
		}

		public virtual void EndRange(object context)
		{
		}

		protected void StampStorageIdentity(PropertyBag propertyBag)
		{
			object obj = propertyBag[DataSourceManager.StorageIdentityName];
			if (obj == null)
			{
				obj = propertyBag["Identity"];
			}
			propertyBag.Add(DataSourceManager.StorageIdentityName + "." + base.GetType().Name, obj);
		}

		protected string GetStorageIdentity(PropertyBag propertyBag)
		{
			return (string)propertyBag[DataSourceManager.StorageIdentityName + "." + base.GetType().Name];
		}

		protected virtual void Dispose(bool disposing)
		{
			ExTraceGlobals.DataSourceManagerTracer.Information((long)this.GetHashCode(), "DataSourceManager::Dispose - disposing of data source session.");
			if (!this.isDisposed && disposing)
			{
				this.isDisposed = true;
			}
		}

		protected virtual object ConvertValue(object valueToConvert, Type newType)
		{
			ExTraceGlobals.DataSourceManagerTracer.Information<object, object, Type>((long)this.GetHashCode(), "DataSourceManager::ConvertValue - converting value {0} from type {1} to type {2}.", (valueToConvert == null) ? "null" : valueToConvert, (valueToConvert == null) ? "null" : valueToConvert.GetType(), newType);
			Type type = valueToConvert.GetType();
			if (type == newType)
			{
				return valueToConvert;
			}
			if (typeof(string) == newType)
			{
				return valueToConvert.ToString();
			}
			if (typeof(bool) == newType)
			{
				return bool.Parse(valueToConvert.ToString());
			}
			if (typeof(int) == newType)
			{
				return int.Parse(valueToConvert.ToString());
			}
			if (typeof(double) == newType)
			{
				return double.Parse(valueToConvert.ToString());
			}
			if (typeof(long) == newType)
			{
				return long.Parse(valueToConvert.ToString());
			}
			if (typeof(DateTime) == newType)
			{
				return DateTime.Parse(valueToConvert.ToString());
			}
			if (typeof(Guid) == newType)
			{
				return new Guid(valueToConvert.ToString());
			}
			if (typeof(IPAddress) == newType)
			{
				return IPAddress.Parse(valueToConvert.ToString());
			}
			if (newType.IsSubclassOf(typeof(Enum)))
			{
				return Enum.Parse(newType, valueToConvert.ToString(), true);
			}
			TypeConverter converter = TypeDescriptor.GetConverter(newType);
			if (converter != null && converter.CanConvertFrom(valueToConvert.GetType()))
			{
				return converter.ConvertFrom(valueToConvert);
			}
			ConstructorInfo constructor = newType.GetConstructor(new Type[]
			{
				type
			});
			if (null != constructor)
			{
				return constructor.Invoke(new object[]
				{
					valueToConvert
				});
			}
			if (typeof(string) == type)
			{
				MethodInfo method = newType.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null, new Type[]
				{
					type
				}, null);
				if (null != method)
				{
					return method.Invoke(null, new object[]
					{
						valueToConvert
					});
				}
			}
			throw new InvalidCastException(Strings.ExceptionNoConversion(valueToConvert.GetType(), newType));
		}

		protected ConfigObject ConstructConfigObject(Type objectType, DataSourceInfo dsi, PropertyBag propertyBag, bool fIsNew)
		{
			ConfigObjectDelegate configObjectDelegate = null;
			ConstructorInfo constructorInfo = null;
			this.StampStorageIdentity(propertyBag);
			object obj = DataSourceManager.constructorHashtable[objectType];
			if (obj == null)
			{
				DataSourceManager.CacheObjectCreationInfo(objectType, ref configObjectDelegate, ref constructorInfo);
			}
			else
			{
				configObjectDelegate = (obj as ConfigObjectDelegate);
				constructorInfo = (obj as ConstructorInfo);
			}
			ConfigObject configObject;
			if (configObjectDelegate != null)
			{
				configObject = configObjectDelegate(propertyBag);
			}
			else
			{
				configObject = (ConfigObject)constructorInfo.Invoke(null);
				configObject.SetIsNew(fIsNew);
				configObject.Fields = propertyBag;
			}
			configObject.SetDataSourceInfo(dsi);
			configObject.InitializeDefaults();
			configObject.Fields.ResetChangeTracking();
			return configObject;
		}

		protected Type GetClassType(string typeName)
		{
			ExTraceGlobals.DataSourceManagerTracer.Information<string>((long)this.GetHashCode(), "DataSourceManager::GetClassType - getting class type for class name {0}.", (typeName == null) ? "null" : typeName);
			Type type = (Type)DataSourceManager.typeHashtable[typeName];
			if (null != type)
			{
				return type;
			}
			type = Assembly.GetCallingAssembly().GetType(typeName, false);
			if (null == type)
			{
				type = Assembly.GetExecutingAssembly().GetType(typeName, false);
				if (null == type)
				{
					foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
					{
						type = assembly.GetType(typeName, false);
						if (null != type)
						{
							break;
						}
					}
				}
			}
			if (null != type)
			{
				lock (typeof(DataSourceManager))
				{
					DataSourceManager.typeHashtable[typeName] = type;
				}
				return type;
			}
			throw new DataSourceManagerException(Strings.ExceptionTypeNotFound(typeName));
		}

		protected void CheckAllowedType(Type configObjectType)
		{
			if (!configObjectType.IsSubclassOf(typeof(ConfigObject)))
			{
				throw new DataSourceManagerException(Strings.ExceptionInvalidConfigObjectType(configObjectType));
			}
			if (configObjectType != this.PersistableType && !configObjectType.IsSubclassOf(this.PersistableType))
			{
				throw new DataSourceManagerException(Strings.ExceptionMismatchedConfigObjectType(this.PersistableType, configObjectType));
			}
		}

		private static void CacheObjectCreationInfo(Type classType, ref ConfigObjectDelegate configObjectDelegate, ref ConstructorInfo configObjectConstructorInfo)
		{
			MethodInfo method = classType.GetMethod("CreateInstance", BindingFlags.Static | BindingFlags.Public);
			object value;
			if (null != method)
			{
				configObjectDelegate = (ConfigObjectDelegate)Delegate.CreateDelegate(typeof(ConfigObjectDelegate), method);
				value = configObjectDelegate;
			}
			else
			{
				configObjectConstructorInfo = classType.GetConstructor(Type.EmptyTypes);
				value = configObjectConstructorInfo;
			}
			lock (typeof(DataSourceManager))
			{
				DataSourceManager.constructorHashtable[classType] = value;
			}
		}

		public static string LinkIdName = "Internal.LinkId";

		public static string StorageIdentityName = "Internal.StorageIdentity";

		private static Hashtable typeHashtable = new Hashtable();

		private static Hashtable constructorHashtable = new Hashtable();

		private static Hashtable schemaManagerHashtable = new Hashtable();

		private bool isDisposed;

		private SchemaManager schemaManager;
	}
}
