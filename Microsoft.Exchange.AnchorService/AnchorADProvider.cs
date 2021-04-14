using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AnchorService
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AnchorADProvider : IAnchorADProvider
	{
		public AnchorADProvider(AnchorContext context, OrganizationId organizationId, string preferredDomainController = null)
		{
			AnchorUtil.ThrowOnNullArgument(organizationId, "organizationId");
			this.Context = context;
			this.OrganizationId = organizationId;
			this.preferredDomainController = preferredDomainController;
			this.RecipientSession = this.CreateRecipientSession();
			this.lazyConfigurationSession = new Lazy<ITopologyConfigurationSession>(new Func<ITopologyConfigurationSession>(AnchorADProvider.CreateTopologyConfigurationSession));
		}

		public string TenantOrganizationName
		{
			get
			{
				if (OrganizationId.ForestWideOrgId == this.OrganizationId)
				{
					return "ForestWideOrganization";
				}
				return this.OrganizationId.OrganizationalUnit.Name;
			}
		}

		public MicrosoftExchangeRecipient PrimaryExchangeRecipient
		{
			get
			{
				MicrosoftExchangeRecipient recipient = null;
				this.DoAdCallAndTranslateExceptions(delegate
				{
					recipient = this.ConfigurationSession.FindMicrosoftExchangeRecipient();
				}, true);
				return recipient;
			}
		}

		public ITopologyConfigurationSession ConfigurationSession
		{
			get
			{
				return this.lazyConfigurationSession.Value;
			}
		}

		private protected OrganizationId OrganizationId { protected get; private set; }

		private AnchorContext Context { get; set; }

		private IRecipientSession RecipientSession { get; set; }

		private string DebugInfo
		{
			get
			{
				return this.TenantOrganizationName;
			}
		}

		public static AnchorADProvider GetRootOrgProvider(AnchorContext context)
		{
			return new AnchorADProvider(context, OrganizationId.ForestWideOrgId, null);
		}

		public ADRecipient GetADRecipientByObjectId(ADObjectId objectId)
		{
			AnchorUtil.ThrowOnNullArgument(objectId, "objectId");
			ADRecipient recipient = null;
			this.DoAdCallAndTranslateExceptions(delegate
			{
				recipient = this.RecipientSession.Read(objectId);
			}, false);
			return recipient;
		}

		public IEnumerable<ADUser> GetOrganizationMailboxesByCapability(OrganizationCapability capability)
		{
			return OrganizationMailbox.GetOrganizationMailboxesByCapability(this.RecipientSession, capability);
		}

		public ADPagedReader<TEntry> FindPagedMiniRecipient<TEntry>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties) where TEntry : MiniRecipient, new()
		{
			return this.RecipientSession.FindPagedMiniRecipient<TEntry>(rootId, scope, filter, sortBy, pageSize, properties);
		}

		public ADPagedReader<ADRawEntry> FindPagedADRawEntry(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties)
		{
			return this.RecipientSession.FindPagedADRawEntry(rootId, scope, filter, sortBy, pageSize, properties);
		}

		public void AddCapability(ADObjectId objectId, OrganizationCapability capability)
		{
			AnchorUtil.ThrowOnNullArgument(objectId, "objectId");
			ADUser user = this.GetAnchorADUser(objectId);
			this.DoAdCallAndTranslateExceptions(delegate
			{
				user.PersistedCapabilities.Add((Capability)capability);
				this.RecipientSession.Save(user);
			}, true);
		}

		public void RemoveCapability(ADObjectId objectId, OrganizationCapability capability)
		{
			AnchorUtil.ThrowOnNullArgument(objectId, "objectId");
			ADUser user = this.GetAnchorADUser(objectId);
			this.DoAdCallAndTranslateExceptions(delegate
			{
				if (user.PersistedCapabilities.Remove((Capability)capability))
				{
					this.RecipientSession.Save(user);
				}
			}, true);
		}

		public string GetDatabaseServerFqdn(Guid mdbGuid, bool forceRediscovery)
		{
			GetServerForDatabaseFlags gsfdFlags = forceRediscovery ? GetServerForDatabaseFlags.ReadThrough : GetServerForDatabaseFlags.None;
			string serverFqdn;
			try
			{
				DatabaseLocationInfo serverForDatabase = ActiveManager.GetCachingActiveManagerInstance().GetServerForDatabase(mdbGuid, gsfdFlags);
				if (serverForDatabase == null)
				{
					throw new MigrationMailboxDatabaseInfoNotAvailableException(mdbGuid.ToString());
				}
				serverFqdn = serverForDatabase.ServerFqdn;
			}
			catch (ObjectNotFoundException ex)
			{
				string text = string.Format("Server for mailbox with Guid {0} not found in ActiveManager using forceDiscovery set to {1}", mdbGuid, forceRediscovery);
				this.Context.Logger.Log(MigrationEventType.Error, ex, text, new object[0]);
				throw new AnchorDatabaseNotFoundTransientException(mdbGuid.ToString(), ex)
				{
					InternalError = text
				};
			}
			catch (StoragePermanentException exception)
			{
				this.Context.Logger.Log(MigrationEventType.Error, exception, "Server for mailbox with Guid {0} not found in ActiveManager using forceDiscovery set to {1}", new object[]
				{
					mdbGuid,
					forceRediscovery
				});
				throw;
			}
			catch (StorageTransientException exception2)
			{
				this.Context.Logger.Log(MigrationEventType.Error, exception2, "Server for mailbox with Guid {0} not found in ActiveManager using forceDiscovery set to {1}", new object[]
				{
					mdbGuid,
					forceRediscovery
				});
				throw;
			}
			catch (ServerForDatabaseNotFoundException ex2)
			{
				string text2 = string.Format("Server for mailbox with Guid {0} not found in ActiveManager using forceDiscovery set to {1}", mdbGuid, forceRediscovery);
				this.Context.Logger.Log(MigrationEventType.Error, ex2, text2, new object[0]);
				throw new AnchorServerNotFoundTransientException(mdbGuid.ToString(), ex2)
				{
					InternalError = text2
				};
			}
			return serverFqdn;
		}

		public string GetPreferredDomainController()
		{
			return this.RecipientSession.DomainController ?? this.RecipientSession.LastUsedDc;
		}

		public ADRecipient GetADRecipientByProxyAddress(string userEmail)
		{
			AnchorUtil.ThrowOnNullOrEmptyArgument(userEmail, "userEmail");
			ProxyAddress proxy = ProxyAddress.Parse(userEmail);
			ADRecipient recipient = null;
			this.DoAdCallAndTranslateExceptions(delegate
			{
				recipient = this.RecipientSession.FindByProxyAddress<ADRecipient>(proxy);
			}, false);
			return recipient;
		}

		public string GetMailboxServerFqdn(ADUser user, bool forceRefresh)
		{
			return this.GetDatabaseServerFqdn(user.Database.ObjectGuid, forceRefresh);
		}

		public void EnsureLocalMailbox(ADUser user, bool forceRefresh)
		{
			AnchorUtil.ThrowOnNullArgument(user, "user");
			string mailboxServerFqdn = this.GetMailboxServerFqdn(user, forceRefresh);
			if (!string.Equals(mailboxServerFqdn, LocalServer.GetServer().Fqdn, StringComparison.OrdinalIgnoreCase))
			{
				throw new AnchorMailboxNotFoundOnServerException(mailboxServerFqdn, LocalServer.GetServer().Fqdn, user.DistinguishedName);
			}
		}

		internal ExchangePrincipal GetMailboxOwner(QueryFilter filter)
		{
			ADUser recipient = this.GetRecipient<ADUser>(this.RecipientSession, filter);
			return ExchangePrincipal.FromADUser(this.RecipientSession.SessionSettings, recipient, RemotingOptions.AllowCrossSite);
		}

		private static ITopologyConfigurationSession CreateTopologyConfigurationSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 409, "CreateTopologyConfigurationSession", "f:\\15.00.1497\\sources\\dev\\assistants\\src\\AnchorService\\Common\\AnchorADProvider.cs");
		}

		private string GetMailboxServerFqdn(ExchangePrincipal mailboxOwner, bool forceRefresh)
		{
			AnchorUtil.ThrowOnNullArgument(mailboxOwner, "mailboxOwner");
			if (mailboxOwner.MailboxInfo.Location == null)
			{
				forceRefresh = true;
			}
			if (!forceRefresh)
			{
				return mailboxOwner.MailboxInfo.Location.ServerFqdn;
			}
			return this.GetDatabaseServerFqdn(mailboxOwner.MailboxInfo.GetDatabaseGuid(), true);
		}

		private ADUser GetAnchorADUser(ADObjectId objectId)
		{
			AnchorUtil.ThrowOnNullArgument(objectId, "objectId");
			ADUser aduser = this.GetADRecipientByObjectId(objectId) as ADUser;
			if (aduser == null)
			{
				throw new AnchorMailboxNotFoundException();
			}
			return aduser;
		}

		private IRecipientSession CreateRecipientSession()
		{
			if (OrganizationId.ForestWideOrgId == this.OrganizationId)
			{
				return DirectorySessionFactory.Default.CreateRootOrgRecipientSession(null, null, LcidMapper.DefaultLcid, false, ConsistencyMode.IgnoreInvalid, null, ADSessionSettings.FromRootOrgScopeSet(), 472, "CreateRecipientSession", "f:\\15.00.1497\\sources\\dev\\assistants\\src\\AnchorService\\Common\\AnchorADProvider.cs");
			}
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.OrganizationId);
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(this.preferredDomainController, null, LcidMapper.DefaultLcid, false, ConsistencyMode.IgnoreInvalid, null, sessionSettings, 485, "CreateRecipientSession", "f:\\15.00.1497\\sources\\dev\\assistants\\src\\AnchorService\\Common\\AnchorADProvider.cs");
		}

		private T GetRecipient<T>(IRecipientSession session, QueryFilter filter) where T : class
		{
			return MigrationHelperBase.GetRecipient<T>(session, filter, (string msg) => new AnchorMailboxNotFoundException(), (string msg) => new MultipleAnchorMailboxesFoundException(), (string msg) => new AnchorMailboxNotFoundException(), this.DebugInfo);
		}

		private void DoAdCallAndTranslateExceptions(ADOperation call, bool expectObject)
		{
			MigrationHelperBase.DoAdCallAndTranslateExceptions(call, expectObject, this.DebugInfo);
		}

		internal static readonly bool IsDataCenter = Datacenter.IsRunningInExchangeDatacenter(false);

		private readonly string preferredDomainController;

		private readonly Lazy<ITopologyConfigurationSession> lazyConfigurationSession;
	}
}
