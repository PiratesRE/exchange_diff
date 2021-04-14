using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PopImap.Core
{
	internal class DataAccessView : DisposeTrackableBase
	{
		public DataAccessView(ResponseFactory factory, Folder folder)
		{
			this.factory = factory;
			this.uidCache = new Queue<DataAccessView.UidPair>(DataAccessView.uidCacheSize);
			IList<PropertyDefinition> list;
			if (this.AdditionalProperties == null)
			{
				list = DataAccessView.ViewProperties;
			}
			else
			{
				list = new List<PropertyDefinition>(DataAccessView.ViewProperties);
				((List<PropertyDefinition>)list).AddRange(this.AdditionalProperties);
			}
			this.view = folder.ItemQuery(ItemQueryType.None, null, this.SortOrders, list);
		}

		public QueryResult TableView
		{
			get
			{
				return this.view;
			}
		}

		protected static SortBy[] SortById
		{
			get
			{
				return DataAccessView.sortById;
			}
			set
			{
				DataAccessView.sortById = value;
			}
		}

		protected static int UidCacheSize
		{
			get
			{
				return DataAccessView.uidCacheSize;
			}
			set
			{
				DataAccessView.uidCacheSize = value;
			}
		}

		protected virtual PropertyDefinition[] AdditionalProperties
		{
			get
			{
				return null;
			}
		}

		protected virtual SortBy[] SortOrders
		{
			get
			{
				return DataAccessView.sortById;
			}
		}

		public void SetPoisonFlag(Folder folder, int imapId, bool isPoison)
		{
			StoreObjectId storeObjectId = this.GetStoreObjectId(imapId);
			if (storeObjectId != null)
			{
				try
				{
					folder.SetItemStatus(storeObjectId, isPoison ? MessageStatusFlags.MimeConversionFailed : MessageStatusFlags.None, MessageStatusFlags.MimeConversionFailed);
				}
				catch (LocalizedException)
				{
				}
			}
		}

		public StoreObjectId GetStoreObjectId(int imapId)
		{
			StoreObjectId storeObjectId = this.GetStoreObjectIdFromCache(imapId);
			if (storeObjectId != null)
			{
				return storeObjectId;
			}
			lock (this.factory.Store)
			{
				bool flag2 = false;
				try
				{
					bool flag3 = this.factory.Store.IsDead;
					if (flag3)
					{
						this.HandleInvalidateStore();
					}
					if (!this.factory.IsStoreConnected)
					{
						flag3 = this.factory.ConnectToTheStore();
						flag2 = true;
						if (flag3)
						{
							this.HandleInvalidateStore();
						}
					}
					storeObjectId = this.GetStoreObjectIdFromBE(imapId);
				}
				finally
				{
					if (flag2)
					{
						this.factory.DisconnectFromTheStore();
					}
				}
			}
			return storeObjectId;
		}

		public StoreObjectId[] GetStoreObjectIds(IList<ProtocolMessage> sortedMessages)
		{
			int[] array = new int[sortedMessages.Count];
			StoreObjectId[] array2 = new StoreObjectId[sortedMessages.Count];
			bool flag = false;
			for (int i = 0; i < sortedMessages.Count; i++)
			{
				ProtocolMessage protocolMessage = sortedMessages[i];
				array[i] = protocolMessage.Id;
				if (!flag)
				{
					StoreObjectId storeObjectIdFromCache = this.GetStoreObjectIdFromCache(protocolMessage.Id);
					if (storeObjectIdFromCache == null)
					{
						flag = true;
					}
					else
					{
						array2[i] = storeObjectIdFromCache;
					}
				}
			}
			if (flag)
			{
				lock (this.factory.Store)
				{
					bool flag3 = false;
					try
					{
						bool flag4 = this.factory.Store.IsDead;
						if (flag4)
						{
							this.HandleInvalidateStore();
						}
						if (!this.factory.IsStoreConnected)
						{
							flag4 = this.factory.ConnectToTheStore();
							flag3 = true;
							if (flag4)
							{
								this.HandleInvalidateStore();
							}
						}
						this.GetStoreObjectIdsFromBE(array, array2);
					}
					finally
					{
						if (flag3)
						{
							this.factory.DisconnectFromTheStore();
						}
					}
				}
			}
			for (int j = 0; j < array2.Length; j++)
			{
				if (array2[j] == null)
				{
					sortedMessages[j].IsDeleted = true;
				}
			}
			return array2;
		}

		protected void AddStoreObjectIdToCache(StoreObjectId storeObjectId, int imapId)
		{
			if (this.uidCache.Count >= DataAccessView.uidCacheSize)
			{
				this.uidCache.Dequeue();
			}
			this.uidCache.Enqueue(new DataAccessView.UidPair(storeObjectId, imapId));
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (this.view != null)
			{
				try
				{
					this.view.Dispose();
				}
				catch (LocalizedException ex)
				{
					ProtocolBaseServices.SessionTracer.TraceDebug<string>(this.factory.Session.SessionId, "Exception caught while disposing DataAccessView. {0}", ex.ToString());
				}
				this.view = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<DataAccessView>(this);
		}

		private static int GetUidCacheSizeFromConfig()
		{
			int num;
			if (!int.TryParse(ConfigurationManager.AppSettings["UidCacheSize"], out num) || num <= 0)
			{
				num = 30;
			}
			return num;
		}

		private void HandleInvalidateStore()
		{
			this.factory.NeedToReloadStoreStates = true;
			throw new StorageTransientException(new LocalizedString("Current MailboxSession is dead"));
		}

		private StoreObjectId GetStoreObjectIdFromCache(int imapId)
		{
			foreach (DataAccessView.UidPair uidPair in this.uidCache)
			{
				if (uidPair.ImapId == imapId)
				{
					return uidPair.StoreObjectId;
				}
			}
			return null;
		}

		private StoreObjectId GetStoreObjectIdFromBE(int imapId)
		{
			StoreObjectId result = null;
			ComparisonFilter seekFilter = new ComparisonFilter(ComparisonOperator.LessThanOrEqual, ItemSchema.ImapId, imapId + DataAccessView.uidCacheSize / 2);
			this.view.SeekToCondition(SeekReference.OriginBeginning, seekFilter);
			int num = DataAccessView.uidCacheSize;
			object[][] rows;
			do
			{
				rows = this.view.GetRows(num);
				for (int i = 0; i < rows.Length; i++)
				{
					int num2 = (int)rows[i][1];
					StoreObjectId objectId = ((VersionedId)rows[i][0]).ObjectId;
					if (num2 == imapId)
					{
						result = ((VersionedId)rows[i][0]).ObjectId;
					}
					this.AddStoreObjectIdToCache(objectId, num2);
				}
				num -= rows.Length;
			}
			while (num > 0 && rows.Length > 0);
			return result;
		}

		private void GetStoreObjectIdsFromBE(int[] sortedImapIds, StoreObjectId[] returnList)
		{
			int num = sortedImapIds[0] - DataAccessView.uidCacheSize / 2;
			int num2 = sortedImapIds[sortedImapIds.Length - 1] + DataAccessView.uidCacheSize / 2;
			int rowCount = (sortedImapIds.Length + DataAccessView.uidCacheSize < 10000) ? (sortedImapIds.Length + DataAccessView.uidCacheSize) : 10000;
			bool flag = true;
			int num3 = sortedImapIds.Length - 1;
			ComparisonFilter seekFilter = new ComparisonFilter(ComparisonOperator.LessThanOrEqual, ItemSchema.ImapId, num2);
			this.view.SeekToCondition(SeekReference.OriginBeginning, seekFilter);
			object[][] rows;
			do
			{
				rows = this.view.GetRows(rowCount);
				for (int i = 0; i < rows.Length; i++)
				{
					int num4 = (int)rows[i][1];
					StoreObjectId objectId = ((VersionedId)rows[i][0]).ObjectId;
					this.AddStoreObjectIdToCache(objectId, num4);
					if (num3 >= 0 && num4 == sortedImapIds[num3])
					{
						returnList[num3] = objectId;
						num3--;
					}
					if (num4 <= num)
					{
						flag = false;
						break;
					}
				}
			}
			while (rows.Length > 0 && flag);
		}

		protected static readonly PropertyDefinition[] ViewProperties = new PropertyDefinition[]
		{
			ItemSchema.Id,
			ItemSchema.ImapId
		};

		private static SortBy[] sortById = new SortBy[]
		{
			new SortBy(ItemSchema.ImapId, SortOrder.Descending)
		};

		private static int uidCacheSize = DataAccessView.GetUidCacheSizeFromConfig();

		private QueryResult view;

		private ResponseFactory factory;

		private Queue<DataAccessView.UidPair> uidCache;

		protected internal struct ViewPropertyIndex
		{
			public const int VersionedId = 0;

			public const int ImapId = 1;

			public const int MaxIndex = 1;
		}

		private struct UidPair
		{
			public UidPair(StoreObjectId storeObjectId, int imapId)
			{
				this.StoreObjectId = storeObjectId;
				this.ImapId = imapId;
			}

			public StoreObjectId StoreObjectId;

			public int ImapId;
		}
	}
}
