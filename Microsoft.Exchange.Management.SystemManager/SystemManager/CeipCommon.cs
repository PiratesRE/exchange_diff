using System;
using System.Collections;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.SystemManager
{
	public static class CeipCommon
	{
		public class IndustryTypeEnumListSource : EnumListSource
		{
			public IndustryTypeEnumListSource() : base(typeof(IndustryType))
			{
				base.Comparer = new CeipCommon.IndustryTypeEnumListSource.IndustryTypeComparer();
			}

			protected override string GetValueText(object objectValue)
			{
				string text = base.GetValueText(objectValue);
				if (CeipCommon.IndustryTypeEnumListSource.CompareIndustryType(objectValue, IndustryType.Other) || CeipCommon.IndustryTypeEnumListSource.CompareIndustryType(objectValue, IndustryType.NotSpecified))
				{
					text = "-- " + text + " --";
				}
				return text;
			}

			private static bool CompareIndustryType(object objectValue, IndustryType industryType)
			{
				return (IndustryType)objectValue == industryType;
			}

			private class IndustryTypeComparer : IComparer
			{
				public int Compare(object x, object y)
				{
					ObjectListSourceItem objectListSourceItem = x as ObjectListSourceItem;
					ObjectListSourceItem objectListSourceItem2 = y as ObjectListSourceItem;
					if (objectListSourceItem != null)
					{
						int num = objectListSourceItem.CompareTo(objectListSourceItem2);
						if (num != 0)
						{
							if (CeipCommon.IndustryTypeEnumListSource.CompareIndustryType(objectListSourceItem.Value, IndustryType.NotSpecified) || CeipCommon.IndustryTypeEnumListSource.CompareIndustryType(objectListSourceItem2.Value, IndustryType.Other))
							{
								num = -1;
							}
							else if (CeipCommon.IndustryTypeEnumListSource.CompareIndustryType(objectListSourceItem.Value, IndustryType.Other) || CeipCommon.IndustryTypeEnumListSource.CompareIndustryType(objectListSourceItem2.Value, IndustryType.NotSpecified))
							{
								num = 1;
							}
						}
						return num;
					}
					if (objectListSourceItem2 != null)
					{
						return -1;
					}
					return 0;
				}
			}
		}
	}
}
