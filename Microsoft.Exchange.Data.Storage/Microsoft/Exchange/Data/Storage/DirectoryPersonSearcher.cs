using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class DirectoryPersonSearcher : IDirectoryPersonSearcher
	{
		public DirectoryPersonSearcher(IExchangePrincipal exchangePrincipal)
		{
			ADSessionSettings adsessionSettings = DirectoryPersonSearcher.GetADSessionSettings(exchangePrincipal);
			if (adsessionSettings != null)
			{
				this.recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.PartiallyConsistent, adsessionSettings, 70, ".ctor", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\Contacts\\DirectoryPersonSearcher.cs");
			}
		}

		public ADRawEntry FindByAdObjectIdGuidOrSmtpAddressCache(Guid adObjectIdGuid, string[] smtpAddressCache, IEnumerable<PropertyDefinition> adProperties)
		{
			ArgumentValidator.ThrowIfEmpty("adObjectIdGuid", adObjectIdGuid);
			ArgumentValidator.ThrowIfNull("adProperties", adProperties);
			if (this.recipientSession == null)
			{
				DirectoryPersonSearcher.Tracer.TraceError(0L, "No recipient session available to perform search");
				return null;
			}
			ADObjectId entryId = new ADObjectId(adObjectIdGuid);
			ADRawEntry adrawEntry = this.recipientSession.ReadADRawEntry(entryId, adProperties);
			if (adrawEntry != null)
			{
				DirectoryPersonSearcher.Tracer.TraceDebug<Guid>(0L, "Found AD entry for AD Object Id Guid:{0}.", adObjectIdGuid);
				return adrawEntry;
			}
			DirectoryPersonSearcher.Tracer.TraceDebug<Guid>(0L, "Nothing found for AD Object Id:{0}, trying with smtp address cache.", adObjectIdGuid);
			if (smtpAddressCache == null || smtpAddressCache.Length == 0)
			{
				DirectoryPersonSearcher.Tracer.TraceDebug<Guid>(0L, "Nothing in SMTP Address Cache to fallback to in order to find AD Object with Id: {0}.", adObjectIdGuid);
				return null;
			}
			QueryFilter filterForFindByEmailAddress = DirectoryPersonSearcher.GetFilterForFindByEmailAddress(string.Empty, smtpAddressCache);
			ADRawEntry[] array = DirectoryPersonSearcher.Find(this.recipientSession, filterForFindByEmailAddress, 1, adProperties);
			if (array == null || array.Length == 0)
			{
				DirectoryPersonSearcher.Tracer.TraceDebug(0L, "Found no matching recipient in AD for the smtp address cache.");
				return null;
			}
			return array[0];
		}

		public ADRecipient FindADRecipientByObjectId(Guid adObjectIdGuid)
		{
			ArgumentValidator.ThrowIfEmpty("adObjectIdGuid", adObjectIdGuid);
			if (this.recipientSession == null)
			{
				DirectoryPersonSearcher.Tracer.TraceError(0L, "No recipient session available to perform search");
				return null;
			}
			ADObjectId adobjectId = new ADObjectId(adObjectIdGuid);
			ADRecipient adrecipient = this.recipientSession.FindByObjectGuid(adobjectId.ObjectGuid);
			if (adrecipient == null)
			{
				DirectoryPersonSearcher.Tracer.TraceDebug<Guid>(0L, "Nothing found for AD Object Id:{0}.", adObjectIdGuid);
				return null;
			}
			return adrecipient;
		}

		public bool TryFind(ContactInfoForLinking contactInfo, out ContactInfoForLinkingFromDirectory matchingContactInfo)
		{
			Util.ThrowOnNullArgument(contactInfo, "contactInfo");
			if (this.recipientSession == null)
			{
				DirectoryPersonSearcher.Tracer.TraceError(0L, "No recipient session available to perform search");
				matchingContactInfo = null;
				return false;
			}
			ADRawEntry adrawEntry = DirectoryPersonSearcher.TryFind(this.recipientSession, DirectoryPersonSearcher.GetFilterForFindByAddresses(contactInfo));
			if (adrawEntry != null)
			{
				ContactInfoForLinkingFromDirectory contactInfoForLinkingFromDirectory = ContactInfoForLinkingFromDirectory.Create(adrawEntry);
				matchingContactInfo = contactInfoForLinkingFromDirectory;
				return true;
			}
			matchingContactInfo = null;
			return false;
		}

		private static QueryFilter GetFilterForFindByAddresses(ContactInfoForLinking contactInfo)
		{
			List<QueryFilter> list = new List<QueryFilter>(contactInfo.EmailAddresses.Count + 1);
			if (contactInfo.EmailAddresses.Count > 0)
			{
				DirectoryPersonSearcher.AddFilterForEachEmailAddress("smtp:", contactInfo.EmailAddresses, list);
			}
			if (!string.IsNullOrEmpty(contactInfo.IMAddress))
			{
				list.Add(DirectoryPersonSearcher.GetFilterForFindByImAddress(contactInfo.IMAddress));
			}
			if (list.Count == 0)
			{
				return null;
			}
			if (list.Count == 1)
			{
				return list[0];
			}
			return new OrFilter(list.ToArray());
		}

		private static QueryFilter GetFilterForFindByImAddress(string imAddress)
		{
			if (!imAddress.StartsWith("sip:", StringComparison.OrdinalIgnoreCase))
			{
				imAddress = "sip:" + imAddress;
			}
			return new ComparisonFilter(ComparisonOperator.Equal, ADUserSchema.RTCSIPPrimaryUserAddress, imAddress);
		}

		private static QueryFilter GetFilterForFindByEmailAddress(string prefix, ICollection<string> emailAddresses)
		{
			List<QueryFilter> list = new List<QueryFilter>(emailAddresses.Count);
			DirectoryPersonSearcher.AddFilterForEachEmailAddress(prefix, emailAddresses, list);
			if (list.Count == 0)
			{
				return null;
			}
			if (list.Count == 1)
			{
				return list[0];
			}
			return new OrFilter(list.ToArray());
		}

		private static void AddFilterForEachEmailAddress(string prefix, ICollection<string> emailAddresses, List<QueryFilter> matchByEmailAddressList)
		{
			foreach (string str in emailAddresses)
			{
				matchByEmailAddressList.Add(new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.EmailAddresses, prefix + str));
			}
		}

		private static ADRawEntry[] Find(IRecipientSession recipientSession, QueryFilter filter, int maxResults, IEnumerable<PropertyDefinition> properties)
		{
			if (filter == null)
			{
				return null;
			}
			QueryFilter filter2 = new AndFilter(new QueryFilter[]
			{
				DirectoryPersonSearcher.BaseFilter,
				filter
			});
			DirectoryPersonSearcher.Tracer.TraceDebug<QueryFilter>(0L, "Searching for matching recipient in AD with filter: {0}", filter);
			return recipientSession.Find(null, QueryScope.SubTree, filter2, null, maxResults, properties);
		}

		private static ADRawEntry TryFind(IRecipientSession recipientSession, QueryFilter filter)
		{
			ADRawEntry[] array = null;
			try
			{
				array = DirectoryPersonSearcher.Find(recipientSession, filter, 2, ContactInfoForLinkingFromDirectory.RequiredADProperties);
			}
			catch (ADTransientException arg)
			{
				DirectoryPersonSearcher.Tracer.TraceError<ADTransientException>(0L, "Unable to find recipient in AD due exception ", arg);
				return null;
			}
			if (array == null || array.Length == 0)
			{
				DirectoryPersonSearcher.Tracer.TraceDebug(0L, "Found no matching recipient in AD");
				return null;
			}
			if (array.Length > 1)
			{
				DirectoryPersonSearcher.Tracer.TraceDebug(0L, "Found more than one matching recipient in AD");
				return null;
			}
			return array[0];
		}

		private static ADSessionSettings GetADSessionSettings(IExchangePrincipal exchangePrincipal)
		{
			OrganizationId organizationId = exchangePrincipal.MailboxInfo.OrganizationId;
			if (organizationId == null)
			{
				organizationId = OrganizationId.ForestWideOrgId;
			}
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId);
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.PartiallyConsistent, sessionSettings, 367, "GetADSessionSettings", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\Contacts\\DirectoryPersonSearcher.cs");
			DirectoryPersonSearcher.Tracer.TraceDebug<Guid, OrganizationId>(0L, "Searching for mailbox {0} in organization {1}", exchangePrincipal.MailboxInfo.MailboxGuid, organizationId);
			ADUser aduser = tenantOrRootOrgRecipientSession.FindByExchangeGuid(exchangePrincipal.MailboxInfo.MailboxGuid) as ADUser;
			if (aduser == null)
			{
				DirectoryPersonSearcher.Tracer.TraceError(0L, "Unable to get FromOrganizationIdWithAddressListScopeServiceOnly because ExchangePrincipal is not complete.");
				return null;
			}
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 380, "GetADSessionSettings", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\Contacts\\DirectoryPersonSearcher.cs");
			AddressBookMailboxPolicy addressBookMailboxPolicy = null;
			if (aduser.AddressBookPolicy != null)
			{
				addressBookMailboxPolicy = tenantOrTopologyConfigurationSession.Read<AddressBookMailboxPolicy>(aduser.AddressBookPolicy);
			}
			ADObjectId adobjectId;
			if (addressBookMailboxPolicy != null)
			{
				adobjectId = addressBookMailboxPolicy.GlobalAddressList;
			}
			else
			{
				adobjectId = null;
			}
			if (adobjectId != null)
			{
				DirectoryPersonSearcher.Tracer.TraceDebug<ADObjectId>(0L, "Using GAL from ABP {0} for directory search.", adobjectId);
				return ADSessionSettings.FromOrganizationIdWithAddressListScope(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), organizationId, adobjectId, null);
			}
			DirectoryPersonSearcher.Tracer.TraceDebug(0L, "Using organization scope for directory search.");
			return ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId);
		}

		private const string SmtpAddressPrefix = "smtp:";

		private const string SipAddressPrefix = "sip:";

		private static readonly Trace Tracer = ExTraceGlobals.ContactLinkingTracer;

		private static readonly QueryFilter BaseFilter = new AndFilter(new QueryFilter[]
		{
			new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, "person"),
			new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.HiddenFromAddressListsEnabled, false),
			new ExistsFilter(ADRecipientSchema.AddressListMembership)
		});

		private readonly IRecipientSession recipientSession;
	}
}
