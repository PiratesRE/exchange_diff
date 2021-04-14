using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[Serializable]
	public sealed class RetryStrategy
	{
		public RetryStrategy(string retryStrategy = null)
		{
			if (string.IsNullOrEmpty(retryStrategy))
			{
				retryStrategy = "2:15;2:30;2:60;2:300;2:1200;2:3600";
			}
			string[] array = retryStrategy.ToLower().Split(new char[]
			{
				';'
			});
			foreach (string text in array)
			{
				string[] array3 = text.Split(new char[]
				{
					':'
				});
				if (array3.Length != 2)
				{
					throw new ArgumentException("invalid format for retryStrategy", "retryStrategy");
				}
				int num = int.Parse(array3[0].Trim());
				int num2 = int.Parse(array3[1].Trim());
				if (num < 0 || num2 < 0)
				{
					throw new ArgumentException("invalid format for retryStrategy", "retryStrategy");
				}
				if (num > 0)
				{
					int key = this.retryTable.Any<KeyValuePair<int, TimeSpan>>() ? (this.retryTable.Keys.Max() + num) : num;
					this.retryTable[key] = TimeSpan.FromSeconds((double)num2);
				}
			}
			if (!this.retryTable.Any<KeyValuePair<int, TimeSpan>>())
			{
				this.retryTable[0] = TimeSpan.Zero;
			}
			this.maxTryCount = this.retryTable.Keys.Max();
		}

		public bool CanRetry(int currentTryCount)
		{
			return currentTryCount <= this.maxTryCount && currentTryCount >= 0;
		}

		public TimeSpan GetRetryInterval(int currentTryCount)
		{
			if (!this.CanRetry(currentTryCount))
			{
				throw new ArgumentException("not retriable", "currentTryCount");
			}
			return this.retryTable[(from p in this.retryTable.Keys
			where p >= currentTryCount
			select p).Min()];
		}

		private const string DefaultRetryStrategy = "2:15;2:30;2:60;2:300;2:1200;2:3600";

		private readonly int maxTryCount;

		private readonly Dictionary<int, TimeSpan> retryTable = new Dictionary<int, TimeSpan>();
	}
}
