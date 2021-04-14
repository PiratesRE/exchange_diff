using System;
using System.Net;

namespace Microsoft.Exchange.Monitoring
{
	internal class OwaState
	{
		internal OwaState(HttpWebRequest request, TestOwaConnectivityOutcome outcome)
		{
			this.request = request;
			this.outcome = outcome;
		}

		internal HttpWebRequest Request
		{
			get
			{
				return this.request;
			}
		}

		internal HttpWebResponse Response
		{
			get
			{
				return this.response;
			}
			set
			{
				this.response = value;
			}
		}

		internal TestOwaConnectivityOutcome Outcome
		{
			get
			{
				return this.outcome;
			}
		}

		internal int RedirectCount { get; set; }

		private HttpWebRequest request;

		private HttpWebResponse response;

		private TestOwaConnectivityOutcome outcome;
	}
}
