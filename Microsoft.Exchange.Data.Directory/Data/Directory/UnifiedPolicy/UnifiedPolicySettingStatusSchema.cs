using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.UnifiedPolicy
{
	internal class UnifiedPolicySettingStatusSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition SettingType = new ADPropertyDefinition("SettingType", ExchangeObjectVersion.Exchange2012, typeof(string), "msExchUnifiedPolicySettingType", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.NonADProperty, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ObjectId = new ADPropertyDefinition("ObjectId", ExchangeObjectVersion.Exchange2012, typeof(Guid), "msExchEdgeSyncSourceGuid", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.NonADProperty, System.Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ParentObjectId = new ADPropertyDefinition("ParentObjectId", ExchangeObjectVersion.Exchange2012, typeof(Guid?), "msExchParentObjectId", ADPropertyDefinitionFlags.NonADProperty, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Container = new ADPropertyDefinition("Container", ExchangeObjectVersion.Exchange2012, typeof(string), "msExchStatusContainer", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.NonADProperty, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ErrorCode = new ADPropertyDefinition("ErrorCode", ExchangeObjectVersion.Exchange2012, typeof(int), "msExchangeUnifiedPolicyErrorCode", ADPropertyDefinitionFlags.PersistDefaultValue | ADPropertyDefinitionFlags.NonADProperty, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ErrorMessage = new ADPropertyDefinition("ErrorMessage", ExchangeObjectVersion.Exchange2012, typeof(string), "msExchangeUnifiedPolicyErrorMessage", ADPropertyDefinitionFlags.NonADProperty, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ObjectVersion = new ADPropertyDefinition("ObjectVersion", ExchangeObjectVersion.Exchange2012, typeof(Guid), "msExchangeUnifiedPolicyObjectVersion", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.NonADProperty, System.Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition WhenProcessedUTC = new ADPropertyDefinition("WhenProcessedUTC", ExchangeObjectVersion.Exchange2012, typeof(DateTime), "msExchUnifiedPolicyStatusWhenProcessed", ADPropertyDefinitionFlags.PersistDefaultValue | ADPropertyDefinitionFlags.NonADProperty, DateTime.MinValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ObjectStatus = new ADPropertyDefinition("ObjectStatus", ExchangeObjectVersion.Exchange2012, typeof(StatusMode), "msExchangeUnifiedPolicyObjectStatus", ADPropertyDefinitionFlags.PersistDefaultValue | ADPropertyDefinitionFlags.NonADProperty, StatusMode.Active, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AdditionalDiagnostics = new ADPropertyDefinition("AdditionalDiagnostics", ExchangeObjectVersion.Exchange2012, typeof(string), "msExchStatusAdditionalDiagnostics", ADPropertyDefinitionFlags.NonADProperty, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
