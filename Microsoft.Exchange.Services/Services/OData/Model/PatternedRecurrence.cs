using System;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class PatternedRecurrence
	{
		public RecurrencePattern Pattern { get; set; }

		public RecurrenceRange Range { get; set; }

		internal static readonly LazyMember<EdmComplexType> EdmComplexType = new LazyMember<EdmComplexType>(delegate()
		{
			EdmComplexType edmComplexType = new EdmComplexType(typeof(PatternedRecurrence).Namespace, typeof(PatternedRecurrence).Name, null, false);
			edmComplexType.AddProperty(new EdmStructuralProperty(edmComplexType, "Pattern", new EdmComplexTypeReference(RecurrencePattern.EdmComplexType.Member, true)));
			edmComplexType.AddProperty(new EdmStructuralProperty(edmComplexType, "Range", new EdmComplexTypeReference(RecurrenceRange.EdmComplexType.Member, true)));
			return edmComplexType;
		});
	}
}
