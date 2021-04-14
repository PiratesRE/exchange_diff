using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "conferenceResource")]
	[Parent("conversation")]
	[Get(typeof(ConversationConferenceResource))]
	internal class ConversationConferenceResource : MeetingResource
	{
		public ConversationConferenceResource(string selfUri) : base(selfUri)
		{
		}

		[DataMember(Name = "organizerName", EmitDefaultValue = false)]
		public string OrganizerName
		{
			get
			{
				return base.GetValue<string>("organizerName");
			}
			set
			{
				base.SetValue<string>("organizerName", value);
			}
		}

		[DataMember(Name = "disclaimerBody", EmitDefaultValue = false)]
		public string Disclaimer
		{
			get
			{
				return base.GetValue<string>("disclaimerBody");
			}
			set
			{
				base.SetValue<string>("disclaimerBody", value);
			}
		}

		[DataMember(Name = "disclaimerTitle", EmitDefaultValue = false)]
		public string DisclaimerTitle
		{
			get
			{
				return base.GetValue<string>("disclaimerTitle");
			}
			set
			{
				base.SetValue<string>("disclaimerTitle", value);
			}
		}

		[DataMember(Name = "hostingNetwork", EmitDefaultValue = false)]
		public string HostingNetwork
		{
			get
			{
				return base.GetValue<string>("hostingNetwork");
			}
			set
			{
				base.SetValue<string>("hostingNetwork", value);
			}
		}

		public const string Token = "onlineMeeting";
	}
}
