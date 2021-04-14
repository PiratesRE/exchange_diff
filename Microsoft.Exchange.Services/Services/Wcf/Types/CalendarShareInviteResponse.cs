using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract]
	public class CalendarShareInviteResponse : CalendarActionResponse
	{
		public CalendarShareInviteResponse()
		{
			this.success = new List<string>();
			this.failure = new List<ShareInviteFailure>();
		}

		public void AddSucessResponse(string emailAddress)
		{
			this.success.Add(emailAddress);
		}

		public void AddFailureResponse(string emailAddress, string message)
		{
			this.failure.Add(new ShareInviteFailure
			{
				Recipient = emailAddress,
				Error = message
			});
		}

		[DataMember]
		public string[] SuccessResponses
		{
			get
			{
				return this.success.ToArray();
			}
		}

		[DataMember]
		public ShareInviteFailure[] FailureResponses
		{
			get
			{
				return this.failure.ToArray();
			}
		}

		private readonly List<string> success;

		private readonly List<ShareInviteFailure> failure;
	}
}
