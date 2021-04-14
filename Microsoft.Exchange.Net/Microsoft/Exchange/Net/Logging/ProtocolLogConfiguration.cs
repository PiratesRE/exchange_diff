using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.Logging
{
	internal sealed class ProtocolLogConfiguration
	{
		public ProtocolLogConfiguration(string logNameInitials)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("logNameInitials", logNameInitials);
			this.softwareName = "Microsoft Exchange Server";
			this.softwareVersion = Assembly.GetExecutingAssembly().GetName().Version;
			this.logTypeName = logNameInitials + " Protocol Logs";
			this.logFilePrefix = logNameInitials;
			this.logComponent = logNameInitials + "ProtocolLogs";
			string path = string.Format(CultureInfo.InvariantCulture, "TransportRoles\\Logs\\ProtocolLog\\{0}Client", new object[]
			{
				logNameInitials
			});
			this.logFilePath = Path.Combine(ProtocolLogConfiguration.exchangeInstallDirPath, path);
			this.ageQuota = 168L;
			this.directorySizeQuota = 256000L;
			this.perFileSizeQuota = 10240L;
			this.protocolLoggingLevel = ProtocolLoggingLevel.None;
		}

		public string SoftwareName
		{
			get
			{
				return this.softwareName;
			}
			set
			{
				ArgumentValidator.ThrowIfNull("SoftwareName", value);
				this.softwareName = value;
			}
		}

		public Version SoftwareVersion
		{
			get
			{
				return this.softwareVersion;
			}
			set
			{
				ArgumentValidator.ThrowIfNull("SoftwareVersion", value);
				this.softwareVersion = value;
			}
		}

		public string LogTypeName
		{
			get
			{
				return this.logTypeName;
			}
			set
			{
				ArgumentValidator.ThrowIfNull("LogTypeName", value);
				this.logTypeName = value;
			}
		}

		public string LogFilePrefix
		{
			get
			{
				return this.logFilePrefix;
			}
			set
			{
				ArgumentValidator.ThrowIfNull("LogFilePrefix", value);
				this.logFilePrefix = value;
			}
		}

		public string LogComponent
		{
			get
			{
				return this.logComponent;
			}
			set
			{
				ArgumentValidator.ThrowIfNull("LogComponent", value);
				this.logComponent = value;
			}
		}

		public bool IsEnabled
		{
			get
			{
				return this.isEnabled;
			}
			set
			{
				this.isEnabled = value;
			}
		}

		public string LogFilePath
		{
			get
			{
				return this.logFilePath;
			}
			set
			{
				ArgumentValidator.ThrowIfNullOrEmpty("LogFilePath", value);
				this.logFilePath = Path.Combine(ProtocolLogConfiguration.exchangeInstallDirPath, value);
			}
		}

		public long AgeQuota
		{
			get
			{
				return this.ageQuota;
			}
			set
			{
				ProtocolLogConfiguration.ThrowIfArgumentLessThanZero("AgeQuota", value);
				this.ageQuota = value;
			}
		}

		public long DirectorySizeQuota
		{
			get
			{
				return this.directorySizeQuota;
			}
			set
			{
				ProtocolLogConfiguration.ThrowIfArgumentLessThanZero("DirectorySizeQuota", value);
				this.directorySizeQuota = value;
			}
		}

		public long PerFileSizeQuota
		{
			get
			{
				return this.perFileSizeQuota;
			}
			set
			{
				ProtocolLogConfiguration.ThrowIfArgumentLessThanZero("PerFileSizeQuota", value);
				this.perFileSizeQuota = value;
			}
		}

		public ProtocolLoggingLevel ProtocolLoggingLevel
		{
			get
			{
				return this.protocolLoggingLevel;
			}
			set
			{
				this.protocolLoggingLevel = value;
			}
		}

		private static void ThrowIfArgumentLessThanZero(string name, long arg)
		{
			if (arg < 0L)
			{
				throw new ArgumentOutOfRangeException(name, arg, "The value is set to less than 0.");
			}
		}

		private static string exchangeInstallDirPath = Assembly.GetExecutingAssembly().Location + "\\..\\..\\";

		private string softwareName;

		private Version softwareVersion;

		private string logTypeName;

		private string logFilePrefix;

		private string logComponent;

		private bool isEnabled;

		private string logFilePath;

		private long ageQuota;

		private long directorySizeQuota;

		private long perFileSizeQuota;

		private ProtocolLoggingLevel protocolLoggingLevel;
	}
}
