using System;
using System.Collections.Generic;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class RecurrencePattern
	{
		public RecurrencePatternType Type { get; set; }

		public int Interval { get; set; }

		public int Month { get; set; }

		public int DayOfMonth { get; set; }

		public ISet<DayOfWeek> DaysOfWeek { get; set; }

		public DayOfWeek FirstDayOfWeek { get; set; }

		public WeekIndex Index { get; set; }

		internal static readonly LazyMember<EdmComplexType> EdmComplexType = new LazyMember<EdmComplexType>(delegate()
		{
			EdmComplexType edmComplexType = new EdmComplexType(typeof(RecurrencePattern).Namespace, typeof(RecurrencePattern).Name, null, false);
			edmComplexType.AddProperty(new EdmStructuralProperty(edmComplexType, "Type", new EdmEnumTypeReference(EnumTypes.GetEdmEnumType(typeof(RecurrencePatternType)), true)));
			edmComplexType.AddProperty(new EdmStructuralProperty(edmComplexType, "Interval", EdmCoreModel.Instance.GetInt32(false)));
			edmComplexType.AddProperty(new EdmStructuralProperty(edmComplexType, "DayOfMonth", EdmCoreModel.Instance.GetInt32(false)));
			edmComplexType.AddProperty(new EdmStructuralProperty(edmComplexType, "Month", EdmCoreModel.Instance.GetInt32(false)));
			edmComplexType.AddProperty(new EdmStructuralProperty(edmComplexType, "DaysOfWeek", new EdmCollectionTypeReference(new EdmCollectionType(new EdmEnumTypeReference(EnumTypes.GetEdmEnumType(typeof(DayOfWeek)), true)))));
			edmComplexType.AddProperty(new EdmStructuralProperty(edmComplexType, "FirstDayOfWeek", new EdmEnumTypeReference(EnumTypes.GetEdmEnumType(typeof(DayOfWeek)), true)));
			edmComplexType.AddProperty(new EdmStructuralProperty(edmComplexType, "Index", new EdmEnumTypeReference(EnumTypes.GetEdmEnumType(typeof(WeekIndex)), true)));
			return edmComplexType;
		});
	}
}
