using System;
using System.Data.Services.Common;
using Microsoft.Exchange.Management.FfoReporting.Common;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[DataServiceKey("Index")]
	[Serializable]
	public class MailTrafficSummaryReport : FfoReportObject, IPageableObject
	{
		[Redact]
		public string C1 { get; internal set; }

		[Redact]
		public string C2 { get; internal set; }

		[Redact]
		public string C3 { get; internal set; }

		[Redact]
		public string C4 { get; internal set; }

		[Redact]
		public string C5 { get; internal set; }

		[Redact]
		public string C6 { get; internal set; }

		[Redact]
		public string C7 { get; internal set; }

		[Redact]
		public string C8 { get; internal set; }

		[Redact]
		public string C9 { get; internal set; }

		[Redact]
		public string C10 { get; internal set; }

		[Redact]
		public string C11 { get; internal set; }

		[Redact]
		public string C12 { get; internal set; }

		[Redact]
		public string C13 { get; internal set; }

		[Redact]
		public string C14 { get; internal set; }

		[Redact]
		public string C15 { get; internal set; }

		[Redact]
		public string C16 { get; internal set; }

		[Redact]
		public string C17 { get; internal set; }

		[Redact]
		public string C18 { get; internal set; }

		[Redact]
		public string C19 { get; internal set; }

		[Redact]
		public string C20 { get; internal set; }

		[ODataInput("Category")]
		public string Category { get; private set; }

		[ODataInput("Domain")]
		public string Domain { get; private set; }

		[ODataInput("StartDate")]
		public DateTime StartDate { get; private set; }

		[ODataInput("EndDate")]
		public DateTime EndDate { get; private set; }

		[ODataInput("DlpPolicy")]
		public string DlpPolicy { get; private set; }

		[ODataInput("TransportRule")]
		public string TransportRule { get; private set; }

		public int Index { get; set; }
	}
}
