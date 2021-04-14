using System;
using System.ComponentModel;
using System.Security.Principal;
using Microsoft.Exchange.Clients.EventLogs;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public sealed class OwaLiveIDIdentity : OwaClientSecurityContextIdentity
	{
		private OwaLiveIDIdentity(SecurityIdentifier userSid, bool hasAcceptedAccruals) : base(userSid)
		{
			this.hasAcceptedAccruals = hasAcceptedAccruals;
		}

		public bool HasAcceptedAccruals
		{
			get
			{
				return this.hasAcceptedAccruals;
			}
		}

		public static OwaClientSecurityContextIdentity CreateFromLiveIDIdentity(LiveIDIdentity liveIDIdentity)
		{
			if (liveIDIdentity == null)
			{
				throw new ArgumentNullException("liveIDIdentity");
			}
			OwaLiveIDIdentity owaLiveIDIdentity = new OwaLiveIDIdentity(liveIDIdentity.Sid, liveIDIdentity.HasAcceptedAccruals);
			owaLiveIDIdentity.userOrganizationProperties = liveIDIdentity.UserOrganizationProperties;
			owaLiveIDIdentity.DomainName = SmtpAddress.Parse(liveIDIdentity.MemberName).Domain;
			try
			{
				ClientSecurityContext clientSecurityContext = liveIDIdentity.CreateClientSecurityContext();
				owaLiveIDIdentity.UpgradePartialIdentity(clientSecurityContext, liveIDIdentity.PrincipalName, string.Empty);
			}
			catch (AuthzException ex)
			{
				if (ex.InnerException is Win32Exception)
				{
					OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ErrorCreatingClientContext, string.Empty, new object[]
					{
						owaLiveIDIdentity.UserSid.ToString(),
						ex.ToString()
					});
					throw new OwaCreateClientSecurityContextFailedException("There was a problem creating the Client Security Context.");
				}
				throw;
			}
			return owaLiveIDIdentity;
		}

		public override void Refresh(OwaIdentity identity)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			base.Refresh(identity);
			OwaLiveIDIdentity owaLiveIDIdentity = identity as OwaLiveIDIdentity;
			if (owaLiveIDIdentity != null)
			{
				this.hasAcceptedAccruals = owaLiveIDIdentity.HasAcceptedAccruals;
			}
		}

		private bool hasAcceptedAccruals;
	}
}
