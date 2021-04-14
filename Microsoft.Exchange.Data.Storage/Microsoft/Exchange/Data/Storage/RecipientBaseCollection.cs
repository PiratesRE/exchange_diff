using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class RecipientBaseCollection<ITEM_TYPE> : IRecipientBaseCollection<ITEM_TYPE>, IList<ITEM_TYPE>, ICollection<ITEM_TYPE>, IEnumerable<!0>, IEnumerable where ITEM_TYPE : RecipientBase
	{
		internal RecipientBaseCollection(CoreRecipientCollection coreRecipientCollection)
		{
			StorageGlobals.TraceConstructIDisposable(this);
			this.coreRecipientCollection = coreRecipientCollection;
		}

		public ITEM_TYPE this[RecipientId id]
		{
			get
			{
				foreach (CoreRecipient coreRecipient in this.coreRecipientCollection)
				{
					if (coreRecipient.Id.Equals(id))
					{
						return this.ConstructStronglyTypedRecipient(coreRecipient);
					}
				}
				return default(ITEM_TYPE);
			}
		}

		protected abstract ITEM_TYPE ConstructStronglyTypedRecipient(CoreRecipient coreRecipient);

		public abstract ITEM_TYPE Add(Participant participant);

		public virtual void Remove(RecipientId id)
		{
			int num = this.coreRecipientCollection.IndexOf(id);
			if (num != -1)
			{
				this.RemoveAt(num);
			}
		}

		public ITEM_TYPE this[int index]
		{
			get
			{
				return this.ConstructStronglyTypedRecipient(this.coreRecipientCollection.GetCoreRecipient(index));
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public virtual void Clear()
		{
			for (int i = this.coreRecipientCollection.Count - 1; i >= 0; i--)
			{
				this.RemoveAt(i);
			}
		}

		public int IndexOf(ITEM_TYPE item)
		{
			return this.coreRecipientCollection.IndexOf((item != null) ? item.CoreRecipient : null);
		}

		public void Insert(int index, ITEM_TYPE item)
		{
			throw new NotSupportedException();
		}

		public virtual void RemoveAt(int index)
		{
			this.coreRecipientCollection.RemoveAt(index);
			this.OnRemoveRecipient();
		}

		protected virtual void OnRemoveRecipient()
		{
		}

		public abstract void Add(ITEM_TYPE item);

		public bool Contains(ITEM_TYPE item)
		{
			return this.coreRecipientCollection.Contains(item.CoreRecipient);
		}

		public void CopyTo(ITEM_TYPE[] array, int arrayIndex)
		{
			int num = 0;
			foreach (CoreRecipient coreRecipient in this.coreRecipientCollection)
			{
				array[arrayIndex + num++] = this.ConstructStronglyTypedRecipient(coreRecipient);
			}
		}

		public int Count
		{
			get
			{
				return this.coreRecipientCollection.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool Remove(ITEM_TYPE item)
		{
			int num = this.IndexOf(item);
			if (num != -1)
			{
				this.RemoveAt(num);
			}
			return num != -1;
		}

		public IEnumerator<ITEM_TYPE> GetEnumerator()
		{
			foreach (CoreRecipient coreRecipient in this.coreRecipientCollection)
			{
				yield return this.ConstructStronglyTypedRecipient(coreRecipient);
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		internal bool IsDirty
		{
			get
			{
				return this.coreRecipientCollection.IsDirty;
			}
		}

		public void LoadAdditionalParticipantProperties(params PropertyDefinition[] keyProperties)
		{
			this.coreRecipientCollection.LoadAdditionalParticipantProperties(keyProperties);
		}

		internal ICoreItem CoreItem
		{
			get
			{
				return this.coreRecipientCollection.CoreItem;
			}
		}

		internal void CopyTo(RecipientBaseCollection<ITEM_TYPE> target)
		{
			foreach (ITEM_TYPE item in this)
			{
				target.Add(item);
			}
		}

		protected CoreRecipientCollection CoreRecipientCollection
		{
			get
			{
				return this.coreRecipientCollection;
			}
		}

		protected CoreRecipient CreateCoreRecipient(CoreRecipient.SetDefaultPropertiesDelegate setdefaultRecipientPropertiesDelegate, Participant participant)
		{
			if (participant == null)
			{
				throw new ArgumentNullException("participant");
			}
			CoreRecipient sourceCoreRecipient = null;
			CoreRecipient result;
			if (this.coreRecipientCollection.FindRemovedRecipient(participant, out sourceCoreRecipient))
			{
				result = this.CoreRecipientCollection.CreateCoreRecipient(sourceCoreRecipient);
			}
			else
			{
				result = this.CoreRecipientCollection.CreateCoreRecipient(setdefaultRecipientPropertiesDelegate, participant);
			}
			return result;
		}

		private readonly CoreRecipientCollection coreRecipientCollection;
	}
}
