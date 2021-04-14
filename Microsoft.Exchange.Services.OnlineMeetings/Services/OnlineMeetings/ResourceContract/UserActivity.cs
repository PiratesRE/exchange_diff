using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "UserActivity")]
	internal class UserActivity : Resource
	{
		public UserActivity(string selfUri) : base(selfUri)
		{
		}

		public const string Token = "userActivity";
	}
}
