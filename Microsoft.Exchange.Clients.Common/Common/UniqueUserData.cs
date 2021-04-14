using System;

namespace Microsoft.Exchange.Clients.Common
{
	internal sealed class UniqueUserData
	{
		internal UniqueUserData()
		{
		}

		internal int CurrentLightSessionCount
		{
			get
			{
				return this.currentLightSessionCount;
			}
		}

		internal int CurrentPremiumSessionCount
		{
			get
			{
				return this.currentPremiumSessionCount;
			}
		}

		internal bool IsFirstLightSession
		{
			get
			{
				return this.isFirstLightSession;
			}
		}

		internal bool IsFirstPremiumSession
		{
			get
			{
				return this.isFirstPremiumSession;
			}
		}

		internal void IncreaseSessionCounter(bool isProxy, bool isLightExperience)
		{
			lock (this)
			{
				if (!isProxy)
				{
					if (isLightExperience)
					{
						this.currentLightSessionCount++;
						this.isFirstLightSession = false;
					}
					else
					{
						this.currentPremiumSessionCount++;
						this.isFirstPremiumSession = false;
					}
				}
			}
		}

		internal void DecreaseSessionCounter(bool isProxy, bool isLightExperience)
		{
			lock (this)
			{
				if (!isProxy)
				{
					if (isLightExperience)
					{
						this.currentLightSessionCount--;
					}
					else
					{
						this.currentPremiumSessionCount--;
					}
				}
			}
		}

		internal int CurrentSessionCount
		{
			get
			{
				return this.currentLightSessionCount + this.currentPremiumSessionCount;
			}
		}

		private int currentLightSessionCount;

		private int currentPremiumSessionCount;

		private bool isFirstLightSession = true;

		private bool isFirstPremiumSession = true;
	}
}
