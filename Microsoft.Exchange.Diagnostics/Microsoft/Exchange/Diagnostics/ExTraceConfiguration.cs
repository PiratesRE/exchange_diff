using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics
{
	internal sealed class ExTraceConfiguration
	{
		public event Action OnConfigurationChange;

		public static ExTraceConfiguration Instance
		{
			get
			{
				return ExTraceConfiguration.instanceConfig;
			}
		}

		public static HashSet<uint> DisabledLids
		{
			get
			{
				return ExTraceConfiguration.disabledLids;
			}
			set
			{
				ExTraceConfiguration.disabledLids = value;
			}
		}

		public static HashSet<Guid> DisableAllTraces
		{
			get
			{
				return ExTraceConfiguration.disableAllTraces;
			}
			set
			{
				ExTraceConfiguration.disableAllTraces = value;
			}
		}

		public int Version
		{
			get
			{
				return this.version;
			}
		}

		internal bool PerThreadTracingConfigured
		{
			get
			{
				return this.perThreadTracingConfigured;
			}
		}

		internal bool InMemoryTracingEnabled
		{
			get
			{
				return this.inMemoryTracingEnabled;
			}
		}

		internal bool ConsoleTracingEnabled
		{
			get
			{
				return this.consoleTracingEnabled;
			}
		}

		internal bool SystemDiagnosticsTracingEnabled
		{
			get
			{
				return this.systemDiagnosticsTracingEnabled;
			}
		}

		public bool IsEnabled(TraceType value)
		{
			return this.typeFlags[(int)value];
		}

		public BitArray EnabledTypesArray()
		{
			return this.typeFlags;
		}

		public BitArray EnabledTagArray(Guid componentGuid)
		{
			return this.componentDictionary[componentGuid];
		}

		public BitArray EnabledInMemoryTagArray(Guid componentGuid)
		{
			return this.inMemoryComponentDictionary[componentGuid];
		}

		public BitArray PerThreadModeTagArray(Guid componentGuid)
		{
			return this.perThreadModeComponentDictionary[componentGuid];
		}

		public Dictionary<string, List<string>> CustomParameters
		{
			get
			{
				return this.customParameters;
			}
		}

		internal FaultInjectionConfig FaultInjectionConfiguration
		{
			get
			{
				return this.faultInjectionConfiguration;
			}
		}

		internal ExceptionInjection ExceptionInjection
		{
			get
			{
				return this.exceptionInjection;
			}
		}

		internal ComponentInjectionCallback ComponentInjection
		{
			get
			{
				return this.componentInjection;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentException("Component identifier is null.");
				}
				lock (this.locker)
				{
					this.componentInjection = value;
				}
			}
		}

		private static Dictionary<Guid, BitArray> DeepCopyComponentDictionary()
		{
			Dictionary<Guid, BitArray> dictionary = new Dictionary<Guid, BitArray>(ComponentDictionary.InnerDictionary);
			foreach (Guid key in ComponentDictionary.InnerDictionary.Keys)
			{
				dictionary[key] = new BitArray(dictionary[key].Length);
			}
			return dictionary;
		}

		private ExTraceConfiguration()
		{
			ConfigFiles.Trace.FileHandler.Changed += this.TraceConfigFileChangeHandler;
			ConfigFiles.InMemory.FileHandler.Changed += this.InMemoryTraceConfigFileChangeHandler;
			ConfigFiles.FaultInjection.FileHandler.Changed += this.FaultInjectionConfigFileChangeHandler;
			lock (this.locker)
			{
				this.TraceConfigUpdate();
				this.InMemoryTraceConfigUpdate();
				this.FaultInjectionConfigUpdate();
				TraceException.Setup();
			}
		}

		public void EnableInMemoryTracing(Guid componentGuid, bool enable)
		{
			lock (this.locker)
			{
				BitArray bitArray;
				if (!this.inMemoryComponentDictionary.TryGetValue(componentGuid, out bitArray))
				{
					throw new ArgumentException("Component " + componentGuid + " does not exist");
				}
				bitArray.SetAll(enable);
				this.typeFlags.SetAll(enable);
				this.typeFlags[1] = false;
				this.typeFlags[7] = false;
				this.inMemoryTracingEnabled = true;
				this.version++;
			}
			this.InvokeOnConfigurationChange();
			InternalBypassTrace.TracingConfigurationTracer.TraceDebug(0, (long)this.GetHashCode(), "In Memory tracing {0} for all tags in component {1}.", new object[]
			{
				this.inMemoryTracingEnabled ? "enabled" : "disabled",
				componentGuid
			});
		}

		private void TraceConfigFileChangeHandler()
		{
			lock (this.locker)
			{
				this.TraceConfigUpdate();
			}
			this.InvokeOnConfigurationChange();
		}

		private void InMemoryTraceConfigFileChangeHandler()
		{
			lock (this.locker)
			{
				this.InMemoryTraceConfigUpdate();
			}
			this.InvokeOnConfigurationChange();
		}

		private void FaultInjectionConfigFileChangeHandler()
		{
			lock (this.locker)
			{
				this.FaultInjectionConfigUpdate();
			}
			this.InvokeOnConfigurationChange();
		}

		private void InvokeOnConfigurationChange()
		{
			Action onConfigurationChange = this.OnConfigurationChange;
			if (onConfigurationChange != null)
			{
				onConfigurationChange();
			}
		}

		private void TraceConfigUpdate()
		{
			ConfigurationDocument configurationDocument = ConfigurationDocument.LoadFromFile(ConfigFiles.Trace.ConfigFilePath);
			this.UpdateTrace(configurationDocument);
			InternalBypassTrace.TracingConfigurationTracer.TraceDebug(39471, 0L, "New tracing configuration took effect", new object[0]);
			TraceConfigSync.Signal(configurationDocument.FileContentHash, this.ComponentInjection(), InternalBypassTrace.TracingConfigurationTracer);
		}

		private void InMemoryTraceConfigUpdate()
		{
			ConfigurationDocument configurationDocument = ConfigurationDocument.LoadFromFile(ConfigFiles.InMemory.ConfigFilePath);
			this.UpdateInMemory(configurationDocument);
			InternalBypassTrace.TracingConfigurationTracer.TraceDebug(49839, 0L, "New in-memory tracing configuration took effect", new object[0]);
			TraceConfigSync.Signal(configurationDocument.FileContentHash, this.ComponentInjection(), InternalBypassTrace.TracingConfigurationTracer);
		}

		private void FaultInjectionConfigUpdate()
		{
			ConfigurationDocument configurationDocument = ConfigurationDocument.LoadFaultInjectionFromFile(ConfigFiles.FaultInjection.ConfigFilePath);
			this.faultInjectionConfiguration = configurationDocument.FaultInjectionConfig;
			InternalBypassTrace.FaultInjectionConfigurationTracer.TraceDebug(64047, 0L, "New FI configuration took effect", new object[0]);
			TraceConfigSync.Signal(configurationDocument.FileContentHash, this.ComponentInjection(), InternalBypassTrace.FaultInjectionConfigurationTracer);
		}

		private void UpdateTrace(ConfigurationDocument traceConfigDoc)
		{
			List<TraceComponentInfo> enabledComponentsFromFile;
			List<TraceComponentInfo> enabledComponentsFromFile2;
			if (traceConfigDoc.FilteredTracingEnabled)
			{
				enabledComponentsFromFile = traceConfigDoc.BypassFilterEnabledComponentsList;
				enabledComponentsFromFile2 = traceConfigDoc.EnabledComponentsList;
			}
			else
			{
				enabledComponentsFromFile = traceConfigDoc.EnabledComponentsList;
				enabledComponentsFromFile2 = new List<TraceComponentInfo>();
			}
			this.UpdateComponentsState(enabledComponentsFromFile, this.componentDictionary);
			this.UpdateComponentsState(enabledComponentsFromFile2, this.perThreadModeComponentDictionary);
			traceConfigDoc.GetEnabledTypes(this.typeFlags, false);
			this.perThreadTracingConfigured = traceConfigDoc.FilteredTracingEnabled;
			this.customParameters = traceConfigDoc.CustomParameters;
			this.consoleTracingEnabled = traceConfigDoc.ConsoleTracingEnabled;
			this.systemDiagnosticsTracingEnabled = traceConfigDoc.SystemDiagnosticsTracingEnabled;
			bool anyExchangeTracingProvidersEnabled = ETWTrace.IsEnabled || this.InMemoryTracingEnabled || this.ConsoleTracingEnabled || this.SystemDiagnosticsTracingEnabled || this.FaultInjectionConfiguration.Count > 0;
			SystemTraceControl.Update(this.componentDictionary, this.EnabledTypesArray(), anyExchangeTracingProvidersEnabled);
			this.version++;
		}

		private void UpdateInMemory(ConfigurationDocument inMemoryTraceConfigDoc)
		{
			List<TraceComponentInfo> list;
			if (inMemoryTraceConfigDoc.FilteredTracingEnabled)
			{
				list = inMemoryTraceConfigDoc.BypassFilterEnabledComponentsList;
			}
			else
			{
				list = inMemoryTraceConfigDoc.EnabledComponentsList;
			}
			this.UpdateComponentsState(list, this.inMemoryComponentDictionary);
			inMemoryTraceConfigDoc.GetEnabledTypes(this.typeFlags, true);
			if (list != null && list.Count > 0)
			{
				this.inMemoryTracingEnabled = true;
			}
			else
			{
				this.inMemoryTracingEnabled = false;
			}
			InternalBypassTrace.TracingConfigurationTracer.TraceDebug(0, (long)this.GetHashCode(), "In Memory tracing is {0}", new object[]
			{
				this.inMemoryTracingEnabled ? "enabled" : "disabled"
			});
			this.version++;
		}

		private void UpdateComponentsState(List<TraceComponentInfo> enabledComponentsFromFile, Dictionary<Guid, BitArray> componentConfigurationInMemory)
		{
			HashSet<Guid> hashSet = new HashSet<Guid>();
			foreach (TraceComponentInfo traceComponentInfo in enabledComponentsFromFile)
			{
				BitArray bitArray = componentConfigurationInMemory[traceComponentInfo.ComponentGuid];
				BitArray bitArray2 = new BitArray(bitArray.Length);
				bitArray2.SetAll(false);
				TraceTagInfo[] tagInfoList = traceComponentInfo.TagInfoList;
				foreach (TraceTagInfo traceTagInfo in tagInfoList)
				{
					bitArray2[traceTagInfo.NumericValue] = true;
				}
				for (int j = 0; j < bitArray2.Length; j++)
				{
					bitArray[j] = bitArray2[j];
				}
				hashSet.Add(traceComponentInfo.ComponentGuid);
			}
			foreach (Guid guid in this.componentDictionary.Keys)
			{
				if (!hashSet.Contains(guid))
				{
					BitArray bitArray3 = componentConfigurationInMemory[guid];
					bitArray3.SetAll(false);
				}
			}
		}

		private static readonly ExTraceConfiguration instanceConfig = new ExTraceConfiguration();

		[ThreadStatic]
		private static HashSet<uint> disabledLids;

		[ThreadStatic]
		private static HashSet<Guid> disableAllTraces;

		private readonly BitArray typeFlags = new BitArray(ConfigurationDocument.TraceTypesCount + 1);

		private readonly Dictionary<Guid, BitArray> componentDictionary = ComponentDictionary.InnerDictionary;

		private readonly Dictionary<Guid, BitArray> inMemoryComponentDictionary = ExTraceConfiguration.DeepCopyComponentDictionary();

		private bool perThreadTracingConfigured;

		private bool inMemoryTracingEnabled;

		private bool consoleTracingEnabled;

		private bool systemDiagnosticsTracingEnabled;

		private Dictionary<string, List<string>> customParameters;

		private Dictionary<Guid, BitArray> perThreadModeComponentDictionary = ExTraceConfiguration.DeepCopyComponentDictionary();

		private FaultInjectionConfig faultInjectionConfiguration = new FaultInjectionConfig();

		private ExceptionInjection exceptionInjection = new ExceptionInjection();

		private ComponentInjectionCallback componentInjection = () => string.Empty;

		private volatile int version;

		private object locker = new object();
	}
}
