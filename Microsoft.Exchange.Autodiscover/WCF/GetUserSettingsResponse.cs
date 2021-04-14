using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[DataContract(Name = "GetUserSettingsResponse", Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	public class GetUserSettingsResponse : AutodiscoverResponse
	{
		public GetUserSettingsResponse()
		{
			this.UserResponses = new List<UserResponse>();
		}

		[DataMember(Name = "UserResponses")]
		public List<UserResponse> UserResponses { get; set; }
	}
}
