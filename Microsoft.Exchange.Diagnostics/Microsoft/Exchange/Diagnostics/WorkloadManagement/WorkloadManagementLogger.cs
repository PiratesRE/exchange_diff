using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Diagnostics.WorkloadManagement
{
	internal class WorkloadManagementLogger
	{
		internal WorkloadManagementLogger(IWorkloadLogger log)
		{
			this.logger = log;
			ActivityContext.OnActivityEvent += this.OnActivityContextEvent;
		}

		internal static List<KeyValuePair<string, object>> FormatWlmActivity(IActivityScope activityScope, bool includeMetaData = true)
		{
			List<KeyValuePair<string, object>> list = null;
			if (activityScope != null)
			{
				list = activityScope.GetFormattableStatistics();
				if (includeMetaData)
				{
					List<KeyValuePair<string, object>> formattableMetadata = activityScope.GetFormattableMetadata();
					if (formattableMetadata != null)
					{
						foreach (KeyValuePair<string, object> item in formattableMetadata)
						{
							if (!item.Key.StartsWith("ActivityStandardMetadata"))
							{
								list.Add(item);
							}
						}
					}
				}
			}
			return list;
		}

		internal static bool SetWorkloadMetadataValues(string workloadType, string workloadClassification, bool isServiceAccount, bool isInteractive, IActivityScope activityScope = null)
		{
			WorkloadManagementLogger.RegisterMetadataIfNecessary();
			return WorkloadManagementLogger.DoIfStarted(activityScope, delegate(IActivityScope scope)
			{
				scope.SetProperty(WlmMetaData.WorkloadType, workloadType);
				scope.SetProperty(WlmMetaData.WorkloadClassification, workloadClassification);
				scope.SetProperty(WlmMetaData.IsServiceAccount, isServiceAccount.ToString());
				scope.SetProperty(WlmMetaData.IsInteractive, isInteractive.ToString());
			});
		}

		internal static bool SetBudgetType(string budgetType, IActivityScope activityScope = null)
		{
			WorkloadManagementLogger.RegisterMetadataIfNecessary();
			return WorkloadManagementLogger.DoIfStarted(activityScope, delegate(IActivityScope scope)
			{
				scope.SetProperty(WlmMetaData.BudgetType, budgetType);
			});
		}

		internal static bool SetOverBudget(string policyPart, string policyValue, IActivityScope activityScope = null)
		{
			WorkloadManagementLogger.RegisterMetadataIfNecessary();
			return WorkloadManagementLogger.DoIfStarted(activityScope, delegate(IActivityScope scope)
			{
				ActivityContext.AddOperation(scope, ActivityOperationType.OverBudget, null, 0f, 1);
				scope.SetProperty(WlmMetaData.OverBudgetPolicyPart, policyPart);
				scope.SetProperty(WlmMetaData.OverBudgetPolicyValue, policyValue);
			});
		}

		internal static bool SetQueueTime(TimeSpan queueTime, IActivityScope activityScope = null)
		{
			return WorkloadManagementLogger.DoIfStarted(activityScope, delegate(IActivityScope scope)
			{
				ActivityContext.AddOperation(scope, ActivityOperationType.QueueTime, null, (float)queueTime.TotalMilliseconds, 1);
			});
		}

		internal static bool SetThrottlingValues(TimeSpan delay, bool user, string instance, IActivityScope activityScope = null)
		{
			return WorkloadManagementLogger.DoIfStarted(activityScope, delegate(IActivityScope scope)
			{
				if (user)
				{
					ActivityContext.AddOperation(scope, ActivityOperationType.UserDelay, instance, (float)delay.TotalMilliseconds, 1);
					return;
				}
				ActivityContext.AddOperation(scope, ActivityOperationType.ResourceDelay, instance, (float)delay.TotalMilliseconds, 1);
			});
		}

		internal static bool SetBudgetUsage(TimeSpan usage, string instance, IActivityScope activityScope = null)
		{
			return WorkloadManagementLogger.DoIfStarted(activityScope, delegate(IActivityScope scope)
			{
				ActivityContext.AddOperation(scope, ActivityOperationType.BudgetUsed, instance, (float)usage.TotalMilliseconds, 1);
			});
		}

		internal static bool SetResourceBlocked(string resourceInstance, IActivityScope activityScope = null)
		{
			return WorkloadManagementLogger.DoIfStarted(activityScope, delegate(IActivityScope scope)
			{
				ActivityContext.AddOperation(scope, ActivityOperationType.ResourceBlocked, resourceInstance, 0f, 1);
			});
		}

		internal static bool SetBudgetBalance(string budgetBalance, IActivityScope activityScope = null)
		{
			WorkloadManagementLogger.RegisterMetadataIfNecessary();
			return WorkloadManagementLogger.DoIfStarted(activityScope, delegate(IActivityScope scope)
			{
				scope.SetProperty(WlmMetaData.BudgetBalance, budgetBalance);
			});
		}

		internal void OnActivityContextEvent(object sender, ActivityEventArgs args)
		{
			this.logger.LogActivityEvent((IActivityScope)sender, args.ActivityEventType);
		}

		private static void RegisterMetadataIfNecessary()
		{
			if (!WorkloadManagementLogger.wlmMetadataIsRegistered)
			{
				ActivityContext.RegisterMetadata(typeof(WlmMetaData));
				WorkloadManagementLogger.wlmMetadataIsRegistered = true;
			}
		}

		private static bool DoIfStarted(IActivityScope activityScope, Action<IActivityScope> action)
		{
			if (activityScope == null)
			{
				activityScope = ActivityContext.GetCurrentActivityScope();
			}
			if (activityScope != null && activityScope.Status == ActivityContextStatus.ActivityStarted)
			{
				action(activityScope);
				return true;
			}
			return false;
		}

		private static bool wlmMetadataIsRegistered;

		private IWorkloadLogger logger;
	}
}
