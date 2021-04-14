using System;
using System.Collections.Generic;
using Microsoft.Exchange.Services.OData.Web;
using Microsoft.OData.Core;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class AttendeeODataConverter : IODataPropertyValueConverter
	{
		public object FromODataPropertyValue(object odataPropertyValue)
		{
			return AttendeeODataConverter.ODataValueToAttendee((ODataComplexValue)odataPropertyValue);
		}

		public object ToODataPropertyValue(object rawValue)
		{
			return AttendeeODataConverter.AttendeeToODataValue((Attendee)rawValue);
		}

		internal static ODataValue AttendeeToODataValue(Attendee attendee)
		{
			if (attendee == null)
			{
				return null;
			}
			ODataComplexValue odataComplexValue = new ODataComplexValue();
			odataComplexValue.TypeName = typeof(Attendee).FullName;
			List<ODataProperty> properties = new List<ODataProperty>
			{
				new ODataProperty
				{
					Name = "Name",
					Value = attendee.Name
				},
				new ODataProperty
				{
					Name = "Address",
					Value = attendee.Address
				},
				new ODataProperty
				{
					Name = "Status",
					Value = ResponseStatusODataConverter.ResponseStatusToODataValue(attendee.Status)
				},
				new ODataProperty
				{
					Name = "Type",
					Value = EnumConverter.ToODataEnumValue(attendee.Type)
				}
			};
			odataComplexValue.Properties = properties;
			return odataComplexValue;
		}

		internal static Attendee ODataValueToAttendee(ODataComplexValue complexValue)
		{
			if (complexValue == null)
			{
				return null;
			}
			return new Attendee
			{
				Name = complexValue.GetPropertyValue("Name", null),
				Address = complexValue.GetPropertyValue("Address", null),
				Status = ResponseStatusODataConverter.ODataValueToResponseStatus(complexValue.GetPropertyValue("Status", null)),
				Type = EnumConverter.FromODataEnumValue<AttendeeType>(complexValue.GetPropertyValue("Type", null))
			};
		}
	}
}
