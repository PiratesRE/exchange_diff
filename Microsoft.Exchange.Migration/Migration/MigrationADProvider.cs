using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Migration.Logging;
using Microsoft.Exchange.Transport.Configuration;

namespace Microsoft.Exchange.Migration
{
	internal sealed class MigrationADProvider : IMigrationADProvider
	{
		public MigrationADProvider(IRecipientSession recipientSession)
		{
			this.recipientSession = recipientSession;
			if (recipientSession != null)
			{
				this.OrganizationId = recipientSession.SessionSettings.CurrentOrganizationId;
				return;
			}
			this.OrganizationId = OrganizationId.ForestWideOrgId;
		}

		public MigrationADProvider(OrganizationId organizationId)
		{
			this.OrganizationId = organizationId;
			this.recipientSession = this.CreateRecipientSession();
		}

		public MigrationADProvider(TenantPartitionHint partitionHint) : this(MigrationHelperBase.CreateRecipientSession(partitionHint))
		{
		}

		public string TenantOrganizationName
		{
			get
			{
				if (this.OrganizationId == OrganizationId.ForestWideOrgId)
				{
					return "ForestWideOrganization";
				}
				return this.OrganizationId.OrganizationalUnit.Name;
			}
		}

		public bool IsLicensingEnforced
		{
			get
			{
				return !this.IsEnterpriseOrFirstOrg && this.ExchangeConfigurationUnit.IsLicensingEnforced;
			}
		}

		public bool IsSmtpAddressCheckWithAcceptedDomain
		{
			get
			{
				return !this.IsEnterpriseOrFirstOrg && this.ExchangeConfigurationUnit.SMTPAddressCheckWithAcceptedDomain;
			}
		}

		public bool IsMigrationEnabled
		{
			get
			{
				if (!MigrationServiceFactory.Instance.IsMultiTenantEnabled())
				{
					return true;
				}
				TransportConfigContainer configContainer = null;
				this.DoAdCallAndTranslateExceptions(delegate
				{
					configContainer = this.GetConfigurationSession().FindSingletonConfigurationObject<TransportConfigContainer>();
				}, true);
				if (configContainer == null)
				{
					MigrationLogger.Log(MigrationEventType.Warning, "Expect to have a transport config container, but not found.  migration requires it", new object[0]);
					return false;
				}
				PerTenantTransportSettings perTenantTransportSettings = new PerTenantTransportSettings(configContainer);
				return perTenantTransportSettings.MigrationEnabled;
			}
		}

		public bool IsDirSyncEnabled
		{
			get
			{
				return !this.IsEnterpriseOrFirstOrg && this.ExchangeConfigurationUnit.IsDirSyncRunning;
			}
		}

		public bool IsMSOSyncEnabled
		{
			get
			{
				return !this.IsEnterpriseOrFirstOrg && this.ExchangeConfigurationUnit.MSOSyncEnabled;
			}
		}

		public MicrosoftExchangeRecipient PrimaryExchangeRecipient
		{
			get
			{
				MicrosoftExchangeRecipient recipient = null;
				this.DoAdCallAndTranslateExceptions(delegate
				{
					recipient = this.GetConfigurationSession().FindMicrosoftExchangeRecipient();
				}, true);
				return recipient;
			}
		}

		public IRecipientSession RecipientSession
		{
			get
			{
				if (this.recipientSession != null)
				{
					return this.recipientSession;
				}
				return this.CreateRecipientSession();
			}
		}

		private bool IsEnterpriseOrFirstOrg
		{
			get
			{
				return !MigrationServiceFactory.Instance.IsMultiTenantEnabled() || this.OrganizationId == OrganizationId.ForestWideOrgId;
			}
		}

		private ExchangeConfigurationUnit ExchangeConfigurationUnit
		{
			get
			{
				if (this.IsEnterpriseOrFirstOrg)
				{
					return null;
				}
				if (this.exchangeConfigurationUnit == null)
				{
					ITenantConfigurationSession tenantSession = this.GetConfigurationSession() as ITenantConfigurationSession;
					if (tenantSession != null)
					{
						this.DoAdCallAndTranslateExceptions(delegate
						{
							this.exchangeConfigurationUnit = tenantSession.GetExchangeConfigurationUnitByName(this.TenantOrganizationName);
						}, true);
					}
				}
				return this.exchangeConfigurationUnit;
			}
		}

		private OrganizationId OrganizationId { get; set; }

		private string DebugInfo
		{
			get
			{
				return this.TenantOrganizationName;
			}
		}

		public static MigrationADProvider GetRootOrgADProvider()
		{
			return new MigrationADProvider(OrganizationId.ForestWideOrgId);
		}

