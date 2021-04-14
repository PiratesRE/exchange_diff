using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.DataConverter;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "RecurringMasterItemIdRangesType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class RecurringMasterItemIdRanges : ItemId
	{
		[DataMember(IsRequired = false)]
		[XmlArrayItem("Range", IsNullable = false)]
		public RecurringMasterItemIdRanges.OccurrencesRange[] Ranges { get; set; }

		[XmlIgnore]
		[DataMember(IsRequired = false)]
		public RecurringMasterItemIdRanges.ExpandAroundDateOccurrenceRangeType ExpandAroundDateOccurrenceRange { get; set; }

		[XmlType(TypeName = "OccurrencesRangeType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[DataContract(Name = "OccurrencesRange", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
		[Serializable]
		public class OccurrencesRange
		{
			[XmlAttribute]
			[DataMember(IsRequired = false)]
			public int Count { get; set; }

			[DataMember(IsRequired = false)]
			[XmlAttribute]
			public bool CompareOriginalStartTime { get; set; }

			[DataMember(IsRequired = false)]
			[XmlAttribute]
			public string Start { get; set; }

			[DataMember(IsRequired = false)]
			[XmlAttribute]
			public string End { get; set; }

			[XmlIgnore]
			public ExDateTime StartExDateTime
			{
				get
				{
					if (string.IsNullOrEmpty(this.Start))
					{
						return ExDateTime.MinValue;
					}
					return ExDateTimeConverter.ParseTimeZoneRelated(this.Start, EWSSettings.RequestTimeZone);
				}
			}

			[XmlIgnore]
			public ExDateTime EndExDateTime
			{
				get
				{
					if (string.IsNullOrEmpty(this.End))
					{
						return ExDateTime.MaxValue;
					}
					return ExDateTimeConverter.ParseTimeZoneRelated(this.End, EWSSettings.RequestTimeZone);
				}
			}

			public OccurrencesRange()
			{
				this.Count = 1;
				this.CompareOriginalStartTime = false;
			}

			[XmlIgnore]
			public const int MaxCount = 732;
		}

		[DataContract(Name = "ExpandAroundDateOccurrenceRangeType", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
		public class ExpandAroundDateOccurrenceRangeType : RecurringMasterItemIdRanges.OccurrencesRange
		{
			[DataMember(IsRequired = true)]
			public string ExpandOccurrencesAroundDate { get; set; }

			public ExDateTime? ExpandOccurrencesAroundExDateTime
			{
				get
				{
					if (!string.IsNullOrEmpty(this.ExpandOccurrencesAroundDate))
					{
						return new ExDateTime?(ExDateTimeConverter.ParseTimeZoneRelated(this.ExpandOccurrencesAroundDate, EWSSettings.RequestTimeZone));
					}
					return new ExDateTime?(ExDateTime.Now);
				}
			}
		}
	}
}
