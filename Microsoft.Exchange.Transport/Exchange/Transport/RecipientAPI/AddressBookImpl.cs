using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.IsMemberOfProvider;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Configuration;
using Microsoft.Win32;

namespace Microsoft.Exchange.Transport.RecipientAPI
{
	internal class AddressBookImpl : TransportAddressBook
	{
		internal AddressBookImpl() : this(null)
		{
		}

		private OrganizationId OrganizationId
		{
			get
			{
				ADRecipientCache<TransportMiniRecipient> adrecipientCache = base.RecipientCache as ADRecipientCache<TransportMiniRecipient>;
				if (adrecipientCache == null)
				{
					return OrganizationId.ForestWideOrgId;
				}
				return adrecipientCache.OrganizationId ?? OrganizationId.ForestWideOrgId;
			}
		}

		internal AddressBookImpl(IIsMemberOfResolver<RoutingAddress> memberOfResolver)
		{
			if (memberOfResolver != null)
			{
				this.memberOfResolver = memberOfResolver;
				return;
			}
			this.memberOfResolver = Components.TransportIsMemberOfResolverComponent.IsMemberOfResolver;
		}

		internal static ExEventLog EventLog
		{
			get
			{
				return AddressBookImpl.eventLog;
			}
		}

		internal static bool UsingValidator
		{
			get
			{
				return null != AddressBookImpl.recipientValidator;
			}
		}

		public static bool UsingAdam()
		{
			if (AddressBookImpl.usingAdam == null)
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\EdgeTransportRole\\AdamSettings"))
				{
					AddressBookImpl.usingAdam = new bool?(null != registryKey);
				}
			}
			return AddressBookImpl.usingAdam.Value;
		}

		public override bool Contains(RoutingAddress smtpAddress)
		{
			if (!Utility.IsValidAddress(smtpAddress))
			{
				return false;
			}
			if (AddressBookImpl.RecipientDoesNotExist(smtpAddress))
			{
				return false;
			}
			ADRecipientCache<TransportMiniRecipient> cache = this.GetCache();
			ProxyAddress proxyAddress = AddressBookImpl.CreateProxyAddress(smtpAddress);
			Result<TransportMiniRecipient> recipientEntry = cache.FindAndCacheRecipient(proxyAddress);
			AddressBookImpl.LogRecipientDataValidationExceptionIfNeeded(recipientEntry, smtpAddress);
			return null != recipientEntry.Data;
		}

		public override AddressBookEntry Find(RoutingAddress smtpAddress)
		{
			if (!Utility.IsValidAddress(smtpAddress))
			{
				return null;
			}
			if (AddressBookImpl.RecipientDoesNotExist(smtpAddress))
			{
				return null;
			}
			ADRecipientCache<TransportMiniRecipient> cache = this.GetCache();
			ProxyAddress proxyAddress = AddressBookImpl.CreateProxyAddress(smtpAddress);
			Result<TransportMiniRecipient> recipientEntry = cache.FindAndCacheRecipient(proxyAddress);
			AddressBookImpl.LogRecipientDataValidationExceptionIfNeeded(recipientEntry, smtpAddress);
			if (recipientEntry.Data == null)
			{
				return null;
			}
			return AddressBookImpl.CreateAddressBookEntry(recipientEntry.Data, smtpAddress);
		}

		public override ReadOnlyCollection<AddressBookEntry> Find(params RoutingAddress[] smtpAddresses)
		{
			if (smtpAddresses == null)
			{
				throw new ArgumentNullException("smtpAddresses");
			}
			return this.Find(smtpAddresses.Length, (int index) => smtpAddresses[index]);
		}

		public override ReadOnlyCollection<AddressBookEntry> Find(EnvelopeRecipientCollection recipients)
		{
			if (recipients == null)
			{
				throw new ArgumentNullException("recipients");
			}
			return this.Find(recipients.Count, (int index) => recipients[index].Address);
		}

		public override bool IsMemberOf(RoutingAddress recipientSmtpAddress, RoutingAddress groupSmtpAddress)
		{
			if (recipientSmtpAddress.IsValid && groupSmtpAddress.IsValid)
			{
				ADRecipientCache<TransportMiniRecipient> cache = this.GetCache();
				ProxyAddress proxyAddress = AddressBookImpl.CreateProxyAddress(recipientSmtpAddress);
				Result<TransportMiniRecipient> recipientEntry = cache.FindAndCacheRecipient(proxyAddress);
				AddressBookImpl.LogRecipientDataValidationExceptionIfNeeded(recipientEntry, recipientSmtpAddress);
				if (recipientEntry.Data != null)
				{
					return this.memberOfResolver.IsMemberOf(cache.ADSession, recipientEntry.Data.Id, groupSmtpAddress);
				}
			}
			return false;
		}

