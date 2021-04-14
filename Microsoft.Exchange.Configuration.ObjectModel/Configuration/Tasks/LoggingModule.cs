using System;
using System.Globalization;
using System.Web;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.Common;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Configuration.Core.EventLog;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.Configuration.Core;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class LoggingModule : TaskIOPipelineBase, ITaskModule, ICriticalFeature
	{
		public LoggingModule(TaskContext context)
		{
			this.context = context;
			if (!ActivityContext.IsStarted)
			{
				ActivityContext.ClearThreadScope();
				this.activityScope = ActivityContext.Start(this);
				if (HttpContext.Current != null)
				{
					this.activityScope.UpdateFromMessage(HttpContext.Current.Request);
					this.activityScope.SerializeTo(HttpContext.Current.Response);
				}
			}
			InitializeLoggerSettingsHelper.InitLoggerSettings();
			this.StartLogging();
		}

		bool ICriticalFeature.IsCriticalException(Exception ex)
		{
			return false;
		}

		void ITaskModule.Init(ITaskEvent task)
		{
			task.InitCompleted += this.OnInitCompleted;
			task.PreIterate += this.OnPreIterate;
			task.IterateCompleted += this.OnIterateCompleted;
			task.Release += this.OnRelease;
			task.Stop += this.OnStop;
			task.Error += this.OnError;
			if (this.context != null && this.context.CommandShell != null)
			{
				this.context.CommandShell.PrependTaskIOPipelineHandler(this);
			}
		}

		void ITaskModule.Dispose()
		{
			this.CommitLog("Dispose");
			if (this.activityScope != null)
			{
				this.activityScope.Dispose();
				this.activityScope = null;
			}
		}

		public override bool WriteError(TaskErrorInfo input, out TaskErrorInfo output)
		{
			if (input != null && input.Exception != null)
			{
				CmdletStaticDataWithUniqueId<Exception>.Set(this.context.UniqueId, input.Exception);
			}
			return base.WriteError(input, out output);
		}

		private void OnInitCompleted(object sender, EventArgs eventArgs)
		{
			if (this.context.ErrorInfo != null && this.context.ShouldTerminateCmdletExecution)
			{
				this.CommitLog("InitOnError");
			}
		}

		private void OnPreIterate(object sender, EventArgs eventArgs)
		{
			if (!this.isFirstIteration)
			{
				this.CommitLog("OnPreIterate");
				this.StartLogging();
			}
			this.isFirstIteration = false;
		}

		private void OnIterateCompleted(object sender, EventArgs eventArgs)
		{
			if (this.context.InvocationInfo != null)
			{
				Guid uniqueId = this.context.UniqueId;
				this.parametersSetInLog = true;
				CmdletLogger.SafeSetLogger(uniqueId, RpsCmdletMetadata.Cmdlet, this.context.InvocationInfo.CommandName);
				CmdletLogger.SafeSetLogger(uniqueId, RpsCmdletMetadata.Parameters, TaskVerboseStringHelper.FormatUserSpecifiedParameters(this.context.InvocationInfo.UserSpecifiedParameters ?? new PropertyBag()));
			}
		}

		private void OnStop(object sender, EventArgs eventArgs)
		{
			this.CommitLog("OnStop");
		}

		private void OnRelease(object sender, EventArgs eventArgs)
		{
			this.CommitLog(null);
		}

		private void OnError(object sender, GenericEventArg<TaskErrorEventArg> genericEventArg)
		{
			if (genericEventArg.Data.ExceptionHandled)
			{
				return;
			}
			Exception exception = genericEventArg.Data.Exception;
			Guid uniqueId = this.context.UniqueId;
			Exception ex = CmdletStaticDataWithUniqueId<Exception>.Get(uniqueId);
			if (ex != null && ex != exception)
			{
				CmdletLogger.SafeAppendGenericError(uniqueId, this.context.Stage.ToString() + ".FromWriteError", ex.ToString(), false);
			}
			bool isUnhandledException = (genericEventArg.Data.IsUnknownException != null) ? genericEventArg.Data.IsUnknownException.Value : TaskHelper.IsTaskUnhandledException(exception);
			CmdletLogger.SafeAppendGenericError(uniqueId, this.context.Stage.ToString(), exception.ToString(), isUnhandledException);
			if (exception is LocalizedException)
			{
				CmdletLogger.SafeAppendGenericError(uniqueId, "ExceptionStringId", ((LocalizedException)exception).LocalizedString.StringId, false);
				if (CmdletLogHelper.NeedConvertLogMessageToUS)
				{
					LocalizedException ex2 = (LocalizedException)exception;
					IFormatProvider formatProvider = ex2.FormatProvider;
					try
					{
						ex2.FormatProvider = CmdletLogHelper.DefaultLoggingCulture;
						CmdletLogger.SafeAppendGenericError(uniqueId, this.context.Stage.ToString() + "(en-us)", ex2.ToString(), false);
					}
					finally
					{
						ex2.FormatProvider = formatProvider;
					}
				}
			}
		}

		private void StartLogging()
		{
			this.logPendingCommit = true;
			this.parametersSetInLog = false;
			Guid uniqueId = this.context.UniqueId;
			CmdletThreadStaticData.RegisterCmdletUniqueId(uniqueId);
			CmdletLogger.SafeSetLogger(uniqueId, RpsCmdletMetadata.StartTime, DateTime.UtcNow);
			CmdletLogger.SafeSetLogger(uniqueId, RpsCmdletMetadata.ExecutionResult, "Success");
			CmdletLatencyTracker.StartLatencyTracker(uniqueId);
			CmdletLatencyTracker.StartInternalTracking(uniqueId, "Cmd", true);
		}

		private void CommitLog(string loggingStep)
		{
			if (!this.logPendingCommit)
			{
				return;
			}
			this.logPendingCommit = false;
			Guid cmdletUniqueId = this.context.UniqueId;
			try
			{
				CmdletLogger.SafeSetLogger(cmdletUniqueId, RpsCmdletMetadata.CmdletUniqueId, cmdletUniqueId);
				if (loggingStep != null)
				{
					CmdletLogger.SafeAppendGenericInfo(cmdletUniqueId, "Logging", loggingStep);
				}
				CmdletLogger.SafeSetLogger(cmdletUniqueId, RpsCmdletMetadata.ProcessId, Constants.ProcessId);
				CmdletLogger.SafeSetLogger(cmdletUniqueId, RpsCmdletMetadata.ProcessName, Constants.ProcessName);
				CmdletLogger.SafeSetLogger(cmdletUniqueId, RpsCmdletMetadata.ThreadId, Environment.CurrentManagedThreadId);
				CmdletLatencyTracker.EndInternalTracking(cmdletUniqueId, "Cmd");
				CmdletLatencyTracker.PushLatencyDetailsToLog(cmdletUniqueId, CmdletLogHelper.FuncNameToLogMetaDic, delegate(Enum metadata, double latency)
				{
					CmdletLogger.UpdateLatency(cmdletUniqueId, metadata, latency);
				}, delegate(string funcName, string totalLatency)
				{
					CmdletLogger.SafeAppendColumn(RpsCommonMetadata.GenericLatency, funcName, totalLatency);
				});
				long num = CmdletLatencyTracker.StopLatencyTracker(cmdletUniqueId);
				CmdletLogger.SafeSetLogger(cmdletUniqueId, RpsCmdletMetadata.TotalTime, num);
				CmdletLatencyTracker.DisposeLatencyTracker(cmdletUniqueId);
				if (!this.parametersSetInLog)
				{
					if (this.context.InvocationInfo != null)
					{
						CmdletLogger.SafeSetLogger(cmdletUniqueId, RpsCmdletMetadata.Cmdlet, this.context.InvocationInfo.CommandName);
						CmdletLogger.SafeSetLogger(cmdletUniqueId, RpsCmdletMetadata.Parameters, TaskVerboseStringHelper.FormatUserSpecifiedParameters(this.context.InvocationInfo.UserSpecifiedParameters ?? new PropertyBag()));
					}
					else
					{
						CmdletLogger.SafeAppendGenericInfo(cmdletUniqueId, "InvocationInfo", "null");
					}
				}
				TaskUserInfo userInfo = this.context.UserInfo;
				if (userInfo != null)
				{
					CmdletLogger.SafeSetLogger(cmdletUniqueId, RpsCmdletMetadata.TenantId, userInfo.ExecutingUserOrganizationId.GetFriendlyName());
					CmdletLogger.SafeSetLogger(cmdletUniqueId, RpsCmdletMetadata.AuthenticatedUser, (userInfo.ExecutingWindowsLiveId != SmtpAddress.Empty) ? userInfo.ExecutingWindowsLiveId.ToString() : ((userInfo.ExecutingUserId != null && !string.IsNullOrWhiteSpace(userInfo.ExecutingUserId.Name)) ? userInfo.ExecutingUserId.Name : userInfo.ExecutingUserIdentityName));
					CmdletLogger.SafeSetLogger(cmdletUniqueId, RpsCmdletMetadata.EffectiveOrganization, userInfo.CurrentOrganizationId.GetFriendlyName());
				}
				else
				{
					CmdletLogger.SafeAppendGenericInfo(cmdletUniqueId, "UserInfo", "null");
				}
				if (this.context.ExchangeRunspaceConfig != null)
				{
					CmdletLogger.SafeSetLogger(cmdletUniqueId, RpsCmdletMetadata.UserServicePlan, this.context.ExchangeRunspaceConfig.ServicePlanForLogging);
					CmdletLogger.SafeSetLogger(cmdletUniqueId, RpsCmdletMetadata.IsAdmin, this.context.ExchangeRunspaceConfig.HasAdminRoles);
					if (this.context.ExchangeRunspaceConfig.ConfigurationSettings != null && this.context.ExchangeRunspaceConfig.ConfigurationSettings.ClientApplication != ExchangeRunspaceConfigurationSettings.ExchangeApplication.Unknown)
					{
						CmdletLogger.SafeSetLogger(cmdletUniqueId, RpsCmdletMetadata.ClientApplication, this.context.ExchangeRunspaceConfig.ConfigurationSettings.ClientApplication);
					}
				}
				else
				{
					CmdletLogger.SafeAppendGenericInfo(cmdletUniqueId, "ExchangeRunspaceConfig", "null");
				}
				if (!CmdletLogHelper.DefaultLoggingCulture.Equals(CultureInfo.CurrentUICulture) || !CmdletLogHelper.DefaultLoggingCulture.Equals(CultureInfo.CurrentCulture))
				{
					CmdletLogger.SafeSetLogger(cmdletUniqueId, RpsCmdletMetadata.CultureInfo, CultureInfo.CurrentUICulture + "," + CultureInfo.CurrentCulture);
				}
			}
			catch (Exception ex)
			{
				Diagnostics.LogExceptionWithTrace(Constants.CoreEventLogger, TaskEventLogConstants.Tuple_NonCrashingException, null, ExTraceGlobals.InstrumentationTracer, null, "Exception from CmdletLogger.AsyncCommit : {0}", ex);
				CmdletLogger.SafeAppendGenericError(cmdletUniqueId, "CommitLog", ex, new Func<Exception, bool>(TaskHelper.IsTaskUnhandledException));
			}
			finally
			{
				try
				{
					CmdletLogger.SafeSetLogger(this.context.UniqueId, RpsCmdletMetadata.EndTime, DateTime.UtcNow);
					CmdletLogger.AsyncCommit(cmdletUniqueId, true);
					CmdletThreadStaticData.UnRegisterCmdletUniqueId(cmdletUniqueId);
				}
				catch (Exception exception)
				{
					this.logPendingCommit = true;
					Diagnostics.LogExceptionWithTrace(Constants.CoreEventLogger, TaskEventLogConstants.Tuple_NonCrashingException, null, ExTraceGlobals.InstrumentationTracer, null, "Exception from CmdletLogger.AsyncCommit : {0}", exception);
				}
			}
		}

		private bool isFirstIteration = true;

		private bool logPendingCommit;

		private bool parametersSetInLog;

		private readonly TaskContext context;

		private ActivityScope activityScope;
	}
}
