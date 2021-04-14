using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Exchange.Data.Directory.UnifiedPolicy
{
	internal class UnifiedPolicyStorageBaseSchema : ADConfigurationObjectSchema
	{
		public static ADPropertyDefinition WorkloadProp = new ADPropertyDefinition("Workload", ExchangeObjectVersion.Exchange2012, typeof(Workload), "msExchOWASettings", ADPropertyDefinitionFlags.None, Workload.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static ADPropertyDefinition PolicyVersion = new ADPropertyDefinition("PolicyVersion", ExchangeObjectVersion.Exchange2012, typeof(Guid), "msExchCanaryData0", ADPropertyDefinitionFlags.Binary, System.Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static ADPropertyDefinition MasterIdentity = new ADPropertyDefinition("MasterIdentity", ExchangeObjectVersion.Exchange2012, typeof(Guid), "msExchEdgeSyncSourceGuid", ADPropertyDefinitionFlags.WriteOnce, System.Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static ADPropertyDefinition ContainerProp = new ADPropertyDefinition("Container", ExchangeObjectVersion.Exchange2012, typeof(string), "msExchEwsExceptions", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
