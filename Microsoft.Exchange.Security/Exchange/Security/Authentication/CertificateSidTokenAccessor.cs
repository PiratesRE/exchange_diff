using System;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Security.Authentication
{
	internal class CertificateSidTokenAccessor : CommonAccessTokenAccessor
	{
		private CertificateSidTokenAccessor(CommonAccessToken token) : base(token)
		{
		}

		public override AccessTokenType TokenType
		{
			get
			{
				return AccessTokenType.CertificateSid;
			}
		}

		public static CertificateSidTokenAccessor Create(ADRawEntry adRawEntry)
		{
			return CertificateSidTokenAccessor.Create(adRawEntry, null);
		}

		public static CertificateSidTokenAccessor Create(ADRawEntry adRawEntry, X509Identifier certId)
		{
			if (adRawEntry == null)
			{
				throw new ArgumentNullException("adRawEntry");
			}
			CommonAccessToken token = new CommonAccessToken(AccessTokenType.CertificateSid);
			CertificateSidTokenAccessor certificateSidTokenAccessor = new CertificateSidTokenAccessor(token);
			certificateSidTokenAccessor.UserSid = ((SecurityIdentifier)adRawEntry[ADMailboxRecipientSchema.Sid]).ToString();
			OrganizationId organizationId = (OrganizationId)adRawEntry[ADObjectSchema.OrganizationId];
			if (organizationId != null && !organizationId.Equals(OrganizationId.ForestWideOrgId) && organizationId.PartitionId != null)
			{
				certificateSidTokenAccessor.PartitionId = organizationId.PartitionId.ToString();
			}
			if (certId != null)
			{
				certificateSidTokenAccessor.CertificateSubject = certId.ToString();
			}
			return certificateSidTokenAccessor;
		}

		public static CertificateSidTokenAccessor Create(GenericSidIdentity sidIdentity)
		{
			if (sidIdentity == null)
			{
				throw new ArgumentNullException("sidIdentity");
			}
			CommonAccessToken token = new CommonAccessToken(AccessTokenType.CertificateSid);
			return new CertificateSidTokenAccessor(token)
			{
				UserSid = sidIdentity.Sid.ToString(),
				PartitionId = sidIdentity.PartitionId
			};
		}

		public static CertificateSidTokenAccessor Attach(CommonAccessToken token)
		{
			if (token == null)
			{
				throw new ArgumentNullException("token");
			}
			return new CertificateSidTokenAccessor(token);
		}

		public string UserSid
		{
			get
			{
				return base.SafeGetValue("UserSid");
			}
			set
			{
				base.SafeSetValue("UserSid", value);
			}
		}

		public string PartitionId
		{
			get
			{
				return base.SafeGetValue("Partition");
			}
			set
			{
				base.SafeSetValue("Partition", value);
			}
		}

		public string CertificateSubject
		{
			get
			{
				return base.SafeGetValue("CertificateSubject");
			}
			set
			{
				base.SafeSetValue("CertificateSubject", value);
			}
		}
	}
}
