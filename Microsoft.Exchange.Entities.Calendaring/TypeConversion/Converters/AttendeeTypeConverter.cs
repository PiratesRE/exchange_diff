using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.TypeConversion.Converters;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.Converters
{
	internal struct AttendeeTypeConverter : IAttendeeTypeConverter, IConverter<Microsoft.Exchange.Data.Storage.AttendeeType, Microsoft.Exchange.Entities.DataModel.Calendaring.AttendeeType>, IConverter<Microsoft.Exchange.Entities.DataModel.Calendaring.AttendeeType, Microsoft.Exchange.Data.Storage.AttendeeType>
	{
		public Microsoft.Exchange.Entities.DataModel.Calendaring.AttendeeType Convert(Microsoft.Exchange.Data.Storage.AttendeeType value)
		{
			return AttendeeTypeConverter.mappingConverter.Convert(value);
		}

		public Microsoft.Exchange.Data.Storage.AttendeeType Convert(Microsoft.Exchange.Entities.DataModel.Calendaring.AttendeeType value)
		{
			return AttendeeTypeConverter.mappingConverter.Reverse(value);
		}

		private static SimpleMappingConverter<Microsoft.Exchange.Data.Storage.AttendeeType, Microsoft.Exchange.Entities.DataModel.Calendaring.AttendeeType> mappingConverter = SimpleMappingConverter<Microsoft.Exchange.Data.Storage.AttendeeType, Microsoft.Exchange.Entities.DataModel.Calendaring.AttendeeType>.CreateStrictConverter(new Tuple<Microsoft.Exchange.Data.Storage.AttendeeType, Microsoft.Exchange.Entities.DataModel.Calendaring.AttendeeType>[]
		{
			new Tuple<Microsoft.Exchange.Data.Storage.AttendeeType, Microsoft.Exchange.Entities.DataModel.Calendaring.AttendeeType>(Microsoft.Exchange.Data.Storage.AttendeeType.Required, Microsoft.Exchange.Entities.DataModel.Calendaring.AttendeeType.Required),
			new Tuple<Microsoft.Exchange.Data.Storage.AttendeeType, Microsoft.Exchange.Entities.DataModel.Calendaring.AttendeeType>(Microsoft.Exchange.Data.Storage.AttendeeType.Optional, Microsoft.Exchange.Entities.DataModel.Calendaring.AttendeeType.Optional),
			new Tuple<Microsoft.Exchange.Data.Storage.AttendeeType, Microsoft.Exchange.Entities.DataModel.Calendaring.AttendeeType>(Microsoft.Exchange.Data.Storage.AttendeeType.Resource, Microsoft.Exchange.Entities.DataModel.Calendaring.AttendeeType.Resource)
		});
	}
}
