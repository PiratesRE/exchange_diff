using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Services.OData.Web;
using Microsoft.OData.Core;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class PatternedRecurrenceODataConverter : IODataPropertyValueConverter
	{
		public object FromODataPropertyValue(object odataPropertyValue)
		{
			if (odataPropertyValue == null)
			{
				return null;
			}
			return PatternedRecurrenceODataConverter.ODataValueToPatternedRecurrence((ODataComplexValue)odataPropertyValue);
		}

		public object ToODataPropertyValue(object rawValue)
		{
			if (rawValue == null)
			{
				return null;
			}
			return PatternedRecurrenceODataConverter.PatternedRecurrenceToODataValue((PatternedRecurrence)rawValue);
		}

		internal static ODataValue PatternedRecurrenceToODataValue(PatternedRecurrence patternedRecurrence)
		{
			if (patternedRecurrence == null)
			{
				return null;
			}
			ODataComplexValue odataComplexValue = new ODataComplexValue();
			odataComplexValue.TypeName = typeof(PatternedRecurrence).FullName;
			List<ODataProperty> list = new List<ODataProperty>();
			RecurrencePattern pattern = patternedRecurrence.Pattern;
			if (pattern != null)
			{
				ODataComplexValue odataComplexValue2 = new ODataComplexValue();
				odataComplexValue2.TypeName = pattern.GetType().FullName;
				List<ODataProperty> list2 = new List<ODataProperty>();
				list2.Add(new ODataProperty
				{
					Name = "Type",
					Value = EnumConverter.ToODataEnumValue(pattern.Type)
				});
				list2.Add(new ODataProperty
				{
					Name = "Interval",
					Value = pattern.Interval
				});
				list2.Add(new ODataProperty
				{
					Name = "Month",
					Value = pattern.Month
				});
				list2.Add(new ODataProperty
				{
					Name = "Index",
					Value = EnumConverter.ToODataEnumValue(pattern.Index)
				});
				list2.Add(new ODataProperty
				{
					Name = "FirstDayOfWeek",
					Value = EnumConverter.ToODataEnumValue(pattern.FirstDayOfWeek)
				});
				list2.Add(new ODataProperty
				{
					Name = "DayOfMonth",
					Value = pattern.DayOfMonth
				});
				if (pattern.DaysOfWeek != null)
				{
					ODataCollectionValue odataCollectionValue = new ODataCollectionValue();
					odataCollectionValue.TypeName = typeof(DayOfWeek).MakeODataCollectionTypeName();
					odataCollectionValue.Items = from e in pattern.DaysOfWeek
					select EnumConverter.ToODataEnumValue(e);
					list2.Add(new ODataProperty
					{
						Name = "DaysOfWeek",
						Value = odataCollectionValue
					});
				}
				odataComplexValue2.Properties = list2;
				list.Add(new ODataProperty
				{
					Name = "Pattern",
					Value = odataComplexValue2
				});
			}
			RecurrenceRange range = patternedRecurrence.Range;
			if (range != null)
			{
				ODataComplexValue odataComplexValue3 = new ODataComplexValue();
				odataComplexValue3.TypeName = range.GetType().FullName;
				odataComplexValue3.Properties = new List<ODataProperty>
				{
					new ODataProperty
					{
						Name = "Type",
						Value = EnumConverter.ToODataEnumValue(range.Type)
					},
					new ODataProperty
					{
						Name = "StartDate",
						Value = range.StartDate
					},
					new ODataProperty
					{
						Name = "EndDate",
						Value = range.EndDate
					},
					new ODataProperty
					{
						Name = "NumberOfOccurrences",
						Value = range.NumberOfOccurrences
					}
				};
				list.Add(new ODataProperty
				{
					Name = "Range",
					Value = odataComplexValue3
				});
			}
			odataComplexValue.Properties = list;
			return odataComplexValue;
		}

		internal static PatternedRecurrence ODataValueToPatternedRecurrence(ODataComplexValue patternedRecurrenceComplexValue)
		{
			if (patternedRecurrenceComplexValue == null)
			{
				return null;
			}
			PatternedRecurrence patternedRecurrence = new PatternedRecurrence();
			ODataComplexValue propertyValue = patternedRecurrenceComplexValue.GetPropertyValue("Pattern", null);
			if (propertyValue != null)
			{
				RecurrencePattern recurrencePattern = new RecurrencePattern();
				recurrencePattern.Type = EnumConverter.FromODataEnumValue<RecurrencePatternType>(propertyValue.GetPropertyValue("Type", null));
				recurrencePattern.Interval = propertyValue.GetPropertyValue("Interval", 0);
				recurrencePattern.DayOfMonth = propertyValue.GetPropertyValue("DayOfMonth", 0);
				recurrencePattern.Month = propertyValue.GetPropertyValue("Month", 0);
				recurrencePattern.Index = EnumConverter.FromODataEnumValue<WeekIndex>(propertyValue.GetPropertyValue("Index", null));
				recurrencePattern.FirstDayOfWeek = EnumConverter.FromODataEnumValue<DayOfWeek>(propertyValue.GetPropertyValue("FirstDayOfWeek", null));
				ODataCollectionValue propertyValue2 = propertyValue.GetPropertyValue("DaysOfWeek", null);
				if (propertyValue2 != null)
				{
					HashSet<DayOfWeek> hashSet = new HashSet<DayOfWeek>();
					foreach (object obj in propertyValue2.Items)
					{
						ODataEnumValue odataValue = (ODataEnumValue)obj;
						hashSet.Add(EnumConverter.FromODataEnumValue<DayOfWeek>(odataValue));
					}
					recurrencePattern.DaysOfWeek = hashSet;
				}
				patternedRecurrence.Pattern = recurrencePattern;
			}
			ODataComplexValue propertyValue3 = patternedRecurrenceComplexValue.GetPropertyValue("Range", null);
			if (propertyValue3 != null)
			{
				patternedRecurrence.Range = new RecurrenceRange
				{
					Type = EnumConverter.FromODataEnumValue<RecurrenceRangeType>(propertyValue3.GetPropertyValue("Type", null)),
					StartDate = propertyValue3.GetPropertyValue("StartDate", default(DateTimeOffset)),
					EndDate = propertyValue3.GetPropertyValue("EndDate", default(DateTimeOffset)),
					NumberOfOccurrences = propertyValue3.GetPropertyValue("NumberOfOccurrences", 0)
				};
			}
			return patternedRecurrence;
		}
	}
}
