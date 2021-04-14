using System;
using System.Web;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Diagnostics
{
	internal static class BudgetMetadataPublisher
	{
		public static void PublishMetadata()
		{
			if (!BudgetMetadataPublisher.isBudgetMetadataRegistered)
			{
				ActivityContext.RegisterMetadata(typeof(BudgetMetadata));
				BudgetMetadataPublisher.isBudgetMetadataRegistered = true;
			}
			HttpContext httpContext = HttpContext.Current;
			if (httpContext == null)
			{
				return;
			}
			IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
			if (currentActivityScope == null)
			{
				return;
			}
			string contextItem = httpContext.GetContextItem("StartBudget");
			if (contextItem != null)
			{
				string[] budgetElements = contextItem.Split(new char[]
				{
					','
				});
				currentActivityScope.SetProperty(BudgetMetadata.BeginBudgetConnections, BudgetMetadataPublisher.GetBudgetSnapshotValueAtIndex(budgetElements, 1));
				currentActivityScope.SetProperty(BudgetMetadata.MaxConn, BudgetMetadataPublisher.GetBudgetSnapshotValueAtIndex(budgetElements, 2));
				currentActivityScope.SetProperty(BudgetMetadata.MaxBurst, BudgetMetadataPublisher.GetBudgetSnapshotValueAtIndex(budgetElements, 3));
				currentActivityScope.SetProperty(BudgetMetadata.BeginBalance, BudgetMetadataPublisher.GetBudgetSnapshotValueAtIndex(budgetElements, 4));
				currentActivityScope.SetProperty(BudgetMetadata.Cutoff, BudgetMetadataPublisher.GetBudgetSnapshotValueAtIndex(budgetElements, 5));
				currentActivityScope.SetProperty(BudgetMetadata.RechargeRate, BudgetMetadataPublisher.GetBudgetSnapshotValueAtIndex(budgetElements, 6));
				currentActivityScope.SetProperty(BudgetMetadata.ThrottlingPolicy, BudgetMetadataPublisher.GetBudgetSnapshotValueAtIndex(budgetElements, 7));
				currentActivityScope.SetProperty(BudgetMetadata.IsServiceAct, BudgetMetadataPublisher.GetBudgetSnapshotValueAtIndex(budgetElements, 8));
				currentActivityScope.SetProperty(BudgetMetadata.LiveTime, BudgetMetadataPublisher.GetBudgetSnapshotValueAtIndex(budgetElements, 9));
				currentActivityScope.SetProperty(BudgetMetadata.BeginBudgetHangingConnections, BudgetMetadataPublisher.GetBudgetSnapshotValueAtIndex(budgetElements, 10));
				currentActivityScope.SetProperty(BudgetMetadata.BeginBudgetSubscriptions, BudgetMetadataPublisher.GetBugdetSnapshotSubscriptions(contextItem));
			}
			currentActivityScope.SetProperty(BudgetMetadata.TotalDCRequestCount, httpContext.GetContextItem("TotalLdapRequestCount"));
			currentActivityScope.SetProperty(BudgetMetadata.TotalDCRequestLatency, httpContext.GetContextItem("TotalLdapRequestLatency"));
			currentActivityScope.SetProperty(BudgetMetadata.TotalMBXRequestCount, httpContext.GetContextItem("TotalRpcRequestCount"));
			currentActivityScope.SetProperty(BudgetMetadata.TotalMBXRequestLatency, httpContext.GetContextItem("TotalRpcRequestLatency"));
			string contextItem2 = httpContext.GetContextItem("EndBudget");
			if (contextItem2 != null)
			{
				string[] budgetElements2 = contextItem2.Split(new char[]
				{
					','
				});
				currentActivityScope.SetProperty(BudgetMetadata.EndBudgetConnections, BudgetMetadataPublisher.GetBudgetSnapshotValueAtIndex(budgetElements2, 1));
				currentActivityScope.SetProperty(BudgetMetadata.EndBalance, BudgetMetadataPublisher.GetBudgetSnapshotValueAtIndex(budgetElements2, 4));
				currentActivityScope.SetProperty(BudgetMetadata.EndBudgetHangingConnections, BudgetMetadataPublisher.GetBudgetSnapshotValueAtIndex(budgetElements2, 10));
				string bugdetSnapshotResources = BudgetMetadataPublisher.GetBugdetSnapshotResources(contextItem2);
				BudgetMetadataPublisher.SetBudgetSnapshotResourceInfo(bugdetSnapshotResources, currentActivityScope);
				currentActivityScope.SetProperty(BudgetMetadata.EndBudgetSubscriptions, BudgetMetadataPublisher.GetBugdetSnapshotSubscriptions(contextItem2));
			}
		}

		private static string GetBudgetSnapshotValueAtIndex(string[] budgetElements, int index)
		{
			if (budgetElements != null && budgetElements.Length > index && budgetElements[index].Contains(":"))
			{
				int num = budgetElements[index].IndexOf(':');
				return budgetElements[index].Substring(Math.Min(budgetElements[index].Length, num + 1));
			}
			return null;
		}

		private static string GetBugdetSnapshotResources(string budget)
		{
			if (!string.IsNullOrEmpty(budget) && budget.Contains("Norm[Resources:"))
			{
				return budget.Split(new string[]
				{
					"Norm[Resources:"
				}, StringSplitOptions.RemoveEmptyEntries)[1].Split(new char[]
				{
					']'
				})[0];
			}
			return null;
		}

		private static string GetBugdetSnapshotSubscriptions(string budget)
		{
			if (!string.IsNullOrEmpty(budget) && budget.Contains("Sub:"))
			{
				return budget.Split(new string[]
				{
					"Sub:"
				}, StringSplitOptions.RemoveEmptyEntries)[1].Split(new char[]
				{
					';'
				})[0];
			}
			return null;
		}

		private static void SetBudgetSnapshotResourceInfo(string budgetResourcesString, IActivityScope activityScope)
		{
			if (!string.IsNullOrEmpty(budgetResourcesString))
			{
				foreach (string text in budgetResourcesString.Split(new string[]
				{
					"),"
				}, StringSplitOptions.RemoveEmptyEntries))
				{
					string[] array2 = text.Split(new char[]
					{
						'('
					});
					if (array2.Length > 1)
					{
						string[] array3 = array2[1].Split(new char[]
						{
							')'
						});
						string text2 = array3[0].ToLower();
						string value = array3[1];
						string[] array4 = array2[2].Split(new char[]
						{
							','
						});
						string value2 = array4[0].Split(new char[]
						{
							':'
						})[1];
						string value3 = array4[1].Split(new char[]
						{
							':'
						})[1];
						string a;
						if ((a = text2) != null && a == "mdb")
						{
							activityScope.SetProperty(BudgetMetadata.MDBResource, value);
							activityScope.SetProperty(BudgetMetadata.MDBHealth, value2);
							activityScope.SetProperty(BudgetMetadata.MDBHistoricalLoad, value3);
						}
					}
				}
			}
		}

		private static bool isBudgetMetadataRegistered;
	}
}
