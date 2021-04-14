using System;
using System.IO;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal class HaTaskOutputHelper : ITaskOutputHelper, ILogTraceHelper
	{
		internal Exception LastException
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

		internal HaTaskOutputHelper(string taskName, Task.TaskErrorLoggingDelegate writeError, Task.TaskWarningLoggingDelegate writeWarning, Task.TaskVerboseLoggingDelegate writeVerbose, Task.TaskProgressLoggingDelegate writeProgress, int hashCode)
		{
			this.m_taskName = taskName;
			this.m_writeError = writeError;
			this.m_writeWarning = writeWarning;
			this.m_writeVerbose = writeVerbose;
			this.m_writeProgress = writeProgress;
			this.m_hashCode = hashCode;
		}

		internal void CreateTempLogFile()
		{
			string path = string.Format("dagtask_{0}_{1}.log", DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss.fff"), this.m_taskName);
			string text = Path.Combine(ConfigurationContext.Setup.SetupLoggingPath, "DagTasks");
			bool flag = true;
			try
			{
				this.m_logFileName = Path.Combine(text, path);
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				ExTraceGlobals.ClusterTracer.TraceDebug<string>((long)this.m_hashCode, "Opening the log file {0}.", this.m_logFileName);
				this.m_writer = new StreamWriter(new FileStream(this.m_logFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read));
			}
			catch (UnauthorizedAccessException arg)
			{
				ExTraceGlobals.ClusterTracer.TraceDebug<string, string, UnauthorizedAccessException>((long)this.m_hashCode, "Could not create the directory {0} OR open the log file {1}, error {2}. Falling back to %temp%", text, this.m_logFileName, arg);
				flag = false;
			}
			catch (SecurityException arg2)
			{
				ExTraceGlobals.ClusterTracer.TraceDebug<string, string, SecurityException>((long)this.m_hashCode, "Could not create the directory {0} OR open the log file {1}, error {2}. Falling back to %temp%", text, this.m_logFileName, arg2);
				flag = false;
			}
			catch (IOException arg3)
			{
				ExTraceGlobals.ClusterTracer.TraceDebug<string, string, IOException>((long)this.m_hashCode, "Could not create the directory {0} OR open the log file {1}, error {2}. Falling back to %temp%", text, this.m_logFileName, arg3);
				flag = false;
			}
			if (!flag)
			{
				this.m_logFileName = Path.Combine(Path.GetTempPath(), path);
				ExTraceGlobals.ClusterTracer.TraceDebug<string>((long)this.m_hashCode, "Opening the log file {0}.", this.m_logFileName);
				this.m_writer = new StreamWriter(this.m_logFileName);
			}
			this.m_writer.AutoFlush = true;
			this.m_writer.WriteLine("{0} started on machine {1}.", this.m_taskName, Environment.MachineName);
		}

		internal void CloseTempLogFile()
		{
			if (this.m_writer != null)
			{
				this.m_writer.WriteLine("{0} explicitly called CloseTempLogFile().", this.m_taskName);
				this.m_writer.Close();
				this.m_writer = null;
			}
		}

		public void AppendLogMessage(LocalizedString locMessage)
		{
			if (this.m_writer != null)
			{
				string arg = DateTime.UtcNow.ToString("s");
				this.m_writer.WriteLine("[{0}] {1}", arg, locMessage.ToString());
			}
		}

		public void AppendLogMessage(string englishMessage, params object[] args)
		{
			if (this.m_writer != null)
			{
				string arg = DateTime.UtcNow.ToString("s");
				this.m_writer.Write("[{0}] ", arg);
				this.m_writer.WriteLine(englishMessage, args);
			}
		}

		public void WriteErrorSimple(Exception error)
		{
			this.WriteErrorEx(error, ErrorCategory.InvalidArgument, null);
		}

		internal void WriteError(Exception error, ErrorCategory errorCategory, object target)
		{
			this.WriteErrorEx(error, errorCategory, target);
		}

		internal void WriteErrorEx(Exception error, ErrorCategory errorCategory, object target)
		{
			if (this.m_writer != null)
			{
				if (!string.IsNullOrEmpty(this.m_logFileName))
				{
					this.WriteWarning(Strings.DagTaskErrorEncounteredMoreDetailsInLog(this.m_logFileName, Environment.MachineName));
				}
				this.AppendLogMessage("WriteError! Exception = {0}", new object[]
				{
					error.ToString()
				});
			}
			this.m_writeError(error, errorCategory, target);
		}

		public void WriteVerbose(LocalizedString locString)
		{
			this.WriteVerboseEx(locString);
		}

		internal void WriteVerboseEx(LocalizedString locString)
		{
			this.AppendLogMessage(locString.ToString(), new object[0]);
			this.m_writeVerbose(locString);
		}

		public void WriteWarning(LocalizedString locString)
		{
			this.WriteWarningEx(locString);
		}

		internal void WriteWarningEx(LocalizedString locString)
		{
			this.AppendLogMessage(locString.ToString(), new object[0]);
			this.m_writeWarning(locString);
		}

		public void WriteProgressSimple(LocalizedString locString)
		{
			this.WriteProgressIncrementalSimple(locString, 2);
		}

		internal void WriteProgressIncrementalSimple(LocalizedString locString, int incrementalPercent)
		{
			this.m_percentComplete = Math.Min(this.m_percentComplete + incrementalPercent, 100);
			this.WriteProgressEx(Strings.ProgressStatusInProgress, locString, this.m_percentComplete);
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
			this.m_writeProgress(activity, statusDescription, percentComplete);
		}

		private Task.TaskErrorLoggingDelegate m_writeError;

		private Task.TaskWarningLoggingDelegate m_writeWarning;

		private Task.TaskVerboseLoggingDelegate m_writeVerbose;

		private Task.TaskProgressLoggingDelegate m_writeProgress;

		private int m_percentComplete;

		private readonly string m_taskName;

		private readonly int m_hashCode;

		private string m_logFileName;

		private StreamWriter m_writer;

		private Exception m_lastException;
	}
}
