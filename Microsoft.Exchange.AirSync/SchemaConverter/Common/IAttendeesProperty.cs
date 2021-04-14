using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal interface IAttendeesProperty : IMultivaluedProperty<AttendeeData>, IProperty, IEnumerable<AttendeeData>, IEnumerable
	{
	}
}
