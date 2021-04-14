using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class LookupTable<TData> where TData : class
	{
		public LookupTable()
		{
			this.indexes = new List<LookupTable<TData>.IIndex>();
		}

		public void RegisterIndex(LookupTable<TData>.IIndex index)
		{
			this.indexes.Add(index);
			index.SetOwner(this);
		}

		public void Clear()
		{
			foreach (LookupTable<TData>.IIndex index in this.indexes)
			{
				index.Clear();
			}
		}

		public void LookupUnresolvedKeys()
		{
			foreach (LookupTable<TData>.IIndex index in this.indexes)
			{
				index.LookupUnresolvedKeys();
			}
		}

		public void InsertObject(TData data)
		{
			foreach (LookupTable<TData>.IIndex index in this.indexes)
			{
				index.InsertObject(data);
			}
		}

		private List<LookupTable<TData>.IIndex> indexes;

		internal interface IIndex
		{
			void SetOwner(LookupTable<TData> owner);

			void InsertObject(TData data);

			void LookupUnresolvedKeys();

			void Clear();
		}
	}
}
