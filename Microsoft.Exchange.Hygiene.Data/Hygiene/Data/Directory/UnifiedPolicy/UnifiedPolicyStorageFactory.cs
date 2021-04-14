using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;
using Microsoft.Office.CompliancePolicy.PolicySync;

namespace Microsoft.Exchange.Hygiene.Data.Directory.UnifiedPolicy
{
	internal static class UnifiedPolicyStorageFactory
	{
		public static PolicyStorage ToPolicyStorage(PolicyConfiguration policy)
		{
			PolicyStorage policyStorage = new PolicyStorage();
			policyStorage[ADObjectSchema.OrganizationalUnitRoot] = new ADObjectId(policy.TenantId);
			policyStorage.Name = policy.Name;
			policyStorage.SetId((ADObjectId)DalHelper.ConvertFromStoreObject(policy.ObjectId, typeof(ADObjectId)));
			UnifiedPolicyStorageFactory.CopyPropertiesToStorage<PolicyConfiguration>(new TenantSettingFacade<PolicyStorage>(policyStorage), policy);
			return policyStorage;
		}

		public static RuleStorage ToRuleStorage(RuleConfiguration rule)
		{
			RuleStorage ruleStorage = new RuleStorage();
			ruleStorage[ADObjectSchema.OrganizationalUnitRoot] = new ADObjectId(rule.TenantId);
			ruleStorage.Name = rule.Name;
			ruleStorage.SetId((ADObjectId)DalHelper.ConvertFromStoreObject(rule.ObjectId, typeof(ADObjectId)));
			UnifiedPolicyStorageFactory.CopyPropertiesToStorage<RuleConfiguration>(new TenantSettingFacade<RuleStorage>(ruleStorage), rule);
			return ruleStorage;
		}

		public static BindingStorage ToBindingStorage(BindingConfiguration binding)
		{
			BindingStorage bindingStorage = new BindingStorage();
			bindingStorage[ADObjectSchema.OrganizationalUnitRoot] = new ADObjectId(binding.TenantId);
			bindingStorage.Name = binding.Name;
			bindingStorage.SetId((ADObjectId)DalHelper.ConvertFromStoreObject(binding.ObjectId, typeof(ADObjectId)));
			UnifiedPolicyStorageFactory.CopyPropertiesToStorage<BindingConfiguration>(new TenantSettingFacade<BindingStorage>(bindingStorage), binding);
			if (binding.AppliedScopes != null && binding.AppliedScopes.Changed)
			{
				bindingStorage.AppliedScopes = new MultiValuedProperty<ScopeStorage>(from s in binding.AppliedScopes.ChangedValues
				select UnifiedPolicyStorageFactory.ToScopeStorage(s));
			}
			return bindingStorage;
		}

		public static AssociationStorage ToAssociationStorage(AssociationConfiguration association)
		{
			AssociationStorage associationStorage = new AssociationStorage();
			associationStorage[ADObjectSchema.OrganizationalUnitRoot] = new ADObjectId(association.TenantId);
			associationStorage.Name = association.Name;
			associationStorage.SetId((ADObjectId)DalHelper.ConvertFromStoreObject(association.ObjectId, typeof(ADObjectId)));
			UnifiedPolicyStorageFactory.CopyPropertiesToStorage<AssociationConfiguration>(new TenantSettingFacade<AssociationStorage>(associationStorage), association);
			return associationStorage;
		}

		public static UnifiedPolicySettingStatus ToStatusStorage(UnifiedPolicyStatus status)
		{
			UnifiedPolicySettingStatus unifiedPolicySettingStatus = new UnifiedPolicySettingStatus();
			unifiedPolicySettingStatus[ADObjectSchema.OrganizationalUnitRoot] = new ADObjectId(status.TenantId);
			unifiedPolicySettingStatus.SetId((ADObjectId)DalHelper.ConvertFromStoreObject(status.ObjectId, typeof(ADObjectId)));
			unifiedPolicySettingStatus.SettingType = UnifiedPolicyStorageFactory.ConvertToSettingType(status.ObjectType);
			unifiedPolicySettingStatus.ParentObjectId = status.ParentObjectId;
			unifiedPolicySettingStatus.Container = status.Workload.ToString();
			unifiedPolicySettingStatus.ObjectVersion = status.Version.InternalStorage;
			unifiedPolicySettingStatus.ErrorCode = (int)status.ErrorCode;
			unifiedPolicySettingStatus.ErrorMessage = status.ErrorMessage;
			unifiedPolicySettingStatus.WhenProcessedUTC = status.WhenProcessedUTC;
			unifiedPolicySettingStatus.AdditionalDiagnostics = status.AdditionalDiagnostics;
			switch (status.Mode)
			{
			case Mode.PendingDeletion:
				unifiedPolicySettingStatus.ObjectStatus = StatusMode.PendingDeletion;
				break;
			case Mode.Deleted:
				unifiedPolicySettingStatus.ObjectStatus = StatusMode.Deleted;
				break;
			default:
				unifiedPolicySettingStatus.ObjectStatus = StatusMode.Active;
				break;
			}
			return unifiedPolicySettingStatus;
		}

