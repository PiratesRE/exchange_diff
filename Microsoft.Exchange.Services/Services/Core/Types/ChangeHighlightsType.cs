using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", TypeName = "ChangeHighlightsType")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "ChangeHighlights")]
	[Serializable]
	public class ChangeHighlightsType
	{
		[DataMember(EmitDefaultValue = false, Order = 1)]
		public bool HasLocationChanged { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 2)]
		public string Location { get; set; }

		[XmlIgnore]
		[IgnoreDataMember]
		public bool LocationSpecified
		{
			get
			{
				return this.HasLocationChanged;
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 3)]
		public bool HasStartTimeChanged { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 4)]
		[DateTimeString]
		public string Start { get; set; }

		[XmlIgnore]
		[IgnoreDataMember]
		public bool StartSpecified
		{
			get
			{
				return this.HasStartTimeChanged;
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 5)]
		public bool HasEndTimeChanged { get; set; }

		[DateTimeString]
		[DataMember(EmitDefaultValue = false, Order = 6)]
		public string End { get; set; }

		[XmlIgnore]
		[IgnoreDataMember]
		public bool EndSpecified
		{
			get
			{
				return this.HasEndTimeChanged;
			}
			set
			{
			}
		}
	}
}
