using System;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions
{
	public abstract class EventWorkflowParametersSchema : TypeSchema
	{
		protected EventWorkflowParametersSchema()
		{
			base.RegisterPropertyDefinition(EventWorkflowParametersSchema.StaticImportanceProperty);
			base.RegisterPropertyDefinition(EventWorkflowParametersSchema.StaticNotesProperty);
		}

		public TypedPropertyDefinition<Importance> ImportanceProperty
		{
			get
			{
				return EventWorkflowParametersSchema.StaticImportanceProperty;
			}
		}

		public TypedPropertyDefinition<ItemBody> NotesProperty
		{
			get
			{
				return EventWorkflowParametersSchema.StaticNotesProperty;
			}
		}

		private static readonly TypedPropertyDefinition<Importance> StaticImportanceProperty = new TypedPropertyDefinition<Importance>("EventWorkflowParameters.Importance", Importance.Low, true);

		private static readonly TypedPropertyDefinition<ItemBody> StaticNotesProperty = new TypedPropertyDefinition<ItemBody>("EventWorkflowParameters.Notes", null, true);
	}
}
