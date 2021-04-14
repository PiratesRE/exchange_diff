using System;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class Attendee : Recipient
	{
		public ResponseStatus Status { get; set; }

		public AttendeeType Type { get; set; }

		internal new static readonly LazyMember<EdmComplexType> EdmComplexType = new LazyMember<EdmComplexType>(delegate()
		{
			EdmComplexType edmComplexType = new EdmComplexType(typeof(Attendee).Namespace, typeof(Attendee).Name, Recipient.EdmComplexType.Member, false);
			edmComplexType.AddProperty(new EdmStructuralProperty(edmComplexType, "Status", new EdmComplexTypeReference(ResponseStatus.EdmComplexType.Member, true)));
			edmComplexType.AddProperty(new EdmStructuralProperty(edmComplexType, "Type", new EdmEnumTypeReference(EnumTypes.GetEdmEnumType(typeof(AttendeeType)), true)));
			return edmComplexType;
		});
	}
}
