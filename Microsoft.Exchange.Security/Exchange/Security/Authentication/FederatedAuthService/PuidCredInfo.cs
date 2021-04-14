using System;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	internal class PuidCredInfo
	{
		public PuidCredInfo(ExDateTime time, string puid, byte[] hash, string tag, int lifeTimeInMinutes, string passwordExpiry, UserType userType, bool appPassword, bool requestPasswordConfidenceCheckInBackend)
		{
			this.Time = time;
			this.PUID = puid;
			this.Tag = tag;
			this.Hash = hash;
			this.LifeTimeInMinutes = lifeTimeInMinutes;
			this.RequestPasswordConfidenceCheckInBackend = requestPasswordConfidenceCheckInBackend;
			this.passwordExpiry = passwordExpiry;
			this.userType = userType;
			this.appPassword = appPassword;
		}

		public ExDateTime Time;

		public string PUID;

		public string Tag;

		public byte[] Hash;

		public int LifeTimeInMinutes;

		public bool RequestPasswordConfidenceCheckInBackend;

		public string passwordExpiry;

		public UserType userType;

		public bool appPassword;
	}
}