		public ADRecipient GetADRecipientByObjectId(ADObjectId objectId)
		{
			MigrationUtil.ThrowOnNullArgument(objectId, "objectId");
			ADRecipient recipient = null;
			this.DoAdCallAndTranslateExceptions(delegate
			{
				recipient = this.RecipientSession.Read(objectId);
			}, false);
			return recipient;
		}

		public ADRecipient GetADRecipientByExchangeObjectId(Guid exchangeObjectGuid)
		{
			MigrationUtil.ThrowOnGuidEmptyArgument(exchangeObjectGuid, "exchangeObjectGuid");
			ADRecipient recipient = null;
			this.DoAdCallAndTranslateExceptions(delegate
			{
				recipient = this.RecipientSession.FindByExchangeObjectId(exchangeObjectGuid);
			}, false);
			return recipient;
		}

		public ADRecipient GetADRecipientByProxyAddress(string userEmail)
		{
			return this.GetADRecipientByProxyAddress<ADRecipient>(userEmail);
		}

		public MailboxData GetMailboxDataFromLegacyDN(string userLegDN, bool forceRefresh, string userEmailAddressForDebug)
		{
			MigrationUtil.ThrowOnNullOrEmptyArgument(userLegDN, "userLegDN");
			MailboxData result;
			LocalizedException ex;
			if (this.TryGetMailboxDataFromAdUser(() => this.GetADRecipientByLegDN(userLegDN, false) as ADUser, userLegDN, forceRefresh, out result, out ex))
			{
				return result;
			}
			if (ex != null)
			{
				throw ex;
			}
			throw new MigrationRecipientNotFoundException(userLegDN);
		}

		public MailboxData GetMailboxDataFromSmtpAddress(string userEmail, bool forceRefresh, bool throwOnNotFound = true)
		{
			MigrationUtil.ThrowOnNullOrEmptyArgument(userEmail, "userEmail");
			MailboxData result;
			LocalizedException ex;
			if (this.TryGetMailboxDataFromAdUser(() => this.GetADRecipientByProxyAddress<ADUser>(userEmail), userEmail, forceRefresh, out result, out ex))
			{
				return result;
			}
			if (!throwOnNotFound)
			{
				return null;
			}
			if (ex != null)
			{
				throw ex;
			}
			throw new MigrationRecipientNotFoundException(userEmail);
		}

		public MailboxData GetPublicFolderMailboxDataFromName(string name, bool forceRefresh, bool throwOnNotFound = true)
		{
			MigrationUtil.ThrowOnNullOrEmptyArgument(name, "name");
			Func<ADUser> findUser = delegate()
			{
				QueryFilter filter = QueryFilter.AndTogether(new QueryFilter[]
				{
					Filters.GetRecipientTypeDetailsFilterOptimization(RecipientTypeDetails.PublicFolderMailbox),
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, name)
				});
				return this.GetRecipient<ADUser>(this.RecipientSession, filter);
			};
			MailboxData result;
			LocalizedException ex;
			if (this.TryGetMailboxDataFromAdUser(findUser, name, forceRefresh, out result, out ex))
			{
				return result;
			}
			if (!throwOnNotFound)
			{
				return null;
			}
			if (ex != null)
			{
				throw ex;
			}
			throw new MigrationRecipientNotFoundException(name);
		}

