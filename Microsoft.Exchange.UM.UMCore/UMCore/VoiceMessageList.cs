using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class VoiceMessageList
	{
		internal StoreObjectId CurrentMessageBeingRead { get; private set; }

		internal bool MessageListIsNull
		{
			get
			{
				return this.currentPagerList == null;
			}
		}

		internal void InitializeCurrentPagerList(MessageItemList itemList)
		{
			this.currentPagerList = itemList;
			this.currentPagerList.Start();
		}

		internal bool GetNextMessageToRead(bool readingSavedMessages)
		{
			bool flag = true;
			StoreObjectId next = this.readMessageList.GetNext();
			if (next == null)
			{
				while (this.currentPagerList.Next(!readingSavedMessages))
				{
					if (!this.ShouldWeLoopAndReadNextMessage(readingSavedMessages))
					{
						this.CurrentMessageBeingRead = this.currentPagerList.CurrentStoreObjectId;
						flag = false;
						goto IL_49;
					}
				}
				return false;
			}
			this.CurrentMessageBeingRead = next;
			IL_49:
			if (!flag)
			{
				this.readMessageList.AddToList(this.CurrentMessageBeingRead);
			}
			return true;
		}

		internal bool GetPreviousMessageToRead()
		{
			StoreObjectId previous = this.readMessageList.GetPrevious();
			if (previous == null)
			{
				return false;
			}
			this.CurrentMessageBeingRead = previous;
			return true;
		}

		internal void DeleteMessage(StoreObjectId element)
		{
			if (!this.readMessageList.CheckIfPresentAndIgnore(element))
			{
				this.currentPagerList.Ignore(element);
			}
		}

		internal void UnDeleteMessage(StoreObjectId element)
		{
			if (!this.readMessageList.CheckIfPresentAndUnIgnore(element))
			{
				this.currentPagerList.UnIgnore(element);
			}
		}

		private bool ShouldWeLoopAndReadNextMessage(bool readingSavedMessages)
		{
			bool flag = this.currentPagerList.SafeGetProperty<bool>(MessageItemSchema.IsRead, false);
			return (!readingSavedMessages && flag != readingSavedMessages) || (readingSavedMessages && (flag != readingSavedMessages || this.readMessageList.Contains(this.currentPagerList.CurrentStoreObjectId)));
		}

		private MessageItemList currentPagerList;

		private VoiceMessageList.ListOfMessagesReadSoFar readMessageList = new VoiceMessageList.ListOfMessagesReadSoFar();

		private class ListOfMessagesReadSoFar
		{
			internal int Count
			{
				get
				{
					return this.mylist.Count;
				}
			}

			internal bool Contains(StoreObjectId element)
			{
				return this.mylist.Contains(element);
			}

			internal void AddToList(StoreObjectId element)
			{
				this.mylist.Add(element);
				this.currentIndex++;
			}

			internal StoreObjectId GetNext()
			{
				while (this.currentIndex >= 0 && this.currentIndex < this.mylist.Count - 1)
				{
					StoreObjectId storeObjectId = this.mylist[++this.currentIndex];
					if (!this.ignoreTable.ContainsKey(storeObjectId))
					{
						return storeObjectId;
					}
				}
				return null;
			}

			internal StoreObjectId GetPrevious()
			{
				StoreObjectId storeObjectId = null;
				if (this.currentIndex <= 0)
				{
					return null;
				}
				int i = this.currentIndex;
				while (i > 0)
				{
					storeObjectId = this.mylist[--i];
					if (!this.ignoreTable.ContainsKey(storeObjectId))
					{
						this.currentIndex = i;
						break;
					}
					storeObjectId = null;
				}
				return storeObjectId;
			}

			internal bool CheckIfPresentAndIgnore(StoreObjectId element)
			{
				if (this.Contains(element))
				{
					this.ignoreTable.Add(element, true);
					return true;
				}
				return false;
			}

			internal bool CheckIfPresentAndUnIgnore(StoreObjectId element)
			{
				if (this.ignoreTable.ContainsKey(element))
				{
					this.ignoreTable.Remove(element);
					return true;
				}
				return false;
			}

			private List<StoreObjectId> mylist = new List<StoreObjectId>(100);

			private Dictionary<StoreObjectId, bool> ignoreTable = new Dictionary<StoreObjectId, bool>();

			private int currentIndex = -1;
		}
	}
}
