using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.BirthdayCalendar.TypeConversion.StorageAccessors;
using Microsoft.Exchange.Entities.DataModel.BirthdayCalendar;
using Microsoft.Exchange.Entities.TypeConversion;
using Microsoft.Exchange.Entities.TypeConversion.Translators;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.BirthdayCalendar.TypeConversion.Translators
{
	internal class BirthdayContactTranslator : StorageEntityTranslator<IContact, BirthdayContact, BirthdayContactSchema>
	{
		protected BirthdayContactTranslator(IEnumerable<ITranslationRule<IContact, BirthdayContact>> additionalRules = null) : base(BirthdayContactTranslator.CreateTranslationRules().AddRules(additionalRules))
		{
		}

		public new static BirthdayContactTranslator Instance
		{
			get
			{
				return BirthdayContactTranslator.SingletonInstance;
			}
		}

		private static List<ITranslationRule<IContact, BirthdayContact>> CreateTranslationRules()
		{
			return new List<ITranslationRule<IContact, BirthdayContact>>
			{
				ContactAccessors.Birthday.MapTo(BirthdayContact.Accessors.Birthday),
				ContactAccessors.DisplayName.MapTo(BirthdayContact.Accessors.DisplayName),
				ContactAccessors.PersonId.MapTo(BirthdayContact.Accessors.PersonId),
				ContactAccessors.NotInBirthdayCalendar.MapTo(BirthdayContact.Accessors.ShouldHideBirthday),
				ContactAccessors.Attribution.MapTo(BirthdayContact.Accessors.Attribution),
				ContactAccessors.IsWritable.MapTo(BirthdayContact.Accessors.IsWritable)
			};
		}

		private static readonly BirthdayContactTranslator SingletonInstance = new BirthdayContactTranslator(null);
	}
}
