using System;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions
{
	public abstract class EventWorkflowParameters<TSchema> : SchematizedObject<TSchema> where TSchema : EventWorkflowParametersSchema, new()
	{
		public Importance Importance
		{
			get
			{
				TSchema schema = base.Schema;
				return base.GetPropertyValueOrDefault<Importance>(schema.ImportanceProperty);
			}
			set
			{
				TSchema schema = base.Schema;
				base.SetPropertyValue<Importance>(schema.ImportanceProperty, value);
			}
		}

		public ItemBody Notes
		{
			get
			{
				TSchema schema = base.Schema;
				return base.GetPropertyValueOrDefault<ItemBody>(schema.NotesProperty);
			}
			set
			{
				TSchema schema = base.Schema;
				base.SetPropertyValue<ItemBody>(schema.NotesProperty, value);
			}
		}

		public static class Accessors
		{
			// Note: this type is marked as 'beforefieldinit'.
			static Accessors()
			{
				TSchema schemaInstance = SchematizedObject<TSchema>.SchemaInstance;
				EventWorkflowParameters<TSchema>.Accessors.Importance = new EntityPropertyAccessor<EventWorkflowParameters<TSchema>, Importance>(schemaInstance.ImportanceProperty, (EventWorkflowParameters<TSchema> parameters) => parameters.Importance, delegate(EventWorkflowParameters<TSchema> parameters, Importance importance)
				{
					parameters.Importance = importance;
				});
				TSchema schemaInstance2 = SchematizedObject<TSchema>.SchemaInstance;
				EventWorkflowParameters<TSchema>.Accessors.Notes = new EntityPropertyAccessor<EventWorkflowParameters<TSchema>, ItemBody>(schemaInstance2.NotesProperty, (EventWorkflowParameters<TSchema> parameters) => parameters.Notes, delegate(EventWorkflowParameters<TSchema> parameters, ItemBody body)
				{
					parameters.Notes = body;
				});
			}

			public static readonly EntityPropertyAccessor<EventWorkflowParameters<TSchema>, Importance> Importance;

			public static readonly EntityPropertyAccessor<EventWorkflowParameters<TSchema>, ItemBody> Notes;
		}
	}
}
