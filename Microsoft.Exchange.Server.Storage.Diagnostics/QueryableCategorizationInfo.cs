using System;
using System.Text;
using Microsoft.Exchange.Server.Storage.LazyIndexing;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	public class QueryableCategorizationInfo
	{
		public QueryableCategorizationInfo(CategorizationInfo categorizationInfo)
		{
			this.categorizationInfo = categorizationInfo;
		}

		public int BaseMessageViewLogicalIndexNumber
		{
			get
			{
				return this.categorizationInfo.BaseMessageViewLogicalIndexNumber;
			}
		}

		public bool BaseMessageViewInReverseOrder
		{
			get
			{
				return this.categorizationInfo.BaseMessageViewInReverseOrder;
			}
		}

		public int CategoryCount
		{
			get
			{
				return this.categorizationInfo.CategoryCount;
			}
		}

		public string CategoryHeaderSortOverrides
		{
			get
			{
				if (this.categoryHeaderSortOverrides == null && this.categorizationInfo.CategoryHeaderSortOverrides != null && this.categorizationInfo.CategoryHeaderSortOverrides.Length > 0)
				{
					StringBuilder stringBuilder = new StringBuilder(50 * this.categorizationInfo.CategoryHeaderSortOverrides.Length);
					for (int i = 0; i < this.categorizationInfo.CategoryHeaderSortOverrides.Length; i++)
					{
						if (i != 0)
						{
							stringBuilder.Append(", ");
						}
						CategoryHeaderSortOverride categoryHeaderSortOverride = this.categorizationInfo.CategoryHeaderSortOverrides[i];
						if (categoryHeaderSortOverride == null)
						{
							stringBuilder.AppendFormat("Level {0}: None", i);
						}
						else
						{
							stringBuilder.AppendFormat("Level {0}: ", i);
							categoryHeaderSortOverride.Column.AppendToString(stringBuilder, StringFormatOptions.IncludeDetails);
							stringBuilder.AppendFormat(" {0} (aggregate by {1})", categoryHeaderSortOverride.Ascending ? "asc" : "desc", categoryHeaderSortOverride.AggregateByMaxValue ? "max" : "min");
						}
					}
					this.categoryHeaderSortOverrides = stringBuilder.ToString();
				}
				return this.categoryHeaderSortOverrides;
			}
		}

		private readonly CategorizationInfo categorizationInfo;

		private string categoryHeaderSortOverrides;
	}
}
