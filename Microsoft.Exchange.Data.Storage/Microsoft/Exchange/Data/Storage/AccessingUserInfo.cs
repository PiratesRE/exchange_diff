using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class AccessingUserInfo
	{
		public AccessingUserInfo(string legacyExchangeDN, string externalDirectoryObjectId, OrganizationId organizationId, ADObjectId userObjectId)
		{
			if (legacyExchangeDN == null && externalDirectoryObjectId == null)
			{
				throw new ArgumentException("Either legacyExchangeDN or externalDirectoryObjectId should be provided");
			}
			ArgumentValidator.ThrowIfNull("organizationId", organizationId);
			this.legacyExchangeDN = legacyExchangeDN;
			this.externalDirectoryObjectId = externalDirectoryObjectId;
			this.organizationId = organizationId;
			this.userObjectId = userObjectId;
		}

		public string LegacyExchangeDN
		{
			get
			{
				return this.legacyExchangeDN;
			}
		}

		public string ExternalDirectoryObjectId
		{
			get
			{
				return this.externalDirectoryObjectId;
			}
		}

		public ADObjectId UserObjectId
		{
			get
			{
				if (this.userObjectId == null)
				{
					this.userObjectId = this.FindUserObjectId();
				}
				return this.userObjectId;
			}
		}

		public string Identity
		{
			get
			{
				return this.LegacyExchangeDN ?? this.ExternalDirectoryObjectId;
			}
		}

		public static IRecipientSession GetRecipientSession(OrganizationId organizationId)
		{
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.FullyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId), 120, "GetRecipientSession", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\Groups\\AccessingUserInfo.cs");
		}

		private ADObjectId FindUserObjectId()
		{
			ADRecipient adrecipient = null;
			IRecipientSession recipientSession = AccessingUserInfo.GetRecipientSession(this.organizationId);
			ArgumentValidator.ThrowIfNull("recipientSession", recipientSession);
			if (this.LegacyExchangeDN != null)
			{
				adrecipient = recipientSession.FindByLegacyExchangeDN(this.LegacyExchangeDN);
			}
			else if (this.ExternalDirectoryObjectId != null)
			{
				adrecipient = recipientSession.FindADUserByExternalDirectoryObjectId(this.ExternalDirectoryObjectId);
			}
			if (adrecipient == null)
			{
				return null;
			}
			return adrecipient.Id;
		}

		private ADObjectId userObjectId;

		private readonly string legacyExchangeDN;

		private readonly string externalDirectoryObjectId;

		private OrganizationId organizationId;
	}
}
