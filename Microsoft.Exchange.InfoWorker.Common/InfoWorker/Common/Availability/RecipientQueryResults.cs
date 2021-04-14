using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class RecipientQueryResults : IList<RecipientData>, ICollection<RecipientData>, IEnumerable<RecipientData>, IEnumerable
	{
		internal RecipientQueryResults(RecipientQuery recipientQuery, EmailAddress[] emailAddressArray)
		{
			this.recipientQuery = recipientQuery;
			this.emailAddressArray = emailAddressArray;
			this.recipientDataArray = new RecipientData[emailAddressArray.Length];
		}

		internal RecipientQuery RecipientQuery
		{
			get
			{
				return this.recipientQuery;
			}
		}

		public int IndexOf(RecipientData item)
		{
			if (this.recipientDataArray == null)
			{
				return -1;
			}
			return Array.IndexOf<RecipientData>(this.recipientDataArray, item);
		}

		public void Insert(int index, RecipientData item)
		{
			throw new NotSupportedException("RecipientQueryResults does not support insertion.");
		}

		public void RemoveAt(int index)
		{
			throw new NotSupportedException("RecipientQueryResults does not support removal.");
		}

		public RecipientData this[int index]
		{
			get
			{
				if (this.recipientDataArray[index] == null)
				{
					this.PopulateAtIndex(index);
				}
				return this.recipientDataArray[index];
			}
			set
			{
				throw new NotSupportedException("RecipientQueryResults is readonly and does not support assignment.");
			}
		}

		public void Add(RecipientData item)
		{
			throw new NotSupportedException("RecipientQueryResults does not support add.");
		}

		public void Clear()
		{
			throw new NotSupportedException("RecipientQueryResults does not support clear.");
		}

		public bool Contains(RecipientData item)
		{
			return -1 != this.IndexOf(item);
		}

		public void CopyTo(RecipientData[] array, int arrayIndex)
		{
			throw new NotSupportedException("RecipientQueryResults does not support copy.");
		}

		public int Count
		{
			get
			{
				return this.recipientDataArray.Length;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public bool Remove(RecipientData item)
		{
			throw new NotSupportedException("RecipientQueryResults does not support remove.");
		}

		public IEnumerator<RecipientData> GetEnumerator()
		{
			if (this.recipientDataArray == null)
			{
				return null;
			}
			return (IEnumerator<RecipientData>)this.recipientDataArray.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			if (this.recipientDataArray == null)
			{
				return null;
			}
			return this.recipientDataArray.GetEnumerator();
		}

		private void PopulateAtIndex(int index)
		{
			int num;
			IEnumerable<RecipientData> enumerable = this.recipientQuery.LookupRecipientsBatchAtIndex(this.emailAddressArray, index, out num);
			foreach (RecipientData recipientData in enumerable)
			{
				this.recipientDataArray[num++] = recipientData;
			}
		}

		private RecipientQuery recipientQuery;

		private EmailAddress[] emailAddressArray;

		private RecipientData[] recipientDataArray;
	}
}
