using System;
using System.Globalization;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Data.DocumentLibrary;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public sealed class OwaAlternateMailboxIdentity : OwaIdentity
	{
		private OwaAlternateMailboxIdentity(OwaIdentity logonIdentity, ExchangePrincipal logonExchangePrincipal, Guid aggregatedMailboxGuid)
		{
			this.logonIdentity = logonIdentity;
			this.logonExchangePrincipal = logonExchangePrincipal;
			this.aggregatedMailboxGuid = aggregatedMailboxGuid;
		}

		public override WindowsIdentity WindowsIdentity
		{
			get
			{
				return this.logonIdentity.WindowsIdentity;
			}
		}

		public override SecurityIdentifier UserSid
		{
			get
			{
				return this.logonIdentity.UserSid;
			}
		}

		public override string AuthenticationType
		{
			get
			{
				return this.logonIdentity.AuthenticationType;
			}
		}

		public override string UniqueId
		{
			get
			{
				return this.aggregatedMailboxGuid.ToString();
			}
		}

		public override bool IsPartial
		{
			get
			{
				return this.logonIdentity.IsPartial;
			}
		}

		public override string GetLogonName()
		{
			return this.logonIdentity.GetLogonName();
		}

		public override string SafeGetRenderableName()
		{
			return this.logonExchangePrincipal.MailboxInfo.DisplayName;
		}

		internal static Guid? GetAlternateMailbox(IExchangePrincipal exchangePrincipal, string smtpAddress)
		{
			return null;
		}

		internal override ClientSecurityContext ClientSecurityContext
		{
			get
			{
				return this.logonIdentity.ClientSecurityContext;
			}
		}

		internal static OwaAlternateMailboxIdentity Create(OwaIdentity logonIdentity, ExchangePrincipal logonExchangePrincipal, Guid aggregatedMailboxGuid)
		{
			return new OwaAlternateMailboxIdentity(logonIdentity, logonExchangePrincipal, aggregatedMailboxGuid);
		}

		internal override ExchangePrincipal InternalCreateExchangePrincipal()
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "OwaAlternateMailboxIdentity.CreateExchangePrincipal");
			return this.logonExchangePrincipal.GetAggregatedExchangePrincipal(this.aggregatedMailboxGuid);
		}

		internal override MailboxSession CreateMailboxSession(IExchangePrincipal exchangePrincipal, CultureInfo cultureInfo, HttpRequest clientRequest)
		{
			return this.logonIdentity.CreateMailboxSession(exchangePrincipal, cultureInfo, clientRequest);
		}

		internal override MailboxSession CreateWebPartMailboxSession(IExchangePrincipal mailBoxExchangePrincipal, CultureInfo cultureInfo, HttpRequest clientRequest)
		{
			return this.logonIdentity.CreateWebPartMailboxSession(mailBoxExchangePrincipal, cultureInfo, clientRequest);
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

		private readonly Guid aggregatedMailboxGuid;

		private readonly OwaIdentity logonIdentity;

		private readonly ExchangePrincipal logonExchangePrincipal;
	}
}
