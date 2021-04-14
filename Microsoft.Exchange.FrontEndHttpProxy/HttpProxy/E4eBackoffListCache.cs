using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.HttpProxy
{
	internal sealed class E4eBackoffListCache
	{
		private E4eBackoffListCache()
		{
			this.senderBackoffListCache = new ExactTimeoutCache<string, DateTime>(null, null, null, 10240, false);
			this.recipientsBackoffListCache = new ExactTimeoutCache<string, DateTime>(null, null, null, 10240, false);
		}

		public static E4eBackoffListCache Instance
		{
			get
			{
				return E4eBackoffListCache.instance;
			}
		}

		public void UpdateCache(string budgetType, string emailAddress, string backoffUntilUtcStr)
		{
			if (string.IsNullOrEmpty(budgetType) || string.IsNullOrEmpty(emailAddress) || string.IsNullOrEmpty(backoffUntilUtcStr))
			{
				return;
			}
			if (!SmtpAddress.IsValidSmtpAddress(emailAddress))
			{
				return;
			}
			BudgetType budgetType2;
			try
			{
				if (!Enum.TryParse<BudgetType>(budgetType, true, out budgetType2))
				{
					return;
				}
			}
			catch (ArgumentException)
			{
				return;
			}
			DateTime dateTime;
			bool flag = DateTime.TryParse(backoffUntilUtcStr, out dateTime);
			if (flag && !(dateTime <= DateTime.UtcNow))
			{
				TimeSpan absoluteLiveTime = (dateTime == DateTime.MaxValue) ? TimeSpan.MaxValue : (dateTime - DateTime.UtcNow);
				if (absoluteLiveTime.TotalMilliseconds <= 0.0)
				{
					return;
				}
				if (budgetType2 == BudgetType.E4eSender)
				{
					this.senderBackoffListCache.TryInsertAbsolute(emailAddress, dateTime, absoluteLiveTime);
					return;
				}
				if (budgetType2 == BudgetType.E4eRecipient)
				{
					this.recipientsBackoffListCache.TryInsertAbsolute(emailAddress, dateTime, absoluteLiveTime);
				}
				return;
			}
		}

		public bool ShouldBackOff(string senderEmailAddress, string recipientEmailAddress)
		{
			return (!string.IsNullOrEmpty(senderEmailAddress) && this.ContainsValidBackoffEntry(this.senderBackoffListCache, senderEmailAddress)) || (!string.IsNullOrEmpty(recipientEmailAddress) && this.ContainsValidBackoffEntry(this.recipientsBackoffListCache, recipientEmailAddress));
		}

		private bool ContainsValidBackoffEntry(ExactTimeoutCache<string, DateTime> timeoutCache, string emailAddress)
		{
			DateTime t;
			return timeoutCache.TryGetValue(emailAddress, out t) && t > DateTime.UtcNow;
		}

		private const int BackoffListCacheSize = 10240;

		private static E4eBackoffListCache instance = new E4eBackoffListCache();

		private static object staticLock = new object();

		private ExactTimeoutCache<string, DateTime> senderBackoffListCache;

		private ExactTimeoutCache<string, DateTime> recipientsBackoffListCache;
	}
}
