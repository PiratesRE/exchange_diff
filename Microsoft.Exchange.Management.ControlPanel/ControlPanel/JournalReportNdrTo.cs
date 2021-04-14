using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class JournalReportNdrTo : BaseRow
	{
		public JournalReportNdrTo(TransportConfigContainer container) : base(null, container)
		{
			this.MyContainer = container;
		}

		[DataMember]
		public string JournalingReportNdrTo
		{
			get
			{
				SmtpAddress journalingReportNdrTo = this.MyContainer.JournalingReportNdrTo;
				if (journalingReportNdrTo == SmtpAddress.NullReversePath)
				{
					return string.Empty;
				}
				return journalingReportNdrTo.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string JournalingReportNdrToOrSelectString
		{
			get
			{
				string journalingReportNdrTo = this.JournalingReportNdrTo;
				if (!string.IsNullOrEmpty(journalingReportNdrTo))
				{
					return journalingReportNdrTo;
				}
				return Strings.SelectJournalingReportNdrToText;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		private readonly TransportConfigContainer MyContainer;
	}
}