		public static ScopeStorage ToScopeStorage(ScopeConfiguration scope)
		{
			ScopeStorage scopeStorage = new ScopeStorage();
			scopeStorage[ADObjectSchema.OrganizationalUnitRoot] = new ADObjectId(scope.TenantId);
			scopeStorage.Name = scope.Name;
			scopeStorage.SetId((ADObjectId)DalHelper.ConvertFromStoreObject(scope.ObjectId, typeof(ADObjectId)));
			UnifiedPolicyStorageFactory.CopyPropertiesToStorage<ScopeConfiguration>(new TenantSettingFacade<ScopeStorage>(scopeStorage), scope);
			return scopeStorage;
		}

		public static PolicyConfiguration FromPolicyStorage(PolicyStorage policyStorage)
		{
			PolicyConfiguration policyConfiguration = new PolicyConfiguration(policyStorage.OrganizationalUnitRoot.ObjectGuid, policyStorage.Id.ObjectGuid);
			UnifiedPolicyStorageFactory.CopyPropertiesFromStorage<PolicyConfiguration>(policyConfiguration, new TenantSettingFacade<PolicyStorage>(policyStorage));
			return policyConfiguration;
		}

		public static RuleConfiguration FromRuleStorage(RuleStorage ruleStorage)
		{
			RuleConfiguration ruleConfiguration = new RuleConfiguration(ruleStorage.OrganizationalUnitRoot.ObjectGuid, ruleStorage.Id.ObjectGuid);
			UnifiedPolicyStorageFactory.CopyPropertiesFromStorage<RuleConfiguration>(ruleConfiguration, new TenantSettingFacade<RuleStorage>(ruleStorage));
			return ruleConfiguration;
		}

		public static AssociationConfiguration FromAssociationStorage(AssociationStorage associationStorage)
		{
			AssociationConfiguration associationConfiguration = new AssociationConfiguration(associationStorage.OrganizationalUnitRoot.ObjectGuid, associationStorage.Id.ObjectGuid);
			UnifiedPolicyStorageFactory.CopyPropertiesFromStorage<AssociationConfiguration>(associationConfiguration, new TenantSettingFacade<AssociationStorage>(associationStorage));
			return associationConfiguration;
		}

		public static BindingConfiguration FromBindingStorage(BindingStorage bindingStorage)
		{
			BindingConfiguration bindingConfiguration = new BindingConfiguration(bindingStorage.OrganizationalUnitRoot.ObjectGuid, bindingStorage.Id.ObjectGuid);
			UnifiedPolicyStorageFactory.CopyPropertiesFromStorage<BindingConfiguration>(bindingConfiguration, new TenantSettingFacade<BindingStorage>(bindingStorage));
			if (bindingStorage.AppliedScopes.Any<ScopeStorage>() || bindingStorage.RemovedScopes.Any<ScopeStorage>())
			{
				bindingConfiguration.AppliedScopes = new IncrementalCollection<ScopeConfiguration>(from s in bindingStorage.AppliedScopes
				select UnifiedPolicyStorageFactory.FromScopeStorage(s), bindingStorage.RemovedScopes.Select((ScopeStorage s) => UnifiedPolicyStorageFactory.FromScopeStorage(s)));
			}
			else
			{
				bindingConfiguration.AppliedScopes = new IncrementalCollection<ScopeConfiguration>();
			}
			return bindingConfiguration;
		}

		public static ScopeConfiguration FromScopeStorage(ScopeStorage scopeStorage)
		{
			ScopeConfiguration scopeConfiguration = new ScopeConfiguration(scopeStorage.OrganizationalUnitRoot.ObjectGuid, scopeStorage.Id.ObjectGuid);
			UnifiedPolicyStorageFactory.CopyPropertiesFromStorage<ScopeConfiguration>(scopeConfiguration, new TenantSettingFacade<ScopeStorage>(scopeStorage));
			return scopeConfiguration;
		}

