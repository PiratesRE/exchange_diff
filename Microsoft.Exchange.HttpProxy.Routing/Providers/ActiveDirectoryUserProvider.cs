using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.HttpProxy.Routing.Providers
{
	internal class ActiveDirectoryUserProvider : IUserProvider
	{
		public ActiveDirectoryUserProvider(bool useActiveDirectoryCache)
		{
			if (useActiveDirectoryCache)
			{
				this.directorySessionFactoryInstance = DirectorySessionFactory.GetInstance(DirectorySessionFactoryType.Cached);
				return;
			}
			this.directorySessionFactoryInstance = DirectorySessionFactory.GetInstance(DirectorySessionFactoryType.Default);
		}

		public User FindByExchangeGuidIncludingAlternate(Guid exchangeGuid, string tenantDomain, IRoutingDiagnostics diagnostics)
		{
			return this.Execute<User>(delegate
			{
				ADSessionSettings sessionSettings = ADSessionSettings.RootOrgOrSingleTenantFromAcceptedDomainAutoDetect(tenantDomain);
				IRecipientSession tenantOrRootOrgRecipientSession = this.directorySessionFactoryInstance.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 64, "FindByExchangeGuidIncludingAlternate", "f:\\15.00.1497\\sources\\dev\\cafe\\src\\Routing\\Providers\\ActiveDirectoryUserProvider.cs");
				ADRawEntry rawEntry = ActiveDirectoryUserProvider.FindByExchangeGuidIncludingAlternate(exchangeGuid, tenantOrRootOrgRecipientSession, diagnostics);
				return ActiveDirectoryUserProvider.CreateUserFromAdRawEntry(rawEntry);
			}, "FindByExchangeGuidIncludingAlternate failed");
		}

		public User FindBySmtpAddress(SmtpAddress smtpAddress, IRoutingDiagnostics diagnostics)
		{
			return this.Execute<User>(delegate
			{
				SmtpProxyAddress proxyAddress = new SmtpProxyAddress(smtpAddress.ToString(), true);
				ADSessionSettings sessionSettings = ADSessionSettings.RootOrgOrSingleTenantFromAcceptedDomainAutoDetect(smtpAddress.Domain);
				IRecipientSession tenantOrRootOrgRecipientSession = this.directorySessionFactoryInstance.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 88, "FindBySmtpAddress", "f:\\15.00.1497\\sources\\dev\\cafe\\src\\Routing\\Providers\\ActiveDirectoryUserProvider.cs");
				ADRawEntry rawEntry = ActiveDirectoryUserProvider.FindByProxyAddress(proxyAddress, tenantOrRootOrgRecipientSession, diagnostics);
				return ActiveDirectoryUserProvider.CreateUserFromAdRawEntry(rawEntry);
			}, "FindBySmtpAddress failed");
		}

		public User FindByExternalDirectoryObjectId(Guid userGuid, Guid tenantGuid, IRoutingDiagnostics diagnostics)
		{
			return this.Execute<User>(delegate
			{
				ADSessionSettings sessionSettings = ActiveDirectoryUserProvider.FromExternalDirectoryOrganizationId(tenantGuid, diagnostics);
				IRecipientSession tenantOrRootOrgRecipientSession = this.directorySessionFactoryInstance.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 111, "FindByExternalDirectoryObjectId", "f:\\15.00.1497\\sources\\dev\\cafe\\src\\Routing\\Providers\\ActiveDirectoryUserProvider.cs");
				ADRawEntry rawEntry = ActiveDirectoryUserProvider.FindAdUserByExternalDirectoryObjectId(userGuid, tenantOrRootOrgRecipientSession, diagnostics);
				return ActiveDirectoryUserProvider.CreateUserFromAdRawEntry(rawEntry);
			}, "FindByExternalDirectoryObjectId failed");
		}

		public User FindByLiveIdMemberName(SmtpAddress liveIdMemberName, string organizationContext, IRoutingDiagnostics diagnostics)
		{
			return this.Execute<User>(delegate
			{
				string text = organizationContext;
				if (string.IsNullOrEmpty(text))
				{
					text = liveIdMemberName.Domain;
				}
				ADSessionSettings sessionSettings = ADSessionSettings.RootOrgOrSingleTenantFromAcceptedDomainAutoDetect(text);
				ITenantRecipientSession recipientSession = this.directorySessionFactoryInstance.CreateTenantRecipientSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 140, "FindByLiveIdMemberName", "f:\\15.00.1497\\sources\\dev\\cafe\\src\\Routing\\Providers\\ActiveDirectoryUserProvider.cs");
				ADRawEntry rawEntry = ActiveDirectoryUserProvider.FindByLiveIdMemberName(liveIdMemberName, recipientSession, diagnostics);
				return ActiveDirectoryUserProvider.CreateUserFromAdRawEntry(rawEntry);
			}, "FindByLiveIdMemberName failed");
		}

		public string FindResourceForestFqdnByAcceptedDomainName(string acceptedDomain, IRoutingDiagnostics diagnostics)
		{
			return ADAccountPartitionLocator.GetResourceForestFqdnByAcceptedDomainName(acceptedDomain);
		}

		public string FindResourceForestFqdnByExternalDirectoryOrganizationId(Guid externalDirectoryOrganizationId, IRoutingDiagnostics diagnostics)
		{
			return ADAccountPartitionLocator.GetResourceForestFqdnByExternalDirectoryOrganizationId(externalDirectoryOrganizationId);
		}

		private static User CreateUserFromAdRawEntry(ADRawEntry rawEntry)
		{
			if (rawEntry == null)
			{
				return null;
			}
			Guid? databaseGuid = null;
			string databaseResourceForest = null;
			Guid? archiveDatabaseGuid = null;
			string archiveDatabaseResourceForest = null;
			ADObjectId adobjectId = rawEntry[ADMailboxRecipientSchema.Database] as ADObjectId;
			if (adobjectId != null)
			{
				databaseGuid = new Guid?(adobjectId.ObjectGuid);
				databaseResourceForest = adobjectId.PartitionFQDN;
			}
			ADObjectId adobjectId2 = rawEntry[ADUserSchema.ArchiveDatabase] as ADObjectId;
			if (adobjectId2 != null)
			{
				archiveDatabaseGuid = new Guid?(adobjectId2.ObjectGuid);
				archiveDatabaseResourceForest = adobjectId2.PartitionFQDN;
			}
			return new User
			{
				ArchiveDatabaseGuid = archiveDatabaseGuid,
				ArchiveDatabaseResourceForest = archiveDatabaseResourceForest,
				ArchiveGuid = (rawEntry[ADUserSchema.ArchiveGuid] as Guid?),
				DatabaseGuid = databaseGuid,
				DatabaseResourceForest = databaseResourceForest,
				LastModifiedTime = (rawEntry[ADObjectSchema.WhenChangedUTC] as DateTime?)
			};
		}

		private static ADSessionSettings FromExternalDirectoryOrganizationId(Guid tenantGuid, IRoutingDiagnostics diagnostics)
		{
			DateTime utcNow = DateTime.UtcNow;
			ADSessionSettings result;
			try
			{
				result = ADSessionSettings.FromExternalDirectoryOrganizationId(tenantGuid);
			}
			finally
			{
				diagnostics.AddGlobalLocatorLatency(DateTime.UtcNow - utcNow);
			}
			return result;
		}

		private static ADRawEntry FindByExchangeGuidIncludingAlternate(Guid exchangeGuid, IRecipientSession recipientSession, IRoutingDiagnostics diagnostics)
		{
			DateTime utcNow = DateTime.UtcNow;
			ADRawEntry result;
			try
			{
				result = recipientSession.FindByExchangeGuidIncludingAlternate(exchangeGuid, ActiveDirectoryUserProvider.AdRawEntryProperties);
			}
			finally
			{
				diagnostics.AddAccountForestLatency(DateTime.UtcNow - utcNow);
			}
			return result;
		}

		private static ADRawEntry FindByProxyAddress(ProxyAddress proxyAddress, IRecipientSession recipientSession, IRoutingDiagnostics diagnostics)
		{
			DateTime utcNow = DateTime.UtcNow;
			ADRawEntry result;
			try
			{
				result = recipientSession.FindByProxyAddress(proxyAddress, ActiveDirectoryUserProvider.AdRawEntryProperties);
			}
			finally
			{
				diagnostics.AddAccountForestLatency(DateTime.UtcNow - utcNow);
			}
			return result;
		}

		private static ADRawEntry FindByLiveIdMemberName(SmtpAddress liveIdMemberName, ITenantRecipientSession recipientSession, IRoutingDiagnostics diagnostics)
		{
			DateTime utcNow = DateTime.UtcNow;
			ADRawEntry result;
			try
			{
				result = recipientSession.FindByLiveIdMemberName(liveIdMemberName.ToString(), ActiveDirectoryUserProvider.AdRawEntryProperties);
			}
			finally
			{
				diagnostics.AddAccountForestLatency(DateTime.UtcNow - utcNow);
			}
			return result;
		}

		private static ADRawEntry FindAdUserByExternalDirectoryObjectId(Guid userGuid, IRecipientSession recipientSession, IRoutingDiagnostics diagnostics)
		{
			DateTime utcNow = DateTime.UtcNow;
			ADRawEntry result;
			try
			{
				result = recipientSession.FindADUserByExternalDirectoryObjectId(userGuid.ToString());
			}
			finally
			{
				diagnostics.AddAccountForestLatency(DateTime.UtcNow - utcNow);
			}
			return result;
		}

		private TResult Execute<TResult>(Func<TResult> func, string exceptionMessage)
		{
			TResult result;
			try
			{
				result = func();
			}
			catch (DataValidationException innerException)
			{
				throw new UserProviderException(exceptionMessage, innerException);
			}
			catch (ADTransientException innerException2)
			{
				throw new UserProviderException(exceptionMessage, innerException2);
			}
			catch (ADOperationException innerException3)
			{
				throw new UserProviderException(exceptionMessage, innerException3);
			}
			return result;
		}

		private static readonly PropertyDefinition[] AdRawEntryProperties = new PropertyDefinition[]
		{
			ADUserSchema.ArchiveDatabase,
			ADUserSchema.ArchiveGuid,
			ADMailboxRecipientSchema.Database
		};

		private readonly DirectorySessionFactory directorySessionFactoryInstance;
	}
}