		public override bool IsSameRecipient(RoutingAddress proxyAddressA, RoutingAddress proxyAddressB)
		{
			if (string.Equals((string)proxyAddressA, (string)proxyAddressB, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			if (!Utility.IsValidAddress(proxyAddressA) || !Utility.IsValidAddress(proxyAddressB))
			{
				return false;
			}
			if (AddressBookImpl.RecipientDoesNotExist(proxyAddressA) || AddressBookImpl.RecipientDoesNotExist(proxyAddressB))
			{
				return false;
			}
			ADRecipientCache<TransportMiniRecipient> cache = this.GetCache();
			ProxyAddress proxyAddress = AddressBookImpl.CreateProxyAddress(proxyAddressA);
			ProxyAddress proxyAddress2 = AddressBookImpl.CreateProxyAddress(proxyAddressB);
			bool flag = false;
			if (!cache.ContainsKey(proxyAddress) && cache.ContainsKey(proxyAddress2))
			{
				flag = true;
			}
			if (flag)
			{
				ProxyAddress proxyAddress3 = proxyAddress;
				proxyAddress = proxyAddress2;
				proxyAddress2 = proxyAddress3;
			}
			Result<TransportMiniRecipient> recipientEntry = cache.FindAndCacheRecipient(proxyAddress);
			AddressBookImpl.LogRecipientDataValidationExceptionIfNeeded(recipientEntry, flag ? proxyAddressB : proxyAddressA);
			if (recipientEntry.Data == null)
			{
				return false;
			}
			object obj = recipientEntry.Data[ADRecipientSchema.EmailAddresses];
			if (obj == null)
			{
				return false;
			}
			ProxyAddressCollection proxyAddressCollection = (ProxyAddressCollection)obj;
			foreach (ProxyAddress proxyAddress4 in proxyAddressCollection)
			{
				if (proxyAddress4.Equals(proxyAddress2))
				{
					return true;
				}
			}
			return false;
		}

		public override bool IsInternal(RoutingAddress address)
		{
			RoutingDomain domain;
			return RoutingDomain.TryParse(address.DomainPart, out domain) && this.IsInternal(domain);
		}

		public override bool IsInternal(RoutingDomain domain)
		{
			return this.IsInternalTo(domain, this.OrganizationId, false);
		}

		public override bool IsInternalTo(RoutingAddress address, RoutingAddress organizationAddress)
		{
			RoutingDomain organizationDomain;
			return RoutingDomain.TryParse(organizationAddress.DomainPart, out organizationDomain) && this.IsInternalTo(address, organizationDomain);
		}

		public override bool IsInternalTo(RoutingAddress address, RoutingDomain organizationDomain)
		{
			SmtpDomain domain;
			if (!SmtpDomain.TryParse(organizationDomain.Domain, out domain))
			{
				return false;
			}
			OrganizationId organizationId;
			if (Components.Configuration.FirstOrgAcceptedDomainTable.CheckAccepted(domain))
			{
				organizationId = OrganizationId.ForestWideOrgId;
			}
			else
			{
				organizationId = this.OrganizationId;
			}
			return this.IsInternalTo(address, organizationId, false);
		}

		public bool IsInternalTo(RoutingAddress address, OrganizationId organizationId, bool acceptedDomainsOnly = false)
		{
			RoutingDomain domain;
			return RoutingDomain.TryParse(address.DomainPart, out domain) && this.IsInternalTo(domain, organizationId, acceptedDomainsOnly);
		}

		public bool IsInternalTo(RoutingDomain domain, OrganizationId organizationId, bool acceptedDomainsOnly = false)
		{
			if (organizationId == null)
			{
				organizationId = this.OrganizationId;
			}
			return TransportIsInternalResolver.IsInternal(organizationId, domain, acceptedDomainsOnly);
		}

		public ADOperationResult TryGetIsInternal(RoutingAddress address, out bool isInternal)
		{
			return this.TryGetIsInternal(address, false, out isInternal);
		}

		public override ADOperationResult TryGetIsInternal(RoutingAddress address, bool acceptedDomainsOnly, out bool isInternal)
		{
			isInternal = false;
			bool tempIsInternal = false;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				tempIsInternal = this.IsInternalTo(address, this.OrganizationId, acceptedDomainsOnly);
			}, 0);
			if (adoperationResult.Succeeded)
			{
				isInternal = tempIsInternal;
			}
			return adoperationResult;
		}

