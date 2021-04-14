using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.Common.IL;
using Microsoft.Exchange.Diagnostics.Internal;
using Microsoft.Win32;

namespace Microsoft.Exchange.Diagnostics
{
	internal static class ExWatson
	{
		public static bool MsInternal { get; private set; }

		public static bool TestLabMachine { get; private set; }

		public static bool KillProcessAfterWatson
		{
			get
			{
				return ExWatson.killProcessAfterWatson;
			}
		}

		public static bool WatsonAllowed { get; private set; }

		public static bool ErrorReportingAllowed
		{
			get
			{
				return ExWatson.WatsonAllowed;
			}
		}

		public static bool ReaperThreadAllowed { get; private set; }

		public static bool WerSubmitBypassDataThrottling { get; set; }

		public static string RealAppName
		{
			get
			{
				if (ExWatson.realApplicationName == null)
				{
					ExWatson.realApplicationName = Util.EvaluateOrDefault<string>(delegate()
					{
						string realAppName;
						using (Process currentProcess = Process.GetCurrentProcess())
						{
							string fileName = Path.GetFileName(currentProcess.MainModule.FileName);
							realAppName = ExWatson.GetRealAppName(fileName, Environment.CommandLine);
						}
						return realAppName;
					}, "unknown");
				}
				return ExWatson.realApplicationName;
			}
		}

		public static string LabName
		{
			get
			{
				return ExWatson.labName;
			}
		}

		public static Version ExchangeVersion
		{
			get
			{
				if (ExWatson.exchangeVersion == null && !ExWatson.TryGetExchangeVersionInstalled(out ExWatson.exchangeVersion))
				{
					ExWatson.exchangeVersion = ExWatson.DefaultAssemblyVersion;
				}
				return ExWatson.exchangeVersion;
			}
		}

		public static string ExchangeInstallSource
		{
			get
			{
				if (ExWatson.exchangeInstallSource == null && !ExWatson.TryRegistryKeyGetValue<string>("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{4934D1EA-BE46-48B1-8847-F1AF20E892C1}", "InstallSource", string.Empty, out ExWatson.exchangeInstallSource))
				{
					ExWatson.exchangeInstallSource = string.Empty;
				}
				return ExWatson.exchangeInstallSource;
			}
		}

		public static string ExchangeInstallPath
		{
			get
			{
				if (ExWatson.exchangeInstallPath == null && !ExWatson.TryRegistryKeyGetValue<string>("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup", "MsiInstallPath", string.Empty, out ExWatson.exchangeInstallPath))
				{
					ExWatson.exchangeInstallPath = string.Empty;
				}
				return ExWatson.exchangeInstallPath;
			}
		}

		public static Version DefaultAssemblyVersion
		{
			get
			{
				if (ExWatson.defaultAssemblyVersion == null && !ExWatson.TryGetDefaultAssemblyVersion(out ExWatson.defaultAssemblyVersion))
				{
					ExWatson.defaultAssemblyVersion = new Version(14, 0, 0, 0);
				}
				return ExWatson.defaultAssemblyVersion;
			}
		}

		public static Version RealApplicationVersion
		{
			get
			{
				if (ExWatson.realApplicationVersion == null)
				{
					using (Process currentProcess = Process.GetCurrentProcess())
					{
						if (!ExWatson.TryGetRealApplicationVersion(currentProcess, out ExWatson.realApplicationVersion))
						{
							ExWatson.realApplicationVersion = ExWatson.DefaultAssemblyVersion;
						}
					}
				}
				return ExWatson.realApplicationVersion;
			}
		}

		public static Version ApplicationVersion
		{
			get
			{
				Version version = ExWatson.RealApplicationVersion;
				if (version == null || (ExWatson.ExchangeVersion != null && version < ExWatson.ExchangeVersion))
				{
					version = ExWatson.ExchangeVersion;
				}
				if (version == null)
				{
					version = ExWatson.DefaultAssemblyVersion;
				}
				return version;
			}
		}

		public static string ComputerName
		{
			get
			{
				if (ExWatson.computerName == null && !ExWatson.TryGetComputerName(out ExWatson.computerName))
				{
					ExWatson.computerName = "unknown";
				}
				return ExWatson.computerName;
			}
		}

		public static string ProcessorArchitecture
		{
			get
			{
				if (ExWatson.processorArchitecture == null && !ExWatson.TryGetProcessorArchitecture(out ExWatson.processorArchitecture))
				{
					ExWatson.processorArchitecture = string.Empty;
				}
				return ExWatson.processorArchitecture;
			}
		}

		public static string AppName
		{
			get
			{
				if (!string.IsNullOrEmpty(ExWatson.applicationName))
				{
					return ExWatson.applicationName;
				}
				return ExWatson.RealAppName;
			}
			set
			{
				ExWatson.applicationName = value;
			}
		}

		public static int WatsonCountIn
		{
			get
			{
				return ExWatson.watsonCountIn;
			}
		}

		public static int WatsonCountOut
		{
			get
			{
				return ExWatson.watsonCountOut;
			}
		}

		public static TimeSpan CrashReportTimeout
		{
			get
			{
				return ExWatson.crashReportTimeout;
			}
			set
			{
				ExWatson.crashReportTimeout = value;
			}
		}

		public static int NativeCrashNowThreadId
		{
			get
			{
				return ExWatson.nativeCrashNowThreadId;
			}
		}

		public static long WatsonReportCount
		{
			get
			{
				return ExWatson.watsonReportCount;
			}
		}

		internal static DateTime LastWatsonReport
		{
			get
			{
				return ExWatson.lastWatsonReport;
			}
		}

