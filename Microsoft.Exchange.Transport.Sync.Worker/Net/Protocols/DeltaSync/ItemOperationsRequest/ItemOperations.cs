using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.ItemOperationsRequest
{
	[XmlRoot(ElementName = "ItemOperations", Namespace = "ItemOperations:", IsNullable = false)]
	[Serializable]
	public class ItemOperations
	{
		[XmlIgnore]
		public FetchType Fetch
		{
			get
			{
				if (this.internalFetch == null)
				{
					this.internalFetch = new FetchType();
				}
				return this.internalFetch;
			}
			set
			{
				this.internalFetch = value;
			}
		}

		[XmlIgnore]
		public ScanType Scan
		{
			get
			{
				if (this.internalScan == null)
				{
					this.internalScan = new ScanType();
				}
				return this.internalScan;
			}
			set
			{
				this.internalScan = value;
			}
		}

		[XmlIgnore]
		public ReportAsSpamTypeCollection ReportAsSpamCollection
		{
			get
			{
				if (this.internalReportAsSpamCollection == null)
				{
					this.internalReportAsSpamCollection = new ReportAsSpamTypeCollection();
				}
				return this.internalReportAsSpamCollection;
			}
			set
			{
				this.internalReportAsSpamCollection = value;
			}
		}

		[XmlIgnore]
		public ReportAsNotSpamTypeCollection ReportAsNotSpamCollection
		{
			get
			{
				if (this.internalReportAsNotSpamCollection == null)
				{
					this.internalReportAsNotSpamCollection = new ReportAsNotSpamTypeCollection();
				}
				return this.internalReportAsNotSpamCollection;
			}
			set
			{
				this.internalReportAsNotSpamCollection = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(FetchType), ElementName = "Fetch", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "ItemOperations:")]
		public FetchType internalFetch;

		[XmlElement(Type = typeof(ScanType), ElementName = "Scan", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "ItemOperations:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public ScanType internalScan;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(ReportAsSpamType), ElementName = "ReportAsSpam", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "ItemOperations:")]
		public ReportAsSpamTypeCollection internalReportAsSpamCollection;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(ReportAsNotSpamType), ElementName = "ReportAsNotSpam", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "ItemOperations:")]
		public ReportAsNotSpamTypeCollection internalReportAsNotSpamCollection;
	}
}
