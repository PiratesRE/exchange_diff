using System;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.Authentication.FederatedAuthService;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Security.Authentication
{
	internal class LiveIdBasicTokenAccessor : CommonAccessTokenAccessor
	{
		private LiveIdBasicTokenAccessor(CommonAccessToken token) : base(token)
		{
		}

		public override AccessTokenType TokenType
		{
			get
			{
				return AccessTokenType.LiveIdBasic;
			}
		}

		public static LiveIdBasicTokenAccessor Create(ADRawEntry adRawEntry)
		{
			if (adRawEntry == null)
			{
				throw new ArgumentNullException("adRawEntry");
			}
			CommonAccessToken commonAccessToken = new CommonAccessToken(AccessTokenType.LiveIdBasic);
			LiveIdBasicTokenAccessor liveIdBasicTokenAccessor = new LiveIdBasicTokenAccessor(commonAccessToken);
			liveIdBasicTokenAccessor.UserSid = ((SecurityIdentifier)adRawEntry[ADMailboxRecipientSchema.Sid]).ToString();
			liveIdBasicTokenAccessor.UserPrincipalName = (string)adRawEntry[ADUserSchema.UserPrincipalName];
			liveIdBasicTokenAccessor.LiveIdMemberName = ((SmtpAddress)adRawEntry[ADRecipientSchema.WindowsLiveID]).ToString();
			OrganizationId organizationId = (OrganizationId)adRawEntry[ADObjectSchema.OrganizationId];
			string arg = (string)adRawEntry[ADMailboxRecipientSchema.SamAccountName];
			liveIdBasicTokenAccessor.ImplicitUpn = string.Format("{0}@{1}", arg, organizationId.PartitionId.ForestFQDN);
			liveIdBasicTokenAccessor.OrganizationId = organizationId;
			commonAccessToken.ExtensionData["Partition"] = organizationId.PartitionId.ToString();
			if (adRawEntry[ADUserSchema.NetID] != null)
			{
				commonAccessToken.ExtensionData["Puid"] = adRawEntry[ADUserSchema.NetID].ToString();
			}
			if (AuthServiceHelper.IsMailboxRole && ConfigBase<AdDriverConfigSchema>.GetConfig<bool>("AccountValidationEnabled"))
			{
				commonAccessToken.ExtensionData.Add("CreateTime", ExDateTime.UtcNow.ToString());
			}
			return liveIdBasicTokenAccessor;
		}

		public static LiveIdBasicTokenAccessor Create(string puid, string memberName)
		{
			return new LiveIdBasicTokenAccessor(new CommonAccessToken(AccessTokenType.LiveIdBasic)
			{
				Version = 2
			})
			{
				Puid = puid,
				LiveIdMemberName = memberName
			};
		}

		public static LiveIdBasicTokenAccessor Attach(CommonAccessToken token)
		{
			if (token == null)
			{
				throw new ArgumentNullException("token");
			}
			return new LiveIdBasicTokenAccessor(token);
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

		public string ImplicitUpn
		{
			get
			{
				return base.SafeGetValue("ImplicitUpn");
			}
			set
			{
				base.SafeSetValue("ImplicitUpn", value);
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

		public string Puid
		{
			get
			{
				return base.SafeGetValue("Puid");
			}
			set
			{
				base.SafeSetValue("Puid", value);
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
	}
}
