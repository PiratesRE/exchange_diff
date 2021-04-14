using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	internal abstract class ActiveDirectoryConnectivityBase : DisposeTrackableBase
	{
		protected ActiveDirectoryConnectivityBase(ActiveDirectoryConnectivityContext context)
		{
			this.context = context;
			this.CanContinue = true;
		}

		protected abstract IEnumerable<AsyncResult<ActiveDirectoryConnectivityOutcome>> BuildTransactions();

		protected ActiveDirectoryConnectivityOutcome CreateOutcome(TestActiveDirectoryConnectivityTask.ScenarioId id, LocalizedString scenario, string performanceCounter, string domainController)
		{
			return new ActiveDirectoryConnectivityOutcome(this.context, id, scenario, performanceCounter, domainController);
		}

		protected void WriteVerbose(LocalizedString message)
		{
			this.context.WriteVerbose(message);
		}

		protected void WriteWarning(LocalizedString message)
		{
			this.context.WriteWarning(message);
		}

		protected void WriteDebug(LocalizedString message)
		{
			this.context.WriteDebug(message);
		}

		protected void AddMonitoringEvent(TestActiveDirectoryConnectivityTask.ScenarioId eventId, EventTypeEnumeration eventType, LocalizedString eventMessage)
		{
			if (this.context.MonitoringData == null)
			{
				return;
			}
			string text = eventMessage.ToString();
			if (eventType != EventTypeEnumeration.Success)
			{
				text = this.context.GetDiagnosticInfo(text);
			}
			MonitoringEvent item = new MonitoringEvent(TestActiveDirectoryConnectivityTask.CmdletMonitoringEventSource, (int)eventId, eventType, text);
			this.context.MonitoringData.Events.Add(item);
		}

		protected ActiveDirectoryConnectivityContext Context
		{
			get
			{
				return this.context;
			}
		}

		protected bool CanContinue { get; set; }

		protected DomainController CurrentDomainController { get; set; }

		protected LocalizedString GenerateErrorMessage(TestActiveDirectoryConnectivityTask.ScenarioId scenario, int errorCode, string errorMessage, LocalizedException e)
		{
			string error = errorCode.ToString() + ((errorMessage == null) ? "" : ("(" + errorMessage + ")"));
			return Strings.ActiveDirectoryConnectivityTestFailed(scenario.ToString(), error, (e == null) ? "<Null>" : e.ToString());
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ActiveDirectoryConnectivityBase>(this);
		}

		protected static AsyncResult<ActiveDirectoryConnectivityOutcome> SingleCommandTransactionAsync(ActiveDirectoryConnectivityBase.ActiveDirectoryConnectivityAsyncTask setupCommand)
		{
			AsyncResult<ActiveDirectoryConnectivityOutcome> asyncResult = new AsyncResult<ActiveDirectoryConnectivityOutcome>();
			setupCommand(asyncResult);
			return asyncResult;
		}

		protected static AsyncResult<ActiveDirectoryConnectivityOutcome> SingleCommandTransactionSync(ActiveDirectoryConnectivityBase.ActiveDirectoryConnectivityTask command)
		{
			AsyncResult<ActiveDirectoryConnectivityOutcome> result = new AsyncResult<ActiveDirectoryConnectivityOutcome>();
			ActiveDirectoryConnectivityBase.SingleCommandTransaction(command, result);
			return result;
		}

		protected static AsyncResult<ActiveDirectoryConnectivityOutcome> SingleCommandTransaction(ActiveDirectoryConnectivityBase.ActiveDirectoryConnectivityTask command, AsyncResult<ActiveDirectoryConnectivityOutcome> result)
		{
			ExDateTime now = ExDateTime.Now;
			ActiveDirectoryConnectivityOutcome item = command();
			result.Outcomes.Add(item);
			result.Complete();
			return result;
		}

		protected virtual string GenerateReportContext()
		{
			return null;
		}

		internal static IEnumerable<AsyncResult<ActiveDirectoryConnectivityOutcome>> BuildTransactionHelper(params Func<ActiveDirectoryConnectivityBase>[] createInstances)
		{
			int i = 0;
			while (i < createInstances.Length)
			{
				Func<ActiveDirectoryConnectivityBase> createInstance = createInstances[i];
				using (ActiveDirectoryConnectivityBase instance = createInstance())
				{
					if (instance != null)
					{
						goto IL_78;
					}
				}
				IL_DF:
				i++;
				continue;
				goto IL_DF;
				IL_78:
				ActiveDirectoryConnectivityBase instance;
				foreach (AsyncResult<ActiveDirectoryConnectivityOutcome> transaction in instance.BuildTransactions())
				{
					yield return transaction;
				}
				goto IL_DF;
			}
			yield break;
		}

		private readonly ActiveDirectoryConnectivityContext context;

		protected delegate ActiveDirectoryConnectivityOutcome ActiveDirectoryConnectivityTask();

		protected delegate void ActiveDirectoryConnectivityAsyncTask(AsyncResult<ActiveDirectoryConnectivityOutcome> state);
	}
}
