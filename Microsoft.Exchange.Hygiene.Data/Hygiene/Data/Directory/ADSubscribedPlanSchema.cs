using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Hygiene.Data.Sync;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class ADSubscribedPlanSchema : CommonSyncProperties
	{
		public static readonly HygienePropertyDefinition AccountIdProperty = new HygienePropertyDefinition("accountId", typeof(ADObjectId));

		public static readonly HygienePropertyDefinition CapabilityProperty = AssignedPlanSchema.CapabilityProp;

		public static readonly HygienePropertyDefinition ServiceTypeProperty = new HygienePropertyDefinition("serviceType", typeof(string));

		public static readonly HygienePropertyDefinition MaximumOverageUnitsDetailProperty = new HygienePropertyDefinition("MaximumOverageUnitsDetail", typeof(string));

		public static readonly HygienePropertyDefinition PrepaidUnitsDetailProperty = new HygienePropertyDefinition("PrepaidUnitsDetail", typeof(string));

		public static readonly HygienePropertyDefinition TotalTrialUnitsDetailProperty = new HygienePropertyDefinition("TotalTrialUnitsDetail", typeof(string));
	}
}
