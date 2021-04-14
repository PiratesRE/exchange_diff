using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.AttachmentBlob;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class SubobjectCollection
	{
		public SubobjectCollection(Item item)
		{
			this.item = item;
		}

		public SubobjectCollection(Item item, byte[] blob)
		{
			this.item = item;
			if (blob != null)
			{
				int count = AttachmentBlob.GetCount(blob, false);
				if (count != 0)
				{
					this.subobjects = new List<SubobjectCollection.SubobjectEntry>(count + 1);
					this.subobjects.AddRange(from kvp in AttachmentBlob.Deserialize(blob, false)
					select SubobjectCollection.SubobjectEntry.ConstructDeserialized(kvp));
					foreach (SubobjectCollection.SubobjectEntry subobjectEntry in this.subobjects)
					{
						this.SubobjectReferenceState.Addref(subobjectEntry.Inid);
						if (subobjectEntry.IsChild && subobjectEntry.ChildNumber >= this.nextChildNumber)
						{
							this.nextChildNumber = subobjectEntry.ChildNumber + 1;
						}
					}
					this.descendantCount = this.subobjects.Count;
				}
			}
		}

		public bool IsDirty
		{
			get
			{
				return this.dirty;
			}
		}

		public int ChildrenCount
		{
			get
			{
				int num = 0;
				if (this.subobjects != null)
				{
					foreach (SubobjectCollection.SubobjectEntry subobjectEntry in this.subobjects)
					{
						if (!subobjectEntry.IsDeleted && subobjectEntry.IsChild)
						{
							num++;
						}
					}
				}
				return num;
			}
		}

		public int DescendantCount
		{
			get
			{
				return this.descendantCount;
			}
		}

		private SubobjectReferenceState SubobjectReferenceState
		{
			get
			{
				return this.item.SubobjectReferenceState;
			}
		}

		public void ReleaseAll(bool calledFromFinalizer, Context context)
		{
			List<SubobjectCollection.SubobjectEntry> list = Interlocked.Exchange<List<SubobjectCollection.SubobjectEntry>>(ref this.subobjects, null);
			this.descendantCount = 0;
			if (list != null)
			{
				foreach (SubobjectCollection.SubobjectEntry subobjectEntry in list)
				{
					this.SubobjectReferenceState.Release(context, subobjectEntry.Inid, calledFromFinalizer ? null : this.item.Mailbox);
				}
			}
		}

		public void ClearDeleted(Context context)
		{
			if (this.subobjects != null)
			{
				int num = 0;
				for (int i = 0; i < this.subobjects.Count; i++)
				{
					if (!this.subobjects[i].IsDeleted)
					{
						if (num != i)
						{
							this.subobjects[num] = this.subobjects[i];
						}
						num++;
					}
					else
					{
						this.SubobjectReferenceState.Release(context, this.subobjects[i].Inid, this.item.Mailbox);
					}
				}
				if (num != this.subobjects.Count)
				{
					if (num != 0)
					{
						this.subobjects.RemoveRange(num, this.subobjects.Count - num);
						return;
					}
					this.subobjects = null;
				}
			}
		}

		public void Add(Context context, SubobjectCollection otherCollection)
		{
			if (otherCollection.subobjects != null)
			{
				foreach (SubobjectCollection.SubobjectEntry subobjectEntry in otherCollection.subobjects)
				{
					if (!subobjectEntry.IsDeleted)
					{
						int num = this.FindDescendant(subobjectEntry.Inid);
						if (num < 0)
						{
							this.CheckMaxDescendantCount();
							this.subobjects.Add(SubobjectCollection.SubobjectEntry.ConstructNew(subobjectEntry.Inid));
							this.SubobjectReferenceState.Addref(subobjectEntry.Inid);
							this.dirty = true;
							this.descendantCount++;
						}
						else if (this.subobjects[num].IsDeleted)
						{
							this.CheckMaxDescendantCount();
							this.subobjects[num] = SubobjectCollection.SubobjectEntry.ConstructResurected(this.subobjects[num]);
							this.dirty = true;
							this.descendantCount++;
						}
					}
				}
			}
		}

		public void Delete(Context context, SubobjectCollection otherCollection)
		{
			if (otherCollection.subobjects != null)
			{
				foreach (SubobjectCollection.SubobjectEntry subobjectEntry in otherCollection.subobjects)
				{
					if (!subobjectEntry.IsDeleted)
					{
						int num = this.FindDescendant(subobjectEntry.Inid);
						if (num >= 0 && !this.subobjects[num].IsDeleted)
						{
							long size = (subobjectEntry.Size != -1L) ? subobjectEntry.Size : this.subobjects[num].Size;
							this.subobjects[num] = SubobjectCollection.SubobjectEntry.ConstructDeleted(this.subobjects[num], size);
							this.dirty = true;
							this.descendantCount--;
						}
					}
				}
			}
		}

		public void DeleteDeleted(Context context, SubobjectCollection otherCollection)
		{
			if (otherCollection.subobjects != null)
			{
				foreach (SubobjectCollection.SubobjectEntry subobjectEntry in otherCollection.subobjects)
				{
					if (subobjectEntry.IsDeleted)
					{
						int num = this.FindDescendant(subobjectEntry.Inid);
						if (num >= 0 && !this.subobjects[num].IsDeleted)
						{
							this.subobjects[num] = SubobjectCollection.SubobjectEntry.ConstructDeleted(this.subobjects[num]);
							this.dirty = true;
							this.descendantCount--;
						}
					}
				}
			}
		}

		public void TombstoneAll(Context context)
		{
			if (this.subobjects != null)
			{
				foreach (SubobjectCollection.SubobjectEntry subobjectEntry in this.subobjects)
				{
					if (!subobjectEntry.IsDeleted && !subobjectEntry.IsNew)
					{
						SubobjectCleanup.AddTombstone(context, this.item, subobjectEntry.Inid, 0L);
					}
				}
			}
		}

		public void TombstoneDeleted(Context context)
		{
			if (this.subobjects != null)
			{
				foreach (SubobjectCollection.SubobjectEntry subobjectEntry in this.subobjects)
				{
					if (subobjectEntry.IsDeleted && !subobjectEntry.IsNew)
					{
						SubobjectCleanup.AddTombstone(context, this.item, subobjectEntry.Inid, (subobjectEntry.Size != -1L) ? subobjectEntry.Size : 0L);
					}
				}
			}
		}

		public void CommitAll(Context context)
		{
			if (this.subobjects != null)
			{
				for (int i = 0; i < this.subobjects.Count; i++)
				{
					if (!this.subobjects[i].IsDeleted && this.subobjects[i].IsNew)
					{
						SubobjectCleanup.RemoveTombstone(context, this.item, this.subobjects[i].Inid);
						this.subobjects[i] = SubobjectCollection.SubobjectEntry.ConstructCommitted(this.subobjects[i]);
					}
				}
			}
		}

		public long? GetChildInid(int childNumber)
		{
			int num = this.FindChild(childNumber);
			if (num >= 0)
			{
				return new long?(this.subobjects[num].Inid);
			}
			return null;
		}

		public long GetChildSize(int childNumber)
		{
			int num = this.FindChild(childNumber);
			if (num >= 0)
			{
				return this.subobjects[num].Size;
			}
			return 0L;
		}

		public void SetChildSize(int childNumber, long size)
		{
			int index = this.FindChild(childNumber);
			this.subobjects[index] = SubobjectCollection.SubobjectEntry.ConstructChildChangeSize(this.subobjects[index], size);
		}

		public IEnumerable<int> GetChildNumbers()
		{
			int childrenCount = this.ChildrenCount;
			if (childrenCount == 0)
			{
				return Enumerable.Empty<int>();
			}
			int[] array = new int[childrenCount];
			int num = 0;
			foreach (SubobjectCollection.SubobjectEntry subobjectEntry in this.subobjects)
			{
				if (!subobjectEntry.IsDeleted && subobjectEntry.IsChild)
				{
					array[num++] = subobjectEntry.ChildNumber;
				}
			}
			Array.Sort<int>(array);
			return array;
		}

		public bool ContainsChild(int childNumber)
		{
			return this.FindChild(childNumber) >= 0;
		}

		public void AddOrUpdateChild(Context context, int childNumber, long inid, long size)
		{
			int num = this.FindChild(childNumber);
			if (num < 0)
			{
				if (this.subobjects == null)
				{
					this.subobjects = new List<SubobjectCollection.SubobjectEntry>(1);
				}
				this.CheckMaxDescendantCount();
				this.subobjects.Add(SubobjectCollection.SubobjectEntry.ConstructNewChild(inid, childNumber, size));
				this.SubobjectReferenceState.Addref(inid);
				this.dirty = true;
				this.descendantCount++;
				return;
			}
			this.subobjects.Add(SubobjectCollection.SubobjectEntry.ConstructDeleted(this.subobjects[num]));
			this.subobjects[num] = SubobjectCollection.SubobjectEntry.ConstructNewChild(inid, childNumber, size);
			this.SubobjectReferenceState.Addref(inid);
			this.dirty = true;
		}

		public void DeleteChild(Context context, int childNumber, long childSize)
		{
			int index = this.FindChild(childNumber);
			this.subobjects[index] = SubobjectCollection.SubobjectEntry.ConstructDeleted(this.subobjects[index], childSize);
			this.descendantCount--;
			this.dirty = true;
		}

		public int ReserveChildNumber()
		{
			if (this.nextChildNumber == 2147483647)
			{
				throw new StoreException((LID)51192U, ErrorCodeValue.CallFailed, "child number overflow");
			}
			return this.nextChildNumber++;
		}

		public void ResetNextChildNumber()
		{
			this.nextChildNumber = 0;
		}

		public void SetDirty()
		{
			this.dirty = true;
		}

		public byte[] Serialize(bool renumberChildren, bool resetDirtyFlag)
		{
			if (this.subobjects == null)
			{
				if (resetDirtyFlag)
				{
					this.dirty = false;
				}
				return null;
			}
			byte[] result = AttachmentBlob.Serialize(from so in this.subobjects
			where !so.IsDeleted
			select new KeyValuePair<int, long>(so.IsChild ? so.ChildNumber : int.MinValue, so.Inid), renumberChildren);
			if (resetDirtyFlag)
			{
				this.dirty = false;
			}
			return result;
		}

		public byte[] SerializeChildren()
		{
			if (this.subobjects == null)
			{
				return null;
			}
			return AttachmentBlob.Serialize(from so in this.subobjects
			where !so.IsDeleted && so.IsChild
			select new KeyValuePair<int, long>(so.IsChild ? so.ChildNumber : int.MinValue, so.Inid), false);
		}

		[Conditional("DEBUG")]
		internal void AssertHasChild(long inid)
		{
			this.FindDescendant(inid);
		}

		[Conditional("DEBUG")]
		internal void AssertHasAllDescendants(SubobjectCollection otherCollection)
		{
			if (otherCollection != null && otherCollection.subobjects != null)
			{
				foreach (SubobjectCollection.SubobjectEntry subobjectEntry in otherCollection.subobjects)
				{
					if (!subobjectEntry.IsDeleted)
					{
						this.FindDescendant(subobjectEntry.Inid);
					}
				}
			}
		}

		private int FindChild(int childNumber)
		{
			int result = -1;
			if (this.subobjects != null)
			{
				for (int i = 0; i < this.subobjects.Count; i++)
				{
					if (!this.subobjects[i].IsDeleted && this.subobjects[i].IsChild && this.subobjects[i].ChildNumber == childNumber)
					{
						result = i;
						break;
					}
				}
			}
			return result;
		}

		private int FindDescendant(long inid)
		{
			int result = -1;
			if (this.subobjects != null)
			{
				for (int i = 0; i < this.subobjects.Count; i++)
				{
					if (this.subobjects[i].Inid == inid)
					{
						result = i;
						break;
					}
				}
			}
			return result;
		}

		private void CheckMaxDescendantCount()
		{
			if (this.DescendantCount >= AttachmentBlob.MaxSupportedDescendantCountWrite)
			{
				throw new StoreException((LID)63328U, ErrorCodeValue.MaxAttachmentExceeded);
			}
		}

		private readonly Item item;

		private List<SubobjectCollection.SubobjectEntry> subobjects;

		private bool dirty;

		private int nextChildNumber;

		private int descendantCount;

		private struct SubobjectEntry
		{
			private SubobjectEntry(long inid, bool isDeleted, bool isNew, bool isChild, int childNumber, long size)
			{
				this.inid = inid;
				this.isDeleted = isDeleted;
				this.isNew = isNew;
				this.isChild = isChild;
				this.childNumber = childNumber;
				this.size = size;
			}

			public long Inid
			{
				get
				{
					return this.inid;
				}
			}

			public int ChildNumber
			{
				get
				{
					return this.childNumber;
				}
			}

			public long Size
			{
				get
				{
					return this.size;
				}
			}

			public bool IsDeleted
			{
				get
				{
					return this.isDeleted;
				}
			}

			public bool IsChild
			{
				get
				{
					return this.isChild;
				}
			}

			public bool IsNew
			{
				get
				{
					return this.isNew;
				}
			}

			public static SubobjectCollection.SubobjectEntry ConstructDeserialized(KeyValuePair<int, long> deserializedPair)
			{
				return new SubobjectCollection.SubobjectEntry(deserializedPair.Value, false, false, deserializedPair.Key >= 0, deserializedPair.Key, -1L);
			}

			public static SubobjectCollection.SubobjectEntry ConstructNew(long inid)
			{
				return new SubobjectCollection.SubobjectEntry(inid, false, true, false, -1, -1L);
			}

			public static SubobjectCollection.SubobjectEntry ConstructNewChild(long inid, int childNumber, long size)
			{
				return new SubobjectCollection.SubobjectEntry(inid, false, true, true, childNumber, size);
			}

			public static SubobjectCollection.SubobjectEntry ConstructDeleted(SubobjectCollection.SubobjectEntry originalSubobjectEntry)
			{
				return new SubobjectCollection.SubobjectEntry(originalSubobjectEntry.Inid, true, originalSubobjectEntry.IsNew, originalSubobjectEntry.IsChild, originalSubobjectEntry.ChildNumber, originalSubobjectEntry.Size);
			}

			public static SubobjectCollection.SubobjectEntry ConstructDeleted(SubobjectCollection.SubobjectEntry originalSubobjectEntry, long size)
			{
				return new SubobjectCollection.SubobjectEntry(originalSubobjectEntry.Inid, true, originalSubobjectEntry.IsNew, originalSubobjectEntry.IsChild, originalSubobjectEntry.ChildNumber, size);
			}

			public static SubobjectCollection.SubobjectEntry ConstructResurected(SubobjectCollection.SubobjectEntry originalSubobjectEntry)
			{
				return new SubobjectCollection.SubobjectEntry(originalSubobjectEntry.Inid, false, originalSubobjectEntry.IsNew, originalSubobjectEntry.IsChild, originalSubobjectEntry.ChildNumber, originalSubobjectEntry.Size);
			}

			public static SubobjectCollection.SubobjectEntry ConstructChildChangeSize(SubobjectCollection.SubobjectEntry originalSubobjectEntry, long newSize)
			{
				return new SubobjectCollection.SubobjectEntry(originalSubobjectEntry.Inid, originalSubobjectEntry.IsDeleted, originalSubobjectEntry.IsNew, originalSubobjectEntry.IsChild, originalSubobjectEntry.ChildNumber, newSize);
			}

			public static SubobjectCollection.SubobjectEntry ConstructCommitted(SubobjectCollection.SubobjectEntry originalSubobjectEntry)
			{
				return new SubobjectCollection.SubobjectEntry(originalSubobjectEntry.Inid, originalSubobjectEntry.IsDeleted, false, originalSubobjectEntry.IsChild, originalSubobjectEntry.ChildNumber, originalSubobjectEntry.Size);
			}

			private long inid;

			private bool isDeleted;

			private bool isNew;

			private bool isChild;

			private int childNumber;

			private long size;
		}
	}
}
