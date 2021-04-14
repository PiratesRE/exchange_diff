using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.CompliancePolicy.ComplianceData;
using Microsoft.Office.CompliancePolicy.ComplianceTask;

namespace Microsoft.Office.CompliancePolicy.Dar
{
	public class DarIterator
	{
		public DarIterator(ComplianceServiceProvider complianceServiceProvider, string tenantId, string scope)
		{
			if (scope == null)
			{
				throw new ArgumentNullException("scope");
			}
			if (complianceServiceProvider == null)
			{
				throw new ArgumentNullException("complianceServiceProvider");
			}
			this.scope = scope;
			this.complianceServiceProvider = complianceServiceProvider;
			this.tenantId = tenantId;
			this.reader = complianceServiceProvider.GetPagedReader(complianceServiceProvider.GetComplianceItemContainer(tenantId, scope));
		}

		public static DarIterator Get(string scope, string tenantId, ComplianceServiceProvider complianceServiceProvider)
		{
			return DarIterator.Iterators.GetOrAdd(scope, (string scopeKey) => new DarIterator(complianceServiceProvider, tenantId, scopeKey));
		}

		public DarTaskExecutionResult RunOnNextPage(DarIterationTask task, DarTaskManager darTaskManager)
		{
			if (task.ExecutedPage == this.currentPage)
			{
				try
				{
					this.lastPage = this.ReadNextPage();
					goto IL_77;
				}
				catch (Exception ex)
				{
					task.FailReason = "Page read error";
					this.LogError(darTaskManager.ExecutionLog, task, ex);
					return DarTaskExecutionResult.Failed;
				}
			}
			if (task.ExecutedPage != 0 && task.ExecutedPage != this.prevPage)
			{
				task.FailReason = "Iteration gap error";
				this.LogError(darTaskManager.ExecutionLog, task, new InvalidOperationException("Error during scheduling of iteration task"));
				return DarTaskExecutionResult.Failed;
			}
			IL_77:
			if (task.StartPage == 0)
			{
				task.StartPage = this.currentPage;
			}
			else if (task.StartPage == this.currentPage)
			{
				return DarTaskExecutionResult.Completed;
			}
			foreach (ComplianceItem complianceItem in this.lastPage)
			{
				try
				{
					task.ProcessCurrent(complianceItem);
				}
				catch (Exception ex2)
				{
					task.FailReason = "Item processing exception";
					this.LogError(darTaskManager.ExecutionLog, task, ex2);
					return DarTaskExecutionResult.Failed;
				}
			}
			task.ExecutedPage = this.currentPage;
			return DarTaskExecutionResult.Yielded;
		}

		private void LogError(ExecutionLog executionLog, DarIterationTask task, Exception ex)
		{
			executionLog.LogError("DarIterator", null, task.CorrelationId, ex, task.FailReason, new KeyValuePair<string, object>[0]);
		}

		private IEnumerable<ComplianceItem> ReadNextPage()
		{
			this.prevPage = this.currentPage;
			IEnumerable<ComplianceItem> nextPage = this.reader.GetNextPage();
			if (nextPage == null || !nextPage.Any<ComplianceItem>())
			{
				this.reader = this.complianceServiceProvider.GetPagedReader(this.complianceServiceProvider.GetComplianceItemContainer(this.tenantId, this.scope));
				nextPage = this.reader.GetNextPage();
				this.currentPage = 1;
			}
			else
			{
				this.currentPage++;
			}
			return nextPage;
		}

		private static readonly ConcurrentDictionary<string, DarIterator> Iterators = new ConcurrentDictionary<string, DarIterator>();

		private readonly string scope;

		private readonly string tenantId;

		private ComplianceItemPagedReader reader;

		private IEnumerable<ComplianceItem> lastPage;

		private ComplianceServiceProvider complianceServiceProvider;

		private int currentPage;

		private int prevPage;
	}
}
