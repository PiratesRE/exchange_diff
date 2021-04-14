using System;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class MapiSessionPerUserAndClientTypeCounter : IMapiObjectCounter
	{
		public MapiSessionPerUserAndClientTypeCounter(string userDN, SecurityIdentifier userSid, ClientType clientType)
		{
			this.userDN = userDN;
			this.userSid = userSid;
			this.clientType = clientType;
		}

		public long GetCount()
		{
			return MapiSessionPerUserCounter.GetCount(this.userSid, this.clientType);
		}

		public void IncrementCount()
		{
			MapiSessionPerUserCounter.IncrementCount(this.userSid, this.clientType);
		}

		public void DecrementCount()
		{
			MapiSessionPerUserCounter.DecrementCount(this.userSid, this.clientType);
		}

		public void CheckObjectQuota(bool mustBeStrictlyUnderQuota)
		{
			long num = ActiveObjectLimits.EffectiveLimitation(this.clientType);
			if (num == -1L)
			{
				return;
			}
			bool flag;
			if (MapiSessionPerUserCounter.IsClientOverQuota(this.userSid, this.clientType, num, mustBeStrictlyUnderQuota, out flag))
			{
				if (flag)
				{
					this.LogEvent(num);
				}
				throw new StoreException((LID)57384U, ErrorCodeValue.MaxObjectsExceeded, "Exceeded session size limitation, user sid=" + this.userSid.Value + ", client type=" + this.clientType.ToString());
			}
		}

		private void LogEvent(long effectiveLimitation)
		{
			string periodicKey = this.userDN + this.clientType + MapiObjectTrackedType.Session;
			Microsoft.Exchange.Server.Storage.Common.Globals.LogPeriodicEvent(periodicKey, MSExchangeISEventLogConstants.Tuple_MaxObjectsExceeded, new object[]
			{
				this.userDN,
				this.clientType,
				effectiveLimitation,
				MapiObjectTrackedType.Session
			});
		}

		private readonly string userDN;

		private readonly SecurityIdentifier userSid;

		private readonly ClientType clientType;
	}
}
