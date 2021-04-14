using System;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Security.Authentication
{
	internal class LiveIdFbaTokenAccessor : CommonAccessTokenAccessor
	{
		private LiveIdFbaTokenAccessor(CommonAccessToken token) : base(token)
		{
		}

		public override AccessTokenType TokenType
		{
			get
			{
				return AccessTokenType.LiveId;
			}
		}

		public static LiveIdFbaTokenAccessor Create(ADRawEntry adRawEntry)
		{
			if (adRawEntry == null)
			{
				throw new ArgumentNullException("adRawEntry");
			}
			CommonAccessToken commonAccessToken = new CommonAccessToken(AccessTokenType.LiveId);
			LiveIdFbaTokenAccessor liveIdFbaTokenAccessor = new LiveIdFbaTokenAccessor(commonAccessToken);
			liveIdFbaTokenAccessor.UserSid = ((SecurityIdentifier)adRawEntry[ADMailboxRecipientSchema.Sid]).ToString();
			liveIdFbaTokenAccessor.UserPrincipalName = (string)adRawEntry[ADUserSchema.UserPrincipalName];
			liveIdFbaTokenAccessor.LiveIdMemberName = ((SmtpAddress)adRawEntry[ADRecipientSchema.WindowsLiveID]).ToString();
			OrganizationId organizationId = (OrganizationId)adRawEntry[ADObjectSchema.OrganizationId];
			liveIdFbaTokenAccessor.OrganizationId = organizationId;
			liveIdFbaTokenAccessor.LiveIdHasAcceptedAccruals = true;
			commonAccessToken.ExtensionData["Partition"] = organizationId.PartitionId.ToString();
			commonAccessToken.ExtensionData["OrganizationName"] = organizationId.ConfigurationUnit.Parent.Name;
			return liveIdFbaTokenAccessor;
		}

		public static LiveIdFbaTokenAccessor Create(LiveIDIdentity liveIdIdentity)
		{
			if (liveIdIdentity == null)
			{
				throw new ArgumentNullException("liveIdIdentity");
			}
			CommonAccessToken commonAccessToken = new CommonAccessToken(AccessTokenType.LiveId);
			LiveIdFbaTokenAccessor liveIdFbaTokenAccessor = new LiveIdFbaTokenAccessor(commonAccessToken);
			liveIdFbaTokenAccessor.UserSid = liveIdIdentity.Sid.ToString();
			liveIdFbaTokenAccessor.UserPrincipalName = liveIdIdentity.PrincipalName;
			liveIdFbaTokenAccessor.LiveIdMemberName = liveIdIdentity.MemberName;
			liveIdFbaTokenAccessor.OrganizationId = liveIdIdentity.UserOrganizationId;
			liveIdFbaTokenAccessor.LiveIdHasAcceptedAccruals = liveIdIdentity.HasAcceptedAccruals;
			commonAccessToken.ExtensionData["LoginAttributes"] = liveIdIdentity.LoginAttributes.Value.ToString();
			commonAccessToken.ExtensionData["Partition"] = liveIdIdentity.PartitionId;
			commonAccessToken.ExtensionData["OrganizationName"] = liveIdIdentity.UserOrganizationId.ConfigurationUnit.Parent.Name;
			return liveIdFbaTokenAccessor;
		}

		public static LiveIdFbaTokenAccessor Attach(CommonAccessToken token)
		{
			if (token == null)
			{
				throw new ArgumentNullException("token");
			}
			return new LiveIdFbaTokenAccessor(token);
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

		public string LiveIdMemberName
		{
			get
			{
				return base.SafeGetValue("MemberName");
			}
			set
			{
				base.SafeSetValue("MemberName", value);
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

		public bool LiveIdHasAcceptedAccruals
		{
			get
			{
				string value = base.SafeGetValue("LiveIdHasAcceptedAccruals");
				return string.IsNullOrEmpty(value) || bool.Parse(value);
			}
			set
			{
				base.SafeSetValue("LiveIdHasAcceptedAccruals", value.ToString());
			}
		}
	}
}
