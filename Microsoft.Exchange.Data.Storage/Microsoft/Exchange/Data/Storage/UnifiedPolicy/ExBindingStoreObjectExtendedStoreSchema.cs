using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Data.Storage.UnifiedPolicy
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ExBindingStoreObjectExtendedStoreSchema : ObjectSchema
	{
		public static readonly ExtendedPropertyDefinition Name = new ExtendedPropertyDefinition(WellKnownPropertySet.UnifiedPolicy, "Name", 25);

		public static readonly ExtendedPropertyDefinition MasterIdentity = new ExtendedPropertyDefinition(WellKnownPropertySet.UnifiedPolicy, "MasterIdentity", 25);

		public static readonly ExtendedPropertyDefinition Workload = new ExtendedPropertyDefinition(WellKnownPropertySet.UnifiedPolicy, "Workload", 14);

		public static readonly ExtendedPropertyDefinition PolicyVersion = new ExtendedPropertyDefinition(WellKnownPropertySet.UnifiedPolicy, "PolicyVersion", 2);
	}
}
