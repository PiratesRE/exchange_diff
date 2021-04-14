using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.Calendaring.TypeConversion.PropertyAccessors.StorageAccessors;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.TypeConversion;
using Microsoft.Exchange.Entities.TypeConversion.Translators;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.Translators
{
	internal class MeetingMessageTranslator : ItemTranslator<MeetingMessage, MeetingMessage, MeetingMessageSchema>
	{
		protected MeetingMessageTranslator(IEnumerable<ITranslationRule<MeetingMessage, MeetingMessage>> additionalRules = null) : base(MeetingMessageTranslator.CreateTranslationRules().AddRules(additionalRules))
		{
		}

		public new static MeetingMessageTranslator Instance
		{
			get
			{
				return MeetingMessageTranslator.SingletonInstance;
			}
		}

		private static List<ITranslationRule<MeetingMessage, MeetingMessage>> CreateTranslationRules()
		{
			return new List<ITranslationRule<MeetingMessage, MeetingMessage>>
			{
				MeetingMessageAccessors.OccurrencesExceptionalViewProperties.MapTo(MeetingMessage.Accessors.OccurrencesExceptionalViewProperties),
				MeetingMessageAccessors.Type.MapTo(MeetingMessage.Accessors.Type)
			};
		}

		private static readonly MeetingMessageTranslator SingletonInstance = new MeetingMessageTranslator(null);
	}
}
