using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal abstract class BaseQuery
	{
		public EmailAddress Email
		{
			get
			{
				return this.recipientData.EmailAddress;
			}
		}

		public ExchangePrincipal ExchangePrincipal
		{
			get
			{
				return this.recipientData.ExchangePrincipal;
			}
		}

		public RecipientData RecipientData
		{
			get
			{
				return this.recipientData;
			}
		}

		public long ExchangePrincipalLatency
		{
			get
			{
				return this.recipientData.ExchangePrincipalLatency;
			}
		}

		public long ServiceDiscoveryLatency { get; set; }

		public BaseQueryResult Result
		{
			get
			{
				return this.result;
			}
		}

		public RequestType? Type { get; set; }

		public string Target { get; set; }

		public Dictionary<string, string> LogData
		{
			get
			{
				return this.logData;
			}
		}

		protected BaseQuery(RecipientData recipientData, BaseQueryResult result)
		{
			this.recipientData = recipientData;
			this.result = result;
			this.logData = new Dictionary<string, string>();
		}

		public bool SetResultOnFirstCall(BaseQueryResult result)
		{
			BaseQueryResult baseQueryResult = Interlocked.CompareExchange<BaseQueryResult>(ref this.result, result, null);
			return baseQueryResult == null;
		}

		public void LogAutoDiscRequestDetails(string frontEndServer, string backEndServer, string redirectAddress = null)
		{
			if (string.IsNullOrEmpty(frontEndServer) && string.IsNullOrEmpty(backEndServer) && string.IsNullOrEmpty(redirectAddress))
			{
				return;
			}
			string empty = string.Empty;
			string value = string.Empty;
			this.logData.TryGetValue("AutoDInfo", out empty);
			if (string.IsNullOrEmpty(redirectAddress))
			{
				value = string.Format("{0}<FE-{1}|BE-{2}|>", empty, frontEndServer, backEndServer);
			}
			else
			{
				value = string.Format("{0}<FE-{1}|BE-{2}|Redirect-{3}|>", new object[]
				{
					empty,
					frontEndServer,
					backEndServer,
					redirectAddress
				});
			}
			this.logData["AutoDInfo"] = value;
		}

		internal void LogLatency(string key, long value)
		{
			this.logData.Add(key, value.ToString());
		}

		private RecipientData recipientData;

		private BaseQueryResult result;

		private Dictionary<string, string> logData;
	}
}
