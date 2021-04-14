using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class AbstractDistributionList : AbstractContactBase, IDistributionList, IContactBase, IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable, IRecipientBaseCollection<DistributionListMember>, IList<DistributionListMember>, ICollection<DistributionListMember>, IEnumerable<DistributionListMember>, IEnumerable
	{
		public virtual DistributionListMember this[int index]
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual DistributionListMember this[RecipientId id]
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual bool IsReadOnly
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual int Count
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual void Add(DistributionListMember item)
		{
			throw new NotImplementedException();
		}

		public virtual DistributionListMember Add(Participant participant)
		{
			throw new NotImplementedException();
		}

		public virtual void Insert(int index, DistributionListMember item)
		{
			throw new NotImplementedException();
		}

		public virtual int IndexOf(DistributionListMember item)
		{
			throw new NotImplementedException();
		}

		public virtual void RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		public virtual bool Remove(IDistributionListMember item)
		{
			throw new NotImplementedException();
		}

		public virtual void Remove(RecipientId id)
		{
			throw new NotImplementedException();
		}

		public virtual bool Remove(DistributionListMember item)
		{
			throw new NotImplementedException();
		}

		public virtual void Clear()
		{
			throw new NotImplementedException();
		}

		public virtual bool Contains(DistributionListMember item)
		{
			throw new NotImplementedException();
		}

		public virtual void CopyTo(DistributionListMember[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public virtual IEnumerator<DistributionListMember> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public virtual IEnumerator<IDistributionListMember> IGetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public override void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
