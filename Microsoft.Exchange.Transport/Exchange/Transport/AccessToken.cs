using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Transport
{
	internal class AccessToken
	{
		public AccessToken(WaitCondition condition, NextHopSolutionKey queue, WaitConditionManager issuingMap)
		{
			this.condition = condition;
			this.queue = queue;
			this.issuingMap = issuingMap;
			lock (AccessToken.validEpochs)
			{
				this.epoch = 0;
				if (!AccessToken.validEpochs.TryGetValue(condition, out this.epoch))
				{
					AccessToken.validEpochs[condition] = 0;
				}
			}
			lock (AccessToken.outstandingCount)
			{
				int num;
				if (!AccessToken.outstandingCount.TryGetValue(condition, out num))
				{
					AccessToken.outstandingCount[condition] = 1;
				}
				else
				{
					num = (AccessToken.outstandingCount[condition] = num + 1);
				}
			}
			this.valid = true;
		}

		public static void ReclaimAll(WaitCondition condition)
		{
			lock (AccessToken.validEpochs)
			{
				if (AccessToken.validEpochs.ContainsKey(condition))
				{
					Dictionary<WaitCondition, int> dictionary;
					(dictionary = AccessToken.validEpochs)[condition] = dictionary[condition] + 1;
				}
			}
			lock (AccessToken.outstandingCount)
			{
				AccessToken.outstandingCount.Remove(condition);
			}
		}

		internal static Dictionary<WaitCondition, int>.Enumerator OutstandingCountsEnumerator
		{
			get
			{
				return AccessToken.outstandingCount.GetEnumerator();
			}
		}

		public WaitCondition Condition
		{
			get
			{
				return this.condition;
			}
		}

		public bool Return(bool returnToIssuingMap)
		{
			if (this.valid)
			{
				this.valid = false;
				lock (AccessToken.outstandingCount)
				{
					int num;
					if (AccessToken.outstandingCount.TryGetValue(this.condition, out num))
					{
						num = (AccessToken.outstandingCount[this.condition] = num - 1);
						if (num == 0)
						{
							AccessToken.outstandingCount.Remove(this.condition);
						}
					}
				}
				if (returnToIssuingMap)
				{
					this.issuingMap.RevokeToken(this.condition, this.queue);
				}
				return true;
			}
			return false;
		}

		public bool Validate(WaitCondition accessFor)
		{
			if (this.valid && this.condition.Equals(accessFor))
			{
				lock (AccessToken.validEpochs)
				{
					if (AccessToken.validEpochs.ContainsKey(this.condition))
					{
						this.valid = (this.epoch >= AccessToken.validEpochs[this.condition]);
						return this.valid;
					}
				}
				return false;
			}
			return false;
		}

		internal static void CleanupUnusedCondition(WaitCondition condition)
		{
			lock (AccessToken.outstandingCount)
			{
				if (AccessToken.outstandingCount.ContainsKey(condition))
				{
					AccessToken.ReclaimAll(condition);
					return;
				}
			}
			lock (AccessToken.validEpochs)
			{
				AccessToken.validEpochs.Remove(condition);
			}
		}

		private static Dictionary<WaitCondition, int> validEpochs = new Dictionary<WaitCondition, int>();

		private static Dictionary<WaitCondition, int> outstandingCount = new Dictionary<WaitCondition, int>();

		private WaitCondition condition;

		private NextHopSolutionKey queue;

		private WaitConditionManager issuingMap;

		private int epoch;

		private bool valid;
	}
}