		internal static IEnumerable<PropertyInfo> GetReflectedProperties<T>() where T : PolicyConfigurationBase
		{
			return from p in typeof(T).GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
			where p.GetCustomAttribute(typeof(DataMemberAttribute), false) != null && p.GetCustomAttribute(typeof(SkipReflectionMappingAttribute), false) == null
			select p;
		}

		internal static bool PropertiesMatch(PropertyDefinition schemaProperty, PropertyInfo reflectedProperty)
		{
			return string.Equals(schemaProperty.Name, reflectedProperty.Name, StringComparison.InvariantCultureIgnoreCase);
		}

		internal static string GetRemovedCollectionPropertyName(PropertyDefinition schemaProperty)
		{
			return string.Format("_DELETED_{0}", schemaProperty.Name);
		}

		private static void CopyPropertiesFromStorage<T>(T baseConfiguration, FacadeBase storage) where T : PolicyConfigurationBase
		{
			baseConfiguration.Name = (string)storage.InnerPropertyBag[ADObjectSchema.Name];
			baseConfiguration.Workload = (Workload)storage.InnerPropertyBag[UnifiedPolicyStorageBaseSchema.WorkloadProp];
			baseConfiguration.WhenCreatedUTC = (DateTime?)storage.InnerPropertyBag[ADObjectSchema.WhenCreatedUTC];
			baseConfiguration.WhenChangedUTC = (DateTime?)storage.InnerPropertyBag[ADObjectSchema.WhenChangedUTC];
			baseConfiguration.ChangeType = ((storage.InnerConfigurable.ObjectState == ObjectState.Deleted) ? ChangeType.Delete : ChangeType.Update);
			baseConfiguration.Version = PolicyVersion.Create((Guid)storage.InnerPropertyBag[UnifiedPolicyStorageBaseSchema.PolicyVersion]);
			IEnumerable<PropertyInfo> reflectedProperties = UnifiedPolicyStorageFactory.GetReflectedProperties<T>();
			UnifiedPolicyStorageFactory.InitializeIncrementalAttributes(baseConfiguration, reflectedProperties);
			IEnumerable<PropertyDefinition> propertyDefinitions = DalHelper.GetPropertyDefinitions(storage, false);
			using (IEnumerator<PropertyDefinition> enumerator = propertyDefinitions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PropertyDefinition property = enumerator.Current;
					object propertyValue;
					if (UnifiedPolicyStorageFactory.TryReadPropertyValue(storage, property, out propertyValue))
					{
						PropertyInfo propertyInfo = reflectedProperties.FirstOrDefault((PropertyInfo p) => UnifiedPolicyStorageFactory.PropertiesMatch(property, p));
						if (!(propertyInfo == null) && !UnifiedPolicyStorageFactory.IsIncrementalCollection(propertyInfo))
						{
							UnifiedPolicyStorageFactory.CopyPropertyFromStorage(propertyInfo, propertyValue, baseConfiguration);
						}
					}
				}
			}
			using (IEnumerator<PropertyInfo> enumerator2 = (from p in reflectedProperties
			where UnifiedPolicyStorageFactory.IsIncrementalCollection(p)
			select p).GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					UnifiedPolicyStorageFactory.<>c__DisplayClassf<T> CS$<>8__locals2 = new UnifiedPolicyStorageFactory.<>c__DisplayClassf<T>();
					CS$<>8__locals2.incrementalCollectionProp = enumerator2.Current;
					PropertyDefinition addedProperty = propertyDefinitions.FirstOrDefault((PropertyDefinition p) => UnifiedPolicyStorageFactory.PropertiesMatch(p, CS$<>8__locals2.incrementalCollectionProp));
					PropertyDefinition propertyDefinition = propertyDefinitions.FirstOrDefault((PropertyDefinition p) => p.Name == UnifiedPolicyStorageFactory.GetRemovedCollectionPropertyName(addedProperty));
					if (addedProperty != null && propertyDefinition != null)
					{
						UnifiedPolicyStorageFactory.CopyIncrementalCollection(CS$<>8__locals2.incrementalCollectionProp, addedProperty, propertyDefinition, storage, baseConfiguration);
					}
				}
			}
		}

		private static void CopyPropertyFromStorage(PropertyInfo property, object propertyValue, PolicyConfigurationBase baseConfiguration)
		{
			if (UnifiedPolicyStorageFactory.IsIncrementalAttribute(property))
			{
				IncrementalAttributeBase incrementalAttribute = UnifiedPolicyStorageFactory.GetIncrementalAttribute(property, true, propertyValue);
				property.GetSetMethod().Invoke(baseConfiguration, new IncrementalAttributeBase[]
				{
					incrementalAttribute
				});
				return;
			}
			property.GetSetMethod().Invoke(baseConfiguration, new object[]
			{
				propertyValue
			});
		}

		private static void CopyIncrementalCollection(PropertyInfo incrementalProperty, PropertyDefinition addedProperty, PropertyDefinition removedProperty, FacadeBase storage, PolicyConfigurationBase baseConfiguration)
		{
			object obj = null;
			object obj2 = null;
			UnifiedPolicyStorageFactory.TryReadPropertyValue(storage, addedProperty, out obj);
			UnifiedPolicyStorageFactory.TryReadPropertyValue(storage, removedProperty, out obj2);
			if (obj != null || obj2 != null)
			{
				IncrementalAttributeBase incrementalCollection = UnifiedPolicyStorageFactory.GetIncrementalCollection(incrementalProperty, true, (MultiValuedPropertyBase)obj, (MultiValuedPropertyBase)obj2);
				incrementalProperty.GetSetMethod().Invoke(baseConfiguration, new IncrementalAttributeBase[]
				{
					incrementalCollection
				});
			}
		}

		private static void CopyPropertiesToStorage<T>(FacadeBase storage, T baseConfiguration) where T : PolicyConfigurationBase
		{
			PropertyBag propertyBag = (storage.InnerPropertyBag as UnifiedPolicyStorageBase).propertyBag;
			propertyBag.SetField(UnifiedPolicyStorageBaseSchema.WorkloadProp, baseConfiguration.Workload);
			propertyBag.SetField(ADObjectSchema.WhenCreatedRaw, (baseConfiguration.WhenCreatedUTC != null) ? baseConfiguration.WhenCreatedUTC.Value.ToString("yyyyMMddHHmmss'.0Z'") : null);
			propertyBag.SetField(ADObjectSchema.WhenChangedRaw, (baseConfiguration.WhenChangedUTC != null) ? baseConfiguration.WhenChangedUTC.Value.ToString("yyyyMMddHHmmss'.0Z'") : null);
			propertyBag.SetField(UnifiedPolicyStorageBaseSchema.PolicyVersion, baseConfiguration.Version.InternalStorage);
			IEnumerable<PropertyDefinition> propertyDefinitions = DalHelper.GetPropertyDefinitions(storage, false);
			using (IEnumerator<PropertyInfo> enumerator = UnifiedPolicyStorageFactory.GetReflectedProperties<T>().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PropertyInfo prop = enumerator.Current;
					PropertyDefinition propertyDefinition = propertyDefinitions.FirstOrDefault((PropertyDefinition p) => UnifiedPolicyStorageFactory.PropertiesMatch(p, prop));
					if (propertyDefinition != null)
					{
						UnifiedPolicyStorageFactory.CopyPropertyToStorage(propertyDefinition, prop, storage.InnerPropertyBag, baseConfiguration);
					}
				}
			}
		}

		private static void CopyPropertyToStorage(PropertyDefinition schemaProperty, PropertyInfo property, IPropertyBag storage, PolicyConfigurationBase baseConfiguration)
		{
			object obj = property.GetGetMethod().Invoke(baseConfiguration, null);
			if (UnifiedPolicyStorageFactory.IsIncrementalAttribute(property) || UnifiedPolicyStorageFactory.IsIncrementalCollection(property))
			{
				IncrementalAttributeBase incrementalAttributeBase = (IncrementalAttributeBase)obj;
				if (incrementalAttributeBase != null && incrementalAttributeBase.Changed)
				{
					UnifiedPolicyStorageFactory.StoreValue(storage, schemaProperty, incrementalAttributeBase.GetObjectValue());
					return;
				}
			}
			else
			{
				UnifiedPolicyStorageFactory.StoreValue(storage, schemaProperty, obj);
			}
		}

		private static void StoreValue(IPropertyBag storage, PropertyDefinition property, object value)
		{
			if (property is ProviderPropertyDefinition && ((ProviderPropertyDefinition)property).IsMultivalued)
			{
				storage[property] = Activator.CreateInstance(typeof(MultiValuedProperty<>).MakeGenericType(new Type[]
				{
					property.Type
				}), new object[]
				{
					value
				});
				return;
			}
			storage[property] = value;
		}

		private static void InitializeIncrementalAttributes(PolicyConfigurationBase baseConfiguration, IEnumerable<PropertyInfo> allProperties)
		{
			foreach (PropertyInfo propertyInfo in from p in allProperties
			where UnifiedPolicyStorageFactory.IsIncrementalAttribute(p)
			select p)
			{
				IncrementalAttributeBase incrementalAttribute = UnifiedPolicyStorageFactory.GetIncrementalAttribute(propertyInfo, false, null);
				propertyInfo.GetSetMethod().Invoke(baseConfiguration, new IncrementalAttributeBase[]
				{
					incrementalAttribute
				});
			}
			foreach (PropertyInfo propertyInfo2 in from p in allProperties
			where UnifiedPolicyStorageFactory.IsIncrementalCollection(p)
			select p)
			{
				IncrementalAttributeBase incrementalCollection = UnifiedPolicyStorageFactory.GetIncrementalCollection(propertyInfo2, false, null, null);
				propertyInfo2.GetSetMethod().Invoke(baseConfiguration, new IncrementalAttributeBase[]
				{
					incrementalCollection
				});
			}
		}

		private static bool IsIncrementalAttribute(PropertyInfo property)
		{
			return property.PropertyType.Name == typeof(IncrementalAttribute<>).Name;
		}

		private static bool IsIncrementalCollection(PropertyInfo property)
		{
			return property.PropertyType.Name == typeof(IncrementalCollection<>).Name;
		}

		private static bool TryReadPropertyValue(FacadeBase facade, PropertyDefinition property, out object propertyValue)
		{
			propertyValue = null;
			return (facade.InnerConfigurable as UnifiedPolicyStorageBase).propertyBag.TryGetField((ProviderPropertyDefinition)property, ref propertyValue);
		}

		private static IncrementalAttributeBase GetIncrementalAttribute(PropertyInfo property, bool isChanged, object propertyValue)
		{
			if (isChanged)
			{
				return (IncrementalAttributeBase)Activator.CreateInstance(typeof(IncrementalAttribute<>).MakeGenericType(new Type[]
				{
					property.PropertyType.GenericTypeArguments[0]
				}), new object[]
				{
					propertyValue
				});
			}
			return (IncrementalAttributeBase)Activator.CreateInstance(typeof(IncrementalAttribute<>).MakeGenericType(new Type[]
			{
				property.PropertyType.GenericTypeArguments[0]
			}));
		}

		private static IncrementalAttributeBase GetIncrementalCollection(PropertyInfo property, bool isChanged, MultiValuedPropertyBase addedValues, MultiValuedPropertyBase removedValues)
		{
			IncrementalAttributeBase incrementalAttributeBase = (IncrementalAttributeBase)Activator.CreateInstance(typeof(IncrementalCollection<>).MakeGenericType(new Type[]
			{
				property.PropertyType.GenericTypeArguments[0]
			}));
			if (isChanged)
			{
				IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(new Type[]
				{
					property.PropertyType.GenericTypeArguments[0]
				}));
				IList list2 = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(new Type[]
				{
					property.PropertyType.GenericTypeArguments[0]
				}));
				if (addedValues != null)
				{
					foreach (object value in ((IEnumerable)addedValues))
					{
						list.Add(value);
					}
				}
				if (removedValues != null)
				{
					foreach (object value2 in ((IEnumerable)removedValues))
					{
						list2.Add(value2);
					}
				}
				return (IncrementalAttributeBase)Activator.CreateInstance(typeof(IncrementalCollection<>).MakeGenericType(new Type[]
				{
					property.PropertyType.GenericTypeArguments[0]
				}), new IList[]
				{
					list,
					list2
				});
			}
			return (IncrementalAttributeBase)Activator.CreateInstance(typeof(IncrementalCollection<>).MakeGenericType(new Type[]
			{
				property.PropertyType.GenericTypeArguments[0]
			}));
		}

		private static string ConvertToSettingType(ConfigurationObjectType objectType)
		{
			switch (objectType)
			{
			case ConfigurationObjectType.Policy:
				return typeof(PolicyStorage).Name;
			case ConfigurationObjectType.Rule:
				return typeof(RuleStorage).Name;
			case ConfigurationObjectType.Association:
				return typeof(AssociationStorage).Name;
			case ConfigurationObjectType.Binding:
				return typeof(BindingStorage).Name;
			case ConfigurationObjectType.Scope:
				return typeof(ScopeStorage).Name;
			default:
				throw new NotSupportedException(string.Format("Object type {0} not supported", objectType));
			}
		}
	}
}
