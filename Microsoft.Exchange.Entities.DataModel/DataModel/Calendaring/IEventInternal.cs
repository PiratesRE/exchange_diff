using System;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring
{
	internal interface IEventInternal : IPropertyChangeTracker<PropertyDefinition>
	{
		bool MarkAllPropagatedPropertiesAsException { get; set; }

		bool SeriesToInstancePropagation { get; set; }

		bool IsReceived { get; set; }

		int InstanceCreationIndex { get; set; }

		int SeriesCreationHash { get; set; }

		int SeriesSequenceNumber { get; set; }

		string GlobalObjectId { get; set; }
	}
}
