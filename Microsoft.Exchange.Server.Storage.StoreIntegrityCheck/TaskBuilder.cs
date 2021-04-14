using System;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public class TaskBuilder
	{
		private TaskBuilder(TaskId taskId)
		{
			this.taskId = taskId;
		}

		public static TaskBuilder Create(TaskId taskId)
		{
			return new TaskBuilder(taskId);
		}

		public TaskBuilder TrackedBy(IJobExecutionTracker tracker)
		{
			this.tracker = tracker;
			return this;
		}

		public IIntegrityCheckTask Build()
		{
			TaskId taskId = this.taskId;
			if (taskId <= TaskId.MidsetDeleted)
			{
				switch (taskId)
				{
				case TaskId.SearchBacklinks:
					return new SearchBacklinksCheckTask(this.tracker);
				case TaskId.FolderView:
					break;
				case TaskId.AggregateCounts:
					return new AggregateCountsCheckTask(this.tracker);
				default:
					if (taskId == TaskId.MidsetDeleted)
					{
						return new MidsetDeletedCheckTask(this.tracker);
					}
					break;
				}
			}
			else
			{
				switch (taskId)
				{
				case TaskId.RuleMessageClass:
					return new RuleMessageClassCheckTask(this.tracker);
				case TaskId.RestrictionFolder:
					return new FolderTypeCheckTask(this.tracker);
				case TaskId.FolderACL:
					return new FolderAclCheckTask(this.tracker);
				case TaskId.UniqueMidIndex:
					return new UniqueMidIndexCheckTask(this.tracker);
				case TaskId.CorruptJunkRule:
					return new CorruptJunkRuleCheckTask(this.tracker);
				case TaskId.MissingSpecialFolders:
					return new MissingSpecialFoldersCheckTask(this.tracker);
				case TaskId.DropAllLazyIndexes:
					return new DropAllLazyIndexesTask(this.tracker);
				case TaskId.ImapId:
					return new ImapIdCheckTask(this.tracker);
				case TaskId.InMemoryFolderHierarchy:
					return new InMemoryFolderHierarchyCheckTask(this.tracker);
				case TaskId.DiscardFolderHierarchyCache:
					return new DiscardFolderHierarchyCacheTask(this.tracker);
				default:
					if (taskId == TaskId.ScheduledCheck)
					{
						return new ScheduledCheckTask(this.tracker);
					}
					break;
				}
			}
			return new NullIntegrityCheckTask(this.taskId, this.tracker);
		}

		private readonly TaskId taskId;

		private IJobExecutionTracker tracker;
	}
}
