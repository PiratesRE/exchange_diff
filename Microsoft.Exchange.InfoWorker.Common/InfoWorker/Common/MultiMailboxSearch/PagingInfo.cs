using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal class PagingInfo
	{
		public PagingInfo(List<PropertyDefinition> data, SortBy sort, int pageSize, PageDirection direction, ReferenceItem referenceItem, ExTimeZone timeZone) : this(data, sort, pageSize, direction, referenceItem, timeZone, false)
		{
		}

		public PagingInfo(List<PropertyDefinition> data, SortBy sort, int pageSize, PageDirection direction, ReferenceItem referenceItem, ExTimeZone timeZone, bool excludeDuplicatesItems) : this(data, sort, pageSize, direction, referenceItem, timeZone, excludeDuplicatesItems, PreviewItemBaseShape.Default, null)
		{
		}

		public PagingInfo(List<PropertyDefinition> data, SortBy sort, int pageSize, PageDirection direction, ReferenceItem referenceItem, ExTimeZone timeZone, bool excludeDuplicatesItems, PreviewItemBaseShape baseShape, List<ExtendedPropertyInfo> additionalProperties)
		{
			Util.ThrowOnNull(data, "data");
			Util.ThrowOnNull(sort, "sort");
			if (pageSize == 0)
			{
				throw new ArgumentException("Page size cannot be 0");
			}
			if (referenceItem == null && direction == PageDirection.Previous)
			{
				throw new ArgumentException("PagingInfo: Have to provide sort column value to view previous page");
			}
			if (!PagingInfo.ValidateDataColumns(data))
			{
				throw new ArgumentException("PagingInfo: Invalid data columns");
			}
			this.originalDataColumns = data;
			this.dataColumns = new List<PropertyDefinition>(data);
			this.sort = sort;
			this.referenceItem = referenceItem;
			this.direction = direction;
			this.pageSize = pageSize;
			this.timeZone = timeZone;
			this.excludeDuplicateItems = excludeDuplicatesItems;
			this.baseShape = baseShape;
			this.additionalProperties = additionalProperties;
			if (!this.originalDataColumns.Contains(sort.ColumnDefinition))
			{
				this.dataColumns.Add(sort.ColumnDefinition);
			}
			foreach (PropertyDefinition item in PagingInfo.RequiredDataPropertiesFromStore)
			{
				if (!this.originalDataColumns.Contains(item))
				{
					this.dataColumns.Add(item);
				}
			}
			if (this.additionalProperties != null)
			{
				foreach (ExtendedPropertyInfo extendedPropertyInfo in this.additionalProperties)
				{
					if (extendedPropertyInfo.XsoPropertyDefinition != null && !this.dataColumns.Contains(extendedPropertyInfo.XsoPropertyDefinition))
					{
						this.dataColumns.Add(extendedPropertyInfo.XsoPropertyDefinition);
					}
				}
			}
			this.sorts = new SortBy[]
			{
				sort,
				new SortBy(ItemSchema.DocumentId, SortOrder.Ascending)
			};
		}

		public int PageSize
		{
			get
			{
				return this.pageSize;
			}
		}

		public SortBy[] Sorts
		{
			get
			{
				return this.sorts;
			}
		}

		public List<PropertyDefinition> DataColumns
		{
			get
			{
				return this.dataColumns;
			}
		}

		public List<PropertyDefinition> OriginalDataColumns
		{
			get
			{
				return this.originalDataColumns;
			}
		}

		public int DataColumnCount
		{
			get
			{
				return this.dataColumns.Count + 1;
			}
		}

		public SortBy SortBy
		{
			get
			{
				return this.sort;
			}
		}

		public PropertyDefinition SortColumn
		{
			get
			{
				return this.sort.ColumnDefinition;
			}
		}

		public bool AscendingSort
		{
			get
			{
				return this.sort.SortOrder == SortOrder.Ascending;
			}
		}

		public ReferenceItem SortValue
		{
			get
			{
				return this.referenceItem;
			}
		}

		public PageDirection Direction
		{
			get
			{
				return this.direction;
			}
		}

		public ExTimeZone TimeZone
		{
			get
			{
				return this.timeZone;
			}
		}

		public bool ExcludeDuplicates
		{
			get
			{
				return this.excludeDuplicateItems;
			}
		}

		public PreviewItemBaseShape BaseShape
		{
			get
			{
				return this.baseShape;
			}
		}

		public List<ExtendedPropertyInfo> AdditionalProperties
		{
			get
			{
				return this.additionalProperties;
			}
		}

		public string OriginalSortByReference { get; set; }

		public QueryFilter GetPagingFilter(MailboxInfo mailbox)
		{
			if (this.referenceItem == null || this.referenceItem.SortColumnValue == null)
			{
				return null;
			}
			if (this.direction == PageDirection.Next)
			{
				if (this.sort.SortOrder == SortOrder.Ascending)
				{
					if (this.referenceItem.MailboxIdHash < mailbox.MailboxGuid.GetHashCode())
					{
						return new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, this.sort.ColumnDefinition, this.referenceItem.SortColumnValue);
					}
					if (this.referenceItem.MailboxIdHash > mailbox.MailboxGuid.GetHashCode())
					{
						return new ComparisonFilter(ComparisonOperator.GreaterThan, this.sort.ColumnDefinition, this.referenceItem.SortColumnValue);
					}
					ComparisonFilter comparisonFilter = new ComparisonFilter(ComparisonOperator.GreaterThan, this.sort.ColumnDefinition, this.referenceItem.SortColumnValue);
					ComparisonFilter comparisonFilter2 = new ComparisonFilter(ComparisonOperator.Equal, this.sort.ColumnDefinition, this.referenceItem.SortColumnValue);
					ComparisonFilter comparisonFilter3 = new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ItemSchema.DocumentId, this.referenceItem.DocId);
					AndFilter andFilter = new AndFilter(new QueryFilter[]
					{
						comparisonFilter2,
						comparisonFilter3
					});
					return new OrFilter(new QueryFilter[]
					{
						comparisonFilter,
						andFilter
					});
				}
				else
				{
					if (this.referenceItem.MailboxIdHash < mailbox.MailboxGuid.GetHashCode())
					{
						return new ComparisonFilter(ComparisonOperator.LessThanOrEqual, this.sort.ColumnDefinition, this.referenceItem.SortColumnValue);
					}
					if (this.referenceItem.MailboxIdHash > mailbox.MailboxGuid.GetHashCode())
					{
						return new ComparisonFilter(ComparisonOperator.LessThan, this.sort.ColumnDefinition, this.referenceItem.SortColumnValue);
					}
					ComparisonFilter comparisonFilter4 = new ComparisonFilter(ComparisonOperator.LessThan, this.sort.ColumnDefinition, this.referenceItem.SortColumnValue);
					ComparisonFilter comparisonFilter5 = new ComparisonFilter(ComparisonOperator.Equal, this.sort.ColumnDefinition, this.referenceItem.SortColumnValue);
					ComparisonFilter comparisonFilter6 = new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ItemSchema.DocumentId, this.referenceItem.DocId);
					AndFilter andFilter2 = new AndFilter(new QueryFilter[]
					{
						comparisonFilter5,
						comparisonFilter6
					});
					return new OrFilter(new QueryFilter[]
					{
						comparisonFilter4,
						andFilter2
					});
				}
			}
			else
			{
				if (this.direction != PageDirection.Previous)
				{
					return null;
				}
				if (this.sort.SortOrder == SortOrder.Ascending)
				{
					if (this.referenceItem.MailboxIdHash < mailbox.MailboxGuid.GetHashCode())
					{
						return new ComparisonFilter(ComparisonOperator.LessThan, this.sort.ColumnDefinition, this.referenceItem.SortColumnValue);
					}
					if (this.referenceItem.MailboxIdHash > mailbox.MailboxGuid.GetHashCode())
					{
						return new ComparisonFilter(ComparisonOperator.LessThanOrEqual, this.sort.ColumnDefinition, this.referenceItem.SortColumnValue);
					}
					ComparisonFilter comparisonFilter7 = new ComparisonFilter(ComparisonOperator.LessThan, this.sort.ColumnDefinition, this.referenceItem.SortColumnValue);
					ComparisonFilter comparisonFilter8 = new ComparisonFilter(ComparisonOperator.Equal, this.sort.ColumnDefinition, this.referenceItem.SortColumnValue);
					ComparisonFilter comparisonFilter9 = new ComparisonFilter(ComparisonOperator.LessThanOrEqual, ItemSchema.DocumentId, this.referenceItem.DocId);
					AndFilter andFilter3 = new AndFilter(new QueryFilter[]
					{
						comparisonFilter8,
						comparisonFilter9
					});
					return new OrFilter(new QueryFilter[]
					{
						comparisonFilter7,
						andFilter3
					});
				}
				else
				{
					if (this.referenceItem.MailboxIdHash < mailbox.MailboxGuid.GetHashCode())
					{
						return new ComparisonFilter(ComparisonOperator.GreaterThan, this.sort.ColumnDefinition, this.referenceItem.SortColumnValue);
					}
					if (this.referenceItem.MailboxIdHash > mailbox.MailboxGuid.GetHashCode())
					{
						return new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, this.sort.ColumnDefinition, this.referenceItem.SortColumnValue);
					}
					ComparisonFilter comparisonFilter10 = new ComparisonFilter(ComparisonOperator.GreaterThan, this.sort.ColumnDefinition, this.referenceItem.SortColumnValue);
					ComparisonFilter comparisonFilter11 = new ComparisonFilter(ComparisonOperator.Equal, this.sort.ColumnDefinition, this.referenceItem.SortColumnValue);
					ComparisonFilter comparisonFilter12 = new ComparisonFilter(ComparisonOperator.LessThanOrEqual, ItemSchema.DocumentId, this.referenceItem.DocId);
					AndFilter andFilter4 = new AndFilter(new QueryFilter[]
					{
						comparisonFilter11,
						comparisonFilter12
					});
					return new OrFilter(new QueryFilter[]
					{
						comparisonFilter10,
						andFilter4
					});
				}
			}
		}

		public override bool Equals(object obj)
		{
			PagingInfo pagingInfo = obj as PagingInfo;
			return pagingInfo != null && (this.PageSize == pagingInfo.PageSize && this.SortColumn.Equals(pagingInfo.SortColumn) && this.AscendingSort == pagingInfo.AscendingSort && this.direction == pagingInfo.Direction && PagingInfo.SameSet(this.DataColumns, pagingInfo.DataColumns) && (this.referenceItem == null || this.referenceItem.Equals(pagingInfo.SortValue))) && (this.referenceItem != null || pagingInfo.SortValue == null);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private static bool SameSet(List<PropertyDefinition> left, List<PropertyDefinition> right)
		{
			if (left.Count != right.Count)
			{
				return false;
			}
			for (int i = 0; i < left.Count; i++)
			{
				if (!left[i].Equals(right[i]))
				{
					return false;
				}
			}
			return true;
		}

		private static bool ValidateDataColumns(List<PropertyDefinition> data)
		{
			PagingInfo.<>c__DisplayClass1 CS$<>8__locals1 = new PagingInfo.<>c__DisplayClass1();
			CS$<>8__locals1.data = data;
			if (CS$<>8__locals1.data == null)
			{
				return false;
			}
			int i;
			for (i = 0; i < CS$<>8__locals1.data.Count; i++)
			{
				if (!Array.Exists<PropertyDefinition>(PagingInfo.allowedDataProperties, (PropertyDefinition obj) => CS$<>8__locals1.data[i].Equals(obj)))
				{
					return false;
				}
			}
			return true;
		}

		private static PropertyDefinition[] allowedDataProperties = new PropertyDefinition[]
		{
			StoreObjectSchema.ParentItemId,
			ItemSchema.Id,
			StoreObjectSchema.ItemClass,
			ItemSchema.HasAttachment,
			ItemSchema.Size,
			ItemSchema.BodyTag,
			ItemSchema.InternetMessageId,
			ItemSchema.ConversationId,
			ItemSchema.ConversationTopic,
			ItemSchema.ConversationIndexTracking,
			ItemSchema.Subject,
			MessageItemSchema.IsRead,
			ItemSchema.SentTime,
			ItemSchema.ReceivedTime,
			MessageItemSchema.SenderDisplayName,
			MessageItemSchema.SenderSmtpAddress,
			ItemSchema.Importance,
			ItemSchema.Categories,
			ItemSchema.DisplayCc,
			ItemSchema.DisplayBcc,
			ItemSchema.DisplayTo,
			StoreObjectSchema.CreationTime
		};

		private static readonly PropertyDefinition[] RequiredDataPropertiesFromStore = new PropertyDefinition[]
		{
			StoreObjectSchema.ParentItemId,
			ItemSchema.Id,
			StoreObjectSchema.ItemClass,
			ItemSchema.Size,
			ItemSchema.DocumentId,
			ItemSchema.InternetMessageId,
			ItemSchema.ConversationId,
			ItemSchema.ConversationTopic,
			ItemSchema.BodyTag
		};

		private readonly List<PropertyDefinition> originalDataColumns;

		private readonly List<PropertyDefinition> dataColumns;

		private readonly SortBy sort;

		private readonly SortBy[] sorts;

		private readonly int pageSize;

		private readonly ReferenceItem referenceItem;

		private readonly PageDirection direction;

		private readonly ExTimeZone timeZone;

		private readonly bool excludeDuplicateItems;

		private readonly PreviewItemBaseShape baseShape;

		private readonly List<ExtendedPropertyInfo> additionalProperties;
	}
}
