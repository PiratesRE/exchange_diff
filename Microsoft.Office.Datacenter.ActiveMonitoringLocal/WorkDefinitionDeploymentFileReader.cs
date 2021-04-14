using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public class WorkDefinitionDeploymentFileReader
	{
		public WorkDefinitionDeploymentFileReader(string deploymentFileFolderLocation, TracingContext traceContext)
		{
			this.traceContext = traceContext;
			this.deploymentFileFolderLocation = deploymentFileFolderLocation;
			if (!Path.IsPathRooted(this.deploymentFileFolderLocation))
			{
				using (Process currentProcess = Process.GetCurrentProcess())
				{
					string fileName = currentProcess.MainModule.FileName;
					string directoryName = Path.GetDirectoryName(fileName);
					this.deploymentFileFolderLocation = Path.Combine(directoryName, this.deploymentFileFolderLocation);
				}
			}
			if (!Directory.Exists(this.deploymentFileFolderLocation))
			{
				string message = string.Format("The Deployment file folder {0} does not exist.", this.deploymentFileFolderLocation);
				WTFDiagnostics.TraceError(WTFLog.DataAccess, this.traceContext, message, null, ".ctor", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkDefinitionDeploymentFileReader.cs", 83);
				throw new ArgumentException(message);
			}
			this.traceContext = traceContext;
		}

		public IEnumerable<WorkDefinition> GetWorkDefinitions(List<string> fileList = null)
		{
			string nameSpace = base.GetType().Namespace;
			foreach (XmlNode workDefinitionXml in this.GetDeploymentElementsContainingFilter("Definition", fileList))
			{
				string fullTypeName = string.Format("{0}.{1}", nameSpace, workDefinitionXml.Name);
				XmlAttribute workDefinitionNameAttribute = workDefinitionXml.Attributes["Name"];
				string workDefinitionName = string.Format("{0}. Unable to read the Xml element's Name attribute to provide more information.", workDefinitionXml.Name);
				if (workDefinitionNameAttribute != null)
				{
					workDefinitionName = workDefinitionNameAttribute.Value;
				}
				WorkDefinition workDefinition = null;
				try
				{
					WTFDiagnostics.TraceInformation<string>(WTFLog.DataAccess, this.traceContext, "[WorkItemDeploymentFileReader.GetWorkDefinitions]: Attempting to populate Work Definition {0}", workDefinitionName, null, "GetWorkDefinitions", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkDefinitionDeploymentFileReader.cs", 124);
					Type type = Type.GetType(fullTypeName);
					if (type == null)
					{
						string message = string.Format("Error loading WorkDefinition type {0}. The type was not found.", fullTypeName);
						WTFDiagnostics.TraceError(WTFLog.DataAccess, this.traceContext, message, null, "GetWorkDefinitions", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkDefinitionDeploymentFileReader.cs", 131);
						throw new TypeLoadException(message);
					}
					workDefinition = (WorkDefinition)Activator.CreateInstance(type);
					workDefinition.FromXml(workDefinitionXml);
					WTFDiagnostics.TraceInformation<string>(WTFLog.DataAccess, this.traceContext, "[WorkItemDeploymentFileReader.GetWorkDefinitions]: Successfully populated Work Definition {0}", workDefinitionName, null, "GetWorkDefinitions", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkDefinitionDeploymentFileReader.cs", 139);
				}
				catch (Exception ex)
				{
					string message2 = string.Format("[WorkItemDeploymentFileReader.GetWorkDefinitions]: Failed to populate WorkDefinition {0}. Skipping.\n{1}", workDefinitionName, ex);
					WTFDiagnostics.TraceError(WTFLog.DataAccess, this.traceContext, message2, null, "GetWorkDefinitions", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkDefinitionDeploymentFileReader.cs", 144);
					this.exceptionList.Add(ex);
				}
				yield return workDefinition;
			}
			yield break;
		}

		public IEnumerable<WorkDefinitionDeploymentFileReader.PerformanceCounter> GetPerformanceCounterFilters()
		{
			foreach (XmlNode perfCounterFilterXml in this.GetDeploymentElementsContainingFilter("PerformanceCounters", null))
			{
				foreach (object obj in perfCounterFilterXml.ChildNodes)
				{
					XmlNode perfCounterFilter = (XmlNode)obj;
					XmlAttribute objectName = perfCounterFilter.Attributes["Object"];
					XmlAttribute counterName = perfCounterFilter.Attributes["Counter"];
					XmlAttribute instanceName = perfCounterFilter.Attributes["Instance"];
					if (objectName == null || counterName == null || instanceName == null || string.IsNullOrWhiteSpace(objectName.Value.ToString()) || string.IsNullOrWhiteSpace(counterName.Value.ToString()) || string.IsNullOrWhiteSpace(instanceName.Value.ToString()))
					{
						string message = "[WorkItemDeploymentFileReader.GetPerformanceCounterFilters]: Failed to read Counter element from file. Either Object, Counter or Instance attribute was missing or empty.";
						WTFDiagnostics.TraceError(WTFLog.DataAccess, this.traceContext, message, null, "GetPerformanceCounterFilters", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkDefinitionDeploymentFileReader.cs", 174);
						throw new XmlException(message);
					}
					yield return new WorkDefinitionDeploymentFileReader.PerformanceCounter
					{
						ObjectName = objectName.Value.ToString(),
						CounterName = counterName.Value.ToString(),
						InstanceName = instanceName.Value.ToString()
					};
				}
			}
			yield break;
		}

		public XmlNode GetMappingsforInstance(string mappingFileName, string nodeName)
		{
			string filter = string.Format("Instance[@Name='{0}']", Settings.InstanceName);
			XmlNode xmlNode = this.GetDeploymentElementsContainingFilter(mappingFileName, "/Mapping", filter, null).FirstOrDefault<XmlNode>();
			if (xmlNode == null)
			{
				string message = string.Format("[WorkItemDeploymentFileReader.GetMappingsforInstance]: Failed to read Instance node for instance {0}.", Settings.InstanceName);
				WTFDiagnostics.TraceError(WTFLog.DataAccess, this.traceContext, message, null, "GetMappingsforInstance", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkDefinitionDeploymentFileReader.cs", 205);
				throw new XmlException(message);
			}
			XmlNode xmlNode2 = xmlNode.SelectSingleNode(nodeName);
			if (xmlNode2 == null)
			{
				string message2 = string.Format("[WorkItemDeploymentFileReader.GetMappingsforInstance]: Failed to read {0} node for instance {1}.", nodeName, Settings.InstanceName);
				WTFDiagnostics.TraceError(WTFLog.DataAccess, this.traceContext, message2, null, "GetMappingsforInstance", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkDefinitionDeploymentFileReader.cs", 215);
				throw new XmlException(message2);
			}
			return xmlNode2;
		}

		public IEnumerable<string> GetTopologyScopeObjectsMappings(string topologyMappingFileName, out int aggregationLevel)
		{
			XmlNode mappingsforInstance = this.GetMappingsforInstance(topologyMappingFileName, "TopologyScopes");
			XmlAttribute xmlAttribute = mappingsforInstance.Attributes["AggregationLevel"];
			if (xmlAttribute == null || string.IsNullOrWhiteSpace(xmlAttribute.Value.ToString()))
			{
				string message = string.Format("[WorkItemDeploymentFileReader.GetInstanceMappings]: Failed to read AggregationLevel attribute from the TopologyScopes node for instance {0}.", Settings.InstanceName);
				WTFDiagnostics.TraceError(WTFLog.DataAccess, this.traceContext, message, null, "GetTopologyScopeObjectsMappings", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkDefinitionDeploymentFileReader.cs", 238);
				throw new XmlException(message);
			}
			aggregationLevel = Convert.ToInt32(xmlAttribute.Value);
			return this.EnumerateTopologyScopeObjects(mappingsforInstance.ChildNodes);
		}

		public List<string> GetWorkItemFileMappings(string workItemMappingFileName)
		{
			List<string> list = new List<string>();
			XmlNode mappingsforInstance = this.GetMappingsforInstance(workItemMappingFileName, "WorkItemFiles");
			foreach (object obj in mappingsforInstance.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				XmlAttribute xmlAttribute = xmlNode.Attributes["Name"];
				if (xmlAttribute == null || string.IsNullOrWhiteSpace(xmlAttribute.Value.ToString()))
				{
					string message = "[WorkItemDeploymentFileReader.GetWorkItemFileMappings]: Failed to read work item element from file. The Name attribute was missing or empty.";
					WTFDiagnostics.TraceError(WTFLog.DataAccess, this.traceContext, message, null, "GetWorkItemFileMappings", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkDefinitionDeploymentFileReader.cs", 267);
					throw new XmlException(message);
				}
				list.Add(xmlAttribute.Value.ToString());
			}
			return list;
		}

		public IEnumerable<WorkDefinitionDeploymentFileReader.WorkItemManifest> GetWorkItemManifest()
		{
			List<WorkDefinitionDeploymentFileReader.WorkItemManifest> list = new List<WorkDefinitionDeploymentFileReader.WorkItemManifest>();
			foreach (XmlNode xmlNode in this.GetDeploymentElementsContainingFilter("*.xml", "/", "Definition", null))
			{
				if (this.XmlNodeHasAttribute(xmlNode, "AggregationLevel"))
				{
					string fileName = Path.GetFileName(xmlNode.BaseURI);
					WorkDefinitionDeploymentFileReader.WorkItemManifest workItemManifest = new WorkDefinitionDeploymentFileReader.WorkItemManifest(fileName);
					workItemManifest.AggregationLevel = Convert.ToInt32(this.ReadXmlAttribute(xmlNode, "AggregationLevel"));
					workItemManifest.IsProductionReady = Convert.ToBoolean(this.ReadXmlAttribute(xmlNode, "IsProductionReady"));
					XmlNodeList xmlNodeList = xmlNode.SelectNodes("SupportedEnvironments/Environment");
					if (xmlNodeList.Count == 0)
					{
						string message = string.Format("[WorkItemDeploymentFileReader.GetWorkItemManifest]: Failed to read the Envrionment nodes from file {0}.", fileName);
						throw new XmlException(message);
					}
					foreach (object obj in xmlNodeList)
					{
						XmlNode node = (XmlNode)obj;
						string item = this.ReadXmlAttribute(node, "Name");
						workItemManifest.SupportedEnvironments.Add(item);
					}
					XmlNode xmlNode2 = xmlNode.SelectSingleNode("MaintenanceDefinition");
					if (xmlNode2 == null)
					{
						string message2 = string.Format("[WorkItemDeploymentFileReader.GetWorkItemManifest]: Failed to read MaintenanceDefinition nodes from file {0}.", fileName);
						throw new XmlException(message2);
					}
					workItemManifest.MaintenanceDefinitionName = this.ReadXmlAttribute(xmlNode2, "Name");
					XmlNodeList xmlNodeList2 = xmlNode.SelectNodes("Discovery/Probe");
					if (xmlNodeList2.Count == 0)
					{
						string message3 = string.Format("[WorkItemDeploymentFileReader.GetWorkItemManifest]: Failed to read Probe nodes from file {0}.", fileName);
						throw new XmlException(message3);
					}
					foreach (object obj2 in xmlNodeList2)
					{
						XmlNode xmlNode3 = (XmlNode)obj2;
						XmlNode xmlNode4 = xmlNode3.SelectSingleNode("ExpectedNumberOfSamples");
						if (xmlNode4 == null)
						{
							string message4 = string.Format("[WorkItemDeploymentFileReader.GetWorkItemManifest]: Failed to read the ExpectedNumberOfSamples nodes from file {0}.", fileName);
							throw new XmlException(message4);
						}
						int probesPerScope = Convert.ToInt32(this.ReadXmlAttribute(xmlNode4, "Max"));
						int recurrenceInterval = Convert.ToInt32(this.ReadXmlAttribute(xmlNode3, "RecurrenceIntervalSeconds"));
						string name = this.ReadXmlAttribute(xmlNode3, "Name");
						WorkDefinitionDeploymentFileReader.ProbeRuntimeProperties item2 = new WorkDefinitionDeploymentFileReader.ProbeRuntimeProperties(name, recurrenceInterval, probesPerScope);
						workItemManifest.RuntimeProperties.Add(item2);
					}
					list.Add(workItemManifest);
				}
			}
			return list;
		}

		internal IEnumerable<XmlNode> GetDeploymentElementsContainingFilter(string filter, List<string> fileList = null)
		{
			string filter2 = string.Format("child::*[contains(name(),'{0}')]", filter);
			return this.GetDeploymentElementsContainingFilter("*.xml", "/Definition", filter2, fileList);
		}

		internal IEnumerable<XmlNode> GetDeploymentElementsContainingFilter(string fileNameFilter, string rootName, string filter, List<string> fileList = null)
		{
			string[] deploymentFiles = Directory.GetFiles(this.deploymentFileFolderLocation, fileNameFilter);
			foreach (string deploymentFile in deploymentFiles)
			{
				if (fileList == null || fileList.Contains(Path.GetFileName(deploymentFile)))
				{
					WTFDiagnostics.TraceDebug<string>(WTFLog.DataAccess, this.traceContext, "[WorkItemDeploymentFileReader.GetDeploymentElementsContainingFilter]: Attempting to read deployment file {0}", deploymentFile, null, "GetDeploymentElementsContainingFilter", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkDefinitionDeploymentFileReader.cs", 403);
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.Load(deploymentFile);
					XmlElement root = xmlDocument.DocumentElement;
					if (root == null)
					{
						string message = string.Format("Error loading deployment element from file {0}. File is malformed and contains no root element.", deploymentFile);
						WTFDiagnostics.TraceError(WTFLog.DataAccess, this.traceContext, message, null, "GetDeploymentElementsContainingFilter", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkDefinitionDeploymentFileReader.cs", 414);
						throw new XmlException(message);
					}
					XmlNode definitionNode = root.SelectSingleNode(rootName);
					if (definitionNode != null)
					{
						XmlNodeList deploymentNodes = definitionNode.SelectNodes(filter);
						foreach (object obj in deploymentNodes)
						{
							XmlNode deploymentElementXml = (XmlNode)obj;
							yield return deploymentElementXml;
						}
					}
				}
			}
			yield break;
		}

		private string ReadXmlAttribute(XmlNode node, string attributeName)
		{
			XmlAttribute xmlAttribute = node.Attributes[attributeName];
			if (xmlAttribute == null || string.IsNullOrWhiteSpace(xmlAttribute.Value.ToString()))
			{
				string message = string.Format("[WorkItemDeploymentFileReader.ReadXmlAttribute]: Failed to read attribute {0} from {1}.", attributeName, node.BaseURI);
				WTFDiagnostics.TraceError(WTFLog.DataAccess, this.traceContext, message, null, "ReadXmlAttribute", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkDefinitionDeploymentFileReader.cs", 448);
				throw new XmlException(message);
			}
			return xmlAttribute.Value.ToString();
		}

		private bool XmlNodeHasAttribute(XmlNode node, string attributeName)
		{
			XmlAttribute xmlAttribute = node.Attributes[attributeName];
			return xmlAttribute != null && !string.IsNullOrWhiteSpace(xmlAttribute.Value.ToString());
		}

		private IEnumerable<string> EnumerateTopologyScopeObjects(XmlNodeList nodeList)
		{
			foreach (object obj in nodeList)
			{
				XmlNode topologyNode = (XmlNode)obj;
				XmlAttribute name = topologyNode.Attributes["Name"];
				if (name == null || string.IsNullOrWhiteSpace(name.Value.ToString()))
				{
					string message = "[WorkItemDeploymentFileReader.EnumerateTopologyScopeObjects]: Failed to read topology element from file. The Name attribute was missing or empty.";
					WTFDiagnostics.TraceError(WTFLog.DataAccess, this.traceContext, message, null, "EnumerateTopologyScopeObjects", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkDefinitionDeploymentFileReader.cs", 489);
					throw new XmlException(message);
				}
				yield return name.Value.ToString();
			}
			yield break;
		}

		private const string TopologyScopesNode = "TopologyScopes";

		private const string WorkItemFilesNode = "WorkItemFiles";

		private readonly string deploymentFileFolderLocation = string.Empty;

		private readonly TracingContext traceContext = TracingContext.Default;

		internal List<Exception> exceptionList = new List<Exception>();

		public struct PerformanceCounter
		{
			public string ObjectName;

			public string CounterName;

			public string InstanceName;
		}

		public class WorkItemManifest
		{
			public WorkItemManifest(string fileName)
			{
				this.MaintenanceDefinitionName = string.Empty;
				this.FileName = fileName;
				this.SupportedEnvironments = new List<string>();
				this.RuntimeProperties = new List<WorkDefinitionDeploymentFileReader.ProbeRuntimeProperties>();
				this.AggregationLevel = -1;
				this.IsProductionReady = false;
			}

			public double CalculateTotalCost()
			{
				double num = 0.0;
				foreach (WorkDefinitionDeploymentFileReader.ProbeRuntimeProperties probeRuntimeProperties in this.RuntimeProperties)
				{
					num += (double)probeRuntimeProperties.ProbesPerScope / (double)probeRuntimeProperties.RecurrenceIntervalSeconds * (double)probeRuntimeProperties.ProbeSize;
				}
				return num;
			}

			public string MaintenanceDefinitionName;

			public string FileName;

			public int AggregationLevel;

			public bool IsProductionReady;

			public List<string> SupportedEnvironments;

			public List<WorkDefinitionDeploymentFileReader.ProbeRuntimeProperties> RuntimeProperties;
		}

		public struct ProbeRuntimeProperties
		{
			public ProbeRuntimeProperties(string name, int recurrenceInterval, int probesPerScope)
			{
				this.Name = name;
				this.RecurrenceIntervalSeconds = recurrenceInterval;
				this.ProbesPerScope = probesPerScope;
				this.ProbeSize = 1;
			}

			public string Name;

			public int RecurrenceIntervalSeconds;

			public int ProbesPerScope;

			public int ProbeSize;
		}
	}
}
