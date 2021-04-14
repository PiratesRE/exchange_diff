using System;
using System.IO;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Isam.Esent.Interop.Unpublished;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class LogContinuityChecker
	{
		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.LogInspectorTracer;
			}
		}

		private bool Initialized { get; set; }

		public bool Examine(JET_LOGINFOMISC logInfo, string logFileName, out LocalizedString error)
		{
			error = LocalizedString.Empty;
			if (this.Initialized)
			{
				if (logInfo.ulGeneration != this.m_lastCheckedInfo.ulGeneration + 1)
				{
					error = ReplayStrings.FileCheckLogfileGeneration(logFileName, (long)logInfo.ulGeneration, (long)(this.m_lastCheckedInfo.ulGeneration + 1));
					return false;
				}
				if (logInfo.logtimePreviousGeneration != this.m_lastCheckedInfo.logtimeCreate)
				{
					error = ReplayStrings.FileCheckLogfileCreationTime(logFileName, logInfo.logtimePreviousGeneration.ToDateTime() ?? DateTime.MinValue, this.m_lastCheckedInfo.logtimeCreate.ToDateTime() ?? DateTime.MinValue);
					return false;
				}
			}
			else
			{
				this.Initialized = true;
			}
			this.m_lastCheckedInfo = logInfo;
			return true;
		}

		public void Initialize(long logGeneration, string logPath, string prefix, string suffix)
		{
			string text = Path.Combine(logPath, EseHelper.MakeLogfileName(prefix, suffix, logGeneration));
			this.Initialized = false;
			try
			{
				JET_LOGINFOMISC logInfo;
				UnpublishedApi.JetGetLogFileInfo(text, out logInfo, JET_LogInfo.Misc2);
				LocalizedString localizedString;
				this.Examine(logInfo, text, out localizedString);
			}
			catch (EsentErrorException ex)
			{
				LogContinuityChecker.Tracer.TraceError<string, EsentErrorException>((long)this.GetHashCode(), "LogContinuityChecker failed to init with '{0}': {1}", text, ex);
				throw new LogInspectorFailedGeneralException(text, ex.Message, ex);
			}
		}

		private JET_LOGINFOMISC m_lastCheckedInfo;
	}
}
