using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.Search
{
	[XmlType(TypeName = "IndexedPageFolderView", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class IndexedPageView : BasePagingType
	{
		[XmlAttribute]
		[DataMember]
		public int Offset { get; set; }

		[XmlAttribute(AttributeName = "BasePoint")]
		[IgnoreDataMember]
		public BasePagingType.PagingOrigin Origin { get; set; }

		[XmlIgnore]
		[DataMember(Name = "BasePoint", IsRequired = true)]
		public string OriginString
		{
			get
			{
				return EnumUtilities.ToString<BasePagingType.PagingOrigin>(this.Origin);
			}
			set
			{
				this.Origin = EnumUtilities.Parse<BasePagingType.PagingOrigin>(value);
			}
		}

		internal override BasePageResult CreatePageResult(IQueryResult queryResult, BaseQueryView view)
		{
			if (this.Origin == BasePagingType.PagingOrigin.End)
			{
				view.RetrievedLastItem = (this.startPoint == queryResult.EstimatedRowCount);
			}
			int indexedOffset;
			if (this.Origin == BasePagingType.PagingOrigin.Beginning)
			{
				indexedOffset = (base.RowsFetchedSpecified ? (base.RowsFetched + this.Offset) : queryResult.CurrentRow);
			}
			else
			{
				indexedOffset = this.startPoint;
			}
			return new IndexedPageResult(view, indexedOffset);
		}

		internal override void PositionResultSet(IQueryResult queryResult)
		{
			if (this.Offset < 0)
			{
				throw new InvalidIndexedPagingParametersException();
			}
			if (this.Origin == BasePagingType.PagingOrigin.Beginning)
			{
				if (this.Offset != 0)
				{
					queryResult.SeekToOffset((SeekReference)this.Origin, this.Offset);
				}
				this.startPoint = 0;
				return;
			}
			int num = queryResult.EstimatedRowCount - this.Offset - base.MaxRows;
			if (num < 0)
			{
				base.MaxRows += num;
			}
			queryResult.SeekToOffset((SeekReference)this.Origin, -this.Offset - base.MaxRows);
			this.startPoint = queryResult.EstimatedRowCount - queryResult.CurrentRow;
		}

		internal override bool BudgetInducedTruncationAllowed
		{
			get
			{
				return this.Origin == BasePagingType.PagingOrigin.Beginning;
			}
		}

		private int startPoint;
	}
}
