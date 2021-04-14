using System;
using System.Collections.Generic;
using Microsoft.Exchange.Services.OData.Web;
using Microsoft.OData.Core;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class ResponseStatusODataConverter : IODataPropertyValueConverter
	{
		public object FromODataPropertyValue(object odataPropertyValue)
		{
			return ResponseStatusODataConverter.ODataValueToResponseStatus((ODataComplexValue)odataPropertyValue);
		}

		public object ToODataPropertyValue(object rawValue)
		{
			return ResponseStatusODataConverter.ResponseStatusToODataValue((ResponseStatus)rawValue);
		}

		internal static ODataValue ResponseStatusToODataValue(ResponseStatus responseStatus)
		{
			if (responseStatus == null)
			{
				return null;
			}
			ODataComplexValue odataComplexValue = new ODataComplexValue();
			odataComplexValue.TypeName = typeof(ResponseStatus).FullName;
			List<ODataProperty> properties = new List<ODataProperty>
			{
				new ODataProperty
				{
					Name = "Response",
					Value = EnumConverter.ToODataEnumValue(responseStatus.Response)
				},
				new ODataProperty
				{
					Name = "Time",
					Value = responseStatus.Time
				}
			};
			odataComplexValue.Properties = properties;
			return odataComplexValue;
		}

		internal static ResponseStatus ODataValueToResponseStatus(ODataComplexValue complexValue)
		{
			if (complexValue == null)
			{
				return null;
			}
			return new ResponseStatus
			{
				Response = EnumConverter.FromODataEnumValue<ResponseType>(complexValue.GetPropertyValue("Response", null)),
				Time = complexValue.GetPropertyValue("Time", default(DateTimeOffset))
			};
		}
	}
}
