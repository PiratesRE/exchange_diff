using System;
using System.Globalization;
using System.Net;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class FfoDirectorySesssionFactory : DirectorySessionFactory
	{
		public override ITenantConfigurationSession CreateTenantConfigurationSession(ConsistencyMode consistencyMode, ADSessionSettings sessionSettings, int callerFileLine, string memberName, string callerFilePath)
		{
			return new FfoTenantConfigurationSession(consistencyMode, sessionSettings);
		}

		public override ITenantConfigurationSession CreateTenantConfigurationSession(bool readOnly, ConsistencyMode consistencyMode, ADSessionSettings sessionSettings, int callerFileLine, string memberName, string callerFilePath)
		{
			return new FfoTenantConfigurationSession(readOnly, consistencyMode, sessionSettings);
		}

		public override ITenantConfigurationSession CreateTenantConfigurationSession(string domainController, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, int callerFileLine, string memberName, string callerFilePath)
		{
			return new FfoTenantConfigurationSession(domainController, readOnly, consistencyMode, networkCredential, sessionSettings);
		}

		public override ITenantConfigurationSession CreateTenantConfigurationSession(string domainController, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, ConfigScopes configScope, int callerFileLine, string memberName, string callerFilePath)
		{
			return new FfoTenantConfigurationSession(domainController, readOnly, consistencyMode, networkCredential, sessionSettings, configScope);
		}

		public override ITenantConfigurationSession CreateTenantConfigurationSession(ConsistencyMode consistencyMode, Guid externalDirectoryOrganizationId, int callerFileLine, string memberName, string callerFilePath)
		{
			ADObjectId tenantId = new ADObjectId(DalHelper.GetTenantDistinguishedName(externalDirectoryOrganizationId.ToString()), externalDirectoryOrganizationId);
			return new FfoTenantConfigurationSession(tenantId);
		}

		public override ITopologyConfigurationSession CreateTopologyConfigurationSession(ConsistencyMode consistencyMode, ADSessionSettings sessionSettings, int callerFileLine, string memberName, string callerFilePath)
		{
			return new ADTopologyConfigurationSession(consistencyMode, sessionSettings);
		}

		public override ITopologyConfigurationSession CreateTopologyConfigurationSession(bool readOnly, ConsistencyMode consistencyMode, ADSessionSettings sessionSettings, int callerFileLine, string memberName, string callerFilePath)
		{
			return new ADTopologyConfigurationSession(readOnly, consistencyMode, sessionSettings);
		}

		public override ITopologyConfigurationSession CreateTopologyConfigurationSession(string domainController, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, int callerFileLine, string memberName, string callerFilePath)
		{
			return new ADTopologyConfigurationSession(domainController, readOnly, consistencyMode, networkCredential, sessionSettings);
		}

		public override ITopologyConfigurationSession CreateTopologyConfigurationSession(string domainController, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, ConfigScopes configScope, int callerFileLine, string memberName, string callerFilePath)
		{
			return new ADTopologyConfigurationSession(domainController, readOnly, consistencyMode, networkCredential, sessionSettings, configScope);
		}

		public override ITenantRecipientSession CreateTenantRecipientSession(string domainController, ADObjectId searchRoot, int lcid, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, int callerFileLine, string memberName, string callerFilePath)
		{
			return new FfoTenantRecipientSession(true, readOnly, consistencyMode, networkCredential, sessionSettings);
		}

		public override ITenantRecipientSession CreateTenantRecipientSession(string domainController, ADObjectId searchRoot, int lcid, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, ConfigScopes configScope, int callerFileLine, string memberName, string callerFilePath)
		{
			return new FfoTenantRecipientSession(true, readOnly, consistencyMode, networkCredential, sessionSettings);
		}

		public override IRootOrganizationRecipientSession CreateRootOrgRecipientSession(string domainController, ADObjectId searchRoot, int lcid, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, int callerFileLine, string memberName, string callerFilePath)
		{
			return new ADRootOrganizationRecipientSession(domainController, searchRoot, lcid, readOnly, consistencyMode, networkCredential, sessionSettings);
		}

		public override IRootOrganizationRecipientSession CreateRootOrgRecipientSession(string domainController, ADObjectId searchRoot, int lcid, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, ConfigScopes configScope, int callerFileLine, string memberName, string callerFilePath)
		{
			return new ADRootOrganizationRecipientSession(domainController, searchRoot, lcid, readOnly, consistencyMode, networkCredential, sessionSettings, configScope);
		}

		public override IRecipientSession GetReducedRecipientSession(IRecipientSession baseSession, int callerFileLine, string memberName, string callerFilePath)
		{
			IRecipientSession recipientSession;
			if (baseSession is ADRootOrganizationRecipientSession)
			{
				ADRootOrganizationRecipientSession adrootOrganizationRecipientSession = new ADRootOrganizationRecipientSession(baseSession.DomainController, null, CultureInfo.CurrentCulture.LCID, true, baseSession.ConsistencyMode, baseSession.NetworkCredential, baseSession.SessionSettings);
				adrootOrganizationRecipientSession.EnableReducedRecipientSession();
				recipientSession = adrootOrganizationRecipientSession;
			}
			else
			{
				FfoTenantRecipientSession ffoTenantRecipientSession = new FfoTenantRecipientSession(baseSession.UseConfigNC, true, baseSession.ConsistencyMode, baseSession.NetworkCredential, baseSession.SessionSettings);
				ffoTenantRecipientSession.EnableReducedRecipientSession();
				recipientSession = ffoTenantRecipientSession;
			}
			recipientSession.UseGlobalCatalog = baseSession.UseGlobalCatalog;
			return recipientSession;
		}
	}
}
