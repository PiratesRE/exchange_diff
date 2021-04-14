using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.Search
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "FractionalPageFolderView", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class FractionalPageView : BasePagingType
	{
		[XmlAttribute]
		[DataMember(Name = "Numerator", IsRequired = true)]
		public int Numerator { get; set; }

		[DataMember(Name = "Denominator", IsRequired = true)]
		[XmlAttribute]
		public int Denominator { get; set; }

		internal override BasePageResult CreatePageResult(IQueryResult queryResult, BaseQueryView view)
		{
			return new FractionalPageResult(view, queryResult.CurrentRow, queryResult.EstimatedRowCount);
		}

		internal override void PositionResultSet(IQueryResult queryResult)
		{
			if (this.Numerator > this.Denominator || this.Numerator < 0 || this.Denominator <= 0)
			{
				throw new InvalidFractionalPagingParametersException();
			}
			int offset = (int)((float)this.Numerator / (float)this.Denominator * (float)queryResult.EstimatedRowCount);
			queryResult.SeekToOffset(SeekReference.OriginBeginning, offset);
		}

		internal override bool BudgetInducedTruncationAllowed
		{
			get
			{
				return true;
			}
		}
	}
}
