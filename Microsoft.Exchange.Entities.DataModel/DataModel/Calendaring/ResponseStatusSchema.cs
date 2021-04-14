using System;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring
{
	public class ResponseStatusSchema : TypeSchema
	{
		public ResponseStatusSchema()
		{
			base.RegisterPropertyDefinition(ResponseStatusSchema.StaticResponseProperty);
			base.RegisterPropertyDefinition(ResponseStatusSchema.StaticTimeProperty);
		}

		public TypedPropertyDefinition<ResponseType> ResponseProperty
		{
			get
			{
				return ResponseStatusSchema.StaticResponseProperty;
			}
		}

		public TypedPropertyDefinition<ExDateTime> TimeProperty
		{
			get
			{
				return ResponseStatusSchema.StaticTimeProperty;
			}
		}

		private static readonly TypedPropertyDefinition<ResponseType> StaticResponseProperty = new TypedPropertyDefinition<ResponseType>("ResponseStatus.Response", ResponseType.None, true);

		private static readonly TypedPropertyDefinition<ExDateTime> StaticTimeProperty = new TypedPropertyDefinition<ExDateTime>("ResponseStatus.Time", default(ExDateTime), true);
	}
}
