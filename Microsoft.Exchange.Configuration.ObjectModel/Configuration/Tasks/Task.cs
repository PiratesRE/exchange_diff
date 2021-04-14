using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.Protocols;
using System.IO;
using System.Management.Automation;
using System.Net.Sockets;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.Common;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Configuration.ObjectModel;
using Microsoft.Exchange.Configuration.ObjectModel.EventLog;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.Tasks;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Provisioning;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class Task : PSCmdlet, IDisposable, ICommandShell, ITaskIOPipeline
	{
		internal TaskContext CurrentTaskContext { get; private set; }

		static Task()
		{
			if (ExchangeSetupContext.IsUnpacked)
			{
				Task.fileSearchAssemblyResolver.FileNameFilter = ((string fileName) => fileName.StartsWith("Microsoft.Exchange."));
				Task.fileSearchAssemblyResolver.Recursive = false;
				Task.fileSearchAssemblyResolver.SearchPaths = new string[]
				{
					ExchangeSetupContext.BinPath,
					Path.Combine(ExchangeSetupContext.BinPath, "FIP-FS\\Bin"),
					Path.Combine(ExchangeSetupContext.BinPath, "CmdletExtensionAgents"),
					Path.Combine(ExchangeSetupContext.BinPath, "res")
				};
				Task.fileSearchAssemblyResolver.ErrorTracer = delegate(string error)
				{
					TaskLogger.Trace(error, new object[0]);
				};
				Task.fileSearchAssemblyResolver.Install();
			}
		}

		internal Task()
		{
			this.CurrentTaskContext = new TaskContext(this);
			TaskLogger.LogEnter(new object[]
			{
				base.GetType().FullName
			});
			this.Fields = new PropertyBag();
			this.taskIOPipeline = new TaskIOPipeline(this.CurrentTaskContext);
			this.taskIOPipeline.PrependTaskIOPipelineHandler(this);
			ITaskModuleFactory taskModuleFactory = this.CreateTaskModuleFactory();
			this.taskModules.AddRange(taskModuleFactory.Create(this.CurrentTaskContext));
			this.taskEvents = new TaskEvent(this.CurrentTaskContext);
			TaskLogger.LogExit();
		}

		~Task()
		{
			this.Dispose(false);
		}

		public void SetFileSearchPath(string[] searchPath)
		{
			Task.fileSearchAssemblyResolver.SearchPaths = searchPath;
		}

		internal ConfigurationContext Context
		{
			get
			{
				return this.context;
			}
			set
			{
				this.context = value;
			}
		}

		protected internal PropertyBag Fields { get; private set; }

		internal int CurrentObjectIndex
		{
			get
			{
				return this.CurrentTaskContext.CurrentObjectIndex;
			}
		}

		internal TaskStage Stage
		{
			get
			{
				return this.CurrentTaskContext.Stage;
			}
		}

		internal ADServerSettings ServerSettings
		{
			get
			{
				ADDriverContext threadADContext = ADSessionSettings.GetThreadADContext();
				if (threadADContext == null)
				{
					return null;
				}
				return threadADContext.ServerSettings;
			}
		}

		private TaskInvocationInfo MyInvocationInfo
		{
			get
			{
				if (this.invocationInfo == null)
				{
					this.invocationInfo = this.CreateFromCmdletInvocationInfo();
					ActionPreference preference;
					if (this.TryGetVariableValue<ActionPreference>("VerbosePreference", out preference))
					{
						this.invocationInfo.IsVerboseOn = Task.IsSwitchOn(base.MyInvocation.BoundParameters, "Verbose", preference);
					}
					if (this.TryGetVariableValue<ActionPreference>("DebugPreference", out preference))
					{
						this.invocationInfo.IsDebugOn = Task.IsSwitchOn(base.MyInvocation.BoundParameters, "Debug", preference);
					}
				}
				return this.invocationInfo;
			}
		}

		public new ISessionState SessionState
		{
			get
			{
				return this.CurrentTaskContext.SessionState;
			}
		}

		public new string ParameterSetName
		{
			get
			{
				return this.MyInvocationInfo.ParameterSetName;
			}
			set
			{
				this.MyInvocationInfo.ParameterSetName = value;
			}
		}

		internal ProvisioningCache ProvisioningCache
		{
			get
			{
				return ProvisioningCache.Instance;
			}
		}

		protected SwitchParameter InternalForce
		{
			get
			{
				return (SwitchParameter)(this.Fields["InternalForce"] ?? false);
			}
			set
			{
				this.Fields["InternalForce"] = value;
			}
		}

		public bool NeedSuppressingPiiData
		{
			get
			{
				return this.ExchangeRunspaceConfig != null && this.ExchangeRunspaceConfig.NeedSuppressingPiiData;
			}
		}

		protected internal ProvisioningHandler[] ProvisioningHandlers
		{
			get
			{
				return this.provisioningHandlers;
			}
		}

		protected internal bool IsProvisioningLayerAvailable
		{
			get
			{
				return this.provisioningHandlers != null && this.provisioningHandlers.Length > 0;
			}
		}

		internal string ExecutingUserIdentityName
		{
			get
			{
				if (this.CurrentTaskContext.UserInfo != null)
				{
					return this.CurrentTaskContext.UserInfo.ExecutingUserIdentityName;
				}
				return null;
			}
		}

		internal bool TryGetExecutingUserId(out ADObjectId executingUserId)
		{
			executingUserId = ((this.CurrentTaskContext.UserInfo == null) ? null : this.CurrentTaskContext.UserInfo.ExecutingUserId);
			return executingUserId != null;
		}

		internal ExchangeRunspaceConfiguration ExchangeRunspaceConfig
		{
			get
			{
				return this.CurrentTaskContext.ExchangeRunspaceConfig;
			}
		}

		internal OrganizationId ExecutingUserOrganizationId
		{
			get
			{
				if (this.CurrentTaskContext.UserInfo != null)
				{
					return this.CurrentTaskContext.UserInfo.ExecutingUserOrganizationId;
				}
				return null;
			}
		}

		protected internal OrganizationId CurrentOrganizationId
		{
			get
			{
				if (this.CurrentTaskContext.UserInfo != null)
				{
					return this.CurrentTaskContext.UserInfo.CurrentOrganizationId;
				}
				return null;
			}
			set
			{
				this.CurrentTaskContext.UserInfo.UpdateCurrentOrganizationId(value);
				ProvisioningLayer.SetUserScope(this);
			}
		}

		internal ScopeSet ScopeSet
		{
			get
			{
				return this.CurrentTaskContext.ScopeSet;
			}
		}

		protected PropertyBag UserSpecifiedParameters
		{
			get
			{
				return this.MyInvocationInfo.UserSpecifiedParameters;
			}
		}

		protected virtual LocalizedString ConfirmationMessage
		{
			get
			{
				return LocalizedString.Empty;
			}
		}

		protected virtual ITaskModuleFactory CreateTaskModuleFactory()
		{
			return new TaskModuleFactory();
		}

		protected bool HasErrors
		{
			get
			{
				return this.CurrentTaskContext.ErrorInfo.HasErrors;
			}
		}

		protected int ProcessId
		{
			get
			{
				return Constants.ProcessId;
			}
		}

		protected string AdditionalLogData
		{
			get
			{
				return (string)this.CurrentTaskContext.Items["Log_AdditionalLogData"];
			}
			set
			{
				this.CurrentTaskContext.Items["Log_AdditionalLogData"] = value;
			}
		}

		public void Dispose()
		{
			TaskLogger.LogEnter();
			this.Dispose(true);
			GC.SuppressFinalize(this);
			TaskLogger.LogExit();
		}

		internal void Execute()
		{
			this.BeginProcessing();
			this.ProcessRecord();
			this.EndProcessing();
		}

		internal virtual void PreInternalProcessRecord()
		{
		}

		internal void CheckExclusiveParameters(params object[] parameterKeys)
		{
			object obj = null;
			string arg = null;
			foreach (object obj2 in parameterKeys)
			{
				if (this.Fields.IsModified(obj2))
				{
					if (obj == null)
					{
						obj = obj2;
						arg = ((obj2 is ADPropertyDefinition) ? ((ADPropertyDefinition)obj2).Name : obj2.ToString());
					}
					else
					{
						string arg2 = (obj2 is ADPropertyDefinition) ? ((ADPropertyDefinition)obj2).Name : obj2.ToString();
						this.ThrowTerminatingError(new ArgumentException(Strings.MutuallyExclusiveArguments(arg, arg2)), ErrorCategory.InvalidArgument, null);
					}
				}
			}
		}

		protected virtual bool IsKnownException(Exception e)
		{
			return TaskHelper.IsTaskKnownException(e);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.context = null;
				this.Fields = null;
				if (this.taskModules != null)
				{
					foreach (ITaskModule taskModule in this.taskModules)
					{
						taskModule.Dispose();
					}
					this.taskModules = null;
				}
			}
		}

		protected sealed override void BeginProcessing()
		{
			this.ProcessTaskStage(TaskStage.BeginProcessing, delegate
			{
				using (new CmdletMonitoredScope(this.CurrentTaskContext.UniqueId, "BizLogic", "Task.BeginProcessing/Task.InitTaskContext", LoggerHelper.CmdletPerfMonitors))
				{
					this.InitTaskContext();
				}
				using (new CmdletMonitoredScope(this.CurrentTaskContext.UniqueId, "BizLogic", "Task.BeginProcessing/Task.InitTaskModule", LoggerHelper.CmdletPerfMonitors))
				{
					this.InitTaskModule();
				}
				this.taskEvents.OnPreInit(null);
				this.WriteVerbose(Strings.VerboseTaskBeginProcessing(this.MyInvocationInfo.InvocationName));
				if (!ProvisioningLayer.Disabled)
				{
					this.provisioningHandlers = ProvisioningLayer.GetProvisioningHandlers(this);
					ProvisioningLayer.SetLogMessageDelegate(this);
				}
			}, delegate
			{
				using (new CmdletMonitoredScope(this.CurrentTaskContext.UniqueId, "BizLogic", "Task.BeginProcessing/InternalBeginProcessing", LoggerHelper.CmdletPerfMonitors))
				{
					this.InternalBeginProcessing();
				}
			}, delegate
			{
				this.taskEvents.OnInitCompleted(null);
			});
		}

		private void InitTaskModule()
		{
			TaskLogger.LogEnter();
			foreach (ITaskModule taskModule in this.taskModules)
			{
				taskModule.Init(this.taskEvents);
			}
			TaskLogger.LogExit();
		}

		private void InitTaskContext()
		{
			if (this.CurrentTaskContext.SessionState == null)
			{
				this.CurrentTaskContext.SessionState = ((base.SessionState != null) ? new PSSessionState(base.SessionState) : null);
			}
			if (this.CurrentTaskContext.InvocationInfo != null)
			{
				this.invocationInfo = this.CurrentTaskContext.InvocationInfo;
				foreach (object key in this.Fields.Keys)
				{
					object value = this.Fields[key];
					this.CurrentTaskContext.InvocationInfo.Fields[key] = value;
					this.CurrentTaskContext.InvocationInfo.AddToUserSpecifiedParameter(key, value);
				}
				this.Fields = this.CurrentTaskContext.InvocationInfo.Fields;
				return;
			}
			this.CurrentTaskContext.InvocationInfo = this.MyInvocationInfo;
		}

		protected virtual void InternalBeginProcessing()
		{
		}

		protected sealed override void EndProcessing()
		{
			this.ProcessTaskStage(TaskStage.EndProcessing, delegate
			{
				this.WriteVerbose(Strings.VerboseTaskEndProcessing(this.MyInvocationInfo.InvocationName));
				this.taskEvents.OnPreRelease(null);
			}, delegate
			{
				using (new CmdletMonitoredScope(this.CurrentTaskContext.UniqueId, "BizLogic", "Task.EndProcessing/InternalEndProcessing", LoggerHelper.CmdletPerfMonitors))
				{
					this.InternalEndProcessing();
				}
				ProvisioningLayer.EndProcessing(this);
			}, delegate
			{
				this.taskEvents.OnRelease(null);
			});
		}

		protected virtual void InternalEndProcessing()
		{
		}

		protected sealed override void StopProcessing()
		{
			using (new CmdletMonitoredScope(this.CurrentTaskContext.UniqueId, "BizLogic", "Task.StopProcessing", LoggerHelper.CmdletPerfMonitors))
			{
				TaskLogger.LogEnter();
				try
				{
					this.taskEvents.OnPreStop(null);
					this.CurrentTaskContext.WasStopped = true;
					CmdletLogger.SafeSetLogger(this.CurrentTaskContext.UniqueId, RpsCmdletMetadata.ExecutionResult, "Stopped");
					using (new CmdletMonitoredScope(this.CurrentTaskContext.UniqueId, "BizLogic", "Task.StopProcessing/InternalStopProcessing", LoggerHelper.CmdletPerfMonitors))
					{
						this.InternalStopProcessing();
					}
				}
				catch (Exception ex)
				{
					TaskErrorEventArg taskErrorEventArg = new TaskErrorEventArg(ex, null);
					this.taskEvents.OnError(new GenericEventArg<TaskErrorEventArg>(taskErrorEventArg));
					TaskLogger.LogError(ex);
					if (!taskErrorEventArg.ExceptionHandled)
					{
						throw;
					}
				}
				finally
				{
					this.taskEvents.OnStop(null);
					TaskLogger.LogExit();
				}
			}
		}

		protected virtual void InternalStopProcessing()
		{
		}

		protected sealed override void ProcessRecord()
		{
			this.ProcessTaskStage(TaskStage.ProcessRecord, delegate
			{
				this.CurrentTaskContext.CurrentObjectIndex++;
				if (this.CurrentObjectIndex == 0)
				{
					this.userCurrentOrganizationId = this.CurrentOrganizationId;
				}
				else
				{
					this.CurrentOrganizationId = this.userCurrentOrganizationId;
				}
				if (!this.MyInvocationInfo.IsCmdletInvokedWithoutPSFramework)
				{
					this.MyInvocationInfo.UpdateSpecifiedParameters(base.MyInvocation.BoundParameters);
				}
				this.taskEvents.OnPreIterate(null);
				if (this.Context == null)
				{
					try
					{
						this.Context = new ConfigurationContext();
					}
					catch (SocketException exception)
					{
						this.WriteError(exception, (ErrorCategory)1001, this.CurrentObjectIndex);
					}
				}
				if (ExTraceGlobals.LogTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					string arg = TaskVerboseStringHelper.FormatUserSpecifiedParameters(this.UserSpecifiedParameters);
					ExTraceGlobals.LogTracer.Information<string, string>(0L, "Processing {0} {1}", this.MyInvocationInfo.CommandName, arg);
				}
				ProvisioningLayer.SetUserSpecifiedParameters(this, this.UserSpecifiedParameters);
				ProvisioningLayer.SetProvisioningCache(this, this.ProvisioningCache);
				ProvisioningLayer.SetUserScope(this);
			}, delegate
			{
				using (new CmdletMonitoredScope(this.CurrentTaskContext.UniqueId, "BizLogic", "Task.ProcessRecord/InnerProcessRecord", LoggerHelper.CmdletPerfMonitors))
				{
					using (new CmdletMonitoredScope(this.CurrentTaskContext.UniqueId, "BizLogic", "InnerProcessRecord/InternalStateReset", LoggerHelper.CmdletPerfMonitors))
					{
						this.InternalStateReset();
					}
					if (!this.HasErrors)
					{
						using (new CmdletMonitoredScope(this.CurrentTaskContext.UniqueId, "BizLogic", "InnerProcessRecord/InternalValidate", LoggerHelper.CmdletPerfMonitors))
						{
							this.InternalValidate();
						}
						this.PostInternalValidate();
					}
					if (!this.HasErrors)
					{
						ProvisioningValidationError[] array = ProvisioningLayer.ValidateUserScope(this);
						if (array != null && array.Length > 0)
						{
							for (int i = 0; i < array.Length; i++)
							{
								ProvisioningValidationException exception = new ProvisioningValidationException(array[i].Description, array[i].AgentName, array[i].Exception);
								this.WriteError(exception, (ErrorCategory)array[i].ErrorCategory, null, array.Length - 1 == i);
							}
						}
						this.InternalProvisioningValidation();
					}
					if (!this.HasErrors)
					{
						if (this.ConfirmationMessage == LocalizedString.Empty || this.ShouldProcess(this.ConfirmationMessage))
						{
							string orgId = (this.CurrentOrganizationId != null) ? this.CurrentOrganizationId.ToString() : string.Empty;
							if (this.IsVerboseOn && !TaskLogger.IsSetupLogging)
							{
								this.WriteVerbose(Strings.VerboseResolvedOrganization(orgId));
							}
							this.PreInternalProcessRecord();
							using (new CmdletMonitoredScope(this.CurrentTaskContext.UniqueId, "BizLogic", "InnerProcessRecord/InternalProcessRecord", LoggerHelper.CmdletPerfMonitors))
							{
								this.InternalProcessRecord();
								goto IL_1CA;
							}
						}
						this.CurrentTaskContext.WasCancelled = true;
						CmdletLogger.SafeSetLogger(this.CurrentTaskContext.UniqueId, RpsCmdletMetadata.ExecutionResult, "Cancelled");
					}
					IL_1CA:
					ProvisioningLayer.OnComplete(this, !this.CurrentTaskContext.WasCancelled && !this.HasErrors, null);
				}
			}, delegate
			{
				this.taskEvents.OnIterateCompleted(null);
			});
		}

		protected virtual void InternalStateReset()
		{
		}

		protected virtual void InternalValidate()
		{
		}

		protected virtual void PostInternalValidate()
		{
		}

		protected virtual void InternalProcessRecord()
		{
		}

		protected virtual void InternalProvisioningValidation()
		{
		}

		private bool ProcessError(Exception exception, bool terminating)
		{
			bool flag;
			return this.ProcessError(exception, terminating, false, false, out flag);
		}

		private bool ProcessError(Exception exception, bool terminating, bool shouldRetryForRetryableError, bool shouldLogIfRetryNotHappens, out bool retryHappens)
		{
			TaskLogger.LogEnter();
			TaskErrorInfo errorInfo = this.CurrentTaskContext.ErrorInfo;
			bool? isUnknownException = null;
			if (errorInfo.Exception == null)
			{
				ErrorCategory errorCategory = ErrorCategory.NotSpecified;
				bool flag = this.IsKnownException(exception);
				isUnknownException = new bool?(!flag);
				if (flag)
				{
					this.TranslateException(ref exception, out errorCategory);
				}
				errorInfo.SetErrorInfo(exception, (ExchangeErrorCategory)errorCategory, flag ? null : this.CurrentObjectIndex, null, terminating, flag);
			}
			else
			{
				isUnknownException = new bool?(!errorInfo.IsKnownError);
				errorInfo.TerminatePipeline = terminating;
			}
			TaskErrorEventArg taskErrorEventArg = new TaskErrorEventArg(exception, isUnknownException);
			this.taskEvents.OnError(new GenericEventArg<TaskErrorEventArg>(taskErrorEventArg));
			if (ExTraceGlobals.LogTracer.IsTraceEnabled(TraceType.ErrorTrace))
			{
				ExTraceGlobals.LogTracer.TraceError<int, string, Exception>(0L, "Cmdlet iteration index: '{0}', Stage: '{1}', Error: '{2}'", this.CurrentObjectIndex, this.Stage.ToString(), exception);
			}
			ProvisioningLayer.OnComplete(this, false, exception);
			retryHappens = false;
			bool result;
			try
			{
				if (taskErrorEventArg.ExceptionHandled)
				{
					result = true;
				}
				else
				{
					if (exception is TransientException)
					{
						if (shouldRetryForRetryableError)
						{
							retryHappens = true;
							return true;
						}
						if (shouldLogIfRetryNotHappens)
						{
							CmdletLogger.SafeAppendGenericInfo("RetryableError-Not-Retried", "true");
						}
					}
					if (this.Stage != TaskStage.ProcessRecord)
					{
						this.CurrentTaskContext.ShouldTerminateCmdletExecution = true;
					}
					CmdletLogger.SafeSetLogger(this.CurrentTaskContext.UniqueId, RpsCmdletMetadata.ExecutionResult, "Error");
					if (!errorInfo.IsKnownError)
					{
						TaskLogger.LogEvent(TaskEventLogConstants.Tuple_TaskThrowingUnhandledException, this.MyInvocationInfo, exception.Message, new object[]
						{
							this.MyInvocationInfo.DisplayName,
							exception
						});
						CmdletLogHelper.SetCmdletErrorType(this.CurrentTaskContext.UniqueId, "UnHandled");
						result = false;
					}
					else if (this.CurrentTaskContext.ServerSettingsAfterFailOver != null && (exception is ADTransientException || exception is LdapException || exception is DirectoryOperationException))
					{
						ADServerSettingsChangedException ex = new ADServerSettingsChangedException(DirectoryStrings.RunspaceServerSettingsChanged, this.CurrentTaskContext.ServerSettingsAfterFailOver);
						CmdletLogger.SafeAppendGenericError(this.CurrentTaskContext.UniqueId, "ADServerSettingsChangedException", ex, (Exception ex1) => false);
						this.WriteError(ex, ExchangeErrorCategory.ServerTransient, this.CurrentTaskContext.CurrentObjectIndex, terminating);
						result = true;
					}
					else
					{
						if (errorInfo.TerminatePipeline)
						{
							CmdletLogHelper.SetCmdletErrorType(this.CurrentTaskContext.UniqueId, "TerminatePipeline");
							this.CurrentTaskContext.ShouldTerminateCmdletExecution = true;
							this.ThrowTerminatePipelineError(Task.CreateErrorRecord(errorInfo));
						}
						this.WriteError(errorInfo);
						result = true;
					}
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
			return result;
		}

		protected virtual void TranslateException(ref Exception e, out ErrorCategory category)
		{
			category = ErrorCategory.NotSpecified;
			if (typeof(TranslatableProvisioningException).IsInstanceOfType(e) && e.InnerException != null)
			{
				e = e.InnerException;
			}
		}

		internal static ErrorRecord CreateErrorRecord(TaskErrorInfo errorInfo)
		{
			int num = LocalizedException.GenerateErrorCode(errorInfo.Exception);
			string str = string.Format("[Server={0},RequestId={1},TimeStamp={2}] ", Environment.MachineName, (ActivityContext.ActivityId != null) ? ActivityContext.ActivityId.Value : Guid.Empty, DateTime.UtcNow);
			str += string.Format("[FailureCategory={0}] ", FailureCategory.Cmdlet.ToString() + "-" + errorInfo.Exception.GetType().Name);
			ErrorRecord errorRecord = new ErrorRecord(errorInfo.Exception, str + num.ToString("X"), (ErrorCategory)errorInfo.ExchangeErrorCategory.Value, errorInfo.Target);
			if (errorInfo.HelpUrl != null)
			{
				errorRecord.ErrorDetails = new ErrorDetails(errorInfo.Exception.Message);
				errorRecord.ErrorDetails.RecommendedAction = errorInfo.HelpUrl;
			}
			return errorRecord;
		}

		private TaskInvocationInfo CreateFromCmdletInvocationInfo()
		{
			InvocationInfo invocationInfo = this.SessionState.Variables["MyInvocation"] as InvocationInfo;
			string rootScriptName = (invocationInfo != null) ? invocationInfo.ScriptName : null;
			return new TaskInvocationInfo((base.MyInvocation.MyCommand != null) ? base.MyInvocation.MyCommand.Name : this.GenerateCmdletName(), base.MyInvocation.InvocationName, base.MyInvocation.ScriptName, rootScriptName, base.MyInvocation.CommandOrigin == CommandOrigin.Internal, base.MyInvocation.BoundParameters, this.Fields, (base.Host == null) ? "HostWithoutPSRunspace" : base.Host.Name)
			{
				ParameterSetName = base.ParameterSetName
			};
		}

		private string GenerateCmdletName()
		{
			CmdletAttribute cmdletAttribute = (CmdletAttribute)Attribute.GetCustomAttribute(base.GetType(), typeof(CmdletAttribute), false);
			if (cmdletAttribute != null)
			{
				return string.Format("{0}-{1}", cmdletAttribute.VerbName, cmdletAttribute.NounName);
			}
			return base.GetType().Name;
		}

		private static string CurrentTimeString
		{
			get
			{
				return ExDateTime.UtcNow.ToString("[HH:mm:ss.fff G\\MT] ");
			}
		}

		private string VerboseTaskName
		{
			get
			{
				if (this.verboseTaskName == null)
				{
					this.verboseTaskName = this.MyInvocationInfo.CommandName + " : ";
				}
				return this.verboseTaskName;
			}
		}

		internal bool IsDebugOn
		{
			get
			{
				return this.MyInvocationInfo.IsDebugOn;
			}
		}

		internal bool IsVerboseOn
		{
			get
			{
				return this.MyInvocationInfo.IsVerboseOn;
			}
		}

		public void WriteVerbose(LocalizedString text)
		{
			TaskLogger.LogEnter();
			try
			{
				TaskLogger.Log(text);
				LocalizedString localizedString;
				this.taskIOPipeline.WriteVerbose(text, out localizedString);
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		public void WriteDebug(LocalizedString text)
		{
			TaskLogger.LogEnter();
			try
			{
				TaskLogger.Log(text);
				LocalizedString localizedString;
				this.taskIOPipeline.WriteDebug(text, out localizedString);
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		public void WriteError(Exception exception, ErrorCategory category, object target)
		{
			this.WriteError(exception, category, target, true, null);
		}

		public void WriteError(LocalizedException exception, ExchangeErrorCategory category, object target)
		{
			this.WriteError(exception, (ErrorCategory)category, target, true);
		}

		public virtual void WriteError(Exception exception, ErrorCategory category, object target, bool reThrow)
		{
			this.WriteError(exception, category, target, reThrow, null);
		}

		public virtual void WriteError(LocalizedException exception, ExchangeErrorCategory category, object target, bool reThrow)
		{
			this.WriteError(exception, (ErrorCategory)category, target, reThrow);
		}

		public void WriteError(Exception exception, ErrorCategory category, object target, bool reThrow, string helpUrl)
		{
			if (reThrow)
			{
				this.ThrowError(exception, category, target, helpUrl);
				return;
			}
			this.WriteErrorNoThrow(exception, category, target, helpUrl);
		}

		public void WriteWarning(LocalizedString text, string helpUrl)
		{
			TaskLogger.LogEnter();
			try
			{
				TaskLogger.LogWarning(text);
				LocalizedString localizedString;
				this.taskIOPipeline.WriteWarning(text, helpUrl, out localizedString);
				if (ExTraceGlobals.LogTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.LogTracer.Information<int, TaskStage, LocalizedString>(0L, "Cmdlet iteration index: '{0}', Stage: '{1}', Warning: '{2}'", this.CurrentObjectIndex, this.Stage, text);
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		public virtual void WriteWarning(LocalizedString text)
		{
			this.WriteWarning(text, null);
		}

		public void WriteProgress(LocalizedString activity, LocalizedString statusDescription, int percentCompleted)
		{
			TaskLogger.LogEnter();
			this.WriteProgress(new ExProgressRecord(this.CurrentObjectIndex, activity, statusDescription)
			{
				PercentComplete = percentCompleted
			});
			TaskLogger.LogExit();
		}

		public void WriteProgress(ExProgressRecord record)
		{
			TaskLogger.LogEnter();
			ExProgressRecord exProgressRecord;
			this.taskIOPipeline.WriteProgress(record, out exProgressRecord);
			TaskLogger.LogExit();
		}

		public bool ShouldProcess(LocalizedString message)
		{
			TaskLogger.LogEnter(new object[]
			{
				message
			});
			bool result;
			try
			{
				result = this.ShouldProcess(message.ToString(), null, null);
			}
			finally
			{
				TaskLogger.LogExit();
			}
			return result;
		}

		public bool ShouldContinue(LocalizedString message)
		{
			ConfirmationChoice confirmationChoice = ConfirmationChoice.No;
			if (this.confirmationPreferences.ContainsKey(message.BaseId))
			{
				confirmationChoice = this.confirmationPreferences[message.BaseId];
			}
			bool flag = confirmationChoice == ConfirmationChoice.YesToAll;
			bool flag2 = confirmationChoice == ConfirmationChoice.NoToAll;
			bool? flag3;
			this.taskIOPipeline.ShouldContinue(message, string.Empty, ref flag, ref flag2, out flag3);
			if (flag || flag2)
			{
				this.confirmationPreferences[message.BaseId] = (flag ? ConfirmationChoice.YesToAll : ConfirmationChoice.NoToAll);
			}
			return flag3.Value;
		}

		public new void WriteObject(object sendToPipeline)
		{
			this.CurrentTaskContext.ObjectWrittenToPipeline = true;
			object obj;
			this.taskIOPipeline.WriteObject(sendToPipeline, out obj);
		}

		public void ThrowTerminatingError(LocalizedException exception, ExchangeErrorCategory category, object target)
		{
			this.ThrowTerminatingError(exception, (ErrorCategory)category, target);
		}

		public void SetShouldExit(int exitCode)
		{
			base.Host.SetShouldExit(exitCode);
		}

		public void PrependTaskIOPipelineHandler(ITaskIOPipeline pipeline)
		{
			this.taskIOPipeline.PrependTaskIOPipelineHandler(pipeline);
		}

		protected void ThrowTerminatingError(Exception exception, ErrorCategory category, object target)
		{
			TaskLogger.LogEnter(new object[]
			{
				(exception != null) ? exception.Message : string.Empty,
				category,
				target
			});
			TaskLogger.LogError(exception);
			this.CurrentTaskContext.ErrorInfo.SetErrorInfo(exception, (ExchangeErrorCategory)category, target, null, true, true);
			throw exception;
		}

		protected void ThrowNonLocalizedTerminatingError(Exception exception, ExchangeErrorCategory category, object target)
		{
			this.ThrowTerminatingError(exception, (ErrorCategory)category, target);
		}

		[Obsolete("Use WriteProgress(LocalizedString activity, LocalizedString statusDescription, int percentCompleted) instead.")]
		protected new void WriteProgress(ProgressRecord record)
		{
		}

		[Obsolete("Use void WriteVerbose(LocalizedString text) instead.")]
		protected new void WriteVerbose(string text)
		{
		}

		[Obsolete("Use ShouldContinue(LocalizedString message) instead.")]
		protected new bool ShouldProcess(string target)
		{
			return base.ShouldProcess(target);
		}

		protected new bool ShouldProcess(string target, string action, string caption)
		{
			bool? flag;
			this.taskIOPipeline.ShouldProcess(target, action, caption, out flag);
			return flag.Value;
		}

		[Obsolete("Use ThrowTerminatingError(LocalizedException e, ErrorCategory category, object target) instead.")]
		protected new void ThrowTerminatingError(ErrorRecord errorRecord)
		{
		}

		[Obsolete("Use WriteError(Exception e, ErrorCategory category, object target) instead.")]
		protected new void WriteError(ErrorRecord errorRecord)
		{
		}

		public bool TryGetVariableValue<T>(string name, out T value)
		{
			object obj = null;
			if (this.SessionState != null && this.SessionState.Variables != null)
			{
				this.SessionState.Variables.TryGetValue(name, out obj);
			}
			if (obj == null)
			{
				value = default(T);
				return null == value;
			}
			if (obj is T)
			{
				value = (T)((object)obj);
				return true;
			}
			PSObject psobject = obj as PSObject;
			if (psobject != null && psobject.BaseObject is T)
			{
				value = (T)((object)psobject.BaseObject);
				return true;
			}
			value = default(T);
			return false;
		}

		private void WriteErrorNoThrow(Exception exception, ErrorCategory errorCategory, object target, string helpUrl)
		{
			TaskLogger.LogEnter();
			TaskLogger.LogError(exception);
			try
			{
				if (!string.IsNullOrEmpty(helpUrl))
				{
					TaskLogger.Log(Strings.LogHelpUrl(helpUrl));
				}
				this.CurrentTaskContext.ErrorInfo.SetErrorInfo(exception, (ExchangeErrorCategory)errorCategory, target, helpUrl, false, true);
				this.WriteError(this.CurrentTaskContext.ErrorInfo);
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		private void WriteError(TaskErrorInfo errorInfo)
		{
			TaskErrorInfo taskErrorInfo;
			this.taskIOPipeline.WriteError(errorInfo, out taskErrorInfo);
		}

		private void ThrowError(Exception exception, ErrorCategory errorCategory, object target, string helpUrl)
		{
			TaskLogger.LogEnter();
			TaskLogger.LogError(exception);
			if (!string.IsNullOrEmpty(helpUrl))
			{
				TaskLogger.Log(Strings.LogHelpUrl(helpUrl));
			}
			this.CurrentTaskContext.ErrorInfo.SetErrorInfo(exception, (ExchangeErrorCategory)errorCategory, target, helpUrl, false, true);
			throw exception;
		}

		private void ThrowTerminatePipelineError(ErrorRecord errorRecord)
		{
			throw new ThrowTerminatingErrorException(errorRecord);
		}

		private static bool IsSwitchOn(Dictionary<string, object> boundParameters, string parameterName, ActionPreference preference)
		{
			object obj;
			return preference != ActionPreference.SilentlyContinue || !boundParameters.TryGetValue(parameterName, out obj) || !(obj is SwitchParameter) || !((SwitchParameter)obj).IsPresent || true;
		}

		[Conditional("DEBUG")]
		private static void AssertObjectIsSerializable(object obj)
		{
		}

		public bool WriteVerbose(LocalizedString input, out LocalizedString output)
		{
			output = input;
			this.StartAndEndTaskIOPipelineFunctionInternalTracking(this.CurrentTaskContext.UniqueId, "PowerShellLatency", "WriteVerbose", delegate()
			{
				this.WriteVerbose(Task.CurrentTimeString + this.VerboseTaskName + input);
			});
			return false;
		}

		public bool WriteDebug(LocalizedString input, out LocalizedString output)
		{
			output = input;
			this.StartAndEndTaskIOPipelineFunctionInternalTracking(this.CurrentTaskContext.UniqueId, "PowerShellLatency", "WriteDebug", delegate()
			{
				this.WriteDebug(Task.CurrentTimeString + input);
			});
			return false;
		}

		public bool WriteWarning(LocalizedString input, string helperUrl, out LocalizedString output)
		{
			output = input;
			this.StartAndEndTaskIOPipelineFunctionInternalTracking(this.CurrentTaskContext.UniqueId, "PowerShellLatency", "WriteWarning", delegate()
			{
				if (string.IsNullOrEmpty(helperUrl))
				{
					this.WriteWarning(input);
					return;
				}
				this.WriteWarning(WarningReportEventArgs.CombineWarningMessageHelpUrl(input, helperUrl));
			});
			return false;
		}

		public bool WriteError(TaskErrorInfo input, out TaskErrorInfo output)
		{
			output = input;
			if (input != null)
			{
				this.StartAndEndTaskIOPipelineFunctionInternalTracking(this.CurrentTaskContext.UniqueId, "PowerShellLatency", "WriteError", delegate()
				{
					this.WriteError(Task.CreateErrorRecord(input));
				});
			}
			return false;
		}

		public bool WriteObject(object input, out object output)
		{
			output = input;
			this.StartAndEndTaskIOPipelineFunctionInternalTracking(this.CurrentTaskContext.UniqueId, "PowerShellLatency", "WriteObject", delegate()
			{
				this.WriteObject(input);
			});
			return false;
		}

		public bool WriteProgress(ExProgressRecord input, out ExProgressRecord output)
		{
			output = input;
			if (input != null)
			{
				this.StartAndEndTaskIOPipelineFunctionInternalTracking(this.CurrentTaskContext.UniqueId, "PowerShellLatency", "WriteProgress", delegate()
				{
					this.WriteProgress(input);
				});
			}
			return false;
		}

		public bool ShouldContinue(string query, string caption, ref bool yesToAll, ref bool noToAll, out bool? output)
		{
			output = new bool?(false);
			if (this.CurrentTaskContext.WasCancelled || this.CurrentTaskContext.WasStopped)
			{
				return false;
			}
			using (new CmdletMonitoredScope(this.CurrentTaskContext.UniqueId, "UserInteractionLatency", "ShouldContinue", LoggerHelper.CmdletPerfMonitors))
			{
				output = new bool?(base.ShouldContinue(query, caption, ref yesToAll, ref noToAll));
			}
			return false;
		}

		public bool ShouldProcess(string verboseDescription, string verboseWarning, string caption, out bool? output)
		{
			output = new bool?(this.StartAndEndTaskIOPipelineFunctionInternalTracking<bool>(this.CurrentTaskContext.UniqueId, "UserInteractionLatency", "ShouldProcess", () => this.ShouldProcess(verboseDescription, verboseWarning, caption)));
			return false;
		}

		private void StartAndEndTaskIOPipelineFunctionInternalTracking(Guid cmdletUniqueId, string groupName, string funcName, Action func)
		{
			if (this.CurrentTaskContext.WasCancelled || this.CurrentTaskContext.WasStopped)
			{
				return;
			}
			using (new CmdletMonitoredScope(cmdletUniqueId, groupName, funcName, LoggerHelper.CmdletPerfMonitors))
			{
				func();
			}
		}

		private T StartAndEndTaskIOPipelineFunctionInternalTracking<T>(Guid cmdletUniqueId, string groupName, string funcName, Func<T> func)
		{
			if (this.CurrentTaskContext.WasCancelled || this.CurrentTaskContext.WasStopped)
			{
				return default(T);
			}
			T result;
			using (new CmdletMonitoredScope(cmdletUniqueId, groupName, funcName, LoggerHelper.CmdletPerfMonitors))
			{
				result = func();
			}
			return result;
		}

		protected virtual bool IsRetryableTask
		{
			get
			{
				return false;
			}
		}

		private void ProcessTaskStage(TaskStage taskStage, Action initFunc, Action mainFunc, Action completeFunc)
		{
			TaskLogger.LogEnter(new object[]
			{
				taskStage
			});
			if (this.CurrentTaskContext.ShouldTerminateCmdletExecution && taskStage == TaskStage.ProcessRecord)
			{
				TaskLogger.LogExit();
				return;
			}
			using (new CmdletMonitoredScope(this.CurrentTaskContext.UniqueId, "BizLogic", "Task." + taskStage, LoggerHelper.CmdletPerfMonitors))
			{
				bool terminatePipelineIfFailed = taskStage != TaskStage.ProcessRecord;
				try
				{
					this.CurrentTaskContext.Stage = taskStage;
					CmdletLogHelper.ClearCmdletErrorType(this.CurrentTaskContext.UniqueId);
					if (this.CurrentTaskContext != null)
					{
						this.CurrentTaskContext.Reset();
					}
					using (new CmdletMonitoredScope(this.CurrentTaskContext.UniqueId, "BizLogic", "Task." + taskStage + ".Init", LoggerHelper.CmdletPerfMonitors))
					{
						if (!this.InvokeNonRetryableFunc(initFunc, terminatePipelineIfFailed))
						{
							return;
						}
					}
					using (new CmdletMonitoredScope(this.CurrentTaskContext.UniqueId, "BizLogic", "Task." + taskStage + ".Retry", LoggerHelper.CmdletPerfMonitors))
					{
						this.InvokeRetryableFunc("Task." + taskStage + ".Main", mainFunc, terminatePipelineIfFailed);
					}
				}
				finally
				{
					using (new CmdletMonitoredScope(this.CurrentTaskContext.UniqueId, "BizLogic", "Task." + taskStage + ".Complete", LoggerHelper.CmdletPerfMonitors))
					{
						this.InvokeNonRetryableFunc(completeFunc, terminatePipelineIfFailed);
					}
					TaskLogger.LogExit();
				}
			}
		}

		private bool InvokeNonRetryableFunc(Action func, bool terminatePipelineIfFailed)
		{
			bool result;
			try
			{
				func();
				result = true;
			}
			catch (Exception exception)
			{
				if (!this.ProcessError(exception, terminatePipelineIfFailed))
				{
					throw;
				}
				result = false;
			}
			return result;
		}

		private void InvokeRetryableFunc(string funcName, Action func, bool terminatePipelineIfFailed)
		{
			int num = this.IsRetryableTask ? (AppSettings.Current.MaxCmdletRetryCnt + 1) : 1;
			for (int i = 0; i < num; i++)
			{
				if (this.CurrentTaskContext != null)
				{
					this.CurrentTaskContext.Reset();
				}
				bool flag = false;
				Exception ex = null;
				using (new CmdletMonitoredScope(this.CurrentTaskContext.UniqueId, "BizLogic", funcName + "." + (i + 1), LoggerHelper.CmdletPerfMonitors))
				{
					try
					{
						func();
					}
					catch (Exception ex2)
					{
						ex = ex2;
						bool shouldRetryForRetryableError = i < num - 1 && !this.CurrentTaskContext.ObjectWrittenToPipeline;
						bool shouldLogIfRetryNotHappens = i < num - 1 && this.CurrentTaskContext.ObjectWrittenToPipeline;
						if (!this.ProcessError(ex2, terminatePipelineIfFailed, shouldRetryForRetryableError, shouldLogIfRetryNotHappens, out flag))
						{
							throw;
						}
					}
				}
				if (!flag)
				{
					break;
				}
				this.CurrentTaskContext.CommandShell.WriteVerbose(Strings.WarningTaskRetried(ex.Message));
				CmdletLogger.SafeAppendGenericInfo(this.CurrentTaskContext.UniqueId, funcName + "#Retry#" + (i + 1), (this.CurrentTaskContext.ErrorInfo.Exception != null) ? this.CurrentTaskContext.ErrorInfo.Exception.GetType().Name : "NULL");
			}
		}

		private ConfigurationContext context;

		private ProvisioningHandler[] provisioningHandlers;

		private OrganizationId userCurrentOrganizationId;

		private TaskInvocationInfo invocationInfo;

		private List<ITaskModule> taskModules = new List<ITaskModule>();

		private TaskEvent taskEvents;

		private static FileSearchAssemblyResolver fileSearchAssemblyResolver = new FileSearchAssemblyResolver();

		private string verboseTaskName;

		private Dictionary<int, ConfirmationChoice> confirmationPreferences = new Dictionary<int, ConfirmationChoice>();

		private TaskIOPipeline taskIOPipeline;

		internal delegate void ErrorLoggerDelegate(LocalizedException exception, ExchangeErrorCategory category, object target);

		internal delegate void ErrorLoggerReThrowDelegate(LocalizedException exception, ExchangeErrorCategory category, object target, bool reThrow);

		internal delegate void TaskVerboseLoggingDelegate(LocalizedString message);

		internal delegate void TaskWarningLoggingDelegate(LocalizedString message);

		internal delegate void TaskProgressLoggingDelegate(LocalizedString activity, LocalizedString statusDescription, int percent);

		internal delegate bool TaskShouldContinueDelegate(LocalizedString promptMessage);

		internal delegate void TaskErrorLoggingDelegate(Exception exception, ErrorCategory category, object target);

		internal delegate void TaskErrorLoggingReThrowDelegate(Exception exception, ErrorCategory category, object target, bool reThrow);
	}
}
