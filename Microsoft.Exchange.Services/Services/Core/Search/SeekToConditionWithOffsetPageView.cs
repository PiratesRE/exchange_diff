using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.Search
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class SeekToConditionWithOffsetPageView : SeekToConditionPageView
	{
		[DataMember]
		public int Offset { get; set; }

		internal override BasePageResult ApplyPostQueryPaging(IQueryResult queryResult)
		{
			throw new NotSupportedException("ApplyPostQueryPaging is not supported on SeekToConditionWithOffsetPageView.");
		}

		internal override BasePageResult CreatePageResult(IQueryResult queryResult, BaseQueryView view)
		{
			throw new NotSupportedException("ApplyPostQueryPaging is not supported on SeekToConditionWithOffsetPageView.");
		}

		internal override void PositionResultSet(IQueryResult queryResult)
		{
			throw new NotSupportedException("ApplyPostQueryPaging is not supported on SeekToConditionWithOffsetPageView.");
		}
	}
}
