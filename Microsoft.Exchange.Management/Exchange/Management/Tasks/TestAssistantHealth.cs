using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Management.Automation;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Monitoring;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Test", "AssistantHealth", DefaultParameterSetName = "AssistantHealthParameterSetName", SupportsShouldProcess = true)]
	public sealed class TestAssistantHealth : Task
	{
		[Parameter(ParameterSetName = "AssistantHealthParameterSetName", Position = 0, ValueFromPipeline = true)]
		public ServerIdParameter ServerName
		{
			get
			{
				return (ServerIdParameter)(base.Fields["ServerName"] ?? ServerIdParameter.Parse(Environment.MachineName));
			}
			set
			{
				base.Fields["ServerName"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AssistantHealthParameterSetName")]
		public SwitchParameter IncludeCrashDump
		{
			get
			{
				return (SwitchParameter)(base.Fields["IncludeCrashDump"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["IncludeCrashDump"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AssistantHealthParameterSetName")]
		public SwitchParameter MonitoringContext
		{
			get
			{
				return (SwitchParameter)(base.Fields["MonitoringContext"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["MonitoringContext"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AssistantHealthParameterSetName")]
		public SwitchParameter ResolveProblems
		{
			get
			{
				return (SwitchParameter)(base.Fields["ResolveProblems"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ResolveProblems"] = value;
			}
		}

		[ValidateRange(1, 3600)]
		[Parameter(Mandatory = false, ParameterSetName = "AssistantHealthParameterSetName")]
		public uint MaxProcessingTimeInMinutes
		{
			get
			{
				return (uint)base.Fields["MaxProcessingTimeInMinutes"];
			}
			set
			{
				base.Fields["MaxProcessingTimeInMinutes"] = value;
			}
		}

		[ValidateRange(0, 10080)]
		[Parameter(Mandatory = false, ParameterSetName = "AssistantHealthParameterSetName")]
		public uint WatermarkBehindWarningThreholdInMinutes
		{
			get
			{
				return (uint)base.Fields["WatermarkBehindWarningThreholdInMinutes"];
			}
			set
			{
				base.Fields["WatermarkBehindWarningThreholdInMinutes"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationTestAssistantHealth;
			}
		}

		protected override bool IsKnownException(Exception e)
		{
			return e is Win32Exception || e is InvalidOperationException || e is ManagementObjectNotFoundException || e is ManagementObjectAmbiguousException || e is System.ServiceProcess.TimeoutException || e is WmiException || e is TSCrashDumpsOnlyAvailableOnLocalMachineException || base.IsKnownException(e);
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			Server server = this.GetServer();
			ExchangeServer exchangeServer = new ExchangeServer(server);
			base.Fields["ExchangeServer"] = exchangeServer;
			if (this.ResolveProblems.IsPresent && this.IncludeCrashDump.IsPresent && !StringComparer.OrdinalIgnoreCase.Equals(exchangeServer.Name, Environment.MachineName))
			{
				throw new TSCrashDumpsOnlyAvailableOnLocalMachineException();
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			MonitoringData monitoringData = null;
			List<TroubleshooterCheck> checkList = this.GetCheckList();
			List<TroubleshooterCheck> failedCheckList = TroubleshooterCheck.RunChecks(checkList, new TroubleshooterCheck.ContinueToNextCheck(TroubleshooterCheck.ShouldContinue), out monitoringData);
			this.RunResolveProblems(monitoringData, checkList, failedCheckList);
			base.WriteObject(monitoringData, true);
			TaskLogger.LogExit();
		}

		private List<TroubleshooterCheck> GetCheckList()
		{
			return new List<TroubleshooterCheck>
			{
				new MailboxServerCheck(base.Fields),
				new MailboxAssistantsServiceStatusCheck(base.Fields),
				new MailboxAssistantsProcessingEvents(base.Fields),
				new MailboxAssistantsWatermarks(base.Fields)
			};
		}

		private Server GetServer()
		{
			ITopologyConfigurationSession session = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(null, true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 172, "GetServer", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxAssistants\\TroubleShooter\\TestAssistantHealth.cs");
			LocalizedString? localizedString = null;
			IEnumerable<Server> objects = this.ServerName.GetObjects<Server>(null, session, new OptionalIdentityData(), out localizedString);
			using (IEnumerator<Server> enumerator = objects.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					Server result = enumerator.Current;
					if (enumerator.MoveNext())
					{
						throw new ManagementObjectAmbiguousException(Strings.ErrorServerNotUnique(this.ServerName.ToString()));
					}
					return result;
				}
			}
			throw new ManagementObjectNotFoundException(localizedString ?? Strings.ErrorManagementObjectNotFound(this.ServerName.ToString()));
		}

		private void RunResolveProblems(MonitoringData monitoringData, List<TroubleshooterCheck> checkList, List<TroubleshooterCheck> failedCheckList)
		{
			if (failedCheckList.Count == 0)
			{
				ExchangeServer exchangeServer = (ExchangeServer)base.Fields["ExchangeServer"];
				monitoringData.Events.Add(new MonitoringEvent(AssistantTroubleshooterBase.EventSource, 5000, EventTypeEnumeration.Information, Strings.TSNoProblemsDetected(exchangeServer.Name)));
				using (List<TroubleshooterCheck>.Enumerator enumerator = checkList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TroubleshooterCheck troubleshooterCheck = enumerator.Current;
						AssistantTroubleshooterBase assistantTroubleshooterBase = troubleshooterCheck as AssistantTroubleshooterBase;
						if (assistantTroubleshooterBase != null)
						{
							monitoringData.PerformanceCounters.Add(assistantTroubleshooterBase.GetCrashDumpCountPerformanceCounter());
							break;
						}
					}
					return;
				}
			}
			if (this.ResolveProblems.IsPresent)
			{
				foreach (TroubleshooterCheck troubleshooterCheck2 in failedCheckList)
				{
					troubleshooterCheck2.Resolve(monitoringData);
				}
			}
		}

		private const string AssistantHealthParameterSetName = "AssistantHealthParameterSetName";
	}
}
