using System;
using Microsoft.Exchange.Data.Storage.ActivityLog;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class InferenceOLKUserActivityLoggingEnabledSmartProperty : SmartPropertyDefinition
	{
		public InferenceOLKUserActivityLoggingEnabledSmartProperty() : base("InferenceOLKUserActivityLoggingEnabled", typeof(bool), PropertyFlags.ReadOnly, PropertyDefinitionConstraint.None, new PropertyDependency[0])
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			return ActivityLogHelper.IsActivityLoggingEnabled(false);
		}
	}
}
