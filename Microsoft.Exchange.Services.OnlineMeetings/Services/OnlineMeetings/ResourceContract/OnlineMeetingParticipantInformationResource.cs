using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "ConferenceParticipantInformation")]
	internal class OnlineMeetingParticipantInformationResource : Resource
	{
		public OnlineMeetingParticipantInformationResource(string selfUri) : base(selfUri)
		{
		}

		[DataMember(Name = "Role", EmitDefaultValue = false)]
		public ConferencingRole Role
		{
			get
			{
				return base.GetValue<ConferencingRole>("Role");
			}
			set
			{
				base.SetValue<ConferencingRole>("Role", value);
			}
		}

		[DataMember(Name = "Uri", EmitDefaultValue = false)]
		public string Uri
		{
			get
			{
				return base.GetValue<string>("Uri");
			}
			set
			{
				base.SetValue<string>("Uri", value);
			}
		}

		public const string Token = "onlinemeetingparticipantinformation";
	}
}
