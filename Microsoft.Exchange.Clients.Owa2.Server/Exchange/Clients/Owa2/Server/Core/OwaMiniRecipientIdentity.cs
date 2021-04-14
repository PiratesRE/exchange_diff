using System;
using System.Globalization;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public sealed class OwaMiniRecipientIdentity : OwaIdentity
	{
		private OwaMiniRecipientIdentity(OWAMiniRecipient owaMiniRecipient)
		{
			base.OwaMiniRecipient = owaMiniRecipient;
		}

		private OwaMiniRecipientIdentity(ProxyAddress proxyAddress)
		{
			this.proxyAddress = proxyAddress;
		}

		public override WindowsIdentity WindowsIdentity
		{
			get
			{
				return null;
			}
		}

		public override SecurityIdentifier UserSid
		{
			get
			{
				return base.OwaMiniRecipient.Sid;
			}
		}

		public override string UniqueId
		{
			get
			{
				return this.proxyAddress.ToString();
			}
		}

		public override string AuthenticationType
		{
			get
			{
				return null;
			}
		}

		public ProxyAddress ProxyAddress
		{
			get
			{
				return this.proxyAddress;
			}
		}

		public override bool IsPartial
		{
			get
			{
				return base.OwaMiniRecipient == null;
			}
		}

		internal override ClientSecurityContext ClientSecurityContext
		{
			get
			{
				return null;
			}
		}

		public static OwaMiniRecipientIdentity CreateFromOWAMiniRecipient(OWAMiniRecipient recipient)
		{
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			ExTraceGlobals.CoreCallTracer.TraceDebug<SecurityIdentifier>(0L, "OwaMiniRecipientIdentity.CreateFromOWAMiniRecipient for recipient with Sid:{0}", recipient.Sid);
			return new OwaMiniRecipientIdentity(recipient);
		}

		public static OwaMiniRecipientIdentity CreateFromProxyAddress(string emailString)
		{
			if (emailString == null)
			{
				throw new ArgumentNullException("emailString");
			}
			ExTraceGlobals.CoreCallTracer.TraceDebug<string>(0L, "OwaMiniRecipientIdentity.CreateFromProxyAddress for emailString:{0}", emailString);
			ProxyAddress proxyAddress = null;
			try
			{
				proxyAddress = ProxyAddress.Parse(emailString);
			}
			catch (ArgumentNullException)
			{
				proxyAddress = null;
			}
			if (proxyAddress == null || proxyAddress.GetType() != typeof(SmtpProxyAddress))
			{
				throw new OwaExplicitLogonException(string.Format("{0} is not a valid SMTP address", emailString), string.Format(Strings.GetLocalizedString(-13616305), emailString));
			}
			return new OwaMiniRecipientIdentity(proxyAddress);
		}

		public override string SafeGetRenderableName()
		{
			return this.proxyAddress.ToString();
		}

		public override bool IsEqualsTo(OwaIdentity otherIdentity)
		{
			if (otherIdentity == null)
			{
				return false;
			}
			OwaMiniRecipientIdentity owaMiniRecipientIdentity = otherIdentity as OwaMiniRecipientIdentity;
			if (owaMiniRecipientIdentity == null)
			{
				throw new OwaInvalidOperationException("Comparing OwaMiniRecipientIdentity with identities of another type is not supported");
			}
			return owaMiniRecipientIdentity.ProxyAddress == this.proxyAddress;
		}

		public override string GetLogonName()
		{
			return null;
		}

		public void UpgradePartialIdentity()
		{
			OWAMiniRecipient owaminiRecipient = null;
			SmtpAddress smtpAddress = new SmtpAddress(this.proxyAddress.AddressString);
			ExTraceGlobals.CoreCallTracer.TraceDebug<string>(0L, "OwaMiniRecipientIdentity.UpgradePartialIdentity for smtp: {0}", this.proxyAddress.AddressString);
			IRecipientSession recipientSession = UserContextUtilities.CreateScopedRecipientSession(true, ConsistencyMode.FullyConsistent, smtpAddress.Domain, null);
			Exception ex = null;
			try
			{
				owaminiRecipient = recipientSession.FindMiniRecipientByProxyAddress<OWAMiniRecipient>(this.proxyAddress, OWAMiniRecipientSchema.AdditionalProperties);
				if (owaminiRecipient == null)
				{
					throw new OwaExplicitLogonException(string.Format("The address {0} is an object in AD database but it is not an user", this.proxyAddress), Strings.GetLocalizedString(-1332692688), ex);
				}
			}
			catch (NonUniqueRecipientException ex2)
			{
				ex = ex2;
			}
			if (owaminiRecipient == null || ex != null)
			{
				throw new OwaExplicitLogonException(string.Format("Couldn't find a match for {0}", this.proxyAddress.ToString()), string.Format(Strings.GetLocalizedString(-13616305), this.proxyAddress), ex);
			}
			base.OwaMiniRecipient = owaminiRecipient;
		}

		internal override MailboxSession CreateMailboxSession(ExchangePrincipal exchangePrincipal, CultureInfo cultureInfo)
		{
			return null;
		}

		internal override MailboxSession CreateInstantSearchMailboxSession(ExchangePrincipal exchangePrincipal, CultureInfo cultureInfo)
		{
			return null;
		}

		internal override MailboxSession CreateDelegateMailboxSession(ExchangePrincipal exchangePrincipal, CultureInfo cultureInfo)
		{
			return null;
		}

		internal override ExchangePrincipal InternalCreateExchangePrincipal()
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "OwaMiniRecipientIdentity.InternalCreateExchangePrincipal");
			return ExchangePrincipal.FromMiniRecipient(base.OwaMiniRecipient);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<OwaMiniRecipientIdentity>(this);
		}

		private ProxyAddress proxyAddress;
	}
}
