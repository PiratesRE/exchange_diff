using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Configuration.CertificateAuthentication
{
	internal class CertificateADUserCache
	{
		internal CertificateADUserCache() : this(4, 300)
		{
		}

		internal CertificateADUserCache(int expirationTimeInHours, int maximumSize)
		{
			this.expirationTimeInHours = expirationTimeInHours;
			this.maximumSize = maximumSize;
			this.Cleanup();
		}

		internal ADUser GetUser(X509Identifier certificateId)
		{
			ADUser result;
			lock (this.syncObj)
			{
				ADUser aduser;
				if (!this.Cleanup() && this.userMapping.TryGetValue(certificateId, out aduser))
				{
					result = aduser;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		internal void AddUser(X509Identifier certificateId, ADUser user)
		{
			lock (this.syncObj)
			{
				this.Cleanup();
				if (this.userMapping.Count <= this.maximumSize)
				{
					this.userMapping[certificateId] = user;
				}
			}
		}

		private bool Cleanup()
		{
			if (this.userMapping == null || DateTime.UtcNow.CompareTo(this.expirationTimeUTC) > 0)
			{
				this.userMapping = new Dictionary<X509Identifier, ADUser>();
				this.expirationTimeUTC = DateTime.UtcNow.AddHours((double)this.expirationTimeInHours);
				return true;
			}
			return false;
		}

		internal const int CacheDefaultMaximumSize = 300;

		internal const int CacheDefaultExpirationTimeInHours = 4;

		private int expirationTimeInHours;

		private DateTime expirationTimeUTC;

		private int maximumSize;

		private Dictionary<X509Identifier, ADUser> userMapping;

		private object syncObj = new object();
	}
}
