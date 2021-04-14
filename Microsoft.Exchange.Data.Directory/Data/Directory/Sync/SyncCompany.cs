using System;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal class SyncCompany : SyncObject
	{
		public SyncCompany(SyncDirection syncDirection) : base(syncDirection)
		{
		}

		public override SyncObjectSchema Schema
		{
			get
			{
				return SyncCompany.schema;
			}
		}

		public override bool IsValid(bool isFullSyncObject)
		{
			return !isFullSyncObject || this.InitialDomainNameRetrieved != null;
		}

		internal override DirectoryObjectClass ObjectClass
		{
			get
			{
				return DirectoryObjectClass.Company;
			}
		}

		protected override DirectoryObject CreateDirectoryObject()
		{
			return new Company();
		}

		public SyncProperty<RightsManagementTenantConfigurationValue> RightsManagementTenantConfiguration
		{
			get
			{
				return (SyncProperty<RightsManagementTenantConfigurationValue>)base[SyncCompanySchema.RightsManagementTenantConfiguration];
			}
			set
			{
				base[SyncCompanySchema.RightsManagementTenantConfiguration] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<RightsManagementTenantKeyValue>> RightsManagementTenantKey
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<RightsManagementTenantKeyValue>>)base[SyncCompanySchema.RightsManagementTenantKey];
			}
			set
			{
				base[SyncCompanySchema.RightsManagementTenantKey] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<ServiceInfoValue>> ServiceInfo
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<ServiceInfoValue>>)base[SyncCompanySchema.ServiceInfo];
			}
			set
			{
				base[SyncCompanySchema.ServiceInfo] = value;
			}
		}

		public SyncProperty<string> CompanyPartnership
		{
			get
			{
				return (SyncProperty<string>)base[SyncCompanySchema.CompanyPartnership];
			}
			set
			{
				base[SyncCompanySchema.CompanyPartnership] = value;
			}
		}

		public SyncProperty<string> Description
		{
			get
			{
				return (SyncProperty<string>)base[SyncCompanySchema.Description];
			}
			set
			{
				base[SyncCompanySchema.Description] = value;
			}
		}

		public SyncProperty<string> DisplayName
		{
			get
			{
				return (SyncProperty<string>)base[SyncCompanySchema.DisplayName];
			}
			set
			{
				base[SyncCompanySchema.DisplayName] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<CompanyVerifiedDomainValue>> VerifiedDomain
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<CompanyVerifiedDomainValue>>)base[SyncCompanySchema.VerifiedDomain];
			}
			set
			{
				base[SyncCompanySchema.VerifiedDomain] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<AssignedPlanValue>> AssignedPlan
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<AssignedPlanValue>>)base[SyncCompanySchema.AssignedPlan];
			}
			set
			{
				base[SyncCompanySchema.AssignedPlan] = value;
			}
		}

		public SyncProperty<string> C
		{
			get
			{
				return (SyncProperty<string>)base[SyncCompanySchema.C];
			}
			set
			{
				base[SyncCompanySchema.C] = value;
			}
		}

		public SyncProperty<bool> IsDirSyncRunning
		{
			get
			{
				return (SyncProperty<bool>)base[SyncCompanySchema.IsDirSyncRunning];
			}
			set
			{
				base[SyncCompanySchema.IsDirSyncRunning] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<string>> DirSyncStatus
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<string>>)base[SyncCompanySchema.DirSyncStatus];
			}
			set
			{
				base[SyncCompanySchema.DirSyncStatus] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<string>> DirSyncStatusAck
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<string>>)base[SyncCompanySchema.DirSyncStatusAck];
			}
			set
			{
				base[SyncCompanySchema.DirSyncStatusAck] = value;
			}
		}

		public SyncProperty<int?> TenantType
		{
			get
			{
				return (SyncProperty<int?>)base[SyncCompanySchema.TenantType];
			}
			set
			{
				base[SyncCompanySchema.TenantType] = value;
			}
		}

		public SyncProperty<int> QuotaAmount
		{
			get
			{
				return (SyncProperty<int>)base[SyncCompanySchema.QuotaAmount];
			}
			set
			{
				base[SyncCompanySchema.QuotaAmount] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<string>> CompanyTags
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<string>>)base[SyncCompanySchema.CompanyTags];
			}
			set
			{
				base[SyncCompanySchema.CompanyTags] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<Capability>> PersistedCapabilities
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<Capability>>)base[SyncCompanySchema.PersistedCapabilities];
			}
		}

		public string InitialDomainName
		{
			get
			{
				string result;
				if ((result = this.InitialDomainNameRetrieved) == null)
				{
					if (base.Id == null)
					{
						return base.ContextId;
					}
					result = base.Id.ToString();
				}
				return result;
			}
		}

		public bool IsInitialDomainOutBoundOnly
		{
			get
			{
				if (!this.VerifiedDomain.HasValue || this.VerifiedDomain.Value == null)
				{
					return false;
				}
				foreach (CompanyVerifiedDomainValue companyVerifiedDomainValue in this.VerifiedDomain.Value)
				{
					if (companyVerifiedDomainValue.Initial && !string.IsNullOrEmpty(companyVerifiedDomainValue.Name))
					{
						return (companyVerifiedDomainValue.Capabilities & SyncCompany.OutBoundOnlyFlag) != 0;
					}
				}
				return false;
			}
		}

		internal static object PersistedCapabilityGetter(IPropertyBag propertyBag)
		{
			return SyncCompany.GetEffectivePersistedCapabilities(propertyBag);
		}

		private static MultiValuedProperty<Capability> GetEffectivePersistedCapabilities(IPropertyBag propertyBag)
		{
			MultiValuedProperty<AssignedPlanValue> multiValuedProperty = (MultiValuedProperty<AssignedPlanValue>)propertyBag[SyncCompanySchema.AssignedPlan];
			MultiValuedProperty<Capability> multiValuedProperty2 = new MultiValuedProperty<Capability>();
			if (multiValuedProperty != null && multiValuedProperty.Count != 0)
			{
				foreach (AssignedPlanValue assignedPlanValue in multiValuedProperty)
				{
					Capability exchangeCapability = SyncUser.GetExchangeCapability(assignedPlanValue.Capability);
					if (assignedPlanValue.CapabilityStatus != AssignedCapabilityStatus.Deleted && exchangeCapability != Capability.None && !multiValuedProperty2.Contains(exchangeCapability))
					{
						multiValuedProperty2.Add(exchangeCapability);
					}
				}
			}
			return multiValuedProperty2;
		}

		private string InitialDomainNameRetrieved
		{
			get
			{
				if (!this.VerifiedDomain.HasValue || this.VerifiedDomain.Value == null)
				{
					return null;
				}
				foreach (CompanyVerifiedDomainValue companyVerifiedDomainValue in this.VerifiedDomain.Value)
				{
					if (companyVerifiedDomainValue.Initial && !string.IsNullOrEmpty(companyVerifiedDomainValue.Name))
					{
						return companyVerifiedDomainValue.Name;
					}
				}
				return null;
			}
		}

		private static readonly SyncCompanySchema schema = ObjectSchema.GetInstance<SyncCompanySchema>();

		public static int OutBoundOnlyFlag = 4;
	}
}
