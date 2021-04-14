using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal sealed class MailboxContentsCrawler : DisposeTrackableBase
	{
		internal MailboxContentsCrawler(MailboxCopierBase mailboxCopier, IReadOnlyCollection<FolderMapping> foldersToCopy)
		{
			ArgumentValidator.ThrowIfNull("mailboxCopier", mailboxCopier);
			this.mailboxCopier = mailboxCopier;
			this.Initialize(mailboxCopier.SourceMailbox, foldersToCopy, ConfigBase<MRSConfigSchema>.GetConfig<int>("MaxFolderOpened"), ConfigBase<MRSConfigSchema>.GetConfig<int>("CrawlerPageSize"), ConfigBase<MRSConfigSchema>.GetConfig<int>("EnumerateMessagesPageSize"));
		}

		internal MailboxContentsCrawler(ISourceMailbox sourceMalibox, IReadOnlyCollection<FolderMapping> foldersToCopy, int maxFoldersOpened, int pageSize, int maxPageSize)
		{
			this.Initialize(sourceMalibox, foldersToCopy, maxFoldersOpened, pageSize, maxPageSize);
		}

		internal FolderMapping GetNextFolderToCopy(out FolderContentsCrawler folderContentsCrawler, out bool shouldCopyProperties)
		{
			MrsTracer.Service.Function("MailboxContentsCrawler.GetNextFolderToCopy", new object[0]);
			base.CheckDisposed();
			folderContentsCrawler = null;
			shouldCopyProperties = false;
			for (int i = 0; i <= this.folderCount; i++)
			{
				if (!this.folderEnumerator.MoveNext())
				{
					MrsTracer.Service.Debug("Tail is reached, move to head", new object[0]);
					this.folderEnumerator.Reset();
					if (!this.folderEnumerator.MoveNext())
					{
						break;
					}
				}
				FolderMapping folderMapping = this.folderEnumerator.Current;
				if (folderMapping.FolderType != FolderType.Search && folderMapping.IsIncluded && !this.crawledFolders.ContainsKey(folderMapping.EntryId))
				{
					FolderContentsCrawler folderContentsCrawler2;
					if (!this.crawlers.TryGetValue(folderMapping.EntryId, out folderContentsCrawler2) && this.crawlers.Count < this.maxFoldersOpened)
					{
						MrsTracer.Service.Debug("Add crawler for the folder: '{0}'", new object[]
						{
							folderMapping.FullFolderName
						});
						ISourceFolder folder = this.sourceMalibox.GetFolder(folderMapping.EntryId);
						if (folder != null)
						{
							folderContentsCrawler2 = new FolderContentsCrawler(folder, this.pageSize, this.maxPageSize)
							{
								MailboxCopier = this.mailboxCopier
							};
							using (DisposeGuard disposeGuard = folderContentsCrawler2.Guard())
							{
								this.crawlers.Add(folderMapping.EntryId, folderContentsCrawler2);
								disposeGuard.Success();
								shouldCopyProperties = true;
							}
						}
					}
					if (folderContentsCrawler2 != null)
					{
						if (folderContentsCrawler2.HasMoreMessages)
						{
							MrsTracer.Service.Debug("Return the folder to copy: '{0}'", new object[]
							{
								folderMapping.FullFolderName
							});
							folderContentsCrawler = folderContentsCrawler2;
							return folderMapping;
						}
						MrsTracer.Service.Debug("The folder has completed crawling: '{0}'", new object[]
						{
							folderMapping.FullFolderName
						});
						this.crawledFolders.Add(folderMapping.EntryId, folderMapping);
						folderContentsCrawler2.Dispose();
						this.crawlers.Remove(folderMapping.EntryId);
					}
				}
			}
			MrsTracer.Service.Debug("No more folders", new object[0]);
			return null;
		}

		internal void ResetFolder(FolderMapping folder)
		{
			MrsTracer.Service.Function("MailboxContentsCrawler.ResetFolder: '{0}'", new object[]
			{
				folder
			});
			base.CheckDisposed();
			byte[] entryId = folder.EntryId;
			this.crawledFolders.Remove(entryId);
			FolderContentsCrawler folderContentsCrawler;
			if (this.crawlers.TryGetValue(entryId, out folderContentsCrawler))
			{
				folderContentsCrawler.Dispose();
				this.crawlers.Remove(entryId);
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MailboxContentsCrawler>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.folderEnumerator != null)
				{
					this.folderEnumerator.Dispose();
					this.folderEnumerator = null;
				}
				if (this.crawlers != null)
				{
					foreach (FolderContentsCrawler folderContentsCrawler in this.crawlers.Values)
					{
						if (folderContentsCrawler != null)
						{
							folderContentsCrawler.Dispose();
						}
					}
					this.crawlers.Clear();
					this.crawlers = null;
				}
				if (this.crawledFolders != null)
				{
					this.crawledFolders.Clear();
					this.crawledFolders = null;
				}
			}
		}

		private void Initialize(ISourceMailbox sourceMalibox, IReadOnlyCollection<FolderMapping> foldersToCopy, int maxFoldersOpened, int pageSize, int maxPageSize)
		{
			ArgumentValidator.ThrowIfNull("sourceMalibox", sourceMalibox);
			ArgumentValidator.ThrowIfNull("foldersToCopy", foldersToCopy);
			ArgumentValidator.ThrowIfZeroOrNegative("maxFolderOpened", maxFoldersOpened);
			ArgumentValidator.ThrowIfZeroOrNegative("pageSize", pageSize);
			ArgumentValidator.ThrowIfZeroOrNegative("maxPageSize", maxPageSize);
			this.sourceMalibox = sourceMalibox;
			this.pageSize = pageSize;
			this.maxPageSize = maxPageSize;
			this.folderCount = foldersToCopy.Count;
			this.maxFoldersOpened = maxFoldersOpened;
			this.folderEnumerator = foldersToCopy.GetEnumerator();
			this.crawlers = new EntryIdMap<FolderContentsCrawler>(Math.Min(this.folderCount, this.maxFoldersOpened));
			this.crawledFolders = new EntryIdMap<FolderMapping>(this.folderCount);
		}

		private readonly MailboxCopierBase mailboxCopier;

		private ISourceMailbox sourceMalibox;

		private int pageSize;

		private int maxPageSize;

		private int maxFoldersOpened;

		private int folderCount;

		private IEnumerator<FolderMapping> folderEnumerator;

		private EntryIdMap<FolderContentsCrawler> crawlers;

		private EntryIdMap<FolderMapping> crawledFolders;
	}
}
