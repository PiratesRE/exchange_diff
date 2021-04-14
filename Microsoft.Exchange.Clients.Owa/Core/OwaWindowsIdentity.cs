using System;
using System.Globalization;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Data.DocumentLibrary;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public sealed class OwaWindowsIdentity : OwaIdentity
	{
		private OwaWindowsIdentity(WindowsIdentity windowsIdentity)
		{
			this.windowsIdentity = new WindowsIdentity(windowsIdentity.Token);
		}

		public static OwaWindowsIdentity CreateFromWindowsIdentity(WindowsIdentity windowsIdentity)
		{
			return new OwaWindowsIdentity(windowsIdentity);
		}

		public WindowsPrincipal WindowsPrincipal
		{
			get
			{
				if (this.windowsPrincipal == null && this.WindowsIdentity != null)
				{
					this.windowsPrincipal = new WindowsPrincipal(this.WindowsIdentity);
				}
				return this.windowsPrincipal;
			}
		}

		public override WindowsIdentity WindowsIdentity
		{
			get
			{
				return this.windowsIdentity;
			}
		}

		public override SecurityIdentifier UserSid
		{
			get
			{
				return this.windowsIdentity.User;
			}
		}

		public override string GetLogonName()
		{
			string name;
			try
			{
				name = this.windowsIdentity.Name;
			}
			catch (SystemException innerException)
			{
				throw new OwaIdentityException("Failed to retrieve user name", innerException);
			}
			return name;
		}

		public override string SafeGetRenderableName()
		{
			string result = null;
			try
			{
				result = this.GetLogonName();
			}
			catch (OwaIdentityException)
			{
				result = this.UniqueId;
			}
			return result;
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
				return this.windowsIdentity.AuthenticationType;
			}
		}

		internal override ClientSecurityContext ClientSecurityContext
		{
			get
			{
				if (this.clientSecurityContext == null)
				{
					this.clientSecurityContext = new ClientSecurityContext(this.windowsIdentity);
				}
				return this.clientSecurityContext;
			}
		}

		public override bool IsPartial
		{
			get
			{
				return false;
			}
		}

		internal override ExchangePrincipal InternalCreateExchangePrincipal()
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "OwaWindowsIdentity.CreateExchangePrincipal");
			return ExchangePrincipal.FromMiniRecipient(base.GetOWAMiniRecipient());
		}

		internal override MailboxSession CreateMailboxSession(IExchangePrincipal exchangePrincipal, CultureInfo cultureInfo, HttpRequest clientRequest)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "OwaWindowsIdentity.CreateMailboxSession");
			MailboxSession result;
			try
			{
				MailboxSession mailboxSession = MailboxSession.Open(exchangePrincipal, this.WindowsPrincipal, cultureInfo, "Client=OWA");
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
			MailboxSession result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				MailboxSession mailboxSession = MailboxSession.OpenWithBestAccess(mailBoxExchangePrincipal, base.CreateADOrgPersonForWebPartUserBySid(), this.WindowsPrincipal, cultureInfo, "Client=OWA;Action=WebPart + Delegate");
				disposeGuard.Add<MailboxSession>(mailboxSession);
				GccUtils.SetStoreSessionClientIPEndpointsFromHttpRequest(mailboxSession, clientRequest);
				disposeGuard.Success();
				result = mailboxSession;
			}
			return result;
		}

		internal override UncSession CreateUncSession(DocumentLibraryObjectId objectId)
		{
			return UncSession.Open(objectId, this.WindowsPrincipal);
		}

		internal override SharepointSession CreateSharepointSession(DocumentLibraryObjectId objectId)
		{
			return SharepointSession.Open(objectId, this.WindowsPrincipal);
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing && !this.isDisposed)
			{
				if (this.clientSecurityContext != null)
				{
					this.clientSecurityContext.Dispose();
					this.clientSecurityContext = null;
					if (this.windowsIdentity != null)
					{
						this.windowsIdentity.Dispose();
						this.windowsIdentity = null;
					}
				}
				this.isDisposed = true;
			}
			base.InternalDispose(isDisposing);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<OwaWindowsIdentity>(this);
		}

		private ClientSecurityContext clientSecurityContext;

		private WindowsIdentity windowsIdentity;

		private bool isDisposed;

		private WindowsPrincipal windowsPrincipal;
	}
}
