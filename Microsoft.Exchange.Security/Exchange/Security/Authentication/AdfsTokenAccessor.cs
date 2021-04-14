using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Security.Authentication
{
	internal class AdfsTokenAccessor : CommonAccessTokenAccessor
	{
		private AdfsTokenAccessor(CommonAccessToken token) : base(token)
		{
		}

		public override AccessTokenType TokenType
		{
			get
			{
				return AccessTokenType.Adfs;
			}
		}

		public static AdfsTokenAccessor Create(AdfsIdentity identity)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			CommonAccessToken token = new CommonAccessToken(AccessTokenType.Adfs);
			return new AdfsTokenAccessor(token)
			{
				UserSid = identity.Sid.ToString(),
				UserPrincipalName = identity.PrincipalName,
				OrganizationId = identity.UserOrganizationId,
				GroupMembershipSids = identity.PrepopulatedGroupSidIds,
				IsPublicSession = identity.IsPublicSession
			};
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

		public string UserPrincipalName
		{
			get
			{
				return base.SafeGetValue("UserPrincipalName");
			}
			set
			{
				base.SafeSetValue("UserPrincipalName", value);
			}
		}

		public OrganizationId OrganizationId
		{
			get
			{
				return CommonAccessTokenAccessor.DeserializeOrganizationId(base.SafeGetValue("OrganizationIdBase64"));
			}
			set
			{
				base.SafeSetValue("OrganizationIdBase64", CommonAccessTokenAccessor.SerializeOrganizationId(value));
			}
		}

		public IEnumerable<string> GroupMembershipSids
		{
			get
			{
				return CommonAccessTokenAccessor.DeserializeGroupMembershipSids(base.SafeGetValue("GroupMembershipSids"));
			}
			set
			{
				base.SafeSetValue("GroupMembershipSids", CommonAccessTokenAccessor.SerializeGroupMembershipSids(value));
			}
		}

		public bool IsPublicSession
		{
			get
			{
				return CommonAccessTokenAccessor.DeserializIsPublicSession(base.SafeGetValue("IsPublicSession"));
			}
			set
			{
				base.SafeSetValue("IsPublicSession", CommonAccessTokenAccessor.SerializeIsPublicSession(value));
			}
		}
	}
}
