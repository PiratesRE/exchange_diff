using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Directory.Diagnostics
{
	internal abstract class BaseDirectoryProtocolLog
	{
		protected static bool LoggingEnabled
		{
			get
			{
				return BaseDirectoryProtocolLog.loggingEnabled.Value;
			}
		}

		private protected bool Initialized { protected get; private set; }

		protected abstract LogSchema Schema { get; }

		protected Log Logger
		{
			get
			{
				return this.log;
			}
		}

		protected void Initialize(ExDateTime serviceStartTime, string logFilePath, TimeSpan maxRetentionPeriond, ByteQuantifiedSize directorySizeQuota, ByteQuantifiedSize perFileSizeQuota, bool applyHourPrecision, string logComponent)
		{
			if (this.Initialized)
			{
				throw new NotSupportedException("Protocol Log is already initialized");
			}
			BaseDirectoryProtocolLog.InitializeGlobalConfigIfRequired();
			if (this.log == null)
			{
				this.log = new Log(BaseDirectoryProtocolLog.logFilePrefix, new LogHeaderFormatter(this.Schema, LogHeaderCsvOption.CsvCompatible), logComponent);
				AppDomain.CurrentDomain.ProcessExit += this.CurrentDomainProcessExit;
			}
			if (BaseDirectoryProtocolLog.loggingEnabled.Value)
			{
				this.log.Configure(logFilePath, maxRetentionPeriond, (long)directorySizeQuota.ToBytes(), (long)perFileSizeQuota.ToBytes(), applyHourPrecision, BaseDirectoryProtocolLog.bufferSize, BaseDirectoryProtocolLog.DefaultFlushInterval, LogFileRollOver.Hourly);
			}
			BaseDirectoryProtocolLog.callsBacks = (TimerCallback)Delegate.Combine(BaseDirectoryProtocolLog.callsBacks, new TimerCallback(this.UpdateConfigIfChanged));
			this.Initialized = true;
		}

		protected virtual void UpdateConfigIfChanged(object state)
		{
			if ((bool)state)
			{
				this.Initialized = false;
			}
		}

		protected int GetNextSequenceNumber()
		{
			int result = Interlocked.Increment(ref this.sequenceNumber);
			if (0 > this.sequenceNumber)
			{
				this.sequenceNumber = 0;
				result = 0;
			}
			return result;
		}

		protected static bool GetRegistryBool(RegistryKey regkey, string key, bool defaultValue)
		{
			int? num = null;
			if (regkey != null)
			{
				num = (regkey.GetValue(key) as int?);
			}
			if (num == null)
			{
				return defaultValue;
			}
			return Convert.ToBoolean(num.Value);
		}

		private void CurrentDomainProcessExit(object sender, EventArgs e)
		{
			this.Initialized = false;
			this.Shutdown();
		}

		private void Shutdown()
		{
			if (this.log != null)
			{
				this.log.Close();
			}
		}

		protected static string[] GetColumnArray(BaseDirectoryProtocolLog.FieldInfo[] fields)
		{
			string[] array = new string[fields.Length];
			for (int i = 0; i < fields.Length; i++)
			{
				array[i] = fields[i].ColumnName;
			}
			return array;
		}

		protected static string GetExchangeInstallPath()
		{
			string result = string.Empty;
			try
			{
				result = ExchangeSetupContext.InstallPath;
			}
			catch
			{
			}
			return result;
		}

		private static void InitializeGlobalConfigIfRequired()
		{
			lock (BaseDirectoryProtocolLog.globalLock)
			{
				if (BaseDirectoryProtocolLog.bufferSize == 0 || BaseDirectoryProtocolLog.loggingEnabled == null)
				{
					BaseDirectoryProtocolLog.ReadGlobalConfig();
					AppDomain.CurrentDomain.ProcessExit += delegate(object x, EventArgs y)
					{
						BaseDirectoryProtocolLog.loggingEnabled = new bool?(false);
						if (BaseDirectoryProtocolLog.timer != null)
						{
							BaseDirectoryProtocolLog.timer.Dispose();
						}
					};
				}
				if (BaseDirectoryProtocolLog.registryWatcher == null)
				{
					BaseDirectoryProtocolLog.registryWatcher = new RegistryWatcher("SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess\\Parameters", false);
				}
				if (BaseDirectoryProtocolLog.timer == null)
				{
					BaseDirectoryProtocolLog.timer = new Timer(new TimerCallback(BaseDirectoryProtocolLog.GlobalUpdateConfigIfChanged), null, 0, 300000);
				}
			}
		}

		private static void GlobalUpdateConfigIfChanged(object state)
		{
			if (BaseDirectoryProtocolLog.registryWatcher.IsChanged())
			{
				bool flag = false;
				using (Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess\\Parameters"))
				{
					bool value = BaseDirectoryProtocolLog.loggingEnabled.Value;
					BaseDirectoryProtocolLog.ReadGlobalConfig();
					flag = (value != BaseDirectoryProtocolLog.loggingEnabled.Value);
				}
				TimerCallback timerCallback = BaseDirectoryProtocolLog.callsBacks;
				if (timerCallback != null)
				{
					timerCallback(flag);
				}
			}
		}

		private static void ReadGlobalConfig()
		{
			BaseDirectoryProtocolLog.bufferSize = Globals.GetIntValueFromRegistry("SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess\\Parameters", "LogBufferSize", 1048576, 0);
			int intValueFromRegistry = Globals.GetIntValueFromRegistry("SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess\\Parameters", "FlushIntervalInMinutes", 15, 0);
			if (intValueFromRegistry > 0)
			{
				BaseDirectoryProtocolLog.DefaultFlushInterval = TimeSpan.FromMinutes((double)intValueFromRegistry);
			}
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess\\Parameters"))
			{
				BaseDirectoryProtocolLog.loggingEnabled = new bool?(BaseDirectoryProtocolLog.GetRegistryBool(registryKey, "ProtocolLoggingEnabled", true));
			}
		}

		private const int DefaultLogBufferSize = 1048576;

		protected const string RegistryParameters = "SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess\\Parameters";

		private const string LoggingEnabledRegKeyName = "ProtocolLoggingEnabled";

		private const string LogBufferSizeRegKeyName = "LogBufferSize";

		private const string LogFlushIntervalRegKeyName = "FlushIntervalInMinutes";

		protected static readonly TimeSpan DefaultMaxRetentionPeriod = TimeSpan.FromHours(8.0);

		protected static readonly ByteQuantifiedSize DefaultDirectorySizeQuota = ByteQuantifiedSize.Parse("200MB");

		protected static readonly ByteQuantifiedSize DefaultPerFileSizeQuota = ByteQuantifiedSize.Parse("10MB");

		protected static TimeSpan DefaultFlushInterval = TimeSpan.FromMinutes(15.0);

		private static readonly string logFilePrefix = Globals.ProcessName + "_" + Globals.ProcessAppName + "_";

		private static object globalLock = new object();

		private static Timer timer;

		private static int bufferSize;

		private static bool? loggingEnabled;

		private static RegistryWatcher registryWatcher;

		private static TimerCallback callsBacks;

		private int sequenceNumber;

		private Log log;

		internal struct FieldInfo
		{
			public FieldInfo(byte field, string columnName)
			{
				this.Field = field;
				this.ColumnName = columnName;
			}

			internal readonly byte Field;

			internal readonly string ColumnName;
		}
	}
}
