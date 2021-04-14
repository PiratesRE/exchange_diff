using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "PhoneNormalizationResultResource")]
	[Get(typeof(PhoneNormalizationResultResource))]
	[Parent("Communication")]
	internal class PhoneNormalizationResultResource : Resource
	{
		public PhoneNormalizationResultResource() : base(string.Empty)
		{
		}

		[DataMember(Name = "NormalizedNumber", EmitDefaultValue = false)]
		public string NormalizedNumber
		{
			get
			{
				return base.GetValue<string>("NormalizedNumber");
			}
			set
			{
				base.SetValue<string>("NormalizedNumber", value);
			}
		}
	}
}
