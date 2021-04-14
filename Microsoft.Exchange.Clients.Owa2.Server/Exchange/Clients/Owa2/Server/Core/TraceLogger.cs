using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.SharePointSignalStore;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class TraceLogger : ILogger
	{
		public TraceLogger(bool silent = false)
		{
			this.silent = silent;
			if (silent)
			{
				return;
			}
			this.logSchema = new LogSchema("Microsoft Exchange Server", Assembly.GetExecutingAssembly().GetName().Version.ToString(), "SharePoint Activity Logger Diagnostics Logs", this.columns);
			this.logger = new Log("SendLinkClickedSignalToSP_" + Process.GetCurrentProcess().Id + "_", new LogHeaderFormatter(this.logSchema), "SendLinkClickedSignalToSP");
			this.logger.Configure(Path.Combine(ExchangeSetupContext.InstallPath, "Logging\\SendLinkClickedSignalToSP"), TimeSpan.FromDays(3.0), 262144000L, 10485760L);
		}

		void ILogger.LogInfo(string format, params object[] args)
		{
			this.Log(TraceLogger.LoggingLevel.Information, format, args);
		}

		void ILogger.LogWarning(string format, params object[] args)
		{
			this.Log(TraceLogger.LoggingLevel.Warning, format, args);
		}

		private void Log(TraceLogger.LoggingLevel level, string format, params object[] args)
		{
			if (this.silent)
			{
				return;
			}
			LogRowFormatter logRowFormatter = new LogRowFormatter(this.logSchema);
			logRowFormatter[0] = DateTime.UtcNow;
			logRowFormatter[1] = level;
			logRowFormatter[2] = ((args.Length != 0) ? new StringBuilder(format.Length).AppendFormat(format, args).ToString() : format);
			this.logger.Append(logRowFormatter, -1);
		}

		public void Close()
		{
			if (this.silent)
			{
				return;
			}
			this.logger.Flush();
			this.logger.Close();
		}

		private readonly string[] columns = new string[]
		{
			"date-time",
			"level",
			"message"
		};

		private Log logger;

		private LogSchema logSchema;

		private readonly bool silent;

		public enum LoggingLevel
		{
			Information,
			Warning
		}
	}
}
