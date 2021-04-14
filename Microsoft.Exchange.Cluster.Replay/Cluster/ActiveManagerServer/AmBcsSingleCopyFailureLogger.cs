using System;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AmBcsSingleCopyFailureLogger : IAmBcsErrorLogger
	{
		private string ErrorMessage { get; set; }

		public bool IsFailed()
		{
			return this.IsFailedForServer(null);
		}

		public bool IsFailedForServer(AmServerName server)
		{
			return !string.IsNullOrEmpty(this.ErrorMessage);
		}

		public void ReportCopyStatusFailure(AmServerName server, string statusCheckThatFailed, string checksRun, string errorMessage)
		{
			this.SetErrorIfApplicable(false, errorMessage);
		}

		public void ReportCopyStatusFailure(AmServerName server, string statusCheckThatFailed, string checksRun, string errorMessage, ReplayCrimsonEvent evt, params object[] evtArgs)
		{
			this.SetErrorIfApplicable(false, errorMessage);
		}

		public void ReportServerFailure(AmServerName server, string serverCheckThatFailed, string errorMessage)
		{
			this.SetErrorIfApplicable(false, errorMessage);
		}

		public void ReportServerFailure(AmServerName server, string serverCheckThatFailed, string errorMessage, bool overwriteAllowed)
		{
			this.SetErrorIfApplicable(overwriteAllowed, errorMessage);
		}

		public void ReportServerFailure(AmServerName server, string serverCheckThatFailed, string errorMessage, ReplayCrimsonEvent evt, params object[] evtArgs)
		{
			this.SetErrorIfApplicable(false, errorMessage);
		}

		public Exception GetLastException()
		{
			if (string.IsNullOrEmpty(this.ErrorMessage))
			{
				return null;
			}
			return new AmBcsSingleCopyValidationException(this.ErrorMessage);
		}

		public string[] GetAllExceptions()
		{
			throw new NotImplementedException();
		}

		private void SetErrorIfApplicable(bool overwriteAllowed, string errorMessage)
		{
			if (overwriteAllowed || !this.IsFailed())
			{
				this.ErrorMessage = errorMessage;
			}
		}
	}
}
