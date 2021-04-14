using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.Search
{
	[XmlType(TypeName = "SeekToConditionPageViewType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class SeekToConditionPageView : BasePagingType
	{
		[XmlAttribute(AttributeName = "BasePoint")]
		[IgnoreDataMember]
		public BasePagingType.PagingOrigin Origin { get; set; }

		[DataMember(Name = "BasePoint", IsRequired = true)]
		[XmlIgnore]
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

		[DataMember(Name = "Condition", IsRequired = true)]
		[XmlElement("Condition", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public RestrictionType Condition { get; set; }

		internal override BasePageResult CreatePageResult(IQueryResult queryResult, BaseQueryView view)
		{
			if (this.Origin == BasePagingType.PagingOrigin.End)
			{
				view.RetrievedLastItem = this.isLastPage;
			}
			return new IndexedPageResult(view, (this.Origin == BasePagingType.PagingOrigin.Beginning) ? queryResult.CurrentRow : (queryResult.EstimatedRowCount - queryResult.CurrentRow));
		}

		internal override void PositionResultSet(IQueryResult queryResult)
		{
			ServiceObjectToFilterConverter serviceObjectToFilterConverter = new ServiceObjectToFilterConverter();
			bool flag = queryResult.SeekToCondition((SeekReference)this.Origin, serviceObjectToFilterConverter.Convert(this.Condition.Item), SeekToConditionFlags.AllowExtendedSeekReferences);
			if (this.Origin == BasePagingType.PagingOrigin.End)
			{
				if (flag)
				{
					base.MaxRows = Math.Min(base.MaxRows, queryResult.CurrentRow + 1);
					queryResult.SeekToOffset(SeekReference.OriginCurrent, -(base.MaxRows - 1));
				}
				else
				{
					queryResult.SeekToOffset(SeekReference.OriginBeginning, queryResult.EstimatedRowCount);
				}
				this.isLastPage = (queryResult.CurrentRow == 0);
			}
		}

		internal override bool BudgetInducedTruncationAllowed
		{
			get
			{
				return this.Origin == BasePagingType.PagingOrigin.Beginning;
			}
		}

		private bool isLastPage;
	}
}
