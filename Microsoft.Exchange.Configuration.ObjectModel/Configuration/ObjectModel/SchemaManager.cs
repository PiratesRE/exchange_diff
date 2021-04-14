using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics.Components.ObjectModel;

namespace Microsoft.Exchange.Configuration.ObjectModel
{
	[Serializable]
	internal class SchemaManager
	{
		internal SchemaManager(Type persistableType, MasterSchemaMappingEntry masterSchemaMappingEntry)
		{
			ExTraceGlobals.SchemaManagerTracer.Information<string>((long)this.GetHashCode(), "SchemaManager::SchemaManager - creating a schema manager for class type {0}", masterSchemaMappingEntry.ClassName);
			this.persistableType = persistableType;
			this.LoadDetailSchemaInfo(masterSchemaMappingEntry);
		}

		public DataSourceManager DataSourceManager
		{
			get
			{
				return this.dataSourceManager;
			}
		}

		internal Type PersistableType
		{
			get
			{
				return this.persistableType;
			}
		}

		public SchemaMappingEntry[] GetAllMappings()
		{
			ExTraceGlobals.SchemaManagerTracer.Information((long)this.GetHashCode(), "SchemaManager::GetAllMappings - retrieving all schema mappings.");
			return this.schemaMappingEntryArray;
		}

		public PersistableClass[] GetPersistableClassMappings(string className)
		{
			ExTraceGlobals.SchemaManagerTracer.Information<string>((long)this.GetHashCode(), "SchemaManager::GetPersistableClassMappings - retrieving all persistable class mappings for class {0}.", className);
			return this.FilterSchemaMappingEntry<PersistableClass>((SchemaMappingEntry schemaMappingEntry) => schemaMappingEntry is PersistableClass && ((PersistableClass)schemaMappingEntry).SourceClassName == className);
		}

		public PersistableClass[] GetPersistableClassMappings(string className, Type mappingType)
		{
			ExTraceGlobals.SchemaManagerTracer.Information<string, Type>((long)this.GetHashCode(), "SchemaManager::GetPersistableClassMappings - retrieving all persistable class mappings for class {0} and mapping type {1}.", className, mappingType);
			return this.FilterSchemaMappingEntry<PersistableClass>((SchemaMappingEntry schemaMappingEntry) => schemaMappingEntry is PersistableClass && ((PersistableClass)schemaMappingEntry).SourceClassName == className && mappingType.IsInstanceOfType(schemaMappingEntry));
		}

		public PersistableProperty[] GetPersistablePropertyMappings(string className)
		{
			ExTraceGlobals.SchemaManagerTracer.Information<string>((long)this.GetHashCode(), "SchemaManager::GetPersistablePropertyMappings - retrieving all persistable property mappings for class {0}.", className);
			return this.FilterSchemaMappingEntry<PersistableProperty>((SchemaMappingEntry schemaMappingEntry) => schemaMappingEntry is PersistableProperty && ((PersistableProperty)schemaMappingEntry).SourceClassName == className);
		}

		public PersistableProperty[] GetPersistablePropertyMappings(string className, Type mappingType)
		{
			ExTraceGlobals.SchemaManagerTracer.Information<string, Type>((long)this.GetHashCode(), "SchemaManager::GetPersistablePropertyMappings - retrieving all persistable property mappings for class {0} and mapping type {1}.", className, mappingType);
			return this.FilterSchemaMappingEntry<PersistableProperty>((SchemaMappingEntry schemaMappingEntry) => schemaMappingEntry is PersistableProperty && ((PersistableProperty)schemaMappingEntry).SourceClassName == className && mappingType.IsInstanceOfType(schemaMappingEntry));
		}

		public PersistableProperty[] GetPersistablePropertyMappings(string className, string propertyName)
		{
			ExTraceGlobals.SchemaManagerTracer.Information<string, string>((long)this.GetHashCode(), "SchemaManager::GetPersistablePropertyMappings - retrieving all persistable property mappings for class {0} and property {1}.", className, propertyName);
			return this.FilterSchemaMappingEntry<PersistableProperty>((SchemaMappingEntry schemaMappingEntry) => schemaMappingEntry is PersistableProperty && ((PersistableProperty)schemaMappingEntry).SourceClassName == className && ((PersistableProperty)schemaMappingEntry).SourcePropertyName == propertyName);
		}

