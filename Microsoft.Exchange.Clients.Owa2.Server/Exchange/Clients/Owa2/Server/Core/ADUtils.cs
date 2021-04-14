using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.InfoWorker.Common.OrganizationConfiguration;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Transport.RecipientAPI;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal static class ADUtils
	{
		private static TenantConfigurationCache<PolicyTipRulesPerTenantSettings> PolicyTipRulesPerTenantSettingsCache
		{
			get
			{
				if (ADUtils.policyTipRulesPerTenantSettingsCache == null)
				{
					lock (ADUtils.lockObjectRules)
					{
						if (ADUtils.policyTipRulesPerTenantSettingsCache == null)
						{
							ADUtils.policyTipRulesPerTenantSettingsCache = new TenantConfigurationCache<PolicyTipRulesPerTenantSettings>(ADUtils.policyTipRulesCacheSize, ADUtils.policyTipRulesCacheExpirationInterval, ADUtils.policyTipRulesCacheCleanupInterval, new PerTenantCacheTracer(ExTraceGlobals.OwaPerTenantCacheTracer, "PolicyTipRulesPerTenantSettings"), new PerTenantCachePerformanceCounters("PolicyTipRulesPerTenantSettings"));
						}
					}
				}
				return ADUtils.policyTipRulesPerTenantSettingsCache;
			}
		}

		private static TenantConfigurationCache<PerTenantPolicyTipMessageConfig> PerTenantPolicyTipMessageConfigCache
		{
			get
			{
				if (ADUtils.perTenantPolicyTipMessageConfigCache == null)
				{
					lock (ADUtils.lockObjectMessageConfig)
					{
						if (ADUtils.perTenantPolicyTipMessageConfigCache == null)
						{
							ADUtils.perTenantPolicyTipMessageConfigCache = new TenantConfigurationCache<PerTenantPolicyTipMessageConfig>(ADUtils.policyTipMessageConfigCacheSize, ADUtils.policyTipMessageConfigCacheExpirationInterval, ADUtils.policyTipMessageConfigCacheCleanupInterval, new PerTenantCacheTracer(ExTraceGlobals.OwaPerTenantCacheTracer, "PerTenantPolicyTipMessageConfig"), new PerTenantCachePerformanceCounters("PerTenantPolicyTipMessageConfig"));
						}
					}
				}
				return ADUtils.perTenantPolicyTipMessageConfigCache;
			}
		}

		internal static ADRecipientSessionContext ADRecipientSessionContext
		{
			get
			{
				if (ADUtils.adRecipientSessionContext == null)
				{
					return CallContext.Current.ADRecipientSessionContext;
				}
				return ADUtils.adRecipientSessionContext;
			}
			set
			{
				ADUtils.adRecipientSessionContext = value;
			}
		}

		public static void ResetPolicyTipMessageConfigCache()
		{
			lock (ADUtils.lockObjectMessageConfig)
			{
				ADUtils.perTenantPolicyTipMessageConfigCache = null;
			}
		}

		public static void ResetPolicyTipRulesCache()
		{
			lock (ADUtils.lockObjectRules)
			{
				ADUtils.policyTipRulesPerTenantSettingsCache = null;
			}
		}

		public static PolicyTipRulesPerTenantSettings GetPolicyTipRulesPerTenantSettings(OrganizationId organizationId)
		{
			return ADUtils.PolicyTipRulesPerTenantSettingsCache.GetValue(organizationId);
		}

		internal static PolicyTipCustomizedStrings GetPolicyTipStrings(OrganizationId organizationId, string locale)
		{
			if (string.IsNullOrEmpty(locale))
			{
				throw new ArgumentNullException("locale");
			}
			if (organizationId == null)
			{
				throw new ArgumentNullException("organizationId");
			}
			PerTenantPolicyTipMessageConfig value = ADUtils.PerTenantPolicyTipMessageConfigCache.GetValue(organizationId);
			PolicyTipCustomizedStrings policyTipCustomizedStrings = null;
			string policyTipMessage = value.GetPolicyTipMessage(string.Empty, PolicyTipMessageConfigAction.Url);
			string policyTipMessage2 = value.GetPolicyTipMessage(locale, PolicyTipMessageConfigAction.NotifyOnly);
			string policyTipMessage3 = value.GetPolicyTipMessage(locale, PolicyTipMessageConfigAction.RejectOverride);
			string policyTipMessage4 = value.GetPolicyTipMessage(locale, PolicyTipMessageConfigAction.Reject);
			if (!string.IsNullOrEmpty(policyTipMessage) || !string.IsNullOrEmpty(policyTipMessage2) || !string.IsNullOrEmpty(policyTipMessage3) || !string.IsNullOrEmpty(policyTipMessage4))
			{
				policyTipCustomizedStrings = new PolicyTipCustomizedStrings();
				policyTipCustomizedStrings.ComplianceURL = policyTipMessage;
				policyTipCustomizedStrings.PolicyTipMessageNotifyString = policyTipMessage2;
				policyTipCustomizedStrings.PolicyTipMessageOverrideString = policyTipMessage3;
				policyTipCustomizedStrings.PolicyTipMessageBlockString = policyTipMessage4;
			}
			return policyTipCustomizedStrings;
		}

		public static ShortList<string> GetAllEmailAddresses(ShortList<string> addressesToExpand, OrganizationId organizationId)
		{
			if (addressesToExpand == null)
			{
				return null;
			}
			ShortList<string> shortList = new ShortList<string>();
			foreach (string emailAddress in addressesToExpand)
			{
				string[] allEmailAddresses = ADUtils.GetAllEmailAddresses(emailAddress, organizationId);
				if (allEmailAddresses != null)
				{
					foreach (string item in allEmailAddresses)
					{
						shortList.Add(item);
					}
				}
			}
			return shortList;
		}

		public static string[] GetAllEmailAddresses(string emailAddress, OrganizationId organizationId)
		{
			if (string.IsNullOrEmpty(emailAddress))
			{
				throw new ArgumentNullException("emailAddress");
			}
			if (organizationId == null)
			{
				throw new ArgumentNullException("organizationId");
			}
			if (emailAddress.IndexOf('@') > 0)
			{
				if (!SmtpAddress.IsValidSmtpAddress(emailAddress))
				{
					throw new ArgumentException(string.Format("emailAddress:{0} is not a valid ProxyAddress", emailAddress));
				}
				ProxyAddress proxyAddress = new SmtpProxyAddress(emailAddress, false);
				IRecipientSession recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId), 345, "GetAllEmailAddresses", "f:\\15.00.1497\\sources\\dev\\clients\\src\\Owa2\\Server\\Core\\PolicyTips\\ADUtils.cs");
				ADRawEntry lookupResult = null;
				ADNotificationAdapter.RunADOperation(delegate()
				{
					lookupResult = recipientSession.FindByProxyAddress(proxyAddress, ADUtils.PropertiesToGet);
				});
				if (lookupResult != null)
				{
					ProxyAddressCollection proxyAddressCollection = (ProxyAddressCollection)lookupResult[ADRecipientSchema.EmailAddresses];
					if (proxyAddressCollection != null && proxyAddressCollection.Count > 0)
					{
						string[] array = new string[proxyAddressCollection.Count];
						int num = 0;
						foreach (ProxyAddress proxyAddress2 in proxyAddressCollection)
						{
							array[num++] = proxyAddress2.AddressString;
						}
						return array;
					}
				}
			}
			return new string[]
			{
				emailAddress
			};
		}

		public static bool IsMemberOf(string emailAddress, RoutingAddress distributionListRoutingAddress, OrganizationId organizationId)
		{
			if (string.IsNullOrEmpty(emailAddress))
			{
				throw new ArgumentNullException("emailAddress");
			}
			if (organizationId == null)
			{
				throw new ArgumentNullException("organizationId");
			}
			if (!SmtpAddress.IsValidSmtpAddress(emailAddress))
			{
				throw new ArgumentException(string.Format("emailAddress:{0} is not a valid ProxyAddress", emailAddress));
			}
			UserIdentity userIdentity = ADIdentityInformationCache.Singleton.GetUserIdentity(emailAddress, ADUtils.ADRecipientSessionContext);
			Guid objectGuid = userIdentity.ObjectGuid;
			CachedOrganizationConfiguration cachedOrganizationConfiguration = ADUtils.GetCachedOrganizationConfiguration(organizationId);
			return cachedOrganizationConfiguration.GroupsConfiguration.IsMemberOf(objectGuid, distributionListRoutingAddress);
		}

		public static bool IsAnyInternal(List<RoutingAddress> routingAddresses, OrganizationId organizationId, ConditionEvaluationMode mode, ref List<string> internalPropertyValues)
		{
			if (routingAddresses == null)
			{
				throw new ArgumentNullException("routingAddresses");
			}
			if (internalPropertyValues == null)
			{
				internalPropertyValues = new List<string>();
			}
			bool result = false;
			foreach (RoutingAddress routingAddress in routingAddresses)
			{
				if (ADUtils.IsInternal(routingAddress, organizationId))
				{
					if (mode == ConditionEvaluationMode.Optimized)
					{
						return true;
					}
					internalPropertyValues.Add(routingAddress.ToString());
					result = true;
				}
			}
			return result;
		}

		public static bool IsInternal(RoutingAddress routingAddress, OrganizationId organizationId)
		{
			if (organizationId == null)
			{
				throw new ArgumentNullException("organizationId");
			}
			if (string.IsNullOrEmpty(routingAddress.DomainPart))
			{
				throw new ArgumentException(string.Format("routingAddress.DomainPart is null or empty for the routingaddress:{0}.", routingAddress));
			}
			OwaPerTenantAcceptedDomains owaPerTenantAcceptedDomains = ADCacheUtils.GetOwaPerTenantAcceptedDomains(organizationId);
			OwaPerTenantRemoteDomains owaPerTenantRemoteDomains = ADCacheUtils.GetOwaPerTenantRemoteDomains(organizationId);
			IsInternalResolver isInternalResolver = new IsInternalResolver(organizationId, new IsInternalResolver.GetAcceptedDomainCollectionDelegate(owaPerTenantAcceptedDomains.GetAcceptedDomainMap), new IsInternalResolver.GetRemoteDomainCollectionDelegate(owaPerTenantRemoteDomains.GetRemoteDomainMap));
			return isInternalResolver.IsInternal(new RoutingDomain(routingAddress.DomainPart));
		}

		public static bool IsAnyExternal(List<RoutingAddress> routingAddresses, OrganizationId organizationId, ConditionEvaluationMode mode, ref List<string> externalPropertyValues)
		{
			if (routingAddresses == null)
			{
				throw new ArgumentNullException("routingAddresses");
			}
			if (externalPropertyValues == null)
			{
				externalPropertyValues = new List<string>();
			}
			bool result = false;
			foreach (RoutingAddress routingAddress in routingAddresses)
			{
				if (!ADUtils.IsInternal(routingAddress, organizationId))
				{
					if (mode == ConditionEvaluationMode.Optimized)
					{
						return true;
					}
					externalPropertyValues.Add(routingAddress.ToString());
					result = true;
				}
			}
			return result;
		}

		public static bool IsExternal(RoutingAddress routingAddress, OrganizationId organizationId)
		{
			return !ADUtils.IsInternal(routingAddress, organizationId);
		}

		public static bool IsAnyExternalPartner(List<RoutingAddress> routingAddresses, OrganizationId organizationId, ConditionEvaluationMode mode, ref List<string> externalPartnerPropertyValues)
		{
			if (routingAddresses == null)
			{
				throw new ArgumentNullException("routingAddresses");
			}
			if (externalPartnerPropertyValues == null)
			{
				externalPartnerPropertyValues = new List<string>();
			}
			bool result = false;
			foreach (RoutingAddress routingAddress in routingAddresses)
			{
				if (ADUtils.IsExternalPartner(routingAddress, organizationId))
				{
					if (mode == ConditionEvaluationMode.Optimized)
					{
						return true;
					}
					externalPartnerPropertyValues.Add(routingAddress.ToString());
					result = true;
				}
			}
			return result;
		}

		public static bool IsExternalPartner(RoutingAddress routingAddress, OrganizationId organizationId)
		{
			if (organizationId == null)
			{
				throw new ArgumentNullException("emailAddress");
			}
			if (string.IsNullOrEmpty(routingAddress.DomainPart))
			{
				throw new ArgumentException(string.Format("routingAddress.DomainPart is null or empty for the routingaddress:{0}.", routingAddress));
			}
			OwaPerTenantTransportSettings owaPerTenantTransportSettings = ADCacheUtils.GetOwaPerTenantTransportSettings(organizationId);
			return owaPerTenantTransportSettings.IsTLSSendSecureDomain(routingAddress.DomainPart);
		}

		public static bool IsAnyExternalNonPartner(List<RoutingAddress> routingAddresses, OrganizationId organizationId, ConditionEvaluationMode mode, ref List<string> externalNonPartnerPropertyValues)
		{
			bool result = false;
			if (externalNonPartnerPropertyValues == null)
			{
				externalNonPartnerPropertyValues = new List<string>();
			}
			foreach (RoutingAddress routingAddress in routingAddresses)
			{
				if (ADUtils.IsExternal(routingAddress, organizationId) && !ADUtils.IsExternalPartner(routingAddress, organizationId))
				{
					if (mode == ConditionEvaluationMode.Optimized)
					{
						return true;
					}
					externalNonPartnerPropertyValues.Add(routingAddress.ToString());
					result = true;
				}
			}
			return result;
		}

		private static CachedOrganizationConfiguration GetCachedOrganizationConfiguration(OrganizationId organizationId)
		{
			return CachedOrganizationConfiguration.GetInstance(organizationId, CachedOrganizationConfiguration.ConfigurationTypes.GroupsConfiguration);
		}

		private const string PerTenantCachePolicyTipRulesName = "PolicyTipRulesPerTenantSettings";

		private const string PerTenantCachePolicyTipMessageConfigName = "PerTenantPolicyTipMessageConfig";

		private static long policyTipRulesCacheSize = BaseApplication.GetAppSetting<long>("DlpPolicyTipRulesCacheSize", 50L) * 1024L * 1024L;

		private static TimeSpan policyTipRulesCacheExpirationInterval = TimeSpan.FromMinutes((double)BaseApplication.GetAppSetting<int>("DlpPolicyTipRulesCacheExpirationInterval", 15));

		private static TimeSpan policyTipRulesCacheCleanupInterval = TimeSpan.FromMinutes((double)BaseApplication.GetAppSetting<int>("DlpPolicyTipRulesCacheCleanupInterval", 15));

		private static TenantConfigurationCache<PolicyTipRulesPerTenantSettings> policyTipRulesPerTenantSettingsCache;

		private static long policyTipMessageConfigCacheSize = BaseApplication.GetAppSetting<long>("DlpPolicyTipMessageConfigCacheSize", 50L) * 1024L * 1024L;

		private static TimeSpan policyTipMessageConfigCacheExpirationInterval = TimeSpan.FromMinutes((double)BaseApplication.GetAppSetting<int>("DlpPolicyTipMessageConfigCacheExpirationInterval", 60));

		private static TimeSpan policyTipMessageConfigCacheCleanupInterval = TimeSpan.FromMinutes((double)BaseApplication.GetAppSetting<int>("DlpPolicyTipMessageConfigCacheCleanupInterval", 60));

		private static TenantConfigurationCache<PerTenantPolicyTipMessageConfig> perTenantPolicyTipMessageConfigCache;

		private static readonly List<PropertyDefinition> PropertiesToGet = new List<PropertyDefinition>
		{
			ADRecipientSchema.EmailAddresses
		};

		private static object lockObjectRules = new object();

		private static object lockObjectMessageConfig = new object();

		private static volatile ADRecipientSessionContext adRecipientSessionContext;
	}
}
