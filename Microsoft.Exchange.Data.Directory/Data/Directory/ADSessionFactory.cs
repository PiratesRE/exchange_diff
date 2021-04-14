using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory
{
	internal class ADSessionFactory : DirectorySessionFactory
	{
		[Conditional("DEBUG")]
		private void EnsureSessionSettingsScope(ADSessionSettings sessionSettings, ADSessionFactory.IntendedUse intendedUse)
		{
			if (sessionSettings.ConfigScopes == ConfigScopes.TenantSubTree)
			{
				return;
			}
			bool flag = OrganizationId.ForestWideOrgId.Equals(sessionSettings.CurrentOrganizationId);
			bool flag2 = sessionSettings.ConfigScopes == ConfigScopes.AllTenants || (sessionSettings.ConfigScopes == ConfigScopes.TenantLocal && !flag);
			if (flag2 != (intendedUse == ADSessionFactory.IntendedUse.Tenant))
			{
				string message = string.Format("Invalid ADSessionFactory usage: the sessionSettings passed has ConfigScopes={0}, CurrentOrganizationId is {1}root org. Intended use: {2}", sessionSettings.ConfigScopes, flag ? string.Empty : "not ", intendedUse);
				ExTraceGlobals.GetConnectionTracer.TraceDebug((long)this.GetHashCode(), message);
			}
		}

		internal static bool UseAggregateSession(ADSessionSettings sessionSettings)
		{
			ADServerSettings externalServerSettings = ADSessionSettings.ExternalServerSettings;
			ADDriverContext processADContext = ADSessionSettings.GetProcessADContext();
			bool flag = processADContext != null && processADContext.Mode == ContextMode.Setup;
			bool flag2 = externalServerSettings != null && externalServerSettings.ForceADInTemplateScope;
			bool flag3 = !ConfigBase<AdDriverConfigSchema>.GetConfig<bool>("ConsumerMailboxScenarioDisabled");
			return flag3 && TemplateTenantConfiguration.IsTemplateTenant(sessionSettings.CurrentOrganizationId) && !sessionSettings.ForceADInTemplateScope && !flag2 && !flag;
		}

		public override ITenantConfigurationSession CreateTenantConfigurationSession(ConsistencyMode consistencyMode, ADSessionSettings sessionSettings, int callerFileLine, string memberName, string callerFilePath)
		{
			ADTenantConfigurationSession adtenantConfigurationSession = new ADTenantConfigurationSession(consistencyMode, sessionSettings);
			adtenantConfigurationSession.SetCallerInfo(callerFilePath, memberName, callerFileLine);
			return adtenantConfigurationSession;
		}

		public override ITenantConfigurationSession CreateTenantConfigurationSession(bool readOnly, ConsistencyMode consistencyMode, ADSessionSettings sessionSettings, int callerFileLine, string memberName, string callerFilePath)
		{
			ADTenantConfigurationSession adtenantConfigurationSession = new ADTenantConfigurationSession(readOnly, consistencyMode, sessionSettings);
			adtenantConfigurationSession.SetCallerInfo(callerFilePath, memberName, callerFileLine);
			return adtenantConfigurationSession;
		}

		public override ITenantConfigurationSession CreateTenantConfigurationSession(string domainController, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, int callerFileLine, string memberName, string callerFilePath)
		{
			ADTenantConfigurationSession adtenantConfigurationSession = new ADTenantConfigurationSession(domainController, readOnly, consistencyMode, networkCredential, sessionSettings);
			adtenantConfigurationSession.SetCallerInfo(callerFilePath, memberName, callerFileLine);
			return adtenantConfigurationSession;
		}

		public override ITenantConfigurationSession CreateTenantConfigurationSession(string domainController, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, ConfigScopes configScope, int callerFileLine, string memberName, string callerFilePath)
		{
			ADTenantConfigurationSession adtenantConfigurationSession = new ADTenantConfigurationSession(domainController, readOnly, consistencyMode, networkCredential, sessionSettings, configScope);
			adtenantConfigurationSession.SetCallerInfo(callerFilePath, memberName, callerFileLine);
			return adtenantConfigurationSession;
		}

		public override ITenantConfigurationSession CreateTenantConfigurationSession(ConsistencyMode consistencyMode, Guid externalDirectoryOrganizationId, int callerFileLine, string memberName, string callerFilePath)
		{
			ADSessionSettings adsessionSettings = ADSessionSettings.FromExternalDirectoryOrganizationId(externalDirectoryOrganizationId);
			if (adsessionSettings == null)
			{
				return null;
			}
			return this.CreateTenantConfigurationSession(consistencyMode, adsessionSettings, callerFileLine, memberName, callerFilePath);
		}

		public override ITopologyConfigurationSession CreateTopologyConfigurationSession(ConsistencyMode consistencyMode, ADSessionSettings sessionSettings, int callerFileLine, string memberName, string callerFilePath)
		{
			ADTopologyConfigurationSession adtopologyConfigurationSession = new ADTopologyConfigurationSession(consistencyMode, sessionSettings);
			adtopologyConfigurationSession.SetCallerInfo(callerFilePath, memberName, callerFileLine);
			return adtopologyConfigurationSession;
		}

		public override ITopologyConfigurationSession CreateTopologyConfigurationSession(bool readOnly, ConsistencyMode consistencyMode, ADSessionSettings sessionSettings, int callerFileLine, string memberName, string callerFilePath)
		{
			ADTopologyConfigurationSession adtopologyConfigurationSession = new ADTopologyConfigurationSession(readOnly, consistencyMode, sessionSettings);
			adtopologyConfigurationSession.SetCallerInfo(callerFilePath, memberName, callerFileLine);
			return adtopologyConfigurationSession;
		}

		public override ITopologyConfigurationSession CreateTopologyConfigurationSession(string domainController, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, int callerFileLine, string memberName, string callerFilePath)
		{
			ADTopologyConfigurationSession adtopologyConfigurationSession = new ADTopologyConfigurationSession(domainController, readOnly, consistencyMode, networkCredential, sessionSettings);
			adtopologyConfigurationSession.SetCallerInfo(callerFilePath, memberName, callerFileLine);
			return adtopologyConfigurationSession;
		}

		public override ITopologyConfigurationSession CreateTopologyConfigurationSession(string domainController, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, ConfigScopes configScope, int callerFileLine, string memberName, string callerFilePath)
		{
			ADTopologyConfigurationSession adtopologyConfigurationSession = new ADTopologyConfigurationSession(domainController, readOnly, consistencyMode, networkCredential, sessionSettings, configScope);
			adtopologyConfigurationSession.SetCallerInfo(callerFilePath, memberName, callerFileLine);
			return adtopologyConfigurationSession;
		}

		public override ITenantRecipientSession CreateTenantRecipientSession(string domainController, ADObjectId searchRoot, int lcid, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, int callerFileLine, string memberName, string callerFilePath)
		{
			DirectorySessionBase directorySessionBase = ADSessionFactory.UseAggregateSession(sessionSettings) ? new AggregateTenantRecipientSession(domainController, searchRoot, lcid, readOnly, consistencyMode, networkCredential, sessionSettings) : new ADTenantRecipientSession(domainController, searchRoot, lcid, readOnly, consistencyMode, networkCredential, sessionSettings);
			directorySessionBase.SetCallerInfo(callerFilePath, memberName, callerFileLine);
			return (ITenantRecipientSession)directorySessionBase;
		}

		public override ITenantRecipientSession CreateTenantRecipientSession(string domainController, ADObjectId searchRoot, int lcid, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, ConfigScopes configScope, int callerFileLine, string memberName, string callerFilePath)
		{
			DirectorySessionBase directorySessionBase = ADSessionFactory.UseAggregateSession(sessionSettings) ? new AggregateTenantRecipientSession(domainController, searchRoot, lcid, readOnly, consistencyMode, networkCredential, sessionSettings, configScope) : new ADTenantRecipientSession(domainController, searchRoot, lcid, readOnly, consistencyMode, networkCredential, sessionSettings, configScope);
			directorySessionBase.SetCallerInfo(callerFilePath, memberName, callerFileLine);
			return (ITenantRecipientSession)directorySessionBase;
		}

		public override IRootOrganizationRecipientSession CreateRootOrgRecipientSession(string domainController, ADObjectId searchRoot, int lcid, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, ConfigScopes configScope, int callerFileLine, string memberName, string callerFilePath)
		{
			ADRootOrganizationRecipientSession adrootOrganizationRecipientSession = new ADRootOrganizationRecipientSession(domainController, searchRoot, lcid, readOnly, consistencyMode, networkCredential, sessionSettings, configScope);
			adrootOrganizationRecipientSession.SetCallerInfo(callerFilePath, memberName, callerFileLine);
			return adrootOrganizationRecipientSession;
		}

		public override IRootOrganizationRecipientSession CreateRootOrgRecipientSession(string domainController, ADObjectId searchRoot, int lcid, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, int callerFileLine, string memberName, string callerFilePath)
		{
			ADRootOrganizationRecipientSession adrootOrganizationRecipientSession = new ADRootOrganizationRecipientSession(domainController, searchRoot, lcid, readOnly, consistencyMode, networkCredential, sessionSettings);
			adrootOrganizationRecipientSession.SetCallerInfo(callerFilePath, memberName, callerFileLine);
			return adrootOrganizationRecipientSession;
		}

		public override IRecipientSession GetReducedRecipientSession(IRecipientSession baseSession, int callerFileLine, string memberName, string callerFilePath)
		{
			if (!(baseSession is ADRecipientObjectSession) && !(baseSession is CompositeTenantRecipientSession) && !(baseSession is CompositeRecipientSession))
			{
				throw new ArgumentException("baseSession");
			}
			ADRecipientObjectSession adrecipientObjectSession;
			if (baseSession is ADRootOrganizationRecipientSession)
			{
				adrecipientObjectSession = new ADRootOrganizationRecipientSession(baseSession.DomainController, null, CultureInfo.CurrentCulture.LCID, true, baseSession.ConsistencyMode, baseSession.NetworkCredential, baseSession.SessionSettings);
			}
			else
			{
				adrecipientObjectSession = new ADTenantRecipientSession(baseSession.DomainController, null, CultureInfo.CurrentCulture.LCID, true, baseSession.ConsistencyMode, baseSession.NetworkCredential, baseSession.SessionSettings);
			}
			adrecipientObjectSession.EnableReducedRecipientSession();
			adrecipientObjectSession.UseGlobalCatalog = baseSession.UseGlobalCatalog;
			adrecipientObjectSession.SetCallerInfo(callerFilePath, memberName, callerFileLine);
			return adrecipientObjectSession;
		}

		private enum IntendedUse
		{
			Tenant,
			RootOrg
		}
	}
}
