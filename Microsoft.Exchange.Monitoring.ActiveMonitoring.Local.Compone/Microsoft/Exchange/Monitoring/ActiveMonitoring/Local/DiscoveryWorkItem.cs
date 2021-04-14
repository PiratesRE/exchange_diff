using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local
{
	public abstract class DiscoveryWorkItem : MaintenanceWorkItem
	{
		protected abstract Trace Trace { get; }

		protected static void AddTestsForEachResource<TResource>(Func<IEnumerable<TResource>> resourceAccessor, params Action<IEnumerable<TResource>>[] testCreators)
		{
			Task<TResource[]> resourceTask = Task.Factory.StartNew<TResource[]>(() => resourceAccessor().ToArray<TResource>(), TaskCreationOptions.AttachedToParent);
			Array.ForEach<Action<IEnumerable<TResource>>>(testCreators, delegate(Action<IEnumerable<TResource>> testCreator)
			{
				resourceTask.ContinueWith(delegate(Task<TResource[]> task)
				{
					testCreator(task.Result);
				}, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
			});
		}

		protected abstract void CreateWorkTasks(CancellationToken cancellationToken);

		protected sealed override void DoWork(CancellationToken cancellationToken)
		{
			Task.Factory.StartNew(delegate()
			{
				this.CreateWorkTasks(cancellationToken);
			}, TaskCreationOptions.AttachedToParent).ContinueWith(delegate(Task param0)
			{
				base.Result.StateAttribute1 = Strings.RcaWorkItemCreationSummaryEntry(this.workItemCreationLog.Count((DiscoveryWorkItem.WorkItemCreationLogEntry entry) => entry.Exception == null), this.workItemCreationLog.Count<DiscoveryWorkItem.WorkItemCreationLogEntry>());
				base.Result.StateAttribute2 = string.Join<LocalizedString>(Environment.NewLine, from entry in this.workItemCreationLog
				where entry.Exception != null
				select Strings.RcaWorkItemDescriptionEntry(entry.BaseIdentity.GetAlertMask(), string.Join("; ", from ex in new AggregateException(new Exception[]
				{
					entry.Exception
				}).Flatten().InnerExceptions
				select ex.Message)));
			}, TaskContinuationOptions.AttachedToParent);
		}

		protected Task CreateRelatedWorkItems<TIdentity>(TIdentity basedOnIdentity, Action<TIdentity> workDefinitionCreator) where TIdentity : WorkItemIdentity
		{
			return this.CreateRelatedWorkItems<TIdentity, object>(basedOnIdentity, null, delegate(TIdentity identity, object unused2)
			{
				workDefinitionCreator(identity);
			});
		}

		protected Task CreateRelatedWorkItems<TIdentity, TResource>(TIdentity basedOnIdentity, TResource resource, Action<TIdentity, TResource> workDefinitionCreator) where TIdentity : WorkItemIdentity
		{
			ArgumentValidator.ThrowIfNull("basedOnIdentity", basedOnIdentity);
			return Task.Factory.StartNew(delegate()
			{
				workDefinitionCreator(basedOnIdentity, resource);
			}, TaskCreationOptions.AttachedToParent).ContinueWith(delegate(Task workDefinitionTask)
			{
				this.workItemCreationLog.Add(new DiscoveryWorkItem.WorkItemCreationLogEntry
				{
					BaseIdentity = basedOnIdentity,
					Exception = workDefinitionTask.Exception
				});
				if (workDefinitionTask.IsFaulted)
				{
					WTFDiagnostics.TraceError<AggregateException>(this.Trace, this.TraceContext, "MapiMTDiscovery.TryAddProbeWorkDefinition() failed.  Skipping this probe's creation.  Error: {0}", workDefinitionTask.Exception, null, "CreateRelatedWorkItems", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\DiscoveryWorkItem.cs", 171);
				}
			}, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.ExecuteSynchronously);
		}

		protected TDefinition AddWorkDefinition<TDefinition>(TDefinition definition) where TDefinition : WorkDefinition
		{
			base.Broker.AddWorkDefinition<TDefinition>(definition, base.TraceContext);
			return definition;
		}

		private readonly ConcurrentBag<DiscoveryWorkItem.WorkItemCreationLogEntry> workItemCreationLog = new ConcurrentBag<DiscoveryWorkItem.WorkItemCreationLogEntry>();

		private struct WorkItemCreationLogEntry
		{
			public WorkItemIdentity BaseIdentity { get; set; }

			public Exception Exception { get; set; }
		}
	}
}