		public PersistableProperty[] GetPersistablePropertyMappings(string className, string propertyName, Type mappingType)
		{
			ExTraceGlobals.SchemaManagerTracer.Information<string, string, Type>((long)this.GetHashCode(), "SchemaManager::GetPersistablePropertyMappings - retrieving all persistable property mappings for class {0} and property name {1} and mapping type {2}.", className, propertyName, mappingType);
			return this.FilterSchemaMappingEntry<PersistableProperty>((SchemaMappingEntry schemaMappingEntry) => schemaMappingEntry is PersistableProperty && ((PersistableProperty)schemaMappingEntry).SourceClassName == className && ((PersistableProperty)schemaMappingEntry).SourcePropertyName == propertyName && mappingType.IsInstanceOfType(schemaMappingEntry));
		}

		public SearchableProperty[] GetSearchablePropertyMappings(string className)
		{
			ExTraceGlobals.SchemaManagerTracer.Information<string>((long)this.GetHashCode(), "SchemaManager::GetSearchablePropertyMappings - retrieving all searchable property mappings for class {0}.", className);
			return this.FilterSchemaMappingEntry<SearchableProperty>((SchemaMappingEntry schemaMappingEntry) => schemaMappingEntry is SearchableProperty && ((SearchableProperty)schemaMappingEntry).SourceClassName == className);
		}

		public SearchableProperty[] GetSearchablePropertyMappings(string className, Type mappingType)
		{
			ExTraceGlobals.SchemaManagerTracer.Information<string, Type>((long)this.GetHashCode(), "SchemaManager::GetSearchablePropertyMappings - retrieving all searchable property mappings for class {0} and mapping type {1}.", className, mappingType);
			return this.FilterSchemaMappingEntry<SearchableProperty>((SchemaMappingEntry schemaMappingEntry) => schemaMappingEntry is SearchableProperty && ((SearchableProperty)schemaMappingEntry).SourceClassName == className && mappingType.IsInstanceOfType(schemaMappingEntry));
		}

		public SearchableProperty[] GetSearchablePropertyMappings(string className, string propertyName)
		{
			ExTraceGlobals.SchemaManagerTracer.Information<string, string>((long)this.GetHashCode(), "SchemaManager::GetSearchablePropertyMappings - retrieving all searchable property mappings for class {0} and property name {1}.", className, propertyName);
			return this.FilterSchemaMappingEntry<SearchableProperty>((SchemaMappingEntry schemaMappingEntry) => schemaMappingEntry is SearchableProperty && ((SearchableProperty)schemaMappingEntry).SourceClassName == className && ((SearchableProperty)schemaMappingEntry).SourcePropertyName == propertyName);
		}

		public SearchableProperty[] GetSearchablePropertyMappings(string className, string propertyName, Type mappingType)
		{
			ExTraceGlobals.SchemaManagerTracer.Information<string, string, Type>((long)this.GetHashCode(), "SchemaManager::GetSearchablePropertyMappings - retrieving all searchable property mappings for class {0} and property name {1} and mapping type {2}.", className, propertyName, mappingType);
			return this.FilterSchemaMappingEntry<SearchableProperty>((SchemaMappingEntry schemaMappingEntry) => schemaMappingEntry is SearchableProperty && ((SearchableProperty)schemaMappingEntry).SourceClassName == className && ((SearchableProperty)schemaMappingEntry).SourcePropertyName == propertyName && mappingType.IsInstanceOfType(schemaMappingEntry));
		}

		public SchemaMappingEntry[] GetMappingsOfType(Type mappingType)
		{
			ExTraceGlobals.SchemaManagerTracer.Information<Type>((long)this.GetHashCode(), "SchemaManager::GetMappingsOfType - retrieving all mappings of type {0}.", mappingType);
			return this.FilterSchemaMappingEntry<SchemaMappingEntry>((SchemaMappingEntry schemaMappingEntry) => mappingType.IsInstanceOfType(schemaMappingEntry));
		}

		private T[] FilterSchemaMappingEntry<T>(SchemaManager.SchemaMappingEntryCondition condition) where T : SchemaMappingEntry
		{
			List<T> list = new List<T>();
			foreach (SchemaMappingEntry schemaMappingEntry in this.schemaMappingEntryArray)
			{
				if (condition(schemaMappingEntry))
				{
					list.Add((T)((object)schemaMappingEntry));
				}
			}
			return list.ToArray();
		}

