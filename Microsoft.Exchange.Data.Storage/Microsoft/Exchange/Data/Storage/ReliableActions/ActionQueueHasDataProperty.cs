using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.ReliableActions
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ActionQueueHasDataProperty : SmartPropertyDefinition
	{
		public ActionQueueHasDataProperty(NativeStorePropertyDefinition queueHasDataFlagProperty) : base("ActionQueueHasData", typeof(bool), PropertyFlags.ReadOnly, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(queueHasDataFlagProperty, PropertyDependencyType.NeedForRead)
		})
		{
			ArgumentValidator.ThrowIfNull("queueHasDataFlagProperty", queueHasDataFlagProperty);
			this.queueHasDataFlagProperty = queueHasDataFlagProperty;
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			return propertyBag.GetValueOrDefault<bool>(this.queueHasDataFlagProperty);
		}

		private readonly NativeStorePropertyDefinition queueHasDataFlagProperty;
	}
}
