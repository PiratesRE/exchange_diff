using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;

namespace Microsoft.Exchange.Search.Mdb
{
	internal class CrawlerDocIdViewIterator : ICrawlerItemIterator<int>
	{
		internal CrawlerDocIdViewIterator(int maxRowCount)
		{
			this.diagnosticsSession = DiagnosticsSession.CreateComponentDiagnosticsSession("CrawlerDocIdViewIterator", ComponentInstance.Globals.Search.ServiceName, ExTraceGlobals.MdbCrawlerFeederTracer, (long)this.GetHashCode());
			this.maxRowCount = Math.Min(maxRowCount, 10000);
		}

		public virtual IEnumerable<MdbItemIdentity> GetItems(StoreSession storeSession, int startDocumentId, int endDocumentId)
		{
			Util.ThrowOnNullArgument(storeSession, "storeSession");
			int mailboxNumber = (int)storeSession.Mailbox.TryGetProperty(MailboxSchema.MailboxNumber);
			Guid mailboxGuid = storeSession.MailboxGuid;
			Guid mdbGuid = storeSession.MdbGuid;
			this.diagnosticsSession.TraceDebug<Guid, int, Guid>("GetItems for mailbox {0}({1}) in mdb {2}", mailboxGuid, mailboxNumber, mdbGuid);
			HashSet<StoreObjectId> parentFolderIds = new HashSet<StoreObjectId>();
			using (Folder rootFolder = XsoUtil.TranslateXsoExceptionsWithReturnValue<Folder>(this.diagnosticsSession, Strings.ConnectionToMailboxFailed(mailboxGuid), () => XsoUtil.GetRootFolder(storeSession)))
			{
				foreach (StoreObjectId item in XsoUtil.GetSubfolders(this.diagnosticsSession, rootFolder, CrawlerDocIdViewIterator.FolderFilter))
				{
					parentFolderIds.Add(item);
				}
				this.diagnosticsSession.TraceDebug<int>("Built up {0} folders that are interesting", parentFolderIds.Count);
				foreach (object[][] rows in this.GetRowsFromDocIdView(rootFolder, startDocumentId))
				{
					foreach (object[] properties in rows)
					{
						this.diagnosticsSession.Assert(properties != null && properties.Length == 5, "properties", new object[0]);
						int documentId = (int)properties[0];
						if (documentId > endDocumentId)
						{
							this.diagnosticsSession.TraceDebug<int, int>("Current item (DocId={0}) has exceeded the requested range (EndDocId={1})", documentId, endDocumentId);
							yield break;
						}
						bool isAssociated = (bool)properties[3];
						if (isAssociated)
						{
							this.diagnosticsSession.TraceDebug<int>("Skip a FAI item (DocId={0})", documentId);
						}
						else
						{
							string messageClass = properties[4] as string;
							if (XsoUtil.ShouldSkipMessageClass(messageClass))
							{
								this.diagnosticsSession.TraceDebug<int>("Skip an item (DocId={0}) due to message class", documentId);
							}
							else
							{
								StoreObjectId parentId = (StoreObjectId)properties[2];
								if (!parentFolderIds.Contains(parentId))
								{
									this.diagnosticsSession.TraceDebug<int>("Skip an item (DocId={0}) from non-interesting folder", documentId);
								}
								else
								{
									this.diagnosticsSession.TraceDebug<int>("yield returning an item (DocId={0})", documentId);
									VersionedId itemId = (VersionedId)properties[1];
									yield return new MdbItemIdentity(storeSession.PersistableTenantPartitionHint, mdbGuid, mailboxGuid, mailboxNumber, itemId.ObjectId, documentId, storeSession.IsPublicFolderSession);
								}
							}
						}
					}
				}
			}
			yield break;
		}

		private IEnumerable<object[][]> GetRowsFromDocIdView(Folder rootFolder, int minDocumentId)
		{
			CrawlerDocIdViewIterator.<>c__DisplayClass1b CS$<>8__locals1 = new CrawlerDocIdViewIterator.<>c__DisplayClass1b();
			CS$<>8__locals1.rootFolder = rootFolder;
			CS$<>8__locals1.<>4__this = this;
			Util.ThrowOnNullArgument(CS$<>8__locals1.rootFolder, "rootFolder");
			Guid mailboxGuid = CS$<>8__locals1.rootFolder.Session.MailboxGuid;
			using (QueryResult queryResult = XsoUtil.TranslateXsoExceptionsWithReturnValue<QueryResult>(this.diagnosticsSession, Strings.ConnectionToMailboxFailed(mailboxGuid), () => CS$<>8__locals1.rootFolder.ItemQuery(ItemQueryType.DocumentIdView, null, null, CrawlerDocIdViewIterator.QueryProperties)))
			{
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.GreaterThan, ItemSchema.DocumentId, minDocumentId);
				if (XsoUtil.TranslateXsoExceptionsWithReturnValue<bool>(this.diagnosticsSession, Strings.ConnectionToMailboxFailed(mailboxGuid), () => queryResult.SeekToCondition(SeekReference.OriginCurrent, filter, SeekToConditionFlags.KeepCursorPositionWhenNoMatch)))
				{
					for (;;)
					{
						object[][] rows = XsoUtil.TranslateXsoExceptionsWithReturnValue<object[][]>(this.diagnosticsSession, Strings.ConnectionToMailboxFailed(mailboxGuid), () => queryResult.GetRows(CS$<>8__locals1.<>4__this.maxRowCount));
						if (rows == null || rows.Length == 0)
						{
							break;
						}
						yield return rows;
					}
				}
			}
			yield break;
		}

		private static readonly QueryFilter FolderFilter = new ComparisonFilter(ComparisonOperator.Equal, FolderSchema.PartOfContentIndexing, true);

		private static readonly PropertyDefinition[] QueryProperties = new PropertyDefinition[]
		{
			ItemSchema.DocumentId,
			ItemSchema.Id,
			StoreObjectSchema.ParentItemId,
			MessageItemSchema.IsAssociated,
			StoreObjectSchema.ItemClass
		};

		private readonly IDiagnosticsSession diagnosticsSession;

		private readonly int maxRowCount;

		private enum QueryPropertiesIndex
		{
			DocumentId,
			ItemId,
			ParentId,
			IsAssociated,
			ItemClass,
			Max
		}
	}
}
