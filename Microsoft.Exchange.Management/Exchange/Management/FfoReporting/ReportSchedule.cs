using System;
using System.Globalization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[Serializable]
	public class ReportSchedule
	{
		internal ReportSchedule()
		{
		}

		public string Action { get; internal set; }

		public ReportDeliveryStatus DeliveryStatus { get; internal set; }

		public ReportDirection Direction { get; internal set; }

		public MultiValuedProperty<Guid> DLPPolicy { get; internal set; }

		public string Domain { get; internal set; }

		public DateTime EndDate { get; internal set; }

		public MultiValuedProperty<string> EventType { get; set; }

		public CultureInfo Locale { get; internal set; }

		public MultiValuedProperty<Guid> MalwareName { get; internal set; }

		public MultiValuedProperty<string> MessageID { get; internal set; }

		public MultiValuedProperty<string> NotifyAddress { get; internal set; }

		public string OriginalClientIP { get; internal set; }

		public MultiValuedProperty<string> RecipientAddress { get; internal set; }

		public ReportRecurrence Recurrence { get; set; }

		public string ReportTitle { get; set; }

		public ScheduleReportType ReportType { get; set; }

		public MultiValuedProperty<string> SenderAddress { get; internal set; }

		public ReportSeverity Severity { get; internal set; }

		public DateTime StartDate { get; internal set; }

		public MultiValuedProperty<Guid> TransportRule { get; internal set; }
	}
}
