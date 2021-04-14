using System;
using Microsoft.Exchange.Entities.DataModel.Calendaring;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal static class ResponseStatusDataEntityConverter
	{
		internal static ResponseStatus ToResponseStatus(this ResponseStatus dataEntityResponseStatus)
		{
			if (dataEntityResponseStatus == null)
			{
				return null;
			}
			return new ResponseStatus
			{
				Response = EnumConverter.CastEnumType<ResponseType>(dataEntityResponseStatus.Response),
				Time = dataEntityResponseStatus.Time.ToDateTimeOffset()
			};
		}

		internal static ResponseStatus ToDataEntityResponseStatus(this ResponseStatus responseStatus)
		{
			if (responseStatus == null)
			{
				return null;
			}
			return new ResponseStatus
			{
				Response = EnumConverter.CastEnumType<ResponseType>(responseStatus.Response),
				Time = responseStatus.Time.ToExDateTime()
			};
		}
	}
}
