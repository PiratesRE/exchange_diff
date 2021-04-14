using System;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class RecurrenceRange
	{
		public RecurrenceRangeType Type { get; set; }

		public DateTimeOffset StartDate { get; set; }

		public DateTimeOffset EndDate { get; set; }

		public int NumberOfOccurrences { get; set; }

		internal static readonly LazyMember<EdmComplexType> EdmComplexType = new LazyMember<EdmComplexType>(delegate()
		{
			EdmComplexType edmComplexType = new EdmComplexType(typeof(RecurrenceRange).Namespace, typeof(RecurrenceRange).Name, null, false);
			edmComplexType.AddProperty(new EdmStructuralProperty(edmComplexType, "Type", new EdmEnumTypeReference(EnumTypes.GetEdmEnumType(typeof(RecurrenceRangeType)), true)));
			edmComplexType.AddProperty(new EdmStructuralProperty(edmComplexType, "StartDate", EdmCoreModel.Instance.GetDateTimeOffset(true)));
			edmComplexType.AddProperty(new EdmStructuralProperty(edmComplexType, "EndDate", EdmCoreModel.Instance.GetDateTimeOffset(true)));
			edmComplexType.AddProperty(new EdmStructuralProperty(edmComplexType, "NumberOfOccurrences", EdmCoreModel.Instance.GetInt32(false)));
			return edmComplexType;
		});
	}
}
