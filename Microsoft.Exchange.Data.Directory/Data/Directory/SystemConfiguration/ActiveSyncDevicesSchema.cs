using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ActiveSyncDevicesSchema : ADContainerSchema
	{
		public static readonly ADPropertyDefinition DeletionPeriod = new ADPropertyDefinition("DeletionPeriod", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), "msExchDeletionPeriod", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ObjectsDeletedThisPeriod = new ADPropertyDefinition("ObjectsDeletedThisPeriod", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchObjectsDeletedThisPeriod", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
