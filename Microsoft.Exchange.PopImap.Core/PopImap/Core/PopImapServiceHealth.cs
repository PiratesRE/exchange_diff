using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.PopImap.Core
{
	public class PopImapServiceHealth
	{
		public string ServerName { get; set; }

		public long NumberOfRequests { get; set; }

		public int NumberOfErroredRequests { get; set; }

		public List<ErrorDetail> ErrorDetails
		{
			get
			{
				return this.errorDetails;
			}
			set
			{
				this.errorDetails = value;
			}
		}

		public double OKResponseRatio { get; set; }

		public double AverageRequestTime { get; set; }

		public double AverageRpcLatency { get; set; }

		public double AverageLdapLatency { get; set; }

		private List<ErrorDetail> errorDetails = new List<ErrorDetail>();
	}
}
