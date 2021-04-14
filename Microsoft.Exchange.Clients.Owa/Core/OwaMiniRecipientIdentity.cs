using System;
using System.Globalization;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.DocumentLibrary;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public sealed class OwaMiniRecipientIdentity : OwaIdentity
	{
		private OwaMiniRecipientIdentity(OWAMiniRecipient owaMiniRecipient)
		{
			this.owaMiniRecipient = owaMiniRecipient;
		}

		private OwaMiniRecipientIdentity(ProxyAddress proxyAddress)
		{
			this.proxyAddress = proxyAddress;
		}

		public static OwaMiniRecipientIdentity CreateFromOWAMiniRecipient(OWAMiniRecipient recipient)
		{
			if (recipient == null)
			{
				throw new ArgumentNullException("user", "AD User cannot be null");
			}
			return new OwaMiniRecipientIdentity(recipient);
		}

		public static OwaMiniRecipientIdentity CreateFromProxyAddress(string emailString)
		{
			if (emailString == null)
			{
				throw new ArgumentNullException("emailString");
			}
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
				throw new OwaExplicitLogonException(string.Format("{0} is not a valid SMTP address", emailString), string.Format(LocalizedStrings.GetNonEncoded(-13616305), emailString));
			}
			return new OwaMiniRecipientIdentity(proxyAddress);
		}

		public void UpgradePartialIdentity()
		{
			OWAMiniRecipient owaminiRecipient = null;
			SmtpAddress smtpAddress = new SmtpAddress(this.proxyAddress.AddressString);
			IRecipientSession recipientSession = Utilities.CreateScopedRecipientSession(true, ConsistencyMode.FullyConsistent, smtpAddress.Domain);
			Exception ex = null;
			try
			{
				owaminiRecipient = recipientSession.FindMiniRecipientByProxyAddress<OWAMiniRecipient>(this.proxyAddress, OWAMiniRecipientSchema.AdditionalProperties);
				if (owaminiRecipient == null)
				{
					throw new OwaExplicitLogonException(string.Format("The address {0} is an object in AD database but it is not an user", this.proxyAddress), LocalizedStrings.GetNonEncoded(-1332692688), ex);
				}
			}
			catch (NonUniqueRecipientException ex2)
			{
				ex = ex2;
			}
			base.LastRecipientSessionDCServerName = recipientSession.LastUsedDc;
			if (owaminiRecipient == null || ex != null)
			{
				throw new OwaExplicitLogonException(string.Format("Couldn't find a match for {0}", this.proxyAddress.ToString()), string.Format(LocalizedStrings.GetNonEncoded(-13616305), this.proxyAddress), ex);
			}
			this.owaMiniRecipient = owaminiRecipient;
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
				return this.owaMiniRecipient.Sid;
			}
		}

		public override string GetLogonName()
		{
			base.ThrowNotSupported("LogonName");
			return null;
		}

		public override string SafeGetRenderableName()
		{
			return this.proxyAddress.ToString();
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
				base.ThrowNotSupported("AuthenticationType");
				return null;
			}
		}

		internal override ClientSecurityContext ClientSecurityContext
		{
			get
			{
				base.ThrowNotSupported("ClientSecurityContext");
				return null;
			}
		}

		public override bool IsPartial
		{
			get
			{
				return this.owaMiniRecipient == null;
			}
		}

		internal override ExchangePrincipal InternalCreateExchangePrincipal()
		{
			return ExchangePrincipal.FromMiniRecipient(this.owaMiniRecipient);
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

		internal override MailboxSession CreateMailboxSession(IExchangePrincipal exchangePrincipal, CultureInfo cultureInfo, HttpRequest clientRequest)
		{
			base.ThrowNotSupported("CreateMailboxSession");
			return null;
		}

		internal override MailboxSession CreateWebPartMailboxSession(IExchangePrincipal mailBoxExchangePrincipal, CultureInfo cultureInfo, HttpRequest clientRequest)
		{
			base.ThrowNotSupported("CreateWebPartMailboxSession");
			return null;
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

		public ProxyAddress ProxyAddress
		{
			get
			{
				return this.proxyAddress;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<OwaMiniRecipientIdentity>(this);
		}

		private ProxyAddress proxyAddress;
	}
}
