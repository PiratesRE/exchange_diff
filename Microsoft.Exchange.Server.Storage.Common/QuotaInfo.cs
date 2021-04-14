using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class QuotaInfo
	{
		public UnlimitedBytes MailboxWarningQuota { get; private set; }

		public UnlimitedBytes MailboxSendQuota { get; private set; }

		public UnlimitedBytes MailboxShutoffQuota { get; private set; }

		public UnlimitedBytes DumpsterWarningQuota { get; private set; }

		public UnlimitedBytes DumpsterShutoffQuota { get; private set; }

		public UnlimitedItems MailboxMessagesPerFolderCountWarningQuota { get; private set; }

		public UnlimitedItems MailboxMessagesPerFolderCountReceiveQuota { get; private set; }

		public UnlimitedItems DumpsterMessagesPerFolderCountWarningQuota { get; private set; }

		public UnlimitedItems DumpsterMessagesPerFolderCountReceiveQuota { get; private set; }

		public UnlimitedItems FolderHierarchyChildrenCountWarningQuota { get; private set; }

		public UnlimitedItems FolderHierarchyChildrenCountReceiveQuota { get; private set; }

		public UnlimitedItems FolderHierarchyDepthWarningQuota { get; private set; }

		public UnlimitedItems FolderHierarchyDepthReceiveQuota { get; private set; }

		public UnlimitedItems FoldersCountWarningQuota { get; private set; }

		public UnlimitedItems FoldersCountReceiveQuota { get; private set; }

		public UnlimitedItems NamedPropertiesCountQuota { get; private set; }

		public QuotaInfo(UnlimitedBytes mailboxWarningQuota, UnlimitedBytes mailboxSendQuota, UnlimitedBytes mailboxShutoffQuota, UnlimitedBytes dumpsterWarningQuota, UnlimitedBytes dumpsterShutoffQuota, UnlimitedItems mailboxMessagesPerFolderCountWarningQuota, UnlimitedItems mailboxMessagesPerFolderCountReceiveQuota, UnlimitedItems dumpsterMessagesPerFolderCountWarningQuota, UnlimitedItems dumpsterMessagesPerFolderCountReceiveQuota, UnlimitedItems folderHierarchyChildCountWarningQuota, UnlimitedItems folderHierarchyChildCountReceiveQuota, UnlimitedItems folderHierarchyDepthWarningQuota, UnlimitedItems folderHierarchyDepthReceiveQuota, UnlimitedItems foldersCountWarningQuota, UnlimitedItems foldersCountReceiveQuota, UnlimitedItems namedPropertiesCountQuota)
		{
			this.MailboxWarningQuota = mailboxWarningQuota;
			this.MailboxSendQuota = mailboxSendQuota;
			this.MailboxShutoffQuota = mailboxShutoffQuota;
			this.DumpsterWarningQuota = dumpsterWarningQuota;
			this.DumpsterShutoffQuota = dumpsterShutoffQuota;
			this.MailboxMessagesPerFolderCountWarningQuota = mailboxMessagesPerFolderCountWarningQuota;
			this.MailboxMessagesPerFolderCountReceiveQuota = mailboxMessagesPerFolderCountReceiveQuota;
			this.DumpsterMessagesPerFolderCountWarningQuota = dumpsterMessagesPerFolderCountWarningQuota;
			this.DumpsterMessagesPerFolderCountReceiveQuota = dumpsterMessagesPerFolderCountReceiveQuota;
			this.FolderHierarchyChildrenCountWarningQuota = folderHierarchyChildCountWarningQuota;
			this.FolderHierarchyChildrenCountReceiveQuota = folderHierarchyChildCountReceiveQuota;
			this.FolderHierarchyDepthWarningQuota = folderHierarchyDepthWarningQuota;
			this.FolderHierarchyDepthReceiveQuota = folderHierarchyDepthReceiveQuota;
			this.FoldersCountWarningQuota = foldersCountWarningQuota;
			this.FoldersCountReceiveQuota = foldersCountReceiveQuota;
			this.NamedPropertiesCountQuota = namedPropertiesCountQuota;
		}

		public QuotaInfo(UnlimitedBytes mailboxWarningQuota, UnlimitedBytes mailboxSendQuota, UnlimitedBytes mailboxShutoffQuota, UnlimitedBytes dumpsterWarningQuota, UnlimitedBytes dumpsterShutoffQuota)
		{
			this.MailboxWarningQuota = mailboxWarningQuota;
			this.MailboxSendQuota = mailboxSendQuota;
			this.MailboxShutoffQuota = mailboxShutoffQuota;
			this.DumpsterWarningQuota = dumpsterWarningQuota;
			this.DumpsterShutoffQuota = dumpsterShutoffQuota;
			this.MailboxMessagesPerFolderCountWarningQuota = UnlimitedItems.UnlimitedValue;
			this.MailboxMessagesPerFolderCountReceiveQuota = UnlimitedItems.UnlimitedValue;
			this.DumpsterMessagesPerFolderCountWarningQuota = UnlimitedItems.UnlimitedValue;
			this.DumpsterMessagesPerFolderCountReceiveQuota = UnlimitedItems.UnlimitedValue;
			this.FolderHierarchyChildrenCountWarningQuota = UnlimitedItems.UnlimitedValue;
			this.FolderHierarchyChildrenCountReceiveQuota = UnlimitedItems.UnlimitedValue;
			this.FolderHierarchyDepthWarningQuota = UnlimitedItems.UnlimitedValue;
			this.FolderHierarchyDepthReceiveQuota = UnlimitedItems.UnlimitedValue;
			this.FoldersCountWarningQuota = UnlimitedItems.UnlimitedValue;
			this.FoldersCountReceiveQuota = UnlimitedItems.UnlimitedValue;
			this.NamedPropertiesCountQuota = UnlimitedItems.UnlimitedValue;
		}

		public QuotaInfo(UnlimitedItems mailboxMessagesPerFolderCountWarningQuota, UnlimitedItems mailboxMessagesPerFolderCountReceiveQuota, UnlimitedItems dumpsterMessagesPerFolderCountWarningQuota, UnlimitedItems dumpsterMessagesPerFolderCountReceiveQuota, UnlimitedItems folderHierarchyChildCountWarningQuota, UnlimitedItems folderHierarchyChildCountReceiveQuota, UnlimitedItems folderHierarchyDepthWarningQuota, UnlimitedItems folderHierarchyDepthReceiveQuota, UnlimitedItems foldersCountWarningQuota, UnlimitedItems foldersCountReceiveQuota, UnlimitedItems namedPropertiesCountQuota)
		{
			this.MailboxWarningQuota = UnlimitedBytes.UnlimitedValue;
			this.MailboxSendQuota = UnlimitedBytes.UnlimitedValue;
			this.MailboxShutoffQuota = UnlimitedBytes.UnlimitedValue;
			this.DumpsterWarningQuota = UnlimitedBytes.UnlimitedValue;
			this.DumpsterShutoffQuota = UnlimitedBytes.UnlimitedValue;
			this.MailboxMessagesPerFolderCountWarningQuota = mailboxMessagesPerFolderCountWarningQuota;
			this.MailboxMessagesPerFolderCountReceiveQuota = mailboxMessagesPerFolderCountReceiveQuota;
			this.DumpsterMessagesPerFolderCountWarningQuota = dumpsterMessagesPerFolderCountWarningQuota;
			this.DumpsterMessagesPerFolderCountReceiveQuota = dumpsterMessagesPerFolderCountReceiveQuota;
			this.FolderHierarchyChildrenCountWarningQuota = folderHierarchyChildCountWarningQuota;
			this.FolderHierarchyChildrenCountReceiveQuota = folderHierarchyChildCountReceiveQuota;
			this.FolderHierarchyDepthWarningQuota = folderHierarchyDepthWarningQuota;
			this.FolderHierarchyDepthReceiveQuota = folderHierarchyDepthReceiveQuota;
			this.FoldersCountWarningQuota = foldersCountWarningQuota;
			this.FoldersCountReceiveQuota = foldersCountReceiveQuota;
			this.NamedPropertiesCountQuota = namedPropertiesCountQuota;
		}

		public void MergeQuotaFromAD(QuotaInfo quotaInfo)
		{
			this.MailboxWarningQuota = quotaInfo.MailboxWarningQuota;
			this.MailboxSendQuota = quotaInfo.MailboxSendQuota;
			this.MailboxShutoffQuota = quotaInfo.MailboxShutoffQuota;
			this.DumpsterWarningQuota = quotaInfo.DumpsterWarningQuota;
			this.DumpsterShutoffQuota = quotaInfo.DumpsterShutoffQuota;
		}

		public void ResetFolderRelatedQuotaToUnlimited()
		{
			this.FolderHierarchyChildrenCountWarningQuota = UnlimitedItems.UnlimitedValue;
			this.FolderHierarchyChildrenCountReceiveQuota = UnlimitedItems.UnlimitedValue;
			this.FolderHierarchyDepthWarningQuota = UnlimitedItems.UnlimitedValue;
			this.FolderHierarchyDepthReceiveQuota = UnlimitedItems.UnlimitedValue;
			this.FoldersCountWarningQuota = UnlimitedItems.UnlimitedValue;
			this.FoldersCountReceiveQuota = UnlimitedItems.UnlimitedValue;
		}

		public static readonly QuotaInfo Unlimited = new QuotaInfo(UnlimitedBytes.UnlimitedValue, UnlimitedBytes.UnlimitedValue, UnlimitedBytes.UnlimitedValue, UnlimitedBytes.UnlimitedValue, UnlimitedBytes.UnlimitedValue, UnlimitedItems.UnlimitedValue, UnlimitedItems.UnlimitedValue, UnlimitedItems.UnlimitedValue, UnlimitedItems.UnlimitedValue, UnlimitedItems.UnlimitedValue, UnlimitedItems.UnlimitedValue, UnlimitedItems.UnlimitedValue, UnlimitedItems.UnlimitedValue, UnlimitedItems.UnlimitedValue, UnlimitedItems.UnlimitedValue, UnlimitedItems.UnlimitedValue);
	}
}
