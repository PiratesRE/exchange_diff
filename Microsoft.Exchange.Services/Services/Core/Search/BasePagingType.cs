using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.Search
{
	[XmlInclude(typeof(IndexedPageView))]
	[KnownType(typeof(FractionalPageView))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[KnownType(typeof(CalendarPageView))]
	[XmlInclude(typeof(SeekToConditionPageView))]
	[KnownType(typeof(SeekToConditionWithOffsetPageView))]
	[XmlInclude(typeof(CalendarPageView))]
	[XmlInclude(typeof(ContactsPageView))]
	[XmlInclude(typeof(FractionalPageView))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[KnownType(typeof(ContactsPageView))]
	[KnownType(typeof(IndexedPageView))]
	[KnownType(typeof(SeekToConditionPageView))]
	[Serializable]
	public abstract class BasePagingType
	{
		[XmlAttribute(AttributeName = "MaxEntriesReturned")]
		[DataMember(Name = "MaxEntriesReturned", IsRequired = false)]
		public int MaxRows
		{
			get
			{
				if (!this.MaxRowsSpecified)
				{
					return int.MaxValue;
				}
				return this.maxRows;
			}
			set
			{
				this.maxRowsSpecified = true;
				this.maxRows = value;
			}
		}

		[XmlIgnore]
		public bool MaxRowsSpecified
		{
			get
			{
				return this.maxRowsSpecified;
			}
			set
			{
				this.maxRowsSpecified = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public int RowsFetched
		{
			get
			{
				return this.rowsFetched;
			}
			set
			{
				this.rowsFetchedSpecified = true;
				this.rowsFetched = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool RowsFetchedSpecified
		{
			get
			{
				return this.rowsFetchedSpecified;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool LoadPartialPageRows { get; set; }

		[IgnoreDataMember]
		[XmlIgnore]
		public bool NoRowCountRetrieval { get; set; }

		internal static BasePageResult ApplyPostQueryPaging(IQueryResult queryResult, BasePagingType paging)
		{
			if (paging == null)
			{
				return new BasePageResult(new NormalQueryView(queryResult, int.MaxValue, null));
			}
			return paging.ApplyPostQueryPaging(queryResult);
		}

		internal static BasePageResult ApplyPostQueryGroupedPaging(GroupedQueryResult groupedResult, BasePagingType paging, int groupByPropDefIndex)
		{
			if (paging == null)
			{
				GroupedQueryView view = new GroupedQueryView(groupedResult, int.MaxValue, groupByPropDefIndex, paging);
				return new BasePageResult(view);
			}
			return paging.ApplyPostQueryGroupedPaging(groupedResult, groupByPropDefIndex);
		}

		internal static QueryFilter ApplyQueryAppend(QueryFilter filter, BasePagingType paging)
		{
			if (paging == null)
			{
				return filter;
			}
			return paging.ApplyQueryAppend(filter);
		}

		internal static void Validate(BasePagingType paging)
		{
			if (paging == null)
			{
				return;
			}
			if (paging.MaxRowsSpecified && paging.MaxRows <= 0)
			{
				throw new InvalidPagingMaxRowsException();
			}
		}

		internal virtual BasePageResult ApplyPostQueryPaging(IQueryResult queryResult)
		{
			this.PositionResultSet(queryResult);
			return this.CreatePageResult(queryResult, new NormalQueryView(queryResult, this.MaxRows, this));
		}

		internal virtual BasePageResult ApplyPostQueryGroupedPaging(GroupedQueryResult groupedQuery, int groupByPropDefIndex)
		{
			this.PositionResultSet(groupedQuery);
			GroupedQueryView view = new GroupedQueryView(groupedQuery, this.MaxRows, groupByPropDefIndex, this);
			return this.CreatePageResult(groupedQuery, view);
		}

		internal virtual QueryFilter ApplyQueryAppend(QueryFilter filter)
		{
			return filter;
		}

		internal virtual void PositionResultSet(IQueryResult queryResult)
		{
		}

		internal virtual BasePageResult CreatePageResult(IQueryResult queryResult, BaseQueryView view)
		{
			return new BasePageResult(view);
		}

		internal virtual bool BudgetInducedTruncationAllowed
		{
			get
			{
				return false;
			}
		}

		private int maxRows;

		private bool maxRowsSpecified;

		private int rowsFetched;

		private bool rowsFetchedSpecified;

		[XmlType(TypeName = "BasePointType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[Serializable]
		public enum PagingOrigin
		{
			Beginning,
			End = 6
		}
	}
}
