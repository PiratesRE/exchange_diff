using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public sealed class SearchPathSnapshotType
	{
		public SearchPathSnapshotType()
		{
		}

		internal SearchPathSnapshotType(QueryOptionsType queryOptionType, SearchPerfMarkerContainer perfMarkerContainer)
		{
			this.QueryOptions = queryOptionType;
			this.PerfMarkerContainer = perfMarkerContainer;
		}

		[DataMember]
		public QueryOptionsType QueryOptions { get; set; }

		[DataMember]
		public InstantSearchPerfMarkerType[] PerfMarkers
		{
			get
			{
				return this.PerfMarkerContainer.MarkerCollection.ToArray();
			}
		}

		internal SearchPerfMarkerContainer PerfMarkerContainer { get; set; }
	}
}