		private static Dictionary<Type, List<WatsonReportAction>> CurrentThreadReportActions
		{
			get
			{
				if (ExWatson.currentThreadReportActions == null)
				{
					ExWatson.currentThreadReportActions = new Dictionary<Type, List<WatsonReportAction>>(0);
				}
				return ExWatson.currentThreadReportActions;
			}
		}

		private static Dictionary<Type, List<WatsonReportAction>> ProcessWideReportActions
		{
			get
			{
				if (ExWatson.processWideReportActions == null)
				{
					ExWatson.processWideReportActions = new Dictionary<Type, List<WatsonReportAction>>(0);
				}
				return ExWatson.processWideReportActions;
			}
		}

		private static ExWatson.IWatsonTestHook TestHook { get; set; }

		public static void Register()
		{
			ExWatson.Register(null);
		}

		public static void Register(string specifiedEventType)
		{
			lock (ExWatson.mutex)
			{
				ExWatson.Init(specifiedEventType);
				AppDomain.CurrentDomain.UnhandledException += ExWatson.HandleException;
			}
		}

		public static void Init()
		{
			ExWatson.Init(string.Empty);
		}

		public static void Init(string specifiedEventType)
		{
			lock (ExWatson.mutex)
			{
				if (!ExWatson.MsInternal)
				{
					try
					{
						string text = ("." + IPGlobalProperties.GetIPGlobalProperties().DomainName) ?? string.Empty;
						if (text.Length > 1 && (text.EndsWith(".exchangelabs.com", StringComparison.OrdinalIgnoreCase) || text.EndsWith(".exchangelabs.live-int.com", StringComparison.OrdinalIgnoreCase) || text.EndsWith(".ffo.gbl", StringComparison.OrdinalIgnoreCase) || text.EndsWith(".microsoft.com", StringComparison.OrdinalIgnoreCase) || text.EndsWith(".mgd.msft.net", StringComparison.OrdinalIgnoreCase) || text.EndsWith(".outlook.com", StringComparison.OrdinalIgnoreCase) || text.EndsWith(".outlook-int.com", StringComparison.OrdinalIgnoreCase) || text.EndsWith(".protection.gbl", StringComparison.OrdinalIgnoreCase)))
						{
							ExWatson.MsInternal = true;
						}
					}
					catch
					{
					}
				}
				int num = 45;
				if (ExWatson.TryRegistryKeyGetValue<int>("SOFTWARE\\Microsoft\\ExchangeServer\\v15", "PerIssueErrorReportingIntervalInMinutes", 45, out num))
				{
					if (num < 0)
					{
						num = 45;
					}
					if (num > 43200)
					{
						num = 43200;
					}
				}
				ExWatson.throttlingPeriod = TimeSpan.FromMinutes((double)num);
				ExWatson.eventType = ExWatson.ParseEventType(specifiedEventType);
				ExWatson.ReInit();
				ExWatson.initialized = true;
			}
		}

		public static void RegisterReportAction(WatsonReportAction action, WatsonActionScope scope)
		{
			ExWatson.RegisterReportAction(action, scope, null);
		}

		public static void RegisterReportAction(WatsonReportAction action, WatsonActionScope scope, WatsonReport report)
		{
			Dictionary<Type, List<WatsonReportAction>> reportActions;
			switch (scope)
			{
			case WatsonActionScope.Process:
				reportActions = ExWatson.ProcessWideReportActions;
				lock (reportActions)
				{
					ExWatson.MergeAction(action, reportActions);
					return;
				}
				break;
			case WatsonActionScope.Thread:
				break;
			case WatsonActionScope.Report:
				if (report == null)
				{
					throw new ArgumentNullException("report");
				}
				if (report.ReportActions == null)
				{
					report.ReportActions = new Dictionary<Type, List<WatsonReportAction>>(1);
				}
				reportActions = report.ReportActions;
				ExWatson.MergeAction(action, reportActions);
				return;
			default:
				throw new ArgumentException("Scope must be either WatsonActionScope.Process, WatsonActionScope.Thread or WatsonActionScope.Report", "scope");
			}
			reportActions = ExWatson.CurrentThreadReportActions;
			ExWatson.MergeAction(action, reportActions);
		}

		public static void UnregisterReportAction(WatsonReportAction action, WatsonActionScope scope)
		{
			Dictionary<Type, List<WatsonReportAction>> dictionary;
			switch (scope)
			{
			case WatsonActionScope.Process:
				dictionary = ExWatson.ProcessWideReportActions;
				lock (dictionary)
				{
					ExWatson.RemoveAction(action, dictionary);
					return;
				}
				break;
			case WatsonActionScope.Thread:
				break;
			default:
				throw new ArgumentException("Scope must be either WatsonActionScope.Process or WatsonActionScope.Thread", "scope");
			}
			dictionary = ExWatson.CurrentThreadReportActions;
			ExWatson.RemoveAction(action, dictionary);
		}

		public static void ClearReportActions(WatsonActionScope scope)
		{
			if (scope == WatsonActionScope.Process)
			{
				Dictionary<Type, List<WatsonReportAction>> dictionary = ExWatson.ProcessWideReportActions;
				lock (dictionary)
				{
					dictionary.Clear();
					return;
				}
			}
			if (scope == WatsonActionScope.Thread)
			{
				Dictionary<Type, List<WatsonReportAction>> dictionary = ExWatson.CurrentThreadReportActions;
				dictionary.Clear();
				return;
			}
			throw new ArgumentException("Scope must be either WatsonActionScope.Process or WatsonActionScope.Thread", "scope");
		}

		public static void HandleException(object sender, UnhandledExceptionEventArgs e)
		{
			ExWatson.HandleException(e, ReportOptions.None);
		}

