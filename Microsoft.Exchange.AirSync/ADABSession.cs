using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ABProviderFramework;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory.ABProviderFramework;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.AirSync
{
	internal sealed class ADABSession : ABSession
	{
		public ADABSession(OrganizationId organizationId, ADObjectId searchRoot, int lcid, ConsistencyMode consistencyMode, ClientSecurityContext clientSecurityContext) : base(ExTraceGlobals.ActiveDirectoryTracer)
		{
			this.organizationId = organizationId;
			this.searchRoot = searchRoot;
			this.lcid = lcid;
			this.consistencyMode = consistencyMode;
			this.clientSecurityContext = clientSecurityContext;
			this.recipientSession = null;
			this.configurationSession = null;
		}

		public static ABSession Create(IABSessionSettings sessionSettings)
		{
			if (sessionSettings == null)
			{
				throw new ArgumentNullException("sessionSettings");
			}
			ADABSession adabsession = null;
			bool flag = false;
			try
			{
				adabsession = new ADABSession(sessionSettings.Get<OrganizationId>("OrganizationId"), sessionSettings.Get<ADObjectId>("SearchRoot"), sessionSettings.Get<int>("Lcid"), sessionSettings.Get<ConsistencyMode>("ConsistencyMode"), sessionSettings.Get<ClientSecurityContext>("ClientSecurityContext"));
				flag = true;
			}
			finally
			{
				if (!flag && adabsession != null)
				{
					adabsession.Dispose();
					adabsession = null;
				}
			}
			return adabsession;
		}

		public override ABProviderCapabilities ProviderCapabilities
		{
			get
			{
				return ADABSession.providerCapabilities;
			}
		}

		protected override string InternalProviderName
		{
			get
			{
				return "AD";
			}
		}

		private IConfigurationSession ConfigurationSession
		{
			get
			{
				if (this.configurationSession == null)
				{
					this.configurationSession = this.CreateConfigurationSession();
				}
				return this.configurationSession;
			}
		}

		private IRecipientSession RecipientSession
		{
			get
			{
				if (this.recipientSession == null)
				{
					if (this.searchRoot != null)
					{
						this.recipientSession = this.CreateRecipientSession();
					}
					else
					{
						this.recipientSession = this.CreateGalScopedRecipientSession(this.FindGlobalAddressList());
					}
				}
				return this.recipientSession;
			}
		}

		internal void SetRecipientSessionForTesting(IRecipientSession recipientSession)
		{
			this.recipientSession = recipientSession;
		}

		protected override ABObject InternalFindById(ABObjectId id)
		{
			ADABObjectId adabobjectId = (ADABObjectId)id;
			ADRecipient recipient;
			try
			{
				recipient = this.RecipientSession.Read(adabobjectId.NativeId);
			}
			catch (DataSourceOperationException ex)
			{
				throw new ABOperationException(ex.LocalizedString, ex);
			}
			catch (DataSourceTransientException ex2)
			{
				throw new ABTransientException(ex2.LocalizedString, ex2);
			}
			return this.RecipientToABObject(recipient);
		}

		protected override ABRawEntry InternalFindById(ABObjectId id, ABPropertyDefinitionCollection properties)
		{
			ADABObjectId adabobjectId = (ADABObjectId)id;
			ADRawEntry rawEntry;
			try
			{
				rawEntry = this.recipientSession.ReadADRawEntry(adabobjectId.NativeId, ADABPropertyMapper.ConvertToADProperties(properties));
			}
			catch (DataSourceOperationException ex)
			{
				throw new ABOperationException(ex.LocalizedString, ex);
			}
			catch (DataSourceTransientException ex2)
			{
				throw new ABTransientException(ex2.LocalizedString, ex2);
			}
			return new ADABRawEntry(this, properties, rawEntry);
		}

		protected override IList<ABRawEntry> InternalFindByIds(ICollection<ABObjectId> ids, ABPropertyDefinitionCollection properties)
		{
			ADObjectId[] array = new ADObjectId[ids.Count];
			int num = 0;
			foreach (ABObjectId abobjectId in ids)
			{
				ADABObjectId adabobjectId = (ADABObjectId)abobjectId;
				array[num++] = adabobjectId.NativeId;
			}
			Result<ADRawEntry>[] activeDirectoryRawEntries;
			try
			{
				activeDirectoryRawEntries = this.recipientSession.ReadMultiple(array, ADABPropertyMapper.ConvertToADProperties(properties));
			}
			catch (DataSourceOperationException ex)
			{
				throw new ABOperationException(ex.LocalizedString, ex);
			}
			catch (DataSourceTransientException ex2)
			{
				throw new ABTransientException(ex2.LocalizedString, ex2);
			}
			return this.ADRawEntryResultsToABRawEntries(properties, activeDirectoryRawEntries);
		}

		protected override ABObject InternalFindByProxyAddress(ProxyAddress proxyAddress)
		{
			ADRecipient recipient;
			try
			{
				recipient = this.RecipientSession.FindByProxyAddress(proxyAddress);
			}
			catch (DataSourceOperationException ex)
			{
				throw new ABOperationException(ex.LocalizedString, ex);
			}
			catch (DataSourceTransientException ex2)
			{
				throw new ABTransientException(ex2.LocalizedString, ex2);
			}
			return this.RecipientToABObject(recipient);
		}

		protected override ABObject InternalFindByLegacyExchangeDN(string legacyExchangeDN)
		{
			ADRecipient recipient;
			try
			{
				recipient = this.RecipientSession.FindByLegacyExchangeDN(legacyExchangeDN);
			}
			catch (DataSourceOperationException ex)
			{
				throw new ABOperationException(ex.LocalizedString, ex);
			}
			catch (DataSourceTransientException ex2)
			{
				throw new ABTransientException(ex2.LocalizedString, ex2);
			}
			return this.RecipientToABObject(recipient);
		}

		protected override List<ABObject> InternalFindByANR(string anrMatch, int maxResults)
		{
			ADRecipient[] recipients;
			try
			{
				recipients = this.RecipientSession.FindByANR(anrMatch, maxResults, ADABSession.sortByDisplayName);
			}
			catch (DataSourceOperationException ex)
			{
				throw new ABOperationException(ex.LocalizedString, ex);
			}
			catch (DataSourceTransientException ex2)
			{
				throw new ABTransientException(ex2.LocalizedString, ex2);
			}
			return this.RecipientsToABObjects(recipients);
		}

		protected override List<ABRawEntry> InternalFindByANR(string anrMatch, int maxResults, ABPropertyDefinitionCollection properties)
		{
			ADRecipient[] recipients;
			try
			{
				recipients = this.recipientSession.FindByANR(anrMatch, maxResults, ADABSession.sortByDisplayName);
			}
			catch (DataSourceOperationException ex)
			{
				throw new ABOperationException(ex.LocalizedString, ex);
			}
			catch (DataSourceTransientException ex2)
			{
				throw new ABTransientException(ex2.LocalizedString, ex2);
			}
			return this.RecipientsToABRawEntries(recipients, properties);
		}

		private ABRawEntry RecipientToABRawEntry(ADRecipient recipient, ABPropertyDefinitionCollection properties)
		{
			return new ADABRawEntry(this, properties, recipient);
		}

		private List<ABObject> RecipientsToABObjects(ICollection<ADRecipient> recipients)
		{
			List<ABObject> list = new List<ABObject>(recipients.Count);
			foreach (ADRecipient recipient in recipients)
			{
				list.Add(this.RecipientToABObject(recipient));
			}
			return list;
		}

		private List<ABRawEntry> RecipientsToABRawEntries(ICollection<ADRecipient> recipients, ABPropertyDefinitionCollection properties)
		{
			List<ABRawEntry> list = new List<ABRawEntry>(recipients.Count);
			foreach (ADRecipient recipient in recipients)
			{
				list.Add(this.RecipientToABRawEntry(recipient, properties));
			}
			return list;
		}

		private ABObject RecipientToABObject(ADRecipient recipient)
		{
			if (recipient == null)
			{
				return null;
			}
			switch (recipient.RecipientType)
			{
			default:
				return null;
			case RecipientType.User:
			case RecipientType.UserMailbox:
			case RecipientType.MailUser:
			case RecipientType.Contact:
			case RecipientType.MailContact:
			case RecipientType.PublicFolder:
			case RecipientType.SystemAttendantMailbox:
			case RecipientType.SystemMailbox:
			case RecipientType.MicrosoftExchange:
				return new ADABContact(this, recipient);
			case RecipientType.Group:
			case RecipientType.MailUniversalDistributionGroup:
			case RecipientType.MailUniversalSecurityGroup:
			case RecipientType.MailNonUniversalGroup:
				return new ADABGroup(this, (ADGroup)recipient);
			case RecipientType.DynamicDistributionGroup:
				return new ADABGroup(this, (ADDynamicGroup)recipient);
			}
		}

		private List<ABRawEntry> ADRawEntryResultsToABRawEntries(ABPropertyDefinitionCollection properties, Result<ADRawEntry>[] activeDirectoryRawEntries)
		{
			List<ABRawEntry> list = new List<ABRawEntry>(activeDirectoryRawEntries.Length);
			foreach (Result<ADRawEntry> result in activeDirectoryRawEntries)
			{
				list.Add(this.ADRawEntryResultToABRawEntry(properties, result));
			}
			return list;
		}

		private ADABRawEntry ADRawEntryResultToABRawEntry(ABPropertyDefinitionCollection properties, Result<ADRawEntry> result)
		{
			if (result.Data == null || result.Error != null)
			{
				if (result.Error == ProviderError.NotFound)
				{
					AirSyncDiagnostics.TraceDebug(base.Tracer, null, "Map Result<ADRawEntry> to null since result indicates entry not found.");
				}
				else
				{
					AirSyncDiagnostics.TraceError(base.Tracer, null, "Map Result<ADRawEntry> to null since result indicates unknown error or data is null.");
				}
				return null;
			}
			return new ADABRawEntry(this, properties, result.Data);
		}

		private IRecipientSession CreateRecipientSession()
		{
			ADSessionSettings adsessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.organizationId);
			adsessionSettings.AccountingObject = null;
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, this.searchRoot, this.lcid, true, this.consistencyMode, null, adsessionSettings, 614, "CreateRecipientSession", "f:\\15.00.1497\\sources\\dev\\AirSync\\src\\AirSync\\GalSearch\\ADABSession.cs");
		}

		private IRecipientSession CreateGalScopedRecipientSession(AddressBookBase globalAddressList)
		{
			ADSessionSettings adsessionSettings = ADSessionSettings.FromOrganizationIdWithAddressListScopeServiceOnly(this.organizationId, (globalAddressList != null) ? globalAddressList.Id : null);
			adsessionSettings.AccountingObject = null;
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, this.searchRoot, this.lcid, true, this.consistencyMode, null, adsessionSettings, 641, "CreateGalScopedRecipientSession", "f:\\15.00.1497\\sources\\dev\\AirSync\\src\\AirSync\\GalSearch\\ADABSession.cs");
		}

		private IConfigurationSession CreateConfigurationSession()
		{
			ADSessionSettings adsessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.organizationId);
			adsessionSettings.AccountingObject = null;
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(this.consistencyMode, adsessionSettings, 667, "CreateConfigurationSession", "f:\\15.00.1497\\sources\\dev\\AirSync\\src\\AirSync\\GalSearch\\ADABSession.cs");
		}

		private AddressBookBase FindGlobalAddressList()
		{
			IConfigurationSession configurationSession = this.ConfigurationSession;
			return AddressBookBase.GetGlobalAddressList(this.clientSecurityContext, this.ConfigurationSession, this.CreateRecipientSession());
		}

		private static ABProviderCapabilities providerCapabilities = new ABProviderCapabilities(ABProviderFlags.HasGal | ABProviderFlags.CanBrowse);

		private static readonly SortBy sortByDisplayName = new SortBy(ADRecipientSchema.DisplayName, SortOrder.Ascending);

		private readonly int lcid;

		private readonly ConsistencyMode consistencyMode;

		private IRecipientSession recipientSession;

		private IConfigurationSession configurationSession;

		private OrganizationId organizationId;

		private ADObjectId searchRoot;

		private ClientSecurityContext clientSecurityContext;
	}
}
