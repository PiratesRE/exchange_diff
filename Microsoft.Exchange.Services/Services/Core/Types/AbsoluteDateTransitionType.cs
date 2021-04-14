using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "AbsoluteDateTransition")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class AbsoluteDateTransitionType : TransitionType
	{
		public AbsoluteDateTransitionType()
		{
		}

		public AbsoluteDateTransitionType(TransitionTargetType to, DateTime dateTime) : base(to)
		{
			this.DateTime = dateTime;
		}

		[IgnoreDataMember]
		public DateTime DateTime { get; set; }

		[DataMember(Name = "DateTime", IsRequired = false, EmitDefaultValue = false)]
		[DateTimeString]
		[XmlIgnore]
		public string DateTimeString
		{
			get
			{
				ExDateTime dateTime = (ExDateTime)this.DateTime;
				return ExDateTimeConverter.ToOffsetXsdDateTime(dateTime, dateTime.TimeZone);
			}
			set
			{
				this.DateTime = (DateTime)ExDateTimeConverter.Parse(value);
			}
		}
	}
}
