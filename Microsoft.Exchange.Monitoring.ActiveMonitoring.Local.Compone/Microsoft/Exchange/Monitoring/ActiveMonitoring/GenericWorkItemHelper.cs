using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring
{
	public class GenericWorkItemHelper
	{
		public static XmlNode GetDefinitionNode(string deploymentFile, TracingContext traceContext)
		{
			if (string.IsNullOrWhiteSpace(deploymentFile))
			{
				WTFDiagnostics.TraceError(ExTraceGlobals.GenericHelperTracer, traceContext, "Empty argument 'deploymentFile'", null, "GetDefinitionNode", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\GenericWorkItemHelper.cs", 51);
				throw new ArgumentNullException("deploymentFile");
			}
			string text = Settings.FileStorageLocation;
			if (!Path.IsPathRooted(text))
			{
				using (Process currentProcess = Process.GetCurrentProcess())
				{
					string fileName = currentProcess.MainModule.FileName;
					string directoryName = Path.GetDirectoryName(fileName);
					text = Path.Combine(directoryName, text);
				}
			}
			string text2 = Path.Combine(text, deploymentFile);
			if (!File.Exists(text2))
			{
				string message = string.Format("The Deployment file {0} does not exist.", text2);
				WTFDiagnostics.TraceError(ExTraceGlobals.GenericHelperTracer, traceContext, message, null, "GetDefinitionNode", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\GenericWorkItemHelper.cs", 73);
				throw new FileNotFoundException(message);
			}
			WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.GenericHelperTracer, traceContext, "Attempting to read deployment file {0}", text2, null, "GetDefinitionNode", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\GenericWorkItemHelper.cs", 77);
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			safeXmlDocument.Load(text2);
			XmlElement documentElement = safeXmlDocument.DocumentElement;
			if (documentElement == null)
			{
				string message2 = string.Format("Error loading deployment file {0}. File is malformed.", text2);
				WTFDiagnostics.TraceError(ExTraceGlobals.GenericHelperTracer, traceContext, message2, null, "GetDefinitionNode", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\GenericWorkItemHelper.cs", 89);
				throw new XmlException(message2);
			}
			XmlNode xmlNode = documentElement.SelectSingleNode("/Definition");
			if (xmlNode == null)
			{
				string message3 = string.Format("Error finding root element 'Definition' from file {0}.", text2);
				WTFDiagnostics.TraceError(ExTraceGlobals.GenericHelperTracer, traceContext, message3, null, "GetDefinitionNode", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\GenericWorkItemHelper.cs", 98);
				throw new XmlException(message3);
			}
			return xmlNode;
		}

		public static void CreatePerfCounterDefinitions(XmlNode definitionNode, IMaintenanceWorkBroker broker, TracingContext traceContext)
		{
			GenericWorkItemHelper.CreatePerfCounterDefinitions(definitionNode, broker, traceContext, null);
		}

		public static void CreatePerfCounterDefinitions(XmlNode definitionNode, IMaintenanceWorkBroker broker, TracingContext traceContext, MaintenanceResult result)
		{
			GenericWorkItemHelper.CheckArgs<XmlNode>(definitionNode, "definitionNode", traceContext);
			GenericWorkItemHelper.CheckArgs<IMaintenanceWorkBroker>(broker, "broker", traceContext);
			foreach (XmlNode xmlNode in DefinitionHelperBase.GetDescendants(definitionNode, "PerformanceCounters"))
			{
				foreach (object obj in xmlNode.SelectNodes("Counter"))
				{
					XmlNode xmlNode2 = (XmlNode)obj;
					try
					{
						lock (GenericWorkItemHelper.discoveryLock)
						{
							new PerfCounter(xmlNode2, traceContext, result).ProcessDefinitions(broker);
						}
					}
					catch (Exception ex)
					{
						string text = string.Format("Failed to discover workitem {0}.", xmlNode2.Name);
						GenericWorkItemHelper.WriteEntry(traceContext, "{0} {1}", new object[]
						{
							text,
							ex.Message
						});
						WTFDiagnostics.TraceError<string, string>(ExTraceGlobals.GenericHelperTracer, traceContext, "{0} {1}", text, ex.ToString(), null, "CreatePerfCounterDefinitions", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\GenericWorkItemHelper.cs", 145);
					}
				}
			}
		}

		public static void CreateNTEventDefinitions(XmlNode definitionNode, IMaintenanceWorkBroker broker, TracingContext traceContext)
		{
			GenericWorkItemHelper.CreateNTEventDefinitions(definitionNode, broker, traceContext, null);
		}

		public static void CreateNTEventDefinitions(XmlNode definitionNode, IMaintenanceWorkBroker broker, TracingContext traceContext, MaintenanceResult result)
		{
			GenericWorkItemHelper.CheckArgs<XmlNode>(definitionNode, "definitionNode", traceContext);
			GenericWorkItemHelper.CheckArgs<IMaintenanceWorkBroker>(broker, "broker", traceContext);
			foreach (XmlNode xmlNode in DefinitionHelperBase.GetDescendants(definitionNode, "NTEvent"))
			{
				try
				{
					lock (GenericWorkItemHelper.discoveryLock)
					{
						new NTEvent(xmlNode, traceContext, result).ProcessDefinitions(broker);
					}
				}
				catch (Exception ex)
				{
					string text = string.Format("Failed to discover workitem {0}.", xmlNode.Name);
					GenericWorkItemHelper.WriteEntry(traceContext, "{0} {1}", new object[]
					{
						text,
						ex.Message
					});
					WTFDiagnostics.TraceError<string, string>(ExTraceGlobals.GenericHelperTracer, traceContext, "{0} {1}", text, ex.ToString(), null, "CreateNTEventDefinitions", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\GenericWorkItemHelper.cs", 189);
				}
			}
		}

		public static void CreateCustomDefinitions(XmlNode definitionNode, IMaintenanceWorkBroker broker, TracingContext traceContext)
		{
			GenericWorkItemHelper.CreateCustomDefinitions(definitionNode, broker, traceContext, null);
		}

		public static void CreateCustomDefinitions(XmlNode definitionNode, IMaintenanceWorkBroker broker, TracingContext traceContext, MaintenanceResult result)
		{
			GenericWorkItemHelper.CheckArgs<XmlNode>(definitionNode, "definitionNode", traceContext);
			GenericWorkItemHelper.CheckArgs<IMaintenanceWorkBroker>(broker, "broker", traceContext);
			foreach (XmlNode xmlNode in DefinitionHelperBase.GetDescendants(definitionNode, "CustomWorkItem"))
			{
				try
				{
					lock (GenericWorkItemHelper.discoveryLock)
					{
						new CustomWorkItem(xmlNode, traceContext, result).ProcessDefinitions(broker);
					}
				}
				catch (Exception ex)
				{
					string text = string.Format("Failed to discover workitem {0}.", xmlNode.Name);
					GenericWorkItemHelper.WriteEntry(traceContext, "{0} {1}", new object[]
					{
						text,
						ex.Message
					});
					WTFDiagnostics.TraceError<string, string>(ExTraceGlobals.GenericHelperTracer, traceContext, "{0} {1}", text, ex.ToString(), null, "CreateCustomDefinitions", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\GenericWorkItemHelper.cs", 232);
				}
			}
		}

		public static void CreateAllDefinitions(IEnumerable<string> deploymentFiles, IMaintenanceWorkBroker broker, TracingContext traceContext)
		{
			GenericWorkItemHelper.CreateAllDefinitions(deploymentFiles, broker, traceContext, null);
		}

		public static void CreateAllDefinitions(IEnumerable<string> deploymentFiles, IMaintenanceWorkBroker broker, TracingContext traceContext, MaintenanceResult result)
		{
			GenericWorkItemHelper.CheckArgs<IEnumerable<string>>(deploymentFiles, "deploymentFiles", traceContext);
			GenericWorkItemHelper.CheckArgs<IMaintenanceWorkBroker>(broker, "broker", traceContext);
			StringBuilder stringBuilder = new StringBuilder();
			try
			{
				foreach (string text in deploymentFiles)
				{
					try
					{
						XmlNode definitionNode = GenericWorkItemHelper.GetDefinitionNode(text, traceContext);
						GenericWorkItemHelper.CreatePerfCounterDefinitions(definitionNode, broker, traceContext, result);
						GenericWorkItemHelper.CreateNTEventDefinitions(definitionNode, broker, traceContext, result);
						GenericWorkItemHelper.CreateCustomDefinitions(definitionNode, broker, traceContext, result);
					}
					catch (Exception ex)
					{
						GenericWorkItemHelper.WriteEntry(traceContext, ex.Message, new object[0]);
						WTFDiagnostics.TraceError(ExTraceGlobals.GenericHelperTracer, traceContext, ex.ToString(), null, "CreateAllDefinitions", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\GenericWorkItemHelper.cs", 284);
					}
					finally
					{
						string entry = GenericWorkItemHelper.GetEntry(traceContext);
						if (!string.IsNullOrWhiteSpace(entry))
						{
							stringBuilder.AppendLine(string.Format("<{0}> {1}", text, entry));
							GenericWorkItemHelper.RemoveEntry(traceContext);
						}
					}
				}
				GenericWorkItemHelper.WriteEntry(traceContext, stringBuilder.ToString(), new object[0]);
				GenericWorkItemHelper.CompleteDiscovery(traceContext);
			}
			catch (Exception ex2)
			{
				GenericWorkItemHelper.WriteEntry(traceContext, ex2.Message, new object[0]);
				WTFDiagnostics.TraceError(ExTraceGlobals.GenericHelperTracer, traceContext, ex2.ToString(), null, "CreateAllDefinitions", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\GenericWorkItemHelper.cs", 309);
			}
		}

		public static void CompleteDiscovery(TracingContext traceContext)
		{
			string text = string.Empty;
			bool flag = false;
			try
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.GenericHelperTracer, traceContext, string.Format("AddedWorkItemRunningTotal={0}, FailedToAddRunningTotal={1} (Duplicates={2}), ErrorReportProbeTotal={3}, ErrorReportMonitorTotal={4}, ErrorReportResponderTotal={5} ", new object[]
				{
					DiscoveryContext.AddedWorkItemTotal,
					DiscoveryContext.FailedToAddTotal,
					DiscoveryContext.DuplicateTotal,
					DiscoveryContext.ErrorReportProbeTotal,
					DiscoveryContext.ErrorReportMonitorTotal,
					DiscoveryContext.ErrorReportResponderTotal
				}), null, "CompleteDiscovery", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\GenericWorkItemHelper.cs", 324);
				text = GenericWorkItemHelper.GetEntry(traceContext);
				flag = !string.IsNullOrWhiteSpace(text);
			}
			catch (Exception ex)
			{
				flag = true;
				text += ex.ToString();
			}
			finally
			{
				if (flag)
				{
					WTFDiagnostics.TraceError<string>(ExTraceGlobals.GenericHelperTracer, traceContext, "GenericWorkItemHelper.CompleteDiscovery(): Errors were found while loading WorkItems: {0}", text, null, "CompleteDiscovery", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\GenericWorkItemHelper.cs", 338);
				}
				else
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.GenericHelperTracer, traceContext, "GenericWorkItemHelper.CompleteDiscovery(): No errors were found while loading WorkItems.", null, "CompleteDiscovery", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\GenericWorkItemHelper.cs", 342);
				}
				GenericWorkItemHelper.RemoveEntry(traceContext);
			}
		}

		internal static string GetEntry(TracingContext traceContext)
		{
			StringBuilder stringBuilder = null;
			if (GenericWorkItemHelper.discoveryError.TryGetValue(traceContext, out stringBuilder))
			{
				return stringBuilder.ToString();
			}
			return null;
		}

		internal static void WriteEntry(TracingContext traceContext, string format, params object[] args)
		{
			StringBuilder error = new StringBuilder(string.Format(format, args) + " ");
			lock (GenericWorkItemHelper.lockObj)
			{
				GenericWorkItemHelper.discoveryError.AddOrUpdate(traceContext, error, delegate(TracingContext key, StringBuilder existingError)
				{
					existingError.AppendLine(error.ToString());
					return existingError;
				});
			}
		}

		internal static void RemoveEntry(TracingContext traceContext)
		{
			if (GenericWorkItemHelper.discoveryError.ContainsKey(traceContext))
			{
				StringBuilder stringBuilder;
				GenericWorkItemHelper.discoveryError.TryRemove(traceContext, out stringBuilder);
			}
		}

		private static void CheckArgs<T>(T obj, string objVarName, TracingContext traceContext) where T : class
		{
			if (obj == null)
			{
				WTFDiagnostics.TraceError<string>(ExTraceGlobals.GenericHelperTracer, traceContext, "Argument '{0}' is null", objVarName, null, "CheckArgs", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\GenericWorkItemHelper.cs", 426);
				throw new ArgumentNullException(objVarName);
			}
		}

		private static ConcurrentDictionary<TracingContext, StringBuilder> discoveryError = new ConcurrentDictionary<TracingContext, StringBuilder>();

		private static object lockObj = new object();

		private static object discoveryLock = new object();
	}
}
