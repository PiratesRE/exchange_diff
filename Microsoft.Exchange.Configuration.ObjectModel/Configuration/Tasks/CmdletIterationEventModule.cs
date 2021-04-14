using System;
using System.Globalization;
using System.Security.Principal;
using System.Text;
using System.Web;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.Common;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Configuration.ObjectModel.EventLog;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public class CmdletIterationEventModule : TaskIOPipelineBase, ITaskModule, ICriticalFeature
	{
		private ExEventLog.EventTuple CmdletSuccessEventTuple
		{
			get
			{
				if (this.context.InvocationInfo != null && this.context.InvocationInfo.CommandName.Equals("Get-ManagementEndpoint", StringComparison.OrdinalIgnoreCase))
				{
					return TaskEventLogConstants.Tuple_LogCmdletSuccess;
				}
				if (this.context.ExchangeRunspaceConfig == null)
				{
					return TaskEventLogConstants.Tuple_LogMediumLevelCmdletSuccess;
				}
				if (this.context.InvocationInfo != null && this.context.InvocationInfo.CommandName.StartsWith("Get", StringComparison.OrdinalIgnoreCase))
				{
					return TaskEventLogConstants.Tuple_LogLowLevelCmdletSuccess;
				}
				return TaskEventLogConstants.Tuple_LogCmdletSuccess;
			}
		}

		public CmdletIterationEventModule(TaskContext context)
		{
			this.context = context;
		}

		bool ICriticalFeature.IsCriticalException(Exception ex)
		{
			return false;
		}

		public void Init(ITaskEvent task)
		{
			task.Error += new EventHandler<GenericEventArg<TaskErrorEventArg>>(this.LogCmdletIterationEvent);
			task.IterateCompleted += this.LogCmdletIterationEvent;
			task.PreStop += this.OnPreStop;
			task.Stop += this.LogCmdletStopEvent;
			if (this.context.CommandShell != null)
			{
				this.context.CommandShell.PrependTaskIOPipelineHandler(this);
			}
		}

		public override bool WriteObject(object input, out object output)
		{
			output = input;
			this.outputObjectCount++;
			CmdletLogger.SafeSetLogger(this.context.UniqueId, RpsCmdletMetadata.OutputObjectCount, this.outputObjectCount);
			return true;
		}

		public void Dispose()
		{
		}

		private void OnPreStop(object sender, EventArgs e)
		{
			this.wasStopped = true;
		}

		private void LogCmdletStopEvent(object sender, EventArgs e)
		{
			this.LogCmdletIterationEvent();
		}

		private void LogCmdletIterationEvent(object sender, EventArgs e)
		{
			if (!this.wasStopped)
			{
				this.LogCmdletIterationEvent();
			}
		}

		private void LogCmdletIterationEvent()
		{
			bool flag = this.context.ExchangeRunspaceConfig == null;
			bool flag2 = VariantConfiguration.InvariantNoFlightingSnapshot.CmdletInfra.WriteEventLogInEnglish.Enabled && (CultureInfo.CurrentUICulture != CmdletLogHelper.DefaultLoggingCulture || CultureInfo.CurrentCulture != CmdletLogHelper.DefaultLoggingCulture);
			object[] array = new object[27];
			array[0] = ((this.context.InvocationInfo != null) ? this.context.InvocationInfo.DisplayName : string.Empty);
			array[1] = ((this.context.InvocationInfo == null) ? null : TaskVerboseStringHelper.FormatUserSpecifiedParameters(this.context.InvocationInfo.UserSpecifiedParameters ?? new PropertyBag()));
			array[2] = (flag ? ((this.context.UserInfo != null && this.context.UserInfo.ExecutingUserId != null) ? this.context.UserInfo.ExecutingUserId.ToString() : string.Empty) : this.context.ExchangeRunspaceConfig.IdentityName);
			array[3] = (flag ? null : this.context.ExchangeRunspaceConfig.LogonUserSid);
			array[4] = null;
			if (!flag)
			{
				SecurityIdentifier securityIdentifier = null;
				this.context.ExchangeRunspaceConfig.TryGetExecutingUserSid(out securityIdentifier);
				array[4] = securityIdentifier;
			}
			array[5] = this.GenerateApplicationString();
			array[6] = CmdletIterationEventModule.processIdAndName;
			array[7] = (flag ? ((this.context.UserInfo != null) ? this.context.UserInfo.CurrentOrganizationId : null) : this.context.ExchangeRunspaceConfig.OrganizationId);
			array[8] = Environment.CurrentManagedThreadId;
			DateTime utcNow = DateTime.UtcNow;
			array[9] = utcNow.Subtract(this.lastDateTimeValue);
			this.lastDateTimeValue = utcNow;
			ADDriverContext threadADContext = ADSessionSettings.GetThreadADContext();
			if (threadADContext == null)
			{
				array[10] = null;
			}
			else
			{
				array[10] = (flag2 ? TaskVerboseStringHelper.GetADServerSettings(null, threadADContext.ServerSettings, CmdletLogHelper.DefaultLoggingCulture) : TaskVerboseStringHelper.GetADServerSettings(null, threadADContext.ServerSettings, null));
			}
			if (this.context.ErrorInfo.HasErrors)
			{
				if (this.context.ErrorInfo.Exception != null)
				{
					Exception exception = this.context.ErrorInfo.Exception;
					array[11] = exception;
					array[12] = this.context.ErrorInfo.ExchangeErrorCategory.Value;
					if (exception != null && exception.InnerException != null)
					{
						array[13] = exception.InnerException;
					}
					if (exception is LocalizedException)
					{
						array[14] = ((LocalizedException)exception).LocalizedString.StringId;
						if (!flag2)
						{
							goto IL_2DE;
						}
						LocalizedException ex = (LocalizedException)exception;
						IFormatProvider formatProvider = ex.FormatProvider;
						try
						{
							ex.FormatProvider = CmdletLogHelper.DefaultLoggingCulture;
							array[11] = ex.ToString();
							goto IL_2DE;
						}
						finally
						{
							ex.FormatProvider = formatProvider;
						}
					}
					array[14] = "NonLocalizedException";
				}
				else
				{
					array[11] = "null";
				}
			}
			IL_2DE:
			object obj;
			this.context.Items.TryGetValue("Log_AdditionalLogData", out obj);
			array[15] = obj;
			LocalizedString delayedInfo = ThrottlingModule<ResourceThrottlingCallback>.GetDelayedInfo(this.context);
			if (!string.IsNullOrEmpty(delayedInfo))
			{
				array[16] = (flag2 ? delayedInfo.ToString(CmdletLogHelper.DefaultLoggingCulture) : delayedInfo) + ThrottlingModule<ResourceThrottlingCallback>.GetThrottlingInfo(this.context);
			}
			array[17] = SuppressingPiiContext.NeedPiiSuppression;
			this.context.Items.TryGetValue("Log_CmdletProxyInfo", out obj);
			array[18] = obj;
			if (this.context.Items.TryGetValue("Log_ProxiedObjectCount", out obj))
			{
				obj = string.Format("{0} objects execution has been proxied to remote server.", obj);
			}
			array[19] = obj;
			if (this.context.Items.TryGetValue("Log_RequestQueryFilterInGetTasks", out obj))
			{
				array[20] = string.Format("Request Filter used is: {0}", obj);
			}
			if (this.context.Items.TryGetValue("Log_InternalQueryFilterInGetTasks", out obj))
			{
				array[21] = string.Format("Cmdlet Filter used is: {0}", obj);
			}
			array[22] = this.outputObjectCount;
			array[23] = "ActivityId: " + ((ActivityContext.ActivityId != null) ? ActivityContext.ActivityId.Value.ToString() : string.Empty);
			if (!flag && this.context.ExchangeRunspaceConfig != null)
			{
				array[24] = this.context.ExchangeRunspaceConfig.GetRBACInformationSummary();
			}
			if (Constants.IsPowerShellWebService && HttpContext.Current != null && HttpContext.Current.Request != null && HttpContext.Current.Request.Headers != null)
			{
				array[25] = HttpContext.Current.Request.Headers["client-request-id"];
			}
			array[26] = CultureInfo.CurrentUICulture.Name;
			ExEventLog.EventTuple eventInfo;
			if (this.context.ErrorInfo.HasErrors)
			{
				eventInfo = TaskEventLogConstants.Tuple_LogCmdletError;
			}
			else if (this.context.WasCancelled)
			{
				eventInfo = TaskEventLogConstants.Tuple_LogCmdletCancelled;
			}
			else if (this.wasStopped)
			{
				eventInfo = TaskEventLogConstants.Tuple_LogCmdletStopped;
			}
			else
			{
				eventInfo = this.CmdletSuccessEventTuple;
			}
			try
			{
				TaskLogger.LogEvent("All", eventInfo, array);
			}
			catch (ArgumentException ex2)
			{
				if (this.context.CommandShell != null)
				{
					this.context.CommandShell.WriteWarning(Strings.WarningCannotWriteToEventLog(ex2.ToString()));
				}
			}
		}

		private string GenerateApplicationString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = this.context.InvocationInfo != null && string.Compare(this.context.InvocationInfo.ShellHostName, "ServerRemoteHost", true) == 0;
			string value;
			if (Constants.IsPowerShellWebService)
			{
				value = "Psws";
			}
			else if (flag)
			{
				value = "Remote";
			}
			else
			{
				value = "Local";
			}
			string value2;
			if (this.context.ExchangeRunspaceConfig != null && (flag || this.context.ExchangeRunspaceConfig.ConfigurationSettings.ClientApplication != ExchangeRunspaceConfigurationSettings.ExchangeApplication.Unknown))
			{
				value2 = this.context.ExchangeRunspaceConfig.ConfigurationSettings.ClientApplication.ToString();
			}
			else
			{
				value2 = ((this.context.InvocationInfo != null) ? this.context.InvocationInfo.ShellHostName : string.Empty);
			}
			stringBuilder.Append(value);
			stringBuilder.Append("-");
			stringBuilder.Append(value2);
			string value3 = ExchangeRunspaceConfigurationSettings.ExchangeUserType.Unknown.ToString();
			if (this.context.ExchangeRunspaceConfig != null)
			{
				value3 = this.context.ExchangeRunspaceConfig.ConfigurationSettings.UserType.ToString();
			}
			stringBuilder.Append("-");
			stringBuilder.Append(value3);
			if (this.context.ExchangeRunspaceConfig != null && this.context.ExchangeRunspaceConfig.ConfigurationSettings.IsProxy)
			{
				stringBuilder.Append("-Proxy");
			}
			return stringBuilder.ToString();
		}

		public const string ProxiedObjectRecordCountKey = "Log_ProxiedObjectCount";

		public const string RequestQueryFilterInGetTasksKey = "Log_RequestQueryFilterInGetTasks";

		public const string InternalQueryFilterInGetTasksKey = "Log_InternalQueryFilterInGetTasks";

		public const string AdditionalLogDataKey = "Log_AdditionalLogData";

		public const string CmdletProxyInfoKey = "Log_CmdletProxyInfo";

		private static readonly string processIdAndName = string.Format("{0} {1}", Constants.ProcessId, Constants.ProcessName);

		private readonly TaskContext context;

		private bool wasStopped;

		private int outputObjectCount;

		private DateTime lastDateTimeValue = DateTime.UtcNow;
	}
}