		public static void HandleException(UnhandledExceptionEventArgs e, ReportOptions options)
		{
			ExWatson.<>c__DisplayClass8 CS$<>8__locals1 = new ExWatson.<>c__DisplayClass8();
			CS$<>8__locals1.e = e;
			CS$<>8__locals1.options = options;
			if (CS$<>8__locals1.e.ExceptionObject is ExWatson.CrashNowException)
			{
				return;
			}
			using (Process thisProcess = Process.GetCurrentProcess())
			{
				ExWatson.SendEnglishReport(delegate
				{
					if (CS$<>8__locals1.e.IsTerminating)
					{
						CS$<>8__locals1.options |= ReportOptions.ReportTerminateAfterSend;
					}
					return new WatsonExceptionReport(ExWatson.eventType, thisProcess, CS$<>8__locals1.e.ExceptionObject as Exception, CS$<>8__locals1.options);
				});
			}
			if (ExWatson.killProcessAfterWatson && CS$<>8__locals1.e.IsTerminating)
			{
				ExWatson.TerminateCurrentProcess();
			}
		}

		public static void SendReport(Exception exception)
		{
			ExWatson.SendReport(exception, ReportOptions.ReportTerminateAfterSend, null);
		}

		[PermissionSet(SecurityAction.Demand, Unrestricted = true)]
		public static void SendReport(Exception exception, ReportOptions options, string extraData)
		{
			if (exception != null && exception is FileNotFoundException && exception.Message.Contains("Could not load file or assembly"))
			{
				return;
			}
			if (ExWatson.ShouldSendCheckThrottle(exception))
			{
				using (Process thisProcess = Process.GetCurrentProcess())
				{
					ExWatson.SendEnglishReport(delegate
					{
						if (ExWatson.IsObjectNotDisposedException(exception))
						{
							options |= (ReportOptions.DoNotCollectDumps | ReportOptions.DeepStackTraceHash);
						}
						WatsonReport watsonReport = new WatsonExceptionReport(ExWatson.eventType, thisProcess, exception, options);
						if (!string.IsNullOrEmpty(extraData))
						{
							ExWatson.RegisterReportAction(new WatsonExtraDataReportAction(extraData), WatsonActionScope.Report, watsonReport);
						}
						return watsonReport;
					});
				}
			}
		}

		public static void SendHangWatsonReport(Exception exception, Process process)
		{
			ExWatson.SendEnglishReport(() => new WatsonHangReport("E12", process, exception));
		}

		public static void SendReportAndCrashOnAnotherThread(Exception exception)
		{
			ExWatson.SendReportAndCrashOnAnotherThread(exception, ReportOptions.None, null, null);
		}

		public static void SendReportAndCrashOnAnotherThread(Exception exception, ReportOptions options, ExWatson.CrashNowDelegate crashDelegate, string extraData)
		{
			options |= ReportOptions.ReportTerminateAfterSend;
			options |= ReportOptions.DoNotFreezeThreads;
			ExWatson.SendReport(exception, options, extraData);
			if (crashDelegate == null && ExWatson.killProcessAfterWatson)
			{
				ExWatson.TerminateCurrentProcess();
				return;
			}
			if (Interlocked.CompareExchange(ref ExWatson.crashNowManagedThreadId, Environment.CurrentManagedThreadId, 0) == 0)
			{
				ExWatson.crashNowException = exception;
				ExWatson.crashNowDelegate = crashDelegate;
				ExWatson.crashNowEvent.Set();
				ExWatson.crashNowThread.Join();
				return;
			}
			ExWatson.guardCrashEvent.WaitOne();
		}

		[PermissionSet(SecurityAction.Demand, Unrestricted = true)]
		public static void SendClientWatsonReport(WatsonClientReport report, string extraData)
		{
			ExWatson.SendEnglishReport(delegate
			{
				if (!string.IsNullOrEmpty(extraData))
				{
					ExWatson.RegisterReportAction(new WatsonExtraDataReportAction(extraData), WatsonActionScope.Report, report);
				}
				return report;
			});
		}

		[PermissionSet(SecurityAction.Demand, Unrestricted = true)]
		public static void SendLatencyWatsonReport(string triggerVersion, string locationIdentity, string exceptionName, string callstack, string methodName, string detailedExceptionInformation)
		{
			ExWatson.SendEnglishReport(() => new WatsonLatencyReport(ExWatson.eventType, triggerVersion, locationIdentity, exceptionName, callstack, methodName, detailedExceptionInformation));
		}

		[PermissionSet(SecurityAction.Demand, Unrestricted = true)]
		public static void SendTroubleshootingWatsonReport(string triggerVersion, string locationIdentity, string exceptionName, string callstack, string methodName, string detailedExceptionInformation, string traceFilePath)
		{
			ExWatson.SendEnglishReport(() => new WatsonTroubleshootingReport(ExWatson.eventType, triggerVersion, locationIdentity, exceptionName, callstack, methodName, detailedExceptionInformation, traceFilePath));
		}

		[PermissionSet(SecurityAction.Demand, Unrestricted = true)]
		public static void SendGenericWatsonReport(string eventType, string appVersion, string appName, string assemblyVersion, string assemblyName, string exceptionType, string callstack, string callstackHash, string methodName, string detailedExceptionInformation)
		{
			ExWatson.SendEnglishReport(() => new WatsonGenericReport(ExWatson.ParseEventType(eventType), appVersion, appName, assemblyVersion, assemblyName, exceptionType, callstack, callstackHash, methodName, detailedExceptionInformation));
		}

