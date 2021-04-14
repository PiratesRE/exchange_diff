using System;
using System.Security.Principal;
using Microsoft.Exchange.Data.ApplicationLogic.Directory;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class UserIdentity : RecipientIdentity
	{
		public UserIdentity(ADUser adUser)
		{
			this.adUser = adUser;
			this.adRecipient = adUser;
			this.masterAccountSid = RecipientHelper.TryGetMasterAccountSid(adUser);
			this.objectSid = adUser.Sid;
			this.sid = (this.masterAccountSid ?? this.objectSid);
		}

		public static bool TryCreate(ADRecipient adRecipient, out UserIdentity userIdentity)
		{
			userIdentity = null;
			ADUser aduser = adRecipient as ADUser;
			if (aduser != null)
			{
				userIdentity = new UserIdentity(aduser);
			}
			else
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<ADRecipient, string>(0L, "adRecipient {0} is not ADUser. Type is {1}.", adRecipient, adRecipient.GetType().ToString());
			}
			return userIdentity != null;
		}

		public SecurityIdentifier ObjectSid
		{
			get
			{
				return this.objectSid;
			}
		}

		public ADUser ADUser
		{
			get
			{
				return this.adUser;
			}
		}

		public bool IsClientSecurityContextCreatedFromAccountGroupInformation
		{
			get
			{
				return this.isClientSecurityContextCreatedFromAccountGroupInformation;
			}
			set
			{
				this.isClientSecurityContextCreatedFromAccountGroupInformation = value;
			}
		}

		public override IRecipientSession GetADRecipientSession()
		{
			return Directory.CreateADRecipientSessionForOrganization(this.adUser.QueryBaseDN, this.adUser.OrganizationId);
		}

		public override IRecipientSession GetGALScopedADRecipientSession(ClientSecurityContext clientSecurityContext)
		{
			ADObjectId searchRoot;
			if (this.adUser.AddressBookPolicy != null)
			{
				searchRoot = this.adUser.GlobalAddressListFromAddressBookPolicy;
			}
			else
			{
				searchRoot = this.adUser.QueryBaseDN;
			}
			return Directory.CreateGALScopedADRecipientSessionForOrganization(searchRoot, 0, this.adUser.OrganizationId, clientSecurityContext);
		}

		private ADUser adUser;

		private SecurityIdentifier objectSid;

		private bool isClientSecurityContextCreatedFromAccountGroupInformation;
	}
}
