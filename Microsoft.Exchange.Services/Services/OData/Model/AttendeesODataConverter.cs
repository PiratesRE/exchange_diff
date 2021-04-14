using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Services.OData.Web;
using Microsoft.OData.Core;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class AttendeesODataConverter : IODataPropertyValueConverter
	{
		public object FromODataPropertyValue(object odataPropertyValue)
		{
			ODataCollectionValue odataCollectionValue = (ODataCollectionValue)odataPropertyValue;
			IEnumerable<Attendee> source = from ODataComplexValue x in odataCollectionValue.Items
			select AttendeeODataConverter.ODataValueToAttendee(x);
			return source.ToArray<Attendee>();
		}

		public object ToODataPropertyValue(object rawValue)
		{
			Attendee[] array = ((Attendee[])rawValue) ?? new Attendee[0];
			ODataCollectionValue odataCollectionValue = new ODataCollectionValue();
			odataCollectionValue.TypeName = typeof(Attendee).MakeODataCollectionTypeName();
			odataCollectionValue.Items = Array.ConvertAll<Attendee, ODataValue>(array, (Attendee x) => AttendeeODataConverter.AttendeeToODataValue(x));
			return odataCollectionValue;
		}
	}
}
