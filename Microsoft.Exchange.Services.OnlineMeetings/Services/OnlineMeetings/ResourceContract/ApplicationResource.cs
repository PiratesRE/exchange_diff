using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[Link("events", true, Rel = "next")]
	[Link("policies", true)]
	[DataContract(Name = "application")]
	[Parent("applications")]
	[Get(typeof(ApplicationResource))]
	[Link("batching")]
	[Delete]
	internal class ApplicationResource : Resource
	{
		public ApplicationResource(string selfUri) : base(selfUri)
		{
		}

		[DataMember(Name = "userAgent", EmitDefaultValue = false)]
		public string UserAgent
		{
			get
			{
				return base.GetValue<string>("userAgent");
			}
			set
			{
				base.SetValue<string>("userAgent", value);
			}
		}

		[DataMember(Name = "culture", EmitDefaultValue = false)]
		public string Culture
		{
			get
			{
				return base.GetValue<string>("culture");
			}
			set
			{
				base.SetValue<string>("culture", value);
			}
		}

		[DataMember(Name = "type", EmitDefaultValue = false)]
		public ApplicationType ApplicationType
		{
			get
			{
				return base.GetValue<ApplicationType>("type");
			}
			set
			{
				base.SetValue<ApplicationType>("type", value);
			}
		}

		[DataMember(Name = "onlineMeetings", EmitDefaultValue = false)]
		public MyMeetingsResource MyMeetings
		{
			get
			{
				return base.GetValue<MyMeetingsResource>("onlineMeetings");
			}
			set
			{
				base.SetValue<MyMeetingsResource>("onlineMeetings", value);
			}
		}

		public const string Token = "application";
	}
}
