using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring
{
	internal abstract class DefinitionHelperBase
	{
		internal string AssemblyPath { get; set; }

		internal string TypeName { get; set; }

		internal string Name { get; set; }

		internal Component Component { get; set; }

		internal void SetComponentByName(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentNullException("name");
			}
			Component component = null;
			ExchangeComponent.WellKnownComponents.TryGetValue(name, out component);
			if (component == null)
			{
				string message = string.Format("Cannot find the component name {0} in the Exchange components table.", name);
				WTFDiagnostics.TraceError(ExTraceGlobals.GenericHelperTracer, this.TraceContext, message, null, "SetComponentByName", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\DefinitionHelperBase.cs", 70);
				throw new XmlException(message);
			}
			this.Component = component;
		}

		internal string WorkItemVersion { get; set; }

		internal string ComponentName { get; set; }

		internal string ServiceName { get; set; }

		internal int DeploymentId { get; set; }

		internal string ExecutionLocation { get; set; }

		internal DateTime CreatedTime { get; set; }

		internal bool Enabled { get; set; }

		internal string TargetPartition { get; set; }

		internal string TargetGroup { get; set; }

		internal string TargetResource { get; set; }

		internal string TargetExtension { get; set; }

		internal string TargetVersion { get; set; }

		internal int RecurrenceIntervalSeconds { get; set; }

		internal int TimeoutSeconds { get; set; }

		internal DateTime StartTime { get; set; }

		internal DateTime UpdateTime { get; set; }

		internal int MaxRetryAttempts { get; set; }

		internal string ExtensionAttributes { get; set; }

		internal Type WorkItemType { get; set; }

		internal XmlNode DefinitionNode { get; set; }

		internal DiscoveryContext DiscoveryContext { get; set; }

		internal TracingContext TraceContext { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(base.ToString());
			stringBuilder.AppendLine("AssemblyPath: " + this.AssemblyPath);
			stringBuilder.AppendLine("TypeName: " + this.TypeName);
			stringBuilder.AppendLine("Name: " + this.Name);
			stringBuilder.AppendLine("ComponentName: " + this.ComponentName);
			stringBuilder.AppendLine("MaxRetryAttempts: " + this.MaxRetryAttempts);
			stringBuilder.AppendLine("RecurrenceIntervalSeconds: " + this.RecurrenceIntervalSeconds);
			stringBuilder.AppendLine("TimeoutSeconds: " + this.TimeoutSeconds);
			stringBuilder.AppendLine("Enabled: " + this.Enabled);
			stringBuilder.AppendLine("ExtensionAttributes: " + this.ExtensionAttributes);
			stringBuilder.AppendLine("TargetPartition: " + this.TargetPartition);
			stringBuilder.AppendLine("TargetGroup: " + this.TargetGroup);
			stringBuilder.AppendLine("TargetResource: " + this.TargetResource);
			stringBuilder.AppendLine("TargetVersion: " + this.TargetVersion);
			stringBuilder.AppendLine("TargetExtension: " + this.TargetExtension);
			stringBuilder.AppendLine("WorkItemVersion: " + this.WorkItemVersion);
			stringBuilder.AppendLine("DeploymentId: " + this.DeploymentId);
			stringBuilder.AppendLine("ExecutionLocation: " + this.ExecutionLocation);
			return stringBuilder.ToString();
		}

		internal static ReturnType GetMandatoryXmlAttribute<ReturnType>(XmlNode definition, string attributeName, TracingContext traceContext)
		{
			ReturnType returnType = default(ReturnType);
			XmlAttribute xmlAttribute = DefinitionHelperBase.GetXmlAttribute(definition, attributeName, true, traceContext);
			return (ReturnType)((object)Convert.ChangeType(xmlAttribute.Value, typeof(ReturnType)));
		}

		internal static ReturnType GetOptionalXmlAttribute<ReturnType>(XmlNode definition, string attributeName, ReturnType defaultValue, TracingContext traceContext)
		{
			ReturnType result = default(ReturnType);
			XmlAttribute xmlAttribute = DefinitionHelperBase.GetXmlAttribute(definition, attributeName, false, traceContext);
			if (xmlAttribute == null)
			{
				result = defaultValue;
			}
			else
			{
				result = (ReturnType)((object)Convert.ChangeType(xmlAttribute.Value, typeof(ReturnType)));
			}
			return result;
		}

		internal static ReturnType GetMandatoryXmlEnumAttribute<ReturnType>(XmlNode definition, string attributeName, TracingContext traceContext)
		{
			XmlAttribute xmlAttribute = DefinitionHelperBase.GetXmlAttribute(definition, attributeName, true, traceContext);
			return (ReturnType)((object)Enum.Parse(typeof(ReturnType), xmlAttribute.Value));
		}

		internal static ReturnType GetOptionalXmlEnumAttribute<ReturnType>(XmlNode definition, string attributeName, ReturnType defaultValue, TracingContext traceContext)
		{
			XmlAttribute xmlAttribute = DefinitionHelperBase.GetXmlAttribute(definition, attributeName, false, traceContext);
			ReturnType result = defaultValue;
			if (xmlAttribute != null && !string.IsNullOrEmpty(xmlAttribute.Value) && Enum.IsDefined(typeof(ReturnType), xmlAttribute.Value))
			{
				result = (ReturnType)((object)Enum.Parse(typeof(ReturnType), xmlAttribute.Value));
			}
			return result;
		}

		internal static XmlAttribute GetXmlAttribute(XmlNode definition, string attributeName, bool throwOnFailure, TracingContext traceContext)
		{
			XmlAttribute xmlAttribute = definition.Attributes[attributeName];
			if (xmlAttribute == null)
			{
				string text = string.Format("Attribute {0} was not found in the WorkDefinition xml.", attributeName);
				XmlAttribute xmlAttribute2 = definition.Attributes["Name"];
				if (xmlAttribute2 != null)
				{
					text = string.Format("{0} WorkDefinition name was {1}", text, xmlAttribute2.Value);
				}
				WTFDiagnostics.TraceDebug(ExTraceGlobals.GenericHelperTracer, traceContext, text, null, "GetXmlAttribute", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\DefinitionHelperBase.cs", 345);
				if (throwOnFailure)
				{
					throw new XmlException(text);
				}
			}
			return xmlAttribute;
		}

		internal static IEnumerable<XmlNode> GetDescendants(XmlNode node, string descendantName)
		{
			string filterString = string.Format("descendant::{0}", descendantName);
			using (XmlNodeList deploymentNodes = node.SelectNodes(filterString))
			{
				foreach (object obj in deploymentNodes)
				{
					XmlNode elementXml = (XmlNode)obj;
					yield return elementXml;
				}
			}
			yield break;
		}

		internal static IEnumerable<XmlNode> GetDescendantsContainingFilter(XmlNode node, string filter)
		{
			string filterString = string.Format("descendant::*[contains(name(),'{0}')]", filter);
			using (XmlNodeList deploymentNodes = node.SelectNodes(filterString))
			{
				foreach (object obj in deploymentNodes)
				{
					XmlNode elementXml = (XmlNode)obj;
					yield return elementXml;
				}
			}
			yield break;
		}

		internal static Dictionary<string, string> ConvertExtensionAttributesToDictionary(string extensionAttributes)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
			if (!string.IsNullOrWhiteSpace(extensionAttributes))
			{
				XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
				xmlReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;
				xmlReaderSettings.CloseInput = true;
				xmlReaderSettings.IgnoreComments = true;
				xmlReaderSettings.IgnoreProcessingInstructions = true;
				xmlReaderSettings.IgnoreWhitespace = true;
				using (XmlReader xmlReader = XmlReader.Create(new StringReader(extensionAttributes), xmlReaderSettings))
				{
					xmlReader.Read();
					dictionary = new Dictionary<string, string>(xmlReader.AttributeCount);
					for (int i = 0; i < xmlReader.AttributeCount; i++)
					{
						xmlReader.MoveToAttribute(i);
						dictionary.Add(xmlReader.Name, xmlReader.Value);
					}
				}
			}
			return dictionary;
		}

		internal virtual void ReadDiscoveryXml()
		{
			this.AssemblyPath = this.WorkItemType.Assembly.Location;
			this.TypeName = this.WorkItemType.FullName;
			this.Name = this.GetMandatoryXmlAttribute<string>("Name");
			this.ComponentName = this.GetMandatoryXmlAttribute<string>("ComponentName");
			this.ServiceName = this.ComponentName;
			this.SetComponentByName(this.ComponentName);
			this.MaxRetryAttempts = this.GetOptionalXmlAttribute<int>("MaxRetryAttempts", 0);
			this.RecurrenceIntervalSeconds = this.GetOptionalXmlAttribute<int>("RecurrenceIntervalSeconds", 0);
			this.TimeoutSeconds = this.GetOptionalXmlAttribute<int>("TimeoutSeconds", 30);
			this.Enabled = this.GetOptionalXmlAttribute<bool>("Enabled", true);
			XmlNode xmlNode = this.DefinitionNode.SelectSingleNode("ExtensionAttributes");
			this.attributes = null;
			this.ExtensionAttributes = null;
			if (xmlNode != null)
			{
				this.ExtensionAttributes = xmlNode.OuterXml;
				this.ParseExtensionAttributes();
			}
			this.TargetPartition = this.GetOptionalXmlAttribute<string>("TargetPartition", null);
			this.TargetGroup = this.GetOptionalXmlAttribute<string>("TargetGroup", null);
			this.TargetResource = this.GetOptionalXmlAttribute<string>("TargetResource", null);
			this.TargetVersion = this.GetOptionalXmlAttribute<string>("TargetVersion", null);
			this.TargetExtension = this.GetOptionalXmlAttribute<string>("TargetExtension", null);
			this.WorkItemVersion = this.GetOptionalXmlAttribute<string>("WorkItemVersion", "Binaries");
			this.DeploymentId = this.GetOptionalXmlAttribute<int>("DeploymentId", 0);
			this.ExecutionLocation = this.GetOptionalXmlAttribute<string>("ExecutionLocation", string.Empty);
			this.CreatedTime = this.GetOptionalXmlAttribute<DateTime>("CreatedTime", DateTime.UtcNow);
			this.StartTime = this.GetOptionalXmlAttribute<DateTime>("StartTime", DateTime.UtcNow);
			this.UpdateTime = this.GetOptionalXmlAttribute<DateTime>("UpdateTime", DateTime.UtcNow);
		}

		internal virtual string ToString(WorkDefinition workItem)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (workItem != null)
			{
				stringBuilder.AppendLine("AssemblyPath: " + workItem.AssemblyPath);
				stringBuilder.AppendLine("TypeName: " + workItem.TypeName);
				stringBuilder.AppendLine("Name: " + workItem.Name);
				stringBuilder.AppendLine("ServiceName: " + workItem.ServiceName);
				stringBuilder.AppendLine("MaxRetryAttempts: " + workItem.MaxRetryAttempts);
				stringBuilder.AppendLine("RecurrenceIntervalSeconds: " + workItem.RecurrenceIntervalSeconds);
				stringBuilder.AppendLine("TimeoutSeconds: " + workItem.TimeoutSeconds);
				stringBuilder.AppendLine("Enabled: " + workItem.Enabled);
				stringBuilder.AppendLine("ExtensionAttributes: " + workItem.ExtensionAttributes);
				stringBuilder.AppendLine("TargetPartition: " + workItem.TargetPartition);
				stringBuilder.AppendLine("TargetGroup: " + workItem.TargetGroup);
				stringBuilder.AppendLine("TargetResource: " + workItem.TargetResource);
				stringBuilder.AppendLine("TargetVersion: " + workItem.TargetVersion);
				stringBuilder.AppendLine("TargetExtension: " + workItem.TargetExtension);
				stringBuilder.AppendLine("WorkItemVersion: " + workItem.WorkItemVersion);
				stringBuilder.AppendLine("DeploymentId: " + workItem.DeploymentId);
				stringBuilder.AppendLine("ExecutionLocation: " + workItem.ExecutionLocation);
			}
			return stringBuilder.ToString();
		}

		internal void ParseExtensionAttributes()
		{
			if (!string.IsNullOrWhiteSpace(this.ExtensionAttributes))
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

		internal void LogDefinitions(WorkDefinition workItem)
		{
			DefinitionHelperBase definitionHelperBase;
			if (this is ProbeDefinitionHelper)
			{
				definitionHelperBase = (ProbeDefinitionHelper)this;
			}
			else if (this is MonitorDefinitionHelper)
			{
				definitionHelperBase = (MonitorDefinitionHelper)this;
			}
			else
			{
				definitionHelperBase = (ResponderDefinitionHelper)this;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("The following are the settings defined in the XML:");
			stringBuilder.AppendLine(definitionHelperBase.ToString());
			stringBuilder.AppendLine("The following are the actual settings of the work item created:");
			stringBuilder.AppendLine(definitionHelperBase.ToString(workItem));
			WTFDiagnostics.TraceInformation(ExTraceGlobals.GenericHelperTracer, this.TraceContext, stringBuilder.ToString(), null, "LogDefinitions", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\DefinitionHelperBase.cs", 578);
		}

		protected ReturnType GetMandatoryXmlAttribute<ReturnType>(string attributeName)
		{
			return DefinitionHelperBase.GetMandatoryXmlAttribute<ReturnType>(this.DefinitionNode, attributeName, this.TraceContext);
		}

		protected ReturnType GetOptionalXmlAttribute<ReturnType>(string attributeName, ReturnType defaultValue)
		{
			return DefinitionHelperBase.GetOptionalXmlAttribute<ReturnType>(this.DefinitionNode, attributeName, defaultValue, this.TraceContext);
		}

		protected ReturnType GetOptionalXmlEnumAttribute<ReturnType>(string attributeName, ReturnType defaultValue)
		{
			return DefinitionHelperBase.GetOptionalXmlEnumAttribute<ReturnType>(this.DefinitionNode, attributeName, defaultValue, this.TraceContext);
		}

		protected ReturnType GetMandatoryValue<ReturnType>(string key)
		{
			string dictionaryValue = this.GetDictionaryValue(key, true);
			return (ReturnType)((object)Convert.ChangeType(dictionaryValue, typeof(ReturnType)));
		}

		protected ReturnType GetOptionalValue<ReturnType>(string key, ReturnType defaultValue)
		{
			ReturnType result = default(ReturnType);
			string dictionaryValue = this.GetDictionaryValue(key, false);
			if (string.IsNullOrWhiteSpace(dictionaryValue))
			{
				WTFDiagnostics.TraceDebug(ExTraceGlobals.GenericHelperTracer, this.TraceContext, string.Format("Using default value of {0}", defaultValue), null, "GetOptionalValue", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\DefinitionHelperBase.cs", 645);
				result = defaultValue;
			}
			else
			{
				result = (ReturnType)((object)Convert.ChangeType(dictionaryValue, typeof(ReturnType)));
			}
			return result;
		}

		protected ReturnType GetMandatoryEnumValue<ReturnType>(string key)
		{
			string dictionaryValue = this.GetDictionaryValue(key, true);
			return (ReturnType)((object)Enum.Parse(typeof(ReturnType), dictionaryValue));
		}

		protected ReturnType GetOptionalEnumValue<ReturnType>(string key, ReturnType defaultValue)
		{
			ReturnType result = default(ReturnType);
			string dictionaryValue = this.GetDictionaryValue(key, false);
			if (string.IsNullOrWhiteSpace(dictionaryValue))
			{
				WTFDiagnostics.TraceDebug(ExTraceGlobals.GenericHelperTracer, this.TraceContext, string.Format("Using default value of {0}", defaultValue), null, "GetOptionalEnumValue", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\DefinitionHelperBase.cs", 687);
				result = defaultValue;
			}
			else
			{
				result = (ReturnType)((object)Enum.Parse(typeof(ReturnType), dictionaryValue));
			}
			return result;
		}

		protected string GetDictionaryValue(string key, bool mandatory)
		{
			if (this.attributes == null)
			{
				if (mandatory)
				{
					string message = string.Format("The ExtensionAttributes node cannot be missing or empty.", new object[0]);
					WTFDiagnostics.TraceError(ExTraceGlobals.GenericHelperTracer, this.TraceContext, message, null, "GetDictionaryValue", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\DefinitionHelperBase.cs", 711);
					throw new XmlException(message);
				}
				return null;
			}
			else if (!this.attributes.ContainsKey(key))
			{
				if (mandatory)
				{
					string message2 = string.Format("The ExtensionAttributes does not contain key '{0}'.", key);
					WTFDiagnostics.TraceError(ExTraceGlobals.GenericHelperTracer, this.TraceContext, message2, null, "GetDictionaryValue", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\DefinitionHelperBase.cs", 725);
					throw new XmlException(message2);
				}
				return null;
			}
			else
			{
				string text = this.attributes[key];
				if (string.IsNullOrWhiteSpace(text) && mandatory)
				{
					string message3 = string.Format("The value of key '{0}' in the ExtensionAttributes cannot be null.", key);
					WTFDiagnostics.TraceError(ExTraceGlobals.GenericHelperTracer, this.TraceContext, message3, null, "GetDictionaryValue", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\DefinitionHelperBase.cs", 739);
					throw new XmlException(message3);
				}
				return text;
			}
		}

		protected void CreateBaseWorkDefinition(WorkDefinition workitem)
		{
			workitem.AssemblyPath = this.AssemblyPath;
			workitem.TypeName = this.TypeName;
			workitem.Name = this.Name;
			workitem.ServiceName = this.ServiceName;
			workitem.RecurrenceIntervalSeconds = this.RecurrenceIntervalSeconds;
			workitem.TimeoutSeconds = this.TimeoutSeconds;
			workitem.MaxRetryAttempts = this.MaxRetryAttempts;
			workitem.Enabled = this.Enabled;
			workitem.TargetResource = this.TargetResource;
			workitem.TargetGroup = this.TargetGroup;
			workitem.TargetPartition = this.TargetPartition;
			workitem.TargetExtension = this.TargetExtension;
			workitem.TargetVersion = this.TargetVersion;
			workitem.ExtensionAttributes = this.ExtensionAttributes;
			workitem.ParseExtensionAttributes(false);
		}

		private Dictionary<string, string> attributes;
	}
}
