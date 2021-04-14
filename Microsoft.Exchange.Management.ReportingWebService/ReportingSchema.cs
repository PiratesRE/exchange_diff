using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Services.Providers;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.ReportingWebService.PowerShell;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	internal class ReportingSchema
	{
		internal ReportingSchema(string schemaFilePath, string version)
		{
			this.Initialize(schemaFilePath);
		}

		private ReportingSchema(string version)
		{
			this.defaultConfigPath = Path.Combine(ConfigurationContext.Setup.InstallPath, "ClientAccess\\ReportingWebService");
			this.Initialize(this.GetConfigPath(version));
		}

		public Dictionary<string, IEntity> Entities
		{
			get
			{
				return this.entities;
			}
		}

		public Dictionary<string, ResourceType> ComplexTypeResourceTypes
		{
			get
			{
				return this.complexTypeResourceTypes;
			}
		}

		public List<RoleEntry> CmdletFilter { get; private set; }

		public static ReportingSchema GetCurrentReportingSchema(HttpContext httpContext)
		{
			ReportingSchema schema = null;
			ElapsedTimeWatcher.Watch(RequestStatistics.RequestStatItem.GetReportingSchemaLatency, delegate
			{
				string currentReportingVersion = ReportingVersion.GetCurrentReportingVersion(httpContext);
				schema = ReportingSchema.GetReportingSchema(currentReportingVersion);
			});
			return schema;
		}

		public static ReportingSchema GetReportingSchema(string version)
		{
			ReportingSchema schema = null;
			ElapsedTimeWatcher.Watch(RequestStatistics.RequestStatItem.GetReportingSchemaLatency, delegate
			{
				if (!ReportingSchema.SchemaDictionary.TryGetValue(version, out schema))
				{
					lock (ReportingSchema.SyncRoot)
					{
						if (!ReportingSchema.SchemaDictionary.TryGetValue(version, out schema))
						{
							schema = new ReportingSchema(version);
							ReportingSchema.SchemaDictionary[version] = schema;
						}
					}
				}
			});
			return schema;
		}

		public static bool IsNullableType(Type type, out Type underlyingType)
		{
			underlyingType = null;
			bool flag = type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
			if (flag)
			{
				NullableConverter nullableConverter = new NullableConverter(type);
				underlyingType = nullableConverter.UnderlyingType;
			}
			return flag;
		}

		public static void CheckCondition(bool condition, string error)
		{
			if (!condition)
			{
				throw new ReportingSchema.SchemaLoadException(error);
			}
		}

		public static XmlNode SelectSingleNode(XmlNode node, string xpath)
		{
			XmlNode result;
			using (XmlNodeList xmlNodeList = node.SelectNodes(xpath))
			{
				ReportingSchema.CheckCondition(xmlNodeList.Count == 1, string.Format("expect only one {0}.", xpath));
				result = xmlNodeList[0];
			}
			return result;
		}

		private void Initialize(string schemaFilePath)
		{
			this.LoadConfigFile(schemaFilePath);
			this.GenerateComplexTypesSchemaForEntities();
			this.CmdletFilter = this.GetCmdletFilter();
		}

		private void LoadConfigFile(string configPath)
		{
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			if (!File.Exists(configPath))
			{
				ReportingWebServiceEventLogConstants.Tuple_LoadReportingschemaFailed.LogEvent(new object[]
				{
					Strings.ErrorSchemaConfigurationFileMissing(configPath)
				});
				ServiceDiagnostics.ThrowError(ReportingErrorCode.ErrorSchemaInitializationFail, Strings.ErrorSchemaInitializationFail);
			}
			safeXmlDocument.Load(configPath);
			try
			{
				Dictionary<string, ReportingSchema.ReportPropertyCmdletParamMapping[]> reportPropertyCmdletParamMapping = this.LoadRbacMapping(safeXmlDocument);
				this.LoadEntityNodes(safeXmlDocument, reportPropertyCmdletParamMapping);
			}
			catch (ReportingSchema.SchemaLoadException ex)
			{
				ReportingWebServiceEventLogConstants.Tuple_LoadReportingschemaFailed.LogEvent(new object[]
				{
					ex.Message
				});
				ServiceDiagnostics.ThrowError(ReportingErrorCode.ErrorSchemaInitializationFail, Strings.ErrorSchemaInitializationFail, ex);
			}
		}

		private Dictionary<string, ReportingSchema.ReportPropertyCmdletParamMapping[]> LoadRbacMapping(SafeXmlDocument doc)
		{
			Dictionary<string, ReportingSchema.ReportPropertyCmdletParamMapping[]> result;
			using (XmlNodeList xmlNodeList = doc.SelectNodes("/Configuration/CmdletParameterMappings/CmdletParameterMapping"))
			{
				Dictionary<string, ReportingSchema.ReportPropertyCmdletParamMapping[]> dictionary = new Dictionary<string, ReportingSchema.ReportPropertyCmdletParamMapping[]>(xmlNodeList.Count);
				foreach (object obj in xmlNodeList)
				{
					XmlNode xmlNode = (XmlNode)obj;
					using (XmlNodeList xmlNodeList2 = xmlNode.SelectNodes("Cmdlet"))
					{
						using (XmlNodeList xmlNodeList3 = xmlNode.SelectNodes("Mappings/Mapping"))
						{
							ReportingSchema.CheckCondition(xmlNodeList2.Count == 1, "There must be one and only one Cmdlet node under Rbac node.");
							string key = xmlNodeList2[0].Attributes["Name"].Value.Trim();
							ReportingSchema.CheckCondition(!dictionary.ContainsKey(key), "There shouldn't be duplicate Cmdlet under Rbac node.");
							ReportingSchema.CheckCondition(xmlNodeList3.Count > 0, "The mapping shouldn't be empty.");
							ReportingSchema.ReportPropertyCmdletParamMapping[] array = new ReportingSchema.ReportPropertyCmdletParamMapping[xmlNodeList3.Count];
							int num = 0;
							foreach (object obj2 in xmlNodeList3)
							{
								XmlNode xmlNode2 = (XmlNode)obj2;
								array[num++] = new ReportingSchema.ReportPropertyCmdletParamMapping(xmlNode2.Attributes["CmdletParameter"].Value.Trim(), xmlNode2.Attributes["ReportObjectProperty"].Value.Trim());
							}
							dictionary.Add(key, array);
						}
					}
				}
				result = dictionary;
			}
			return result;
		}

		private void LoadEntityNodes(SafeXmlDocument doc, Dictionary<string, ReportingSchema.ReportPropertyCmdletParamMapping[]> reportPropertyCmdletParamMapping)
		{
			using (XmlNodeList xmlNodeList = doc.SelectNodes("/Configuration/Reports/Report"))
			{
				HashSet<string> hashSet = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
				foreach (object obj in xmlNodeList)
				{
					XmlNode xmlNode = (XmlNode)obj;
					ReportingSchema.CheckCondition(!string.IsNullOrWhiteSpace(xmlNode.Attributes["Name"].Value) && !string.IsNullOrWhiteSpace(xmlNode.Attributes["Snapin"].Value) && !string.IsNullOrWhiteSpace(xmlNode.Attributes["Cmdlet"].Value), string.Format("Attributes {0}, {1}, {2} of entity should not be empty.", "Name", "Cmdlet", "Snapin"));
					hashSet.Add(xmlNode.Attributes["Snapin"].Value.Trim());
				}
				using (IPSCommandResolver ipscommandResolver = DependencyFactory.CreatePSCommandResolver(hashSet))
				{
					foreach (object obj2 in xmlNodeList)
					{
						XmlNode xmlNode2 = (XmlNode)obj2;
						string text = xmlNode2.Attributes["Name"].Value.Trim();
						string text2 = xmlNode2.Attributes["Cmdlet"].Value.Trim();
						string snapinName = xmlNode2.Attributes["Snapin"].Value.Trim();
						ReportingSchema.CheckCondition(!this.entities.ContainsKey(text), "Duplicate entity in the config file");
						Dictionary<string, string> dictionary = null;
						using (XmlNodeList xmlNodeList2 = xmlNode2.SelectNodes("CmdletParameters/CmdletParameter"))
						{
							if (xmlNodeList2.Count > 0)
							{
								dictionary = new Dictionary<string, string>(xmlNodeList2.Count);
								foreach (object obj3 in xmlNodeList2)
								{
									XmlNode xmlNode3 = (XmlNode)obj3;
									ReportingSchema.CheckCondition(!string.IsNullOrWhiteSpace(xmlNode3.Attributes["Name"].Value) && !string.IsNullOrWhiteSpace(xmlNode3.Attributes["Value"].Value), "cmdlet parameter name and value should not be empty.");
									string key = xmlNode3.Attributes["Name"].Value.Trim();
									string value = xmlNode3.Attributes["Value"].Value.Trim();
									dictionary.Add(key, value);
								}
							}
						}
						Dictionary<string, List<string>> dictionary2 = null;
						if (reportPropertyCmdletParamMapping.ContainsKey(text2))
						{
							dictionary2 = new Dictionary<string, List<string>>(reportPropertyCmdletParamMapping[text2].Length);
							foreach (ReportingSchema.ReportPropertyCmdletParamMapping reportPropertyCmdletParamMapping2 in reportPropertyCmdletParamMapping[text2])
							{
								if (!dictionary2.ContainsKey(reportPropertyCmdletParamMapping2.ReportObjectProperty))
								{
									dictionary2.Add(reportPropertyCmdletParamMapping2.ReportObjectProperty, new List<string>());
								}
								dictionary2[reportPropertyCmdletParamMapping2.ReportObjectProperty].Add(reportPropertyCmdletParamMapping2.CmdletParameter);
							}
						}
						XmlNode annotationNode = ReportingSchema.SelectSingleNode(xmlNode2, "Annotation");
						IReportAnnotation annotation = DependencyFactory.CreateReportAnnotation(annotationNode);
						IEntity entity = DependencyFactory.CreateEntity(text, new TaskInvocationInfo(text2, snapinName, dictionary), dictionary2, annotation);
						entity.Initialize(ipscommandResolver);
						this.entities.Add(text, entity);
					}
				}
			}
		}

		private void GenerateComplexTypesSchemaForEntities()
		{
			foreach (KeyValuePair<string, IEntity> keyValuePair in this.entities)
			{
				if (ResourceType.GetPrimitiveResourceType(keyValuePair.Value.ClrType) == null)
				{
					this.GenerateComplexTypeSchema(keyValuePair.Value.ClrType, 0);
				}
			}
		}

		private ResourceType GenerateComplexTypeSchema(Type clrType, ResourceTypeKind resourceTypeKind)
		{
			if (this.complexTypeResourceTypes.ContainsKey(clrType.FullName))
			{
				return this.complexTypeResourceTypes[clrType.FullName];
			}
			ResourceType resourceType = new ResourceType(clrType, resourceTypeKind, null, "TenantReporting", clrType.Name, false);
			foreach (PropertyInfo propertyInfo in clrType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy))
			{
				Type type = propertyInfo.PropertyType;
				Type type2;
				if (ReportingSchema.IsNullableType(type, out type2))
				{
					type = type2;
				}
				ResourcePropertyKind resourcePropertyKind = 1;
				ResourceType resourceType2 = ResourceType.GetPrimitiveResourceType(type);
				if (resourceType2 == null)
				{
					if (type.IsEnum || type.IsValueType)
					{
						throw new NotSupportedException("struct and enum are not supported. For struct, try to change it to class. For enum, try to change it to integer or string.");
					}
					if (type.Equals(clrType))
					{
						resourceType2 = resourceType;
					}
					else
					{
						resourceType2 = this.GenerateComplexTypeSchema(type, 1);
					}
					resourcePropertyKind = 4;
				}
				resourceType.AddProperty(new ResourceProperty(propertyInfo.Name, resourcePropertyKind, resourceType2));
			}
			if (resourceTypeKind == 1)
			{
				this.complexTypeResourceTypes.Add(clrType.FullName, resourceType);
			}
			return resourceType;
		}

		private string GetConfigPath(string version)
		{
			string path = this.defaultConfigPath;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchange Control Panel", false))
			{
				if (registryKey != null)
				{
					string text = (string)registryKey.GetValue("TenantReportingSchemaPath");
					if (!string.IsNullOrWhiteSpace(text))
					{
						path = text;
					}
				}
			}
			string path2 = string.Format("ReportingSchema_{0}.xml", version);
			return Path.Combine(path, path2);
		}

		private List<RoleEntry> GetCmdletFilter()
		{
			Dictionary<string, RoleEntry> dictionary = new Dictionary<string, RoleEntry>(StringComparer.OrdinalIgnoreCase);
			foreach (KeyValuePair<string, IEntity> keyValuePair in this.Entities)
			{
				string cmdletName = keyValuePair.Value.TaskInvocationInfo.CmdletName;
				if (!dictionary.ContainsKey(cmdletName))
				{
					CmdletRoleEntry value = new CmdletRoleEntry(cmdletName, keyValuePair.Value.TaskInvocationInfo.SnapinName, null);
					dictionary.Add(cmdletName, value);
				}
			}
			List<RoleEntry> list = dictionary.Values.ToList<RoleEntry>();
			list.Sort(RoleEntry.NameComparer);
			return list;
		}

		public const string ECPRegistryPath = "SYSTEM\\CurrentControlSet\\Services\\MSExchange Control Panel";

		public const string SchemaPathRegistryValueName = "TenantReportingSchemaPath";

		public const string ContainerNamespace = "TenantReporting";

		private const string EntityNodePath = "/Configuration/Reports/Report";

		private const string CmdletParameterMappingNodePath = "/Configuration/CmdletParameterMappings/CmdletParameterMapping";

		private const string EntityNameAttributeName = "Name";

		private const string EntityCmdletAttributeName = "Cmdlet";

		private const string EntitySnapinAttributeName = "Snapin";

		private const string CmdletParameterNodeRelativePath = "CmdletParameters/CmdletParameter";

		private const string CmdletParameterNameAttributeName = "Name";

		private const string CmdletParameterValueAttributeName = "Value";

		private const string CmdletNode = "Cmdlet";

		private const string CmdletNameAttributeName = "Name";

		private const string CmdletParameterMappingRelativePath = "Mappings/Mapping";

		private const string CmdletParameterAttributeName = "CmdletParameter";

		private const string ReportObjectPropertyAttributeName = "ReportObjectProperty";

		private const string AnnotationNode = "Annotation";

		private static readonly Dictionary<string, ReportingSchema> SchemaDictionary = new Dictionary<string, ReportingSchema>(StringComparer.OrdinalIgnoreCase);

		private static readonly object SyncRoot = new object();

		private readonly string defaultConfigPath;

		private readonly Dictionary<string, IEntity> entities = new Dictionary<string, IEntity>();

		private readonly Dictionary<string, ResourceType> complexTypeResourceTypes = new Dictionary<string, ResourceType>();

		private class ReportPropertyCmdletParamMapping
		{
			public ReportPropertyCmdletParamMapping(string cmdletParameter, string reportObjectProperty)
			{
				this.CmdletParameter = cmdletParameter;
				this.ReportObjectProperty = reportObjectProperty;
			}

			public string CmdletParameter { get; private set; }

			public string ReportObjectProperty { get; private set; }
		}

		private class SchemaLoadException : Exception
		{
			public SchemaLoadException(string message) : base(message)
			{
			}
		}
	}
}
