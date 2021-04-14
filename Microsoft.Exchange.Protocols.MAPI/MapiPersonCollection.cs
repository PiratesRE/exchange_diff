using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.Mapi;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public sealed class MapiPersonCollection : DisposableBase, IList<MapiPerson>, ICollection<MapiPerson>, IEnumerable<MapiPerson>, IEnumerable
	{
		internal MapiPersonCollection(MapiMessage message)
		{
			this.parentObject = message;
			this.people = new List<MapiPerson>();
			this.initialized = false;
			this.extraPropList = new List<StorePropTag>();
		}

		internal MapiPerson this[int index]
		{
			get
			{
				this.ThrowIfNotValid();
				this.Load();
				return this.people[index];
			}
		}

		[Conditional("DEBUG")]
		private void AssertConsistency()
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MapiPersonCollection>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.people != null)
			{
				foreach (MapiPerson mapiPerson in this.people)
				{
					mapiPerson.Dispose();
				}
				this.people = null;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			this.ThrowIfNotValid();
			this.Load();
			return this.people.GetEnumerator();
		}

		public int GetCount()
		{
			this.ThrowIfNotValid();
			this.Load();
			return this.people.Count;
		}

		public int GetAliveCount()
		{
			this.ThrowIfNotValid();
			this.Load();
			int num = 0;
			foreach (MapiPerson mapiPerson in this.people)
			{
				if (!mapiPerson.IsDeleted)
				{
					num++;
				}
			}
			return num;
		}

		public void DeleteAll()
		{
			this.ThrowIfNotValid();
			this.Load();
			foreach (MapiPerson mapiPerson in this.people)
			{
				if (!mapiPerson.IsDeleted)
				{
					mapiPerson.Delete();
				}
			}
			this.extraPropList.Clear();
		}

		public void SaveChanges(MapiContext context)
		{
			bool flag = true;
			this.ThrowIfNotValid();
			for (int i = 0; i < this.people.Count; i++)
			{
				MapiPerson mapiPerson = this.people[i];
				mapiPerson.SaveChanges(context);
				if (!mapiPerson.IsDeleted)
				{
					flag = false;
				}
			}
			if (flag)
			{
				this.extraPropList.Clear();
			}
		}

		public MapiPerson GetItem(int rowId, bool createIfNotFound)
		{
			this.ThrowIfNotValid();
			this.Load();
			int num = this.IndexFromRowId(rowId);
			if (num >= 0 && (!this.people[num].IsDeleted || !createIfNotFound))
			{
				return this.people[num];
			}
			if (!createIfNotFound)
			{
				return null;
			}
			MapiPerson mapiPerson = new MapiPerson();
			MapiPerson result;
			try
			{
				Recipient storePerson = this.parentObject.CreateStorePerson(this.parentObject.CurrentOperationContext, rowId);
				mapiPerson.Configure(this.parentObject, rowId, storePerson);
				if (num < 0)
				{
					num = ~num;
					this.people.Insert(num, mapiPerson);
				}
				else
				{
					this.people[num].Dispose();
					this.people[num] = mapiPerson;
				}
				MapiPerson mapiPerson2 = mapiPerson;
				mapiPerson = null;
				result = mapiPerson2;
			}
			finally
			{
				if (mapiPerson != null)
				{
					mapiPerson.Dispose();
				}
			}
			return result;
		}

		public void RemoveItem(int rowId)
		{
			this.ThrowIfNotValid();
			this.Load();
			MapiPerson item = this.GetItem(rowId, true);
			item.Delete();
		}

		public List<StorePropTag> GetRecipientPropListExtra()
		{
			this.ThrowIfNotValid();
			this.Load();
			return this.extraPropList;
		}

		public void SetRecipientPropListExtra(List<StorePropTag> newList)
		{
			this.ThrowIfNotValid();
			this.Load();
			if (!this.VerifyExtraPropTags(newList))
			{
				throw new ExExceptionMapiRpcFormat((LID)33592U, "Extra recipient properties are invalid.");
			}
			if (newList.Count > this.extraPropList.Count)
			{
				this.extraPropList = newList;
			}
		}

		private bool VerifyExtraPropTags(IList<StorePropTag> newList)
		{
			bool result = true;
			int num = 0;
			while (num < this.extraPropList.Count && num < newList.Count)
			{
				if (this.extraPropList[num] != newList[num])
				{
					result = false;
					break;
				}
				num++;
			}
			return result;
		}

		private int IndexFromRowId(int rowId)
		{
			int i = 0;
			int num = this.people.Count - 1;
			while (i <= num)
			{
				int num2 = i + (num - i) / 2;
				int rowId2 = this.people[num2].GetRowId();
				if (rowId2 == rowId)
				{
					return num2;
				}
				if (rowId2 < rowId)
				{
					i = num2 + 1;
				}
				else
				{
					num = num2 - 1;
				}
			}
			return ~i;
		}

		private int IndexOf(MapiPerson person)
		{
			return this.people.IndexOf(person);
		}

		internal RecipientCollection GetRecipientCollection()
		{
			return this.parentObject.StoreRecipientCollection(this.parentObject.CurrentOperationContext);
		}

		private void Load()
		{
			if (this.initialized)
			{
				return;
			}
			if (this.parentObject.Mid.Counter == 0UL && this.parentObject.Mid.Guid == Guid.Empty && !this.parentObject.StoreMessage.IsEmbedded)
			{
				return;
			}
			if (this.people.Count == 0)
			{
				RecipientCollection recipientCollection = this.GetRecipientCollection();
				if (recipientCollection != null)
				{
					HashSet<StorePropTag> hashSet = new HashSet<StorePropTag>();
					for (int i = 0; i < recipientCollection.Count; i++)
					{
						MapiPerson mapiPerson = new MapiPerson();
						mapiPerson.Configure(this.parentObject, i, recipientCollection[i]);
						this.people.Add(mapiPerson);
						mapiPerson.CollectExtraProperties(this.parentObject.CurrentOperationContext, hashSet);
					}
					hashSet.Remove(PropTag.Recipient.EntryIdSvrEid);
					hashSet.ExceptWith(MapiPerson.GetRecipientPropListStandard());
					this.extraPropList = hashSet.ToList<StorePropTag>();
				}
			}
			this.initialized = true;
		}

		private void ThrowIfNotValid()
		{
			if (this.parentObject == null || !this.parentObject.IsValid)
			{
				ExTraceGlobals.GeneralTracer.TraceError(0L, "Invalid MapiMessageBase for MapiPersonCollection.  Throwing ExExceptionInvalidParameter!");
				throw new ExExceptionInvalidParameter((LID)58168U, "The MapiMessageBase within the MapiRecipientCollection is null or invalid");
			}
		}

		IEnumerator<MapiPerson> IEnumerable<MapiPerson>.GetEnumerator()
		{
			this.Load();
			return this.people.GetEnumerator();
		}

		void ICollection<MapiPerson>.Add(MapiPerson item)
		{
			throw new NotImplementedException();
		}

		void ICollection<MapiPerson>.Clear()
		{
			throw new NotImplementedException();
		}

		bool ICollection<MapiPerson>.Contains(MapiPerson item)
		{
			this.ThrowIfNotValid();
			this.Load();
			return this.people.Contains(item);
		}

		void ICollection<MapiPerson>.CopyTo(MapiPerson[] array, int arrayIndex)
		{
			this.ThrowIfNotValid();
			this.Load();
			this.people.CopyTo(array, arrayIndex);
		}

		int ICollection<MapiPerson>.Count
		{
			get
			{
				return this.GetCount();
			}
		}

		bool ICollection<MapiPerson>.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		bool ICollection<MapiPerson>.Remove(MapiPerson item)
		{
			throw new NotImplementedException();
		}

		MapiPerson IList<MapiPerson>.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				throw new System.NotSupportedException();
			}
		}

		int IList<MapiPerson>.IndexOf(MapiPerson item)
		{
			return this.IndexOf(item);
		}

		void IList<MapiPerson>.Insert(int index, MapiPerson item)
		{
			throw new NotImplementedException("read only for now");
		}

		void IList<MapiPerson>.RemoveAt(int index)
		{
			throw new NotImplementedException("read only for now");
		}

		private MapiMessage parentObject;

		private List<MapiPerson> people;

		private bool initialized;

		private List<StorePropTag> extraPropList;
	}
}
