using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class AddressLists
	{
		public AddressLists(ClientSecurityContext clientSecurityContext, IExchangePrincipal exchangePrincipal, UserContext userContext)
		{
			if (clientSecurityContext == null)
			{
				throw new ArgumentNullException("clientSecurityContext");
			}
			if (exchangePrincipal == null)
			{
				throw new ArgumentNullException("exchangePrincipal");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			this.clientSecurityContext = clientSecurityContext;
			this.exchangePrincipal = exchangePrincipal;
			this.globalAddressListId = userContext.GlobalAddressListId;
			this.isModernGroupsFeatureEnabled = userContext.FeaturesManager.ClientServerSettings.ModernGroups.Enabled;
			this.configurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.exchangePrincipal.MailboxInfo.OrganizationId), 90, ".ctor", "f:\\15.00.1497\\sources\\dev\\clients\\src\\Owa2\\Server\\Core\\people\\AddressLists.cs");
			this.configurationContext = new ConfigurationContext(userContext);
		}

		public AddressBookBase GlobalAddressList
		{
			get
			{
				if (!this.globalAddressListLoaded)
				{
					if (this.ShouldFeatureBeVisible(Feature.GlobalAddressList))
					{
						this.globalAddressList = AddressBookBase.GetGlobalAddressList(this.clientSecurityContext, this.configurationSession, this.GetRecipientSession(), this.globalAddressListId);
					}
					this.globalAddressListLoaded = true;
				}
				return this.globalAddressList;
			}
		}

		public AddressBookBase AllRoomsAddressList
		{
			get
			{
				if (!this.allRoomsAddressListLoaded)
				{
					if (this.ShouldFeatureBeVisible(Feature.AddressLists))
					{
						this.allRoomsAddressList = AddressBookBase.GetAllRoomsAddressList(this.clientSecurityContext, this.configurationSession, this.exchangePrincipal.MailboxInfo.Configuration.AddressBookPolicy);
					}
					this.allRoomsAddressListLoaded = true;
				}
				return this.allRoomsAddressList;
			}
		}

		public IEnumerable<AddressBookBase> AllAddressLists
		{
			get
			{
				if (this.allAddressLists == null)
				{
					if (this.ShouldFeatureBeVisible(Feature.AddressLists))
					{
						this.allAddressLists = AddressBookBase.GetAllAddressLists(this.clientSecurityContext, this.configurationSession, this.exchangePrincipal.MailboxInfo.Configuration.AddressBookPolicy, this.isModernGroupsFeatureEnabled);
					}
					else
					{
						this.allAddressLists = Array<AddressBookBase>.Empty;
					}
				}
				return this.allAddressLists;
			}
		}

		private bool ShouldFeatureBeVisible(Feature feature)
		{
			return this.configurationContext.IsFeatureEnabled(feature);
		}

		private IRecipientSession GetRecipientSession()
		{
			ADSessionSettings sessionSettings;
			if (this.exchangePrincipal.MailboxInfo.Configuration.AddressBookPolicy != null)
			{
				sessionSettings = ADSessionSettings.FromOrganizationIdWithAddressListScopeServiceOnly(this.exchangePrincipal.MailboxInfo.OrganizationId, this.globalAddressListId);
			}
			else
			{
				sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.exchangePrincipal.MailboxInfo.OrganizationId);
			}
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, null, CultureInfo.CurrentCulture.LCID, true, ConsistencyMode.IgnoreInvalid, null, sessionSettings, 210, "GetRecipientSession", "f:\\15.00.1497\\sources\\dev\\clients\\src\\Owa2\\Server\\Core\\people\\AddressLists.cs");
		}

		private readonly ClientSecurityContext clientSecurityContext;

		private readonly IExchangePrincipal exchangePrincipal;

		private readonly IConfigurationSession configurationSession;

		private readonly ConfigurationContext configurationContext;

		private readonly bool isModernGroupsFeatureEnabled;

		private AddressBookBase globalAddressList;

		private ADObjectId globalAddressListId;

		private bool globalAddressListLoaded;

		private AddressBookBase allRoomsAddressList;

		private bool allRoomsAddressListLoaded;

		private IEnumerable<AddressBookBase> allAddressLists;
	}
}
