using System;
using System.Collections.Generic;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions
{
	public sealed class UpdateEventParametersSchema : TypeSchema
	{
		public UpdateEventParametersSchema()
		{
			base.RegisterPropertyDefinition(UpdateEventParametersSchema.StaticAttendeesToAddProperty);
			base.RegisterPropertyDefinition(UpdateEventParametersSchema.StaticAttendeesToRemoveProperty);
		}

		public TypedPropertyDefinition<IList<Attendee>> AttendeesToAddProperty
		{
			get
			{
				return UpdateEventParametersSchema.StaticAttendeesToAddProperty;
			}
		}

		public TypedPropertyDefinition<IList<string>> AttendeesToRemoveProperty
		{
			get
			{
				return UpdateEventParametersSchema.StaticAttendeesToRemoveProperty;
			}
		}

		private static readonly TypedPropertyDefinition<IList<Attendee>> StaticAttendeesToAddProperty = new TypedPropertyDefinition<IList<Attendee>>("UpdateEvent.AttendeesToAdd", null, true);

		private static readonly TypedPropertyDefinition<IList<string>> StaticAttendeesToRemoveProperty = new TypedPropertyDefinition<IList<string>>("UpdateEvent.AttendeesToRemove", null, true);
	}
}
