using System;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Search.Core.Diagnostics
{
	internal class DiagnosticsLogConfig : Config, IDiagnosticsLogConfig, IConfig
	{
		public DiagnosticsLogConfig(DiagnosticsLogConfig.LogDefaults logDefaults)
		{
			this.DefaultEventLogComponentGuid = logDefaults.EventLogComponentGuid;
			this.DefaultServiceName = logDefaults.ServiceName;
			this.DefaultLogTypeName = logDefaults.LogTypeName;
			this.DefaultLogFilePath = logDefaults.LogFilePath;
			this.DefaultLogFilePrefix = logDefaults.LogFilePrefix;
			this.DefaultLogComponent = logDefaults.LogComponent;
			this.ConfigurableLoad();
		}

		public Guid EventLogComponentGuid { get; private set; }

		public bool IsEnabled { get; private set; }

		public string ServiceName { get; private set; }

		public string LogTypeName { get; private set; }

		public string LogFilePath { get; private set; }

		public string LogFilePrefix { get; private set; }

		public string LogComponent { get; private set; }

		public int MaxAge { get; private set; }

		public int MaxDirectorySize { get; private set; }

		public int MaxFileSize { get; private set; }

		public bool ApplyHourPrecision { get; private set; }

		public bool IncludeExtendedLogging { get; private set; }

		public int ExtendedLoggingSize { get; private set; }

		public DiagnosticsLoggingTag DiagnosticsLoggingTag { get; private set; }

		private Guid DefaultEventLogComponentGuid
		{
			get
			{
				return this.defaultEventLogComponentGuid;
			}
			set
			{
				this.defaultEventLogComponentGuid = value;
			}
		}

		private string DefaultServiceName
		{
			get
			{
				return this.defaultServiceName;
			}
			set
			{
				this.defaultServiceName = value;
			}
		}

		private string DefaultLogTypeName
		{
			get
			{
				return this.defaultLogTypeName;
			}
			set
			{
				this.defaultLogTypeName = value;
			}
		}

		private string DefaultLogFilePath
		{
			get
			{
				return this.defaultLogFilePath;
			}
			set
			{
				this.defaultLogFilePath = value;
			}
		}

		private string DefaultLogFilePrefix
		{
			get
			{
				return this.defaultLogFilePrefix;
			}
			set
			{
				this.defaultLogFilePrefix = value;
			}
		}

		private string DefaultLogComponent
		{
			get
			{
				return this.defaultLogComponent;
			}
			set
			{
				this.defaultLogComponent = value;
			}
		}

		public override void Load()
		{
		}

		private void ConfigurableLoad()
		{
			this.EventLogComponentGuid = this.DefaultEventLogComponentGuid;
			if (this.EventLogComponentGuid == Guid.Empty)
			{
				throw new ArgumentNullException("eventLogComponentGuid must be set.");
			}
			this.ServiceName = this.DefaultServiceName;
			Util.ThrowOnNullOrEmptyArgument(this.ServiceName, "serviceName must be set.");
			this.LogTypeName = base.ReadString("logTypeName", this.DefaultLogTypeName);
			Util.ThrowOnNullOrEmptyArgument(this.LogTypeName, "logTypeName config key must be set.");
			this.LogFilePath = base.ReadString("logFilePath", this.DefaultLogFilePath);
			Util.ThrowOnNullOrEmptyArgument(this.LogFilePath, "logFilePath config key must be set.");
			this.LogFilePrefix = base.ReadString("logFilePrefix", this.DefaultLogFilePrefix);
			Util.ThrowOnNullOrEmptyArgument(this.LogFilePrefix, "logFilePrefix config key must be set.");
			this.LogComponent = base.ReadString("logComponent", this.DefaultLogComponent);
			Util.ThrowOnNullOrEmptyArgument(this.LogComponent, "logComponent config key must be set.");
			this.IsEnabled = base.ReadBool("fileLoggingEnabled", true);
			this.MaxAge = base.ReadInt("ageQuota", 1440);
			this.MaxDirectorySize = base.ReadInt("maxDirectorySize", 1048576);
			this.MaxFileSize = base.ReadInt("maxFileSize", 10240);
			this.ApplyHourPrecision = base.ReadBool("applyHourPrecision", true);
			this.IncludeExtendedLogging = base.ReadBool("includeExtendedLogging", true);
			this.DiagnosticsLoggingTag = (DiagnosticsLoggingTag)base.ReadInt("diagnosticsLoggingTag", 7);
			this.ExtendedLoggingSize = base.ReadInt("extendedLoggingSize", 16);
			if (this.ExtendedLoggingSize != 8 && this.ExtendedLoggingSize != 16 && this.ExtendedLoggingSize != 32 && this.ExtendedLoggingSize != 64 && this.ExtendedLoggingSize != 128 && this.ExtendedLoggingSize != 256)
			{
				throw new ArgumentException("Extended logging size must be (8 | 16 | 32 | 64 | 128 | 256).", "extendedLoggingSize");
			}
		}

		internal const int DefaultExtendedLoggingSize = 16;

		private const bool DefaultFileLoggingEnabled = true;

		private const int DefaultMaxAge = 1440;

		private const int DefaultMaxDirectorySize = 1048576;

		private const int DefaultMaxFileSize = 10240;

		private const bool DefaultApplyHourPrecision = true;

		private const bool DefaultIncludeExtendedLogging = true;

		private const int DefaultDiagnosticsLoggingTag = 7;

		private Guid defaultEventLogComponentGuid = Guid.Empty;

		private string defaultServiceName;

		private string defaultLogTypeName;

		private string defaultLogFilePath;

		private string defaultLogFilePrefix;

		private string defaultLogComponent;

		internal class LogDefaults
		{
			internal LogDefaults(Guid eventLogComponentGuid, string serviceName, string logTypeName, string logFilePath, string logFilePrefix, string logComponent)
			{
				this.EventLogComponentGuid = eventLogComponentGuid;
				this.ServiceName = serviceName;
				this.LogTypeName = logTypeName;
				this.LogFilePath = logFilePath;
				this.LogFilePrefix = logFilePrefix;
				this.LogComponent = logComponent;
			}

			internal Guid EventLogComponentGuid { get; set; }

			internal string ServiceName { get; set; }

			internal string LogTypeName { get; set; }

			internal string LogFilePath { get; set; }

			internal string LogFilePrefix { get; set; }

			internal string LogComponent { get; set; }
		}
	}
}
