using System;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel.ReliableActions
{
	internal interface IActionPropagationState : IPropertyChangeTracker<PropertyDefinition>
	{
		Guid? LastExecutedAction { get; set; }
	}
}
