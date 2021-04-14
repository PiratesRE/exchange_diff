using System;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.PartnerToken;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ExchangeRunspaceConfigurationCache : BaseWebCache<string, ExchangeRunspaceConfiguration>
	{
		internal ExchangeRunspaceConfigurationCache() : base(ExchangeRunspaceConfigurationCache.ExchangeRunspaceConfigurationCacheKeyPrefix, SlidingOrAbsoluteTimeout.Absolute, ExchangeRunspaceConfigurationCache.CacheTimeoutInMinutes)
		{
		}

		public static ExchangeRunspaceConfigurationCache Singleton
		{
			get
			{
				return ExchangeRunspaceConfigurationCache.singleton;
			}
		}

		public ExchangeRunspaceConfiguration Get(IIdentity impersonatingIdentity, OrganizationId impersonatedOrganizationId, bool forceNewRunspace = false)
		{
			string impersonatingUserId = string.Empty;
			if (impersonatingIdentity is PartnerIdentity)
			{
				PartnerIdentity partnerIdentity = impersonatingIdentity as PartnerIdentity;
				DelegatedPrincipal delegatedPrincipal = partnerIdentity.DelegatedPrincipal;
				impersonatingUserId = delegatedPrincipal.ToString();
				impersonatingIdentity = delegatedPrincipal.Identity;
			}
			else if (impersonatingIdentity is WindowsIdentity)
			{
				WindowsIdentity windowsIdentity = impersonatingIdentity as WindowsIdentity;
				impersonatingUserId = windowsIdentity.User.ToString();
			}
			else if (impersonatingIdentity is ClientSecurityContextIdentity)
			{
				ClientSecurityContextIdentity clientSecurityContextIdentity = impersonatingIdentity as ClientSecurityContextIdentity;
				impersonatingUserId = clientSecurityContextIdentity.Sid.ToString();
			}
			return this.Get(impersonatingIdentity, impersonatingUserId, impersonatedOrganizationId, forceNewRunspace);
		}

		public ExchangeRunspaceConfiguration Get(AuthZClientInfo clientInfo, OrganizationId impersonatedOrganizationId, bool forceNewRunspace = false)
		{
			if (clientInfo == null || clientInfo.ObjectSid == null)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<bool, bool>((long)this.GetHashCode(), "[ExchangeRunspaceConfigurationCache::Get] Either clientInfo was null ({0}) or clientInfo.ObjectSid was null ({1})", clientInfo == null, clientInfo != null && clientInfo.ObjectSid == null);
				throw new ImpersonateUserDeniedException();
			}
			IIdentity impersonatingIdentity = new SidWithGroupsIdentity(clientInfo.ObjectSid.ToString(), string.Empty, clientInfo.ClientSecurityContext);
			string impersonatingUserId = clientInfo.ObjectSid.ToString();
			return this.Get(impersonatingIdentity, impersonatingUserId, impersonatedOrganizationId, forceNewRunspace);
		}

		private ExchangeRunspaceConfiguration Get(IIdentity impersonatingIdentity, string impersonatingUserId, OrganizationId impersonatedOrganizationId, bool forceNewRunspace)
		{
			string text = this.ConstructKey(impersonatingUserId, impersonatedOrganizationId);
			ExchangeRunspaceConfiguration exchangeRunspaceConfiguration = null;
			if (!forceNewRunspace)
			{
				exchangeRunspaceConfiguration = base.Get(text);
			}
			if (exchangeRunspaceConfiguration == null)
			{
				ExchangeRunspaceConfigurationSettings settings;
				if (impersonatedOrganizationId == null)
				{
					settings = ExchangeRunspaceConfigurationSettings.GetDefaultInstance();
				}
				else
				{
					string tenantOrganization = null;
					if (impersonatedOrganizationId.ConfigurationUnit != null)
					{
						tenantOrganization = impersonatedOrganizationId.ConfigurationUnit.Parent.Name;
					}
					settings = new ExchangeRunspaceConfigurationSettings(ExchangeRunspaceConfigurationSettings.ExchangeApplication.EWS, tenantOrganization, ExchangeRunspaceConfigurationSettings.SerializationLevel.None);
				}
				exchangeRunspaceConfiguration = new ExchangeRunspaceConfiguration(impersonatingIdentity, settings);
				this.ForceAdd(text, exchangeRunspaceConfiguration);
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>((long)this.GetHashCode(), "[ExchangeRunspaceConfigurationCache::Get] No ExchangeRunspaceConfiguration with key: {0}", text);
			}
			return exchangeRunspaceConfiguration;
		}

		private string ConstructKey(string sidOrName, OrganizationId organizationId)
		{
			return sidOrName + ((organizationId != null) ? (":" + organizationId.ToString()) : string.Empty);
		}

		private static readonly string ExchangeRunspaceConfigurationCacheKeyPrefix = "_ERCKP_";

		private static readonly int CacheTimeoutInMinutes = Global.GetAppSettingAsInt("ExchangeRunspaceConfigurationCacheTimeoutInMinutes", 5);

		private static ExchangeRunspaceConfigurationCache singleton = new ExchangeRunspaceConfigurationCache();
	}
}