		protected static MasterSchemaMappingEntry[] GetMasterSchemaMappings()
		{
			return new MasterSchemaMappingEntry[0];
		}

		protected static Stream GetSchemaXmlDocument(string documentName)
		{
			Stream stream = null;
			Assembly entryAssembly = Assembly.GetEntryAssembly();
			if (null != entryAssembly)
			{
				try
				{
					stream = entryAssembly.GetManifestResourceStream(documentName);
				}
				catch (NotSupportedException)
				{
				}
			}
			if (stream != null)
			{
				return stream;
			}
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				try
				{
					stream = assembly.GetManifestResourceStream(documentName);
				}
				catch (NotSupportedException)
				{
				}
				catch (SecurityException)
				{
				}
				if (stream != null)
				{
					return stream;
				}
			}
			string path = Path.Combine(SchemaManager.currentDirectory, documentName);
			FileStream fileStream = null;
			try
			{
				fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
			}
			catch (FileNotFoundException)
			{
			}
			catch (DirectoryNotFoundException)
			{
			}
			if (fileStream != null)
			{
				return fileStream;
			}
			path = Path.Combine(ConfigurationContext.Setup.DataPath, documentName);
			try
			{
				fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
			}
			catch (FileNotFoundException)
			{
			}
			catch (DirectoryNotFoundException)
			{
			}
			if (fileStream != null)
			{
				return fileStream;
			}
			path = Path.Combine(ConfigurationContext.Setup.BinPath, documentName);
			try
			{
				fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
			}
			catch (FileNotFoundException)
			{
			}
			catch (DirectoryNotFoundException)
			{
			}
			if (fileStream != null)
			{
				return fileStream;
			}
			throw new FileNotFoundException(documentName);
		}

		protected static string GetCurrentDirectory()
		{
			Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
			return Path.GetDirectoryName(uri.LocalPath);
		}

		protected void LoadDetailSchemaInfo(MasterSchemaMappingEntry masterSchemaMappingEntry)
		{
			ExTraceGlobals.SchemaManagerTracer.Information<string, string>((long)this.GetHashCode(), "SchemaManager::LoadDetailSchemaInfo - retrieving info from detail schema file {0} and DataSourceManager assembly {1}.", (masterSchemaMappingEntry == null) ? "null" : masterSchemaMappingEntry.SchemaFileName, (masterSchemaMappingEntry == null) ? "null" : masterSchemaMappingEntry.DataSourceManagerAssemblyName);
			this.LoadDetailSchemaMappings(masterSchemaMappingEntry);
			this.LoadDataSourceManager(masterSchemaMappingEntry);
		}

		protected void LoadDetailSchemaMappings(MasterSchemaMappingEntry masterSchemaMappingEntry)
		{
			ExTraceGlobals.SchemaManagerTracer.Information<string>((long)this.GetHashCode(), "SchemaManager::LoadDetailSchemaMappings - retrieving info from detail schema mapping XML file {0}.", (masterSchemaMappingEntry == null) ? "null" : masterSchemaMappingEntry.SchemaFileName);
			string schemaFileName = masterSchemaMappingEntry.SchemaFileName;
			this.schemaMappingEntryArray = (SchemaMappingEntry[])SchemaManager.schemaMappingEntryArrayHashTable[schemaFileName];
			if (this.schemaMappingEntryArray == null)
			{
				lock (typeof(SchemaManager))
				{
					string dataSourceManagerAssemblyName = masterSchemaMappingEntry.DataSourceManagerAssemblyName;
					Assembly assembly = (Assembly)SchemaManager.dsmAssemblyHashTable[dataSourceManagerAssemblyName];
					if (null == assembly)
					{
						string assemblyString = dataSourceManagerAssemblyName;
						if (Path.GetExtension(dataSourceManagerAssemblyName).ToLower() == ".dll")
						{
							assemblyString = Path.GetFileNameWithoutExtension(dataSourceManagerAssemblyName);
						}
						try
						{
							assembly = Assembly.Load(assemblyString);
						}
						catch (FileNotFoundException)
						{
						}
						if (assembly == null)
						{
							string assemblyFile = Path.Combine(ConfigurationContext.Setup.BinPath, dataSourceManagerAssemblyName);
							assembly = Assembly.LoadFrom(assemblyFile);
						}
						SchemaManager.dsmAssemblyHashTable[dataSourceManagerAssemblyName] = assembly;
					}
					List<Type> list = new List<Type>();
					foreach (Type type in assembly.GetTypes())
					{
						if (type.IsSubclassOf(typeof(SchemaMappingEntry)))
						{
							list.Add(type);
						}
					}
					Assembly executingAssembly = Assembly.GetExecutingAssembly();
					foreach (Type type2 in executingAssembly.GetTypes())
					{
						if (type2.IsSubclassOf(typeof(SchemaMappingEntry)))
						{
							list.Add(type2);
						}
					}
					Type[] extraTypes = list.ToArray();
					SafeXmlSerializer safeXmlSerializer = new SafeXmlSerializer(typeof(SchemaMappingEntry[]), extraTypes);
					Stream schemaXmlDocument = SchemaManager.GetSchemaXmlDocument(masterSchemaMappingEntry.SchemaFileName);
					using (schemaXmlDocument)
					{
						this.schemaMappingEntryArray = (SchemaMappingEntry[])safeXmlSerializer.Deserialize(schemaXmlDocument);
						SchemaManager.schemaMappingEntryArrayHashTable[masterSchemaMappingEntry.SchemaFileName] = this.schemaMappingEntryArray;
					}
				}
			}
		}

