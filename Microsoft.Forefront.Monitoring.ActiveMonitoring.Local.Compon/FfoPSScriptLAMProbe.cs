using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.Win32;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class FfoPSScriptLAMProbe : ProbeWorkItem
	{
		protected virtual void InitializeAttributes(AttributeHelper attributeHelper = null)
		{
			if (attributeHelper == null)
			{
				attributeHelper = new AttributeHelper(base.Definition);
			}
			string @string = attributeHelper.GetString("ScriptPath", true, "Datacenter");
			string string2 = attributeHelper.GetString("Parameters", false, null);
			this.verbosePreference = attributeHelper.GetString("VerbosePreference", false, "SilentlyContinue");
			this.debugPreference = attributeHelper.GetString("DebugPreference", false, "SilentlyContinue");
			this.errorPreference = attributeHelper.GetString("ErrorPreference", false, null);
			this.warningPreference = attributeHelper.GetString("WarningPreference", false, null);
			this.scriptCommand = this.CreateScriptCommand(@string, string2);
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			base.Result.ExecutionContext = string.Format("FfoPSScriptLAMProbe started at {0}.{1}", DateTime.UtcNow, Environment.NewLine);
			this.InitializeAttributes(null);
			if (!string.IsNullOrEmpty(this.scriptCommand))
			{
				WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "In FfoPSScriptLAMProbe", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Background\\Probes\\FfoPSScriptLAMProbe.cs", 110);
				InitialSessionState initialSessionState = InitialSessionState.CreateDefault();
				initialSessionState.LanguageMode = PSLanguageMode.FullLanguage;
				StringBuilder stringBuilder = new StringBuilder();
				StringBuilder stringBuilder2 = new StringBuilder();
				WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Opening New Runspace", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Background\\Probes\\FfoPSScriptLAMProbe.cs", 117);
				using (Runspace runspace = RunspaceFactory.CreateRunspace(initialSessionState))
				{
					runspace.Open();
					PowerShell powerShell2;
					PowerShell powerShell = powerShell2 = PowerShell.Create();
					try
					{
						powerShell.Commands.AddScript(string.Format("$DebugPreference='{0}'", this.debugPreference));
						powerShell.Commands.AddScript(string.Format("$VerbosePreference='{0}'", this.verbosePreference));
						if (!string.IsNullOrEmpty(this.warningPreference))
						{
							powerShell.Commands.AddScript(string.Format("$WarningPreference='{0}'", this.warningPreference));
						}
						if (!string.IsNullOrEmpty(this.errorPreference))
						{
							powerShell.Commands.AddScript(string.Format("$ErrorPreference='{0}'", this.errorPreference));
						}
						powerShell.Commands.AddScript(this.scriptCommand);
						powerShell.Runspace = runspace;
						string text = string.Format("Executing Powershell Command: {0}. {1}", this.scriptCommand, Environment.NewLine);
						ProbeResult result = base.Result;
						result.ExecutionContext += text;
						WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, text, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Background\\Probes\\FfoPSScriptLAMProbe.cs", 143);
						text = string.Format("$DebugPreference='{0}', $VerbosePreference='{1}', $WarningPreference='{2}', $ErrorPreference='{3}'. {4}", new object[]
						{
							this.debugPreference,
							this.verbosePreference,
							this.warningPreference,
							this.errorPreference,
							Environment.NewLine
						});
						ProbeResult result2 = base.Result;
						result2.ExecutionContext += text;
						WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, text, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Background\\Probes\\FfoPSScriptLAMProbe.cs", 148);
						try
						{
							Collection<PSObject> collection = powerShell.Invoke();
							WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Processing result...", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Background\\Probes\\FfoPSScriptLAMProbe.cs", 153);
							base.Result.SampleValue = 1.0;
							string value = "A command that prompts the user failed because the host program or the command type does not support user interaction";
							foreach (PSObject arg in collection)
							{
								stringBuilder2.AppendFormat("INFO: {0} {1} ", arg, Environment.NewLine);
							}
							foreach (DebugRecord arg2 in powerShell.Streams.Debug)
							{
								stringBuilder2.AppendFormat("DEBUG: {0} {1} ", arg2, Environment.NewLine);
							}
							foreach (WarningRecord arg3 in powerShell.Streams.Warning)
							{
								stringBuilder2.AppendFormat("WARNING: {0} {1} ", arg3, Environment.NewLine);
							}
							foreach (VerboseRecord arg4 in powerShell.Streams.Verbose)
							{
								stringBuilder2.AppendFormat("VERBOSE: {0} {1} ", arg4, Environment.NewLine);
							}
							foreach (ErrorRecord errorRecord in powerShell.Streams.Error)
							{
								if (errorRecord.ToString().IndexOf(value) == -1)
								{
									stringBuilder.AppendFormat("ERROR: {0} {1} ", errorRecord, Environment.NewLine);
								}
							}
						}
						catch (FfoLAMProbeException ex)
						{
							ProbeResult result3 = base.Result;
							result3.ExecutionContext += "Probe execution failed";
							throw new FfoLAMProbeException(string.Format("Executing Powershell Probe Command: '{0}' failed with error: {1} {2}", this.scriptCommand, Environment.NewLine, ex.Message));
						}
						finally
						{
							runspace.Close();
						}
					}
					finally
					{
						if (powerShell2 != null)
						{
							((IDisposable)powerShell2).Dispose();
						}
					}
					runspace.Close();
					ProbeResult result4 = base.Result;
					result4.ExecutionContext += stringBuilder2.ToString();
					ProbeResult result5 = base.Result;
					result5.ExecutionContext += string.Format("FfoPSScriptLAMProbe completed at {0}.{1}", DateTime.UtcNow, Environment.NewLine);
					string text2 = stringBuilder.ToString();
					if (text2.Length > 0)
					{
						WTFDiagnostics.TraceFunction<string, string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Probe execution reported errors {0} on machine {1}", text2, Environment.MachineName, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Background\\Probes\\FfoPSScriptLAMProbe.cs", 207);
						base.Result.FailureContext = text2;
						base.Result.Error = text2;
						throw new FfoLAMProbeException(text2);
					}
					return;
				}
			}
			WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "FfoPSScriptLAMProbe terminated due to missing script command. Check the probe definition.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Background\\Probes\\FfoPSScriptLAMProbe.cs", 217);
			throw new FfoLAMProbeException("The probe had no specified command to run despite being enabled. This could be a coverage gap, please check the probe defintion to ensure it is correctly defined.");
		}

		private string CreateScriptCommand(string scriptPath, string parameters)
		{
			string text = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\HealthManager", "MsiInstallPath", null) as string;
			string text2 = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\FfoBackground\\Setup", "MsiInstallPath", null) as string;
			if (string.IsNullOrEmpty(text2))
			{
				text2 = text;
			}
			if (string.IsNullOrEmpty(text2))
			{
				throw new FfoLAMProbeException(string.Format("Could not retrieve Install Path on Machine {0}", Environment.MachineName));
			}
			WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, string.Format("Creating Powershell Command:  ScriptPath: {0}, Parameters: {1}", scriptPath, parameters), null, "CreateScriptCommand", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Background\\Probes\\FfoPSScriptLAMProbe.cs", 242);
			string text3 = Path.Combine(text2, scriptPath);
			StringBuilder stringBuilder = new StringBuilder();
			if (File.Exists(text3))
			{
				if (!string.IsNullOrEmpty(parameters))
				{
					string[] array = parameters.Split(new char[]
					{
						';'
					});
					foreach (string text4 in array)
					{
						if (text4.IndexOf("=") == -1)
						{
							stringBuilder.AppendFormat(" -{0}", text4);
						}
						else
						{
							string[] array3 = text4.Split(new char[]
							{
								'='
							});
							stringBuilder.AppendFormat(" -{0} {1}", array3[0], array3[1]);
						}
					}
				}
				return string.Format("&(gi '{0}') {1}", text3, stringBuilder);
			}
			throw new FfoLAMProbeException(string.Format("Probe Script file {0} could not be found on Machine {1}. Install Path: {2}, ScriptPath: {3}", new object[]
			{
				text3,
				Environment.MachineName,
				text2,
				scriptPath
			}));
		}

		private string scriptCommand;

		private string warningPreference;

		private string verbosePreference;

		private string errorPreference;

		private string debugPreference;

		internal static class AttributeNames
		{
			internal const string ScriptPath = "ScriptPath";

			internal const string Parameters = "Parameters";

			internal const string DebugPreference = "DebugPreference";

			internal const string VerbosePreference = "VerbosePreference";

			internal const string ErrorPreference = "ErrorPreference";

			internal const string WarningPreference = "WarningPreference";
		}

		internal static class DefaultValues
		{
			internal const string ScriptPath = "Datacenter";

			internal const string DebugPreference = "SilentlyContinue";

			internal const string VerbosePreference = "SilentlyContinue";
		}
	}
}
