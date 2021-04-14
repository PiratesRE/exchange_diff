using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.TypeConversion.Converters;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.Converters
{
	internal interface IAttendeeTypeConverter : IConverter<Microsoft.Exchange.Data.Storage.AttendeeType, Microsoft.Exchange.Entities.DataModel.Calendaring.AttendeeType>, IConverter<Microsoft.Exchange.Entities.DataModel.Calendaring.AttendeeType, Microsoft.Exchange.Data.Storage.AttendeeType>
	{
	}
}
