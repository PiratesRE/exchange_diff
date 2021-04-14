using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Xml;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal class ScopeMappingLocalEndpoint : ScopeMappingEndpoint, IEndpoint
	{
		public bool RestartOnChange
		{
			get
			{
				return false;
			}
		}

		public Exception Exception { get; set; }

		public void Initialize()
		{
			if (LocalEndpointManager.IsDataCenter)
			{
				this.InitializeScopeAndSystemMonitoringMappings();
				this.InitializeDefaultScopes();
			}
		}

		public bool DetectChange()
		{
			bool result;
			try
			{
				this.Initialize();
				result = true;
			}
			catch (Exception arg)
			{
				WTFDiagnostics.TraceError<Exception>(ExTraceGlobals.ScopeMappingLocalEndpointTracer, this.traceContext, "[ScopeMappingLocalEndpoint.DetectChange] Detection failed: {0}", arg, null, "DetectChange", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\ScopeMappingLocalEndpoint.cs", 85);
				throw;
			}
			return result;
		}

		internal override void InitializeScopeAndSystemMonitoringMappings()
		{
			try
			{
				ConcurrentDictionary<string, ScopeMapping> scopeMappings = new ConcurrentDictionary<string, ScopeMapping>(StringComparer.InvariantCultureIgnoreCase);
				ConcurrentDictionary<string, SystemMonitoringMapping> concurrentDictionary = new ConcurrentDictionary<string, SystemMonitoringMapping>(StringComparer.InvariantCultureIgnoreCase);
				string text = null;
				try
				{
					text = ExchangeSetupContext.BinPath;
				}
				catch (Exception arg)
				{
					WTFDiagnostics.TraceInformation<Exception>(ExTraceGlobals.ScopeMappingLocalEndpointTracer, this.traceContext, "[ScopeMappingLocalEndpoint.InitializeScopeAndSystemMonitoringMappings] Failed to get Exchange Bin folder using ExchangeSetupContext.BinPath: {0}", arg, null, "InitializeScopeAndSystemMonitoringMappings", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\ScopeMappingLocalEndpoint.cs", 114);
				}
				if (string.IsNullOrWhiteSpace(text))
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.ScopeMappingLocalEndpointTracer, this.traceContext, "[ScopeMappingLocalEndpoint.InitializeScopeAndSystemMonitoringMappings] Using current executing folder as the root.", null, "InitializeScopeAndSystemMonitoringMappings", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\ScopeMappingLocalEndpoint.cs", 124);
					text = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				}
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.ScopeMappingLocalEndpointTracer, this.traceContext, "[ScopeMappingLocalEndpoint.InitializeScopeAndSystemMonitoringMappings] fileFolder: {0}", text, null, "InitializeScopeAndSystemMonitoringMappings", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\ScopeMappingLocalEndpoint.cs", 132);
				string text2 = Path.Combine(text, "Monitoring\\Config\\ScopeMappings.xml");
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.ScopeMappingLocalEndpointTracer, this.traceContext, "[ScopeMappingLocalEndpoint.InitializeScopeAndSystemMonitoringMappings] fileName: {0}", text2, null, "InitializeScopeAndSystemMonitoringMappings", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\ScopeMappingLocalEndpoint.cs", 135);
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(text2);
				string localServiceEnvironmentName = this.GetLocalServiceEnvironmentName();
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.ScopeMappingLocalEndpointTracer, this.traceContext, "[ScopeMappingLocalEndpoint.InitializeScopeAndSystemMonitoringMappings] serviceName: {0}", localServiceEnvironmentName, null, "InitializeScopeAndSystemMonitoringMappings", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\ScopeMappingLocalEndpoint.cs", 142);
				XmlNode xmlNode = xmlDocument.SelectSingleNode(string.Format("//Scope[@Name='{0}' and @Type='Service']", localServiceEnvironmentName));
				if (xmlNode == null)
				{
					WTFDiagnostics.TraceWarning<string>(ExTraceGlobals.ScopeMappingLocalEndpointTracer, this.traceContext, "[ScopeMappingLocalEndpoint.InitializeScopeAndSystemMonitoringMappings] Unabled to find startingScope for Service '{0}'.", localServiceEnvironmentName, null, "InitializeScopeAndSystemMonitoringMappings", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\ScopeMappingLocalEndpoint.cs", 147);
				}
				else
				{
					base.InitializeScopeAndSystemMonitoringMappingsFromXml(xmlNode, scopeMappings, concurrentDictionary, null);
					lock (this.updateLocker)
					{
						base.ScopeMappings = scopeMappings;
						base.SystemMonitoringMappings = concurrentDictionary;
					}
				}
			}
			catch (Exception arg2)
			{
				WTFDiagnostics.TraceError<Exception>(ExTraceGlobals.ScopeMappingLocalEndpointTracer, this.traceContext, "[ScopeMappingLocalEndpoint.InitializeScopeAndSystemMonitoringMappings] Initialization failed: {0}", arg2, null, "InitializeScopeAndSystemMonitoringMappings", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\ScopeMappingLocalEndpoint.cs", 166);
				throw;
			}
		}

		internal override void InitializeDefaultScopes()
		{
			try
			{
				if (base.ScopeMappings == null || base.ScopeMappings.Count == 0)
				{
					WTFDiagnostics.TraceWarning<string>(ExTraceGlobals.ScopeMappingLocalEndpointTracer, this.traceContext, "[ScopeMappingLocalEndpoint.InitializeDefaultScopes] No ScopeMappings found for machine: {0}", Environment.MachineName, null, "InitializeDefaultScopes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\ScopeMappingLocalEndpoint.cs", 186);
				}
				else
				{
					ConcurrentDictionary<string, ScopeMapping> concurrentDictionary = new ConcurrentDictionary<string, ScopeMapping>(StringComparer.InvariantCultureIgnoreCase);
					ScopeMapping value = null;
					string localForestFqdn = TopologyProvider.LocalForestFqdn;
					if (!string.IsNullOrWhiteSpace(localForestFqdn) && base.ScopeMappings.TryGetValue(localForestFqdn, out value))
					{
						concurrentDictionary[localForestFqdn] = value;
					}
					try
					{
						IADDatabaseAvailabilityGroup localDAG = CachedAdReader.Instance.LocalDAG;
						if (localDAG != null && base.ScopeMappings.TryGetValue(localDAG.Name, out value))
						{
							concurrentDictionary[localDAG.Name] = value;
						}
					}
					catch (Exception arg)
					{
						WTFDiagnostics.TraceInformation<string, Exception>(ExTraceGlobals.ScopeMappingLocalEndpointTracer, this.traceContext, "[ScopeMappingLocalEndpoint.InitializeDefaultScopes] DAG discovery is n/a on '{0}': {1}", Environment.MachineName, arg, null, "InitializeDefaultScopes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\ScopeMappingLocalEndpoint.cs", 223);
					}
					lock (this.updateLocker)
					{
						base.DefaultScopes = concurrentDictionary;
					}
				}
			}
			catch (Exception arg2)
			{
				WTFDiagnostics.TraceError<Exception>(ExTraceGlobals.ScopeMappingLocalEndpointTracer, this.traceContext, "[ScopeMappingLocalEndpoint.InitializeDefaultScopes] Initialization failed: {0}", arg2, null, "InitializeDefaultScopes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\ScopeMappingLocalEndpoint.cs", 238);
				throw;
			}
		}

		protected virtual string GetLocalServiceEnvironmentName()
		{
			NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			foreach (NetworkInterface networkInterface in allNetworkInterfaces)
			{
				IPInterfaceProperties ipproperties = networkInterface.GetIPProperties();
				if (!string.IsNullOrWhiteSpace(ipproperties.DnsSuffix))
				{
					string text = ipproperties.DnsSuffix.ToLowerInvariant();
					string result;
					if (text.EndsWith("prod.outlook.com") || text.EndsWith("prod.exchangelabs.com"))
					{
						result = ScopeMappingEndpoint.ServiceEnvironment.Prod.ToString();
					}
					else if (text.EndsWith("sdf.exchangelabs.com") && !text.EndsWith("nampdt01.sdf.exchangelabs.com"))
					{
						result = ScopeMappingEndpoint.ServiceEnvironment.Sdf.ToString();
					}
					else if (text.EndsWith("nampdt01.sdf.exchangelabs.com"))
					{
						result = ScopeMappingEndpoint.ServiceEnvironment.Pdt.ToString();
					}
					else
					{
						if (!text.EndsWith("partner.outlook.cn"))
						{
							goto IL_C4;
						}
						result = ScopeMappingEndpoint.ServiceEnvironment.Gallatin.ToString();
					}
					return result;
				}
				IL_C4:;
			}
			return ScopeMappingEndpoint.ServiceEnvironment.Test.ToString();
		}

		private TracingContext traceContext = TracingContext.Default;

		private object updateLocker = new object();
	}
}
