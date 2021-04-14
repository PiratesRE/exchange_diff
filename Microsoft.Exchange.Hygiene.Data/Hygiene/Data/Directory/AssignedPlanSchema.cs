using System;
using System.Data.SqlTypes;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Hygiene.Data.Sync;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class AssignedPlanSchema : CommonSyncProperties
	{
		public static readonly HygienePropertyDefinition SubscribedPlanIdProp = new HygienePropertyDefinition("subscribedPlanId", typeof(Guid));

		public static readonly HygienePropertyDefinition CapabilityProp = new HygienePropertyDefinition("capability", typeof(string));

		public static readonly HygienePropertyDefinition CapabilityStatusProp = new HygienePropertyDefinition("capabilityStatus", typeof(AssignedCapabilityStatus), AssignedCapabilityStatus.Enabled, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition AssignedTimeStampProp = new HygienePropertyDefinition("assignedTimestamp", typeof(DateTime), SqlDateTime.MinValue.Value, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition InitialStateProp = new HygienePropertyDefinition("initialState", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
