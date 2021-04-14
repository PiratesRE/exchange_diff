using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ArrayOfTransitionsType
	{
		public ArrayOfTransitionsType()
		{
		}

		public ArrayOfTransitionsType(bool transitionsGroup, string id, TransitionType[] transitions) : this(transitionsGroup)
		{
			this.Id = id;
			this.Transition = transitions;
		}

		internal ArrayOfTransitionsType(bool transitionsGroup)
		{
			this.name = (transitionsGroup ? "TransitionsGroup" : "Transitions");
		}

		[XmlElement("RecurringDayTransition", typeof(RecurringDayTransitionType))]
		[XmlElement("RecurringDateTransition", typeof(RecurringDateTransitionType))]
		[XmlElement("Transition", typeof(TransitionType))]
		[DataMember(EmitDefaultValue = false, Order = 1)]
		[XmlElement("AbsoluteDateTransition", typeof(AbsoluteDateTransitionType))]
		public TransitionType[] Transition { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 0)]
		[XmlAttribute]
		public string Id { get; set; }

		[IgnoreDataMember]
		[XmlIgnore]
		internal string Name
		{
			get
			{
				return this.name;
			}
		}

		internal void Add(TransitionType transition)
		{
			if (this.Transition != null)
			{
				TransitionType[] array = new TransitionType[this.Transition.Length + 1];
				this.Transition.CopyTo(array, 0);
				array[this.Transition.Length] = transition;
				this.Transition = array;
				return;
			}
			this.Transition = new TransitionType[]
			{
				transition
			};
		}

		private const string TransitionsGroupString = "TransitionsGroup";

		private const string TransitionsString = "Transitions";

		private readonly string name;
	}
}
