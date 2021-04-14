using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Sync
{
	internal class BaseCookieSchema : ADObjectSchema
	{
		public static readonly HygienePropertyDefinition IdentityProp = new HygienePropertyDefinition("Identity", typeof(ADObjectId));

		public static readonly HygienePropertyDefinition DataProp = new HygienePropertyDefinition("Data", typeof(byte[]));

		public static readonly HygienePropertyDefinition ServiceInstanceProp = CommonSyncProperties.ServiceInstanceProp;

		public static readonly HygienePropertyDefinition VersionProp = new HygienePropertyDefinition("Version", typeof(string));

		public static readonly HygienePropertyDefinition ActiveMachineProperty = new HygienePropertyDefinition("ActiveMachine", typeof(string));

		public static readonly HygienePropertyDefinition CallerProp = new HygienePropertyDefinition("Caller", typeof(Guid));

		public static readonly HygienePropertyDefinition BatchIdProp = new HygienePropertyDefinition("BatchId", typeof(Guid));

		public static readonly HygienePropertyDefinition AllowNullCookieProp = new HygienePropertyDefinition("AllowNullCookie", typeof(bool));

		public static readonly HygienePropertyDefinition AcquireCookieLockProp = new HygienePropertyDefinition("AcquireCookieLock", typeof(bool));

		public static readonly HygienePropertyDefinition LastChangedProp = new HygienePropertyDefinition("LastChanged", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition ProvisioningFlagsProperty = CommonSyncProperties.ProvisioningFlagsProperty;

		public static readonly HygienePropertyDefinition CompleteProp = new HygienePropertyDefinition("Complete", typeof(bool), false, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition LastUpdatedCutoffThresholdQueryProp = new HygienePropertyDefinition("LastUpdatedCutoffThreshold", typeof(DateTime?));

		public static readonly HygienePropertyDefinition LastCompletedCutoffThresholdQueryProp = new HygienePropertyDefinition("LastCompletedCutoffThreshold", typeof(DateTime?));
	}
}
