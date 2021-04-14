using System;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring
{
	public class ResponseStatus : SchematizedObject<ResponseStatusSchema>
	{
		public ResponseType Response
		{
			get
			{
				return base.GetPropertyValueOrDefault<ResponseType>(base.Schema.ResponseProperty);
			}
			set
			{
				base.SetPropertyValue<ResponseType>(base.Schema.ResponseProperty, value);
			}
		}

		public ExDateTime Time
		{
			get
			{
				return base.GetPropertyValueOrDefault<ExDateTime>(base.Schema.TimeProperty);
			}
			set
			{
				base.SetPropertyValue<ExDateTime>(base.Schema.TimeProperty, value);
			}
		}

		public static class Accessors
		{
			public static readonly EntityPropertyAccessor<ResponseStatus, ResponseType> Response = new EntityPropertyAccessor<ResponseStatus, ResponseType>(SchematizedObject<ResponseStatusSchema>.SchemaInstance.ResponseProperty, (ResponseStatus status) => status.Response, delegate(ResponseStatus status, ResponseType responseType)
			{
				status.Response = responseType;
			});

			public static readonly EntityPropertyAccessor<ResponseStatus, ExDateTime> Time = new EntityPropertyAccessor<ResponseStatus, ExDateTime>(SchematizedObject<ResponseStatusSchema>.SchemaInstance.TimeProperty, (ResponseStatus status) => status.Time, delegate(ResponseStatus status, ExDateTime time)
			{
				status.Time = time;
			});
		}
	}
}
