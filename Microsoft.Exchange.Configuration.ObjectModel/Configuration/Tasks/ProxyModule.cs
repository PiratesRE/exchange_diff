using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Net;
using System.Text;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Configuration.Core.EventLog;
using Microsoft.Exchange.Configuration.PswsProxy;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.Configuration.Core;
using Microsoft.Exchange.Diagnostics.Components.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class ProxyModule : TaskIOPipelineBase, ITaskModule
	{
		public ProxyModule(TaskContext context)
		{
			this.context = context;
		}

		public void Init(ITaskEvent task)
		{
			task.PreIterate += this.OnPreIterate;
			task.Error += this.ProxyCmdletOnException;
			if (this.context.CommandShell != null)
			{
				this.context.CommandShell.PrependTaskIOPipelineHandler(this);
			}
		}

		public void Dispose()
		{
		}

		public override bool ShouldProcess(string verboseDescription, string verboseWarning, string caption, out bool? output)
		{
			this.shouldProcessHasBeenPrompted = true;
			return base.ShouldProcess(verboseDescription, verboseWarning, caption, out output);
		}

		public override bool WriteObject(object input, out object output)
		{
			output = input;
			ICmdletProxyable cmdletProxyable = input as ICmdletProxyable;
			if (cmdletProxyable == null)
			{
				return true;
			}
			CmdletProxyInfo cmdletProxyInfo = cmdletProxyable.GetProxyInfo() as CmdletProxyInfo;
			if (cmdletProxyInfo == null)
			{
				return true;
			}
			this.ProxyCmdlet(cmdletProxyInfo);
			return false;
		}

		private void OnPreIterate(object sender, EventArgs eventArgs)
		{
			this.shouldProcessHasBeenPrompted = false;
			this.proxiedObjectCount = 0;
			CmdletLogger.SafeSetLogger(this.context.UniqueId, RpsCmdletMetadata.ProxiedObjectCount, 0);
			this.context.Items["Log_ProxiedObjectCount"] = 0;
		}

		private void ProxyCmdletOnException(object sender, GenericEventArg<TaskErrorEventArg> e)
		{
			CmdletNeedsProxyException ex = e.Data.Exception as CmdletNeedsProxyException;
			if (ex != null && this.ProxyCmdlet(ex.CmdletProxyInfo))
			{
				e.Data.ExceptionHandled = true;
				if (this.context.Stage == TaskStage.BeginProcessing)
				{
					this.context.ShouldTerminateCmdletExecution = true;
				}
			}
		}

		private bool ProxyCmdlet(CmdletProxyInfo cmdletProxyInfo)
		{
			this.proxiedObjectCount++;
			CmdletLogger.SafeSetLogger(this.context.UniqueId, RpsCmdletMetadata.ProxiedObjectCount, this.proxiedObjectCount);
			this.context.Items["Log_ProxiedObjectCount"] = this.proxiedObjectCount;
			if (cmdletProxyInfo.ConfirmationMessage != LocalizedString.Empty)
			{
				if (!this.shouldProcessHasBeenPrompted && this.context.CommandShell != null && !this.context.CommandShell.ShouldProcess(cmdletProxyInfo.ConfirmationMessage))
				{
					CmdletLogger.SafeAppendGenericInfo(this.context.UniqueId, "Proxy", "CancelAfterConfirm");
					CmdletLogger.SafeSetLogger(this.context.UniqueId, RpsCmdletMetadata.ExecutionResult, "Cancelled");
					this.context.WasCancelled = true;
					return false;
				}
				SwitchParameter switchParameter = new SwitchParameter(false);
				if (this.ShouldRemoveConfirmParam() && this.context.InvocationInfo != null)
				{
					this.context.InvocationInfo.UserSpecifiedParameters.Remove("Confirm");
				}
				if (this.context.InvocationInfo != null)
				{
					this.context.InvocationInfo.UserSpecifiedParameters.Add("Confirm", switchParameter);
				}
			}
			using (new CmdletMonitoredScope(this.context.UniqueId, "CmdletProxyLatency", "CmdletProxyLatency", LoggerHelper.CmdletPerfMonitors))
			{
				this.ProxyCmdletExecution(cmdletProxyInfo);
			}
			this.context.Items["Log_CmdletProxyInfo"] = string.Format("Cmdlet proxied to remote server {0}, version {1}.", cmdletProxyInfo.RemoteServerFqdn, ProxyHelper.GetFriendlyVersionInformation(cmdletProxyInfo.RemoteServerVersion));
			return true;
		}

		private bool ShouldRemoveConfirmParam()
		{
			if (this.context.InvocationInfo != null)
			{
				return this.context.InvocationInfo.UserSpecifiedParameters.Keys.Cast<string>().Any((string key) => key == "Confirm");
			}
			return false;
		}

		private void ProxyCmdletExecution(CmdletProxyInfo cmdletProxyInfo)
		{
			ExAssert.RetailAssert(this.context.ExchangeRunspaceConfig != null, "this.context.ExchangeRunspaceConfig should not be null.");
			string remoteServerFqdn = cmdletProxyInfo.RemoteServerFqdn;
			int remoteServerVersion = cmdletProxyInfo.RemoteServerVersion;
			string friendlyVersionInformation = ProxyHelper.GetFriendlyVersionInformation(remoteServerVersion);
			CmdletProxyInfo.ChangeCmdletProxyParametersDelegate changeCmdletProxyParameters = cmdletProxyInfo.ChangeCmdletProxyParameters;
			if (Microsoft.Exchange.Diagnostics.Components.Tasks.ExTraceGlobals.FaultInjectionTracer.IsTraceEnabled(TraceType.FaultInjection))
			{
				Microsoft.Exchange.Diagnostics.Components.Tasks.ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(3720752445U, ref remoteServerFqdn);
				UserToken userToken = this.context.ExchangeRunspaceConfig.ConfigurationSettings.UserToken;
				ProxyHelper.FaultInjection_UserSid(ref userToken);
				Microsoft.Exchange.Diagnostics.Components.Tasks.ExTraceGlobals.FaultInjectionTracer.TraceTest<int>(3452316989U, ref remoteServerVersion);
			}
			string text = this.GeneratePswsProxyCmdlet(changeCmdletProxyParameters);
			ExchangeRunspaceConfigurationSettings.ProxyMethod proxyMethod = this.DetermineProxyMethod(remoteServerVersion);
			if (this.context.CommandShell != null)
			{
				this.context.CommandShell.WriteVerbose(Strings.VerboseCmdletProxiedToAnotherServer(text, remoteServerFqdn, friendlyVersionInformation, proxyMethod.ToString()));
			}
			Guid uniqueId = this.context.UniqueId;
			CmdletLogger.SafeAppendColumn(uniqueId, RpsCmdletMetadata.CmdletProxyRemoteServer, this.proxiedObjectCount.ToString(), remoteServerFqdn);
			CmdletLogger.SafeAppendColumn(uniqueId, RpsCmdletMetadata.CmdletProxyRemoteServerVersion, this.proxiedObjectCount.ToString(), friendlyVersionInformation.ToString());
			CmdletLogger.SafeAppendColumn(uniqueId, RpsCmdletMetadata.CmdletProxyMethod, this.proxiedObjectCount.ToString(), proxyMethod.ToString());
			try
			{
				IEnumerable<PSObject> enumerable;
				if (proxyMethod == ExchangeRunspaceConfigurationSettings.ProxyMethod.RPS)
				{
					PSCommand command = this.GenerateProxyCmdlet(changeCmdletProxyParameters);
					Task.TaskWarningLoggingDelegate writeWarning = null;
					if (this.context.CommandShell != null)
					{
						writeWarning = new Task.TaskWarningLoggingDelegate(this.context.CommandShell.WriteWarning);
					}
					enumerable = ProxyHelper.RPSProxyExecution(this.context.UniqueId, command, remoteServerFqdn, this.context.ExchangeRunspaceConfig, remoteServerVersion, cmdletProxyInfo.ShouldAsyncProxy, writeWarning);
				}
				else
				{
					enumerable = CommandInvocation.Invoke(this.context.UniqueId, ProxyHelper.GetPSWSProxySiteUri(remoteServerFqdn), text, CredentialCache.DefaultNetworkCredentials, ProxyHelper.GetPSWSProxyRequestHeaders(this.context.ExchangeRunspaceConfig), this.context.ExchangeRunspaceConfig.TypeTable);
				}
				foreach (PSObject psobject in enumerable)
				{
					object sendToPipeline = psobject;
					if (psobject.BaseObject != null && !(psobject.BaseObject is PSCustomObject))
					{
						sendToPipeline = psobject.BaseObject;
					}
					else if (this.context.ExchangeRunspaceConfig != null)
					{
						if (this.context.ExchangeRunspaceConfig.ConfigurationSettings.ClientApplication != ExchangeRunspaceConfigurationSettings.ExchangeApplication.ECP)
						{
							if (this.context.ExchangeRunspaceConfig.ConfigurationSettings.ClientApplication != ExchangeRunspaceConfigurationSettings.ExchangeApplication.OSP)
							{
								goto IL_2CB;
							}
						}
						try
						{
							Task.TaskVerboseLoggingDelegate writeVerbose = null;
							if (this.context.CommandShell != null)
							{
								writeVerbose = new Task.TaskVerboseLoggingDelegate(this.context.CommandShell.WriteWarning);
							}
							sendToPipeline = ProxyHelper.ConvertPSObjectToOriginalType(psobject, remoteServerVersion, writeVerbose);
						}
						catch (Exception ex)
						{
							CmdletLogger.SafeAppendGenericError(uniqueId, "ConvertPSObjectToOriginalTyp", ex, new Func<Exception, bool>(TaskHelper.IsTaskUnhandledException));
							Diagnostics.ReportException(ex, Constants.CoreEventLogger, TaskEventLogConstants.Tuple_UnhandledException, null, null, Microsoft.Exchange.Diagnostics.Components.Configuration.Core.ExTraceGlobals.InstrumentationTracer, "Exception from ProxyHelper.ConvertPSObjectToOriginalType : {0}");
						}
					}
					IL_2CB:
					if (this.context.CommandShell != null)
					{
						this.context.CommandShell.WriteObject(sendToPipeline);
					}
				}
			}
			catch (Exception ex2)
			{
				CmdletLogger.SafeAppendGenericError(this.context.UniqueId, "ProxyCmdletExecution", ex2, new Func<Exception, bool>(TaskHelper.IsTaskUnhandledException));
				if (this.context.CommandShell != null)
				{
					this.context.CommandShell.WriteError(new CmdletProxyException(text, remoteServerFqdn, friendlyVersionInformation, proxyMethod.ToString(), ex2.Message), ExchangeErrorCategory.ServerOperation, null);
				}
			}
		}

		private PSCommand GenerateProxyCmdlet(CmdletProxyInfo.ChangeCmdletProxyParametersDelegate changeCmdletProxyParameters)
		{
			this.MakeNecessaryChangeOnParametersBeforeProxy(changeCmdletProxyParameters);
			PSCommand pscommand = new PSCommand();
			Command command = null;
			if (this.context.InvocationInfo != null)
			{
				command = new Command(this.context.InvocationInfo.CommandName);
			}
			if (this.context.InvocationInfo != null)
			{
				foreach (object obj in this.context.InvocationInfo.UserSpecifiedParameters.Keys)
				{
					string text = (string)obj;
					command.Parameters.Add(text, ProxyHelper.TranslateCmdletProxyParameter(this.GetParameterValueToProxy(text), ExchangeRunspaceConfigurationSettings.ProxyMethod.RPS));
				}
			}
			pscommand.AddCommand(command);
			return pscommand;
		}

		private string GeneratePswsProxyCmdlet(CmdletProxyInfo.ChangeCmdletProxyParametersDelegate changeCmdletProxyParameters)
		{
			this.MakeNecessaryChangeOnParametersBeforeProxy(changeCmdletProxyParameters);
			StringBuilder stringBuilder = null;
			if (this.context.InvocationInfo != null)
			{
				stringBuilder = new StringBuilder(this.context.InvocationInfo.CommandName);
				foreach (object obj in this.context.InvocationInfo.UserSpecifiedParameters.Keys)
				{
					string text = (string)obj;
					ProxyHelper.BuildCommandFromParameter(text, this.GetParameterValueToProxy(text), stringBuilder);
				}
			}
			return stringBuilder.ToString();
		}

		private object GetParameterValueToProxy(string parameter)
		{
			object result = this.context.InvocationInfo.UserSpecifiedParameters[parameter];
			if (Constants.IsPowerShellWebService)
			{
				string commandName = this.context.InvocationInfo.CommandName;
				List<string> propertiesNeedUrlTokenInputDecode = PswsKeyProperties.GetPropertiesNeedUrlTokenInputDecode(commandName);
				if (propertiesNeedUrlTokenInputDecode != null && propertiesNeedUrlTokenInputDecode.Contains(parameter) && this.context.InvocationInfo.Fields.Contains(parameter))
				{
					result = this.context.InvocationInfo.Fields[parameter];
				}
			}
			return result;
		}

		private void MakeNecessaryChangeOnParametersBeforeProxy(CmdletProxyInfo.ChangeCmdletProxyParametersDelegate changeCmdletProxyParameters)
		{
			if (changeCmdletProxyParameters != null && this.context.InvocationInfo != null)
			{
				changeCmdletProxyParameters(this.context.InvocationInfo.UserSpecifiedParameters);
			}
			if (Microsoft.Exchange.Diagnostics.Components.Tasks.ExTraceGlobals.FaultInjectionTracer.IsTraceEnabled(TraceType.FaultInjection) && this.context.InvocationInfo != null)
			{
				ProxyHelper.FaultInjection_Identity(this.context.InvocationInfo.UserSpecifiedParameters);
			}
		}

		private ExchangeRunspaceConfigurationSettings.ProxyMethod DetermineProxyMethod(int remoteServerVersion)
		{
			if (this.context.ExchangeRunspaceConfig != null && this.context.ExchangeRunspaceConfig.ConfigurationSettings.CurrentProxyMethod != ExchangeRunspaceConfigurationSettings.ProxyMethod.None)
			{
				return this.context.ExchangeRunspaceConfig.ConfigurationSettings.CurrentProxyMethod;
			}
			if (!VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).CmdletInfra.PswsCmdletProxy.Enabled || remoteServerVersion < ProxyHelper.PswsSupportProxyMinimumVersion)
			{
				return ExchangeRunspaceConfigurationSettings.ProxyMethod.RPS;
			}
			return ExchangeRunspaceConfigurationSettings.ProxyMethod.PSWS;
		}

		private const string ConfirmParamName = "Confirm";

		private readonly TaskContext context;

		private bool shouldProcessHasBeenPrompted;

		private int proxiedObjectCount;
	}
}
