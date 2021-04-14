using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AnalysisGroupData
	{
		public AnalysisGroupData(AnalysisFolderItems parent, AnalysisGroupKey key)
		{
			this.parent = parent;
			this.key = key;
			this.allItemsItemId = null;
			this.newestItemId = null;
			this.newestItemLMT = ExDateTime.MinValue;
			this.clientInfoStats = new Dictionary<int, int>();
		}

		public Item GetItemInAllItems(ICollection<PropertyDefinition> propsToReturn)
		{
			Item result = null;
			this.FindItemInAllItems();
			if (this.allItemsItemId == null)
			{
				return null;
			}
			try
			{
				result = Item.Bind(this.parent.MailboxSession, this.allItemsItemId, propsToReturn);
			}
			catch (Exception ex)
			{
				ExTraceGlobals.SessionTracer.TraceError<string, string>((long)this.parent.MailboxSession.GetHashCode(), "AnalysisGroupData.GetItemInAllItems failed to get item {0}, error {1}.", this.allItemsItemId.ToString(), ex.ToString());
				result = null;
			}
			return result;
		}

		public Item GetItemInGroup(ICollection<PropertyDefinition> propsToReturn)
		{
			Item result = null;
			if (this.newestItemId == null)
			{
				return null;
			}
			try
			{
				result = Item.Bind(this.parent.MailboxSession, this.newestItemId, propsToReturn);
			}
			catch (Exception ex)
			{
				ExTraceGlobals.SessionTracer.TraceError<string, string>((long)this.parent.MailboxSession.GetHashCode(), "AnalysisGroupData.GetItemInGroup failed to get item {0}, error {1}.", this.newestItemId.ToString(), ex.ToString());
				result = null;
			}
			return result;
		}

		public StoreObjectId GroupItemId
		{
			get
			{
				return this.newestItemId;
			}
		}

		public StoreObjectId AllItemsItemId
		{
			get
			{
				return this.allItemsItemId;
			}
		}

		public int GroupCount
		{
			get
			{
				return this.groupCount;
			}
		}

		public ByteQuantifiedSize GroupSize
		{
			get
			{
				return new ByteQuantifiedSize(this.groupSize);
			}
		}

		public KeyValuePair<string, int> TopClientInfo
		{
			get
			{
				int num = -1;
				int num2 = -1;
				foreach (KeyValuePair<int, int> keyValuePair in this.clientInfoStats)
				{
					if (keyValuePair.Value > num)
					{
						num = keyValuePair.Value;
						num2 = keyValuePair.Key;
					}
				}
				if (num2 >= 0)
				{
					return new KeyValuePair<string, int>(this.parent.Clients[num2], num);
				}
				return new KeyValuePair<string, int>(string.Empty, 0);
			}
		}

		public string GetItemInGroupFolderPath()
		{
			if (this.newestItemId == null)
			{
				return string.Empty;
			}
			return AnalysisGroupData.GetFolderPathForFolderId(this.parent.MailboxSession, IdConverter.GetParentIdFromMessageId(this.newestItemId));
		}

		public string GetItemInAllItemsFolderPath()
		{
			if (this.allItemsItemId == null)
			{
				return string.Empty;
			}
			return AnalysisGroupData.GetFolderPathForFolderId(this.parent.MailboxSession, IdConverter.GetParentIdFromMessageId(this.allItemsItemId));
		}

		internal static string GetFolderPathForFolderId(MailboxSession session, StoreObjectId folderId)
		{
			StringBuilder stringBuilder = new StringBuilder(128);
			StoreObjectId storeObjectId = folderId;
			int num = 0;
			int num2 = 10;
			while (storeObjectId != null && num < num2)
			{
				using (Folder folder = Folder.Bind(session, storeObjectId))
				{
					num++;
					if (stringBuilder.Length == 0)
					{
						stringBuilder.Append(folder.DisplayName);
					}
					else
					{
						stringBuilder.Insert(0, "\\");
						stringBuilder.Insert(0, folder.DisplayName);
					}
					num++;
					if (folder.ParentId != null && storeObjectId.Equals(folder.ParentId))
					{
						break;
					}
					storeObjectId = folder.ParentId;
				}
			}
			if (num >= num2)
			{
				ExTraceGlobals.SessionTracer.TraceWarning<int, string, string>((long)session.GetHashCode(), "AnalysisGroupData.GetFolderPathForFolderId hit the max {0} folder depth for folder if {1}, path {2}.", num, folderId.ToString(), stringBuilder.ToString());
				stringBuilder.Insert(0, "...\\");
			}
			return stringBuilder.ToString();
		}

		internal void AddOneItem(AnalysisItemsQueryData item)
		{
			this.groupCount++;
			this.groupSize += (ulong)((long)item.Size);
			if (item.ClientInfo != null && !item.ClientInfo.Equals(string.Empty))
			{
				int clientInfoId = this.parent.GetClientInfoId(item.ClientInfo);
				if (!this.clientInfoStats.ContainsKey(clientInfoId))
				{
					this.clientInfoStats[clientInfoId] = 1;
				}
				else
				{
					this.clientInfoStats[clientInfoId] = this.clientInfoStats[clientInfoId] + 1;
				}
			}
			if (this.newestItemLMT < item.LastModifiedTime)
			{
				this.newestItemLMT = item.LastModifiedTime;
				this.newestItemId = item.Id;
			}
		}

		private void FindItemInAllItems()
		{
			this.allItemsItemId = null;
			AllItemsFolderHelper.CheckAndCreateDefaultFolders(this.parent.MailboxSession);
			using (Folder folder = Folder.Bind(this.parent.MailboxSession, this.key.FolderToSearch()))
			{
				PropertyDefinition[] dataColumns = new PropertyDefinition[]
				{
					ItemSchema.Id
				};
				using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, this.key.Filter, null, dataColumns))
				{
					object[][] rows = queryResult.GetRows(1);
					if (rows == null)
					{
						ExTraceGlobals.SessionTracer.TraceWarning<string, string>((long)this.parent.MailboxSession.GetHashCode(), "AnalysisGroupData.FindItemInAllItems found NO items in folder {0} for the key {1}", this.key.FolderToSearch().ToString(), this.key.ToString());
						this.allItemsItemId = null;
					}
					else if (rows.Length != 1)
					{
						ExTraceGlobals.SessionTracer.TraceWarning<string, string, int>((long)this.parent.MailboxSession.GetHashCode(), "AnalysisGroupData.FindItemInAllItems found {2} items in folder {0} for the key {1}", this.key.FolderToSearch().ToString(), this.key.ToString(), rows.Length);
						this.allItemsItemId = null;
					}
					else
					{
						this.allItemsItemId = StoreId.GetStoreObjectId(rows[0][0] as StoreId);
					}
				}
			}
		}

		private AnalysisFolderItems parent;

		private AnalysisGroupKey key;

		private int groupCount;

		private ulong groupSize;

		private StoreObjectId newestItemId;

		private ExDateTime newestItemLMT;

		private StoreObjectId allItemsItemId;

		private Dictionary<int, int> clientInfoStats;
	}
}
