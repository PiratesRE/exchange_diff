using System;
using System.Configuration;

namespace Microsoft.Exchange.ProcessManager
{
	internal class ServiceConfiguration
	{
		private ServiceConfiguration()
		{
		}

		public int CheckProcessHandleTimeOut
		{
			get
			{
				return this.checkProcessHandleTimeOut;
			}
		}

		public bool DisconnectTransportPerformanceCounters
		{
			get
			{
				return this.disconnectTransportPerformanceCounters;
			}
		}

		public int MaxIOThreads
		{
			get
			{
				return this.maxIOThreads;
			}
		}

		public int MaxWorkerProcessThreads
		{
			get
			{
				return this.maxWorkerProcessThreads;
			}
		}

		public long MaxWorkerProcessWorkingSet
		{
			get
			{
				return this.maxWorkerProcessWorkingSet;
			}
		}

		public int MaxWorkerProcessRefreshInterval
		{
			get
			{
				return this.maxWorkerProcessRefreshInterval;
			}
		}

		public int MaxWorkerProcessExitTimeout
		{
			get
			{
				return this.maxWorkerProcessExitTimeout;
			}
		}

		public int MaxWorkerProcessDumpTimeout
		{
			get
			{
				return this.maxWorkerProcessDumpTimeout;
			}
		}

		public int MaxProcessManagerRestartAttempts
		{
			get
			{
				return this.maxProcessManagerRestartAttempts;
			}
		}

		public bool ServiceListening
		{
			get
			{
				return this.serviceListening;
			}
		}

		public int MaxProcessRestartAttemptsWhileInStartingState
		{
			get
			{
				return this.maxProcessRestartAttemptsWhileInStartingState;
			}
		}

		public int ThrashCrashMaximum
		{
			get
			{
				return this.thrashCrashMaximum;
			}
		}

		public static ServiceConfiguration Load(ProcessManagerService processManagerService)
		{
			ServiceConfiguration serviceConfiguration = new ServiceConfiguration();
			if (!ServiceConfiguration.LoadAppSettingsConfigInt32("CheckProcessHandleTimeOut", out serviceConfiguration.checkProcessHandleTimeOut))
			{
				serviceConfiguration.checkProcessHandleTimeOut = 30;
			}
			else if (serviceConfiguration.checkProcessHandleTimeOut < ServiceConfiguration.CheckProcessHandleTimeOutMinimum || serviceConfiguration.checkProcessHandleTimeOut > ServiceConfiguration.CheckProcessHandleTimeOutMaximum)
			{
				serviceConfiguration.checkProcessHandleTimeOut = 30;
			}
			if (!ServiceConfiguration.LoadAppSettingsConfigBool("DisconnectTransportPerformanceCounters", out serviceConfiguration.disconnectTransportPerformanceCounters))
			{
				serviceConfiguration.disconnectTransportPerformanceCounters = ServiceConfiguration.DisconnectTransportPerformanceCountersDefault;
			}
			if (!ServiceConfiguration.LoadAppSettingsConfigInt32("MaxIOThreads", out serviceConfiguration.maxIOThreads))
			{
				serviceConfiguration.maxIOThreads = 0;
			}
			if (serviceConfiguration.maxIOThreads < ServiceConfiguration.MaxIOThreadsMinimum || serviceConfiguration.maxIOThreads > ServiceConfiguration.MaxIOThreadsMaximum)
			{
				serviceConfiguration.maxIOThreads = 30;
			}
			if (!ServiceConfiguration.LoadAppSettingsConfigInt32("MaxWorkerProcessThreads", out serviceConfiguration.maxWorkerProcessThreads))
			{
				serviceConfiguration.maxWorkerProcessThreads = 0;
			}
			if (!ServiceConfiguration.LoadAppSettingsConfigInt64("MaxWorkerProcessWorkingSet", out serviceConfiguration.maxWorkerProcessWorkingSet))
			{
				serviceConfiguration.maxWorkerProcessWorkingSet = 0L;
			}
			if (!ServiceConfiguration.LoadAppSettingsConfigInt32("MaxWorkerProcessRefreshInterval", out serviceConfiguration.maxWorkerProcessRefreshInterval))
			{
				serviceConfiguration.maxWorkerProcessRefreshInterval = 0;
			}
			if (!ServiceConfiguration.LoadAppSettingsConfigInt32("MaxWorkerProcessExitTimeout", out serviceConfiguration.maxWorkerProcessExitTimeout))
			{
				serviceConfiguration.maxWorkerProcessExitTimeout = processManagerService.MaxWorkerProcessExitTimeoutDefault;
			}
			if (!ServiceConfiguration.LoadAppSettingsConfigInt32("MaxProcessManagerRestartAttempts", out serviceConfiguration.maxProcessManagerRestartAttempts))
			{
				serviceConfiguration.maxProcessManagerRestartAttempts = 4;
			}
			if (!ServiceConfiguration.LoadAppSettingsConfigInt32("MaxProcessRestartAttemptsWhileInStartingState", out serviceConfiguration.maxProcessRestartAttemptsWhileInStartingState))
			{
				serviceConfiguration.maxProcessRestartAttemptsWhileInStartingState = 1;
			}
			if (!ServiceConfiguration.LoadAppSettingsConfigInt32("ThrashCrashMaximum", out serviceConfiguration.thrashCrashMaximum))
			{
				serviceConfiguration.thrashCrashMaximum = 3;
			}
			if (!ServiceConfiguration.LoadAppSettingsConfigInt32("MaxWorkerProcessExitTimeout", out serviceConfiguration.maxWorkerProcessExitTimeout))
			{
				serviceConfiguration.maxWorkerProcessExitTimeout = processManagerService.MaxWorkerProcessExitTimeoutDefault;
			}
			if (!ServiceConfiguration.LoadAppSettingsConfigInt32("MaxWorkerProcessDumpTimeout", out serviceConfiguration.maxWorkerProcessDumpTimeout))
			{
				serviceConfiguration.maxWorkerProcessDumpTimeout = processManagerService.MaxWorkerProcessDumpTimeoutDefault;
			}
			if (!ServiceConfiguration.LoadAppSettingsConfigBool("serviceListening", out serviceConfiguration.serviceListening))
			{
				serviceConfiguration.serviceListening = true;
			}
			return serviceConfiguration;
		}