		protected void LoadDataSourceManager(MasterSchemaMappingEntry masterSchemaMappingEntry)
		{
			ExTraceGlobals.SchemaManagerTracer.Information<string>((long)this.GetHashCode(), "SchemaManager::LoadDataSourceManager - retrieving info from DataSourceManager assembly {0}.", (masterSchemaMappingEntry == null) ? "null" : masterSchemaMappingEntry.DataSourceManagerAssemblyName);
			DataSourceManagerDelegate dataSourceManagerDelegate = (DataSourceManagerDelegate)SchemaManager.dsmDelegateHashTable[masterSchemaMappingEntry.DataSourceManagerAssemblyName];
			if (dataSourceManagerDelegate == null)
			{
				Assembly assembly = (Assembly)SchemaManager.dsmAssemblyHashTable[masterSchemaMappingEntry.DataSourceManagerAssemblyName];
				lock (typeof(SchemaManager))
				{
					Type[] types = assembly.GetTypes();
					int i = 0;
					while (i < types.Length)
					{
						Type type = types[i];
						if (type.IsSubclassOf(typeof(DataSourceManager)))
						{
							MethodInfo method = type.GetMethod("CreateInstance", BindingFlags.Static | BindingFlags.Public);
							if (null == method)
							{
								throw new SchemaMappingException(Strings.ExceptionMissingCreateInstance(type, assembly.CodeBase));
							}
							dataSourceManagerDelegate = (DataSourceManagerDelegate)Delegate.CreateDelegate(typeof(DataSourceManagerDelegate), method);
							string dataSourceManagerAssemblyName = masterSchemaMappingEntry.DataSourceManagerAssemblyName;
							SchemaManager.dsmDelegateHashTable[dataSourceManagerAssemblyName] = dataSourceManagerDelegate;
							break;
						}
						else
						{
							i++;
						}
					}
				}
				if (dataSourceManagerDelegate == null)
				{
					throw new SchemaMappingException(Strings.ExceptionMissingDataSourceManager(assembly.CodeBase));
				}
			}
			this.dataSourceManager = dataSourceManagerDelegate(this, masterSchemaMappingEntry.DataSourceInfoPath);
		}

		internal static readonly MasterSchemaMappingEntry[] masterSchemaMappingEntryArray = SchemaManager.GetMasterSchemaMappings();

		private static readonly string currentDirectory = SchemaManager.GetCurrentDirectory();

		private static Hashtable schemaMappingEntryArrayHashTable = new Hashtable();

		private static Hashtable dsmAssemblyHashTable = new Hashtable();

		private static Hashtable dsmDelegateHashTable = new Hashtable();

		private SchemaMappingEntry[] schemaMappingEntryArray;

		private DataSourceManager dataSourceManager;

		private Type persistableType;

		private delegate bool SchemaMappingEntryCondition(SchemaMappingEntry schemaMappingEntry);
	}
}
