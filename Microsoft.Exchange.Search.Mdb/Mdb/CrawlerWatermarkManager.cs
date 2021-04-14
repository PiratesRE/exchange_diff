using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Search.Mdb
{
	internal sealed class CrawlerWatermarkManager : WatermarkManager<int>
	{
		internal CrawlerWatermarkManager(int batchSize) : base(batchSize)
		{
			base.DiagnosticsSession.ComponentName = "CrawlerWatermarkManager";
			base.DiagnosticsSession.Tracer = ExTraceGlobals.CrawlerWatermarkManagerTracer;
			this.stateUpdateListPerMailbox = new Dictionary<int, SortedDictionary<int, bool>>();
			this.lastDocumentIdPerMailbox = new Dictionary<int, int>();
		}

		internal void SetLast(int mailboxNumber, int lastDocumentId)
		{
			base.DiagnosticsSession.TraceDebug<int, int>("Set last document (DocId={1}) for mailbox({0})", mailboxNumber, lastDocumentId);
			this.lastDocumentIdPerMailbox.Add(mailboxNumber, lastDocumentId);
		}

		internal void Add(MdbItemIdentity compositeId)
		{
			int mailboxNumber = compositeId.MailboxNumber;
			int documentId = compositeId.DocumentId;
			lock (this.stateUpdateListPerMailbox)
			{
				SortedDictionary<int, bool> sortedDictionary;
				if (!this.stateUpdateListPerMailbox.TryGetValue(mailboxNumber, out sortedDictionary))
				{
					base.DiagnosticsSession.TraceDebug<int>("Create a new SortedDictionary for mailbox({0})", mailboxNumber);
					sortedDictionary = new SortedDictionary<int, bool>();
					this.stateUpdateListPerMailbox.Add(mailboxNumber, sortedDictionary);
				}
				base.DiagnosticsSession.TraceDebug<int, int>("Add a new document (DocId={1}) from mailbox({0})", mailboxNumber, documentId);
				sortedDictionary.Add(documentId, false);
			}
		}

		internal bool TryComplete(MdbItemIdentity compositeId, out MailboxCrawlerState stateToUpdate)
		{
			int mailboxNumber = compositeId.MailboxNumber;
			int documentId = compositeId.DocumentId;
			bool result;
			lock (this.stateUpdateListPerMailbox)
			{
				SortedDictionary<int, bool> sortedDictionary;
				if (!this.stateUpdateListPerMailbox.TryGetValue(mailboxNumber, out sortedDictionary))
				{
					throw new InvalidOperationException("Not found the list for mailbox " + mailboxNumber);
				}
				bool flag2;
				if (!sortedDictionary.TryGetValue(documentId, out flag2))
				{
					throw new InvalidOperationException("Not found the docId for document " + documentId);
				}
				if (flag2)
				{
					throw new InvalidOperationException("Invalid to complete again for document " + documentId);
				}
				base.DiagnosticsSession.TraceDebug<int, int>("Try complete the document (DocId={1}) in mailbox({0})", mailboxNumber, documentId);
				sortedDictionary[documentId] = true;
				int num;
				if (base.TryFindNewWatermark(sortedDictionary, out num))
				{
					int num2;
					if (this.lastDocumentIdPerMailbox.TryGetValue(mailboxNumber, out num2) && num2 == num)
					{
						stateToUpdate = new MailboxCrawlerState(mailboxNumber, int.MaxValue, 0);
					}
					else
					{
						stateToUpdate = new MailboxCrawlerState(mailboxNumber, num, 0);
					}
					base.DiagnosticsSession.TraceDebug<int, int>("State needs to update to {1} for mailbox({0})", stateToUpdate.MailboxNumber, stateToUpdate.LastDocumentIdIndexed);
					result = true;
				}
				else
				{
					stateToUpdate = null;
					result = false;
				}
			}
			return result;
		}

		private readonly Dictionary<int, SortedDictionary<int, bool>> stateUpdateListPerMailbox;

		private readonly Dictionary<int, int> lastDocumentIdPerMailbox;
	}
}
