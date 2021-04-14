using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Provisioning
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class ADProvisioningPolicy : ADConfigurationObject, IProvisioningCacheInvalidation
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADProvisioningPolicy.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADProvisioningPolicy.MostDerivedClass;
			}
		}

		internal virtual ICollection<Type> SupportedPresentationObjectTypes
		{
			get
			{
				return null;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				if (base.GetType() == typeof(ADProvisioningPolicy))
				{
					return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, ADProvisioningPolicy.MostDerivedClass);
				}
				return base.ImplicitFilter;
			}
		}

		internal virtual IEnumerable<IProvisioningTemplate> ProvisioningTemplateRules
		{
			get
			{
				return null;
			}
		}

		internal virtual IEnumerable<IProvisioningEnforcement> ProvisioningEnforcementRules
		{
			get
			{
				return null;
			}
		}

		internal virtual void ProvisionCustomDefaultProperties(IConfigurable provisionedDefault)
		{
			if (provisionedDefault == null)
			{
				throw new ArgumentNullException("provisionedDefault");
			}
			if (this.SupportedPresentationObjectTypes == null || !this.SupportedPresentationObjectTypes.Contains(provisionedDefault.GetType()))
			{
				throw new InvalidOperationException(DirectoryStrings.ErrorPolicyDontSupportedPresentationObject(provisionedDefault.GetType(), base.GetType()));
			}
		}

		internal virtual ProvisioningValidationError[] ProvisioningCustomValidate(IConfigurable roPresentationObject)
		{
			if (roPresentationObject == null)
			{
				throw new ArgumentNullException("roPresentationObject");
			}
			if (this.SupportedPresentationObjectTypes == null || !this.SupportedPresentationObjectTypes.Contains(roPresentationObject.GetType()))
			{
				throw new InvalidOperationException(DirectoryStrings.ErrorPolicyDontSupportedPresentationObject(roPresentationObject.GetType(), base.GetType()));
			}
			return null;
		}

		public ProvisioningPolicyType PolicyType
		{
			get
			{
				return (ProvisioningPolicyType)this[ADProvisioningPolicySchema.PolicyType];
			}
		}

		public MultiValuedProperty<string> TargetObjects
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADProvisioningPolicySchema.TargetObjects];
			}
			private set
			{
				this[ADProvisioningPolicySchema.TargetObjects] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> Scopes
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADProvisioningPolicySchema.Scopes];
			}
			internal set
			{
				this[ADProvisioningPolicySchema.Scopes] = value;
			}
		}

		public new string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		internal override void StampPersistableDefaultValues()
		{
			if (this.SupportedPresentationObjectTypes != null && !base.IsModified(ADProvisioningPolicySchema.TargetObjects) && this.TargetObjects.Count == 0)
			{
				foreach (Type poType in this.SupportedPresentationObjectTypes)
				{
					this.TargetObjects.Add(ProvisioningHelper.GetProvisioningObjectTag(poType));
				}
			}
			base.StampPersistableDefaultValues();
		}

		internal bool ShouldInvalidProvisioningCache(out OrganizationId orgId, out Guid[] keys)
		{
			orgId = null;
			keys = null;
			if (base.OrganizationId == null || base.OrganizationId.Equals(OrganizationId.ForestWideOrgId) || base.ObjectState == ObjectState.Unchanged)
			{
				return false;
			}
			orgId = base.OrganizationId;
			keys = new Guid[1];
			keys[0] = CannedProvisioningCacheKeys.EnforcementProvisioningPolicies;
			return true;
		}

		bool IProvisioningCacheInvalidation.ShouldInvalidProvisioningCache(out OrganizationId orgId, out Guid[] keys)
		{
			return this.ShouldInvalidProvisioningCache(out orgId, out keys);
		}

		private static ADProvisioningPolicySchema schema = ObjectSchema.GetInstance<ADProvisioningPolicySchema>();

		internal static readonly ADObjectId RdnContainer = new ADObjectId("CN=Provisioning Policy Container");

		internal static readonly string MostDerivedClass = "msExchProvisioningPolicy";
	}
}
