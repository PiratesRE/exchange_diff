using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Monitors;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Fips.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.Components.Common.Probes;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Transport.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring
{
	internal abstract class DiscoveryContext
	{
		public DiscoveryContext(XmlNode node, TracingContext traceContext) : this(node, traceContext, null)
		{
		}

		public DiscoveryContext(XmlNode node, TracingContext traceContext, MaintenanceResult result)
		{
			this.ContextNode = node;
			this.MaintenanceResult = result;
			this.TraceContext = traceContext;
			this.Probes = new List<ProbeDefinition>();
			this.Monitors = new List<MonitorDefinition>();
			this.Responders = new List<ResponderDefinition>();
		}

		internal static int AddedWorkItemTotal
		{
			get
			{
				return DiscoveryContext.existingWorkItems.Count;
			}
		}

		internal static int FailedToAddTotal
		{
			get
			{
				return DiscoveryContext.failedToAddTotal;
			}
		}

		internal static int DuplicateTotal
		{
			get
			{
				return DiscoveryContext.duplicateTotal;
			}
		}

		internal static int ErrorReportProbeTotal
		{
			get
			{
				return DiscoveryContext.errorReportProbeTotal;
			}
		}

		internal static int ErrorReportMonitorTotal
		{
			get
			{
				return DiscoveryContext.errorReportMonitorTotal;
			}
		}

		internal static int ErrorReportResponderTotal
		{
			get
			{
				return DiscoveryContext.errorReportResponderTotal;
			}
		}

		internal XmlNode ContextNode { get; set; }

		internal MaintenanceResult MaintenanceResult { get; set; }

		internal TracingContext TraceContext { get; set; }

		internal List<ProbeDefinition> Probes { get; set; }

		internal List<MonitorDefinition> Monitors { get; set; }

		internal List<ResponderDefinition> Responders { get; set; }

		internal abstract void ProcessDefinitions(IMaintenanceWorkBroker broker);

		protected void ProcessProbeDefinition()
		{
			foreach (XmlNode xmlNode in DefinitionHelperBase.GetDescendants(this.ContextNode, "Probe"))
			{
				try
				{
					string mandatoryXmlAttribute = DefinitionHelperBase.GetMandatoryXmlAttribute<string>(xmlNode, "TypeName", this.TraceContext);
					string assemblyName = this.GetAssemblyName(xmlNode);
					ProbeDefinitionHelper probeHelper = this.GetProbeHelper(mandatoryXmlAttribute, assemblyName, xmlNode, this);
					probeHelper.ReadDiscoveryXml();
					foreach (ProbeDefinition probeDefinition in probeHelper.CreateDefinition())
					{
						probeHelper.LogDefinitions(probeDefinition);
						List<string> list = new List<string>();
						if (!probeDefinition.Validate(list))
						{
							StringBuilder sb = new StringBuilder();
							sb.AppendFormat("A Probe node of type {0} fails validation or misses mandatory attributes. ", mandatoryXmlAttribute);
							sb.AppendLine();
							list.ForEach(delegate(string e)
							{
								sb.AppendLine(e);
							});
							WTFDiagnostics.TraceError(ExTraceGlobals.GenericHelperTracer, this.TraceContext, sb.ToString(), null, "ProcessProbeDefinition", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\DiscoveryContext.cs", 368);
							throw new XmlException(sb.ToString());
						}
						this.Probes.Add(probeDefinition);
					}
				}
				catch (Exception e)
				{
					Exception e2;
					ProbeDefinition item = this.CreateDiscoveryErrorReportProbe(e2, null);
					this.Probes.Add(item);
				}
			}
		}

		protected void ProcessMonitorDefinitions()
		{
			foreach (XmlNode xmlNode in DefinitionHelperBase.GetDescendants(this.ContextNode, "Monitor"))
			{
				try
				{
					string mandatoryXmlAttribute = DefinitionHelperBase.GetMandatoryXmlAttribute<string>(xmlNode, "TypeName", this.TraceContext);
					string assemblyName = this.GetAssemblyName(xmlNode);
					MonitorDefinitionHelper monitorHelper = this.GetMonitorHelper(mandatoryXmlAttribute, assemblyName, xmlNode, this);
					monitorHelper.ReadDiscoveryXml();
					MonitorDefinition monitorDefinition = monitorHelper.CreateDefinition();
					if (monitorDefinition != null)
					{
						monitorHelper.LogDefinitions(monitorDefinition);
						List<string> list = new List<string>();
						if (!monitorDefinition.Validate(list))
						{
							StringBuilder sb = new StringBuilder();
							sb.AppendFormat("A Monitor node of type {0} fails validation or misses mandatory attributes. ", mandatoryXmlAttribute);
							sb.AppendLine();
							list.ForEach(delegate(string e)
							{
								sb.AppendLine(e);
							});
							WTFDiagnostics.TraceError(ExTraceGlobals.GenericHelperTracer, this.TraceContext, sb.ToString(), null, "ProcessMonitorDefinitions", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\DiscoveryContext.cs", 417);
							throw new XmlException(sb.ToString());
						}
						this.Monitors.Add(monitorDefinition);
					}
				}
				catch (Exception e)
				{
					Exception e2;
					MonitorDefinition item = this.CreateDiscoveryErrorReportMonitor(e2, null);
					this.Monitors.Add(item);
				}
			}
		}

		protected void ProcessResponderDefinitions()
		{
			if (this.Monitors.Count == 0)
			{
				if (this.GetDescendantCount("Responder") != 0)
				{
					Exception e3 = new XmlException(string.Format("A '{0}' node has responders but no monitors", this.ContextNode.Name));
					ResponderDefinition item = this.CreateDiscoveryErrorReportResponder(e3, null);
					this.Responders.Add(item);
				}
				return;
			}
			foreach (XmlNode xmlNode in DefinitionHelperBase.GetDescendants(this.ContextNode, "Responder"))
			{
				try
				{
					string mandatoryXmlAttribute = DefinitionHelperBase.GetMandatoryXmlAttribute<string>(xmlNode, "TypeName", this.TraceContext);
					string assemblyName = this.GetAssemblyName(xmlNode);
					ResponderDefinitionHelper responderHelper = this.GetResponderHelper(mandatoryXmlAttribute, assemblyName, xmlNode, this);
					responderHelper.ReadDiscoveryXml();
					ResponderDefinition responderDefinition = responderHelper.CreateDefinition();
					if (responderDefinition != null)
					{
						responderHelper.LogDefinitions(responderDefinition);
						List<string> list = new List<string>();
						if (!responderDefinition.Validate(list))
						{
							StringBuilder sb = new StringBuilder();
							sb.AppendFormat("A Responder node of type {0} fails validation or misses mandatory attributes. ", mandatoryXmlAttribute);
							sb.AppendLine();
							list.ForEach(delegate(string e)
							{
								sb.AppendLine(e);
							});
							WTFDiagnostics.TraceError(ExTraceGlobals.GenericHelperTracer, this.TraceContext, sb.ToString(), null, "ProcessResponderDefinitions", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\DiscoveryContext.cs", 479);
							throw new XmlException(sb.ToString());
						}
						this.Responders.Add(responderDefinition);
					}
				}
				catch (Exception e2)
				{
					ResponderDefinition item2 = this.CreateDiscoveryErrorReportResponder(e2, null);
					this.Responders.Add(item2);
				}
			}
		}

		protected void AddDefinitions(IMaintenanceWorkBroker broker)
		{
			DiscoveryContext.AddDefinitionResult arg = this.AddProbeDefinitions(broker, this.Probes);
			DiscoveryContext.AddDefinitionResult arg2 = this.AddMonitorDefinition(broker, this.Monitors);
			DiscoveryContext.AddDefinitionResult arg3 = this.AddResponderDefinitions(broker, this.Responders);
			if (this.MaintenanceResult != null)
			{
				string text = string.Format("ProbeDefinitionResult = {0}; MonitorDefinitionResult = {1}; ResponderDefinitionResult = {2};", arg, arg2, arg3);
				this.MaintenanceResult.StateAttribute2 = string.Format("AddedWorkItemRunningTotal={0}, FailedToAddRunningTotal={1} (Duplicates={2}), ErrorReportProbeTotal={3}, ErrorReportMonitorTotal={4}, ErrorReportResponderTotal={5} {6} ", new object[]
				{
					DiscoveryContext.AddedWorkItemTotal,
					DiscoveryContext.FailedToAddTotal,
					DiscoveryContext.DuplicateTotal,
					DiscoveryContext.ErrorReportProbeTotal,
					DiscoveryContext.ErrorReportMonitorTotal,
					DiscoveryContext.ErrorReportResponderTotal,
					text
				});
			}
		}

		protected bool CheckEnvironment()
		{
			return this.CheckEnv() && this.CheckRoles();
		}

		protected bool CheckEnv()
		{
			bool optionalXmlAttribute = DefinitionHelperBase.GetOptionalXmlAttribute<bool>(this.ContextNode, "FfoDataCenter", true, this.TraceContext);
			bool optionalXmlAttribute2 = DefinitionHelperBase.GetOptionalXmlAttribute<bool>(this.ContextNode, "ExoDataCenter", true, this.TraceContext);
			bool optionalXmlAttribute3 = DefinitionHelperBase.GetOptionalXmlAttribute<bool>(this.ContextNode, "DataCenterDedicated", true, this.TraceContext);
			bool optionalXmlAttribute4 = DefinitionHelperBase.GetOptionalXmlAttribute<bool>(this.ContextNode, "OnPremise", true, this.TraceContext);
			bool optionalXmlAttribute5 = DefinitionHelperBase.GetOptionalXmlAttribute<bool>(this.ContextNode, "FfoGallatinDataCenter", true, this.TraceContext);
			bool flag = optionalXmlAttribute && FfoLocalEndpointManager.IsForefrontForOfficeDatacenter && (optionalXmlAttribute5 || !FfoLocalEndpointManager.IsForefrontForOfficeGallatinDatacenter);
			bool flag2 = optionalXmlAttribute2 && LocalEndpointManager.IsDataCenter && !FfoLocalEndpointManager.IsForefrontForOfficeDatacenter;
			bool flag3 = optionalXmlAttribute3 && LocalEndpointManager.IsDataCenterDedicated;
			bool flag4 = optionalXmlAttribute4 && !LocalEndpointManager.IsDataCenter && !LocalEndpointManager.IsDataCenterDedicated;
			if (flag || flag2 || flag3 || flag4)
			{
				return true;
			}
			string message = string.Format("None of the supported environments were selected or the current environment isn't supported (see {0}).", this.ContextNode.Name);
			WTFDiagnostics.TraceDebug(ExTraceGlobals.GenericHelperTracer, this.TraceContext, message, null, "CheckEnv", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\DiscoveryContext.cs", 559);
			return false;
		}

		protected List<string> ParseXmlRolesAttribute(string attributeName)
		{
			char[] separator = new char[]
			{
				','
			};
			List<string> listRoles = new List<string>();
			string optionalXmlAttribute = DefinitionHelperBase.GetOptionalXmlAttribute<string>(this.ContextNode, attributeName, string.Empty, this.TraceContext);
			optionalXmlAttribute.Split(separator).ToList<string>().ForEach(delegate(string r)
			{
				listRoles.Add(r.Trim());
			});
			listRoles.RemoveAll((string r) => string.IsNullOrWhiteSpace(r));
			return listRoles;
		}

		internal static bool CheckRoles(List<string> excludeFfoRoles, List<string> ffoRoles, List<string> excludeExchangeRoles, List<string> exchangeRoles)
		{
			bool flag = LocalEndpointManager.IsDataCenter && !FfoLocalEndpointManager.IsForefrontForOfficeDatacenter;
			if (!FfoLocalEndpointManager.IsForefrontForOfficeDatacenter && (ffoRoles.Count != 0 || excludeFfoRoles.Count != 0))
			{
				return false;
			}
			if (!flag && (exchangeRoles.Count != 0 || excludeExchangeRoles.Count != 0))
			{
				return false;
			}
			if (FfoLocalEndpointManager.IsForefrontForOfficeDatacenter)
			{
				if (excludeFfoRoles.Any((string role) => DiscoveryContext.FfoInstalledRoles.ContainsKey(role) && DiscoveryContext.FfoInstalledRoles[role]))
				{
					return false;
				}
				return ffoRoles.Any((string role) => DiscoveryContext.FfoInstalledRoles.ContainsKey(role) && DiscoveryContext.FfoInstalledRoles[role]) || ffoRoles.Count == 0;
			}
			else
			{
				if (!LocalEndpointManager.IsDataCenter || FfoLocalEndpointManager.IsForefrontForOfficeDatacenter)
				{
					return true;
				}
				if (excludeExchangeRoles.Any((string role) => DiscoveryContext.ExchangeInstalledRoles.ContainsKey(role) && DiscoveryContext.ExchangeInstalledRoles[role]))
				{
					return false;
				}
				return exchangeRoles.Any((string role) => DiscoveryContext.ExchangeInstalledRoles.ContainsKey(role) && DiscoveryContext.ExchangeInstalledRoles[role]) || exchangeRoles.Count == 0;
			}
		}

		protected bool CheckRoles()
		{
			List<string> excludeFfoRoles = this.ParseXmlRolesAttribute("ExcludeFfoRoles");
			List<string> ffoRoles = this.ParseXmlRolesAttribute("FfoRoles");
			List<string> excludeExchangeRoles = this.ParseXmlRolesAttribute("ExcludeExchangeRoles");
			List<string> exchangeRoles = this.ParseXmlRolesAttribute("ExchangeRoles");
			return DiscoveryContext.CheckRoles(excludeFfoRoles, ffoRoles, excludeExchangeRoles, exchangeRoles);
		}

		protected int GetDescendantCount(string descendantName)
		{
			return this.ContextNode.SelectNodes(descendantName).Count;
		}

		private DiscoveryContext.AddDefinitionResult AddProbeDefinitions(IMaintenanceWorkBroker broker, IEnumerable<ProbeDefinition> probeDefinitions)
		{
			int num = 0;
			int num2 = 0;
			foreach (ProbeDefinition probeDefinition in probeDefinitions)
			{
				string text = string.Format("{0}.{1}.{2}", probeDefinition.Name, probeDefinition.ServiceName, probeDefinition.TypeName);
				try
				{
					string entryKey = string.Format("{0},{1}", probeDefinition.Name, probeDefinition.ServiceName);
					this.AddEntry(entryKey, text);
					broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, this.TraceContext);
					num++;
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.GenericHelperTracer, this.TraceContext, "Added probe ({0}).", text, null, "AddProbeDefinitions", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\DiscoveryContext.cs", 694);
				}
				catch (Exception e)
				{
					Interlocked.Increment(ref DiscoveryContext.failedToAddTotal);
					this.CreateDiscoveryErrorReportProbe(e, text);
					broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, this.TraceContext);
					WTFDiagnostics.TraceError<string>(ExTraceGlobals.GenericHelperTracer, this.TraceContext, "Failed to add probe ({0}).", text, null, "AddProbeDefinitions", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\DiscoveryContext.cs", 704);
				}
				finally
				{
					num2++;
				}
			}
			WTFDiagnostics.TraceInformation<int, int>(ExTraceGlobals.GenericHelperTracer, this.TraceContext, "Probes: added={0}, total={1}.", num, num2, null, "AddProbeDefinitions", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\DiscoveryContext.cs", 712);
			return new DiscoveryContext.AddDefinitionResult(num2, num);
		}

		private DiscoveryContext.AddDefinitionResult AddMonitorDefinition(IMaintenanceWorkBroker broker, IEnumerable<MonitorDefinition> monitorDefinitions)
		{
			int num = 0;
			int num2 = 0;
			foreach (MonitorDefinition monitorDefinition in monitorDefinitions)
			{
				string text = string.Format("{0}.{1}.{2}.{3}", new object[]
				{
					monitorDefinition.Name,
					monitorDefinition.ServiceName,
					monitorDefinition.SampleMask,
					monitorDefinition.TypeName
				});
				try
				{
					string entryKey = string.Format("{0},{1}", monitorDefinition.Name, monitorDefinition.ServiceName);
					this.AddEntry(entryKey, text);
					broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, this.TraceContext);
					num++;
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.GenericHelperTracer, this.TraceContext, "Added monitor ({0}).", text, null, "AddMonitorDefinition", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\DiscoveryContext.cs", 739);
				}
				catch (Exception e)
				{
					Interlocked.Increment(ref DiscoveryContext.failedToAddTotal);
					MonitorDefinition definition = this.CreateDiscoveryErrorReportMonitor(e, text);
					broker.AddWorkDefinition<MonitorDefinition>(definition, this.TraceContext);
					WTFDiagnostics.TraceError<string>(ExTraceGlobals.GenericHelperTracer, this.TraceContext, "Failed to add monitor ({0}).", text, null, "AddMonitorDefinition", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\DiscoveryContext.cs", 749);
				}
				finally
				{
					num2++;
				}
			}
			WTFDiagnostics.TraceInformation<int, int>(ExTraceGlobals.GenericHelperTracer, this.TraceContext, "Monitors: added={0}, total={1}.", num, num2, null, "AddMonitorDefinition", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\DiscoveryContext.cs", 757);
			return new DiscoveryContext.AddDefinitionResult(num2, num);
		}

		private DiscoveryContext.AddDefinitionResult AddResponderDefinitions(IMaintenanceWorkBroker broker, IEnumerable<ResponderDefinition> responderDefintions)
		{
			int num = 0;
			int num2 = 0;
			foreach (ResponderDefinition responderDefinition in this.Responders)
			{
				string text = string.Format("{0}.{1}.{2}.{3}", new object[]
				{
					responderDefinition.Name,
					responderDefinition.ServiceName,
					responderDefinition.AlertMask,
					responderDefinition.TypeName
				});
				try
				{
					string entryKey = string.Format("{0},{1}", responderDefinition.Name, responderDefinition.ServiceName);
					this.AddEntry(entryKey, text);
					broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, this.TraceContext);
					num++;
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.GenericHelperTracer, this.TraceContext, "Added responder ({0}).", text, null, "AddResponderDefinitions", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\DiscoveryContext.cs", 784);
				}
				catch (Exception e)
				{
					Interlocked.Increment(ref DiscoveryContext.failedToAddTotal);
					ResponderDefinition definition = this.CreateDiscoveryErrorReportResponder(e, text);
					broker.AddWorkDefinition<ResponderDefinition>(definition, this.TraceContext);
					WTFDiagnostics.TraceError<string>(ExTraceGlobals.GenericHelperTracer, this.TraceContext, "Failed to add responder ({0}).", text, null, "AddResponderDefinitions", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\DiscoveryContext.cs", 794);
				}
				finally
				{
					num2++;
				}
			}
			WTFDiagnostics.TraceInformation<int, int>(ExTraceGlobals.GenericHelperTracer, this.TraceContext, "Responders: added={0}, total={1}.", num, num2, null, "AddResponderDefinitions", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\DiscoveryContext.cs", 802);
			return new DiscoveryContext.AddDefinitionResult(num2, num);
		}

		private ProbeDefinitionHelper GetProbeHelper(string typeName, string assemblyName, XmlNode probeNode, DiscoveryContext context)
		{
			Type workItemType = this.GetWorkItemType(typeName, assemblyName);
			if (workItemType == null && (!DiscoveryContext.WellKnownProbes.TryGetValue(typeName, out workItemType) || workItemType == null))
			{
				string message = string.Format("Can't find the type of this probe {0}.", typeName);
				WTFDiagnostics.TraceError(ExTraceGlobals.GenericHelperTracer, this.TraceContext, message, null, "GetProbeHelper", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\DiscoveryContext.cs", 824);
				throw new ArgumentException(message);
			}
			ProbeDefinitionHelper probeDefinitionHelper = (ProbeDefinitionHelper)this.GetUserSpecifiedHelper(probeNode);
			if (probeDefinitionHelper == null && (!DiscoveryContext.WellKnownProbeHelpers.TryGetValue(typeName, out probeDefinitionHelper) || probeDefinitionHelper == null))
			{
				probeDefinitionHelper = new DefaultProbeHelper();
			}
			probeDefinitionHelper.WorkItemType = workItemType;
			probeDefinitionHelper.DefinitionNode = probeNode;
			probeDefinitionHelper.DiscoveryContext = context;
			probeDefinitionHelper.TraceContext = context.TraceContext;
			return probeDefinitionHelper;
		}

		private MonitorDefinitionHelper GetMonitorHelper(string typeName, string assemblyName, XmlNode monitorNode, DiscoveryContext context)
		{
			Type workItemType = this.GetWorkItemType(typeName, assemblyName);
			if (workItemType == null && (!DiscoveryContext.WellKnownMonitors.TryGetValue(typeName, out workItemType) || workItemType == null))
			{
				string message = string.Format("Can't find the type of this monitor {0}.", typeName);
				WTFDiagnostics.TraceError(ExTraceGlobals.GenericHelperTracer, this.TraceContext, message, null, "GetMonitorHelper", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\DiscoveryContext.cs", 865);
				throw new ArgumentException(message);
			}
			MonitorDefinitionHelper monitorDefinitionHelper = (MonitorDefinitionHelper)this.GetUserSpecifiedHelper(monitorNode);
			if (monitorDefinitionHelper == null && (!DiscoveryContext.WellKnownMonitorHelpers.TryGetValue(typeName, out monitorDefinitionHelper) || monitorDefinitionHelper == null))
			{
				monitorDefinitionHelper = new DefaultMonitorHelper();
			}
			monitorDefinitionHelper.WorkItemType = workItemType;
			monitorDefinitionHelper.DefinitionNode = monitorNode;
			monitorDefinitionHelper.DiscoveryContext = context;
			monitorDefinitionHelper.TraceContext = context.TraceContext;
			return monitorDefinitionHelper;
		}

		private ResponderDefinitionHelper GetResponderHelper(string typeName, string assemblyName, XmlNode responderNode, DiscoveryContext context)
		{
			Type workItemType = this.GetWorkItemType(typeName, assemblyName);
			if (workItemType == null && (!DiscoveryContext.WellKnownResponders.TryGetValue(typeName, out workItemType) || workItemType == null))
			{
				string message = string.Format("Can't find the type of this responder {0}.", typeName);
				WTFDiagnostics.TraceError(ExTraceGlobals.GenericHelperTracer, this.TraceContext, message, null, "GetResponderHelper", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\DiscoveryContext.cs", 906);
				throw new ArgumentException(message);
			}
			ResponderDefinitionHelper responderDefinitionHelper = (ResponderDefinitionHelper)this.GetUserSpecifiedHelper(responderNode);
			if (responderDefinitionHelper == null && (!DiscoveryContext.WellKnownResponderHelpers.TryGetValue(typeName, out responderDefinitionHelper) || responderDefinitionHelper == null))
			{
				responderDefinitionHelper = new DefaultResponderHelper();
			}
			responderDefinitionHelper.WorkItemType = workItemType;
			responderDefinitionHelper.DefinitionNode = responderNode;
			responderDefinitionHelper.DiscoveryContext = context;
			responderDefinitionHelper.TraceContext = context.TraceContext;
			return responderDefinitionHelper;
		}

		private Type GetWorkItemType(string typeName, string assemblyName)
		{
			Type type = Type.GetType(typeName, false);
			if (type == null)
			{
				string assemblyFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), assemblyName);
				Assembly assembly = Assembly.LoadFrom(assemblyFile);
				type = assembly.GetType(typeName, false, true);
			}
			return type;
		}

		private ProbeDefinition CreateDiscoveryErrorReportProbe(Exception e, string probeName)
		{
			Interlocked.Increment(ref DiscoveryContext.errorReportProbeTotal);
			GenericWorkItemHelper.WriteEntry(this.TraceContext, "{0} from probe: {1}", new object[]
			{
				e.GetType().Name,
				this.GetExceptionMessage(e)
			});
			string text = string.Format("Probe error: {0}", e.ToString());
			WTFDiagnostics.TraceError(ExTraceGlobals.GenericHelperTracer, this.TraceContext, text, null, "CreateDiscoveryErrorReportProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\DiscoveryContext.cs", 965);
			return new ProbeDefinition
			{
				TypeName = typeof(DiscoveryErrorReportProbe).FullName,
				AssemblyPath = typeof(DiscoveryErrorReportProbe).Assembly.Location,
				Name = string.Format("DiscoveryErrorReportProbe.{0}", string.IsNullOrWhiteSpace(probeName) ? DiscoveryContext.rand.Next().ToString() : probeName),
				ServiceName = ExchangeComponent.FfoDeployment.Name,
				RecurrenceIntervalSeconds = 0,
				TimeoutSeconds = 30,
				MaxRetryAttempts = 0,
				ExtensionAttributes = new XElement("Exception", text).ToString(),
				Enabled = false
			};
		}

		private MonitorDefinition CreateDiscoveryErrorReportMonitor(Exception e, string monitorName)
		{
			Interlocked.Increment(ref DiscoveryContext.errorReportMonitorTotal);
			GenericWorkItemHelper.WriteEntry(this.TraceContext, "{0} from monitor: {1}", new object[]
			{
				e.GetType().Name,
				this.GetExceptionMessage(e)
			});
			string text = string.Format("Monitor error: {0}", e.ToString());
			WTFDiagnostics.TraceError(ExTraceGlobals.GenericHelperTracer, this.TraceContext, text, null, "CreateDiscoveryErrorReportMonitor", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\DiscoveryContext.cs", 995);
			MonitorDefinition monitorDefinition = new MonitorDefinition();
			monitorDefinition.TypeName = typeof(DiscoveryErrorReportMonitor).FullName;
			monitorDefinition.AssemblyPath = typeof(DiscoveryErrorReportMonitor).Assembly.Location;
			monitorDefinition.Name = string.Format("DiscoveryErrorReportMonitor.{0}", string.IsNullOrWhiteSpace(monitorName) ? DiscoveryContext.rand.Next().ToString() : monitorName);
			monitorDefinition.ServiceName = ExchangeComponent.FfoDeployment.Name;
			monitorDefinition.SampleMask = "DiscoveryErrorReportMonitor";
			Component component = null;
			ExchangeComponent.WellKnownComponents.TryGetValue("FfoMonitoring", out component);
			monitorDefinition.Component = component;
			monitorDefinition.RecurrenceIntervalSeconds = 0;
			monitorDefinition.TimeoutSeconds = 30;
			monitorDefinition.MaxRetryAttempts = 0;
			monitorDefinition.MonitoringIntervalSeconds = 1000;
			monitorDefinition.ExtensionAttributes = new XElement("Exception", text).ToString();
			monitorDefinition.Enabled = false;
			return monitorDefinition;
		}

		private ResponderDefinition CreateDiscoveryErrorReportResponder(Exception e, string responderName)
		{
			Interlocked.Increment(ref DiscoveryContext.errorReportResponderTotal);
			GenericWorkItemHelper.WriteEntry(this.TraceContext, "{0} from responder: {1}", new object[]
			{
				e.GetType().Name,
				this.GetExceptionMessage(e)
			});
			string text = string.Format("Responder error: {0}", e.ToString());
			WTFDiagnostics.TraceError(ExTraceGlobals.GenericHelperTracer, this.TraceContext, text, null, "CreateDiscoveryErrorReportResponder", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\DiscoveryContext.cs", 1030);
			return new ResponderDefinition
			{
				TypeName = typeof(DiscoveryErrorReportResponder).FullName,
				AssemblyPath = typeof(DiscoveryErrorReportResponder).Assembly.Location,
				Name = string.Format("DiscoveryErrorReportResponder.{0}", string.IsNullOrWhiteSpace(responderName) ? DiscoveryContext.rand.Next().ToString() : responderName),
				ServiceName = ExchangeComponent.FfoDeployment.Name,
				AlertMask = "DiscoveryErrorReportResponder",
				AlertTypeId = "DiscoveryErrorReportResponder",
				RecurrenceIntervalSeconds = 0,
				MaxRetryAttempts = 0,
				TimeoutSeconds = 30,
				ExtensionAttributes = new XElement("Exception", text).ToString(),
				Enabled = false
			};
		}

		private DefinitionHelperBase GetUserSpecifiedHelper(XmlNode node)
		{
			string optionalXmlAttribute = DefinitionHelperBase.GetOptionalXmlAttribute<string>(node, "HelperTypeName", string.Empty, this.TraceContext);
			DefinitionHelperBase result = null;
			if (!string.IsNullOrWhiteSpace(optionalXmlAttribute))
			{
				Type type = Type.GetType(optionalXmlAttribute, false);
				if (type == null)
				{
					type = Type.GetType("Microsoft.Exchange.Monitoring.ActiveMonitoring." + optionalXmlAttribute, false);
				}
				if (!(type != null))
				{
					string message = string.Format("Can't find the type of the specified helper {0}.", optionalXmlAttribute);
					WTFDiagnostics.TraceError(ExTraceGlobals.GenericHelperTracer, this.TraceContext, message, null, "GetUserSpecifiedHelper", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\DiscoveryContext.cs", 1077);
					throw new ArgumentException(message);
				}
				result = (DefinitionHelperBase)Activator.CreateInstance(type);
			}
			return result;
		}

		private string GetAssemblyName(XmlNode node)
		{
			string optionalXmlAttribute = DefinitionHelperBase.GetOptionalXmlAttribute<string>(node, "AssemblyPath", string.Empty, this.TraceContext);
			if (string.IsNullOrWhiteSpace(optionalXmlAttribute))
			{
				optionalXmlAttribute = DefinitionHelperBase.GetOptionalXmlAttribute<string>(node, "AssemblyName", "Microsoft.Forefront.Monitoring.ActiveMonitoring.Local.Components.dll", this.TraceContext);
			}
			return optionalXmlAttribute;
		}

		private void AddEntry(string entryKey, string value)
		{
			DiscoveryContext.existingWorkItems.AddOrUpdate(entryKey, value, delegate(string key, string existingvalue)
			{
				DiscoveryContext.duplicateTotal++;
				string message = string.Format("Duplicate work item '{0}'. The Name and ServiceName combo must be unique.", key);
				WTFDiagnostics.TraceError(ExTraceGlobals.GenericHelperTracer, this.TraceContext, message, null, "AddEntry", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\DiscoveryContext.cs", 1118);
				throw new XmlException(message);
			});
		}

		private string GetExceptionMessage(Exception e)
		{
			if (e.InnerException == null)
			{
				return e.Message;
			}
			return e.Message + " InnerException: " + e.InnerException.Message;
		}

		internal static readonly Dictionary<string, Type> WellKnownProbes = new Dictionary<string, Type>
		{
			{
				"FilteredGenericEventLogProbe",
				typeof(FilteredGenericEventLogProbe)
			},
			{
				"GenericEventLogProbe",
				typeof(GenericEventLogProbe)
			},
			{
				"GenericServiceProbe",
				typeof(GenericServiceProbe)
			},
			{
				"GenericProcessCrashDetectionProbe",
				typeof(GenericProcessCrashDetectionProbe)
			}
		};

		internal static readonly Dictionary<string, ProbeDefinitionHelper> WellKnownProbeHelpers = new Dictionary<string, ProbeDefinitionHelper>
		{
			{
				"Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes.TransportSmtpProbe",
				new TransportProbeHelper()
			},
			{
				"Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes.SafetyNetProbe",
				new TransportProbeHelper()
			},
			{
				"Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes.SmtpConnectionProbe",
				new TransportProbeHelper()
			},
			{
				"Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes.SmtpProbe",
				new TransportProbeHelper()
			},
			{
				"Microsoft.Forefront.Monitoring.ActiveMonitoring.WebUIProbe.WebUIProbe",
				new WebUIProbeHelper()
			},
			{
				"Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes.IrmTransportSmtpProbe",
				new TransportProbeHelper()
			},
			{
				"Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes.E4eTransportSmtpProbe",
				new TransportProbeHelper()
			}
		};

		internal static readonly Dictionary<string, Type> WellKnownMonitors = new Dictionary<string, Type>
		{
			{
				"ComponentHealthHeartbeatMonitor",
				typeof(ComponentHealthHeartbeatMonitor)
			},
			{
				"ComponentHealthPercentFailureMonitor",
				typeof(ComponentHealthPercentFailureMonitor)
			},
			{
				"OverallConsecutiveSampleValueAboveThresholdMonitor",
				typeof(OverallConsecutiveSampleValueAboveThresholdMonitor)
			},
			{
				"OverallConsecutiveSampleValueBelowThresholdMonitor",
				typeof(OverallConsecutiveSampleValueBelowThresholdMonitor)
			},
			{
				"OverallConsecutiveProbeFailuresMonitor",
				typeof(OverallConsecutiveProbeFailuresMonitor)
			},
			{
				"OverallXFailuresMonitor",
				typeof(OverallXFailuresMonitor)
			},
			{
				"OverallPercentSuccessMonitor",
				typeof(OverallPercentSuccessMonitor)
			}
		};

		internal static readonly Dictionary<string, MonitorDefinitionHelper> WellKnownMonitorHelpers = new Dictionary<string, MonitorDefinitionHelper>
		{
			{
				"ComponentHealthHeartbeatMonitor",
				new ComponentHealthHeartbeatMonitorHelper()
			},
			{
				typeof(ComponentHealthHeartbeatMonitor).FullName,
				new ComponentHealthHeartbeatMonitorHelper()
			},
			{
				"ComponentHealthPercentFailureMonitor",
				new ComponentHealthPercentFailureMonitorHelper()
			},
			{
				typeof(ComponentHealthPercentFailureMonitor).FullName,
				new ComponentHealthPercentFailureMonitorHelper()
			},
			{
				"OverallConsecutiveSampleValueAboveThresholdMonitor",
				new OverallConsecutiveSampleValueAboveThresholdMonitorHelper()
			},
			{
				typeof(OverallConsecutiveSampleValueAboveThresholdMonitor).FullName,
				new OverallConsecutiveSampleValueAboveThresholdMonitorHelper()
			},
			{
				"OverallConsecutiveSampleValueBelowThresholdMonitor",
				new OverallConsecutiveSampleValueBelowThresholdMonitorHelper()
			},
			{
				typeof(OverallConsecutiveSampleValueBelowThresholdMonitor).FullName,
				new OverallConsecutiveSampleValueBelowThresholdMonitorHelper()
			},
			{
				"OverallConsecutiveProbeFailuresMonitor",
				new OverallConsecutiveProbeFailuresMonitorHelper()
			},
			{
				typeof(OverallConsecutiveProbeFailuresMonitor).FullName,
				new OverallConsecutiveProbeFailuresMonitorHelper()
			},
			{
				"OverallXFailuresMonitor",
				new OverallXFailuresMonitorHelper()
			},
			{
				typeof(OverallXFailuresMonitor).FullName,
				new OverallXFailuresMonitorHelper()
			},
			{
				"OverallPercentSuccessMonitor",
				new OverallPercentSuccessMonitorHelper()
			},
			{
				typeof(OverallPercentSuccessMonitor).FullName,
				new OverallPercentSuccessMonitorHelper()
			}
		};

		internal static readonly Dictionary<string, Type> WellKnownResponders = new Dictionary<string, Type>
		{
			{
				"EscalateResponder",
				typeof(EscalateResponder)
			},
			{
				"RecoveryModeResponder",
				typeof(RecoveryModeResponder)
			},
			{
				"ResetIISAppPoolResponder",
				typeof(ResetIISAppPoolResponder)
			},
			{
				"RestartServiceResponder",
				typeof(RestartServiceResponder)
			},
			{
				"SystemFailoverResponder",
				typeof(SystemFailoverResponder)
			},
			{
				"ForceRebootServerResponder",
				typeof(ForceRebootServerResponder)
			},
			{
				"CollectFIPSLogsResponder",
				typeof(CollectFIPSLogsResponder)
			},
			{
				"ControlServiceResponder",
				typeof(ControlServiceResponder)
			}
		};

		internal static readonly Dictionary<string, ResponderDefinitionHelper> WellKnownResponderHelpers = new Dictionary<string, ResponderDefinitionHelper>
		{
			{
				"EscalateResponder",
				new EscalateResponderHelper()
			},
			{
				typeof(EscalateResponder).FullName,
				new EscalateResponderHelper()
			},
			{
				"ResetIISAppPoolResponder",
				new ResetIISAppPoolResponderHelper()
			},
			{
				typeof(ResetIISAppPoolResponder).FullName,
				new ResetIISAppPoolResponderHelper()
			},
			{
				"RestartServiceResponder",
				new RestartServiceResponderHelper()
			},
			{
				typeof(RestartServiceResponder).FullName,
				new RestartServiceResponderHelper()
			},
			{
				"SystemFailoverResponder",
				new SystemFailoverResponderHelper()
			},
			{
				typeof(SystemFailoverResponder).FullName,
				new SystemFailoverResponderHelper()
			},
			{
				"ForceRebootServerResponder",
				new ForceRebootServerResponderHelper()
			},
			{
				typeof(ForceRebootServerResponder).FullName,
				new ForceRebootServerResponderHelper()
			},
			{
				"CollectFIPSLogsResponder",
				new CollectFIPSLogsResponderHelper()
			},
			{
				typeof(CollectFIPSLogsResponder).FullName,
				new CollectFIPSLogsResponderHelper()
			},
			{
				"ControlServiceResponder",
				new ControlServiceResponderHelper()
			},
			{
				typeof(ControlServiceResponder).FullName,
				new ControlServiceResponderHelper()
			}
		};

		internal static readonly Dictionary<string, bool> FfoInstalledRoles = new Dictionary<string, bool>
		{
			{
				"Background",
				FfoLocalEndpointManager.IsBackgroundRoleInstalled
			},
			{
				"CentralAdmin",
				FfoLocalEndpointManager.IsCentralAdminRoleInstalled
			},
			{
				"Database",
				FfoLocalEndpointManager.IsDatabaseRoleInstalled
			},
			{
				"DomainNameServer",
				FfoLocalEndpointManager.IsDomainNameServerRoleInstalled
			},
			{
				"FrontendTransport",
				FfoLocalEndpointManager.IsFrontendTransportRoleInstalled
			},
			{
				"HubTransport",
				FfoLocalEndpointManager.IsHubTransportRoleInstalled
			},
			{
				"InfraDatabase",
				FfoLocalEndpointManager.IsInfraDatabaseRoleInstalled
			},
			{
				"WebService",
				FfoLocalEndpointManager.IsWebServiceInstalled
			}
		};

		internal static readonly Dictionary<string, bool> ExchangeInstalledRoles = new Dictionary<string, bool>
		{
			{
				"AdminTools",
				LocalEndpointManager.Instance.ExchangeServerRoleEndpoint != null && LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsAdminToolsRoleInstalled
			},
			{
				"Bridgehead",
				LocalEndpointManager.Instance.ExchangeServerRoleEndpoint != null && LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsBridgeheadRoleInstalled
			},
			{
				"Cafe",
				LocalEndpointManager.Instance.ExchangeServerRoleEndpoint != null && LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsCafeRoleInstalled
			},
			{
				"CentralAdminDatabase",
				LocalEndpointManager.Instance.ExchangeServerRoleEndpoint != null && LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsCentralAdminDatabaseRoleInstalled
			},
			{
				"CentralAdmin",
				LocalEndpointManager.Instance.ExchangeServerRoleEndpoint != null && LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsCentralAdminRoleInstalled
			},
			{
				"ClientAccess",
				LocalEndpointManager.Instance.ExchangeServerRoleEndpoint != null && LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsClientAccessRoleInstalled
			},
			{
				"FfoWebService",
				LocalEndpointManager.Instance.ExchangeServerRoleEndpoint != null && LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsFfoWebServiceRoleInstalled
			},
			{
				"FrontendTransport",
				LocalEndpointManager.Instance.ExchangeServerRoleEndpoint != null && LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsFrontendTransportRoleInstalled
			},
			{
				"Gateway",
				LocalEndpointManager.Instance.ExchangeServerRoleEndpoint != null && LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsGatewayRoleInstalled
			},
			{
				"LanguangePacks",
				LocalEndpointManager.Instance.ExchangeServerRoleEndpoint != null && LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsLanguangePacksRoleInstalled
			},
			{
				"Mailbox",
				LocalEndpointManager.Instance.ExchangeServerRoleEndpoint != null && LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled
			},
			{
				"MonitoringRole",
				LocalEndpointManager.Instance.ExchangeServerRoleEndpoint != null && LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsMonitoringRoleInstalled
			},
			{
				"UnifiedMessaging",
				LocalEndpointManager.Instance.ExchangeServerRoleEndpoint != null && LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsUnifiedMessagingRoleInstalled
			}
		};

		private static ConcurrentDictionary<string, string> existingWorkItems = new ConcurrentDictionary<string, string>();

		private static int failedToAddTotal = 0;

		private static int duplicateTotal = 0;

		private static int errorReportProbeTotal = 0;

		private static int errorReportMonitorTotal = 0;

		private static int errorReportResponderTotal = 0;

		private static Random rand = new Random();

		private class AddDefinitionResult
		{
			public int Total { get; private set; }

			public int Success { get; private set; }

			public int Failed
			{
				get
				{
					return this.Total - this.Success;
				}
			}

			public AddDefinitionResult(int total, int success)
			{
				if (success > total)
				{
					throw new ArgumentException("success", "Success cannot be greater than the Total");
				}
				this.Total = total;
				this.Success = success;
			}

			public override string ToString()
			{
				return string.Format("Total = {0}; Success = {1}; Failed = {2};", this.Total, this.Success, this.Failed);
			}
		}
	}
}
