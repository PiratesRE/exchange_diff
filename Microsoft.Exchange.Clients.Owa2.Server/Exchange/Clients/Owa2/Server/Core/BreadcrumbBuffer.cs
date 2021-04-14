using System;
using System.Collections.Concurrent;
using System.Text;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class BreadcrumbBuffer
	{
		internal BreadcrumbBuffer(int maxEntries)
		{
			this.maxEntries = maxEntries;
			this.buffer = new ConcurrentQueue<Breadcrumb>();
			this.lockObj = new object();
		}

		public void DumpTo(StringBuilder sb)
		{
			lock (this.lockObj)
			{
				foreach (Breadcrumb breadcrumb in this.buffer)
				{
					sb.Append(breadcrumb.ToString());
				}
			}
		}

		internal void Add(Breadcrumb entry)
		{
			this.buffer.Enqueue(entry);
			if (this.buffer.Count > this.maxEntries)
			{
				lock (this.lockObj)
				{
					while (this.buffer.Count > this.maxEntries)
					{
						Breadcrumb breadcrumb = null;
						if (!this.buffer.TryDequeue(out breadcrumb))
						{
							break;
						}
					}
				}
			}
		}

		private readonly int maxEntries;

		private readonly ConcurrentQueue<Breadcrumb> buffer;

		private readonly object lockObj;
	}
}