		public string GetPublicFolderHierarchyMailboxName()
		{
			TenantPublicFolderConfigurationCache.Instance.RemoveValue(this.OrganizationId);
			TenantPublicFolderConfiguration value = TenantPublicFolderConfigurationCache.Instance.GetValue(this.OrganizationId);
			PublicFolderInformation hierarchyMailboxInformation = value.GetHierarchyMailboxInformation();
			if (hierarchyMailboxInformation == null || hierarchyMailboxInformation.HierarchyMailboxGuid == Guid.Empty)
			{
				return null;
			}
			PublicFolderRecipient localMailboxRecipient = value.GetLocalMailboxRecipient(hierarchyMailboxInformation.HierarchyMailboxGuid);
			if (localMailboxRecipient == null)
			{
				return null;
			}
			return localMailboxRecipient.MailboxName;
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
				MigrationLogger.Log(MigrationEventType.Error, ex, text, new object[0]);
				throw new AnchorDatabaseNotFoundTransientException(mdbGuid.ToString(), ex)
				{
					InternalError = text
				};
			}
			catch (StoragePermanentException exception)
			{
				MigrationLogger.Log(MigrationEventType.Error, exception, "Server for mailbox with Guid {0} not found in ActiveManager using forceDiscovery set to {1}", new object[]
				{
					mdbGuid,
					forceRediscovery
				});
				throw;
			}
			catch (StorageTransientException exception2)
			{
				MigrationLogger.Log(MigrationEventType.Error, exception2, "Server for mailbox with Guid {0} not found in ActiveManager using forceDiscovery set to {1}", new object[]
				{
					mdbGuid,
					forceRediscovery
				});
				throw;
			}
			catch (ServerForDatabaseNotFoundException ex2)
			{
				string text2 = string.Format("Server for mailbox with Guid {0} not found in ActiveManager using forceDiscovery set to {1}", mdbGuid, forceRediscovery);
				MigrationLogger.Log(MigrationEventType.Error, ex2, text2, new object[0]);
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

		public ExchangePrincipal GetExchangePrincipalFromMbxGuid(Guid mailboxGuid)
		{
			return ExchangePrincipal.FromMailboxGuid(this.RecipientSession.SessionSettings, mailboxGuid, RemotingOptions.AllowCrossSite, null);
		}

		public void UpdateMigrationUpgradeConstraint(UpgradeConstraint constraint)
		{
			MigrationUtil.ThrowOnNullArgument(constraint, "constraint");
			this.DoAdCallAndTranslateExceptions(delegate
			{
				ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(false, ConsistencyMode.PartiallyConsistent, this.RecipientSession.SessionSettings, 610, "UpdateMigrationUpgradeConstraint", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Migration\\DataAccessLayer\\MigrationADProvider.cs");
				ExchangeConfigurationUnit exchangeConfigurationUnitByName = tenantConfigurationSession.GetExchangeConfigurationUnitByName(this.TenantOrganizationName);
				ExAssert.RetailAssert(exchangeConfigurationUnitByName != null, "Organization must always exist.");
				if (exchangeConfigurationUnitByName.UpgradeConstraints == null || exchangeConfigurationUnitByName.UpgradeConstraints.UpgradeConstraints == null)
				{
					exchangeConfigurationUnitByName.UpgradeConstraints = new UpgradeConstraintArray(new UpgradeConstraint[]
					{
						constraint
					});
				}
				else
				{
					IComparable<UpgradeConstraint> comparableConstraint = constraint;
					if (exchangeConfigurationUnitByName.UpgradeConstraints.UpgradeConstraints.Any((UpgradeConstraint existingConstraint) => comparableConstraint.CompareTo(existingConstraint) == 0))
					{
						return;
					}
					IEnumerable<UpgradeConstraint> first = from c in exchangeConfigurationUnitByName.UpgradeConstraints.UpgradeConstraints
					where !c.Name.Equals(constraint.Name)
					select c;
					exchangeConfigurationUnitByName.UpgradeConstraints = new UpgradeConstraintArray(first.Union(new UpgradeConstraint[]
					{
						constraint
					}).ToArray<UpgradeConstraint>());
				}
				MigrationLogger.Log(MigrationEventType.Verbose, "Saving organization constraints", new object[0]);
				tenantConfigurationSession.Save(exchangeConfigurationUnitByName);
			}, false);
		}

		public void RemovePublicFolderMigrationLock()
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.OrganizationId), 651, "RemovePublicFolderMigrationLock", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Migration\\DataAccessLayer\\MigrationADProvider.cs");
			Organization orgContainer = tenantOrTopologyConfigurationSession.GetOrgContainer();
			orgContainer.DefaultPublicFolderMailbox = orgContainer.DefaultPublicFolderMailbox.Clone();
			orgContainer.DefaultPublicFolderMailbox.SetHierarchyMailbox(orgContainer.DefaultPublicFolderMailbox.HierarchyMailboxGuid, PublicFolderInformation.HierarchyType.MailboxGuid);
			tenantOrTopologyConfigurationSession.Save(orgContainer);
		}

