using System;
using Microsoft.Exchange.Net.WSTrust;

namespace Microsoft.Exchange.Net.XropService
{
	internal sealed class FederatedClientCredentials
	{
		public FederatedClientCredentials(FederatedIdentity userFederatedIdentity, string userEmailAddress, FedOrgCredentials organizationCredentials)
		{
			if (userFederatedIdentity == null)
			{
				throw new ArgumentException("userFederatedIdentity");
			}
			if (userEmailAddress == null)
			{
				throw new ArgumentException("userEmailAddress");
			}
			if (organizationCredentials == null)
			{
				throw new ArgumentException("organizationCredentials");
			}
			this.userFederatedIdentity = userFederatedIdentity;
			this.organizationCredentials = organizationCredentials;
			this.userEmailAddress = userEmailAddress;
		}

		private FederatedClientCredentials(FederatedClientCredentials other)
		{
			this.userFederatedIdentity = other.userFederatedIdentity;
			this.organizationCredentials = other.organizationCredentials;
			this.userEmailAddress = other.userEmailAddress;
		}

		internal string UserEmailAddress
		{
			get
			{
				return this.userEmailAddress;
			}
		}

		internal FederatedIdentity UserFederatedIdentity
		{
			get
			{
				return this.userFederatedIdentity;
			}
		}

		public RequestedToken GetToken()
		{
			return this.organizationCredentials.GetToken();
		}

		private FederatedIdentity userFederatedIdentity;

		private string userEmailAddress;

		private FedOrgCredentials organizationCredentials;
	}
}
