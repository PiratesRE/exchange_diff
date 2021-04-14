using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Security;
using System.Text;
using Microsoft.Exchange.Common.Cluster;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class OsComponentManager
	{
		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.ClusterTracer;
			}
		}

		internal OsComponentManager(string machineName, HaTaskStringBuilderOutputHelper output)
		{
			if (machineName != null && !Cluster.StringIEquals(machineName, Environment.MachineName))
			{
				throw new NotImplementedException("OsComponentManager is not remoteable yet.");
			}
			this.m_output = output;
		}

		internal void AddWindowsFeature(OsComponent componentId)
		{
			if (componentId >= OsComponent.ComponentMax)
			{
				throw new ArgumentOutOfRangeException();
			}
			string text = OsComponentManager.m_OsComponents[(int)componentId];
			OsComponentManager.Tracer.TraceDebug<string>((long)this.GetHashCode(), "AddWindowsFeature(): Adding the following Windows Feature: {0}.", text);
			Exception ex = null;
			try
			{
				using (PowerShell powerShell = PowerShell.Create())
				{
					PSCommand pscommand = new PSCommand();
					if (OsComponentManager.Tracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						OsComponentManager.Tracer.TraceDebug((long)this.GetHashCode(), "AddWindowsFeature() Running: PS> Invoke-Expression $env:PSModulePath");
						this.m_output.WriteVerbose(new LocalizedString("Running PS> Invoke-Expression $env:PSModulePath"));
						pscommand.Clear();
						pscommand.AddCommand("Invoke-Expression").AddArgument("$env:PSModulePath");
						this.RunPSCommand(powerShell, pscommand, true);
						OsComponentManager.Tracer.TraceDebug((long)this.GetHashCode(), "AddWindowsFeature() Running: PS> Get-Module -ListAvailable -All");
						this.m_output.WriteVerbose(new LocalizedString("Running PS> Get-Module -ListAvailable -All"));
						pscommand.Clear();
						pscommand.AddCommand("Get-Module").AddParameter("ListAvailable", true).AddParameter("All", true);
						this.RunPSCommand(powerShell, pscommand, true);
					}
					OsComponentManager.Tracer.TraceDebug((long)this.GetHashCode(), "AddWindowsFeature() Running: PS> Import-Module -Name ServerManager");
					this.m_output.WriteVerbose(new LocalizedString("Running PS> Import-Module -Name ServerManager"));
					pscommand.Clear();
					pscommand.AddCommand("Import-Module").AddParameter("Name", "ServerManager");
					this.RunPSCommand(powerShell, pscommand, false);
					OsComponentManager.Tracer.TraceDebug<string>((long)this.GetHashCode(), "AddWindowsFeature() Running: PS> Add-WindowsFeature -Name {0} -IncludeAllSubFeature", text);
					this.m_output.WriteVerbose(new LocalizedString(string.Format("Running PS> Add-WindowsFeature -Name {0} -IncludeAllSubFeature", text)));
					pscommand.Clear();
					pscommand.AddCommand("Add-WindowsFeature").AddParameter("Name", text).AddParameter("IncludeAllSubFeature", true);
					Collection<PSObject> collection = this.RunPSCommand(powerShell, pscommand, false);
					PSObject psobject = collection[0];
					PSMemberInfo psmemberInfo = psobject.Members["Success"];
					if (psmemberInfo == null || !(bool)psmemberInfo.Value)
					{
						throw new DagTaskComponentManagerServerManagerPSFailure(ReplayStrings.AmBcsNoneSpecified);
					}
					PSMemberInfo psmemberInfo2 = psobject.Members["RestartNeeded"];
					if (psmemberInfo2 == null)
					{
						throw new DagTaskComponentManagerServerManagerPSFailure(ReplayStrings.AmBcsNoneSpecified);
					}
					if ((int)psmemberInfo2.Value == 0 || (int)psmemberInfo2.Value == 2)
					{
						OsComponentManager.Tracer.TraceError((long)this.GetHashCode(), "AddWindowsFeature(): A server reboot may be required!");
						this.m_output.WriteWarning(ReplayStrings.DagTaskComponentManagerWantsToRebootException);
					}
				}
			}
			catch (InvalidOperationException ex2)
			{
				ex = ex2;
			}
			catch (ScriptCallDepthException ex3)
			{
				ex = ex3;
			}
			catch (ParameterBindingException ex4)
			{
				ex = ex4;
			}
			catch (CmdletProviderInvocationException ex5)
			{
				ex = ex5;
			}
			catch (CmdletInvocationException ex6)
			{
				ex = ex6;
			}
			catch (ActionPreferenceStopException ex7)
			{
				ex = ex7;
			}
			catch (PipelineStoppedException ex8)
			{
				ex = ex8;
			}
			catch (MetadataException ex9)
			{
				ex = ex9;
			}
			catch (RuntimeException ex10)
			{
				ex = ex10;
			}
			catch (SecurityException ex11)
			{
				ex = ex11;
			}
			if (ex != null)
			{
				throw new DagTaskComponentManagerServerManagerPSFailure(ex.Message, ex);
			}
		}

		private Collection<PSObject> RunPSCommand(PowerShell ps, PSCommand cmd, bool asString)
		{
			ps.Streams.ClearStreams();
			ps.Commands = cmd;
			if (asString)
			{
				cmd.AddCommand("Out-String");
				ps.Commands = cmd;
			}
			Collection<PSObject> collection = null;
			try
			{
				collection = ps.Invoke();
			}
			catch (CommandNotFoundException ex)
			{
				throw new DagTaskComponentManagerServerManagerPSFailure(ex.Message, ex);
			}
			foreach (PSObject psobject in collection)
			{
				OsComponentManager.Tracer.TraceDebug<PSObject>((long)this.GetHashCode(), "RunPSCommand(): Produced result: {0}", psobject);
				string text = this.GenerateStringForResult(psobject);
				this.m_output.AppendLogMessage("Produced result: {0}", new object[]
				{
					text
				});
			}
			string text2 = null;
			if (ps.Streams.Error.Count > 0)
			{
				text2 = this.GetPsStream<ErrorRecord>(ps.Streams.Error);
				OsComponentManager.Tracer.TraceError<string>((long)this.GetHashCode(), "RunPSCommand(): ERROR STREAM: {0}", text2);
			}
			if (ps.Streams.Warning.Count > 0)
			{
				OsComponentManager.Tracer.TraceError<string>((long)this.GetHashCode(), "RunPSCommand(): WARNING STREAM: {0}", this.GetPsStream<WarningRecord>(ps.Streams.Warning));
			}
			if (ps.Streams.Verbose.Count > 0)
			{
				OsComponentManager.Tracer.TraceDebug<string>((long)this.GetHashCode(), "RunPSCommand(): VERBOSE STREAM: {0}", this.GetPsStream<VerboseRecord>(ps.Streams.Verbose));
			}
			if (ps.Streams.Debug.Count > 0)
			{
				OsComponentManager.Tracer.TraceDebug<string>((long)this.GetHashCode(), "RunPSCommand(): DEBUG STREAM: {0}", this.GetPsStream<DebugRecord>(ps.Streams.Debug));
			}
			if (!string.IsNullOrEmpty(text2))
			{
				throw new DagTaskComponentManagerServerManagerPSFailure(text2);
			}
			return collection;
		}

		private string GenerateStringForResult(PSObject res)
		{
			string text = res.ToString();
			if (text == "Microsoft.Windows.ServerManager.Commands.FeatureOperationResult")
			{
				try
				{
					StringBuilder stringBuilder = new StringBuilder();
					string[] array = new string[]
					{
						"Success",
						"ExitCode",
						"RestartNeeded"
					};
					bool flag = false;
					foreach (string text2 in array)
					{
						PSPropertyInfo pspropertyInfo = res.Properties[text2];
						if (pspropertyInfo != null)
						{
							if (flag)
							{
								stringBuilder.Append(", ");
							}
							stringBuilder.Append(text2);
							stringBuilder.Append(": ");
							stringBuilder.Append(pspropertyInfo.Value);
							flag = true;
						}
					}
					return stringBuilder.ToString();
				}
				catch (ArgumentException arg)
				{
					OsComponentManager.Tracer.TraceError<ArgumentException>((long)this.GetHashCode(), "GenerateStringForResult: {0}", arg);
					return text;
				}
				return text;
			}
			return text;
		}

		private string GetPsStream<T>(PSDataCollection<T> collection)
		{
			string result = null;
			if (collection != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (T t in collection)
				{
					stringBuilder.AppendLine(t.ToString());
				}
				result = stringBuilder.ToString();
			}
			return result;
		}

		private static readonly string[] m_OsComponents = new string[]
		{
			"Failover-Clustering",
			"RSAT-Clustering",
			"MAX_SENTINEL"
		};

		private readonly HaTaskStringBuilderOutputHelper m_output;
	}
}
