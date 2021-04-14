using System;
using System.Globalization;
using System.Security.Principal;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public sealed class OwaWindowsIdentity : OwaIdentity
	{
		private OwaWindowsIdentity(WindowsIdentity windowsIdentity)
		{
			this.windowsIdentity = new WindowsIdentity(windowsIdentity.Token);
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

		public override bool IsPartial
		{
			get
			{
				return false;
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

		public static OwaWindowsIdentity CreateFromWindowsIdentity(WindowsIdentity windowsIdentity)
		{
			return new OwaWindowsIdentity(windowsIdentity);
		}

		public override string GetLogonName()
		{
			string result = string.Empty;
			try
			{
				result = this.windowsIdentity.Name;
			}
			catch (SystemException innerException)
			{
				throw new OwaIdentityException("Failed to retrieve user name", innerException);
			}
			return result;
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

		internal override ExchangePrincipal InternalCreateExchangePrincipal()
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "OwaWindowsIdentity.CreateExchangePrincipal");
			return ExchangePrincipal.FromMiniRecipient(base.GetOWAMiniRecipient());
		}

		internal override MailboxSession CreateMailboxSession(ExchangePrincipal exchangePrincipal, CultureInfo cultureInfo)
		{
			return this.CreateMailboxSession(exchangePrincipal, cultureInfo, "Client=OWA");
		}

		internal override MailboxSession CreateInstantSearchMailboxSession(ExchangePrincipal exchangePrincipal, CultureInfo cultureInfo)
		{
			return this.CreateMailboxSession(exchangePrincipal, cultureInfo, "Client=OWA;Action=InstantSearch");
		}

		internal MailboxSession CreateMailboxSession(ExchangePrincipal exchangePrincipal, CultureInfo cultureInfo, string userContextString)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "OwaWindowsIdentity.CreateMailboxSession");
			MailboxSession result;
			try
			{
				MailboxSession mailboxSession = MailboxSession.OpenAsTransport(exchangePrincipal, userContextString);
				result = mailboxSession;
			}
			catch (AccessDeniedException innerException)
			{
				throw new OwaExplicitLogonException("User has no access rights to the mailbox", "ErrorExplicitLogonAccessDenied", innerException);
			}
			return result;
		}

		internal override MailboxSession CreateDelegateMailboxSession(ExchangePrincipal exchangePrincipal, CultureInfo cultureInfo)
		{
			return null;
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
