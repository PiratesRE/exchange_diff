using System;
using System.Globalization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[Serializable]
	public class HistoricalSearch
	{
		internal HistoricalSearch()
		{
		}

		public Guid JobId { get; internal set; }

		public DateTime SubmitDate { get; internal set; }

		public string ReportTitle { get; set; }

		public JobStatus Status { get; internal set; }

		public int Rows { get; internal set; }

		public int FileRows { get; internal set; }

		public string ErrorCode { get; internal set; }

		public string ErrorDescription { get; internal set; }

		public string FileUrl { get; internal set; }

		public HistoricalSearchReportType ReportType { get; internal set; }

		public MultiValuedProperty<string> NotifyAddress { get; internal set; }

		public DateTime StartDate { get; internal set; }

		public DateTime EndDate { get; internal set; }

		public MessageDeliveryStatus DeliveryStatus { get; internal set; }

		public MultiValuedProperty<string> SenderAddress { get; internal set; }

		public MultiValuedProperty<string> RecipientAddress { get; internal set; }

		public string OriginalClientIP { get; internal set; }

		public MultiValuedProperty<string> MessageID { get; internal set; }

		public MultiValuedProperty<Guid> DLPPolicy { get; internal set; }

		public MultiValuedProperty<Guid> TransportRule { get; internal set; }

		public CultureInfo Locale { get; internal set; }

		public MessageDirection Direction { get; internal set; }
	}
}