		[PermissionSet(SecurityAction.Demand, Unrestricted = true)]
		public static void SendThrottledGenericWatsonReport(string eventType, string appVersion, string appName, string assemblyVersion, string assemblyName, string exceptionType, string callstack, string callstackHash, string methodName, string detailedExceptionInformation, TimeSpan throttlingInterval, out bool wasThrottled)
		{
			wasThrottled = true;
			if (ExWatson.ShouldSendInfoWatson(callstackHash, throttlingInterval))
			{
				wasThrottled = false;
				ExWatson.SendGenericWatsonReport(eventType, appVersion, appName, assemblyVersion, assemblyName, exceptionType, callstack, callstackHash, methodName, detailedExceptionInformation);
			}
		}

		[PermissionSet(SecurityAction.Demand, Unrestricted = true)]
		public static void SendExternalProcessWatsonReportWithFiles(Process process, string eventType, Exception exception, string detailedExceptionInformation, string[] filesToUpload, ReportOptions reportOptions)
		{
			ExWatson.SendEnglishReport(delegate
			{
				reportOptions |= ReportOptions.DoNotFreezeThreads;
				WatsonExternalProcessReport watsonExternalProcessReport = new WatsonExternalProcessReport(process, ExWatson.ParseEventType(eventType), exception, detailedExceptionInformation, reportOptions);
				if (filesToUpload != null)
				{
					foreach (string text in filesToUpload)
					{
						if (File.Exists(text))
						{
							ExWatson.RegisterReportAction(new WatsonExtraFileReportAction(text), WatsonActionScope.Report, watsonExternalProcessReport);
						}
					}
				}
				return watsonExternalProcessReport;
			});
		}

		public static void SendReportOnUnhandledException(ExWatson.MethodDelegate methodDelegate)
		{
			ExWatson.SendReportOnUnhandledException(methodDelegate, (object exception) => true, ReportOptions.ReportTerminateAfterSend);
		}

		public static void SendReportOnUnhandledException(ExWatson.MethodDelegate methodDelegate, ExWatson.IsExceptionInteresting exceptionInteresting)
		{
			ExWatson.SendReportOnUnhandledException(methodDelegate, exceptionInteresting, ReportOptions.ReportTerminateAfterSend);
		}

