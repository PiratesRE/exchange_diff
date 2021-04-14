using System;
using System.IO;
using System.Reflection;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport.RightsManagement
{
	internal class E4eLog : IE4eLogger
	{
		internal static E4eLog Instance
		{
			get
			{
				return E4eLog.instance;
			}
		}

		private E4eLog()
		{
			this.enabled = false;
			try
			{
				string[] array = new string[4];
				for (int i = 0; i < 4; i++)
				{
					array[i] = ((E4eLogField)i).ToString();
				}
				this.logSchema = new LogSchema("Microsoft Exchange Server", Assembly.GetExecutingAssembly().GetName().Version.ToString(), "E4E Agent Log", array);
				this.log = new Log("E4EAgent", new LogHeaderFormatter(this.logSchema), "E4EAgentLogs");
				string installPath = ExchangeSetupContext.InstallPath;
				if (string.IsNullOrEmpty(installPath))
				{
					throw new Exception("Install path is empty");
				}
				this.log.Configure(Path.Combine(installPath, E4eLog.DefaultLogPath), E4eLog.DefaultMaxAge, (long)E4eLog.DefaultMaxDirectorySize.ToBytes(), (long)E4eLog.DefaultMaxLogFileSize.ToBytes(), E4eLog.DefaultBufferSize, E4eLog.DefaultStreamFlushInterval);
				this.enabled = true;
			}
			catch (Exception ex)
			{
				ExTraceGlobals.GeneralTracer.TraceDebug<string>(0L, "E4e Agent Log is disabled - {0}", ex.ToString());
				this.enabled = false;
			}
		}

		internal void LogInfo(string messageID, string formatString, params object[] args)
		{
			if (this.enabled)
			{
				LogRowFormatter logRowFormatter = new LogRowFormatter(this.logSchema);
				logRowFormatter[1] = E4eLogType.Info;
				logRowFormatter[2] = messageID;
				logRowFormatter[3] = string.Format(formatString, args);
				this.log.Append(logRowFormatter, 0);
				return;
			}
			ExTraceGlobals.HostedEncryptionTracer.TraceInformation(0, 0L, formatString, args);
		}

		internal void LogError(string messageID, string formatString, params object[] args)
		{
			if (this.enabled)
			{
				LogRowFormatter logRowFormatter = new LogRowFormatter(this.logSchema);
				logRowFormatter[1] = E4eLogType.Error;
				logRowFormatter[2] = messageID;
				logRowFormatter[3] = string.Format(formatString, args);
				this.log.Append(logRowFormatter, 0);
				return;
			}
			ExTraceGlobals.HostedEncryptionTracer.TraceInformation(0, 0L, formatString, args);
		}

		public void LogInfo(HttpContext context, string methodName, string messageID, string messageFormat, params object[] args)
		{
			this.LogInfo(messageID, messageFormat, args);
		}

		public void LogError(HttpContext context, string methodName, string messageID, string messageFormat, params object[] args)
		{
			this.LogError(messageID, messageFormat, args);
		}

		private static readonly string DefaultLogPath = "Logging\\E4E\\Agent";

		private static readonly TimeSpan DefaultMaxAge = TimeSpan.FromDays(30.0);

		private static readonly ByteQuantifiedSize DefaultMaxDirectorySize = ByteQuantifiedSize.Parse("200MB");

		private static readonly ByteQuantifiedSize DefaultMaxLogFileSize = ByteQuantifiedSize.Parse("10MB");

		private static readonly int DefaultBufferSize = 1048576;

		private static readonly TimeSpan DefaultStreamFlushInterval = TimeSpan.FromMinutes(1.0);

		private static E4eLog instance = new E4eLog();

		private LogSchema logSchema;

		private Log log;

		private readonly bool enabled;
	}
}
