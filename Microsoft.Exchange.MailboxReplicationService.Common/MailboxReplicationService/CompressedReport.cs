using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[XmlType(TypeName = "CompressedReport")]
	[Serializable]
	public sealed class CompressedReport : XMLSerializableBase
	{
		internal CompressedReport()
		{
			this.Entries = new List<ReportEntry>();
		}

		internal CompressedReport(List<ReportEntry> entries)
		{
			this.Entries = (entries ?? new List<ReportEntry>());
		}

		[XmlIgnore]
		public List<ReportEntry> Entries { get; set; }

		[XmlElement(ElementName = "CompressedEntries")]
		public byte[] CompressedEntries
		{
			get
			{
				return CommonUtils.PackString(XMLSerializableBase.Serialize(this.Entries, false), true);
			}
			set
			{
				this.Entries = (XMLSerializableBase.Deserialize<List<ReportEntry>>(CommonUtils.UnpackString(value, true), false) ?? new List<ReportEntry>());
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ReportEntry reportEntry in this.Entries)
			{
				stringBuilder.AppendLine(reportEntry.ToString());
			}
			return stringBuilder.ToString();
		}
	}
}
