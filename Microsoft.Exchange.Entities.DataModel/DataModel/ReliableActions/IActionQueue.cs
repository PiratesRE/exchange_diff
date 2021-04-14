using System;
using Microsoft.Exchange.Data.Storage.ReliableActions;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel.ReliableActions
{
	internal interface IActionQueue : IPropertyChangeTracker<PropertyDefinition>
	{
		ActionInfo[] ActionsToAdd { get; set; }

		Guid[] ActionsToRemove { get; set; }

		bool HasData { get; set; }

		ActionInfo[] OriginalActions { get; set; }
	}
}
