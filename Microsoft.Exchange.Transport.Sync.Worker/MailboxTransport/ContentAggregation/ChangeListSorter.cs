using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ChangeListSorter
	{
		public ChangeListSorter(Func<SyncChangeEntry, bool> parentFolderAlreadySeenByCaller)
		{
			SyncUtilities.ThrowIfArgumentNull("parentFolderAlreadySeenByCaller", parentFolderAlreadySeenByCaller);
			this.parentFolderAlreadySeenByCaller = parentFolderAlreadySeenByCaller;
		}

		public void Run(IList<SyncChangeEntry> incomingChangList, out List<SyncChangeEntry> inOrderChangeList, out List<SyncChangeEntry> orphanChangeList)
		{
			SyncUtilities.ThrowIfArgumentNull("incomingChangList", incomingChangList);
			inOrderChangeList = new List<SyncChangeEntry>(incomingChangList.Count);
			HashSet<string> hashSet = new HashSet<string>();
			Dictionary<string, List<SyncChangeEntry>> dictionary = new Dictionary<string, List<SyncChangeEntry>>(incomingChangList.Count / 2);
			foreach (SyncChangeEntry syncChangeEntry in incomingChangList)
			{
				this.AddSyncChangeEntryInOrder(syncChangeEntry, inOrderChangeList, hashSet, dictionary);
			}
			orphanChangeList = new List<SyncChangeEntry>(dictionary.Count);
			foreach (List<SyncChangeEntry> collection in dictionary.Values)
			{
				orphanChangeList.AddRange(collection);
			}
			dictionary.Clear();
			dictionary = null;
			hashSet.Clear();
			hashSet = null;
		}

		private void AddSyncChangeEntryInOrder(SyncChangeEntry syncChangeEntry, List<SyncChangeEntry> inOrderChangeList, HashSet<string> inOrderFolders, Dictionary<string, List<SyncChangeEntry>> outOfOrderChangeList)
		{
			if (syncChangeEntry.CloudFolderId == null || this.parentFolderAlreadySeenByCaller(syncChangeEntry) || inOrderFolders.Contains(syncChangeEntry.CloudFolderId))
			{
				this.AddSyncChangeEntryAndItsChildrenToInOrderList(syncChangeEntry, inOrderChangeList, inOrderFolders, outOfOrderChangeList);
				return;
			}
			List<SyncChangeEntry> list = null;
			if (!outOfOrderChangeList.TryGetValue(syncChangeEntry.CloudFolderId, out list))
			{
				list = new List<SyncChangeEntry>(1);
				outOfOrderChangeList.Add(syncChangeEntry.CloudFolderId, list);
			}
			list.Add(syncChangeEntry);
		}

		private void AddSyncChangeEntryAndItsChildrenToInOrderList(SyncChangeEntry syncChangeEntry, List<SyncChangeEntry> inOrderChangeList, HashSet<string> inOrderFolders, Dictionary<string, List<SyncChangeEntry>> outOfOrderChangeList)
		{
			inOrderChangeList.Add(syncChangeEntry);
			inOrderFolders.Add(syncChangeEntry.CloudId);
			if (outOfOrderChangeList.ContainsKey(syncChangeEntry.CloudId))
			{
				foreach (SyncChangeEntry syncChangeEntry2 in outOfOrderChangeList[syncChangeEntry.CloudId])
				{
					this.AddSyncChangeEntryAndItsChildrenToInOrderList(syncChangeEntry2, inOrderChangeList, inOrderFolders, outOfOrderChangeList);
				}
				outOfOrderChangeList.Remove(syncChangeEntry.CloudId);
			}
		}

		private readonly Func<SyncChangeEntry, bool> parentFolderAlreadySeenByCaller;
	}
}
