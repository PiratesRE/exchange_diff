using System;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class ResponseStatus
	{
		public ResponseType Response { get; set; }

		public DateTimeOffset Time { get; set; }

		internal static readonly LazyMember<EdmComplexType> EdmComplexType = new LazyMember<EdmComplexType>(delegate()
		{
			EdmComplexType edmComplexType = new EdmComplexType(typeof(ResponseStatus).Namespace, typeof(ResponseStatus).Name, Recipient.EdmComplexType.Member, false);
			edmComplexType.AddProperty(new EdmStructuralProperty(edmComplexType, "Response", new EdmEnumTypeReference(EnumTypes.GetEdmEnumType(typeof(ResponseType)), true)));
			edmComplexType.AddProperty(new EdmStructuralProperty(edmComplexType, "Time", EdmCoreModel.Instance.GetDateTimeOffset(true)));
			return edmComplexType;
		});
	}
}
