using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlInclude(typeof(AbsoluteDateTransitionType))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "Transition")]
	[XmlInclude(typeof(RecurringDateTransitionType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[KnownType(typeof(AbsoluteDateTransitionType))]
	[XmlInclude(typeof(RecurringTimeTransitionType))]
	[XmlInclude(typeof(RecurringDayTransitionType))]
	[KnownType(typeof(RecurringTimeTransitionType))]
	[KnownType(typeof(RecurringDayTransitionType))]
	[KnownType(typeof(RecurringDateTransitionType))]
	[Serializable]
	public class TransitionType
	{
		public TransitionType()
		{
		}

		public TransitionType(TransitionTargetType to)
		{
			this.To = to;
		}

		[DataMember(EmitDefaultValue = false)]
		public TransitionTargetType To { get; set; }
	}
}
