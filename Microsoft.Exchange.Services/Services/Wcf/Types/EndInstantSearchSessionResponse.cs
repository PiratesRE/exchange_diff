using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "EndInstantSearchSessionResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public sealed class EndInstantSearchSessionResponse : ResponseMessage
	{
		public EndInstantSearchSessionResponse()
		{
		}

		public EndInstantSearchSessionResponse(Dictionary<long, List<SearchPathSnapshotType>> dataDictionary)
		{
			int count = dataDictionary.Count;
			this.RequestIds = new long[count];
			this.SearchPathSnapshots = new SearchPathSnapshotType[count][];
			int num = 0;
			foreach (long num2 in dataDictionary.Keys)
			{
				this.RequestIds[num] = num2;
				this.SearchPathSnapshots[num] = dataDictionary[num2].ToArray();
				num++;
			}
		}

		[XmlIgnore]
		[DataMember]
		public long[] RequestIds { get; set; }

		[XmlIgnore]
		[DataMember]
		public SearchPathSnapshotType[][] SearchPathSnapshots { get; set; }
	}
}
