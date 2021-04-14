using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal class BreadcrumbBuffer
	{
		internal BreadcrumbBuffer(int maxEntries)
		{
			this.maxEntries = maxEntries;
		}

		internal void Add(Breadcrumb entry)
		{
			if (this.buffer == null)
			{
				this.buffer = new List<Breadcrumb>(1);
			}
			if (this.buffer.Count >= this.maxEntries)
			{
				this.buffer[this.firstEntry] = entry;
				this.firstEntry = (this.firstEntry + 1) % this.maxEntries;
				return;
			}
			this.buffer.Add(entry);
		}

		internal int Count
		{
			get
			{
				if (this.buffer != null)
				{
					return this.buffer.Count;
				}
				return 0;
			}
		}

		internal Breadcrumb this[int index]
		{
			get
			{
				if (this.buffer.Count == 0 || index < 0 || index >= this.buffer.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return this.buffer[(this.firstEntry + index) % this.maxEntries];
			}
		}

		private List<Breadcrumb> buffer;

		private int maxEntries;

		private int firstEntry;
	}
}
