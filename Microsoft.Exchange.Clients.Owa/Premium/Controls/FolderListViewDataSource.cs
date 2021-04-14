using System;
using System.Collections;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal class FolderListViewDataSource : ExchangeListViewDataSource, IListViewDataSource
	{
		public FolderListViewDataSource(UserContext context, Hashtable properties, Folder folder, SortBy[] sortBy) : this(context, false, properties, folder, sortBy, null)
		{
		}

		public FolderListViewDataSource(UserContext context, Hashtable properties, Folder folder, SortBy[] sortBy, QueryFilter filter) : this(context, false, properties, folder, sortBy, filter)
		{
		}

		public FolderListViewDataSource(UserContext context, bool conversationMode, Hashtable properties, Folder folder, SortBy[] sortBy) : this(context, conversationMode, properties, folder, sortBy, null)
		{
		}

		public FolderListViewDataSource(UserContext context, bool conversationMode, Hashtable properties, Folder folder, SortBy[] sortBy, QueryFilter filter)
		{
			this.shouldDisposeQueryResult = true;
			base..ctor(properties);
			this.folder = folder;
			this.sortBy = sortBy;
			this.userContext = context;
			this.filterQuery = filter;
			this.conversationMode = conversationMode;
		}

		public FolderListViewDataSource(UserContext context, bool conversationMode, Hashtable properties, Folder folder, SortBy[] sortBy, QueryFilter filter, QueryResult queryResult, bool shouldDisposeQueryResult)
		{
			this.shouldDisposeQueryResult = true;
			base..ctor(properties);
			this.folder = folder;
			this.sortBy = sortBy;
			this.userContext = context;
			this.filterQuery = filter;
			this.conversationMode = conversationMode;
			this.queryResult = queryResult;
			this.shouldDisposeQueryResult = shouldDisposeQueryResult;
		}

		public Folder Folder
		{
			get
			{
				return this.folder;
			}
		}

		internal override QueryResult QueryResult
		{
			get
			{
				return this.queryResult;
			}
		}

		public SortBy[] SortBy
		{
			get
			{
				return this.sortBy;
			}
		}

		public override int TotalCount
		{
			get
			{
				return this.totalCount;
			}
		}

		public override int TotalItemCount
		{
			get
			{
				if (this.conversationMode)
				{
					return this.folder.ItemCount;
				}
				return base.TotalItemCount;
			}
		}

		public int UnreadCount
		{
			get
			{
				if (this.folder.TryGetProperty(FolderSchema.UnreadCount) is int)
				{
					return (int)this.folder[FolderSchema.UnreadCount];
				}
				return 0;
			}
		}

		public string ContainerId
		{
			get
			{
				return Utilities.GetIdAsString(this.folder);
			}
		}

		public bool UserHasRightToLoad
		{
			get
			{
				if (!Utilities.IsPublic(this.Folder))
				{
					this.hasRightToLoad = new bool?(true);
				}
				if (this.hasRightToLoad == null)
				{
					object obj = this.Folder.TryGetProperty(StoreObjectSchema.EffectiveRights);
					EffectiveRights effectiveRights = (EffectiveRights)obj;
					this.hasRightToLoad = new bool?((effectiveRights & EffectiveRights.Read) == EffectiveRights.Read);
				}
				return this.hasRightToLoad.Value;
			}
		}

		public OwaStoreObjectId NewSelectionId
		{
			get
			{
				if (this.conversationMode)
				{
					return this.newSelectionId;
				}
				return null;
			}
		}

		protected override bool IsPreviousItemLoaded
		{
			get
			{
				return this.isPreviousItemLoaded;
			}
		}

		public bool IsSearchInProgress()
		{
			bool result = false;
			SearchFolder searchFolder = this.Folder as SearchFolder;
			if (searchFolder != null && this.userContext.IsPushNotificationsEnabled)
			{
				result = this.userContext.MapiNotificationManager.IsSearchInProgress((MailboxSession)searchFolder.Session, searchFolder.StoreObjectId);
			}
			return result;
		}

		public void Load(string seekValue, int itemCount)
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "FolderListViewDataSource.Load(string seekValue, int itemCount)");
			if (itemCount < 1)
			{
				throw new ArgumentOutOfRangeException("itemCount", "itemCount must be greater than 0");
			}
			if (seekValue == null)
			{
				throw new ArgumentNullException("seekValue");
			}
			if (!this.UserHasRightToLoad)
			{
				return;
			}
			PropertyDefinition[] requestedProperties = base.GetRequestedProperties();
			if (this.folder.ItemCount == 0)
			{
				return;
			}
			bool flag = false;
			try
			{
				if (this.queryResult == null)
				{
					this.queryResult = this.CreateQueryResult(this.filterQuery, this.sortBy, requestedProperties);
				}
				PropertyDefinition property = this.sortBy[0].ColumnDefinition;
				if (this.sortBy[0].ColumnDefinition == ItemSchema.Subject)
				{
					property = ItemSchema.NormalizedSubject;
				}
				if (this.sortBy[0].SortOrder == SortOrder.Ascending)
				{
					this.queryResult.SeekToCondition(SeekReference.OriginBeginning, new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, property, seekValue), SeekToConditionFlags.None);
				}
				else
				{
					int length = seekValue.Length;
					char[] array = seekValue.ToCharArray();
					char[] array2 = array;
					int num = length - 1;
					array2[num] += '\u0001';
					string propertyValue = new string(array);
					this.queryResult.SeekToCondition(SeekReference.OriginBeginning, new ComparisonFilter(ComparisonOperator.LessThan, property, propertyValue), SeekToConditionFlags.None);
				}
				if (this.queryResult.EstimatedRowCount == this.queryResult.CurrentRow)
				{
					this.queryResult.SeekToOffset(SeekReference.OriginCurrent, -1 * itemCount);
				}
				this.GetView(this.queryResult, itemCount, this.queryResult.CurrentRow);
				flag = true;
			}
			finally
			{
				if (!flag || this.shouldDisposeQueryResult)
				{
					this.DisposeQueryResultIfPresent();
				}
			}
		}

		public bool LoadAdjacent(ObjectId adjacentObjectId, SeekDirection seekDirection, int itemCount)
		{
			return this.LoadById(adjacentObjectId, seekDirection, itemCount, true);
		}

		public void Load(ObjectId seekToObjectId, SeekDirection seekDirection, int itemCount)
		{
			this.LoadById(seekToObjectId, seekDirection, itemCount, false);
		}

		private bool LoadById(ObjectId seekToObjectId, SeekDirection seekDirection, int itemCount, bool adjacent)
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "FolderListViewDataSource.Load(IUniqueItemId seekToItemId, SeekDirection seekDirection, int itemCount)");
			if (itemCount < 1)
			{
				throw new ArgumentOutOfRangeException("itemCount", "itemCount must be greater than 0");
			}
			if (seekToObjectId == null)
			{
				throw new ArgumentNullException("seekToObjectId");
			}
			if (!this.UserHasRightToLoad)
			{
				return true;
			}
			StoreId storeId = Utilities.TryGetStoreId(seekToObjectId);
			PropertyDefinition[] requestedProperties = base.GetRequestedProperties();
			if (this.folder.ItemCount == 0)
			{
				return true;
			}
			bool flag = false;
			bool flag2 = false;
			try
			{
				if (this.queryResult == null)
				{
					this.queryResult = this.CreateQueryResult(this.filterQuery, this.sortBy, requestedProperties);
				}
				if (!this.conversationMode)
				{
					StoreObjectId storeObjectId = storeId as StoreObjectId;
					if (storeObjectId != null)
					{
						flag2 = this.queryResult.SeekToCondition(SeekReference.OriginBeginning, new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.Id, storeObjectId));
					}
				}
				else
				{
					ConversationId conversationId = storeId as ConversationId;
					if (conversationId != null)
					{
						if (adjacent)
						{
							OwaStoreObjectId owaStoreObjectId = seekToObjectId as OwaStoreObjectId;
							if (owaStoreObjectId.InstanceKey != null)
							{
								flag2 = this.queryResult.SeekToCondition(SeekReference.OriginBeginning, new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.InstanceKey, owaStoreObjectId.InstanceKey));
							}
						}
						else
						{
							flag2 = this.queryResult.SeekToCondition(SeekReference.OriginBeginning, new ComparisonFilter(ComparisonOperator.Equal, ConversationItemSchema.ConversationId, conversationId));
							if (flag2)
							{
								IStorePropertyBag[] propertyBags = this.queryResult.GetPropertyBags(1);
								byte[] instanceKey = propertyBags[0].TryGetProperty(ItemSchema.InstanceKey) as byte[];
								this.newSelectionId = OwaStoreObjectId.CreateFromConversationId(conversationId, this.Folder, instanceKey);
								this.queryResult.SeekToOffset(SeekReference.OriginCurrent, -1);
							}
						}
					}
				}
				if (adjacent && !flag2)
				{
					return false;
				}
				if (!flag2)
				{
					this.queryResult.SeekToOffset(SeekReference.OriginBeginning, 0);
				}
				switch (seekDirection)
				{
				case SeekDirection.Next:
					if (adjacent)
					{
						this.queryResult.SeekToOffset(SeekReference.OriginCurrent, 1);
					}
					else if (this.queryResult.EstimatedRowCount < this.queryResult.CurrentRow + itemCount + 1)
					{
						this.queryResult.SeekToOffset(SeekReference.OriginCurrent, this.queryResult.EstimatedRowCount - this.queryResult.CurrentRow - itemCount);
					}
					break;
				case SeekDirection.Previous:
				{
					int offset;
					if (adjacent)
					{
						if (this.queryResult.CurrentRow == 0)
						{
							return true;
						}
						if (this.queryResult.CurrentRow < itemCount)
						{
							itemCount = this.queryResult.CurrentRow;
						}
						offset = -1 * itemCount;
					}
					else if (this.queryResult.CurrentRow + 1 < itemCount)
					{
						offset = -1 * (this.queryResult.CurrentRow + 1);
					}
					else
					{
						offset = 1 - itemCount;
					}
					this.queryResult.SeekToOffset(SeekReference.OriginCurrent, offset);
					break;
				}
				}
				this.GetView(this.queryResult, itemCount, this.queryResult.CurrentRow);
				flag = true;
			}
			finally
			{
				if (!flag || this.shouldDisposeQueryResult)
				{
					this.DisposeQueryResultIfPresent();
				}
			}
			return true;
		}

		public void Load(int startRange, int itemCount)
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "FolderListViewDataSource.Load(int startRange, int itemCount)");
			if (startRange < 0)
			{
				throw new ArgumentOutOfRangeException("startRange", "Start range (startRange) must be greater than 0");
			}
			if (itemCount < 1)
			{
				throw new ArgumentOutOfRangeException("itemCount", "itemCount must be greater than 0");
			}
			if (!this.UserHasRightToLoad)
			{
				return;
			}
			PropertyDefinition[] requestedProperties = base.GetRequestedProperties();
			if (this.folder.ItemCount <= startRange)
			{
				ExTraceGlobals.MailTracer.TraceDebug((long)this.GetHashCode(), "Requested start range is greater than the number of items in the folder, back up to last page");
				startRange = this.folder.ItemCount - itemCount;
				if (startRange < 0)
				{
					startRange = 0;
				}
			}
			if (this.folder.ItemCount == 0)
			{
				return;
			}
			bool flag = false;
			try
			{
				if (this.queryResult == null)
				{
					this.queryResult = this.CreateQueryResult(this.filterQuery, this.sortBy, requestedProperties);
				}
				int currentRow = 0;
				if (startRange != 0 && startRange < this.queryResult.EstimatedRowCount)
				{
					currentRow = this.queryResult.SeekToOffset(SeekReference.OriginCurrent, startRange);
				}
				this.GetView(this.queryResult, itemCount, currentRow);
				flag = true;
			}
			finally
			{
				if (!flag || this.shouldDisposeQueryResult)
				{
					this.DisposeQueryResultIfPresent();
				}
			}
		}

		private void GetView(QueryResult queryResult, int itemCount, int currentRow)
		{
			if (currentRow > 0)
			{
				queryResult.SeekToOffset(SeekReference.OriginBeginning, currentRow - 1);
				itemCount++;
				this.isPreviousItemLoaded = true;
			}
			try
			{
				this.totalCount = queryResult.EstimatedRowCount;
			}
			catch (AccessDeniedException)
			{
				if (Utilities.IsWebPartDelegateAccessRequest(OwaContext.Current))
				{
					this.totalCount = 0;
					base.Items = new object[0][];
					return;
				}
				throw;
			}
			object[][] array = Utilities.FetchRowsFromQueryResult(queryResult, itemCount);
			if (0 < array.Length)
			{
				base.StartRange = currentRow;
				base.EndRange = currentRow + array.Length - 1;
				if (this.isPreviousItemLoaded)
				{
					base.EndRange--;
				}
			}
			base.Items = array;
		}

		public string GetItemId()
		{
			if (this.conversationMode)
			{
				ConversationId itemProperty = this.GetItemProperty<ConversationId>(ConversationItemSchema.ConversationId);
				byte[] itemProperty2 = this.GetItemProperty<byte[]>(ItemSchema.InstanceKey);
				if (itemProperty != null)
				{
					return OwaStoreObjectId.CreateFromConversationId(itemProperty, this.Folder, itemProperty2).ToString();
				}
				return null;
			}
			else
			{
				VersionedId itemProperty3 = this.GetItemProperty<VersionedId>(ItemSchema.Id);
				if (itemProperty3 != null)
				{
					return Utilities.GetItemIdString(itemProperty3.ObjectId, this.Folder);
				}
				return null;
			}
		}

		public string GetChangeKey()
		{
			VersionedId itemProperty = this.GetItemProperty<VersionedId>(ItemSchema.Id);
			return itemProperty.ChangeKeyAsBase64String();
		}

		public string GetItemClass()
		{
			if (this.conversationMode)
			{
				return "IPM.Conversation";
			}
			string text = this.GetItemProperty<string>(StoreObjectSchema.ItemClass, null);
			if (string.IsNullOrEmpty(text))
			{
				text = "IPM.Unknown";
			}
			return text;
		}

		private QueryResult CreateQueryResult(QueryFilter filterQuery, SortBy[] sortBy, PropertyDefinition[] requestedProperties)
		{
			if (this.conversationMode)
			{
				return this.folder.ConversationItemQuery(filterQuery, sortBy, requestedProperties);
			}
			return this.folder.ItemQuery(ItemQueryType.None, filterQuery, sortBy, requestedProperties);
		}

		private void DisposeQueryResultIfPresent()
		{
			if (this.queryResult != null)
			{
				this.queryResult.Dispose();
				this.queryResult = null;
			}
		}

		protected int totalCount;

		private Folder folder;

		private SortBy[] sortBy;

		private UserContext userContext;

		private QueryResult queryResult;

		private bool shouldDisposeQueryResult;

		private QueryFilter filterQuery;

		private bool? hasRightToLoad;

		private bool isPreviousItemLoaded;

		private bool conversationMode;

		private OwaStoreObjectId newSelectionId;
	}
}
