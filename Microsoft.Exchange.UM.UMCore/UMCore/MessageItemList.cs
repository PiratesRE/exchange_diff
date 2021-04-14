using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class MessageItemList
	{
		internal MessageItemList(UMSubscriber user, StoreObjectId folderId, MessageItemListSortType sortType, PropertyDefinition[] properties)
		{
			this.pager = MessageItemList.MessageItemPager.Create(sortType);
			this.idx = -1;
			this.user = user;
			this.folderId = folderId;
			this.ignoreList = new Dictionary<StoreObjectId, bool>();
			int num = 0;
			this.propIndexMap = new Dictionary<PropertyDefinition, int>();
			foreach (PropertyDefinition key in properties)
			{
				if (!this.propIndexMap.ContainsKey(key))
				{
					this.propIndexMap.Add(key, num++);
				}
			}
			foreach (PropertyDefinition key2 in this.pager.RequiredProperties)
			{
				if (!this.propIndexMap.ContainsKey(key2))
				{
					this.propIndexMap.Add(key2, num++);
				}
			}
			foreach (PropertyDefinition key3 in MessageItemList.myProps)
			{
				if (!this.propIndexMap.ContainsKey(key3))
				{
					this.propIndexMap.Add(key3, num++);
				}
			}
			this.properties = new PropertyDefinition[this.propIndexMap.Count];
			foreach (KeyValuePair<PropertyDefinition, int> keyValuePair in this.propIndexMap)
			{
				this.properties[keyValuePair.Value] = keyValuePair.Key;
			}
		}

		internal StoreObjectId CurrentStoreObjectId
		{
			get
			{
				this.CheckIsValid();
				return StoreId.GetStoreObjectId(this.SafeGetProperty<StoreId>(this.view[this.idx], ItemSchema.Id, null));
			}
		}

		internal int CurrentOffset
		{
			get
			{
				return this.idx;
			}
		}

		internal T SafeGetProperty<T>(PropertyDefinition prop, T defaultValue)
		{
			this.CheckIsValid();
			return this.SafeGetProperty<T>(this.view[this.idx], prop, defaultValue);
		}

		internal void Ignore(StoreObjectId id)
		{
			this.ignoreList[id] = true;
		}

		internal void UnIgnore(StoreObjectId id)
		{
			this.ignoreList.Remove(id);
		}

		internal void Seek(StoreId storeId)
		{
			this.idx = -1;
			for (int i = 0; i < this.view.Length; i++)
			{
				StoreObjectId storeObjectId = StoreId.GetStoreObjectId(this.SafeGetProperty<StoreId>(this.view[i], ItemSchema.Id, null));
				if (storeObjectId.Equals(storeId))
				{
					this.idx = i;
				}
			}
			this.CheckIsValid();
		}

		internal void Seek(int offset)
		{
			this.idx = offset;
			if (this.idx < -1 || (this.view != null && this.idx > this.view.Length))
			{
				throw new InvalidOperationException("InvalidIndex");
			}
		}

		internal void Start()
		{
			this.idx = -1;
		}

		internal void End()
		{
			this.idx = this.view.Length;
		}

		internal bool Next(bool unreadOnly)
		{
			StoreObjectId storeObjectId = null;
			if (this.view == null && this.PopulateItemView(unreadOnly) == 0)
			{
				return false;
			}
			if (this.idx >= this.view.Length)
			{
				storeObjectId = null;
			}
			else
			{
				while (++this.idx < this.view.Length || this.PopulateItemView(unreadOnly) > 0)
				{
					if (this.idx >= this.view.Length)
					{
						this.idx = this.view.Length;
						break;
					}
					bool flag = this.SafeGetProperty<bool>(this.view[this.idx], MessageItemSchema.IsRead, true);
					if (!this.ignoreList.ContainsKey(this.CurrentStoreObjectId) && (!unreadOnly || !flag))
					{
						storeObjectId = this.CurrentStoreObjectId;
						break;
					}
				}
			}
			return null != storeObjectId;
		}

		internal bool Previous()
		{
			StoreObjectId storeObjectId = null;
			if (this.view == null)
			{
				this.PopulateItemView(false);
			}
			if (0 >= this.idx)
			{
				storeObjectId = null;
			}
			else
			{
				while (--this.idx >= 0)
				{
					if (!this.ignoreList.ContainsKey(this.CurrentStoreObjectId))
					{
						storeObjectId = this.CurrentStoreObjectId;
						break;
					}
				}
			}
			return null != storeObjectId;
		}

		private T SafeGetProperty<T>(object[] row, PropertyDefinition prop, T defaultValue)
		{
			int num = -1;
			if (!this.propIndexMap.TryGetValue(prop, out num))
			{
				throw new InvalidOperationException();
			}
			object obj = row[num];
			if (obj == null || obj is PropertyError)
			{
				return defaultValue;
			}
			return (T)((object)obj);
		}

		private int PopulateItemView(bool unreadOnly)
		{
			if (this.endOfMessages)
			{
				return 0;
			}
			if (unreadOnly && this.view != null && this.numUnreadMessagesNotYetPaged == 0)
			{
				return 0;
			}
			int result;
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.user.CreateSessionLock())
			{
				using (Folder folder = Folder.Bind(mailboxSessionLock.Session, this.folderId, new PropertyDefinition[]
				{
					FolderSchema.UnreadCount
				}))
				{
					object[][] array = this.pager.NextPage(this, folder, null == this.view);
					if (0 < array.Length)
					{
						this.lastViewRow = array[array.Length - 1];
					}
					if (this.view == null)
					{
						this.view = array;
						this.numUnreadMessagesNotYetPaged = folder.GetValueOrDefault<int>(FolderSchema.UnreadCount, int.MaxValue);
					}
					else if (0 < array.Length)
					{
						object[][] array2 = new object[this.view.Length + array.Length][];
						for (int i = 0; i < this.view.Length; i++)
						{
							array2[i] = this.view[i];
						}
						for (int j = 0; j < array.Length; j++)
						{
							array2[this.view.Length + j] = array[j];
						}
						this.view = array2;
					}
					this.endOfMessages = (array.Length < MessageItemList.PageSize);
					int num = 0;
					while (this.numUnreadMessagesNotYetPaged > 0 && num < array.Length)
					{
						if (!this.SafeGetProperty<bool>(this.view[num], MessageItemSchema.IsRead, true))
						{
							this.numUnreadMessagesNotYetPaged--;
						}
						num++;
					}
					result = array.Length;
				}
			}
			return result;
		}

		private void CheckIsValid()
		{
			if (this.view == null || this.idx < 0 || this.idx >= this.view.Length)
			{
				throw new InvalidOperationException("InvalidIndex");
			}
		}

		internal static readonly int PageSize = 50;

		private static PropertyDefinition[] myProps = new PropertyDefinition[]
		{
			ItemSchema.Id,
			MessageItemSchema.IsRead
		};

		private UMSubscriber user;

		private StoreObjectId folderId;

		private PropertyDefinition[] properties;

		private int idx;

		private object[][] view;

		private Dictionary<StoreObjectId, bool> ignoreList;

		private bool endOfMessages;

		private int numUnreadMessagesNotYetPaged;

		private MessageItemList.MessageItemPager pager;

		private object[] lastViewRow;

		private Dictionary<PropertyDefinition, int> propIndexMap;

		private abstract class MessageItemPager
		{
			protected abstract SortBy[] Sort { get; }

			internal abstract PropertyDefinition[] RequiredProperties { get; }

			internal static MessageItemList.MessageItemPager Create(MessageItemListSortType sortType)
			{
				MessageItemList.MessageItemPager result;
				switch (sortType)
				{
				case MessageItemListSortType.LifoVoicemail:
					result = new MessageItemList.LifoVoicemailPager();
					break;
				case MessageItemListSortType.FifoVoicemail:
					result = new MessageItemList.FifoVoicemailPager();
					break;
				case MessageItemListSortType.Email:
					result = new MessageItemList.EmailPager();
					break;
				default:
					throw new InvalidOperationException();
				}
				return result;
			}

			internal virtual object[][] NextPage(MessageItemList list, Folder folder, bool isFirstPage)
			{
				object[][] rows;
				using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, this.Sort, list.properties))
				{
					if (!isFirstPage)
					{
						queryResult.SeekToCondition(SeekReference.OriginBeginning, this.BuildSeekFilter(list), SeekToConditionFlags.AllowExtendedFilters);
					}
					rows = queryResult.GetRows(MessageItemList.PageSize);
				}
				return rows;
			}

			protected abstract QueryFilter BuildSeekFilter(MessageItemList list);
		}

		private abstract class VoicemailPager : MessageItemList.MessageItemPager
		{
			internal override PropertyDefinition[] RequiredProperties
			{
				get
				{
					return MessageItemList.VoicemailPager.myProps;
				}
			}

			private static PropertyDefinition[] myProps = new PropertyDefinition[]
			{
				ItemSchema.Importance,
				ItemSchema.ReceivedTime
			};
		}

		private class FifoVoicemailPager : MessageItemList.VoicemailPager
		{
			protected override SortBy[] Sort
			{
				get
				{
					return MessageItemList.FifoVoicemailPager.sort;
				}
			}

			protected override QueryFilter BuildSeekFilter(MessageItemList list)
			{
				ExDateTime exDateTime = list.SafeGetProperty<ExDateTime>(list.lastViewRow, ItemSchema.ReceivedTime, ExDateTime.MaxValue);
				Importance importance = list.SafeGetProperty<Importance>(list.lastViewRow, ItemSchema.Importance, Importance.High);
				QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.GreaterThan, ItemSchema.ReceivedTime, exDateTime);
				QueryFilter queryFilter2 = new ComparisonFilter(ComparisonOperator.LessThanOrEqual, ItemSchema.Importance, importance);
				return new AndFilter(new QueryFilter[]
				{
					queryFilter,
					queryFilter2
				});
			}

			private static SortBy[] sort = new SortBy[]
			{
				new SortBy(ItemSchema.Importance, SortOrder.Descending),
				new SortBy(ItemSchema.ReceivedTime, SortOrder.Ascending)
			};
		}

		private class LifoVoicemailPager : MessageItemList.VoicemailPager
		{
			protected override SortBy[] Sort
			{
				get
				{
					return MessageItemList.LifoVoicemailPager.sort;
				}
			}

			protected override QueryFilter BuildSeekFilter(MessageItemList list)
			{
				ExDateTime exDateTime = list.SafeGetProperty<ExDateTime>(list.lastViewRow, ItemSchema.ReceivedTime, ExDateTime.MaxValue);
				Importance importance = list.SafeGetProperty<Importance>(list.lastViewRow, ItemSchema.Importance, Importance.High);
				QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.LessThan, ItemSchema.ReceivedTime, exDateTime);
				QueryFilter queryFilter2 = new ComparisonFilter(ComparisonOperator.LessThanOrEqual, ItemSchema.Importance, importance);
				return new AndFilter(new QueryFilter[]
				{
					queryFilter,
					queryFilter2
				});
			}

			private static SortBy[] sort = new SortBy[]
			{
				new SortBy(ItemSchema.Importance, SortOrder.Descending),
				new SortBy(ItemSchema.ReceivedTime, SortOrder.Descending)
			};
		}

		private class EmailPager : MessageItemList.MessageItemPager
		{
			internal override PropertyDefinition[] RequiredProperties
			{
				get
				{
					return MessageItemList.EmailPager.myProps;
				}
			}

			protected override SortBy[] Sort
			{
				get
				{
					return MessageItemList.EmailPager.sort;
				}
			}

			protected override QueryFilter BuildSeekFilter(MessageItemList list)
			{
				ExDateTime exDateTime = list.SafeGetProperty<ExDateTime>(list.lastViewRow, ItemSchema.ReceivedTime, ExDateTime.MaxValue);
				return new ComparisonFilter(ComparisonOperator.LessThan, ItemSchema.ReceivedTime, exDateTime);
			}

			private static PropertyDefinition[] myProps = new PropertyDefinition[]
			{
				ItemSchema.ReceivedTime
			};

			private static SortBy[] sort = new SortBy[]
			{
				new SortBy(ItemSchema.ReceivedTime, SortOrder.Descending)
			};
		}
	}
}
