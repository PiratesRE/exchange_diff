using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Web;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Management.ControlPanel;
using Microsoft.Exchange.Diagnostics.LatencyDetection;
using Microsoft.Exchange.Management.DDIService;
using Microsoft.Exchange.PowerShell.RbacHostingTools;
using Microsoft.Mapi.Unmanaged;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class PSCommandExtension
	{
		public static PSCommand AddParameters(this PSCommand psCommand, WebServiceParameters parameters)
		{
			if (parameters != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in parameters)
				{
					psCommand.AddParameter(keyValuePair.Key, keyValuePair.Value);
				}
				if (parameters.CanSuppressConfirm)
				{
					string suppressConfirmParameterName = parameters.SuppressConfirmParameterName;
					if (parameters.SuppressConfirm)
					{
						psCommand.AddParameter(parameters.SuppressConfirmParameterName, new SwitchParameter(true));
					}
				}
			}
			return psCommand;
		}

		public static PowerShellResults<O> Invoke<O>(this PSCommand psCommand, RunspaceMediator runspaceMediator, IEnumerable pipelineInput, WebServiceParameters parameters, CmdletActivity activity, bool isGetListAsync)
		{
			return PSCommandExtension.InvokeCore<O>(psCommand, runspaceMediator, pipelineInput, parameters, activity, isGetListAsync);
		}

		public static PowerShellResults<O> Invoke<O>(this PSCommand psCommand, RunspaceMediator runspaceMediator, IEnumerable pipelineInput, WebServiceParameters parameters)
		{
			return PSCommandExtension.InvokeCore<O>(psCommand, runspaceMediator, pipelineInput, parameters, null, false);
		}

		private static PowerShellResults<O> InvokeCore<O>(PSCommand psCommand, RunspaceMediator runspaceMediator, IEnumerable pipelineInput, WebServiceParameters parameters, CmdletActivity activity, bool isGetListAsync)
		{
			Func<WarningRecord, string> func = null;
			Func<Command, string> func2 = null;
			PSCommandExtension.EnsureNoWriteTaskInGetRequest(psCommand, parameters);
			PowerShellResults<O> powerShellResults = new PowerShellResults<O>();
			ExTraceGlobals.EventLogTracer.TraceInformation<string, EcpTraceFormatter<PSCommand>>(0, 0L, "{0} tries to invoke {1}. For more details, refer to task trace", RbacPrincipal.Current.NameForEventLog, psCommand.GetTraceFormatter());
			using (RunspaceProxy runspaceProxy = new RunspaceProxy(runspaceMediator))
			{
				runspaceProxy.SetVariable("ConfirmPreference", "None");
				if (parameters != null)
				{
					psCommand.AddParameters(parameters);
				}
				using (PowerShell powerShell = runspaceProxy.CreatePowerShell(psCommand))
				{
					List<PSObject> list = null;
					if (activity != null)
					{
						AsyncServiceManager.RegisterPowerShellToActivity(powerShell, activity, pipelineInput, out list, isGetListAsync);
					}
					else
					{
						AsyncServiceManager.RegisterPowerShell(powerShell);
					}
					int requestLatency = 0;
					DateTime utcNow = DateTime.UtcNow;
					try
					{
						TaskPerformanceRecord taskPerformanceRecord = new TaskPerformanceRecord(psCommand.GetCmdletName(), PSCommandExtension.powerShellLatencyDetectionContextFactory, EcpEventLogConstants.Tuple_EcpPowerShellInvoked, EcpEventLogConstants.Tuple_EcpPowerShellCompleted, EcpEventLogExtensions.EventLog, new IPerformanceDataProvider[]
						{
							PerformanceContext.Current,
							RpcDataProvider.Instance,
							TaskPerformanceData.CmdletInvoked,
							TaskPerformanceData.BeginProcessingInvoked,
							TaskPerformanceData.ProcessRecordInvoked,
							TaskPerformanceData.EndProcessingInvoked,
							EcpPerformanceData.PowerShellInvoke
						});
						try
						{
							using (EcpPerformanceData.PowerShellInvoke.StartRequestTimer())
							{
								if (list == null)
								{
									powerShellResults.Output = powerShell.Invoke<O>(pipelineInput).ToArray<O>();
								}
								else
								{
									powerShell.Invoke<PSObject>(pipelineInput, list);
									powerShellResults.Output = Array<O>.Empty;
								}
							}
						}
						finally
						{
							requestLatency = (int)taskPerformanceRecord.Stop().TotalMilliseconds;
							IDisposable disposable2 = taskPerformanceRecord;
							if (disposable2 != null)
							{
								disposable2.Dispose();
							}
						}
						List<ErrorRecord> list2 = new List<ErrorRecord>();
						bool flag = false;
						foreach (ErrorRecord errorRecord in powerShell.Streams.Error)
						{
							if (!flag)
							{
								flag = PSCommandExtension.TryPatchShouldContinueException(errorRecord, psCommand, parameters);
							}
							list2.Add(new ErrorRecord(errorRecord));
						}
						powerShellResults.ErrorRecords = list2.ToArray();
						PowerShellResults powerShellResults2 = powerShellResults;
						IEnumerable<WarningRecord> warning = powerShell.Streams.Warning;
						if (func == null)
						{
							func = ((WarningRecord warningRecord) => warningRecord.Message);
						}
						powerShellResults2.Warnings = warning.Select(func).ToArray<string>();
					}
					catch (RuntimeException ex)
					{
						PSCommandExtension.TryPatchShouldContinueException(ex.ErrorRecord, psCommand, parameters);
						ErrorRecord errorRecord2;
						if (ex.ErrorRecord != null && !(ex is ParameterBindingException))
						{
							errorRecord2 = new ErrorRecord(ex.ErrorRecord);
						}
						else
						{
							errorRecord2 = new ErrorRecord(ex);
						}
						powerShellResults.ErrorRecords = new ErrorRecord[]
						{
							errorRecord2
						};
					}
					finally
					{
						string text = HttpContext.Current.Request.QueryString["reqId"];
						ServerLogEvent logEvent = new ServerLogEvent(psCommand, pipelineInput, requestLatency, string.IsNullOrEmpty(text) ? string.Empty : text, (powerShellResults.ErrorRecords != null && powerShellResults.ErrorRecords.Length > 0) ? powerShellResults.ErrorRecords.ToLogString() : string.Empty, (powerShellResults.Output != null) ? powerShellResults.Output.Length : 0);
						ServerLogger.Instance.LogEvent(logEvent);
					}
					PowerShellResults powerShellResults3 = powerShellResults;
					IEnumerable<Command> commands = psCommand.Commands;
					if (func2 == null)
					{
						func2 = ((Command cmd) => cmd.CommandText);
					}
					powerShellResults3.Cmdlets = commands.Select(func2).ToArray<string>();
					if (powerShellResults.ErrorRecords.Length > 0)
					{
						ExTraceGlobals.EventLogTracer.TraceError<string, EcpTraceFormatter<PSCommand>, EcpTraceFormatter<ErrorRecord[]>>(0, 0L, "{0} invoked {1} and encountered errors: {2}", RbacPrincipal.Current.NameForEventLog, psCommand.GetTraceFormatter(), powerShellResults.ErrorRecords.GetTraceFormatter());
					}
					CmdExecuteInfo cmdExecuteInfo = CmdletLogger.CaculateLogAndSaveToContext(powerShell, utcNow, powerShellResults.ErrorRecords);
					if (cmdExecuteInfo != null)
					{
						powerShellResults.CmdletLogInfo = new CmdExecuteInfo[]
						{
							cmdExecuteInfo
						};
					}
				}
			}
			return powerShellResults;
		}

		private static bool TryPatchShouldContinueException(ErrorRecord errorRecord, PSCommand psCommand, WebServiceParameters parameters)
		{
			if (errorRecord != null && errorRecord.Exception is ShouldContinueException)
			{
				ShouldContinueException ex = errorRecord.Exception as ShouldContinueException;
				ex.Details.CurrentCmdlet = psCommand.GetCmdletName();
				ex.Details.SuppressConfirmParameterName = ((parameters != null && parameters.CanSuppressConfirm) ? parameters.SuppressConfirmParameterName : null);
				return true;
			}
			return false;
		}

		private static void EnsureNoWriteTaskInGetRequest(PSCommand psCommand, WebServiceParameters parameters)
		{
			if (HttpContext.Current.Request.HttpMethod == "GET" && (parameters == null || !parameters.AllowExceuteThruHttpGetRequest))
			{
				string cmdletName = psCommand.GetCmdletName();
				if (!cmdletName.StartsWith("Get-", StringComparison.OrdinalIgnoreCase) && !cmdletName.StartsWith("Test-", StringComparison.OrdinalIgnoreCase) && !cmdletName.StartsWith("Export-", StringComparison.OrdinalIgnoreCase) && !cmdletName.StartsWith("Search-", StringComparison.OrdinalIgnoreCase))
				{
					throw new BadRequestException(new InvalidOperationException("HTTP GET request is not allowed to invoke any cmdlet that can make any change to Exchange AD/Store. Cmdlet: " + cmdletName + "."));
				}
			}
		}

		public static string GetCmdletName(this PSCommand psCommand)
		{
			if (psCommand.Commands.Count <= 0)
			{
				return string.Empty;
			}
			return psCommand.Commands[0].CommandText;
		}

		public static string ToLogString(PSCommand psCommand, IEnumerable pipelineInput)
		{
			int capacity = 128;
			StringBuilder stringBuilder = new StringBuilder(capacity);
			if (pipelineInput != null)
			{
				stringBuilder.Append("Pipeline.");
				stringBuilder.Append(pipelineInput.OfType<object>().Count<object>());
				stringBuilder.Append("|");
			}
			for (int i = 0; i < psCommand.Commands.Count; i++)
			{
				Command command = psCommand.Commands[i];
				stringBuilder.Append(command.CommandText);
				foreach (CommandParameter commandParameter in command.Parameters)
				{
					stringBuilder.Append(".");
					stringBuilder.Append(commandParameter.Name);
					stringBuilder.Append("=");
					stringBuilder.Append(EcpTraceExtensions.FormatParameterValue(commandParameter.Value));
				}
				if (i != psCommand.Commands.Count - 1)
				{
					stringBuilder.Append("|");
				}
			}
			return stringBuilder.ToString();
		}

		private static LatencyDetectionContextFactory powerShellLatencyDetectionContextFactory = LatencyDetectionContextFactory.CreateFactory("ECP.PowerShell");
	}
}
