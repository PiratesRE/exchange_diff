using System;
using System.Net;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory
{
	internal class CompositeDirectorySessionFactory : DirectorySessionFactory
	{
		public override ITenantConfigurationSession CreateTenantConfigurationSession(ConsistencyMode consistencyMode, ADSessionSettings sessionSettings, int callerFileLine, string memberName, string callerFilePath)
		{
			return new CompositeTenantConfigurationSession(DirectorySessionFactory.CacheSessionFactory.CreateTenantConfigurationSession(consistencyMode, sessionSettings, callerFileLine, memberName, callerFilePath), DirectorySessionFactory.NonCacheSessionFactory.CreateTenantConfigurationSession(consistencyMode, sessionSettings, callerFileLine, memberName, callerFilePath), false);
		}

		public override ITenantConfigurationSession CreateTenantConfigurationSession(bool readOnly, ConsistencyMode consistencyMode, ADSessionSettings sessionSettings, int callerFileLine, string memberName, string callerFilePath)
		{
			return new CompositeTenantConfigurationSession(DirectorySessionFactory.CacheSessionFactory.CreateTenantConfigurationSession(readOnly, consistencyMode, sessionSettings, callerFileLine, memberName, callerFilePath), DirectorySessionFactory.NonCacheSessionFactory.CreateTenantConfigurationSession(readOnly, consistencyMode, sessionSettings, callerFileLine, memberName, callerFilePath), false);
		}

		public override ITenantConfigurationSession CreateTenantConfigurationSession(string domainController, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, int callerFileLine, string memberName, string callerFilePath)
		{
			bool cacheSessionForDeletingOnly = true;
			if (networkCredential == null && string.IsNullOrEmpty(domainController))
			{
				cacheSessionForDeletingOnly = false;
			}
			return new CompositeTenantConfigurationSession(DirectorySessionFactory.CacheSessionFactory.CreateTenantConfigurationSession(domainController, readOnly, consistencyMode, networkCredential, sessionSettings, callerFileLine, memberName, callerFilePath), DirectorySessionFactory.NonCacheSessionFactory.CreateTenantConfigurationSession(domainController, readOnly, consistencyMode, networkCredential, sessionSettings, callerFileLine, memberName, callerFilePath), cacheSessionForDeletingOnly);
		}

		public override ITenantConfigurationSession CreateTenantConfigurationSession(string domainController, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, ConfigScopes configScope, int callerFileLine, string memberName, string callerFilePath)
		{
			bool cacheSessionForDeletingOnly = true;
			if (networkCredential == null && string.IsNullOrEmpty(domainController))
			{
				cacheSessionForDeletingOnly = false;
			}
			return new CompositeTenantConfigurationSession(DirectorySessionFactory.CacheSessionFactory.CreateTenantConfigurationSession(domainController, readOnly, consistencyMode, networkCredential, sessionSettings, configScope, callerFileLine, memberName, callerFilePath), DirectorySessionFactory.NonCacheSessionFactory.CreateTenantConfigurationSession(domainController, readOnly, consistencyMode, networkCredential, sessionSettings, configScope, callerFileLine, memberName, callerFilePath), cacheSessionForDeletingOnly);
		}

		public override ITenantConfigurationSession CreateTenantConfigurationSession(ConsistencyMode consistencyMode, Guid externalDirectoryOrganizationId, int callerFileLine, string memberName, string callerFilePath)
		{
			ADSessionSettings adsessionSettings = ADSessionSettings.FromExternalDirectoryOrganizationId(externalDirectoryOrganizationId);
			if (adsessionSettings == null)
			{
				return null;
			}
			return new CompositeTenantConfigurationSession(DirectorySessionFactory.CacheSessionFactory.CreateTenantConfigurationSession(consistencyMode, adsessionSettings, callerFileLine, memberName, callerFilePath), DirectorySessionFactory.NonCacheSessionFactory.CreateTenantConfigurationSession(consistencyMode, adsessionSettings, callerFileLine, memberName, callerFilePath), false);
		}

		public override ITopologyConfigurationSession CreateTopologyConfigurationSession(ConsistencyMode consistencyMode, ADSessionSettings sessionSettings, int callerFileLine, string memberName, string callerFilePath)
		{
			return DirectorySessionFactory.NonCacheSessionFactory.CreateTopologyConfigurationSession(consistencyMode, sessionSettings, callerFileLine, memberName, callerFilePath);
		}

		public override ITopologyConfigurationSession CreateTopologyConfigurationSession(bool readOnly, ConsistencyMode consistencyMode, ADSessionSettings sessionSettings, int callerFileLine, string memberName, string callerFilePath)
		{
			return DirectorySessionFactory.NonCacheSessionFactory.CreateTopologyConfigurationSession(readOnly, consistencyMode, sessionSettings, callerFileLine, memberName, callerFilePath);
		}

		public override ITopologyConfigurationSession CreateTopologyConfigurationSession(string domainController, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, int callerFileLine, string memberName, string callerFilePath)
		{
			return DirectorySessionFactory.NonCacheSessionFactory.CreateTopologyConfigurationSession(domainController, readOnly, consistencyMode, networkCredential, sessionSettings, callerFileLine, memberName, callerFilePath);
		}

		public override ITopologyConfigurationSession CreateTopologyConfigurationSession(string domainController, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, ConfigScopes configScope, int callerFileLine, string memberName, string callerFilePath)
		{
			return DirectorySessionFactory.NonCacheSessionFactory.CreateTopologyConfigurationSession(domainController, readOnly, consistencyMode, networkCredential, sessionSettings, configScope, callerFileLine, memberName, callerFilePath);
		}

		public override ITenantRecipientSession CreateTenantRecipientSession(string domainController, ADObjectId searchRoot, int lcid, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, int callerFileLine, string memberName, string callerFilePath)
		{
			bool cacheSessionForDeletingOnly = true;
			if (networkCredential == null && string.IsNullOrEmpty(domainController))
			{
				cacheSessionForDeletingOnly = false;
			}
			return new CompositeTenantRecipientSession(DirectorySessionFactory.CacheSessionFactory.CreateTenantRecipientSession(domainController, searchRoot, lcid, readOnly, consistencyMode, networkCredential, sessionSettings, callerFileLine, memberName, callerFilePath), DirectorySessionFactory.NonCacheSessionFactory.CreateTenantRecipientSession(domainController, searchRoot, lcid, readOnly, consistencyMode, networkCredential, sessionSettings, callerFileLine, memberName, callerFilePath), cacheSessionForDeletingOnly);
		}

		public override ITenantRecipientSession CreateTenantRecipientSession(string domainController, ADObjectId searchRoot, int lcid, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, ConfigScopes configScopes, int callerFileLine, string memberName, string callerFilePath)
		{
			bool cacheSessionForDeletingOnly = true;
			if (networkCredential == null && string.IsNullOrEmpty(domainController))
			{
				cacheSessionForDeletingOnly = false;
			}
			return new CompositeTenantRecipientSession(DirectorySessionFactory.CacheSessionFactory.CreateTenantRecipientSession(domainController, searchRoot, lcid, readOnly, consistencyMode, networkCredential, sessionSettings, configScopes, callerFileLine, memberName, callerFilePath), DirectorySessionFactory.NonCacheSessionFactory.CreateTenantRecipientSession(domainController, searchRoot, lcid, readOnly, consistencyMode, networkCredential, sessionSettings, configScopes, callerFileLine, memberName, callerFilePath), cacheSessionForDeletingOnly);
		}

		public override IRootOrganizationRecipientSession CreateRootOrgRecipientSession(string domainController, ADObjectId searchRoot, int lcid, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, ConfigScopes configScopes, int callerFileLine, string memberName, string callerFilePath)
		{
			return DirectorySessionFactory.NonCacheSessionFactory.CreateRootOrgRecipientSession(domainController, searchRoot, lcid, readOnly, consistencyMode, networkCredential, sessionSettings, configScopes, callerFileLine, memberName, callerFilePath);
		}

		public override IRootOrganizationRecipientSession CreateRootOrgRecipientSession(string domainController, ADObjectId searchRoot, int lcid, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, int callerFileLine, string memberName, string callerFilePath)
		{
			return DirectorySessionFactory.NonCacheSessionFactory.CreateRootOrgRecipientSession(domainController, searchRoot, lcid, readOnly, consistencyMode, networkCredential, sessionSettings, callerFileLine, memberName, callerFilePath);
		}

		public override IRecipientSession GetReducedRecipientSession(IRecipientSession baseSession, int callerFileLine, string memberName, string callerFilePath)
		{
			return new CompositeRecipientSession(DirectorySessionFactory.CacheSessionFactory.GetReducedRecipientSession(baseSession, callerFileLine, memberName, callerFilePath), DirectorySessionFactory.NonCacheSessionFactory.GetReducedRecipientSession(baseSession, callerFileLine, memberName, callerFilePath), true);
		}
	}
}
