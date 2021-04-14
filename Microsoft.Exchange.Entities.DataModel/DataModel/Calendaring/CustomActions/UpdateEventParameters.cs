using System;
using System.Collections.Generic;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions
{
	public sealed class UpdateEventParameters : SchematizedObject<UpdateEventParametersSchema>
	{
		public IList<Attendee> AttendeesToAdd
		{
			get
			{
				return base.GetPropertyValueOrDefault<IList<Attendee>>(base.Schema.AttendeesToAddProperty);
			}
			set
			{
				base.SetPropertyValue<IList<Attendee>>(base.Schema.AttendeesToAddProperty, value);
			}
		}

		public IList<string> AttendeesToRemove
		{
			get
			{
				return base.GetPropertyValueOrDefault<IList<string>>(base.Schema.AttendeesToRemoveProperty);
			}
			set
			{
				base.SetPropertyValue<IList<string>>(base.Schema.AttendeesToRemoveProperty, value);
			}
		}
	}
}
