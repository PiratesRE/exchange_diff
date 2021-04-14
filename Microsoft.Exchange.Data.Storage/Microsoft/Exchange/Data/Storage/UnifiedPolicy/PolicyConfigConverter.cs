using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Exchange.Data.Storage.UnifiedPolicy
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PolicyConfigConverter<TPolicyConfig, TPolicyStorage> : PolicyConfigConverterBase where TPolicyConfig : PolicyConfigBase where TPolicyStorage : UnifiedPolicyStorageBase, new()
	{
		public PolicyConfigConverter(ADPropertyDefinition policyIdProperty, ConfigurationObjectType configurationObjectType, Action<TPolicyStorage, TPolicyConfig> copyPropertiesToPolicyConfigDelegate, Action<TPolicyConfig, TPolicyStorage> copyPropertiesToStorageDelegate) : base(typeof(TPolicyConfig), configurationObjectType, typeof(TPolicyStorage), policyIdProperty)
		{
			ArgumentValidator.ThrowIfNull("copyPropertiesToPolicyConfigDelegate", copyPropertiesToPolicyConfigDelegate);
			ArgumentValidator.ThrowIfNull("copyPropertiesToStorageDelegate", copyPropertiesToStorageDelegate);
			this.copyPropertiesToPolicyConfigDelegate = copyPropertiesToPolicyConfigDelegate;
			this.copyPropertiesToStorageDelegate = copyPropertiesToStorageDelegate;
		}

		public override Func<QueryFilter, ObjectId, bool, SortBy, IConfigurable[]> GetFindStorageObjectsDelegate(ExPolicyConfigProvider provider)
		{
			return new Func<QueryFilter, ObjectId, bool, SortBy, IConfigurable[]>(provider.Find<TPolicyStorage>);
		}

		public override PolicyConfigBase ConvertFromStorage(ExPolicyConfigProvider provider, UnifiedPolicyStorageBase storageObject)
		{
			ArgumentValidator.ThrowIfNull("provider", provider);
			ArgumentValidator.ThrowIfNull("storageObject", storageObject);
			PolicyConfigBase policyConfigBase = provider.NewBlankConfigInstance<TPolicyConfig>();
			if (!provider.ReadOnly)
			{
				policyConfigBase.RawObject = storageObject;
			}
			Guid identity = storageObject.Guid;
			if (!ExPolicyConfigProvider.IsFFOOnline)
			{
				identity = storageObject.MasterIdentity;
			}
			policyConfigBase.Identity = identity;
			policyConfigBase.Name = storageObject.Name;
			policyConfigBase.Version = PolicyVersion.Create(storageObject.PolicyVersion);
			policyConfigBase.Workload = storageObject.Workload;
			policyConfigBase.WhenChangedUTC = storageObject.WhenChangedUTC;
			policyConfigBase.WhenCreatedUTC = storageObject.WhenChangedUTC;
			this.copyPropertiesToPolicyConfigDelegate((TPolicyStorage)((object)storageObject), (TPolicyConfig)((object)policyConfigBase));
			policyConfigBase.ResetChangeTracking();
			return policyConfigBase;
		}

		public override UnifiedPolicyStorageBase ConvertToStorage(ExPolicyConfigProvider provider, PolicyConfigBase policyConfig)
		{
			ArgumentValidator.ThrowIfNull("provider", provider);
			ArgumentValidator.ThrowIfNull("storageObject", policyConfig);
			UnifiedPolicyStorageBase unifiedPolicyStorageBase = policyConfig.RawObject as TPolicyStorage;
			if (unifiedPolicyStorageBase == null)
			{
				unifiedPolicyStorageBase = Activator.CreateInstance<TPolicyStorage>();
				unifiedPolicyStorageBase.OrganizationId = provider.GetOrganizationId();
				if (ExPolicyConfigProvider.IsFFOOnline)
				{
					unifiedPolicyStorageBase.SetId(new ADObjectId(PolicyStorage.PoliciesContainer.GetChildId(policyConfig.Name).DistinguishedName, policyConfig.Identity));
				}
				else
				{
					PolicyRuleConfig policyRuleConfig = policyConfig as PolicyRuleConfig;
					ADObjectId policyConfigContainer = provider.GetPolicyConfigContainer((policyRuleConfig == null) ? null : new Guid?(policyRuleConfig.PolicyDefinitionConfigId));
					unifiedPolicyStorageBase.SetId(policyConfigContainer.GetChildId(policyConfig.Name));
					unifiedPolicyStorageBase.MasterIdentity = policyConfig.Identity;
				}
			}
			else if ((ExPolicyConfigProvider.IsFFOOnline && policyConfig.Identity != unifiedPolicyStorageBase.Guid) || (!ExPolicyConfigProvider.IsFFOOnline && policyConfig.Identity != unifiedPolicyStorageBase.MasterIdentity))
			{
				throw new PolicyConfigProviderPermanentException(ServerStrings.ErrorCouldNotUpdateMasterIdentityProperty(policyConfig.Name));
			}
			if (policyConfig.Version != null)
			{
				unifiedPolicyStorageBase.PolicyVersion = policyConfig.Version.InternalStorage;
			}
			unifiedPolicyStorageBase.Name = policyConfig.Name;
			if (unifiedPolicyStorageBase.Workload != policyConfig.Workload)
			{
				unifiedPolicyStorageBase.Workload = policyConfig.Workload;
			}
			this.copyPropertiesToStorageDelegate((TPolicyConfig)((object)policyConfig), (TPolicyStorage)((object)unifiedPolicyStorageBase));
			return unifiedPolicyStorageBase;
		}

		private Action<TPolicyStorage, TPolicyConfig> copyPropertiesToPolicyConfigDelegate;

		private Action<TPolicyConfig, TPolicyStorage> copyPropertiesToStorageDelegate;
	}
}
