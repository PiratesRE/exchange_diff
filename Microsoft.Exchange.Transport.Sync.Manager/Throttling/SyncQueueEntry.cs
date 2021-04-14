using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Sync.Common;

namespace Microsoft.Exchange.Transport.Sync.Manager.Throttling
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SyncQueueEntry<T> : IComparable<SyncQueueEntry<T>>
	{
		public SyncQueueEntry(T item, ExDateTime nextPollingTime)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			this.item = item;
			this.nextPollingTime = nextPollingTime;
		}

		public T Item
		{
			get
			{
				return this.item;
			}
		}

		public ExDateTime NextPollingTime
		{
			get
			{
				return this.nextPollingTime;
			}
		}

		public int CompareTo(SyncQueueEntry<T> syncQueueEntry)
		{
			SyncUtilities.ThrowIfArgumentNull("syncQueueEntry", syncQueueEntry);
			return this.NextPollingTime.UtcTicks.CompareTo(syncQueueEntry.NextPollingTime.UtcTicks);
		}

		internal void AddDiagnosticInfoTo(XElement parentElement, string itemName)
		{
			XName name = itemName;
			T t = this.item;
			parentElement.Add(new XElement(name, t.ToString()));
			parentElement.Add(new XElement("nextPollingTime", this.nextPollingTime.ToString("o")));
		}

		private readonly T item;

		private readonly ExDateTime nextPollingTime;
	}
}
