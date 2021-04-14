using System;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal abstract class RecipientIdentity : ADIdentityInformation
	{
		public static bool TryCreate(ADRecipient adRecipient, out RecipientIdentity recipientIdentity)
		{
			UserIdentity userIdentity = null;
			ContactIdentity contactIdentity = null;
			if (UserIdentity.TryCreate(adRecipient, out userIdentity))
			{
				recipientIdentity = userIdentity;
			}
			else if (ContactIdentity.TryCreate(adRecipient, out contactIdentity))
			{
				recipientIdentity = contactIdentity;
			}
			else
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<ADRecipient>(0L, "adRecipient {0} is not ADUser or ADContact", adRecipient);
				recipientIdentity = null;
			}
			return recipientIdentity != null;
		}

		public ADRecipient Recipient
		{
			get
			{
				return this.adRecipient;
			}
		}

		public override SecurityIdentifier Sid
		{
			get
			{
				return this.sid;
			}
		}

		public SecurityIdentifier MasterAccountSid
		{
			get
			{
				return this.masterAccountSid;
			}
		}

		public override string SmtpAddress
		{
			get
			{
				return this.adRecipient.PrimarySmtpAddress.ToString();
			}
		}

		public override Guid ObjectGuid
		{
			get
			{
				return this.adRecipient.Id.ObjectGuid;
			}
		}

		public override OrganizationId OrganizationId
		{
			get
			{
				return this.adRecipient.OrganizationId;
			}
		}

		public override IRecipientSession GetADRecipientSession()
		{
			return Directory.CreateADRecipientSessionForOrganization(null, 0, this.adRecipient.OrganizationId);
		}

		public override IRecipientSession GetGALScopedADRecipientSession(ClientSecurityContext clientSecurityContext)
		{
			return Directory.CreateGALScopedADRecipientSessionForOrganization(null, 0, this.adRecipient.OrganizationId, clientSecurityContext);
		}

		protected ADRecipient adRecipient;

		protected SecurityIdentifier sid;

		protected SecurityIdentifier masterAccountSid;
	}
}
