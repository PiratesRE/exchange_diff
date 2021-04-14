using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	internal sealed class MachinePingTask : ActiveDirectoryConnectivityBase
	{
		internal MachinePingTask(ActiveDirectoryConnectivityContext context) : base(context)
		{
			base.CanContinue = !context.Instance.SkipRemainingTests;
		}

		protected override IEnumerable<AsyncResult<ActiveDirectoryConnectivityOutcome>> BuildTransactions()
		{
			yield return ActiveDirectoryConnectivityBase.SingleCommandTransactionSync(new ActiveDirectoryConnectivityBase.ActiveDirectoryConnectivityTask(this.MachinePing));
			yield break;
		}

		protected override void InternalDispose(bool isDisposing)
		{
		}

		private ActiveDirectoryConnectivityOutcome IsNTDSRunning()
		{
			ActiveDirectoryConnectivityOutcome activeDirectoryConnectivityOutcome = base.CreateOutcome(TestActiveDirectoryConnectivityTask.ScenarioId.IsNTDSRunning, Strings.IsNTDSRunningScenario, Strings.IsNTDSRunningScenario, base.Context.CurrentDomainController);
			if (!base.CanContinue)
			{
				base.WriteVerbose(Strings.CannotContinue(Strings.IsNTDSRunningScenario));
				activeDirectoryConnectivityOutcome.Update(CasTransactionResultEnum.Skipped);
				return activeDirectoryConnectivityOutcome;
			}
			return this.IsServiceRunning("NTDS", activeDirectoryConnectivityOutcome, TestActiveDirectoryConnectivityTask.ScenarioId.NTDSNotRunning);
		}

		private ActiveDirectoryConnectivityOutcome IsNetlogonRunning()
		{
			ActiveDirectoryConnectivityOutcome activeDirectoryConnectivityOutcome = base.CreateOutcome(TestActiveDirectoryConnectivityTask.ScenarioId.IsNetlogonRunning, Strings.IsNetlogonRunningScenario, Strings.IsNetlogonRunningScenario, base.Context.CurrentDomainController);
			if (!base.CanContinue)
			{
				base.WriteVerbose(Strings.CannotContinue(Strings.IsNetlogonRunningScenario));
				activeDirectoryConnectivityOutcome.Update(CasTransactionResultEnum.Skipped);
				return activeDirectoryConnectivityOutcome;
			}
			return this.IsServiceRunning("Netlogon", activeDirectoryConnectivityOutcome, TestActiveDirectoryConnectivityTask.ScenarioId.NetLogonNotRunning);
		}

		private ActiveDirectoryConnectivityOutcome IsServiceRunning(string serviceName, ActiveDirectoryConnectivityOutcome outcome, TestActiveDirectoryConnectivityTask.ScenarioId failureId)
		{
			try
			{
				using (ServiceController serviceController = new ServiceController(serviceName, base.Context.CurrentDomainController))
				{
					if (serviceController.Status != ServiceControllerStatus.Running)
					{
						outcome.Update(CasTransactionResultEnum.Failure, TimeSpan.Zero, Strings.ServiceNotRunning(serviceName));
						base.AddMonitoringEvent(failureId, EventTypeEnumeration.Error, base.GenerateErrorMessage(outcome.Id, 0, Strings.ServiceNotRunning(serviceName), null));
						return outcome;
					}
				}
			}
			catch (InvalidOperationException ex)
			{
				outcome.Update(CasTransactionResultEnum.Failure, TimeSpan.Zero, ex.ToString());
				base.AddMonitoringEvent(failureId, EventTypeEnumeration.Error, base.GenerateErrorMessage(outcome.Id, 0, ex.ToString(), null));
				return outcome;
			}
			outcome.Update(CasTransactionResultEnum.Success);
			return outcome;
		}

		private ActiveDirectoryConnectivityOutcome MachinePing()
		{
			ActiveDirectoryConnectivityOutcome activeDirectoryConnectivityOutcome = base.CreateOutcome(TestActiveDirectoryConnectivityTask.ScenarioId.MachinePing, Strings.MachinePingScenario, Strings.MachinePingScenario, base.Context.CurrentDomainController);
			if (!base.CanContinue)
			{
				base.WriteVerbose(Strings.CannotContinue(Strings.MachinePingScenario));
				activeDirectoryConnectivityOutcome.Update(CasTransactionResultEnum.Skipped);
				return activeDirectoryConnectivityOutcome;
			}
			try
			{
				Dns.GetHostEntry(base.Context.CurrentDomainController);
				activeDirectoryConnectivityOutcome.Update(CasTransactionResultEnum.Success);
			}
			catch (SocketException ex)
			{
				base.Context.Instance.SkipRemainingTests = true;
				activeDirectoryConnectivityOutcome.Update(CasTransactionResultEnum.Failure);
				base.AddMonitoringEvent(TestActiveDirectoryConnectivityTask.ScenarioId.MachinePingFailed, EventTypeEnumeration.Error, base.GenerateErrorMessage(TestActiveDirectoryConnectivityTask.ScenarioId.MachinePing, 0, ex.Message, null));
			}
			return activeDirectoryConnectivityOutcome;
		}
	}
}
