using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.TypeConversion.Converters;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.Converters
{
	internal struct ResponseTypeConverter : IResponseTypeConverter, IConverter<ResponseType, ResponseType>, IConverter<ResponseType, ResponseType>
	{
		public ResponseType Convert(ResponseType value)
		{
			return ResponseTypeConverter.mappingConverter.Convert(value);
		}

		public ResponseType Convert(ResponseType value)
		{
			return ResponseTypeConverter.mappingConverter.Reverse(value);
		}

		private static SimpleMappingConverter<ResponseType, ResponseType> mappingConverter = SimpleMappingConverter<ResponseType, ResponseType>.CreateStrictConverter(new Tuple<ResponseType, ResponseType>[]
		{
			new Tuple<ResponseType, ResponseType>(ResponseType.None, ResponseType.None),
			new Tuple<ResponseType, ResponseType>(ResponseType.Organizer, ResponseType.Organizer),
			new Tuple<ResponseType, ResponseType>(ResponseType.Tentative, ResponseType.TentativelyAccepted),
			new Tuple<ResponseType, ResponseType>(ResponseType.Accept, ResponseType.Accepted),
			new Tuple<ResponseType, ResponseType>(ResponseType.Decline, ResponseType.Declined),
			new Tuple<ResponseType, ResponseType>(ResponseType.NotResponded, ResponseType.NotResponded)
		});
	}
}
