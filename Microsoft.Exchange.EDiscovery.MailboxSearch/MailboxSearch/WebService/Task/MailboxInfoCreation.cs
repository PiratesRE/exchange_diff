using System;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Infrastructure;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model;
using Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Task
{
	internal class MailboxInfoCreation : SearchTask<SearchSource>
	{
		public MailboxInfoCreation.MailboxInfoCreationContext TaskContext
		{
			get
			{
				return (MailboxInfoCreation.MailboxInfoCreationContext)base.Context.TaskContext;
			}
		}

		public override void Process(SearchSource item)
		{
			Recorder.Trace(4L, TraceType.InfoTrace, new object[]
			{
				"MailboxInfoCreation.Process Item:",
				item,
				"SourceType:",
				item.SourceType,
				"SourceLocation:",
				item.SourceLocation,
				"SuppressDuplicates:",
				this.TaskContext.SuppressDuplicates
			});
			bool flag = false;
			if (item != null)
			{
				if (item.SourceLocation == SourceLocation.All || item.SourceLocation == SourceLocation.PrimaryOnly)
				{
					flag = true;
					item.MailboxInfo = new MailboxInfo(item.Recipient.ADEntry, MailboxType.Primary)
					{
						Folder = item.FolderSpec,
						SourceMailbox = item
					};
					if (!this.IsDuplicate(item) && (item.MailboxInfo.MdbGuid != Guid.Empty || item.MailboxInfo.IsRemoteMailbox))
					{
						base.Executor.EnqueueNext(item);
					}
					else if (item.MailboxInfo.MdbGuid == Guid.Empty)
					{
						Recorder.Trace(4L, TraceType.InfoTrace, new object[]
						{
							"MailboxInfoCreation.Process Ignoring primary mailbox:",
							item.ReferenceId,
							"Primary database is empty and mailbox is not remote"
						});
					}
					else
					{
						Recorder.Trace(4L, TraceType.WarningTrace, new object[]
						{
							"MailboxInfoCreation.Process Duplicate:",
							item.ReferenceId,
							"SourceType:",
							item.SourceType
						});
					}
				}
				if (item.SourceLocation == SourceLocation.All || item.SourceLocation == SourceLocation.ArchiveOnly)
				{
					if (flag)
					{
						item = item.Clone();
					}
					item.MailboxInfo = new MailboxInfo(item.Recipient.ADEntry, MailboxType.Archive)
					{
						Folder = item.FolderSpec,
						SourceMailbox = item
					};
					if (!this.IsDuplicate(item) && (item.MailboxInfo.ArchiveDatabase != Guid.Empty || item.MailboxInfo.IsRemoteMailbox || item.MailboxInfo.IsCloudArchive) && item.MailboxInfo.ArchiveGuid != Guid.Empty)
					{
						base.Executor.EnqueueNext(item);
					}
					else if (item.MailboxInfo.ArchiveDatabase == Guid.Empty)
					{
						Recorder.Trace(4L, TraceType.InfoTrace, new object[]
						{
							"MailboxInfoCreation.Process Ignoring archive mailbox:",
							item.ReferenceId,
							"Archive database is empty and mailbox is not remote"
						});
					}
					else
					{
						Recorder.Trace(4L, TraceType.WarningTrace, new object[]
						{
							"MailboxInfoCreation.Process Duplicate:",
							item.ReferenceId,
							"SourceType:",
							item.SourceType
						});
					}
				}
				if (this.TaskContext.SuppressDuplicates && this.TaskContext.MaximumItems < this.TaskContext.DupeCheck.Count)
				{
					Recorder.Trace(4L, TraceType.WarningTrace, new object[]
					{
						"MailboxInfoCreation.Process Failed TooManySources Count:",
						this.TaskContext.DupeCheck.Count,
						"Limit:",
						this.TaskContext.MaximumItems
					});
					throw new SearchException(KnownError.TooManyMailboxesException, new object[]
					{
						this.TaskContext.DupeCheck.Count,
						this.TaskContext.MaximumItems
					});
				}
			}
		}

		private bool IsDuplicate(SearchSource source)
		{
			if (this.TaskContext.SuppressDuplicates)
			{
				string text = string.Format("{0}{1}{2}{3}", new object[]
				{
					source.MailboxInfo.DisplayName,
					source.MailboxInfo.ExchangeGuid,
					source.MailboxInfo.IsArchive,
					source.MailboxInfo.Folder
				});
				if (this.TaskContext.DupeCheck.Contains(text))
				{
					return true;
				}
				this.TaskContext.DupeCheck.Add(text);
			}
			return false;
		}

		internal class MailboxInfoCreationContext
		{
			public bool SuppressDuplicates { get; set; }

			public int MaximumItems { get; set; }

			internal ConcurrentBag<string> DupeCheck
			{
				get
				{
					return this.dupeCheck;
				}
			}

			private ConcurrentBag<string> dupeCheck = new ConcurrentBag<string>();
		}
	}
}
