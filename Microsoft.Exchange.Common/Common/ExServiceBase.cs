using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Common
{
	public abstract class ExServiceBase : ServiceBase
	{
		protected virtual TimeSpan StartTimeout
		{
			get
			{
				return ExServiceBase.DefaultTimeout;
			}
		}

		protected virtual TimeSpan StopTimeout
		{
			get
			{
				return ExServiceBase.DefaultTimeout;
			}
		}

		protected virtual TimeSpan PauseTimeout
		{
			get
			{
				return ExServiceBase.DefaultTimeout;
			}
		}

		protected virtual TimeSpan ContinueTimeout
		{
			get
			{
				return ExServiceBase.DefaultTimeout;
			}
		}

		protected virtual TimeSpan CustomCommandTimeout
		{
			get
			{
				return ExServiceBase.DefaultTimeout;
			}
		}

		public static void RunAsConsole(ExServiceBase service)
		{
			Console.WriteLine("Starting...");
			service.OnStart(null);
			Console.WriteLine("Started. Type ENTER to stop.");
			Console.ReadLine();
			Console.WriteLine("Stopping...");
			service.OnStop();
			Console.WriteLine("Stopped");
		}

		public void ExRequestAdditionalTime(int milliseconds)
		{
			if (!Environment.UserInteractive)
			{
				base.RequestAdditionalTime(milliseconds);
			}
		}

		protected static string GetProcessesInfo()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Processes running are: ");
			try
			{
				Process[] processes = Process.GetProcesses();
				if (processes != null)
				{
					foreach (Process process in processes)
					{
						stringBuilder.AppendLine(process.Id + " " + process.ProcessName);
					}
				}
			}
			catch (Exception ex)
			{
				stringBuilder.AppendLine("An Exception occurred when getting running processes: " + ex.ToString());
			}
			return stringBuilder.ToString();
		}

		protected sealed override void OnStart(string[] args)
		{
			this.SendWatsonReportOnTimeout((string message) => new ExServiceBase.ServiceStartTimeoutException(message), this.StartTimeout, delegate
			{
				ExWatson.SendReportOnUnhandledException(delegate()
				{
					try
					{
						this.OnStartInternal(args);
					}
					catch (ExServiceBase.GracefulServiceStartupFailureException)
					{
						this.Stop();
					}
				});
			});
		}

		protected sealed override void OnStop()
		{
			this.SendWatsonReportOnTimeout((string message) => new ExServiceBase.ServiceStopTimeoutException(message), this.StopTimeout, delegate
			{
				ExWatson.SendReportOnUnhandledException(delegate()
				{
					this.OnStopInternal();
				});
			});
		}

		protected sealed override void OnShutdown()
		{
			this.SendWatsonReportOnTimeout((string message) => new ExServiceBase.ServiceShutdownTimeoutException(message), this.StopTimeout, delegate
			{
				ExWatson.SendReportOnUnhandledException(delegate()
				{
					this.OnShutdownInternal();
				});
			});
		}

		protected sealed override void OnPause()
		{
			this.SendWatsonReportOnTimeout((string message) => new ExServiceBase.ServicePauseTimeoutException(message), this.PauseTimeout, delegate
			{
				ExWatson.SendReportOnUnhandledException(delegate()
				{
					this.OnPauseInternal();
				});
			});
		}

		protected sealed override void OnContinue()
		{
			this.SendWatsonReportOnTimeout((string message) => new ExServiceBase.ServiceContinueTimeoutException(message), this.ContinueTimeout, delegate
			{
				ExWatson.SendReportOnUnhandledException(delegate()
				{
					this.OnContinueInternal();
				});
			});
		}

		protected sealed override void OnCustomCommand(int command)
		{
			this.SendWatsonReportOnTimeout((string message) => new ExServiceBase.ServiceCustomCommandTimeoutException(message), this.CustomCommandTimeout, delegate
			{
				ExWatson.SendReportOnUnhandledException(delegate()
				{
					this.OnCustomCommandInternal(command);
				});
			});
		}

		protected abstract void OnStartInternal(string[] args);

		protected abstract void OnStopInternal();

		protected virtual void OnShutdownInternal()
		{
		}

		protected virtual void OnPauseInternal()
		{
		}

		protected virtual void OnContinueInternal()
		{
		}

		protected virtual void OnCommandTimeout()
		{
		}

		protected virtual void OnCustomCommandInternal(int command)
		{
		}

		protected void GracefullyAbortStartup()
		{
			throw new ExServiceBase.GracefulServiceStartupFailureException();
		}

		protected void LogExWatsonTimeoutServiceStateChangeInfo(string info)
		{
			if (this.serviceStateChanges == null)
			{
				this.serviceStateChanges = new StringBuilder();
			}
			this.serviceStateChanges.AppendFormat("{0} : {1}", DateTime.UtcNow, info);
		}

		private static bool IsSendEventLogsWithWatsonReportEnabled()
		{
			bool result;
			try
			{
				NameValueCollection appSettings = ConfigurationManager.AppSettings;
				string value = appSettings["SendEventLogsWithWatsonReport"];
				bool flag;
				if (string.IsNullOrEmpty(value))
				{
					result = false;
				}
				else if (bool.TryParse(value, out flag) && flag)
				{
					result = true;
				}
				else
				{
					result = false;
				}
			}
			catch (ConfigurationErrorsException)
			{
				result = false;
			}
			return result;
		}

		private static string GetLastEventLogEntries(TimeSpan timeSpan, int maxEntries)
		{
			DateTime t = DateTime.UtcNow - timeSpan;
			StringBuilder stringBuilder = new StringBuilder(5000);
			try
			{
				using (EventLog eventLog = new EventLog("Application"))
				{
					int num = 0;
					int i = eventLog.Entries.Count - 1;
					while (i >= 0)
					{
						using (EventLogEntry eventLogEntry = eventLog.Entries[i])
						{
							stringBuilder.AppendLine(string.Format("{0}: {1}: {2}: {3}", new object[]
							{
								eventLogEntry.TimeGenerated,
								eventLogEntry.EntryType,
								eventLogEntry.Source,
								eventLogEntry.Message
							}));
							if (eventLogEntry.TimeGenerated.ToUniversalTime() < t || num > maxEntries)
							{
								break;
							}
						}
						i--;
						num++;
					}
				}
			}
			catch (Exception ex)
			{
				stringBuilder.AppendLine(ex.ToString());
			}
			return stringBuilder.ToString();
		}

		private void SendWatsonReportOnTimeout(Func<string, ExServiceBase.ServiceTimeoutException> newException, TimeSpan timeout, ExServiceBase.UnderTimeoutDelegate underTimeoutDelegate)
		{
			string message = string.Concat(new object[]
			{
				"Started on thread ",
				Environment.CurrentManagedThreadId,
				" at UTC time: ",
				DateTime.UtcNow.ToString(),
				", but did not complete in alloted time of ",
				timeout.ToString()
			});
			Func<string, ExServiceBase.ServiceTimeoutException> newException2 = (string additionalInfo) => newException(message + additionalInfo);
			Task task = Task.Run(delegate()
			{
				underTimeoutDelegate();
			});
			try
			{
				task.Wait(timeout);
			}
			catch (AggregateException ex)
			{
				throw ex.InnerException;
			}
			if (!task.IsCompleted)
			{
				this.TimeoutHandler(newException2);
			}
		}

		private void TimeoutHandler(Func<string, ExServiceBase.ServiceTimeoutException> newException)
		{
			if (newException == null)
			{
				throw new ArgumentNullException("newException");
			}
			if (!Debugger.IsAttached)
			{
				this.OnCommandTimeout();
				ExServiceBase.ServiceTimeoutException exception = newException((this.serviceStateChanges != null) ? (Environment.NewLine + this.serviceStateChanges.ToString()) : string.Empty);
				if (ExServiceBase.IsSendEventLogsWithWatsonReportEnabled())
				{
					string lastEventLogEntries = ExServiceBase.GetLastEventLogEntries(TimeSpan.FromMinutes(10.0), 5000);
					ExWatson.SendReport(exception, ReportOptions.ReportTerminateAfterSend, lastEventLogEntries);
					return;
				}
				ExWatson.SendReport(exception);
			}
		}

		private const string SendEventLogsWithWatsonReport = "SendEventLogsWithWatsonReport";

		private static readonly TimeSpan DefaultTimeout = TimeSpan.FromMinutes(5.0);

		private StringBuilder serviceStateChanges;

		private delegate void UnderTimeoutDelegate();

		private abstract class ServiceTimeoutException : Exception
		{
			public ServiceTimeoutException(string message) : base(message)
			{
			}
		}

		private sealed class ServiceStartTimeoutException : ExServiceBase.ServiceTimeoutException
		{
			public ServiceStartTimeoutException(string message) : base(message)
			{
			}
		}

		private sealed class ServiceStopTimeoutException : ExServiceBase.ServiceTimeoutException
		{
			public ServiceStopTimeoutException(string message) : base(message)
			{
			}
		}

		private sealed class ServiceShutdownTimeoutException : ExServiceBase.ServiceTimeoutException
		{
			public ServiceShutdownTimeoutException(string message) : base(message)
			{
			}
		}

		private sealed class ServicePauseTimeoutException : ExServiceBase.ServiceTimeoutException
		{
			public ServicePauseTimeoutException(string message) : base(message)
			{
			}
		}

		private sealed class ServiceContinueTimeoutException : ExServiceBase.ServiceTimeoutException
		{
			public ServiceContinueTimeoutException(string message) : base(message)
			{
			}
		}

		private sealed class ServiceCustomCommandTimeoutException : ExServiceBase.ServiceTimeoutException
		{
			public ServiceCustomCommandTimeoutException(string message) : base(message)
			{
			}
		}

		private class GracefulServiceStartupFailureException : Exception
		{
		}
	}
}
