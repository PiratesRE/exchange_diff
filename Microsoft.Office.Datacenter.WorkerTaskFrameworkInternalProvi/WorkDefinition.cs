using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	public abstract class WorkDefinition : IWorkData
	{
		public WorkDefinition()
		{
			this.Enabled = true;
			this.CreatedTime = DateTime.UtcNow;
			this.StartTime = DateTime.MinValue;
			this.UpdateTime = DateTime.UtcNow;
			this.DeploymentId = Settings.DeploymentId;
			this.TargetResource = string.Empty;
		}

		public abstract int Id { get; internal set; }

		public abstract string AssemblyPath { get; set; }

		public abstract string TypeName { get; set; }

		public abstract string Name { get; set; }

		public abstract string WorkItemVersion { get; set; }

		[PropertyInformation("The name of the target service.", false)]
		public abstract string ServiceName { get; set; }

		public abstract int DeploymentId { get; set; }

		public abstract string ExecutionLocation { get; set; }

		public abstract DateTime CreatedTime { get; internal set; }

		public abstract bool Enabled { get; set; }

		public abstract string TargetPartition { get; set; }

		public abstract string TargetGroup { get; set; }

		[PropertyInformation("The name of the target resource.", false)]
		public abstract string TargetResource { get; set; }

		public abstract string TargetExtension { get; set; }

		public abstract string TargetVersion { get; set; }

		public abstract int RecurrenceIntervalSeconds { get; set; }

		[PropertyInformation("The amount of time to wait before assuming the probe is hung.", false)]
		public abstract int TimeoutSeconds { get; set; }

		public abstract DateTime StartTime { get; set; }

		public abstract DateTime UpdateTime { get; internal set; }

		public abstract int MaxRetryAttempts { get; set; }

		public abstract int CreatedById { get; set; }

		public string DeploymentSourceName { get; set; }

		public DateTime? LastExecutionStartTime { get; internal set; }

		public Dictionary<string, string> Attributes
		{
			get
			{
				if (this.attributes == null)
				{
					this.attributes = new Dictionary<string, string>();
				}
				return this.attributes;
			}
		}

		public byte PoisonedCount
		{
			get
			{
				if (this.PoisonedResultCount == null)
				{
					return 0;
				}
				return this.PoisonedResultCount.Value;
			}
			internal set
			{
				this.PoisonedResultCount = new byte?(value);
			}
		}

		public abstract string ExtensionAttributes { get; internal set; }

		public virtual string InternalStorageKey
		{
			get
			{
				return this.internalStorageKey;
			}
			internal set
			{
				this.internalStorageKey = value;
			}
		}

		public virtual string ExternalStorageKey
		{
			get
			{
				return this.Name;
			}
		}

		public virtual string SecondaryExternalStorageKey
		{
			get
			{
				return string.Format("{0}_{1}_{2}_{3}_{4}", new object[]
				{
					Settings.InstanceName,
					Settings.MachineName,
					this.DeploymentSourceName,
					this.TargetVersion,
					this.Id
				});
			}
		}

		internal abstract int Version { get; set; }

		internal Type WorkItemType
		{
			get
			{
				return this.workItemType;
			}
			set
			{
				this.workItemType = value;
			}
		}

		internal object ObjectData
		{
			get
			{
				return this.objectData;
			}
			set
			{
				this.objectData = value;
			}
		}

		internal DateTime IntendedStartTime { get; set; }

		internal TracingContext TraceContext
		{
			get
			{
				return new TracingContext(null)
				{
					Id = this.Id,
					LId = this.ExecutionId
				};
			}
		}

		internal int ExecutionId
		{
			get
			{
				return this.GetHashCode();
			}
		}

		protected internal byte? PoisonedResultCount { get; internal set; }

		public void SetType(Type implementingType)
		{
			this.AssemblyPath = implementingType.Assembly.Location;
			this.TypeName = implementingType.FullName;
		}

		public override string ToString()
		{
			return string.Format("{0}: {1}", base.ToString(), this.Id);
		}

		public virtual void FromXml(XmlNode definition)
		{
			this.AssemblyPath = this.GetMandatoryXmlAttribute<string>(definition, "AssemblyPath");
			this.TypeName = this.GetMandatoryXmlAttribute<string>(definition, "TypeName");
			this.Name = this.GetMandatoryXmlAttribute<string>(definition, "Name");
			this.ServiceName = this.GetMandatoryXmlAttribute<string>(definition, "ServiceName");
			this.RecurrenceIntervalSeconds = this.GetMandatoryXmlAttribute<int>(definition, "RecurrenceIntervalSeconds");
			this.TimeoutSeconds = this.GetMandatoryXmlAttribute<int>(definition, "TimeoutSeconds");
			this.MaxRetryAttempts = this.GetMandatoryXmlAttribute<int>(definition, "MaxRetryAttempts");
			this.Enabled = this.GetMandatoryXmlAttribute<bool>(definition, "Enabled");
			this.DeploymentSourceName = Path.GetFileName(definition.BaseURI);
			XmlNode xmlNode = definition.SelectSingleNode("ExtensionAttributes");
			if (xmlNode != null)
			{
				this.ExtensionAttributes = xmlNode.OuterXml;
				WTFDiagnostics.TraceDebug(WTFLog.DataAccess, this.TraceContext, "[WorkDefinition.FromXml]: Attempting to parse extension attributes.", null, "FromXml", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\WorkDefinition.cs", 431);
				this.ParseExtensionAttributes(false);
				WTFDiagnostics.TraceDebug(WTFLog.DataAccess, this.TraceContext, "[WorkDefinition.FromXml]: Successfully parsed extension attributes.", null, "FromXml", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\WorkDefinition.cs", 435);
			}
			this.TargetPartition = this.GetOptionalXmlAttribute<string>(definition, "TargetPartition", string.Empty);
			this.TargetGroup = this.GetOptionalXmlAttribute<string>(definition, "TargetGroup", string.Empty);
			this.TargetResource = this.GetOptionalXmlAttribute<string>(definition, "TargetResource", string.Empty);
			this.TargetVersion = this.GetOptionalXmlAttribute<string>(definition, "TargetVersion", string.Empty);
			this.TargetExtension = this.GetOptionalXmlAttribute<string>(definition, "TargetExtension", string.Empty);
			this.WorkItemVersion = this.GetOptionalXmlAttribute<string>(definition, "WorkItemVersion", "Binaries");
			this.DeploymentId = this.GetOptionalXmlAttribute<int>(definition, "DeploymentId", 0);
			this.ExecutionLocation = this.GetOptionalXmlAttribute<string>(definition, "ExecutionLocation", string.Empty);
			this.CreatedTime = this.GetOptionalXmlAttribute<DateTime>(definition, "CreatedTime", DateTime.UtcNow);
			this.StartTime = this.GetOptionalXmlAttribute<DateTime>(definition, "StartTime", DateTime.UtcNow);
			this.UpdateTime = this.GetOptionalXmlAttribute<DateTime>(definition, "UpdateTime", DateTime.UtcNow);
			this.LastExecutionStartTime = new DateTime?(this.GetOptionalXmlAttribute<DateTime>(definition, "LastExecutionStartTime", DateTime.UtcNow));
		}

		public string ConstructWorkItemResultName()
		{
			StringBuilder stringBuilder = new StringBuilder(this.Name);
			if (!string.IsNullOrWhiteSpace(this.TargetPartition))
			{
				stringBuilder.Append("/");
				stringBuilder.Append(this.TargetPartition);
			}
			if (!string.IsNullOrWhiteSpace(this.TargetGroup))
			{
				stringBuilder.Append("/");
				stringBuilder.Append(this.TargetGroup);
			}
			if (!string.IsNullOrWhiteSpace(this.TargetResource))
			{
				stringBuilder.Append("/");
				stringBuilder.Append(this.TargetResource);
			}
			if (!string.IsNullOrWhiteSpace(this.TargetExtension))
			{
				stringBuilder.Append("/");
				stringBuilder.Append(this.TargetExtension);
			}
			return stringBuilder.ToString();
		}

		internal static string SerializeExtensionAttributes(IDictionary<string, string> attributes)
		{
			if (attributes != null && attributes.Count > 0)
			{
				return new XElement("ExtensionAttributes", from pair in attributes
				select new XAttribute(pair.Key, pair.Value)).ToString(SaveOptions.DisableFormatting);
			}
			return null;
		}

		internal string GetWorkItemResultTokens(int numberOfTokens)
		{
			IEnumerable<string> values = this.ConstructWorkItemResultName().Split(new string[]
			{
				"/"
			}, StringSplitOptions.RemoveEmptyEntries).Take(numberOfTokens);
			return string.Join("/", values);
		}

		internal ReturnType GetOptionalXmlAttribute<ReturnType>(XmlNode definition, string attributeName, ReturnType defaultValue)
		{
			ReturnType result = default(ReturnType);
			XmlAttribute xmlAttribute = this.GetXmlAttribute(definition, attributeName, false);
			if (xmlAttribute == null)
			{
				WTFDiagnostics.TraceDebug(WTFLog.DataAccess, this.TraceContext, string.Format("Using default value of {0}", defaultValue), null, "GetOptionalXmlAttribute", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\WorkDefinition.cs", 544);
				result = defaultValue;
			}
			else
			{
				result = (ReturnType)((object)Convert.ChangeType(xmlAttribute.Value, typeof(ReturnType)));
			}
			return result;
		}

		internal ReturnType GetMandatoryXmlAttribute<ReturnType>(XmlNode definition, string attributeName)
		{
			ReturnType returnType = default(ReturnType);
			XmlAttribute xmlAttribute = this.GetXmlAttribute(definition, attributeName, true);
			return (ReturnType)((object)Convert.ChangeType(xmlAttribute.Value, typeof(ReturnType)));
		}

		internal XmlAttribute GetXmlAttribute(XmlNode definition, string attributeName, bool throwOnFailure)
		{
			WTFDiagnostics.TraceDebug<string>(WTFLog.DataAccess, this.TraceContext, "[WorkDefinition.TryGetXmlAttribute]: Attempting to find attribute named {0}.", attributeName, null, "GetXmlAttribute", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\WorkDefinition.cs", 583);
			XmlAttribute xmlAttribute = definition.Attributes[attributeName];
			if (xmlAttribute == null)
			{
				string text = string.Format("Attribute {0} was not found in the WorkDefinition xml.", attributeName);
				XmlAttribute xmlAttribute2 = definition.Attributes["Name"];
				if (xmlAttribute2 != null)
				{
					text = string.Format("{0} WorkDefinition name was {1}", text, xmlAttribute2.Value);
				}
				WTFDiagnostics.TraceDebug(WTFLog.DataAccess, this.TraceContext, text, null, "GetXmlAttribute", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\WorkDefinition.cs", 599);
				if (throwOnFailure)
				{
					throw new XmlException(text);
				}
			}
			return xmlAttribute;
		}

		internal ReturnType GetMandatoryXmlEnumAttribute<ReturnType>(XmlNode definition, string attributeName)
		{
			XmlAttribute xmlAttribute = this.GetXmlAttribute(definition, attributeName, true);
			return (ReturnType)((object)Enum.Parse(typeof(ReturnType), xmlAttribute.Value));
		}

		internal ReturnType GetOptionalXmlEnumAttribute<ReturnType>(XmlNode definition, string attributeName, ReturnType defaultValue)
		{
			XmlAttribute xmlAttribute = this.GetXmlAttribute(definition, attributeName, false);
			ReturnType result = defaultValue;
			if (xmlAttribute != null && !string.IsNullOrEmpty(xmlAttribute.Value) && Enum.IsDefined(typeof(ReturnType), xmlAttribute.Value))
			{
				result = (ReturnType)((object)Enum.Parse(typeof(ReturnType), xmlAttribute.Value));
			}
			return result;
		}

		internal abstract WorkItemResult CreateResult();

		internal void ParseExtensionAttributes(bool force = false)
		{
			if ((this.attributes == null || force) && !string.IsNullOrWhiteSpace(this.ExtensionAttributes))
			{
				XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
				xmlReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;
				xmlReaderSettings.CloseInput = true;
				xmlReaderSettings.IgnoreComments = true;
				xmlReaderSettings.IgnoreProcessingInstructions = true;
				xmlReaderSettings.IgnoreWhitespace = true;
				using (XmlReader xmlReader = XmlReader.Create(new StringReader(this.ExtensionAttributes), xmlReaderSettings))
				{
					xmlReader.Read();
					this.attributes = new Dictionary<string, string>(xmlReader.AttributeCount);
					for (int i = 0; i < xmlReader.AttributeCount; i++)
					{
						xmlReader.MoveToAttribute(i);
						this.attributes.Add(xmlReader.Name, xmlReader.Value);
					}
				}
			}
		}

		internal void SyncExtensionAttributes(bool force = false)
		{
			if ((string.IsNullOrWhiteSpace(this.ExtensionAttributes) || force) && this.attributes != null && this.attributes.Count != 0)
			{
				this.ExtensionAttributes = WorkDefinition.SerializeExtensionAttributes(this.attributes);
			}
		}

		internal virtual bool Validate(List<string> errors)
		{
			int count = errors.Count;
			if (string.IsNullOrWhiteSpace(this.AssemblyPath))
			{
				errors.Add("AssemblyPath cannot be null or empty. ");
			}
			if (string.IsNullOrWhiteSpace(this.TypeName))
			{
				errors.Add("TypeName cannot be null or empty. ");
			}
			if (string.IsNullOrWhiteSpace(this.Name))
			{
				errors.Add("Name cannot be null or empty. ");
			}
			if (this.RecurrenceIntervalSeconds < 0)
			{
				errors.Add("RecurrenceIntervalSeconds cannot be less than 0. ");
			}
			if (this.TimeoutSeconds <= 0)
			{
				errors.Add("TimeoutSeconds cannot be less than or equal to 0. ");
			}
			if (this.Name.IndexOfAny(this.specialCharacters) != -1)
			{
				errors.Add("Name contains illegal characters. ");
			}
			return count == errors.Count;
		}

		[Conditional("DEBUG")]
		private void VerifyImplementingType(Type implementingType)
		{
			if (implementingType.IsAbstract)
			{
				throw new ArgumentException("Implementing type cannot be abstract");
			}
			if (implementingType.GetConstructor(new Type[0]) == null)
			{
				throw new ArgumentException("Implementing type should have a default public constructor");
			}
		}

		private const string DefaultDefinitionKey = "DefaultDefinitions";

		private const string ResultTokenDelimiter = "/";

		private readonly char[] specialCharacters = new char[]
		{
			'~',
			'\\'
		};

		private Dictionary<string, string> attributes;

		private Type workItemType;

		private object objectData;

		private string internalStorageKey;
	}
}
