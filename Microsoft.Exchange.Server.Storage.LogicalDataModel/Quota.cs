using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public static class Quota
	{
		internal static IDisposable SetFolderCheckHook(Quota.FolderCheckDelegate hook)
		{
			return Quota.hookableFolderCheck.SetTestHook(hook);
		}

		internal static IDisposable SetMailboxCheckHook(Quota.MailboxCheckDelegate hook)
		{
			return Quota.hookableMailboxCheck.SetTestHook(hook);
		}

		public static void Enforce(LID lid, Context context, Folder parentFolder, QuotaType quotaType, bool forReportDelivery)
		{
			QuotaInfo quotaInfo;
			long num;
			ErrorCode errorCode = Quota.hookableFolderCheck.Value(context, parentFolder, quotaType, forReportDelivery, out quotaInfo, out num);
			if (errorCode != ErrorCode.NoError)
			{
				DiagnosticContext.TraceDword(lid, (uint)quotaType);
				throw new StoreException((LID)29884U, errorCode);
			}
		}

		public static void Enforce(LID lid, Context context, Mailbox mailbox, QuotaType quotaType, bool forReportDelivery)
		{
			QuotaInfo quotaInfo;
			long num;
			ErrorCode errorCode = Quota.hookableMailboxCheck.Value(context, mailbox, quotaType, forReportDelivery, out quotaInfo, out num);
			if (errorCode != ErrorCode.NoError)
			{
				DiagnosticContext.TraceDword(lid, (uint)quotaType);
				throw new StoreException((LID)29888U, errorCode);
			}
		}

		public static ErrorCode CheckForOverQuota(Context context, Folder parentFolder, QuotaType quotaType, bool forReportDelivery, out QuotaInfo quotaInfo, out long valueSize)
		{
			string moniker = string.Format("{0}-{1}", parentFolder.Mailbox.MailboxGuid, parentFolder.GetId(context));
			long valueSize2;
			switch (quotaType)
			{
			case QuotaType.StorageWarningLimit:
			case QuotaType.StorageOverQuotaLimit:
			case QuotaType.StorageShutoff:
			case QuotaType.DumpsterWarningLimit:
			case QuotaType.DumpsterShutoff:
				quotaInfo = parentFolder.GetQuotaInfo(context);
				valueSize = parentFolder.GetMessageSize(context) + parentFolder.GetHiddenItemSize(context);
				valueSize2 = valueSize;
				break;
			case QuotaType.MailboxMessagesPerFolderCountWarningQuota:
			case QuotaType.MailboxMessagesPerFolderCountReceiveQuota:
			case QuotaType.DumpsterMessagesPerFolderCountWarningQuota:
			case QuotaType.DumpsterMessagesPerFolderCountReceiveQuota:
				quotaInfo = parentFolder.Mailbox.QuotaInfo;
				valueSize = parentFolder.GetMessageCount(context) + parentFolder.GetHiddenItemCount(context);
				valueSize2 = valueSize + 1L;
				break;
			case QuotaType.FolderHierarchyChildrenCountWarningQuota:
			case QuotaType.FolderHierarchyChildrenCountReceiveQuota:
				quotaInfo = parentFolder.Mailbox.QuotaInfo;
				valueSize = parentFolder.GetFolderCount(context);
				valueSize2 = valueSize + 1L;
				break;
			case QuotaType.FolderHierarchyDepthWarningQuota:
			case QuotaType.FolderHierarchyDepthReceiveQuota:
			{
				quotaInfo = parentFolder.Mailbox.QuotaInfo;
				FolderHierarchy folderHierarchy = FolderHierarchy.GetFolderHierarchy(context, parentFolder.Mailbox, ExchangeShortId.Zero, FolderInformationType.Basic);
				valueSize = folderHierarchy.GetFolderHierarchyDepth(parentFolder.GetId(context).ToExchangeShortId());
				valueSize2 = valueSize + 1L;
				break;
			}
			default:
				quotaInfo = QuotaInfo.Unlimited;
				valueSize = 0L;
				return ErrorCode.CreateInvalidParameter((LID)33600U);
			}
			return Quota.Check(context, moniker, quotaInfo, quotaType, valueSize2, forReportDelivery).Propagate((LID)29876U);
		}

		public static ErrorCode CheckForOverQuota(Context context, Mailbox mailbox, QuotaType quotaType, bool forReportDelivery, out QuotaInfo quotaInfo, out long valueSize)
		{
			quotaInfo = mailbox.QuotaInfo;
			string moniker = mailbox.MailboxGuid.ToString();
			switch (quotaType)
			{
			case QuotaType.StorageWarningLimit:
			case QuotaType.StorageOverQuotaLimit:
			case QuotaType.StorageShutoff:
				valueSize = mailbox.GetMessageSize(context) + mailbox.GetHiddenMessageSize(context);
				break;
			case QuotaType.DumpsterWarningLimit:
			case QuotaType.DumpsterShutoff:
				valueSize = mailbox.GetDeletedMessageSize(context) + mailbox.GetHiddenDeletedMessageSize(context);
				break;
			default:
				switch (quotaType)
				{
				case QuotaType.FoldersCountWarningQuota:
				case QuotaType.FoldersCountReceiveQuota:
				{
					FolderHierarchy folderHierarchy = FolderHierarchy.GetFolderHierarchy(context, mailbox, ExchangeShortId.Zero, FolderInformationType.Basic);
					valueSize = (long)(folderHierarchy.TotalFolderCount + 1);
					break;
				}
				default:
					quotaInfo = QuotaInfo.Unlimited;
					valueSize = 0L;
					return ErrorCode.CreateInvalidParameter((LID)49984U);
				}
				break;
			}
			return Quota.Check(context, moniker, quotaInfo, quotaType, valueSize, forReportDelivery).Propagate((LID)29880U);
		}

		private static ErrorCode Check(Context context, string moniker, QuotaInfo quotaInfo, QuotaType quotaType, long valueSize, bool forReportDelivery)
		{
			long num;
			Unlimited<long> unlimited;
			ErrorCodeValue errorCodeValue;
			switch (quotaType)
			{
			default:
				num = 52428800L;
				unlimited = quotaInfo.MailboxWarningQuota;
				errorCodeValue = ErrorCodeValue.QuotaExceeded;
				break;
			case QuotaType.StorageOverQuotaLimit:
				num = 52428800L;
				unlimited = quotaInfo.MailboxSendQuota;
				errorCodeValue = ErrorCodeValue.QuotaExceeded;
				break;
			case QuotaType.StorageShutoff:
				num = 52428800L;
				unlimited = quotaInfo.MailboxShutoffQuota;
				errorCodeValue = ErrorCodeValue.ShutoffQuotaExceeded;
				break;
			case QuotaType.DumpsterWarningLimit:
				num = 52428800L;
				unlimited = quotaInfo.DumpsterWarningQuota;
				errorCodeValue = ErrorCodeValue.QuotaExceeded;
				break;
			case QuotaType.DumpsterShutoff:
				num = 52428800L;
				unlimited = quotaInfo.DumpsterShutoffQuota;
				errorCodeValue = ErrorCodeValue.ShutoffQuotaExceeded;
				break;
			case QuotaType.MailboxMessagesPerFolderCountWarningQuota:
				num = 1024L;
				unlimited = quotaInfo.MailboxMessagesPerFolderCountWarningQuota;
				errorCodeValue = ErrorCodeValue.QuotaExceeded;
				break;
			case QuotaType.MailboxMessagesPerFolderCountReceiveQuota:
				num = 1024L;
				unlimited = quotaInfo.MailboxMessagesPerFolderCountReceiveQuota;
				errorCodeValue = ErrorCodeValue.MessagePerFolderCountReceiveQuotaExceeded;
				break;
			case QuotaType.DumpsterMessagesPerFolderCountWarningQuota:
				num = 1024L;
				unlimited = quotaInfo.DumpsterMessagesPerFolderCountWarningQuota;
				errorCodeValue = ErrorCodeValue.QuotaExceeded;
				break;
			case QuotaType.DumpsterMessagesPerFolderCountReceiveQuota:
				num = 1024L;
				unlimited = quotaInfo.DumpsterMessagesPerFolderCountReceiveQuota;
				errorCodeValue = ErrorCodeValue.MessagePerFolderCountReceiveQuotaExceeded;
				break;
			case QuotaType.FolderHierarchyChildrenCountWarningQuota:
				num = 0L;
				unlimited = quotaInfo.FolderHierarchyChildrenCountWarningQuota;
				errorCodeValue = ErrorCodeValue.QuotaExceeded;
				break;
			case QuotaType.FolderHierarchyChildrenCountReceiveQuota:
				num = 0L;
				unlimited = quotaInfo.FolderHierarchyChildrenCountReceiveQuota;
				errorCodeValue = ErrorCodeValue.FolderHierarchyChildrenCountReceiveQuotaExceeded;
				break;
			case QuotaType.FolderHierarchyDepthWarningQuota:
				num = 0L;
				unlimited = quotaInfo.FolderHierarchyDepthWarningQuota;
				errorCodeValue = ErrorCodeValue.QuotaExceeded;
				break;
			case QuotaType.FolderHierarchyDepthReceiveQuota:
				num = 0L;
				unlimited = quotaInfo.FolderHierarchyDepthReceiveQuota;
				errorCodeValue = ErrorCodeValue.FolderHierarchyDepthReceiveQuotaExceeded;
				break;
			case QuotaType.FoldersCountWarningQuota:
				num = 0L;
				unlimited = quotaInfo.FoldersCountWarningQuota;
				errorCodeValue = ErrorCodeValue.FolderHierarchySizeReceiveQuotaExceeded;
				break;
			case QuotaType.FoldersCountReceiveQuota:
				num = 0L;
				unlimited = quotaInfo.FoldersCountReceiveQuota;
				errorCodeValue = ErrorCodeValue.FolderHierarchySizeReceiveQuotaExceeded;
				break;
			}
			if (forReportDelivery && !unlimited.IsUnlimited)
			{
				long num2 = unlimited.Value * 110L / 100L;
				if (num2 - unlimited.Value < num)
				{
					num2 = unlimited.Value + num;
				}
				unlimited = new Unlimited<long>(num2);
			}
			if (ExTraceGlobals.QuotaTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.QuotaTracer.TraceDebug(29976L, "Quota.Check: container={0}, container size={1}, container quota={2}, quotaType={3}", new object[]
				{
					moniker,
					valueSize,
					unlimited,
					quotaType
				});
			}
			if (!unlimited.IsUnlimited && valueSize > unlimited.Value)
			{
				DiagnosticContext.TraceDwordAndString((LID)29818U, forReportDelivery ? 1U : 0U, moniker);
				DiagnosticContext.TraceDword((LID)29920U, (uint)quotaType);
				DiagnosticContext.TraceLong((LID)29828U, (ulong)valueSize);
				DiagnosticContext.TraceLong((LID)29832U, (ulong)unlimited.Value);
				return ErrorCode.CreateWithLid((LID)45884U, errorCodeValue);
			}
			return ErrorCode.NoError;
		}

		private static Hookable<Quota.FolderCheckDelegate> hookableFolderCheck = Hookable<Quota.FolderCheckDelegate>.Create(true, new Quota.FolderCheckDelegate(Quota.CheckForOverQuota));

		private static Hookable<Quota.MailboxCheckDelegate> hookableMailboxCheck = Hookable<Quota.MailboxCheckDelegate>.Create(true, new Quota.MailboxCheckDelegate(Quota.CheckForOverQuota));

		public delegate ErrorCode FolderCheckDelegate(Context context, Folder parentFolder, QuotaType quotaType, bool forReportDelivery, out QuotaInfo quotaInfo, out long currentContainerSize);

		public delegate ErrorCode MailboxCheckDelegate(Context context, Mailbox mailbox, QuotaType quotaType, bool forReportDelivery, out QuotaInfo quotaInfo, out long currentContainerSize);
	}
}
