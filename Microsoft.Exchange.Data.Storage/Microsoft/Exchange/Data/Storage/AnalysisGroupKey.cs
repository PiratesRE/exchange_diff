using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AnalysisGroupKey : IEquatable<AnalysisGroupKey>
	{
		public AnalysisGroupKey(object[] item)
		{
			this.item = item;
			string str = this.Subject + "_#_";
			this.isCalendar = (this.CleanGlobalObjectId != null && this.CleanGlobalObjectId.Length > 0);
			if (this.isCalendar)
			{
				this.key = str + this.CleanGlobalObjectId.ToString();
				return;
			}
			this.key = str + this.ReceivedTime.ToString() + "_#_" + this.ItemClass;
		}

		public override string ToString()
		{
			return this.key;
		}

		public string Subject
		{
			get
			{
				return (this.item[0] as string) ?? string.Empty;
			}
		}

		public string ItemClass
		{
			get
			{
				return (this.item[3] as string) ?? string.Empty;
			}
		}

		public byte[] CleanGlobalObjectId
		{
			get
			{
				return this.item[2] as byte[];
			}
		}

		public ExDateTime ReceivedTime
		{
			get
			{
				if (!(this.item[1] is ExDateTime))
				{
					return ExDateTime.MinValue;
				}
				return (ExDateTime)this.item[1];
			}
		}

		public QueryFilter Filter
		{
			get
			{
				QueryFilter result;
				if (this.isCalendar)
				{
					QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.NormalizedSubject, this.Subject);
					result = new AndFilter(new QueryFilter[]
					{
						new ComparisonFilter(ComparisonOperator.Equal, InternalSchema.CleanGlobalObjectId, this.CleanGlobalObjectId),
						queryFilter
					});
				}
				else
				{
					QueryFilter queryFilter2 = new AndFilter(new QueryFilter[]
					{
						new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.NormalizedSubject, this.Subject),
						new TextFilter(StoreObjectSchema.ItemClass, this.ItemClass, MatchOptions.ExactPhrase, MatchFlags.IgnoreCase)
					});
					result = new AndFilter(new QueryFilter[]
					{
						new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.ReceivedTime, this.ReceivedTime),
						queryFilter2
					});
				}
				return result;
			}
		}

		public DefaultFolderType FolderToSearch()
		{
			if (this.isCalendar)
			{
				return DefaultFolderType.Calendar;
			}
			return DefaultFolderType.AllItems;
		}

		public bool Equals(AnalysisGroupKey other)
		{
			if (!this.Subject.Equals(other.Subject))
			{
				return false;
			}
			bool flag = this.CleanGlobalObjectId != null && this.CleanGlobalObjectId.Length > 0;
			bool flag2 = other.CleanGlobalObjectId != null && other.CleanGlobalObjectId.Length > 0;
			if (flag ^ flag2)
			{
				return false;
			}
			if (flag)
			{
				return this.CleanGlobalObjectId.Equals(other.CleanGlobalObjectId);
			}
			return ExDateTime.Compare(this.ReceivedTime, other.ReceivedTime) == 0 && this.ItemClass.Equals(other.ItemClass);
		}

		private object[] item;

		private string key;

		private bool isCalendar;
	}
}
