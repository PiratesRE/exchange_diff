using System;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring
{
	public class AttendeeSchema : RecipientSchema
	{
		public AttendeeSchema()
		{
			base.RegisterPropertyDefinition(AttendeeSchema.StaticStatusProperty);
			base.RegisterPropertyDefinition(AttendeeSchema.StaticTypeProperty);
		}

		public TypedPropertyDefinition<ResponseStatus> StatusProperty
		{
			get
			{
				return AttendeeSchema.StaticStatusProperty;
			}
		}

		public TypedPropertyDefinition<AttendeeType> TypeProperty
		{
			get
			{
				return AttendeeSchema.StaticTypeProperty;
			}
		}

		private static readonly TypedPropertyDefinition<ResponseStatus> StaticStatusProperty = new TypedPropertyDefinition<ResponseStatus>("Attendee.Status", null, true);

		private static readonly TypedPropertyDefinition<AttendeeType> StaticTypeProperty = new TypedPropertyDefinition<AttendeeType>("Attendee.Type", (AttendeeType)0, true);
	}
}
