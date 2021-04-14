using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.Directory.TopologyService;
using Microsoft.Exchange.Diagnostics.FaultInjection;
using Microsoft.Exchange.Net;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Directory
{
	internal static class Globals
	{
		internal static void SetMdbConcurrencyControlTestHook(Globals.MdbStartConcurrencyControlTestHook startConcurrency, Globals.MdbEndConcurrencyControlTestHook endConcurrency)
		{
			Globals.StartConcurrencyTestHook = startConcurrency;
			Globals.EndConcurrencyTestHook = endConcurrency;
		}

		static Globals()
		{
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				Globals.processName = currentProcess.MainModule.ModuleName;
				Globals.processId = currentProcess.Id;
				Globals.processProcessName = currentProcess.ProcessName;
				Globals.isExchangeTestExecutable = Globals.CheckExchangeTestExecutables(Globals.processProcessName);
			}
			Globals.SetProcessNameAppName();
			Globals.logger = new ExEventLog(Microsoft.Exchange.Diagnostics.Components.Data.Directory.ExTraceGlobals.TopologyProviderTracer.Category, "MSExchange ADAccess");
			Globals.logger.SetEventPeriod(Globals.GetIntValueFromRegistry("LogPeriod", 900, 0));
			Globals.isDatacenterFlag = Datacenter.IsMultiTenancyEnabled();
			Globals.isMicrosoftHostedOnly = Datacenter.IsMicrosoftHostedOnly(true);
			Globals.testPassTypeValue = Globals.GetValueFromRegistry<string>("SOFTWARE\\Microsoft\\Exchange_Test\\v15", "TestPassType", string.Empty, Microsoft.Exchange.Diagnostics.Components.Data.Directory.ExTraceGlobals.ADTopologyTracer);
			Globals.forceLdapLatency = (Globals.GetValueFromRegistry<int>("SOFTWARE\\Microsoft\\Exchange_Test\\v15", "ForceLdapLatency", 0, Microsoft.Exchange.Diagnostics.Components.Data.Directory.ExTraceGlobals.ADTopologyTracer) > 0);
			Globals.TenantInfoCacheTime = Globals.GetValueFromRegistry<int>("SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess", "TenantInfoCacheTime", 1800, Microsoft.Exchange.Diagnostics.Components.Data.Directory.ExTraceGlobals.ADTopologyTracer);
			Globals.RecipientInfoCacheTime = Globals.GetValueFromRegistry<int>("SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess", "RecipientInfoCacheTime", 300, Microsoft.Exchange.Diagnostics.Components.Data.Directory.ExTraceGlobals.ADTopologyTracer);
			Globals.RecipientTokenGroupsCacheTime = Globals.GetValueFromRegistry<int>("SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess", "RecipientTokenGroupsCacheTime", 900, Microsoft.Exchange.Diagnostics.Components.Data.Directory.ExTraceGlobals.ADTopologyTracer);
			Globals.EnableNotification = (Globals.GetValueFromRegistry<int>("SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess", "EnableNotification", 1, Microsoft.Exchange.Diagnostics.Components.Data.Directory.ExTraceGlobals.ADTopologyTracer) == 0);
			Globals.LdapConnectionPoolSize = Globals.GetValueFromRegistry<int>("SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess", "LdapConnectionPoolSize", 5, Microsoft.Exchange.Diagnostics.Components.Data.Directory.ExTraceGlobals.ADTopologyTracer);
		}

		public static string ProcessName
		{
			get
			{
				return Globals.processName;
			}
		}

		public static int ProcessId
		{
			get
			{
				return Globals.processId;
			}
		}

		internal static bool IsVirtualMachine
		{
			get
			{
				if (Globals.isVirtualMachine == null)
				{
					Globals.isVirtualMachine = new bool?(Globals.DetectIfMachineIsVirtualMachine());
				}
				return Globals.isVirtualMachine.Value;
			}
		}

		internal static bool IsDatacenter
		{
			get
			{
				return Globals.isDatacenterFlag;
			}
		}

		internal static bool IsMicrosoftHostedOnly
		{
			get
			{
				return Globals.isMicrosoftHostedOnly;
			}
		}

		internal static string TestPassTypeValue
		{
			get
			{
				return Globals.testPassTypeValue;
			}
		}

		internal static bool ForceLdapLatency
		{
			get
			{
				return Globals.forceLdapLatency;
			}
		}

		internal static InstanceType ProcessInstanceType
		{
			get
			{
				return Globals.InstanceType;
			}
		}

		internal static string ProcessAppName
		{
			get
			{
				return Globals.CurrentAppName;
			}
		}

		internal static string CurrentAppName
		{
			get
			{
				return Globals.currentAppName;
			}
			private set
			{
				Globals.currentAppName = value;
				Globals.SetProcessNameAppName();
			}
		}

		internal static string ProcessNameAppName
		{
			get
			{
				return Globals.processNameAppName;
			}
		}

		internal static SecurityIdentifier LocalSystemSid
		{
			get
			{
				return Globals.localSystemSid;
			}
		}

		internal static SecurityIdentifier NetworkServiceSid
		{
			get
			{
				return Globals.networkServiceSid;
			}
		}

		internal static int GetIntValueFromRegistry(string valueName, int defaultValue, int tracingKey)
		{
			return Globals.GetIntValueFromRegistry("SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess", valueName, defaultValue, tracingKey);
		}

		internal static int GetIntValueFromRegistry(string registryPath, string valueName, int defaultValue, int tracingKey)
		{
			int result = defaultValue;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(registryPath, false))
				{
					if (registryKey != null)
					{
						object value = registryKey.GetValue(valueName, defaultValue);
						if (value is int)
						{
							int num = (int)value;
							if (num >= 1)
							{
								result = num;
								Microsoft.Exchange.Diagnostics.Components.Data.Directory.ExTraceGlobals.ADTopologyTracer.TraceDebug<string, int>((long)tracingKey, "Using {0} = {1} (from registry)", valueName, num);
							}
							else
							{
								Microsoft.Exchange.Diagnostics.Components.Data.Directory.ExTraceGlobals.ADTopologyTracer.TraceDebug<string, int, int>((long)tracingKey, "{0} has wrong value: {1}, using {2}.", valueName, num, defaultValue);
							}
						}
						else
						{
							Microsoft.Exchange.Diagnostics.Components.Data.Directory.ExTraceGlobals.ADTopologyTracer.TraceDebug<string, string, int>((long)tracingKey, "{0} has wrong type {1}, using {2}.", valueName, value.GetType().Name, defaultValue);
						}
					}
					else
					{
						Microsoft.Exchange.Diagnostics.Components.Data.Directory.ExTraceGlobals.ADTopologyTracer.TraceError<string, int>((long)tracingKey, "Opening registry key {0} failed, using {1}.", valueName, defaultValue);
					}
				}
			}
			catch (SecurityException ex)
			{
				Microsoft.Exchange.Diagnostics.Components.Data.Directory.ExTraceGlobals.ADTopologyTracer.TraceError<string>((long)tracingKey, "SecurityException: {0}", ex.Message);
			}
			catch (UnauthorizedAccessException ex2)
			{
				Microsoft.Exchange.Diagnostics.Components.Data.Directory.ExTraceGlobals.ADTopologyTracer.TraceError<string>((long)tracingKey, "UnauthorizedAccessException: {0}", ex2.Message);
			}
			return result;
		}

		internal static bool DetectIfMachineIsVirtualMachine()
		{
			try
			{
				using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("select * from win32_computersystem"))
				{
					using (ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get())
					{
						foreach (ManagementBaseObject managementBaseObject in managementObjectCollection)
						{
							ManagementObject managementObject = (ManagementObject)managementBaseObject;
							using (managementObject)
							{
								string text = (string)managementObject["Model"];
								if (text.Equals("Virtual Machine", StringComparison.InvariantCultureIgnoreCase))
								{
									return true;
								}
							}
						}
					}
				}
			}
			catch (Exception)
			{
			}
			return false;
		}

		internal static T GetEnumValueFromRegistry<T>(string registryPath, string valueName, T defaultValue, Microsoft.Exchange.Diagnostics.Trace tracer) where T : struct
		{
			string valueFromRegistry = Globals.GetValueFromRegistry<string>(registryPath, valueName, defaultValue.ToString(), tracer);
			T result;
			if (Enum.TryParse<T>(valueFromRegistry, out result))
			{
				tracer.TraceDebug<string, string>(0L, "Using enum {0} = {1} (from registry)", valueName, valueFromRegistry);
				return result;
			}
			tracer.TraceError<string, string, T>(0L, "{0} has wrong enum value: {1}, using {2}.", valueName, valueFromRegistry, defaultValue);
			return defaultValue;
		}

		internal static T GetValueFromRegistry<T>(string registryPath, string valueName, T defaultValue, Microsoft.Exchange.Diagnostics.Trace tracer)
		{
			T result = defaultValue;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(registryPath, false))
				{
					if (registryKey != null)
					{
						object value = registryKey.GetValue(valueName, defaultValue);
						if (value is T)
						{
							tracer.TraceDebug<string, object>(0L, "Using {0} = {1} (from registry)", valueName, value);
							result = (T)((object)value);
						}
						else
						{
							tracer.TraceError<string, string, T>(0L, "{0} has wrong type {1}, using {2}.", valueName, (value == null) ? "<undefined>" : value.GetType().Name, defaultValue);
						}
					}
					else
					{
						tracer.TraceError<string, T>(0L, "Opening registry key {0} failed, using {1}.", valueName, defaultValue);
					}
				}
			}
			catch (SecurityException ex)
			{
				tracer.TraceError<string>(0L, "SecurityException: {0}", ex.Message);
			}
			catch (UnauthorizedAccessException ex2)
			{
				tracer.TraceError<string>(0L, "UnauthorizedAccessException: {0}", ex2.Message);
			}
			return result;
		}

		internal static string[] GetMultiStringValueFromRegistry(string valueName, int tracingKey)
		{
			string[] result = new string[0];
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess", false))
				{
					if (registryKey != null)
					{
						object value = registryKey.GetValue(valueName);
						if (value == null)
						{
							return result;
						}
						if (value is string[])
						{
							return (string[])value;
						}
						Microsoft.Exchange.Diagnostics.Components.Data.Directory.ExTraceGlobals.ADTopologyTracer.TraceDebug<string, string>((long)tracingKey, "{0} has wrong type {1}.", valueName, value.GetType().Name);
					}
					else
					{
						Microsoft.Exchange.Diagnostics.Components.Data.Directory.ExTraceGlobals.ADTopologyTracer.TraceError<string>((long)tracingKey, "Opening registry key {0} failed.", valueName);
					}
				}
			}
			catch (SecurityException ex)
			{
				Microsoft.Exchange.Diagnostics.Components.Data.Directory.ExTraceGlobals.ADTopologyTracer.TraceError<string>((long)tracingKey, "SecurityException: {0}", ex.Message);
			}
			catch (UnauthorizedAccessException ex2)
			{
				Microsoft.Exchange.Diagnostics.Components.Data.Directory.ExTraceGlobals.ADTopologyTracer.TraceError<string>((long)tracingKey, "UnauthorizedAccessException: {0}", ex2.Message);
			}
			return result;
		}

		internal static bool LogEvent(ExEventLog.EventTuple tuple, string periodicKey, params object[] messageArgs)
		{
			bool result;
			try
			{
				object[] array = new object[messageArgs.Length + 2];
				if (InstanceType.Multiple == Globals.ProcessInstanceType)
				{
					array[0] = Globals.processNameAppName;
				}
				else
				{
					array[0] = Globals.processName;
				}
				array[1] = Globals.processId;
				messageArgs.CopyTo(array, 2);
				result = Globals.logger.LogEvent(tuple, periodicKey, array);
			}
			catch (Win32Exception ex)
			{
				if (ex.ErrorCode == 5)
				{
					throw new SecurityException(DirectoryStrings.AccessDeniedToEventLog, ex);
				}
				throw;
			}
			return result;
		}

		internal static bool IsEventCategoryEnabled(short category, ExEventLog.EventLevel level)
		{
			return Globals.logger.IsEventCategoryEnabled(category, level);
		}

		internal static bool LogExchangeTopologyEvent(ExEventLog.EventTuple tuple, string periodicKey, params object[] messageArgs)
		{
			object[] array = new object[messageArgs.Length + 2];
			array[0] = Globals.ProcessName;
			array[1] = Globals.ProcessId;
			messageArgs.CopyTo(array, 2);
			return Globals.exchangeTopologyLogger.LogEvent(tuple, periodicKey, array);
		}

		internal static void InitializeSinglePerfCounterInstance()
		{
			Globals.InitializePerfCounterInstance(null, false);
		}

		internal static void InitializeMultiPerfCounterInstance(string applicationName)
		{
			if (string.IsNullOrEmpty(applicationName))
			{
				throw new ArgumentNullException("applicationName");
			}
			if (applicationName.Length >= 16)
			{
				applicationName = applicationName.Substring(0, 15);
			}
			Globals.InitializePerfCounterInstance(applicationName, true);
		}

		internal static void InitializeUnknownPerfCounterInstance()
		{
			Globals.InitializePerfCounterInstance("UNKNOWN", true);
		}

		internal static void ReportNonTerminatingWatson(Exception exception)
		{
			ReportOptions options = ReportOptions.DoNotCollectDumps | ReportOptions.DeepStackTraceHash | ReportOptions.DoNotFreezeThreads;
			ExWatson.HandleException(new UnhandledExceptionEventArgs(exception, false), options);
		}

		internal static void LogBadNumber(string regKeyName, int minAllowed, int maxAllowed, int actual, int defaultValue)
		{
			if (string.IsNullOrEmpty(regKeyName))
			{
				throw new ArgumentNullException("regKeyName");
			}
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_REG_BAD_DWORD, null, new object[]
			{
				regKeyName,
				minAllowed,
				maxAllowed,
				actual,
				defaultValue
			});
		}

		internal static ulong GetTickDifference(int tickStart, int tickEnd)
		{
			return TickDiffer.GetTickDifference(tickStart, tickEnd);
		}

		internal static bool IsConsumerOrganization(OrganizationId organizationId)
		{
			return TemplateTenantConfiguration.IsTemplateTenant(organizationId);
		}

		private static bool IsPerfCounterInstanceInitialized(string applicationName, bool hasMultiInstance)
		{
			if (Globals.InstanceType == InstanceType.NotInitialized)
			{
				return false;
			}
			if (hasMultiInstance != (InstanceType.Multiple == Globals.InstanceType))
			{
				throw new InvalidOperationException(DirectoryStrings.ErrorOneProcessInitializedAsBothSingleAndMultiple);
			}
			if (Globals.CurrentAppName != applicationName)
			{
				if ((!(Globals.CurrentAppName == "EMC") || !(applicationName == "EMS")) && (!(Globals.CurrentAppName == "OWA") || !(applicationName == "EMS")) && (!(Globals.CurrentAppName == "EDS") || !(applicationName == "EMS")) && (!(Globals.CurrentAppName == "MSExchMbxAsst") || !(applicationName == "EMS")) && (!(Globals.CurrentAppName == "MSExchMbxRepl") || !(applicationName == "EMS")) && (!(Globals.CurrentAppName == "MSExchMigWkfl") || !(applicationName == "EMS")) && (!(Globals.CurrentAppName == "LogSearchSvc") || !(applicationName == "EMS")) && (!(Globals.CurrentAppName == "LawEnfSearch") || !(applicationName == "EMS")) && (!(Globals.CurrentAppName == "CentralAdmin") || !(applicationName == "EMS")) && (!(Globals.CurrentAppName == "ExBPACmd") || !(applicationName == "EMS")) && (!(Globals.CurrentAppName == "ExHMWorker") || !(applicationName == "EMS")) && (!(Globals.CurrentAppName == "UNKNOWN") || !(applicationName == "EMS")) && (!(Globals.CurrentAppName == "UNKNOWN") || !(applicationName == "EMC")) && (!(Globals.CurrentAppName == "UNKNOWN") || !(applicationName == "ECP")) && !Globals.isExchangeTestExecutable)
				{
					string format = "The process '{0}' (PID = {1}) has been initialized twice as Multiple instance type in the way we don't understand: current App Name is '{2}' and to-be-set App Name is '{3}'.\r\n Reasons of this exception: 1. For any product executables, which will be shipped out of box, they should never use AD driver functionality without initializing its performance counter instance since we can NOT show a intuitive Perf Counter instance name here.\r\n 2. We don't need the validation for test assembly because we don't care for its performance counter naming; however, we definitely want to make sure it is under our control, so that we don't miss any out of box product DLLs/EXEs.\r\n What to do to avoid this exception: For any newly added Exchange test executables which utilize AD perf, please add the executable name of the process into exclusion list in Microsoft.Exchange.Data.Directory.Globals.CheckExchangeTestExecutables.";
					throw new InvalidOperationException(string.Format(format, new object[]
					{
						Globals.processProcessName ?? string.Empty,
						Globals.processId,
						Globals.CurrentAppName,
						applicationName
					}));
				}
				Microsoft.Exchange.Diagnostics.Components.Data.Directory.ExTraceGlobals.PerfCountersTracer.TraceDebug(0L, "The DSAccess Perf Counter of the process '{0}' (PID = {1}) has been initialized as <AppName:'{2}'> before; Its AppName will not be set as {3}.", new object[]
				{
					Globals.processProcessName ?? string.Empty,
					Globals.processId,
					Globals.CurrentAppName ?? string.Empty,
					applicationName ?? string.Empty
				});
			}
			else
			{
				Microsoft.Exchange.Diagnostics.Components.Data.Directory.ExTraceGlobals.PerfCountersTracer.TraceDebug<string, int, string>(0L, "The DSAccess Perf Counter of the process '{0}' (PID = {1}) has been initialized as <AppName:'{2}'> before; The operation is ignored.", Globals.processProcessName ?? string.Empty, Globals.processId, Globals.CurrentAppName);
			}
			return true;
		}

		private static void InitializePerfCounterInstance(string applicationName, bool hasMultiInstance)
		{
			Microsoft.Exchange.Diagnostics.Components.Data.Directory.ExTraceGlobals.FaultInjectionTracer.RegisterExceptionInjectionCallback(new ExceptionInjectionCallback(ADFaultInjectionUtils.DirectoryTracerCallback));
			Microsoft.Exchange.Diagnostics.Components.Directory.TopologyService.ExTraceGlobals.FaultInjectionTracer.RegisterExceptionInjectionCallback(new ExceptionInjectionCallback(ADFaultInjectionUtils.TopologyServiceTracerCallback));
			if (Globals.IsPerfCounterInstanceInitialized(applicationName, hasMultiInstance))
			{
				return;
			}
			lock (Globals.locker)
			{
				if (!Globals.IsPerfCounterInstanceInitialized(applicationName, hasMultiInstance))
				{
					if (applicationName == "UNKNOWN" && !Globals.isExchangeTestExecutable)
					{
						string format = "The process '{0}' (PID = {1}) has been initialized as Unknown Multiple instance type un-expectedly. \r\n Reasons of this exception: 1. For any product executables, which will be shipped out of box, they should never use AD driver functionality without initializing its performance counter instance since we can NOT show a intuitive Perf Counter instance name here.\r\n 2. We don't need the validation for test assembly because we don't care for its performance counter naming; however, we definitely want to make sure it is under our control, so that we don't miss any out of box product DLLs/EXEs.\r\n What to do to avoid this exception: For any newly added Exchange test executables which utilize AD perf, please add the executable name of the process into exclusion list in Microsoft.Exchange.Data.Directory.Globals.CheckExchangeTestExecutables.";
						throw new InvalidOperationException(string.Format(format, Globals.processProcessName ?? string.Empty, Globals.processId));
					}
					Microsoft.Exchange.Diagnostics.Components.Data.Directory.ExTraceGlobals.PerfCountersTracer.TraceDebug(0L, "Initializing the DSAccess Perf Counter of the process '{0}' (PID = {1}) as {2} with application name '{3}'.", new object[]
					{
						Globals.processProcessName ?? string.Empty,
						Globals.processId,
						hasMultiInstance ? InstanceType.Multiple : InstanceType.Single,
						applicationName ?? string.Empty
					});
					Globals.DSAccessPerfCounterInitializationWithRetry(Globals.processProcessName, applicationName, hasMultiInstance);
					if (hasMultiInstance)
					{
						Globals.InstanceType = InstanceType.Multiple;
						Globals.CurrentAppName = applicationName;
					}
					else
					{
						Globals.InstanceType = InstanceType.Single;
					}
				}
			}
		}

		private static void SetProcessNameAppName()
		{
			Globals.processNameAppName = Globals.processName + " (" + Globals.ProcessAppName + ")";
		}

		private static bool CheckExchangeTestExecutables(string processName)
		{
			return string.Equals(processName, "PerseusHarnessRuntime", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "PerseusHarnessRuntime.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "PerseusHarnessService", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "PerseusHarnessService.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "PerseusStudio", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "PerseusStudio.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "CPerseus", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "CPerseus.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "XSOExplorer", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "XSOExplorer.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "ContactsImporter.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "ContactsImporter", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "w3wp", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "w3wp.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "TopoAgent", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "EndpointTester.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "EndpointTester", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "TopoAgent.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "Microsoft.Exchange.Monitoring", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "Microsoft.Exchange.Monitoring.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "ImportUsers", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "ImportUsers.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "mad", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "mad.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "try_getserverfordatabase", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "try_getserverfordatabase.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "POWERS~1", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "powershell", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "powershell_ise", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "SharePointSyncDriver", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "starttopnscan", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "starttopnscan.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "MeetingValidator.ConsoleApp", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "DumpMailboxRules", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "DumpMailboxRules.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "hatestutil", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "hatestutil.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "dumpstertestutil", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "dumpstertestutil.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "amclienttestutil", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "amclienttestutil.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "webservicetestutil", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "webservicetestutil.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "wsmprovhost", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "wsmprovhost.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "CiUtil", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "CiUtil.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "FixImapId", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "FixImapId.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "DiagnosticRuntime", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "MsgBodyConverter", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "MsgBodyConverter.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "CalendarPopulator", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "CalendarPopulator.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "ImportEdb", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "ImportEdb.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "PSWSHarvester", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "PSWSHarvester.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "datapopulation", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "datapopulation.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "emltomsg", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "Microsoft.Forefront.AntiSpam.OutboundSpamAlertingBackgroundJob", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "Microsoft.Forefront.AntiSpam.OutboundSpamAlertingBackgroundJob.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "Microsoft.Forefront.AntiSpam.UriGenerator", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "Microsoft.Forefront.AntiSpam.UriGenerator.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "PowershellReflection", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "PowershellReflection.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "DCSetup", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "DCSetup.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "Microsoft.Exchange.Monitoring.ServiceHealth.NotificationService", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "Microsoft.Exchange.Monitoring.ServiceHealth.NotificationService.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "RunE2E", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "RunE2E.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "MAILME", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "MAILME.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "provision", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "provision.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "Microsoft.Exchange.DrumTesting", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "Microsoft.Exchange.DrumTesting.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "vstest.executionengine", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "vstest.executionengine.x86", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "TestConsumer", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "TestConsumer.exe", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "WorkflowHost", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "WorkflowHost.exe", StringComparison.OrdinalIgnoreCase) || processName.StartsWith("Internal.Exchange.", StringComparison.OrdinalIgnoreCase);
		}

		private static void DSAccessPerfCounterInitializationWithRetry(string processName, string applicationName, bool hasMultiInstance)
		{
			int i = 0;
			while (i < 2)
			{
				try
				{
					i++;
					NativeMethods.DsaccessPerfSetProcessName(processName, applicationName, hasMultiInstance);
					break;
				}
				catch (DllNotFoundException ex)
				{
					if (i == 2)
					{
						ex.Data["FileStateInformation"] = Globals.GetDllNotFoundExceptionDiagnostic();
						throw ex;
					}
					Thread.Sleep(500);
				}
			}
		}

		private static string GetDllNotFoundExceptionDiagnostic()
		{
			string text = null;
			try
			{
				text = ExchangeSetupContext.BinPath;
			}
			catch (Exception)
			{
				return "Unable to determine installation bin path";
			}
			if (string.IsNullOrEmpty(text))
			{
				return "Install path not found";
			}
			if (File.Exists(Path.Combine(text, "dsaccessperf.dll")))
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("File exist in current assembly path");
				stringBuilder.AppendLine("Executing processes:");
				foreach (Process process in Process.GetProcesses())
				{
					stringBuilder.AppendLine(process.ProcessName);
				}
				return stringBuilder.ToString();
			}
			return "File doesn't exist in current assembly path";
		}

		private const string ExchangeTopologyEventSource = "MSExchange Topology";

		private const string ExchangeTopologyComponentGuidString = "{35c30fa5-0b20-44d4-8c51-a61b8ed123bf}";

		private const string WmiQueryString = "select * from win32_computersystem";

		public const string MsExchangeADAccessRegistryPath = "SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess";

		public const string MsExchangeTestPassTypeRegistryPath = "SOFTWARE\\Microsoft\\Exchange_Test\\v15";

		public const string EventSource = "MSExchange ADAccess";

		public const string LogPeriodicName = "LogPeriod";

		public const int LogPeriodicDefault = 900;

		internal const string UnknownAppName = "UNKNOWN";

		private static readonly Guid exchangeTopologyComponentGuid = new Guid("{35c30fa5-0b20-44d4-8c51-a61b8ed123bf}");

		private static readonly bool isDatacenterFlag;

		private static string testPassTypeValue;

		private static bool? isVirtualMachine;

		private static bool forceLdapLatency;

		private static readonly bool isMicrosoftHostedOnly;

		private static ExEventLog exchangeTopologyLogger = new ExEventLog(Globals.exchangeTopologyComponentGuid, "MSExchange Topology");

		public static readonly StringPool StringPool = Globals.StringPool;

		internal static readonly int TenantInfoCacheTime = 1800;

		internal static readonly int RecipientInfoCacheTime = 300;

		internal static readonly int RecipientTokenGroupsCacheTime = 900;

		internal static readonly bool EnableNotification = false;

		internal static readonly int LdapConnectionPoolSize = 5;

		internal static InstanceType InstanceType = InstanceType.NotInitialized;

		private static ExEventLog logger;

		private static string currentAppName;

		private static string processName;

		private static string processNameAppName;

		private static readonly string processProcessName;

		private static int processId;

		private static object locker = new object();

		private static readonly bool isExchangeTestExecutable;

		private static SecurityIdentifier localSystemSid = new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null);

		private static SecurityIdentifier networkServiceSid = new SecurityIdentifier(WellKnownSidType.NetworkServiceSid, null);

		public static Globals.MdbStartConcurrencyControlTestHook StartConcurrencyTestHook = null;

		public static Globals.MdbEndConcurrencyControlTestHook EndConcurrencyTestHook = null;

		internal delegate void MdbStartConcurrencyControlTestHook(int semSize, int semCount, int overflowQueueSize, DateTime arrivalTime);

		internal delegate void MdbEndConcurrencyControlTestHook(int semSize, int semCount, int overflowQueueSize, bool isResourceUnhealthyDueToTimeout);
	}
}
