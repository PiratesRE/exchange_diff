using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlInclude(typeof(RecurringDayTransitionType))]
	[XmlInclude(typeof(RecurringDateTransitionType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[KnownType(typeof(RecurringDateTransitionType))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[KnownType(typeof(RecurringDayTransitionType))]
	[Serializable]
	public abstract class RecurringTimeTransitionType : TransitionType
	{
		public RecurringTimeTransitionType()
		{
		}

		public RecurringTimeTransitionType(TransitionTargetType to, string timeOffset, int month) : base(to)
		{
			this.TimeOffset = timeOffset;
			this.Month = month;
		}

		[XmlElement(DataType = "duration")]
		[DataMember(EmitDefaultValue = false, Order = 1)]
		public string TimeOffset { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 2)]
		public int Month { get; set; }
	}
}
