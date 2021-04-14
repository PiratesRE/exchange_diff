using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Extensions;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Rps.Probes
{
	public class RpsTipProbeBase : RPSLogonProbe
	{
		protected DateTime StartTime { get; set; }

		protected string ExecutionId
		{
			get
			{
				if (string.IsNullOrEmpty(this.executionId))
				{
					if (base.Definition.Attributes.ContainsKey("ExecutionId"))
					{
						this.executionId = base.Definition.Attributes["ExecutionId"];
					}
					if (string.IsNullOrEmpty(this.executionId))
					{
						this.executionId = DateTime.UtcNow.ToString("yyMMddhhmm");
					}
				}
				return this.executionId;
			}
		}

		protected string ServicePlan
		{
			get
			{
				if (string.IsNullOrEmpty(this.servicePlan))
				{
					if (base.Definition.Attributes.ContainsKey("ServicePlan"))
					{
						this.servicePlan = base.Definition.Attributes["ServicePlan"];
					}
					else
					{
						this.servicePlan = "BPOS";
					}
				}
				return this.servicePlan;
			}
		}

		protected string LiveIDParameterName
		{
			get
			{
				if (this.ServicePlan.Equals("BPOS", StringComparison.OrdinalIgnoreCase))
				{
					return "MicrosoftOnlineServicesID";
				}
				return "WindowsLiveID";
			}
		}

		protected string DomainName
		{
			get
			{
				return base.Definition.Account.Substring(base.Definition.Account.IndexOf("@") + 1);
			}
		}

		protected string HostName
		{
			get
			{
				if (string.IsNullOrEmpty(this.hostName))
				{
					Uri uri = new Uri(base.Definition.Endpoint);
					this.hostName = uri.Host;
				}
				return this.hostName;
			}
		}

		protected string AccountUserName
		{
			get
			{
				return base.Definition.Account.Split(new char[]
				{
					'@'
				})[0];
			}
		}

		public RpsTipProbeBase()
		{
			this.psInvocationSetting = new PSInvocationSettings();
			this.psInvocationSetting.Host = new RunspaceHost();
			this.psInvocationSetting.RemoteStreamOptions = RemoteStreamOptions.AddInvocationInfo;
		}

		protected override Runspace InvokeCmdlet()
		{
			try
			{
				using (PowerShell powerShell = PowerShell.Create())
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.RPSTracer, base.TraceContext, "Begin to execute TIP scenarioes", null, "InvokeCmdlet", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\TipProbes\\RpsTipProbeBase.cs", 177);
					this.ExecuteTipScenarioes(powerShell);
					WTFDiagnostics.TraceInformation(ExTraceGlobals.RPSTracer, base.TraceContext, "End to execute TIP scenarioes", null, "InvokeCmdlet", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\TipProbes\\RpsTipProbeBase.cs", 179);
				}
			}
			catch (Exception innerException)
			{
				base.ThrowFailureException("Execute cmdlet failed.", DateTime.UtcNow - this.StartTime, innerException);
			}
			finally
			{
				try
				{
					base.Result.StateAttribute11 = base.Result.StateAttribute11 + ((WSManConnectionInfo)base.Runspace.ConnectionInfo).ConnectionUri.ToString();
				}
				catch (Exception ex)
				{
					base.Result.StateAttribute11 = base.Result.StateAttribute11 + "Exception:" + ex.ToString();
				}
			}
			return base.Runspace;
		}

		protected virtual void ExecuteTipScenarioes(PowerShell powershell)
		{
		}

		protected Collection<PSObject> ExecuteCmdlet(PowerShell powerShell, Command command)
		{
			this.StartTime = DateTime.UtcNow;
			powerShell.Commands.Clear();
			powerShell.Commands.AddCommand(command);
			powerShell.Runspace = base.Runspace;
			Collection<PSObject> result = powerShell.Invoke(null, this.psInvocationSetting);
			TimeSpan timeSpan = DateTime.UtcNow - this.StartTime;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.GetCommandDescription(command));
			stringBuilder.AppendLine(string.Format(" ({0} seconds)", timeSpan.TotalSeconds));
			stringBuilder.AppendLine();
			if (powerShell.Streams.Error != null && powerShell.Streams.Error.Count > 0)
			{
				stringBuilder.AppendLine("Error Information:");
				foreach (ErrorRecord errorRecord in powerShell.Streams.Error)
				{
					stringBuilder.AppendLine(errorRecord.Exception.ToString());
				}
				throw new ApplicationException(stringBuilder.ToString());
			}
			if (powerShell.Streams.Warning != null && powerShell.Streams.Warning.Count > 0)
			{
				stringBuilder.AppendLine("Warning message: ");
				foreach (WarningRecord warningRecord in powerShell.Streams.Warning)
				{
					stringBuilder.AppendLine(warningRecord.Message);
				}
				powerShell.Streams.Warning.Clear();
			}
			if (string.IsNullOrEmpty(base.Result.StateAttribute5))
			{
				base.Result.StateAttribute5 = stringBuilder.ToString();
			}
			else
			{
				ProbeResult result2 = base.Result;
				result2.StateAttribute5 += stringBuilder.ToString();
			}
			return result;
		}

		protected string GetUniqueName(string baseName)
		{
			return string.Format("{0}_{1}_{2}", baseName, this.ExecutionId, (DateTime.UtcNow.Ticks % 600000000L).ToString());
		}

		protected PSObject CreateMailbox(PowerShell powershell, string baseName)
		{
			string uniqueName = this.GetUniqueName(baseName);
			Collection<PSObject> collection = this.ExecuteCmdlet(powershell, new Command("New-Mailbox")
			{
				Parameters = 
				{
					{
						"Name",
						uniqueName
					},
					{
						this.LiveIDParameterName,
						uniqueName + "@" + this.DomainName
					},
					{
						"Password",
						base.Definition.AccountPassword.ConvertToSecureString()
					},
					{
						"ResetPasswordOnNextLogon",
						false
					}
				}
			});
			if (collection.Count <= 0)
			{
				throw new ApplicationException("New-Mailbox return no result");
			}
			return collection[0];
		}

		protected void RemoveObject(PowerShell powershell, string objectType, string identity)
		{
			if (string.IsNullOrEmpty(objectType) || string.IsNullOrEmpty(identity))
			{
				return;
			}
			this.ExecuteCmdlet(powershell, new Command("Remove-" + objectType)
			{
				Parameters = 
				{
					{
						"Identity",
						identity
					}
				}
			});
		}

		private string GetCommandDescription(Command command)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(command.CommandText);
			foreach (CommandParameter commandParameter in command.Parameters)
			{
				stringBuilder.AppendFormat(" -{0}:{1}", commandParameter.Name, commandParameter.Value);
			}
			return stringBuilder.ToString();
		}

		protected PSObject NewDistributionGroup(PowerShell powershell, string baseName)
		{
			return this.NewDistributionGroup(powershell, baseName, this.AccountUserName);
		}

		protected PSObject NewDistributionGroup(PowerShell powershell, string baseName, string managedBy)
		{
			string uniqueName = this.GetUniqueName(baseName);
			Collection<PSObject> collection = this.ExecuteCmdlet(powershell, new Command("New-DistributionGroup")
			{
				Parameters = 
				{
					{
						"Name",
						uniqueName
					},
					{
						"ManagedBy",
						managedBy
					}
				}
			});
			if (collection.Count <= 0)
			{
				throw new ApplicationException("New-DistributionGroup didn't return any result");
			}
			return collection[0];
		}

		private string executionId;

		private string servicePlan;

		private string hostName;

		private PSInvocationSettings psInvocationSetting;
	}
}
