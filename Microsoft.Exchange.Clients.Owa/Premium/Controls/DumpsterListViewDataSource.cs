using System;
using System.Collections;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class DumpsterListViewDataSource : FolderListViewDataSource, IListViewDataSource
	{
		public DumpsterListViewDataSource(UserContext userContext, Hashtable properties, Folder folder, SortBy[] sortBy) : base(userContext, properties, folder, sortBy)
		{
		}

		public new void Load(string seekValue, int itemCount)
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "DumpsterListViewDataSource.Load(string seekValue, int itemCount)");
			if (itemCount < 1)
			{
				throw new ArgumentOutOfRangeException("itemCount", "itemCount must be greater than 0");
			}
			if (seekValue == null)
			{
				throw new ArgumentNullException("seekValue");
			}
			if (!base.UserHasRightToLoad)
			{
				return;
			}
			PropertyDefinition[] requestedProperties = base.GetRequestedProperties();
			using (QueryResult queryResult = base.Folder.FolderQuery(FolderQueryFlags.SoftDeleted, null, DumpsterListViewDataSource.folderSortBy, requestedProperties))
			{
				using (QueryResult queryResult2 = base.Folder.ItemQuery(ItemQueryType.SoftDeleted, null, base.SortBy, requestedProperties))
				{
					this.totalCount = queryResult.EstimatedRowCount + queryResult2.EstimatedRowCount;
					if (this.totalCount != 0)
					{
						object[][] array = null;
						object[][] itemsItemQuery = null;
						ComparisonOperator comparisonOperator = (base.SortBy[0].SortOrder == SortOrder.Ascending) ? ComparisonOperator.GreaterThanOrEqual : ComparisonOperator.LessThanOrEqual;
						bool flag = true;
						if (base.SortBy[0].ColumnDefinition == ItemSchema.SentRepresentingDisplayName)
						{
							flag = false;
						}
						if (base.SortBy[0].ColumnDefinition == ItemSchema.Subject && queryResult.EstimatedRowCount != 0 && queryResult2.EstimatedRowCount != 0)
						{
							queryResult.SeekToCondition(SeekReference.OriginBeginning, new ComparisonFilter(comparisonOperator, FolderSchema.DisplayName, seekValue));
							queryResult2.SeekToCondition(SeekReference.OriginBeginning, new ComparisonFilter(comparisonOperator, ItemSchema.NormalizedSubject, seekValue));
							object[][] rows = queryResult.GetRows(1);
							object[][] rows2 = queryResult2.GetRows(1);
							string text = string.Empty;
							string text2 = string.Empty;
							if (rows != null && rows.Length > 0)
							{
								text = (rows[0][base.GetPropertyIndex(FolderSchema.DisplayName)] as string);
							}
							if (rows2 != null && rows2.Length > 0)
							{
								text2 = (rows2[0][base.GetPropertyIndex(ItemSchema.NormalizedSubject)] as string);
							}
							if (string.IsNullOrEmpty(text))
							{
								flag = false;
							}
							else if (string.IsNullOrEmpty(text2))
							{
								flag = true;
							}
							else if (comparisonOperator == ComparisonOperator.GreaterThanOrEqual)
							{
								flag = (text.CompareTo(text2) <= 0);
							}
							else
							{
								flag = (text.CompareTo(text2) >= 0);
							}
						}
						if (flag)
						{
							ComparisonFilter seekFilter = new ComparisonFilter(comparisonOperator, (base.SortBy[0].ColumnDefinition == ItemSchema.Subject) ? FolderSchema.DisplayName : base.SortBy[0].ColumnDefinition, seekValue);
							if (!queryResult.SeekToCondition(SeekReference.OriginBeginning, seekFilter) && queryResult2.EstimatedRowCount == 0)
							{
								queryResult.SeekToOffset(SeekReference.OriginCurrent, -1 * itemCount);
							}
							array = Utilities.FetchRowsFromQueryResult(queryResult, itemCount);
							itemCount -= array.Length;
						}
						if (itemCount > 0)
						{
							if (!flag)
							{
								if (base.SortBy[0].ColumnDefinition == ItemSchema.Subject)
								{
									queryResult2.SeekToCondition(SeekReference.OriginBeginning, new ComparisonFilter(comparisonOperator, ItemSchema.NormalizedSubject, seekValue));
								}
								else
								{
									queryResult2.SeekToCondition(SeekReference.OriginBeginning, new ComparisonFilter(comparisonOperator, base.SortBy[0].ColumnDefinition, seekValue));
								}
							}
							else
							{
								queryResult2.SeekToOffset(SeekReference.OriginBeginning, 0);
							}
							if (queryResult2.CurrentRow == queryResult2.EstimatedRowCount)
							{
								queryResult2.SeekToOffset(SeekReference.OriginCurrent, -1 * itemCount);
							}
							itemsItemQuery = Utilities.FetchRowsFromQueryResult(queryResult2, itemCount);
						}
						this.CombineView(array, itemsItemQuery, queryResult, queryResult2);
					}
				}
			}
		}

		public new void Load(int startRange, int itemCount)
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "DumpsterListViewDataSource.Load(int startRange, int itemCount)");
			if (startRange < 0)
			{
				throw new ArgumentOutOfRangeException("startRange", "Start range (startRange) must be greater than 0");
			}
			if (itemCount < 1)
			{
				throw new ArgumentOutOfRangeException("itemCount", "itemCount must be greater than 0");
			}
			if (!base.UserHasRightToLoad)
			{
				return;
			}
			PropertyDefinition[] requestedProperties = base.GetRequestedProperties();
			using (QueryResult queryResult = base.Folder.FolderQuery(FolderQueryFlags.SoftDeleted, null, DumpsterListViewDataSource.folderSortBy, requestedProperties))
			{
				using (QueryResult queryResult2 = base.Folder.ItemQuery(ItemQueryType.SoftDeleted, null, base.SortBy, requestedProperties))
				{
					this.totalCount = queryResult.EstimatedRowCount + queryResult2.EstimatedRowCount;
					if (this.totalCount != 0)
					{
						object[][] array = null;
						object[][] itemsItemQuery = null;
						if (this.totalCount <= startRange)
						{
							ExTraceGlobals.MailTracer.TraceDebug((long)this.GetHashCode(), "Requested start range is greater than the number of items in the folder, back up to last page");
							startRange = this.totalCount - itemCount;
							if (startRange < 0)
							{
								startRange = 0;
							}
						}
						if (startRange <= queryResult.EstimatedRowCount)
						{
							queryResult.SeekToOffset(SeekReference.OriginCurrent, startRange);
							array = Utilities.FetchRowsFromQueryResult(queryResult, itemCount);
						}
						if (array != null)
						{
							itemCount -= array.Length;
						}
						if (itemCount > 0)
						{
							int num = startRange - queryResult.EstimatedRowCount;
							if (num < 0)
							{
								num = 0;
							}
							if (num != 0 && num < queryResult2.EstimatedRowCount)
							{
								queryResult2.SeekToOffset(SeekReference.OriginCurrent, num);
							}
							itemsItemQuery = Utilities.FetchRowsFromQueryResult(queryResult2, itemCount);
						}
						this.CombineView(array, itemsItemQuery, queryResult, queryResult2);
					}
				}
			}
		}

		public new void Load(ObjectId seekToObjectId, SeekDirection seekDirection, int itemCount)
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "DumpsterListViewDataSource.Load(ObjectId seekToObjectId, SeekDirection seekDirection, int itemCount)");
			if (itemCount < 1)
			{
				throw new ArgumentOutOfRangeException("itemCount", "itemCount must be greater than 0");
			}
			if (seekToObjectId == null)
			{
				throw new ArgumentNullException("seekToObjectId");
			}
			if (!base.UserHasRightToLoad)
			{
				return;
			}
			StoreObjectId storeObjectId = Utilities.TryGetStoreId(seekToObjectId) as StoreObjectId;
			if (storeObjectId == null)
			{
				throw new ArgumentException("seekToObjectId could not be converted to a StoreObjectId");
			}
			PropertyDefinition[] requestedProperties = base.GetRequestedProperties();
			using (QueryResult queryResult = base.Folder.FolderQuery(FolderQueryFlags.SoftDeleted, null, DumpsterListViewDataSource.folderSortBy, requestedProperties))
			{
				using (QueryResult queryResult2 = base.Folder.ItemQuery(ItemQueryType.SoftDeleted, null, base.SortBy, requestedProperties))
				{
					this.totalCount = queryResult.EstimatedRowCount + queryResult2.EstimatedRowCount;
					object[][] itemsItemQuery = null;
					if (this.totalCount != 0)
					{
						queryResult.SeekToCondition(SeekReference.OriginBeginning, new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.Id, storeObjectId));
						switch (seekDirection)
						{
						case SeekDirection.Next:
							if (this.totalCount < queryResult.CurrentRow + itemCount + 1)
							{
								queryResult.SeekToOffset(SeekReference.OriginCurrent, this.totalCount - queryResult.CurrentRow - itemCount);
							}
							break;
						case SeekDirection.Previous:
						{
							int offset;
							if (queryResult.CurrentRow + 1 < itemCount)
							{
								offset = -1 * (queryResult.CurrentRow + 1);
							}
							else
							{
								offset = 1 - itemCount;
							}
							queryResult.SeekToOffset(SeekReference.OriginCurrent, offset);
							break;
						}
						}
						object[][] array = Utilities.FetchRowsFromQueryResult(queryResult, itemCount);
						if (array.Length > 0)
						{
							base.StartRange = queryResult.CurrentRow - array.Length;
						}
						itemCount -= array.Length;
						if (itemCount > 0)
						{
							if (array.Length == 0)
							{
								queryResult2.SeekToCondition(SeekReference.OriginBeginning, new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.Id, storeObjectId));
								switch (seekDirection)
								{
								case SeekDirection.Next:
									if (queryResult2.EstimatedRowCount < queryResult2.CurrentRow + itemCount + 1)
									{
										queryResult2.SeekToOffset(SeekReference.OriginCurrent, queryResult2.EstimatedRowCount - queryResult2.CurrentRow - itemCount);
									}
									break;
								case SeekDirection.Previous:
								{
									int offset2;
									if (queryResult2.CurrentRow + 1 < itemCount)
									{
										offset2 = -1 * (queryResult2.CurrentRow + 1);
										int num = itemCount - queryResult2.CurrentRow;
										int estimatedRowCount = queryResult.EstimatedRowCount;
										if (num > estimatedRowCount)
										{
											num = estimatedRowCount;
										}
										queryResult.SeekToOffset(SeekReference.OriginCurrent, -1 * num);
										array = Utilities.FetchRowsFromQueryResult(queryResult, num);
									}
									else
									{
										offset2 = 1 - itemCount;
									}
									queryResult.SeekToOffset(SeekReference.OriginCurrent, offset2);
									break;
								}
								}
							}
							itemsItemQuery = Utilities.FetchRowsFromQueryResult(queryResult2, itemCount);
						}
						this.CombineView(array, itemsItemQuery, queryResult, queryResult2);
					}
				}
			}
		}

		private void CombineView(object[][] itemsFolderQuery, object[][] itemsItemQuery, QueryResult folderQueryResult, QueryResult itemQueryResult)
		{
			if ((itemsFolderQuery == null || itemsFolderQuery.Length == 0) && itemsItemQuery != null)
			{
				base.Items = itemsItemQuery;
				base.StartRange = itemQueryResult.CurrentRow - itemsItemQuery.Length + folderQueryResult.EstimatedRowCount;
				base.EndRange = itemQueryResult.CurrentRow - 1 + folderQueryResult.EstimatedRowCount;
				return;
			}
			if ((itemsItemQuery == null || itemsItemQuery.Length == 0) && itemsFolderQuery != null)
			{
				base.Items = itemsFolderQuery;
				base.StartRange = folderQueryResult.CurrentRow - itemsFolderQuery.Length;
				base.EndRange = folderQueryResult.CurrentRow - 1;
				return;
			}
			object[][] array = new object[itemsFolderQuery.Length + itemsItemQuery.Length][];
			Array.Copy(itemsFolderQuery, 0, array, 0, itemsFolderQuery.Length);
			Array.Copy(itemsItemQuery, 0, array, itemsFolderQuery.Length, itemsItemQuery.Length);
			base.Items = array;
			base.StartRange = folderQueryResult.CurrentRow - itemsFolderQuery.Length;
			base.EndRange = itemQueryResult.CurrentRow - 1 + folderQueryResult.EstimatedRowCount;
		}

		public new string GetItemId()
		{
			VersionedId itemProperty = this.GetItemProperty<VersionedId>(FolderSchema.Id);
			if (itemProperty != null)
			{
				return OwaStoreObjectId.CreateFromMailboxFolderId(itemProperty.ObjectId).ToString();
			}
			itemProperty = this.GetItemProperty<VersionedId>(ItemSchema.Id);
			if (itemProperty != null)
			{
				return Utilities.GetItemIdString(itemProperty.ObjectId, base.Folder);
			}
			return null;
		}

		private static readonly SortBy[] folderSortBy = new SortBy[]
		{
			new SortBy(FolderSchema.DisplayName, SortOrder.Ascending)
		};
	}
}