		private static bool LoadAppSettingsConfigBool(string configName, out bool configvalue)
		{
			bool result = false;
			configvalue = false;
			string text = ConfigurationManager.AppSettings[configName];
			bool flag;
			if (text != null && bool.TryParse(text, out flag))
			{
				configvalue = flag;
				result = true;
			}
			return result;
		}

		private static bool LoadAppSettingsConfigInt32(string configName, out int configvalue)
		{
			bool result = false;
			configvalue = 0;
			string text = ConfigurationManager.AppSettings[configName];
			int num;
			if (text != null && int.TryParse(text, out num))
			{
				configvalue = num;
				result = true;
			}
			return result;
		}

		private static bool LoadAppSettingsConfigInt64(string configName, out long configvalue)
		{
			bool result = false;
			configvalue = 0L;
			string text = ConfigurationManager.AppSettings[configName];
			long num;
			if (text != null && long.TryParse(text, out num))
			{
				configvalue = num;
				result = true;
			}
			return result;
		}

		private const int CheckProcessHandleTimeOutDefault = 30;

		private const int MaxIOThreadsDefault = 30;

		private const int MaxProcessManagerRestartAttemptsDefault = 4;

		private const int MaxProcessRestartAttemptsWhileInStartingStateDefault = 1;

		private const int ThrashCrashMaximumDefault = 3;

		private const bool ServiceListeningDefault = true;

		private const string CheckProcessHandleTimeOutLabel = "CheckProcessHandleTimeOut";

		private const string DisconnectTransportPerformanceCountersLabel = "DisconnectTransportPerformanceCounters";

		private const string MaxIOThreadsLabel = "MaxIOThreads";

		private const string MaxWorkerProcessExitTimeoutLabel = "MaxWorkerProcessExitTimeout";

		private const string MaxWorkerProcessDumpTimeoutLabel = "MaxWorkerProcessDumpTimeout";

		private const string MaxWorkerProcessThreadsLabel = "MaxWorkerProcessThreads";

		private const string MaxWorkerProcessWorkingSetLabel = "MaxWorkerProcessWorkingSet";

		private const string MaxWorkerProcessRefreshIntervalLabel = "MaxWorkerProcessRefreshInterval";

		private const string MaxProcessManagerRestartAttemptsLabel = "MaxProcessManagerRestartAttempts";

		private const string MaxProcessRestartAttemptsWhileInStartingStateLabel = "MaxProcessRestartAttemptsWhileInStartingState";

		private const string ThrashCrashMaximumLabel = "ThrashCrashMaximum";

		private const string serviceListeningLabel = "serviceListening";

		private int checkProcessHandleTimeOut;

		private bool disconnectTransportPerformanceCounters;

		private int maxIOThreads;

		private int maxWorkerProcessThreads;

		private long maxWorkerProcessWorkingSet;

		private int maxWorkerProcessRefreshInterval;

		private int maxWorkerProcessExitTimeout;

		private int maxWorkerProcessDumpTimeout;

		private int maxProcessManagerRestartAttempts;

		private int maxProcessRestartAttemptsWhileInStartingState;

		private bool serviceListening;

		private static int CheckProcessHandleTimeOutMinimum = 0;

		private static int CheckProcessHandleTimeOutMaximum = 300;

		private static bool DisconnectTransportPerformanceCountersDefault = true;

		private static int MaxIOThreadsMinimum = Environment.ProcessorCount;

		private static int MaxIOThreadsMaximum = Environment.ProcessorCount * 100;

		private int thrashCrashMaximum;
	}
}
