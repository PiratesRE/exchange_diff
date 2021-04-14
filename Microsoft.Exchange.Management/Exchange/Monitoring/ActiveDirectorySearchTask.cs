using System;
using System.Collections.Generic;
using System.DirectoryServices;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	internal sealed class ActiveDirectorySearchTask : ActiveDirectoryConnectivityBase
	{
		internal ActiveDirectorySearchTask(ActiveDirectoryConnectivityContext context) : base(context)
		{
			base.CanContinue = !context.Instance.SkipRemainingTests;
		}

		protected override IEnumerable<AsyncResult<ActiveDirectoryConnectivityOutcome>> BuildTransactions()
		{
			if (base.Context.UseADDriver)
			{
				yield return ActiveDirectoryConnectivityBase.SingleCommandTransactionSync(new ActiveDirectoryConnectivityBase.ActiveDirectoryConnectivityTask(this.SearchUsingADDriver));
			}
			else
			{
				yield return ActiveDirectoryConnectivityBase.SingleCommandTransactionSync(new ActiveDirectoryConnectivityBase.ActiveDirectoryConnectivityTask(this.Search));
			}
			yield break;
		}

		protected override void InternalDispose(bool isDisposing)
		{
		}

		private ActiveDirectoryConnectivityOutcome Search()
		{
			return this.RunSearchOperationWithTimeCheck(delegate
			{
				int result;
				using (DirectorySearcher directorySearcher = new DirectorySearcher("(objectClass=*)"))
				{
					directorySearcher.SearchRoot = base.Context.CurrentDCDirectoryEntry;
					directorySearcher.SearchScope = SearchScope.Base;
					directorySearcher.FindAll();
					result = 1;
				}
				return result;
			});
		}

		private ActiveDirectoryConnectivityOutcome SearchUsingADDriver()
		{
			return this.RunSearchOperationWithTimeCheck(delegate
			{
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, "*");
				base.Context.RecipientSession.Find(ADSession.GetDomainNamingContextForLocalForest(), QueryScope.Base, filter, null, 2);
				base.Context.CurrentDomainController = base.Context.RecipientSession.LastUsedDc;
				return 1;
			});
		}

		private ActiveDirectoryConnectivityOutcome RunSearchOperationWithTimeCheck(Func<int> operation)
		{
			ActiveDirectoryConnectivityOutcome activeDirectoryConnectivityOutcome = base.CreateOutcome(TestActiveDirectoryConnectivityTask.ScenarioId.Search, Strings.ActiveDirectorySearchScenario, Strings.ActiveDirectorySearchScenario, base.Context.CurrentDomainController);
			if (!base.CanContinue)
			{
				base.WriteVerbose(Strings.CannotContinue(Strings.ActiveDirectorySearchScenario));
				activeDirectoryConnectivityOutcome.Update(CasTransactionResultEnum.Skipped);
				return activeDirectoryConnectivityOutcome;
			}
			try
			{
				operation();
				activeDirectoryConnectivityOutcome.UpdateTarget(base.Context.CurrentDomainController);
				TimeSpan timeSpan = ExDateTime.Now - activeDirectoryConnectivityOutcome.StartTime;
				if (timeSpan.TotalMilliseconds > (double)base.Context.Instance.SearchLatencyThresholdInMilliseconds)
				{
					activeDirectoryConnectivityOutcome.Update(CasTransactionResultEnum.Success, string.Format("Over Threshold. Threshold :{0} Actual {1}", base.Context.Instance.SearchLatencyThresholdInMilliseconds, timeSpan.TotalMilliseconds));
					base.Context.Instance.WriteVerbose(string.Format("Over Threshold. Threshold :{0} Actual {1}", base.Context.Instance.SearchLatencyThresholdInMilliseconds, timeSpan.TotalMilliseconds));
					base.AddMonitoringEvent(TestActiveDirectoryConnectivityTask.ScenarioId.SearchOverLatency, EventTypeEnumeration.Error, base.GenerateErrorMessage(TestActiveDirectoryConnectivityTask.ScenarioId.SearchLatency, 0, string.Format("Over Threshold. Threshold :{0} Actual {1}", base.Context.Instance.SearchLatencyThresholdInMilliseconds, timeSpan.TotalMilliseconds), null));
				}
				else
				{
					base.AddMonitoringEvent(TestActiveDirectoryConnectivityTask.ScenarioId.SearchLatency, EventTypeEnumeration.Success, base.GenerateErrorMessage(TestActiveDirectoryConnectivityTask.ScenarioId.SearchLatency, 0, string.Empty, null));
					activeDirectoryConnectivityOutcome.Update(CasTransactionResultEnum.Success);
				}
			}
			catch (Exception ex)
			{
				ActiveDirectorySearchError errorCode;
				if (ex is ADTransientException)
				{
					errorCode = ActiveDirectorySearchError.ADTransientException;
				}
				else
				{
					errorCode = ActiveDirectorySearchError.OtherException;
				}
				TimeSpan timeSpan = ExDateTime.Now - activeDirectoryConnectivityOutcome.StartTime;
				activeDirectoryConnectivityOutcome.Update(CasTransactionResultEnum.Failure);
				base.AddMonitoringEvent(TestActiveDirectoryConnectivityTask.ScenarioId.SearchFailed, EventTypeEnumeration.Error, base.GenerateErrorMessage(TestActiveDirectoryConnectivityTask.ScenarioId.Search, (int)errorCode, ex.ToString(), null));
			}
			return activeDirectoryConnectivityOutcome;
		}
	}
}
