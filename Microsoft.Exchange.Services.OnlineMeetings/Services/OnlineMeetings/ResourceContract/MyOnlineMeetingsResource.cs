using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[Post(typeof(OnlineMeetingInput), typeof(OnlineMeetingResource))]
	[Get(typeof(MyOnlineMeetingsResource))]
	[CollectionDataContract(Name = "myOnlineMeetings")]
	internal class MyOnlineMeetingsResource : Resource
	{
		public MyOnlineMeetingsResource(string selfUri) : base(selfUri)
		{
		}

		public ResourceCollection<OnlineMeetingResource> MyOnlineMeetings
		{
			get
			{
				return base.GetValue<ResourceCollection<OnlineMeetingResource>>("myOnlineMeeting");
			}
			set
			{
				base.SetValue<ResourceCollection<OnlineMeetingResource>>("myOnlineMeeting", value);
			}
		}

		public ResourceCollection<AssignedOnlineMeetingResource> AssignedOnlineMeetings
		{
			get
			{
				return base.GetValue<ResourceCollection<AssignedOnlineMeetingResource>>("myAssignedOnlineMeeting");
			}
			set
			{
				base.SetValue<ResourceCollection<AssignedOnlineMeetingResource>>("myAssignedOnlineMeeting", value);
			}
		}

		public const string Token = "myOnlineMeetings";
	}
}
