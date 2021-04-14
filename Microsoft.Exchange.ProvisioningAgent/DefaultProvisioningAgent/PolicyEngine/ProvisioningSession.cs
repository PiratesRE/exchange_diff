using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Provisioning;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Provisioning;
using Microsoft.Exchange.ProvisioningAgent;

namespace Microsoft.Exchange.DefaultProvisioningAgent.PolicyEngine
{
	internal class ProvisioningSession
	{
		public ProvisioningSession(ProvisioningHandler provisioningHandler)
		{
			if (provisioningHandler == null)
			{
				throw new ArgumentNullException("provisioningHandler");
			}
			this.provisioningHandler = provisioningHandler;
			this.logger = this.provisioningHandler.LogMessage;
			this.taskName = this.provisioningHandler.TaskName;
			if (this.provisioningHandler.UserScope.CurrentOrganizationId != null)
			{
				this.organizationId = this.provisioningHandler.UserScope.CurrentOrganizationId;
			}
			else if (this.provisioningHandler.UserScope.ExecutingUserOrganizationId != null)
			{
				this.organizationId = this.provisioningHandler.UserScope.ExecutingUserOrganizationId;
			}
			string domainController = this.provisioningHandler.UserSpecifiedParameters["DomainController"] as string;
			UserScope userScope = this.provisioningHandler.UserScope;
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), userScope.CurrentOrganizationId, userScope.ExecutingUserOrganizationId, false);
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(domainController, true, ConsistencyMode.PartiallyConsistent, sessionSettings, 77, ".ctor", "f:\\15.00.1497\\sources\\dev\\Management\\src\\ProvisioningAgent\\PolicyEngine\\ProvisioningSession.cs");
			this.policyDataProvider = new DefaultPolicyDataProvider(tenantOrTopologyConfigurationSession);
		}

		internal ProvisioningSession(string taskName, OrganizationId organizationId, LogMessageDelegate logger, IPolicyDataProvider policyDataProvider)
		{
			this.TaskName = taskName;
			this.OrganizationId = organizationId;
			this.PolicyDataProvider = policyDataProvider;
			this.logger = logger;
		}

		public string TaskName
		{
			get
			{
				return this.taskName;
			}
			set
			{
				if (this.provisioningHandler == null && string.IsNullOrEmpty(value))
				{
					throw new ArgumentNullException("TaskName");
				}
				this.taskName = value;
			}
		}

		public OrganizationId OrganizationId
		{
			get
			{
				return this.organizationId;
			}
			set
			{
				this.organizationId = value;
			}
		}

		internal LruPolicyDataCache PolicyDataCache
		{
			get
			{
				return this.lruPolicyCache;
			}
		}

		public LogMessageDelegate Logger
		{
			get
			{
				LogMessageDelegate result;
				if ((result = this.logger) == null)
				{
					result = delegate(string message)
					{
					};
				}
				return result;
			}
			set
			{
				this.logger = value;
			}
		}

		public IPolicyDataProvider PolicyDataProvider
		{
			get
			{
				return this.policyDataProvider;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("PolicyDataProvider");
				}
				this.policyDataProvider = value;
			}
		}

		public virtual IConfigurable ProvisionDefaultProperties()
		{
			this.Logger(Strings.VerboseProvisionDefaultProperties((this.OrganizationId.ConfigurationUnit == null) ? "null" : this.OrganizationId.ConfigurationUnit.DistinguishedName));
			if (!PolicyConfiguration.Task2DefaultObjectTypeDictionary.ContainsKey(this.TaskName))
			{
				this.Logger(Strings.VerboseNoDefaultForTask(this.TaskName));
				return null;
			}
			Type type = PolicyConfiguration.Task2DefaultObjectTypeDictionary[this.TaskName];
			this.Logger(Strings.VerboseTaskUseTypeAsDefault(this.TaskName, type.FullName));
			PolicyDataCacheKey key = new PolicyDataCacheKey(this.OrganizationId, type, ProvisioningPolicyType.Template);
			IEnumerable<ADProvisioningPolicy> effectiveProvisioningPolicy;
			if (!this.PolicyDataCache.TryGetValue(key, out effectiveProvisioningPolicy))
			{
				effectiveProvisioningPolicy = this.PolicyDataProvider.GetEffectiveProvisioningPolicy(this.OrganizationId, type, ProvisioningPolicyType.Template, 2, (this.provisioningHandler != null) ? this.provisioningHandler.ProvisioningCache : ProvisioningCache.Instance);
				this.PolicyDataCache.Add(key, effectiveProvisioningPolicy);
			}
			if (effectiveProvisioningPolicy == null)
			{
				this.Logger(Strings.VerboseZeroProvisioningPolicyFound);
				return null;
			}
			ADProvisioningPolicy adprovisioningPolicy = null;
			foreach (ADProvisioningPolicy adprovisioningPolicy2 in effectiveProvisioningPolicy)
			{
				if (adprovisioningPolicy != null)
				{
					throw new ProvisioningDataCorruptException(Strings.MultiTemplatePolicyFound(ProvisioningHelper.GetProvisioningObjectTag(type)));
				}
				adprovisioningPolicy = adprovisioningPolicy2;
			}
			IConfigurable configurable = null;
			if (adprovisioningPolicy != null)
			{
				this.Logger(Strings.VerboseHandlingProvisioningPolicyFound(adprovisioningPolicy.DistinguishedName));
				ProvisioningSession.ValidatePolicy(adprovisioningPolicy);
				configurable = (IConfigurable)Activator.CreateInstance(type);
				if (configurable is ADObject)
				{
					ADObject adobject = (ADObject)configurable;
					adobject.EnableSaveCalculatedValues();
				}
				configurable.ResetChangeTracking();
				IEnumerable<IProvisioningTemplate> provisioningTemplateRules = adprovisioningPolicy.ProvisioningTemplateRules;
				if (provisioningTemplateRules != null)
				{
					ProvisioningContext context = new ProvisioningContext(this.TaskName, (this.provisioningHandler != null) ? this.provisioningHandler.UserSpecifiedParameters : null);
					foreach (IProvisioningTemplate provisioningTemplate in provisioningTemplateRules)
					{
						if (provisioningTemplate.TargetObjectTypes.Contains(type))
						{
							provisioningTemplate.Context = context;
							provisioningTemplate.Provision(adprovisioningPolicy, configurable);
						}
					}
				}
				adprovisioningPolicy.ProvisionCustomDefaultProperties(configurable);
				ADObject adobject2 = configurable as ADObject;
				if (adobject2 != null)
				{
					this.Logger(TaskVerboseStringHelper.GetConfigurableObjectChangedProperties(adobject2));
				}
			}
			else
			{
				this.Logger(Strings.VerboseZeroProvisioningPolicyFound);
			}
			return configurable;
		}

		public static OrganizationId ResolveOrganizationContainerId(IConfigurable readOnlyPresentationObject)
		{
			if (readOnlyPresentationObject is MailEnabledRecipient)
			{
				return (OrganizationId)((MailEnabledRecipient)readOnlyPresentationObject)[ADObjectSchema.OrganizationId];
			}
			if (readOnlyPresentationObject is ADPublicFolder)
			{
				return (OrganizationId)((ADPublicFolder)readOnlyPresentationObject)[ADObjectSchema.OrganizationId];
			}
			throw new NotImplementedException(string.Format("BUGBUG: policy engine cannot resolve the organization container identity of the object '{0}' of of object type '{1}'.", readOnlyPresentationObject.Identity.ToString(), ProvisioningHelper.GetProvisioningObjectTag(readOnlyPresentationObject.GetType())));
		}

		public virtual ProvisioningValidationError[] Validate(IConfigurable readOnlyPresentationObject)
		{
			if (readOnlyPresentationObject == null)
			{
				throw new ArgumentNullException("readOnlyPresentationObject");
			}
			Type type = readOnlyPresentationObject.GetType();
			this.Logger(Strings.VerboseToBeValidateObject((readOnlyPresentationObject.Identity == null) ? "null" : readOnlyPresentationObject.Identity.ToString(), type.FullName));
			if (!PolicyConfiguration.ObjectType2PolicyEntryDictionary.ContainsKey(type))
			{
				this.Logger(Strings.VerboseNoNeedToValidate(type.FullName));
				return null;
			}
			OrganizationId organizationId = ProvisioningSession.ResolveOrganizationContainerId(readOnlyPresentationObject);
			this.Logger(Strings.VerboseWorkingOrganizationForPolicy((organizationId.ConfigurationUnit == null) ? "null" : organizationId.ConfigurationUnit.DistinguishedName));
			PolicyDataCacheKey key = new PolicyDataCacheKey(organizationId, type, ProvisioningPolicyType.Enforcement);
			IEnumerable<ADProvisioningPolicy> effectiveProvisioningPolicy;
			if (!this.PolicyDataCache.TryGetValue(key, out effectiveProvisioningPolicy))
			{
				effectiveProvisioningPolicy = this.PolicyDataProvider.GetEffectiveProvisioningPolicy(organizationId, type, ProvisioningPolicyType.Enforcement, 0, (this.provisioningHandler != null) ? this.provisioningHandler.ProvisioningCache : ProvisioningCache.Instance);
				this.PolicyDataCache.Add(key, effectiveProvisioningPolicy);
			}
			if (effectiveProvisioningPolicy == null)
			{
				this.Logger(Strings.VerboseZeroProvisioningPolicyFound);
				return null;
			}
			foreach (ADProvisioningPolicy policy in effectiveProvisioningPolicy)
			{
				ProvisioningSession.ValidatePolicy(policy);
			}
			List<ProvisioningValidationError> list = new List<ProvisioningValidationError>();
			bool flag = false;
			foreach (ADProvisioningPolicy adprovisioningPolicy in effectiveProvisioningPolicy)
			{
				flag = true;
				this.Logger(Strings.VerboseHandlingProvisioningPolicyFound(adprovisioningPolicy.DistinguishedName));
				IEnumerable<IProvisioningEnforcement> provisioningEnforcementRules = adprovisioningPolicy.ProvisioningEnforcementRules;
				ProvisioningValidationError[] array;
				if (provisioningEnforcementRules != null)
				{
					ProvisioningContext context = new ProvisioningContext(this.TaskName, (this.provisioningHandler != null) ? this.provisioningHandler.UserSpecifiedParameters : null);
					foreach (IProvisioningEnforcement provisioningEnforcement in provisioningEnforcementRules)
					{
						if (provisioningEnforcement.TargetObjectTypes.Contains(type))
						{
							provisioningEnforcement.Context = context;
							if (provisioningEnforcement.IsApplicable(readOnlyPresentationObject))
							{
								array = provisioningEnforcement.Validate(adprovisioningPolicy, readOnlyPresentationObject);
								if (array != null)
								{
									list.AddRange(array);
								}
							}
						}
					}
				}
				array = adprovisioningPolicy.ProvisioningCustomValidate(readOnlyPresentationObject);
				if (array != null)
				{
					list.AddRange(array);
				}
			}
			if (!flag)
			{
				this.Logger(Strings.VerboseZeroProvisioningPolicyFound);
			}
			return list.ToArray();
		}

		protected static void ValidatePolicy(ADProvisioningPolicy policy)
		{
			ValidationError[] array = policy.ValidateRead();
			if (array.Length == 0)
			{
				return;
			}
			string value = "\r\n\t";
			StringBuilder stringBuilder = new StringBuilder(Strings.ErrorProvisioningPolicyCorrupt(policy.Id.ToString()));
			foreach (ValidationError validationError in array)
			{
				stringBuilder.Append(value);
				stringBuilder.Append(validationError.Description);
			}
			throw new ProvisioningPolicyValidationException(new LocalizedString(stringBuilder.ToString()));
		}

		private ProvisioningHandler provisioningHandler;

		private string taskName;

		private OrganizationId organizationId;

		private LogMessageDelegate logger;

		private IPolicyDataProvider policyDataProvider;

		private LruPolicyDataCache lruPolicyCache = new LruPolicyDataCache(1000);
	}
}
