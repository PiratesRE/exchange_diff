using System;
using System.Globalization;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.DocumentLibrary;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class OwaClientSecurityContextIdentity : OwaIdentity
	{
		private OwaClientSecurityContextIdentity(ClientSecurityContext clientSecurityContext, string logonName, string authenticationType)
		{
			this.clientSecurityContext = clientSecurityContext;
			this.logonName = logonName;
			this.authenticationType = authenticationType;
		}

		protected OwaClientSecurityContextIdentity(SecurityIdentifier userSid)
		{
			this.userSid = userSid;
		}

		internal static OwaClientSecurityContextIdentity CreateFromClientSecurityContextIdentity(ClientSecurityContextIdentity cscIdentity)
		{
			ClientSecurityContext clientSecurityContext = cscIdentity.CreateClientSecurityContext();
			return new OwaClientSecurityContextIdentity(clientSecurityContext, cscIdentity.Name, cscIdentity.AuthenticationType);
		}

		internal static OwaClientSecurityContextIdentity CreateFromClientSecurityContext(ClientSecurityContext clientSecurityContext, string logonName, string authenticationType)
		{
			return new OwaClientSecurityContextIdentity(clientSecurityContext, logonName, authenticationType);
		}

		public static OwaClientSecurityContextIdentity CreateFromSecurityIdentifier(SecurityIdentifier userSid)
		{
			return new OwaClientSecurityContextIdentity(userSid);
		}

		public override bool IsPartial
		{
			get
			{
				return this.clientSecurityContext == null;
			}
		}

		internal void UpgradePartialIdentity(ClientSecurityContext clientSecurityContext, string logonName, string authenticationType)
		{
			this.clientSecurityContext = clientSecurityContext;
			this.logonName = logonName;
			this.authenticationType = authenticationType;
			if (this.clientSecurityContext.UserSid != this.userSid)
			{
				throw new OwaInvalidOperationException("Can't upgrade a partial identity to a full identity that doesn't correspond to the same user sid");
			}
			this.userSid = null;
			this.owaMiniRecipient = null;
		}

		public override WindowsIdentity WindowsIdentity
		{
			get
			{
				base.ThrowNotSupported("WindowsIdentity");
				return null;
			}
		}

		public override SecurityIdentifier UserSid
		{
			get
			{
				if (this.IsPartial)
				{
					return this.userSid;
				}
				return this.clientSecurityContext.UserSid;
			}
		}

		public override string GetLogonName()
		{
			return this.logonName;
		}

		public override string SafeGetRenderableName()
		{
			if (!this.IsPartial)
			{
				return this.logonName;
			}
			return this.userSid.ToString();
		}

		public override string UniqueId
		{
			get
			{
				return this.UserSid.ToString();
			}
		}

		public override string AuthenticationType
		{
			get
			{
				return this.authenticationType;
			}
		}

		internal override ClientSecurityContext ClientSecurityContext
		{
			get
			{
				return this.clientSecurityContext;
			}
		}

		internal override ExchangePrincipal InternalCreateExchangePrincipal()
		{
			ADSessionSettings adSettings = Utilities.CreateScopedADSessionSettings(this.DomainName);
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "OwaWindowsIdentity.CreateExchangePrincipal");
			return ExchangePrincipal.FromUserSid(adSettings, this.UserSid, RemotingOptions.AllowCrossSite);
		}

		internal override MailboxSession CreateMailboxSession(IExchangePrincipal exchangePrincipal, CultureInfo cultureInfo, HttpRequest clientRequest)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "OwaClientSecurityContextIdentity.CreateMailboxSession");
			MailboxSession result;
			try
			{
				MailboxSession mailboxSession = MailboxSession.Open(exchangePrincipal, this.clientSecurityContext, cultureInfo, "Client=OWA;Action=ViaProxy");
				GccUtils.SetStoreSessionClientIPEndpointsFromHttpRequest(mailboxSession, clientRequest);
				result = mailboxSession;
			}
			catch (AccessDeniedException innerException)
			{
				throw new OwaExplicitLogonException("User has no access rights to the mailbox", LocalizedStrings.GetNonEncoded(882888134), innerException);
			}
			return result;
		}

		internal override MailboxSession CreateWebPartMailboxSession(IExchangePrincipal mailBoxExchangePrincipal, CultureInfo cultureInfo, HttpRequest clientRequest)
		{
			MailboxSession mailboxSession = MailboxSession.OpenWithBestAccess(mailBoxExchangePrincipal, base.CreateADOrgPersonForWebPartUserBySid(), this.clientSecurityContext, cultureInfo, "Client=OWA;Action=WebPart + Delegate + ViaProxy");
			GccUtils.SetStoreSessionClientIPEndpointsFromHttpRequest(mailboxSession, clientRequest);
			return mailboxSession;
		}

		internal override UncSession CreateUncSession(DocumentLibraryObjectId objectId)
		{
			base.ThrowNotSupported("CreateUncSession");
			return null;
		}

		internal override SharepointSession CreateSharepointSession(DocumentLibraryObjectId objectId)
		{
			base.ThrowNotSupported("CreateSharepointSession");
			return null;
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing && this.clientSecurityContext != null)
			{
				this.clientSecurityContext.Dispose();
				this.clientSecurityContext = null;
			}
			base.InternalDispose(isDisposing);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<OwaClientSecurityContextIdentity>(this);
		}

		private SecurityIdentifier userSid;

		private ClientSecurityContext clientSecurityContext;

		private string logonName;

		private string authenticationType;
	}
}
