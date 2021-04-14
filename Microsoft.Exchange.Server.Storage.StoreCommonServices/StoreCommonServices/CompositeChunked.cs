using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class CompositeChunked : IChunked
	{
		public CompositeChunked(IList<Func<Context, IChunked>> workQueue)
		{
			this.workQueue = workQueue;
		}

		internal IList<Func<Context, IChunked>> WorkQueueForTest
		{
			get
			{
				return this.workQueue;
			}
		}

		public bool DoChunk(Context context)
		{
			if (this.currentWork == null)
			{
				while (this.currentIndex < this.workQueue.Count)
				{
					this.currentWork = this.workQueue[this.currentIndex](context);
					if (this.currentWork != null)
					{
						break;
					}
					this.currentIndex++;
				}
				if (this.currentWork == null)
				{
					return true;
				}
			}
			if (this.currentWork.DoChunk(context))
			{
				this.currentWork.Dispose(context);
				this.currentWork = null;
				this.currentIndex++;
			}
			return this.currentIndex >= this.workQueue.Count;
		}

		public void Dispose(Context context)
		{
			if (this.currentWork != null)
			{
				this.currentWork.Dispose(context);
			}
		}

		public bool MustYield
		{
			get
			{
				return this.currentWork != null && this.currentWork.MustYield;
			}
		}

		private readonly IList<Func<Context, IChunked>> workQueue;

		private IChunked currentWork;

		private int currentIndex;
	}
}
