using System;
using System.Threading;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class ThreadPoolThreadCount
	{
		public int MinWorkerThreads { get; private set; }

		public int MinCompletionPortThreads { get; private set; }

		public int MaxWorkerThreads { get; private set; }

		public int MaxCompletionPortThreads { get; private set; }

		public ThreadPoolThreadCount()
		{
			int minWorkerThreads;
			int minCompletionPortThreads;
			int maxWorkerThreads;
			int maxCompletionPortThreads;
			this.GetMinMaxThreads(out minWorkerThreads, out minCompletionPortThreads, out maxWorkerThreads, out maxCompletionPortThreads);
			this.MinWorkerThreads = minWorkerThreads;
			this.MinCompletionPortThreads = minCompletionPortThreads;
			this.MaxWorkerThreads = maxWorkerThreads;
			this.MaxCompletionPortThreads = maxCompletionPortThreads;
		}

		public ThreadPoolThreadCount(int minw, int minc, int maxw, int maxc)
		{
			this.MinWorkerThreads = minw;
			this.MinCompletionPortThreads = minc;
			this.MaxWorkerThreads = maxw;
			this.MaxCompletionPortThreads = maxc;
		}

		private void GetMinMaxThreads(out int minWorkerThreads, out int minCompletionPortThreads, out int maxWorkerThreads, out int maxCompletionPortThreads)
		{
			ThreadPool.GetMaxThreads(out maxWorkerThreads, out maxCompletionPortThreads);
			ThreadPool.GetMinThreads(out minWorkerThreads, out minCompletionPortThreads);
		}

		public override string ToString()
		{
			return string.Format("[MinWorkerThreads={0},MinCompletionPortThreads={1},MaxWorkerThreads={2},MaxCompletionPortThreads={3}]", new object[]
			{
				this.MinWorkerThreads,
				this.MinCompletionPortThreads,
				this.MaxWorkerThreads,
				this.MaxCompletionPortThreads
			});
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			ThreadPoolThreadCount threadPoolThreadCount = obj as ThreadPoolThreadCount;
			return !(threadPoolThreadCount == null) && this.Equals(threadPoolThreadCount);
		}

		public bool Equals(ThreadPoolThreadCount other)
		{
			return other != null && (this.MinWorkerThreads == other.MinWorkerThreads && this.MinCompletionPortThreads == other.MinCompletionPortThreads && this.MaxWorkerThreads == other.MaxWorkerThreads) && this.MaxCompletionPortThreads == other.MaxCompletionPortThreads;
		}

		public static bool operator ==(ThreadPoolThreadCount tc1, ThreadPoolThreadCount tc2)
		{
			return object.ReferenceEquals(tc1, tc2) || (tc1 != null && tc2 != null && tc1.Equals(tc2));
		}

		public static bool operator !=(ThreadPoolThreadCount tc1, ThreadPoolThreadCount tc2)
		{
			return !(tc1 == tc2);
		}
	}
}
