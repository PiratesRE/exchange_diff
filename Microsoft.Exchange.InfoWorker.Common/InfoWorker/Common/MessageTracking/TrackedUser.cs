using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class TrackedUser
	{
		private TrackedUser()
		{
		}

		private TrackedUser(string address)
		{
			this.smtpAddress = new SmtpAddress(address);
			this.proxyAddresses = new ProxyAddressCollection();
			this.proxyAddresses.Add(address);
			this.PopulateProxyHashSet();
		}

		private TrackedUser(ADRecipient mailbox)
		{
			if (mailbox.ExternalEmailAddress != null && mailbox.ExternalEmailAddress is SmtpProxyAddress)
			{
				this.smtpAddress = new SmtpAddress(mailbox.ExternalEmailAddress.AddressString);
			}
			else
			{
				SmtpAddress primarySmtpAddress = mailbox.PrimarySmtpAddress;
				if (!mailbox.PrimarySmtpAddress.IsValidAddress)
				{
					throw new TrackedUserCreationException("PrimarySmtpAddress is invalid: {0}", new object[]
					{
						mailbox.PrimarySmtpAddress
					});
				}
				this.smtpAddress = mailbox.PrimarySmtpAddress;
			}
			if (mailbox.EmailAddresses == null || mailbox.EmailAddresses.Count == 0)
			{
				throw new TrackedUserCreationException("No EmailAddresses", new object[0]);
			}
			ProxyAddressCollection proxyAddressCollection = mailbox.EmailAddresses;
			foreach (ProxyAddress proxyAddress in mailbox.EmailAddresses)
			{
				if (proxyAddress is InvalidProxyAddress)
				{
					TraceWrapper.SearchLibraryTracer.TraceError(this.GetHashCode(), "Invalid proxies found, will use only valid ones", new object[0]);
					IEnumerable<ProxyAddress> source = from proxy in mailbox.EmailAddresses
					where !(proxy is InvalidProxyAddress)
					select proxy;
					proxyAddressCollection = new ProxyAddressCollection(source.ToArray<ProxyAddress>());
					break;
				}
			}
			if (proxyAddressCollection.Count == 0)
			{
				throw new TrackedUserCreationException("All EmailAddresses discarded as invalid", new object[0]);
			}
			this.proxyAddresses = proxyAddressCollection;
			this.adRecipient = mailbox;
			this.readStatusTrackingEnabled = !(bool)mailbox[ADRecipientSchema.MessageTrackingReadStatusDisabled];
			this.PopulateProxyHashSet();
		}

		public static TrackedUser CreateUnresolved(string smtpAddress)
		{
			return new TrackedUser(smtpAddress);
		}

		public static TrackedUser Create(ADUser user)
		{
			try
			{
				return new TrackedUser(user);
			}
			catch (TrackedUserCreationException arg)
			{
				TraceWrapper.SearchLibraryTracer.TraceError<TrackedUserCreationException>(0, "TrackedUserCreationException initializing from ADRecipient: {0}", arg);
			}
			return null;
		}

		public static TrackedUser Create(string smtpAddress, IRecipientSession galSession)
		{
			ProxyAddress proxyAddress = ProxyAddress.Parse(smtpAddress);
			ADRecipient adrecipient = null;
			try
			{
				adrecipient = galSession.FindByProxyAddress(proxyAddress);
			}
			catch (NonUniqueRecipientException arg)
			{
				TraceWrapper.SearchLibraryTracer.TraceError<ProxyAddress, NonUniqueRecipientException>(0, "Create from SmtpAddress: Caught NonUniqueRecipientException when attempting to look up user for address {0}, exception: {1}", proxyAddress, arg);
				return null;
			}
			if (adrecipient != null)
			{
				try
				{
					return new TrackedUser(adrecipient);
				}
				catch (TrackedUserCreationException arg2)
				{
					TraceWrapper.SearchLibraryTracer.TraceError<string, TrackedUserCreationException>(0, "Create from SmtpAddress: TrackedUserCreationException initializing from ADRecipient: {0}, {1}", smtpAddress, arg2);
					return null;
				}
			}
			return new TrackedUser(smtpAddress);
		}

		public static TrackedUser Create(Guid guid, IRecipientSession galSession)
		{
			ADRecipient adrecipient = null;
			try
			{
				adrecipient = galSession.FindByExchangeGuid(guid);
			}
			catch (NonUniqueRecipientException arg)
			{
				TraceWrapper.SearchLibraryTracer.TraceError<Guid, NonUniqueRecipientException>(0, "Create from GUID: Caught NonUniqueRecipientException when attempting to look up server for guid {0}, exception: {1}", guid, arg);
				return null;
			}
			if (adrecipient != null)
			{
				try
				{
					return new TrackedUser(adrecipient);
				}
				catch (TrackedUserCreationException arg2)
				{
					TraceWrapper.SearchLibraryTracer.TraceError<Guid, TrackedUserCreationException>(0, "Create from GUID: TrackedUserCreationException initializing from ADRecipient: {0}, {1}", guid, arg2);
				}
			}
			return null;
		}

		public SmtpAddress SmtpAddress
		{
			get
			{
				return this.smtpAddress;
			}
		}

		public ProxyAddressCollection ProxyAddresses
		{
			get
			{
				return this.proxyAddresses;
			}
		}

		public ADUser ADUser
		{
			get
			{
				return this.adRecipient as ADUser;
			}
		}

		public ADRecipient ADRecipient
		{
			get
			{
				return this.adRecipient;
			}
		}

		public bool IsArbitrationMailbox
		{
			get
			{
				return this.adRecipient != null && this.adRecipient.RecipientTypeDetails == RecipientTypeDetails.ArbitrationMailbox;
			}
		}

		public bool IsMailbox
		{
			get
			{
				return this.adRecipient != null && this.adRecipient.RecipientType == RecipientType.UserMailbox;
			}
		}

		public bool IsSupportedForTrackingAsSender
		{
			get
			{
				return this.adRecipient == null || this.adRecipient.RecipientType == RecipientType.MailUser || this.adRecipient.RecipientType == RecipientType.MailContact || this.adRecipient.RecipientType == RecipientType.UserMailbox;
			}
		}

		public string DisplayName
		{
			get
			{
				if (this.adRecipient != null && !string.IsNullOrEmpty(this.adRecipient.DisplayName))
				{
					return this.adRecipient.DisplayName;
				}
				return this.smtpAddress.ToString();
			}
		}

		public bool ReadStatusTrackingEnabled
		{
			get
			{
				return this.readStatusTrackingEnabled;
			}
		}

		public bool IsAddressOneOfProxies(string address)
		{
			return this.proxyAddressesHashset.Contains(address);
		}

		private void PopulateProxyHashSet()
		{
			this.proxyAddressesHashset = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			this.proxyAddressesHashset.Add((string)this.smtpAddress);
			foreach (ProxyAddress proxyAddress in this.proxyAddresses)
			{
				this.proxyAddressesHashset.Add(proxyAddress.AddressString);
			}
		}

		private SmtpAddress smtpAddress;

		private ProxyAddressCollection proxyAddresses;

		private ADRecipient adRecipient;

		private bool readStatusTrackingEnabled;

		private HashSet<string> proxyAddressesHashset;
	}
}
