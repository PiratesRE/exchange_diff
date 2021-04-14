using System;
using System.Net;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Monitoring
{
	internal class ActiveSyncState
	{
		internal ActiveSyncState(HttpWebRequest request, CasTransactionOutcome outcome)
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

		internal CasTransactionOutcome Outcome
		{
			get
			{
				return this.outcome;
			}
		}

		public ExDateTime StartTime
		{
			get
			{
				return this.startTime;
			}
			set
			{
				this.startTime = value;
			}
		}

		private HttpWebRequest request;

		private HttpWebResponse response;

		private CasTransactionOutcome outcome;

		private ExDateTime startTime;
	}
}
