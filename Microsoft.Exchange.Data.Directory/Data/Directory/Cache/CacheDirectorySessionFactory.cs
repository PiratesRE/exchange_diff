using System;
using System.Net;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Cache
{
	internal class CacheDirectorySessionFactory : DirectorySessionFactory
	{
		public override ITenantConfigurationSession CreateTenantConfigurationSession(ConsistencyMode consistencyMode, ADSessionSettings sessionSettings, int callerFileLine, string memberName, string callerFilePath)
		{
			CacheDirectorySession cacheDirectorySession = new CacheDirectorySession(sessionSettings);
			cacheDirectorySession.SetCallerInfo(callerFilePath, memberName, callerFileLine);
			return cacheDirectorySession;
		}

		public override ITenantConfigurationSession CreateTenantConfigurationSession(bool readOnly, ConsistencyMode consistencyMode, ADSessionSettings sessionSettings, int callerFileLine, string memberName, string callerFilePath)
		{
			CacheDirectorySession cacheDirectorySession = new CacheDirectorySession(sessionSettings);
			cacheDirectorySession.SetCallerInfo(callerFilePath, memberName, callerFileLine);
			return cacheDirectorySession;
		}

		public override ITenantConfigurationSession CreateTenantConfigurationSession(string domainController, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, int callerFileLine, string memberName, string callerFilePath)
		{
			CacheDirectorySession cacheDirectorySession = new CacheDirectorySession(sessionSettings);
			cacheDirectorySession.SetCallerInfo(callerFilePath, memberName, callerFileLine);
			return cacheDirectorySession;
		}

		public override ITenantConfigurationSession CreateTenantConfigurationSession(string domainController, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, ConfigScopes configScope, int callerFileLine, string memberName, string callerFilePath)
		{
			CacheDirectorySession cacheDirectorySession = new CacheDirectorySession(sessionSettings);
			cacheDirectorySession.SetCallerInfo(callerFilePath, memberName, callerFileLine);
			return cacheDirectorySession;
		}

		public override ITenantConfigurationSession CreateTenantConfigurationSession(ConsistencyMode consistencyMode, Guid externalDirectoryOrganizationId, int callerFileLine, string memberName, string callerFilePath)
		{
			CacheDirectorySession cacheDirectorySession = new CacheDirectorySession(ADSessionSettings.SessionSettingsFactory.Default.FromExternalDirectoryOrganizationId(externalDirectoryOrganizationId));
			cacheDirectorySession.SetCallerInfo(callerFilePath, memberName, callerFileLine);
			return cacheDirectorySession;
		}

		public override ITopologyConfigurationSession CreateTopologyConfigurationSession(ConsistencyMode consistencyMode, ADSessionSettings sessionSettings, int callerFileLine, string memberName, string callerFilePath)
		{
			throw new NotImplementedException();
		}

		public override ITopologyConfigurationSession CreateTopologyConfigurationSession(bool readOnly, ConsistencyMode consistencyMode, ADSessionSettings sessionSettings, int callerFileLine, string memberName, string callerFilePath)
		{
			throw new NotImplementedException();
		}

		public override ITopologyConfigurationSession CreateTopologyConfigurationSession(string domainController, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, int callerFileLine, string memberName, string callerFilePath)
		{
			throw new NotImplementedException();
		}

		public override ITopologyConfigurationSession CreateTopologyConfigurationSession(string domainController, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, ConfigScopes configScope, int callerFileLine, string memberName, string callerFilePath)
		{
			throw new NotImplementedException();
		}

		public override ITenantRecipientSession CreateTenantRecipientSession(string domainController, ADObjectId searchRoot, int lcid, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, int callerFileLine, string memberName, string callerFilePath)
		{
			CacheDirectorySession cacheDirectorySession = new CacheDirectorySession(sessionSettings);
			cacheDirectorySession.SetCallerInfo(callerFilePath, memberName, callerFileLine);
			return cacheDirectorySession;
		}

		public override ITenantRecipientSession CreateTenantRecipientSession(string domainController, ADObjectId searchRoot, int lcid, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, ConfigScopes configScopes, int callerFileLine, string memberName, string callerFilePath)
		{
			CacheDirectorySession cacheDirectorySession = new CacheDirectorySession(sessionSettings);
			cacheDirectorySession.SetCallerInfo(callerFilePath, memberName, callerFileLine);
			return cacheDirectorySession;
		}

		public override IRootOrganizationRecipientSession CreateRootOrgRecipientSession(string domainController, ADObjectId searchRoot, int lcid, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, ConfigScopes configScopes, int callerFileLine, string memberName, string callerFilePath)
		{
			throw new NotImplementedException();
		}

		public override IRootOrganizationRecipientSession CreateRootOrgRecipientSession(string domainController, ADObjectId searchRoot, int lcid, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, int callerFileLine, string memberName, string callerFilePath)
		{
			throw new NotImplementedException();
		}

		public override IRecipientSession GetReducedRecipientSession(IRecipientSession baseSession, int callerFileLine, string memberName, string callerFilePath)
		{
			CacheDirectorySession cacheDirectorySession = new CacheDirectorySession(baseSession.SessionSettings);
			cacheDirectorySession.SetCallerInfo(callerFilePath, memberName, callerFileLine);
			return cacheDirectorySession;
		}
	}
}
