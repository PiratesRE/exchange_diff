using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class DistributionListParticipantProperty : SmartPropertyDefinition
	{
		internal DistributionListParticipantProperty() : base("DistributionListParticipant", typeof(Participant), PropertyFlags.ReadOnly, Array<PropertyDefinitionConstraint>.Empty, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.EntryId, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.DisplayNameFirstLast, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.ItemClass, PropertyDependencyType.NeedForRead)
		})
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			if (!ObjectClass.IsOfClass(propertyBag.GetValueOrDefault<string>(InternalSchema.ItemClass), "IPM.DistList"))
			{
				return new PropertyError(this, PropertyErrorCode.NotSupported);
			}
			byte[] array = propertyBag.GetValue(InternalSchema.EntryId) as byte[];
			if (array != null)
			{
				return new Participant(propertyBag.GetValue(InternalSchema.DisplayNameFirstLast) as string, null, "MAPIPDL", new StoreParticipantOrigin(StoreObjectId.FromProviderSpecificId(array)), new KeyValuePair<PropertyDefinition, object>[0]);
			}
			return new PropertyError(this, PropertyErrorCode.NotFound);
		}
	}
}
