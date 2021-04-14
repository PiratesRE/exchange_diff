using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Core.Transcoding
{
	internal sealed class BlockList
	{
		public BlockList(int maxBlocklistSize, TimeSpan maxBlockTime)
		{
			this.maxBlocklistSize = maxBlocklistSize;
			this.maxBlockTime = maxBlockTime;
			this.blockList = new Dictionary<string, ExDateTime>();
			this.thisLock = new object();
		}

		public bool CheckItem(string docId)
		{
			if (string.IsNullOrEmpty(docId))
			{
				throw new ArgumentException("Input invalid document ID.", "docId");
			}
			bool result;
			lock (this.thisLock)
			{
				if (this.blockList.ContainsKey(docId))
				{
					if (ExDateTime.UtcNow - this.blockList[docId] > this.maxBlockTime)
					{
						this.blockList.Remove(docId);
						result = false;
					}
					else
					{
						result = true;
					}
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public int Count
		{
			get
			{
				int count;
				lock (this.thisLock)
				{
					count = this.blockList.Count;
				}
				return count;
			}
		}

		public void AddNew(string docId)
		{
			if (string.IsNullOrEmpty(docId))
			{
				throw new ArgumentException("Input invalid document ID", "docId");
			}
			ExTraceGlobals.TranscodingTracer.TraceDebug<string>((long)this.GetHashCode(), "Add document {0} to block list", docId);
			lock (this.thisLock)
			{
				if (this.blockList.Count >= this.maxBlocklistSize)
				{
					this.blockList.Clear();
				}
				if (!this.blockList.ContainsKey(docId))
				{
					this.blockList.Add(docId, ExDateTime.UtcNow);
				}
			}
		}

		private Dictionary<string, ExDateTime> blockList;

		private object thisLock;

		private TimeSpan maxBlockTime;

		private int maxBlocklistSize;
	}
}