		public bool CheckPublicFoldersLockedForMigration()
		{
			Organization orgContainer = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.OrganizationId), 672, "CheckPublicFoldersLockedForMigration", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Migration\\DataAccessLayer\\MigrationADProvider.cs").GetOrgContainer();
			return orgContainer.DefaultPublicFolderMailbox.Type == PublicFolderInformation.HierarchyType.InTransitMailboxGuid;
		}

		internal static Uri GetEcpUrl(IExchangePrincipal owner)
		{
			try
			{
				Uri frontEndEcpUrl = FrontEndLocator.GetFrontEndEcpUrl(owner);
				if (frontEndEcpUrl != null)
				{
					return frontEndEcpUrl;
				}
			}
			catch (ServerNotFoundException)
			{
			}
			return BackEndLocator.GetBackEndEcpUrl(owner.MailboxInfo);
		}

		internal static OrganizationId GetOrganization(string tenantName)
		{
			return ADSessionSettings.FromTenantCUName(tenantName).CurrentOrganizationId;
		}

		internal static OrganizationId GetOrganization(ADObjectId organizationADObjectId)
		{
			if (!MigrationServiceFactory.Instance.IsMultiTenantEnabled() || ADObjectId.IsNullOrEmpty(organizationADObjectId))
			{
				return OrganizationId.ForestWideOrgId;
			}
			OrganizationId organizationId;
			if (MigrationADProvider.CachedOrganizations.TryGetValue(organizationADObjectId, out organizationId))
			{
				return organizationId;
			}
			ADOrganizationalUnit[] organizations = null;
			ITenantConfigurationSession tenantSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromAllTenantsObjectId(organizationADObjectId), 733, "GetOrganization", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Migration\\DataAccessLayer\\MigrationADProvider.cs");
			MigrationHelperBase.DoAdCallAndTranslateExceptions(delegate
			{
				organizations = tenantSession.Find<ADOrganizationalUnit>(organizationADObjectId, QueryScope.Base, null, null, 1);
			}, true, organizationADObjectId.ToString());
			if (organizations == null || organizations.Length <= 0)
			{
				throw new MigrationObjectNotFoundInADException(organizationADObjectId.ToString(), tenantSession.Source);
			}
			organizationId = organizations[0].OrganizationId;
			MigrationADProvider.CachedOrganizations[organizationADObjectId] = organizationId;
			return organizationId;
		}

		internal ExchangePrincipal GetLocalSystemMailboxOwner(Guid mdbGuid)
		{
			MigrationUtil.ThrowOnGuidEmptyArgument(mdbGuid, "mdbGuid");
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, MigrationADProvider.GetSystemMailboxName(mdbGuid));
			ADSystemMailbox recipient = this.GetRecipient<ADSystemMailbox>(this.RecipientSession, filter);
			return ExchangePrincipal.FromADSystemMailbox(this.RecipientSession.SessionSettings, recipient, LocalServer.GetServer());
		}

		internal ExchangePrincipal GetLocalMigrationMailboxOwner(string migrationMailboxLegacyDn)
		{
			MigrationUtil.ThrowOnNullArgument(migrationMailboxLegacyDn, "migrationMailboxLegacyDn");
			ADUser aduser = this.GetADRecipientByLegDN(migrationMailboxLegacyDn, true) as ADUser;
			if (aduser == null)
			{
				throw new MigrationObjectNotFoundInADException(migrationMailboxLegacyDn, this.RecipientSession.Source);
			}
			return ExchangePrincipal.FromADUser(this.RecipientSession.SessionSettings, aduser, RemotingOptions.LocalConnectionsOnly);
		}

		internal MailboxData GetMailboxDataForManagementMailbox()
		{
			MailboxData result;
			LocalizedException ex;
			if (this.TryGetMailboxDataFromAdUser(() => this.GetRecipient<ADUser>(this.RecipientSession, MigrationHelperBase.ManagementMailboxFilter), "Management mailbox", true, out result, out ex))
			{
				return result;
			}
			if (ex != null)
			{
				throw ex;
			}
			throw new MigrationRecipientNotFoundException("Management mailbox");
		}

		internal void EnsureLocalMailbox(IExchangePrincipal mailboxOwner, bool forceRefresh)
		{
			MigrationUtil.ThrowOnNullArgument(mailboxOwner, "mailboxOwner");
			if (mailboxOwner.MailboxInfo.Location == null)
			{
				forceRefresh = true;
			}
			string text;
			if (!forceRefresh)
			{
				text = mailboxOwner.MailboxInfo.Location.ServerFqdn;
			}
			else
			{
				text = this.GetDatabaseServerFqdn(mailboxOwner.MailboxInfo.GetDatabaseGuid(), true);
			}
			if (!string.Equals(text, MigrationServiceFactory.Instance.GetLocalServerFqdn(), StringComparison.OrdinalIgnoreCase))
			{
				throw new MigrationMailboxNotFoundOnServerException(text, MigrationServiceFactory.Instance.GetLocalServerFqdn(), mailboxOwner.LegacyDn);
			}
		}

		private static string GetSystemMailboxName(Guid mdbGuid)
		{
			MigrationUtil.ThrowOnGuidEmptyArgument(mdbGuid, "mdbGuid");
			return "SystemMailbox{" + mdbGuid.ToString() + "}";
		}

		private IRecipientSession CreateRecipientSession()
		{
			if (OrganizationId.ForestWideOrgId == this.OrganizationId)
			{
				return DirectorySessionFactory.Default.CreateRootOrgRecipientSession(null, null, LcidMapper.DefaultLcid, true, ConsistencyMode.IgnoreInvalid, null, ADSessionSettings.FromRootOrgScopeSet(), 894, "CreateRecipientSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Migration\\DataAccessLayer\\MigrationADProvider.cs");
			}
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.OrganizationId);
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, null, LcidMapper.DefaultLcid, true, ConsistencyMode.IgnoreInvalid, null, sessionSettings, 907, "CreateRecipientSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Migration\\DataAccessLayer\\MigrationADProvider.cs");
		}

		private IConfigurationSession GetConfigurationSession()
		{
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, this.RecipientSession.SessionSettings, 923, "GetConfigurationSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Migration\\DataAccessLayer\\MigrationADProvider.cs");
		}

		private void DoAdCallAndTranslateExceptions(ADOperation call, bool expectObject)
		{
			MigrationHelperBase.DoAdCallAndTranslateExceptions(call, expectObject, this.DebugInfo);
		}

		private TResult GetADRecipientByProxyAddress<TResult>(string userEmail) where TResult : ADObject, new()
		{
			MigrationUtil.ThrowOnNullOrEmptyArgument(userEmail, "userEmail");
			ProxyAddress proxy = ProxyAddress.Parse(userEmail);
			TResult recipient = default(TResult);
			this.DoAdCallAndTranslateExceptions(delegate
			{
				recipient = this.RecipientSession.FindByProxyAddress<TResult>(proxy);
			}, false);
			return recipient;
		}

		private T GetRecipient<T>(IRecipientSession session, QueryFilter filter) where T : class
		{
			return MigrationHelperBase.GetRecipient<T>(session, filter, (string msg) => new MigrationMailboxNotFoundException(), (string msg) => new MultipleMigrationMailboxesFoundException(), (string msg) => new MigrationMailboxNotFoundException(), this.DebugInfo);
		}

		private ADRecipient GetADRecipientByLegDN(string legDN, bool expectObject = false)
		{
			MigrationUtil.ThrowOnNullOrEmptyArgument(legDN, "legDN");
			ADRecipient recipient = null;
			this.DoAdCallAndTranslateExceptions(delegate
			{
				recipient = this.RecipientSession.FindByLegacyExchangeDN(legDN);
			}, expectObject);
			return recipient;
		}

		private bool TryGetMailboxDataFromAdUser(Func<ADUser> findUser, string userIdentifier, bool refreshMailboxData, out MailboxData result, out LocalizedException error)
		{
			result = null;
			try
			{
				ADUser aduser = findUser();
				if (aduser == null)
				{
					error = new MigrationRecipientNotFoundException(userIdentifier);
					return false;
				}
				switch (aduser.RecipientType)
				{
				case RecipientType.UserMailbox:
				{
					Guid objectGuid = aduser.Database.ObjectGuid;
					string databaseServerFqdn = this.GetDatabaseServerFqdn(objectGuid, refreshMailboxData);
					result = new MailboxData(aduser.ExchangeGuid, objectGuid, new Fqdn(databaseServerFqdn), aduser.LegacyExchangeDN, aduser.Id, aduser.ExchangeObjectId);
					break;
				}
				case RecipientType.MailUser:
					if (aduser.ExchangeGuid == Guid.Empty)
					{
						error = new MissingExchangeGuidException(userIdentifier);
						return false;
					}
					result = new MailboxData(aduser.ExchangeGuid, aduser.LegacyExchangeDN, aduser.Id, aduser.ExchangeObjectId);
					break;
				default:
					MigrationLogger.Log(MigrationEventType.Verbose, "Attempted to get a MailboxData for an unknown recipient type: {0}:{1}", new object[]
					{
						aduser.Guid,
						aduser.RecipientType
					});
					break;
				}
				error = null;
			}
			catch (LocalizedException ex)
			{
				error = ex;
				MigrationLogger.Log(MigrationEventType.Information, ex, "Could not find a valid user for '{0}'.", new object[]
				{
					userIdentifier
				});
			}
			if (result != null)
			{
				result.Update(userIdentifier, this.OrganizationId);
			}
			return result != null;
		}

		private static readonly MruDictionaryCache<ADObjectId, OrganizationId> CachedOrganizations = new MruDictionaryCache<ADObjectId, OrganizationId>(8, 1000, 480);

		private readonly IRecipientSession recipientSession;

		private ExchangeConfigurationUnit exchangeConfigurationUnit;
	}
}
