using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class MailboxLocator : IMailboxLocator
	{
		protected MailboxLocator(IRecipientSession adSession)
		{
			ArgumentValidator.ThrowIfNull("adSession", adSession);
			this.adSession = adSession;
		}

		protected MailboxLocator(IRecipientSession adSession, string externalDirectoryObjectId, string legacyDn) : this(adSession)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("legacyDn", legacyDn);
			this.externalDirectoryObjectId = externalDirectoryObjectId;
			this.legacyDn = legacyDn;
		}

		public abstract string LocatorType { get; }

		public string ExternalId
		{
			get
			{
				return this.externalDirectoryObjectId;
			}
			set
			{
				this.externalDirectoryObjectId = value;
			}
		}

		public string LegacyDn
		{
			get
			{
				return this.legacyDn;
			}
			set
			{
				ArgumentValidator.ThrowIfNullOrWhiteSpace("value", value);
				this.legacyDn = value;
			}
		}

		public Guid MailboxGuid
		{
			get
			{
				Guid result = Guid.Empty;
				if (this.adUser != null)
				{
					result = this.adUser.ExchangeGuid;
				}
				else
				{
					MailboxLocator.Tracer.TraceError((long)this.GetHashCode(), "MailboxLocator::get_MailboxGuid: adUser was null at the time");
				}
				return result;
			}
		}

		public string IdentityHash
		{
			get
			{
				if (this.identityHash == null)
				{
					using (SHA1 sha = SHA1.Create())
					{
						string s = (this.legacyDn + this.externalDirectoryObjectId).ToLower();
						byte[] bytes = Encoding.Unicode.GetBytes(s);
						byte[] inArray = sha.ComputeHash(bytes);
						this.identityHash = Convert.ToBase64String(inArray);
					}
				}
				return this.identityHash;
			}
		}

		public ADUser FindAdUser()
		{
			if (this.adUser == null)
			{
				try
				{
					ADUser aduser = this.FindByLegacyDN();
					this.InitializeFromAd(aduser);
				}
				catch (NonUniqueRecipientException innerException)
				{
					throw new MailboxNotFoundException(ServerStrings.NonUniqueRecipientError, innerException);
				}
			}
			return this.adUser;
		}

		public string[] FindAlternateLegacyDNs()
		{
			return (from address in this.FindAdUser().EmailAddresses
			where address.Prefix == ProxyAddressPrefix.X500
			select address.AddressString).ToArray<string>();
		}

		public virtual bool IsValidReplicationTarget()
		{
			throw new NotImplementedException("Replication to this locator is not yet supported");
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append("{Type:\"");
			stringBuilder.Append(this.LocatorType);
			stringBuilder.Append("\", ExternalDirectoryObjectId:\"");
			stringBuilder.Append(this.ExternalId);
			stringBuilder.Append("\"}");
			return stringBuilder.ToString();
		}

		protected abstract bool IsValidAdUser(ADUser adUser);

		protected void InitializeFromAd(ProxyAddress proxyAddress)
		{
			ArgumentValidator.ThrowIfNull("smtpAddress", proxyAddress);
			ADUser aduser = this.adSession.FindByProxyAddress(proxyAddress) as ADUser;
			if (aduser == null)
			{
				throw new MailboxNotFoundException(ServerStrings.InvalidAddressError(proxyAddress.AddressString));
			}
			this.InitializeFromAd(aduser);
		}

		protected void InitializeFromAd(ADUser adUser)
		{
			ArgumentValidator.ThrowIfNull("adUser", adUser);
			if (!this.IsValidAdUser(adUser))
			{
				throw new MailboxWrongTypeException(adUser.ExternalDirectoryObjectId, adUser.RecipientTypeDetails.ToString());
			}
			this.adUser = adUser;
			this.externalDirectoryObjectId = adUser.ExternalDirectoryObjectId;
			this.legacyDn = adUser.LegacyExchangeDN;
		}

		private ADUser FindByLegacyDN()
		{
			MailboxLocator.Tracer.TraceDebug<string>((long)this.GetHashCode(), "MailboxLocator::FindByLegacyDN. Retrieving AD User by LegacyDN={0}", this.legacyDn);
			ADUser aduser = this.adSession.FindByLegacyExchangeDN(this.LegacyDn) as ADUser;
			if (aduser == null)
			{
				throw new MailboxNotFoundException(ServerStrings.InvalidAddressError(this.LegacyDn));
			}
			return aduser;
		}

		protected static readonly Trace Tracer = ExTraceGlobals.MailboxLocatorTracer;

		private readonly IRecipientSession adSession;

		private string externalDirectoryObjectId;

		private string legacyDn;

		private ADUser adUser;

		private string identityHash;
	}
}
