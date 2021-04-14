using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.TypeConversion.Converters;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.Converters
{
	internal interface IResponseTypeConverter : IConverter<ResponseType, ResponseType>, IConverter<ResponseType, ResponseType>
	{
	}
}
