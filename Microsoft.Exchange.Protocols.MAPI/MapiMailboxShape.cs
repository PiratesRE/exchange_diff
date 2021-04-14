using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.Mapi;
using Microsoft.Exchange.Protocols.MAPI.ExtensionMethods;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public sealed class MapiMailboxShape
	{
		internal static IDisposable SetFolderCheckHook(Quota.FolderCheckDelegate hook)
		{
			return MapiMailboxShape.hookableFolderCheck.SetTestHook(hook);
		}

		internal static IDisposable SetMailboxCheckHook(Quota.MailboxCheckDelegate hook)
		{
			return MapiMailboxShape.hookableMailboxCheck.SetTestHook(hook);
		}

		internal static bool IsDumpster(QuotaType quotaType)
		{
			return (quotaType >= QuotaType.DumpsterWarningLimit && quotaType <= QuotaType.DumpsterShutoff) || (quotaType >= QuotaType.DumpsterMessagesPerFolderCountWarningQuota && quotaType <= QuotaType.DumpsterMessagesPerFolderCountReceiveQuota);
		}

		internal static void PerformLogonQuotaCheck(MapiContext context, MapiLogon mapiLogon)
		{
			MapiMailboxShape.PerformLogonQuotaCheck(context, mapiLogon, QuotaType.DumpsterWarningLimit, QuotaType.DumpsterShutoff);
			MapiMailboxShape.PerformLogonQuotaCheck(context, mapiLogon, QuotaType.StorageWarningLimit, QuotaType.StorageShutoff);
			MapiMailboxShape.PerformLogonQuotaCheck(context, mapiLogon, QuotaType.FoldersCountWarningQuota, QuotaType.FoldersCountReceiveQuota);
		}

		internal static ErrorCode PerformMailboxShapeQuotaCheck(MapiContext context, MapiLogon mapiLogon, Folder parentFolder, MapiMailboxShape.Operation operation, bool isReportMessage)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			if (mapiLogon.AllowLargeItem())
			{
				return errorCode;
			}
			bool flag = ExTraceGlobals.QuotaTracer.IsTraceEnabled(TraceType.DebugTrace);
			DateTime lastQuotaNotificationTime = parentFolder.GetLastQuotaNotificationTime(context);
			DateTime utcNow = mapiLogon.StoreMailbox.UtcNow;
			if (utcNow - lastQuotaNotificationTime > MapiMailboxShape.QuotaWarningCheckInterval)
			{
				if (flag)
				{
					ExTraceGlobals.QuotaTracer.TraceDebug<string, string>(50297L, "Folder {1} require quota check. Last check: {0}", lastQuotaNotificationTime.ToString(), parentFolder.GetParentFolderId(context).ToString());
				}
				if (operation == MapiMailboxShape.Operation.CreateMessage && mapiLogon.MapiMailbox.IsPublicFolderMailbox)
				{
					MapiMailboxShape.PerformMailboxShapeQuotaNotificationCheck(context, mapiLogon, parentFolder, QuotaType.StorageWarningLimit, QuotaType.StorageOverQuotaLimit, isReportMessage);
				}
				if (parentFolder.IsDumpsterMarkedFolder(context))
				{
					MapiMailboxShape.PerformMailboxShapeQuotaNotificationCheck(context, mapiLogon, parentFolder, QuotaType.DumpsterMessagesPerFolderCountWarningQuota, QuotaType.DumpsterMessagesPerFolderCountReceiveQuota, isReportMessage);
				}
				else
				{
					MapiMailboxShape.PerformMailboxShapeQuotaNotificationCheck(context, mapiLogon, parentFolder, QuotaType.MailboxMessagesPerFolderCountWarningQuota, QuotaType.MailboxMessagesPerFolderCountReceiveQuota, isReportMessage);
				}
				MapiMailboxShape.PerformMailboxShapeQuotaNotificationCheck(context, mapiLogon, parentFolder, QuotaType.FolderHierarchyChildrenCountWarningQuota, QuotaType.FolderHierarchyChildrenCountReceiveQuota, isReportMessage);
				MapiMailboxShape.PerformMailboxShapeQuotaNotificationCheck(context, mapiLogon, parentFolder, QuotaType.FolderHierarchyDepthWarningQuota, QuotaType.FolderHierarchyDepthReceiveQuota, isReportMessage);
				parentFolder.SetLastQuotaNotificationTime(context, utcNow);
			}
			else if (flag)
			{
				ExTraceGlobals.QuotaTracer.TraceDebug<string, string>(65433L, "Folder {1} does not require quota check. Last check: {0}", lastQuotaNotificationTime.ToString(), parentFolder.GetParentFolderId(context).ToString());
			}
			if (flag)
			{
				ExTraceGlobals.QuotaTracer.TraceDebug(32400L, "folder-scoped quota enforcement on message creation...");
			}
			if (operation == MapiMailboxShape.Operation.CreateMessage && mapiLogon.MapiMailbox.IsPublicFolderMailbox && !mapiLogon.IsMoveDestination)
			{
				errorCode = MapiMailboxShape.PerformMailboxShapeOverQuotaCheck(context, parentFolder, QuotaType.StorageOverQuotaLimit, isReportMessage);
				if (errorCode != ErrorCode.NoError)
				{
					return errorCode.Propagate((LID)62272U);
				}
			}
			if (operation == MapiMailboxShape.Operation.CreateMessage || operation == MapiMailboxShape.Operation.CopyMessage || operation == MapiMailboxShape.Operation.MoveMessage)
			{
				if (parentFolder.IsDumpsterMarkedFolder(context))
				{
					errorCode = MapiMailboxShape.PerformMailboxShapeOverQuotaCheck(context, parentFolder, QuotaType.DumpsterMessagesPerFolderCountReceiveQuota, isReportMessage);
					if (errorCode != ErrorCode.NoError)
					{
						return errorCode.Propagate((LID)44096U);
					}
				}
				else
				{
					errorCode = MapiMailboxShape.PerformMailboxShapeOverQuotaCheck(context, parentFolder, QuotaType.MailboxMessagesPerFolderCountReceiveQuota, isReportMessage);
					if (errorCode != ErrorCode.NoError)
					{
						return errorCode.Propagate((LID)37696U);
					}
				}
			}
			else if (operation == MapiMailboxShape.Operation.CreateFolder || operation == MapiMailboxShape.Operation.CopyFolder || operation == MapiMailboxShape.Operation.MoveFolder)
			{
				errorCode = MapiMailboxShape.PerformMailboxShapeOverQuotaCheck(context, parentFolder, QuotaType.FolderHierarchyChildrenCountReceiveQuota, isReportMessage);
				if (errorCode != ErrorCode.NoError)
				{
					return errorCode.Propagate((LID)54080U);
				}
				errorCode = MapiMailboxShape.PerformMailboxShapeOverQuotaCheck(context, parentFolder, QuotaType.FolderHierarchyDepthReceiveQuota, isReportMessage);
				if (errorCode != ErrorCode.NoError)
				{
					return errorCode.Propagate((LID)63580U);
				}
				if (operation == MapiMailboxShape.Operation.CreateFolder || operation == MapiMailboxShape.Operation.CopyFolder)
				{
					errorCode = MapiMailboxShape.PerformMailboxShapeOverQuotaCheck(context, parentFolder.Mailbox, QuotaType.FoldersCountReceiveQuota);
					if (errorCode != ErrorCode.NoError)
					{
						return errorCode.Propagate((LID)50940U);
					}
				}
			}
			return errorCode;
		}

		private static void PerformMailboxShapeQuotaNotificationCheck(MapiContext context, MapiLogon mapiLogon, Folder folder, QuotaType minScan, QuotaType maxScan, bool isReportMessage)
		{
			for (QuotaType quotaType = maxScan; quotaType >= minScan; quotaType--)
			{
				QuotaInfo quotaInfo;
				long containerSize;
				ErrorCode errorCode = MapiMailboxShape.hookableFolderCheck.Value(context, folder, quotaType, isReportMessage, out quotaInfo, out containerSize);
				if (errorCode != ErrorCode.NoError && !isReportMessage)
				{
					if (mapiLogon.MapiMailbox.IsPublicFolderMailbox)
					{
						List<FolderSecurity.AclTableEntry> list = null;
						try
						{
							byte[] buffer = (byte[])folder.GetPropertyValue(context, PropTag.Folder.AclTableAndSecurityDescriptor);
							FolderSecurity.AclTableAndSecurityDescriptorProperty aclTableAndSecurityDescriptorProperty = AclTableHelper.Parse(context, buffer);
							list = AclTableHelper.ParseAclTable(context, aclTableAndSecurityDescriptorProperty.SerializedAclTable);
						}
						catch (StoreException exception)
						{
							context.Diagnostics.OnExceptionCatch(exception);
						}
						if (list != null)
						{
							List<byte[]> list2 = new List<byte[]>(list.Count);
							for (int i = 0; i < list.Count; i++)
							{
								if ((list[i].Rights & FolderSecurity.ExchangeFolderRights.Contact) == FolderSecurity.ExchangeFolderRights.Contact)
								{
									list2.Add(list[i].EntryId);
								}
							}
							errorCode = MapiLogon.GenerateQuotaReport(context, mapiLogon, folder, quotaType, quotaInfo, list2, containerSize);
						}
						else
						{
							errorCode = ErrorCode.CreateNoRecipients((LID)64793U);
						}
						if (errorCode != ErrorCode.NoError)
						{
							Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_FailedToSendPublicFolderQuotaWarning, new object[]
							{
								MapiMailboxShape.GetFolderPath(context, folder),
								errorCode
							});
							return;
						}
					}
					else
					{
						errorCode = MapiLogon.GenerateQuotaReport(context, mapiLogon, folder, quotaType, quotaInfo, new byte[][]
						{
							mapiLogon.MailboxOwnerAddressInfo.UserEntryId()
						}, containerSize);
						if (errorCode != ErrorCode.NoError)
						{
							Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_FailedToSendPerFolderQuotaWarning, new object[]
							{
								mapiLogon.MailboxOwnerAddressInfo.PrimaryEmailAddress,
								MapiMailboxShape.GetFolderPath(context, folder),
								errorCode
							});
						}
					}
					return;
				}
			}
		}

		private static string GetFolderPath(MapiContext context, Folder folder)
		{
			string text = Folder.GetFolderPathName(context, folder.Mailbox, folder.GetId(context), '\\');
			if (text.StartsWith("\\IPM_SUBTREE", StringComparison.OrdinalIgnoreCase))
			{
				text = text.Substring("\\IPM_SUBTREE".Length);
			}
			return text;
		}

		private static void PerformLogonQuotaCheck(MapiContext context, MapiLogon mapiLogon, QuotaType minScan, QuotaType maxScan)
		{
			for (QuotaType quotaType = maxScan; quotaType >= minScan; quotaType--)
			{
				QuotaInfo quotaInfo;
				long containerSize;
				ErrorCode errorCode = Quota.CheckForOverQuota(context, mapiLogon.StoreMailbox, quotaType, mapiLogon.IsQuotaMessageDelivery, out quotaInfo, out containerSize);
				if (errorCode != ErrorCode.NoError)
				{
					errorCode = MapiLogon.GenerateQuotaReport(context, mapiLogon, null, quotaType, quotaInfo, new byte[][]
					{
						mapiLogon.MailboxOwnerAddressInfo.UserEntryId()
					}, containerSize);
					if (errorCode != ErrorCode.NoError)
					{
						Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_FailedToSendQuotaWarning, new object[]
						{
							mapiLogon.MailboxOwnerAddressInfo.PrimaryEmailAddress,
							errorCode
						});
					}
					return;
				}
			}
		}

		private static ErrorCode PerformMailboxShapeOverQuotaCheck(MapiContext context, Folder folder, QuotaType mailboxShapeQuota, bool isReportMessage)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			QuotaInfo quotaInfo;
			long num;
			errorCode = MapiMailboxShape.hookableFolderCheck.Value(context, folder, mailboxShapeQuota, isReportMessage, out quotaInfo, out num);
			if (errorCode != ErrorCode.NoError)
			{
				return errorCode.Propagate((LID)30344U);
			}
			return errorCode;
		}

		private static ErrorCode PerformMailboxShapeOverQuotaCheck(MapiContext context, Mailbox mailbox, QuotaType mailboxShapeQuota)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			QuotaInfo quotaInfo;
			long num;
			errorCode = MapiMailboxShape.hookableMailboxCheck.Value(context, mailbox, mailboxShapeQuota, false, out quotaInfo, out num);
			if (errorCode != ErrorCode.NoError)
			{
				return errorCode.Propagate((LID)41792U);
			}
			return errorCode;
		}

		public static readonly TimeSpan QuotaWarningCheckInterval = Mailbox.MaintenanceRunInterval - TimeSpan.FromMinutes(1.0);

		private static Hookable<Quota.FolderCheckDelegate> hookableFolderCheck = Hookable<Quota.FolderCheckDelegate>.Create(true, new Quota.FolderCheckDelegate(Quota.CheckForOverQuota));

		private static Hookable<Quota.MailboxCheckDelegate> hookableMailboxCheck = Hookable<Quota.MailboxCheckDelegate>.Create(true, new Quota.MailboxCheckDelegate(Quota.CheckForOverQuota));

		public enum Operation
		{
			CreateMessage,
			CopyMessage,
			MoveMessage,
			CreateFolder,
			CopyFolder,
			MoveFolder
		}
	}
}
