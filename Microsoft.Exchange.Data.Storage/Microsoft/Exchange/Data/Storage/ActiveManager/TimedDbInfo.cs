using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.ActiveManager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TimedDbInfo
	{
		internal TimedDbInfo(DatabaseLocationInfo dbInfo)
		{
			this.m_dbInfo = dbInfo;
			this.m_negativeExpiringCounter = 0;
		}

		internal int ExpiringCounter
		{
			get
			{
				return this.m_expiringCounter;
			}
			set
			{
				this.m_expiringCounter = value;
			}
		}

		internal int NegativeExpiringCounter
		{
			get
			{
				return this.m_negativeExpiringCounter;
			}
			set
			{
				this.m_negativeExpiringCounter = value;
			}
		}

		internal DatabaseLocationInfo DbLocationInfo
		{
			get
			{
				return this.m_dbInfo;
			}
			set
			{
				this.m_dbInfo = value;
				this.m_negativeExpiringCounter = 0;
			}
		}

		internal void ResetExpiringCounter()
		{
			this.ExpiringCounter = 0;
		}

		internal bool IsNegativeCacheExpired(int expiryThreshold)
		{
			return this.m_negativeExpiringCounter > expiryThreshold;
		}

		internal bool IsExpired(int expiryThreshold)
		{
			return this.ExpiringCounter > expiryThreshold;
		}

		private int m_expiringCounter;

		private int m_negativeExpiringCounter;

		private DatabaseLocationInfo m_dbInfo;
	}
}
