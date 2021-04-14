using System;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzure.ActiveDirectory.GraphHelper
{
	[DataContract]
	[CLSCompliant(false)]
	public class AADJWTToken
	{
		[DataMember(Name = "token_type")]
		public string TokenType { get; set; }

		[DataMember(Name = "access_token")]
		public string AccessToken { get; set; }

		[DataMember(Name = "not_before")]
		public ulong NotBefore { get; set; }

		[DataMember(Name = "expires_on")]
		public ulong ExpiresOn { get; set; }

		[DataMember(Name = "expires_in")]
		public ulong ExpiresIn { get; set; }

		public bool IsExpired
		{
			get
			{
				return this.WillExpireIn(0);
			}
		}

		public bool WillExpireIn(int minutes)
		{
			return AADJWTToken.GenerateTimeStamp(minutes) > this.ExpiresOn;
		}

		private static ulong GenerateTimeStamp(int minutes)
		{
			return Convert.ToUInt64((DateTime.UtcNow.AddMinutes((double)minutes) - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds);
		}
	}
}