		internal static void LogEvent(ExEventLog.EventTuple tuple, string periodicKey, params string[] messageArgs)
		{
			AddressBookImpl.eventLog.LogEvent(tuple, periodicKey, messageArgs);
		}

		internal static ProxyAddress CreateProxyAddress(RoutingAddress smtpAddress)
		{
			ProxyAddress result;
			if (AddressBookImpl.UsingAdam())
			{
				string address = null;
				lock (AddressBookImpl.stringHasher)
				{
					address = ProxyAddressHasher.GetHashedFormWithoutPrefix(AddressBookImpl.stringHasher, (string)smtpAddress);
				}
				result = new CustomProxyAddress(AddressBookImpl.smtpHashPrefix, address, false);
			}
			else
			{
				result = new SmtpProxyAddress((string)smtpAddress, false);
			}
			return result;
		}

		internal static AddressBookEntry CreateAddressBookEntry(TransportMiniRecipient entry, RoutingAddress address)
		{
			if (AddressBookImpl.UsingAdam())
			{
				return new AddressBookEntryImpl(entry, address);
			}
			return new AddressBookEntryImpl(entry);
		}

		private static RecipientValidator InitializeRecipientValidator(bool suppressDisposeTracker)
		{
			if (Components.Configuration.LocalServer.TransportServer.RecipientValidationCacheEnabled)
			{
				RecipientValidator recipientValidator = new RecipientValidator();
				if (recipientValidator.Initialize(suppressDisposeTracker))
				{
					return recipientValidator;
				}
			}
			return null;
		}

		private static bool RecipientDoesNotExist(RoutingAddress smtpAddress)
		{
			return AddressBookImpl.recipientValidator != null && AddressBookImpl.recipientValidator.RecipientDoesNotExist(smtpAddress);
		}

		private static void LogRecipientDataValidationExceptionIfNeeded(Result<TransportMiniRecipient> recipientEntry, RoutingAddress recipientRoutingAddress)
		{
			ValidationError validationError = recipientEntry.Error as ValidationError;
			if (validationError != null)
			{
				AddressBookImpl.eventLog.LogEvent(TransportEventLogConstants.Tuple_RecipientHasDataValidationException, recipientRoutingAddress.ToString(), new object[]
				{
					recipientRoutingAddress.ToString(),
					recipientEntry.Error
				});
			}
		}

		private ReadOnlyCollection<AddressBookEntry> Find(int count, GetAddress getAddress)
		{
			List<ProxyAddress> list = new List<ProxyAddress>(count);
			for (int i = 0; i < count; i++)
			{
				RoutingAddress routingAddress = getAddress(i);
				if (!Utility.IsValidAddress(routingAddress) || AddressBookImpl.RecipientDoesNotExist(routingAddress))
				{
					list.Add(null);
				}
				else
				{
					ProxyAddress item = AddressBookImpl.CreateProxyAddress(routingAddress);
					list.Add(item);
				}
			}
			ADRecipientCache<TransportMiniRecipient> cache = this.GetCache();
			IList<Result<TransportMiniRecipient>> list2 = cache.FindAndCacheRecipients(list);
			AddressBookEntry[] array = new AddressBookEntryImpl[list.Count];
			for (int j = 0; j < count; j++)
			{
				TransportMiniRecipient data = list2[j].Data;
				AddressBookImpl.LogRecipientDataValidationExceptionIfNeeded(list2[j], getAddress(j));
				if (data != null)
				{
					RoutingAddress address = getAddress(j);
					array[j] = AddressBookImpl.CreateAddressBookEntry(data, address);
				}
			}
			return new ReadOnlyCollection<AddressBookEntry>(array);
		}

		private ADRecipientCache<TransportMiniRecipient> GetCache()
		{
			if (base.RecipientCache != null && base.RecipientCache is ADRecipientCache<TransportMiniRecipient>)
			{
				return (ADRecipientCache<TransportMiniRecipient>)base.RecipientCache;
			}
			return new ADRecipientCache<TransportMiniRecipient>(RecipientSchema.PropertyDefinitions, 5);
		}

		private static readonly ExEventLog eventLog = new ExEventLog(new Guid("8cd349b7-795a-47f7-b99e-6f6a7fb399e1"), TransportEventLog.GetEventSource());

		private static readonly RecipientValidator recipientValidator = AddressBookImpl.InitializeRecipientValidator(true);

		private static readonly CustomProxyAddressPrefix smtpHashPrefix = new CustomProxyAddressPrefix("sh", "SMTP hash");

		private static bool? usingAdam;

		private static readonly StringHasher stringHasher = new StringHasher();

		private readonly IIsMemberOfResolver<RoutingAddress> memberOfResolver;
	}
}
