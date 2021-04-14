using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.AdminInterface;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.AdminInterface
{
	internal class MailboxBasicInformation
	{
		public static void Parse(byte[] buffer, ref int offset, Guid databaseGuid, Guid mailboxGuid, out Guid mailboxInstanceGuid, out Guid folderGuid, out ExchangeId[] fidList)
		{
			mailboxInstanceGuid = ParseSerialize.ParseGuid(buffer, offset);
			offset += 16;
			folderGuid = ParseSerialize.ParseGuid(buffer, offset);
			offset += 16;
			fidList = new ExchangeId[22];
			foreach (SpecialFolders specialFolders in MailboxBasicInformation.folderTypes)
			{
				fidList[(int)specialFolders] = ExchangeId.CreateFromInt64(ParseSerialize.ParseInt64(buffer, offset), folderGuid, ushort.MaxValue);
				offset += 8;
			}
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug(33864L, "Database {0} : Mailbox {1} : FidRoot {2} : FidIPMsubtree {3} : FidDAF {4} : FidSpoolerQ {5} : FidInbox {6} : FidOutbox {7} : FidSentmail {8} : FidWastebasket {9} : FidFinder {10} : FidViews {11} : FidCommonViews {12} : FidSchedule {13} : FidShortcuts {14}", new object[]
				{
					databaseGuid,
					mailboxGuid,
					fidList[1].ToString(),
					fidList[9].ToString(),
					fidList[7].ToString(),
					fidList[8].ToString(),
					fidList[10].ToString(),
					fidList[11].ToString(),
					fidList[12].ToString(),
					fidList[13].ToString(),
					fidList[2].ToString(),
					fidList[3].ToString(),
					fidList[4].ToString(),
					fidList[5].ToString(),
					fidList[6].ToString()
				});
			}
		}

		internal static void ParseMailboxBasicInformation(byte[] buffer, ref int offset, Guid databaseGuid, Guid mailboxGuid, out Guid mailboxInstanceGuid, out Guid folderGuid, out ExchangeId[] fidList)
		{
			MailboxBasicInformation.Parse(buffer, ref offset, databaseGuid, mailboxGuid, out mailboxInstanceGuid, out folderGuid, out fidList);
		}

		public static int Serialize(Mailbox mailbox, byte[] buffer, int offset)
		{
			int num;
			if (buffer == null)
			{
				num = MailboxBasicInformation.Length;
				if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.MailboxSignatureTracer.TraceDebug<string, int, int>(49224L, "Database {0} : Mailbox {1} : Mailbox basic information serialized length {2}", mailbox.Database.MdbName, mailbox.MailboxNumber, num);
				}
			}
			else
			{
				int num2 = offset;
				ExchangeId[] specialFolders = SpecialFoldersCache.GetSpecialFolders(mailbox.CurrentOperationContext, mailbox);
				offset += ParseSerialize.SerializeGuid(mailbox.MailboxInstanceGuid, buffer, offset);
				offset += ParseSerialize.SerializeGuid(specialFolders[1].Guid, buffer, offset);
				bool isPublicFolderMailbox = mailbox.IsPublicFolderMailbox;
				if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.MailboxSignatureTracer.TraceDebug(51272L, "Database {0} : Mailbox {1} : FidRoot {2} : FidIPMsubtree {3} : FidDAF {4} : FidSpoolerQ {5} : FidInbox {6} : FidOutbox {7} : FidSentmail {8} : FidWastebasket {9} : FidFinder {10} : FidViews {11} : FidCommonViews {12} : FidSchedule {13} : FidShortcuts {12} : FidConversations {14}", new object[]
					{
						mailbox.Database.MdbName,
						mailbox.MailboxNumber,
						specialFolders[1].ToString(),
						specialFolders[9].ToString(),
						specialFolders[7].ToString(),
						specialFolders[8].ToString(),
						specialFolders[10].ToString(),
						specialFolders[11].ToString(),
						specialFolders[12].ToString(),
						specialFolders[13].ToString(),
						specialFolders[2].ToString(),
						specialFolders[3].ToString(),
						specialFolders[4].ToString(),
						specialFolders[5].ToString(),
						specialFolders[6].ToString()
					});
				}
				foreach (SpecialFolders specialFolders2 in MailboxBasicInformation.folderTypes)
				{
					offset += ParseSerialize.SerializeInt64(specialFolders[(int)specialFolders2].ToLong(), buffer, offset);
				}
				num = offset - num2;
			}
			return num;
		}

		internal static int SerializeMailboxBasicInformation(Mailbox mailbox, byte[] buffer, int offset)
		{
			int num = offset;
			offset += MailboxBasicInformation.Serialize(mailbox, buffer, offset);
			return offset - num;
		}

		private static readonly SpecialFolders[] folderTypes = new SpecialFolders[]
		{
			SpecialFolders.TopofInformationStore,
			SpecialFolders.DeferredAction,
			SpecialFolders.SpoolerQueue,
			SpecialFolders.Inbox,
			SpecialFolders.Outbox,
			SpecialFolders.SentItems,
			SpecialFolders.DeletedItems,
			SpecialFolders.Finder,
			SpecialFolders.Views,
			SpecialFolders.CommonViews,
			SpecialFolders.Schedule,
			SpecialFolders.Shortcuts,
			SpecialFolders.MailboxRoot
		};

		public static readonly int Length = 32 + 8 * MailboxBasicInformation.folderTypes.Length;
	}
}
