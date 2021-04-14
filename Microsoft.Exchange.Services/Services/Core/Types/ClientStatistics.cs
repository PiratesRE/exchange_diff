using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	public class ClientStatistics
	{
		public string MessageId { get; set; }

		public DateTime RequestTime { get; set; }

		public int ResponseTime { get; set; }

		public int ResponseSize { get; set; }

		public int HttpResponseCode { get; set; }

		public int[] ErrorCode { get; set; }

		public ClientStatistics()
		{
			this.MessageId = null;
			this.RequestTime = DateTime.MinValue;
			this.ResponseSize = -1;
			this.ResponseTime = -1;
			this.HttpResponseCode = -1;
			this.ErrorCode = null;
		}

		public bool IsValid()
		{
			return !string.IsNullOrEmpty(this.MessageId) && this.RequestTime != DateTime.MinValue && this.ResponseTime > 0 && this.HttpResponseCode > 0;
		}
	}
}
