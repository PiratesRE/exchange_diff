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

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.FfoCentralAdmin
{
	public class FfoPowerShellScriptProbe : ProbeWorkItem
	{
		private string ScriptCommand { get; set; }

		private string WarningPreference { get; set; }

		private string VerbosePreference { get; set; }

		private string ErrorPreference { get; set; }

		private string DebugPreference { get; set; }

		protected virtual void InitializeAttributes(AttributeHelper attributeHelper = null)
		{
			if (attributeHelper == null)
			{
				attributeHelper = new AttributeHelper(base.Definition);
			}
			string @string = attributeHelper.GetString("ScriptPath", true, "Datacenter");
			string string2 = attributeHelper.GetString("Parameters", false, null);
			this.VerbosePreference = attributeHelper.GetString("VerbosePreference", false, "SilentlyContinue");
			this.DebugPreference = attributeHelper.GetString("DebugPreference", false, "SilentlyContinue");
			this.ErrorPreference = attributeHelper.GetString("ErrorPreference", false, null);
			this.WarningPreference = attributeHelper.GetString("WarningPreference", false, null);
			this.ScriptCommand = this.CreateScriptCommand(@string, string2);
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			base.Result.ExecutionContext = string.Format("FfoPowerShellScriptProbe started at {0}.{1}", DateTime.UtcNow, Environment.NewLine);
			this.InitializeAttributes(null);
			WTFDiagnostics.TraceFunction(ExTraceGlobals.CentralAdminTracer, base.TraceContext, "In FfoPowerShellScriptProbe", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\FfoCentralAdmin\\Probes\\FfoPowerShellScriptProbe.cs", 109);
			InitialSessionState initialSessionState = InitialSessionState.CreateDefault();
			initialSessionState.LanguageMode = PSLanguageMode.FullLanguage;
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			WTFDiagnostics.TraceFunction(ExTraceGlobals.CentralAdminTracer, base.TraceContext, "Opening New Runspace", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\FfoCentralAdmin\\Probes\\FfoPowerShellScriptProbe.cs", 116);
			using (Runspace runspace = RunspaceFactory.CreateRunspace(initialSessionState))
			{
				runspace.Open();
				PowerShell powerShell2;
				PowerShell powerShell = powerShell2 = PowerShell.Create();
				try
				{
					powerShell.Commands.AddScript(string.Format("$DebugPreference='{0}'", this.DebugPreference));
					powerShell.Commands.AddScript(string.Format("$VerbosePreference='{0}'", this.VerbosePreference));
					if (!string.IsNullOrEmpty(this.WarningPreference))
					{
						powerShell.Commands.AddScript(string.Format("$WarningPreference='{0}'", this.WarningPreference));
					}
					if (!string.IsNullOrEmpty(this.ErrorPreference))
					{
						powerShell.Commands.AddScript(string.Format("$ErrorPreference='{0}'", this.ErrorPreference));
					}
					powerShell.Commands.AddScript(this.ScriptCommand);
					powerShell.Runspace = runspace;
					string text = string.Format("Executing Powershell Command: {0}. {1}", this.ScriptCommand, Environment.NewLine);
					ProbeResult result = base.Result;
					result.ExecutionContext += text;
					WTFDiagnostics.TraceFunction(ExTraceGlobals.CentralAdminTracer, base.TraceContext, text, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\FfoCentralAdmin\\Probes\\FfoPowerShellScriptProbe.cs", 142);
					text = string.Format("$DebugPreference='{0}', $VerbosePreference='{1}', $WarningPreference='{2}', $ErrorPreference='{3}'. {4}", new object[]
					{
						this.DebugPreference,
						this.VerbosePreference,
						this.WarningPreference,
						this.ErrorPreference,
						Environment.NewLine
					});
					ProbeResult result2 = base.Result;
					result2.ExecutionContext += text;
					WTFDiagnostics.TraceFunction(ExTraceGlobals.CentralAdminTracer, base.TraceContext, text, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\FfoCentralAdmin\\Probes\\FfoPowerShellScriptProbe.cs", 147);
					try
					{
						Collection<PSObject> collection = powerShell.Invoke();
						WTFDiagnostics.TraceFunction(ExTraceGlobals.CentralAdminTracer, base.TraceContext, "Processing result...", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\FfoCentralAdmin\\Probes\\FfoPowerShellScriptProbe.cs", 152);
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
					catch (Exception ex)
					{
						ProbeResult result3 = base.Result;
						result3.ExecutionContext += "Probe execution failed";
						throw new Exception(string.Format("Executing Powershell Probe Command: '{0}' failed with error: {1} {2}", this.ScriptCommand, Environment.NewLine, ex.Message));
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
				result5.ExecutionContext += string.Format("FfoPowerShellScriptProbe completed at {0}.{1}", DateTime.UtcNow, Environment.NewLine);
				string text2 = stringBuilder.ToString();
				if (text2.Length > 0)
				{
					WTFDiagnostics.TraceFunction<string, string>(ExTraceGlobals.CentralAdminTracer, base.TraceContext, "Probe execution reported errors {0} on machine {1}", text2, Environment.MachineName, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\FfoCentralAdmin\\Probes\\FfoPowerShellScriptProbe.cs", 201);
					base.Result.FailureContext = text2;
					base.Result.Error = text2;
					throw new Exception(text2);
				}
			}
		}

		private string CreateScriptCommand(string scriptPath, string parameters)
		{
			string text = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup", "MsiInstallPath", null);
			if (string.IsNullOrEmpty(text))
			{
				throw new Exception(string.Format("Could not retrieve Exchange Install Path on Machine {0}", Environment.MachineName));
			}
			WTFDiagnostics.TraceFunction(ExTraceGlobals.CentralAdminTracer, base.TraceContext, string.Format("Creating Powershell Command:  ScriptPath: {0}, Parameters: {1}", scriptPath, parameters), null, "CreateScriptCommand", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\FfoCentralAdmin\\Probes\\FfoPowerShellScriptProbe.cs", 221);
			string text2 = Path.Combine(text, scriptPath);
			StringBuilder stringBuilder = new StringBuilder();
			if (File.Exists(text2))
			{
				if (!string.IsNullOrEmpty(parameters))
				{
					string[] array = parameters.Split(new char[]
					{
						';'
					});
					foreach (string text3 in array)
					{
						if (text3.IndexOf("=") == -1)
						{
							stringBuilder.AppendFormat(" -{0}", text3);
						}
						else
						{
							string[] array3 = text3.Split(new char[]
							{
								'='
							});
							stringBuilder.AppendFormat(" -{0} {1}", array3[0], array3[1]);
						}
					}
				}
				return string.Format("&(gi '{0}') {1}", text2, stringBuilder);
			}
			throw new Exception(string.Format("Probe Script file {0} could not be found on Machine {1}. Exchange Install Path: {2}, ScriptPath: {3}", new object[]
			{
				text2,
				Environment.MachineName,
				text,
				scriptPath
			}));
		}

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
