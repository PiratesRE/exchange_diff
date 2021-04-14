using System;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Security.OAuth
{
	internal class OAuthPreAuthIdentity : GenericIdentity
	{
		public OAuthPreAuthIdentity(OAuthPreAuthType preAuthType, OrganizationId organizationId, string lookupValue) : base(string.Empty, Constants.BearerPreAuthenticationType)
		{
			this.OrganizationId = organizationId;
			this.PreAuthType = preAuthType;
			this.LookupValue = lookupValue;
		}

		public OAuthPreAuthType PreAuthType { get; private set; }

		public OrganizationId OrganizationId { get; private set; }

		public string LookupValue { get; private set; }

		public override string ToString()
		{
			return string.Format("{0}-{1}", this.PreAuthType, this.LookupValue);
		}

		public override bool IsAuthenticated
		{
			get
			{
				return true;
			}
		}
	}
}
