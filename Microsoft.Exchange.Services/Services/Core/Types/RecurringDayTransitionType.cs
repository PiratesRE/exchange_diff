using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "RecurringDayTransition")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class RecurringDayTransitionType : RecurringTimeTransitionType
	{
		public RecurringDayTransitionType()
		{
		}

		public RecurringDayTransitionType(TransitionTargetType to, string timeOffset, int month, string dayOfWeek, int occurrence) : base(to, timeOffset, month)
		{
			this.DayOfWeek = dayOfWeek;
			this.Occurrence = occurrence;
		}

		[DataMember(EmitDefaultValue = false, Order = 1)]
		public string DayOfWeek { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 2)]
		public int Occurrence { get; set; }
	}
}
