using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class AccountPartition : ADConfigurationObject
	{
		internal static QueryFilter IsLocalForestFilterBuilder(SinglePropertyFilter filter)
		{
			return new BitMaskAndFilter(AccountPartitionSchema.ProvisioningFlags, 1UL);
		}

		internal static QueryFilter IsSecondaryFilterBuilder(SinglePropertyFilter filter)
		{
			return new BitMaskAndFilter(AccountPartitionSchema.ProvisioningFlags, 4UL);
		}

		internal static object PartitionIdGetter(IPropertyBag propertyBag)
		{
			bool flag = (bool)propertyBag[AccountPartitionSchema.IsLocalForest];
			if (flag)
			{
				return new PartitionId(PartitionId.LocalForest.ForestFQDN, ADObjectId.ResourcePartitionGuid);
			}
			ADObjectId adobjectId = (ADObjectId)propertyBag[AccountPartitionSchema.TrustedDomainLink];
			if (adobjectId == null)
			{
				return null;
			}
			string name = adobjectId.Name;
			if ((ObjectState)propertyBag[ADObjectSchema.ObjectState] == ObjectState.New)
			{
				return new PartitionId(name);
			}
			return new PartitionId(name, ((ADObjectId)propertyBag[ADObjectSchema.Id]).ObjectGuid);
		}

		public new string Name
		{
			get
			{
				return base.Name;
			}
			internal set
			{
				base.Name = value;
			}
		}

		public ADObjectId TrustedDomain
		{
			get
			{
				return (ADObjectId)this[AccountPartitionSchema.TrustedDomainLink];
			}
			internal set
			{
				this[AccountPartitionSchema.TrustedDomainLink] = value;
			}
		}

		internal PartitionId PartitionId
		{
			get
			{
				return (PartitionId)this[AccountPartitionSchema.PartitionId];
			}
		}

		public bool IsLocalForest
		{
			get
			{
				return (bool)this[AccountPartitionSchema.IsLocalForest];
			}
			internal set
			{
				this[AccountPartitionSchema.IsLocalForest] = value;
			}
		}

		public bool EnabledForProvisioning
		{
			get
			{
				return (bool)this[AccountPartitionSchema.EnabledForProvisioning];
			}
			internal set
			{
				this[AccountPartitionSchema.EnabledForProvisioning] = value;
			}
		}

		public bool IsSecondary
		{
			get
			{
				return (bool)this[AccountPartitionSchema.IsSecondary];
			}
			internal set
			{
				this[AccountPartitionSchema.IsSecondary] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ObjectSchema.GetInstance<AccountPartitionSchema>();
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return AccountPartition.MostDerivedClass;
			}
		}

		internal bool TryGetPartitionId(out PartitionId partitionId)
		{
			partitionId = this.PartitionId;
			return this.TrustedDomain != null || this.IsLocalForest;
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			base.ValidateRead(errors);
			if (this.IsLocalForest && this.TrustedDomain != null)
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorAccountPartitionCantBeLocalAndHaveTrustAtTheSameTime(this.Identity.ToString()), this.Identity, string.Empty));
			}
			if (this.IsLocalForest && this.IsSecondary)
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorAccountPartitionCantBeLocalAndSecondaryAtTheSameTime(this.Identity.ToString()), this.Identity, string.Empty));
			}
			if (!this.IsLocalForest && this.TrustedDomain == null)
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorRemoteAccountPartitionMustHaveTrust(this.Identity.ToString()), this.Identity, string.Empty));
			}
			if (this.IsSecondary && this.EnabledForProvisioning)
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorSecondaryAccountPartitionCantBeUsedForProvisioning(this.Identity.ToString()), this.Identity, string.Empty));
			}
		}

		public static readonly string AccountForestContainerName = "Account Forests";

		public static readonly string ResourceForestContainerName = "Resource Forest";

		internal static string MostDerivedClass = "msExchAccountForest";
	}
}
