using System;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class HaTaskStringBuilderOutputHelper : ITaskOutputHelper, ILogTraceHelper, IClusterSetupProgress
	{
		internal HaTaskStringBuilderOutputHelper(string taskName)
		{
			this.m_taskName = taskName;
			this.m_writer = new StringBuilder(2048);
		}

		public Exception LastException
		{
			get
			{
				return this.m_lastException;
			}
			set
			{
				this.m_lastException = value;
			}
		}

		public int MaxPercentageDuringCallback
		{
			get
			{
				return this.m_maxPercentageDuringCallback;
			}
			set
			{
				this.m_maxPercentageDuringCallback = value;
			}
		}

		public void AppendLogMessage(LocalizedString locMessage)
		{
			string arg = DateTime.UtcNow.ToString("s");
			if (this.m_writer != null)
			{
				this.m_writer.AppendFormat("[{0}] {1}", arg, locMessage.ToString());
				this.m_writer.AppendLine();
			}
		}

		public void AppendLogMessage(string englishMessage, params object[] args)
		{
			string arg = DateTime.UtcNow.ToString("s");
			if (this.m_writer != null)
			{
				this.m_writer.AppendFormat("[{0}] ", arg);
				this.m_writer.AppendFormat(englishMessage, args);
				this.m_writer.AppendLine();
			}
		}

		public void WriteErrorSimple(Exception error)
		{
			this.WriteErrorEx(error, ErrorCategory.InvalidArgument, null);
		}

		public void WriteVerbose(LocalizedString locString)
		{
			this.WriteVerboseEx(locString);
		}

		public void WriteWarning(LocalizedString locString)
		{
			this.WriteWarningEx(locString);
		}

		public void WriteProgressSimple(LocalizedString locString)
		{
			this.WriteProgressIncrementalSimple(locString, 2);
		}

		public int ClusterSetupProgressCallback(IntPtr pvCallbackArg, ClusapiMethods.CLUSTER_SETUP_PHASE eSetupPhase, ClusapiMethods.CLUSTER_SETUP_PHASE_TYPE ePhaseType, ClusapiMethods.CLUSTER_SETUP_PHASE_SEVERITY ePhaseSeverity, uint dwPercentComplete, string lpszObjectName, uint dwStatus)
		{
			this.AppendLogMessage("ClusterSetupProgressCallback( eSetupPhase = {0}, ePhaseType = {1}, ePhaseSeverity = {2}, dwPercentComplete = {3}, szObjectName = {4}, dwStatus = 0x{5:x} )", new object[]
			{
				eSetupPhase,
				ePhaseType,
				ePhaseSeverity,
				dwPercentComplete,
				lpszObjectName,
				dwStatus
			});
			this.m_maxPercentageDuringCallback = Math.Max(this.m_maxPercentageDuringCallback, (int)dwPercentComplete);
			Exception ex = HaTaskCallbackHelper.LookUpStatus(eSetupPhase, ePhaseType, ePhaseSeverity, dwPercentComplete, lpszObjectName, dwStatus);
			if (ex != null)
			{
				this.m_lastException = ex;
				this.AppendLogMessage("Found a matching exception: {0}", new object[]
				{
					ex
				});
			}
			return 1;
		}

		public override string ToString()
		{
			if (this.m_writer != null)
			{
				this.m_logContents = this.m_writer.ToString();
			}
			return this.m_logContents;
		}

		internal void CreateTempLogFile()
		{
			this.m_writer.AppendFormat("{0} started on machine {1}.", this.m_taskName, Environment.MachineName);
			this.m_writer.AppendLine();
		}

		internal void CloseTempLogFile()
		{
			if (this.m_writer != null)
			{
				this.m_writer.AppendFormat("{0} explicitly called CloseTempLogFile().", this.m_taskName);
				this.m_writer.AppendLine();
				this.m_logContents = this.m_writer.ToString();
				this.m_writer = null;
			}
		}

		internal void WriteError(Exception error, ErrorCategory errorCategory, object target)
		{
			this.WriteErrorEx(error, errorCategory, target);
		}

		internal void WriteErrorEx(Exception error, ErrorCategory errorCategory, object target)
		{
			this.AppendLogMessage("WriteError! Exception = {0}", new object[]
			{
				error.ToString()
			});
			throw error;
		}

		internal void WriteVerboseEx(LocalizedString locString)
		{
			this.AppendLogMessage(locString.ToString(), new object[0]);
		}

		internal void WriteWarningEx(LocalizedString locString)
		{
			this.AppendLogMessage("Warning: {0}", new object[]
			{
				locString.ToString()
			});
		}

		internal void WriteProgressIncrementalSimple(LocalizedString locString, int incrementalPercent)
		{
			this.m_percentComplete = Math.Min(this.m_percentComplete + incrementalPercent, 100);
			this.WriteProgressEx(ReplayStrings.ProgressStatusInProgress, locString, this.m_percentComplete);
		}

		internal void WriteProgress(LocalizedString activity, LocalizedString statusDescription, int percentComplete)
		{
			this.WriteProgressEx(activity, statusDescription, percentComplete);
		}

		internal void WriteProgressEx(LocalizedString activity, LocalizedString statusDescription, int percentComplete)
		{
			this.AppendLogMessage("Updated Progress '{0}' {1}%.", new object[]
			{
				statusDescription.ToString(),
				percentComplete.ToString()
			});
			this.AppendLogMessage(activity);
			this.m_percentComplete = percentComplete;
		}

		private int m_percentComplete;

		private string m_taskName;

		private StringBuilder m_writer;

		private string m_logContents = string.Empty;

		private Exception m_lastException;

		private int m_maxPercentageDuringCallback;
	}
}
