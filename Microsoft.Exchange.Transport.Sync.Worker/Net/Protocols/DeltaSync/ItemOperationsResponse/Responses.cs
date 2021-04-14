using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.ItemOperationsResponse
{
	[XmlType(TypeName = "Responses", Namespace = "ItemOperations:")]
	[Serializable]
	public class Responses
	{
		[XmlIgnore]
		public Fetch Fetch
		{
			get
			{
				if (this.internalFetch == null)
				{
					this.internalFetch = new Fetch();
				}
				return this.internalFetch;
			}
			set
			{
				this.internalFetch = value;
			}
		}

		[XmlIgnore]
		public Scan Scan
		{
			get
			{
				if (this.internalScan == null)
				{
					this.internalScan = new Scan();
				}
				return this.internalScan;
			}
			set
			{
				this.internalScan = value;
			}
		}

		[XmlIgnore]
		public ReportAsSpamCollection ReportAsSpamCollection
		{
			get
			{
				if (this.internalReportAsSpamCollection == null)
				{
					this.internalReportAsSpamCollection = new ReportAsSpamCollection();
				}
				return this.internalReportAsSpamCollection;
			}
			set
			{
				this.internalReportAsSpamCollection = value;
			}
		}

		[XmlIgnore]
		public ReportAsNotSpamCollection ReportAsNotSpamCollection
		{
			get
			{
				if (this.internalReportAsNotSpamCollection == null)
				{
					this.internalReportAsNotSpamCollection = new ReportAsNotSpamCollection();
				}
				return this.internalReportAsNotSpamCollection;
			}
			set
			{
				this.internalReportAsNotSpamCollection = value;
			}
		}

		[XmlElement(Type = typeof(Fetch), ElementName = "Fetch", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "ItemOperations:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Fetch internalFetch;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(Scan), ElementName = "Scan", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "ItemOperations:")]
		public Scan internalScan;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(ReportAsSpam), ElementName = "ReportAsSpam", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "ItemOperations:")]
		public ReportAsSpamCollection internalReportAsSpamCollection;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(ReportAsNotSpam), ElementName = "ReportAsNotSpam", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "ItemOperations:")]
		public ReportAsNotSpamCollection internalReportAsNotSpamCollection;
	}
}
