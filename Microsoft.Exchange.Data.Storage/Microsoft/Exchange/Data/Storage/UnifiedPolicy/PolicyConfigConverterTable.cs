using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Exchange.Data.Storage.UnifiedPolicy
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class PolicyConfigConverterTable
	{
		static PolicyConfigConverterTable()
		{
			PolicyConfigConverterBase[] array = new PolicyConfigConverterBase[4];
			array[0] = new PolicyConfigConverter<PolicyDefinitionConfig, PolicyStorage>(ExPolicyConfigProvider.IsFFOOnline ? ADObjectSchema.Id : UnifiedPolicyStorageBaseSchema.MasterIdentity, ConfigurationObjectType.Policy, delegate(PolicyStorage storage, PolicyDefinitionConfig policyConfig)
			{
				policyConfig.Mode = storage.Mode;
				policyConfig.Scenario = storage.Scenario;
				policyConfig.DefaultPolicyRuleConfigId = storage.DefaultRuleId;
				policyConfig.Comment = storage.Comments;
				policyConfig.Description = storage.Description;
				policyConfig.Enabled = storage.IsEnabled;
				policyConfig.CreatedBy = storage.CreatedBy;
				policyConfig.LastModifiedBy = storage.LastModifiedBy;
			}, delegate(PolicyDefinitionConfig policyConfig, PolicyStorage storage)
			{
				if (policyConfig.Mode != storage.Mode)
				{
					storage.Mode = policyConfig.Mode;
				}
				storage.Scenario = policyConfig.Scenario;
				storage.DefaultRuleId = policyConfig.DefaultPolicyRuleConfigId;
				storage.Comments = policyConfig.Comment;
				storage.Description = policyConfig.Description;
				storage.IsEnabled = policyConfig.Enabled;
				storage.CreatedBy = policyConfig.CreatedBy;
				storage.LastModifiedBy = policyConfig.LastModifiedBy;
			});
			array[1] = new PolicyConfigConverter<PolicyRuleConfig, RuleStorage>(RuleStorageSchema.ParentPolicyId, ConfigurationObjectType.Rule, delegate(RuleStorage storage, PolicyRuleConfig policyConfig)
			{
				policyConfig.Mode = storage.Mode;
				policyConfig.PolicyDefinitionConfigId = storage.ParentPolicyId;
				policyConfig.Priority = storage.Priority;
				policyConfig.RuleBlob = storage.RuleBlob;
				policyConfig.Comment = storage.Comments;
				policyConfig.Description = storage.Description;
				policyConfig.Enabled = storage.IsEnabled;
				policyConfig.CreatedBy = storage.CreatedBy;
				policyConfig.LastModifiedBy = storage.LastModifiedBy;
				policyConfig.Scenario = storage.Scenario;
			}, delegate(PolicyRuleConfig policyConfig, RuleStorage storage)
			{
				if (policyConfig.Mode != storage.Mode)
				{
					storage.Mode = policyConfig.Mode;
				}
				storage.ParentPolicyId = policyConfig.PolicyDefinitionConfigId;
				storage.Priority = policyConfig.Priority;
				storage.RuleBlob = policyConfig.RuleBlob;
				storage.Comments = policyConfig.Comment;
				storage.Description = policyConfig.Description;
				storage.IsEnabled = policyConfig.Enabled;
				storage.CreatedBy = policyConfig.CreatedBy;
				storage.LastModifiedBy = policyConfig.LastModifiedBy;
				storage.Scenario = policyConfig.Scenario;
			});
			array[2] = new PolicyConfigConverter<PolicyBindingSetConfig, BindingStorage>(BindingStorageSchema.PolicyId, ConfigurationObjectType.Binding, delegate(BindingStorage storage, PolicyBindingSetConfig policyConfig)
			{
				policyConfig.PolicyDefinitionConfigId = storage.PolicyId;
				List<PolicyBindingConfig> list = new List<PolicyBindingConfig>(storage.AppliedScopes.Count);
				foreach (ScopeStorage storageScope in storage.AppliedScopes)
				{
					PolicyBindingConfig policyBindingConfig = PolicyConfigConverterTable.ToBindingScope(storageScope);
					policyBindingConfig.PolicyDefinitionConfigId = storage.PolicyId;
					list.Add(policyBindingConfig);
				}
				policyConfig.AppliedScopes = list;
			}, delegate(PolicyBindingSetConfig policyConfig, BindingStorage storage)
			{
				PolicyConfigConverterTable.<>c__DisplayClass12 CS$<>8__locals1 = new PolicyConfigConverterTable.<>c__DisplayClass12();
				CS$<>8__locals1.storage = storage;
				CS$<>8__locals1.storage.PolicyId = policyConfig.PolicyDefinitionConfigId;
				IEnumerable<PolicyBindingConfig> enumerable = policyConfig.AppliedScopes ?? ((IEnumerable<PolicyBindingConfig>)Array<PolicyBindingConfig>.Empty);
				int index;
				for (index = CS$<>8__locals1.storage.AppliedScopes.Count - 1; index >= 0; index--)
				{
					if (!enumerable.Any((PolicyBindingConfig bindingScope) => PolicyConfigConverterTable.IsSameScope(bindingScope, CS$<>8__locals1.storage.AppliedScopes[index])))
					{
						CS$<>8__locals1.storage.AppliedScopes.RemoveAt(index);
					}
				}
				using (IEnumerator<PolicyBindingConfig> enumerator = enumerable.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						PolicyBindingConfig bindingScope = enumerator.Current;
						ScopeStorage scopeStorage = CS$<>8__locals1.storage.AppliedScopes.FirstOrDefault((ScopeStorage scope) => PolicyConfigConverterTable.IsSameScope(bindingScope, scope));
						if (scopeStorage == null)
						{
							CS$<>8__locals1.storage.AppliedScopes.Add(PolicyConfigConverterTable.ToStorageScope(bindingScope, CS$<>8__locals1.storage.OrganizationalUnitRoot));
						}
						else
						{
							PolicyConfigConverterTable.UpdateStorageScope(scopeStorage, bindingScope);
						}
					}
				}
			});
			array[3] = new PolicyConfigConverter<PolicyAssociationConfig, AssociationStorage>(AssociationStorageSchema.PolicyIds, ConfigurationObjectType.Association, delegate(AssociationStorage storage, PolicyAssociationConfig policyConfig)
			{
				throw new NotImplementedException("Not in v1 of unified policy.");
			}, delegate(PolicyAssociationConfig policyConfig, AssociationStorage storage)
			{
				throw new NotImplementedException("Not in v1 of unified policy.");
			});
			PolicyConfigConverterTable.policyConverters = array;
		}

		public static PolicyConfigConverterBase GetConverterByType(Type type, bool throwException = true)
		{
			PolicyConfigConverterBase policyConfigConverterBase = PolicyConfigConverterTable.policyConverters.FirstOrDefault(delegate(PolicyConfigConverterBase entry)
			{
				if (typeof(UnifiedPolicyStorageBase).IsAssignableFrom(type))
				{
					return type.Equals(entry.StorageType);
				}
				return typeof(PolicyConfigBase).IsAssignableFrom(type) && type.Equals(entry.PolicyConfigType);
			});
			if (policyConfigConverterBase == null && throwException)
			{
				throw new InvalidOperationException(string.Format("Type {0} has no converter.", type.FullName));
			}
			return policyConfigConverterBase;
		}

		public static ConfigurationObjectType GetConfigurationObjectType(UnifiedPolicyStorageBase policyStorageObject)
		{
			ArgumentValidator.ThrowIfNull("policyStorageObject", policyStorageObject);
			PolicyConfigConverterBase policyConfigConverterBase = PolicyConfigConverterTable.policyConverters.FirstOrDefault((PolicyConfigConverterBase entry) => policyStorageObject.GetType().Equals(entry.StorageType));
			if (policyConfigConverterBase == null)
			{
				throw new InvalidOperationException(string.Format("Type {0} has no converter.", policyStorageObject.GetType()));
			}
			return policyConfigConverterBase.ConfigurationObjectType;
		}

		private static bool IsSameScope(PolicyBindingConfig bindingScope, ScopeStorage storageScope)
		{
			return bindingScope.Identity == (ExPolicyConfigProvider.IsFFOOnline ? storageScope.Guid : storageScope.MasterIdentity);
		}

		private static void UpdateStorageScope(ScopeStorage storageScope, PolicyBindingConfig bindingScope)
		{
			storageScope.Name = bindingScope.Name;
			if (storageScope.Mode != bindingScope.Mode)
			{
				storageScope.Mode = bindingScope.Mode;
			}
			storageScope.Scope = BindingMetadata.ToStorage(bindingScope.Scope);
			storageScope.PolicyVersion = ((bindingScope.Version != null) ? bindingScope.Version.InternalStorage : Guid.Empty);
		}

		private static ScopeStorage ToStorageScope(PolicyBindingConfig bindingScope, ADObjectId organizationalUnitRoot)
		{
			ScopeStorage scopeStorage = new ScopeStorage();
			scopeStorage[ADObjectSchema.OrganizationalUnitRoot] = organizationalUnitRoot;
			scopeStorage.SetId(new ADObjectId(PolicyStorage.PoliciesContainer.GetChildId(bindingScope.Identity.ToString()).DistinguishedName, bindingScope.Identity));
			scopeStorage.MasterIdentity = bindingScope.Identity;
			PolicyConfigConverterTable.UpdateStorageScope(scopeStorage, bindingScope);
			return scopeStorage;
		}

		private static PolicyBindingConfig ToBindingScope(ScopeStorage storageScope)
		{
			PolicyBindingConfig policyBindingConfig = new PolicyBindingConfig();
			policyBindingConfig.Identity = (ExPolicyConfigProvider.IsFFOOnline ? storageScope.Guid : storageScope.MasterIdentity);
			policyBindingConfig.Name = storageScope.Name;
			policyBindingConfig.Mode = storageScope.Mode;
			policyBindingConfig.Scope = BindingMetadata.FromStorage(storageScope.Scope);
			policyBindingConfig.Version = PolicyVersion.Create(storageScope.PolicyVersion);
			policyBindingConfig.ResetChangeTracking();
			return policyBindingConfig;
		}

		private static PolicyConfigConverterBase[] policyConverters;
	}
}
