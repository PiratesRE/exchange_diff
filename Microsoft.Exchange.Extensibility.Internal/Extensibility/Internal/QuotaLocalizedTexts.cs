using System;
using Microsoft.Exchange.Core;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Extensibility.Internal
{
	internal sealed class QuotaLocalizedTexts
	{
		private QuotaLocalizedTexts(LocalizedString subject, LocalizedString topText, LocalizedString details)
		{
			this.subject = subject;
			this.topText = topText;
			this.details = details;
		}

		public LocalizedString TopText
		{
			get
			{
				return this.topText;
			}
		}

		public LocalizedString Subject
		{
			get
			{
				return this.subject;
			}
		}

		public LocalizedString Details
		{
			get
			{
				return this.details;
			}
		}

		public static QuotaLocalizedTexts GetQuotaLocalizedTexts(QuotaMessageType quotaMessageType, string folderName, string currentSize, bool isPrimaryMailbox)
		{
			switch (quotaMessageType)
			{
			case QuotaMessageType.WarningMailboxUnlimitedSize:
				if (!isPrimaryMailbox)
				{
					return new QuotaLocalizedTexts(SystemMessages.ArchiveQuotaWarningNoLimitSubject, SystemMessages.ArchiveQuotaWarningNoLimitTopText(currentSize), SystemMessages.ArchiveQuotaWarningNoLimitDetails);
				}
				return new QuotaLocalizedTexts(SystemMessages.QuotaWarningNoLimitSubject, SystemMessages.QuotaWarningNoLimitTopText(currentSize), SystemMessages.QuotaWarningNoLimitDetails);
			case QuotaMessageType.WarningPublicFolderUnlimitedSize:
				return new QuotaLocalizedTexts(SystemMessages.QuotaWarningNoLimitSubjectPF, SystemMessages.QuotaWarningNoLimitTopTextPF(folderName, currentSize), SystemMessages.QuotaWarningNoLimitDetailsPF);
			case QuotaMessageType.WarningMailbox:
				if (!isPrimaryMailbox)
				{
					return QuotaLocalizedTexts.archiveQuotaWarningTexts;
				}
				return QuotaLocalizedTexts.quotaWarningTexts;
			case QuotaMessageType.WarningPublicFolder:
				return new QuotaLocalizedTexts(SystemMessages.QuotaWarningSubjectPF, SystemMessages.QuotaWarningTopTextPF(folderName), SystemMessages.QuotaWarningDetailsPF);
			case QuotaMessageType.ProhibitSendMailbox:
				return QuotaLocalizedTexts.quotaSendTexts;
			case QuotaMessageType.ProhibitPostPublicFolder:
				return new QuotaLocalizedTexts(SystemMessages.QuotaSendSubjectPF, SystemMessages.QuotaSendTopTextPF(folderName), SystemMessages.QuotaSendDetailsPF);
			case QuotaMessageType.ProhibitSendReceiveMailBox:
				if (!isPrimaryMailbox)
				{
					return QuotaLocalizedTexts.archiveQuotaFullTexts;
				}
				return QuotaLocalizedTexts.quotaSendReceiveTexts;
			case QuotaMessageType.WarningMailboxMessagesPerFolderCount:
				return new QuotaLocalizedTexts(SystemMessages.QuotaWarningMailboxMessagesPerFolderCountSubject, SystemMessages.QuotaWarningMailboxMessagesPerFolderCountTopText(folderName, currentSize), SystemMessages.QuotaWarningMailboxMessagesPerFolderCountDetails);
			case QuotaMessageType.ProhibitReceiveMailboxMessagesPerFolderCount:
				return new QuotaLocalizedTexts(SystemMessages.QuotaProhibitReceiveMailboxMessagesPerFolderCountSubject, SystemMessages.QuotaProhibitReceiveMailboxMessagesPerFolderCountTopText(folderName, currentSize), SystemMessages.QuotaProhibitReceiveMailboxMessagesPerFolderCountDetails);
			case QuotaMessageType.WarningFolderHierarchyChildrenCount:
				return new QuotaLocalizedTexts(SystemMessages.QuotaWarningFolderHierarchyChildrenCountSubject, SystemMessages.QuotaWarningFolderHierarchyChildrenCountTopText(folderName, currentSize), SystemMessages.QuotaWarningFolderHierarchyChildrenCountDetails);
			case QuotaMessageType.ProhibitReceiveFolderHierarchyChildrenCountCount:
				return new QuotaLocalizedTexts(SystemMessages.QuotaProhibitReceiveFolderHierarchyChildrenCountSubject, SystemMessages.QuotaProhibitReceiveFolderHierarchyChildrenCountTopText(folderName, currentSize), SystemMessages.QuotaProhibitReceiveFolderHierarchyChildrenCountDetails);
			case QuotaMessageType.WarningMailboxMessagesPerFolderUnlimitedCount:
				return new QuotaLocalizedTexts(SystemMessages.QuotaWarningMailboxMessagesPerFolderNoLimitSubject, SystemMessages.QuotaWarningMailboxMessagesPerFolderNoLimitTopText(folderName, currentSize), SystemMessages.QuotaWarningMailboxMessagesPerFolderNoLimitDetails);
			case QuotaMessageType.WarningFolderHierarchyChildrenUnlimitedCount:
				return new QuotaLocalizedTexts(SystemMessages.QuotaWarningFolderHierarchyChildrenNoLimitSubject, SystemMessages.QuotaWarningFolderHierarchyChildrenNoLimitTopText(folderName, currentSize), SystemMessages.QuotaWarningFolderHierarchyChildrenNoLimitDetails);
			case QuotaMessageType.WarningFolderHierarchyDepth:
				return new QuotaLocalizedTexts(SystemMessages.QuotaWarningFolderHierarchyDepthSubject, SystemMessages.QuotaWarningFolderHierarchyDepthTopText(folderName, currentSize), SystemMessages.QuotaWarningFolderHierarchyDepthDetails);
			case QuotaMessageType.ProhibitReceiveFolderHierarchyDepth:
				return new QuotaLocalizedTexts(SystemMessages.QuotaProhibitReceiveFolderHierarchyDepthSubject, SystemMessages.QuotaProhibitReceiveFolderHierarchyDepthTopText(folderName, currentSize), SystemMessages.QuotaProhibitReceiveFolderHierarchyDepthDetails);
			case QuotaMessageType.WarningFolderHierarchyDepthUnlimited:
				return new QuotaLocalizedTexts(SystemMessages.QuotaWarningFolderHierarchyDepthNoLimitSubject, SystemMessages.QuotaWarningFolderHierarchyDepthNoLimitTopText(folderName, currentSize), SystemMessages.QuotaWarningFolderHierarchyDepthNoLimitDetails);
			case QuotaMessageType.WarningFoldersCount:
				return new QuotaLocalizedTexts(SystemMessages.QuotaWarningFoldersCountSubject, SystemMessages.QuotaWarningFoldersCountTopText(currentSize), SystemMessages.QuotaWarningFoldersCountDetails);
			case QuotaMessageType.ProhibitReceiveFoldersCount:
				return new QuotaLocalizedTexts(SystemMessages.QuotaProhibitReceiveFoldersCountSubject, SystemMessages.QuotaProhibitReceiveFoldersCountTopText(currentSize), SystemMessages.QuotaProhibitReceiveFoldersCountDetails);
			case QuotaMessageType.WarningFoldersCountUnlimited:
				return new QuotaLocalizedTexts(SystemMessages.QuotaWarningFoldersCountNoLimitSubject, SystemMessages.QuotaWarningFoldersCountNoLimitTopText(currentSize), SystemMessages.QuotaWarningFoldersCountNoLimitDetails);
			default:
				throw new NotSupportedException("quotaMessageType invalid");
			}
		}

		private static readonly QuotaLocalizedTexts quotaWarningTexts = new QuotaLocalizedTexts(SystemMessages.QuotaWarningSubject, SystemMessages.QuotaWarningTopText, SystemMessages.QuotaWarningDetails);

		private static readonly QuotaLocalizedTexts quotaSendTexts = new QuotaLocalizedTexts(SystemMessages.QuotaSendSubject, SystemMessages.QuotaSendTopText, SystemMessages.QuotaSendDetails);

		private static readonly QuotaLocalizedTexts quotaSendReceiveTexts = new QuotaLocalizedTexts(SystemMessages.QuotaSendReceiveSubject, SystemMessages.QuotaSendReceiveTopText, SystemMessages.QuotaSendReceiveDetails);

		private static readonly QuotaLocalizedTexts archiveQuotaWarningTexts = new QuotaLocalizedTexts(SystemMessages.ArchiveQuotaWarningSubject, SystemMessages.ArchiveQuotaWarningTopText, SystemMessages.ArchiveQuotaWarningDetails);

		private static readonly QuotaLocalizedTexts archiveQuotaFullTexts = new QuotaLocalizedTexts(SystemMessages.ArchiveQuotaFullSubject, SystemMessages.ArchiveQuotaFullTopText, SystemMessages.ArchiveQuotaFullDetails);

		private LocalizedString subject;

		private LocalizedString topText;

		private LocalizedString details;
	}
}
