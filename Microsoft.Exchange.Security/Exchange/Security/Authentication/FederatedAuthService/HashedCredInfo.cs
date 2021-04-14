using System;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	internal class HashedCredInfo
	{
		public HashedCredInfo(byte[] hash, ExDateTime time, CredFailure mode, string tag, UserType userType)
		{
			this.Hash = hash;
			this.Time = time;
			this.Mode = mode;
			this.Tag = tag;
			this.UserType = userType;
		}

		public bool IsExpired(LogonCacheConfig config, ExDateTime time)
		{
			int num;
			switch (this.Mode)
			{
			case CredFailure.Expired:
			case CredFailure.LockedOut:
			case CredFailure.LiveIdFailure:
			case CredFailure.STSFailure:
				num = config.badCredsRecoverableLifetime;
				goto IL_35;
			}
			num = config.badCredsLifetime;
			IL_35:
			return time >= this.Time.Add(TimeSpan.FromMinutes((double)num));
		}

		public byte[] Hash;

		public ExDateTime Time;

		public CredFailure Mode;

		public string Tag;

		public UserType UserType;
	}
}
