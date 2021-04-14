using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class TransitionTargetType
	{
		public TransitionTargetType()
		{
		}

		public TransitionTargetType(TransitionTargetKindType kind, string target)
		{
			this.Kind = kind;
			this.Value = target;
		}

		[XmlAttribute]
		[IgnoreDataMember]
		public TransitionTargetKindType Kind { get; set; }

		[DataMember(Name = "Kind", EmitDefaultValue = false, Order = 0)]
		[XmlIgnore]
		public string KindString
		{
			get
			{
				return this.Kind.ToString();
			}
			set
			{
				this.Kind = (TransitionTargetKindType)Enum.Parse(typeof(TransitionTargetKindType), value);
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 1)]
		[XmlText]
		public string Value { get; set; }
	}
}
