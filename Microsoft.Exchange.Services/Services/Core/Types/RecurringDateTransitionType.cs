using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "RecurringDateTransition")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class RecurringDateTransitionType : RecurringTimeTransitionType
	{
		public RecurringDateTransitionType()
		{
		}

		public RecurringDateTransitionType(TransitionTargetType to, string timeOffset, int month, int day) : base(to, timeOffset, month)
		{
			this.Day = day;
		}

		[DataMember(EmitDefaultValue = false)]
		public int Day { get; set; }
	}
}
