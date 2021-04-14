using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.DataModel.BirthdayCalendar
{
	public sealed class BirthdayEventSchema : StorageEntitySchema
	{
		public BirthdayEventSchema()
		{
			base.RegisterPropertyDefinition(BirthdayEventSchema.StaticAttributionProperty);
			base.RegisterPropertyDefinition(BirthdayEventSchema.StaticSubjectProperty);
			base.RegisterPropertyDefinition(BirthdayEventSchema.StaticBirthdayProperty);
			base.RegisterPropertyDefinition(BirthdayEventSchema.StaticPersonIdProperty);
			base.RegisterPropertyDefinition(BirthdayEventSchema.StaticContactIdProperty);
			base.RegisterPropertyDefinition(BirthdayEventSchema.StaticIsWritableProperty);
		}

		public TypedPropertyDefinition<string> AttributionProperty
		{
			get
			{
				return BirthdayEventSchema.StaticAttributionProperty;
			}
		}

		public TypedPropertyDefinition<string> SubjectProperty
		{
			get
			{
				return BirthdayEventSchema.StaticSubjectProperty;
			}
		}

		public TypedPropertyDefinition<ExDateTime> BirthdayProperty
		{
			get
			{
				return BirthdayEventSchema.StaticBirthdayProperty;
			}
		}

		public TypedPropertyDefinition<bool> IsWritableProperty
		{
			get
			{
				return BirthdayEventSchema.StaticIsWritableProperty;
			}
		}

		internal TypedPropertyDefinition<PersonId> PersonIdProperty
		{
			get
			{
				return BirthdayEventSchema.StaticPersonIdProperty;
			}
		}

		internal TypedPropertyDefinition<StoreObjectId> ContactIdProperty
		{
			get
			{
				return BirthdayEventSchema.StaticContactIdProperty;
			}
		}

		private static readonly TypedPropertyDefinition<string> StaticAttributionProperty = new TypedPropertyDefinition<string>("BirthdayEvent.Attribution", null, true);

		private static readonly TypedPropertyDefinition<string> StaticSubjectProperty = new TypedPropertyDefinition<string>("BirthdayEvent.Subject", null, true);

		private static readonly TypedPropertyDefinition<ExDateTime> StaticBirthdayProperty = new TypedPropertyDefinition<ExDateTime>("BirthdayEvent.Birthday", default(ExDateTime), true);

		private static readonly TypedPropertyDefinition<PersonId> StaticPersonIdProperty = new TypedPropertyDefinition<PersonId>("BirthdayEvent.PersonId", null, false);

		private static readonly TypedPropertyDefinition<StoreObjectId> StaticContactIdProperty = new TypedPropertyDefinition<StoreObjectId>("BirthdayEvent.ContactId", null, false);

		private static readonly TypedPropertyDefinition<bool> StaticIsWritableProperty = new TypedPropertyDefinition<bool>("BirthdayEvent.IsWritable", false, true);
	}
}
