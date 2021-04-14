using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "modality")]
	internal class Modality : Resource
	{
		public Modality() : base("no_uri")
		{
		}

		public Modality(string selfUri) : base(selfUri)
		{
		}

		[DataMember(Name = "type", EmitDefaultValue = false)]
		public ModalityType Type
		{
			get
			{
				return base.GetValue<ModalityType>("type");
			}
			set
			{
				base.SetValue<ModalityType>("type", value);
			}
		}

		public bool Equals(Modality other)
		{
			return other != null && this.Type == other.Type;
		}

		public override bool Equals(object obj)
		{
			Modality modality = obj as Modality;
			return modality != null && this.Type == modality.Type;
		}

		public override int GetHashCode()
		{
			return this.Type.GetHashCode();
		}
	}
}
