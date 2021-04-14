using System;
using System.IO;
using System.Security;
using Microsoft.Exchange.Diagnostics.Components.Assistants;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Win32;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class Configuration
	{
		public static TimeSpan EventPollingInterval { get; private set; }

		public static TimeSpan ActiveWatermarksSaveInterval { get; private set; }

		public static TimeSpan IdleWatermarksSaveInterval { get; private set; }

		public static TimeSpan WatermarkCleanupInterval { get; private set; }

		public static int MaxThreadsForAllTimeBasedAssistants { get; private set; }

		public static int MaxThreadsPerTimeBasedAssistantType { get; private set; }

		public static TimeSpan HangDetectionTimeout { get; private set; }

		public static TimeSpan HangDetectionPeriod { get; private set; }

		public static int MaximumEventQueueSize { get; private set; }

		public static bool MemoryMonitorEnabled { get; private set; }

		public static int MemoryBarrierNumberOfSamples { get; private set; }

		public static TimeSpan MemoryBarrierSamplingInterval { get; private set; }

		public static TimeSpan MemoryBarrierSamplingDelay { get; private set; }

		public static long MemoryBarrierPrivateBytesUsageLimit { get; private set; }

		public static TimeSpan WorkCycleUpdatePeriod { get; private set; }

		public static TimeSpan BatchDuration { get; private set; }

		public static string ServiceRegistryKeyPath { get; private set; }

		public static string ParametersRegistryKeyPath { get; private set; }

		public static string LocalMachineParametersRegistryKeyPath { get; private set; }

		public static void Initialize(string serviceName)
		{
			Configuration.ServiceRegistryKeyPath = Path.Combine("System\\CurrentControlSet\\Services", serviceName);
			Configuration.ParametersRegistryKeyPath = Path.Combine(Configuration.ServiceRegistryKeyPath, "Parameters");
			Configuration.LocalMachineParametersRegistryKeyPath = Path.Combine("HKEY_LOCAL_MACHINE", Configuration.ParametersRegistryKeyPath);
			ExTraceGlobals.DatabaseManagerTracer.TraceDebug<string>(0L, "Configuration: Parameters Registry Key Path = {0}", Configuration.ParametersRegistryKeyPath);
			VariantConfiguration.UpdateCommitted += delegate(object sender, UpdateCommittedEventArgs args)
			{
				Configuration.Update();
			};
			Configuration.Update();
		}

		internal static void InitializeForTest()
		{
			Configuration.ServiceRegistryKeyPath = string.Empty;
			Configuration.ParametersRegistryKeyPath = string.Empty;
			Configuration.LocalMachineParametersRegistryKeyPath = string.Empty;
		}

		private static void Update()
		{
			IMailboxAssistantServiceSettings mailboxAssistantService = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).MailboxAssistants.MailboxAssistantService;
			Configuration.EventPollingInterval = Configuration.RegistryConfiguration.EventPollingIntervalMsec.Value.GetValueOrDefault(mailboxAssistantService.EventPollingInterval);
			Configuration.ActiveWatermarksSaveInterval = Configuration.RegistryConfiguration.ActiveWatermarksSaveInterval.Value.GetValueOrDefault(mailboxAssistantService.ActiveWatermarksSaveInterval);
			Configuration.IdleWatermarksSaveInterval = Configuration.RegistryConfiguration.IdleWatermarksSaveInterval.Value.GetValueOrDefault(mailboxAssistantService.IdleWatermarksSaveInterval);
			Configuration.WatermarkCleanupInterval = Configuration.RegistryConfiguration.WatermarkCleanupInterval.Value.GetValueOrDefault(mailboxAssistantService.WatermarkCleanupInterval);
			Configuration.MaxThreadsForAllTimeBasedAssistants = Configuration.RegistryConfiguration.MaxThreadsForAllTimeBasedAssistants.Value.GetValueOrDefault(mailboxAssistantService.MaxThreadsForAllTimeBasedAssistants);
			Configuration.MaxThreadsPerTimeBasedAssistantType = Configuration.RegistryConfiguration.MaxThreadsPerTimeBasedAssistantType.Value.GetValueOrDefault(mailboxAssistantService.MaxThreadsPerTimeBasedAssistantType);
			Configuration.HangDetectionTimeout = Configuration.RegistryConfiguration.HangDetectionTimeout.Value.GetValueOrDefault(mailboxAssistantService.HangDetectionTimeout);
			Configuration.HangDetectionPeriod = Configuration.RegistryConfiguration.HangDetectionPeriod.Value.GetValueOrDefault(mailboxAssistantService.HangDetectionPeriod);
			Configuration.MaximumEventQueueSize = Configuration.RegistryConfiguration.MaximumEventQueueSize.Value.GetValueOrDefault(mailboxAssistantService.MaximumEventQueueSize);
			Configuration.MemoryMonitorEnabled = mailboxAssistantService.MemoryMonitorEnabled;
			Configuration.MemoryBarrierNumberOfSamples = Configuration.RegistryConfiguration.MemoryBarrierNumberOfSamples.Value.GetValueOrDefault(mailboxAssistantService.MemoryBarrierNumberOfSamples);
			Configuration.MemoryBarrierSamplingInterval = Configuration.RegistryConfiguration.MemoryBarrierSamplingInterval.Value.GetValueOrDefault(mailboxAssistantService.MemoryBarrierSamplingInterval);
			Configuration.MemoryBarrierSamplingDelay = Configuration.RegistryConfiguration.MemoryBarrierSamplingDelay.Value.GetValueOrDefault(mailboxAssistantService.MemoryBarrierSamplingDelay);
			Configuration.MemoryBarrierPrivateBytesUsageLimit = Configuration.RegistryConfiguration.MemoryBarrierPrivateBytesUsageLimit.Value.GetValueOrDefault(mailboxAssistantService.MemoryBarrierPrivateBytesUsageLimit / 1024L / 1024L);
			Configuration.WorkCycleUpdatePeriod = Configuration.RegistryConfiguration.WorkCycleUpdatePeriod.Value.GetValueOrDefault(mailboxAssistantService.WorkCycleUpdatePeriod);
			Configuration.BatchDuration = mailboxAssistantService.BatchDuration;
		}

		private const string ServicesRootRegistryKeyPath = "System\\CurrentControlSet\\Services";

		private static class RegistryConfiguration
		{
			public static readonly Configuration.ConfigurationProperty<TimeSpan> EventPollingIntervalMsec = new Configuration.ConfigurationProperty<TimeSpan>("EventPollingIntervalMsec", (object value) => TimeSpan.FromMilliseconds((double)((int)value)));

			public static readonly Configuration.ConfigurationProperty<TimeSpan> ActiveWatermarksSaveInterval = new Configuration.ConfigurationProperty<TimeSpan>("ActiveWatermarksSaveInterval", (object value) => TimeSpan.FromSeconds((double)((int)value)));

			public static readonly Configuration.ConfigurationProperty<TimeSpan> IdleWatermarksSaveInterval = new Configuration.ConfigurationProperty<TimeSpan>("IdleWatermarksSaveInterval", (object value) => TimeSpan.FromMinutes((double)((int)value)));

			public static readonly Configuration.ConfigurationProperty<TimeSpan> WatermarkCleanupInterval = new Configuration.ConfigurationProperty<TimeSpan>("WatermarkCleanupInterval", (object value) => TimeSpan.FromMinutes((double)((int)value)));

			public static readonly Configuration.ConfigurationProperty<int> MaxThreadsForAllTimeBasedAssistants = new Configuration.ConfigurationProperty<int>("MaxThreadsForAllTimeBasedAssistants");

			public static readonly Configuration.ConfigurationProperty<int> MaxThreadsPerTimeBasedAssistantType = new Configuration.ConfigurationProperty<int>("MaxThreadsPerTimeBasedAssistantType");

			public static readonly Configuration.ConfigurationProperty<TimeSpan> HangDetectionTimeout = new Configuration.ConfigurationProperty<TimeSpan>("HangDetectionTimeout", (object value) => TimeSpan.FromMinutes((double)((int)value)));

			public static readonly Configuration.ConfigurationProperty<TimeSpan> HangDetectionPeriod = new Configuration.ConfigurationProperty<TimeSpan>("HangDetectionPeriod", (object value) => TimeSpan.FromMinutes((double)((int)value)));

			public static readonly Configuration.ConfigurationProperty<int> MaximumEventQueueSize = new Configuration.ConfigurationProperty<int>("MaximumEventQueueSize");

			public static readonly Configuration.ConfigurationProperty<int> MemoryBarrierNumberOfSamples = new Configuration.ConfigurationProperty<int>("MemoryBarrierNumberOfSamples");

			public static readonly Configuration.ConfigurationProperty<TimeSpan> MemoryBarrierSamplingInterval = new Configuration.ConfigurationProperty<TimeSpan>("MemoryBarrierSamplingInterval", (object value) => TimeSpan.FromSeconds((double)((int)value)));

			public static readonly Configuration.ConfigurationProperty<TimeSpan> MemoryBarrierSamplingDelay = new Configuration.ConfigurationProperty<TimeSpan>("MemoryBarrierSamplingDelay", (object value) => TimeSpan.FromSeconds((double)((int)value)));

			public static readonly Configuration.ConfigurationProperty<long> MemoryBarrierPrivateBytesUsageLimit = new Configuration.ConfigurationProperty<long>("MemoryBarrierPrivateBytesUsageLimit");

			public static readonly Configuration.ConfigurationProperty<TimeSpan> WorkCycleUpdatePeriod = new Configuration.ConfigurationProperty<TimeSpan>("WorkCycleUpdatePeriod", (object value) => TimeSpan.FromSeconds((double)((int)value)));
		}

		internal class ConfigurationProperty<PropertyType> where PropertyType : struct
		{
			public ConfigurationProperty(string name) : this(name, null)
			{
			}

			public ConfigurationProperty(string name, Configuration.ConfigurationProperty<PropertyType>.ConvertionDelegate convertion)
			{
				this.name = name;
				this.value = null;
				this.convertion = convertion;
			}

			public PropertyType? Value
			{
				get
				{
					if (!this.initialized)
					{
						object obj = Configuration.ConfigurationProperty<PropertyType>.ReadValue(this.name);
						if (obj != null && this.convertion != null)
						{
							obj = this.convertion(obj);
						}
						this.value = obj;
						this.initialized = true;
					}
					return (PropertyType?)this.value;
				}
			}

			private static object ReadValue(string valueName)
			{
				object obj = null;
				Exception ex = null;
				try
				{
					obj = Registry.GetValue(Configuration.LocalMachineParametersRegistryKeyPath, valueName, null);
				}
				catch (SecurityException ex2)
				{
					ex = ex2;
				}
				catch (IOException ex3)
				{
					ex = ex3;
				}
				catch (ArgumentException ex4)
				{
					ex = ex4;
				}
				if (ex != null)
				{
					ExTraceGlobals.DatabaseManagerTracer.TraceDebug<string, Exception>(0L, "Configuration: Exception while reading property {0}: {1}", valueName, ex);
				}
				ExTraceGlobals.DatabaseManagerTracer.TraceDebug<string, object>(0L, "Configuration: {0} = {1}", valueName, obj);
				return obj;
			}

			private bool initialized;

			private string name;

			private object value;

			private Configuration.ConfigurationProperty<PropertyType>.ConvertionDelegate convertion;

			public delegate PropertyType ConvertionDelegate(object value);
		}
	}
}
