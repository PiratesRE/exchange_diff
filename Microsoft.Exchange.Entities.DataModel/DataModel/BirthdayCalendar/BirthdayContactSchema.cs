using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.DataModel.BirthdayCalendar
{
	public sealed class BirthdayContactSchema : StorageEntitySchema
	{
		public BirthdayContactSchema()
		{
			base.RegisterPropertyDefinition(BirthdayContactSchema.StaticAttributionProperty);
			base.RegisterPropertyDefinition(BirthdayContactSchema.StaticDisplayNameProperty);
			base.RegisterPropertyDefinition(BirthdayContactSchema.StaticBirthdayProperty);
			base.RegisterPropertyDefinition(BirthdayContactSchema.StaticPersonIdProperty);
			base.RegisterPropertyDefinition(BirthdayContactSchema.StaticShouldHideBirthdayProperty);
			base.RegisterPropertyDefinition(BirthdayContactSchema.StaticIsWritableProperty);
		}

		public TypedPropertyDefinition<string> AttributionProperty
		{
			get
			{
				return BirthdayContactSchema.StaticAttributionProperty;
			}
		}

		public TypedPropertyDefinition<ExDateTime?> BirthdayProperty
		{
			get
			{
				return BirthdayContactSchema.StaticBirthdayProperty;
			}
		}

		public TypedPropertyDefinition<string> DisplayNameProperty
		{
			get
			{
				return BirthdayContactSchema.StaticDisplayNameProperty;
			}
		}

		public TypedPropertyDefinition<bool> ShouldHideBirthdayProperty
		{
			get
			{
				return BirthdayContactSchema.StaticShouldHideBirthdayProperty;
			}
		}

		public TypedPropertyDefinition<bool> IsWritableProperty
		{
			get
			{
				return BirthdayContactSchema.StaticIsWritableProperty;
			}
		}

		internal TypedPropertyDefinition<PersonId> PersonIdProperty
		{
			get
			{
				return BirthdayContactSchema.StaticPersonIdProperty;
			}
		}

		private static readonly TypedPropertyDefinition<string> StaticDisplayNameProperty = new TypedPropertyDefinition<string>("BirthdayContact.DisplayName", null, false);

		private static readonly TypedPropertyDefinition<string> StaticAttributionProperty = new TypedPropertyDefinition<string>("BirthdayContact.Attribution", null, true);

		private static readonly TypedPropertyDefinition<ExDateTime?> StaticBirthdayProperty = new TypedPropertyDefinition<ExDateTime?>("BirthdayContact.Birthday", null, true);

		private static readonly TypedPropertyDefinition<PersonId> StaticPersonIdProperty = new TypedPropertyDefinition<PersonId>("BirthdayContact.PersonId", null, false);

		private static readonly TypedPropertyDefinition<bool> StaticShouldHideBirthdayProperty = new TypedPropertyDefinition<bool>("BirthdayContact.ShouldHideBirthday", false, true);

		private static readonly TypedPropertyDefinition<bool> StaticIsWritableProperty = new TypedPropertyDefinition<bool>("BirthdayContact.IsWritable", false, true);
	}
}
