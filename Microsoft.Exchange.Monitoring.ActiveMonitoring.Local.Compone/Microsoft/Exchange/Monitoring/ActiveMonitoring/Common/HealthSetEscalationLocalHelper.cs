using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring.Management.Common;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal class HealthSetEscalationLocalHelper : HealthSetEscalationHelper
	{
		static HealthSetEscalationLocalHelper()
		{
			RunspaceFactory runspaceFactory = new RunspaceFactory(new HealthSetEscalationLocalHelper.FullExchangeRunspaceConfigurationFactory(), new BasicPSHostFactory(typeof(RunspaceHost)));
			HealthSetEscalationLocalHelper.Runspace = new RunspaceProxy(new RunspaceMediator(runspaceFactory, new EmptyRunspaceCache()), true);
		}

		~HealthSetEscalationLocalHelper()
		{
			if (HealthSetEscalationLocalHelper.Runspace != null)
			{
				HealthSetEscalationLocalHelper.Runspace.Dispose();
				HealthSetEscalationLocalHelper.Runspace = null;
			}
		}

		internal override HealthSetEscalationState LockHealthSetEscalationStateIfRequired(string healthSetName, EscalationState escalationState, string lockOwnerId)
		{
			return RpcLockHealthSetEscalationStateIfRequiredImpl.SendRequest(Environment.MachineName, healthSetName, escalationState, lockOwnerId, 30000);
		}

		internal override bool SetHealthSetEscalationState(string healthSetName, EscalationState escalationState, string lockOwnerId)
		{
			return RpcSetHealthSetEscalationStateImpl.SendRequest(Environment.MachineName, healthSetName, escalationState, lockOwnerId, 30000);
		}

		internal override void ExtendEscalationMessage(string healthSetName, ref string escalationMessage)
		{
			try
			{
				PSCommand pscommand = new PSCommand();
				pscommand.AddCommand("Get-HealthReport");
				pscommand.AddParameter("Identity", Environment.MachineName);
				PowerShellProxy powerShellProxy = new PowerShellProxy(HealthSetEscalationLocalHelper.Runspace, pscommand);
				Collection<PSObject> collection = powerShellProxy.Invoke<PSObject>();
				if (collection == null || collection.Count == 0 || !(collection[0].BaseObject is ConsolidatedHealth))
				{
					WTFDiagnostics.TraceError(ExTraceGlobals.CommonComponentsTracer, HealthSetEscalationLocalHelper.traceContext, "Invalid results returned from Get-HealthReport.", null, "ExtendEscalationMessage", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Responders\\HealthSetEscalationLocalHelper.cs", 123);
				}
				else
				{
					StringBuilder stringBuilder = new StringBuilder();
					MultiValuedProperty<MonitorHealthEntry> multiValuedProperty = null;
					stringBuilder.AppendLine();
					stringBuilder.AppendLine();
					stringBuilder.AppendFormat(Strings.HealthSetsStates(string.Format("Get-HealthReport -Identity '{0}'", Environment.MachineName)), new object[0]);
					stringBuilder.AppendLine();
					stringBuilder.AppendLine();
					stringBuilder.AppendFormat("{0,-20}{1,-30}{2,-15}{3,-25}{4,-20}", new object[]
					{
						"State",
						"HealthSet",
						"AlertValue",
						"LastTransitionTime",
						"MonitorCount"
					});
					stringBuilder.AppendLine();
					stringBuilder.AppendFormat("{0,-20}{1,-30}{2,-15}{3,-25}{4,-20}", new object[]
					{
						"-----",
						"---------",
						"----------",
						"------------------",
						"------------"
					});
					stringBuilder.AppendLine();
					foreach (PSObject psobject in collection)
					{
						if (psobject.BaseObject != null && psobject.BaseObject is ConsolidatedHealth)
						{
							ConsolidatedHealth consolidatedHealth = (ConsolidatedHealth)psobject.BaseObject;
							stringBuilder.AppendFormat("{0}{1}{2}{3}{4}", new object[]
							{
								HealthSetEscalationLocalHelper.FormatAsString(consolidatedHealth.State, 20),
								HealthSetEscalationLocalHelper.FormatAsString(consolidatedHealth.HealthSet, 30),
								HealthSetEscalationLocalHelper.FormatAsString(consolidatedHealth.AlertValue, 15),
								HealthSetEscalationLocalHelper.FormatAsString(consolidatedHealth.LastTransitionTime, 25),
								HealthSetEscalationLocalHelper.FormatAsString(consolidatedHealth.MonitorCount, 20)
							});
							stringBuilder.AppendLine();
							if (string.Equals(consolidatedHealth.HealthSet, healthSetName, StringComparison.InvariantCultureIgnoreCase))
							{
								multiValuedProperty = consolidatedHealth.Entries;
							}
						}
					}
					StringBuilder stringBuilder2 = new StringBuilder();
					if (multiValuedProperty != null)
					{
						stringBuilder2.AppendLine();
						stringBuilder2.AppendLine();
						stringBuilder2.AppendFormat(Strings.HealthSetMonitorsStates(string.Format("Get-ServerHealth -Identity '{0}' -HealthSet '{1}'", Environment.MachineName, healthSetName)), new object[0]);
						stringBuilder2.AppendLine();
						stringBuilder2.AppendLine();
						stringBuilder2.AppendFormat("{0,-20}{1,-40}{2,-35}{3,-30}{4,-15}{5,-20}", new object[]
						{
							"State",
							"Name",
							"TargetResource",
							"HealthSet",
							"AlertValue",
							"ServerComponent"
						});
						stringBuilder2.AppendLine();
						stringBuilder2.AppendFormat("{0,-20}{1,-40}{2,-35}{3,-30}{4,-15}{5,-20}", new object[]
						{
							"-----",
							"----",
							"--------------",
							"---------",
							"----------",
							"---------------"
						});
						stringBuilder2.AppendLine();
						foreach (MonitorHealthEntry monitorHealthEntry in multiValuedProperty)
						{
							if (monitorHealthEntry != null)
							{
								stringBuilder2.AppendFormat("{0}{1}{2}{3}{4}{5}", new object[]
								{
									HealthSetEscalationLocalHelper.FormatAsString(monitorHealthEntry.CurrentHealthSetState, 20),
									HealthSetEscalationLocalHelper.FormatAsString(monitorHealthEntry.Name, 40),
									HealthSetEscalationLocalHelper.FormatAsString(monitorHealthEntry.TargetResource, 35),
									HealthSetEscalationLocalHelper.FormatAsString(monitorHealthEntry.HealthSetName, 30),
									HealthSetEscalationLocalHelper.FormatAsString(monitorHealthEntry.AlertValue, 15),
									HealthSetEscalationLocalHelper.FormatAsString(monitorHealthEntry.ServerComponentName, 20)
								});
								stringBuilder2.AppendLine();
							}
						}
					}
					escalationMessage = string.Format("{0}{1}{2}{3}{4}", new object[]
					{
						escalationMessage,
						"\r\n\r\n-------------------------------------------------------------------------------",
						stringBuilder2.ToString(),
						stringBuilder.ToString(),
						Strings.HealthSetAlertSuppressionWarning
					});
				}
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceError<string>(ExTraceGlobals.CommonComponentsTracer, HealthSetEscalationLocalHelper.traceContext, "Failed to process results from Get-HealthReport: {0}", ex.ToString(), null, "ExtendEscalationMessage", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Responders\\HealthSetEscalationLocalHelper.cs", 203);
			}
		}

		private static string FormatAsString(object value, int columnWidth)
		{
			if (value == null)
			{
				return string.Empty.PadRight(columnWidth);
			}
			if (columnWidth == 0)
			{
				return string.Empty;
			}
			if (columnWidth == 1)
			{
				return " ";
			}
			string text = value.ToString();
			if (text.Length >= columnWidth)
			{
				if (columnWidth >= 4)
				{
					text = text.Substring(0, columnWidth - 4) + "... ";
				}
				else
				{
					text = "* ".PadRight(columnWidth);
				}
			}
			else
			{
				text = text.PadRight(columnWidth);
			}
			return text;
		}

		private static RunspaceProxy Runspace;

		private static TracingContext traceContext = TracingContext.Default;

		private class FullExchangeRunspaceConfigurationFactory : RunspaceConfigurationFactory
		{
			public static HealthSetEscalationLocalHelper.FullExchangeRunspaceConfigurationFactory GetInstance()
			{
				if (HealthSetEscalationLocalHelper.FullExchangeRunspaceConfigurationFactory.instance == null)
				{
					HealthSetEscalationLocalHelper.FullExchangeRunspaceConfigurationFactory.instance = new HealthSetEscalationLocalHelper.FullExchangeRunspaceConfigurationFactory();
				}
				return HealthSetEscalationLocalHelper.FullExchangeRunspaceConfigurationFactory.instance;
			}

			public override RunspaceConfiguration CreateRunspaceConfiguration()
			{
				RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();
				HealthSetEscalationLocalHelper.FullExchangeRunspaceConfigurationFactory.AddPSSnapIn(runspaceConfiguration, "Microsoft.Exchange.Management.PowerShell.E2010");
				return runspaceConfiguration;
			}

			private static void AddPSSnapIn(RunspaceConfiguration runspaceConfiguration, string mshSnapInName)
			{
				PSSnapInException ex = null;
				runspaceConfiguration.AddPSSnapIn(mshSnapInName, out ex);
				if (ex != null)
				{
					throw new Exception(Strings.CouldNotAddExchangeSnapInExceptionMessage(mshSnapInName), ex);
				}
			}

			private static HealthSetEscalationLocalHelper.FullExchangeRunspaceConfigurationFactory instance;
		}
	}
}
