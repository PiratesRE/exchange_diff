using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal sealed class FolderContentsCrawler : DisposableWrapper<ISourceFolder>
	{
		internal FolderContentsCrawler(ISourceFolder sourceFolder, int pageSize, int maxPageSize) : base(sourceFolder, true)
		{
			ArgumentValidator.ThrowIfZeroOrNegative("pageSize", pageSize);
			ArgumentValidator.ThrowIfZeroOrNegative("maxPageSize", maxPageSize);
			this.pageSize = pageSize;
			this.maxPageSize = maxPageSize;
			this.pageQueue = new Queue<List<MessageRec>>(maxPageSize / pageSize + 1);
			this.HasMoreMessages = true;
		}

		public bool HasMoreMessages { get; private set; }

		public int TotalMessageCount { get; private set; }

		internal MailboxCopierBase MailboxCopier { get; set; }

		internal IReadOnlyCollection<MessageRec> GetMessagesNextPage()
		{
			MrsTracer.Service.Function("FolderContentsCrawler.GetMessagesNextPage", new object[0]);
			base.CheckDisposed();
			bool flag = false;
			while (this.pageQueue.Count <= 0)
			{
				List<MessageRec> list = base.WrappedObject.EnumerateMessagesPaged(this.maxPageSize);
				if (this.MailboxCopier != null)
				{
					this.MailboxCopier.ICSSyncState.ProviderState = this.MailboxCopier.SourceMailbox.GetMailboxSyncState();
					this.MailboxCopier.SaveICSSyncState(true);
				}
				if (list == null)
				{
					MrsTracer.Service.Debug("No more messages", new object[0]);
				}
				else
				{
					if (this.TotalMessageCount == 0)
					{
						this.TotalMessageCount = base.WrappedObject.GetEstimatedItemCount();
					}
					MrsTracer.Service.Debug("Prepare {0} messages into {1} messages/page", new object[]
					{
						list.Count,
						this.pageSize
					});
					using (IEnumerator<MessageRec> enumerator = list.GetEnumerator())
					{
						bool flag2 = false;
						while (!flag2)
						{
							List<MessageRec> list2 = new List<MessageRec>(this.pageSize);
							for (int i = 0; i < this.pageSize; i++)
							{
								if (!enumerator.MoveNext())
								{
									flag2 = true;
									break;
								}
								list2.Add(enumerator.Current);
							}
							if (list2.Count > 0)
							{
								this.pageQueue.Enqueue(list2);
							}
						}
					}
					flag = !flag;
					if (flag)
					{
						continue;
					}
				}
				this.HasMoreMessages = false;
				return Array<MessageRec>.Empty;
			}
			List<MessageRec> list3 = this.pageQueue.Dequeue();
			MrsTracer.Service.Debug("Return {0} messages to copy", new object[]
			{
				list3.Count
			});
			return list3;
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.pageQueue != null)
			{
				this.pageQueue.Clear();
				this.pageQueue = null;
			}
			base.InternalDispose(calledFromDispose);
		}

		private readonly int pageSize;

		private readonly int maxPageSize;

		private Queue<List<MessageRec>> pageQueue;
	}
}
