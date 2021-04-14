using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Search.Core.Diagnostics
{
	internal class OperatorDiagnosticsFactory : IDiagnosable
	{
		private OperatorDiagnosticsFactory()
		{
			this.diagnosticCommands = new Dictionary<string, OperatorDiagnosticsFactory.DiagnosticHandler>(StringComparer.OrdinalIgnoreCase)
			{
				{
					"Get-Breadcrumbs",
					new OperatorDiagnosticsFactory.DiagnosticHandler(this.GetBreadcrumbs)
				},
				{
					"Get-Status",
					new OperatorDiagnosticsFactory.DiagnosticHandler(this.GetStatus)
				}
			};
		}

		public static OperatorDiagnosticsFactory Instance
		{
			[DebuggerStepThrough]
			get
			{
				return OperatorDiagnosticsFactory.instance;
			}
		}

		public static DiagnosticsLogConfig.LogDefaults LanguageDetectionLogDefaults
		{
			get
			{
				if (OperatorDiagnosticsFactory.languageDetectionLogDefaults == null)
				{
					string path;
					try
					{
						path = ExchangeSetupContext.InstallPath;
					}
					catch (SetupVersionInformationCorruptException)
					{
						path = string.Empty;
					}
					OperatorDiagnosticsFactory.languageDetectionLogDefaults = new DiagnosticsLogConfig.LogDefaults(Guid.Parse("9f2dd9a4-0b30-4240-8321-f1028f9b583f"), ComponentInstance.Globals.Search.ServiceName, "Search Language Detection Logs", Path.Combine(path, "Logging\\Search\\LanguageDetection"), "LanguageDetection_", "SearchLogs");
				}
				return OperatorDiagnosticsFactory.languageDetectionLogDefaults;
			}
		}

		private static DiagnosticsLogConfig.LogDefaults FailedItemsLogDefaults
		{
			get
			{
				if (OperatorDiagnosticsFactory.failedItemsLogDefaults == null)
				{
					string path;
					try
					{
						path = ExchangeSetupContext.InstallPath;
					}
					catch (SetupVersionInformationCorruptException)
					{
						path = string.Empty;
					}
					OperatorDiagnosticsFactory.failedItemsLogDefaults = new DiagnosticsLogConfig.LogDefaults(Guid.Parse("19615f4c-11b4-4e4c-97e9-8ceb5f70e860"), ComponentInstance.Globals.Search.ServiceName, "Search Failed Items Logs", Path.Combine(path, "Logging\\Search"), "SearchFailedItems_", "SearchLogs");
				}
				return OperatorDiagnosticsFactory.failedItemsLogDefaults;
			}
		}

		public static void EnableGetExchangeDiagnosticsInfo()
		{
			if (!OperatorDiagnosticsFactory.diagnosticsEndpointEnabled)
			{
				Globals.InitializeMultiPerfCounterInstance("noderunner");
				OperatorDiagnosticsFactory.diagnosticsEndpointEnabled = true;
			}
		}

		public string GetDiagnosticComponentName()
		{
			return base.GetType().Name;
		}

		public XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			string text = parameters.Argument;
			ExTraceGlobals.OperatorDiagnosticsTracer.TraceDebug<string>((long)this.GetHashCode(), "GetDiagnosticsInfo command: {0}", text);
			if (string.IsNullOrEmpty(text))
			{
				text = "Get-Status";
			}
			string[] array = text.Split(null, 2, StringSplitOptions.RemoveEmptyEntries);
			string key = (array.Length >= 1) ? array[0] : null;
			string remainingArgs = (array.Length >= 2) ? array[1] : null;
			OperatorDiagnosticsFactory.DiagnosticHandler diagnosticHandler;
			if (!this.diagnosticCommands.TryGetValue(key, out diagnosticHandler))
			{
				return this.BuildDiagnosticsErrorNode("Unknown command");
			}
			XElement result;
			try
			{
				result = diagnosticHandler(parameters, remainingArgs);
			}
			catch (Exception ex)
			{
				if (Util.ShouldRethrowException(ex))
				{
					throw;
				}
				ExTraceGlobals.OperatorDiagnosticsTracer.TraceError<Exception>((long)this.GetHashCode(), "Caught exception executing Diagnostics command: {0}", ex);
				result = this.BuildDiagnosticsErrorNode(ex.Message);
			}
			return result;
		}

		public OperatorDiagnostics GetDiagnosticsContext(string flowId)
		{
			OperatorDiagnostics result;
			lock (this.contexts)
			{
				if (OperatorDiagnosticsFactory.diagnosticsEndpointEnabled && this.contexts.Count == 0)
				{
					this.RegisterDiagnosticsEndpoint();
				}
				OperatorDiagnosticsFactory.ContextAndRefCount contextAndRefCount;
				if (!this.contexts.TryGetValue(flowId, out contextAndRefCount))
				{
					contextAndRefCount = new OperatorDiagnosticsFactory.ContextAndRefCount(flowId, OperatorDiagnosticsFactory.FailedItemsLogDefaults);
					this.contexts.Add(flowId, contextAndRefCount);
				}
				contextAndRefCount.RefCount++;
				result = contextAndRefCount;
			}
			return result;
		}

		public void ReleaseDiagnosticsContext(OperatorDiagnostics context)
		{
			lock (this.contexts)
			{
				OperatorDiagnosticsFactory.ContextAndRefCount contextAndRefCount;
				if (!this.contexts.TryGetValue(context.FlowIdentifier, out contextAndRefCount))
				{
					throw new InvalidOperationException("context not found");
				}
				if (--contextAndRefCount.RefCount == 0)
				{
					this.contexts.Remove(context.FlowIdentifier);
				}
			}
		}

		private static void OnDomainUnload(object source, EventArgs args)
		{
			ProcessAccessManager.UnregisterComponent(OperatorDiagnosticsFactory.Instance);
			ProcessAccessManager.UnregisterComponent(SettingOverrideSync.Instance);
			SettingOverrideSync.Instance.Stop();
		}

		private void RegisterDiagnosticsEndpoint()
		{
			ProcessAccessManager.RegisterComponent(OperatorDiagnosticsFactory.Instance);
			AppDomain.CurrentDomain.DomainUnload += OperatorDiagnosticsFactory.OnDomainUnload;
			SettingOverrideSync.Instance.Start(true);
			ProcessAccessManager.RegisterComponent(SettingOverrideSync.Instance);
		}

		private XElement GetStatus(DiagnosableParameters parameters, string remainingArgs)
		{
			if (!string.IsNullOrEmpty(remainingArgs))
			{
				return this.BuildDiagnosticsErrorNode("Invalid arguments");
			}
			return this.GetStatusFromAllContexts(false);
		}

		private XElement GetBreadcrumbs(DiagnosableParameters parameters, string remainingArgs)
		{
			if (!string.IsNullOrEmpty(remainingArgs))
			{
				return this.BuildDiagnosticsErrorNode("Invalid arguments");
			}
			return this.GetStatusFromAllContexts(true);
		}

		private XElement GetStatusFromAllContexts(bool verboseBreadcrumbs)
		{
			XElement result;
			lock (this.contexts)
			{
				List<OperatorDiagnostics> list = new List<OperatorDiagnostics>(this.contexts.Count);
				foreach (OperatorDiagnosticsFactory.ContextAndRefCount item in this.contexts.Values)
				{
					list.Add(item);
				}
				list.Sort();
				XElement xelement = new XElement("FeedingGroups");
				XElement xelement2 = null;
				OperatorDiagnosticsFactory.ContextAndRefCount contextAndRefCount = null;
				foreach (OperatorDiagnostics operatorDiagnostics in list)
				{
					OperatorDiagnosticsFactory.ContextAndRefCount contextAndRefCount2 = (OperatorDiagnosticsFactory.ContextAndRefCount)operatorDiagnostics;
					if (contextAndRefCount == null || contextAndRefCount.InstanceName != contextAndRefCount2.InstanceName || contextAndRefCount.InstanceGuid != contextAndRefCount2.InstanceGuid)
					{
						XElement xelement3 = new XElement("FeedingGroup");
						xelement.Add(xelement3);
						xelement3.Add(new XElement("InstanceName", contextAndRefCount2.InstanceName ?? contextAndRefCount2.InstanceGuid.ToString()));
						xelement3.Add(new XElement("InstanceGuid", contextAndRefCount2.InstanceGuid));
						xelement2 = new XElement("Sessions");
						xelement3.Add(xelement2);
						contextAndRefCount = contextAndRefCount2;
					}
					xelement2.Add(contextAndRefCount2.GetBreadcrumbs(verboseBreadcrumbs));
				}
				result = xelement;
			}
			return result;
		}

		private XElement BuildDiagnosticsErrorNode(string reason)
		{
			ExTraceGlobals.OperatorDiagnosticsTracer.TraceError<string>((long)this.GetHashCode(), "Error executing Diagnostics command: {0}", reason);
			return new XElement("Error", reason);
		}

		private static OperatorDiagnosticsFactory instance = new OperatorDiagnosticsFactory();

		private static bool diagnosticsEndpointEnabled;

		private static DiagnosticsLogConfig.LogDefaults failedItemsLogDefaults;

		private static DiagnosticsLogConfig.LogDefaults languageDetectionLogDefaults;

		private readonly Dictionary<string, OperatorDiagnosticsFactory.ContextAndRefCount> contexts = new Dictionary<string, OperatorDiagnosticsFactory.ContextAndRefCount>();

		private readonly Dictionary<string, OperatorDiagnosticsFactory.DiagnosticHandler> diagnosticCommands;

		private delegate XElement DiagnosticHandler(DiagnosableParameters parameters, string remainingArgs);

		private class ContextAndRefCount : OperatorDiagnostics
		{
			public ContextAndRefCount(string flowIdentifier, DiagnosticsLogConfig.LogDefaults logDefaults) : base(flowIdentifier, logDefaults)
			{
			}

			public int RefCount { get; set; }
		}
	}
}
