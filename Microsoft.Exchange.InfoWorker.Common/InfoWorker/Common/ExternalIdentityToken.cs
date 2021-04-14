using System;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Availability;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.InfoWorker.Common
{
	internal class ExternalIdentityToken : ISecurityAccessToken
	{
		internal static ExternalIdentityToken GetExternalIdentityToken(MailboxSession session, SmtpAddress externalId)
		{
			if (session != null && session.Capabilities.CanHaveExternalUsers)
			{
				using (ExternalUserCollection externalUsers = session.GetExternalUsers())
				{
					ExternalUser externalUser = externalUsers.FindExternalUser(externalId.ToString());
					if (externalUser != null)
					{
						return new ExternalIdentityToken(externalUser.Sid);
					}
					ExternalIdentityToken.Tracer.TraceError<SmtpAddress, IExchangePrincipal>(0L, "{0}: Unable to find the requester in the external user collection in mailbox {1}.", externalId, session.MailboxOwner);
				}
			}
			return null;
		}

		public string UserSid
		{
			get
			{
				return this.sid.ToString();
			}
			set
			{
				throw new InvalidOperationException("UsedSid");
			}
		}

		public SidStringAndAttributes[] GroupSids
		{
			get
			{
				return ExternalIdentityToken.groupSidStringAndAttributesArray;
			}
			set
			{
				throw new InvalidOperationException("GroupSids");
			}
		}

		public SidStringAndAttributes[] RestrictedGroupSids
		{
			get
			{
				return ExternalIdentityToken.emptySidStringAndAttributesArray;
			}
			set
			{
				throw new InvalidOperationException("GroupSids");
			}
		}

		private ExternalIdentityToken(SecurityIdentifier sid)
		{
			this.sid = sid;
		}

		private static readonly SidStringAndAttributes[] emptySidStringAndAttributesArray = new SidStringAndAttributes[0];

		private static readonly SidStringAndAttributes[] groupSidStringAndAttributesArray = new SidStringAndAttributes[]
		{
			new SidStringAndAttributes("S-1-1-0", 0U)
		};

		private SecurityIdentifier sid;

		private static readonly Trace Tracer = ExTraceGlobals.SecurityTracer;
	}
}
