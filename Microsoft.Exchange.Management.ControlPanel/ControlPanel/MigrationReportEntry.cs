using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public sealed class MigrationReportEntry
	{
		public MigrationReportEntry(string creationTime, string reportUrl, string result)
		{
			this.CreationTime = creationTime;
			this.ReportUrl = reportUrl;
			this.Result = result;
		}

		[DataMember]
		public string CreationTime { get; private set; }

		[DataMember]
		public string Result { get; private set; }

		[DataMember]
		public string ReportUrl { get; private set; }

		public override int GetHashCode()
		{
			return this.CreationTime.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || base.GetType() != obj.GetType())
			{
				return false;
			}
			MigrationReportEntry migrationReportEntry = (MigrationReportEntry)obj;
			return this == migrationReportEntry || (string.Equals(this.CreationTime, migrationReportEntry.CreationTime) && string.Equals(this.Result, migrationReportEntry.Result) && string.Equals(this.ReportUrl, migrationReportEntry.ReportUrl));
		}
	}
}
