using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Exchange.Data.Storage.UnifiedPolicy
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class PolicyConfigConverterBase
	{
		protected PolicyConfigConverterBase(Type policyConfigType, ConfigurationObjectType configurationObjectType, Type storageType, ADPropertyDefinition policyIdProperty)
		{
			ArgumentValidator.ThrowIfNull("policyConfigType", policyConfigType);
			ArgumentValidator.ThrowIfNull("storageType", storageType);
			ArgumentValidator.ThrowIfNull("policyIdProperty", policyIdProperty);
			this.PolicyConfigType = policyConfigType;
			this.ConfigurationObjectType = configurationObjectType;
			this.StorageType = storageType;
			this.PolicyIdProperty = policyIdProperty;
		}

		public Type PolicyConfigType { get; private set; }

		public Type StorageType { get; private set; }

		public ConfigurationObjectType ConfigurationObjectType { get; private set; }

		public ADPropertyDefinition PolicyIdProperty { get; private set; }

		public abstract Func<QueryFilter, ObjectId, bool, SortBy, IConfigurable[]> GetFindStorageObjectsDelegate(ExPolicyConfigProvider provider);

		public abstract PolicyConfigBase ConvertFromStorage(ExPolicyConfigProvider provider, UnifiedPolicyStorageBase storageObject);

		public abstract UnifiedPolicyStorageBase ConvertToStorage(ExPolicyConfigProvider provider, PolicyConfigBase policyConfig);
	}
}
