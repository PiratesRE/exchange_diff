using System;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class SimpleSetBroken : ISetBroken, ISetDisconnected, IReplicaProgress
	{
		public SimpleSetBroken(string database)
		{
			this.m_dbIdentity = database;
		}

		public bool Broken
		{
			get
			{
				return this.m_fBroken;
			}
		}

		public bool IsBroken
		{
			get
			{
				return this.Broken;
			}
		}

		public bool IsDisconnected
		{
			get
			{
				return this.m_fDisconnected;
			}
		}

		public LocalizedString ErrorMessage
		{
			get
			{
				return this.m_errorMessage;
			}
		}

		public Exception ExtendedErrorInformation
		{
			get
			{
				return this.m_exception;
			}
		}

		public void SetBroken(FailureTag failureTag, ExEventLog.EventTuple failureNotificationEventTuple, ExEventLog.EventTuple setBrokenEventTuple, params string[] setBrokenArgs)
		{
			this.SetBroken(failureTag, setBrokenEventTuple, setBrokenArgs);
		}

		public void SetBroken(FailureTag failureTag, ExEventLog.EventTuple failureNotificationEventTuple, string[] failureNotificationMessageArgs, ExEventLog.EventTuple setBrokenEventTuple, params string[] setBrokenArgs)
		{
			this.SetBroken(failureTag, setBrokenEventTuple, setBrokenArgs);
		}

		public void SetBroken(FailureTag failureTag, ExEventLog.EventTuple setBrokenEventTuple, params string[] setBrokenArgs)
		{
			this.m_fBroken = true;
			string[] argumentsWithDb = ReplicaInstance.GetArgumentsWithDb(setBrokenArgs, this.m_dbIdentity);
			int num;
			string value = setBrokenEventTuple.EventLogToString(out num, argumentsWithDb);
			this.m_errorMessage = new LocalizedString(value);
		}

		public void SetBroken(FailureTag failureTag, ExEventLog.EventTuple setBrokenEventTuple, Exception exception, params string[] setBrokenArgs)
		{
			this.m_exception = exception;
			this.SetBroken(failureTag, setBrokenEventTuple, setBrokenArgs);
		}

		public void SetBroken(FailureTag failureTag, ExEventLog.EventTuple failureNotificationEventTuple, ExEventLog.EventTuple setBrokenEventTuple, Exception exception, params string[] setBrokenArgs)
		{
			this.m_exception = exception;
			this.SetBroken(failureTag, failureNotificationEventTuple, setBrokenEventTuple, setBrokenArgs);
		}

		public void SetDisconnected(FailureTag failureTag, ExEventLog.EventTuple failureNotificationEventTuple, ExEventLog.EventTuple setBrokenEventTuple, params string[] setBrokenArgs)
		{
			this.SetDisconnected(failureTag, setBrokenEventTuple, setBrokenArgs);
		}

		public void SetDisconnected(FailureTag failureTag, ExEventLog.EventTuple setBrokenEventTuple, params string[] setBrokenArgs)
		{
			this.m_fDisconnected = true;
			string[] argumentsWithDb = ReplicaInstance.GetArgumentsWithDb(setBrokenArgs, this.m_dbIdentity);
			int num;
			string value = setBrokenEventTuple.EventLogToString(out num, argumentsWithDb);
			this.m_errorMessage = new LocalizedString(value);
		}

		public void ClearBroken()
		{
			this.m_fBroken = false;
		}

		public void ClearDisconnected()
		{
			this.m_fDisconnected = false;
		}

		public void RestartInstanceSoon(bool fPrepareToStop)
		{
		}

		public void RestartInstanceSoonAdminVisible()
		{
		}

		public void RestartInstanceNow(ReplayConfigChangeHints restartReason)
		{
		}

		public void ReportOneLogCopied()
		{
		}

		public void ReportLogsReplayed(long numLogs)
		{
		}

		private bool m_fBroken;

		private bool m_fDisconnected;

		private LocalizedString m_errorMessage;

		private string m_dbIdentity;

		private Exception m_exception;
	}
}
