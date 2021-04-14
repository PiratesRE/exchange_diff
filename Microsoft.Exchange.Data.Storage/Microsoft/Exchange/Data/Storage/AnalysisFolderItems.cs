using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AnalysisFolderItems
	{
		public AnalysisFolderItems(MailboxSession session, IEnumerable<StoreObjectId> folderIds)
		{
			this.session = session;
			this.folders = new List<StoreObjectId>(folderIds);
			this.itemGroups = new Dictionary<string, AnalysisGroupData>();
			this.clientsByString = new Dictionary<string, int>();
			this.clientsById = new Dictionary<int, string>();
			this.itemGroupsBySize = new List<AnalysisGroupData>(0);
		}

		internal Dictionary<int, string> Clients
		{
			get
			{
				return this.clientsById;
			}
		}

		internal MailboxSession MailboxSession
		{
			get
			{
				return this.session;
			}
		}

		public int GetClientInfoId(string clientInfo)
		{
			int result = -1;
			if (!this.clientsByString.TryGetValue(clientInfo, out result))
			{
				int count = this.clientsByString.Count;
				this.clientsByString[clientInfo] = count;
				result = count;
			}
			return result;
		}

		public List<AnalysisGroupData> GroupsBySize()
		{
			return this.itemGroupsBySize;
		}

		public TimeSpan ExecuteTime()
		{
			return ExDateTime.TimeDiff(this.processEndTime, this.processStartTime);
		}

		public int TotalCount
		{
			get
			{
				return this.totalCount;
			}
		}

		public ByteQuantifiedSize TotalSize
		{
			get
			{
				return new ByteQuantifiedSize(this.totalSize);
			}
		}

		public void Execute()
		{
			this.processEndTime = (this.processStartTime = ExDateTime.Now);
			this.totalCount = 0;
			this.totalSize = 0UL;
			this.itemGroups = new Dictionary<string, AnalysisGroupData>();
			this.clientsByString = new Dictionary<string, int>();
			this.clientsById = new Dictionary<int, string>();
			foreach (StoreObjectId folderId in this.folders)
			{
				this.ProcessOneFolder(folderId);
			}
			this.itemGroupsBySize = new List<AnalysisGroupData>(this.itemGroups.Count);
			foreach (KeyValuePair<string, AnalysisGroupData> keyValuePair in this.itemGroups)
			{
				this.itemGroupsBySize.Add(keyValuePair.Value);
			}
			this.itemGroupsBySize.Sort(new Comparison<AnalysisGroupData>(AnalysisFolderItems.CompareGroupsBySize));
			foreach (KeyValuePair<string, int> keyValuePair2 in this.clientsByString)
			{
				this.clientsById[keyValuePair2.Value] = keyValuePair2.Key;
			}
			ExTraceGlobals.SessionTracer.TraceDebug<int, int, int>((long)this.session.GetHashCode(), "AnalysisFolderItems ended with {0} item, {1} groups, {2} client strings.", this.totalCount, this.itemGroups.Count, this.clientsByString.Count);
			this.processEndTime = ExDateTime.Now;
		}

		private static int CompareGroupsBySize(AnalysisGroupData x, AnalysisGroupData y)
		{
			return -x.GroupSize.CompareTo(y.GroupSize);
		}

		private void ProcessOneFolder(StoreObjectId folderId)
		{
			using (Folder folder = Folder.Bind(this.session, folderId))
			{
				ExTraceGlobals.SessionTracer.TraceDebug<string, string>((long)this.session.GetHashCode(), "AnalysisFolderItems on folder {0}, id {1} starting.", folder.DisplayName ?? string.Empty, folderId.ToString());
				using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, AnalysisFolderItems.querySort, AnalysisFolderItems.queryProperties))
				{
					for (;;)
					{
						object[][] rows = queryResult.GetRows(AnalysisFolderItems.queryPageSize);
						if (rows == null || rows.Length <= 0)
						{
							break;
						}
						for (int i = 0; i < rows.Length; i++)
						{
							this.ProcessOneItem(new AnalysisItemsQueryData(rows[i]));
						}
					}
				}
			}
		}

		private void ProcessOneItem(AnalysisItemsQueryData item)
		{
			this.totalCount++;
			this.totalSize += (ulong)((long)item.Size);
			AnalysisGroupData analysisGroupData;
			if (!this.itemGroups.TryGetValue(item.Key.ToString(), out analysisGroupData))
			{
				analysisGroupData = new AnalysisGroupData(this, item.Key);
				this.itemGroups[item.Key.ToString()] = analysisGroupData;
			}
			analysisGroupData.AddOneItem(item);
		}

		private static int queryPageSize = 500;

		private static SortBy[] querySort = new SortBy[]
		{
			new SortBy(StoreObjectSchema.LastModifiedTime, SortOrder.Descending)
		};

		private static PropertyDefinition[] queryProperties = new PropertyDefinition[]
		{
			ItemSchema.NormalizedSubject,
			ItemSchema.ReceivedTime,
			InternalSchema.CleanGlobalObjectId,
			StoreObjectSchema.ItemClass,
			ItemSchema.Id,
			ItemSchema.Size,
			StoreObjectSchema.LastModifiedTime,
			InternalSchema.ClientInfoString,
			InternalSchema.ClientProcessName,
			InternalSchema.ClientMachineName
		};

		private MailboxSession session;

		private List<StoreObjectId> folders;

		private int totalCount;

		private ulong totalSize;

		private ExDateTime processStartTime;

		private ExDateTime processEndTime;

		private Dictionary<string, AnalysisGroupData> itemGroups;

		private List<AnalysisGroupData> itemGroupsBySize;

		private Dictionary<string, int> clientsByString;

		private Dictionary<int, string> clientsById;

		public enum QueryIndex
		{
			NormalizedSubject,
			ReceivedTime,
			CleanGlobalObjectId,
			ItemClass,
			Id,
			Size,
			LastModifiedTime,
			ClientInfoString,
			ClientProcessName,
			ClientMachineName
		}
	}
}
