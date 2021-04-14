using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[XmlType(TypeName = "InstantSearchPayloadType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class InstantSearchPayloadType
	{
		public InstantSearchPayloadType()
		{
		}

		internal InstantSearchPayloadType(string searchSessionId, long searchRequestId, InstantSearchResultType resultDataType, SearchPerfMarkerContainer perfMarkerContainer)
		{
			this.SearchSessionId = searchSessionId;
			this.SearchRequestId = searchRequestId;
			this.ResultType = resultDataType;
			this.SearchPerfMarkerContainer = perfMarkerContainer;
		}

		[DataMember(IsRequired = true)]
		public string SearchSessionId { get; set; }

		[DataMember(IsRequired = true)]
		public long SearchRequestId { get; set; }

		[DataMember(IsRequired = true)]
		public InstantSearchResultType ResultType { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public bool QueryProcessingComplete { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public QueryStatisticsType QueryStatistics { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public ItemType[] Items { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public EwsCalendarItemType[] CalendarItems { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public SearchSuggestionType[] SearchSuggestions { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public ConversationType[] Conversations { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public Persona[] PersonaItems { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public RefinerDataType[] RefinerData { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public string[] Errors { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public string[] SearchTerms
		{
			get
			{
				return this.searchTerms;
			}
			set
			{
				this.searchTerms = value;
				this.ResultType |= InstantSearchResultType.SearchTerms;
			}
		}

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public InstantSearchPerfMarkerType[] PerfMarkers { get; set; }

		[OnSerializing]
		private void StampSerializationTime(StreamingContext context)
		{
			this.SearchPerfMarkerContainer.SetPerfMarker(InstantSearchPerfKey.NotificationSerializationTime);
			this.PerfMarkers = this.SearchPerfMarkerContainer.GetMarkerSnapshot();
		}

		[XmlIgnore]
		[IgnoreDataMember]
		internal SearchPerfMarkerContainer SearchPerfMarkerContainer { get; private set; }

		private string[] searchTerms;
	}
}
