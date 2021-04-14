using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.Entities.TypeConversion;
using Microsoft.Exchange.Entities.TypeConversion.Converters;
using Microsoft.Exchange.Entities.TypeConversion.PropertyAccessors.StorageAccessors;
using Microsoft.Exchange.Entities.TypeConversion.Translators;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.Translators
{
	internal class EventWorkflowParametersTranslator<TParameters, TSchema> : StorageTranslator<MessageItem, TParameters> where TParameters : EventWorkflowParameters<TSchema>, new() where TSchema : EventWorkflowParametersSchema, new()
	{
		private EventWorkflowParametersTranslator() : base(EventWorkflowParametersTranslator<TParameters, TSchema>.CreateTranslationRules())
		{
		}

		public static EventWorkflowParametersTranslator<TParameters, TSchema> Instance
		{
			get
			{
				return EventWorkflowParametersTranslator<TParameters, TSchema>.SingletonInstance;
			}
		}

		protected override TParameters CreateEntity()
		{
			return Activator.CreateInstance<TParameters>();
		}

		private static IList<ITranslationRule<MessageItem, TParameters>> CreateTranslationRules()
		{
			return new List<ITranslationRule<MessageItem, TParameters>>
			{
				ItemAccessors<MessageItem>.Body.MapTo(EventWorkflowParameters<TSchema>.Accessors.Notes),
				ItemAccessors<MessageItem>.Importance.MapTo(EventWorkflowParameters<TSchema>.Accessors.Importance, default(ImportanceConverter))
			};
		}

		private static readonly EventWorkflowParametersTranslator<TParameters, TSchema> SingletonInstance = new EventWorkflowParametersTranslator<TParameters, TSchema>();
	}
}
