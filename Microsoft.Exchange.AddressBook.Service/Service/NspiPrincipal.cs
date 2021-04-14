using System;
using System.Globalization;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.AddressBook.Service;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal class NspiPrincipal
	{
		private NspiPrincipal(MiniRecipient miniRecipient)
		{
			this.LegacyDistinguishedName = miniRecipient.LegacyExchangeDN;
			if (string.IsNullOrEmpty(this.LegacyDistinguishedName))
			{
				this.LegacyDistinguishedName = LegacyDN.FormatLegacyDnFromGuid(Guid.Empty, (Guid)miniRecipient[ADObjectSchema.Guid]);
			}
			this.AddressBookPolicy = miniRecipient.AddressBookPolicy;
			this.OrganizationId = miniRecipient.OrganizationId;
			this.DirectorySearchRoot = miniRecipient.QueryBaseDN;
			this.PrimarySmtpAddress = miniRecipient.PrimarySmtpAddress;
			this.ExchangeGuid = miniRecipient.ExchangeGuid;
			this.MAPIEnabled = miniRecipient.MAPIEnabled;
			this.Database = miniRecipient.Database;
			this.ExchangeVersion = miniRecipient.ExchangeVersion;
			if (miniRecipient.Languages != null && miniRecipient.Languages.Count > 0)
			{
				this.PreferredCulture = miniRecipient.Languages[0];
			}
		}

		private NspiPrincipal(ADUser adUser)
		{
			this.LegacyDistinguishedName = adUser.LegacyExchangeDN;
			if (string.IsNullOrEmpty(this.LegacyDistinguishedName))
			{
				this.LegacyDistinguishedName = LegacyDN.FormatLegacyDnFromGuid(Guid.Empty, adUser.Guid);
			}
			this.AddressBookPolicy = adUser.AddressBookPolicy;
			this.OrganizationId = adUser.OrganizationId;
			this.DirectorySearchRoot = adUser.QueryBaseDN;
			this.PrimarySmtpAddress = adUser.PrimarySmtpAddress;
			this.ExchangeGuid = adUser.ExchangeGuid;
			this.MAPIEnabled = adUser.MAPIEnabled;
			this.Database = adUser.Database;
			this.ExchangeVersion = adUser.ExchangeVersion;
			if (adUser.Languages != null && adUser.Languages.Count > 0)
			{
				this.PreferredCulture = adUser.Languages[0];
			}
		}

		private NspiPrincipal(SecurityIdentifier sid)
		{
			this.LegacyDistinguishedName = "/SID=" + sid.ToString();
			this.OrganizationId = OrganizationId.ForestWideOrgId;
			this.MAPIEnabled = true;
		}

		public ADObjectId GlobalAddressListFromAddressBookPolicy
		{
			get
			{
				if (this.AddressBookPolicy != null && this.globalAddressListFromAddressBookPolicy == null)
				{
					this.PopulateDataFromAddressBookPolicy();
				}
				return this.globalAddressListFromAddressBookPolicy;
			}
		}

		public ADObjectId AllRoomsListFromAddressBookPolicy
		{
			get
			{
				if (this.AddressBookPolicy != null && this.allRoomsListFromAddressBookPolicy == null)
				{
					this.PopulateDataFromAddressBookPolicy();
				}
				return this.allRoomsListFromAddressBookPolicy;
			}
		}

		public ADObjectId AddressBookPolicy { get; private set; }

		public string LegacyDistinguishedName { get; private set; }

		public OrganizationId OrganizationId { get; private set; }

		public ADObjectId ConfigurationUnit
		{
			get
			{
				if (!(this.OrganizationId != null))
				{
					return null;
				}
				return this.OrganizationId.ConfigurationUnit;
			}
		}

		public ADObjectId DirectorySearchRoot { get; private set; }

		public SmtpAddress PrimarySmtpAddress { get; private set; }

		public Guid ExchangeGuid { get; private set; }

		public CultureInfo PreferredCulture { get; private set; }

		public bool MAPIEnabled { get; private set; }

		public ADObjectId Database { get; private set; }

		public ExchangeObjectVersion ExchangeVersion { get; private set; }

		public static NspiPrincipal FromUserSid(SecurityIdentifier sid, string userDomain)
		{
			NspiPrincipal principal = null;
			if (!string.IsNullOrEmpty(userDomain))
			{
				MiniRecipient miniRecipient = NspiPrincipal.FindMiniRecipientBySid(ADSessionSettings.RootOrgOrSingleTenantFromAcceptedDomainAutoDetect(userDomain), sid);
				if (miniRecipient != null)
				{
					principal = new NspiPrincipal(miniRecipient);
				}
			}
			else if (Configuration.IsDatacenter)
			{
				ExTraceGlobals.NspiTracer.TraceWarning<SecurityIdentifier>(0L, "We have to do a fan out query for user {0} because of legacy client.", sid);
				DirectoryHelper.DoAdCallAndTranslateExceptions(delegate
				{
					MiniRecipient miniRecipientFromUserId = PartitionDataAggregator.GetMiniRecipientFromUserId(sid);
					if (miniRecipientFromUserId != null)
					{
						principal = new NspiPrincipal(miniRecipientFromUserId);
					}
				}, "ADAccountPartitionLocator::GetAllAccountPartitionIds");
			}
			else
			{
				principal = NspiPrincipal.FromUserSid(ADSessionSettings.FromRootOrgScopeSet(), sid);
			}
			return principal ?? new NspiPrincipal(sid);
		}

		public static NspiPrincipal FromUserSid(ADSessionSettings sessionSettings, SecurityIdentifier sid)
		{
			MiniRecipient miniRecipient = NspiPrincipal.FindMiniRecipientBySid(sessionSettings, sid);
			if (miniRecipient != null)
			{
				return new NspiPrincipal(miniRecipient);
			}
			return new NspiPrincipal(sid);
		}

		public static NspiPrincipal FromADUser(ADUser adUser)
		{
			return new NspiPrincipal(adUser);
		}

		private static MiniRecipient FindMiniRecipientBySid(ADSessionSettings sessionSettings, SecurityIdentifier sid)
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.PartiallyConsistent, sessionSettings, 303, "FindMiniRecipientBySid", "f:\\15.00.1497\\sources\\dev\\DoMT\\src\\Service\\NspiPrincipal.cs");
			try
			{
				return tenantOrRootOrgRecipientSession.FindMiniRecipientBySid<MiniRecipient>(sid, null);
			}
			catch (NonUniqueRecipientException)
			{
			}
			catch (ObjectNotFoundException)
			{
			}
			return null;
		}

		private void PopulateDataFromAddressBookPolicy()
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.OrganizationId), 329, "PopulateDataFromAddressBookPolicy", "f:\\15.00.1497\\sources\\dev\\DoMT\\src\\Service\\NspiPrincipal.cs");
			if (tenantOrTopologyConfigurationSession != null)
			{
				AddressBookMailboxPolicy addressBookMailboxPolicy = tenantOrTopologyConfigurationSession.Read<AddressBookMailboxPolicy>(this.AddressBookPolicy);
				if (addressBookMailboxPolicy != null)
				{
					this.globalAddressListFromAddressBookPolicy = addressBookMailboxPolicy.GlobalAddressList;
					this.allRoomsListFromAddressBookPolicy = addressBookMailboxPolicy.RoomList;
				}
			}
		}

		private ADObjectId globalAddressListFromAddressBookPolicy;

		private ADObjectId allRoomsListFromAddressBookPolicy;
	}
}
