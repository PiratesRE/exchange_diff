using System;

namespace Microsoft.Exchange.Data.ApplicationLogic.Diagnostics
{
	public class RegisterConditionResult
	{
		public RegisterConditionResult()
		{
		}

		internal RegisterConditionResult(ConditionalRegistration registration)
		{
			this.Cookie = registration.Cookie;
			this.ParsedFilter = registration.QueryFilter.ToString();
			this.MaxHits = registration.MaxHits;
			this.TimeToLive = registration.TimeToLive.ToString();
		}

		public string Cookie { get; set; }

		public string ParsedFilter { get; set; }

		public int MaxHits { get; set; }

		public string TimeToLive { get; set; }
	}
}
