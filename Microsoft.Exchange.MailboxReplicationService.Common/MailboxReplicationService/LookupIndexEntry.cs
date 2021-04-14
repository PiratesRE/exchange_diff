using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class LookupIndexEntry<TData> where TData : class
	{
		public LookupIndexEntry()
		{
			this.dataList = new List<TData>();
			this.isResolved = false;
		}

		public TData Data
		{
			get
			{
				if (this.dataList.Count == 1)
				{
					return this.dataList[0];
				}
				return default(TData);
			}
			set
			{
				if (!this.dataList.Contains(value))
				{
					this.dataList.Add(value);
				}
			}
		}

		public List<TData> DataList
		{
			get
			{
				return this.dataList;
			}
		}

		public bool IsResolved
		{
			get
			{
				return this.isResolved;
			}
			set
			{
				this.isResolved = value;
			}
		}

		private List<TData> dataList;

		private bool isResolved;
	}
}
