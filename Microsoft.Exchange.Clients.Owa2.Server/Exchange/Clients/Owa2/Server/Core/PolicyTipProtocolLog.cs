using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PolicyTipProtocolLog
	{
		internal static void WriteToLog(string correlationId, string stage, string data, string extraData, TimeSpan elapsedTime, string outerExceptionType, string outerExceptionMessage, string innerExceptionType, string innerExceptionMessage, string exceptionChain)
		{
			PolicyTipProtocolLog.InitializeIfNeeded();
			LogRowFormatter logRowFormatter = new LogRowFormatter(PolicyTipProtocolLog.LogSchema);
			logRowFormatter[1] = Environment.MachineName;
			logRowFormatter[2] = correlationId;
			logRowFormatter[3] = stage;
			logRowFormatter[5] = data;
			logRowFormatter[6] = extraData;
			logRowFormatter[4] = elapsedTime.TotalMilliseconds;
			logRowFormatter[7] = outerExceptionType;
			logRowFormatter[8] = outerExceptionMessage;
			logRowFormatter[9] = innerExceptionType;
			logRowFormatter[10] = innerExceptionMessage;
			logRowFormatter[11] = exceptionChain;
			PolicyTipProtocolLog.instance.logInstance.Append(logRowFormatter, 0);
		}

		internal static string GetExceptionLogString(Exception e)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e is null");
			}
			string empty = string.Empty;
			List<string> list = null;
			List<string> list2 = null;
			PolicyTipProtocolLog.GetExceptionTypeAndDetails(e, out list, out list2, out empty, true);
			return empty;
		}

		internal static void GetExceptionTypeAndDetails(Exception e, out List<string> types, out List<string> messages, out string chain, bool chainOnly)
		{
			Exception ex = e;
			chain = string.Empty;
			types = null;
			messages = null;
			if (!chainOnly)
			{
				types = new List<string>();
				messages = new List<string>();
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num = 1;
			for (;;)
			{
				string text = ex.GetType().ToString();
				string text2 = ex.Message;
				if (ex is SharePointException && ex.InnerException != null && ex.InnerException is WebException)
				{
					text = text + "; WebException:" + text2;
					text2 = text2 + "; DiagnosticInfo:" + ((SharePointException)ex).DiagnosticInfo;
				}
				if (!chainOnly)
				{
					types.Add(text);
					messages.Add(text2);
				}
				stringBuilder.Append("[Type:");
				stringBuilder.Append(text);
				stringBuilder.Append("]");
				stringBuilder.Append("[Message:");
				stringBuilder.Append(text2);
				stringBuilder.Append("]");
				stringBuilder.Append("[Stack:");
				stringBuilder.Append(string.IsNullOrEmpty(ex.StackTrace) ? string.Empty : ex.StackTrace.Replace("\r\n", string.Empty));
				stringBuilder.Append("]");
				if (ex.InnerException == null || num > 10)
				{
					break;
				}
				ex = ex.InnerException;
				num++;
			}
			chain = stringBuilder.ToString();
		}

		private static void InitializeIfNeeded()
		{
			if (!PolicyTipProtocolLog.instance.initialized)
			{
				lock (PolicyTipProtocolLog.instance.initializeLockObject)
				{
					if (!PolicyTipProtocolLog.instance.initialized)
					{
						PolicyTipProtocolLog.instance.Initialize();
						PolicyTipProtocolLog.instance.initialized = true;
					}
				}
			}
		}

		private void Initialize()
		{
			PolicyTipProtocolLog.instance.logInstance = new Log(PolicyTipProtocolLog.GetLogFileName(), new LogHeaderFormatter(PolicyTipProtocolLog.LogSchema), "OwaPolicyTipLog");
			PolicyTipProtocolLog.instance.logInstance.Configure(Path.Combine(ExchangeSetupContext.InstallPath, "Logging\\PolicyTip\\"), PolicyTipProtocolLog.LogMaxAge, 262144000L, 10485760L);
		}

		public static string GetLogFileName()
		{
			string result;
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				result = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", new object[]
				{
					currentProcess.ProcessName,
					"OwaPolicyTipLog"
				});
			}
			return result;
		}

		internal const string CRLF = "\r\n";

		private const string DefaultLogPath = "Logging\\PolicyTip\\";

		private const string LogType = "PolicyTip Log";

		private const string LogComponent = "OwaPolicyTipLog";

		private const string LogSuffix = "OwaPolicyTipLog";

		private const int MaxLogDirectorySize = 262144000;

		private const int MaxLogFileSize = 10485760;

		private static readonly EnhancedTimeSpan LogMaxAge = EnhancedTimeSpan.FromDays(30.0);

		private static readonly PolicyTipProtocolLog instance = new PolicyTipProtocolLog();

		private static readonly string[] Fields = new string[]
		{
			"timestamp",
			"server",
			"correlation-id",
			"stage",
			"elapsed",
			"data",
			"extra-data",
			"outerexception-type",
			"outerexception-message",
			"innerexception-type",
			"innerexception-message",
			"exceptionchain"
		};

		private static readonly LogSchema LogSchema = new LogSchema("Microsoft Exchange Server", Assembly.GetExecutingAssembly().GetName().Version.ToString(), "PolicyTip Log", PolicyTipProtocolLog.Fields);

		private readonly object initializeLockObject = new object();

		private Log logInstance;

		private bool initialized;

		private enum Field
		{
			TimeStamp,
			MachineName,
			CorrelationId,
			Stage,
			Elapsed,
			Data,
			ExtraData,
			OuterExceptionType,
			OuterExceptionMessage,
			InnerExceptionType,
			InnerExceptionMessage,
			ExceptionChain
		}
	}
}