		public static void SendReportOnUnhandledException(ExWatson.MethodDelegate methodDelegate, ExWatson.IsExceptionInteresting exceptionInteresting, ReportOptions options)
		{
			ExWatson.<>c__DisplayClass2b CS$<>8__locals1 = new ExWatson.<>c__DisplayClass2b();
			CS$<>8__locals1.methodDelegate = methodDelegate;
			CS$<>8__locals1.exceptionInteresting = exceptionInteresting;
			CS$<>8__locals1.options = options;
			ILUtil.DoTryFilterCatch(new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<SendReportOnUnhandledException>b__27)), new FilterDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<SendReportOnUnhandledException>b__28)), new CatchDelegate(null, (UIntPtr)ldftn(<SendReportOnUnhandledException>b__29)));
		}

		public static void AddExtraData(string data)
		{
			if (!string.IsNullOrEmpty(data))
			{
				ExWatson.RegisterReportAction(new WatsonExtraDataReportAction(data), WatsonActionScope.Process);
			}
		}

		public static bool TryAddExtraFile(string filename)
		{
			if (File.Exists(filename))
			{
				ExWatson.RegisterReportAction(new WatsonExtraFileReportAction(filename), WatsonActionScope.Process);
				return true;
			}
			return false;
		}

		public static bool TryGetRealApplicationVersion(Process appProcess, out Version appVersion)
		{
			bool result = false;
			appVersion = null;
			try
			{
				if (appProcess != null && (appProcess.MainModule.FileVersionInfo.ProductName.StartsWith("Microsoft® Exchange") || appProcess.MainModule.FileVersionInfo.ProductName.StartsWith("Microsoft Exchange")))
				{
					appVersion = new Version(appProcess.MainModule.FileVersionInfo.FileVersion);
					result = true;
				}
			}
			catch
			{
			}
			return result;
		}

		public static void IncrementWatsonCountIn()
		{
			Interlocked.Increment(ref ExWatson.watsonCountIn);
		}

		public static void IncrementWatsonCountOut()
		{
			Interlocked.Increment(ref ExWatson.watsonCountOut);
		}

		public static void SetWatsonReportAlreadySent(Exception exception)
		{
			if (exception.Data != null)
			{
				exception.Data["WatsonReportAlreadySent"] = null;
			}
		}

		public static bool IsWatsonReportAlreadySent(Exception exception)
		{
			while (exception != null)
			{
				if (exception.Data != null && exception.Data.Contains("WatsonReportAlreadySent"))
				{
					return true;
				}
				exception = exception.InnerException;
			}
			return false;
		}

		internal static string GetRealAppName(string appName, string commandLineArgs)
		{
			if (ExWatson.ProcessNameRemapSet.Contains(appName))
			{
				if (appName.Equals("w3wp.exe", StringComparison.OrdinalIgnoreCase))
				{
					Match match = Regex.Match(commandLineArgs, "w3wp\\.exe -ap \"(.*?)\"");
					if (match.Success)
					{
						return "w3wp#" + match.Groups[1].Value;
					}
				}
				else if (appName.Equals("svchost.exe", StringComparison.OrdinalIgnoreCase))
				{
					int num = commandLineArgs.IndexOf("svchost.exe -k", StringComparison.OrdinalIgnoreCase);
					if (num >= 0)
					{
						return "svchost#" + commandLineArgs.Substring(num + "svchost.exe -k".Length + 1);
					}
				}
				else if (appName.Equals("noderunner.exe", StringComparison.OrdinalIgnoreCase))
				{
					foreach (string text in ExWatson.NodeRunnerInstanceNames)
					{
						if (commandLineArgs.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0)
						{
							return "NodeRunner#" + text;
						}
					}
				}
			}
			return appName;
		}

		internal static bool ShouldSendInfoWatson(string callstackHash, TimeSpan throttlingInterval)
		{
			DateTime utcNow = DateTime.UtcNow;
			if (string.IsNullOrEmpty(callstackHash) || throttlingInterval == TimeSpan.Zero)
			{
				return true;
			}
			bool result;
			lock (ExWatson.infoWatsonLock)
			{
				InfoWatsonThrottlingData infoWatsonThrottlingData;
				if (ExWatson.infoWatsonThrottling.TryGetValue(callstackHash, out infoWatsonThrottlingData))
				{
					infoWatsonThrottlingData.LastAccessTimeUtc = utcNow;
					if (utcNow >= infoWatsonThrottlingData.NextAllowableLogTimeUtc)
					{
						infoWatsonThrottlingData.NextAllowableLogTimeUtc = utcNow + throttlingInterval;
						result = true;
					}
					else
					{
						result = false;
					}
				}
				else
				{
					ExWatson.PurgeInfoWatsonThrottlingDictionary();
					infoWatsonThrottlingData = new InfoWatsonThrottlingData(callstackHash, utcNow + throttlingInterval);
					ExWatson.infoWatsonThrottling[callstackHash] = infoWatsonThrottlingData;
					result = true;
				}
			}
			return result;
		}

		internal static bool IsObjectNotDisposedException(Exception exception)
		{
			Type type = exception.GetType();
			return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(ObjectNotDisposedException<>);
		}

		internal static void TerminateCurrentProcess()
		{
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				DiagnosticsNativeMethods.TerminateProcess(currentProcess.Handle, -559034355);
			}
		}

		internal static void FreezeAllThreads()
		{
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				int currentThreadId = DiagnosticsNativeMethods.GetCurrentThreadId();
				for (int i = ExWatson.firstUserThread; i < currentProcess.Threads.Count; i++)
				{
					ProcessThread processThread = currentProcess.Threads[i];
					int id = processThread.Id;
					if (id != currentThreadId)
					{
						using (ThreadSafeHandle threadSafeHandle = DiagnosticsNativeMethods.OpenThread(DiagnosticsNativeMethods.ThreadAccess.SuspendResume, false, id))
						{
							DiagnosticsNativeMethods.SuspendThread(threadSafeHandle);
						}
					}
				}
			}
		}

		internal static bool TryRegistryKeyGetValue<T>(string regKeyPath, string name, T defaultValue, out T returnValue)
		{
			bool result = false;
			RegistryKey registryKey = null;
			returnValue = defaultValue;
			try
			{
				registryKey = Registry.LocalMachine.OpenSubKey(regKeyPath, false);
				if (registryKey != null)
				{
					result = ExWatson.TryRegistryKeyGetValue<T>(registryKey, name, defaultValue, out returnValue);
				}
			}
			catch
			{
			}
			finally
			{
				if (registryKey != null)
				{
					registryKey.Close();
				}
			}
			return result;
		}

		internal static void EvaluateReportActions(XmlWriter writer, WatsonReport report)
		{
			Dictionary<Type, List<WatsonReportAction>> dictionary = ExWatson.processWideReportActions;
			Dictionary<Type, List<WatsonReportAction>> dictionary2 = ExWatson.currentThreadReportActions;
			Dictionary<Type, List<WatsonReportAction>> reportActions = report.ReportActions;
			if (dictionary != null)
			{
				List<WatsonReportAction> list = null;
				int num = 0;
				lock (dictionary)
				{
					foreach (List<WatsonReportAction> list2 in dictionary.Values)
					{
						num += list2.Count;
					}
					list = new List<WatsonReportAction>(num);
					foreach (List<WatsonReportAction> collection in dictionary.Values)
					{
						list.AddRange(collection);
					}
				}
				for (int i = 0; i < num; i++)
				{
					list[i].WriteResult(report, writer);
				}
			}
			if (dictionary2 != null)
			{
				foreach (KeyValuePair<Type, List<WatsonReportAction>> keyValuePair in dictionary2)
				{
					foreach (WatsonReportAction watsonReportAction in keyValuePair.Value)
					{
						watsonReportAction.WriteResult(report, writer);
					}
				}
			}
			if (reportActions != null)
			{
				foreach (KeyValuePair<Type, List<WatsonReportAction>> keyValuePair2 in reportActions)
				{
					foreach (WatsonReportAction watsonReportAction2 in keyValuePair2.Value)
					{
						watsonReportAction2.WriteResult(report, writer);
					}
				}
			}
		}

		internal static bool ShouldSubmitReport(WatsonReport report, string reportXmlFileName, string reportTextFileName, ref DiagnosticsNativeMethods.WER_SUBMIT_RESULT submitResult)
		{
			ExWatson.IWatsonTestHook testHook = ExWatson.TestHook;
			if (testHook != null)
			{
				try
				{
					return testHook.ShouldSubmitReport(report, reportXmlFileName, reportTextFileName, ref submitResult);
				}
				catch
				{
				}
				return true;
			}
			return true;
		}

		internal static ExWatson.IWatsonTestHook SetTestHook(ExWatson.IWatsonTestHook newTestHook)
		{
			ExWatson.IWatsonTestHook testHook = ExWatson.TestHook;
			ExWatson.TestHook = newTestHook;
			return testHook;
		}

		[PermissionSet(SecurityAction.Demand, Unrestricted = true)]
		private static void SendEnglishReport(Func<WatsonReport> reportCallback)
		{
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
			try
			{
				CultureInfo cultureInfo = CultureInfo.GetCultureInfo("en-US");
				Thread.CurrentThread.CurrentCulture = cultureInfo;
				Thread.CurrentThread.CurrentUICulture = cultureInfo;
				WatsonReport watsonReport = null;
				lock (ExWatson.mutex)
				{
					ExWatson.EnsureWatsonInitialization();
					watsonReport = reportCallback();
				}
				watsonReport.Send();
				Interlocked.Increment(ref ExWatson.watsonReportCount);
			}
			finally
			{
				Thread.CurrentThread.CurrentCulture = currentCulture;
				Thread.CurrentThread.CurrentUICulture = currentUICulture;
			}
		}

		private static void PurgeInfoWatsonThrottlingDictionary()
		{
			DateTime utcNow = DateTime.UtcNow;
			if (ExWatson.infoWatsonThrottling.Count >= 512)
			{
				InfoWatsonThrottlingData infoWatsonThrottlingData = null;
				ExWatson.infoWatsonEntriesToRemove.Clear();
				foreach (InfoWatsonThrottlingData infoWatsonThrottlingData2 in ExWatson.infoWatsonThrottling.Values)
				{
					if (utcNow >= infoWatsonThrottlingData2.NextAllowableLogTimeUtc)
					{
						ExWatson.infoWatsonEntriesToRemove.Add(infoWatsonThrottlingData2);
					}
					if (infoWatsonThrottlingData == null || infoWatsonThrottlingData2.LastAccessTimeUtc < infoWatsonThrottlingData.LastAccessTimeUtc)
					{
						infoWatsonThrottlingData = infoWatsonThrottlingData2;
					}
				}
				if (ExWatson.infoWatsonEntriesToRemove.Count > 0)
				{
					using (List<InfoWatsonThrottlingData>.Enumerator enumerator2 = ExWatson.infoWatsonEntriesToRemove.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							InfoWatsonThrottlingData infoWatsonThrottlingData3 = enumerator2.Current;
							ExWatson.infoWatsonThrottling.Remove(infoWatsonThrottlingData3.Hash);
						}
						return;
					}
				}
				ExWatson.infoWatsonThrottling.Remove(infoWatsonThrottlingData.Hash);
			}
		}

		private static int InitFirstUserThread()
		{
			int result;
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				result = currentProcess.Threads.Count - 1;
			}
			return result;
		}

		private static void ReInit()
		{
			int num = 0;
			if (ExWatson.TryRegistryKeyGetValue<int>("SOFTWARE\\Microsoft\\.NETFramework", "DbgJITDebugLaunchSetting", 0, out num))
			{
				ExWatson.killProcessAfterWatson = (num == 0);
			}
			if (ExWatson.TryRegistryKeyGetValue<int>("SOFTWARE\\Microsoft\\ExchangeServer\\v15", "DisableErrorReporting", 1, out num))
			{
				ExWatson.WatsonAllowed = (num == 0);
			}
			else if (ExWatson.TryRegistryKeyGetValue<int>("SOFTWARE\\Microsoft\\OLMA", "DisableErrorReporting", 1, out num))
			{
				ExWatson.WatsonAllowed = (num == 0);
			}
			else if (ExWatson.TryRegistryKeyGetValue<int>("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Load Generator", "DisableErrorReporting", 1, out num))
			{
				ExWatson.WatsonAllowed = (num == 0);
			}
			else if (ExWatson.TryRegistryKeyGetValue<int>("SOFTWARE\\Microsoft\\ExRCA", "DisableErrorReporting", 1, out num))
			{
				ExWatson.WatsonAllowed = (num == 0);
			}
			if (ExWatson.TryRegistryKeyGetValue<int>("SOFTWARE\\Microsoft\\ExchangeServer\\v15", "DisableReaperThread", 1, out num))
			{
				ExWatson.ReaperThreadAllowed = (num == 0);
			}
			if (ExWatson.crashNowThread == null)
			{
				ExWatson.crashNowEvent = new ManualResetEvent(false);
				ExWatson.guardCrashEvent = new ManualResetEvent(false);
				ExWatson.crashNowThreadIsRunning = new ManualResetEvent(false);
				ExWatson.crashNowThread = new Thread(new ThreadStart(ExWatson.CrashNow));
				ExWatson.crashNowThread.Name = "ExWatson CrashNow Thread";
				ExWatson.crashNowThread.IsBackground = true;
				ExWatson.crashNowThread.Start();
				ExWatson.crashNowThreadIsRunning.WaitOne();
			}
			if (ExWatson.MsInternal)
			{
				string text = string.Empty;
				if (ExWatson.TryRegistryKeyGetValue<string>("SOFTWARE\\Microsoft\\ExchangeServer\\v15", "LabName", string.Empty, out text) && !string.IsNullOrEmpty(text))
				{
					if (text.Length > 32)
					{
						text = text.Substring(0, 32);
					}
					ExWatson.labName = text + "-";
					ExWatson.TestLabMachine = ExWatson.TestLabNameSet.Contains(text);
					return;
				}
			}
			else
			{
				ExWatson.labName = "c-";
			}
		}

		private static void MergeAction(WatsonReportAction action, Dictionary<Type, List<WatsonReportAction>> allActions)
		{
			List<WatsonReportAction> reportActionListForType = ExWatson.GetReportActionListForType(allActions, action.GetType());
			if (!reportActionListForType.Contains(action))
			{
				reportActionListForType.Add(action);
			}
		}

		private static void RemoveAction(WatsonReportAction action, Dictionary<Type, List<WatsonReportAction>> allActions)
		{
			List<WatsonReportAction> reportActionListForType = ExWatson.GetReportActionListForType(allActions, action.GetType());
			reportActionListForType.Remove(action);
		}

		private static List<WatsonReportAction> GetReportActionListForType(Dictionary<Type, List<WatsonReportAction>> allActions, Type type)
		{
			List<WatsonReportAction> list = null;
			if (!allActions.TryGetValue(type, out list))
			{
				list = new List<WatsonReportAction>(1);
				allActions.Add(type, list);
			}
			return list;
		}

		private static bool ShouldSendCheckThrottle(Exception exception)
		{
			TimeSpan exceptionThrottlingTimeout = ExWatson.throttlingPeriod;
			ExWatson.IWatsonTestHook testHook = ExWatson.TestHook;
			if (testHook != null)
			{
				try
				{
					if (testHook.ShouldSkipThrottling(exception))
					{
						return true;
					}
					exceptionThrottlingTimeout = testHook.GetExceptionThrottlingTimeout(exception, exceptionThrottlingTimeout);
				}
				catch
				{
				}
			}
			try
			{
				if (exception is ThreadAbortException && DateTime.UtcNow - ExWatson.lastWatsonReport < TimeSpan.FromMinutes(5.0))
				{
					return false;
				}
				if (exceptionThrottlingTimeout == TimeSpan.Zero)
				{
					ExWatson.lastWatsonReport = DateTime.UtcNow;
					return true;
				}
				string text = string.Empty;
				WatsonExceptionReport.TryStringHashFromStackTrace(exception, false, out text);
				text += exception.GetType().ToString();
				lock (ExWatson.watsonLock)
				{
					DateTime d;
					if (ExWatson.watsonThrottling.TryGetValue(text, out d))
					{
						if (DateTime.UtcNow - d > exceptionThrottlingTimeout)
						{
							ExWatson.watsonThrottling[text] = DateTime.UtcNow;
							ExWatson.lastWatsonReport = DateTime.UtcNow;
							return true;
						}
						return false;
					}
					else
					{
						if (ExWatson.watsonThrottling.Count > 200)
						{
							ExWatson.watsonThrottling.Clear();
						}
						ExWatson.watsonThrottling[text] = DateTime.UtcNow;
					}
				}
			}
			catch
			{
			}
			ExWatson.lastWatsonReport = DateTime.UtcNow;
			return true;
		}

		private static bool TryGetExchangeVersionInstalled(out Version installedVersion)
		{
			bool result = false;
			RegistryKey registryKey = null;
			int num = 0;
			int minor = 0;
			int build = 0;
			int revision = 0;
			installedVersion = null;
			try
			{
				registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup");
				if (registryKey != null)
				{
					ExWatson.TryRegistryKeyGetValue<int>(registryKey, "MsiProductMajor", 0, out num);
					ExWatson.TryRegistryKeyGetValue<int>(registryKey, "MsiProductMinor", 0, out minor);
					ExWatson.TryRegistryKeyGetValue<int>(registryKey, "MsiBuildMajor", 0, out build);
					ExWatson.TryRegistryKeyGetValue<int>(registryKey, "MsiBuildMinor", 0, out revision);
					if (num > 0)
					{
						installedVersion = new Version(num, minor, build, revision);
						result = true;
					}
				}
			}
			catch
			{
			}
			finally
			{
				if (registryKey != null)
				{
					registryKey.Close();
				}
			}
			return result;
		}

		private static bool TryGetDefaultAssemblyVersion(out Version assemblyVersion)
		{
			bool result = false;
			assemblyVersion = null;
			try
			{
				object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
				if (customAttributes.Length == 1)
				{
					AssemblyFileVersionAttribute assemblyFileVersionAttribute = customAttributes[0] as AssemblyFileVersionAttribute;
					if (assemblyFileVersionAttribute != null)
					{
						assemblyVersion = new Version(assemblyFileVersionAttribute.Version);
						result = true;
					}
				}
			}
			catch
			{
			}
			return result;
		}

		private static bool TryGetComputerName(out string computerName)
		{
			bool result = false;
			computerName = null;
			try
			{
				computerName = Environment.MachineName;
				result = true;
			}
			catch
			{
			}
			return result;
		}

		private static bool TryGetProcessorArchitecture(out string processorArch)
		{
			bool result = true;
			processorArch = null;
			if (IntPtr.Size == 4)
			{
				processorArch = "x86";
			}
			else if (IntPtr.Size == 8)
			{
				processorArch = "AMD64";
			}
			else
			{
				result = false;
			}
			return result;
		}

		private static bool TryRegistryKeyGetValue<T>(RegistryKey regKey, string name, T defaultValue, out T returnValue)
		{
			bool result = false;
			returnValue = defaultValue;
			try
			{
				if (regKey != null)
				{
					returnValue = (T)((object)regKey.GetValue(name, defaultValue));
					result = true;
				}
			}
			catch
			{
			}
			return result;
		}

		private static void CrashNow()
		{
			ExWatson.nativeCrashNowThreadId = DiagnosticsNativeMethods.GetCurrentThreadId();
			ExWatson.crashNowThreadIsRunning.Set();
			ExWatson.crashNowEvent.WaitOne();
			string message = string.Format("Crashing because of a request from another thread. ManagedThreadId = {0} (0x{0:x})", ExWatson.crashNowManagedThreadId);
			if (ExWatson.crashNowDelegate != null)
			{
				ExWatson.crashNowDelegate(ExWatson.crashNowException, ExWatson.crashNowManagedThreadId);
			}
			if (ExWatson.crashNowException == null)
			{
				throw new ExWatson.CrashNowException(message);
			}
			throw new ExWatson.CrashNowException(message, ExWatson.crashNowException);
		}

		private static void EnsureWatsonInitialization()
		{
			if (!ExWatson.initialized)
			{
				ExWatson.Init();
				return;
			}
			ExWatson.ReInit();
		}

		private static string ParseEventType(string eventTypeName)
		{
			if (!ExWatson.WatsonEventSet.Contains(eventTypeName))
			{
				eventTypeName = "E12IIS";
			}
			return eventTypeName;
		}

		public const string Exchange12 = "E12";

		public const string Exchange12IE = "E12IE";

		public const string Exchange12IIS = "E12IIS";

		internal const string ExchangeRegistryKeyWatsonThrottlingDWord = "PerIssueErrorReportingIntervalInMinutes";

		internal const int DefaultThrottlingIntervalInMinutes = 45;

		internal const string ExchangeRegistryKeyPath = "SOFTWARE\\Microsoft\\ExchangeServer\\v15";

		internal const string ExchangeRegistryKeyDisableErrorReportingDWord = "DisableErrorReporting";

		internal const string ExchangeRegistryKeyLabNameString = "LabName";

		internal const int MaxInfoWatsonThrottlingDictionarySize = 512;

		private const string Unknown = "unknown";

		private const string GALSyncRegistryKeyPath = "SOFTWARE\\Microsoft\\OLMA";

		private const string LoadGenRegistryKeyPath = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Load Generator";

		private const string ExRCARegistryKeyPath = "SOFTWARE\\Microsoft\\ExRCA";

		private const string ExchangeRegistryKeyDisableReaperThreadDWord = "DisableReaperThread";

		private const string ExchangeSetupRegistryKeyPath = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup";

		private const string ExchangeProductUninstallRegistryKeyPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{4934D1EA-BE46-48B1-8847-F1AF20E892C1}";

		private const string ExchangeProductUninstallRegistryKeyInstallSourceString = "InstallSource";

		private const string WatsonProductsRegistryKeyPath = "Software\\Microsoft\\PCHealth\\ErrorReporting\\DW\\Products";

		private const string DotNetFrameworkRegistryKeyPath = "SOFTWARE\\Microsoft\\.NETFramework";

		private const string DotNetFrameworkRegistryKeyDbgJITDWord = "DbgJITDebugLaunchSetting";

		private const int MinThrottlingIntervalInMinutes = 0;

		private const int MaxThrottlingIntervalInMinutes = 43200;

		private const int MaxThrottlingDictionarySize = 200;

		private const string WatsonReportAlreadySentTag = "WatsonReportAlreadySent";

		private static readonly HashSet<string> TestLabNameSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"DART",
			"FUZZ",
			"EXTST",
			"E15_EXTST"
		};

		private static readonly HashSet<string> ProcessNameRemapSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"w3wp.exe",
			"noderunner.exe",
			"svchost.exe"
		};

		private static readonly string[] NodeRunnerInstanceNames = new string[]
		{
			"AdminNode1",
			"ContentEngineNode1",
			"IndexNode1",
			"InteractionEngineNode1"
		};

		private static readonly HashSet<string> WatsonEventSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"E12",
			"E12IE",
			"E12IIS"
		};

		private static readonly object infoWatsonLock = new object();

		private static Dictionary<string, InfoWatsonThrottlingData> infoWatsonThrottling = new Dictionary<string, InfoWatsonThrottlingData>(0);

		private static object mutex = new object();

		private static bool killProcessAfterWatson = true;

		private static string eventType = "E12IIS";

		private static bool initialized;

		private static TimeSpan throttlingPeriod = TimeSpan.Zero;

		private static Dictionary<string, DateTime> watsonThrottling = new Dictionary<string, DateTime>(0);

		private static object watsonLock = new object();

		private static List<InfoWatsonThrottlingData> infoWatsonEntriesToRemove = new List<InfoWatsonThrottlingData>(256);

		private static DateTime lastWatsonReport = DateTime.MinValue;

		private static long watsonReportCount = 0L;

		private static string applicationName;

		private static string realApplicationName;

		private static string computerName;

		private static string labName = string.Empty;

		private static Version exchangeVersion;

		private static string exchangeInstallSource;

		private static string exchangeInstallPath;

		private static Version defaultAssemblyVersion;

		private static Version realApplicationVersion;

		private static string processorArchitecture;

		private static int watsonCountIn;

		private static int watsonCountOut;

		private static TimeSpan crashReportTimeout = new TimeSpan(0, 30, 0);

		[ThreadStatic]
		private static Dictionary<Type, List<WatsonReportAction>> currentThreadReportActions;

		private static Dictionary<Type, List<WatsonReportAction>> processWideReportActions;

		private static int firstUserThread = ExWatson.InitFirstUserThread();

		private static Thread crashNowThread;

		private static ManualResetEvent crashNowThreadIsRunning;

		private static ManualResetEvent crashNowEvent;

		private static ManualResetEvent guardCrashEvent;

		private static Exception crashNowException;

		private static ExWatson.CrashNowDelegate crashNowDelegate;

		private static int crashNowManagedThreadId;

		private static int nativeCrashNowThreadId;

		public delegate void MethodDelegate();

		public delegate void CrashNowDelegate(Exception exception, int threadId);

		public delegate bool IsExceptionInteresting(object exception);

		public enum EventType
		{
			E12,
			E12IIS,
			E12IE
		}

		internal interface IWatsonTestHook
		{
			bool ShouldSkipThrottling(Exception ex);

			TimeSpan GetExceptionThrottlingTimeout(Exception ex, TimeSpan defaultTimeout);

			bool ShouldSubmitReport(WatsonReport report, string reportXmlFileName, string reportTextFileName, ref DiagnosticsNativeMethods.WER_SUBMIT_RESULT submitResult);
		}

		private class CrashNowException : Exception
		{
			public CrashNowException(string message) : base(message)
			{
			}

			public CrashNowException(string message, Exception innerException) : base(message, innerException)
			{
			}
		}
	}
}
