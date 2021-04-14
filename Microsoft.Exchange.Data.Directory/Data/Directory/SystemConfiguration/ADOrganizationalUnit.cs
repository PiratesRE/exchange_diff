using System;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class ADOrganizationalUnit : ADConfigurationObject, IProvisioningCacheInvalidation
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADOrganizationalUnit.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADOrganizationalUnit.MostDerivedClass;
			}
		}

		public new OrganizationId OrganizationId
		{
			get
			{
				return (OrganizationId)this[ADOrganizationalUnitSchema.OrganizationId];
			}
		}

		internal ADObjectId ConfigurationUnitLink
		{
			get
			{
				return ADOrganizationalUnit.ConfigurationUnitLinkGetter(this.propertyBag);
			}
		}

		public bool IsOrganizationalUnitRoot
		{
			get
			{
				return this.propertyBag[ADOrganizationalUnitSchema.ConfigurationUnitLink] != null;
			}
		}

		public bool MSOSyncEnabled
		{
			get
			{
				return (bool)this[ADOrganizationalUnitSchema.MSOSyncEnabled];
			}
			internal set
			{
				this[ADOrganizationalUnitSchema.MSOSyncEnabled] = value;
			}
		}

		public bool SMTPAddressCheckWithAcceptedDomain
		{
			get
			{
				return (bool)this[ADOrganizationalUnitSchema.SMTPAddressCheckWithAcceptedDomain];
			}
			internal set
			{
				this[ADOrganizationalUnitSchema.SMTPAddressCheckWithAcceptedDomain] = value;
			}
		}

		public bool SyncMEUSMTPToMServ
		{
			get
			{
				return !(bool)this[ADOrganizationalUnitSchema.MSOSyncEnabled] || (bool)this[ADOrganizationalUnitSchema.SMTPAddressCheckWithAcceptedDomain];
			}
		}

		public bool SyncMBXAndDLToMServ
		{
			get
			{
				return (bool)this[ADOrganizationalUnitSchema.SyncMBXAndDLToMserv] || !(bool)this[ADOrganizationalUnitSchema.MSOSyncEnabled];
			}
			internal set
			{
				this[ADOrganizationalUnitSchema.SyncMBXAndDLToMserv] = value;
			}
		}

		internal bool RelocationInProgress
		{
			get
			{
				return (bool)this[ADOrganizationalUnitSchema.RelocationInProgress];
			}
			set
			{
				this[ADOrganizationalUnitSchema.RelocationInProgress] = value;
			}
		}

		internal new ADObjectId OrganizationalUnitRoot
		{
			get
			{
				return ((OrganizationId)this[ADOrganizationalUnitSchema.OrganizationId]).OrganizationalUnit;
			}
		}

		internal new ADObjectId ConfigurationUnit
		{
			get
			{
				return ((OrganizationId)this[ADOrganizationalUnitSchema.OrganizationId]).ConfigurationUnit;
			}
			set
			{
				this[ADObjectSchema.ConfigurationUnit] = value;
				if (value == null)
				{
					this[ADObjectSchema.OrganizationalUnitRoot] = null;
					return;
				}
				this[ADObjectSchema.OrganizationalUnitRoot] = this[ADObjectSchema.Id];
			}
		}

		internal static object OuOrganizationIdGetter(IPropertyBag propertyBag)
		{
			OrganizationId organizationId = (OrganizationId)ADObject.OrganizationIdGetter(propertyBag);
			if (organizationId.Equals(OrganizationId.ForestWideOrgId))
			{
				ADObjectId adobjectId = ADOrganizationalUnit.ConfigurationUnitLinkGetter(propertyBag);
				if (adobjectId != null)
				{
					organizationId = new OrganizationId((ADObjectId)propertyBag[ADObjectSchema.Id], adobjectId);
				}
			}
			return organizationId;
		}

		private static ADObjectId ConfigurationUnitLinkGetter(IPropertyBag propertyBag)
		{
			ADObjectId result = null;
			if (propertyBag[ADOrganizationalUnitSchema.ConfigurationUnitLink] != null)
			{
				MultiValuedProperty<ADObjectId> multiValuedProperty = (MultiValuedProperty<ADObjectId>)propertyBag[ADOrganizationalUnitSchema.ConfigurationUnitLink];
				if (multiValuedProperty.Count > 0)
				{
					result = multiValuedProperty[0];
				}
			}
			return result;
		}

		public MultiValuedProperty<string> UPNSuffixes
		{
			get
			{
				return (MultiValuedProperty<string>)this.propertyBag[ADOrganizationalUnitSchema.UPNSuffixes];
			}
		}

		internal bool ShouldInvalidProvisioningCache(out OrganizationId orgId, out Guid[] keys)
		{
			orgId = null;
			keys = null;
			if (base.ObjectState == ObjectState.Deleted)
			{
				keys = new Guid[1];
				keys[0] = CannedProvisioningCacheKeys.OrganizationIdDictionary;
				return true;
			}
			return false;
		}

		bool IProvisioningCacheInvalidation.ShouldInvalidProvisioningCache(out OrganizationId orgId, out Guid[] keys)
		{
			return this.ShouldInvalidProvisioningCache(out orgId, out keys);
		}

		private static ADOrganizationalUnitSchema schema = ObjectSchema.GetInstance<ADOrganizationalUnitSchema>();

		internal static string MostDerivedClass = "organizationalUnit";
	}
}
