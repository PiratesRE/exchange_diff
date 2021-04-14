using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Win32;

namespace Microsoft.Exchange.Compliance.Logging
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class GccProtocolActivityLogger
	{
		public GccProtocolActivityLogger(string protocol)
		{
			if (GccProtocolActivityLogger.schema == null)
			{
				GccProtocolActivityLogger.schema = new LogSchema("Microsoft Exchange Server", GccProtocolActivityLogger.version, "Global Criminal Compliance Log", GccProtocolActivityLogger.Fields);
			}
			this.protocol = protocol;
			this.notifyReadyToClose = new ManualResetEvent(true);
			this.notifyDoneClosing = new ManualResetEvent(false);
		}

		public static GccProtocolActivityLoggerConfig Config
		{
			get
			{
				if (GccProtocolActivityLogger.config == null)
				{
					GccProtocolActivityLogger.config = new GccProtocolActivityLoggerConfig();
				}
				return GccProtocolActivityLogger.config;
			}
			internal set
			{
				GccProtocolActivityLogger.config = value;
			}
		}

		public void Initialize()
		{
			string path = Regex.Replace(this.protocol, "[/\\:\\\\]", "_");
			string path2 = Path.Combine(GccProtocolActivityLogger.Config.LogRoot, path);
			this.log = new Log("gcc", new LogHeaderFormatter(GccProtocolActivityLogger.schema), "gcc-" + this.protocol, true);
			this.log.Configure(path2, GccProtocolActivityLogger.Config.MaxAge, GccProtocolActivityLogger.Config.MaxDirectorySize, GccProtocolActivityLogger.Config.MaxLogfileSize);
			this.rowFormatter = new LogRowFormatter(GccProtocolActivityLogger.schema);
			this.rowFormatter[1] = this.protocol;
		}

		public void Append(string account, IPAddress clientIP, IPAddress serverIP, ExDateTime accessTimestamp, TimeSpan accessDuration, bool messageDownload)
		{
			if (this.shuttingDown != 0)
			{
				return;
			}
			try
			{
				if (Interlocked.Increment(ref this.callerCount) == 1)
				{
					this.notifyReadyToClose.Reset();
				}
				this.rowFormatter[2] = account;
				this.rowFormatter[3] = clientIP;
				this.rowFormatter[4] = serverIP;
				this.rowFormatter[5] = accessTimestamp;
				this.rowFormatter[6] = accessDuration;
				this.rowFormatter[7] = messageDownload;
				this.log.Append(this.rowFormatter, 0);
			}
			finally
			{
				if (Interlocked.Decrement(ref this.callerCount) == 0)
				{
					this.notifyReadyToClose.Set();
				}
			}
		}

		public void Close()
		{
			int num = Interlocked.Exchange(ref this.shuttingDown, 1);
			this.notifyReadyToClose.WaitOne();
			if (num == 0)
			{
				try
				{
					if (this.log != null)
					{
						this.log.Close();
						this.log = null;
					}
					return;
				}
				finally
				{
					this.notifyDoneClosing.Set();
				}
			}
			this.notifyDoneClosing.WaitOne();
		}

		private static string GetExchangeInstallPath()
		{
			return (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup", "MsiInstallPath", null);
		}

		private static string GetExchangeVersion()
		{
			int num = (int)(Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup", "MsiProductMajor", null) ?? 14);
			int num2 = (int)(Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup", "MsiProductMinor", null) ?? 0);
			int num3 = (int)(Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup", "MsiBuildMajor", null) ?? 0);
			int num4 = (int)(Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup", "MsiBuildMinor", null) ?? 0);
			return string.Format("{0:00}.{1:00}.{2:0000}.{3:000}", new object[]
			{
				num,
				num2,
				num3,
				num4
			});
		}

		private const string ExchangeSetupRegkey = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup";

		private static readonly string[] Fields = new string[]
		{
			"time",
			"protocol",
			"account",
			"client-ip",
			"server-ip",
			"access-timestamp",
			"access-duration",
			"message-download"
		};

		private static readonly string version = GccProtocolActivityLogger.GetExchangeVersion();

		private static LogSchema schema;

		private static GccProtocolActivityLoggerConfig config;

		private Log log;

		private LogRowFormatter rowFormatter;

		private string protocol;

		private int shuttingDown;

		private int callerCount;

		private ManualResetEvent notifyReadyToClose;

		private ManualResetEvent notifyDoneClosing;

		internal enum Field
		{
			Time,
			Protocol,
			Account,
			ClientIP,
			ServerIP,
			AccessTimestamp,
			AccessDuration,
			MessageDownload
		}
	}
}
